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

## GĐ3 - Trả thiết bị, biên bản và quá hạn

### Đã triển khai trong bản GĐ3

- F12 - Ghi nhận trả: lọc phiếu còn số lượng chưa trả; hiển thị chi tiết từng thiết bị; nhập số lượng trả, tình trạng (tốt/hư hỏng/mất/thiếu phụ kiện) và ghi chú tình trạng; lưu qua `ReturnService.Create` để dùng transaction có sẵn.
- F13 - Biên bản trả/bàn giao: tìm kiếm theo mã biên bản, mã phiếu, người mượn; lọc ngày và trạng thái; xem chi tiết; hủy biên bản có yêu cầu lý do và xác nhận, chỉ bật nút hủy khi tài khoản có quyền `RECORD_CANCEL`.
- F14 - Phiếu quá hạn: gọi stored procedure `sp_GetOverdueSlips`, tìm kiếm và lọc số ngày quá hạn tối thiểu.
- Thay 3 trang Placeholder F12, F13, F14 trong `NavCatalog` bằng trang chức năng.

### Lưu ý kiểm thử

- Chưa thể Build hoặc chạy WinForms trong môi trường hiện tại vì không có .NET SDK/Windows.
- Chưa chạy các luồng ghi nhận trả/hủy biên bản trên SQL Server. Cần kiểm thử trên database mẫu, đặc biệt các tình huống trả một phần, trả nhiều tình trạng cho cùng thiết bị, trả vượt số còn lại, hủy biên bản sau khi thiết bị đã được mượn lại/sửa chữa.

## GĐ4 - Quản trị, báo cáo, thống kê và in ấn

### Đã triển khai trong bản GĐ4

- F04 - Quản lý tài khoản: danh sách/tìm kiếm, tạo tài khoản, sửa thông tin và nhóm quyền, khóa/mở khóa, đặt lại mật khẩu. Mật khẩu mới được băm PBKDF2 qua `PasswordHasher`; không lưu mật khẩu thuần.
- F05 - Nhóm quyền: tạo/sửa tên nhóm, xem danh sách quyền và gán quyền chức năng cho nhóm trong transaction.
- F15 - Báo cáo: tổng hợp phiếu mượn, số lượng mượn/đã trả/còn phải trả, phiếu quá hạn và tồn kho hiện tại theo khoảng ngày. Tài khoản chỉ có quyền in không được xem số liệu báo cáo.
- F18 - Thống kê thiết bị mượn nhiều: xếp hạng theo tổng số lượng mượn, kèm số phiếu mượn và bộ lọc thời gian.
- F16 - Cấu hình mã tự động: chỉnh tiền tố, chèn ngày, độ dài số và đặt lại bộ đếm hằng ngày; có chức năng xem mã tiếp theo. Chặn cấu hình reset mỗi ngày nếu mã không chứa ngày để tránh sinh mã trùng.
- F15 - In ấn: thêm xem trước/in phiếu mượn và biên bản trả bằng `PrintDocument`, có phần ký xác nhận.
- Thay các trang Placeholder F04, F05, F15, F16, F18 bằng trang chức năng.

### Lưu ý kiểm thử

- Chưa thể Build hoặc chạy WinForms trong môi trường hiện tại vì không có .NET SDK/Windows.
- Chưa thực thi các truy vấn mới trên SQL Server. Cần kiểm thử quyền truy cập theo hai tài khoản mẫu, thao tác phân quyền, sinh mã sau khi thay cấu hình và in/xem trước trên máy Windows.
