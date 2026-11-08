# DANH SÁCH CA KIỂM THỬ - QUẢN LÝ MƯỢN TRẢ THIẾT BỊ

Điền cột **Kết quả thực tế** và **Trạng thái** sau khi chạy ứng dụng trên Windows/SQL Server.

| ID | Chức năng | Dữ liệu / thao tác | Kết quả mong đợi | Kết quả thực tế | Trạng thái |
|---|---|---|---|---|---|
| TC01 | Đăng nhập | Đăng nhập `admin` với mật khẩu mẫu | Vào trang Tổng quan với quyền quản lý | | Chưa chạy |
| TC02 | Đăng nhập | Nhập sai mật khẩu | Báo thông tin đăng nhập không hợp lệ | | Chưa chạy |
| TC03 | Đăng nhập | Đăng nhập tài khoản bị khóa | Từ chối đăng nhập | | Chưa chạy |
| TC04 | Phân quyền | Đăng nhập `kho01` | Chỉ hiển thị các chức năng được cấp | | Chưa chạy |
| TC05 | F06 | Tạo loại thiết bị với mã mới | Lưu thành công | | Chưa chạy |
| TC06 | F06 | Tạo loại thiết bị trùng mã | Báo trùng, không tạo bản ghi | | Chưa chạy |
| TC07 | F07 | Thêm thiết bị với số lượng hợp lệ | Lưu thiết bị thành công | | Chưa chạy |
| TC08 | F07 | Nhập khả dụng lớn hơn tổng số lượng | Từ chối dữ liệu không hợp lệ | | Chưa chạy |
| TC09 | F08 | Tạo người mượn mới | Lưu thành công và có mã người mượn | | Chưa chạy |
| TC10 | F08 | Xóa người mượn đã có lịch sử mượn | Không cho xóa | | Chưa chạy |
| TC11 | F09 | Lập phiếu với một thiết bị đủ tồn | Tạo phiếu và giảm khả dụng đúng số lượng | | Chưa chạy |
| TC12 | F09 | Mượn số lượng lớn hơn khả dụng | Báo thiếu tồn; phiếu không được lưu | | Chưa chạy |
| TC13 | F09 | Lập phiếu có nhiều thiết bị | Tạo một phiếu với đủ các dòng chi tiết | | Chưa chạy |
| TC14 | F10/F11 | Tìm phiếu theo mã và mở chi tiết | Hiển thị đúng thông tin và các dòng thiết bị | | Chưa chạy |
| TC15 | F12 | Trả một phần thiết bị ở tình trạng tốt | Tăng ReturnedQuantity và AvailableQuantity đúng số lượng | | Chưa chạy |
| TC16 | F12 | Trả phần còn lại ở tình trạng tốt | Phiếu chuyển sang đã trả; tồn kho tăng đúng | | Chưa chạy |
| TC17 | F12 | Trả thiết bị hư hỏng | Tăng DamagedQuantity, không tăng AvailableQuantity | | Chưa chạy |
| TC18 | F12 | Trả thiết bị mất | Tăng LostQuantity, không tăng AvailableQuantity | | Chưa chạy |
| TC19 | F12 | Trả thiếu phụ kiện nhưng bỏ trống ghi chú | Từ chối lưu và yêu cầu ghi chú | | Chưa chạy |
| TC20 | F12 | Trả vượt số lượng còn lại | Từ chối; dữ liệu transaction không thay đổi | | Chưa chạy |
| TC21 | F13 | Tra cứu biên bản theo mã | Hiển thị đúng biên bản | | Chưa chạy |
| TC22 | F13 | Hủy biên bản với lý do hợp lệ | Biên bản chuyển trạng thái đã hủy, tồn kho được hoàn tác | | Chưa chạy |
| TC23 | F13 | Hủy biên bản sau khi thiết bị đã được mượn lại | Từ chối nếu không thể hoàn tác tồn kho an toàn | | Chưa chạy |
| TC24 | F14 | Mở danh sách quá hạn | Chỉ hiện phiếu quá hạn còn thiết bị chưa trả | | Chưa chạy |
| TC25 | F14 | Lọc số ngày quá hạn tối thiểu | Chỉ hiện phiếu thỏa điều kiện | | Chưa chạy |
| TC26 | F04 | Tạo tài khoản mới | Tạo thành công, mật khẩu được băm | | Chưa chạy |
| TC27 | F04 | Đặt lại mật khẩu | Mật khẩu mới đăng nhập được, mật khẩu cũ không còn hợp lệ | | Chưa chạy |
| TC28 | F04 | Tự khóa tài khoản đang đăng nhập | Từ chối thao tác | | Chưa chạy |
| TC29 | F05 | Gán quyền cho nhóm nhân viên kho | Quyền lưu đúng và có hiệu lực ở lần đăng nhập kế tiếp | | Chưa chạy |
| TC30 | F16 | Đổi tiền tố mã | Mã xem trước sử dụng tiền tố mới | | Chưa chạy |
| TC31 | F16 | Bật reset hằng ngày khi không chèn ngày | Từ chối cấu hình để tránh mã trùng | | Chưa chạy |
| TC32 | F15 | Xem báo cáo theo khoảng ngày | Số liệu khớp với dữ liệu phiếu trong khoảng lọc | | Chưa chạy |
| TC33 | F15 | Xem trước/in phiếu mượn | Bản in có thông tin phiếu và chi tiết thiết bị | | Chưa chạy |
| TC34 | F15 | Xem trước/in biên bản trả | Bản in có tình trạng và số lượng trả | | Chưa chạy |
| TC35 | F18 | Xem top thiết bị mượn nhiều | Xếp hạng theo tổng số lượng mượn giảm dần | | Chưa chạy |

