namespace RawMat.ViewsMaterialNCR.Controls
{
    partial class ucPager
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
            this.lb_Summary = new System.Windows.Forms.Label();
            this.flp_Pager = new System.Windows.Forms.FlowLayoutPanel();
            this.pn_PageSize = new System.Windows.Forms.Panel();
            this.cmb_PageSize = new System.Windows.Forms.ComboBox();
            this.lb_PerPage = new System.Windows.Forms.Label();
            this.pn_PageSize.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_Summary
            // 
            this.lb_Summary.AutoSize = true;
            this.lb_Summary.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lb_Summary.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(122)))), ((int)(((byte)(140)))));
            this.lb_Summary.Location = new System.Drawing.Point(16, 21);
            this.lb_Summary.Name = "lb_Summary";
            this.lb_Summary.Size = new System.Drawing.Size(0, 16);
            this.lb_Summary.TabIndex = 0;
            // 
            // flp_Pager
            // 
            this.flp_Pager.AutoSize = true;
            this.flp_Pager.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flp_Pager.Location = new System.Drawing.Point(560, 13);
            this.flp_Pager.Name = "flp_Pager";
            this.flp_Pager.Size = new System.Drawing.Size(0, 34);
            this.flp_Pager.TabIndex = 1;
            this.flp_Pager.WrapContents = false;
            // 
            // pn_PageSize
            // 
            this.pn_PageSize.Controls.Add(this.cmb_PageSize);
            this.pn_PageSize.Controls.Add(this.lb_PerPage);
            this.pn_PageSize.Location = new System.Drawing.Point(897, 13);
            this.pn_PageSize.Name = "pn_PageSize";
            this.pn_PageSize.Size = new System.Drawing.Size(130, 34);
            this.pn_PageSize.TabIndex = 2;
            // 
            // cmb_PageSize
            // 
            this.cmb_PageSize.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmb_PageSize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmb_PageSize.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.cmb_PageSize.FormattingEnabled = true;
            this.cmb_PageSize.Location = new System.Drawing.Point(0, 5);
            this.cmb_PageSize.Name = "cmb_PageSize";
            this.cmb_PageSize.Size = new System.Drawing.Size(62, 24);
            this.cmb_PageSize.TabIndex = 0;
            // 
            // lb_PerPage
            // 
            this.lb_PerPage.AutoSize = true;
            this.lb_PerPage.Font = new System.Drawing.Font("Microsoft YaHei UI", 9F);
            this.lb_PerPage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(107)))), ((int)(((byte)(122)))), ((int)(((byte)(140)))));
            this.lb_PerPage.Location = new System.Drawing.Point(70, 9);
            this.lb_PerPage.Name = "lb_PerPage";
            this.lb_PerPage.Size = new System.Drawing.Size(56, 16);
            this.lb_PerPage.TabIndex = 1;
            this.lb_PerPage.Text = "per page";
            // 
            // ucPager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.lb_Summary);
            this.Controls.Add(this.flp_Pager);
            this.Controls.Add(this.pn_PageSize);
            this.Name = "ucPager";
            this.Size = new System.Drawing.Size(1043, 60);
            this.pn_PageSize.ResumeLayout(false);
            this.pn_PageSize.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_Summary;
        private System.Windows.Forms.FlowLayoutPanel flp_Pager;
        private System.Windows.Forms.Panel pn_PageSize;
        private System.Windows.Forms.ComboBox cmb_PageSize;
        private System.Windows.Forms.Label lb_PerPage;
    }
}
