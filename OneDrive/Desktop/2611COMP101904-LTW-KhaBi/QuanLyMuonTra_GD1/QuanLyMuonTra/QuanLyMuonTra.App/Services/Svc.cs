using QuanLyMuonTra.App.Data;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App.Services;

/// <summary>
/// Tiện ích chung của tầng BLL: kiểm tra quyền (không tin giao diện), bắt lỗi và đổi thành thông báo tiếng Việt.
/// Giao diện chỉ nhận về Result, không phải tự xử lý ngoại lệ SQL.
/// </summary>
internal static class Svc
{
    public const string Denied = "Bạn không có quyền thực hiện chức năng này.";

    static bool Allowed(string[] permissions) =>
        permissions.Length == 0 || permissions.Any(Session.Has);

    /// <summary>Chạy nghiệp vụ nếu người dùng có ÍT NHẤT MỘT trong các quyền (rỗng = chỉ cần đã đăng nhập).</summary>
    public static Result<T> Run<T>(string[] permissions, Func<T> work)
    {
        if (Session.CurrentUser is null) return Result<T>.Fail("Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.");
        if (!Allowed(permissions)) return Result<T>.Fail(Denied);
        try { return Result<T>.Ok(work()); }
        catch (Exception ex) { return Result<T>.Fail(Db.Describe(ex)); }
    }

    public static Result Run(string[] permissions, Action work)
    {
        if (Session.CurrentUser is null) return Result.Fail("Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.");
        if (!Allowed(permissions)) return Result.Fail(Denied);
        try { work(); return Result.Ok(); }
        catch (Exception ex) { return Result.Fail(Db.Describe(ex)); }
    }

    public static int CurrentUserId => Session.CurrentUser?.UserId ?? 0;

    public static string? Clean(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();
}
