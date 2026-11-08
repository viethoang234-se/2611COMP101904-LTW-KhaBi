using QuanLyMuonTra.App.Models;
using QuanLyMuonTra.App.Services;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F09 - lập phiếu mượn nhiều thiết bị trong một giao dịch.</summary>
public sealed class LoanCreatePage : ThemedUserControl
{
    sealed class CartRow
    {
        public int DeviceId { get; set; }
        public string DeviceCode { get; set; } = "";
        public string DeviceName { get; set; } = "";
        public int AvailableQuantity { get; set; }
        public int Quantity { get; set; }
        public string Unit { get; set; } = "";
    }

    readonly ComboBox _borrower = new() { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250, Height = 36, DisplayMember = nameof(BorrowerRow.FullName), ValueMember = nameof(BorrowerRow.BorrowerId), Margin = new Padding(4) };
    readonly DateTimePicker _borrowDate = new() { Format = DateTimePickerFormat.Short, Width = 130, Margin = new Padding(4) };
    readonly DateTimePicker _dueDate = new() { Format = DateTimePickerFormat.Short, Width = 130, Margin = new Padding(4) };
    readonly AppTextBox _note = PageUi.TextBox("Ghi chú phiếu mượn", 240);
    readonly AppTextBox _deviceSearch = PageUi.TextBox("Tìm thiết bị", 230);
    readonly DataGridView _devices = PageUi.Grid();
    readonly DataGridView _cartGrid = PageUi.Grid();
    readonly NumericUpDown _quantity = new() { Minimum = 1, Maximum = 100000, Value = 1, Width = 90, Height = 36, Margin = new Padding(4) };
    readonly AppButton _add = PageUi.Button("Thêm vào phiếu", ButtonKind.Primary, 135);
    readonly AppButton _remove = PageUi.Button("Bỏ khỏi phiếu", ButtonKind.Danger, 125);
    readonly AppButton _save = PageUi.Button("Lưu phiếu mượn", ButtonKind.Primary, 145);
    readonly Label _summary = PageUi.Label("Chưa có thiết bị được chọn", true);
    readonly System.Windows.Forms.Timer _timer = new() { Interval = 350 };
    readonly BindingSource _cartSource = new();
    List<CartRow> _cart = new();
    List<DeviceRow> _deviceRows = new();

    public LoanCreatePage()
    {
        var header = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 108, Padding = new Padding(8), WrapContents = true };
        header.Controls.Add(PageUi.Label("Người mượn")); header.Controls.Add(_borrower);
        header.Controls.Add(PageUi.Label("Ngày mượn")); header.Controls.Add(_borrowDate);
        header.Controls.Add(PageUi.Label("Hạn trả")); header.Controls.Add(_dueDate);
        header.Controls.Add(_note);

