using QuanLyMuonTra.App.Forms;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F07 - Quản lý thiết bị: tìm kiếm, lọc theo loại, thêm / sửa / xóa. Đây là MẪU cho các trang danh sách khác.</summary>
public partial class DevicePage : ThemedUserControl
{
    bool _loadingCategories;

    public DevicePage()
    {
        InitializeComponent();

        txtSearch.TextChanged += (_, _) => { timerSearch.Stop(); timerSearch.Start(); };
        timerSearch.Tick += async (_, _) => { timerSearch.Stop(); await ReloadAsync(); };
        cboCategory.SelectedIndexChanged += async (_, _) => { if (!_loadingCategories) await ReloadAsync(); };

        btnAdd.Click += async (_, _) => await OpenDialogAsync(null);
        btnEdit.Click += async (_, _) => await EditSelectedAsync();
        btnDelete.Click += async (_, _) => await DeleteSelectedAsync();
        dgvDevices.CellDoubleClick += async (_, e) => { if (e.RowIndex >= 0) await EditSelectedAsync(); };
        dgvDevices.SelectionChanged += (_, _) => UpdateButtons();

        Load += async (_, _) =>
        {
            if (IsDesigning) return;
            AppGrid.Apply(dgvDevices);
            UpdateButtons();
            try
            {
                LoadCategories();
                await ReloadAsync();
            }
            catch (Exception ex)
            {
                ShowError("Không tải được dữ liệu thiết bị:\n" + ex.Message);
            }
        };
    }

    private DeviceRow? Selected() => dgvDevices.CurrentRow?.DataBoundItem as DeviceRow;

    private void LoadCategories()
    {
        var result = DeviceService.Categories();
        if (!result.Success) throw new InvalidOperationException(result.Error);
        var categories = result.Value ?? new List<CategoryItem>();
        categories.Insert(0, new CategoryItem { CategoryId = 0, CategoryName = "Tất cả loại" });

        _loadingCategories = true;
        cboCategory.DataSource = categories;
        cboCategory.SelectedIndex = 0;
        _loadingCategories = false;
    }

    private async Task ReloadAsync()
    {
        string keyword = txtSearch.Text;
        int? categoryId = cboCategory.SelectedValue is int id && id > 0 ? id : null;
        int? keepId = Selected()?.DeviceId;

        try
        {
            var result = await Task.Run(() => DeviceService.Search(keyword, categoryId));
            if (IsDisposed) return;
            if (!result.Success) { ShowError(result.Error); return; }
            var rows = result.Value ?? new List<DeviceRow>();

            dgvDevices.DataSource = rows;
            lblCount.Text = $"Hiển thị {rows.Count} thiết bị";

            if (keepId != null)
            {
                foreach (DataGridViewRow row in dgvDevices.Rows)
                {
                    if (row.DataBoundItem is DeviceRow d && d.DeviceId == keepId)
                    {
                        dgvDevices.CurrentCell = row.Cells[0];
                        break;
                    }
                }
            }
            UpdateButtons();
        }
        catch (Exception ex)
        {
            ShowError("Không tải được danh sách thiết bị:\n" + ex.Message);
        }
    }

    private void UpdateButtons()
    {
        bool has = Selected() != null;
        btnEdit.Enabled = has;
        btnDelete.Enabled = has;
    }

    private async Task EditSelectedAsync()
    {
        var row = Selected();
        if (row != null) await OpenDialogAsync(row);
    }

    private async Task OpenDialogAsync(DeviceRow? existing)
    {
        using var dialog = new DeviceEditDialog(existing);
        if (dialog.ShowDialog(FindForm()) == DialogResult.OK)
            await ReloadAsync();
    }

    private async Task DeleteSelectedAsync()
    {
        var row = Selected();
        if (row is null) return;

        var confirm = MessageBox.Show(FindForm(),
            $"Xóa thiết bị \"{row.DeviceName}\" ({row.DeviceCode})?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
        if (confirm != DialogResult.Yes) return;

        try
        {
            var result = await Task.Run(() => DeviceService.Delete(row.DeviceId));
            if (!result.Success)
            {
                MessageBox.Show(FindForm(), result.Error, "Không thể xóa", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            await ReloadAsync();
        }
        catch (Exception ex)
        {
            ShowError("Không xóa được thiết bị:\n" + ex.Message);
        }
    }

    private void ShowError(string message) =>
        MessageBox.Show(FindForm(), message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
}
