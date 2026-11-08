using QuanLyMuonTra.App.Forms;
using QuanLyMuonTra.App.Security;

namespace QuanLyMuonTra.App;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        // Font mặc định (Segoe UI 10pt) khai báo trong file .csproj: <ApplicationDefaultFont>
        ApplicationConfiguration.Initialize();

        while (true)
        {
            using (var login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK) return;
            }

            using var main = new MainForm();
            Application.Run(main);

            if (!main.LoggedOut) return; // đóng cửa sổ = thoát; bấm Đăng xuất = quay lại màn đăng nhập
            Session.End();
        }
    }
}
