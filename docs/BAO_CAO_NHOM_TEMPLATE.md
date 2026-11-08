# BÁO CÁO ĐỒ ÁN - QUẢN LÝ MƯỢN TRẢ THIẾT BỊ

> Đây là khung mẫu. Nhóm cần thay toàn bộ nội dung trong dấu `[ ... ]` bằng thông tin thực tế; không tự ghi nhận kết quả kiểm thử hoặc tỷ lệ đóng góp khi chưa xác nhận.

## Trang bìa

- Trường: `[Tên trường]`
- Khoa: `[Tên khoa]`
- Học phần: `[Tên học phần / mã học phần]`
- Đề tài: **Quản lý mượn trả thiết bị**
- Giảng viên: `[Họ tên giảng viên]`
- Nhóm: `[Số nhóm]`
- Thành viên: `[Danh sách thành viên và mã số sinh viên]`
- Thời gian: `[Học kỳ / năm học]`

## 1. Giới thiệu đề tài

Đề tài xây dựng ứng dụng desktop hỗ trợ quản lý danh mục thiết bị, người mượn, lập phiếu mượn nhiều thiết bị, ghi nhận trả và theo dõi biên bản bàn giao. Hệ thống hướng đến việc giảm thao tác thủ công, kiểm soát số lượng khả dụng và tra cứu lịch sử mượn trả.

## 2. Phân tích yêu cầu

### 2.1. Vai trò

- Người quản lý: quản trị tài khoản, nhóm quyền, cấu hình và báo cáo.
- Nhân viên kho: quản lý dữ liệu nghiệp vụ được phân quyền, lập phiếu, ghi nhận trả và tra cứu quá hạn.

### 2.2. Chức năng

- Quản lý thiết bị, loại thiết bị và người mượn.
- Lập và tra cứu phiếu mượn.
- Ghi nhận trả đủ/trả một phần, phân loại tình trạng thiết bị.
- Quản lý biên bản trả, tra cứu phiếu quá hạn.
- Báo cáo, thống kê, in biểu mẫu và cấu hình mã tự động.

## 3. Phân tích và thiết kế

### 3.1. Công nghệ

- Ngôn ngữ: C#.
- Giao diện: Windows Forms.
- Cơ sở dữ liệu: Microsoft SQL Server.
- Truy cập dữ liệu: Dapper.
- Kiến trúc: UI - Services (BLL) - Data (DAL) - Models.

### 3.2. Các bảng dữ liệu

`Users`, `Roles`, `Permissions`, `RolePermissions`, `DeviceCategories`, `Devices`, `Borrowers`, `LoanSlips`, `LoanSlipDetails`, `ReturnRecords`, `ReturnRecordDetails`, `CodeConfigs`.

### 3.3. Quy tắc nghiệp vụ

- Khi lập phiếu, hệ thống kiểm tra tồn kho và trừ khả dụng trong transaction.
- Khi trả, hệ thống kiểm tra số lượng còn phải trả và cập nhật nhóm tồn theo tình trạng.
- Khi hủy biên bản, hệ thống chỉ hoàn tác nếu tồn kho hiện tại cho phép.
- Phiếu quá hạn được xác định theo hạn trả và số lượng chưa trả.

## 4. Giao diện và chức năng

`[Chèn ảnh chụp màn hình thực tế của từng chức năng F01, F02, F04-F16, F18. Không dùng ảnh minh họa thay cho ảnh chạy thật.]`

## 5. Kiểm thử

- Dùng bảng ca kiểm thử trong `TEST_CASES.md`.
- Nêu rõ môi trường, dữ liệu kiểm thử, kết quả mong đợi và kết quả thực tế.
- Đính kèm ảnh các trường hợp thành công và trường hợp hệ thống từ chối dữ liệu sai.

## 6. Kết luận và hướng phát triển

`[Nhóm tự đánh giá mức độ hoàn thành, hạn chế đã xác nhận và hướng phát triển.]`

## 7. Phân công và tỷ lệ đóng góp

| STT | Họ tên | MSSV | Công việc thực tế | Tỷ lệ đóng góp |
|---|---|---|---|---|
| 1 | `[Thành viên 1]` | `[MSSV]` | `[Công việc]` | `[ % ]` |
| 2 | `[Thành viên 2]` | `[MSSV]` | `[Công việc]` | `[ % ]` |
| 3 | `[Thành viên 3]` | `[MSSV]` | `[Công việc]` | `[ % ]` |
| ... | ... | ... | ... | ... |

**Tổng tỷ lệ:** 100%.

## Phụ lục

- Hướng dẫn cài đặt và kết nối SQL Server.
- Tài khoản mẫu và lưu ý đổi mật khẩu.
- Danh sách script SQL.
- Link repository: `[Điền URL GitHub nếu có]`.
