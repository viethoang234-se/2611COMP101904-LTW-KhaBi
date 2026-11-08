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

    public static List<DeviceRow> Search(string? keyword, int? categoryId)
    {
        string? kw = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();
        string? like = kw is null ? null : $"%{kw}%";

        using var conn = Db.Open();
        return conn.Query<DeviceRow>(
            @"SELECT d.DeviceId, d.DeviceCode, d.DeviceName, d.CategoryId, c.CategoryName,
                     d.TotalQuantity, d.AvailableQuantity, ISNULL(d.Unit, N'') AS Unit, ISNULL(d.Note, N'') AS Note
              FROM Devices d
              JOIN DeviceCategories c ON c.CategoryId = d.CategoryId
              WHERE d.IsActive = 1
                AND (@like IS NULL OR d.DeviceCode LIKE @like OR d.DeviceName LIKE @like)
                AND (@categoryId IS NULL OR d.CategoryId = @categoryId)
              ORDER BY d.DeviceName",
            new { like, categoryId }).ToList();
    }

    public static string SuggestCode()
    {
        using var conn = Db.Open();
        int next = conn.ExecuteScalar<int>("SELECT ISNULL(MAX(DeviceId), 0) + 1 FROM Devices");
        return $"TB{next:0000}";
    }

    /// <summary>Thêm thiết bị. Trả về thông báo lỗi, hoặc null nếu thành công.</summary>
    public static string? Insert(DeviceRow d)
    {
        try
        {
            using var conn = Db.Open();
            conn.Execute(
                @"INSERT INTO Devices (DeviceCode, DeviceName, CategoryId, TotalQuantity, AvailableQuantity, Unit, Note)
                  VALUES (@DeviceCode, @DeviceName, @CategoryId, @TotalQuantity, @TotalQuantity, @Unit, @Note)", d);
            return null;
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return "Mã thiết bị đã tồn tại.";
        }
    }

    /// <summary>Sửa thiết bị. Số lượng còn lại được điều chỉnh theo chênh lệch tổng số lượng.</summary>
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
            return rows == 0 ? "Tổng số lượng không được nhỏ hơn số lượng đang được mượn." : null;
        }
        catch (SqlException ex) when (ex.Number is 2601 or 2627)
        {
            return "Mã thiết bị đã tồn tại.";
        }
    }

    /// <summary>Xóa mềm thiết bị. Không cho xóa nếu đang nằm trong phiếu mượn chưa trả.</summary>
    public static string? Delete(int deviceId)
    {
        using var conn = Db.Open();
        bool inUse = conn.ExecuteScalar<int>(
            @"SELECT COUNT(*)
              FROM LoanSlipDetails d
              JOIN LoanSlips s ON s.LoanSlipId = d.LoanSlipId
              WHERE d.DeviceId = @deviceId AND s.Status = 0", new { deviceId }) > 0;
        if (inUse) return "Thiết bị đang nằm trong phiếu mượn chưa trả, không thể xóa.";

        conn.Execute("UPDATE Devices SET IsActive = 0 WHERE DeviceId = @deviceId", new { deviceId });
        return null;
    }
}
