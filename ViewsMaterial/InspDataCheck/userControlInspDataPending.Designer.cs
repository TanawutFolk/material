namespace RawMat.ViewsMaterial.InspDataCheck
{
    partial class userControlInspDataPending
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
            this.lb_top = new System.Windows.Forms.Label();
            this.lb_lotSize = new System.Windows.Forms.Label();
            this.lb_recDate = new System.Windows.Forms.Label();
            this.lb_invoice = new System.Windows.Forms.Label();
            this.lb_mcode = new System.Windows.Forms.Label();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.panel_pdf = new System.Windows.Forms.Panel();
            this.gb_data_judge = new System.Windows.Forms.GroupBox();
            this.lb_length_detail = new System.Windows.Forms.Label();
            this.tb_data_detail = new System.Windows.Forms.TextBox();
            this.rb_ng = new System.Windows.Forms.RadioButton();
            this.rb_ok = new System.Windows.Forms.RadioButton();
            this.lb_data_judge = new System.Windows.Forms.Label();
            this.gb_data_qa_judge = new System.Windows.Forms.GroupBox();
            this.lb_length_qa_detail = new System.Windows.Forms.Label();
            this.tb_data_qa_detail = new System.Windows.Forms.TextBox();
            this.rb_qa_ng = new System.Windows.Forms.RadioButton();
            this.rb_qa_ok = new System.Windows.Forms.RadioButton();
            this.lb_data_qa_judge = new System.Windows.Forms.Label();
            this.bt_confirm = new System.Windows.Forms.Button();
            this.lb_emp_op = new System.Windows.Forms.Label();
            this.lb_insp_date = new System.Windows.Forms.Label();
            this.gb_data_judge.SuspendLayout();
            this.gb_data_qa_judge.SuspendLayout();
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
            this.lb_top.Text = "Inspection Data Check Pending";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_lotSize
            // 
            this.lb_lotSize.AutoSize = true;
            this.lb_lotSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotSize.Location = new System.Drawing.Point(623, 58);
            this.lb_lotSize.Name = "lb_lotSize";
            this.lb_lotSize.Size = new System.Drawing.Size(94, 19);
            this.lb_lotSize.TabIndex = 82;
            this.lb_lotSize.Text = "LOT SIZE :";
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(14, 96);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(267, 19);
            this.lb_recDate.TabIndex = 81;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(313, 58);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(216, 19);
            this.lb_invoice.TabIndex = 80;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(313, 96);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(231, 19);
            this.lb_mcode.TabIndex = 79;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(14, 58);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(216, 19);
            this.lb_reportNo.TabIndex = 78;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // panel_pdf
            // 
            this.panel_pdf.Location = new System.Drawing.Point(68, 129);
            this.panel_pdf.Name = "panel_pdf";
            this.panel_pdf.Size = new System.Drawing.Size(975, 631);
            this.panel_pdf.TabIndex = 83;
            // 
            // gb_data_judge
            // 
            this.gb_data_judge.Controls.Add(this.lb_length_detail);
            this.gb_data_judge.Controls.Add(this.tb_data_detail);
            this.gb_data_judge.Controls.Add(this.rb_ng);
            this.gb_data_judge.Controls.Add(this.rb_ok);
            this.gb_data_judge.Controls.Add(this.lb_data_judge);
            this.gb_data_judge.Location = new System.Drawing.Point(70, 766);
            this.gb_data_judge.Name = "gb_data_judge";
            this.gb_data_judge.Size = new System.Drawing.Size(973, 69);
            this.gb_data_judge.TabIndex = 84;
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
            // 
            // rb_ng
            // 
            this.rb_ng.AutoSize = true;
            this.rb_ng.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.rb_ng.Location = new System.Drawing.Point(519, 14);
            this.rb_ng.Name = "rb_ng";
            this.rb_ng.Size = new System.Drawing.Size(57, 27);
            this.rb_ng.TabIndex = 51;
            this.rb_ng.TabStop = true;
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
            this.rb_ok.TabStop = true;
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
            // gb_data_qa_judge
            // 
            this.gb_data_qa_judge.Controls.Add(this.lb_length_qa_detail);
            this.gb_data_qa_judge.Controls.Add(this.tb_data_qa_detail);
            this.gb_data_qa_judge.Controls.Add(this.rb_qa_ng);
            this.gb_data_qa_judge.Controls.Add(this.rb_qa_ok);
            this.gb_data_qa_judge.Controls.Add(this.lb_data_qa_judge);
            this.gb_data_qa_judge.Location = new System.Drawing.Point(70, 841);
            this.gb_data_qa_judge.Name = "gb_data_qa_judge";
            this.gb_data_qa_judge.Size = new System.Drawing.Size(973, 69);
            this.gb_data_qa_judge.TabIndex = 85;
            this.gb_data_qa_judge.TabStop = false;
            // 
            // lb_length_qa_detail
            // 
            this.lb_length_qa_detail.AutoSize = true;
            this.lb_length_qa_detail.Font = new System.Drawing.Font("Tahoma", 8.25F);
            this.lb_length_qa_detail.Location = new System.Drawing.Point(914, 45);
            this.lb_length_qa_detail.Name = "lb_length_qa_detail";
            this.lb_length_qa_detail.Size = new System.Drawing.Size(53, 13);
            this.lb_length_qa_detail.TabIndex = 53;
            this.lb_length_qa_detail.Text = "000 / 255";
            this.lb_length_qa_detail.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tb_data_qa_detail
            // 
            this.tb_data_qa_detail.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.tb_data_qa_detail.Location = new System.Drawing.Point(582, 16);
            this.tb_data_qa_detail.Name = "tb_data_qa_detail";
            this.tb_data_qa_detail.Size = new System.Drawing.Size(385, 26);
            this.tb_data_qa_detail.TabIndex = 52;
            this.tb_data_qa_detail.TextChanged += new System.EventHandler(this.tb_data_qa_detail_TextChanged);
            // 
            // rb_qa_ng
            // 
            this.rb_qa_ng.AutoSize = true;
            this.rb_qa_ng.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.rb_qa_ng.Location = new System.Drawing.Point(519, 14);
            this.rb_qa_ng.Name = "rb_qa_ng";
            this.rb_qa_ng.Size = new System.Drawing.Size(57, 27);
            this.rb_qa_ng.TabIndex = 51;
            this.rb_qa_ng.TabStop = true;
            this.rb_qa_ng.Text = "NG";
            this.rb_qa_ng.UseVisualStyleBackColor = true;
            // 
            // rb_qa_ok
            // 
            this.rb_qa_ok.AutoSize = true;
            this.rb_qa_ok.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.rb_qa_ok.Location = new System.Drawing.Point(363, 14);
            this.rb_qa_ok.Name = "rb_qa_ok";
            this.rb_qa_ok.Size = new System.Drawing.Size(56, 27);
            this.rb_qa_ok.TabIndex = 50;
            this.rb_qa_ok.TabStop = true;
            this.rb_qa_ok.Text = "OK";
            this.rb_qa_ok.UseVisualStyleBackColor = true;
            // 
            // lb_data_qa_judge
            // 
            this.lb_data_qa_judge.AutoSize = true;
            this.lb_data_qa_judge.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_data_qa_judge.ForeColor = System.Drawing.Color.Black;
            this.lb_data_qa_judge.Location = new System.Drawing.Point(6, 16);
            this.lb_data_qa_judge.Name = "lb_data_qa_judge";
            this.lb_data_qa_judge.Size = new System.Drawing.Size(139, 23);
            this.lb_data_qa_judge.TabIndex = 49;
            this.lb_data_qa_judge.Text = "QA Judgment";
            // 
            // bt_confirm
            // 
            this.bt_confirm.Font = new System.Drawing.Font("Tahoma", 14.25F, System.Drawing.FontStyle.Bold);
            this.bt_confirm.Location = new System.Drawing.Point(901, 935);
            this.bt_confirm.Name = "bt_confirm";
            this.bt_confirm.Size = new System.Drawing.Size(142, 47);
            this.bt_confirm.TabIndex = 86;
            this.bt_confirm.Text = "CONFIRM";
            this.bt_confirm.UseVisualStyleBackColor = true;
            this.bt_confirm.Click += new System.EventHandler(this.bt_confirm_Click);
            // 
            // lb_emp_op
            // 
            this.lb_emp_op.AutoSize = true;
            this.lb_emp_op.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_emp_op.Location = new System.Drawing.Point(811, 58);
            this.lb_emp_op.Name = "lb_emp_op";
            this.lb_emp_op.Size = new System.Drawing.Size(159, 19);
            this.lb_emp_op.TabIndex = 87;
            this.lb_emp_op.Text = "EMP OPERATION :";
            // 
            // lb_insp_date
            // 
            this.lb_insp_date.AutoSize = true;
            this.lb_insp_date.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_insp_date.Location = new System.Drawing.Point(623, 96);
            this.lb_insp_date.Name = "lb_insp_date";
            this.lb_insp_date.Size = new System.Drawing.Size(115, 19);
            this.lb_insp_date.TabIndex = 88;
            this.lb_insp_date.Text = "INSP. DATE :";
            // 
            // userControlInspDataPending
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Thistle;
            this.Controls.Add(this.lb_insp_date);
            this.Controls.Add(this.lb_emp_op);
            this.Controls.Add(this.bt_confirm);
            this.Controls.Add(this.gb_data_qa_judge);
            this.Controls.Add(this.gb_data_judge);
            this.Controls.Add(this.panel_pdf);
            this.Controls.Add(this.lb_lotSize);
            this.Controls.Add(this.lb_recDate);
            this.Controls.Add(this.lb_invoice);
            this.Controls.Add(this.lb_mcode);
            this.Controls.Add(this.lb_reportNo);
            this.Controls.Add(this.lb_top);
            this.Name = "userControlInspDataPending";
            this.Size = new System.Drawing.Size(1115, 1007);
            this.Load += new System.EventHandler(this.userControlInspDataPending_Load);
            this.gb_data_judge.ResumeLayout(false);
            this.gb_data_judge.PerformLayout();
            this.gb_data_qa_judge.ResumeLayout(false);
            this.gb_data_qa_judge.PerformLayout();
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
        private System.Windows.Forms.Panel panel_pdf;
        private System.Windows.Forms.GroupBox gb_data_judge;
        private System.Windows.Forms.Label lb_length_detail;
        private System.Windows.Forms.TextBox tb_data_detail;
        private System.Windows.Forms.RadioButton rb_ng;
        private System.Windows.Forms.RadioButton rb_ok;
        private System.Windows.Forms.Label lb_data_judge;
        private System.Windows.Forms.GroupBox gb_data_qa_judge;
        private System.Windows.Forms.Label lb_length_qa_detail;
        private System.Windows.Forms.TextBox tb_data_qa_detail;
        private System.Windows.Forms.RadioButton rb_qa_ng;
        private System.Windows.Forms.RadioButton rb_qa_ok;
        private System.Windows.Forms.Label lb_data_qa_judge;
        private System.Windows.Forms.Button bt_confirm;
        private System.Windows.Forms.Label lb_emp_op;
        private System.Windows.Forms.Label lb_insp_date;
    }
}
