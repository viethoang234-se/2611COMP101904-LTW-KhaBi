using System.Drawing;

namespace QuanLyMuonTra.UI;

public enum ThemeMode { Light, Dark }

/// <summary>
/// Bảng màu tập trung cho toàn app. Đổi Theme.SetMode(...) là mọi control tự vẽ lại.
/// Muốn đổi màu chủ đạo: sửa Primary / PrimaryHover bên dưới.
/// </summary>
public static class Theme
{
    public static event EventHandler? Changed;

    public static ThemeMode Mode { get; private set; } = ThemeMode.Light;

    public static void SetMode(ThemeMode mode)
    {
        if (Mode == mode) return;
        Mode = mode;
        Changed?.Invoke(null, EventArgs.Empty);
    }

    public static void Toggle() => SetMode(Mode == ThemeMode.Light ? ThemeMode.Dark : ThemeMode.Light);

    static bool L => Mode == ThemeMode.Light;

    // Nền & bề mặt
    public static Color Background    => L ? Hex("#F5F7FA") : Hex("#0F172A");
    public static Color Surface       => L ? Hex("#FFFFFF") : Hex("#1E293B");
    public static Color InputFill     => L ? Hex("#F8FAFC") : Hex("#0F172A");
    public static Color Hover         => L ? Hex("#F1F5F9") : Hex("#334155");
    public static Color Border        => L ? Hex("#E5E9F0") : Hex("#334155");

    // Chữ
    public static Color TextPrimary   => L ? Hex("#1F2937") : Hex("#F1F5F9");
    public static Color TextSecondary => L ? Hex("#6B7280") : Hex("#94A3B8");

    // Màu thương hiệu & trạng thái
    public static Color Primary       => L ? Hex("#2563EB") : Hex("#3B82F6");
    public static Color PrimaryHover  => L ? Hex("#1D4ED8") : Hex("#2563EB");
    public static Color PrimarySoft   => Blend(Primary, Surface, L ? 0.12f : 0.22f);
    public static Color Success       => Hex("#16A34A");
    public static Color Warning       => Hex("#F59E0B");
    public static Color Danger        => Hex("#DC2626");

    /// <summary>Trộn màu fg lên bg với độ đậm amount (0..1).</summary>
    public static Color Blend(Color fg, Color bg, float amount)
    {
        int r = (int)(fg.R * amount + bg.R * (1 - amount));
        int g = (int)(fg.G * amount + bg.G * (1 - amount));
        int b = (int)(fg.B * amount + bg.B * (1 - amount));
        return Color.FromArgb(r, g, b);
    }

    static Color Hex(string s) => ColorTranslator.FromHtml(s);
}
