using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F15 - báo cáo tình hình mượn trả và tồn kho.</summary>
public sealed class ReportPage : ThemedUserControl
{
    readonly DateTimePicker _from = new() { Format = DateTimePickerFormat.Short, Width = 130, Margin = new Padding(4) };
    readonly DateTimePicker _to = new() { Format = DateTimePickerFormat.Short, Width = 130, Margin = new Padding(4) };
    readonly AppButton _load = PageUi.Button("Xem báo cáo", ButtonKind.Primary, 115);
    readonly DataGridView _grid = PageUi.Grid();
    readonly Label _summary = PageUi.Label("Chưa có dữ liệu báo cáo", true);
    readonly Label _inventory = PageUi.Label("", true);

    public ReportPage()
    {
        var toolbar = PageUi.Toolbar(); toolbar.Controls.Add(PageUi.Label("Từ ngày")); toolbar.Controls.Add(_from); toolbar.Controls.Add(PageUi.Label("Đến ngày")); toolbar.Controls.Add(_to); toolbar.Controls.Add(_load);
        var card = PageUi.Card(); card.Controls.Add(_grid);
        var footer = new ThemedPanel { Dock = DockStyle.Bottom, Height = 75, Fill = PanelFill.Surface, Padding = new Padding(8) };
        _summary.Dock = DockStyle.Top; _inventory.Dock = DockStyle.Bottom; footer.Controls.Add(_summary); footer.Controls.Add(_inventory); card.Controls.Add(footer);
        Controls.Add(card); Controls.Add(toolbar); _from.Value = new DateTime(DateTime.Today.Year, 1, 1); _to.Value = DateTime.Today;
        _load.Click += async (_, _) => await ReloadAsync();
        Load += async (_, _) => { if (!IsDesigning) await ReloadAsync(); };
    }

    async Task ReloadAsync()
    {
        if (!Session.Has(Perm.ReportView))
        {
            _summary.Text = "Tài khoản hiện tại chỉ có quyền in phiếu/biên bản, không có quyền xem báo cáo tổng hợp.";
            _inventory.Text = ""; _grid.DataSource = null; return;
        }
        var r = await Task.Run(() => AdminService.Summary(_from.Value.Date, _to.Value.Date)); if (IsDisposed) return;
        if (!r.Success) { PageUi.ShowError(FindForm(), r.Error); return; }
        var x = r.Value!;
        _summary.Text = $"Phiếu mượn: {x.SlipCount:N0}    Số lượng mượn: {x.BorrowedQuantity:N0}    Đã trả: {x.ReturnedQuantity:N0}    Còn phải trả: {x.OutstandingQuantity:N0}    Phiếu quá hạn: {x.OverdueSlipCount:N0}";
        _inventory.Text = $"Tồn kho hiện tại - Thiết bị đang hoạt động: {x.ActiveDeviceCount:N0}    Khả dụng: {x.AvailableQuantity:N0}    Hư hỏng: {x.DamagedQuantity:N0}    Mất: {x.LostQuantity:N0}";
        var popular = await Task.Run(() => AdminService.PopularDevices(_from.Value.Date, _to.Value.Date, 10));
        if (!popular.Success) { PageUi.ShowError(FindForm(), popular.Error); return; }
        _grid.DataSource = popular.Value;
        PageUi.Header(_grid, (nameof(PopularDeviceRow.DeviceCode), "Mã thiết bị", 15), (nameof(PopularDeviceRow.DeviceName), "Thiết bị", 30),
            (nameof(PopularDeviceRow.CategoryName), "Loại", 20), (nameof(PopularDeviceRow.LoanCount), "Số phiếu", 15),
            (nameof(PopularDeviceRow.BorrowedQuantity), "SL mượn", 15));
    }
}
