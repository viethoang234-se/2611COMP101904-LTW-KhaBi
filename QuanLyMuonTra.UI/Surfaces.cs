using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace QuanLyMuonTra.UI;

/// <summary>Khung trắng bo góc (card). Đặt các control con vào bên trong như Panel thường.</summary>
public class CardPanel : Panel, IBackdropProvider
{
    int _radius = 12;
    readonly EventHandler _onTheme;

    [Category("Appearance"), DefaultValue(12)]
    public int Radius { get => _radius; set { _radius = value; Invalidate(); } }

    public Color BackdropColor => Theme.Surface;

    public CardPanel()
    {
        SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.ResizeRedraw | ControlStyles.UserPaint, true);
        _onTheme = (_, _) => Invalidate(true);
        Theme.Changed += _onTheme;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) Theme.Changed -= _onTheme;
        base.Dispose(disposing);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(Gfx.Backdrop(this));

        using var path = Gfx.RoundedRect(new Rectangle(0, 0, Width - 1, Height - 1), Radius);
        using var fill = new SolidBrush(Theme.Surface);
        using var pen = new Pen(Theme.Border);
        g.FillPath(fill, path);
        g.DrawPath(pen, path);
    }
}

/// <summary>Panel nền gradient theo màu chủ đạo (dùng cho cột thương hiệu ở màn đăng nhập).</summary>
public class GradientPanel : Panel
{
    readonly EventHandler _onTheme;

    public GradientPanel()
    {
        DoubleBuffered = true;
        SetStyle(ControlStyles.ResizeRedraw, true);
        _onTheme = (_, _) => Invalidate(true);
        Theme.Changed += _onTheme;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) Theme.Changed -= _onTheme;
        base.Dispose(disposing);
    }

    protected override void OnPaintBackground(PaintEventArgs e)
    {
        if (Width <= 0 || Height <= 0) return;
        var end = Theme.Blend(Color.FromArgb(15, 23, 42), Theme.Primary, 0.45f);
        using var brush = new LinearGradientBrush(ClientRectangle, Theme.Primary, end, 60f);
        e.Graphics.FillRectangle(brush, ClientRectangle);
    }
}

public enum TextTone { Primary, Secondary, Accent, Success, Warning, Danger }

/// <summary>Label tự đổi màu chữ theo Theme. Chọn Tone thay vì chọn ForeColor.</summary>
public class AppLabel : Label
{
    TextTone _tone = TextTone.Primary;
    readonly EventHandler _onTheme;

    [Category("Appearance"), DefaultValue(TextTone.Primary)]
    public TextTone Tone { get => _tone; set { _tone = value; Invalidate(); } }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
     DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override Color ForeColor
    {
        get => _tone switch
        {
            TextTone.Secondary => Theme.TextSecondary,
            TextTone.Accent => Theme.Primary,
            TextTone.Success => Theme.Success,
            TextTone.Warning => Theme.Warning,
            TextTone.Danger => Theme.Danger,
            _ => Theme.TextPrimary,
        };
        set { }
    }

    public AppLabel()
    {
        BackColor = Color.Transparent;
        _onTheme = (_, _) => Invalidate();
        Theme.Changed += _onTheme;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) Theme.Changed -= _onTheme;
        base.Dispose(disposing);
    }
}
