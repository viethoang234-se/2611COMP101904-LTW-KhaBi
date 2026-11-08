using QuanLyMuonTra.App.Forms;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F08 - quản lý hồ sơ người mượn và lịch sử giao dịch.</summary>
public sealed class BorrowerPage : ThemedUserControl
{
    readonly AppTextBox _search = PageUi.TextBox("Tìm mã, họ tên, điện thoại, email...", 330);
    readonly CheckBox _includeInactive = new() { Text = "Hiện cả người đã ngừng hoạt động", AutoSize = true, Margin = new Padding(8, 12, 8, 4) };
    readonly DataGridView _grid = PageUi.Grid();
    readonly AppButton _add = PageUi.Button("Thêm người mượn", ButtonKind.Primary, 145);
    readonly AppButton _edit = PageUi.Button("Sửa", ButtonKind.Secondary, 80);
    readonly AppButton _history = PageUi.Button("Lịch sử mượn", ButtonKind.Secondary, 120);
    readonly AppButton _delete = PageUi.Button("Xóa", ButtonKind.Danger, 80);
    readonly Label _count = PageUi.Label("0 người mượn");
    readonly System.Windows.Forms.Timer _timer = new() { Interval = 350 };

    BorrowerRow? Selected => _grid.CurrentRow?.DataBoundItem as BorrowerRow;

    public BorrowerPage()
    {
        var toolbar = PageUi.Toolbar(); toolbar.Controls.Add(_search); toolbar.Controls.Add(_includeInactive);
        toolbar.Controls.Add(_add); toolbar.Controls.Add(_edit); toolbar.Controls.Add(_history); toolbar.Controls.Add(_delete);
        var card = PageUi.Card(); card.Controls.Add(_grid);
        var footer = new ThemedPanel { Dock = DockStyle.Bottom, Height = 34, Fill = PanelFill.Surface }; _count.Dock = DockStyle.Left; footer.Controls.Add(_count); card.Controls.Add(footer);
        Controls.Add(card); Controls.Add(toolbar);
        _search.TextChanged += (_, _) => { _timer.Stop(); _timer.Start(); };
        _timer.Tick += async (_, _) => { _timer.Stop(); await ReloadAsync(); };
        _includeInactive.CheckedChanged += async (_, _) => await ReloadAsync();
        _add.Click += async (_, _) => await EditAsync(null);
        _edit.Click += async (_, _) => { if (Selected is { } row) await EditAsync(row); };
        _history.Click += async (_, _) => await ShowHistoryAsync();
        _delete.Click += async (_, _) => await DeleteAsync();
        _grid.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0 && Selected is { } row) await EditAsync(row); };
        _grid.SelectionChanged += (_, _) => UpdateButtons();
        Load += async (_, _) => { if (!IsDesigning) { UpdateButtons(); await ReloadAsync(); } };
    }

    async Task ReloadAsync()
    {
        var result = await Task.Run(() => BorrowerService.Search(_search.Text, _includeInactive.Checked));
        if (IsDisposed) return;
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        _grid.DataSource = result.Value;
        PageUi.Header(_grid, (nameof(BorrowerRow.BorrowerCode), "Mã", 13), (nameof(BorrowerRow.FullName), "Họ và tên", 23),
            (nameof(BorrowerRow.Phone), "Điện thoại", 14), (nameof(BorrowerRow.Email), "Email", 20),
            (nameof(BorrowerRow.Department), "Đơn vị", 16), (nameof(BorrowerRow.LoanCount), "Số phiếu", 8),
            (nameof(BorrowerRow.IsActive), "Hoạt động", 10));
        PageUi.HideColumns(_grid, nameof(BorrowerRow.BorrowerId), nameof(BorrowerRow.Address), nameof(BorrowerRow.Note));
        _count.Text = $"Hiển thị {result.Value!.Count} người mượn"; UpdateButtons();
    }

    void UpdateButtons()
    {
        bool has = Selected is not null;
        _edit.Enabled = _history.Enabled = _delete.Enabled = has;
    }

    async Task EditAsync(BorrowerRow? row)
    {
        using var dlg = new BorrowerEditDialog(row);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK) await ReloadAsync();
    }

    async Task DeleteAsync()
    {
        if (Selected is not { } row) return;
        if (MessageBox.Show(FindForm(), $"Xóa người mượn '{row.FullName}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        var result = await Task.Run(() => BorrowerService.Delete(row.BorrowerId));
        if (!result.Success) PageUi.ShowError(FindForm(), result.Error);
        else await ReloadAsync();
    }

    async Task ShowHistoryAsync()
    {
        if (Selected is not { } row) return;
        var result = await Task.Run(() => BorrowerService.History(row.BorrowerId));
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        using var dlg = new BorrowerHistoryDialog(row, result.Value ?? new());
        dlg.ShowDialog(FindForm());
    }
}
