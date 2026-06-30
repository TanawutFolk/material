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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(userControlAppear));
            this.lb_top = new System.Windows.Forms.Label();
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
            this.lb_Qty = new System.Windows.Forms.Label();
            this.dtg_ngMode = new System.Windows.Forms.DataGridView();
            this.QTY_NG = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NG_MODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gb_input = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lb_CountAll = new System.Windows.Forms.Label();
            this.tb_record = new Bunifu.Framework.UI.BunifuFlatButton();
            this.dtg_packing_size_appear = new System.Windows.Forms.DataGridView();
            this.bt_select_packing_size_appear = new System.Windows.Forms.Button();
            this.bt_Clear = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lbCount = new System.Windows.Forms.Label();
            this.gb_pack = new System.Windows.Forms.GroupBox();
            this.bt_back = new Bunifu.Framework.UI.BunifuFlatButton();
            this.gb_cavity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_Appear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_show_appear)).BeginInit();
            this.gb_ngMode.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_ngMode)).BeginInit();
            this.gb_input.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_packing_size_appear)).BeginInit();
            this.gb_pack.SuspendLayout();
            this.SuspendLayout();
            // 
            // lb_top
            // 
            this.lb_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_top.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_top.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_top.Location = new System.Drawing.Point(0, 0);
            this.lb_top.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(1487, 47);
            this.lb_top.TabIndex = 32;
            this.lb_top.Text = "Appearance Check";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_lotSize
            // 
            this.lb_lotSize.AutoSize = true;
            this.lb_lotSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotSize.Location = new System.Drawing.Point(905, 58);
            this.lb_lotSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_lotSize.Name = "lb_lotSize";
            this.lb_lotSize.Size = new System.Drawing.Size(180, 24);
            this.lb_lotSize.TabIndex = 77;
            this.lb_lotSize.Text = "LOT SIZE/ทั้งหมด";
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(91, 101);
            this.lb_recDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(330, 24);
            this.lb_recDate.TabIndex = 76;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(455, 58);
            this.lb_invoice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(272, 24);
            this.lb_invoice.TabIndex = 75;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(455, 101);
            this.lb_mcode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(282, 24);
            this.lb_mcode.TabIndex = 74;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(91, 58);
            this.lb_reportNo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(265, 24);
            this.lb_reportNo.TabIndex = 73;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // lb_inspQty
            // 
            this.lb_inspQty.AutoSize = true;
            this.lb_inspQty.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_inspQty.Location = new System.Drawing.Point(905, 101);
            this.lb_inspQty.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_inspQty.Name = "lb_inspQty";
            this.lb_inspQty.Size = new System.Drawing.Size(474, 24);
            this.lb_inspQty.TabIndex = 78;
            this.lb_inspQty.Text = "INSPECTION QTY/คำนวนจาก db_packing_size";
            // 
            // gb_cavity
            // 
            this.gb_cavity.Controls.Add(this.picbox_cavity);
            this.gb_cavity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_cavity.Location = new System.Drawing.Point(96, 142);
            this.gb_cavity.Margin = new System.Windows.Forms.Padding(4);
            this.gb_cavity.Name = "gb_cavity";
            this.gb_cavity.Padding = new System.Windows.Forms.Padding(4);
            this.gb_cavity.Size = new System.Drawing.Size(1337, 260);
            this.gb_cavity.TabIndex = 79;
            this.gb_cavity.TabStop = false;
            this.gb_cavity.Text = "Cavity";
            // 
            // picbox_cavity
            // 
            this.picbox_cavity.Image = global::RawMat.Properties.Resources.Cavity;
            this.picbox_cavity.Location = new System.Drawing.Point(21, 36);
            this.picbox_cavity.Margin = new System.Windows.Forms.Padding(4);
            this.picbox_cavity.Name = "picbox_cavity";
            this.picbox_cavity.Size = new System.Drawing.Size(1308, 244);
            this.picbox_cavity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_cavity.TabIndex = 0;
            this.picbox_cavity.TabStop = false;
            // 
            // picbox_Appear
            // 
            this.picbox_Appear.Image = global::RawMat.Properties.Resources.SHIN005;
            this.picbox_Appear.Location = new System.Drawing.Point(96, 428);
            this.picbox_Appear.Margin = new System.Windows.Forms.Padding(4);
            this.picbox_Appear.Name = "picbox_Appear";
            this.picbox_Appear.Size = new System.Drawing.Size(1329, 446);
            this.picbox_Appear.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_Appear.TabIndex = 80;
            this.picbox_Appear.TabStop = false;
            // 
            // dtg_show_appear
            // 
            this.dtg_show_appear.AllowUserToAddRows = false;
            this.dtg_show_appear.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_show_appear.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dtg_show_appear.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtg_show_appear.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtg_show_appear.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_show_appear.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_show_appear.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Blue;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_show_appear.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtg_show_appear.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_show_appear.DoubleBuffered = true;
            this.dtg_show_appear.EnableHeadersVisualStyles = false;
            this.dtg_show_appear.HeaderBgColor = System.Drawing.Color.White;
            this.dtg_show_appear.HeaderForeColor = System.Drawing.Color.Blue;
            this.dtg_show_appear.Location = new System.Drawing.Point(8, 59);
            this.dtg_show_appear.Margin = new System.Windows.Forms.Padding(4);
            this.dtg_show_appear.Name = "dtg_show_appear";
            this.dtg_show_appear.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_show_appear.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dtg_show_appear.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtg_show_appear.RowTemplate.Height = 41;
            this.dtg_show_appear.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dtg_show_appear.Size = new System.Drawing.Size(912, 486);
            this.dtg_show_appear.TabIndex = 81;
            this.dtg_show_appear.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dtg_show_appear_DataBindingComplete);
            // 
            // gb_ngMode
            // 
            this.gb_ngMode.Controls.Add(this.lb_Qty);
            this.gb_ngMode.Controls.Add(this.dtg_ngMode);
            this.gb_ngMode.Enabled = false;
            this.gb_ngMode.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_ngMode.Location = new System.Drawing.Point(961, 902);
            this.gb_ngMode.Margin = new System.Windows.Forms.Padding(4);
            this.gb_ngMode.Name = "gb_ngMode";
            this.gb_ngMode.Padding = new System.Windows.Forms.Padding(4);
            this.gb_ngMode.Size = new System.Drawing.Size(508, 668);
            this.gb_ngMode.TabIndex = 85;
            this.gb_ngMode.TabStop = false;
            this.gb_ngMode.Text = "Q\'ty Pending";
            // 
            // lb_Qty
            // 
            this.lb_Qty.AutoSize = true;
            this.lb_Qty.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_Qty.Location = new System.Drawing.Point(148, 0);
            this.lb_Qty.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_Qty.Name = "lb_Qty";
            this.lb_Qty.Size = new System.Drawing.Size(45, 25);
            this.lb_Qty.TabIndex = 87;
            this.lb_Qty.Text = "Pcs";
            // 
            // dtg_ngMode
            // 
            this.dtg_ngMode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_ngMode.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.QTY_NG,
            this.NG_MODE});
            this.dtg_ngMode.Location = new System.Drawing.Point(13, 48);
            this.dtg_ngMode.Margin = new System.Windows.Forms.Padding(4);
            this.dtg_ngMode.Name = "dtg_ngMode";
            this.dtg_ngMode.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dtg_ngMode.Size = new System.Drawing.Size(487, 613);
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
            // gb_input
            // 
            this.gb_input.Controls.Add(this.label3);
            this.gb_input.Controls.Add(this.lb_CountAll);
            this.gb_input.Controls.Add(this.tb_record);
            this.gb_input.Controls.Add(this.dtg_show_appear);
            this.gb_input.Enabled = false;
            this.gb_input.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_input.ForeColor = System.Drawing.Color.Blue;
            this.gb_input.Location = new System.Drawing.Point(8, 1287);
            this.gb_input.Margin = new System.Windows.Forms.Padding(4);
            this.gb_input.Name = "gb_input";
            this.gb_input.Padding = new System.Windows.Forms.Padding(4);
            this.gb_input.Size = new System.Drawing.Size(945, 673);
            this.gb_input.TabIndex = 87;
            this.gb_input.TabStop = false;
            this.gb_input.Text = "Result Appearance Inspection";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(320, 0);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(128, 25);
            this.label3.TabIndex = 90;
            this.label3.Text = "จำนวนตรวจสอบ";
            // 
            // lb_CountAll
            // 
            this.lb_CountAll.AutoSize = true;
            this.lb_CountAll.ForeColor = System.Drawing.Color.Blue;
            this.lb_CountAll.Location = new System.Drawing.Point(463, 0);
            this.lb_CountAll.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_CountAll.Name = "lb_CountAll";
            this.lb_CountAll.Size = new System.Drawing.Size(116, 25);
            this.lb_CountAll.TabIndex = 89;
            this.lb_CountAll.Text = "1000 / 1000";
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
            this.tb_record.Location = new System.Drawing.Point(328, 582);
            this.tb_record.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.tb_record.Name = "tb_record";
            this.tb_record.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.tb_record.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.tb_record.OnHoverTextColor = System.Drawing.Color.White;
            this.tb_record.selected = false;
            this.tb_record.Size = new System.Drawing.Size(229, 71);
            this.tb_record.TabIndex = 82;
            this.tb_record.Text = "Record Data";
            this.tb_record.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tb_record.Textcolor = System.Drawing.Color.Lavender;
            this.tb_record.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_record.Click += new System.EventHandler(this.tb_record_Click);
            // 
            // dtg_packing_size_appear
            // 
            this.dtg_packing_size_appear.AllowUserToAddRows = false;
            this.dtg_packing_size_appear.AllowUserToDeleteRows = false;
            this.dtg_packing_size_appear.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtg_packing_size_appear.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtg_packing_size_appear.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dtg_packing_size_appear.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_packing_size_appear.Location = new System.Drawing.Point(8, 50);
            this.dtg_packing_size_appear.Margin = new System.Windows.Forms.Padding(4);
            this.dtg_packing_size_appear.Name = "dtg_packing_size_appear";
            this.dtg_packing_size_appear.ReadOnly = true;
            this.dtg_packing_size_appear.Size = new System.Drawing.Size(917, 255);
            this.dtg_packing_size_appear.TabIndex = 0;
            this.dtg_packing_size_appear.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dtg_packing_size_appear_CellFormatting);
            this.dtg_packing_size_appear.SelectionChanged += new System.EventHandler(this.dtg_packing_size_appear_SelectionChanged);
            this.dtg_packing_size_appear.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dtg_packing_size_appear_MouseDown);
            // 
            // bt_select_packing_size_appear
            // 
            this.bt_select_packing_size_appear.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_select_packing_size_appear.ForeColor = System.Drawing.Color.Blue;
            this.bt_select_packing_size_appear.Location = new System.Drawing.Point(307, 313);
            this.bt_select_packing_size_appear.Margin = new System.Windows.Forms.Padding(4);
            this.bt_select_packing_size_appear.Name = "bt_select_packing_size_appear";
            this.bt_select_packing_size_appear.Size = new System.Drawing.Size(168, 60);
            this.bt_select_packing_size_appear.TabIndex = 1;
            this.bt_select_packing_size_appear.Text = "เริ่มตรวจสอบ";
            this.bt_select_packing_size_appear.UseVisualStyleBackColor = true;
            this.bt_select_packing_size_appear.Click += new System.EventHandler(this.bt_Select_Click);
            // 
            // bt_Clear
            // 
            this.bt_Clear.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_Clear.ForeColor = System.Drawing.Color.Blue;
            this.bt_Clear.Location = new System.Drawing.Point(483, 313);
            this.bt_Clear.Margin = new System.Windows.Forms.Padding(4);
            this.bt_Clear.Name = "bt_Clear";
            this.bt_Clear.Size = new System.Drawing.Size(168, 60);
            this.bt_Clear.TabIndex = 89;
            this.bt_Clear.Text = "เลือกลำดับ";
            this.bt_Clear.UseVisualStyleBackColor = true;
            this.bt_Clear.Click += new System.EventHandler(this.bt_Clear_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.Blue;
            this.label1.Location = new System.Drawing.Point(292, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 25);
            this.label1.TabIndex = 90;
            this.label1.Text = "จำนวนรวม ";
            // 
            // lbCount
            // 
            this.lbCount.AutoSize = true;
            this.lbCount.ForeColor = System.Drawing.Color.Blue;
            this.lbCount.Location = new System.Drawing.Point(388, 2);
            this.lbCount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbCount.Name = "lbCount";
            this.lbCount.Size = new System.Drawing.Size(72, 25);
            this.lbCount.TabIndex = 91;
            this.lbCount.Text = "50 / 50";
            // 
            // gb_pack
            // 
            this.gb_pack.Controls.Add(this.lbCount);
            this.gb_pack.Controls.Add(this.label1);
            this.gb_pack.Controls.Add(this.bt_Clear);
            this.gb_pack.Controls.Add(this.bt_select_packing_size_appear);
            this.gb_pack.Controls.Add(this.dtg_packing_size_appear);
            this.gb_pack.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_pack.Location = new System.Drawing.Point(16, 900);
            this.gb_pack.Margin = new System.Windows.Forms.Padding(4);
            this.gb_pack.Name = "gb_pack";
            this.gb_pack.Padding = new System.Windows.Forms.Padding(4);
            this.gb_pack.Size = new System.Drawing.Size(937, 380);
            this.gb_pack.TabIndex = 86;
            this.gb_pack.TabStop = false;
            this.gb_pack.Text = "Appearance Inspection Q\'ty";
            // 
            // bt_back
            // 
            this.bt_back.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.bt_back.BackColor = System.Drawing.Color.Goldenrod;
            this.bt_back.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bt_back.BorderRadius = 0;
            this.bt_back.ButtonText = "ย้อนกลับ";
            this.bt_back.DisabledColor = System.Drawing.Color.Gray;
            this.bt_back.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_back.Iconcolor = System.Drawing.Color.Transparent;
            this.bt_back.Iconimage = null;
            this.bt_back.Iconimage_right = ((System.Drawing.Image)(resources.GetObject("bt_back.Iconimage_right")));
            this.bt_back.Iconimage_right_Selected = null;
            this.bt_back.Iconimage_Selected = null;
            this.bt_back.IconMarginLeft = 0;
            this.bt_back.IconMarginRight = 0;
            this.bt_back.IconRightVisible = true;
            this.bt_back.IconRightZoom = 0D;
            this.bt_back.IconVisible = true;
            this.bt_back.IconZoom = 56D;
            this.bt_back.IsTab = false;
            this.bt_back.Location = new System.Drawing.Point(24, 9);
            this.bt_back.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.bt_back.Name = "bt_back";
            this.bt_back.Normalcolor = System.Drawing.Color.Goldenrod;
            this.bt_back.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.bt_back.OnHoverTextColor = System.Drawing.Color.White;
            this.bt_back.selected = false;
            this.bt_back.Size = new System.Drawing.Size(191, 38);
            this.bt_back.TabIndex = 88;
            this.bt_back.Text = "ย้อนกลับ";
            this.bt_back.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bt_back.Textcolor = System.Drawing.Color.White;
            this.bt_back.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_back.Click += new System.EventHandler(this.bt_back_Click);
            // 
            // userControlAppear
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.bt_back);
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
            this.Controls.Add(this.lb_top);
            this.Controls.Add(this.gb_input);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "userControlAppear";
            this.Size = new System.Drawing.Size(1487, 1970);
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
            this.gb_input.ResumeLayout(false);
            this.gb_input.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_packing_size_appear)).EndInit();
            this.gb_pack.ResumeLayout(false);
            this.gb_pack.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
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
        private System.Windows.Forms.Label lb_Qty;
        private System.Windows.Forms.DataGridView dtg_ngMode;
        private System.Windows.Forms.GroupBox gb_input;
        private Bunifu.Framework.UI.BunifuFlatButton tb_record;
        private System.Windows.Forms.DataGridViewTextBoxColumn QTY_NG;
        private System.Windows.Forms.DataGridViewTextBoxColumn NG_MODE;
        private System.Windows.Forms.DataGridView dtg_packing_size_appear;
        private System.Windows.Forms.Button bt_select_packing_size_appear;
        private System.Windows.Forms.Button bt_Clear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbCount;
        private System.Windows.Forms.GroupBox gb_pack;
        private Bunifu.Framework.UI.BunifuFlatButton bt_back;
        private System.Windows.Forms.Label lb_CountAll;
        private System.Windows.Forms.Label label3;
    }
}
