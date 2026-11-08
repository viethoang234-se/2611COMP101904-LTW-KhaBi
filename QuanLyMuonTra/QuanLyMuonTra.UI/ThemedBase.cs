using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace QuanLyMuonTra.UI;

/// <summary>Form nền: tự tô màu nền theo Theme (không đụng BackColor nên Designer không bị ghi đè màu).</summary>
public class ThemedForm : Form
{
    readonly EventHandler _onTheme;

    [DllImport("user32.dll")] static extern bool ReleaseCapture();
    [DllImport("user32.dll")] static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);

    public ThemedForm()
    {
        DoubleBuffered = true;
        _onTheme = (_, _) => { ThemeApplier.Apply(this); Invalidate(true); };
    }

    protected bool IsDesigning => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    protected override void OnPaintBackground(PaintEventArgs e) => e.Graphics.Clear(Theme.Background);

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (IsDesigning) return;
        ThemeApplier.Apply(this);
        Theme.Changed += _onTheme;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        Theme.Changed -= _onTheme;
        base.OnFormClosed(e);
    }

    /// <summary>Cho phép kéo cửa sổ (form không viền) bằng cách nắm vào control này.</summary>
    protected void EnableDrag(Control c)
    {
        c.MouseDown += (_, e) =>
        {
            if (e.Button != MouseButtons.Left) return;
            ReleaseCapture();
            SendMessage(Handle, 0xA1, 2, 0); // WM_NCLBUTTONDOWN, HTCAPTION
        };
    }
}

/// <summary>UserControl nền cho các trang nội dung.</summary>
public class ThemedUserControl : UserControl
{
    readonly EventHandler _onTheme;

    public ThemedUserControl()
    {
        DoubleBuffered = true;
        _onTheme = (_, _) => { ThemeApplier.Apply(this); Invalidate(true); };
    }

    protected bool IsDesigning => DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

    protected override void OnPaintBackground(PaintEventArgs e) => e.Graphics.Clear(Theme.Background);

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (IsDesigning) return;
        ThemeApplier.Apply(this);
        Theme.Changed += _onTheme;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) Theme.Changed -= _onTheme;
        base.Dispose(disposing);
    }
}

public enum PanelFill { Background, Surface }
public enum PanelEdge { None, Right, Bottom }

/// <summary>Panel tô nền theo Theme. Dùng cho sidebar, header, vùng nội dung.</summary>
public class ThemedPanel : Panel, IBackdropProvider
{
    PanelFill _fill = PanelFill.Background;
    PanelEdge _edge = PanelEdge.None;
    readonly EventHandler _onTheme;

    [Category("Appearance"), DefaultValue(PanelFill.Background)]
    public PanelFill Fill { get => _fill; set { _fill = value; Invalidate(); } }

    [Category("Appearance"), DefaultValue(PanelEdge.None)]
    public PanelEdge Edge { get => _edge; set { _edge = value; Invalidate(); } }

    public Color BackdropColor => _fill == PanelFill.Surface ? Theme.Surface : Theme.Background;

    public ThemedPanel()
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

    protected override void OnPaintBackground(PaintEventArgs e) => e.Graphics.Clear(BackdropColor);

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (_edge == PanelEdge.None) return;
        using var pen = new Pen(Theme.Border);
        if (_edge == PanelEdge.Right)
            e.Graphics.DrawLine(pen, Width - 1, 0, Width - 1, Height);
        else
            e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
    }
}

/// <summary>Tô màu các control "nhập liệu" chuẩn của Windows (ComboBox, NumericUpDown...) theo Theme khi chạy app.</summary>
public static class ThemeApplier
{
    public static void Apply(Control root)
    {
        foreach (Control c in root.Controls)
        {
            if (c is AppTextBox) continue; // tự xử lý

            switch (c)
            {
                case ComboBox cb:
                    cb.FlatStyle = FlatStyle.Flat;
                    cb.BackColor = Theme.InputFill;
                    cb.ForeColor = Theme.TextPrimary;
                    break;
                case NumericUpDown nud:
                    nud.BorderStyle = BorderStyle.FixedSingle;
                    nud.BackColor = Theme.InputFill;
                    nud.ForeColor = Theme.TextPrimary;
                    break;
                case TextBox tb:
                    tb.BackColor = Theme.InputFill;
                    tb.ForeColor = Theme.TextPrimary;
                    break;
                case CheckBox or RadioButton:
                    c.ForeColor = Theme.TextPrimary;
                    break;
            }

            if (c.HasChildren && c is not DataGridView) Apply(c);
        }
    }
}
