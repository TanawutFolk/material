namespace RawMat.ViewsMaterial.ReceiveWH
{
    partial class userControlCheckSheet
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(userControlCheckSheet));
            this.dtp_recDate = new Bunifu.Framework.UI.BunifuDatepicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dtg_receiveMat = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.M_CODE = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.INVOICE_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PART_NAME = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.VENDOR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GR_QTY = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.STATUS = new System.Windows.Forms.DataGridViewImageColumn();
            this.lb_top = new System.Windows.Forms.Label();
            this.bt_okCheckSheet = new System.Windows.Forms.Button();
            this.picLoading = new System.Windows.Forms.PictureBox();
            this.pgbOkSearch = new System.Windows.Forms.ProgressBar();
            this.lb_update = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_receiveMat)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).BeginInit();
            this.SuspendLayout();
            // 
            // dtp_recDate
            // 
            this.dtp_recDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.dtp_recDate.BorderRadius = 0;
            this.dtp_recDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.dtp_recDate.ForeColor = System.Drawing.Color.LavenderBlush;
            this.dtp_recDate.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dtp_recDate.FormatCustom = null;
            this.dtp_recDate.Location = new System.Drawing.Point(162, 99);
            this.dtp_recDate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dtp_recDate.Name = "dtp_recDate";
            this.dtp_recDate.Size = new System.Drawing.Size(211, 50);
            this.dtp_recDate.TabIndex = 19;
            this.dtp_recDate.Value = new System.DateTime(2024, 9, 19, 12, 9, 58, 27);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 124);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 25);
            this.label1.TabIndex = 18;
            this.label1.Text = "Receive Date";
            // 
            // dtg_receiveMat
            // 
            this.dtg_receiveMat.AllowUserToAddRows = false;
            this.dtg_receiveMat.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_receiveMat.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dtg_receiveMat.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dtg_receiveMat.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_receiveMat.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_receiveMat.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_receiveMat.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dtg_receiveMat.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_receiveMat.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.M_CODE,
            this.INVOICE_NO,
            this.PART_NAME,
            this.VENDOR,
            this.GR_QTY,
            this.STATUS});
            this.dtg_receiveMat.DoubleBuffered = true;
            this.dtg_receiveMat.EnableHeadersVisualStyles = false;
            this.dtg_receiveMat.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_receiveMat.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_receiveMat.Location = new System.Drawing.Point(17, 191);
            this.dtg_receiveMat.Name = "dtg_receiveMat";
            this.dtg_receiveMat.ReadOnly = true;
            this.dtg_receiveMat.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_receiveMat.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_receiveMat.Size = new System.Drawing.Size(1081, 523);
            this.dtg_receiveMat.TabIndex = 20;
            this.dtg_receiveMat.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_receiveMat_CellClick);
            // 
            // M_CODE
            // 
            this.M_CODE.FillWeight = 14F;
            this.M_CODE.HeaderText = "M-CODE";
            this.M_CODE.MinimumWidth = 100;
            this.M_CODE.Name = "M_CODE";
            this.M_CODE.ReadOnly = true;
            this.M_CODE.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // INVOICE_NO
            // 
            this.INVOICE_NO.FillWeight = 14F;
            this.INVOICE_NO.HeaderText = "Invoice No.";
            this.INVOICE_NO.MinimumWidth = 40;
            this.INVOICE_NO.Name = "INVOICE_NO";
            this.INVOICE_NO.ReadOnly = true;
            this.INVOICE_NO.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // PART_NAME
            // 
            this.PART_NAME.FillWeight = 25F;
            this.PART_NAME.HeaderText = "Part Name";
            this.PART_NAME.MinimumWidth = 160;
            this.PART_NAME.Name = "PART_NAME";
            this.PART_NAME.ReadOnly = true;
            this.PART_NAME.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // VENDOR
            // 
            this.VENDOR.FillWeight = 20F;
            this.VENDOR.HeaderText = "Vendor";
            this.VENDOR.MinimumWidth = 140;
            this.VENDOR.Name = "VENDOR";
            this.VENDOR.ReadOnly = true;
            this.VENDOR.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // GR_QTY
            // 
            this.GR_QTY.FillWeight = 10F;
            this.GR_QTY.HeaderText = "Qty";
            this.GR_QTY.MinimumWidth = 75;
            this.GR_QTY.Name = "GR_QTY";
            this.GR_QTY.ReadOnly = true;
            this.GR_QTY.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // STATUS
            // 
            this.STATUS.FillWeight = 15F;
            this.STATUS.HeaderText = "STATUS";
            this.STATUS.MinimumWidth = 100;
            this.STATUS.Name = "STATUS";
            this.STATUS.ReadOnly = true;
            this.STATUS.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // lb_top
            // 
            this.lb_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_top.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold);
            this.lb_top.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_top.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lb_top.Location = new System.Drawing.Point(0, 0);
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(1115, 40);
            this.lb_top.TabIndex = 22;
            this.lb_top.Text = "Receive WH Issue Check Sheet";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // bt_okCheckSheet
            // 
            this.bt_okCheckSheet.BackColor = System.Drawing.Color.Lime;
            this.bt_okCheckSheet.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bt_okCheckSheet.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_okCheckSheet.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bt_okCheckSheet.Location = new System.Drawing.Point(406, 88);
            this.bt_okCheckSheet.Name = "bt_okCheckSheet";
            this.bt_okCheckSheet.Size = new System.Drawing.Size(135, 76);
            this.bt_okCheckSheet.TabIndex = 23;
            this.bt_okCheckSheet.Text = "OK";
            this.bt_okCheckSheet.UseVisualStyleBackColor = false;
            this.bt_okCheckSheet.Click += new System.EventHandler(this.bt_okCheckSheet_Click);
            // 
            // picLoading
            // 
            this.picLoading.Image = ((System.Drawing.Image)(resources.GetObject("picLoading.Image")));
            this.picLoading.Location = new System.Drawing.Point(669, 43);
            this.picLoading.Name = "picLoading";
            this.picLoading.Size = new System.Drawing.Size(114, 124);
            this.picLoading.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLoading.TabIndex = 24;
            this.picLoading.TabStop = false;
            this.picLoading.Visible = false;
            // 
            // pgbOkSearch
            // 
            this.pgbOkSearch.Location = new System.Drawing.Point(17, 178);
            this.pgbOkSearch.MarqueeAnimationSpeed = 0;
            this.pgbOkSearch.Name = "pgbOkSearch";
            this.pgbOkSearch.Size = new System.Drawing.Size(1081, 12);
            this.pgbOkSearch.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.pgbOkSearch.TabIndex = 25;
            this.pgbOkSearch.Visible = false;
            // 
            // lb_update
            // 
            this.lb_update.AutoSize = true;
            this.lb_update.BackColor = System.Drawing.Color.Navy;
            this.lb_update.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_update.ForeColor = System.Drawing.Color.White;
            this.lb_update.Location = new System.Drawing.Point(906, 136);
            this.lb_update.Name = "lb_update";
            this.lb_update.Size = new System.Drawing.Size(189, 20);
            this.lb_update.TabIndex = 26;
            this.lb_update.Text = "Last update To day 00:00";
            // 
            // userControlCheckSheet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightPink;
            this.Controls.Add(this.lb_update);
            this.Controls.Add(this.pgbOkSearch);
            this.Controls.Add(this.picLoading);
            this.Controls.Add(this.bt_okCheckSheet);
            this.Controls.Add(this.lb_top);
            this.Controls.Add(this.dtg_receiveMat);
            this.Controls.Add(this.dtp_recDate);
            this.Controls.Add(this.label1);
            this.Name = "userControlCheckSheet";
            this.Size = new System.Drawing.Size(1115, 730);
            this.Load += new System.EventHandler(this.userControlCheckSheet_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_receiveMat)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.picLoading)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Bunifu.Framework.UI.BunifuDatepicker dtp_recDate;
        private System.Windows.Forms.Label label1;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_receiveMat;
        private System.Windows.Forms.Label lb_top;
        private System.Windows.Forms.DataGridViewTextBoxColumn M_CODE;
        private System.Windows.Forms.DataGridViewTextBoxColumn INVOICE_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn PART_NAME;
        private System.Windows.Forms.DataGridViewTextBoxColumn VENDOR;
        private System.Windows.Forms.DataGridViewTextBoxColumn GR_QTY;
        private System.Windows.Forms.DataGridViewImageColumn STATUS;
        private System.Windows.Forms.Button bt_okCheckSheet;
        private System.Windows.Forms.PictureBox picLoading;
        private System.Windows.Forms.ProgressBar pgbOkSearch;
        private System.Windows.Forms.Label lb_update;
    }
}
