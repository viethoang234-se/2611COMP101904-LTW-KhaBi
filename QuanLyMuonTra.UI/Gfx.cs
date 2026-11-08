using System.Drawing;
using System.Drawing.Drawing2D;

namespace QuanLyMuonTra.UI;

/// <summary>Control nào "tô nền" cho con của nó thì implement interface này để các control bo góc biết tô 4 góc màu gì.</summary>
public interface IBackdropProvider
{
    Color BackdropColor { get; }
}

internal static class Gfx
{
    public static GraphicsPath RoundedRect(Rectangle r, int radius)
    {
        int d = Math.Max(1, radius * 2);
        var p = new GraphicsPath();
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    /// <summary>Màu nền nằm phía sau control (để tô sạch 4 góc bo).</summary>
    public static Color Backdrop(Control c)
    {
        for (var p = c.Parent; p != null; p = p.Parent)
        {
            if (p is IBackdropProvider provider) return provider.BackdropColor;
            if (p is ThemedForm || p is ThemedUserControl) return Theme.Background;
        }
        return Theme.Background;
    }
}

/// <summary>Glyph của font "Segoe MDL2 Assets" (có sẵn trên Windows 10/11). Muốn đổi icon: mở Character Map, chọn font này, copy mã.</summary>
public static class Icons
{
    public const string Home = "\uE80F";
    public const string Users = "\uE716";
    public const string Lock = "\uE72E";
    public const string Tag = "\uE8EC";
    public const string Device = "\uE7F4";
    public const string Contact = "\uE77B";
    public const string Add = "\uE710";
    public const string Search = "\uE721";
    public const string Return = "\uE7A7";
    public const string Document = "\uE8A5";
    public const string Warning = "\uE7BA";
    public const string Print = "\uE749";
    public const string Settings = "\uE713";
    public const string Chart = "\uE9D2";
    public const string Edit = "\uE70F";
    public const string Delete = "\uE74D";
    public const string Close = "\uE711";
    public const string Refresh = "\uE72C";
    public const string Save = "\uE74E";
    public const string Sun = "\uE706";
    public const string Moon = "\uE708";
    public const string SignOut = "\uF3B1";
    public const string Calendar = "\uE787";
    public const string Accept = "\uE8FB";
    public const string Package = "\uE7B8";

    public static Font Create(float size) => new("Segoe MDL2 Assets", size, FontStyle.Regular, GraphicsUnit.Point);
}
