/* =====================================================================
   04 - INDEX cho các cột hay tìm kiếm / lọc / nối bảng. Chạy lại an toàn.
   ===================================================================== */
USE QuanLyMuonTra;
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Devices_Name' AND object_id = OBJECT_ID('dbo.Devices'))
    CREATE NONCLUSTERED INDEX IX_Devices_Name ON dbo.Devices (DeviceName);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Devices_Category' AND object_id = OBJECT_ID('dbo.Devices'))
    CREATE NONCLUSTERED INDEX IX_Devices_Category ON dbo.Devices (CategoryId, IsActive) INCLUDE (AvailableQuantity, TotalQuantity);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Borrowers_Name' AND object_id = OBJECT_ID('dbo.Borrowers'))
    CREATE NONCLUSTERED INDEX IX_Borrowers_Name ON dbo.Borrowers (FullName);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Borrowers_Phone' AND object_id = OBJECT_ID('dbo.Borrowers'))
    CREATE NONCLUSTERED INDEX IX_Borrowers_Phone ON dbo.Borrowers (Phone);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LoanSlips_Borrower' AND object_id = OBJECT_ID('dbo.LoanSlips'))
    CREATE NONCLUSTERED INDEX IX_LoanSlips_Borrower ON dbo.LoanSlips (BorrowerId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LoanSlips_BorrowDate' AND object_id = OBJECT_ID('dbo.LoanSlips'))
    CREATE NONCLUSTERED INDEX IX_LoanSlips_BorrowDate ON dbo.LoanSlips (BorrowDate);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LoanSlips_Due_Status' AND object_id = OBJECT_ID('dbo.LoanSlips'))
    CREATE NONCLUSTERED INDEX IX_LoanSlips_Due_Status ON dbo.LoanSlips (DueDate, Status);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LoanSlipDetails_Slip' AND object_id = OBJECT_ID('dbo.LoanSlipDetails'))
    CREATE NONCLUSTERED INDEX IX_LoanSlipDetails_Slip ON dbo.LoanSlipDetails (LoanSlipId) INCLUDE (DeviceId, Quantity, ReturnedQuantity);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LoanSlipDetails_Device' AND object_id = OBJECT_ID('dbo.LoanSlipDetails'))
    CREATE NONCLUSTERED INDEX IX_LoanSlipDetails_Device ON dbo.LoanSlipDetails (DeviceId) INCLUDE (Quantity, ReturnedQuantity);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ReturnRecords_Slip' AND object_id = OBJECT_ID('dbo.ReturnRecords'))
    CREATE NONCLUSTERED INDEX IX_ReturnRecords_Slip ON dbo.ReturnRecords (LoanSlipId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_ReturnRecords_Date' AND object_id = OBJECT_ID('dbo.ReturnRecords'))
    CREATE NONCLUSTERED INDEX IX_ReturnRecords_Date ON dbo.ReturnRecords (ReturnDate);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RRD_Record' AND object_id = OBJECT_ID('dbo.ReturnRecordDetails'))
    CREATE NONCLUSTERED INDEX IX_RRD_Record ON dbo.ReturnRecordDetails (ReturnRecordId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RRD_Device' AND object_id = OBJECT_ID('dbo.ReturnRecordDetails'))
    CREATE NONCLUSTERED INDEX IX_RRD_Device ON dbo.ReturnRecordDetails (DeviceId);
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_RRD_LoanSlipDetail' AND object_id = OBJECT_ID('dbo.ReturnRecordDetails'))
    CREATE NONCLUSTERED INDEX IX_RRD_LoanSlipDetail ON dbo.ReturnRecordDetails (LoanSlipDetailId);
GO
