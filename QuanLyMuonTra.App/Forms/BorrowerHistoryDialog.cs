using QuanLyMuonTra.App.Pages;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

public sealed class BorrowerHistoryDialog : ThemedForm
{
    readonly DataGridView _grid = new() { Dock = DockStyle.Fill, AutoGenerateColumns = true };

    public BorrowerHistoryDialog(BorrowerRow borrower, List<LoanSlipRow> rows)
    {
        Text = $"Lịch sử mượn - {borrower.FullName}";
        StartPosition = FormStartPosition.CenterParent; Size = new Size(980, 560); MinimumSize = new Size(800, 420);
        AppGrid.Apply(_grid); _grid.DataSource = rows;
        PageUi.Header(_grid, (nameof(LoanSlipRow.SlipCode), "Mã phiếu", 18), (nameof(LoanSlipRow.BorrowDate), "Ngày mượn", 14),
            (nameof(LoanSlipRow.DueDate), "Hạn trả", 14), (nameof(LoanSlipRow.TotalQty), "Tổng mượn", 10),
            (nameof(LoanSlipRow.ReturnedQty), "Đã trả", 10), (nameof(LoanSlipRow.RemainingQty), "Còn lại", 10),
            (nameof(LoanSlipRow.StatusText), "Trạng thái", 18));
        PageUi.HideColumns(_grid, nameof(LoanSlipRow.LoanSlipId), nameof(LoanSlipRow.BorrowerId), nameof(LoanSlipRow.BorrowerCode),
            nameof(LoanSlipRow.BorrowerName), nameof(LoanSlipRow.Phone), nameof(LoanSlipRow.CreatedBy), nameof(LoanSlipRow.CreatedByName),
            nameof(LoanSlipRow.Status), nameof(LoanSlipRow.Note), nameof(LoanSlipRow.CreatedAt), nameof(LoanSlipRow.DisplayStatus), nameof(LoanSlipRow.DaysOverdue));
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0 && _grid.CurrentRow?.DataBoundItem is LoanSlipRow r) OpenDetail(r); };
        var close = new AppButton { Text = "Đóng", Kind = ButtonKind.Secondary, Size = new Size(100, 38), Anchor = AnchorStyles.Right | AnchorStyles.Bottom };
        close.Click += (_, _) => Close();
        var bottom = new ThemedPanel { Dock = DockStyle.Bottom, Height = 54, Padding = new Padding(8), Fill = PanelFill.Background }; bottom.Controls.Add(close); close.Location = new Point(bottom.Width - 112, 8); close.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        Controls.Add(_grid); Controls.Add(bottom);
    }

    void OpenDetail(LoanSlipRow row)
    {
        var result = LoanService.GetFull(row.LoanSlipId);
        if (!result.Success) { MessageBox.Show(this, result.Error); return; }
        using var dlg = new LoanSlipDetailDialog(result.Value!); dlg.ShowDialog(this);
    }
}
