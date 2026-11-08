using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

public sealed class PasswordResetDialog : ThemedForm
{
    public string NewPassword => _password.Text;
    readonly TextBox _password = new() { Dock = DockStyle.Top, UseSystemPasswordChar = true, MaxLength = 100 };
    public PasswordResetDialog()
    {
        Text = "Đặt lại mật khẩu"; StartPosition = FormStartPosition.CenterParent; Size = new Size(390, 170); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
        var label = new Label { Text = "Mật khẩu mới (ít nhất 8 ký tự)", Dock = DockStyle.Top, Height = 28, Padding = new Padding(8) };
        var footer = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 52, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        var ok = new AppButton { Text = "Xác nhận", Kind = ButtonKind.Primary, Width = 105, Height = 36 }; var cancel = new AppButton { Text = "Hủy", Width = 80, Height = 36 };
        footer.Controls.Add(ok); footer.Controls.Add(cancel); Controls.Add(_password); Controls.Add(label); Controls.Add(footer);
        ok.Click += (_, _) => { if (_password.Text.Length < 8) { MessageBox.Show(this, "Mật khẩu phải có ít nhất 8 ký tự."); return; } DialogResult = DialogResult.OK; Close(); };
        cancel.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
    }
}
