using QuanLyMuonTra.App.Data;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App.Services;

/// <summary>F07 - quản lý thiết bị (kiểm tra quyền và dữ liệu ở tầng nghiệp vụ).</summary>
public static class DeviceService
{
    static readonly string[] Manage = { Perm.DeviceManage };

    public static Result<List<CategoryItem>> Categories() =>
        Svc.Run(new[] { Perm.DeviceManage, Perm.CategoryView, Perm.LoanCreate }, DeviceRepository.GetCategories);

    public static Result<List<DeviceRow>> Search(string? keyword, int? categoryId, DeviceStockFilter stock = DeviceStockFilter.All) =>
        Svc.Run(new[] { Perm.DeviceManage, Perm.LoanCreate }, () => DeviceRepository.Search(keyword, categoryId, stock));

    /// <summary>Thiết bị còn khả dụng để chọn khi lập phiếu mượn.</summary>
    public static Result<List<DeviceRow>> SearchAvailable(string? keyword) =>
        Svc.Run(new[] { Perm.LoanCreate }, () => DeviceRepository.SearchAvailable(keyword));

    public static Result<string> SuggestCode() => Svc.Run(Manage, DeviceRepository.SuggestCode);

    public static Result Save(DeviceRow d) =>
        Svc.Run(Manage, () =>
        {
            d.DeviceCode = (d.DeviceCode ?? "").Trim();
            d.DeviceName = (d.DeviceName ?? "").Trim();
            d.Unit = (d.Unit ?? "").Trim();
            d.Note = (d.Note ?? "").Trim();

            if (d.DeviceCode.Length == 0) throw new BusinessException("Vui lòng nhập mã thiết bị.");
            if (d.DeviceCode.Length > 30) throw new BusinessException("Mã thiết bị tối đa 30 ký tự.");
            if (d.DeviceName.Length == 0) throw new BusinessException("Vui lòng nhập tên thiết bị.");
            if (d.DeviceName.Length > 150) throw new BusinessException("Tên thiết bị tối đa 150 ký tự.");
            if (d.CategoryId <= 0) throw new BusinessException("Vui lòng chọn loại thiết bị.");
            if (d.TotalQuantity < 0 || d.TotalQuantity > 100000) throw new BusinessException("Tổng số lượng không hợp lệ.");
            if (d.Unit.Length > 30) throw new BusinessException("Đơn vị tính tối đa 30 ký tự.");
            if (d.Note.Length > 255) throw new BusinessException("Ghi chú tối đa 255 ký tự.");

            string? error = d.DeviceId == 0 ? DeviceRepository.Insert(d) : DeviceRepository.Update(d);
            if (error != null) throw new BusinessException(error);
        });

    public static Result Delete(int deviceId) =>
        Svc.Run(Manage, () =>
        {
            string? error = DeviceRepository.Delete(deviceId);
            if (error != null) throw new BusinessException(error);
        });

    /// <summary>Thiết bị hư hỏng đã sửa xong: chuyển về khả dụng.</summary>
    public static Result RecoverDamaged(int deviceId, int quantity) =>
        Svc.Run(Manage, () =>
        {
            if (quantity <= 0) throw new BusinessException("Số lượng phải lớn hơn 0.");
            if (!DeviceRepository.RecoverDamaged(deviceId, quantity))
                throw new BusinessException("Số lượng thiết bị hư hỏng hiện có không đủ.");
        });
}
