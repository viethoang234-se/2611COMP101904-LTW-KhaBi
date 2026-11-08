namespace QuanLyMuonTra.App.Models;

public sealed class UserAdminRow
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public int RoleId { get; set; }
    public string RoleName { get; set; } = "";
    public bool IsActive { get; set; }
}

public sealed class RoleRow
{
    public int RoleId { get; set; }
    public string RoleName { get; set; } = "";
    public string Description { get; set; } = "";
    public int UserCount { get; set; }
}

public sealed class PermissionRow
{
    public int PermissionId { get; set; }
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";
}

public sealed class CodeConfigRow
{
    public int ConfigId { get; set; }
    public string EntityName { get; set; } = "";
    public string Prefix { get; set; } = "";
    public bool UseDate { get; set; }
    public int NumberLength { get; set; }
    public int CurrentNumber { get; set; }
    public bool ResetDaily { get; set; }
    public DateTime? LastResetDate { get; set; }
}

public sealed class ReportSummary
{
    public int SlipCount { get; set; }
    public int BorrowedQuantity { get; set; }
    public int ReturnedQuantity { get; set; }
    public int OutstandingQuantity { get; set; }
    public int OverdueSlipCount { get; set; }
    public int ActiveDeviceCount { get; set; }
    public int AvailableQuantity { get; set; }
    public int DamagedQuantity { get; set; }
    public int LostQuantity { get; set; }
}

public sealed class PopularDeviceRow
{
    public string DeviceCode { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public string CategoryName { get; set; } = "";
    public int LoanCount { get; set; }
    public int BorrowedQuantity { get; set; }
}
