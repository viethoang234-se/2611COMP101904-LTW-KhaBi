using QuanLyMuonTra.App.Models;

namespace QuanLyMuonTra.App.Security;

/// <summary>Phiên đăng nhập hiện tại: người dùng + tập quyền.</summary>
public static class Session
{
    static HashSet<string> _permissions = new(StringComparer.OrdinalIgnoreCase);

    public static AppUser? CurrentUser { get; private set; }

    public static void Start(AppUser user, IEnumerable<string> permissions)
    {
        CurrentUser = user;
        _permissions = new HashSet<string>(permissions, StringComparer.OrdinalIgnoreCase);
    }

    public static void End()
    {
        CurrentUser = null;
        _permissions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }

    public static bool Has(string permissionCode) => _permissions.Contains(permissionCode);
}
