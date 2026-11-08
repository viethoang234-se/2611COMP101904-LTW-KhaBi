using Dapper;
using Microsoft.Data.SqlClient;
using QuanLyMuonTra.App.Models;

namespace QuanLyMuonTra.App.Data;

public static class ReturnRepository
{
    sealed class SlipLock
    {
        public int LoanSlipId { get; set; }
        public DateTime BorrowDate { get; set; }
    }

    sealed class DetailLock
    {
        public int DetailId { get; set; }
        public int DeviceId { get; set; }
        public int Quantity { get; set; }
        public int ReturnedQuantity { get; set; }
    }

    sealed class RecordDetailLock
    {
        public int Id { get; set; }
        public int DeviceId { get; set; }
        public int? LoanSlipDetailId { get; set; }
        public int Quantity { get; set; }
        public int ConditionCode { get; set; }
    }

    /// <summary>Nhóm tồn kho mà một số lượng trả sẽ đi vào: (khả dụng, hư hỏng, mất).</summary>
    static (int Good, int Damaged, int Lost) Bucket(int conditionCode, int qty) => (ReturnCondition)conditionCode switch
    {
        ReturnCondition.Good => (qty, 0, 0),
        ReturnCondition.Lost => (0, 0, qty),
        _ => (0, qty, 0), // Hư hỏng, Thiếu phụ kiện
    };

    const string RecomputeStatusSql =
        @"UPDATE LoanSlips
          SET Status = CASE WHEN EXISTS (SELECT 1 FROM LoanSlipDetails WHERE LoanSlipId = @loanSlipId AND ReturnedQuantity < Quantity)
                            THEN 0 ELSE 1 END
          WHERE LoanSlipId = @loanSlipId";

    static string DeviceName(SqlConnection conn, SqlTransaction tx, int deviceId) =>
        conn.ExecuteScalar<string?>("SELECT DeviceName FROM Devices WHERE DeviceId = @deviceId", new { deviceId }, tx) ?? "?";

