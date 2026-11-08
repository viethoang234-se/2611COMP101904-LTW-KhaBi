using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

public sealed class RoleEditDialog : ThemedForm
{
    public string RoleName => _name.Text.Trim();
    public string Description => _description.Text.Trim();
    readonly TextBox _name = new() { Dock = DockStyle.Top, MaxLength = 50 };
    readonly TextBox _description = new() { Dock = DockStyle.Top, MaxLength = 200 };
    public RoleEditDialog(string? name = null, string? description = null)
    {
        Text = "Thông tin nhóm quyền"; StartPosition = FormStartPosition.CenterParent; Size = new Size(430, 220); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
        _name.Text = name ?? ""; _description.Text = description ?? "";
        var body = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(12), ColumnCount = 1, RowCount = 4 };
        body.Controls.Add(new Label { Text = "Tên nhóm quyền", AutoSize = true }); body.Controls.Add(_name);
        body.Controls.Add(new Label { Text = "Mô tả", AutoSize = true }); body.Controls.Add(_description);
        var footer = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        var ok = new AppButton { Text = "Lưu", Kind = ButtonKind.Primary, Width = 90, Height = 36 }; footer.Controls.Add(ok);
        Controls.Add(body); Controls.Add(footer); ok.Click += (_, _) => { if (RoleName.Length < 2) { MessageBox.Show(this, "Tên nhóm quyền tối thiểu 2 ký tự."); return; } DialogResult = DialogResult.OK; Close(); };
    }
}
