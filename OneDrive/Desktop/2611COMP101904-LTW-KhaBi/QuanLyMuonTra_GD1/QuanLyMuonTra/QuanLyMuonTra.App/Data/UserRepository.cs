using Dapper;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App.Data;

public static class UserRepository
{
    sealed class UserRow
    {
        public int UserId { get; set; }
        public string Username { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string FullName { get; set; } = "";
        public int RoleId { get; set; }
        public string RoleName { get; set; } = "";
        public bool IsActive { get; set; }
    }

    public static AuthResult Authenticate(string username, string password)
    {
        using var conn = Db.Open();

        var row = conn.QuerySingleOrDefault<UserRow>(
            @"SELECT u.UserId, u.Username, u.PasswordHash, u.FullName, u.RoleId, r.RoleName, u.IsActive
              FROM Users u
              JOIN Roles r ON r.RoleId = u.RoleId
              WHERE u.Username = @username",
            new { username });

        if (row is null || !PasswordHasher.Verify(password, row.PasswordHash))
            return new AuthResult(null, Array.Empty<string>(), "Sai tên đăng nhập hoặc mật khẩu.");

        if (!row.IsActive)
            return new AuthResult(null, Array.Empty<string>(), "Tài khoản đã bị khóa. Vui lòng liên hệ người quản lý.");

        var permissions = conn.Query<string>(
            @"SELECT p.Code
              FROM RolePermissions rp
              JOIN Permissions p ON p.PermissionId = rp.PermissionId
              WHERE rp.RoleId = @roleId",
            new { roleId = row.RoleId }).ToList();

        var user = new AppUser
        {
            UserId = row.UserId,
            Username = row.Username,
            FullName = row.FullName,
            RoleId = row.RoleId,
            RoleName = row.RoleName,
        };
        return new AuthResult(user, permissions, null);
    }
}
