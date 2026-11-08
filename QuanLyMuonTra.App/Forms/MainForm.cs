using QuanLyMuonTra.App.Security;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

/// <summary>Khung chính: sidebar (menu theo quyền) + header + vùng nội dung.</summary>
public partial class MainForm : ThemedForm
{
    readonly List<NavButton> _navButtons = new();
    NavButton? _activeButton;
    readonly EventHandler _onThemeChanged;

    public bool LoggedOut { get; private set; }

    public MainForm()
    {
        InitializeComponent();

        _onThemeChanged = (_, _) => UpdateThemeButton();

        if (IsDesigning) return;

        var user = Session.CurrentUser;
        lblUserName.Text = user?.FullName ?? "";
        lblUserRole.Text = user?.RoleName ?? "";

        BuildNav();
        UpdateThemeButton();
        Theme.Changed += _onThemeChanged;

        btnTheme.Click += (_, _) => Theme.Toggle();
        btnLogout.Click += (_, _) =>
        {
            LoggedOut = true;
            Close();
        };

        Load += (_, _) => OpenFirstPage();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        Theme.Changed -= _onThemeChanged;
        base.OnFormClosed(e);
    }

    private void BuildNav()
    {
        var allowed = NavCatalog.Items.Where(i => Session.Has(i.Permission)).ToList();

        int y = 8;
        pnlNav.SuspendLayout();

        foreach (var group in allowed.GroupBy(i => i.Group))
        {
            var header = new AppLabel
            {
                Text = group.Key.ToUpperInvariant(),
                Tone = TextTone.Secondary,
                Font = new Font(Font.FontFamily, 8f, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(22, y + 6),
            };
            pnlNav.Controls.Add(header);
            y += 30;

            foreach (var item in group)
            {
                var button = new NavButton
                {
                    Text = item.Title,
                    IconGlyph = item.Icon,
                    Location = new Point(12, y),
                    Size = new Size(pnlNav.ClientSize.Width - 24, 42),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Tag = item,
                };
                button.Click += (_, _) => Navigate(button, item);
                pnlNav.Controls.Add(button);
                _navButtons.Add(button);
                y += 46;
            }

            y += 8;
        }

        pnlNav.ResumeLayout(true);
    }

    private void OpenFirstPage()
    {
        if (_navButtons.Count > 0)
            Navigate(_navButtons[0], (NavItem)_navButtons[0].Tag!);
    }

    private void Navigate(NavButton button, NavItem item)
    {
        if (_activeButton != null) _activeButton.Active = false;
        button.Active = true;
        _activeButton = button;

        lblPageTitle.Text = item.Title;

        pnlContent.SuspendLayout();
        foreach (var old in pnlContent.Controls.Cast<Control>().ToList())
        {
            pnlContent.Controls.Remove(old);
            old.Dispose();
        }

        var page = item.Create();
        page.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(page);
        pnlContent.ResumeLayout(true);
    }

    private void UpdateThemeButton()
    {
        bool light = Theme.Mode == ThemeMode.Light;
        btnTheme.Text = light ? "Giao diện tối" : "Giao diện sáng";
        btnTheme.IconGlyph = light ? Icons.Moon : Icons.Sun;
    }
}
