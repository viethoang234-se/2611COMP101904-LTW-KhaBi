/* =====================================================================
   07 - DỮ LIỆU THỬ (TÙY CHỌN) - tạo các tình huống mượn/trả để kiểm tra giao diện và báo cáo
     PM-0001  Quá hạn, chưa trả gì
     PM-0002  Đang mượn trong hạn
     PM-0003  Đã trả đủ (có biên bản BB-0001)
     PM-0004  Trả một phần (BB-0002)
     PM-0005  Quá hạn + trả một phần, có 1 thiết bị hư hỏng (BB-0003)
   Mỗi phiếu chỉ được nạp khi chưa tồn tại. Cuối script tồn kho được tính lại cho khớp.
   Chạy sau 06. Không nên chạy trên CSDL đang dùng thật.
   ===================================================================== */
USE QuanLyMuonTra;
GO

DECLARE @admin INT = (SELECT UserId FROM dbo.Users WHERE Username = 'admin');
DECLARE @kho   INT = (SELECT UserId FROM dbo.Users WHERE Username = 'kho01');
DECLARE @today DATE = CAST(SYSDATETIME() AS DATE);

IF @admin IS NULL THROW 50010, N'Chưa có tài khoản admin. Hãy chạy 06_SeedData.sql trước.', 1;

/* ----- PM-0001 ----- */
IF NOT EXISTS (SELECT 1 FROM dbo.LoanSlips WHERE SlipCode = 'PM-0001')
BEGIN
    INSERT INTO dbo.LoanSlips (SlipCode, BorrowerId, CreatedBy, BorrowDate, DueDate, Status)
    SELECT 'PM-0001', BorrowerId, @admin, DATEADD(DAY, -10, @today), DATEADD(DAY, -3, @today), 0 FROM dbo.Borrowers WHERE BorrowerCode = 'NM0001';
    INSERT INTO dbo.LoanSlipDetails (LoanSlipId, DeviceId, Quantity, ReturnedQuantity)
    SELECT s.LoanSlipId, d.DeviceId, v.Qty, 0
    FROM (VALUES ('TB0001', 2), ('TB0004', 1)) v(Dev, Qty)
    JOIN dbo.LoanSlips s ON s.SlipCode = 'PM-0001' JOIN dbo.Devices d ON d.DeviceCode = v.Dev;
END

/* ----- PM-0002 ----- */
IF NOT EXISTS (SELECT 1 FROM dbo.LoanSlips WHERE SlipCode = 'PM-0002')
BEGIN
    INSERT INTO dbo.LoanSlips (SlipCode, BorrowerId, CreatedBy, BorrowDate, DueDate, Status)
    SELECT 'PM-0002', BorrowerId, @admin, DATEADD(DAY, -2, @today), DATEADD(DAY, 5, @today), 0 FROM dbo.Borrowers WHERE BorrowerCode = 'NM0002';
    INSERT INTO dbo.LoanSlipDetails (LoanSlipId, DeviceId, Quantity, ReturnedQuantity)
    SELECT s.LoanSlipId, d.DeviceId, v.Qty, 0
    FROM (VALUES ('TB0003', 1), ('TB0006', 1)) v(Dev, Qty)
    JOIN dbo.LoanSlips s ON s.SlipCode = 'PM-0002' JOIN dbo.Devices d ON d.DeviceCode = v.Dev;
END

/* ----- PM-0003 (đã trả đủ) + BB-0001 ----- */
IF NOT EXISTS (SELECT 1 FROM dbo.LoanSlips WHERE SlipCode = 'PM-0003')
BEGIN
    INSERT INTO dbo.LoanSlips (SlipCode, BorrowerId, CreatedBy, BorrowDate, DueDate, Status)
    SELECT 'PM-0003', BorrowerId, @admin, DATEADD(DAY, -14, @today), DATEADD(DAY, -7, @today), 1 FROM dbo.Borrowers WHERE BorrowerCode = 'NM0003';
    INSERT INTO dbo.LoanSlipDetails (LoanSlipId, DeviceId, Quantity, ReturnedQuantity)
    SELECT s.LoanSlipId, d.DeviceId, 1, 1
    FROM dbo.LoanSlips s JOIN dbo.Devices d ON d.DeviceCode = 'TB0008' WHERE s.SlipCode = 'PM-0003';
