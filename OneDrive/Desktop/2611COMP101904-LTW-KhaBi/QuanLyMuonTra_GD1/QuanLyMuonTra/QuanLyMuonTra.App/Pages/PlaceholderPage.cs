using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>Trang tạm cho các form chưa làm. Thay bằng trang thật trong NavCatalog.cs khi làm xong.</summary>
public partial class PlaceholderPage : ThemedUserControl
{
    public PlaceholderPage()
    {
        InitializeComponent();
    }

    public PlaceholderPage(string code, string title, string note) : this()
    {
        lblTitle.Text = $"{code} · {title}";
        lblNote.Text = note;
    }
}