        var left = new ThemedPanel { Dock = DockStyle.Fill, Padding = new Padding(8), Fill = PanelFill.Background };
        var leftTools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 54, WrapContents = false };
        leftTools.Controls.Add(_deviceSearch); leftTools.Controls.Add(_quantity); leftTools.Controls.Add(_add);
        left.Controls.Add(_devices); left.Controls.Add(leftTools);

        var right = new ThemedPanel { Dock = DockStyle.Fill, Padding = new Padding(8), Fill = PanelFill.Background };
        var rightTools = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 54, WrapContents = false };
        rightTools.Controls.Add(_remove); rightTools.Controls.Add(_summary);
        right.Controls.Add(_cartGrid); right.Controls.Add(rightTools);

        var split = new SplitContainer { Dock = DockStyle.Fill, Size = new Size(1000, 600), Orientation = Orientation.Vertical, SplitterDistance = 500, Panel1MinSize = 320, Panel2MinSize = 320 };
        split.Panel1.Controls.Add(left); split.Panel2.Controls.Add(right);
        var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 58, FlowDirection = FlowDirection.RightToLeft, Padding = new Padding(8) };
        bottom.Controls.Add(_save);
        Controls.Add(split); Controls.Add(bottom); Controls.Add(header);

        _borrowDate.Value = DateTime.Today; _dueDate.Value = DateTime.Today.AddDays(7);
        _deviceSearch.TextChanged += (_, _) => { _timer.Stop(); _timer.Start(); };
        _timer.Tick += async (_, _) => { _timer.Stop(); await LoadDevicesAsync(); };
        _add.Click += (_, _) => AddSelectedDevice(); _remove.Click += (_, _) => RemoveSelectedDevice();
        _save.Click += async (_, _) => await SaveLoanAsync();
        _devices.SelectionChanged += (_, _) => UpdateAddButton();
        _quantity.ValueChanged += (_, _) => UpdateAddButton();
        _cartGrid.SelectionChanged += (_, _) => _remove.Enabled = _cartGrid.CurrentRow is not null;
        Load += async (_, _) => { if (!IsDesigning) await InitializeAsync(); };
    }

    async Task InitializeAsync()
    {
        var borrowers = await Task.Run(() => BorrowerService.Search(null));
        if (!borrowers.Success) { PageUi.ShowError(FindForm(), borrowers.Error); return; }
        _borrower.DataSource = borrowers.Value;
        _borrowDate.MaxDate = DateTime.Today;
        _devices.AutoGenerateColumns = true; _cartGrid.AutoGenerateColumns = true;
        _cartSource.DataSource = _cart; _cartGrid.DataSource = _cartSource;
        PageUi.Header(_cartGrid, (nameof(CartRow.DeviceCode), "Mã", 18), (nameof(CartRow.DeviceName), "Thiết bị", 34),
            (nameof(CartRow.Quantity), "Số lượng mượn", 16), (nameof(CartRow.Unit), "Đơn vị", 12));
        PageUi.HideColumns(_cartGrid, nameof(CartRow.DeviceId), nameof(CartRow.AvailableQuantity));
        await LoadDevicesAsync(); UpdateAddButton();
    }

    async Task LoadDevicesAsync()
    {
        var result = await Task.Run(() => DeviceService.SearchAvailable(_deviceSearch.Text));
        if (IsDisposed) return;
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        _deviceRows = result.Value ?? new(); _devices.DataSource = _deviceRows;
        PageUi.Header(_devices, (nameof(DeviceRow.DeviceCode), "Mã", 16), (nameof(DeviceRow.DeviceName), "Tên thiết bị", 32),
            (nameof(DeviceRow.CategoryName), "Loại", 20), (nameof(DeviceRow.AvailableQuantity), "Khả dụng", 12), (nameof(DeviceRow.Unit), "Đơn vị", 10));
        PageUi.HideColumns(_devices, nameof(DeviceRow.DeviceId), nameof(DeviceRow.CategoryId), nameof(DeviceRow.TotalQuantity),
            nameof(DeviceRow.Note), nameof(DeviceRow.DamagedQuantity), nameof(DeviceRow.LostQuantity), nameof(DeviceRow.OnLoanQuantity));
        UpdateAddButton();
    }

    DeviceRow? SelectedDevice => _devices.CurrentRow?.DataBoundItem as DeviceRow;

    void UpdateAddButton()
    {
        var d = SelectedDevice;
        _add.Enabled = d is not null && _quantity.Value > 0 && _quantity.Value <= (d.AvailableQuantity - (_cart.FirstOrDefault(x => x.DeviceId == d.DeviceId)?.Quantity ?? 0));
        _remove.Enabled = _cartGrid.CurrentRow is not null;
    }

    void AddSelectedDevice()
    {
        if (SelectedDevice is not { } d) return;
        int qty = (int)_quantity.Value;
        var current = _cart.FirstOrDefault(x => x.DeviceId == d.DeviceId);
        if (current is null)
            _cart.Add(new CartRow { DeviceId = d.DeviceId, DeviceCode = d.DeviceCode, DeviceName = d.DeviceName, AvailableQuantity = d.AvailableQuantity, Quantity = qty, Unit = d.Unit });
        else current.Quantity += qty;
        RefreshCart();
    }

    void RemoveSelectedDevice()
    {
        if (_cartGrid.CurrentRow?.DataBoundItem is not CartRow row) return;
        _cart.RemoveAll(x => x.DeviceId == row.DeviceId); RefreshCart();
    }

    void RefreshCart()
    {
        _cartSource.DataSource = null; _cartSource.DataSource = _cart.ToList();
        PageUi.Header(_cartGrid, (nameof(CartRow.DeviceCode), "Mã", 18), (nameof(CartRow.DeviceName), "Thiết bị", 34),
            (nameof(CartRow.Quantity), "Số lượng mượn", 16), (nameof(CartRow.Unit), "Đơn vị", 12));
        PageUi.HideColumns(_cartGrid, nameof(CartRow.DeviceId), nameof(CartRow.AvailableQuantity));
        _summary.Text = $"{_cart.Count} loại thiết bị | Tổng {_cart.Sum(x => x.Quantity)} đơn vị";
        UpdateAddButton();
    }

    async Task SaveLoanAsync()
    {
        if (_borrower.SelectedValue is not int borrowerId) { PageUi.ShowError(FindForm(), "Vui lòng chọn người mượn."); return; }
        var slip = new NewLoanSlip
        {
            BorrowerId = borrowerId, BorrowDate = _borrowDate.Value.Date, DueDate = _dueDate.Value.Date,
            Note = _note.Text, Items = _cart.Select(x => new NewLoanItem { DeviceId = x.DeviceId, Quantity = x.Quantity }).ToList()
        };
        _save.Enabled = false;
        var result = await Task.Run(() => LoanService.Create(slip));
        _save.Enabled = true;
        if (!result.Success) { PageUi.ShowError(FindForm(), result.Error); return; }
        MessageBox.Show(FindForm(), $"Đã tạo phiếu mượn {result.Value!.SlipCode}.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        _cart.Clear(); RefreshCart(); _note.Text = ""; await LoadDevicesAsync();
    }
}
