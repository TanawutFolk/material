namespace RawMat.Views.RegularCheck
{
    partial class FormRegularReportStamp
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lb_top = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pb_stamp = new System.Windows.Forms.PictureBox();
            this.bt_cancel = new System.Windows.Forms.Button();
            this.bt_ok = new System.Windows.Forms.Button();
            this.tb_programStamp = new System.Windows.Forms.TextBox();
            this.bt_browse = new System.Windows.Forms.Button();
            this.bt_copy = new System.Windows.Forms.Button();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_stamp)).BeginInit();
            this.SuspendLayout();
            // 
            // lb_top
            // 
            this.lb_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_top.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_top.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_top.Location = new System.Drawing.Point(0, 0);
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(815, 38);
            this.lb_top.TabIndex = 32;
            this.lb_top.Text = "Regular Report";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pb_stamp);
            this.panel1.Controls.Add(this.bt_cancel);
            this.panel1.Controls.Add(this.bt_ok);
            this.panel1.Controls.Add(this.tb_programStamp);
            this.panel1.Controls.Add(this.bt_browse);
            this.panel1.Controls.Add(this.bt_copy);
            this.panel1.Controls.Add(this.lb_reportNo);
            this.panel1.Location = new System.Drawing.Point(126, 57);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(558, 246);
            this.panel1.TabIndex = 34;
            // 
            // pb_stamp
            // 
            this.pb_stamp.Location = new System.Drawing.Point(55, 14);
            this.pb_stamp.Name = "pb_stamp";
            this.pb_stamp.Size = new System.Drawing.Size(137, 148);
            this.pb_stamp.TabIndex = 40;
            this.pb_stamp.TabStop = false;
            // 
            // bt_cancel
            // 
            this.bt_cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bt_cancel.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_cancel.Location = new System.Drawing.Point(340, 207);
            this.bt_cancel.Name = "bt_cancel";
            this.bt_cancel.Size = new System.Drawing.Size(144, 36);
            this.bt_cancel.TabIndex = 39;
            this.bt_cancel.Text = "Cancel";
            this.bt_cancel.UseVisualStyleBackColor = true;
            this.bt_cancel.Click += new System.EventHandler(this.bt_cancel_Click);
            // 
            // bt_ok
            // 
            this.bt_ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bt_ok.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_ok.Location = new System.Drawing.Point(76, 207);
            this.bt_ok.Name = "bt_ok";
            this.bt_ok.Size = new System.Drawing.Size(144, 36);
            this.bt_ok.TabIndex = 38;
            this.bt_ok.Text = "OK";
            this.bt_ok.UseVisualStyleBackColor = true;
            this.bt_ok.Click += new System.EventHandler(this.bt_ok_Click);
            // 
            // tb_programStamp
            // 
            this.tb_programStamp.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_programStamp.Location = new System.Drawing.Point(14, 168);
            this.tb_programStamp.Name = "tb_programStamp";
            this.tb_programStamp.ReadOnly = true;
            this.tb_programStamp.Size = new System.Drawing.Size(334, 33);
            this.tb_programStamp.TabIndex = 37;
            // 
            // bt_browse
            // 
            this.bt_browse.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bt_browse.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_browse.Location = new System.Drawing.Point(393, 168);
            this.bt_browse.Name = "bt_browse";
            this.bt_browse.Size = new System.Drawing.Size(144, 33);
            this.bt_browse.TabIndex = 36;
            this.bt_browse.Text = "Browse";
            this.bt_browse.UseVisualStyleBackColor = true;
            this.bt_browse.Click += new System.EventHandler(this.bt_browse_Click);
            // 
            // bt_copy
            // 
            this.bt_copy.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bt_copy.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_copy.Location = new System.Drawing.Point(393, 126);
            this.bt_copy.Name = "bt_copy";
            this.bt_copy.Size = new System.Drawing.Size(144, 36);
            this.bt_copy.TabIndex = 35;
            this.bt_copy.Text = "Copy";
            this.bt_copy.UseVisualStyleBackColor = true;
            this.bt_copy.Click += new System.EventHandler(this.bt_copy_Click);
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(219, 132);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(129, 25);
            this.lb_reportNo.TabIndex = 34;
            this.lb_reportNo.Text = "RI0001-0001";
            // 
            // FormRegularReportStamp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.PaleTurquoise;
            this.ClientSize = new System.Drawing.Size(815, 315);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lb_top);
            this.Name = "FormRegularReportStamp";
            this.Text = "Regular Report Stamp";
            this.Load += new System.EventHandler(this.FormRegularReportStamp_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_stamp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button bt_copy;
        private System.Windows.Forms.Label lb_reportNo;
        private System.Windows.Forms.TextBox tb_programStamp;
        private System.Windows.Forms.Button bt_browse;
        private System.Windows.Forms.Button bt_cancel;
        private System.Windows.Forms.Button bt_ok;
        private System.Windows.Forms.PictureBox pb_stamp;
    }
}