## Kiểm thử đồng thời đề xuất

1. Mở hai phiên ứng dụng bằng hai tài khoản nhân viên kho.
2. Cho cả hai phiên lập phiếu mượn cùng một thiết bị khi tồn khả dụng chỉ còn đủ cho một phiếu.
3. Kết quả mong đợi: chỉ một giao dịch thành công; giao dịch còn lại nhận thông báo thiếu tồn; tồn kho không âm và không tạo phiếu dở dang.

## Xác nhận kiểm thử

- Máy kiểm thử / Windows: `[Điền]`
- Phiên bản SQL Server: `[Điền]`
- Phiên bản Visual Studio / .NET SDK: `[Điền]`
- Người kiểm thử: `[Điền]`
- Ngày kiểm thử: `[Điền]`

## Bổ sung v5.2 - Nhập sai cố ý
| Mã | Chức năng | Thao tác nhập sai | Kết quả mong đợi |
|---|---|---|---|
| N01 | Tài khoản | Tên đăng nhập có dấu/khoảng trắng | "Tên đăng nhập chỉ gồm chữ không dấu..." |
| N02 | Tài khoản | Email "abc", SĐT "12ab" | "Email/Số điện thoại không hợp lệ" |
| N03 | Nhóm quyền | Tên 1 ký tự hoặc >50 ký tự | Báo lỗi độ dài |
| N04 | Nhóm quyền | Sửa tên rồi xem danh sách | Hiện đúng tên nhóm, không hiện tên kiểu |
| N05 | Nhóm quyền | Bỏ quyền quản lý nhóm quyền của nhóm mình | Bị chặn, có thông báo |
| N06 | Phiếu mượn | Cùng thiết bị 2 dòng / SL 0 / SL âm / hạn trả trước ngày mượn / hạn >1 năm | Báo lỗi tương ứng |
| N07 | Biên bản trả | Trả 2 lần cùng dòng / SL 0 / hỏng không ghi chú | Báo lỗi tương ứng |
| N08 | Menu | Mục báo cáo | Hiện "Báo cáo", không có ký tự `_` |
