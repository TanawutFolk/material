namespace RawMat.ViewsMaterial.PackingCheck
{
    partial class userControlPackingCheck
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(userControlPackingCheck));
            this.lb_top = new System.Windows.Forms.Label();
            this.lb_method1 = new System.Windows.Forms.Label();
            this.tlp_methods = new System.Windows.Forms.TableLayoutPanel();
            this.pb_packing3 = new System.Windows.Forms.PictureBox();
            this.pb_packing2 = new System.Windows.Forms.PictureBox();
            this.lb_method2 = new System.Windows.Forms.Label();
            this.lb_method3 = new System.Windows.Forms.Label();
            this.pb_packing1 = new System.Windows.Forms.PictureBox();
            this.gb_method1 = new System.Windows.Forms.GroupBox();
            this.lb_length_detail_method1 = new System.Windows.Forms.Label();
            this.tb_detail_method1 = new System.Windows.Forms.TextBox();
            this.rb_ng_method1 = new System.Windows.Forms.RadioButton();
            this.rb_ok_method1 = new System.Windows.Forms.RadioButton();
            this.gb_method2 = new System.Windows.Forms.GroupBox();
            this.lb_length_detail_method2 = new System.Windows.Forms.Label();
            this.tb_detail_method2 = new System.Windows.Forms.TextBox();
            this.rb_ng_method2 = new System.Windows.Forms.RadioButton();
            this.rb_ok_method2 = new System.Windows.Forms.RadioButton();
            this.gb_method3 = new System.Windows.Forms.GroupBox();
            this.dtg_packing_size = new System.Windows.Forms.DataGridView();
            this.VALUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PACK_COUNT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CALVALUE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lb_length_detail_method3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_detail_method3 = new System.Windows.Forms.TextBox();
            this.rb_ng_method3 = new System.Windows.Forms.RadioButton();
            this.rb_ok_method3 = new System.Windows.Forms.RadioButton();
            this.bt_save = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.lb_mcode = new System.Windows.Forms.Label();
            this.lb_lotSize = new System.Windows.Forms.Label();
            this.lb_invoice = new System.Windows.Forms.Label();
            this.lb_recDate = new System.Windows.Forms.Label();
            this.dtg_lot_no = new System.Windows.Forms.DataGridView();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bt_back = new Bunifu.Framework.UI.BunifuFlatButton();
            this.tlp_methods.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_packing3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_packing2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_packing1)).BeginInit();
            this.gb_method1.SuspendLayout();
            this.gb_method2.SuspendLayout();
            this.gb_method3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_packing_size)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_lot_no)).BeginInit();
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
            this.lb_top.TabIndex = 28;
            this.lb_top.Text = "Packing Check";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_method1
            // 
            this.lb_method1.AutoSize = true;
            this.lb_method1.BackColor = System.Drawing.Color.Aquamarine;
            this.lb_method1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_method1.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_method1.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lb_method1.Location = new System.Drawing.Point(4, 1);
            this.lb_method1.Name = "lb_method1";
            this.lb_method1.Size = new System.Drawing.Size(706, 27);
            this.lb_method1.TabIndex = 1;
            this.lb_method1.Text = "กล่อง/ถุง อยู่ในสภาพสมบูรณ์ ไม่บุบ ยุบหรือฉีกขาด";
            // 
            // tlp_methods
            // 
            this.tlp_methods.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlp_methods.ColumnCount = 1;
            this.tlp_methods.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_methods.Controls.Add(this.pb_packing3, 0, 5);
            this.tlp_methods.Controls.Add(this.pb_packing2, 0, 3);
            this.tlp_methods.Controls.Add(this.lb_method2, 0, 2);
            this.tlp_methods.Controls.Add(this.lb_method3, 0, 4);
            this.tlp_methods.Controls.Add(this.lb_method1, 0, 0);
            this.tlp_methods.Controls.Add(this.pb_packing1, 0, 1);
            this.tlp_methods.Location = new System.Drawing.Point(26, 99);
            this.tlp_methods.Name = "tlp_methods";
            this.tlp_methods.RowCount = 6;
            this.tlp_methods.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3.878116F));
            this.tlp_methods.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27.28532F));
            this.tlp_methods.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 4.016621F));
            this.tlp_methods.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 22.99169F));
            this.tlp_methods.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 7.479224F));
            this.tlp_methods.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 34.07202F));
            this.tlp_methods.Size = new System.Drawing.Size(714, 723);
            this.tlp_methods.TabIndex = 29;
            // 
            // pb_packing3
            // 
            this.pb_packing3.Image = global::RawMat.Properties.Resources.no_photo;
            this.pb_packing3.Location = new System.Drawing.Point(4, 477);
            this.pb_packing3.Name = "pb_packing3";
            this.pb_packing3.Size = new System.Drawing.Size(706, 242);
            this.pb_packing3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_packing3.TabIndex = 12;
            this.pb_packing3.TabStop = false;
            // 
            // pb_packing2
            // 
            this.pb_packing2.Image = global::RawMat.Properties.Resources.no_photo;
            this.pb_packing2.Location = new System.Drawing.Point(4, 257);
            this.pb_packing2.Name = "pb_packing2";
            this.pb_packing2.Size = new System.Drawing.Size(706, 159);
            this.pb_packing2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_packing2.TabIndex = 11;
            this.pb_packing2.TabStop = false;
            // 
            // lb_method2
            // 
            this.lb_method2.AutoSize = true;
            this.lb_method2.BackColor = System.Drawing.Color.Aquamarine;
            this.lb_method2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_method2.Font = new System.Drawing.Font("Tahoma", 15.75F);
            this.lb_method2.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lb_method2.Location = new System.Drawing.Point(4, 225);
            this.lb_method2.Name = "lb_method2";
            this.lb_method2.Size = new System.Drawing.Size(706, 28);
            this.lb_method2.TabIndex = 9;
            this.lb_method2.Text = "ชื่อของชิ้นงานที่ได้รับตรงกับชิ้นงานจริงในกล่องและตรงกับป้ายแสดงข้างกล่อง";
            // 
            // lb_method3
            // 
            this.lb_method3.AutoSize = true;
            this.lb_method3.BackColor = System.Drawing.Color.Aquamarine;
            this.lb_method3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_method3.Font = new System.Drawing.Font("Tahoma", 15.75F);
            this.lb_method3.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lb_method3.Location = new System.Drawing.Point(4, 420);
            this.lb_method3.Name = "lb_method3";
            this.lb_method3.Size = new System.Drawing.Size(706, 53);
            this.lb_method3.TabIndex = 8;
            this.lb_method3.Text = "จำนวนที่ได้รับตรงกับจำนวนที่แสดงในช่อง Lot Size และตรงกับป้ายแสดงข้างกล่อง";
            // 
            // pb_packing1
            // 
            this.pb_packing1.Image = global::RawMat.Properties.Resources.no_photo;
            this.pb_packing1.Location = new System.Drawing.Point(4, 32);
            this.pb_packing1.Name = "pb_packing1";
            this.pb_packing1.Size = new System.Drawing.Size(706, 189);
            this.pb_packing1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_packing1.TabIndex = 10;
            this.pb_packing1.TabStop = false;
            // 
            // gb_method1
            // 
            this.gb_method1.Controls.Add(this.lb_length_detail_method1);
            this.gb_method1.Controls.Add(this.tb_detail_method1);
            this.gb_method1.Controls.Add(this.rb_ng_method1);
            this.gb_method1.Controls.Add(this.rb_ok_method1);
            this.gb_method1.Location = new System.Drawing.Point(746, 99);
            this.gb_method1.Name = "gb_method1";
            this.gb_method1.Size = new System.Drawing.Size(325, 138);
            this.gb_method1.TabIndex = 30;
            this.gb_method1.TabStop = false;
            // 
            // lb_length_detail_method1
            // 
            this.lb_length_detail_method1.AutoSize = true;
            this.lb_length_detail_method1.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lb_length_detail_method1.Location = new System.Drawing.Point(265, 22);
            this.lb_length_detail_method1.Name = "lb_length_detail_method1";
            this.lb_length_detail_method1.Size = new System.Drawing.Size(53, 13);
            this.lb_length_detail_method1.TabIndex = 3;
            this.lb_length_detail_method1.Text = "000 / 255";
            this.lb_length_detail_method1.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tb_detail_method1
            // 
            this.tb_detail_method1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_detail_method1.Location = new System.Drawing.Point(10, 46);
            this.tb_detail_method1.Multiline = true;
            this.tb_detail_method1.Name = "tb_detail_method1";
            this.tb_detail_method1.Size = new System.Drawing.Size(309, 86);
            this.tb_detail_method1.TabIndex = 2;
            this.tb_detail_method1.TextChanged += new System.EventHandler(this.tb_detail_method1_TextChanged);
            // 
            // rb_ng_method1
            // 
            this.rb_ng_method1.AutoSize = true;
            this.rb_ng_method1.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.rb_ng_method1.Location = new System.Drawing.Point(80, 12);
            this.rb_ng_method1.Name = "rb_ng_method1";
            this.rb_ng_method1.Size = new System.Drawing.Size(54, 27);
            this.rb_ng_method1.TabIndex = 1;
            this.rb_ng_method1.TabStop = true;
            this.rb_ng_method1.Text = "NG";
            this.rb_ng_method1.UseVisualStyleBackColor = true;
            // 
            // rb_ok_method1
            // 
            this.rb_ok_method1.AutoSize = true;
            this.rb_ok_method1.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.rb_ok_method1.Location = new System.Drawing.Point(10, 12);
            this.rb_ok_method1.Name = "rb_ok_method1";
            this.rb_ok_method1.Size = new System.Drawing.Size(52, 27);
            this.rb_ok_method1.TabIndex = 0;
            this.rb_ok_method1.TabStop = true;
            this.rb_ok_method1.Text = "OK";
            this.rb_ok_method1.UseVisualStyleBackColor = true;
            // 
            // gb_method2
            // 
            this.gb_method2.Controls.Add(this.lb_length_detail_method2);
            this.gb_method2.Controls.Add(this.tb_detail_method2);
            this.gb_method2.Controls.Add(this.rb_ng_method2);
            this.gb_method2.Controls.Add(this.rb_ok_method2);
            this.gb_method2.Location = new System.Drawing.Point(746, 324);
            this.gb_method2.Name = "gb_method2";
            this.gb_method2.Size = new System.Drawing.Size(325, 136);
            this.gb_method2.TabIndex = 31;
            this.gb_method2.TabStop = false;
            // 
            // lb_length_detail_method2
            // 
            this.lb_length_detail_method2.AutoSize = true;
            this.lb_length_detail_method2.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lb_length_detail_method2.Location = new System.Drawing.Point(265, 16);
            this.lb_length_detail_method2.Name = "lb_length_detail_method2";
            this.lb_length_detail_method2.Size = new System.Drawing.Size(53, 13);
            this.lb_length_detail_method2.TabIndex = 6;
            this.lb_length_detail_method2.Text = "000 / 255";
            this.lb_length_detail_method2.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tb_detail_method2
            // 
            this.tb_detail_method2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_detail_method2.Location = new System.Drawing.Point(11, 42);
            this.tb_detail_method2.Multiline = true;
            this.tb_detail_method2.Name = "tb_detail_method2";
            this.tb_detail_method2.Size = new System.Drawing.Size(309, 86);
            this.tb_detail_method2.TabIndex = 5;
            this.tb_detail_method2.TextChanged += new System.EventHandler(this.tb_detail_method2_TextChanged);
            // 
            // rb_ng_method2
            // 
            this.rb_ng_method2.AutoSize = true;
            this.rb_ng_method2.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.rb_ng_method2.Location = new System.Drawing.Point(80, 11);
            this.rb_ng_method2.Name = "rb_ng_method2";
            this.rb_ng_method2.Size = new System.Drawing.Size(54, 27);
            this.rb_ng_method2.TabIndex = 4;
            this.rb_ng_method2.TabStop = true;
            this.rb_ng_method2.Text = "NG";
            this.rb_ng_method2.UseVisualStyleBackColor = true;
            // 
            // rb_ok_method2
            // 
            this.rb_ok_method2.AutoSize = true;
            this.rb_ok_method2.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.rb_ok_method2.Location = new System.Drawing.Point(10, 11);
            this.rb_ok_method2.Name = "rb_ok_method2";
            this.rb_ok_method2.Size = new System.Drawing.Size(52, 27);
            this.rb_ok_method2.TabIndex = 3;
            this.rb_ok_method2.TabStop = true;
            this.rb_ok_method2.Text = "OK";
            this.rb_ok_method2.UseVisualStyleBackColor = true;
            // 
            // gb_method3
            // 
            this.gb_method3.Controls.Add(this.dtg_packing_size);
            this.gb_method3.Controls.Add(this.lb_length_detail_method3);
            this.gb_method3.Controls.Add(this.label2);
            this.gb_method3.Controls.Add(this.tb_detail_method3);
            this.gb_method3.Controls.Add(this.rb_ng_method3);
            this.gb_method3.Controls.Add(this.rb_ok_method3);
            this.gb_method3.Location = new System.Drawing.Point(746, 514);
            this.gb_method3.Name = "gb_method3";
            this.gb_method3.Size = new System.Drawing.Size(325, 304);
            this.gb_method3.TabIndex = 32;
            this.gb_method3.TabStop = false;
            // 
            // dtg_packing_size
            // 
            this.dtg_packing_size.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_packing_size.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.VALUE,
            this.PACK_COUNT,
            this.CALVALUE});
            this.dtg_packing_size.Location = new System.Drawing.Point(6, 53);
            this.dtg_packing_size.Name = "dtg_packing_size";
            this.dtg_packing_size.Size = new System.Drawing.Size(313, 175);
            this.dtg_packing_size.TabIndex = 12;
            this.dtg_packing_size.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_packing_size_CellEndEdit);
            this.dtg_packing_size.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dtg_packing_size_RowValidating);
            // 
            // VALUE
            // 
            this.VALUE.HeaderText = "จำนวนงาน";
            this.VALUE.Name = "VALUE";
            // 
            // PACK_COUNT
            // 
            this.PACK_COUNT.HeaderText = "จำนวน Pack";
            this.PACK_COUNT.Name = "PACK_COUNT";
            this.PACK_COUNT.Width = 80;
            // 
            // CALVALUE
            // 
            this.CALVALUE.HeaderText = "คำนวณจำนวนงาน";
            this.CALVALUE.Name = "CALVALUE";
            this.CALVALUE.ReadOnly = true;
            // 
            // lb_length_detail_method3
            // 
            this.lb_length_detail_method3.AutoSize = true;
            this.lb_length_detail_method3.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lb_length_detail_method3.Location = new System.Drawing.Point(265, 231);
            this.lb_length_detail_method3.Name = "lb_length_detail_method3";
            this.lb_length_detail_method3.Size = new System.Drawing.Size(53, 13);
            this.lb_length_detail_method3.TabIndex = 11;
            this.lb_length_detail_method3.Text = "000 / 255";
            this.lb_length_detail_method3.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 12F);
            this.label2.Location = new System.Drawing.Point(223, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 19);
            this.label2.TabIndex = 10;
            this.label2.Text = "Packing size";
            // 
            // tb_detail_method3
            // 
            this.tb_detail_method3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_detail_method3.Location = new System.Drawing.Point(73, 247);
            this.tb_detail_method3.Multiline = true;
            this.tb_detail_method3.Name = "tb_detail_method3";
            this.tb_detail_method3.Size = new System.Drawing.Size(247, 44);
            this.tb_detail_method3.TabIndex = 8;
            this.tb_detail_method3.TextChanged += new System.EventHandler(this.tb_detail_method3_TextChanged);
            // 
            // rb_ng_method3
            // 
            this.rb_ng_method3.AutoSize = true;
            this.rb_ng_method3.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.rb_ng_method3.Location = new System.Drawing.Point(11, 263);
            this.rb_ng_method3.Name = "rb_ng_method3";
            this.rb_ng_method3.Size = new System.Drawing.Size(54, 27);
            this.rb_ng_method3.TabIndex = 7;
            this.rb_ng_method3.TabStop = true;
            this.rb_ng_method3.Text = "NG";
            this.rb_ng_method3.UseVisualStyleBackColor = true;
            // 
            // rb_ok_method3
            // 
            this.rb_ok_method3.AutoSize = true;
            this.rb_ok_method3.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.rb_ok_method3.Location = new System.Drawing.Point(6, 19);
            this.rb_ok_method3.Name = "rb_ok_method3";
            this.rb_ok_method3.Size = new System.Drawing.Size(52, 27);
            this.rb_ok_method3.TabIndex = 6;
            this.rb_ok_method3.TabStop = true;
            this.rb_ok_method3.Text = "OK";
            this.rb_ok_method3.UseVisualStyleBackColor = true;
            this.rb_ok_method3.CheckedChanged += new System.EventHandler(this.rb_ok_method3_CheckedChanged);
            // 
            // bt_save
            // 
            this.bt_save.BackColor = System.Drawing.Color.Lime;
            this.bt_save.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bt_save.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_save.Location = new System.Drawing.Point(757, 845);
            this.bt_save.Name = "bt_save";
            this.bt_save.Size = new System.Drawing.Size(129, 57);
            this.bt_save.TabIndex = 33;
            this.bt_save.Text = "SAVE";
            this.bt_save.UseVisualStyleBackColor = false;
            this.bt_save.Click += new System.EventHandler(this.bt_save_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(22, 869);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(124, 19);
            this.label1.TabIndex = 36;
            this.label1.Text = "LOT NO Check";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(22, 38);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(216, 19);
            this.lb_reportNo.TabIndex = 37;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(420, 38);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(231, 19);
            this.lb_mcode.TabIndex = 39;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_lotSize
            // 
            this.lb_lotSize.AutoSize = true;
            this.lb_lotSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotSize.Location = new System.Drawing.Point(752, 77);
            this.lb_lotSize.Name = "lb_lotSize";
            this.lb_lotSize.Size = new System.Drawing.Size(83, 19);
            this.lb_lotSize.TabIndex = 40;
            this.lb_lotSize.Text = "LOT SIZE";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(420, 77);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(216, 19);
            this.lb_invoice.TabIndex = 41;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(22, 77);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(267, 19);
            this.lb_recDate.TabIndex = 42;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // dtg_lot_no
            // 
            this.dtg_lot_no.AllowUserToDeleteRows = false;
            this.dtg_lot_no.AllowUserToOrderColumns = true;
            this.dtg_lot_no.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_lot_no.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Column1});
            this.dtg_lot_no.Location = new System.Drawing.Point(180, 852);
            this.dtg_lot_no.Margin = new System.Windows.Forms.Padding(2);
            this.dtg_lot_no.Name = "dtg_lot_no";
            this.dtg_lot_no.RowHeadersWidth = 51;
            this.dtg_lot_no.RowTemplate.Height = 24;
            this.dtg_lot_no.Size = new System.Drawing.Size(527, 50);
            this.dtg_lot_no.TabIndex = 43;
            this.dtg_lot_no.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_lot_no_CellEndEdit);
            this.dtg_lot_no.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dtg_lot_no_EditingControlShowing);
            // 
            // Column1
            // 
            this.Column1.HeaderText = "LOT_NO1";
            this.Column1.Name = "Column1";
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
            this.bt_back.Location = new System.Drawing.Point(12, 6);
            this.bt_back.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_back.Name = "bt_back";
            this.bt_back.Normalcolor = System.Drawing.Color.Goldenrod;
            this.bt_back.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.bt_back.OnHoverTextColor = System.Drawing.Color.White;
            this.bt_back.selected = false;
            this.bt_back.Size = new System.Drawing.Size(143, 31);
            this.bt_back.TabIndex = 91;
            this.bt_back.Text = "ย้อนกลับ";
            this.bt_back.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.bt_back.Textcolor = System.Drawing.Color.White;
            this.bt_back.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_back.Click += new System.EventHandler(this.bt_back_Click);
            // 
            // userControlPackingCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.LemonChiffon;
            this.Controls.Add(this.bt_back);
            this.Controls.Add(this.dtg_lot_no);
            this.Controls.Add(this.lb_recDate);
            this.Controls.Add(this.lb_invoice);
            this.Controls.Add(this.lb_lotSize);
            this.Controls.Add(this.lb_mcode);
            this.Controls.Add(this.lb_reportNo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.bt_save);
            this.Controls.Add(this.gb_method3);
            this.Controls.Add(this.gb_method2);
            this.Controls.Add(this.gb_method1);
            this.Controls.Add(this.tlp_methods);
            this.Controls.Add(this.lb_top);
            this.Name = "userControlPackingCheck";
            this.Size = new System.Drawing.Size(1115, 955);
            this.Load += new System.EventHandler(this.userControlPackingCheck_Load);
            this.tlp_methods.ResumeLayout(false);
            this.tlp_methods.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_packing3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_packing2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb_packing1)).EndInit();
            this.gb_method1.ResumeLayout(false);
            this.gb_method1.PerformLayout();
            this.gb_method2.ResumeLayout(false);
            this.gb_method2.PerformLayout();
            this.gb_method3.ResumeLayout(false);
            this.gb_method3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_packing_size)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_lot_no)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
        private System.Windows.Forms.Label lb_method1;
        private System.Windows.Forms.TableLayoutPanel tlp_methods;
        private System.Windows.Forms.GroupBox gb_method1;
        private System.Windows.Forms.GroupBox gb_method2;
        private System.Windows.Forms.GroupBox gb_method3;
        private System.Windows.Forms.Button bt_save;
        private System.Windows.Forms.TextBox tb_detail_method1;
        private System.Windows.Forms.RadioButton rb_ng_method1;
        private System.Windows.Forms.RadioButton rb_ok_method1;
        private System.Windows.Forms.TextBox tb_detail_method2;
        private System.Windows.Forms.RadioButton rb_ng_method2;
        private System.Windows.Forms.RadioButton rb_ok_method2;
        private System.Windows.Forms.TextBox tb_detail_method3;
        private System.Windows.Forms.RadioButton rb_ng_method3;
        private System.Windows.Forms.RadioButton rb_ok_method3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_reportNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lb_method2;
        private System.Windows.Forms.Label lb_method3;
        private System.Windows.Forms.Label lb_length_detail_method1;
        private System.Windows.Forms.Label lb_length_detail_method2;
        private System.Windows.Forms.Label lb_length_detail_method3;
        private System.Windows.Forms.PictureBox pb_packing1;
        private System.Windows.Forms.PictureBox pb_packing3;
        private System.Windows.Forms.PictureBox pb_packing2;
        private System.Windows.Forms.Label lb_mcode;
        private System.Windows.Forms.Label lb_lotSize;
        private System.Windows.Forms.Label lb_invoice;
        private System.Windows.Forms.Label lb_recDate;
        private System.Windows.Forms.DataGridView dtg_packing_size;
        private System.Windows.Forms.DataGridView dtg_lot_no;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewTextBoxColumn VALUE;
        private System.Windows.Forms.DataGridViewTextBoxColumn PACK_COUNT;
        private System.Windows.Forms.DataGridViewTextBoxColumn CALVALUE;
        private Bunifu.Framework.UI.BunifuFlatButton bt_back;
    }
}
