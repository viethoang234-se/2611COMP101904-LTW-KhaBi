using QuanLyMuonTra.App.Pages;
using QuanLyMuonTra.App.Security;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App;

public sealed record NavItem(string Code, string Title, string Group, string Icon, string Permission, Func<UserControl> Create);

/// <summary>
/// Danh sách menu bên trái. Mỗi mục có mã quyền: người dùng không có quyền đó sẽ không thấy mục này.
/// Khi làm xong một form mới, chỉ cần thay PlaceholderPage bằng trang thật ở cột cuối.
/// </summary>
public static class NavCatalog
{
    static UserControl Todo(string code, string title, string note) => new PlaceholderPage(code, title, note);

    public static readonly IReadOnlyList<NavItem> Items = new List<NavItem>
    {
        new("F02", "Tổng quan", "Tổng quan", Icons.Home, Perm.DashboardView, () => new DashboardPage()),

        new("F07", "Quản lý thiết bị", "Danh mục", Icons.Device, Perm.DeviceManage, () => new DevicePage()),
        new("F06", "Loại thiết bị", "Danh mục", Icons.Tag, Perm.CategoryView,
            () => new CategoryPage()),
        new("F08", "Người mượn", "Danh mục", Icons.Contact, Perm.BorrowerManage,
            () => new BorrowerPage()),

        new("F09", "Lập phiếu mượn", "Mượn trả", Icons.Add, Perm.LoanCreate,
            () => new LoanCreatePage()),
        new("F10", "Tra cứu phiếu mượn", "Mượn trả", Icons.Search, Perm.LoanView,
            () => new LoanSearchPage()),
        new("F12", "Ghi nhận trả", "Mượn trả", Icons.Return, Perm.ReturnCreate,
            () => new ReturnCreatePage()),
        new("F13", "Biên bản trả / bàn giao", "Mượn trả", Icons.Document, Perm.RecordView,
            () => new ReturnRecordPage()),
        new("F14", "Phiếu quá hạn", "Mượn trả", Icons.Warning, Perm.OverdueView,
            () => new OverduePage()),

        new("F15", "Báo cáo & in ấn", "Báo cáo", Icons.Print, Perm.ReportPrint,
            () => new ReportPage()),
        new("F18", "Thiết bị mượn nhiều", "Báo cáo", Icons.Chart, Perm.StatsView,
            () => new PopularDevicePage()),

        new("F04", "Người dùng", "Hệ thống", Icons.Users, Perm.UserManage,
            () => new UserPage()),
        new("F05", "Nhóm quyền", "Hệ thống", Icons.Lock, Perm.RoleManage,
            () => new RolePage()),
        new("F16", "Cấu hình mã tự động", "Hệ thống", Icons.Settings, Perm.CodeConfig,
            () => new CodeConfigPage()),
    };
}
