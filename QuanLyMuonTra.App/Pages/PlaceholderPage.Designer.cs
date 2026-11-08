#nullable disable
namespace QuanLyMuonTra.App.Pages
{
    partial class PlaceholderPage
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.card = new QuanLyMuonTra.UI.CardPanel();
            this.lblNote = new QuanLyMuonTra.UI.AppLabel();
            this.lblTitle = new QuanLyMuonTra.UI.AppLabel();
            this.lblIcon = new QuanLyMuonTra.UI.AppLabel();
            this.card.SuspendLayout();
            this.SuspendLayout();
            //
            // card
            //
            this.card.Controls.Add(this.lblNote);
            this.card.Controls.Add(this.lblTitle);
            this.card.Controls.Add(this.lblIcon);
            this.card.Dock = System.Windows.Forms.DockStyle.Fill;
            this.card.Location = new System.Drawing.Point(0, 0);
            this.card.Name = "card";
            this.card.Padding = new System.Windows.Forms.Padding(24, 140, 24, 24);
            this.card.Size = new System.Drawing.Size(800, 500);
            this.card.TabIndex = 0;
            //
            // lblNote
            //
            this.lblNote.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblNote.Location = new System.Drawing.Point(24, 252);
            this.lblNote.Name = "lblNote";
            this.lblNote.Size = new System.Drawing.Size(752, 56);
            this.lblNote.TabIndex = 2;
            this.lblNote.Text = "Mô tả chức năng";
            this.lblNote.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblNote.Tone = QuanLyMuonTra.UI.TextTone.Secondary;
            //
            // lblTitle
            //
            this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(24, 204);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(752, 48);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Chức năng đang phát triển";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // lblIcon
            //
            this.lblIcon.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblIcon.Font = new System.Drawing.Font("Segoe MDL2 Assets", 40F);
            this.lblIcon.Location = new System.Drawing.Point(24, 140);
            this.lblIcon.Name = "lblIcon";
            this.lblIcon.Size = new System.Drawing.Size(752, 64);
            this.lblIcon.TabIndex = 0;
            this.lblIcon.Text = "\uE713";
            this.lblIcon.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblIcon.Tone = QuanLyMuonTra.UI.TextTone.Accent;
            //
            // PlaceholderPage
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.card);
            this.Name = "PlaceholderPage";
            this.Size = new System.Drawing.Size(800, 500);
            this.card.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private QuanLyMuonTra.UI.CardPanel card;
        private QuanLyMuonTra.UI.AppLabel lblNote;
        private QuanLyMuonTra.UI.AppLabel lblTitle;
        private QuanLyMuonTra.UI.AppLabel lblIcon;
    }
}
