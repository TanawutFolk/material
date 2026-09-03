namespace RawMat.ViewsMaterial.FunctionCheck
{
    partial class userControlFunction
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing && (components != null))
        //    {
        //        components.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(userControlFunction));
            this.lb_top = new System.Windows.Forms.Label();
            this.lb_recDate = new System.Windows.Forms.Label();
            this.lb_invoice = new System.Windows.Forms.Label();
            this.lb_mcode = new System.Windows.Forms.Label();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.gb_cavity = new System.Windows.Forms.GroupBox();
            this.bt_confirmCavity = new System.Windows.Forms.Button();
            this.dtg_cavity = new System.Windows.Forms.DataGridView();
            this.picbox_cavity = new System.Windows.Forms.PictureBox();
            this.gb_material = new System.Windows.Forms.GroupBox();
            this.picbox_mat = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.picbox_func = new System.Windows.Forms.PictureBox();
            this.lb_sampName = new System.Windows.Forms.Label();
            this.dtg_function = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.lb_lotSize = new System.Windows.Forms.Label();
            this.tb_record = new Bunifu.Framework.UI.BunifuFlatButton();
            this.lb_TotalCavity = new System.Windows.Forms.Label();
            this.bt_back = new Bunifu.Framework.UI.BunifuFlatButton();
            this.gb_cavity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_cavity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).BeginInit();
            this.gb_material.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_mat)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_func)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_function)).BeginInit();
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
            this.lb_top.Text = "Function Check";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(9, 100);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(267, 19);
            this.lb_recDate.TabIndex = 59;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(349, 100);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(216, 19);
            this.lb_invoice.TabIndex = 58;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(349, 51);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(231, 19);
            this.lb_mcode.TabIndex = 57;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(9, 51);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(216, 19);
            this.lb_reportNo.TabIndex = 56;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // gb_cavity
            // 
            this.gb_cavity.Controls.Add(this.bt_confirmCavity);
            this.gb_cavity.Controls.Add(this.dtg_cavity);
            this.gb_cavity.Controls.Add(this.picbox_cavity);
            this.gb_cavity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_cavity.Location = new System.Drawing.Point(6, 144);
            this.gb_cavity.Name = "gb_cavity";
            this.gb_cavity.Size = new System.Drawing.Size(509, 211);
            this.gb_cavity.TabIndex = 61;
            this.gb_cavity.TabStop = false;
            this.gb_cavity.Text = "Cavity : xxxx";
            // 
            // bt_confirmCavity
            // 
            this.bt_confirmCavity.Location = new System.Drawing.Point(280, 26);
            this.bt_confirmCavity.Name = "bt_confirmCavity";
            this.bt_confirmCavity.Size = new System.Drawing.Size(207, 30);
            this.bt_confirmCavity.TabIndex = 49;
            this.bt_confirmCavity.Text = "CONFIRM CAVITY";
            this.bt_confirmCavity.UseVisualStyleBackColor = true;
            this.bt_confirmCavity.Click += new System.EventHandler(this.bt_confirmCavity_Click);
            // 
            // dtg_cavity
            // 
            this.dtg_cavity.AllowUserToAddRows = false;
            this.dtg_cavity.AllowUserToDeleteRows = false;
            this.dtg_cavity.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dtg_cavity.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_cavity.Location = new System.Drawing.Point(11, 29);
            this.dtg_cavity.Name = "dtg_cavity";
            this.dtg_cavity.Size = new System.Drawing.Size(263, 176);
            this.dtg_cavity.TabIndex = 47;
            // 
            // picbox_cavity
            // 
            this.picbox_cavity.Image = global::RawMat.Properties.Resources.Cavity;
            this.picbox_cavity.Location = new System.Drawing.Point(280, 62);
            this.picbox_cavity.Name = "picbox_cavity";
            this.picbox_cavity.Size = new System.Drawing.Size(207, 143);
            this.picbox_cavity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_cavity.TabIndex = 0;
            this.picbox_cavity.TabStop = false;
            // 
            // gb_material
            // 
            this.gb_material.Controls.Add(this.picbox_mat);
            this.gb_material.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_material.Location = new System.Drawing.Point(521, 144);
            this.gb_material.Name = "gb_material";
            this.gb_material.Size = new System.Drawing.Size(591, 211);
            this.gb_material.TabIndex = 62;
            this.gb_material.TabStop = false;
            this.gb_material.Text = "สิ่งที่ต้องเตรียม (Material)";
            // 
            // picbox_mat
            // 
            this.picbox_mat.Image = global::RawMat.Properties.Resources.Cavity;
            this.picbox_mat.Location = new System.Drawing.Point(6, 29);
            this.picbox_mat.Name = "picbox_mat";
            this.picbox_mat.Size = new System.Drawing.Size(579, 176);
            this.picbox_mat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_mat.TabIndex = 0;
            this.picbox_mat.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.picbox_func);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(7, 361);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1099, 211);
            this.groupBox1.TabIndex = 63;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "วิธีการประกอบ (Checking Method)";
            // 
            // picbox_func
            // 
            this.picbox_func.Image = global::RawMat.Properties.Resources.Cavity;
            this.picbox_func.Location = new System.Drawing.Point(6, 29);
            this.picbox_func.Name = "picbox_func";
            this.picbox_func.Size = new System.Drawing.Size(1087, 176);
            this.picbox_func.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_func.TabIndex = 0;
            this.picbox_func.TabStop = false;
            // 
            // lb_sampName
            // 
            this.lb_sampName.AutoSize = true;
            this.lb_sampName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_sampName.ForeColor = System.Drawing.Color.Black;
            this.lb_sampName.Location = new System.Drawing.Point(15, 568);
            this.lb_sampName.Name = "lb_sampName";
            this.lb_sampName.Size = new System.Drawing.Size(160, 23);
            this.lb_sampName.TabIndex = 65;
            this.lb_sampName.Text = "Sampling Name";
            // 
            // dtg_function
            // 
            this.dtg_function.AllowUserToAddRows = false;
            this.dtg_function.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_function.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dtg_function.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtg_function.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtg_function.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtg_function.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_function.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_function.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.SeaGreen;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_function.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtg_function.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dtg_function.DefaultCellStyle = dataGridViewCellStyle3;
            this.dtg_function.DoubleBuffered = true;
            this.dtg_function.EnableHeadersVisualStyles = false;
            this.dtg_function.HeaderBgColor = System.Drawing.Color.SeaGreen;
            this.dtg_function.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_function.Location = new System.Drawing.Point(13, 594);
            this.dtg_function.Name = "dtg_function";
            this.dtg_function.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_function.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dtg_function.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dtg_function.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtg_function.RowTemplate.Height = 41;
            this.dtg_function.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dtg_function.Size = new System.Drawing.Size(1087, 351);
            this.dtg_function.TabIndex = 64;
            this.dtg_function.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_function_CellEndEdit);
            this.dtg_function.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dtg_function_DataBindingComplete);
            // 
            // lb_lotSize
            // 
            this.lb_lotSize.AutoSize = true;
            this.lb_lotSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotSize.Location = new System.Drawing.Point(797, 51);
            this.lb_lotSize.Name = "lb_lotSize";
            this.lb_lotSize.Size = new System.Drawing.Size(83, 19);
            this.lb_lotSize.TabIndex = 71;
            this.lb_lotSize.Text = "LOT SIZE";
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
            this.tb_record.Location = new System.Drawing.Point(459, 952);
            this.tb_record.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tb_record.Name = "tb_record";
            this.tb_record.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.tb_record.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.tb_record.OnHoverTextColor = System.Drawing.Color.White;
            this.tb_record.selected = false;
            this.tb_record.Size = new System.Drawing.Size(187, 58);
            this.tb_record.TabIndex = 72;
            this.tb_record.Text = "Record Data";
            this.tb_record.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tb_record.Textcolor = System.Drawing.Color.Lavender;
            this.tb_record.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_record.Click += new System.EventHandler(this.tb_record_Click);
            // 
            // lb_TotalCavity
            // 
            this.lb_TotalCavity.AutoSize = true;
            this.lb_TotalCavity.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_TotalCavity.Location = new System.Drawing.Point(595, 100);
            this.lb_TotalCavity.Name = "lb_TotalCavity";
            this.lb_TotalCavity.Size = new System.Drawing.Size(245, 19);
            this.lb_TotalCavity.TabIndex = 73;
            this.lb_TotalCavity.Text = "Total Cavity : XXXXXXXXXXX";
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
            // userControlFunction
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Wheat;
            this.Controls.Add(this.bt_back);
            this.Controls.Add(this.lb_TotalCavity);
            this.Controls.Add(this.tb_record);
            this.Controls.Add(this.lb_lotSize);
            this.Controls.Add(this.lb_sampName);
            this.Controls.Add(this.dtg_function);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gb_material);
            this.Controls.Add(this.gb_cavity);
            this.Controls.Add(this.lb_recDate);
            this.Controls.Add(this.lb_invoice);
            this.Controls.Add(this.lb_mcode);
            this.Controls.Add(this.lb_reportNo);
            this.Controls.Add(this.lb_top);
            this.Name = "userControlFunction";
            this.Size = new System.Drawing.Size(1115, 1016);
            this.Load += new System.EventHandler(this.userControlFunction_Load);
            this.gb_cavity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_cavity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).EndInit();
            this.gb_material.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbox_mat)).EndInit();
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbox_func)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_function)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
        private System.Windows.Forms.Label lb_recDate;
        private System.Windows.Forms.Label lb_invoice;
        private System.Windows.Forms.Label lb_mcode;
        private System.Windows.Forms.Label lb_reportNo;
        private System.Windows.Forms.GroupBox gb_cavity;
        private System.Windows.Forms.Button bt_confirmCavity;
        private System.Windows.Forms.DataGridView dtg_cavity;
        private System.Windows.Forms.PictureBox picbox_cavity;
        private System.Windows.Forms.GroupBox gb_material;
        private System.Windows.Forms.PictureBox picbox_mat;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox picbox_func;
        private System.Windows.Forms.Label lb_sampName;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_function;
        private System.Windows.Forms.Label lb_lotSize;
        private Bunifu.Framework.UI.BunifuFlatButton tb_record;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lb_TotalCavity;
        private Bunifu.Framework.UI.BunifuFlatButton bt_back;
    }
}
