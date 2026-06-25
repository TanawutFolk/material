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
            this.bt_record = new System.Windows.Forms.Button();
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
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(1115, 38);
            this.lb_top.TabIndex = 33;
            this.lb_top.Text = "Appearance Check Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_inspQty
            // 
            this.lb_inspQty.AutoSize = true;
            this.lb_inspQty.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_inspQty.Location = new System.Drawing.Point(706, 82);
            this.lb_inspQty.Name = "lb_inspQty";
            this.lb_inspQty.Size = new System.Drawing.Size(381, 19);
            this.lb_inspQty.TabIndex = 84;
            this.lb_inspQty.Text = "INSPECTION QTY/คำนวนจาก db_packing_size";
            // 
            // lb_lotSize
            // 
            this.lb_lotSize.AutoSize = true;
            this.lb_lotSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotSize.Location = new System.Drawing.Point(706, 47);
            this.lb_lotSize.Name = "lb_lotSize";
            this.lb_lotSize.Size = new System.Drawing.Size(146, 19);
            this.lb_lotSize.TabIndex = 83;
            this.lb_lotSize.Text = "LOT SIZE/ทั้งหมด";
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(69, 82);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(267, 19);
            this.lb_recDate.TabIndex = 82;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(342, 47);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(216, 19);
            this.lb_invoice.TabIndex = 81;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(342, 82);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(231, 19);
            this.lb_mcode.TabIndex = 80;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(69, 47);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(216, 19);
            this.lb_reportNo.TabIndex = 79;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // picbox_Appear
            // 
            this.picbox_Appear.Image = global::RawMat.Properties.Resources.SHIN005;
            this.picbox_Appear.Location = new System.Drawing.Point(72, 332);
            this.picbox_Appear.Name = "picbox_Appear";
            this.picbox_Appear.Size = new System.Drawing.Size(997, 272);
            this.picbox_Appear.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picbox_Appear.TabIndex = 86;
            this.picbox_Appear.TabStop = false;
            // 
            // gb_cavity
            // 
            this.gb_cavity.Controls.Add(this.picbox_cavity);
            this.gb_cavity.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_cavity.Location = new System.Drawing.Point(72, 115);
            this.gb_cavity.Name = "gb_cavity";
            this.gb_cavity.Size = new System.Drawing.Size(1003, 211);
            this.gb_cavity.TabIndex = 85;
            this.gb_cavity.TabStop = false;
            this.gb_cavity.Text = "Cavity";
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
            this.dtg_ngMode.Location = new System.Drawing.Point(244, 624);
            this.dtg_ngMode.Name = "dtg_ngMode";
            this.dtg_ngMode.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dtg_ngMode.Size = new System.Drawing.Size(644, 202);
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
            //
            // bt_record
            //
            this.bt_record.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_record.ForeColor = System.Drawing.Color.DarkGreen;
            this.bt_record.Location = new System.Drawing.Point(482, 842);
            this.bt_record.Name = "bt_record";
            this.bt_record.Size = new System.Drawing.Size(172, 58);
            this.bt_record.TabIndex = 88;
            this.bt_record.Text = "Record Data";
            this.bt_record.UseVisualStyleBackColor = true;
            this.bt_record.Click += new System.EventHandler(this.bt_record_Click);
            // userControlAppearPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.bt_record);
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
            this.Name = "userControlAppearPending";
            this.Size = new System.Drawing.Size(1115, 1182);
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
        private System.Windows.Forms.Button bt_record;
    }
}
