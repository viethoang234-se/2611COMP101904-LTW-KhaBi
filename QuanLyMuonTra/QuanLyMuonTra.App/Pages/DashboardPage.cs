using QuanLyMuonTra.App.Data;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Pages;

/// <summary>F02 - Dashboard: 4 thẻ thống kê + danh sách phiếu mượn gần đây.</summary>
public partial class DashboardPage : ThemedUserControl
{
    public DashboardPage()
    {
        InitializeComponent();

        pnlStats.Resize += (_, _) => LayoutCards();
        Load += async (_, _) =>
        {
            if (IsDesigning) return;
            AppGrid.Apply(dgvRecent);
            AppGrid.EnableBadges(dgvRecent, colStatus.Name);
            LayoutCards();
            await LoadDataAsync();
        };
    }

    private void LayoutCards()
    {
        const int gap = 16;
        var cards = new[] { cardDevices, cardAvailable, cardActive, cardOverdue };
        int width = (pnlStats.ClientSize.Width - gap * (cards.Length - 1)) / cards.Length;
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i].SetBounds(i * (width + gap), 0, width, cards[i].Height);
        }
    }

    private async Task LoadDataAsync()
    {
        try
        {
            var stats = await Task.Run(DashboardRepository.GetStats);
            var recent = await Task.Run(() => DashboardRepository.GetRecentSlips());
            if (IsDisposed) return;

            cardDevices.Value = stats.TotalDevices.ToString("N0");
            cardAvailable.Value = stats.AvailableUnits.ToString("N0");
            cardActive.Value = stats.ActiveLoans.ToString("N0");
            cardOverdue.Value = stats.OverdueLoans.ToString("N0");
            dgvRecent.DataSource = recent;
        }
        catch (Exception ex)
        {
            if (!IsDisposed)
                MessageBox.Show(FindForm(), "Không tải được dữ liệu Dashboard:\n" + ex.Message,
                    "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
