using QuanLyMuonTra.App.Data;
using QuanLyMuonTra.App.Security;
using QuanLyMuonTra.UI;

namespace QuanLyMuonTra.App.Forms;

/// <summary>F01 - Đăng nhập hệ thống.</summary>
public partial class LoginForm : ThemedForm
{
    public LoginForm()
    {
        InitializeComponent();

        AcceptButton = btnLogin;
        EnableDrag(pnlBrand);
        EnableDrag(pnlForm);

        lblClose.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        btnLogin.Click += BtnLogin_Click;
        Shown += (_, _) => txtUser.FocusInput();
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        string username = txtUser.Text.Trim();
        string password = txtPass.Text;

        if (username.Length == 0 || password.Length == 0)
        {
            ShowError("Vui lòng nhập tên đăng nhập và mật khẩu.");
            return;
        }

        SetBusy(true);
        try
        {
            var result = await Task.Run(() => UserRepository.Authenticate(username, password));

            if (result.User is null)
            {
                ShowError(result.Error ?? "Đăng nhập thất bại.");
                txtPass.Text = "";
                txtPass.FocusInput();
                return;
            }

            Session.Start(result.User, result.Permissions);
            DialogResult = DialogResult.OK;
        }
        catch (Exception ex)
        {
            ShowError("Không kết nối được SQL Server (kiểm tra connection.txt và đã chạy script SQL chưa): " + ex.Message);
        }
        finally
        {
            if (!IsDisposed) SetBusy(false);
        }
    }

    private void ShowError(string message) => lblError.Text = message;

    private void SetBusy(bool busy)
    {
        btnLogin.Enabled = !busy;
        btnLogin.Text = busy ? "Đang đăng nhập..." : "Đăng nhập";
        if (busy) lblError.Text = "";
    }
}
