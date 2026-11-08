using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

/// <summary>Chi tiết biên bản trả F13.</summary>
public sealed class ReturnRecordDetailDialog : ThemedForm
{
    public ReturnRecordDetailDialog(ReturnRecordFull data)
    {
        var h = data.Header;
        Text = $"Biên bản trả - {h.RecordCode}"; StartPosition = FormStartPosition.CenterParent; Size = new Size(950, 620); MinimumSize = new Size(760, 480);
        var info = new TableLayoutPanel { Dock = DockStyle.Top, Height = 125, Padding = new Padding(14), ColumnCount = 4, RowCount = 3 };
        for (int i = 0; i < 4; i++) info.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        AddInfo(info, 0, 0, "Mã biên bản", h.RecordCode); AddInfo(info, 1, 0, "Mã phiếu mượn", h.SlipCode);
        AddInfo(info, 2, 0, "Người mượn", h.BorrowerName); AddInfo(info, 3, 0, "Ngày trả", h.ReturnDate.ToString("dd/MM/yyyy HH:mm"));
        AddInfo(info, 0, 1, "Người giao", h.GiverName); AddInfo(info, 1, 1, "Người nhận", h.ReceivedByName);
        AddInfo(info, 2, 1, "Tổng số lượng", h.TotalQuantity.ToString()); AddInfo(info, 3, 1, "Trạng thái", h.StatusText);
        AddInfo(info, 0, 2, "Ghi chú", h.Note, 4);
        var grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true }; AppGrid.Apply(grid); grid.DataSource = data.Details;
        PageUi.Header(grid, (nameof(ReturnRecordDetailRow.DeviceCode), "Mã thiết bị", 14), (nameof(ReturnRecordDetailRow.DeviceName), "Tên thiết bị", 28),
            (nameof(ReturnRecordDetailRow.Quantity), "Số lượng trả", 12), (nameof(ReturnRecordDetailRow.ConditionText), "Tình trạng", 16),
            (nameof(ReturnRecordDetailRow.Condition), "Ghi chú tình trạng", 30));
        PageUi.HideColumns(grid, nameof(ReturnRecordDetailRow.Id), nameof(ReturnRecordDetailRow.ReturnRecordId), nameof(ReturnRecordDetailRow.DeviceId), nameof(ReturnRecordDetailRow.Unit), nameof(ReturnRecordDetailRow.ConditionCode));
        if (h.IsCancelled)
        {
            var cancelled = new Label { Dock = DockStyle.Bottom, Height = 56, Padding = new Padding(12), ForeColor = Color.Firebrick,
                Text = $"ĐÃ HỦY bởi {h.CancelledByName} lúc {h.CancelledAt:dd/MM/yyyy HH:mm}. Lý do: {h.CancelReason}" };
            Controls.Add(cancelled);
        }
        var footer = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        var print = PageUi.Button("In / xem trước", ButtonKind.Primary, 125); footer.Controls.Add(print);
        print.Click += (_, _) => PrintHelper.PreviewReturn(this, data);
        Controls.Add(grid); Controls.Add(footer); Controls.Add(info);
    }

    static void AddInfo(TableLayoutPanel panel, int col, int row, string title, string value, int span = 1)
    {
        var box = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        box.Controls.Add(new Label { Text = value, Dock = DockStyle.Bottom, Height = 24, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), AutoEllipsis = true });
        box.Controls.Add(new Label { Text = title, Dock = DockStyle.Top, Height = 20, ForeColor = Theme.TextSecondary, Font = new Font("Segoe UI", 8.5f) });
        panel.Controls.Add(box, col, row); if (span > 1) panel.SetColumnSpan(box, span);
    }
}
