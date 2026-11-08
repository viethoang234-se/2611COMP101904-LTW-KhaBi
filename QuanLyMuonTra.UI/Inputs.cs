using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace QuanLyMuonTra.UI;

public enum ButtonKind { Primary, Secondary, Danger }

/// <summary>Nút bo góc, 3 kiểu (Primary / Secondary / Danger), có thể kèm icon (IconGlyph).</summary>
public class AppButton : Button
{
    bool _hover, _down;
    ButtonKind _kind = ButtonKind.Primary;
    int _radius = 8;
    string _iconGlyph = "";
    readonly EventHandler _onTheme;

    [Category("Appearance"), DefaultValue(ButtonKind.Primary)]
    public ButtonKind Kind { get => _kind; set { _kind = value; Invalidate(); } }

    [Category("Appearance"), DefaultValue(8)]
    public int Radius { get => _radius; set { _radius = value; Invalidate(); } }

    [Category("Appearance"), DefaultValue("")]
    public string IconGlyph { get => _iconGlyph; set { _iconGlyph = value ?? ""; Invalidate(); } }

    protected override Size DefaultSize => new(120, 40);

    public AppButton()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
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
    protected override void OnMouseLeave(EventArgs e) { _hover = _down = false; Invalidate(); base.OnMouseLeave(e); }
    protected override void OnMouseDown(MouseEventArgs e) { _down = true; Invalidate(); base.OnMouseDown(e); }
    protected override void OnMouseUp(MouseEventArgs e) { _down = false; Invalidate(); base.OnMouseUp(e); }
    protected override void OnEnabledChanged(EventArgs e) { Invalidate(); base.OnEnabledChanged(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Gfx.Backdrop(this));

        Color fill, text, border = Color.Empty;
        switch (_kind)
        {
            case ButtonKind.Secondary:
                fill = _hover ? Theme.Hover : Theme.Surface;
                text = Theme.TextPrimary;
                border = Theme.Border;
                break;
            case ButtonKind.Danger:
                fill = _hover ? ControlPaint.Dark(Theme.Danger, 0.1f) : Theme.Danger;
                text = Color.White;
                break;
            default:
                fill = _hover ? Theme.PrimaryHover : Theme.Primary;
                text = Color.White;
                break;
        }
        if (!Enabled) { fill = Theme.Hover; text = Theme.TextSecondary; border = Theme.Border; }

        var rect = new Rectangle(0, 0, Width - 1, Height - 1);
        if (_down && Enabled) rect.Inflate(-1, -1);

        using var path = Gfx.RoundedRect(rect, _radius);
        using var brush = new SolidBrush(fill);
        g.FillPath(brush, path);
        if (!border.IsEmpty)
        {
            using var pen = new Pen(border);
            g.DrawPath(pen, path);
        }

        const TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.SingleLine |
                                      TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis;

        if (string.IsNullOrEmpty(_iconGlyph))
        {
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, text, flags | TextFormatFlags.HorizontalCenter);
            return;
        }

        using var iconFont = Icons.Create(Font.SizeInPoints + 1f);
        var iconSize = TextRenderer.MeasureText(g, _iconGlyph, iconFont, Size.Empty, TextFormatFlags.NoPadding);
        var textSize = string.IsNullOrEmpty(Text)
            ? Size.Empty
            : TextRenderer.MeasureText(g, Text, Font, Size.Empty, TextFormatFlags.NoPadding);
        int gap = textSize.Width > 0 ? 8 : 0;
        int total = iconSize.Width + gap + textSize.Width;
        int x = Math.Max(8, (Width - total) / 2);

        TextRenderer.DrawText(g, _iconGlyph, iconFont, new Rectangle(x, 0, iconSize.Width + 2, Height), text, flags);
        if (textSize.Width > 0)
            TextRenderer.DrawText(g, Text, Font, new Rectangle(x + iconSize.Width + gap, 0, Math.Max(10, Width - x - iconSize.Width - gap - 8), Height), text, flags);
    }
}

/// <summary>Ô nhập liệu bo góc, có icon bên trái, placeholder, viền đổi màu khi focus.</summary>
public class AppTextBox : Control
{
    readonly TextBox _tb = new();
    readonly EventHandler _onTheme;
    string _iconGlyph = "";
    bool _focused;

    [Category("Appearance"), DefaultValue("")]
    public string IconGlyph { get => _iconGlyph; set { _iconGlyph = value ?? ""; LayoutInner(); Invalidate(); } }

    [Category("Appearance"), DefaultValue("")]
    public string PlaceholderText { get => _tb.PlaceholderText; set => _tb.PlaceholderText = value ?? ""; }

    [Category("Behavior"), DefaultValue(false)]
    public bool UseSystemPasswordChar { get => _tb.UseSystemPasswordChar; set => _tb.UseSystemPasswordChar = value; }

    [Category("Behavior"), DefaultValue(false)]
    public bool ReadOnly { get => _tb.ReadOnly; set => _tb.ReadOnly = value; }

    [Category("Behavior"), DefaultValue(0)]
    public int MaxLength { get => _tb.MaxLength; set => _tb.MaxLength = value; }

    [Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
     DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override string Text { get => _tb.Text; set => _tb.Text = value ?? ""; }

    protected override Size DefaultSize => new(280, 44);

    public AppTextBox()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        SetStyle(ControlStyles.Selectable, false);
        TabStop = false;

        _tb.BorderStyle = BorderStyle.None;
        _tb.TabStop = true;
        _tb.TextChanged += (_, _) => OnTextChanged(EventArgs.Empty);
        _tb.Enter += (_, _) => { _focused = true; Invalidate(); };
        _tb.Leave += (_, _) => { _focused = false; Invalidate(); };
        Controls.Add(_tb);

        ApplyColors();
        _onTheme = (_, _) => { ApplyColors(); Invalidate(); };
        Theme.Changed += _onTheme;
        LayoutInner();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) Theme.Changed -= _onTheme;
        base.Dispose(disposing);
    }

    public void FocusInput() => _tb.Focus();
    public void SelectAllText() { _tb.SelectAll(); }

    void ApplyColors()
    {
        _tb.BackColor = Theme.InputFill;
        _tb.ForeColor = Theme.TextPrimary;
    }

    void LayoutInner()
    {
        int left = string.IsNullOrEmpty(_iconGlyph) ? 14 : 44;
        _tb.Left = left;
        _tb.Width = Math.Max(20, Width - left - 14);
        _tb.Top = Math.Max(0, (Height - _tb.Height) / 2);
    }

    protected override void OnResize(EventArgs e) { base.OnResize(e); LayoutInner(); }
    protected override void OnFontChanged(EventArgs e) { base.OnFontChanged(e); LayoutInner(); }
    protected override void OnHandleCreated(EventArgs e) { base.OnHandleCreated(e); LayoutInner(); }
    protected override void OnMouseDown(MouseEventArgs e) { _tb.Focus(); base.OnMouseDown(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Gfx.Backdrop(this));

        using var path = Gfx.RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), 8);
        using var fill = new SolidBrush(Theme.InputFill);
        using var pen = new Pen(_focused ? Theme.Primary : Theme.Border, _focused ? 1.6f : 1f);
        g.FillPath(fill, path);
        g.DrawPath(pen, path);

        if (!string.IsNullOrEmpty(_iconGlyph))
        {
            using var iconFont = Icons.Create(12f);
            TextRenderer.DrawText(g, _iconGlyph, iconFont, new Rectangle(12, 0, 28, Height),
                _focused ? Theme.Primary : Theme.TextSecondary,
                TextFormatFlags.NoPadding | TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }
    }
}
