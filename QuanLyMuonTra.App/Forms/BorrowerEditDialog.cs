using System.Drawing;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

public sealed class BorrowerEditDialog : ThemedForm
{
    readonly BorrowerRow _model;
    readonly AppTextBox _name = new() { PlaceholderText = "Họ và tên", Dock = DockStyle.Fill };
    readonly AppTextBox _phone = new() { PlaceholderText = "Số điện thoại", Dock = DockStyle.Fill };
    readonly AppTextBox _email = new() { PlaceholderText = "Email", Dock = DockStyle.Fill };
    readonly AppTextBox _department = new() { PlaceholderText = "Đơn vị / phòng ban", Dock = DockStyle.Fill };
    readonly AppTextBox _address = new() { PlaceholderText = "Địa chỉ", Dock = DockStyle.Fill };
    readonly AppTextBox _note = new() { PlaceholderText = "Ghi chú", Dock = DockStyle.Fill };
    readonly CheckBox _active = new() { Text = "Đang hoạt động", AutoSize = true, Checked = true };

    public BorrowerEditDialog(BorrowerRow? existing)
    {
        _model = existing is null ? new BorrowerRow() : new BorrowerRow
        {
            BorrowerId = existing.BorrowerId, BorrowerCode = existing.BorrowerCode, FullName = existing.FullName,
            Phone = existing.Phone, Email = existing.Email, Department = existing.Department,
            Address = existing.Address, Note = existing.Note, IsActive = existing.IsActive, LoanCount = existing.LoanCount
        };
        Text = existing is null ? "Thêm người mượn" : "Cập nhật người mượn";
        StartPosition = FormStartPosition.CenterParent; FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false; MinimizeBox = false; ClientSize = new Size(540, 470);
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(20), ColumnCount = 1, RowCount = 9 };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
        for (int i = 0; i < 6; i++) layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 34)); layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        layout.Controls.Add(new Label { Text = Text, Font = new Font("Segoe UI", 13, FontStyle.Bold), Dock = DockStyle.Fill }, 0, 0);
        layout.Controls.Add(_name, 0, 1); layout.Controls.Add(_phone, 0, 2); layout.Controls.Add(_email, 0, 3);
        layout.Controls.Add(_department, 0, 4); layout.Controls.Add(_address, 0, 5); layout.Controls.Add(_note, 0, 6);
        layout.Controls.Add(_active, 0, 7);
        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
        var save = new AppButton { Text = "Lưu", Kind = ButtonKind.Primary, Size = new Size(100, 40) };
        var cancel = new AppButton { Text = "Hủy", Kind = ButtonKind.Secondary, Size = new Size(100, 40), DialogResult = DialogResult.Cancel };
        save.Click += SaveClick; actions.Controls.Add(save); actions.Controls.Add(cancel); layout.Controls.Add(actions, 0, 8);
        Controls.Add(layout); AcceptButton = save; CancelButton = cancel;
        _name.Text = _model.FullName; _phone.Text = _model.Phone; _email.Text = _model.Email;
        _department.Text = _model.Department; _address.Text = _model.Address; _note.Text = _model.Note; _active.Checked = _model.IsActive;
    }

    void SaveClick(object? sender, EventArgs e)
    {
        _model.FullName = _name.Text; _model.Phone = _phone.Text; _model.Email = _email.Text;
        _model.Department = _department.Text; _model.Address = _address.Text; _model.Note = _note.Text; _model.IsActive = _active.Checked;
        var result = BorrowerService.Save(_model);
        if (!result.Success) { MessageBox.Show(this, result.Error, "Không thể lưu", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        DialogResult = DialogResult.OK; Close();
    }
}
