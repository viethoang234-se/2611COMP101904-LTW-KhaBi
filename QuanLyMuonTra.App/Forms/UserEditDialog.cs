using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Pages;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

public sealed class UserEditDialog : ThemedForm
{
    readonly UserAdminRow? _user;
    readonly TextBox _username = new() { Width = 250 };
    readonly TextBox _password = new() { Width = 250, UseSystemPasswordChar = true };
    readonly TextBox _name = new() { Width = 250 };
    readonly TextBox _email = new() { Width = 250 };
    readonly TextBox _phone = new() { Width = 250 };
    readonly ComboBox _role = new() { Width = 250, DropDownStyle = ComboBoxStyle.DropDownList, DisplayMember = nameof(RoleRow.RoleName), ValueMember = nameof(RoleRow.RoleId) };

    public UserEditDialog(UserAdminRow? user, List<RoleRow> roles)
    {
        _user = user; Text = user is null ? "Tạo tài khoản" : "Sửa tài khoản"; StartPosition = FormStartPosition.CenterParent;
        Size = new Size(440, user is null ? 430 : 390); FormBorderStyle = FormBorderStyle.FixedDialog; MaximizeBox = false; MinimizeBox = false;
        _role.DataSource = roles;
        if (user is not null)
        {
            _username.Text = user.Username; _username.ReadOnly = true; _name.Text = user.FullName; _email.Text = user.Email; _phone.Text = user.Phone; _role.SelectedValue = user.RoleId;
        }
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 2, RowCount = 0 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115)); layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        Add(layout, "Tên đăng nhập", _username); if (user is null) Add(layout, "Mật khẩu", _password);
        Add(layout, "Họ và tên", _name); Add(layout, "Email", _email); Add(layout, "Điện thoại", _phone); Add(layout, "Nhóm quyền", _role);
        var save = new AppButton { Text = "Lưu", Kind = ButtonKind.Primary, Width = 100, Height = 38 };
        var footer = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 55, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) }; footer.Controls.Add(save);
        Controls.Add(layout); Controls.Add(footer); save.Click += (_, _) => Save();
    }

    static void Add(TableLayoutPanel p, string label, Control c)
    {
        int r = p.RowCount;
        p.RowCount++;
        p.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        p.Controls.Add(new Label { Text = label, AutoSize = true, Anchor = AnchorStyles.Left, Margin = new Padding(4, 14, 4, 4) }, 0, r);
        c.Margin = new Padding(4, 7, 4, 4); c.Anchor = AnchorStyles.Left | AnchorStyles.Right; p.Controls.Add(c, 1, r);
    }

    void Save()
    {
        if (_role.SelectedValue is not int roleId) { PageUi.ShowError(this, "Vui lòng chọn nhóm quyền."); return; }
        var result = _user is null
            ? AdminService.CreateUser(_username.Text, _password.Text, _name.Text, _email.Text, _phone.Text, roleId)
            : AdminService.UpdateUser(_user.UserId, _name.Text, _email.Text, _phone.Text, roleId);
        if (!result.Success) { PageUi.ShowError(this, result.Error); return; }
        DialogResult = DialogResult.OK; Close();
    }
}
