namespace RawMat.Views.AppearCheck
{
    partial class frmAlert
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAlert));
            this.label1 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.pcMovePrones = new System.Windows.Forms.PictureBox();
            this.pcPrintLabel = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pcMovePrones)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcPrintLabel)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(32)))), ((int)(((byte)(96)))));
            this.label1.Location = new System.Drawing.Point(107, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(419, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "อย่าลืม Print Label และ Move Prones";
            // 
            // btnOk
            // 
            this.btnOk.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOk.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(32)))), ((int)(((byte)(96)))));
            this.btnOk.Location = new System.Drawing.Point(260, 315);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(95, 38);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // pcMovePrones
            // 
            this.pcMovePrones.Image = ((System.Drawing.Image)(resources.GetObject("pcMovePrones.Image")));
            this.pcMovePrones.Location = new System.Drawing.Point(268, 59);
            this.pcMovePrones.Name = "pcMovePrones";
            this.pcMovePrones.Size = new System.Drawing.Size(367, 250);
            this.pcMovePrones.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pcMovePrones.TabIndex = 2;
            this.pcMovePrones.TabStop = false;
            // 
            // pcPrintLabel
            // 
            this.pcPrintLabel.Image = ((System.Drawing.Image)(resources.GetObject("pcPrintLabel.Image")));
            this.pcPrintLabel.Location = new System.Drawing.Point(12, 59);
            this.pcPrintLabel.Name = "pcPrintLabel";
            this.pcPrintLabel.Size = new System.Drawing.Size(250, 250);
            this.pcPrintLabel.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pcPrintLabel.TabIndex = 3;
            this.pcPrintLabel.TabStop = false;
            // 
            // frmAlert
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Pink;
            this.ClientSize = new System.Drawing.Size(649, 359);
            this.Controls.Add(this.pcPrintLabel);
            this.Controls.Add(this.pcMovePrones);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAlert";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "frmAlert";
            ((System.ComponentModel.ISupportInitialize)(this.pcMovePrones)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pcPrintLabel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.PictureBox pcMovePrones;
        private System.Windows.Forms.PictureBox pcPrintLabel;
    }
}