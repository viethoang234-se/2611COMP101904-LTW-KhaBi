using Dapper;
using Microsoft.Data.SqlClient;
using QuanLyMuonTra.App.Models;

namespace QuanLyMuonTra.App.Data;

public static class DeviceRepository
{
    public static List<CategoryItem> GetCategories()
    {
        using var conn = Db.Open();
        return conn.Query<CategoryItem>(
            "SELECT CategoryId, CategoryName FROM DeviceCategories WHERE IsActive = 1 ORDER BY CategoryName").ToList();
    }

    const string SelectSql =
        @"SELECT d.DeviceId, d.DeviceCode, d.DeviceName, d.CategoryId, c.CategoryName,
                 d.TotalQuantity, d.AvailableQuantity, ISNULL(d.Unit, N'') AS Unit, ISNULL(d.Note, N'') AS Note,
                 d.DamagedQuantity, d.LostQuantity,
                 ISNULL((SELECT SUM(ld.Quantity - ld.ReturnedQuantity) FROM LoanSlipDetails ld WHERE ld.DeviceId = d.DeviceId), 0) AS OnLoanQuantity
          FROM Devices d
          JOIN DeviceCategories c ON c.CategoryId = d.CategoryId ";

    public static List<DeviceRow> Search(string? keyword, int? categoryId, DeviceStockFilter stock = DeviceStockFilter.All)
    {
        string? kw = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();
        string? like = kw is null ? null : $"%{kw}%";

        using var conn = Db.Open();
        var rows = conn.Query<DeviceRow>(
            SelectSql +
            @"WHERE d.IsActive = 1
                AND (@like IS NULL OR d.DeviceCode LIKE @like OR d.DeviceName LIKE @like)
                AND (@categoryId IS NULL OR d.CategoryId = @categoryId)
              ORDER BY d.DeviceName",
            new { like, categoryId }).ToList();

        return stock switch
        {
            DeviceStockFilter.InStock => rows.Where(r => r.AvailableQuantity > 0).ToList(),
            DeviceStockFilter.OutOfStock => rows.Where(r => r.AvailableQuantity == 0).ToList(),
            DeviceStockFilter.OnLoan => rows.Where(r => r.OnLoanQuantity > 0).ToList(),
            DeviceStockFilter.Damaged => rows.Where(r => r.DamagedQuantity + r.LostQuantity > 0).ToList(),
            _ => rows,
        };
    }

    /// <summary>Thiết bị còn khả dụng để chọn khi lập phiếu mượn (F09).</summary>
    public static List<DeviceRow> SearchAvailable(string? keyword) =>
        Search(keyword, null, DeviceStockFilter.InStock);

    public static DeviceRow? Get(int deviceId)
    {
        using var conn = Db.Open();
        return conn.QuerySingleOrDefault<DeviceRow>(SelectSql + "WHERE d.DeviceId = @deviceId", new { deviceId });
    }

    /// <summary>Mã thiết bị tiếp theo theo cấu hình mã tự động (chỉ gợi ý, không tăng bộ đếm).</summary>
    public static string SuggestCode() => CodeRepository.Peek(CodeRepository.Device);

    /// <summary>Thêm thiết bị. Trả về thông báo lỗi, hoặc null nếu thành công.</summary>
    public static string? Insert(DeviceRow d)
    {
        try
        {
            Db.InTransaction((conn, tx) =>
            {
                conn.Execute(
                    @"INSERT INTO Devices (DeviceCode, DeviceName, CategoryId, TotalQuantity, AvailableQuantity, Unit, Note)
                      VALUES (@DeviceCode, @DeviceName, @CategoryId, @TotalQuantity, @TotalQuantity, @Unit, @Note)", d, tx);
                CodeRepository.SyncCounter(conn, tx, CodeRepository.Device, d.DeviceCode);
                return 0;
            });
            return null;
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return "Mã thiết bị đã tồn tại.";
        }
    }

    /// <summary>
    /// Sửa thiết bị. Số lượng khả dụng được điều chỉnh theo chênh lệch tổng số lượng; không cho giảm tổng
    /// xuống thấp hơn (đang mượn + hư hỏng + mất).
    /// </summary>
    public static string? Update(DeviceRow d)
    {
        try
        {
            using var conn = Db.Open();
            int rows = conn.Execute(
                @"UPDATE Devices
                  SET DeviceCode = @DeviceCode,
                      DeviceName = @DeviceName,
                      CategoryId = @CategoryId,
                      AvailableQuantity = AvailableQuantity + (@TotalQuantity - TotalQuantity),
                      TotalQuantity = @TotalQuantity,
                      Unit = @Unit,
                      Note = @Note
                  WHERE DeviceId = @DeviceId
                    AND AvailableQuantity + (@TotalQuantity - TotalQuantity) >= 0", d);
            return rows == 0
                ? "Tổng số lượng không được nhỏ hơn số đang được mượn cộng số hư hỏng/mất."
                : null;
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return "Mã thiết bị đã tồn tại.";
        }
    }

    /// <summary>Xóa mềm thiết bị. Không cho xóa nếu còn số lượng đang được mượn.</summary>
    public static string? Delete(int deviceId)
    {
        using var conn = Db.Open();
        bool inUse = conn.ExecuteScalar<int>(
            @"SELECT COUNT(*)
              FROM LoanSlipDetails d
              WHERE d.DeviceId = @deviceId AND d.ReturnedQuantity < d.Quantity", new { deviceId }) > 0;
        if (inUse) return "Thiết bị đang nằm trong phiếu mượn chưa trả, không thể xóa.";

        conn.Execute("UPDATE Devices SET IsActive = 0 WHERE DeviceId = @deviceId", new { deviceId });
        return null;
    }

    /// <summary>Chuyển số thiết bị hư hỏng đã sửa xong về khả dụng. Trả về false nếu số hư hỏng hiện có không đủ.</summary>
    public static bool RecoverDamaged(int deviceId, int quantity)
    {
        using var conn = Db.Open();
        return conn.Execute(
            @"UPDATE Devices SET DamagedQuantity = DamagedQuantity - @quantity, AvailableQuantity = AvailableQuantity + @quantity
              WHERE DeviceId = @deviceId AND DamagedQuantity >= @quantity", new { deviceId, quantity }) > 0;
    }
}
