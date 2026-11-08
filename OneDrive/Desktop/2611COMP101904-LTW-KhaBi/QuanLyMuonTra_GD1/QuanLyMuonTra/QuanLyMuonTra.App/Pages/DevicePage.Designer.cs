#nullable disable
namespace QuanLyMuonTra.App.Pages
{
    partial class DevicePage
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
            this.components = new System.ComponentModel.Container();
            this.pnlGrid = new QuanLyMuonTra.UI.CardPanel();
            this.dgvDevices = new System.Windows.Forms.DataGridView();
            this.colCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCategory = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colAvailable = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNote = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblCount = new QuanLyMuonTra.UI.AppLabel();
            this.pnlToolbar = new QuanLyMuonTra.UI.ThemedPanel();
            this.btnDelete = new QuanLyMuonTra.UI.AppButton();
            this.btnEdit = new QuanLyMuonTra.UI.AppButton();
            this.btnAdd = new QuanLyMuonTra.UI.AppButton();
            this.cboCategory = new System.Windows.Forms.ComboBox();
            this.txtSearch = new QuanLyMuonTra.UI.AppTextBox();
            this.timerSearch = new System.Windows.Forms.Timer(this.components);
            this.pnlGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevices)).BeginInit();
            this.pnlToolbar.SuspendLayout();
            this.SuspendLayout();
            //
            // pnlGrid
            //
            this.pnlGrid.Controls.Add(this.dgvDevices);
            this.pnlGrid.Controls.Add(this.lblCount);
            this.pnlGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlGrid.Location = new System.Drawing.Point(0, 68);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Padding = new System.Windows.Forms.Padding(16, 12, 16, 8);
            this.pnlGrid.Size = new System.Drawing.Size(982, 432);
            this.pnlGrid.TabIndex = 1;
            //
            // dgvDevices
            //
            this.dgvDevices.AutoGenerateColumns = false;
            this.dgvDevices.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCode,
            this.colName,
            this.colCategory,
            this.colTotal,
            this.colAvailable,
            this.colUnit,
            this.colNote});
            this.dgvDevices.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDevices.Location = new System.Drawing.Point(16, 12);
            this.dgvDevices.Name = "dgvDevices";
            this.dgvDevices.Size = new System.Drawing.Size(950, 380);
            this.dgvDevices.TabIndex = 0;
            //
            // colCode
            //
            this.colCode.DataPropertyName = "DeviceCode";
            this.colCode.FillWeight = 12F;
            this.colCode.HeaderText = "Mã";
            this.colCode.Name = "colCode";
            //
            // colName
            //
            this.colName.DataPropertyName = "DeviceName";
            this.colName.FillWeight = 30F;
            this.colName.HeaderText = "Tên thiết bị";
            this.colName.Name = "colName";
            //
            // colCategory
            //
            this.colCategory.DataPropertyName = "CategoryName";
            this.colCategory.FillWeight = 20F;
            this.colCategory.HeaderText = "Loại";
            this.colCategory.Name = "colCategory";
            //
            // colTotal
            //
            this.colTotal.DataPropertyName = "TotalQuantity";
            this.colTotal.FillWeight = 10F;
            this.colTotal.HeaderText = "Tổng SL";
            this.colTotal.Name = "colTotal";
            //
            // colAvailable
            //
            this.colAvailable.DataPropertyName = "AvailableQuantity";
            this.colAvailable.FillWeight = 10F;
            this.colAvailable.HeaderText = "Còn lại";
            this.colAvailable.Name = "colAvailable";
            //
            // colUnit
            //
            this.colUnit.DataPropertyName = "Unit";
            this.colUnit.FillWeight = 8F;
            this.colUnit.HeaderText = "Đơn vị";
            this.colUnit.Name = "colUnit";
            //
            // colNote
            //
            this.colNote.DataPropertyName = "Note";
            this.colNote.FillWeight = 22F;
            this.colNote.HeaderText = "Ghi chú";
            this.colNote.Name = "colNote";
            //
            // lblCount
            //
            this.lblCount.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.lblCount.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblCount.Location = new System.Drawing.Point(16, 392);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(950, 32);
            this.lblCount.TabIndex = 1;
            this.lblCount.Text = "Hiển thị 0 thiết bị";
            this.lblCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblCount.Tone = QuanLyMuonTra.UI.TextTone.Secondary;
            //
            // pnlToolbar
            //
            this.pnlToolbar.Controls.Add(this.btnDelete);
            this.pnlToolbar.Controls.Add(this.btnEdit);
            this.pnlToolbar.Controls.Add(this.btnAdd);
            this.pnlToolbar.Controls.Add(this.cboCategory);
            this.pnlToolbar.Controls.Add(this.txtSearch);
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Location = new System.Drawing.Point(0, 0);
            this.pnlToolbar.Name = "pnlToolbar";
            this.pnlToolbar.Size = new System.Drawing.Size(982, 68);
            this.pnlToolbar.TabIndex = 0;
            //
            // btnDelete
            //
            this.btnDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDelete.IconGlyph = "\uE74D";
            this.btnDelete.Kind = QuanLyMuonTra.UI.ButtonKind.Danger;
            this.btnDelete.Location = new System.Drawing.Point(890, 0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(92, 46);
            this.btnDelete.TabIndex = 4;
            this.btnDelete.Text = "Xóa";
            //
            // btnEdit
            //
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.IconGlyph = "\uE70F";
            this.btnEdit.Kind = QuanLyMuonTra.UI.ButtonKind.Secondary;
            this.btnEdit.Location = new System.Drawing.Point(788, 0);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(94, 46);
            this.btnEdit.TabIndex = 3;
            this.btnEdit.Text = "Sửa";
            //
            // btnAdd
            //
            this.btnAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAdd.IconGlyph = "\uE710";
            this.btnAdd.Location = new System.Drawing.Point(630, 0);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(150, 46);
            this.btnAdd.TabIndex = 2;
            this.btnAdd.Text = "Thêm thiết bị";
            //
            // cboCategory
            //
            this.cboCategory.DisplayMember = "CategoryName";
            this.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboCategory.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            this.cboCategory.FormattingEnabled = true;
            this.cboCategory.Location = new System.Drawing.Point(356, 8);
            this.cboCategory.Name = "cboCategory";
            this.cboCategory.Size = new System.Drawing.Size(220, 31);
            this.cboCategory.TabIndex = 1;
            this.cboCategory.ValueMember = "CategoryId";
            //
            // txtSearch
            //
            this.txtSearch.IconGlyph = "\uE721";
            this.txtSearch.Location = new System.Drawing.Point(0, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "Tìm theo mã hoặc tên thiết bị...";
            this.txtSearch.Size = new System.Drawing.Size(340, 46);
            this.txtSearch.TabIndex = 0;
            //
            // timerSearch
            //
            this.timerSearch.Interval = 300;
            //
            // DevicePage
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.Controls.Add(this.pnlGrid);
            this.Controls.Add(this.pnlToolbar);
            this.Name = "DevicePage";
            this.Size = new System.Drawing.Size(982, 500);
            this.pnlGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDevices)).EndInit();
            this.pnlToolbar.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private QuanLyMuonTra.UI.CardPanel pnlGrid;
        private System.Windows.Forms.DataGridView dgvDevices;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCategory;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTotal;
        private System.Windows.Forms.DataGridViewTextBoxColumn colAvailable;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNote;
        private QuanLyMuonTra.UI.AppLabel lblCount;
        private QuanLyMuonTra.UI.ThemedPanel pnlToolbar;
        private QuanLyMuonTra.UI.AppButton btnDelete;
        private QuanLyMuonTra.UI.AppButton btnEdit;
        private QuanLyMuonTra.UI.AppButton btnAdd;
        private System.Windows.Forms.ComboBox cboCategory;
        private QuanLyMuonTra.UI.AppTextBox txtSearch;
        private System.Windows.Forms.Timer timerSearch;
    }
}
