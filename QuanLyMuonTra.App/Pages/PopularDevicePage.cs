using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F18 - thống kê thiết bị được mượn nhiều.</summary>
public sealed class PopularDevicePage : ThemedUserControl
{
    readonly DateTimePicker _from = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Width = 130, Margin = new Padding(4) };
    readonly DateTimePicker _to = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Width = 130, Margin = new Padding(4) };
    readonly NumericUpDown _top = new() { Minimum = 1, Maximum = 100, Value = 10, Width = 75, Margin = new Padding(4) };
    readonly AppButton _search = PageUi.Button("Thống kê", ButtonKind.Primary, 100);
    readonly DataGridView _grid = PageUi.Grid();
    public PopularDevicePage()
    {
        var toolbar = PageUi.Toolbar(); toolbar.Controls.Add(PageUi.Label("Từ ngày")); toolbar.Controls.Add(_from); toolbar.Controls.Add(PageUi.Label("Đến ngày")); toolbar.Controls.Add(_to);
        toolbar.Controls.Add(PageUi.Label("Top")); toolbar.Controls.Add(_top); toolbar.Controls.Add(_search);
        var card = PageUi.Card(); card.Controls.Add(_grid); Controls.Add(card); Controls.Add(toolbar);
        _search.Click += async (_, _) => await ReloadAsync(); Load += async (_, _) => { if (!IsDesigning) await ReloadAsync(); };
    }
    async Task ReloadAsync()
    {
        var r = await Task.Run(() => AdminService.PopularDevices(_from.Checked ? _from.Value.Date : null, _to.Checked ? _to.Value.Date : null, (int)_top.Value));
        if (IsDisposed) return; if (!r.Success) { PageUi.ShowError(FindForm(), r.Error); return; }
        _grid.DataSource = r.Value;
        PageUi.Header(_grid, (nameof(PopularDeviceRow.DeviceCode), "Mã thiết bị", 15), (nameof(PopularDeviceRow.DeviceName), "Thiết bị", 30),
            (nameof(PopularDeviceRow.CategoryName), "Loại", 20), (nameof(PopularDeviceRow.LoanCount), "Số phiếu mượn", 15),
            (nameof(PopularDeviceRow.BorrowedQuantity), "Tổng số lượng mượn", 20));
    }
}
