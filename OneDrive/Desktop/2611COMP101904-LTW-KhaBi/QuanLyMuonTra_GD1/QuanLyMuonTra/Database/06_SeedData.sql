/* =====================================================================
   06 - DỮ LIỆU KHỞI TẠO (vai trò, quyền, tài khoản mẫu, danh mục, cấu hình mã)
   Chạy lại an toàn: chỉ thêm phần còn thiếu, không ghi đè dữ liệu đang có.
   Tài khoản mẫu:  admin / Admin@123  (Người quản lý)
                   kho01 / Kho@123    (Nhân viên kho)
   ===================================================================== */
USE QuanLyMuonTra;
GO

/* ---------- Vai trò ---------- */
INSERT INTO dbo.Roles (RoleName, Description)
SELECT v.RoleName, v.Description
FROM (VALUES (N'Người quản lý', N'Toàn quyền quản trị hệ thống'),
             (N'Nhân viên kho', N'Thao tác nghiệp vụ mượn trả hằng ngày')) v(RoleName, Description)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Roles r WHERE r.RoleName = v.RoleName);

/* ---------- Quyền ---------- */
INSERT INTO dbo.Permissions (Code, Name)
SELECT v.Code, v.Name
FROM (VALUES
    ('DASHBOARD_VIEW',  N'Xem Dashboard'),
    ('USER_MANAGE',     N'Quản lý người dùng'),
    ('ROLE_MANAGE',     N'Quản lý nhóm quyền'),
    ('CATEGORY_VIEW',   N'Xem loại thiết bị'),
    ('CATEGORY_MANAGE', N'Quản lý loại thiết bị'),
    ('DEVICE_MANAGE',   N'Quản lý thiết bị'),
    ('BORROWER_MANAGE', N'Quản lý người mượn'),
    ('LOAN_CREATE',     N'Lập phiếu mượn'),
    ('LOAN_VIEW',       N'Tra cứu / xem phiếu mượn'),
    ('RETURN_CREATE',   N'Ghi nhận trả thiết bị'),
    ('RECORD_VIEW',     N'Xem biên bản trả / bàn giao'),
    ('RECORD_CANCEL',   N'Hủy biên bản'),
    ('OVERDUE_VIEW',    N'Xem phiếu quá hạn'),
    ('REPORT_PRINT',    N'In phiếu, biên bản'),
    ('REPORT_VIEW',     N'Xem báo cáo, thống kê'),
    ('CODE_CONFIG',     N'Cấu hình mã tự động'),
    ('STATS_VIEW',      N'Thống kê thiết bị được mượn nhiều')
) v(Code, Name)
WHERE NOT EXISTS (SELECT 1 FROM dbo.Permissions p WHERE p.Code = v.Code);

/* ---------- Gán quyền mặc định ---------- */
-- Người quản lý: mọi quyền (kể cả quyền thêm mới ở các lần chạy sau)
INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId
FROM dbo.Roles r CROSS JOIN dbo.Permissions p
WHERE r.RoleName = N'Người quản lý'
  AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions rp WHERE rp.RoleId = r.RoleId AND rp.PermissionId = p.PermissionId);

-- Nhân viên kho: chỉ gán khi vai trò này CHƯA có quyền nào (tôn trọng cấu hình người quản lý đã chỉnh)
INSERT INTO dbo.RolePermissions (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId
FROM dbo.Roles r
JOIN dbo.Permissions p ON p.Code IN ('DASHBOARD_VIEW','CATEGORY_VIEW','DEVICE_MANAGE','BORROWER_MANAGE',
                                     'LOAN_CREATE','LOAN_VIEW','RETURN_CREATE','RECORD_VIEW',
                                     'OVERDUE_VIEW','REPORT_PRINT')
WHERE r.RoleName = N'Nhân viên kho'
  AND NOT EXISTS (SELECT 1 FROM dbo.RolePermissions rp WHERE rp.RoleId = r.RoleId);

/* ---------- Tài khoản mẫu (mật khẩu đã băm PBKDF2, hãy đổi khi dùng thật) ---------- */
INSERT INTO dbo.Users (Username, PasswordHash, FullName, Email, Phone, RoleId)
SELECT 'admin',
       'PBKDF2$100000$g3YB9EjGXbB4os2m3O8Haw==$dINXvBOZn0icxZ6kQfB/eh6Ui+Z8YyQjNZLTB1LKSs8=',
       N'Nguyễn Quản Lý', 'admin@example.com', '0900000001', RoleId
FROM dbo.Roles WHERE RoleName = N'Người quản lý'
  AND NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'admin');

INSERT INTO dbo.Users (Username, PasswordHash, FullName, Email, Phone, RoleId)
SELECT 'kho01',
       'PBKDF2$100000$fpaLmTN8cX9yx3+WvLzqZA==$wpzDKuGQWJav6aJRM9VeQufTly/KGctqfn5TjzjwJVg=',
       N'Trần Thủ Kho', 'kho01@example.com', '0900000002', RoleId
FROM dbo.Roles WHERE RoleName = N'Nhân viên kho'
  AND NOT EXISTS (SELECT 1 FROM dbo.Users WHERE Username = 'kho01');

