using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F14 - danh sách phiếu quá hạn từ stored procedure.</summary>
public sealed class OverduePage : ThemedUserControl
{
    readonly AppTextBox _keyword = PageUi.TextBox("Mã phiếu, người mượn, điện thoại", 320);
    readonly NumericUpDown _minDays = new() { Minimum = 0, Maximum = 3650, Width = 100, Value = 0, Margin = new Padding(4) };
    readonly AppButton _search = PageUi.Button("Lọc danh sách", ButtonKind.Primary, 120);
    readonly DataGridView _grid = PageUi.Grid();
    readonly Label _summary = PageUi.Label("0 phiếu quá hạn");

    public OverduePage()
    {
        var toolbar = PageUi.Toolbar(); toolbar.Controls.Add(_keyword); toolbar.Controls.Add(PageUi.Label("Quá hạn tối thiểu (ngày)")); toolbar.Controls.Add(_minDays); toolbar.Controls.Add(_search);
        var card = PageUi.Card(); card.Controls.Add(_grid);
        var footer = new ThemedPanel { Dock = DockStyle.Bottom, Height = 34, Fill = PanelFill.Surface }; _summary.Dock = DockStyle.Left; footer.Controls.Add(_summary); card.Controls.Add(footer);
        Controls.Add(card); Controls.Add(toolbar);
        _search.Click += async (_, _) => await ReloadAsync();
        _keyword.KeyDown += async (_, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await ReloadAsync(); } };
        Load += async (_, _) => { if (!IsDesigning) await ReloadAsync(); };
    }

    async Task ReloadAsync()
    {
        int? min = _minDays.Value == 0 ? null : (int)_minDays.Value;
        var result = await Task.Run(() => OverdueService.Get(_keyword.Text, min));
        if (IsDisposed) return;
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        _grid.DataSource = result.Value;
        PageUi.Header(_grid, (nameof(OverdueRow.SlipCode), "Mã phiếu", 14), (nameof(OverdueRow.BorrowerName), "Người mượn", 18),
            (nameof(OverdueRow.Phone), "Điện thoại", 12), (nameof(OverdueRow.BorrowDate), "Ngày mượn", 12),
            (nameof(OverdueRow.DueDate), "Hạn trả", 12), (nameof(OverdueRow.DaysOverdue), "Số ngày trễ", 10),
            (nameof(OverdueRow.UnreturnedQty), "SL chưa trả", 10), (nameof(OverdueRow.UnreturnedDevices), "Thiết bị chưa trả", 25));
        PageUi.HideColumns(_grid, nameof(OverdueRow.LoanSlipId), nameof(OverdueRow.BorrowerId));
        _summary.Text = $"Tìm thấy {result.Value!.Count} phiếu quá hạn";
    }
}