    /// <summary>
    /// Ghi nhận trả trong MỘT transaction: khóa phiếu -> kiểm tra từng dòng thuộc phiếu và không vượt số còn phải trả ->
    /// lưu biên bản -> cập nhật ReturnedQuantity -> cập nhật tồn kho theo tình trạng -> cập nhật trạng thái phiếu.
    /// </summary>
    public static (int ReturnRecordId, string RecordCode) Create(NewReturnRecord r, int userId) =>
        Db.InTransaction((conn, tx) =>
        {
            var slip = conn.QuerySingleOrDefault<SlipLock>(
                "SELECT LoanSlipId, BorrowDate FROM LoanSlips WITH (UPDLOCK, HOLDLOCK) WHERE LoanSlipId = @LoanSlipId",
                new { r.LoanSlipId }, tx);
            if (slip is null) throw new BusinessException("Phiếu mượn không tồn tại.");

            var details = conn.Query<DetailLock>(
                @"SELECT DetailId, DeviceId, Quantity, ReturnedQuantity
                  FROM LoanSlipDetails WITH (UPDLOCK) WHERE LoanSlipId = @LoanSlipId",
                new { r.LoanSlipId }, tx).ToDictionary(d => d.DetailId);

            if (details.Values.All(d => d.ReturnedQuantity >= d.Quantity))
                throw new BusinessException("Phiếu mượn này đã trả đủ, không còn thiết bị cần trả.");
            if (r.ReturnDate.Date < slip.BorrowDate.Date)
                throw new BusinessException("Ngày trả không được trước ngày mượn.");

            foreach (var g in r.Items.GroupBy(i => i.LoanSlipDetailId))
            {
                if (!details.TryGetValue(g.Key, out var d))
                    throw new BusinessException("Có thiết bị không thuộc phiếu mượn này.");
                int remaining = d.Quantity - d.ReturnedQuantity;
                int sum = g.Sum(i => i.Quantity);
                if (sum > remaining)
                    throw new BusinessException(
                        $"Thiết bị \"{DeviceName(conn, tx, d.DeviceId)}\": số lượng trả ({sum}) vượt quá số còn phải trả ({remaining}).");
            }

            string code = CodeRepository.Next(conn, tx, CodeRepository.ReturnRecord);

            int recordId = conn.ExecuteScalar<int>(
                @"INSERT INTO ReturnRecords (RecordCode, LoanSlipId, ReturnDate, ReceivedBy, GiverName, Note)
                  OUTPUT INSERTED.ReturnRecordId
                  VALUES (@code, @LoanSlipId, @ReturnDate, @userId, @GiverName, @Note)",
                new { code, r.LoanSlipId, r.ReturnDate, userId, r.GiverName, r.Note }, tx);

            // Dòng chi tiết biên bản (mỗi tình trạng một dòng)
            foreach (var item in r.Items)
            {
                var d = details[item.LoanSlipDetailId];
                conn.Execute(
                    @"INSERT INTO ReturnRecordDetails (ReturnRecordId, DeviceId, LoanSlipDetailId, Quantity, ConditionCode, Condition)
                      VALUES (@recordId, @DeviceId, @DetailId, @Quantity, @code, @note)",
                    new { recordId, d.DeviceId, d.DetailId, item.Quantity, code = (int)item.Condition, note = item.ConditionNote }, tx);
            }

            // Cập nhật số lượng đã trả của từng dòng mượn
            foreach (var g in r.Items.GroupBy(i => i.LoanSlipDetailId).OrderBy(g => g.Key))
            {
                int qty = g.Sum(i => i.Quantity);
                int rows = conn.Execute(
                    @"UPDATE LoanSlipDetails SET ReturnedQuantity = ReturnedQuantity + @qty
                      WHERE DetailId = @id AND ReturnedQuantity + @qty <= Quantity",
                    new { qty, id = g.Key }, tx);
                if (rows == 0) throw new BusinessException("Số lượng trả không hợp lệ (dữ liệu vừa thay đổi). Vui lòng tải lại phiếu.");
            }

            // Cập nhật tồn kho theo tình trạng, theo thứ tự DeviceId cố định
            var perDevice = r.Items
                .Select(i => (DeviceId: details[i.LoanSlipDetailId].DeviceId, b: Bucket((int)i.Condition, i.Quantity)))
                .GroupBy(x => x.DeviceId).OrderBy(g => g.Key);
            foreach (var g in perDevice)
            {
                int good = g.Sum(x => x.b.Good), damaged = g.Sum(x => x.b.Damaged), lost = g.Sum(x => x.b.Lost);
                int rows = conn.Execute(
                    @"UPDATE Devices
                      SET AvailableQuantity = AvailableQuantity + @good,
                          DamagedQuantity   = DamagedQuantity + @damaged,
                          LostQuantity      = LostQuantity + @lost
                      WHERE DeviceId = @deviceId",
                    new { good, damaged, lost, deviceId = g.Key }, tx);
                if (rows == 0) throw new BusinessException("Thiết bị trong phiếu không còn tồn tại.");
            }

            conn.Execute(RecomputeStatusSql, new { r.LoanSlipId }, tx);
            return (recordId, code);
        });

