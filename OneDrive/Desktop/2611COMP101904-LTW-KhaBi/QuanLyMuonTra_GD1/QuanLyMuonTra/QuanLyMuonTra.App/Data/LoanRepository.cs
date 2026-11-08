using Dapper;
using Microsoft.Data.SqlClient;
using QuanLyMuonTra.App.Models;

namespace QuanLyMuonTra.App.Data;

public static class LoanRepository
{
    sealed class StockInfo
    {
        public string DeviceName { get; set; } = "";
        public int AvailableQuantity { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// Lưu phiếu mượn + chi tiết + trừ tồn kho trong MỘT transaction.
    /// Việc trừ tồn là câu UPDATE có điều kiện (AvailableQuantity >= số mượn) nên hai người cùng mượn
    /// thiết bị tồn kho thấp thì chỉ một người thành công, người kia nhận thông báo và toàn bộ phiếu bị rollback.
    /// </summary>
    public static (int LoanSlipId, string SlipCode) Create(NewLoanSlip s, int userId) =>
        Db.InTransaction((conn, tx) =>
        {
            int borrowerOk = conn.ExecuteScalar<int>(
                "SELECT COUNT(*) FROM Borrowers WHERE BorrowerId = @BorrowerId AND IsActive = 1", new { s.BorrowerId }, tx);
            if (borrowerOk == 0)
                throw new BusinessException("Người mượn không tồn tại hoặc đã ngừng hoạt động.");

            string code = CodeRepository.Next(conn, tx, CodeRepository.LoanSlip);

            int slipId = conn.ExecuteScalar<int>(
                @"INSERT INTO LoanSlips (SlipCode, BorrowerId, CreatedBy, BorrowDate, DueDate, Status, Note)
                  OUTPUT INSERTED.LoanSlipId
                  VALUES (@code, @BorrowerId, @userId, @BorrowDate, @DueDate, 0, @Note)",
                new { code, s.BorrowerId, userId, BorrowDate = s.BorrowDate.Date, DueDate = s.DueDate.Date, s.Note }, tx);

            // Gộp trùng thiết bị và xử lý theo thứ tự DeviceId cố định để tránh deadlock giữa các phiếu.
            foreach (var group in s.Items.GroupBy(i => i.DeviceId).OrderBy(g => g.Key))
            {
                int deviceId = group.Key;
                int qty = group.Sum(i => i.Quantity);

                int rows = conn.Execute(
                    @"UPDATE Devices SET AvailableQuantity = AvailableQuantity - @qty
                      WHERE DeviceId = @deviceId AND IsActive = 1 AND AvailableQuantity >= @qty",
                    new { qty, deviceId }, tx);

                if (rows == 0)
                {
                    var info = conn.QuerySingleOrDefault<StockInfo>(
                        "SELECT DeviceName, AvailableQuantity, IsActive FROM Devices WHERE DeviceId = @deviceId",
                        new { deviceId }, tx);
                    if (info is null || !info.IsActive)
                        throw new BusinessException("Có thiết bị không còn tồn tại hoặc đã ngừng sử dụng.");
                    throw new BusinessException(
                        $"Thiết bị \"{info.DeviceName}\" chỉ còn {info.AvailableQuantity} khả dụng, không đủ để mượn {qty}.");
                }

                conn.Execute(
                    "INSERT INTO LoanSlipDetails (LoanSlipId, DeviceId, Quantity, ReturnedQuantity) VALUES (@slipId, @deviceId, @qty, 0)",
                    new { slipId, deviceId, qty }, tx);
            }

            return (slipId, code);
        });

    public static List<LoanSlipRow> Search(LoanFilter f)
    {
        string? kw = string.IsNullOrWhiteSpace(f.Keyword) ? null : f.Keyword.Trim();
        string? like = kw is null ? null : $"%{kw}%";

        using var conn = Db.Open();
        return conn.Query<LoanSlipRow>(
            @"SELECT * FROM vw_LoanSlipSummary
              WHERE (@like IS NULL OR SlipCode LIKE @like OR BorrowerName LIKE @like OR BorrowerCode LIKE @like OR Phone LIKE @like)
                AND (@borrowFrom IS NULL OR BorrowDate >= @borrowFrom)
                AND (@borrowTo   IS NULL OR BorrowDate <= @borrowTo)
                AND (@dueFrom    IS NULL OR DueDate >= @dueFrom)
                AND (@dueTo      IS NULL OR DueDate <= @dueTo)
                AND (@status     IS NULL OR DisplayStatus = @status)
                AND (@borrowerId IS NULL OR BorrowerId = @borrowerId)
              ORDER BY LoanSlipId DESC",
            new
            {
                like,
                borrowFrom = f.BorrowFrom?.Date,
                borrowTo = f.BorrowTo?.Date,
                dueFrom = f.DueFrom?.Date,
                dueTo = f.DueTo?.Date,
                status = f.Status,
                borrowerId = f.BorrowerId,
            }).ToList();
    }

    public static LoanSlipRow? GetHeader(int loanSlipId)
    {
        using var conn = Db.Open();
        return GetHeader(conn, null, loanSlipId);
    }

    public static LoanSlipRow? GetHeader(SqlConnection conn, SqlTransaction? tx, int loanSlipId) =>
        conn.QuerySingleOrDefault<LoanSlipRow>(
            "SELECT * FROM vw_LoanSlipSummary WHERE LoanSlipId = @loanSlipId", new { loanSlipId }, tx);

    public static LoanSlipRow? GetByCode(string slipCode)
    {
        using var conn = Db.Open();
        return conn.QuerySingleOrDefault<LoanSlipRow>(
            "SELECT * FROM vw_LoanSlipSummary WHERE SlipCode = @slipCode", new { slipCode });
    }

    public static List<LoanSlipDetailRow> GetDetails(int loanSlipId)
    {
        using var conn = Db.Open();
        return GetDetails(conn, loanSlipId);
    }

    static List<LoanSlipDetailRow> GetDetails(SqlConnection conn, int loanSlipId) =>
        conn.Query<LoanSlipDetailRow>(
            @"SELECT d.DetailId, d.DeviceId, dv.DeviceCode, dv.DeviceName, ISNULL(dv.Unit, N'') AS Unit,
                     d.Quantity, d.ReturnedQuantity
              FROM LoanSlipDetails d
              JOIN Devices dv ON dv.DeviceId = d.DeviceId
              WHERE d.LoanSlipId = @loanSlipId
              ORDER BY d.DetailId", new { loanSlipId }).ToList();

    /// <summary>Phiếu đầy đủ: đầu phiếu + thiết bị + lịch sử các lần trả (F11, in phiếu).</summary>
    public static LoanSlipFull? GetFull(int loanSlipId)
    {
        using var conn = Db.Open();
        var header = GetHeader(conn, null, loanSlipId);
        if (header is null) return null;
        return new LoanSlipFull
        {
            Header = header,
            Details = GetDetails(conn, loanSlipId),
            Returns = ReturnRepository.GetFullByLoan(conn, loanSlipId),
        };
    }

    /// <summary>Phiếu còn thiết bị chưa trả (dùng cho F12 chọn phiếu để trả).</summary>
    public static List<LoanSlipRow> SearchOpen(string? keyword)
    {
        string? kw = string.IsNullOrWhiteSpace(keyword) ? null : keyword.Trim();
        string? like = kw is null ? null : $"%{kw}%";
        using var conn = Db.Open();
        return conn.Query<LoanSlipRow>(
            @"SELECT * FROM vw_LoanSlipSummary
              WHERE RemainingQty > 0
                AND (@like IS NULL OR SlipCode LIKE @like OR BorrowerName LIKE @like OR BorrowerCode LIKE @like OR Phone LIKE @like)
              ORDER BY DueDate, LoanSlipId", new { like }).ToList();
    }
}
