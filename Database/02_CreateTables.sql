/* =====================================================================
   02 - TẠO BẢNG và NÂNG CẤP CỘT
   - Tạo bảng nếu chưa có (CSDL mới).
   - Bổ sung cột mới nếu thiếu (CSDL cũ từ phiên bản trước vẫn nâng cấp được, giữ nguyên dữ liệu).

   Quy ước trạng thái:
   - LoanSlips.Status        : 0 = còn thiết bị chưa trả, 1 = đã trả đủ.
                               "Trả một phần" và "Quá hạn" được TÍNH từ chi tiết + ngày hiện tại (xem vw_LoanSlipSummary).
   - ReturnRecordDetails.ConditionCode : 0 = Tốt, 1 = Hư hỏng, 2 = Mất, 3 = Thiếu phụ kiện.
   - Devices : TotalQuantity = AvailableQuantity + (đang mượn) + DamagedQuantity + LostQuantity.
   ===================================================================== */
USE QuanLyMuonTra;
GO

IF OBJECT_ID(N'dbo.Roles') IS NULL
CREATE TABLE dbo.Roles (
    RoleId      INT IDENTITY(1,1) PRIMARY KEY,
    RoleName    NVARCHAR(50)  NOT NULL UNIQUE,
    Description NVARCHAR(200) NULL
);

IF OBJECT_ID(N'dbo.Permissions') IS NULL
CREATE TABLE dbo.Permissions (
    PermissionId INT IDENTITY(1,1) PRIMARY KEY,
    Code         VARCHAR(50)   NOT NULL UNIQUE,
    Name         NVARCHAR(100) NOT NULL
);

IF OBJECT_ID(N'dbo.RolePermissions') IS NULL
CREATE TABLE dbo.RolePermissions (
    RoleId       INT NOT NULL REFERENCES dbo.Roles(RoleId),
    PermissionId INT NOT NULL REFERENCES dbo.Permissions(PermissionId),
    PRIMARY KEY (RoleId, PermissionId)
);

