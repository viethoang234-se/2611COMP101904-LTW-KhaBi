using System.Text.RegularExpressions;
using QuanLyMuonTra.App.Data;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App.Services;

/// <summary>F08 - quản lý người mượn.</summary>
public static partial class BorrowerService
{
    [GeneratedRegex(@"^\+?[0-9][0-9 .\-()]{7,18}$")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$")]
    private static partial Regex EmailRegex();

    static readonly string[] Read = { Perm.BorrowerManage, Perm.LoanCreate };
    static readonly string[] Write = { Perm.BorrowerManage };

    public static string? Validate(BorrowerRow b)
    {
        if (string.IsNullOrWhiteSpace(b.FullName)) return "Vui lòng nhập họ tên người mượn.";
        if (b.FullName.Trim().Length > 100) return "Họ tên tối đa 100 ký tự.";
        if (!string.IsNullOrWhiteSpace(b.Phone) && !PhoneRegex().IsMatch(b.Phone.Trim())) return "Số điện thoại không hợp lệ.";
        if (!string.IsNullOrWhiteSpace(b.Email) && (b.Email.Trim().Length > 100 || !EmailRegex().IsMatch(b.Email.Trim()))) return "Email không hợp lệ.";
        if (b.Department is { Length: > 100 }) return "Đơn vị / phòng ban tối đa 100 ký tự.";
        if (b.Address is { Length: > 200 }) return "Địa chỉ tối đa 200 ký tự.";
        if (b.Note is { Length: > 255 }) return "Ghi chú tối đa 255 ký tự.";
        return null;
    }

    static void Normalize(BorrowerRow b)
    {
        b.FullName = b.FullName.Trim();
        b.Phone = (Svc.Clean(b.Phone) ?? "");
        b.Email = (Svc.Clean(b.Email) ?? "");
        b.Department = (Svc.Clean(b.Department) ?? "");
        b.Address = (Svc.Clean(b.Address) ?? "");
        b.Note = (Svc.Clean(b.Note) ?? "");
    }

    public static Result<List<BorrowerRow>> Search(string? keyword, bool includeInactive = false) =>
        Svc.Run(Read, () => BorrowerRepository.Search(keyword, includeInactive));

    public static Result<BorrowerRow> Get(int borrowerId) =>
        Svc.Run(Read, () => BorrowerRepository.Get(borrowerId) ?? throw new BusinessException("Người mượn không tồn tại."));

    public static Result<string> SuggestCode() => Svc.Run(Write, BorrowerRepository.SuggestCode);

    /// <summary>Thêm mới (BorrowerId = 0, mã tự động) hoặc cập nhật. Trả về mã người mượn.</summary>
    public static Result<string> Save(BorrowerRow b) =>
        Svc.Run(Write, () =>
        {
            string? error = Validate(b);
            if (error != null) throw new BusinessException(error);
            Normalize(b);

            if (b.BorrowerId == 0) return BorrowerRepository.Insert(b);

            if (!BorrowerRepository.Update(b)) throw new BusinessException("Người mượn không tồn tại.");
            return b.BorrowerCode;
        });

    public static Result Delete(int borrowerId) =>
        Svc.Run(Write, () =>
        {
            if (!BorrowerRepository.DeleteIfNoHistory(borrowerId))
                throw new BusinessException(
                    "Người mượn đã có lịch sử giao dịch nên không thể xóa. Hãy chuyển sang trạng thái ngừng hoạt động.");
        });

    /// <summary>Lịch sử mượn trả của một người mượn.</summary>
    public static Result<List<LoanSlipRow>> History(int borrowerId) =>
        Svc.Run(new[] { Perm.BorrowerManage, Perm.LoanView }, () =>
            LoanRepository.Search(new LoanFilter { BorrowerId = borrowerId }));
}
