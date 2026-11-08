# QUẢN LÝ MƯỢN TRẢ THIẾT BỊ

Ứng dụng desktop quản lý thiết bị, người mượn, phiếu mượn, trả thiết bị và biên bản bàn giao. Công nghệ: C# WinForms, .NET 10, SQL Server, Dapper.

## 1. Yêu cầu môi trường

- Windows 10/11.
- Visual Studio có workload **.NET desktop development** và hỗ trợ .NET 10.
- SQL Server 2017 trở lên (Express/Developer/LocalDB đều có thể dùng).
- SQL Server Management Studio (SSMS) hoặc công cụ chạy script SQL tương đương.

## 2. Cài đặt cơ sở dữ liệu

Mở SSMS, kết nối SQL Server và chạy lần lượt các script trong thư mục `Database`:

1. `01_CreateDatabase.sql`
2. `02_CreateTables.sql`
3. `03_Constraints.sql`
4. `04_Indexes.sql`
5. `05_StoredProcedures.sql`
6. `06_SeedData.sql`
7. `07_TestData.sql` (dữ liệu kiểm thử, có thể bỏ qua nếu không cần)

Không chạy script theo thứ tự ngược. Các script được thiết kế để chạy lại trong quá trình nâng cấp, nhưng nên sao lưu database trước khi cập nhật trên dữ liệu thật.

## 3. Cấu hình kết nối

Mở `QuanLyMuonTra.App/connection.txt`, chỉnh chuỗi kết nối phù hợp với SQL Server trên máy.

Ví dụ SQL Server Express:

```text
Server=.\SQLEXPRESS;Database=QuanLyMuonTra;Trusted_Connection=True;TrustServerCertificate=True;
```

Ví dụ LocalDB:

```text
Server=(localdb)\MSSQLLocalDB;Database=QuanLyMuonTra;Trusted_Connection=True;TrustServerCertificate=True;
```

## 4. Mở và chạy ứng dụng

1. Mở `QuanLyMuonTra.slnx` bằng Visual Studio.
2. Restore NuGet Packages.
3. Đặt `QuanLyMuonTra.App` làm Startup Project.
4. Build Solution.
5. Nhấn F5 để chạy.

## 5. Tài khoản mẫu

| Tài khoản | Mật khẩu | Vai trò |
|---|---|---|
| `admin` | `Admin@123` | Người quản lý |
| `kho01` | `Kho@123` | Nhân viên kho |

Mật khẩu mẫu chỉ phục vụ kiểm thử. Đổi mật khẩu trước khi đưa vào sử dụng thật. Mật khẩu được lưu dưới dạng PBKDF2-SHA256, không lưu dạng văn bản thuần.

## 6. Danh sách chức năng

| Mã | Chức năng | Trạng thái trong source |
|---|---|---|
| F01 | Đăng nhập, kiểm tra tài khoản và quyền | Đã có |
| F02 | Tổng quan | Đã có |
| F04 | Quản lý tài khoản: tạo, sửa, khóa/mở khóa, đặt lại mật khẩu | Đã triển khai |
| F05 | Quản lý nhóm quyền và gán quyền chức năng | Đã triển khai |
| F06 | Quản lý loại thiết bị | Đã triển khai |
| F07 | Quản lý thiết bị | Đã có, dùng DeviceService |
| F08 | Quản lý người mượn và lịch sử mượn | Đã triển khai |
| F09 | Lập phiếu mượn nhiều thiết bị | Đã triển khai, có transaction và kiểm tra tồn kho |
| F10 | Tra cứu phiếu mượn | Đã triển khai |
| F11 | Chi tiết phiếu mượn và lịch sử trả | Đã triển khai |
| F12 | Ghi nhận trả thiết bị, tình trạng và số lượng | Đã triển khai, có transaction |
| F13 | Tra cứu, xem và hủy biên bản trả | Đã triển khai, hủy có lý do và kiểm tra tồn kho |
| F14 | Danh sách phiếu quá hạn | Đã triển khai, gọi stored procedure |
| F15 | Báo cáo tổng hợp, xem trước/in phiếu mượn và biên bản trả | Đã triển khai |
| F16 | Cấu hình mã tự động | Đã triển khai |
| F18 | Thống kê thiết bị được mượn nhiều | Đã triển khai |

## 7. Kiến trúc source

- `QuanLyMuonTra.UI`: control giao diện, theme sáng/tối và style dùng chung.
- `QuanLyMuonTra.App/Forms`: form đăng nhập, form chính và các hộp thoại.
- `QuanLyMuonTra.App/Pages`: các màn hình chức năng.
- `QuanLyMuonTra.App/Services`: xử lý nghiệp vụ và kiểm tra quyền.
- `QuanLyMuonTra.App/Data`: truy vấn SQL Server bằng Dapper.
- `QuanLyMuonTra.App/Models`: DTO và model dữ liệu.
- `QuanLyMuonTra.App/Security`: phiên đăng nhập, quyền và băm mật khẩu.
- `Database`: script tạo database, bảng, constraint, index, view, stored procedure và dữ liệu mẫu.

## 8. Quy tắc nghiệp vụ chính

- Lập phiếu mượn và ghi nhận trả được thực hiện trong transaction.
- Số lượng mượn không được vượt quá số lượng khả dụng.
- Số lượng trả không được vượt số còn phải trả trên từng dòng phiếu.
- Thiết bị trả tốt được cộng lại vào khả dụng; hư hỏng/thiếu phụ kiện được đưa vào nhóm hư hỏng; mất được đưa vào nhóm mất.
- Hủy biên bản trả chỉ thành công nếu có thể hoàn tác an toàn tồn kho; không xóa cứng biên bản.
- Trạng thái quá hạn được tính theo hạn trả và số lượng còn chưa trả.
- Quyền được kiểm tra ở cả menu giao diện và tầng Services.

## 9. Kiểm thử và giới hạn xác nhận

Xem `docs/TEST_CASES.md` để biết các ca kiểm thử đề xuất. Source đã được rà soát tĩnh và đóng gói; môi trường tạo source không có Windows/.NET SDK/SQL Server nên chưa xác nhận Build, chạy giao diện hoặc thực thi truy vấn trên SQL Server. Nhóm cần thực hiện các bước kiểm thử trên máy Windows trước khi nộp.

## 10. Tài liệu bàn giao

- `docs/TEST_CASES.md`: danh sách ca kiểm thử.
- `docs/BAO_CAO_NHOM_TEMPLATE.md`: khung báo cáo nhóm và phân công thành viên.
- `docs/SLIDE_OUTLINE.md`: dàn ý slide thuyết trình.
