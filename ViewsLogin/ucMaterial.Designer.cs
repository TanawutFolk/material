namespace RawMat.ViewsLogin
{
    partial class ucMaterial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ucMaterial));
            this.btnMat = new Bunifu.Framework.UI.BunifuTileButton();
            this.SuspendLayout();
            // 
            // btnMat
            // 
            this.btnMat.BackColor = System.Drawing.Color.White;
            this.btnMat.color = System.Drawing.Color.White;
            this.btnMat.colorActive = System.Drawing.Color.LightGreen;
            this.btnMat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnMat.Font = new System.Drawing.Font("Century Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMat.ForeColor = System.Drawing.Color.SeaGreen;
            this.btnMat.Image = ((System.Drawing.Image)(resources.GetObject("btnMat.Image")));
            this.btnMat.ImagePosition = 21;
            this.btnMat.ImageZoom = 50;
            this.btnMat.LabelPosition = 43;
            this.btnMat.LabelText = "Material Inspection Receiving";
            this.btnMat.Location = new System.Drawing.Point(0, 0);
            this.btnMat.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnMat.Name = "btnMat";
            this.btnMat.Size = new System.Drawing.Size(415, 476);
            this.btnMat.TabIndex = 0;
            // 
            // ucMaterial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnMat);
            this.Name = "ucMaterial";
            this.Size = new System.Drawing.Size(415, 476);
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuTileButton btnMat;
    }
}
