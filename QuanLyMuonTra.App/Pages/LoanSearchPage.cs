using QuanLyMuonTra.App.Forms;
using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F10 - tra cứu phiếu mượn; mở chi tiết F11 bằng nút hoặc nhấp đúp.</summary>
public sealed class LoanSearchPage : ThemedUserControl
{
    sealed record StatusOption(int? Code, string Name);
    readonly AppTextBox _keyword = PageUi.TextBox("Mã phiếu, tên, mã người mượn, điện thoại", 330);
    readonly DateTimePicker _borrowFrom = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Width = 125, Margin = new Padding(4) };
    readonly DateTimePicker _borrowTo = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Width = 125, Margin = new Padding(4) };
    readonly DateTimePicker _dueFrom = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Width = 125, Margin = new Padding(4) };
    readonly DateTimePicker _dueTo = new() { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Width = 125, Margin = new Padding(4) };
    readonly ComboBox _status = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 145, Margin = new Padding(4), DisplayMember = nameof(StatusOption.Name), ValueMember = nameof(StatusOption.Code) };
    readonly AppButton _search = PageUi.Button("Tìm kiếm", ButtonKind.Primary, 105);
    readonly AppButton _detail = PageUi.Button("Xem chi tiết", ButtonKind.Secondary, 115);
    readonly DataGridView _grid = PageUi.Grid();
    readonly Label _count = PageUi.Label("0 phiếu mượn");

    LoanSlipRow? Selected => _grid.CurrentRow?.DataBoundItem as LoanSlipRow;

    public LoanSearchPage()
    {
        var toolbar = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 112, Padding = new Padding(8), WrapContents = true };
        toolbar.Controls.Add(_keyword); toolbar.Controls.Add(PageUi.Label("Ngày mượn từ")); toolbar.Controls.Add(_borrowFrom);
        toolbar.Controls.Add(PageUi.Label("đến")); toolbar.Controls.Add(_borrowTo);
        toolbar.Controls.Add(PageUi.Label("Hạn trả từ")); toolbar.Controls.Add(_dueFrom);
        toolbar.Controls.Add(PageUi.Label("đến")); toolbar.Controls.Add(_dueTo);
        toolbar.Controls.Add(_status); toolbar.Controls.Add(_search); toolbar.Controls.Add(_detail);
        var card = PageUi.Card(); card.Controls.Add(_grid);
        var footer = new ThemedPanel { Dock = DockStyle.Bottom, Height = 34, Fill = PanelFill.Surface }; _count.Dock = DockStyle.Left; footer.Controls.Add(_count); card.Controls.Add(footer);
        Controls.Add(card); Controls.Add(toolbar);
        _status.DataSource = new List<StatusOption>
        {
            new(null, "Tất cả trạng thái"), new((int)LoanDisplayStatus.Borrowing, "Đang mượn"),
            new((int)LoanDisplayStatus.Partial, "Trả một phần"), new((int)LoanDisplayStatus.Returned, "Đã trả"),
            new((int)LoanDisplayStatus.Overdue, "Quá hạn")
        };
        _search.Click += async (_, _) => await ReloadAsync();
        _detail.Click += (_, _) => OpenSelected();
        _grid.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) OpenSelected(); };
        _grid.SelectionChanged += (_, _) => _detail.Enabled = Selected is not null;
        AppGrid.EnableBadges(_grid, nameof(LoanSlipRow.StatusText));
        Load += async (_, _) => { if (!IsDesigning) await ReloadAsync(); };
    }

    async Task ReloadAsync()
    {
        if (_borrowFrom.Checked && _borrowTo.Checked && _borrowFrom.Value.Date > _borrowTo.Value.Date)
        { PageUi.ShowError(FindForm(), "Khoảng ngày mượn không hợp lệ: ngày bắt đầu lớn hơn ngày kết thúc."); return; }
        if (_dueFrom.Checked && _dueTo.Checked && _dueFrom.Value.Date > _dueTo.Value.Date)
        { PageUi.ShowError(FindForm(), "Khoảng hạn trả không hợp lệ: ngày bắt đầu lớn hơn ngày kết thúc."); return; }
        int? status = _status.SelectedItem is StatusOption opt ? opt.Code : null;
        var filter = new LoanFilter
        {
            Keyword = _keyword.Text,
            BorrowFrom = _borrowFrom.Checked ? _borrowFrom.Value.Date : null,
            BorrowTo = _borrowTo.Checked ? _borrowTo.Value.Date : null,
            DueFrom = _dueFrom.Checked ? _dueFrom.Value.Date : null,
            DueTo = _dueTo.Checked ? _dueTo.Value.Date : null,
            Status = status
        };
        var result = await Task.Run(() => LoanService.Search(filter));
        if (IsDisposed) return;
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        _grid.DataSource = result.Value;
        PageUi.Header(_grid, (nameof(LoanSlipRow.SlipCode), "Mã phiếu", 16), (nameof(LoanSlipRow.BorrowerName), "Người mượn", 20),
            (nameof(LoanSlipRow.Phone), "Điện thoại", 12), (nameof(LoanSlipRow.BorrowDate), "Ngày mượn", 12),
            (nameof(LoanSlipRow.DueDate), "Hạn trả", 12), (nameof(LoanSlipRow.TotalQty), "Tổng mượn", 8),
            (nameof(LoanSlipRow.ReturnedQty), "Đã trả", 8), (nameof(LoanSlipRow.RemainingQty), "Còn lại", 8),
            (nameof(LoanSlipRow.StatusText), "Trạng thái", 14));
        PageUi.HideColumns(_grid, nameof(LoanSlipRow.LoanSlipId), nameof(LoanSlipRow.BorrowerId), nameof(LoanSlipRow.BorrowerCode),
            nameof(LoanSlipRow.CreatedBy), nameof(LoanSlipRow.CreatedByName), nameof(LoanSlipRow.Status), nameof(LoanSlipRow.Note),
            nameof(LoanSlipRow.CreatedAt), nameof(LoanSlipRow.DisplayStatus), nameof(LoanSlipRow.DaysOverdue));
        _count.Text = $"Tìm thấy {result.Value!.Count} phiếu mượn"; _detail.Enabled = Selected is not null;
    }

    void OpenSelected()
    {
        if (Selected is not { } row) return;
        var result = LoanService.GetFull(row.LoanSlipId);
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        using var dlg = new LoanSlipDetailDialog(result.Value!); dlg.ShowDialog(FindForm());
    }
}
