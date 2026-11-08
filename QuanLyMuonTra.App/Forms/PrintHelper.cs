using System.Drawing.Printing;
using QuanLyMuonTra.App.Models;

namespace QuanLyMuonTra.App.Forms;

/// <summary>In/xem trước phiếu mượn và biên bản trả bằng PrintDocument, không phụ thuộc thư viện ngoài.</summary>
internal static class PrintHelper
{
    public static void PreviewLoan(IWin32Window owner, LoanSlipFull data)
    {
        var h = data.Header;
        var lines = new List<string>
        {
            "PHIẾU MƯỢN THIẾT BỊ", "", $"Mã phiếu: {h.SlipCode}", $"Người mượn: {h.BorrowerName}",
            $"Ngày mượn: {h.BorrowDate:dd/MM/yyyy}     Hạn trả: {h.DueDate:dd/MM/yyyy}", $"Người lập: {h.CreatedByName}", $"Ghi chú: {h.Note}", "",
            "CHI TIẾT THIẾT BỊ", "Mã thiết bị | Tên thiết bị | Mượn | Đã trả | Còn lại"
        };
        lines.AddRange(data.Details.Select(x => $"{x.DeviceCode} | {x.DeviceName} | {x.Quantity} | {x.ReturnedQuantity} | {x.RemainingQuantity}"));
        lines.Add(""); lines.Add($"Tổng số lượng: {h.TotalQty}       Đã trả: {h.ReturnedQty}       Còn lại: {h.RemainingQty}");
        lines.Add(""); lines.Add("Người mượn                              Người giao thiết bị                              Người lập phiếu");
        lines.Add("(Ký, ghi rõ họ tên)                       (Ký, ghi rõ họ tên)                              (Ký, ghi rõ họ tên)");
        Show(owner, "Phiếu mượn " + h.SlipCode, lines);
    }

    public static void PreviewReturn(IWin32Window owner, ReturnRecordFull data)
    {
        var h = data.Header;
        var lines = new List<string>
        {
            h.IsCancelled ? "BIÊN BẢN TRẢ THIẾT BỊ - ĐÃ HỦY" : "BIÊN BẢN TRẢ / BÀN GIAO THIẾT BỊ", "",
            $"Mã biên bản: {h.RecordCode}", $"Mã phiếu mượn: {h.SlipCode}", $"Người mượn: {h.BorrowerName}",
            $"Ngày trả: {h.ReturnDate:dd/MM/yyyy HH:mm}", $"Người giao: {h.GiverName}     Người nhận: {h.ReceivedByName}", $"Ghi chú: {h.Note}", "",
            "CHI TIẾT THIẾT BỊ", "Mã thiết bị | Tên thiết bị | Số lượng | Tình trạng | Ghi chú"
        };
        lines.AddRange(data.Details.Select(x => $"{x.DeviceCode} | {x.DeviceName} | {x.Quantity} | {x.ConditionText} | {x.Condition}"));
        lines.Add(""); lines.Add($"Tổng số lượng bàn giao: {h.TotalQuantity}");
        if (h.IsCancelled) { lines.Add($"Lý do hủy: {h.CancelReason}"); lines.Add($"Người hủy: {h.CancelledByName} - {h.CancelledAt:dd/MM/yyyy HH:mm}"); }
        lines.Add(""); lines.Add("Người giao thiết bị                                      Người nhận thiết bị");
        lines.Add("(Ký, ghi rõ họ tên)                                      (Ký, ghi rõ họ tên)");
        Show(owner, "Biên bản " + h.RecordCode, lines);
    }

    static void Show(IWin32Window owner, string documentName, List<string> lines)
    {
        using var document = new PrintDocument { DocumentName = documentName };
        int index = 0;
        document.DefaultPageSettings.Margins = new Margins(55, 55, 55, 55);
        document.PrintPage += (_, e) =>
        {
            using var body = new Font("Arial", 9.5f);
            using var heading = new Font("Arial", 15, FontStyle.Bold);
            using var bold = new Font("Arial", 10, FontStyle.Bold);
            using var noWrap = new StringFormat { FormatFlags = StringFormatFlags.NoWrap, Trimming = StringTrimming.EllipsisCharacter };
            float y = e.MarginBounds.Top;
            if (index == 0)
            {
                e.Graphics.DrawString(lines[0], heading, Brushes.Black, e.MarginBounds, new StringFormat { Alignment = StringAlignment.Center });
                y += 42; index = 1;
            }
            float lineHeight = body.GetHeight(e.Graphics) + 7;
            while (index < lines.Count && y + lineHeight < e.MarginBounds.Bottom)
            {
                string line = lines[index++];
                e.Graphics.DrawString(line, line.StartsWith("CHI TIẾT") || line.StartsWith("Mã thiết bị") ? bold : body, Brushes.Black,
                    new RectangleF(e.MarginBounds.Left, y, e.MarginBounds.Width, lineHeight), noWrap);
                y += lineHeight;
            }
            e.HasMorePages = index < lines.Count;
        };
        using var preview = new PrintPreviewDialog { Document = document, Width = 1100, Height = 800 };
        preview.ShowDialog(owner);
    }
}
