namespace RawMat.ViewsMaterial.Setting.form
{
    partial class frmAddNewNgMode
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtSerial = new System.Windows.Forms.TextBox();
            this.btnCancel = new Bunifu.Framework.UI.BunifuFlatButton();
            this.btnSave = new Bunifu.Framework.UI.BunifuFlatButton();
            this.label11 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // txtSerial
            // 
            this.txtSerial.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSerial.Location = new System.Drawing.Point(174, 36);
            this.txtSerial.Margin = new System.Windows.Forms.Padding(2);
            this.txtSerial.Name = "txtSerial";
            this.txtSerial.Size = new System.Drawing.Size(198, 26);
            this.txtSerial.TabIndex = 73;
            // 
            // btnCancel
            // 
            this.btnCancel.Activecolor = System.Drawing.Color.DimGray;
            this.btnCancel.BackColor = System.Drawing.Color.DarkGray;
            this.btnCancel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancel.BorderRadius = 0;
            this.btnCancel.ButtonText = "Cancel";
            this.btnCancel.DisabledColor = System.Drawing.Color.Gray;
            this.btnCancel.Iconcolor = System.Drawing.Color.Transparent;
            this.btnCancel.Iconimage = null;
            this.btnCancel.Iconimage_right = null;
            this.btnCancel.Iconimage_right_Selected = null;
            this.btnCancel.Iconimage_Selected = null;
            this.btnCancel.IconMarginLeft = 0;
            this.btnCancel.IconMarginRight = 0;
            this.btnCancel.IconRightVisible = true;
            this.btnCancel.IconRightZoom = 0D;
            this.btnCancel.IconVisible = true;
            this.btnCancel.IconZoom = 90D;
            this.btnCancel.IsTab = false;
            this.btnCancel.Location = new System.Drawing.Point(306, 81);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Normalcolor = System.Drawing.Color.DarkGray;
            this.btnCancel.OnHovercolor = System.Drawing.Color.Pink;
            this.btnCancel.OnHoverTextColor = System.Drawing.Color.Red;
            this.btnCancel.selected = false;
            this.btnCancel.Size = new System.Drawing.Size(82, 25);
            this.btnCancel.TabIndex = 72;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnCancel.Textcolor = System.Drawing.Color.White;
            this.btnCancel.TextFont = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // btnSave
            // 
            this.btnSave.Activecolor = System.Drawing.Color.DarkRed;
            this.btnSave.BackColor = System.Drawing.Color.Red;
            this.btnSave.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSave.BorderRadius = 0;
            this.btnSave.ButtonText = "Save";
            this.btnSave.DisabledColor = System.Drawing.Color.Gray;
            this.btnSave.Iconcolor = System.Drawing.Color.Transparent;
            this.btnSave.Iconimage = null;
            this.btnSave.Iconimage_right = null;
            this.btnSave.Iconimage_right_Selected = null;
            this.btnSave.Iconimage_Selected = null;
            this.btnSave.IconMarginLeft = 0;
            this.btnSave.IconMarginRight = 0;
            this.btnSave.IconRightVisible = true;
            this.btnSave.IconRightZoom = 0D;
            this.btnSave.IconVisible = true;
            this.btnSave.IconZoom = 90D;
            this.btnSave.IsTab = false;
            this.btnSave.Location = new System.Drawing.Point(146, 81);
            this.btnSave.Name = "btnSave";
            this.btnSave.Normalcolor = System.Drawing.Color.Red;
            this.btnSave.OnHovercolor = System.Drawing.Color.Pink;
            this.btnSave.OnHoverTextColor = System.Drawing.Color.Red;
            this.btnSave.selected = false;
            this.btnSave.Size = new System.Drawing.Size(133, 25);
            this.btnSave.TabIndex = 71;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSave.Textcolor = System.Drawing.Color.White;
            this.btnSave.TextFont = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft YaHei", 10.2F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Red;
            this.label11.Location = new System.Drawing.Point(232, 14);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(74, 19);
            this.label11.TabIndex = 70;
            this.label11.Text = "Ng Mode";
            // 
            // frmAddNewNgMode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Pink;
            this.ClientSize = new System.Drawing.Size(555, 120);
            this.Controls.Add(this.txtSerial);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label11);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAddNewNgMode";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "frmAddNewNgMode";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtSerial;
        private Bunifu.Framework.UI.BunifuFlatButton btnCancel;
        private Bunifu.Framework.UI.BunifuFlatButton btnSave;
        private System.Windows.Forms.Label label11;
    }
}