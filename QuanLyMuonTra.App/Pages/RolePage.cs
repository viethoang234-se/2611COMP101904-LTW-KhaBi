using QuanLyMuonTra.App.Forms;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F05 - quản lý nhóm quyền và quyền chức năng.</summary>
public sealed class RolePage : ThemedUserControl
{
    readonly ListBox _roles = new() { Dock = DockStyle.Fill, DisplayMember = nameof(RoleRow.RoleName) };
    readonly CheckedListBox _permissions = new() { Dock = DockStyle.Fill, CheckOnClick = true, DisplayMember = nameof(PermissionRow.Name) };
    readonly AppButton _add = PageUi.Button("Thêm nhóm", ButtonKind.Primary, 105);
    readonly AppButton _edit = PageUi.Button("Sửa tên", ButtonKind.Secondary, 90);
    readonly AppButton _save = PageUi.Button("Lưu quyền", ButtonKind.Primary, 110);
    List<RoleRow> _roleRows = new();
    List<PermissionRow> _permissionRows = new();
    RoleRow? Selected => _roles.SelectedItem as RoleRow;

    public RolePage()
    {
        var left = new ThemedPanel { Dock = DockStyle.Fill, Padding = new Padding(8), Fill = PanelFill.Surface };
        var leftTools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50 }; leftTools.Controls.Add(_add); leftTools.Controls.Add(_edit); left.Controls.Add(_roles); left.Controls.Add(leftTools);
        var right = new ThemedPanel { Dock = DockStyle.Fill, Padding = new Padding(8), Fill = PanelFill.Surface };
        var rightTools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 50 }; rightTools.Controls.Add(PageUi.Label("Quyền được cấp cho nhóm đang chọn", true)); rightTools.Controls.Add(_save);
        right.Controls.Add(_permissions); right.Controls.Add(rightTools);
        var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Vertical, SplitterDistance = 320, Panel1MinSize = 240, Panel2MinSize = 350 };
        split.Panel1.Controls.Add(left); split.Panel2.Controls.Add(right); Controls.Add(split);
        _roles.SelectedIndexChanged += async (_, _) => await LoadRolePermissionsAsync();
        _add.Click += (_, _) => EditRole(null); _edit.Click += (_, _) => { if (Selected is { } r) EditRole(r); };
        _save.Click += (_, _) => SavePermissions();
        Load += async (_, _) => { if (!IsDesigning) await InitializeAsync(); };
    }

    async Task InitializeAsync()
    {
        var p = await Task.Run(() => AdminService.Permissions()); if (!p.Success) { PageUi.ShowError(FindForm(), p.Error); return; }
        _permissionRows = p.Value!; _permissions.DataSource = _permissionRows;
        await ReloadRolesAsync();
    }

    async Task ReloadRolesAsync()
    {
        int? selectedId = Selected?.RoleId;
        var r = await Task.Run(() => AdminService.Roles()); if (!r.Success) { PageUi.ShowError(FindForm(), r.Error); return; }
        _roleRows = r.Value!; _roles.DataSource = null; _roles.DataSource = _roleRows;
        if (selectedId.HasValue) _roles.SelectedItem = _roleRows.FirstOrDefault(x => x.RoleId == selectedId.Value);
        if (_roles.SelectedIndex < 0 && _roleRows.Count > 0) _roles.SelectedIndex = 0;
        _edit.Enabled = Selected is not null; _save.Enabled = Selected is not null;
    }

    async Task LoadRolePermissionsAsync()
    {
        _edit.Enabled = Selected is not null; _save.Enabled = Selected is not null;
        if (Selected is not { } role) { for (int i = 0; i < _permissions.Items.Count; i++) _permissions.SetItemChecked(i, false); return; }
        var r = await Task.Run(() => AdminService.RolePermissionIds(role.RoleId)); if (!r.Success) { PageUi.ShowError(FindForm(), r.Error); return; }
        for (int i = 0; i < _permissionRows.Count; i++) _permissions.SetItemChecked(i, r.Value!.Contains(_permissionRows[i].PermissionId));
    }

    void EditRole(RoleRow? role)
    {
        using var dlg = new RoleEditDialog(role?.RoleName, role?.Description);
        if (dlg.ShowDialog(FindForm()) != DialogResult.OK) return;
        var r = AdminService.SaveRole(role?.RoleId, dlg.RoleName, dlg.Description);
        if (!r.Success) { PageUi.ShowError(FindForm(), r.Error); return; }
        _ = ReloadRolesAsync();
    }

    void SavePermissions()
    {
        if (Selected is not { } role) return;
        var ids = _permissions.CheckedItems.Cast<PermissionRow>().Select(x => x.PermissionId).ToArray();
        if (MessageBox.Show(FindForm(), $"Lưu {ids.Length} quyền cho nhóm {role.RoleName}?", "Xác nhận", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
        var r = AdminService.SaveRolePermissions(role.RoleId, ids); if (!r.Success) PageUi.ShowError(FindForm(), r.Error); else MessageBox.Show(FindForm(), "Đã cập nhật phân quyền.");
    }
}
