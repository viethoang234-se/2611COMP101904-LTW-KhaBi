namespace QuanLyMuonTra.App.Models;

/// <summary>Trạng thái hiển thị của phiếu mượn (tính từ dữ liệu, không lưu cứng).</summary>
public enum LoanDisplayStatus
{
    Borrowing = 0,   // Đang mượn
    Partial = 1,     // Trả một phần
    Returned = 2,    // Đã trả
    Overdue = 3,     // Quá hạn
}

/// <summary>Tình trạng thiết bị khi trả (khớp ReturnRecordDetails.ConditionCode).</summary>
public enum ReturnCondition
{
    Good = 0,              // Tốt: cộng lại vào số lượng khả dụng
    Damaged = 1,           // Hư hỏng: không cộng khả dụng, vào nhóm "hư hỏng"
    Lost = 2,              // Mất: không cộng khả dụng, vào nhóm "mất"
    MissingAccessory = 3,  // Thiếu phụ kiện: tính là đã trả, nhưng vào nhóm "hư hỏng" chờ xử lý
}

public static class LoanText
{
    public static string Status(int code) => (LoanDisplayStatus)code switch
    {
        LoanDisplayStatus.Borrowing => "Đang mượn",
        LoanDisplayStatus.Partial => "Trả một phần",
        LoanDisplayStatus.Returned => "Đã trả",
        LoanDisplayStatus.Overdue => "Quá hạn",
        _ => "?",
    };

    public static string Condition(int code) => (ReturnCondition)code switch
    {
        ReturnCondition.Good => "Tốt",
        ReturnCondition.Damaged => "Hư hỏng",
        ReturnCondition.Lost => "Mất",
        ReturnCondition.MissingAccessory => "Thiếu phụ kiện",
        _ => "?",
    };
}

/// <summary>Một dòng trong danh sách phiếu mượn (dữ liệu từ vw_LoanSlipSummary).</summary>
public sealed class LoanSlipRow
{
    public int LoanSlipId { get; set; }
    public string SlipCode { get; set; } = "";
    public int BorrowerId { get; set; }
    public string BorrowerCode { get; set; } = "";
    public string BorrowerName { get; set; } = "";
    public string Phone { get; set; } = "";
    public int CreatedBy { get; set; }
    public string CreatedByName { get; set; } = "";
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public int Status { get; set; }
    public string Note { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public int TotalQty { get; set; }
    public int ReturnedQty { get; set; }
    public int RemainingQty { get; set; }
    public int DisplayStatus { get; set; }
    public int DaysOverdue { get; set; }
    public string StatusText => LoanText.Status(DisplayStatus);
}

public sealed class LoanSlipDetailRow
{
    public int DetailId { get; set; }
    public int DeviceId { get; set; }
    public string DeviceCode { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public string Unit { get; set; } = "";
    public int Quantity { get; set; }
    public int ReturnedQuantity { get; set; }
    public int RemainingQuantity => Quantity - ReturnedQuantity;
}

/// <summary>Toàn bộ thông tin một phiếu mượn: đầu phiếu, thiết bị, lịch sử trả (dùng cho F11 và in phiếu).</summary>
public sealed class LoanSlipFull
{
    public LoanSlipRow Header { get; set; } = new();
    public List<LoanSlipDetailRow> Details { get; set; } = new();
    public List<ReturnRecordFull> Returns { get; set; } = new();
}

public sealed class NewLoanItem
{
    public int DeviceId { get; set; }
    public int Quantity { get; set; }
}

public sealed class NewLoanSlip
{
    public int BorrowerId { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public string? Note { get; set; }
    public List<NewLoanItem> Items { get; set; } = new();
}

public sealed class NewReturnItem
{
    public int LoanSlipDetailId { get; set; }
    public int Quantity { get; set; }
    public ReturnCondition Condition { get; set; }
    public string? ConditionNote { get; set; }
}

public sealed class NewReturnRecord
{
    public int LoanSlipId { get; set; }
    public DateTime ReturnDate { get; set; }
    public string? GiverName { get; set; }
    public string? Note { get; set; }
    public List<NewReturnItem> Items { get; set; } = new();
}

public sealed class ReturnRecordRow
{
    public int ReturnRecordId { get; set; }
    public string RecordCode { get; set; } = "";
    public int LoanSlipId { get; set; }
    public string SlipCode { get; set; } = "";
    public string BorrowerName { get; set; } = "";
    public DateTime ReturnDate { get; set; }
    public int ReceivedBy { get; set; }
    public string ReceivedByName { get; set; } = "";
    public string GiverName { get; set; } = "";
    public string Note { get; set; } = "";
    public bool IsCancelled { get; set; }
    public string CancelledByName { get; set; } = "";
    public DateTime? CancelledAt { get; set; }
    public string CancelReason { get; set; } = "";
    public int TotalQuantity { get; set; }
    public string StatusText => IsCancelled ? "Đã hủy" : "Hiệu lực";
}

public sealed class ReturnRecordDetailRow
{
    public int Id { get; set; }
    public int ReturnRecordId { get; set; }
    public int DeviceId { get; set; }
    public string DeviceCode { get; set; } = "";
    public string DeviceName { get; set; } = "";
    public string Unit { get; set; } = "";
    public int Quantity { get; set; }
    public int ConditionCode { get; set; }
    public string Condition { get; set; } = "";
    public string ConditionText => LoanText.Condition(ConditionCode);
}

public sealed class ReturnRecordFull
{
    public ReturnRecordRow Header { get; set; } = new();
    public List<ReturnRecordDetailRow> Details { get; set; } = new();
}

public sealed class OverdueRow
{
    public int LoanSlipId { get; set; }
    public string SlipCode { get; set; } = "";
    public int BorrowerId { get; set; }
    public string BorrowerName { get; set; } = "";
    public string Phone { get; set; } = "";
    public DateTime BorrowDate { get; set; }
    public DateTime DueDate { get; set; }
    public int DaysOverdue { get; set; }
    public int UnreturnedQty { get; set; }
    public string UnreturnedDevices { get; set; } = "";
}

public sealed class BorrowerRow
{
    public int BorrowerId { get; set; }
    public string BorrowerCode { get; set; } = "";
    public string FullName { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string Department { get; set; } = "";
    public string Address { get; set; } = "";
    public string Note { get; set; } = "";
    public bool IsActive { get; set; } = true;
    public int LoanCount { get; set; }
}

public sealed class CategoryRow
{
    public int CategoryId { get; set; }
    public string CategoryCode { get; set; } = "";
    public string CategoryName { get; set; } = "";
    public string Description { get; set; } = "";
    public int DeviceCount { get; set; }
}

/// <summary>Kết quả của một nghiệp vụ ở tầng BLL: thành công hoặc có thông báo lỗi tiếng Việt để hiển thị.</summary>
public sealed class Result
{
    public bool Success { get; init; }
    public string Error { get; init; } = "";
    public static Result Ok() => new() { Success = true };
    public static Result Fail(string error) => new() { Success = false, Error = error };
}

public sealed class Result<T>
{
    public bool Success { get; init; }
    public string Error { get; init; } = "";
    public T? Value { get; init; }
    public static Result<T> Ok(T value) => new() { Success = true, Value = value };
    public static Result<T> Fail(string error) => new() { Success = false, Error = error };
}
