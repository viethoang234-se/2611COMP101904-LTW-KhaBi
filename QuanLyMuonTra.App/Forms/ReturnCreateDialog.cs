using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

/// <summary>F12 - ghi nhận một lần trả, cho phép chia số lượng theo tình trạng.</summary>
public sealed class ReturnCreateDialog : ThemedForm
{
    sealed class Line
    {
        public int LoanSlipDetailId { get; set; }
        public string DeviceCode { get; set; } = "";
        public string DeviceName { get; set; } = "";
        public int Remaining { get; set; }
        public int ReturnQuantity { get; set; }
        public string ConditionName { get; set; } = "Tốt";
        public string ConditionNote { get; set; } = "";
    }

    readonly LoanSlipFull _slip;
    readonly DataGridView _grid = new() { Dock = DockStyle.Fill, AutoGenerateColumns = true, AllowUserToAddRows = false, RowHeadersVisible = false };
    readonly DateTimePicker _date = new() { Format = DateTimePickerFormat.Short, Width = 130, Margin = new Padding(4) };
    readonly TextBox _giver = new() { Width = 220, Margin = new Padding(4) };
    readonly TextBox _note = new() { Width = 260, Margin = new Padding(4) };
    readonly AppButton _save = new() { Text = "Lưu biên bản trả", Kind = ButtonKind.Primary, Width = 150, Height = 40, Margin = new Padding(4) };
    readonly List<Line> _lines;

    static readonly string[] Conditions = { "Tốt", "Hư hỏng", "Mất", "Thiếu phụ kiện" };
    static ReturnCondition ToCondition(string value) => value switch
    {
        "Hư hỏng" => ReturnCondition.Damaged,
        "Mất" => ReturnCondition.Lost,
        "Thiếu phụ kiện" => ReturnCondition.MissingAccessory,
        _ => ReturnCondition.Good
    };

    public ReturnCreateDialog(LoanSlipFull slip)
    {
        _slip = slip;
        _lines = slip.Details.Where(x => x.RemainingQuantity > 0).Select(x => new Line
        {
            LoanSlipDetailId = x.DetailId, DeviceCode = x.DeviceCode, DeviceName = x.DeviceName,
            Remaining = x.RemainingQuantity, ReturnQuantity = 0
        }).ToList();
        Text = $"Ghi nhận trả - {slip.Header.SlipCode}";
        StartPosition = FormStartPosition.CenterParent; Size = new Size(1050, 650); MinimumSize = new Size(850, 520);

        var info = new Label { Dock = DockStyle.Top, Height = 34, Padding = new Padding(12, 8, 0, 0), Text = $"Phiếu: {slip.Header.SlipCode}    Người mượn: {slip.Header.BorrowerName}    Hạn trả: {slip.Header.DueDate:dd/MM/yyyy}" };
        var tools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 52, Padding = new Padding(8), WrapContents = true };
        tools.Controls.Add(new Label { Text = "Ngày trả", AutoSize = true, Margin = new Padding(4, 12, 2, 4) }); tools.Controls.Add(_date);
        tools.Controls.Add(new Label { Text = "Người giao", AutoSize = true, Margin = new Padding(4, 12, 2, 4) }); tools.Controls.Add(_giver);
        tools.Controls.Add(new Label { Text = "Ghi chú", AutoSize = true, Margin = new Padding(4, 12, 2, 4) }); tools.Controls.Add(_note);
        var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 56, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) }; bottom.Controls.Add(_save);
        AppGrid.Apply(_grid); _grid.ReadOnly = false; _grid.DataSource = _lines;
        ConfigureGrid();
        Controls.Add(_grid); Controls.Add(bottom); Controls.Add(tools); Controls.Add(info);
        _date.Value = DateTime.Today; _date.MaxDate = DateTime.Today;
        _save.Click += (_, _) => Save();
    }

    void ConfigureGrid()
    {
        Set(nameof(Line.DeviceCode), "Mã thiết bị", false, 90);
        Set(nameof(Line.DeviceName), "Tên thiết bị", false, 220);
        Set(nameof(Line.Remaining), "Còn phải trả", false, 90);
        Set(nameof(Line.ReturnQuantity), "Số lượng trả (0 = bỏ qua)", true, 130);
        Set(nameof(Line.ConditionNote), "Ghi chú tình trạng", true, 200);
        if (_grid.Columns[nameof(Line.ConditionName)] is DataGridViewTextBoxColumn old)
        {
            int index = old.Index; _grid.Columns.Remove(old);
            var combo = new DataGridViewComboBoxColumn { Name = nameof(Line.ConditionName), DataPropertyName = nameof(Line.ConditionName), HeaderText = "Tình trạng", DataSource = Conditions, FlatStyle = FlatStyle.Flat, Width = 130 };
            _grid.Columns.Insert(index, combo);
        }
        _grid.Columns[nameof(Line.LoanSlipDetailId)]!.Visible = false;
        _grid.Columns[nameof(Line.ConditionName)]!.DisplayIndex = 4;
    }

    void Set(string name, string header, bool editable, int width)
    {
        if (!_grid.Columns.Contains(name)) return;
        _grid.Columns[name]!.HeaderText = header; _grid.Columns[name]!.ReadOnly = !editable; _grid.Columns[name]!.Width = width;
    }

    void Save()
    {
        if (_grid.IsCurrentCellInEditMode) _grid.EndEdit();
        var items = _lines.Where(x => x.ReturnQuantity > 0).Select(x => new NewReturnItem
        {
            LoanSlipDetailId = x.LoanSlipDetailId, Quantity = x.ReturnQuantity,
            Condition = ToCondition(x.ConditionName), ConditionNote = x.ConditionNote
        }).ToList();
        var record = new NewReturnRecord { LoanSlipId = _slip.Header.LoanSlipId, ReturnDate = _date.Value.Date, GiverName = _giver.Text, Note = _note.Text, Items = items };
        var result = ReturnService.Create(record);
        if (!result.Success) { MessageBox.Show(this, result.Error, "Không thể lưu", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
        MessageBox.Show(this, $"Đã tạo biên bản {result.Value!.RecordCode}.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        DialogResult = DialogResult.OK; Close();
    }
}
