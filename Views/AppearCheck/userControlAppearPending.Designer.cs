namespace RawMat.Views.AppearCheck
{
    partial class userControlAppearPending
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
            this.lb_top = new System.Windows.Forms.Label();
            this.lb_inspQty = new System.Windows.Forms.Label();
            this.lb_lotSize = new System.Windows.Forms.Label();
            this.lb_recDate = new System.Windows.Forms.Label();
            this.lb_invoice = new System.Windows.Forms.Label();
            this.lb_mcode = new System.Windows.Forms.Label();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.picbox_Appear = new System.Windows.Forms.PictureBox();
            this.gb_cavity = new System.Windows.Forms.GroupBox();
            this.picbox_cavity = new System.Windows.Forms.PictureBox();
            this.dtg_ngMode = new System.Windows.Forms.DataGridView();
            this.QTY_NG = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NG_MODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NOTE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.JUDGEMENT = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.RESULT = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.tb_record = new Bunifu.Framework.UI.BunifuFlatButton();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_Appear)).BeginInit();
            this.gb_cavity.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_ngMode)).BeginInit();
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
            this.lb_top.TabIndex = 33;
            this.lb_top.Text = "Appearance Check Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_inspQty
            // 
            this.lb_inspQty.AutoSize = true;
            this.lb_inspQty.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_inspQty.Location = new System.Drawing.Point(941, 101);
            this.lb_inspQty.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_inspQty.Name = "lb_inspQty";
            this.lb_inspQty.Size = new System.Drawing.Size(474, 24);
            this.lb_inspQty.TabIndex = 84;
            this.lb_inspQty.Text = "INSPECTION QTY/คำนวนจาก db_packing_size";
            // 
            // lb_lotSize
            // 
            this.lb_lotSize.AutoSize = true;
            this.lb_lotSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotSize.Location = new System.Drawing.Point(941, 58);
            this.lb_lotSize.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_lotSize.Name = "lb_lotSize";
            this.lb_lotSize.Size = new System.Drawing.Size(180, 24);
            this.lb_lotSize.TabIndex = 83;
            this.lb_lotSize.Text = "LOT SIZE/ทั้งหมด";
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(92, 101);
            this.lb_recDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(330, 24);
            this.lb_recDate.TabIndex = 82;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(456, 58);
            this.lb_invoice.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(272, 24);
            this.lb_invoice.TabIndex = 81;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(456, 101);
            this.lb_mcode.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(282, 24);
            this.lb_mcode.TabIndex = 80;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(92, 58);
            this.lb_reportNo.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(265, 24);
            this.lb_reportNo.TabIndex = 79;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // picbox_Appear
            // 
            this.picbox_Appear.Image = global::RawMat.Properties.Resources.SHIN005;
            this.picbox_Appear.Location = new System.Drawing.Point(96, 409);
            this.picbox_Appear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picbox_Appear.Name = "picbox_Appear";
            this.picbox_Appear.Size = new System.Drawing.Size(1329, 335);
            this.picbox_Appear.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_Appear.TabIndex = 86;
            this.picbox_Appear.TabStop = false;
            // 
            // gb_cavity
            // 
            this.gb_cavity.Controls.Add(this.picbox_cavity);
            this.gb_cavity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_cavity.Location = new System.Drawing.Point(96, 142);
            this.gb_cavity.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gb_cavity.Name = "gb_cavity";
            this.gb_cavity.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gb_cavity.Size = new System.Drawing.Size(1337, 260);
            this.gb_cavity.TabIndex = 85;
            this.gb_cavity.TabStop = false;
            this.gb_cavity.Text = "Cavity";
            // 
            // picbox_cavity
            // 
            this.picbox_cavity.Image = global::RawMat.Properties.Resources.Cavity;
            this.picbox_cavity.Location = new System.Drawing.Point(21, 36);
            this.picbox_cavity.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.picbox_cavity.Name = "picbox_cavity";
            this.picbox_cavity.Size = new System.Drawing.Size(1308, 217);
            this.picbox_cavity.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_cavity.TabIndex = 0;
            this.picbox_cavity.TabStop = false;
            // 
            // dtg_ngMode
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_ngMode.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dtg_ngMode.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_ngMode.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.QTY_NG,
            this.NG_MODE,
            this.NOTE,
            this.JUDGEMENT,
            this.RESULT});
            this.dtg_ngMode.Location = new System.Drawing.Point(325, 768);
            this.dtg_ngMode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtg_ngMode.Name = "dtg_ngMode";
            this.dtg_ngMode.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dtg_ngMode.Size = new System.Drawing.Size(859, 249);
            this.dtg_ngMode.TabIndex = 87;
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
            this.NG_MODE.Width = 200;
            // 
            // NOTE
            // 
            this.NOTE.HeaderText = "NOTE";
            this.NOTE.Name = "NOTE";
            this.NOTE.Width = 200;
            // 
            // JUDGEMENT
            // 
            this.JUDGEMENT.HeaderText = "JUDGE";
            this.JUDGEMENT.Name = "JUDGEMENT";
            // 
            // RESULT
            // 
            this.RESULT.HeaderText = "RESULT";
            this.RESULT.Name = "RESULT";
            this.RESULT.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.RESULT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
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
            this.tb_record.Location = new System.Drawing.Point(628, 1036);
            this.tb_record.Margin = new System.Windows.Forms.Padding(7, 5, 7, 5);
            this.tb_record.Name = "tb_record";
            this.tb_record.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.tb_record.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(36)))), ((int)(((byte)(129)))), ((int)(((byte)(77)))));
            this.tb_record.OnHoverTextColor = System.Drawing.Color.White;
            this.tb_record.selected = false;
            this.tb_record.Size = new System.Drawing.Size(249, 71);
            this.tb_record.TabIndex = 88;
            this.tb_record.Text = "Record Data";
            this.tb_record.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tb_record.Textcolor = System.Drawing.Color.Lavender;
            this.tb_record.TextFont = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // userControlAppearPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.tb_record);
            this.Controls.Add(this.dtg_ngMode);
            this.Controls.Add(this.picbox_Appear);
            this.Controls.Add(this.gb_cavity);
            this.Controls.Add(this.lb_inspQty);
            this.Controls.Add(this.lb_lotSize);
            this.Controls.Add(this.lb_recDate);
            this.Controls.Add(this.lb_invoice);
            this.Controls.Add(this.lb_mcode);
            this.Controls.Add(this.lb_reportNo);
            this.Controls.Add(this.lb_top);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "userControlAppearPending";
            this.Size = new System.Drawing.Size(1487, 1455);
            this.Load += new System.EventHandler(this.userControlAppearPending_Load);
            ((System.ComponentModel.ISupportInitialize)(this.picbox_Appear)).EndInit();
            this.gb_cavity.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picbox_cavity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_ngMode)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
        private System.Windows.Forms.Label lb_inspQty;
        private System.Windows.Forms.Label lb_lotSize;
        private System.Windows.Forms.Label lb_recDate;
        private System.Windows.Forms.Label lb_invoice;
        private System.Windows.Forms.Label lb_mcode;
        private System.Windows.Forms.Label lb_reportNo;
        private System.Windows.Forms.PictureBox picbox_Appear;
        private System.Windows.Forms.GroupBox gb_cavity;
        private System.Windows.Forms.PictureBox picbox_cavity;
        private System.Windows.Forms.DataGridView dtg_ngMode;
        private System.Windows.Forms.DataGridViewTextBoxColumn QTY_NG;
        private System.Windows.Forms.DataGridViewTextBoxColumn NG_MODE;
        private System.Windows.Forms.DataGridViewTextBoxColumn NOTE;
        private System.Windows.Forms.DataGridViewCheckBoxColumn JUDGEMENT;
        private System.Windows.Forms.DataGridViewComboBoxColumn RESULT;
        private Bunifu.Framework.UI.BunifuFlatButton tb_record;
    }
}