END
IF NOT EXISTS (SELECT 1 FROM dbo.ReturnRecords r JOIN dbo.LoanSlips s ON s.LoanSlipId = r.LoanSlipId WHERE s.SlipCode = 'PM-0003')
BEGIN
    INSERT INTO dbo.ReturnRecords (RecordCode, LoanSlipId, ReturnDate, ReceivedBy, GiverName, Note)
    SELECT 'BB-0001', LoanSlipId, DATEADD(DAY, -8, SYSDATETIME()), @admin, N'Hoàng Minh Châu', N'Trả đủ' FROM dbo.LoanSlips WHERE SlipCode = 'PM-0003';
    INSERT INTO dbo.ReturnRecordDetails (ReturnRecordId, DeviceId, LoanSlipDetailId, Quantity, ConditionCode, Condition)
    SELECT r.ReturnRecordId, ld.DeviceId, ld.DetailId, 1, 0, N'Tốt'
    FROM dbo.ReturnRecords r JOIN dbo.LoanSlipDetails ld ON ld.LoanSlipId = r.LoanSlipId WHERE r.RecordCode = 'BB-0001';
END

/* ----- PM-0004 (trả một phần) + BB-0002 ----- */
IF NOT EXISTS (SELECT 1 FROM dbo.LoanSlips WHERE SlipCode = 'PM-0004')
BEGIN
    INSERT INTO dbo.LoanSlips (SlipCode, BorrowerId, CreatedBy, BorrowDate, DueDate, Status)
    SELECT 'PM-0004', BorrowerId, @kho, DATEADD(DAY, -3, @today), DATEADD(DAY, 4, @today), 0 FROM dbo.Borrowers WHERE BorrowerCode = 'NM0003';
    INSERT INTO dbo.LoanSlipDetails (LoanSlipId, DeviceId, Quantity, ReturnedQuantity)
    SELECT s.LoanSlipId, d.DeviceId, v.Qty, v.Ret
    FROM (VALUES ('TB0002', 3, 1), ('TB0005', 2, 0)) v(Dev, Qty, Ret)
    JOIN dbo.LoanSlips s ON s.SlipCode = 'PM-0004' JOIN dbo.Devices d ON d.DeviceCode = v.Dev;

    INSERT INTO dbo.ReturnRecords (RecordCode, LoanSlipId, ReturnDate, ReceivedBy, GiverName, Note)
    SELECT 'BB-0002', LoanSlipId, DATEADD(DAY, -1, SYSDATETIME()), @kho, N'Hoàng Minh Châu', N'Trả trước 1 máy' FROM dbo.LoanSlips WHERE SlipCode = 'PM-0004';
    INSERT INTO dbo.ReturnRecordDetails (ReturnRecordId, DeviceId, LoanSlipDetailId, Quantity, ConditionCode, Condition)
    SELECT r.ReturnRecordId, ld.DeviceId, ld.DetailId, 1, 0, N'Tốt'
    FROM dbo.ReturnRecords r
    JOIN dbo.LoanSlipDetails ld ON ld.LoanSlipId = r.LoanSlipId AND ld.ReturnedQuantity = 1
    WHERE r.RecordCode = 'BB-0002';
END

/* ----- PM-0005 (quá hạn, trả một phần, 1 thiết bị hư hỏng) + BB-0003 ----- */
IF NOT EXISTS (SELECT 1 FROM dbo.LoanSlips WHERE SlipCode = 'PM-0005')
BEGIN
    INSERT INTO dbo.LoanSlips (SlipCode, BorrowerId, CreatedBy, BorrowDate, DueDate, Status)
    SELECT 'PM-0005', BorrowerId, @admin, DATEADD(DAY, -6, @today), DATEADD(DAY, -1, @today), 0 FROM dbo.Borrowers WHERE BorrowerCode = 'NM0002';
    INSERT INTO dbo.LoanSlipDetails (LoanSlipId, DeviceId, Quantity, ReturnedQuantity)
    SELECT s.LoanSlipId, d.DeviceId, 2, 1
    FROM dbo.LoanSlips s JOIN dbo.Devices d ON d.DeviceCode = 'TB0007' WHERE s.SlipCode = 'PM-0005';

    INSERT INTO dbo.ReturnRecords (RecordCode, LoanSlipId, ReturnDate, ReceivedBy, GiverName, Note)
    SELECT 'BB-0003', LoanSlipId, SYSDATETIME(), @admin, N'Phạm Thị Bình', N'Trả 1 router, bị hỏng nguồn' FROM dbo.LoanSlips WHERE SlipCode = 'PM-0005';
    INSERT INTO dbo.ReturnRecordDetails (ReturnRecordId, DeviceId, LoanSlipDetailId, Quantity, ConditionCode, Condition)
    SELECT r.ReturnRecordId, ld.DeviceId, ld.DetailId, 1, 1, N'Hỏng nguồn, không lên đèn'
    FROM dbo.ReturnRecords r JOIN dbo.LoanSlipDetails ld ON ld.LoanSlipId = r.LoanSlipId WHERE r.RecordCode = 'BB-0003';

    UPDATE dbo.Devices SET DamagedQuantity = DamagedQuantity + 1 WHERE DeviceCode = 'TB0007';
END
GO

/* Tính lại trạng thái phiếu và tồn kho khả dụng cho khớp các phiếu ở trên */
EXEC dbo.sp_RecalculateInventory;
GO
