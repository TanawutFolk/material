namespace RawMat.ViewsMaterial
{
    partial class userControlRecMat
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
            this.bunifuSeparator1 = new Bunifu.Framework.UI.BunifuSeparator();
            this.panelTop = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.bt_findReport = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bt_rec_mat = new Bunifu.Framework.UI.BunifuFlatButton();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.SuspendLayout();
            // 
            // bunifuSeparator1
            // 
            this.bunifuSeparator1.BackColor = System.Drawing.Color.Transparent;
            this.bunifuSeparator1.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(105)))), ((int)(((byte)(105)))));
            this.bunifuSeparator1.LineThickness = 1;
            this.bunifuSeparator1.Location = new System.Drawing.Point(0, 498);
            this.bunifuSeparator1.Name = "bunifuSeparator1";
            this.bunifuSeparator1.Size = new System.Drawing.Size(1109, 18);
            this.bunifuSeparator1.TabIndex = 13;
            this.bunifuSeparator1.Transparency = 255;
            this.bunifuSeparator1.Vertical = false;
            // 
            // panelTop
            // 
            this.panelTop.BackColor = System.Drawing.Color.LightPink;
            this.panelTop.Location = new System.Drawing.Point(0, 81);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1115, 600);
            this.panelTop.TabIndex = 17;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox1.Image = global::RawMat.Properties.Resources.select1;
            this.pictureBox1.Location = new System.Drawing.Point(137, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(93, 75);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox1.TabIndex = 21;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Visible = false;
            // 
            // bt_findReport
            // 
            this.bt_findReport.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.bt_findReport.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.bt_findReport.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_findReport.BorderRadius = 0;
            this.bt_findReport.ButtonText = "Find Report";
            this.bt_findReport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_findReport.DisabledColor = System.Drawing.Color.Gray;
            this.bt_findReport.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.bt_findReport.Iconcolor = System.Drawing.Color.Transparent;
            this.bt_findReport.Iconimage = global::RawMat.Properties.Resources.find_report;
            this.bt_findReport.Iconimage_right = null;
            this.bt_findReport.Iconimage_right_Selected = null;
            this.bt_findReport.Iconimage_Selected = null;
            this.bt_findReport.IconMarginLeft = 0;
            this.bt_findReport.IconMarginRight = 0;
            this.bt_findReport.IconRightVisible = true;
            this.bt_findReport.IconRightZoom = 0D;
            this.bt_findReport.IconVisible = true;
            this.bt_findReport.IconZoom = 90D;
            this.bt_findReport.IsTab = false;
            this.bt_findReport.Location = new System.Drawing.Point(635, 3);
            this.bt_findReport.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_findReport.Name = "bt_findReport";
            this.bt_findReport.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.bt_findReport.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.bt_findReport.OnHoverTextColor = System.Drawing.Color.Salmon;
            this.bt_findReport.selected = false;
            this.bt_findReport.Size = new System.Drawing.Size(170, 72);
            this.bt_findReport.TabIndex = 20;
            this.bt_findReport.Text = "Find Report";
            this.bt_findReport.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_findReport.Textcolor = System.Drawing.Color.Lavender;
            this.bt_findReport.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_findReport.Click += new System.EventHandler(this.bt_findReport_Click);
            // 
            // bt_rec_mat
            // 
            this.bt_rec_mat.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.bt_rec_mat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.bt_rec_mat.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_rec_mat.BorderRadius = 0;
            this.bt_rec_mat.ButtonText = "Receive Mat.";
            this.bt_rec_mat.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_rec_mat.DisabledColor = System.Drawing.Color.Gray;
            this.bt_rec_mat.Font = new System.Drawing.Font("Century Gothic", 9.75F);
            this.bt_rec_mat.Iconcolor = System.Drawing.Color.Transparent;
            this.bt_rec_mat.Iconimage = global::RawMat.Properties.Resources.receive_date;
            this.bt_rec_mat.Iconimage_right = null;
            this.bt_rec_mat.Iconimage_right_Selected = null;
            this.bt_rec_mat.Iconimage_Selected = null;
            this.bt_rec_mat.IconMarginLeft = 0;
            this.bt_rec_mat.IconMarginRight = 0;
            this.bt_rec_mat.IconRightVisible = true;
            this.bt_rec_mat.IconRightZoom = 0D;
            this.bt_rec_mat.IconVisible = true;
            this.bt_rec_mat.IconZoom = 90D;
            this.bt_rec_mat.IsTab = false;
            this.bt_rec_mat.Location = new System.Drawing.Point(237, 3);
            this.bt_rec_mat.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_rec_mat.Name = "bt_rec_mat";
            this.bt_rec_mat.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.bt_rec_mat.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.bt_rec_mat.OnHoverTextColor = System.Drawing.Color.LightSalmon;
            this.bt_rec_mat.selected = false;
            this.bt_rec_mat.Size = new System.Drawing.Size(167, 72);
            this.bt_rec_mat.TabIndex = 19;
            this.bt_rec_mat.Text = "Receive Mat.";
            this.bt_rec_mat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_rec_mat.Textcolor = System.Drawing.Color.Lavender;
            this.bt_rec_mat.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // pictureBox2
            // 
            this.pictureBox2.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox2.Image = global::RawMat.Properties.Resources.select1;
            this.pictureBox2.Location = new System.Drawing.Point(535, 3);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(93, 75);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pictureBox2.TabIndex = 22;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.Visible = false;
            // 
            // userControlRecMat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightPink;
            this.Controls.Add(this.pictureBox2);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.bt_findReport);
            this.Controls.Add(this.bt_rec_mat);
            this.Controls.Add(this.panelTop);
            this.Controls.Add(this.bunifuSeparator1);
            this.Name = "userControlRecMat";
            this.Size = new System.Drawing.Size(1115, 730);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private Bunifu.Framework.UI.BunifuSeparator bunifuSeparator1;
        private System.Windows.Forms.Panel panelTop;
        private Bunifu.Framework.UI.BunifuFlatButton bt_rec_mat;
        private Bunifu.Framework.UI.BunifuFlatButton bt_findReport;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.PictureBox pictureBox2;
    }
}
