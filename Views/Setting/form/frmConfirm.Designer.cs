namespace RawMat.Views.Setting
{
    partial class frmConfirm
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
            this.btnSave = new Bunifu.Framework.UI.BunifuFlatButton();
            this.btnCancle = new Bunifu.Framework.UI.BunifuFlatButton();
            this.lblMessage = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSave
            // 
            this.btnSave.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnSave.BackColor = System.Drawing.Color.Green;
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
            this.btnSave.Location = new System.Drawing.Point(23, 194);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Normalcolor = System.Drawing.Color.Green;
            this.btnSave.OnHovercolor = System.Drawing.Color.Pink;
            this.btnSave.OnHoverTextColor = System.Drawing.Color.Red;
            this.btnSave.selected = false;
            this.btnSave.Size = new System.Drawing.Size(244, 44);
            this.btnSave.TabIndex = 3;
            this.btnSave.Text = "Save";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSave.Textcolor = System.Drawing.Color.White;
            this.btnSave.TextFont = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click_1);
            // 
            // btnCancle
            // 
            this.btnCancle.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnCancle.BackColor = System.Drawing.Color.DarkGray;
            this.btnCancle.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnCancle.BorderRadius = 0;
            this.btnCancle.ButtonText = "Cancel";
            this.btnCancle.DisabledColor = System.Drawing.Color.Gray;
            this.btnCancle.Iconcolor = System.Drawing.Color.Transparent;
            this.btnCancle.Iconimage = null;
            this.btnCancle.Iconimage_right = null;
            this.btnCancle.Iconimage_right_Selected = null;
            this.btnCancle.Iconimage_Selected = null;
            this.btnCancle.IconMarginLeft = 0;
            this.btnCancle.IconMarginRight = 0;
            this.btnCancle.IconRightVisible = true;
            this.btnCancle.IconRightZoom = 0D;
            this.btnCancle.IconVisible = true;
            this.btnCancle.IconZoom = 90D;
            this.btnCancle.IsTab = false;
            this.btnCancle.Location = new System.Drawing.Point(292, 194);
            this.btnCancle.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnCancle.Name = "btnCancle";
            this.btnCancle.Normalcolor = System.Drawing.Color.DarkGray;
            this.btnCancle.OnHovercolor = System.Drawing.Color.Pink;
            this.btnCancle.OnHoverTextColor = System.Drawing.Color.Red;
            this.btnCancle.selected = false;
            this.btnCancle.Size = new System.Drawing.Size(156, 44);
            this.btnCancle.TabIndex = 4;
            this.btnCancle.Text = "Cancel";
            this.btnCancle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnCancle.Textcolor = System.Drawing.Color.White;
            this.btnCancle.TextFont = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancle.Click += new System.EventHandler(this.btnCancle_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMessage.Location = new System.Drawing.Point(41, 137);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(69, 27);
            this.lblMessage.TabIndex = 5;
            this.lblMessage.Text = "label1";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.lblMessage);
            this.panel1.Controls.Add(this.btnCancle);
            this.panel1.Controls.Add(this.btnSave);
            this.panel1.Location = new System.Drawing.Point(2, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(470, 257);
            this.panel1.TabIndex = 6;
            // 
            // frmConfirm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Red;
            this.ClientSize = new System.Drawing.Size(476, 264);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmConfirm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "frmConfirm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuFlatButton btnSave;
        private Bunifu.Framework.UI.BunifuFlatButton btnCancle;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Panel panel1;
    }
}