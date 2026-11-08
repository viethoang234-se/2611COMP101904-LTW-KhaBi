# Quản lý mượn trả thiết bị (WinForms, .NET 10, SQL Server)

## Yêu cầu
- Visual Studio 2022 mới nhất (hoặc VS 2026) có workload **.NET desktop development**, hỗ trợ .NET 10 và file `.slnx`
- SQL Server (Express/LocalDB đều được) + SSMS

## Chạy thử (3 bước)
1. Mở SSMS, chạy file `Database/01_schema_and_seed.sql` (tạo CSDL `QuanLyMuonTra` + dữ liệu mẫu).
2. Mở `QuanLyMuonTra.App/connection.txt`, sửa chuỗi kết nối nếu SQL Server của bạn không phải `localhost`
   (ví dụ SQL Express: `Server=.\SQLEXPRESS;Database=QuanLyMuonTra;Trusted_Connection=True;TrustServerCertificate=True;`).
3. Mở `QuanLyMuonTra.slnx` -> chuột phải Solution -> **Restore NuGet Packages** -> **Build Solution** -> đặt `QuanLyMuonTra.App` làm Startup Project -> F5.

Tài khoản mẫu: `admin / Admin@123` (Người quản lý), `kho01 / Kho@123` (Nhân viên kho). Hãy đổi mật khẩu khi dùng thật.

## Đã có / chưa có
| Form | Trạng thái |
|---|---|
| F01 Đăng nhập | Xong (đăng nhập thật qua SQL, băm mật khẩu PBKDF2, phân quyền) |
| F02 Dashboard | Xong (4 thẻ thống kê + phiếu mượn gần đây) |
| F07 Quản lý thiết bị | Xong (tìm kiếm, lọc loại, thêm/sửa/xóa) - **dùng làm mẫu cho các trang danh sách khác** |
| Khung chính (sidebar, header, đổi sáng/tối, đăng xuất) | Xong, menu tự ẩn theo quyền |
| F03, F04, F05, F06, F08-F18 | Đã có mục menu + trang tạm (`PlaceholderPage`), CSDL đã có sẵn bảng cho tất cả |

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
