namespace RawMat.Views.InspDataCheck
{
    partial class userControlInspData
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(userControlInspData));
            this.lb_top = new System.Windows.Forms.Label();
            this.lb_lotSize = new System.Windows.Forms.Label();
            this.lb_recDate = new System.Windows.Forms.Label();
            this.lb_invoice = new System.Windows.Forms.Label();
            this.lb_mcode = new System.Windows.Forms.Label();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.gb_data_judge = new System.Windows.Forms.GroupBox();
            this.lb_length_detail = new System.Windows.Forms.Label();
            this.tb_data_detail = new System.Windows.Forms.TextBox();
            this.rb_ng = new System.Windows.Forms.RadioButton();
            this.rb_ok = new System.Windows.Forms.RadioButton();
            this.lb_data_judge = new System.Windows.Forms.Label();
            this.bt_confirm = new System.Windows.Forms.Button();
            this.panel_pdf = new System.Windows.Forms.Panel();
            this.bt_back = new Bunifu.Framework.UI.BunifuFlatButton();
            this.gb_data_judge.SuspendLayout();
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
            this.lb_top.Text = "Inspection Data Check";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_lotSize
            // 
            this.lb_lotSize.AutoSize = true;
            this.lb_lotSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotSize.Location = new System.Drawing.Point(732, 58);
            this.lb_lotSize.Name = "lb_lotSize";
            this.lb_lotSize.Size = new System.Drawing.Size(83, 19);
            this.lb_lotSize.TabIndex = 77;
            this.lb_lotSize.Text = "LOT SIZE";
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(76, 93);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(267, 19);
            this.lb_recDate.TabIndex = 76;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(349, 58);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(216, 19);
            this.lb_invoice.TabIndex = 75;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(349, 93);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(231, 19);
            this.lb_mcode.TabIndex = 74;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(76, 58);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(216, 19);
            this.lb_reportNo.TabIndex = 73;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // gb_data_judge
            // 
            this.gb_data_judge.Controls.Add(this.lb_length_detail);
            this.gb_data_judge.Controls.Add(this.tb_data_detail);
            this.gb_data_judge.Controls.Add(this.rb_ng);
            this.gb_data_judge.Controls.Add(this.rb_ok);
            this.gb_data_judge.Controls.Add(this.lb_data_judge);
            this.gb_data_judge.Location = new System.Drawing.Point(80, 801);
            this.gb_data_judge.Name = "gb_data_judge";
            this.gb_data_judge.Size = new System.Drawing.Size(973, 69);
            this.gb_data_judge.TabIndex = 79;
            this.gb_data_judge.TabStop = false;
            // 
            // lb_length_detail
            // 
            this.lb_length_detail.AutoSize = true;
            this.lb_length_detail.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lb_length_detail.Location = new System.Drawing.Point(914, 45);
            this.lb_length_detail.Name = "lb_length_detail";
            this.lb_length_detail.Size = new System.Drawing.Size(53, 13);
            this.lb_length_detail.TabIndex = 53;
            this.lb_length_detail.Text = "000 / 255";
            this.lb_length_detail.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tb_data_detail
            // 
            this.tb_data_detail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.tb_data_detail.Location = new System.Drawing.Point(582, 16);
            this.tb_data_detail.Name = "tb_data_detail";
            this.tb_data_detail.Size = new System.Drawing.Size(385, 26);
            this.tb_data_detail.TabIndex = 52;
            this.tb_data_detail.TextChanged += new System.EventHandler(this.tb_detail_TextChanged);
            // 
            // rb_ng
            // 
            this.rb_ng.AutoSize = true;
            this.rb_ng.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.rb_ng.Location = new System.Drawing.Point(519, 14);
            this.rb_ng.Name = "rb_ng";
            this.rb_ng.Size = new System.Drawing.Size(57, 27);
            this.rb_ng.TabIndex = 51;
            this.rb_ng.Text = "NG";
            this.rb_ng.UseVisualStyleBackColor = true;
            // 
            // rb_ok
            // 
            this.rb_ok.AutoSize = true;
            this.rb_ok.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.rb_ok.Location = new System.Drawing.Point(363, 14);
            this.rb_ok.Name = "rb_ok";
            this.rb_ok.Size = new System.Drawing.Size(56, 27);
            this.rb_ok.TabIndex = 50;
            this.rb_ok.Text = "OK";
            this.rb_ok.UseVisualStyleBackColor = true;
            // 
            // lb_data_judge
            // 
            this.lb_data_judge.AutoSize = true;
            this.lb_data_judge.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_data_judge.ForeColor = System.Drawing.Color.Black;
            this.lb_data_judge.Location = new System.Drawing.Point(6, 16);
            this.lb_data_judge.Name = "lb_data_judge";
            this.lb_data_judge.Size = new System.Drawing.Size(263, 23);
            this.lb_data_judge.TabIndex = 49;
            this.lb_data_judge.Text = "Data Inspection Judgment";
            // 
            // bt_confirm
            // 
            this.bt_confirm.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.bt_confirm.Location = new System.Drawing.Point(911, 900);
            this.bt_confirm.Name = "bt_confirm";
            this.bt_confirm.Size = new System.Drawing.Size(142, 47);
            this.bt_confirm.TabIndex = 80;
            this.bt_confirm.Text = "CONFIRM";
            this.bt_confirm.UseVisualStyleBackColor = true;
            this.bt_confirm.Click += new System.EventHandler(this.bt_confirm_Click);
            // 
            // panel_pdf
            // 
            this.panel_pdf.Location = new System.Drawing.Point(78, 134);
            this.panel_pdf.Name = "panel_pdf";
            this.panel_pdf.Size = new System.Drawing.Size(975, 631);
            this.panel_pdf.TabIndex = 81;
            // 
            // bt_back
            // 
            this.bt_back.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.bt_back.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
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
            this.bt_back.Name = "bt_back";
            this.bt_back.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
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
            // userControlInspData
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Thistle;
            this.Controls.Add(this.bt_back);
            this.Controls.Add(this.panel_pdf);
            this.Controls.Add(this.bt_confirm);
            this.Controls.Add(this.gb_data_judge);
            this.Controls.Add(this.lb_lotSize);
            this.Controls.Add(this.lb_recDate);
            this.Controls.Add(this.lb_invoice);
            this.Controls.Add(this.lb_mcode);
            this.Controls.Add(this.lb_reportNo);
            this.Controls.Add(this.lb_top);
            this.Name = "userControlInspData";
            this.Size = new System.Drawing.Size(1115, 1007);
            this.Load += new System.EventHandler(this.userControlData_Load);
            this.gb_data_judge.ResumeLayout(false);
            this.gb_data_judge.PerformLayout();
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
        private System.Windows.Forms.GroupBox gb_data_judge;
        private System.Windows.Forms.Label lb_data_judge;
        private System.Windows.Forms.RadioButton rb_ng;
        private System.Windows.Forms.RadioButton rb_ok;
        private System.Windows.Forms.TextBox tb_data_detail;
        private System.Windows.Forms.Button bt_confirm;
        private System.Windows.Forms.Panel panel_pdf;
        private System.Windows.Forms.Label lb_length_detail;
        private Bunifu.Framework.UI.BunifuFlatButton bt_back;
    }
}
