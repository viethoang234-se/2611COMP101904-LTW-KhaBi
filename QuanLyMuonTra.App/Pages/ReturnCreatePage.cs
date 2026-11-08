using QuanLyMuonTra.App.Forms;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F12 - tìm phiếu còn thiết bị chưa trả và ghi nhận trả.</summary>
public sealed class ReturnCreatePage : ThemedUserControl
{
    readonly AppTextBox _keyword = PageUi.TextBox("Mã phiếu, tên người mượn, điện thoại", 330);
    readonly AppButton _search = PageUi.Button("Tìm phiếu", ButtonKind.Primary, 105);
    readonly AppButton _create = PageUi.Button("Ghi nhận trả", ButtonKind.Primary, 125);
    readonly DataGridView _grid = PageUi.Grid();
    readonly Label _count = PageUi.Label("0 phiếu còn phải trả");
    LoanSlipRow? Selected => _grid.CurrentRow?.DataBoundItem as LoanSlipRow;

    public ReturnCreatePage()
    {
        var toolbar = PageUi.Toolbar(); toolbar.Controls.Add(_keyword); toolbar.Controls.Add(_search); toolbar.Controls.Add(_create);
        var card = PageUi.Card(); card.Controls.Add(_grid);
        var footer = new ThemedPanel { Dock = DockStyle.Bottom, Height = 34, Fill = PanelFill.Surface }; _count.Dock = DockStyle.Left; footer.Controls.Add(_count); card.Controls.Add(footer);
        Controls.Add(card); Controls.Add(toolbar);
        _search.Click += async (_, _) => await ReloadAsync(); _create.Click += (_, _) => OpenSelected();
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) OpenSelected(); };
        _grid.SelectionChanged += (_, _) => _create.Enabled = Selected is not null;
        Load += async (_, _) => { if (!IsDesigning) await ReloadAsync(); };
    }

    async Task ReloadAsync()
    {
        var result = await Task.Run(() => LoanService.SearchOpen(_keyword.Text));
        if (IsDisposed) return;
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        _grid.DataSource = result.Value;
        PageUi.Header(_grid, (nameof(LoanSlipRow.SlipCode), "Mã phiếu", 15), (nameof(LoanSlipRow.BorrowerName), "Người mượn", 20),
            (nameof(LoanSlipRow.Phone), "Điện thoại", 12), (nameof(LoanSlipRow.BorrowDate), "Ngày mượn", 12),
            (nameof(LoanSlipRow.DueDate), "Hạn trả", 12), (nameof(LoanSlipRow.TotalQty), "Tổng mượn", 9),
            (nameof(LoanSlipRow.ReturnedQty), "Đã trả", 9), (nameof(LoanSlipRow.RemainingQty), "Còn phải trả", 11),
            (nameof(LoanSlipRow.StatusText), "Trạng thái", 12));
        PageUi.HideColumns(_grid, nameof(LoanSlipRow.LoanSlipId), nameof(LoanSlipRow.BorrowerId), nameof(LoanSlipRow.BorrowerCode),
            nameof(LoanSlipRow.CreatedBy), nameof(LoanSlipRow.CreatedByName), nameof(LoanSlipRow.Status), nameof(LoanSlipRow.Note),
            nameof(LoanSlipRow.CreatedAt), nameof(LoanSlipRow.DisplayStatus), nameof(LoanSlipRow.DaysOverdue));
        _count.Text = $"{result.Value!.Count} phiếu còn phải trả"; _create.Enabled = Selected is not null;
    }

    void OpenSelected()
    {
        if (Selected is not { } row) return;
        var full = LoanService.GetFull(row.LoanSlipId);
        if (!full.Success) { PageUi.ShowError(FindForm(), full.Error); return; }
        using var dialog = new ReturnCreateDialog(full.Value!);
        if (dialog.ShowDialog(FindForm()) == DialogResult.OK) _ = ReloadAsync();
    }
}
