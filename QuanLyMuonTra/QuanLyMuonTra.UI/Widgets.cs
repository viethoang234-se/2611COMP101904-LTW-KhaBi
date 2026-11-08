using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace QuanLyMuonTra.UI;

/// <summary>Nút menu ở sidebar: icon + chữ, có trạng thái Active và hover.</summary>
public class NavButton : Control
{
    bool _hover, _active;
    string _iconGlyph = "";
    readonly EventHandler _onTheme;

    [Category("Appearance"), DefaultValue("")]
    public string IconGlyph { get => _iconGlyph; set { _iconGlyph = value ?? ""; Invalidate(); } }

    [Category("Appearance"), DefaultValue(false)]
    public bool Active { get => _active; set { _active = value; Invalidate(); } }

    protected override Size DefaultSize => new(226, 42);

    public NavButton()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint | ControlStyles.StandardClick, true);
        SetStyle(ControlStyles.Selectable, false);
        Cursor = Cursors.Hand;
        _onTheme = (_, _) => Invalidate();
        Theme.Changed += _onTheme;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) Theme.Changed -= _onTheme;
        base.Dispose(disposing);
    }

    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Gfx.Backdrop(this));

        if (_active || _hover)
        {
            using var path = Gfx.RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), 8);
            using var brush = new SolidBrush(_active ? Theme.PrimarySoft : Theme.Hover);
            g.FillPath(brush, path);
        }

        var color = _active ? Theme.Primary : Theme.TextPrimary;
        const TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine |
                                      TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

        if (!string.IsNullOrEmpty(_iconGlyph))
        {
            using var iconFont = Icons.Create(13f);
            TextRenderer.DrawText(g, _iconGlyph, iconFont, new Rectangle(12, 0, 28, Height),
                _active ? Theme.Primary : Theme.TextSecondary,
                flags | TextFormatFlags.HorizontalCenter);
        }

        using var font = new Font(Font, _active ? FontStyle.Bold : FontStyle.Regular);
        TextRenderer.DrawText(g, Text, font, new Rectangle(48, 0, Math.Max(10, Width - 56), Height), color, flags);
    }
}

/// <summary>Thẻ thống kê (icon + tiêu đề + con số lớn) dùng ở Dashboard.</summary>
public class StatCard : Control
{
    string _title = "Tiêu đề";
    string _value = "0";
    string _iconGlyph = "";
    TextTone _accent = TextTone.Accent;
    readonly EventHandler _onTheme;

    [Category("Appearance"), DefaultValue("Tiêu đề")]
    public string Title { get => _title; set { _title = value ?? ""; Invalidate(); } }

    [Category("Appearance"), DefaultValue("0")]
    public string Value { get => _value; set { _value = value ?? ""; Invalidate(); } }

    [Category("Appearance"), DefaultValue("")]
    public string IconGlyph { get => _iconGlyph; set { _iconGlyph = value ?? ""; Invalidate(); } }

    [Category("Appearance"), DefaultValue(TextTone.Accent)]
    public TextTone Accent { get => _accent; set { _accent = value; Invalidate(); } }

    protected override Size DefaultSize => new(240, 104);

    public StatCard()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        _onTheme = (_, _) => Invalidate();
        Theme.Changed += _onTheme;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) Theme.Changed -= _onTheme;
        base.Dispose(disposing);
    }

    Color AccentColor => _accent switch
    {
        TextTone.Success => Theme.Success,
        TextTone.Warning => Theme.Warning,
        TextTone.Danger => Theme.Danger,
        TextTone.Secondary => Theme.TextSecondary,
        _ => Theme.Primary,
    };

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Gfx.Backdrop(this));

        using (var path = Gfx.RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), 12))
        using (var fill = new SolidBrush(Theme.Surface))
        using (var pen = new Pen(Theme.Border))
        {
            g.FillPath(fill, path);
            g.DrawPath(pen, path);
        }

        // Vòng tròn icon
        var circle = new Rectangle(18, (Height - 52) / 2, 52, 52);
        using (var soft = new SolidBrush(Theme.Blend(AccentColor, Theme.Surface, 0.14f)))
            g.FillEllipse(soft, circle);
        if (!string.IsNullOrEmpty(_iconGlyph))
        {
            using var iconFont = Icons.Create(18f);
            TextRenderer.DrawText(g, _iconGlyph, iconFont, circle, AccentColor,
                TextFormatFlags.NoPadding | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        int textX = circle.Right + 16;
        int textW = Math.Max(20, Width - textX - 12);

        using var titleFont = new Font(Font.FontFamily, 9.5f, FontStyle.Regular);
        using var valueFont = new Font(Font.FontFamily, 24f, FontStyle.Bold);
        TextRenderer.DrawText(g, _title, titleFont, new Rectangle(textX, Height / 2 - 34, textW, 24),
            Theme.TextSecondary, TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.Bottom);
        TextRenderer.DrawText(g, _value, valueFont, new Rectangle(textX, Height / 2 - 6, textW, 44),
            Theme.TextPrimary, TextFormatFlags.NoPadding | TextFormatFlags.EndEllipsis | TextFormatFlags.Top);
    }
}

