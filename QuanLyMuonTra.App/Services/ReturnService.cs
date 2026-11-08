using QuanLyMuonTra.App.Data;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App.Services;

/// <summary>Nghiệp vụ trả thiết bị: F12 ghi nhận trả, F13 biên bản, hủy biên bản.</summary>
public static class ReturnService
{
    public sealed record Created(int ReturnRecordId, string RecordCode);

    public static string? Validate(NewReturnRecord r)
    {
        if (r.LoanSlipId <= 0) return "Vui lòng chọn phiếu mượn.";
        if (r.ReturnDate.Date > DateTime.Today) return "Ngày trả không được ở tương lai.";
        if (r.Items.Count == 0) return "Chưa nhập số lượng trả cho thiết bị nào.";
        if (r.GiverName is { Length: > 100 }) return "Tên người giao tối đa 100 ký tự.";
        if (r.Note is { Length: > 255 }) return "Ghi chú tối đa 255 ký tự.";

        foreach (var i in r.Items)
        {
            if (i.LoanSlipDetailId <= 0) return "Có dòng thiết bị không hợp lệ.";
            if (i.Quantity <= 0) return "Số lượng trả phải lớn hơn 0.";
            if (!Enum.IsDefined(i.Condition)) return "Tình trạng thiết bị không hợp lệ.";
            if (i.ConditionNote is { Length: > 100 }) return "Ghi chú tình trạng tối đa 100 ký tự.";
            if (i.Condition is ReturnCondition.Damaged or ReturnCondition.MissingAccessory
                && string.IsNullOrWhiteSpace(i.ConditionNote))
                return $"Vui lòng ghi chú chi tiết khi thiết bị ở tình trạng \"{LoanText.Condition((int)i.Condition)}\".";
        }
        return null;
    }

    public static Result<Created> Create(NewReturnRecord record) =>
        Svc.Run(new[] { Perm.ReturnCreate }, () =>
        {
            string? error = Validate(record);
            if (error != null) throw new BusinessException(error);

            record.GiverName = Svc.Clean(record.GiverName);
            record.Note = Svc.Clean(record.Note);
            foreach (var i in record.Items) i.ConditionNote = Svc.Clean(i.ConditionNote);
            // Chỉ chọn ngày -> lấy giờ hiện tại nếu là hôm nay, ngược lại đặt 12:00
            if (record.ReturnDate.TimeOfDay == TimeSpan.Zero)
                record.ReturnDate = record.ReturnDate.Date == DateTime.Today
                    ? DateTime.Now : record.ReturnDate.Date.AddHours(12);

            var (id, code) = ReturnRepository.Create(record, Svc.CurrentUserId);
            return new Created(id, code);
        });

    public static Result Cancel(int returnRecordId, string reason) =>
        Svc.Run(new[] { Perm.RecordCancel }, () =>
        {
            reason = (reason ?? "").Trim();
            if (reason.Length < 3) throw new BusinessException("Vui lòng nhập lý do hủy biên bản.");
            if (reason.Length > 255) throw new BusinessException("Lý do hủy tối đa 255 ký tự.");
            ReturnRepository.Cancel(returnRecordId, Svc.CurrentUserId, reason);
        });

    public static Result<List<ReturnRecordRow>> Search(RecordFilter filter) =>
        Svc.Run(new[] { Perm.RecordView }, () => ReturnRepository.Search(filter));

    public static Result<ReturnRecordFull> GetFull(int returnRecordId) =>
        Svc.Run(new[] { Perm.RecordView, Perm.ReportPrint }, () =>
            ReturnRepository.GetFull(returnRecordId) ?? throw new BusinessException("Biên bản không tồn tại."));
}
