/* =====================================================================
   05 - VIEW và STORED PROCEDURE (CREATE OR ALTER: chạy lại an toàn)
   ===================================================================== */
USE QuanLyMuonTra;
GO

/* ---------------------------------------------------------------------
   vw_LoanSlipSummary - tổng hợp phiếu mượn + trạng thái hiển thị (tính tại thời điểm truy vấn)
   DisplayStatus: 0 = Đang mượn, 1 = Trả một phần, 2 = Đã trả, 3 = Quá hạn
   (Quá hạn = còn thiết bị chưa trả và đã qua hạn trả; ưu tiên hơn "Trả một phần")
   --------------------------------------------------------------------- */
CREATE OR ALTER VIEW dbo.vw_LoanSlipSummary
AS
SELECT  s.LoanSlipId, s.SlipCode, s.BorrowerId, b.BorrowerCode, b.FullName AS BorrowerName,
        ISNULL(b.Phone, N'') AS Phone, s.CreatedBy, u.FullName AS CreatedByName,
        s.BorrowDate, s.DueDate, s.Status, ISNULL(s.Note, N'') AS Note, s.CreatedAt,
        ISNULL(t.TotalQty, 0)    AS TotalQty,
        ISNULL(t.ReturnedQty, 0) AS ReturnedQty,
        ISNULL(t.TotalQty, 0) - ISNULL(t.ReturnedQty, 0) AS RemainingQty,
        CASE WHEN ISNULL(t.TotalQty, 0) - ISNULL(t.ReturnedQty, 0) = 0 THEN 2
             WHEN s.DueDate < CAST(SYSDATETIME() AS DATE) THEN 3
             WHEN ISNULL(t.ReturnedQty, 0) > 0 THEN 1
             ELSE 0 END AS DisplayStatus,
        CASE WHEN ISNULL(t.TotalQty, 0) - ISNULL(t.ReturnedQty, 0) > 0
              AND s.DueDate < CAST(SYSDATETIME() AS DATE)
             THEN DATEDIFF(DAY, s.DueDate, CAST(SYSDATETIME() AS DATE)) ELSE 0 END AS DaysOverdue
FROM    dbo.LoanSlips s
JOIN    dbo.Borrowers b ON b.BorrowerId = s.BorrowerId
JOIN    dbo.Users u     ON u.UserId = s.CreatedBy
OUTER APPLY (SELECT SUM(d.Quantity) AS TotalQty, SUM(d.ReturnedQuantity) AS ReturnedQty
             FROM dbo.LoanSlipDetails d WHERE d.LoanSlipId = s.LoanSlipId) t;
GO

