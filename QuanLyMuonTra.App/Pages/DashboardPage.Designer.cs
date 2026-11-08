#nullable disable
namespace QuanLyMuonTra.App.Pages
{
    partial class DashboardPage
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyleDate1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyleDate2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlRecent = new QuanLyMuonTra.UI.CardPanel();
            this.dgvRecent = new System.Windows.Forms.DataGridView();
            this.colSlipCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBorrower = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colBorrowDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDueDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblRecentTitle = new QuanLyMuonTra.UI.AppLabel();
            this.pnlStats = new QuanLyMuonTra.UI.ThemedPanel();
            this.cardOverdue = new QuanLyMuonTra.UI.StatCard();
            this.cardActive = new QuanLyMuonTra.UI.StatCard();
            this.cardAvailable = new QuanLyMuonTra.UI.StatCard();
            this.cardDevices = new QuanLyMuonTra.UI.StatCard();
            this.pnlRecent.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecent)).BeginInit();
            this.pnlStats.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlRecent
            //
            this.pnlRecent.Controls.Add(this.dgvRecent);
            this.pnlRecent.Controls.Add(this.lblRecentTitle);
            this.pnlRecent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlRecent.Location = new System.Drawing.Point(0, 128);
            this.pnlRecent.Name = "pnlRecent";
            this.pnlRecent.Padding = new System.Windows.Forms.Padding(20, 16, 20, 16);
            this.pnlRecent.Size = new System.Drawing.Size(982, 372);
            this.pnlRecent.TabIndex = 1;
            //
            // dgvRecent
            //
            this.dgvRecent.AutoGenerateColumns = false;
            this.dgvRecent.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSlipCode,
            this.colBorrower,
            this.colBorrowDate,
            this.colDueDate,
            this.colStatus});
            this.dgvRecent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRecent.Location = new System.Drawing.Point(20, 58);
            this.dgvRecent.Name = "dgvRecent";
            this.dgvRecent.Size = new System.Drawing.Size(942, 298);
            this.dgvRecent.TabIndex = 1;
            //
            // colSlipCode
            //
            this.colSlipCode.DataPropertyName = "SlipCode";
            this.colSlipCode.FillWeight = 18F;
            this.colSlipCode.HeaderText = "Mã phiếu";
            this.colSlipCode.Name = "colSlipCode";
            //
            // colBorrower
            //
            this.colBorrower.DataPropertyName = "BorrowerName";
            this.colBorrower.FillWeight = 34F;
            this.colBorrower.HeaderText = "Người mượn";
            this.colBorrower.Name = "colBorrower";
            //
            // colBorrowDate
            //
            dataGridViewCellStyleDate1.Format = "dd/MM/yyyy";
            this.colBorrowDate.DataPropertyName = "BorrowDate";
            this.colBorrowDate.DefaultCellStyle = dataGridViewCellStyleDate1;
            this.colBorrowDate.FillWeight = 16F;
            this.colBorrowDate.HeaderText = "Ngày mượn";
            this.colBorrowDate.Name = "colBorrowDate";
            //
            // colDueDate
            //
            dataGridViewCellStyleDate2.Format = "dd/MM/yyyy";
            this.colDueDate.DataPropertyName = "DueDate";
            this.colDueDate.DefaultCellStyle = dataGridViewCellStyleDate2;
            this.colDueDate.FillWeight = 16F;
            this.colDueDate.HeaderText = "Hạn trả";
            this.colDueDate.Name = "colDueDate";
            //
            // colStatus
            //
            this.colStatus.DataPropertyName = "Status";
            this.colStatus.FillWeight = 20F;
            this.colStatus.HeaderText = "Trạng thái";
            this.colStatus.Name = "colStatus";
            //
            // lblRecentTitle
            //
            this.lblRecentTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblRecentTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold);
            this.lblRecentTitle.Location = new System.Drawing.Point(20, 16);
            this.lblRecentTitle.Name = "lblRecentTitle";
            this.lblRecentTitle.Size = new System.Drawing.Size(942, 42);
            this.lblRecentTitle.TabIndex = 0;
            this.lblRecentTitle.Text = "Phiếu mượn gần đây";
            this.lblRecentTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // pnlStats
            //
            this.pnlStats.Controls.Add(this.cardOverdue);
            this.pnlStats.Controls.Add(this.cardActive);
            this.pnlStats.Controls.Add(this.cardAvailable);
            this.pnlStats.Controls.Add(this.cardDevices);
            this.pnlStats.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStats.Location = new System.Drawing.Point(0, 0);
            this.pnlStats.Name = "pnlStats";
            this.pnlStats.Size = new System.Drawing.Size(982, 128);
            this.pnlStats.TabIndex = 0;
            //
            // cardOverdue
            //
            this.cardOverdue.Accent = QuanLyMuonTra.UI.TextTone.Danger;
            this.cardOverdue.IconGlyph = "\uE7BA";
            this.cardOverdue.Location = new System.Drawing.Point(738, 0);
            this.cardOverdue.Name = "cardOverdue";
            this.cardOverdue.Size = new System.Drawing.Size(244, 104);
            this.cardOverdue.TabIndex = 3;
            this.cardOverdue.Title = "Phiếu quá hạn";
            //
            // cardActive
            //
            this.cardActive.Accent = QuanLyMuonTra.UI.TextTone.Warning;
            this.cardActive.IconGlyph = "\uE787";
            this.cardActive.Location = new System.Drawing.Point(492, 0);
            this.cardActive.Name = "cardActive";
            this.cardActive.Size = new System.Drawing.Size(230, 104);
            this.cardActive.TabIndex = 2;
            this.cardActive.Title = "Phiếu đang mượn";
            //
            // cardAvailable
            //
            this.cardAvailable.Accent = QuanLyMuonTra.UI.TextTone.Success;
            this.cardAvailable.IconGlyph = "\uE8FB";
            this.cardAvailable.Location = new System.Drawing.Point(246, 0);
            this.cardAvailable.Name = "cardAvailable";
            this.cardAvailable.Size = new System.Drawing.Size(230, 104);
            this.cardAvailable.TabIndex = 1;
            this.cardAvailable.Title = "Thiết bị sẵn sàng";
            //
            // cardDevices
            //
            this.cardDevices.IconGlyph = "\uE7F4";
            this.cardDevices.Location = new System.Drawing.Point(0, 0);
            this.cardDevices.Name = "cardDevices";
            this.cardDevices.Size = new System.Drawing.Size(230, 104);
            this.cardDevices.TabIndex = 0;
            this.cardDevices.Title = "Tổng số thiết bị";
            //
            // DashboardPage
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pnlRecent);
            this.Controls.Add(this.pnlStats);
            this.Name = "DashboardPage";
            this.Size = new System.Drawing.Size(982, 500);
            this.pnlRecent.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRecent)).EndInit();
            this.pnlStats.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private QuanLyMuonTra.UI.CardPanel pnlRecent;
        private System.Windows.Forms.DataGridView dgvRecent;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSlipCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBorrower;
        private System.Windows.Forms.DataGridViewTextBoxColumn colBorrowDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDueDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colStatus;
        private QuanLyMuonTra.UI.AppLabel lblRecentTitle;
        private QuanLyMuonTra.UI.ThemedPanel pnlStats;
        private QuanLyMuonTra.UI.StatCard cardOverdue;
        private QuanLyMuonTra.UI.StatCard cardActive;
        private QuanLyMuonTra.UI.StatCard cardAvailable;
        private QuanLyMuonTra.UI.StatCard cardDevices;
    }
}
