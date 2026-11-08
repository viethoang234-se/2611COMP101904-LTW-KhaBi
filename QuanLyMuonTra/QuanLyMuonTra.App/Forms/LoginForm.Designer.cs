#nullable disable
namespace QuanLyMuonTra.App.Forms
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pnlForm = new QuanLyMuonTra.UI.ThemedPanel();
            this.lblError = new QuanLyMuonTra.UI.AppLabel();
            this.btnLogin = new QuanLyMuonTra.UI.AppButton();
            this.txtPass = new QuanLyMuonTra.UI.AppTextBox();
            this.lblPassCaption = new QuanLyMuonTra.UI.AppLabel();
            this.txtUser = new QuanLyMuonTra.UI.AppTextBox();
            this.lblUserCaption = new QuanLyMuonTra.UI.AppLabel();
            this.lblHint = new QuanLyMuonTra.UI.AppLabel();
            this.lblWelcome = new QuanLyMuonTra.UI.AppLabel();
            this.lblClose = new QuanLyMuonTra.UI.AppLabel();
            this.pnlBrand = new QuanLyMuonTra.UI.GradientPanel();
            this.lblBrandFooter = new System.Windows.Forms.Label();
            this.lblBrandSub = new System.Windows.Forms.Label();
            this.lblBrandTitle = new System.Windows.Forms.Label();
            this.lblBrandIcon = new System.Windows.Forms.Label();
            this.pnlForm.SuspendLayout();
            this.pnlBrand.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlForm
            //
            this.pnlForm.Controls.Add(this.lblError);
            this.pnlForm.Controls.Add(this.btnLogin);
            this.pnlForm.Controls.Add(this.txtPass);
            this.pnlForm.Controls.Add(this.lblPassCaption);
            this.pnlForm.Controls.Add(this.txtUser);
            this.pnlForm.Controls.Add(this.lblUserCaption);
            this.pnlForm.Controls.Add(this.lblHint);
            this.pnlForm.Controls.Add(this.lblWelcome);
            this.pnlForm.Controls.Add(this.lblClose);
            this.pnlForm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlForm.Fill = QuanLyMuonTra.UI.PanelFill.Surface;
            this.pnlForm.Location = new System.Drawing.Point(440, 0);
            this.pnlForm.Name = "pnlForm";
            this.pnlForm.Size = new System.Drawing.Size(560, 600);
            this.pnlForm.TabIndex = 0;
            //
            // lblError
            //
            this.lblError.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblError.Location = new System.Drawing.Point(80, 380);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(400, 56);
            this.lblError.TabIndex = 8;
            this.lblError.Tone = QuanLyMuonTra.UI.TextTone.Danger;
            //
            // btnLogin
            //
            this.btnLogin.Font = new System.Drawing.Font("Segoe UI Semibold", 11F, System.Drawing.FontStyle.Bold);
            this.btnLogin.Location = new System.Drawing.Point(80, 444);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(400, 48);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "Đăng nhập";
            //
            // txtPass
            //
            this.txtPass.IconGlyph = "\uE72E";
            this.txtPass.Location = new System.Drawing.Point(80, 328);
            this.txtPass.Name = "txtPass";
            this.txtPass.PlaceholderText = "Nhập mật khẩu";
            this.txtPass.Size = new System.Drawing.Size(400, 46);
            this.txtPass.TabIndex = 2;
            this.txtPass.UseSystemPasswordChar = true;
            //
            // lblPassCaption
            //
            this.lblPassCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblPassCaption.Location = new System.Drawing.Point(80, 302);
            this.lblPassCaption.Name = "lblPassCaption";
            this.lblPassCaption.Size = new System.Drawing.Size(400, 22);
            this.lblPassCaption.TabIndex = 6;
            this.lblPassCaption.Text = "Mật khẩu";
            //
            // txtUser
            //
            this.txtUser.IconGlyph = "\uE77B";
            this.txtUser.Location = new System.Drawing.Point(80, 238);
            this.txtUser.Name = "txtUser";
            this.txtUser.PlaceholderText = "Nhập tên đăng nhập";
            this.txtUser.Size = new System.Drawing.Size(400, 46);
            this.txtUser.TabIndex = 1;
            //
            // lblUserCaption
            //
            this.lblUserCaption.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblUserCaption.Location = new System.Drawing.Point(80, 212);
            this.lblUserCaption.Name = "lblUserCaption";
            this.lblUserCaption.Size = new System.Drawing.Size(400, 22);
            this.lblUserCaption.TabIndex = 4;
            this.lblUserCaption.Text = "Tên đăng nhập";
            //
            // lblHint
            //
            this.lblHint.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.lblHint.Location = new System.Drawing.Point(80, 158);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(400, 28);
            this.lblHint.TabIndex = 3;
            this.lblHint.Text = "Đăng nhập để quản lý mượn trả thiết bị";
            this.lblHint.Tone = QuanLyMuonTra.UI.TextTone.Secondary;
            //
            // lblWelcome
            //
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI Semibold", 22F, System.Drawing.FontStyle.Bold);
            this.lblWelcome.Location = new System.Drawing.Point(80, 108);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(400, 46);
            this.lblWelcome.TabIndex = 2;
            this.lblWelcome.Text = "Chào mừng trở lại";
            //
            // lblClose
            //
            this.lblClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblClose.Font = new System.Drawing.Font("Segoe MDL2 Assets", 11F);
            this.lblClose.Location = new System.Drawing.Point(508, 14);
            this.lblClose.Name = "lblClose";
            this.lblClose.Size = new System.Drawing.Size(38, 32);
            this.lblClose.TabIndex = 0;
            this.lblClose.Text = "\uE711";
            this.lblClose.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblClose.Tone = QuanLyMuonTra.UI.TextTone.Secondary;
            //
            // pnlBrand
            //
            this.pnlBrand.Controls.Add(this.lblBrandFooter);
            this.pnlBrand.Controls.Add(this.lblBrandSub);
            this.pnlBrand.Controls.Add(this.lblBrandTitle);
            this.pnlBrand.Controls.Add(this.lblBrandIcon);
            this.pnlBrand.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlBrand.Location = new System.Drawing.Point(0, 0);
            this.pnlBrand.Name = "pnlBrand";
            this.pnlBrand.Size = new System.Drawing.Size(440, 600);
            this.pnlBrand.TabIndex = 1;
            //
            // lblBrandFooter
            //
            this.lblBrandFooter.BackColor = System.Drawing.Color.Transparent;
            this.lblBrandFooter.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblBrandFooter.ForeColor = System.Drawing.Color.FromArgb(191, 219, 254);
            this.lblBrandFooter.Location = new System.Drawing.Point(48, 548);
            this.lblBrandFooter.Name = "lblBrandFooter";
            this.lblBrandFooter.Size = new System.Drawing.Size(350, 24);
            this.lblBrandFooter.TabIndex = 3;
            this.lblBrandFooter.Text = "© 2026 Quản lý mượn trả thiết bị";
            //
            // lblBrandSub
            //
            this.lblBrandSub.BackColor = System.Drawing.Color.Transparent;
            this.lblBrandSub.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.lblBrandSub.ForeColor = System.Drawing.Color.FromArgb(219, 234, 254);
            this.lblBrandSub.Location = new System.Drawing.Point(48, 330);
            this.lblBrandSub.Name = "lblBrandSub";
            this.lblBrandSub.Size = new System.Drawing.Size(340, 80);
            this.lblBrandSub.TabIndex = 2;
            this.lblBrandSub.Text = "Theo dõi thiết bị, phiếu mượn và biên bản trả — gọn gàng, minh bạch.";
            //
            // lblBrandTitle
            //
            this.lblBrandTitle.BackColor = System.Drawing.Color.Transparent;
            this.lblBrandTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 24F, System.Drawing.FontStyle.Bold);
            this.lblBrandTitle.ForeColor = System.Drawing.Color.White;
            this.lblBrandTitle.Location = new System.Drawing.Point(48, 214);
            this.lblBrandTitle.Name = "lblBrandTitle";
            this.lblBrandTitle.Size = new System.Drawing.Size(360, 104);
            this.lblBrandTitle.TabIndex = 1;
            this.lblBrandTitle.Text = "QUẢN LÝ MƯỢN TRẢ THIẾT BỊ";
            //
            // lblBrandIcon
            //
            this.lblBrandIcon.BackColor = System.Drawing.Color.Transparent;
            this.lblBrandIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 40F);
            this.lblBrandIcon.ForeColor = System.Drawing.Color.White;
            this.lblBrandIcon.Location = new System.Drawing.Point(42, 90);
            this.lblBrandIcon.Name = "lblBrandIcon";
            this.lblBrandIcon.Size = new System.Drawing.Size(96, 80);
            this.lblBrandIcon.TabIndex = 0;
            this.lblBrandIcon.Text = "\uE7F4";
            //
            // LoginForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1000, 600);
            this.Controls.Add(this.pnlForm);
            this.Controls.Add(this.pnlBrand);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Đăng nhập";
            this.pnlForm.ResumeLayout(false);
            this.pnlBrand.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private QuanLyMuonTra.UI.ThemedPanel pnlForm;
        private QuanLyMuonTra.UI.AppLabel lblError;
        private QuanLyMuonTra.UI.AppButton btnLogin;
        private QuanLyMuonTra.UI.AppTextBox txtPass;
        private QuanLyMuonTra.UI.AppLabel lblPassCaption;
        private QuanLyMuonTra.UI.AppTextBox txtUser;
        private QuanLyMuonTra.UI.AppLabel lblUserCaption;
        private QuanLyMuonTra.UI.AppLabel lblHint;
        private QuanLyMuonTra.UI.AppLabel lblWelcome;
        private QuanLyMuonTra.UI.AppLabel lblClose;
        private QuanLyMuonTra.UI.GradientPanel pnlBrand;
        private System.Windows.Forms.Label lblBrandFooter;
        private System.Windows.Forms.Label lblBrandSub;
        private System.Windows.Forms.Label lblBrandTitle;
        private System.Windows.Forms.Label lblBrandIcon;
    }
}
