using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F16 - cấu hình tiền tố và quy tắc sinh mã tự động.</summary>
public sealed class CodeConfigPage : ThemedUserControl
{
    readonly DataGridView _grid = PageUi.Grid();
    readonly TextBox _prefix = new() { Width = 100, MaxLength = 10, Margin = new Padding(4) };
    readonly NumericUpDown _length = new() { Minimum = 1, Maximum = 9, Width = 80, Margin = new Padding(4) };
    readonly CheckBox _useDate = new() { Text = "Chèn ngày vào mã", AutoSize = true, Margin = new Padding(8, 12, 8, 4) };
    readonly CheckBox _reset = new() { Text = "Đặt lại số mỗi ngày", AutoSize = true, Margin = new Padding(8, 12, 8, 4) };
    readonly Label _preview = PageUi.Label("Mã xem trước: -", true);
    readonly AppButton _previewButton = PageUi.Button("Xem mã tiếp theo", ButtonKind.Secondary, 145);
    readonly AppButton _save = PageUi.Button("Lưu cấu hình", ButtonKind.Primary, 125);
    CodeConfigRow? Selected => _grid.CurrentRow?.DataBoundItem as CodeConfigRow;

    public CodeConfigPage()
    {
        var card = PageUi.Card(); card.Controls.Add(_grid);
        var editor = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 108, Padding = new Padding(8), WrapContents = true };
        editor.Controls.Add(PageUi.Label("Tiền tố")); editor.Controls.Add(_prefix); editor.Controls.Add(PageUi.Label("Độ dài số")); editor.Controls.Add(_length);
        editor.Controls.Add(_useDate); editor.Controls.Add(_reset); editor.Controls.Add(_previewButton); editor.Controls.Add(_save); editor.Controls.Add(_preview);
        card.Controls.Add(editor); Controls.Add(card);
        _grid.SelectionChanged += (_, _) => LoadSelected(); _previewButton.Click += (_, _) => Preview(); _save.Click += (_, _) => Save();
        Load += async (_, _) => { if (!IsDesigning) await ReloadAsync(); };
    }

    async Task ReloadAsync()
    {
        var r = await Task.Run(() => AdminService.CodeConfigs()); if (IsDisposed) return;
        if (!r.Success) { PageUi.ShowError(FindForm(), r.Error); return; }
        _grid.DataSource = r.Value;
        PageUi.Header(_grid, (nameof(CodeConfigRow.EntityName), "Đối tượng", 20), (nameof(CodeConfigRow.Prefix), "Tiền tố", 12),
            (nameof(CodeConfigRow.UseDate), "Có ngày", 10), (nameof(CodeConfigRow.NumberLength), "Độ dài số", 10),
            (nameof(CodeConfigRow.CurrentNumber), "Số hiện tại", 12), (nameof(CodeConfigRow.ResetDaily), "Reset mỗi ngày", 12),
            (nameof(CodeConfigRow.LastResetDate), "Lần reset", 16));
        PageUi.HideColumns(_grid, nameof(CodeConfigRow.ConfigId)); LoadSelected();
    }

    void LoadSelected()
    {
        if (Selected is not { } x) return;
        _prefix.Text = x.Prefix; _length.Value = x.NumberLength; _useDate.Checked = x.UseDate; _reset.Checked = x.ResetDaily; _preview.Text = "Mã xem trước: -";
    }

    void Preview()
    {
        if (Selected is not { } x) return;
        var r = AdminService.PreviewCode(x.EntityName); _preview.Text = r.Success ? $"Mã xem trước: {r.Value}" : r.Error;
    }

    void Save()
    {
        if (Selected is not { } x) return;
        x.Prefix = _prefix.Text; x.NumberLength = (int)_length.Value; x.UseDate = _useDate.Checked; x.ResetDaily = _reset.Checked;
        var r = AdminService.SaveCodeConfig(x); if (!r.Success) { PageUi.ShowError(FindForm(), r.Error); return; }
        MessageBox.Show(FindForm(), "Đã lưu cấu hình mã."); _ = ReloadAsync();
    }
}
