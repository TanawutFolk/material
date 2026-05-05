namespace RawMat.Views.FunctionCheck
{
    partial class userControlFunctionPending
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lb_top = new System.Windows.Forms.Label();
            this.lb_lotSize = new System.Windows.Forms.Label();
            this.lb_recDate = new System.Windows.Forms.Label();
            this.lb_invoice = new System.Windows.Forms.Label();
            this.lb_mcode = new System.Windows.Forms.Label();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.gb_cavity = new System.Windows.Forms.GroupBox();
            this.picbox_cavity = new System.Windows.Forms.PictureBox();
            this.gb_material = new System.Windows.Forms.GroupBox();
            this.picbox_mat = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.picbox_func = new System.Windows.Forms.PictureBox();
            this.tb_record = new Bunifu.Framework.UI.BunifuFlatButton();
            this.lb_sampName = new System.Windows.Forms.Label();
            this.dtg_function = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.gb_cavity.SuspendLayout();
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
            this.lb_top.TabIndex = 33;
            this.lb_top.Text = "Function Check Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_lotSize
            // 
            this.lb_lotSize.AutoSize = true;
            this.lb_lotSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotSize.Location = new System.Drawing.Point(806, 54);
            this.lb_lotSize.Name = "lb_lotSize";
            this.lb_lotSize.Size = new System.Drawing.Size(83, 19);
            this.lb_lotSize.TabIndex = 76;
            this.lb_lotSize.Text = "LOT SIZE";
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(18, 103);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(267, 19);
            this.lb_recDate.TabIndex = 75;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(358, 103);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(216, 19);
            this.lb_invoice.TabIndex = 74;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(358, 54);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(231, 19);
            this.lb_mcode.TabIndex = 73;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(18, 54);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(216, 19);
            this.lb_reportNo.TabIndex = 72;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // gb_cavity
            // 
            this.gb_cavity.Controls.Add(this.picbox_cavity);
            this.gb_cavity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_cavity.Location = new System.Drawing.Point(6, 158);
            this.gb_cavity.Name = "gb_cavity";
            this.gb_cavity.Size = new System.Drawing.Size(497, 211);
            this.gb_cavity.TabIndex = 77;
            this.gb_cavity.TabStop = false;
            this.gb_cavity.Text = "Cavity";
            // 
            // picbox_cavity
            // 
            this.picbox_cavity.Image = global::RawMat.Properties.Resources.Cavity;
            this.picbox_cavity.Location = new System.Drawing.Point(6, 29);
            this.picbox_cavity.Name = "picbox_cavity";
            this.picbox_cavity.Size = new System.Drawing.Size(491, 176);
            this.picbox_cavity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_cavity.TabIndex = 0;
            this.picbox_cavity.TabStop = false;
            // 
            // gb_material
            // 
            this.gb_material.Controls.Add(this.picbox_mat);
            this.gb_material.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_material.Location = new System.Drawing.Point(509, 158);
            this.gb_material.Name = "gb_material";
            this.gb_material.Size = new System.Drawing.Size(591, 211);
            this.gb_material.TabIndex = 78;
            this.gb_material.TabStop = false;
            this.gb_material.Text = "สิ่งที่ต้องเตรียม (Material)";
            // 
            // picbox_mat
            // 
            this.picbox_mat.Image = global::RawMat.Properties.Resources.Cavity;
            this.picbox_mat.Location = new System.Drawing.Point(6, 29);
            this.picbox_mat.Name = "picbox_mat";
            this.picbox_mat.Size = new System.Drawing.Size(590, 176);
            this.picbox_mat.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_mat.TabIndex = 0;
            this.picbox_mat.TabStop = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.picbox_func);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(12, 375);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1099, 211);
            this.groupBox1.TabIndex = 79;
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
            this.tb_record.Location = new System.Drawing.Point(464, 965);
            this.tb_record.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tb_record.Name = "tb_record";
            this.tb_record.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.tb_record.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.tb_record.OnHoverTextColor = System.Drawing.Color.White;
            this.tb_record.selected = false;
            this.tb_record.Size = new System.Drawing.Size(187, 58);
            this.tb_record.TabIndex = 82;
            this.tb_record.Text = "Record Data";
            this.tb_record.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tb_record.Textcolor = System.Drawing.Color.Lavender;
            this.tb_record.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_record.Click += new System.EventHandler(this.tb_record_Click);
            // 
            // lb_sampName
            // 
            this.lb_sampName.AutoSize = true;
            this.lb_sampName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_sampName.ForeColor = System.Drawing.Color.Black;
            this.lb_sampName.Location = new System.Drawing.Point(20, 581);
            this.lb_sampName.Name = "lb_sampName";
            this.lb_sampName.Size = new System.Drawing.Size(160, 23);
            this.lb_sampName.TabIndex = 81;
            this.lb_sampName.Text = "Sampling Name";
            // 
            // dtg_function
            // 
            this.dtg_function.AllowUserToAddRows = false;
            this.dtg_function.AllowUserToDeleteRows = false;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_function.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle9;
            this.dtg_function.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtg_function.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtg_function.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtg_function.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_function.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_function.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.SeaGreen;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_function.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.dtg_function.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dtg_function.DefaultCellStyle = dataGridViewCellStyle11;
            this.dtg_function.DoubleBuffered = true;
            this.dtg_function.EnableHeadersVisualStyles = false;
            this.dtg_function.HeaderBgColor = System.Drawing.Color.SeaGreen;
            this.dtg_function.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_function.Location = new System.Drawing.Point(18, 607);
            this.dtg_function.Name = "dtg_function";
            this.dtg_function.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_function.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.dtg_function.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dtg_function.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtg_function.RowTemplate.Height = 41;
            this.dtg_function.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dtg_function.Size = new System.Drawing.Size(1087, 351);
            this.dtg_function.TabIndex = 80;
            // 
            // userControlFunctionPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Wheat;
            this.Controls.Add(this.tb_record);
            this.Controls.Add(this.lb_sampName);
            this.Controls.Add(this.dtg_function);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gb_material);
            this.Controls.Add(this.gb_cavity);
            this.Controls.Add(this.lb_lotSize);
            this.Controls.Add(this.lb_recDate);
            this.Controls.Add(this.lb_invoice);
            this.Controls.Add(this.lb_mcode);
            this.Controls.Add(this.lb_reportNo);
            this.Controls.Add(this.lb_top);
            this.Name = "userControlFunctionPending";
            this.Size = new System.Drawing.Size(1115, 1033);
            this.Load += new System.EventHandler(this.userControlFunctionPending_Load);
            this.gb_cavity.ResumeLayout(false);
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
        private System.Windows.Forms.Label lb_lotSize;
        private System.Windows.Forms.Label lb_recDate;
        private System.Windows.Forms.Label lb_invoice;
        private System.Windows.Forms.Label lb_mcode;
        private System.Windows.Forms.Label lb_reportNo;
        private System.Windows.Forms.GroupBox gb_cavity;
        private System.Windows.Forms.PictureBox picbox_cavity;
        private System.Windows.Forms.GroupBox gb_material;
        private System.Windows.Forms.PictureBox picbox_mat;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox picbox_func;
        private Bunifu.Framework.UI.BunifuFlatButton tb_record;
        private System.Windows.Forms.Label lb_sampName;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_function;
    }
}
