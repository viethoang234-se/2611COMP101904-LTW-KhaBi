using QuanLyMuonTra.UI;
using System.Drawing;

namespace QuanLyMuonTra.App.Pages;

internal static class PageUi
{
    public static AppButton Button(string text, ButtonKind kind = ButtonKind.Secondary, int width = 112)
    {
        return new AppButton { Text = text, Kind = kind, Size = new Size(width, 40), Margin = new Padding(4), Font = new Font("Segoe UI", 9.5f) };
    }

    public static AppTextBox TextBox(string placeholder, int width = 240)
    {
        return new AppTextBox { PlaceholderText = placeholder, Size = new Size(width, 42), Margin = new Padding(4) };
    }

    public static Label Label(string text, bool bold = false) => new()
    {
        Text = text, AutoSize = true, ForeColor = Theme.TextSecondary,
        Font = new Font("Segoe UI", 9f, bold ? FontStyle.Bold : FontStyle.Regular),
        Margin = new Padding(4, 12, 4, 4)
    };

    public static DataGridView Grid()
    {
        var grid = new DataGridView { Dock = DockStyle.Fill, AutoGenerateColumns = true, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
        AppGrid.Apply(grid);
        return grid;
    }

    public static void HideColumns(DataGridView grid, params string[] names)
    {
        foreach (var name in names)
            if (grid.Columns.Contains(name)) grid.Columns[name].Visible = false;
    }

    public static void Header(DataGridView grid, params (string Name, string Header, float Weight)[] columns)
    {
        foreach (var c in columns)
            if (grid.Columns.Contains(c.Name))
            {
                grid.Columns[c.Name].HeaderText = c.Header;
                grid.Columns[c.Name].FillWeight = c.Weight;
            }
    }

    sealed class ThemedToolbar : FlowLayoutPanel
    {
        public ThemedToolbar()
        {
            Dock = DockStyle.Top; Height = 58; Padding = new Padding(8); WrapContents = false;
            FlowDirection = FlowDirection.LeftToRight; AutoScroll = true;
            Theme.Changed += OnThemeChanged;
        }
        void OnThemeChanged(object? sender, EventArgs e) => Invalidate();
        protected override void OnPaintBackground(PaintEventArgs e) => e.Graphics.Clear(Theme.Background);
        protected override void Dispose(bool disposing)
        {
            if (disposing) Theme.Changed -= OnThemeChanged;
            base.Dispose(disposing);
        }
    }

    public static FlowLayoutPanel Toolbar() => new ThemedToolbar();

    public static ThemedPanel Card() => new() { Dock = DockStyle.Fill, Padding = new Padding(12), Fill = PanelFill.Surface };

    public static void ShowError(IWin32Window? owner, string message) =>
        MessageBox.Show(owner, message, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
}
