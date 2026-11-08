using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

/// <summary>Hộp thoại thêm / sửa thiết bị (dùng cho F07).</summary>
public partial class DeviceEditDialog : ThemedForm
{
    private readonly DeviceRow? _existing;

    // Constructor không tham số để Visual Studio mở được Designer.
    public DeviceEditDialog()
    {
        InitializeComponent();
        AcceptButton = btnSave;
        CancelButton = btnCancel;
        btnSave.Click += BtnSave_Click;
        Shown += (_, _) => txtCode.FocusInput();
    }

    public DeviceEditDialog(DeviceRow? existing) : this()
    {
        _existing = existing;
        if (IsDesigning) return;

        var categories = DeviceService.Categories();
        if (!categories.Success) { ShowError(categories.Error); return; }
        cboCategory.DataSource = categories.Value;

        if (existing is null)
        {
            Text = lblTitle.Text = "Thêm thiết bị";
            var suggested = DeviceService.SuggestCode();
            if (suggested.Success) txtCode.Text = suggested.Value ?? "";
            else ShowError(suggested.Error);
            nudQuantity.Value = 1;
            txtUnit.Text = "Cái";
        }
        else
        {
            Text = lblTitle.Text = "Sửa thiết bị";
            txtCode.Text = existing.DeviceCode;
            txtName.Text = existing.DeviceName;
            cboCategory.SelectedValue = existing.CategoryId;
            nudQuantity.Value = Math.Min(nudQuantity.Maximum, existing.TotalQuantity);
            txtUnit.Text = existing.Unit;
            txtNote.Text = existing.Note;
        }
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        string code = txtCode.Text.Trim();
        string name = txtName.Text.Trim();

        if (code.Length == 0) { ShowError("Vui lòng nhập mã thiết bị."); txtCode.FocusInput(); return; }
        if (name.Length == 0) { ShowError("Vui lòng nhập tên thiết bị."); txtName.FocusInput(); return; }
        if (cboCategory.SelectedValue is not int categoryId) { ShowError("Vui lòng chọn loại thiết bị."); return; }

        var row = new DeviceRow
        {
            DeviceId = _existing?.DeviceId ?? 0,
            DeviceCode = code,
            DeviceName = name,
            CategoryId = categoryId,
            TotalQuantity = (int)nudQuantity.Value,
            Unit = txtUnit.Text.Trim(),
            Note = txtNote.Text.Trim(),
        };

        btnSave.Enabled = false;
        lblError.Text = "";
        try
        {
            var result = await Task.Run(() => DeviceService.Save(row));
            if (!result.Success) { ShowError(result.Error); return; }
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            ShowError("Không lưu được: " + ex.Message);
        }
        finally
        {
            if (!IsDisposed) btnSave.Enabled = true;
        }
    }

    private void ShowError(string message) => lblError.Text = message;
}
