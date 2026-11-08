/* =====================================================================
   03 - RÀNG BUỘC (PK/FK/UNIQUE/CHECK). Chạy lại an toàn: chỉ thêm ràng buộc còn thiếu.
   ===================================================================== */
USE QuanLyMuonTra;
GO

/* Tồn kho: 0 <= Available, Damaged, Lost và Available + Damaged + Lost <= Total */
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_Devices_Inventory' AND parent_object_id = OBJECT_ID('dbo.Devices'))
    ALTER TABLE dbo.Devices WITH CHECK ADD CONSTRAINT CK_Devices_Inventory CHECK (
        AvailableQuantity >= 0 AND DamagedQuantity >= 0 AND LostQuantity >= 0
        AND AvailableQuantity <= TotalQuantity
        AND AvailableQuantity + DamagedQuantity + LostQuantity <= TotalQuantity);

/* Số lượng đã trả: 0 <= ReturnedQuantity <= Quantity */
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_LoanSlipDetails_Returned' AND parent_object_id = OBJECT_ID('dbo.LoanSlipDetails'))
    ALTER TABLE dbo.LoanSlipDetails WITH CHECK ADD CONSTRAINT CK_LoanSlipDetails_Returned CHECK (
        ReturnedQuantity >= 0 AND ReturnedQuantity <= Quantity);

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_LoanSlips_Status' AND parent_object_id = OBJECT_ID('dbo.LoanSlips'))
    ALTER TABLE dbo.LoanSlips WITH CHECK ADD CONSTRAINT CK_LoanSlips_Status CHECK (Status IN (0, 1));

/* Hạn trả >= ngày mượn (NOCHECK để không chặn dữ liệu cũ, vẫn áp dụng cho dữ liệu mới) */
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_LoanSlips_Dates' AND parent_object_id = OBJECT_ID('dbo.LoanSlips'))
    ALTER TABLE dbo.LoanSlips WITH NOCHECK ADD CONSTRAINT CK_LoanSlips_Dates CHECK (DueDate >= BorrowDate);

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_RRD_ConditionCode' AND parent_object_id = OBJECT_ID('dbo.ReturnRecordDetails'))
    ALTER TABLE dbo.ReturnRecordDetails WITH CHECK ADD CONSTRAINT CK_RRD_ConditionCode CHECK (ConditionCode IN (0, 1, 2, 3));

/* Mã tự động: đặt lại theo ngày chỉ hợp lệ khi mã có chứa ngày (tránh trùng mã) */
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = 'CK_CodeConfigs_Rules' AND parent_object_id = OBJECT_ID('dbo.CodeConfigs'))
    ALTER TABLE dbo.CodeConfigs WITH CHECK ADD CONSTRAINT CK_CodeConfigs_Rules CHECK (
        NumberLength BETWEEN 1 AND 9 AND CurrentNumber >= 0 AND (ResetDaily = 0 OR UseDate = 1));

/* Truy vết: dòng trả -> dòng mượn */
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_RRD_LoanSlipDetail')
    ALTER TABLE dbo.ReturnRecordDetails ADD CONSTRAINT FK_RRD_LoanSlipDetail
        FOREIGN KEY (LoanSlipDetailId) REFERENCES dbo.LoanSlipDetails(DetailId);

/* Người hủy biên bản */
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ReturnRecords_CancelledBy')
    ALTER TABLE dbo.ReturnRecords ADD CONSTRAINT FK_ReturnRecords_CancelledBy
        FOREIGN KEY (CancelledBy) REFERENCES dbo.Users(UserId);

/* Một thiết bị chỉ xuất hiện một lần trong cùng phiếu mượn (bỏ qua nếu dữ liệu cũ đang trùng) */
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_LoanSlipDetails_Slip_Device' AND object_id = OBJECT_ID('dbo.LoanSlipDetails'))
   AND NOT EXISTS (SELECT 1 FROM dbo.LoanSlipDetails GROUP BY LoanSlipId, DeviceId HAVING COUNT(*) > 1)
    CREATE UNIQUE INDEX UQ_LoanSlipDetails_Slip_Device ON dbo.LoanSlipDetails (LoanSlipId, DeviceId);
GO
