namespace RawMat.Views.AppearCheck
{
    partial class userControlAppear
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lb_top = new System.Windows.Forms.Label();
            this.bt_back = new System.Windows.Forms.Button();
            this.lb_lotSize = new System.Windows.Forms.Label();
            this.lb_recDate = new System.Windows.Forms.Label();
            this.lb_invoice = new System.Windows.Forms.Label();
            this.lb_mcode = new System.Windows.Forms.Label();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.lb_inspQty = new System.Windows.Forms.Label();
            this.gb_cavity = new System.Windows.Forms.GroupBox();
            this.picbox_cavity = new System.Windows.Forms.PictureBox();
            this.picbox_Appear = new System.Windows.Forms.PictureBox();
            this.dtg_show_appear = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.gb_ngMode = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dtg_ngMode = new System.Windows.Forms.DataGridView();
            this.QTY_NG = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NG_MODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gb_pack = new System.Windows.Forms.GroupBox();
            this.bt_Clear = new System.Windows.Forms.Button();
            this.bt_select_packing_size_appear = new System.Windows.Forms.Button();
            this.dtg_packing_size_appear = new System.Windows.Forms.DataGridView();
            this.gb_input = new System.Windows.Forms.GroupBox();
            this.tb_record = new Bunifu.Framework.UI.BunifuFlatButton();
            this.gb_cavity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_Appear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_show_appear)).BeginInit();
            this.gb_ngMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_ngMode)).BeginInit();
            this.gb_pack.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_packing_size_appear)).BeginInit();
            this.gb_input.SuspendLayout();
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
            this.lb_top.Text = "Appearance Check";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // bt_back
            // 
            this.bt_back.Location = new System.Drawing.Point(6, 3);
            this.bt_back.Name = "bt_back";
            this.bt_back.Size = new System.Drawing.Size(169, 34);
            this.bt_back.TabIndex = 55;
            this.bt_back.Text = "Back";
            this.bt_back.UseVisualStyleBackColor = true;
            this.bt_back.Click += new System.EventHandler(this.bt_back_Click);
            // 
            // lb_lotSize
            // 
            this.lb_lotSize.AutoSize = true;
            this.lb_lotSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotSize.Location = new System.Drawing.Point(705, 47);
            this.lb_lotSize.Name = "lb_lotSize";
            this.lb_lotSize.Size = new System.Drawing.Size(146, 19);
            this.lb_lotSize.TabIndex = 77;
            this.lb_lotSize.Text = "LOT SIZE/ทั้งหมด";
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(68, 82);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(267, 19);
            this.lb_recDate.TabIndex = 76;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(341, 47);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(216, 19);
            this.lb_invoice.TabIndex = 75;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(341, 82);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(231, 19);
            this.lb_mcode.TabIndex = 74;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(68, 47);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(216, 19);
            this.lb_reportNo.TabIndex = 73;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // lb_inspQty
            // 
            this.lb_inspQty.AutoSize = true;
            this.lb_inspQty.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_inspQty.Location = new System.Drawing.Point(705, 82);
            this.lb_inspQty.Name = "lb_inspQty";
            this.lb_inspQty.Size = new System.Drawing.Size(381, 19);
            this.lb_inspQty.TabIndex = 78;
            this.lb_inspQty.Text = "INSPECTION QTY/คำนวนจาก db_packing_size";
            // 
            // gb_cavity
            // 
            this.gb_cavity.Controls.Add(this.picbox_cavity);
            this.gb_cavity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_cavity.Location = new System.Drawing.Point(72, 115);
            this.gb_cavity.Name = "gb_cavity";
            this.gb_cavity.Size = new System.Drawing.Size(1003, 211);
            this.gb_cavity.TabIndex = 79;
            this.gb_cavity.TabStop = false;
            this.gb_cavity.Text = "Cavity";
            // 
            // picbox_cavity
            // 
            this.picbox_cavity.Image = global::RawMat.Properties.Resources.Cavity;
            this.picbox_cavity.Location = new System.Drawing.Point(16, 29);
            this.picbox_cavity.Name = "picbox_cavity";
            this.picbox_cavity.Size = new System.Drawing.Size(981, 198);
            this.picbox_cavity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_cavity.TabIndex = 0;
            this.picbox_cavity.TabStop = false;
            // 
            // picbox_Appear
            // 
            this.picbox_Appear.Image = global::RawMat.Properties.Resources.SHIN005;
            this.picbox_Appear.Location = new System.Drawing.Point(72, 348);
            this.picbox_Appear.Name = "picbox_Appear";
            this.picbox_Appear.Size = new System.Drawing.Size(997, 362);
            this.picbox_Appear.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_Appear.TabIndex = 80;
            this.picbox_Appear.TabStop = false;
            // 
            // dtg_show_appear
            // 
            this.dtg_show_appear.AllowUserToAddRows = false;
            this.dtg_show_appear.AllowUserToDeleteRows = false;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_show_appear.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dtg_show_appear.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtg_show_appear.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtg_show_appear.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_show_appear.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_show_appear.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.SeaGreen;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_show_appear.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.dtg_show_appear.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_show_appear.DoubleBuffered = true;
            this.dtg_show_appear.EnableHeadersVisualStyles = false;
            this.dtg_show_appear.HeaderBgColor = System.Drawing.Color.SeaGreen;
            this.dtg_show_appear.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_show_appear.Location = new System.Drawing.Point(6, 48);
            this.dtg_show_appear.Name = "dtg_show_appear";
            this.dtg_show_appear.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_show_appear.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dtg_show_appear.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtg_show_appear.RowTemplate.Height = 41;
            this.dtg_show_appear.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dtg_show_appear.Size = new System.Drawing.Size(684, 250);
            this.dtg_show_appear.TabIndex = 81;
            this.dtg_show_appear.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dtg_show_appear_DataBindingComplete);
            // 
            // gb_ngMode
            // 
            this.gb_ngMode.Controls.Add(this.label2);
            this.gb_ngMode.Controls.Add(this.dtg_ngMode);
            this.gb_ngMode.Enabled = false;
            this.gb_ngMode.Location = new System.Drawing.Point(721, 733);
            this.gb_ngMode.Name = "gb_ngMode";
            this.gb_ngMode.Size = new System.Drawing.Size(381, 543);
            this.gb_ngMode.TabIndex = 85;
            this.gb_ngMode.TabStop = false;
            this.gb_ngMode.Text = "NG MODE";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(6, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(176, 20);
            this.label2.TabIndex = 87;
            this.label2.Text = "ระบุอาการเสียแล้ว: 0 / 0 ชิ้น";
            // 
            // dtg_ngMode
            // 
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_ngMode.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dtg_ngMode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_ngMode.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.QTY_NG,
            this.NG_MODE});
            this.dtg_ngMode.Location = new System.Drawing.Point(10, 39);
            this.dtg_ngMode.Name = "dtg_ngMode";
            this.dtg_ngMode.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dtg_ngMode.Size = new System.Drawing.Size(365, 498);
            this.dtg_ngMode.TabIndex = 85;
            // 
            // QTY_NG
            // 
            this.QTY_NG.HeaderText = "QTY NG";
            this.QTY_NG.Name = "QTY_NG";
            // 
            // NG_MODE
            // 
            this.NG_MODE.HeaderText = "NG MODE";
            this.NG_MODE.Name = "NG_MODE";
            this.NG_MODE.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // gb_pack
            // 
            this.gb_pack.Controls.Add(this.bt_Clear);
            this.gb_pack.Controls.Add(this.bt_select_packing_size_appear);
            this.gb_pack.Controls.Add(this.dtg_packing_size_appear);
            this.gb_pack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_pack.Location = new System.Drawing.Point(12, 731);
            this.gb_pack.Name = "gb_pack";
            this.gb_pack.Size = new System.Drawing.Size(703, 309);
            this.gb_pack.TabIndex = 86;
            this.gb_pack.TabStop = false;
            this.gb_pack.Text = "Appearance Inspection Q\'ty";
            // 
            // bt_Clear
            // 
            this.bt_Clear.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_Clear.Location = new System.Drawing.Point(362, 254);
            this.bt_Clear.Name = "bt_Clear";
            this.bt_Clear.Size = new System.Drawing.Size(126, 49);
            this.bt_Clear.TabIndex = 89;
            this.bt_Clear.Text = "เลือกชุดอื่น";
            this.bt_Clear.UseVisualStyleBackColor = true;
            this.bt_Clear.Click += new System.EventHandler(this.bt_Clear_Click);
            // 
            // bt_select_packing_size_appear
            // 
            this.bt_select_packing_size_appear.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_select_packing_size_appear.Location = new System.Drawing.Point(230, 254);
            this.bt_select_packing_size_appear.Name = "bt_select_packing_size_appear";
            this.bt_select_packing_size_appear.Size = new System.Drawing.Size(126, 49);
            this.bt_select_packing_size_appear.TabIndex = 1;
            this.bt_select_packing_size_appear.Text = "ตรวจแพ็คนี้";
            this.bt_select_packing_size_appear.UseVisualStyleBackColor = true;
            this.bt_select_packing_size_appear.Click += new System.EventHandler(this.bt_Select_Click);
            // 
            // dtg_packing_size_appear
            // 
            this.dtg_packing_size_appear.AllowUserToAddRows = false;
            this.dtg_packing_size_appear.AllowUserToDeleteRows = false;
            this.dtg_packing_size_appear.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtg_packing_size_appear.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtg_packing_size_appear.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dtg_packing_size_appear.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_packing_size_appear.Location = new System.Drawing.Point(9, 41);
            this.dtg_packing_size_appear.Name = "dtg_packing_size_appear";
            this.dtg_packing_size_appear.ReadOnly = true;
            this.dtg_packing_size_appear.Size = new System.Drawing.Size(688, 207);
            this.dtg_packing_size_appear.TabIndex = 0;
            this.dtg_packing_size_appear.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dtg_packing_size_appear_CellFormatting);
            this.dtg_packing_size_appear.SelectionChanged += new System.EventHandler(this.dtg_packing_size_appear_SelectionChanged);
            this.dtg_packing_size_appear.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dtg_packing_size_appear_MouseDown);
            // 
            // gb_input
            // 
            this.gb_input.Controls.Add(this.tb_record);
            this.gb_input.Controls.Add(this.dtg_show_appear);
            this.gb_input.Enabled = false;
            this.gb_input.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_input.Location = new System.Drawing.Point(6, 1046);
            this.gb_input.Name = "gb_input";
            this.gb_input.Size = new System.Drawing.Size(709, 375);
            this.gb_input.TabIndex = 87;
            this.gb_input.TabStop = false;
            this.gb_input.Text = "Result Appearance Inspection";
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
            this.tb_record.Enabled = false;
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
            this.tb_record.Location = new System.Drawing.Point(246, 306);
            this.tb_record.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tb_record.Name = "tb_record";
            this.tb_record.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.tb_record.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.tb_record.OnHoverTextColor = System.Drawing.Color.White;
            this.tb_record.selected = false;
            this.tb_record.Size = new System.Drawing.Size(172, 58);
            this.tb_record.TabIndex = 82;
            this.tb_record.Text = "Record Data";
            this.tb_record.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tb_record.Textcolor = System.Drawing.Color.Lavender;
            this.tb_record.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_record.Click += new System.EventHandler(this.tb_record_Click);
            // 
            // userControlAppear
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.gb_pack);
            this.Controls.Add(this.gb_ngMode);
            this.Controls.Add(this.picbox_Appear);
            this.Controls.Add(this.gb_cavity);
            this.Controls.Add(this.lb_inspQty);
            this.Controls.Add(this.lb_lotSize);
            this.Controls.Add(this.lb_recDate);
            this.Controls.Add(this.lb_invoice);
            this.Controls.Add(this.lb_mcode);
            this.Controls.Add(this.lb_reportNo);
            this.Controls.Add(this.bt_back);
            this.Controls.Add(this.lb_top);
            this.Controls.Add(this.gb_input);
            this.Name = "userControlAppear";
            this.Size = new System.Drawing.Size(1115, 1430);
            this.Load += new System.EventHandler(this.userControlAppear_Load);
            this.VisibleChanged += new System.EventHandler(this.userControlAppear_VisibleChanged);
            this.Leave += new System.EventHandler(this.userControlAppear_Leave);
            this.gb_cavity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_Appear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_show_appear)).EndInit();
            this.gb_ngMode.ResumeLayout(false);
            this.gb_ngMode.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_ngMode)).EndInit();
            this.gb_pack.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_packing_size_appear)).EndInit();
            this.gb_input.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
        private System.Windows.Forms.Button bt_back;
        private System.Windows.Forms.Label lb_lotSize;
        private System.Windows.Forms.Label lb_recDate;
        private System.Windows.Forms.Label lb_invoice;
        private System.Windows.Forms.Label lb_mcode;
        private System.Windows.Forms.Label lb_reportNo;
        private System.Windows.Forms.Label lb_inspQty;
        private System.Windows.Forms.GroupBox gb_cavity;
        private System.Windows.Forms.PictureBox picbox_cavity;
        private System.Windows.Forms.PictureBox picbox_Appear;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_show_appear;
        private System.Windows.Forms.GroupBox gb_ngMode;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dtg_ngMode;
        private System.Windows.Forms.GroupBox gb_pack;
        private System.Windows.Forms.DataGridView dtg_packing_size_appear;
        private System.Windows.Forms.Button bt_select_packing_size_appear;
        private System.Windows.Forms.GroupBox gb_input;
        private Bunifu.Framework.UI.BunifuFlatButton tb_record;
        private System.Windows.Forms.Button bt_Clear;
        private System.Windows.Forms.DataGridViewTextBoxColumn QTY_NG;
        private System.Windows.Forms.DataGridViewTextBoxColumn NG_MODE;
    }
}