/// <summary>Áp phong cách hiện đại cho DataGridView (header phẳng, hàng cao, không lưới dọc, badge trạng thái).</summary>
public static class AppGrid
{
    public static void Apply(DataGridView g)
    {
        g.BorderStyle = BorderStyle.None;
        g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
        g.EnableHeadersVisualStyles = false;
        g.RowHeadersVisible = false;
        g.AllowUserToAddRows = false;
        g.AllowUserToDeleteRows = false;
        g.AllowUserToResizeRows = false;
        g.ReadOnly = true;
        g.MultiSelect = false;
        g.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        g.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        g.ColumnHeadersHeight = 44;
        g.RowTemplate.Height = 46;

        Recolor(g);

        EventHandler onTheme = (_, _) => { Recolor(g); g.Invalidate(); };
        Theme.Changed += onTheme;
        g.Disposed += (_, _) => Theme.Changed -= onTheme;
    }

    static void Recolor(DataGridView g)
    {
        g.BackgroundColor = Theme.Surface;
        g.GridColor = Theme.Border;

        var header = g.ColumnHeadersDefaultCellStyle;
        header.BackColor = Theme.Surface;
        header.ForeColor = Theme.TextSecondary;
        header.SelectionBackColor = Theme.Surface;
        header.SelectionForeColor = Theme.TextSecondary;
        header.Font = new Font(g.Font.FontFamily, 9.5f, FontStyle.Bold);
        header.Alignment = DataGridViewContentAlignment.MiddleLeft;
        header.Padding = new Padding(10, 0, 0, 0);

        var cell = g.DefaultCellStyle;
        cell.BackColor = Theme.Surface;
        cell.ForeColor = Theme.TextPrimary;
        cell.SelectionBackColor = Theme.PrimarySoft;
        cell.SelectionForeColor = Theme.TextPrimary;
        cell.Padding = new Padding(10, 0, 0, 0);
        cell.Alignment = DataGridViewContentAlignment.MiddleLeft;
    }

    /// <summary>Vẽ cột có tên columnName thành "badge" màu: Đang mượn (xanh dương), Đã trả (xanh lá), Quá hạn (đỏ)...</summary>
    public static void EnableBadges(DataGridView g, string columnName)
    {
        g.CellPainting += (s, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.Graphics is null) return;
            if (g.Columns[e.ColumnIndex].Name != columnName) return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border);

            string text = Convert.ToString(e.FormattedValue) ?? "";
            if (text.Length > 0)
            {
                var color = BadgeColor(text);
                var gr = e.Graphics;
                gr.SmoothingMode = SmoothingMode.AntiAlias;

                using var font = new Font(g.Font.FontFamily, 9f, FontStyle.Bold);
                var size = TextRenderer.MeasureText(gr, text, font, Size.Empty, TextFormatFlags.NoPadding);
                var pill = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + (e.CellBounds.Height - 26) / 2, size.Width + 24, 26);

                using var path = Gfx.RoundedRect(pill, 13);
                using var fill = new SolidBrush(Theme.Blend(color, Theme.Surface, 0.14f));
                gr.FillPath(fill, path);
                TextRenderer.DrawText(gr, text, font, pill, color,
                    TextFormatFlags.NoPadding | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            e.Handled = true;
        };
    }

    static Color BadgeColor(string status) => status switch
    {
        "Quá hạn" => Theme.Danger,
        "Đang mượn" => Theme.Primary,
        "Đã trả" => Theme.Success,
        "Trả một phần" => Theme.Warning,
        _ => Theme.TextSecondary,
    };
}
