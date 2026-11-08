using QuanLyMuonTra.App.Forms;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.App.Security;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F13 - tra cứu, xem chi tiết và hủy biên bản trả theo quyền.</summary>
public sealed class ReturnRecordPage : ThemedUserControl
{
    sealed record CancelOption(bool? Value, string Name);
    readonly AppTextBox _keyword = PageUi.TextBox("Mã biên bản, mã phiếu, người mượn", 300);
    readonly DateTimePicker _from = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Width = 125, Margin = new Padding(4) };
    readonly DateTimePicker _to = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Width = 125, Margin = new Padding(4) };
    readonly ComboBox _status = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 135, Margin = new Padding(4), DisplayMember = nameof(CancelOption.Name), ValueMember = nameof(CancelOption.Value) };
    readonly AppButton _search = PageUi.Button("Tìm kiếm", ButtonKind.Primary, 105);
    readonly AppButton _detail = PageUi.Button("Chi tiết", ButtonKind.Secondary, 95);
    readonly AppButton _cancel = PageUi.Button("Hủy biên bản", ButtonKind.Danger, 125);
    readonly DataGridView _grid = PageUi.Grid();
    ReturnRecordRow? Selected => _grid.CurrentRow?.DataBoundItem as ReturnRecordRow;

    public ReturnRecordPage()
    {
        var toolbar = PageUi.Toolbar(); toolbar.Controls.Add(_keyword); toolbar.Controls.Add(PageUi.Label("Từ ngày")); toolbar.Controls.Add(_from);
        toolbar.Controls.Add(PageUi.Label("Đến ngày")); toolbar.Controls.Add(_to); toolbar.Controls.Add(_status); toolbar.Controls.Add(_search); toolbar.Controls.Add(_detail); toolbar.Controls.Add(_cancel);
        var card = PageUi.Card(); card.Controls.Add(_grid); Controls.Add(card); Controls.Add(toolbar);
        _status.DataSource = new List<CancelOption> { new(null, "Tất cả"), new(false, "Hiệu lực"), new(true, "Đã hủy") };
        _search.Click += async (_, _) => await ReloadAsync(); _detail.Click += (_, _) => OpenDetail(); _cancel.Click += (_, _) => CancelSelected();
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) OpenDetail(); };
        _grid.SelectionChanged += (_, _) => UpdateButtons();
        Load += async (_, _) => { if (!IsDesigning) await ReloadAsync(); };
    }

    async Task ReloadAsync()
    {
        if (_from.Checked && _to.Checked && _from.Value.Date > _to.Value.Date)
        { PageUi.ShowError(FindForm(), "Khoảng ngày không hợp lệ."); return; }
        var opt = _status.SelectedItem as CancelOption;
        var result = await Task.Run(() => ReturnService.Search(new RecordFilter { Keyword = _keyword.Text,
            From = _from.Checked ? _from.Value.Date : null, To = _to.Checked ? _to.Value.Date : null, Cancelled = opt?.Value }));
        if (IsDisposed) return;
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        _grid.DataSource = result.Value;
        PageUi.Header(_grid, (nameof(ReturnRecordRow.RecordCode), "Mã biên bản", 15), (nameof(ReturnRecordRow.SlipCode), "Mã phiếu", 14),
            (nameof(ReturnRecordRow.BorrowerName), "Người mượn", 20), (nameof(ReturnRecordRow.ReturnDate), "Ngày trả", 15),
            (nameof(ReturnRecordRow.ReceivedByName), "Người nhận", 16), (nameof(ReturnRecordRow.GiverName), "Người giao", 14),
            (nameof(ReturnRecordRow.TotalQuantity), "Số lượng", 9), (nameof(ReturnRecordRow.StatusText), "Trạng thái", 12));
        PageUi.HideColumns(_grid, nameof(ReturnRecordRow.ReturnRecordId), nameof(ReturnRecordRow.LoanSlipId), nameof(ReturnRecordRow.ReceivedBy),
            nameof(ReturnRecordRow.Note), nameof(ReturnRecordRow.IsCancelled), nameof(ReturnRecordRow.CancelledByName), nameof(ReturnRecordRow.CancelledAt), nameof(ReturnRecordRow.CancelReason));
        UpdateButtons();
    }

    void UpdateButtons()
    {
        _detail.Enabled = Selected is not null;
        _cancel.Enabled = Selected is { IsCancelled: false } && Session.Has(Perm.RecordCancel);
    }

    void OpenDetail()
    {
        if (Selected is not { } row) return;
        var result = ReturnService.GetFull(row.ReturnRecordId);
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        using var dialog = new ReturnRecordDetailDialog(result.Value!); dialog.ShowDialog(FindForm());
    }

    void CancelSelected()
    {
        if (Selected is not { IsCancelled: false } row) return;
        string? reason = AskCancelReason();
        if (string.IsNullOrWhiteSpace(reason)) return;
        if (MessageBox.Show(FindForm(), $"Xác nhận hủy biên bản {row.RecordCode}? Thao tác sẽ hoàn tác số lượng đã trả nếu tồn kho cho phép.",
            "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
        var result = ReturnService.Cancel(row.ReturnRecordId, reason);
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        _ = ReloadAsync();
    }

    string? AskCancelReason()
    {
        using var dialog = new ThemedForm { Text = "Lý do hủy biên bản", StartPosition = FormStartPosition.CenterParent, Size = new Size(480, 190), FormBorderStyle = FormBorderStyle.FixedDialog, MaximizeBox = false, MinimizeBox = false };
        var input = new TextBox { Dock = DockStyle.Top, MaxLength = 255, Margin = new Padding(12) };
        var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 54, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        var ok = PageUi.Button("Xác nhận", ButtonKind.Primary, 105); var cancel = PageUi.Button("Bỏ qua", ButtonKind.Secondary, 90);
        buttons.Controls.Add(ok); buttons.Controls.Add(cancel);
        dialog.Controls.Add(input); dialog.Controls.Add(buttons);
        ok.Click += (_, _) => { if (input.Text.Trim().Length < 3) { PageUi.ShowError(dialog, "Lý do hủy phải có ít nhất 3 ký tự."); return; } dialog.DialogResult = DialogResult.OK; dialog.Close(); };
        cancel.Click += (_, _) => { dialog.DialogResult = DialogResult.Cancel; dialog.Close(); };
        return dialog.ShowDialog(FindForm()) == DialogResult.OK ? input.Text.Trim() : null;
    }
}
