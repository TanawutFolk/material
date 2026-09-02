namespace RawMat.ViewsLogin
{
    partial class ucNCR
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucNCR));
            this.btnNcr = new Bunifu.Framework.UI.BunifuTileButton();
            this.SuspendLayout();
            // 
            // btnNcr
            // 
            this.btnNcr.BackColor = System.Drawing.Color.White;
            this.btnNcr.color = System.Drawing.Color.White;
            this.btnNcr.colorActive = System.Drawing.Color.SkyBlue;
            this.btnNcr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnNcr.Font = new System.Drawing.Font("Century Gothic", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNcr.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(94)))), ((int)(((byte)(184)))));
            this.btnNcr.Image = ((System.Drawing.Image)(resources.GetObject("btnNcr.Image")));
            this.btnNcr.ImagePosition = 23;
            this.btnNcr.ImageZoom = 50;
            this.btnNcr.LabelPosition = 48;
            this.btnNcr.LabelText = "NCR Management";
            this.btnNcr.Location = new System.Drawing.Point(0, 0);
            this.btnNcr.Margin = new System.Windows.Forms.Padding(7, 7, 7, 7);
            this.btnNcr.Name = "btnNcr";
            this.btnNcr.Size = new System.Drawing.Size(420, 476);
            this.btnNcr.TabIndex = 1;
            // 
            // ucNCR
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnNcr);
            this.Name = "ucNCR";
            this.Size = new System.Drawing.Size(420, 476);
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuTileButton btnNcr;
    }
}
