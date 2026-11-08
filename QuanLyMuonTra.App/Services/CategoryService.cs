using QuanLyMuonTra.App.Data;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App.Services;

/// <summary>F06 - quản lý loại thiết bị. Nhân viên kho chỉ được xem.</summary>
public static class CategoryService
{
    public static Result<List<CategoryRow>> Search(string? keyword) =>
        Svc.Run(new[] { Perm.CategoryView, Perm.CategoryManage }, () => CategoryRepository.Search(keyword));

    public static Result Save(CategoryRow c) =>
        Svc.Run(new[] { Perm.CategoryManage }, () =>
        {
            c.CategoryCode = (c.CategoryCode ?? "").Trim().ToUpperInvariant();
            c.CategoryName = (c.CategoryName ?? "").Trim();
            c.Description = (c.Description ?? "").Trim();

            if (c.CategoryCode.Length == 0) throw new BusinessException("Vui lòng nhập mã loại thiết bị.");
            if (c.CategoryCode.Length > 20) throw new BusinessException("Mã loại tối đa 20 ký tự.");
            if (c.CategoryCode.Any(ch => !(char.IsAsciiLetterOrDigit(ch) || ch is '-' or '_')))
                throw new BusinessException("Mã loại chỉ gồm chữ không dấu, số, dấu gạch ngang hoặc gạch dưới.");
            if (c.CategoryName.Length == 0) throw new BusinessException("Vui lòng nhập tên loại thiết bị.");
            if (c.CategoryName.Length > 100) throw new BusinessException("Tên loại tối đa 100 ký tự.");
            if (c.Description.Length > 255) throw new BusinessException("Mô tả tối đa 255 ký tự.");

            string? duplicate = CategoryRepository.FindDuplicate(c.CategoryId, c.CategoryCode, c.CategoryName);
            if (duplicate != null) throw new BusinessException(duplicate);

            if (c.CategoryId == 0) CategoryRepository.Insert(c);
            else if (!CategoryRepository.Update(c)) throw new BusinessException("Loại thiết bị không tồn tại.");
        });

    public static Result Delete(int categoryId) =>
        Svc.Run(new[] { Perm.CategoryManage }, () =>
        {
            if (!CategoryRepository.DeleteIfUnused(categoryId))
                throw new BusinessException("Loại thiết bị đã có thiết bị thuộc loại này nên không thể xóa.");
        });
}
