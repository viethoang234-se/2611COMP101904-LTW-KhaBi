#nullable disable
namespace QuanLyMuonTra.App.Forms
{
    partial class MainForm
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
            this.pnlMain = new QuanLyMuonTra.UI.ThemedPanel();
            this.pnlContent = new QuanLyMuonTra.UI.ThemedPanel();
            this.pnlHeader = new QuanLyMuonTra.UI.ThemedPanel();
            this.btnTheme = new QuanLyMuonTra.UI.AppButton();
            this.lblPageTitle = new QuanLyMuonTra.UI.AppLabel();
            this.pnlSidebar = new QuanLyMuonTra.UI.ThemedPanel();
            this.pnlNav = new QuanLyMuonTra.UI.ThemedPanel();
            this.pnlUser = new QuanLyMuonTra.UI.ThemedPanel();
            this.btnLogout = new QuanLyMuonTra.UI.AppButton();
            this.lblUserRole = new QuanLyMuonTra.UI.AppLabel();
            this.lblUserName = new QuanLyMuonTra.UI.AppLabel();
            this.pnlLogo = new QuanLyMuonTra.UI.ThemedPanel();
            this.lblLogoText = new QuanLyMuonTra.UI.AppLabel();
            this.lblLogoIcon = new QuanLyMuonTra.UI.AppLabel();
            this.pnlMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlSidebar.SuspendLayout();
            this.pnlUser.SuspendLayout();
            this.pnlLogo.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlMain
            //
            this.pnlMain.Controls.Add(this.pnlContent);
            this.pnlMain.Controls.Add(this.pnlHeader);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(250, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Size = new System.Drawing.Size(1030, 780);
            this.pnlMain.TabIndex = 0;
            //
            // pnlContent
            //
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(0, 72);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Padding = new System.Windows.Forms.Padding(24);
            this.pnlContent.Size = new System.Drawing.Size(1030, 708);
            this.pnlContent.TabIndex = 1;
            //
            // pnlHeader
            //
            this.pnlHeader.Controls.Add(this.btnTheme);
            this.pnlHeader.Controls.Add(this.lblPageTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Edge = QuanLyMuonTra.UI.PanelEdge.Bottom;
            this.pnlHeader.Fill = QuanLyMuonTra.UI.PanelFill.Surface;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1030, 72);
            this.pnlHeader.TabIndex = 0;
            //
            // btnTheme
            //
            this.btnTheme.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTheme.IconGlyph = "\uE708";
            this.btnTheme.Kind = QuanLyMuonTra.UI.ButtonKind.Secondary;
            this.btnTheme.Location = new System.Drawing.Point(850, 16);
            this.btnTheme.Name = "btnTheme";
            this.btnTheme.Size = new System.Drawing.Size(152, 40);
            this.btnTheme.TabIndex = 1;
            this.btnTheme.Text = "Giao diện tối";
            //
            // lblPageTitle
            //
            this.lblPageTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold);
            this.lblPageTitle.Location = new System.Drawing.Point(28, 17);
            this.lblPageTitle.Name = "lblPageTitle";
            this.lblPageTitle.Size = new System.Drawing.Size(600, 38);
            this.lblPageTitle.TabIndex = 0;
            this.lblPageTitle.Text = "Tổng quan";
            this.lblPageTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlSidebar
            //
            this.pnlSidebar.Controls.Add(this.pnlNav);
            this.pnlSidebar.Controls.Add(this.pnlUser);
            this.pnlSidebar.Controls.Add(this.pnlLogo);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Edge = QuanLyMuonTra.UI.PanelEdge.Right;
            this.pnlSidebar.Fill = QuanLyMuonTra.UI.PanelFill.Surface;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(250, 780);
            this.pnlSidebar.TabIndex = 1;
            //
            // pnlNav
            //
            this.pnlNav.AutoScroll = true;
            this.pnlNav.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlNav.Fill = QuanLyMuonTra.UI.PanelFill.Surface;
            this.pnlNav.Location = new System.Drawing.Point(0, 72);
            this.pnlNav.Name = "pnlNav";
            this.pnlNav.Size = new System.Drawing.Size(249, 604);
            this.pnlNav.TabIndex = 2;
            //
            // pnlUser
            //
            this.pnlUser.Controls.Add(this.btnLogout);
            this.pnlUser.Controls.Add(this.lblUserRole);
            this.pnlUser.Controls.Add(this.lblUserName);
            this.pnlUser.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlUser.Fill = QuanLyMuonTra.UI.PanelFill.Surface;
            this.pnlUser.Location = new System.Drawing.Point(0, 676);
            this.pnlUser.Name = "pnlUser";
            this.pnlUser.Size = new System.Drawing.Size(249, 104);
            this.pnlUser.TabIndex = 1;
            //
            // btnLogout
            //
            this.btnLogout.IconGlyph = "\uF3B1";
            this.btnLogout.Kind = QuanLyMuonTra.UI.ButtonKind.Secondary;
            this.btnLogout.Location = new System.Drawing.Point(16, 58);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(217, 36);
            this.btnLogout.TabIndex = 2;
            this.btnLogout.Text = "Đăng xuất";
            //
            // lblUserRole
            //
            this.lblUserRole.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblUserRole.Location = new System.Drawing.Point(20, 32);
            this.lblUserRole.Name = "lblUserRole";
            this.lblUserRole.Size = new System.Drawing.Size(210, 20);
            this.lblUserRole.TabIndex = 1;
            this.lblUserRole.Text = "Nhân viên kho";
            this.lblUserRole.Tone = QuanLyMuonTra.UI.TextTone.Secondary;
            //
            // lblUserName
            //
            this.lblUserName.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblUserName.Location = new System.Drawing.Point(20, 8);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(210, 24);
            this.lblUserName.TabIndex = 0;
            this.lblUserName.Text = "Họ và tên";
            //
            // pnlLogo
            //
            this.pnlLogo.Controls.Add(this.lblLogoText);
            this.pnlLogo.Controls.Add(this.lblLogoIcon);
            this.pnlLogo.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLogo.Fill = QuanLyMuonTra.UI.PanelFill.Surface;
            this.pnlLogo.Location = new System.Drawing.Point(0, 0);
            this.pnlLogo.Name = "pnlLogo";
            this.pnlLogo.Size = new System.Drawing.Size(249, 72);
            this.pnlLogo.TabIndex = 0;
            //
            // lblLogoText
            //
            this.lblLogoText.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblLogoText.Location = new System.Drawing.Point(64, 18);
            this.lblLogoText.Name = "lblLogoText";
            this.lblLogoText.Size = new System.Drawing.Size(176, 36);
            this.lblLogoText.TabIndex = 1;
            this.lblLogoText.Text = "Mượn Trả Thiết Bị";
            this.lblLogoText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // lblLogoIcon
            //
            this.lblLogoIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 22F);
            this.lblLogoIcon.Location = new System.Drawing.Point(18, 14);
            this.lblLogoIcon.Name = "lblLogoIcon";
            this.lblLogoIcon.Size = new System.Drawing.Size(42, 44);
            this.lblLogoIcon.TabIndex = 0;
            this.lblLogoIcon.Text = "\uE7F4";
            this.lblLogoIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblLogoIcon.Tone = QuanLyMuonTra.UI.TextTone.Accent;
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(1280, 780);
            this.Controls.Add(this.pnlMain);
            this.Controls.Add(this.pnlSidebar);
            this.MinimumSize = new System.Drawing.Size(1100, 700);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Quản lý mượn trả thiết bị";
            this.pnlMain.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlSidebar.ResumeLayout(false);
            this.pnlUser.ResumeLayout(false);
            this.pnlLogo.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private QuanLyMuonTra.UI.ThemedPanel pnlMain;
        private QuanLyMuonTra.UI.ThemedPanel pnlContent;
        private QuanLyMuonTra.UI.ThemedPanel pnlHeader;
        private QuanLyMuonTra.UI.AppButton btnTheme;
        private QuanLyMuonTra.UI.AppLabel lblPageTitle;
        private QuanLyMuonTra.UI.ThemedPanel pnlSidebar;
        private QuanLyMuonTra.UI.ThemedPanel pnlNav;
        private QuanLyMuonTra.UI.ThemedPanel pnlUser;
        private QuanLyMuonTra.UI.AppButton btnLogout;
        private QuanLyMuonTra.UI.AppLabel lblUserRole;
        private QuanLyMuonTra.UI.AppLabel lblUserName;
        private QuanLyMuonTra.UI.ThemedPanel pnlLogo;
        private QuanLyMuonTra.UI.AppLabel lblLogoText;
        private QuanLyMuonTra.UI.AppLabel lblLogoIcon;
    }
}
