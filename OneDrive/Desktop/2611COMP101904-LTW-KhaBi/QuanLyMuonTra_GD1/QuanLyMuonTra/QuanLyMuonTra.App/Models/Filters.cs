namespace QuanLyMuonTra.App.Models;

public sealed class LoanFilter
{
    public string? Keyword { get; set; }
    public DateTime? BorrowFrom { get; set; }
    public DateTime? BorrowTo { get; set; }
    public DateTime? DueFrom { get; set; }
    public DateTime? DueTo { get; set; }
    /// <summary>Giá trị của LoanDisplayStatus, hoặc null = tất cả.</summary>
    public int? Status { get; set; }
    public int? BorrowerId { get; set; }
}

public sealed class RecordFilter
{
    public string? Keyword { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    /// <summary>null = tất cả, false = còn hiệu lực, true = đã hủy.</summary>
    public bool? Cancelled { get; set; }
    public int? LoanSlipId { get; set; }
}

public enum DeviceStockFilter
{
    All = 0,
    InStock = 1,      // còn khả dụng
    OutOfStock = 2,   // hết khả dụng
    OnLoan = 3,       // đang có người mượn
    Damaged = 4,      // có thiết bị hư hỏng / mất
}
