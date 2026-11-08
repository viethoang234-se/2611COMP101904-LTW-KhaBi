using QuanLyMuonTra.App.Data;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App.Services;

/// <summary>Nghiệp vụ phiếu mượn: F09 lập phiếu, F10 tra cứu, F11 chi tiết.</summary>
public static class LoanService
{
    public sealed record Created(int LoanSlipId, string SlipCode);

    /// <summary>Kiểm tra dữ liệu phiếu mượn trước khi mở transaction. Trả về thông báo lỗi hoặc null nếu hợp lệ.</summary>
    public static string? Validate(NewLoanSlip s)
    {
        if (s.BorrowerId <= 0) return "Vui lòng chọn người mượn.";
        if (s.BorrowDate.Date > DateTime.Today) return "Ngày mượn không được ở tương lai.";
        if (s.DueDate.Date < s.BorrowDate.Date) return "Ngày dự kiến trả không được trước ngày mượn.";
        if (s.Note is { Length: > 255 }) return "Ghi chú tối đa 255 ký tự.";
        if (s.Items.Count == 0) return "Phiếu mượn phải có ít nhất một thiết bị.";
        if (s.Items.Any(i => i.DeviceId <= 0)) return "Có dòng thiết bị không hợp lệ.";
        if (s.Items.Any(i => i.Quantity <= 0)) return "Số lượng mượn của mỗi thiết bị phải lớn hơn 0.";
        return null;
    }

    public static Result<Created> Create(NewLoanSlip slip) =>
        Svc.Run(new[] { Perm.LoanCreate }, () =>
        {
            string? error = Validate(slip);
            if (error != null) throw new BusinessException(error);
            slip.Note = Svc.Clean(slip.Note);
            var (id, code) = LoanRepository.Create(slip, Svc.CurrentUserId);
            return new Created(id, code);
        });

    public static Result<List<LoanSlipRow>> Search(LoanFilter filter) =>
        Svc.Run(new[] { Perm.LoanView }, () => LoanRepository.Search(filter));

    /// <summary>Phiếu còn thiết bị chưa trả, dùng cho màn hình ghi nhận trả (F12).</summary>
    public static Result<List<LoanSlipRow>> SearchOpen(string? keyword) =>
        Svc.Run(new[] { Perm.ReturnCreate, Perm.LoanView }, () => LoanRepository.SearchOpen(keyword));

    public static Result<LoanSlipFull> GetFull(int loanSlipId) =>
        Svc.Run(new[] { Perm.LoanView, Perm.ReturnCreate, Perm.ReportPrint }, () =>
            LoanRepository.GetFull(loanSlipId) ?? throw new BusinessException("Phiếu mượn không tồn tại."));

    public static Result<LoanSlipFull> GetFullByCode(string slipCode) =>
        Svc.Run(new[] { Perm.LoanView, Perm.ReturnCreate }, () =>
        {
            var header = LoanRepository.GetByCode(slipCode.Trim()) ?? throw new BusinessException("Không tìm thấy phiếu mượn.");
            return LoanRepository.GetFull(header.LoanSlipId) ?? throw new BusinessException("Phiếu mượn không tồn tại.");
        });
}
