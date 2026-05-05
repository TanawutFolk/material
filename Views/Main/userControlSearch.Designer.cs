namespace RawMat.Views.Main
{
    partial class userControlSearch
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
            this.dtg_receiveMatSearch = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cb_vendorSearch = new System.Windows.Forms.ComboBox();
            this.cb_repSearch = new System.Windows.Forms.ComboBox();
            this.cb_mCode = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.gb_status = new System.Windows.Forms.GroupBox();
            this.rb_statusProcess = new System.Windows.Forms.RadioButton();
            this.rb_all = new System.Windows.Forms.RadioButton();
            this.bt_export = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtp_recDateSearch = new System.Windows.Forms.DateTimePicker();
            this.rbMonthYear = new System.Windows.Forms.RadioButton();
            this.rbSpecificDate = new System.Windows.Forms.RadioButton();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_receiveMatSearch)).BeginInit();
            this.gb_status.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // dtg_receiveMatSearch
            // 
            this.dtg_receiveMatSearch.AllowUserToAddRows = false;
            this.dtg_receiveMatSearch.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_receiveMatSearch.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.dtg_receiveMatSearch.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.dtg_receiveMatSearch.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dtg_receiveMatSearch.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_receiveMatSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_receiveMatSearch.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_receiveMatSearch.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.dtg_receiveMatSearch.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            //this.dtg_receiveMatSearch.DoubleBuffered = true;
            this.dtg_receiveMatSearch.EnableHeadersVisualStyles = false;
            this.dtg_receiveMatSearch.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_receiveMatSearch.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_receiveMatSearch.Location = new System.Drawing.Point(37, 137);
            this.dtg_receiveMatSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtg_receiveMatSearch.Name = "dtg_receiveMatSearch";
            this.dtg_receiveMatSearch.ReadOnly = true;
            this.dtg_receiveMatSearch.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_receiveMatSearch.Size = new System.Drawing.Size(1413, 713);
            this.dtg_receiveMatSearch.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.label2.ForeColor = System.Drawing.Color.DarkRed;
            this.label2.Location = new System.Drawing.Point(465, 101);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 29);
            this.label2.TabIndex = 16;
            this.label2.Text = "Report No.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.label1.ForeColor = System.Drawing.Color.DarkRed;
            this.label1.Location = new System.Drawing.Point(921, 101);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 29);
            this.label1.TabIndex = 17;
            this.label1.Text = "Vendor";
            // 
            // cb_vendorSearch
            // 
            this.cb_vendorSearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_vendorSearch.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_vendorSearch.FormattingEnabled = true;
            this.cb_vendorSearch.Location = new System.Drawing.Point(1023, 96);
            this.cb_vendorSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cb_vendorSearch.Name = "cb_vendorSearch";
            this.cb_vendorSearch.Size = new System.Drawing.Size(304, 32);
            this.cb_vendorSearch.TabIndex = 27;
            this.cb_vendorSearch.TextChanged += new System.EventHandler(this.tb_vendorSearch_TextChanged);
            // 
            // cb_repSearch
            // 
            this.cb_repSearch.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_repSearch.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_repSearch.FormattingEnabled = true;
            this.cb_repSearch.Location = new System.Drawing.Point(608, 96);
            this.cb_repSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cb_repSearch.Name = "cb_repSearch";
            this.cb_repSearch.Size = new System.Drawing.Size(304, 32);
            this.cb_repSearch.TabIndex = 28;
            this.cb_repSearch.TextChanged += new System.EventHandler(this.cb_repSearch_TextChanged);
            // 
            // cb_mCode
            // 
            this.cb_mCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_mCode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cb_mCode.FormattingEnabled = true;
            this.cb_mCode.Location = new System.Drawing.Point(152, 96);
            this.cb_mCode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cb_mCode.Name = "cb_mCode";
            this.cb_mCode.Size = new System.Drawing.Size(304, 32);
            this.cb_mCode.TabIndex = 29;
            this.cb_mCode.TextChanged += new System.EventHandler(this.cb_mCode_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 14.25F);
            this.label4.ForeColor = System.Drawing.Color.DarkRed;
            this.label4.Location = new System.Drawing.Point(37, 101);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(101, 29);
            this.label4.TabIndex = 30;
            this.label4.Text = "M-CODE";
            // 
            // gb_status
            // 
            this.gb_status.Controls.Add(this.rb_statusProcess);
            this.gb_status.Controls.Add(this.rb_all);
            this.gb_status.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gb_status.Location = new System.Drawing.Point(37, 4);
            this.gb_status.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gb_status.Name = "gb_status";
            this.gb_status.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.gb_status.Size = new System.Drawing.Size(311, 66);
            this.gb_status.TabIndex = 31;
            this.gb_status.TabStop = false;
            this.gb_status.Text = "Status";
            // 
            // rb_statusProcess
            // 
            this.rb_statusProcess.AutoSize = true;
            this.rb_statusProcess.Location = new System.Drawing.Point(181, 28);
            this.rb_statusProcess.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rb_statusProcess.Name = "rb_statusProcess";
            this.rb_statusProcess.Size = new System.Drawing.Size(67, 28);
            this.rb_statusProcess.TabIndex = 1;
            this.rb_statusProcess.Text = "WIP";
            this.rb_statusProcess.UseVisualStyleBackColor = true;
            this.rb_statusProcess.CheckedChanged += new System.EventHandler(this.rb_statusProcess_CheckedChanged);
            // 
            // rb_all
            // 
            this.rb_all.AutoSize = true;
            this.rb_all.Checked = true;
            this.rb_all.Location = new System.Drawing.Point(67, 28);
            this.rb_all.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rb_all.Name = "rb_all";
            this.rb_all.Size = new System.Drawing.Size(53, 28);
            this.rb_all.TabIndex = 0;
            this.rb_all.TabStop = true;
            this.rb_all.Text = "All";
            this.rb_all.UseVisualStyleBackColor = true;
            this.rb_all.CheckedChanged += new System.EventHandler(this.rb_all_CheckedChanged);
            // 
            // bt_export
            // 
            this.bt_export.Location = new System.Drawing.Point(1223, 18);
            this.bt_export.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_export.Name = "bt_export";
            this.bt_export.Size = new System.Drawing.Size(228, 52);
            this.bt_export.TabIndex = 32;
            this.bt_export.Text = "Export .CSV";
            this.bt_export.UseVisualStyleBackColor = true;
            this.bt_export.Visible = false;
            this.bt_export.Click += new System.EventHandler(this.bt_export_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtp_recDateSearch);
            this.groupBox1.Controls.Add(this.rbMonthYear);
            this.groupBox1.Controls.Add(this.rbSpecificDate);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(356, 4);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.groupBox1.Size = new System.Drawing.Size(461, 66);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Receive Date";
            // 
            // dtp_recDateSearch
            // 
            this.dtp_recDateSearch.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtp_recDateSearch.Location = new System.Drawing.Point(192, 26);
            this.dtp_recDateSearch.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.dtp_recDateSearch.Name = "dtp_recDateSearch";
            this.dtp_recDateSearch.Size = new System.Drawing.Size(260, 32);
            this.dtp_recDateSearch.TabIndex = 34;
            this.dtp_recDateSearch.ValueChanged += new System.EventHandler(this.dtp_recDateSearch_onValueChanged);
            // 
            // rbMonthYear
            // 
            this.rbMonthYear.AutoSize = true;
            this.rbMonthYear.Location = new System.Drawing.Point(111, 28);
            this.rbMonthYear.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbMonthYear.Name = "rbMonthYear";
            this.rbMonthYear.Size = new System.Drawing.Size(66, 28);
            this.rbMonthYear.TabIndex = 1;
            this.rbMonthYear.Text = "M/Y";
            this.rbMonthYear.UseVisualStyleBackColor = true;
            this.rbMonthYear.CheckedChanged += new System.EventHandler(this.rbMonthYear_CheckedChanged);
            // 
            // rbSpecificDate
            // 
            this.rbSpecificDate.AutoSize = true;
            this.rbSpecificDate.Checked = true;
            this.rbSpecificDate.Location = new System.Drawing.Point(8, 28);
            this.rbSpecificDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.rbSpecificDate.Name = "rbSpecificDate";
            this.rbSpecificDate.Size = new System.Drawing.Size(88, 28);
            this.rbSpecificDate.TabIndex = 0;
            this.rbSpecificDate.TabStop = true;
            this.rbSpecificDate.Text = "D/M/Y";
            this.rbSpecificDate.UseVisualStyleBackColor = true;
            this.rbSpecificDate.CheckedChanged += new System.EventHandler(this.rbSpecificDate_CheckedChanged);
            // 
            // userControlSearch
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightPink;
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bt_export);
            this.Controls.Add(this.gb_status);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cb_mCode);
            this.Controls.Add(this.cb_repSearch);
            this.Controls.Add(this.cb_vendorSearch);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dtg_receiveMatSearch);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "userControlSearch";
            this.Size = new System.Drawing.Size(1487, 898);
            this.Load += new System.EventHandler(this.userControlSearch_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_receiveMatSearch)).EndInit();
            this.gb_status.ResumeLayout(false);
            this.gb_status.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_receiveMatSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cb_vendorSearch;
        private System.Windows.Forms.ComboBox cb_repSearch;
        private System.Windows.Forms.ComboBox cb_mCode;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox gb_status;
        private System.Windows.Forms.RadioButton rb_statusProcess;
        private System.Windows.Forms.RadioButton rb_all;
        private System.Windows.Forms.Button bt_export;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbMonthYear;
        private System.Windows.Forms.RadioButton rbSpecificDate;
        private System.Windows.Forms.DateTimePicker dtp_recDateSearch;
    }
}
