namespace RawMat.Views.RegularCheck
{
    partial class userControlRegular
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lb_regularNo = new System.Windows.Forms.Label();
            this.dtg_regular = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.lb_top = new System.Windows.Forms.Label();
            this.lb_recDate = new System.Windows.Forms.Label();
            this.lb_invoice = new System.Windows.Forms.Label();
            this.lb_mcode = new System.Windows.Forms.Label();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.dtg_cavity = new System.Windows.Forms.DataGridView();
            this.gb_cavity = new System.Windows.Forms.GroupBox();
            this.bt_confirmCavity = new System.Windows.Forms.Button();
            this.picbox_cavity = new System.Windows.Forms.PictureBox();
            this.lb_sampName = new System.Windows.Forms.Label();
            this.lb_page = new System.Windows.Forms.Label();
            this.bt_prev = new System.Windows.Forms.Button();
            this.bt_next = new System.Windows.Forms.Button();
            this.tb_record = new Bunifu.Framework.UI.BunifuFlatButton();
            this.picbox_reg = new System.Windows.Forms.PictureBox();
            this.lb_lotNo = new System.Windows.Forms.Label();
            this.bt_back = new System.Windows.Forms.Button();
            this.cb_lotNo = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_regular)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_cavity)).BeginInit();
            this.gb_cavity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_reg)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_regularNo
            // 
            this.lb_regularNo.AutoSize = true;
            this.lb_regularNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_regularNo.ForeColor = System.Drawing.Color.Black;
            this.lb_regularNo.Location = new System.Drawing.Point(62, 46);
            this.lb_regularNo.Name = "lb_regularNo";
            this.lb_regularNo.Size = new System.Drawing.Size(166, 19);
            this.lb_regularNo.TabIndex = 13;
            this.lb_regularNo.Text = "Report Regular No.";
            // 
            // dtg_regular
            // 
            this.dtg_regular.AllowUserToAddRows = false;
            this.dtg_regular.AllowUserToDeleteRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_regular.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.dtg_regular.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dtg_regular.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtg_regular.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtg_regular.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_regular.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_regular.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.SeaGreen;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_regular.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dtg_regular.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dtg_regular.DefaultCellStyle = dataGridViewCellStyle7;
            this.dtg_regular.DoubleBuffered = true;
            this.dtg_regular.EnableHeadersVisualStyles = false;
            this.dtg_regular.HeaderBgColor = System.Drawing.Color.SeaGreen;
            this.dtg_regular.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_regular.Location = new System.Drawing.Point(17, 776);
            this.dtg_regular.Name = "dtg_regular";
            this.dtg_regular.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Tahoma", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_regular.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.dtg_regular.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dtg_regular.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dtg_regular.RowTemplate.Height = 41;
            this.dtg_regular.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dtg_regular.Size = new System.Drawing.Size(1076, 351);
            this.dtg_regular.TabIndex = 33;
            this.dtg_regular.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_regular_CellEndEdit);
            this.dtg_regular.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dtg_regular_CellValidating);
            this.dtg_regular.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_regular_CellValueChanged);
            this.dtg_regular.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.dtg_regular_DataBindingComplete);
            // 
            // lb_top
            // 
            this.lb_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_top.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_top.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_top.Location = new System.Drawing.Point(0, 0);
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(1115, 38);
            this.lb_top.TabIndex = 31;
            this.lb_top.Text = "Regular Check";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(62, 81);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(267, 19);
            this.lb_recDate.TabIndex = 46;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(580, 46);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(216, 19);
            this.lb_invoice.TabIndex = 45;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(335, 81);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(231, 19);
            this.lb_mcode.TabIndex = 44;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(331, 46);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(216, 19);
            this.lb_reportNo.TabIndex = 43;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
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
            this.dtg_cavity.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.dtg_cavity_CellParsing);
            this.dtg_cavity.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dtg_cavity_CellValidating);
            this.dtg_cavity.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dtg_cavity_EditingControlShowing);
            // 
            // gb_cavity
            // 
            this.gb_cavity.Controls.Add(this.bt_confirmCavity);
            this.gb_cavity.Controls.Add(this.dtg_cavity);
            this.gb_cavity.Controls.Add(this.picbox_cavity);
            this.gb_cavity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_cavity.Location = new System.Drawing.Point(6, 113);
            this.gb_cavity.Name = "gb_cavity";
            this.gb_cavity.Size = new System.Drawing.Size(1106, 211);
            this.gb_cavity.TabIndex = 48;
            this.gb_cavity.TabStop = false;
            this.gb_cavity.Text = "Cavity";
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
            // picbox_cavity
            // 
            this.picbox_cavity.Image = global::RawMat.Properties.Resources.Cavity;
            this.picbox_cavity.Location = new System.Drawing.Point(280, 62);
            this.picbox_cavity.Name = "picbox_cavity";
            this.picbox_cavity.Size = new System.Drawing.Size(807, 143);
            this.picbox_cavity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_cavity.TabIndex = 0;
            this.picbox_cavity.TabStop = false;
            // 
            // lb_sampName
            // 
            this.lb_sampName.AutoSize = true;
            this.lb_sampName.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_sampName.ForeColor = System.Drawing.Color.Black;
            this.lb_sampName.Location = new System.Drawing.Point(494, 688);
            this.lb_sampName.Name = "lb_sampName";
            this.lb_sampName.Size = new System.Drawing.Size(160, 23);
            this.lb_sampName.TabIndex = 48;
            this.lb_sampName.Text = "Sampling Name";
            // 
            // lb_page
            // 
            this.lb_page.AutoSize = true;
            this.lb_page.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_page.ForeColor = System.Drawing.Color.Black;
            this.lb_page.Location = new System.Drawing.Point(563, 711);
            this.lb_page.Name = "lb_page";
            this.lb_page.Size = new System.Drawing.Size(21, 23);
            this.lb_page.TabIndex = 50;
            this.lb_page.Text = "/";
            // 
            // bt_prev
            // 
            this.bt_prev.Location = new System.Drawing.Point(495, 737);
            this.bt_prev.Name = "bt_prev";
            this.bt_prev.Size = new System.Drawing.Size(70, 33);
            this.bt_prev.TabIndex = 51;
            this.bt_prev.Text = "Prev.";
            this.bt_prev.UseVisualStyleBackColor = true;
            this.bt_prev.Click += new System.EventHandler(this.bt_prev_Click);
            // 
            // bt_next
            // 
            this.bt_next.Location = new System.Drawing.Point(584, 737);
            this.bt_next.Name = "bt_next";
            this.bt_next.Size = new System.Drawing.Size(70, 33);
            this.bt_next.TabIndex = 52;
            this.bt_next.Text = "Next";
            this.bt_next.UseVisualStyleBackColor = true;
            this.bt_next.Click += new System.EventHandler(this.bt_next_Click);
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
            this.tb_record.Location = new System.Drawing.Point(473, 1134);
            this.tb_record.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tb_record.Name = "tb_record";
            this.tb_record.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.tb_record.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.tb_record.OnHoverTextColor = System.Drawing.Color.White;
            this.tb_record.selected = false;
            this.tb_record.Size = new System.Drawing.Size(187, 58);
            this.tb_record.TabIndex = 30;
            this.tb_record.Text = "Record Data";
            this.tb_record.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tb_record.Textcolor = System.Drawing.Color.Lavender;
            this.tb_record.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_record.Click += new System.EventHandler(this.tb_record_Click);
            // 
            // picbox_reg
            // 
            this.picbox_reg.Image = global::RawMat.Properties.Resources.SHIN005;
            this.picbox_reg.Location = new System.Drawing.Point(17, 330);
            this.picbox_reg.Name = "picbox_reg";
            this.picbox_reg.Size = new System.Drawing.Size(1076, 339);
            this.picbox_reg.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_reg.TabIndex = 19;
            this.picbox_reg.TabStop = false;
            // 
            // lb_lotNo
            // 
            this.lb_lotNo.AutoSize = true;
            this.lb_lotNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotNo.Location = new System.Drawing.Point(816, 46);
            this.lb_lotNo.Name = "lb_lotNo";
            this.lb_lotNo.Size = new System.Drawing.Size(83, 19);
            this.lb_lotNo.TabIndex = 53;
            this.lb_lotNo.Text = "Lot No. : ";
            // 
            // bt_back
            // 
            this.bt_back.Location = new System.Drawing.Point(6, 3);
            this.bt_back.Name = "bt_back";
            this.bt_back.Size = new System.Drawing.Size(169, 34);
            this.bt_back.TabIndex = 54;
            this.bt_back.Text = "Back";
            this.bt_back.UseVisualStyleBackColor = true;
            this.bt_back.Click += new System.EventHandler(this.bt_back_Click);
            // 
            // cb_lotNo
            // 
            this.cb_lotNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_lotNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold);
            this.cb_lotNo.FormattingEnabled = true;
            this.cb_lotNo.Location = new System.Drawing.Point(905, 38);
            this.cb_lotNo.Name = "cb_lotNo";
            this.cb_lotNo.Size = new System.Drawing.Size(204, 27);
            this.cb_lotNo.TabIndex = 71;
            // 
            // userControlRegular
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.Controls.Add(this.cb_lotNo);
            this.Controls.Add(this.bt_back);
            this.Controls.Add(this.lb_lotNo);
            this.Controls.Add(this.bt_next);
            this.Controls.Add(this.lb_sampName);
            this.Controls.Add(this.bt_prev);
            this.Controls.Add(this.lb_page);
            this.Controls.Add(this.gb_cavity);
            this.Controls.Add(this.lb_recDate);
            this.Controls.Add(this.lb_invoice);
            this.Controls.Add(this.lb_mcode);
            this.Controls.Add(this.lb_reportNo);
            this.Controls.Add(this.lb_top);
            this.Controls.Add(this.tb_record);
            this.Controls.Add(this.dtg_regular);
            this.Controls.Add(this.picbox_reg);
            this.Controls.Add(this.lb_regularNo);
            this.Name = "userControlRegular";
            this.Size = new System.Drawing.Size(1115, 1196);
            this.Load += new System.EventHandler(this.userControlRegular_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_regular)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_cavity)).EndInit();
            this.gb_cavity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_reg)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_regularNo;
        private System.Windows.Forms.PictureBox picbox_cavity;
        private System.Windows.Forms.PictureBox picbox_reg;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_regular;
        private Bunifu.Framework.UI.BunifuFlatButton tb_record;
        private System.Windows.Forms.Label lb_top;
        private System.Windows.Forms.Label lb_recDate;
        private System.Windows.Forms.Label lb_invoice;
        private System.Windows.Forms.Label lb_mcode;
        private System.Windows.Forms.Label lb_reportNo;
        private System.Windows.Forms.DataGridView dtg_cavity;
        private System.Windows.Forms.GroupBox gb_cavity;
        private System.Windows.Forms.Label lb_sampName;
        private System.Windows.Forms.Button bt_confirmCavity;
        private System.Windows.Forms.Label lb_page;
        private System.Windows.Forms.Button bt_prev;
        private System.Windows.Forms.Button bt_next;
        private System.Windows.Forms.Label lb_lotNo;
        private System.Windows.Forms.Button bt_back;
        private System.Windows.Forms.ComboBox cb_lotNo;
    }
}
