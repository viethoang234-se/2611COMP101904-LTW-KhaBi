using QuanLyMuonTra.App.Data;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App.Services;

/// <summary>BLL cho các chức năng quản trị và báo cáo F04/F05/F15/F16/F18.</summary>
public static class AdminService
{
    static readonly System.Text.RegularExpressions.Regex EmailRx = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    static readonly System.Text.RegularExpressions.Regex PhoneRx = new(@"^[0-9+\-\s().]{8,20}$");
    static readonly System.Text.RegularExpressions.Regex UserRx = new(@"^[A-Za-z0-9._-]+$");

    static void CheckContact(string? email, string? phone, int roleId)
    {
        email = Svc.Clean(email); phone = Svc.Clean(phone);
        if (email != null && (email.Length > 100 || !EmailRx.IsMatch(email))) throw new BusinessException("Email không hợp lệ.");
        if (phone != null && !PhoneRx.IsMatch(phone)) throw new BusinessException("Số điện thoại không hợp lệ (8-20 ký tự, chỉ gồm chữ số và + - ( ) .).");
        if (roleId <= 0) throw new BusinessException("Vui lòng chọn nhóm quyền.");
    }

    public static Result<List<UserAdminRow>> Users(string? keyword) =>
        Svc.Run(new[] { Perm.UserManage }, () => AdminRepository.Users(keyword));

    public static Result CreateUser(string username, string password, string fullName, string? email, string? phone, int roleId) =>
        Svc.Run(new[] { Perm.UserManage }, () =>
        {
            username = (username ?? "").Trim(); fullName = (fullName ?? "").Trim(); password ??= "";
            if (username.Length < 3 || username.Length > 50) throw new BusinessException("Tên đăng nhập phải từ 3 đến 50 ký tự.");
            if (!UserRx.IsMatch(username)) throw new BusinessException("Tên đăng nhập chỉ gồm chữ không dấu, số, dấu chấm, gạch ngang hoặc gạch dưới.");
            if (password.Length < 8 || password.Length > 100) throw new BusinessException("Mật khẩu phải từ 8 đến 100 ký tự.");
            if (fullName.Length is < 2 or > 100) throw new BusinessException("Họ tên phải từ 2 đến 100 ký tự.");
            CheckContact(email, phone, roleId);
            AdminRepository.CreateUser(username, password, fullName, Svc.Clean(email), Svc.Clean(phone), roleId);
        });

    public static Result UpdateUser(int id, string fullName, string? email, string? phone, int roleId) =>
        Svc.Run(new[] { Perm.UserManage }, () =>
        {
            fullName = (fullName ?? "").Trim();
            if (fullName.Length is < 2 or > 100) throw new BusinessException("Họ tên phải từ 2 đến 100 ký tự.");
            CheckContact(email, phone, roleId);
            AdminRepository.UpdateUser(id, fullName, Svc.Clean(email), Svc.Clean(phone), roleId);
        });

    public static Result SetUserActive(int id, bool active) =>
        Svc.Run(new[] { Perm.UserManage }, () => AdminRepository.SetUserActive(id, active, Svc.CurrentUserId));

    public static Result ResetPassword(int id, string password) =>
        Svc.Run(new[] { Perm.UserManage }, () =>
        {
            if (password == null || password.Length < 8 || password.Length > 100) throw new BusinessException("Mật khẩu mới phải từ 8 đến 100 ký tự.");
            AdminRepository.ResetPassword(id, password);
        });

    public static Result<List<RoleRow>> Roles() => Svc.Run(new[] { Perm.RoleManage, Perm.UserManage }, AdminRepository.Roles);
    public static Result<List<PermissionRow>> Permissions() => Svc.Run(new[] { Perm.RoleManage }, AdminRepository.Permissions);
    public static Result<HashSet<int>> RolePermissionIds(int roleId) => Svc.Run(new[] { Perm.RoleManage }, () => AdminRepository.RolePermissionIds(roleId));

    public static Result SaveRole(int? id, string name, string? description) =>
        Svc.Run(new[] { Perm.RoleManage }, () =>
        {
            name = (name ?? "").Trim();
            if (name.Length is < 2 or > 50) throw new BusinessException("Tên nhóm quyền phải từ 2 đến 50 ký tự.");
            if (description?.Length > 200) throw new BusinessException("Mô tả tối đa 200 ký tự.");
            AdminRepository.SaveRole(id, name, Svc.Clean(description));
        });

    public static Result SaveRolePermissions(int roleId, int[] ids) =>
        Svc.Run(new[] { Perm.RoleManage }, () =>
        {
            ids ??= Array.Empty<int>();
            // Chặn tự khóa quyền quản trị: không bỏ quyền "Quản lý nhóm quyền" khỏi nhóm của chính mình.
            if (Session.CurrentUser is { } me && me.RoleId == roleId)
            {
                var roleManageId = AdminRepository.Permissions().FirstOrDefault(x => x.Code == Perm.RoleManage)?.PermissionId;
                if (roleManageId.HasValue && !ids.Contains(roleManageId.Value))
                    throw new BusinessException("Không thể bỏ quyền quản lý nhóm quyền khỏi nhóm của chính bạn (sẽ bị khóa chức năng này).");
            }
            AdminRepository.SaveRolePermissions(roleId, ids.Distinct().ToArray());
        });

    public static Result<List<CodeConfigRow>> CodeConfigs() => Svc.Run(new[] { Perm.CodeConfig }, AdminRepository.CodeConfigs);
    public static Result<string> PreviewCode(string entityName) => Svc.Run(new[] { Perm.CodeConfig }, () => AdminRepository.PreviewCode(entityName));

    public static Result SaveCodeConfig(CodeConfigRow x) =>
        Svc.Run(new[] { Perm.CodeConfig }, () =>
        {
            x.Prefix = (x.Prefix ?? "").Trim();
            if (x.Prefix.Length is < 1 or > 10 || !x.Prefix.All(char.IsAsciiLetterOrDigit))
                throw new BusinessException("Tiền tố chỉ gồm 1-10 chữ cái hoặc chữ số.");
            if (x.NumberLength is < 1 or > 9) throw new BusinessException("Độ dài phần số phải từ 1 đến 9.");
            if (!x.UseDate && x.ResetDaily) throw new BusinessException("Không thể đặt lại số mỗi ngày khi mã không chứa ngày, vì có thể tạo mã trùng.");
            AdminRepository.SaveCodeConfig(x);
        });

    public static Result<ReportSummary> Summary(DateTime from, DateTime to) =>
        Svc.Run(new[] { Perm.ReportView }, () =>
        {
            if (from.Date > to.Date) throw new BusinessException("Ngày bắt đầu không được lớn hơn ngày kết thúc.");
            return AdminRepository.Summary(from, to);
        });

    public static Result<List<PopularDeviceRow>> PopularDevices(DateTime? from, DateTime? to, int top) =>
        Svc.Run(new[] { Perm.StatsView, Perm.ReportView }, () =>
        {
            if (from.HasValue && to.HasValue && from.Value.Date > to.Value.Date) throw new BusinessException("Khoảng ngày không hợp lệ.");
            if (top is < 1 or > 100) throw new BusinessException("Số lượng top phải từ 1 đến 100.");
            return AdminRepository.PopularDevices(from, to, top);
        });
}