/* ---------------------------------------------------------------------
   sp_GetNextCode - sinh mã tự động theo CodeConfigs.
   Khóa dòng cấu hình (UPDLOCK, HOLDLOCK) nên nhiều người cùng thao tác không bao giờ trùng mã.
   Gọi trong cùng transaction với việc lưu phiếu: nếu rollback thì số cũng được hoàn lại.
   @Peek = 1: chỉ xem mã kế tiếp, không tăng bộ đếm (dùng để gợi ý trên giao diện).
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_GetNextCode
    @EntityName VARCHAR(30),
    @Code       VARCHAR(50) OUTPUT,
    @Peek       BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @today DATE = CAST(SYSDATETIME() AS DATE);
    DECLARE @prefix VARCHAR(10), @useDate BIT, @len INT, @num INT, @reset BIT, @last DATE;

    SELECT @prefix = Prefix, @useDate = UseDate, @len = NumberLength,
           @num = CurrentNumber, @reset = ResetDaily, @last = LastResetDate
    FROM   dbo.CodeConfigs WITH (UPDLOCK, HOLDLOCK)
    WHERE  EntityName = @EntityName;

    IF @prefix IS NULL
        THROW 50001, N'Chưa cấu hình mã tự động cho đối tượng này.', 1;

    IF @reset = 1 AND (@last IS NULL OR @last <> @today) SET @num = 0;
    SET @num = @num + 1;

    IF @Peek = 0
        UPDATE dbo.CodeConfigs SET CurrentNumber = @num, LastResetDate = @today WHERE EntityName = @EntityName;

    DECLARE @numText VARCHAR(20) = CAST(@num AS VARCHAR(20));
    IF LEN(@numText) < @len SET @numText = RIGHT(REPLICATE('0', @len) + @numText, @len);

    SET @Code = @prefix
              + CASE WHEN @useDate = 1 THEN '-' + CONVERT(VARCHAR(8), @today, 112) + '-' ELSE '' END
              + @numText;
END
GO

/* ---------------------------------------------------------------------
   sp_GetOverdueSlips (F14) - danh sách phiếu quá hạn, tính theo ngày hiện tại và số lượng chưa trả thực tế.
   @Keyword: tìm theo mã phiếu, tên người mượn, số điện thoại.
   @MinDaysOverdue: chỉ lấy phiếu quá hạn từ N ngày trở lên.
   @AsOfDate: mặc định là hôm nay.
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_GetOverdueSlips
    @Keyword        NVARCHAR(100) = NULL,
    @MinDaysOverdue INT           = NULL,
    @AsOfDate       DATE          = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @asOf DATE = ISNULL(@AsOfDate, CAST(SYSDATETIME() AS DATE));
    DECLARE @kw NVARCHAR(100) = NULLIF(LTRIM(RTRIM(@Keyword)), N'');
    DECLARE @like NVARCHAR(110) = CASE WHEN @kw IS NULL THEN NULL ELSE N'%' + @kw + N'%' END;

    SELECT  s.LoanSlipId, s.SlipCode, b.BorrowerId, b.FullName AS BorrowerName, ISNULL(b.Phone, N'') AS Phone,
            s.BorrowDate, s.DueDate,
            DATEDIFF(DAY, s.DueDate, @asOf) AS DaysOverdue,
            x.UnreturnedQty, x.UnreturnedDevices
    FROM    dbo.LoanSlips s
    JOIN    dbo.Borrowers b ON b.BorrowerId = s.BorrowerId
    CROSS APPLY (
        SELECT  SUM(d.Quantity - d.ReturnedQuantity) AS UnreturnedQty,
                STRING_AGG(CONVERT(NVARCHAR(MAX), dv.DeviceName + N' (x' + CAST(d.Quantity - d.ReturnedQuantity AS NVARCHAR(10)) + N')'), N', ')
                    WITHIN GROUP (ORDER BY dv.DeviceName) AS UnreturnedDevices
        FROM    dbo.LoanSlipDetails d
        JOIN    dbo.Devices dv ON dv.DeviceId = d.DeviceId
        WHERE   d.LoanSlipId = s.LoanSlipId AND d.ReturnedQuantity < d.Quantity
    ) x
    WHERE   s.DueDate < @asOf
        AND ISNULL(x.UnreturnedQty, 0) > 0
        AND (@MinDaysOverdue IS NULL OR DATEDIFF(DAY, s.DueDate, @asOf) >= @MinDaysOverdue)
        AND (@like IS NULL OR s.SlipCode LIKE @like OR b.FullName LIKE @like OR b.Phone LIKE @like)
    ORDER BY DATEDIFF(DAY, s.DueDate, @asOf) DESC, s.SlipCode;
END
GO

/* ---------------------------------------------------------------------
   sp_RecalculateInventory - đồng bộ lại trạng thái phiếu và tồn kho khả dụng:
   Available = Total - (đang mượn) - Damaged - Lost  (không âm).
   Dùng sau khi nạp dữ liệu mẫu hoặc khi cần sửa sai lệch. An toàn khi chạy nhiều lần.
   --------------------------------------------------------------------- */
CREATE OR ALTER PROCEDURE dbo.sp_RecalculateInventory
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE s
    SET    Status = CASE WHEN EXISTS (SELECT 1 FROM dbo.LoanSlipDetails d WHERE d.LoanSlipId = s.LoanSlipId)
                          AND NOT EXISTS (SELECT 1 FROM dbo.LoanSlipDetails d
                                          WHERE d.LoanSlipId = s.LoanSlipId AND d.ReturnedQuantity < d.Quantity)
                         THEN 1 ELSE 0 END
    FROM   dbo.LoanSlips s;

    ;WITH onloan AS (
        SELECT DeviceId, SUM(Quantity - ReturnedQuantity) AS Qty
        FROM   dbo.LoanSlipDetails GROUP BY DeviceId
    )
    UPDATE dv
    SET    AvailableQuantity =
           CASE WHEN dv.TotalQuantity - ISNULL(o.Qty, 0) - dv.DamagedQuantity - dv.LostQuantity < 0 THEN 0
                ELSE dv.TotalQuantity - ISNULL(o.Qty, 0) - dv.DamagedQuantity - dv.LostQuantity END
    FROM   dbo.Devices dv
    LEFT JOIN onloan o ON o.DeviceId = dv.DeviceId;
END
GO