/* ---------- Loại thiết bị ---------- */
INSERT INTO dbo.DeviceCategories (CategoryCode, CategoryName, Description)
SELECT v.Code, v.Name, v.Descr
FROM (VALUES
    ('LAPTOP', N'Máy tính xách tay', N'Laptop các loại'),
    ('PROJ',   N'Máy chiếu',         N'Máy chiếu và phụ kiện'),
    ('CAM',    N'Máy ảnh / Camera',  N'Thiết bị ghi hình'),
    ('AUDIO',  N'Âm thanh',          N'Micro, loa, mixer'),
    ('NET',    N'Thiết bị mạng',     N'Router, switch, cáp mạng'),
    ('OTHER',  N'Khác',              N'Thiết bị khác')
) v(Code, Name, Descr)
WHERE NOT EXISTS (SELECT 1 FROM dbo.DeviceCategories c WHERE c.CategoryCode = v.Code);

/* ---------- Thiết bị mẫu (chỉ nạp khi bảng còn trống). Khả dụng = tổng, tồn kho thực tế được tính lại ở cuối script 07 ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Devices)
    INSERT INTO dbo.Devices (DeviceCode, DeviceName, CategoryId, TotalQuantity, AvailableQuantity, Unit, Note)
    SELECT v.Code, v.Name, c.CategoryId, v.Total, v.Total, v.Unit, v.Note
    FROM (VALUES
        ('TB0001', N'Laptop Dell Latitude 5440',  'LAPTOP', 10, N'Cái', N'Kèm sạc'),
        ('TB0002', N'Laptop Lenovo ThinkPad E14', 'LAPTOP',  8, N'Cái', NULL),
        ('TB0003', N'Máy chiếu Epson EB-X06',     'PROJ',    5, N'Cái', N'Kèm dây HDMI'),
        ('TB0004', N'Máy ảnh Canon EOS 850D',     'CAM',     3, N'Bộ',  N'Kèm thẻ nhớ 64GB'),
        ('TB0005', N'Micro không dây Shure',      'AUDIO',   6, N'Bộ',  NULL),
        ('TB0006', N'Loa kéo di động',            'AUDIO',   4, N'Cái', NULL),
        ('TB0007', N'Router TP-Link Archer AX55', 'NET',     5, N'Cái', NULL),
        ('TB0008', N'Bút trình chiếu Logitech',   'OTHER',  12, N'Cái', NULL)
    ) v(Code, Name, CatCode, Total, Unit, Note)
    JOIN dbo.DeviceCategories c ON c.CategoryCode = v.CatCode;

/* ---------- Người mượn mẫu (chỉ nạp khi bảng còn trống) ---------- */
IF NOT EXISTS (SELECT 1 FROM dbo.Borrowers)
    INSERT INTO dbo.Borrowers (BorrowerCode, FullName, Phone, Email, Department) VALUES
        ('NM0001', N'Lê Văn An',       '0911111111', 'an@example.com',    N'Phòng Đào tạo'),
        ('NM0002', N'Phạm Thị Bình',   '0922222222', 'binh@example.com',  N'Khoa CNTT'),
        ('NM0003', N'Hoàng Minh Châu', '0933333333', 'chau@example.com',  N'Phòng Hành chính');

/* ---------- Cấu hình mã tự động ---------- */
INSERT INTO dbo.CodeConfigs (EntityName, Prefix, UseDate, NumberLength, CurrentNumber, ResetDaily)
SELECT v.EntityName, v.Prefix, v.UseDate, v.NumberLength, 0, v.ResetDaily
FROM (VALUES ('LoanSlip', 'PM', 1, 4, 1), ('ReturnRecord', 'BB', 1, 4, 1),
             ('Device',   'TB', 0, 4, 0), ('Borrower',     'NM', 0, 4, 0)) v(EntityName, Prefix, UseDate, NumberLength, ResetDaily)
WHERE NOT EXISTS (SELECT 1 FROM dbo.CodeConfigs c WHERE c.EntityName = v.EntityName);

/* Đồng bộ bộ đếm mã thiết bị / người mượn với mã lớn nhất đang có (tránh trùng mã khi dữ liệu đã nhập tay) */
DECLARE @maxDev INT = ISNULL((SELECT MAX(TRY_CAST(SUBSTRING(DeviceCode, 3, 20) AS INT)) FROM dbo.Devices WHERE DeviceCode LIKE 'TB[0-9]%'), 0);
UPDATE dbo.CodeConfigs SET CurrentNumber = @maxDev WHERE EntityName = 'Device' AND Prefix = 'TB' AND CurrentNumber < @maxDev;

DECLARE @maxBor INT = ISNULL((SELECT MAX(TRY_CAST(SUBSTRING(BorrowerCode, 3, 20) AS INT)) FROM dbo.Borrowers WHERE BorrowerCode LIKE 'NM[0-9]%'), 0);
UPDATE dbo.CodeConfigs SET CurrentNumber = @maxBor WHERE EntityName = 'Borrower' AND Prefix = 'NM' AND CurrentNumber < @maxBor;
GO
