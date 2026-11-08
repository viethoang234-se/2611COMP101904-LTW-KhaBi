using System.Drawing;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

public sealed class CategoryEditDialog : ThemedForm
{
    readonly AppTextBox _code = new() { PlaceholderText = "Mã loại", Dock = DockStyle.Top, Height = 42 };
    readonly AppTextBox _name = new() { PlaceholderText = "Tên loại thiết bị", Dock = DockStyle.Top, Height = 42 };
    readonly AppTextBox _description = new() { PlaceholderText = "Mô tả", Dock = DockStyle.Top, Height = 42 };
    readonly CategoryRow _model;

    public CategoryEditDialog(CategoryRow? existing)
    {
        _model = existing is null ? new CategoryRow() : new CategoryRow
        {
            CategoryId = existing.CategoryId, CategoryCode = existing.CategoryCode,
            CategoryName = existing.CategoryName, Description = existing.Description, DeviceCount = existing.DeviceCount
        };
        Text = existing is null ? "Thêm loại thiết bị" : "Cập nhật loại thiết bị";
        StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; MinimizeBox = false; ClientSize = new Size(460, 300);
        var body = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 1, RowCount = 5 };
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        body.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        body.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        body.Controls.Add(new Label { Text = Text, Dock = DockStyle.Fill, Font = new Font("Segoe UI", 13, FontStyle.Bold) }, 0, 0);
        body.Controls.Add(_code, 0, 1); body.Controls.Add(_name, 0, 2); body.Controls.Add(_description, 0, 3);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var save = new AppButton { Text = "Lưu", Kind = ButtonKind.Primary, Size = new Size(100, 40) };
        var cancel = new AppButton { Text = "Hủy", Kind = ButtonKind.Secondary, Size = new Size(100, 40), DialogResult = DialogResult.Cancel };
        save.Click += SaveClick; actions.Controls.Add(save); actions.Controls.Add(cancel); body.Controls.Add(actions, 0, 4);
        Controls.Add(body); AcceptButton = save; CancelButton = cancel;
        _code.Text = _model.CategoryCode; _name.Text = _model.CategoryName; _description.Text = _model.Description;
    }

    void SaveClick(object? sender, EventArgs e)
    {
        _model.CategoryCode = _code.Text; _model.CategoryName = _name.Text; _model.Description = _description.Text;
        var result = CategoryService.Save(_model);
        if (!result.Success) { MessageBox.Show(this, result.Error, "Không thể lưu", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        DialogResult = DialogResult.OK; Close();
    }
}