IF OBJECT_ID(N'dbo.Users') IS NULL
CREATE TABLE dbo.Users (
    UserId       INT IDENTITY(1,1) PRIMARY KEY,
    Username     VARCHAR(50)   NOT NULL UNIQUE,
    PasswordHash VARCHAR(200)  NOT NULL,
    FullName     NVARCHAR(100) NOT NULL,
    Email        NVARCHAR(100) NULL,
    Phone        NVARCHAR(20)  NULL,
    RoleId       INT NOT NULL REFERENCES dbo.Roles(RoleId),
    IsActive     BIT NOT NULL DEFAULT 1,
    CreatedAt    DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

IF OBJECT_ID(N'dbo.DeviceCategories') IS NULL
CREATE TABLE dbo.DeviceCategories (
    CategoryId   INT IDENTITY(1,1) PRIMARY KEY,
    CategoryCode VARCHAR(20)   NOT NULL UNIQUE,
    CategoryName NVARCHAR(100) NOT NULL,
    Description  NVARCHAR(255) NULL,
    IsActive     BIT NOT NULL DEFAULT 1
);

IF OBJECT_ID(N'dbo.Devices') IS NULL
CREATE TABLE dbo.Devices (
    DeviceId          INT IDENTITY(1,1) PRIMARY KEY,
    DeviceCode        VARCHAR(30)   NOT NULL UNIQUE,
    DeviceName        NVARCHAR(150) NOT NULL,
    CategoryId        INT NOT NULL REFERENCES dbo.DeviceCategories(CategoryId),
    TotalQuantity     INT NOT NULL DEFAULT 0 CHECK (TotalQuantity >= 0),
    AvailableQuantity INT NOT NULL DEFAULT 0 CHECK (AvailableQuantity >= 0),
    Unit              NVARCHAR(30)  NULL,
    Note              NVARCHAR(255) NULL,
    IsActive          BIT NOT NULL DEFAULT 1,
    CreatedAt         DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

IF OBJECT_ID(N'dbo.Borrowers') IS NULL
CREATE TABLE dbo.Borrowers (
    BorrowerId   INT IDENTITY(1,1) PRIMARY KEY,
    BorrowerCode VARCHAR(30)   NOT NULL UNIQUE,
    FullName     NVARCHAR(100) NOT NULL,
    Phone        NVARCHAR(20)  NULL,
    Email        NVARCHAR(100) NULL,
    Department   NVARCHAR(100) NULL,
    Note         NVARCHAR(255) NULL,
    IsActive     BIT NOT NULL DEFAULT 1
);

IF OBJECT_ID(N'dbo.LoanSlips') IS NULL
CREATE TABLE dbo.LoanSlips (
    LoanSlipId        INT IDENTITY(1,1) PRIMARY KEY,
    SlipCode          VARCHAR(30) NOT NULL UNIQUE,
    BorrowerId        INT NOT NULL REFERENCES dbo.Borrowers(BorrowerId),
    CreatedBy         INT NOT NULL REFERENCES dbo.Users(UserId),
    BorrowDate        DATE NOT NULL,
    DueDate           DATE NOT NULL,
    Status            TINYINT NOT NULL DEFAULT 0,
    Note              NVARCHAR(255) NULL,
    BorrowerSignature VARBINARY(MAX) NULL,
    CreatedAt         DATETIME2 NOT NULL DEFAULT SYSDATETIME()
);

IF OBJECT_ID(N'dbo.LoanSlipDetails') IS NULL
CREATE TABLE dbo.LoanSlipDetails (
    DetailId         INT IDENTITY(1,1) PRIMARY KEY,
    LoanSlipId       INT NOT NULL REFERENCES dbo.LoanSlips(LoanSlipId),
    DeviceId         INT NOT NULL REFERENCES dbo.Devices(DeviceId),
    Quantity         INT NOT NULL CHECK (Quantity > 0),
    ReturnedQuantity INT NOT NULL DEFAULT 0
);

IF OBJECT_ID(N'dbo.ReturnRecords') IS NULL
CREATE TABLE dbo.ReturnRecords (
    ReturnRecordId    INT IDENTITY(1,1) PRIMARY KEY,
    RecordCode        VARCHAR(30) NOT NULL UNIQUE,
    LoanSlipId        INT NOT NULL REFERENCES dbo.LoanSlips(LoanSlipId),
    ReturnDate        DATETIME2 NOT NULL DEFAULT SYSDATETIME(),
    ReceivedBy        INT NOT NULL REFERENCES dbo.Users(UserId),
    Note              NVARCHAR(255) NULL,
    IsCancelled       BIT NOT NULL DEFAULT 0,
    ReceiverSignature VARBINARY(MAX) NULL,
    BorrowerSignature VARBINARY(MAX) NULL
);

/* Condition (NVARCHAR) giữ nguyên tên cũ, dùng làm GHI CHÚ tình trạng (mô tả hư hỏng, thiếu phụ kiện...). */
IF OBJECT_ID(N'dbo.ReturnRecordDetails') IS NULL
CREATE TABLE dbo.ReturnRecordDetails (
    Id             INT IDENTITY(1,1) PRIMARY KEY,
    ReturnRecordId INT NOT NULL REFERENCES dbo.ReturnRecords(ReturnRecordId),
    DeviceId       INT NOT NULL REFERENCES dbo.Devices(DeviceId),
    Quantity       INT NOT NULL CHECK (Quantity > 0),
    Condition      NVARCHAR(100) NULL
);

/* Mã tự động: ví dụ PM-20261106-0001 (UseDate = 1) hoặc TB0009 (UseDate = 0) */
IF OBJECT_ID(N'dbo.CodeConfigs') IS NULL
CREATE TABLE dbo.CodeConfigs (
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

/* ---------- NÂNG CẤP: bổ sung cột mới (an toàn khi chạy lại) ---------- */
IF COL_LENGTH('dbo.Devices', 'DamagedQuantity') IS NULL
    ALTER TABLE dbo.Devices ADD DamagedQuantity INT NOT NULL CONSTRAINT DF_Devices_Damaged DEFAULT 0;
IF COL_LENGTH('dbo.Devices', 'LostQuantity') IS NULL
    ALTER TABLE dbo.Devices ADD LostQuantity INT NOT NULL CONSTRAINT DF_Devices_Lost DEFAULT 0;

IF COL_LENGTH('dbo.Borrowers', 'Address') IS NULL
    ALTER TABLE dbo.Borrowers ADD Address NVARCHAR(200) NULL;

IF COL_LENGTH('dbo.ReturnRecords', 'GiverName') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD GiverName NVARCHAR(100) NULL;
IF COL_LENGTH('dbo.ReturnRecords', 'CancelledBy') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD CancelledBy INT NULL;
IF COL_LENGTH('dbo.ReturnRecords', 'CancelledAt') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD CancelledAt DATETIME2 NULL;
IF COL_LENGTH('dbo.ReturnRecords', 'CancelReason') IS NULL
    ALTER TABLE dbo.ReturnRecords ADD CancelReason NVARCHAR(255) NULL;

IF COL_LENGTH('dbo.ReturnRecordDetails', 'LoanSlipDetailId') IS NULL
    ALTER TABLE dbo.ReturnRecordDetails ADD LoanSlipDetailId INT NULL;
IF COL_LENGTH('dbo.ReturnRecordDetails', 'ConditionCode') IS NULL
    ALTER TABLE dbo.ReturnRecordDetails ADD ConditionCode TINYINT NOT NULL CONSTRAINT DF_RRD_ConditionCode DEFAULT 0;
GO

/* Truy vết dữ liệu cũ: gắn dòng trả với dòng mượn khi xác định được duy nhất */
UPDATE rd
SET    rd.LoanSlipDetailId = d.DetailId
FROM   dbo.ReturnRecordDetails rd
JOIN   dbo.ReturnRecords r      ON r.ReturnRecordId = rd.ReturnRecordId
JOIN   dbo.LoanSlipDetails d    ON d.LoanSlipId = r.LoanSlipId AND d.DeviceId = rd.DeviceId
WHERE  rd.LoanSlipDetailId IS NULL
  AND (SELECT COUNT(*) FROM dbo.LoanSlipDetails x
       WHERE x.LoanSlipId = r.LoanSlipId AND x.DeviceId = rd.DeviceId) = 1;
GO
