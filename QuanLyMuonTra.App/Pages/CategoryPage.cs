using QuanLyMuonTra.App.Forms;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Security;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F06 - danh mục loại thiết bị.</summary>
public sealed class CategoryPage : ThemedUserControl
{
    readonly AppTextBox _search = PageUi.TextBox("Tìm mã hoặc tên loại", 280);
    readonly DataGridView _grid = PageUi.Grid();
    readonly AppButton _add = PageUi.Button("Thêm loại", ButtonKind.Primary, 110);
    readonly AppButton _edit = PageUi.Button("Sửa", ButtonKind.Secondary, 80);
    readonly AppButton _delete = PageUi.Button("Xóa", ButtonKind.Danger, 80);
    readonly Label _count = PageUi.Label("0 loại thiết bị");
    readonly System.Windows.Forms.Timer _timer = new() { Interval = 350 };

    CategoryRow? Selected => _grid.CurrentRow?.DataBoundItem as CategoryRow;

    public CategoryPage()
    {
        var toolbar = PageUi.Toolbar(); toolbar.Controls.Add(_search); toolbar.Controls.Add(_add); toolbar.Controls.Add(_edit); toolbar.Controls.Add(_delete);
        var card = PageUi.Card(); card.Controls.Add(_grid);
        var footer = new ThemedPanel { Dock = DockStyle.Bottom, Height = 34, Fill = PanelFill.Surface }; _count.Dock = DockStyle.Left; footer.Controls.Add(_count); card.Controls.Add(footer);
        Controls.Add(card); Controls.Add(toolbar);
        bool canManage = Session.Has(Perm.CategoryManage);
        _add.Visible = _edit.Visible = _delete.Visible = canManage;
        _search.TextChanged += (_, _) => { _timer.Stop(); _timer.Start(); };
        _timer.Tick += async (_, _) => { _timer.Stop(); await ReloadAsync(); };
        _add.Click += async (_, _) => await EditAsync(null);
        _edit.Click += async (_, _) => { if (Selected is { } row) await EditAsync(row); };
        _delete.Click += async (_, _) => await DeleteAsync();
        _grid.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0 && canManage && Selected is { } row) await EditAsync(row); };
        _grid.SelectionChanged += (_, _) => UpdateButtons();
        Load += async (_, _) => { if (!IsDesigning) { UpdateButtons(); await ReloadAsync(); } };
    }

    async Task ReloadAsync()
    {
        var result = await Task.Run(() => CategoryService.Search(_search.Text));
        if (IsDisposed) return;
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        _grid.DataSource = result.Value;
        PageUi.Header(_grid, (nameof(CategoryRow.CategoryCode), "Mã loại", 18), (nameof(CategoryRow.CategoryName), "Tên loại thiết bị", 34),
            (nameof(CategoryRow.Description), "Mô tả", 30), (nameof(CategoryRow.DeviceCount), "Số thiết bị", 14));
        PageUi.HideColumns(_grid, nameof(CategoryRow.CategoryId));
        _count.Text = $"Hiển thị {result.Value!.Count} loại thiết bị"; UpdateButtons();
    }

    void UpdateButtons() { bool has = Selected is not null; _edit.Enabled = _delete.Enabled = has; }

    async Task EditAsync(CategoryRow? row)
    {
        using var dlg = new CategoryEditDialog(row);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK) await ReloadAsync();
    }

    async Task DeleteAsync()
    {
        if (Selected is not { } row) return;
        if (MessageBox.Show(FindForm(), $"Xóa loại thiết bị '{row.CategoryName}'?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        var result = await Task.Run(() => CategoryService.Delete(row.CategoryId));
        if (!result.Success) PageUi.ShowError(FindForm(), result.Error);
        else await ReloadAsync();
    }
}
