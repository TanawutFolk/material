namespace RawMat.Views.RegularCheck
{
    partial class userControlRegularPending
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lb_top = new System.Windows.Forms.Label();
            this.lb_recDate = new System.Windows.Forms.Label();
            this.lb_invoice = new System.Windows.Forms.Label();
            this.lb_mcode = new System.Windows.Forms.Label();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.lb_regularNo = new System.Windows.Forms.Label();
            this.gb_cavity = new System.Windows.Forms.GroupBox();
            this.lb_sampName = new System.Windows.Forms.Label();
            this.bt_next = new System.Windows.Forms.Button();
            this.bt_prev = new System.Windows.Forms.Button();
            this.lb_page = new System.Windows.Forms.Label();
            this.dtg_regular = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.tb_record = new Bunifu.Framework.UI.BunifuFlatButton();
            this.picbox_reg = new System.Windows.Forms.PictureBox();
            this.picbox_cavity = new System.Windows.Forms.PictureBox();
            this.gb_cavity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_regular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_reg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_top
            // 
            this.lb_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_top.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_top.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_top.Location = new System.Drawing.Point(0, 0);
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(1115, 38);
            this.lb_top.TabIndex = 32;
            this.lb_top.Text = "Regular Check Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(59, 82);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(267, 19);
            this.lb_recDate.TabIndex = 51;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(787, 47);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(216, 19);
            this.lb_invoice.TabIndex = 50;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(332, 82);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(231, 19);
            this.lb_mcode.TabIndex = 49;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(333, 47);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(216, 19);
            this.lb_reportNo.TabIndex = 48;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // lb_regularNo
            // 
            this.lb_regularNo.AutoSize = true;
            this.lb_regularNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_regularNo.ForeColor = System.Drawing.Color.Black;
            this.lb_regularNo.Location = new System.Drawing.Point(59, 47);
            this.lb_regularNo.Name = "lb_regularNo";
            this.lb_regularNo.Size = new System.Drawing.Size(166, 19);
            this.lb_regularNo.TabIndex = 47;
            this.lb_regularNo.Text = "Report Regular No.";
            // 
            // gb_cavity
            // 
            this.gb_cavity.Controls.Add(this.picbox_cavity);
            this.gb_cavity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_cavity.Location = new System.Drawing.Point(63, 120);
            this.gb_cavity.Name = "gb_cavity";
            this.gb_cavity.Size = new System.Drawing.Size(1003, 211);
            this.gb_cavity.TabIndex = 52;
            this.gb_cavity.TabStop = false;
            this.gb_cavity.Text = "Cavity";
            // 
            // lb_sampName
            // 
            this.lb_sampName.AutoSize = true;
            this.lb_sampName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_sampName.ForeColor = System.Drawing.Color.Black;
            this.lb_sampName.Location = new System.Drawing.Point(65, 334);
            this.lb_sampName.Name = "lb_sampName";
            this.lb_sampName.Size = new System.Drawing.Size(160, 23);
            this.lb_sampName.TabIndex = 53;
            this.lb_sampName.Text = "Sampling Name";
            // 
            // bt_next
            // 
            this.bt_next.Location = new System.Drawing.Point(155, 559);
            this.bt_next.Name = "bt_next";
            this.bt_next.Size = new System.Drawing.Size(70, 33);
            this.bt_next.TabIndex = 57;
            this.bt_next.Text = "Next";
            this.bt_next.UseVisualStyleBackColor = true;
            this.bt_next.Click += new System.EventHandler(this.bt_next_Click);
            // 
            // bt_prev
            // 
            this.bt_prev.Location = new System.Drawing.Point(66, 559);
            this.bt_prev.Name = "bt_prev";
            this.bt_prev.Size = new System.Drawing.Size(70, 33);
            this.bt_prev.TabIndex = 56;
            this.bt_prev.Text = "Prev.";
            this.bt_prev.UseVisualStyleBackColor = true;
            this.bt_prev.Click += new System.EventHandler(this.bt_prev_Click);
            // 
            // lb_page
            // 
            this.lb_page.AutoSize = true;
            this.lb_page.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_page.ForeColor = System.Drawing.Color.Black;
            this.lb_page.Location = new System.Drawing.Point(136, 519);
            this.lb_page.Name = "lb_page";
            this.lb_page.Size = new System.Drawing.Size(21, 23);
            this.lb_page.TabIndex = 55;
            this.lb_page.Text = "/";
            // 
            // dtg_regular
            // 
            this.dtg_regular.AllowUserToAddRows = false;
            this.dtg_regular.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_regular.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dtg_regular.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtg_regular.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtg_regular.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtg_regular.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_regular.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_regular.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.SeaGreen;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_regular.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtg_regular.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dtg_regular.DefaultCellStyle = dataGridViewCellStyle3;
            this.dtg_regular.DoubleBuffered = true;
            this.dtg_regular.EnableHeadersVisualStyles = false;
            this.dtg_regular.HeaderBgColor = System.Drawing.Color.SeaGreen;
            this.dtg_regular.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_regular.Location = new System.Drawing.Point(73, 598);
            this.dtg_regular.Name = "dtg_regular";
            this.dtg_regular.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_regular.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dtg_regular.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dtg_regular.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtg_regular.RowTemplate.Height = 41;
            this.dtg_regular.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dtg_regular.Size = new System.Drawing.Size(993, 308);
            this.dtg_regular.TabIndex = 58;
            // 
            // tb_record
            // 
            this.tb_record.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.tb_record.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.tb_record.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.tb_record.BorderRadius = 0;
            this.tb_record.ButtonText = "Record Data";
            this.tb_record.Cursor = System.Windows.Forms.Cursors.Hand;
            this.tb_record.DisabledColor = System.Drawing.Color.Gray;
            this.tb_record.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_record.Iconcolor = System.Drawing.Color.Transparent;
            this.tb_record.Iconimage = global::RawMat.Properties.Resources.bond;
            this.tb_record.Iconimage_right = null;
            this.tb_record.Iconimage_right_Selected = null;
            this.tb_record.Iconimage_Selected = null;
            this.tb_record.IconMarginLeft = 0;
            this.tb_record.IconMarginRight = 0;
            this.tb_record.IconRightVisible = true;
            this.tb_record.IconRightZoom = 0D;
            this.tb_record.IconVisible = true;
            this.tb_record.IconZoom = 90D;
            this.tb_record.IsTab = false;
            this.tb_record.Location = new System.Drawing.Point(463, 913);
            this.tb_record.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tb_record.Name = "tb_record";
            this.tb_record.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.tb_record.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.tb_record.OnHoverTextColor = System.Drawing.Color.White;
            this.tb_record.selected = false;
            this.tb_record.Size = new System.Drawing.Size(187, 58);
            this.tb_record.TabIndex = 59;
            this.tb_record.Text = "Record Data";
            this.tb_record.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tb_record.Textcolor = System.Drawing.Color.Lavender;
            this.tb_record.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_record.Click += new System.EventHandler(this.tb_record_Click);
            // 
            // picbox_reg
            // 
            this.picbox_reg.Image = global::RawMat.Properties.Resources.SHIN005;
            this.picbox_reg.Location = new System.Drawing.Point(231, 337);
            this.picbox_reg.Name = "picbox_reg";
            this.picbox_reg.Size = new System.Drawing.Size(815, 255);
            this.picbox_reg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_reg.TabIndex = 54;
            this.picbox_reg.TabStop = false;
            // 
            // picbox_cavity
            // 
            this.picbox_cavity.Image = global::RawMat.Properties.Resources.Cavity;
            this.picbox_cavity.Location = new System.Drawing.Point(16, 29);
            this.picbox_cavity.Name = "picbox_cavity";
            this.picbox_cavity.Size = new System.Drawing.Size(981, 176);
            this.picbox_cavity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_cavity.TabIndex = 0;
            this.picbox_cavity.TabStop = false;
            // 
            // userControlRegularPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.Controls.Add(this.tb_record);
            this.Controls.Add(this.dtg_regular);
            this.Controls.Add(this.bt_next);
            this.Controls.Add(this.bt_prev);
            this.Controls.Add(this.lb_page);
            this.Controls.Add(this.picbox_reg);
            this.Controls.Add(this.lb_sampName);
            this.Controls.Add(this.gb_cavity);
            this.Controls.Add(this.lb_recDate);
            this.Controls.Add(this.lb_invoice);
            this.Controls.Add(this.lb_mcode);
            this.Controls.Add(this.lb_reportNo);
            this.Controls.Add(this.lb_regularNo);
            this.Controls.Add(this.lb_top);
            this.Name = "userControlRegularPending";
            this.Size = new System.Drawing.Size(1115, 975);
            this.Load += new System.EventHandler(this.userControlRegularPending_Load);
            this.gb_cavity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_regular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_reg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
        private System.Windows.Forms.Label lb_recDate;
        private System.Windows.Forms.Label lb_invoice;
        private System.Windows.Forms.Label lb_mcode;
        private System.Windows.Forms.Label lb_reportNo;
        private System.Windows.Forms.Label lb_regularNo;
        private System.Windows.Forms.GroupBox gb_cavity;
        private System.Windows.Forms.PictureBox picbox_cavity;
        private System.Windows.Forms.Label lb_sampName;
        private System.Windows.Forms.PictureBox picbox_reg;
        private System.Windows.Forms.Button bt_next;
        private System.Windows.Forms.Button bt_prev;
        private System.Windows.Forms.Label lb_page;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_regular;
        private Bunifu.Framework.UI.BunifuFlatButton tb_record;
    }
}
