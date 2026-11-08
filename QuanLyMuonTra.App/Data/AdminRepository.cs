using Dapper;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App.Data;

/// <summary>DAL cho quản trị tài khoản, vai trò, cấu hình mã và báo cáo.</summary>
public static class AdminRepository
{
    public static List<UserAdminRow> Users(string? keyword)
    {
        string? like = string.IsNullOrWhiteSpace(keyword) ? null : $"%{keyword.Trim()}%";
        using var c = Db.Open();
        return c.Query<UserAdminRow>(@"SELECT u.UserId,u.Username,u.FullName,ISNULL(u.Email,'') Email,ISNULL(u.Phone,'') Phone,
            u.RoleId,r.RoleName,u.IsActive FROM Users u JOIN Roles r ON r.RoleId=u.RoleId
            WHERE @like IS NULL OR u.Username LIKE @like OR u.FullName LIKE @like OR u.Email LIKE @like ORDER BY u.FullName", new { like }).ToList();
    }

    public static void CreateUser(string username, string password, string fullName, string? email, string? phone, int roleId)
    {
        using var c = Db.Open();
        c.Execute(@"INSERT INTO Users(Username,PasswordHash,FullName,Email,Phone,RoleId,IsActive)
                    VALUES(@username,@hash,@fullName,@email,@phone,@roleId,1)",
            new { username, hash = PasswordHasher.Hash(password), fullName, email, phone, roleId });
    }

    public static void UpdateUser(int id, string fullName, string? email, string? phone, int roleId)
    {
        using var c = Db.Open();
        int rows = c.Execute("UPDATE Users SET FullName=@fullName,Email=@email,Phone=@phone,RoleId=@roleId WHERE UserId=@id",
            new { id, fullName, email, phone, roleId });
        if (rows == 0) throw new BusinessException("Tài khoản không tồn tại.");
    }

    public static void SetUserActive(int id, bool active, int currentUserId)
    {
        if (id == currentUserId && !active) throw new BusinessException("Không thể tự khóa tài khoản đang đăng nhập.");
        using var c = Db.Open();
        int rows = c.Execute("UPDATE Users SET IsActive=@active WHERE UserId=@id", new { id, active });
        if (rows == 0) throw new BusinessException("Tài khoản không tồn tại.");
    }

    public static void ResetPassword(int id, string password)
    {
        using var c = Db.Open();
        int rows = c.Execute("UPDATE Users SET PasswordHash=@hash WHERE UserId=@id", new { id, hash = PasswordHasher.Hash(password) });
        if (rows == 0) throw new BusinessException("Tài khoản không tồn tại.");
    }

    public static List<RoleRow> Roles()
    {
        using var c = Db.Open();
        return c.Query<RoleRow>(@"SELECT r.RoleId,r.RoleName,ISNULL(r.Description,'') Description,COUNT(u.UserId) UserCount
            FROM Roles r LEFT JOIN Users u ON u.RoleId=r.RoleId GROUP BY r.RoleId,r.RoleName,r.Description ORDER BY r.RoleName").ToList();
    }

    public static List<PermissionRow> Permissions()
    {
        using var c = Db.Open(); return c.Query<PermissionRow>("SELECT PermissionId,Code,Name FROM Permissions ORDER BY Code").ToList();
    }

    public static HashSet<int> RolePermissionIds(int roleId)
    {
        using var c = Db.Open(); return c.Query<int>("SELECT PermissionId FROM RolePermissions WHERE RoleId=@roleId", new { roleId }).ToHashSet();
    }

    public static void SaveRole(int? roleId, string name, string? description)
    {
        using var c = Db.Open();
        if (roleId is null) c.Execute("INSERT INTO Roles(RoleName,Description) VALUES(@name,@description)", new { name, description });
        else if (c.Execute("UPDATE Roles SET RoleName=@name,Description=@description WHERE RoleId=@roleId", new { name, description, roleId }) == 0)
            throw new BusinessException("Nhóm quyền không tồn tại.");
    }

    public static void SaveRolePermissions(int roleId, int[] permissionIds)
    {
        Db.InTransaction((c, tx) =>
        {
            if (c.ExecuteScalar<int>("SELECT COUNT(*) FROM Roles WHERE RoleId=@roleId", new { roleId }, tx) == 0)
                throw new BusinessException("Nhóm quyền không tồn tại.");
            c.Execute("DELETE FROM RolePermissions WHERE RoleId=@roleId", new { roleId }, tx);
            foreach (int id in permissionIds.Distinct())
                c.Execute("INSERT INTO RolePermissions(RoleId,PermissionId) VALUES(@roleId,@id)", new { roleId, id }, tx);
            return 0;
        });
    }

    public static List<CodeConfigRow> CodeConfigs()
    {
        using var c = Db.Open();
        return c.Query<CodeConfigRow>("SELECT ConfigId,EntityName,Prefix,UseDate,NumberLength,CurrentNumber,ResetDaily,LastResetDate FROM CodeConfigs ORDER BY EntityName").ToList();
    }

    public static string PreviewCode(string entityName) => CodeRepository.Peek(entityName);

    public static void SaveCodeConfig(CodeConfigRow x)
    {
        using var c = Db.Open();
        int rows = c.Execute(@"UPDATE CodeConfigs SET Prefix=@Prefix,UseDate=@UseDate,NumberLength=@NumberLength,ResetDaily=@ResetDaily
            WHERE ConfigId=@ConfigId", x);
        if (rows == 0) throw new BusinessException("Cấu hình mã không tồn tại.");
    }

    public static ReportSummary Summary(DateTime from, DateTime to)
    {
        using var c = Db.Open();
        return c.QuerySingle<ReportSummary>(@"SELECT
            (SELECT COUNT(*) FROM LoanSlips WHERE BorrowDate>=@from AND BorrowDate<DATEADD(DAY,1,@to)) SlipCount,
            (SELECT ISNULL(SUM(d.Quantity),0) FROM LoanSlipDetails d JOIN LoanSlips s ON s.LoanSlipId=d.LoanSlipId WHERE s.BorrowDate>=@from AND s.BorrowDate<DATEADD(DAY,1,@to)) BorrowedQuantity,
            (SELECT ISNULL(SUM(d.ReturnedQuantity),0) FROM LoanSlipDetails d JOIN LoanSlips s ON s.LoanSlipId=d.LoanSlipId WHERE s.BorrowDate>=@from AND s.BorrowDate<DATEADD(DAY,1,@to)) ReturnedQuantity,
            (SELECT ISNULL(SUM(d.Quantity-d.ReturnedQuantity),0) FROM LoanSlipDetails d JOIN LoanSlips s ON s.LoanSlipId=d.LoanSlipId WHERE s.BorrowDate>=@from AND s.BorrowDate<DATEADD(DAY,1,@to)) OutstandingQuantity,
            (SELECT COUNT(*) FROM vw_LoanSlipSummary WHERE DisplayStatus=3 AND BorrowDate>=@from AND BorrowDate<DATEADD(DAY,1,@to)) OverdueSlipCount,
            (SELECT COUNT(*) FROM Devices WHERE IsActive=1) ActiveDeviceCount,
            (SELECT ISNULL(SUM(AvailableQuantity),0) FROM Devices WHERE IsActive=1) AvailableQuantity,
            (SELECT ISNULL(SUM(DamagedQuantity),0) FROM Devices WHERE IsActive=1) DamagedQuantity,
            (SELECT ISNULL(SUM(LostQuantity),0) FROM Devices WHERE IsActive=1) LostQuantity", new { from = from.Date, to = to.Date });
    }

    public static List<PopularDeviceRow> PopularDevices(DateTime? from, DateTime? to, int top)
    {
        using var c = Db.Open();
        return c.Query<PopularDeviceRow>(@"SELECT TOP (@top) dv.DeviceCode,dv.DeviceName,cat.CategoryName,
            COUNT(DISTINCT s.LoanSlipId) LoanCount,SUM(d.Quantity) BorrowedQuantity
            FROM LoanSlipDetails d JOIN LoanSlips s ON s.LoanSlipId=d.LoanSlipId
            JOIN Devices dv ON dv.DeviceId=d.DeviceId JOIN DeviceCategories cat ON cat.CategoryId=dv.CategoryId
            WHERE (@from IS NULL OR s.BorrowDate>=@from) AND (@to IS NULL OR s.BorrowDate<DATEADD(DAY,1,@to))
            GROUP BY dv.DeviceCode,dv.DeviceName,cat.CategoryName ORDER BY SUM(d.Quantity) DESC,COUNT(DISTINCT s.LoanSlipId) DESC,dv.DeviceName",
            new { from = from?.Date, to = to?.Date, top }).ToList();
    }
}