    /// <summary>
    /// Hủy biên bản trả (không xóa dữ liệu): hoàn tác số lượng đã trả và tồn kho đúng nhóm đã cộng lúc trả.
    /// Nếu tồn kho hiện tại không đủ để hoàn tác (thiết bị đã được cho mượn lại hoặc đã sửa xong) thì từ chối.
    /// </summary>
    public static void Cancel(int returnRecordId, int userId, string reason) =>
        Db.InTransaction((conn, tx) =>
        {
            int? slipId = conn.QuerySingleOrDefault<int?>(
                "SELECT LoanSlipId FROM ReturnRecords WHERE ReturnRecordId = @returnRecordId", new { returnRecordId }, tx);
            if (slipId is null) throw new BusinessException("Biên bản không tồn tại.");

            // Khóa phiếu trước (cùng thứ tự khóa với Create) rồi mới đọc lại trạng thái biên bản
            conn.ExecuteScalar<int?>(
                "SELECT LoanSlipId FROM LoanSlips WITH (UPDLOCK, HOLDLOCK) WHERE LoanSlipId = @slipId", new { slipId }, tx);

            bool cancelled = conn.ExecuteScalar<bool>(
                "SELECT IsCancelled FROM ReturnRecords WHERE ReturnRecordId = @returnRecordId", new { returnRecordId }, tx);
            if (cancelled) throw new BusinessException("Biên bản này đã được hủy trước đó.");

            var lines = conn.Query<RecordDetailLock>(
                @"SELECT Id, DeviceId, LoanSlipDetailId, Quantity, ConditionCode
                  FROM ReturnRecordDetails WHERE ReturnRecordId = @returnRecordId",
                new { returnRecordId }, tx).ToList();

            foreach (var line in lines)
            {
                int? detailId = line.LoanSlipDetailId ?? conn.QuerySingleOrDefault<int?>(
                    @"SELECT MIN(DetailId) FROM LoanSlipDetails
                      WHERE LoanSlipId = @slipId AND DeviceId = @DeviceId", new { slipId, line.DeviceId }, tx);
                if (detailId is null)
                    throw new BusinessException("Không xác định được dòng phiếu mượn tương ứng, không thể hủy biên bản.");

                int rows = conn.Execute(
                    @"UPDATE LoanSlipDetails SET ReturnedQuantity = ReturnedQuantity - @qty
                      WHERE DetailId = @detailId AND ReturnedQuantity >= @qty",
                    new { qty = line.Quantity, detailId }, tx);
                if (rows == 0) throw new BusinessException("Số lượng đã trả của phiếu không khớp biên bản, không thể hủy.");
            }

            var perDevice = lines
                .Select(l => (l.DeviceId, b: Bucket(l.ConditionCode, l.Quantity)))
                .GroupBy(x => x.DeviceId).OrderBy(g => g.Key);
            foreach (var g in perDevice)
            {
                int good = g.Sum(x => x.b.Good), damaged = g.Sum(x => x.b.Damaged), lost = g.Sum(x => x.b.Lost);
                int rows = conn.Execute(
                    @"UPDATE Devices
                      SET AvailableQuantity = AvailableQuantity - @good,
                          DamagedQuantity   = DamagedQuantity - @damaged,
                          LostQuantity      = LostQuantity - @lost
                      WHERE DeviceId = @deviceId
                        AND AvailableQuantity >= @good AND DamagedQuantity >= @damaged AND LostQuantity >= @lost",
                    new { good, damaged, lost, deviceId = g.Key }, tx);
                if (rows == 0)
                    throw new BusinessException(
                        $"Không thể hủy: thiết bị \"{DeviceName(conn, tx, g.Key)}\" đã được cho mượn lại hoặc đã xử lý (sửa chữa) " +
                        "sau lần trả này, tồn kho hiện tại không đủ để hoàn tác.");
            }

            int updated = conn.Execute(
                @"UPDATE ReturnRecords
                  SET IsCancelled = 1, CancelledBy = @userId, CancelledAt = SYSDATETIME(), CancelReason = @reason
                  WHERE ReturnRecordId = @returnRecordId AND IsCancelled = 0",
                new { userId, reason, returnRecordId }, tx);
            if (updated == 0) throw new BusinessException("Biên bản này đã được hủy trước đó.");

            conn.Execute(RecomputeStatusSql, new { loanSlipId = slipId }, tx);
            return 0;
        });

    const string RecordSelect =
        @"SELECT r.ReturnRecordId, r.RecordCode, r.LoanSlipId, s.SlipCode, b.FullName AS BorrowerName,
                 r.ReturnDate, r.ReceivedBy, u.FullName AS ReceivedByName,
                 ISNULL(r.GiverName, N'') AS GiverName, ISNULL(r.Note, N'') AS Note,
                 r.IsCancelled, ISNULL(cu.FullName, N'') AS CancelledByName, r.CancelledAt,
                 ISNULL(r.CancelReason, N'') AS CancelReason,
                 ISNULL((SELECT SUM(d.Quantity) FROM ReturnRecordDetails d WHERE d.ReturnRecordId = r.ReturnRecordId), 0) AS TotalQuantity
          FROM ReturnRecords r
          JOIN LoanSlips s ON s.LoanSlipId = r.LoanSlipId
          JOIN Borrowers b ON b.BorrowerId = s.BorrowerId
          JOIN Users u ON u.UserId = r.ReceivedBy
          LEFT JOIN Users cu ON cu.UserId = r.CancelledBy ";

    public static List<ReturnRecordRow> Search(RecordFilter f)
    {
        string? kw = string.IsNullOrWhiteSpace(f.Keyword) ? null : f.Keyword.Trim();
        string? like = kw is null ? null : $"%{kw}%";

        using var conn = Db.Open();
        return conn.Query<ReturnRecordRow>(
            RecordSelect +
            @"WHERE (@like IS NULL OR r.RecordCode LIKE @like OR s.SlipCode LIKE @like OR b.FullName LIKE @like)
                AND (@fromDate IS NULL OR r.ReturnDate >= @fromDate)
                AND (@toExclusive IS NULL OR r.ReturnDate < @toExclusive)
                AND (@cancelled IS NULL OR r.IsCancelled = @cancelled)
                AND (@loanSlipId IS NULL OR r.LoanSlipId = @loanSlipId)
              ORDER BY r.ReturnRecordId DESC",
            new
            {
                like,
                fromDate = f.From?.Date,
                toExclusive = f.To?.Date.AddDays(1),
                cancelled = f.Cancelled,
                loanSlipId = f.LoanSlipId,
            }).ToList();
    }

    public static ReturnRecordFull? GetFull(int returnRecordId)
    {
        using var conn = Db.Open();
        var header = conn.QuerySingleOrDefault<ReturnRecordRow>(
            RecordSelect + "WHERE r.ReturnRecordId = @returnRecordId", new { returnRecordId });
        if (header is null) return null;
        return new ReturnRecordFull { Header = header, Details = GetDetails(conn, new[] { returnRecordId }) };
    }

    public static List<ReturnRecordFull> GetFullByLoan(SqlConnection conn, int loanSlipId)
    {
        var headers = conn.Query<ReturnRecordRow>(
            RecordSelect + "WHERE r.LoanSlipId = @loanSlipId ORDER BY r.ReturnDate, r.ReturnRecordId", new { loanSlipId }).ToList();
        if (headers.Count == 0) return new List<ReturnRecordFull>();

        var details = GetDetails(conn, headers.Select(h => h.ReturnRecordId).ToArray());
        return headers.Select(h => new ReturnRecordFull
        {
            Header = h,
            Details = details.Where(d => d.ReturnRecordId == h.ReturnRecordId).ToList(),
        }).ToList();
    }

    static List<ReturnRecordDetailRow> GetDetails(SqlConnection conn, int[] recordIds) =>
        conn.Query<ReturnRecordDetailRow>(
            @"SELECT d.Id, d.ReturnRecordId, d.DeviceId, dv.DeviceCode, dv.DeviceName, ISNULL(dv.Unit, N'') AS Unit,
                     d.Quantity, d.ConditionCode, ISNULL(d.Condition, N'') AS Condition
              FROM ReturnRecordDetails d
              JOIN Devices dv ON dv.DeviceId = d.DeviceId
              WHERE d.ReturnRecordId IN @recordIds
              ORDER BY d.ReturnRecordId, d.Id", new { recordIds }).ToList();
}
