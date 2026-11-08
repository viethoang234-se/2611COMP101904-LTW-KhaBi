namespace QuanLyMuonTra.App.Models;

public sealed class AppUser
{
    public int UserId { get; set; }
    public string Username { get; set; } = "";
    public string FullName { get; set; } = "";
    public int RoleId { get; set; }
    public string RoleName { get; set; } = "";
}

public sealed record AuthResult(AppUser? User, IReadOnlyCollection<string> Permissions, string? Error);

public sealed class DashboardStats
{
    public int TotalDevices { get; set; }
    public int AvailableUnits { get; set; }
    public int ActiveLoans { get; set; }
    public int OverdueLoans { get; set; }
}

public sealed class RecentSlip
{
    public string SlipCode { get; set; } = "";
    public string BorrowerName { get; set; } = "";
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public string Status { get; set; } = "";
}

public sealed class CategoryItem
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
}

public sealed class DeviceRow
{
    public int DeviceId { get; set; }
    public string DeviceCode { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = "";
    public int TotalQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public string Unit { get; set; } = "";
    public string Note { get; set; } = "";
    public int DamagedQuantity { get; set; }
    public int LostQuantity { get; set; }
    /// <summary>Số lượng đang được mượn (tính từ các phiếu chưa trả đủ).</summary>
    public int OnLoanQuantity { get; set; }
}
