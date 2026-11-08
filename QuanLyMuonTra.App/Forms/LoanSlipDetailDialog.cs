using QuanLyMuonTra.App.Pages;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

/// <summary>F11 - xem chi tiết phiếu mượn và các lần trả thiết bị.</summary>
public sealed class LoanSlipDetailDialog : ThemedForm
{
    readonly LoanSlipFull _data;

    public LoanSlipDetailDialog(LoanSlipFull data)
    {
        _data = data;
        Text = $"Chi tiết phiếu mượn - {data.Header.SlipCode}";
        StartPosition = FormStartPosition.CenterParent; Size = new Size(1100, 700); MinimumSize = new Size(900, 560);

        var header = new TableLayoutPanel { Dock = DockStyle.Top, Height = 118, Padding = new Padding(16), ColumnCount = 4, RowCount = 3 };
        for (int i = 0; i < 4; i++) header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 33)); header.RowStyles.Add(new RowStyle(SizeType.Percent, 33)); header.RowStyles.Add(new RowStyle(SizeType.Percent, 34));
        AddInfo(header, 0, 0, "Mã phiếu", data.Header.SlipCode);
        AddInfo(header, 1, 0, "Người mượn", data.Header.BorrowerName);
        AddInfo(header, 2, 0, "Ngày mượn", data.Header.BorrowDate.ToString("dd/MM/yyyy"));
        AddInfo(header, 3, 0, "Hạn trả", data.Header.DueDate.ToString("dd/MM/yyyy"));
        AddInfo(header, 0, 1, "Người lập", data.Header.CreatedByName);
        AddInfo(header, 1, 1, "Trạng thái", data.Header.StatusText);
        AddInfo(header, 2, 1, "Đã trả / Tổng", $"{data.Header.ReturnedQty} / {data.Header.TotalQty}");
        AddInfo(header, 3, 1, "Còn phải trả", data.Header.RemainingQty.ToString());
        AddInfo(header, 0, 2, "Ghi chú", data.Header.Note, 4);

        var tabs = new TabControl { Dock = DockStyle.Fill, Padding = new Point(12, 5) };
        var tabItems = new TabPage("Thiết bị trong phiếu"); var tabReturns = new TabPage("Lịch sử trả");
        var items = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true }; AppGrid.Apply(items); items.DataSource = data.Details;
        PageUi.Header(items, (nameof(LoanSlipDetailRow.DeviceCode), "Mã thiết bị", 15), (nameof(LoanSlipDetailRow.DeviceName), "Tên thiết bị", 35),
            (nameof(LoanSlipDetailRow.Quantity), "Số lượng mượn", 15), (nameof(LoanSlipDetailRow.ReturnedQuantity), "Đã trả", 12),
            (nameof(LoanSlipDetailRow.RemainingQuantity), "Còn lại", 12), (nameof(LoanSlipDetailRow.Unit), "Đơn vị", 10));
        PageUi.HideColumns(items, nameof(LoanSlipDetailRow.DetailId), nameof(LoanSlipDetailRow.DeviceId));
        tabItems.Controls.Add(items);

        var returns = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true }; AppGrid.Apply(returns);
        var returnDetails = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true }; AppGrid.Apply(returnDetails);
        returns.DataSource = data.Returns.Select(x => x.Header).ToList();
        PageUi.Header(returns, (nameof(ReturnRecordRow.RecordCode), "Mã biên bản", 18), (nameof(ReturnRecordRow.ReturnDate), "Ngày trả", 16),
            (nameof(ReturnRecordRow.ReceivedByName), "Người nhận", 18), (nameof(ReturnRecordRow.GiverName), "Người giao", 18),
            (nameof(ReturnRecordRow.TotalQuantity), "Số lượng", 10), (nameof(ReturnRecordRow.StatusText), "Trạng thái", 14));
        PageUi.HideColumns(returns, nameof(ReturnRecordRow.ReturnRecordId), nameof(ReturnRecordRow.LoanSlipId), nameof(ReturnRecordRow.SlipCode),
            nameof(ReturnRecordRow.BorrowerName), nameof(ReturnRecordRow.ReceivedBy), nameof(ReturnRecordRow.Note), nameof(ReturnRecordRow.IsCancelled),
            nameof(ReturnRecordRow.CancelledByName), nameof(ReturnRecordRow.CancelledAt), nameof(ReturnRecordRow.CancelReason));
        PageUi.Header(returnDetails, (nameof(ReturnRecordDetailRow.DeviceCode), "Mã thiết bị", 14),
            (nameof(ReturnRecordDetailRow.DeviceName), "Tên thiết bị", 30), (nameof(ReturnRecordDetailRow.Quantity), "Số lượng trả", 12),
            (nameof(ReturnRecordDetailRow.ConditionText), "Tình trạng", 16), (nameof(ReturnRecordDetailRow.Condition), "Ghi chú tình trạng", 28));
        PageUi.HideColumns(returnDetails, nameof(ReturnRecordDetailRow.Id), nameof(ReturnRecordDetailRow.ReturnRecordId), nameof(ReturnRecordDetailRow.DeviceId),
            nameof(ReturnRecordDetailRow.Unit), nameof(ReturnRecordDetailRow.ConditionCode));
        returns.SelectionChanged += (_, _) =>
        {
            if (returns.CurrentRow?.DataBoundItem is ReturnRecordRow selected)
                returnDetails.DataSource = data.Returns.FirstOrDefault(x => x.Header.ReturnRecordId == selected.ReturnRecordId)?.Details ?? new List<ReturnRecordDetailRow>();
            else returnDetails.DataSource = new List<ReturnRecordDetailRow>();
        };
        var returnSplit = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, Size = new Size(1000, 500), SplitterDistance = 210, Panel1MinSize = 100, Panel2MinSize = 100 };
        returnSplit.Panel1.Controls.Add(returns); returnSplit.Panel2.Controls.Add(returnDetails);
        tabReturns.Controls.Add(returnSplit);
        if (data.Returns.Count > 0) returnDetails.DataSource = data.Returns[0].Details;
        tabs.TabPages.Add(tabItems); tabs.TabPages.Add(tabReturns);
        Controls.Add(tabs); Controls.Add(header);
    }

    static void AddInfo(TableLayoutPanel panel, int col, int row, string title, string value, int span = 1)
    {
        var box = new Panel { Dock = DockStyle.Fill, Padding = new Padding(4) };
        box.Controls.Add(new Label { Text = value, Dock = DockStyle.Bottom, Height = 24, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), AutoEllipsis = true });
        box.Controls.Add(new Label { Text = title, Dock = DockStyle.Top, Height = 20, ForeColor = Theme.TextSecondary, Font = new Font("Segoe UI", 8.5f) });
        panel.Controls.Add(box, col, row); if (span > 1) panel.SetColumnSpan(box, span);
    }
}
