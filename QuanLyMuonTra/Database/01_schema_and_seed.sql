/* =====================================================================
   QUẢN LÝ MƯỢN TRẢ THIẾT BỊ - Tạo CSDL + dữ liệu mẫu
   Chạy toàn bộ file này trong SSMS (có thể chạy lại nhiều lần, không mất dữ liệu).
   Tài khoản mẫu:  admin / Admin@123  (Người quản lý)
                   kho01 / Kho@123    (Nhân viên kho)
   ===================================================================== */
IF DB_ID(N'QuanLyMuonTra') IS NULL
    CREATE DATABASE QuanLyMuonTra;
GO
USE QuanLyMuonTra;
GO

/* ---------- 1. BẢNG ---------- */
IF OBJECT_ID(N'Roles') IS NULL
CREATE TABLE Roles (
    RoleId      INT IDENTITY(1,1) PRIMARY KEY,
    RoleName    NVARCHAR(50)  NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL
);

IF OBJECT_ID(N'Permissions') IS NULL
CREATE TABLE Permissions (
    PermissionId INT IDENTITY(1,1) PRIMARY KEY,
    Code         VARCHAR(50)   NOT NULL UNIQUE,
    Name         NVARCHAR(100) NOT NULL
);

IF OBJECT_ID(N'RolePermissions') IS NULL
CREATE TABLE RolePermissions (
    RoleId       INT NOT NULL REFERENCES Roles(RoleId),
    PermissionId INT NOT NULL REFERENCES Permissions(PermissionId),
    PRIMARY KEY (RoleId, PermissionId)
);

