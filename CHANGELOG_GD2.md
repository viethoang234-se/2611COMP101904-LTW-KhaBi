# GĐ2 - Danh mục và nghiệp vụ phiếu mượn

## Đã triển khai

- F06 - Loại thiết bị: danh sách/tìm kiếm, thêm, sửa, xóa; kiểm tra trùng mã/tên qua `CategoryService`; không xóa loại đã được thiết bị sử dụng.
- F08 - Người mượn: danh sách/tìm kiếm, lọc người ngừng hoạt động, thêm/sửa, ngừng hoạt động, xóa khi chưa có lịch sử; xem lịch sử phiếu mượn.
- F09 - Lập phiếu mượn: chọn người mượn, chọn nhiều thiết bị, cộng dồn số lượng cùng thiết bị, kiểm tra số lượng khả dụng trên giao diện; lưu qua `LoanService.Create` để dùng transaction và kiểm tra tồn kho phía database.
- F10 - Tra cứu phiếu mượn: tìm từ khóa, lọc ngày mượn, hạn trả và trạng thái.
- F11 - Chi tiết phiếu mượn: xem đầu phiếu, từng thiết bị, số lượng mượn/đã trả/còn lại và lịch sử biên bản trả.
- F07 - Chuyển trang danh sách và hộp thoại thiết bị từ gọi Repository trực tiếp sang gọi `DeviceService`.

## File chính được thêm

- `QuanLyMuonTra.App/Pages/PageUi.cs`
- `QuanLyMuonTra.App/Pages/CategoryPage.cs`
- `QuanLyMuonTra.App/Pages/BorrowerPage.cs`
- `QuanLyMuonTra.App/Pages/LoanCreatePage.cs`
- `QuanLyMuonTra.App/Pages/LoanSearchPage.cs`
- `QuanLyMuonTra.App/Forms/CategoryEditDialog.cs`
- `QuanLyMuonTra.App/Forms/BorrowerEditDialog.cs`
- `QuanLyMuonTra.App/Forms/BorrowerHistoryDialog.cs`
- `QuanLyMuonTra.App/Forms/LoanSlipDetailDialog.cs`

## Chưa xác nhận

- Chưa thể Build hoặc chạy ứng dụng trong môi trường hiện tại vì không có .NET SDK/Windows Forms.
- Chưa thực thi các script SQL trên SQL Server trong môi trường hiện tại.
- Cần kiểm tra giao diện, phân quyền, dữ liệu thực tế và xử lý các lỗi Build/runtime phát sinh trên máy Windows.
