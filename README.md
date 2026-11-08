# Quản lý mượn trả thiết bị (WinForms, .NET 10, SQL Server)

## Yêu cầu
- Visual Studio 2022 mới nhất (hoặc VS 2026) có workload **.NET desktop development**, hỗ trợ .NET 10 và file `.slnx`
- SQL Server (Express/LocalDB đều được) + SSMS

## Chạy thử (3 bước)
1. Mở SSMS, chạy lần lượt các file trong `Database/`: `01` -> `02` -> `03` -> `04` -> `05` -> `06` (bắt buộc), `07` (dữ liệu thử, tùy chọn). Yêu cầu SQL Server 2017 trở lên. Chạy lại nhiều lần vẫn an toàn, kể cả khi nâng cấp từ CSDL cũ.
2. Mở `QuanLyMuonTra.App/connection.txt`, sửa chuỗi kết nối nếu SQL Server của bạn không phải `localhost`
   (ví dụ SQL Express: `Server=.\SQLEXPRESS;Database=QuanLyMuonTra;Trusted_Connection=True;TrustServerCertificate=True;`).
3. Mở `QuanLyMuonTra.slnx` -> chuột phải Solution -> **Restore NuGet Packages** -> **Build Solution** -> đặt `QuanLyMuonTra.App` làm Startup Project -> F5.

Tài khoản mẫu: `admin / Admin@123` (Người quản lý), `kho01 / Kho@123` (Nhân viên kho). Hãy đổi mật khẩu khi dùng thật.

## Đã có / chưa có
| Form | Trạng thái |
|---|---|
| F01 Đăng nhập | Xong (đăng nhập thật qua SQL, băm mật khẩu PBKDF2, phân quyền) |
| F02 Dashboard | Xong (4 thẻ thống kê + phiếu mượn gần đây) |
| F07 Quản lý thiết bị | Xong; đã chuyển thao tác đọc/ghi qua `DeviceService` |
| F06 Loại thiết bị | GĐ2: tìm kiếm, thêm, sửa, xóa có kiểm tra thiết bị đang sử dụng |
| F08 Người mượn | GĐ2: tìm kiếm, thêm, sửa, ngừng hoạt động, xóa khi chưa có lịch sử, xem lịch sử mượn |
| F09 Lập phiếu mượn | GĐ2: chọn người mượn, thêm nhiều thiết bị, kiểm tra tồn, tạo phiếu qua transaction |
| F10 Tra cứu phiếu mượn | GĐ2: tìm theo từ khóa, ngày mượn, hạn trả, trạng thái |
| F11 Chi tiết phiếu mượn | GĐ2: xem thiết bị, số lượng đã trả/còn lại và lịch sử trả |
| F12-F18, F03-F05 | Chưa hoàn thiện; các mục còn lại vẫn là trang tạm |

## Ghi chú phiên bản GĐ2
- Các màn hình GĐ2 gọi tầng `Services`; không thực hiện truy vấn SQL trực tiếp từ giao diện.
- Mã phiếu mượn được cấp trong transaction khi lưu, không cấp trước khi lưu.
- Sau khi cập nhật code, cần mở solution bằng Visual Studio, Restore NuGet và Build để xác nhận trên Windows/.NET 10.

## Cấu trúc
- `QuanLyMuonTra.UI` - bộ control giao diện dùng chung (Theme sáng/tối, CardPanel, AppButton, AppTextBox, AppLabel, NavButton, StatCard, AppGrid...). Build xong sẽ hiện trong **Toolbox**.
- `QuanLyMuonTra.App` - ứng dụng: `Forms/` (form), `Pages/` (trang nội dung UserControl), `Data/` (truy vấn SQL bằng Dapper), `Security/`, `Models/`.

## Thêm một trang mới (ví dụ F08 Người mượn)
1. Chuột phải thư mục `Pages` -> Add -> User Control, đổi `: UserControl` thành `: ThemedUserControl`.
2. Copy cách bố trí từ `DevicePage` (toolbar + CardPanel + DataGridView), gọi `AppGrid.Apply(grid)` khi Load.
3. Mở `NavCatalog.cs`, thay `Todo(...)` của F08 bằng `() => new BorrowerPage()`.

## Lưu ý khi dùng Designer
- Luôn **Build** trước khi mở Designer (control trong project UI cần được build).
- Designer luôn hiển thị theme sáng; chế độ tối chỉ thấy khi chạy.
- Icon dùng font "Segoe MDL2 Assets" (có sẵn trên Windows 10/11). Đổi icon: mở Character Map, chọn font đó, copy ký tự.
- Đổi màu chủ đạo: sửa `Primary`/`PrimaryHover` trong `QuanLyMuonTra.UI/Theme.cs`.