IF OBJECT_ID(N'Users') IS NULL
CREATE TABLE Users (
    UserId       INT IDENTITY(1,1) PRIMARY KEY,
    Username     VARCHAR(50)   NOT NULL UNIQUE,
    PasswordHash VARCHAR(200)  NOT NULL,
    FullName     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(100) NULL,
    Phone        NVARCHAR(20)  NULL,
    RoleId       INT NOT NULL REFERENCES Roles(RoleId),
    IsActive     BIT NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

IF OBJECT_ID(N'DeviceCategories') IS NULL
CREATE TABLE DeviceCategories (
    CategoryId   INT IDENTITY(1,1) PRIMARY KEY,
    CategoryCode VARCHAR(20)   NOT NULL UNIQUE,
    CategoryName NVARCHAR(100) NOT NULL,
    Description  NVARCHAR(255) NULL,
    IsActive     BIT NOT NULL DEFAULT 1
);

IF OBJECT_ID(N'Devices') IS NULL
CREATE TABLE Devices (
    DeviceId          INT IDENTITY(1,1) PRIMARY KEY,
    DeviceCode        VARCHAR(30)   NOT NULL UNIQUE,
    DeviceName        NVARCHAR(150) NOT NULL,
    CategoryId        INT NOT NULL REFERENCES DeviceCategories(CategoryId),
    TotalQuantity     INT NOT NULL DEFAULT 0 CHECK (TotalQuantity >= 0),
    AvailableQuantity INT NOT NULL DEFAULT 0 CHECK (AvailableQuantity >= 0),
    Unit              NVARCHAR(30)  NULL,
    Note              NVARCHAR(255) NULL,
    IsActive          BIT NOT NULL DEFAULT 1,
    CreatedAt         DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

IF OBJECT_ID(N'Borrowers') IS NULL
CREATE TABLE Borrowers (
    BorrowerId   INT IDENTITY(1,1) PRIMARY KEY,
    BorrowerCode VARCHAR(30)   NOT NULL UNIQUE,
    FullName     NVARCHAR(100) NOT NULL,
    Phone        NVARCHAR(20)  NULL,
    Email        NVARCHAR(100) NULL,
    Department   NVARCHAR(100) NULL,
    Note         NVARCHAR(255) NULL,
    IsActive     BIT NOT NULL DEFAULT 1
);

/* Status: 0 = Đang mượn, 1 = Đã trả (Quá hạn được tính từ DueDate) */
IF OBJECT_ID(N'LoanSlips') IS NULL
CREATE TABLE LoanSlips (
    LoanSlipId        INT IDENTITY(1,1) PRIMARY KEY,
    SlipCode          VARCHAR(30) NOT NULL UNIQUE,
    BorrowerId        INT NOT NULL REFERENCES Borrowers(BorrowerId),
    CreatedBy         INT NOT NULL REFERENCES Users(UserId),
    BorrowDate        DATE NOT NULL,
    DueDate           DATE NOT NULL,
    Status            TINYINT NOT NULL DEFAULT 0,
    Note              NVARCHAR(255) NULL,
    BorrowerSignature VARBINARY(MAX) NULL,
    CreatedAt         DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

IF OBJECT_ID(N'LoanSlipDetails') IS NULL
CREATE TABLE LoanSlipDetails (
    DetailId         INT IDENTITY(1,1) PRIMARY KEY,
    LoanSlipId       INT NOT NULL REFERENCES LoanSlips(LoanSlipId),
    DeviceId         INT NOT NULL REFERENCES Devices(DeviceId),
    Quantity         INT NOT NULL CHECK (Quantity > 0),
    ReturnedQuantity INT NOT NULL DEFAULT 0
);

IF OBJECT_ID(N'ReturnRecords') IS NULL
CREATE TABLE ReturnRecords (
    ReturnRecordId    INT IDENTITY(1,1) PRIMARY KEY,
    RecordCode        VARCHAR(30) NOT NULL UNIQUE,
    LoanSlipId        INT NOT NULL REFERENCES LoanSlips(LoanSlipId),
    ReturnDate        DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ReceivedBy        INT NOT NULL REFERENCES Users(UserId),
    Note              NVARCHAR(255) NULL,
    IsCancelled       BIT NOT NULL DEFAULT 0,
    ReceiverSignature VARBINARY(MAX) NULL,
    BorrowerSignature VARBINARY(MAX) NULL
);

IF OBJECT_ID(N'ReturnRecordDetails') IS NULL
CREATE TABLE ReturnRecordDetails (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    ReturnRecordId INT NOT NULL REFERENCES ReturnRecords(ReturnRecordId),
    DeviceId       INT NOT NULL REFERENCES Devices(DeviceId),
    Quantity       INT NOT NULL CHECK (Quantity > 0),
    Condition      NVARCHAR(100) NULL
);

/* Cấu hình mã tự động (UC16): ví dụ PM-20261105-0001 */
IF OBJECT_ID(N'CodeConfigs') IS NULL
CREATE TABLE CodeConfigs (
    ConfigId      INT IDENTITY(1,1) PRIMARY KEY,
    EntityName    VARCHAR(30)  NOT NULL UNIQUE,
    Prefix        VARCHAR(10)  NOT NULL,
    UseDate       BIT NOT NULL DEFAULT 1,
    NumberLength  INT NOT NULL DEFAULT 4,
    CurrentNumber INT NOT NULL DEFAULT 0,
    ResetDaily    BIT NOT NULL DEFAULT 1,
    LastResetDate DATE NULL
);
GO

/* ---------- 2. DỮ LIỆU MẪU (chỉ nạp khi chưa có) ---------- */
IF NOT EXISTS (SELECT 1 FROM Roles)
BEGIN
    INSERT INTO Roles (RoleName, Description) VALUES
        (N'Người quản lý', N'Toàn quyền quản trị hệ thống'),
        (N'Nhân viên kho', N'Thao tác nghiệp vụ mượn trả hằng ngày');

    INSERT INTO Permissions (Code, Name) VALUES
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
        ('STATS_VIEW',      N'Thống kê thiết bị được mượn nhiều');

    -- Người quản lý: tất cả quyền
    INSERT INTO RolePermissions (RoleId, PermissionId)
    SELECT r.RoleId, p.PermissionId
    FROM Roles r CROSS JOIN Permissions p
    WHERE r.RoleName = N'Người quản lý';

    -- Nhân viên kho: nghiệp vụ hằng ngày (không quản trị, không hủy biên bản, chỉ in phiếu/biên bản)
    INSERT INTO RolePermissions (RoleId, PermissionId)
    SELECT r.RoleId, p.PermissionId
    FROM Roles r
    JOIN Permissions p ON p.Code IN ('DASHBOARD_VIEW','CATEGORY_VIEW','DEVICE_MANAGE','BORROWER_MANAGE',
                                     'LOAN_CREATE','LOAN_VIEW','RETURN_CREATE','RECORD_VIEW',
                                     'OVERDUE_VIEW','REPORT_PRINT')
    WHERE r.RoleName = N'Nhân viên kho';

    INSERT INTO Users (Username, PasswordHash, FullName, Email, Phone, RoleId)
    SELECT 'admin',
           'PBKDF2$100000$g3YB9EjGXbB4os2m3O8Haw==$dINXvBOZn0icxZ6kQfB/eh6Ui+Z8YyQjNZLTB1LKSs8=',
           N'Nguyễn Quản Lý', 'admin@example.com', '0900000001', RoleId
    FROM Roles WHERE RoleName = N'Người quản lý';

    INSERT INTO Users (Username, PasswordHash, FullName, Email, Phone, RoleId)
    SELECT 'kho01',
           'PBKDF2$100000$fpaLmTN8cX9yx3+WvLzqZA==$wpzDKuGQWJav6aJRM9VeQufTly/KGctqfn5TjzjwJVg=',
           N'Trần Thủ Kho', 'kho01@example.com', '0900000002', RoleId
    FROM Roles WHERE RoleName = N'Nhân viên kho';

    INSERT INTO DeviceCategories (CategoryCode, CategoryName, Description) VALUES
        ('LAPTOP', N'Máy tính xách tay', N'Laptop các loại'),
        ('PROJ',   N'Máy chiếu', N'Máy chiếu và phụ kiện'),
        ('CAM',    N'Máy ảnh / Camera', N'Thiết bị ghi hình'),
        ('AUDIO',  N'Âm thanh', N'Micro, loa, mixer'),
        ('NET',    N'Thiết bị mạng', N'Router, switch, cáp mạng'),
        ('OTHER',  N'Khác', N'Thiết bị khác');

    INSERT INTO Devices (DeviceCode, DeviceName, CategoryId, TotalQuantity, AvailableQuantity, Unit, Note)
    SELECT v.Code, v.Name, c.CategoryId, v.Total, v.Avail, v.Unit, v.Note
    FROM (VALUES
        ('TB0001', N'Laptop Dell Latitude 5440', 'LAPTOP', 10, 7, N'Cái', N'Kèm sạc'),
        ('TB0002', N'Laptop Lenovo ThinkPad E14', 'LAPTOP', 8, 8, N'Cái', NULL),
        ('TB0003', N'Máy chiếu Epson EB-X06', 'PROJ', 5, 4, N'Cái', N'Kèm dây HDMI'),
        ('TB0004', N'Máy ảnh Canon EOS 850D', 'CAM', 3, 2, N'Bộ', N'Kèm thẻ nhớ 64GB'),
        ('TB0005', N'Micro không dây Shure', 'AUDIO', 6, 6, N'Bộ', NULL),
        ('TB0006', N'Loa kéo di động', 'AUDIO', 4, 3, N'Cái', NULL),
        ('TB0007', N'Router TP-Link Archer AX55', 'NET', 5, 5, N'Cái', NULL),
        ('TB0008', N'Bút trình chiếu Logitech', 'OTHER', 12, 11, N'Cái', NULL)
    ) AS v(Code, Name, CatCode, Total, Avail, Unit, Note)
    JOIN DeviceCategories c ON c.CategoryCode = v.CatCode;

    INSERT INTO Borrowers (BorrowerCode, FullName, Phone, Email, Department) VALUES
        ('NM0001', N'Lê Văn An',    '0911111111', 'an@example.com',   N'Phòng Đào tạo'),
        ('NM0002', N'Phạm Thị Bình','0922222222', 'binh@example.com', N'Khoa CNTT'),
        ('NM0003', N'Hoàng Minh Châu','0933333333','chau@example.com',N'Phòng Hành chính');

    -- Phiếu mượn mẫu: 1 quá hạn, 1 đang mượn, 1 đã trả
    DECLARE @admin INT = (SELECT UserId FROM Users WHERE Username = 'admin');
    DECLARE @today DATE = CAST(SYSDATETIME() AS DATE);

    INSERT INTO LoanSlips (SlipCode, BorrowerId, CreatedBy, BorrowDate, DueDate, Status)
    SELECT 'PM-0001', BorrowerId, @admin, DATEADD(DAY, -10, @today), DATEADD(DAY, -3, @today), 0 FROM Borrowers WHERE BorrowerCode = 'NM0001';
    INSERT INTO LoanSlips (SlipCode, BorrowerId, CreatedBy, BorrowDate, DueDate, Status)
    SELECT 'PM-0002', BorrowerId, @admin, DATEADD(DAY, -2, @today), DATEADD(DAY, 5, @today), 0 FROM Borrowers WHERE BorrowerCode = 'NM0002';
    INSERT INTO LoanSlips (SlipCode, BorrowerId, CreatedBy, BorrowDate, DueDate, Status)
    SELECT 'PM-0003', BorrowerId, @admin, DATEADD(DAY, -14, @today), DATEADD(DAY, -7, @today), 1 FROM Borrowers WHERE BorrowerCode = 'NM0003';

    INSERT INTO LoanSlipDetails (LoanSlipId, DeviceId, Quantity, ReturnedQuantity)
    SELECT s.LoanSlipId, d.DeviceId, v.Qty, v.Ret
    FROM (VALUES
        ('PM-0001','TB0001',2,0), ('PM-0001','TB0004',1,0),
        ('PM-0002','TB0003',1,0), ('PM-0002','TB0006',1,0),
        ('PM-0003','TB0008',1,1)
    ) AS v(Slip, Dev, Qty, Ret)
    JOIN LoanSlips s ON s.SlipCode = v.Slip
    JOIN Devices d ON d.DeviceCode = v.Dev;

    INSERT INTO CodeConfigs (EntityName, Prefix, UseDate, NumberLength, CurrentNumber, ResetDaily) VALUES
        ('LoanSlip',     'PM', 1, 4, 3, 1),
        ('ReturnRecord', 'BB', 1, 4, 0, 1),
        ('Device',       'TB', 0, 4, 8, 0),
        ('Borrower',     'NM', 0, 4, 3, 0);
END
GO
