using QuanLyMuonTra.App.Forms;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F04 - quản lý tài khoản người dùng.</summary>
public sealed class UserPage : ThemedUserControl
{
    readonly AppTextBox _keyword = PageUi.TextBox("Tìm tên đăng nhập, họ tên, email", 300);
    readonly AppButton _search = PageUi.Button("Tìm kiếm", ButtonKind.Primary, 100);
    readonly AppButton _add = PageUi.Button("Thêm tài khoản", ButtonKind.Primary, 130);
    readonly AppButton _edit = PageUi.Button("Sửa", ButtonKind.Secondary, 80);
    readonly AppButton _toggle = PageUi.Button("Khóa / Mở khóa", ButtonKind.Secondary, 130);
    readonly AppButton _reset = PageUi.Button("Đặt lại mật khẩu", ButtonKind.Secondary, 145);
    readonly DataGridView _grid = PageUi.Grid();
    UserAdminRow? Selected => _grid.CurrentRow?.DataBoundItem as UserAdminRow;

    public UserPage()
    {
        var toolbar = PageUi.Toolbar(); toolbar.Controls.Add(_keyword); toolbar.Controls.Add(_search); toolbar.Controls.Add(_add); toolbar.Controls.Add(_edit); toolbar.Controls.Add(_toggle); toolbar.Controls.Add(_reset);
        var card = PageUi.Card(); card.Controls.Add(_grid); Controls.Add(card); Controls.Add(toolbar);
        _search.Click += async (_, _) => await ReloadAsync(); _add.Click += (_, _) => Edit(null); _edit.Click += (_, _) => { if (Selected is { } u) Edit(u); };
        _toggle.Click += (_, _) => Toggle(); _reset.Click += (_, _) => Reset(); _grid.SelectionChanged += (_, _) => UpdateButtons();
        Load += async (_, _) => { if (!IsDesigning) await ReloadAsync(); };
    }

    async Task ReloadAsync()
    {
        var result = await Task.Run(() => AdminService.Users(_keyword.Text)); if (IsDisposed) return;
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        _grid.DataSource = result.Value;
        PageUi.Header(_grid, (nameof(UserAdminRow.Username), "Tài khoản", 16), (nameof(UserAdminRow.FullName), "Họ tên", 24),
            (nameof(UserAdminRow.Email), "Email", 22), (nameof(UserAdminRow.Phone), "Điện thoại", 14),
            (nameof(UserAdminRow.RoleName), "Nhóm quyền", 16), (nameof(UserAdminRow.IsActive), "Đang hoạt động", 12));
        PageUi.HideColumns(_grid, nameof(UserAdminRow.UserId), nameof(UserAdminRow.RoleId)); UpdateButtons();
    }
    void UpdateButtons() { _edit.Enabled = Selected is not null; _toggle.Enabled = Selected is not null; _reset.Enabled = Selected is not null; if (Selected is { } u) _toggle.Text = u.IsActive ? "Khóa tài khoản" : "Mở khóa"; }
    void Edit(UserAdminRow? user)
    {
        var roles = AdminService.Roles(); if (!roles.Success) { PageUi.ShowError(FindForm(), roles.Error); return; }
        using var dlg = new UserEditDialog(user, roles.Value!); if (dlg.ShowDialog(FindForm()) == DialogResult.OK) _ = ReloadAsync();
    }
    void Toggle()
    {
        if (Selected is not { } u) return;
        string action = u.IsActive ? "khóa" : "mở khóa";
        if (MessageBox.Show(FindForm(), $"Bạn có chắc muốn {action} tài khoản {u.Username}?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        var r = AdminService.SetUserActive(u.UserId, !u.IsActive); if (!r.Success) PageUi.ShowError(FindForm(), r.Error); else _ = ReloadAsync();
    }
    void Reset()
    {
        if (Selected is not { } u) return;
        using var dlg = new PasswordResetDialog(); if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;
        var r = AdminService.ResetPassword(u.UserId, dlg.NewPassword); if (!r.Success) PageUi.ShowError(FindForm(), r.Error); else MessageBox.Show(FindForm(), "Đã đặt lại mật khẩu.");
    }
}
