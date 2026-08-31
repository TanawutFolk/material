namespace RawMat.ViewsMaterial.AppearCheck
{
    partial class frmAlertPending
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAlertPending));
            this.pcPrintLabel = new System.Windows.Forms.PictureBox();
            this.pcMovePrones = new System.Windows.Forms.PictureBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.lb_alertText = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pcPrintLabel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcMovePrones)).BeginInit();
            this.SuspendLayout();
            // 
            // pcPrintLabel
            // 
            this.pcPrintLabel.Image = ((System.Drawing.Image)(resources.GetObject("pcPrintLabel.Image")));
            this.pcPrintLabel.Location = new System.Drawing.Point(13, 63);
            this.pcPrintLabel.Name = "pcPrintLabel";
            this.pcPrintLabel.Size = new System.Drawing.Size(250, 250);
            this.pcPrintLabel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pcPrintLabel.TabIndex = 7;
            this.pcPrintLabel.TabStop = false;
            // 
            // pcMovePrones
            // 
            this.pcMovePrones.Image = ((System.Drawing.Image)(resources.GetObject("pcMovePrones.Image")));
            this.pcMovePrones.Location = new System.Drawing.Point(268, 63);
            this.pcMovePrones.Name = "pcMovePrones";
            this.pcMovePrones.Size = new System.Drawing.Size(367, 250);
            this.pcMovePrones.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pcMovePrones.TabIndex = 6;
            this.pcMovePrones.TabStop = false;
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(32)))), ((int)(((byte)(96)))));
            this.btnOk.Location = new System.Drawing.Point(289, 336);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(95, 38);
            this.btnOk.TabIndex = 5;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            // 
            // lb_alertText
            // 
            this.lb_alertText.AutoSize = true;
            this.lb_alertText.Font = new System.Drawing.Font("Century Gothic", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_alertText.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(32)))), ((int)(((byte)(96)))));
            this.lb_alertText.Location = new System.Drawing.Point(74, 15);
            this.lb_alertText.Name = "lb_alertText";
            this.lb_alertText.Size = new System.Drawing.Size(82, 38);
            this.lb_alertText.TabIndex = 4;
            this.lb_alertText.Text = "TEXT";
            // 
            // frmAlertPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(649, 388);
            this.Controls.Add(this.pcPrintLabel);
            this.Controls.Add(this.pcMovePrones);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.lb_alertText);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "frmAlertPending";
            this.ShowIcon = false;
            this.Text = "Alert";
            ((System.ComponentModel.ISupportInitialize)(this.pcPrintLabel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcMovePrones)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pcPrintLabel;
        private System.Windows.Forms.PictureBox pcMovePrones;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Label lb_alertText;
    }
}