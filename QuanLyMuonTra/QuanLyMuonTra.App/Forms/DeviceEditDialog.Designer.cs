#nullable disable
namespace QuanLyMuonTra.App.Forms
{
    partial class DeviceEditDialog
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
            this.pnlBody = new QuanLyMuonTra.UI.ThemedPanel();
            this.btnSave = new QuanLyMuonTra.UI.AppButton();
            this.btnCancel = new QuanLyMuonTra.UI.AppButton();
            this.lblError = new QuanLyMuonTra.UI.AppLabel();
            this.txtNote = new QuanLyMuonTra.UI.AppTextBox();
            this.lblNoteCap = new QuanLyMuonTra.UI.AppLabel();
            this.txtUnit = new QuanLyMuonTra.UI.AppTextBox();
            this.lblUnitCap = new QuanLyMuonTra.UI.AppLabel();
            this.nudQuantity = new System.Windows.Forms.NumericUpDown();
            this.lblQtyCap = new QuanLyMuonTra.UI.AppLabel();
            this.cboCategory = new System.Windows.Forms.ComboBox();
            this.lblCatCap = new QuanLyMuonTra.UI.AppLabel();
            this.txtName = new QuanLyMuonTra.UI.AppTextBox();
            this.lblNameCap = new QuanLyMuonTra.UI.AppLabel();
            this.txtCode = new QuanLyMuonTra.UI.AppTextBox();
            this.lblCodeCap = new QuanLyMuonTra.UI.AppLabel();
            this.lblTitle = new QuanLyMuonTra.UI.AppLabel();
            this.pnlBody.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantity)).BeginInit();
            this.SuspendLayout();
            //
            // pnlBody
            //
            this.pnlBody.Controls.Add(this.btnSave);
            this.pnlBody.Controls.Add(this.btnCancel);
            this.pnlBody.Controls.Add(this.lblError);
            this.pnlBody.Controls.Add(this.txtNote);
            this.pnlBody.Controls.Add(this.lblNoteCap);
            this.pnlBody.Controls.Add(this.txtUnit);
            this.pnlBody.Controls.Add(this.lblUnitCap);
            this.pnlBody.Controls.Add(this.nudQuantity);
            this.pnlBody.Controls.Add(this.lblQtyCap);
            this.pnlBody.Controls.Add(this.cboCategory);
            this.pnlBody.Controls.Add(this.lblCatCap);
            this.pnlBody.Controls.Add(this.txtName);
            this.pnlBody.Controls.Add(this.lblNameCap);
            this.pnlBody.Controls.Add(this.txtCode);
            this.pnlBody.Controls.Add(this.lblCodeCap);
            this.pnlBody.Controls.Add(this.lblTitle);
            this.pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBody.Fill = QuanLyMuonTra.UI.PanelFill.Surface;
            this.pnlBody.Location = new System.Drawing.Point(0, 0);
            this.pnlBody.Name = "pnlBody";
            this.pnlBody.Size = new System.Drawing.Size(520, 578);
            this.pnlBody.TabIndex = 0;
            //
            // btnSave
            //
            this.btnSave.IconGlyph = "\uE74E";
            this.btnSave.Location = new System.Drawing.Point(372, 506);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(120, 44);
            this.btnSave.TabIndex = 7;
            this.btnSave.Text = "Lưu";
            //
            // btnCancel
            //
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Kind = QuanLyMuonTra.UI.ButtonKind.Secondary;
            this.btnCancel.Location = new System.Drawing.Point(252, 506);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(112, 44);
            this.btnCancel.TabIndex = 8;
            this.btnCancel.Text = "Hủy";
            //
            // lblError
            //
            this.lblError.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.lblError.Location = new System.Drawing.Point(28, 474);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(464, 24);
            this.lblError.TabIndex = 15;
            this.lblError.Tone = QuanLyMuonTra.UI.TextTone.Danger;
            //
            // txtNote
            //
            this.txtNote.Location = new System.Drawing.Point(28, 420);
            this.txtNote.Name = "txtNote";
            this.txtNote.PlaceholderText = "Không bắt buộc";
            this.txtNote.Size = new System.Drawing.Size(464, 44);
            this.txtNote.TabIndex = 6;
            //
            // lblNoteCap
            //
            this.lblNoteCap.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblNoteCap.Location = new System.Drawing.Point(28, 394);
            this.lblNoteCap.Name = "lblNoteCap";
            this.lblNoteCap.Size = new System.Drawing.Size(464, 22);
            this.lblNoteCap.TabIndex = 14;
            this.lblNoteCap.Text = "Ghi chú";
            //
            // txtUnit
            //
            this.txtUnit.Location = new System.Drawing.Point(268, 340);
            this.txtUnit.Name = "txtUnit";
            this.txtUnit.Size = new System.Drawing.Size(224, 44);
            this.txtUnit.TabIndex = 5;
            //
            // lblUnitCap
            //
            this.lblUnitCap.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblUnitCap.Location = new System.Drawing.Point(268, 314);
            this.lblUnitCap.Name = "lblUnitCap";
            this.lblUnitCap.Size = new System.Drawing.Size(224, 22);
            this.lblUnitCap.TabIndex = 13;
            this.lblUnitCap.Text = "Đơn vị tính";
            //
            // nudQuantity
            //
            this.nudQuantity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.nudQuantity.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.nudQuantity.Location = new System.Drawing.Point(28, 347);
            this.nudQuantity.Maximum = new decimal(new int[] { 100000, 0, 0, 0 });
            this.nudQuantity.Name = "nudQuantity";
            this.nudQuantity.Size = new System.Drawing.Size(224, 32);
            this.nudQuantity.TabIndex = 4;
            this.nudQuantity.ThousandsSeparator = true;
            //
            // lblQtyCap
            //
            this.lblQtyCap.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblQtyCap.Location = new System.Drawing.Point(28, 314);
            this.lblQtyCap.Name = "lblQtyCap";
            this.lblQtyCap.Size = new System.Drawing.Size(224, 22);
            this.lblQtyCap.TabIndex = 12;
            this.lblQtyCap.Text = "Tổng số lượng";
            //
            // cboCategory
            //
            this.cboCategory.DisplayMember = "CategoryName";
            this.cboCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cboCategory.Font = new System.Drawing.Font("Segoe UI", 11F);
            this.cboCategory.FormattingEnabled = true;
            this.cboCategory.Location = new System.Drawing.Point(28, 274);
            this.cboCategory.Name = "cboCategory";
            this.cboCategory.Size = new System.Drawing.Size(464, 33);
            this.cboCategory.TabIndex = 3;
            this.cboCategory.ValueMember = "CategoryId";
            //
            // lblCatCap
            //
            this.lblCatCap.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblCatCap.Location = new System.Drawing.Point(28, 244);
            this.lblCatCap.Name = "lblCatCap";
            this.lblCatCap.Size = new System.Drawing.Size(464, 22);
            this.lblCatCap.TabIndex = 11;
            this.lblCatCap.Text = "Loại thiết bị";
            //
            // txtName
            //
            this.txtName.Location = new System.Drawing.Point(28, 190);
            this.txtName.Name = "txtName";
            this.txtName.PlaceholderText = "Ví dụ: Laptop Dell Latitude 5440";
            this.txtName.Size = new System.Drawing.Size(464, 44);
            this.txtName.TabIndex = 2;
            //
            // lblNameCap
            //
            this.lblNameCap.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblNameCap.Location = new System.Drawing.Point(28, 164);
            this.lblNameCap.Name = "lblNameCap";
            this.lblNameCap.Size = new System.Drawing.Size(464, 22);
            this.lblNameCap.TabIndex = 10;
            this.lblNameCap.Text = "Tên thiết bị";
            //
            // txtCode
            //
            this.txtCode.Location = new System.Drawing.Point(28, 110);
            this.txtCode.Name = "txtCode";
            this.txtCode.Size = new System.Drawing.Size(464, 44);
            this.txtCode.TabIndex = 1;
            //
            // lblCodeCap
            //
            this.lblCodeCap.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.lblCodeCap.Location = new System.Drawing.Point(28, 84);
            this.lblCodeCap.Name = "lblCodeCap";
            this.lblCodeCap.Size = new System.Drawing.Size(464, 22);
            this.lblCodeCap.TabIndex = 9;
            this.lblCodeCap.Text = "Mã thiết bị";
            //
            // lblTitle
            //
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(28, 24);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(464, 40);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Thêm thiết bị";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            //
            // DeviceEditDialog
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(520, 578);
            this.Controls.Add(this.pnlBody);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeviceEditDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Thiết bị";
            this.pnlBody.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.nudQuantity)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private QuanLyMuonTra.UI.ThemedPanel pnlBody;
        private QuanLyMuonTra.UI.AppButton btnSave;
        private QuanLyMuonTra.UI.AppButton btnCancel;
        private QuanLyMuonTra.UI.AppLabel lblError;
        private QuanLyMuonTra.UI.AppTextBox txtNote;
        private QuanLyMuonTra.UI.AppLabel lblNoteCap;
        private QuanLyMuonTra.UI.AppTextBox txtUnit;
        private QuanLyMuonTra.UI.AppLabel lblUnitCap;
        private System.Windows.Forms.NumericUpDown nudQuantity;
        private QuanLyMuonTra.UI.AppLabel lblQtyCap;
        private System.Windows.Forms.ComboBox cboCategory;
        private QuanLyMuonTra.UI.AppLabel lblCatCap;
        private QuanLyMuonTra.UI.AppTextBox txtName;
        private QuanLyMuonTra.UI.AppLabel lblNameCap;
        private QuanLyMuonTra.UI.AppTextBox txtCode;
        private QuanLyMuonTra.UI.AppLabel lblCodeCap;
        private QuanLyMuonTra.UI.AppLabel lblTitle;
    }
}
