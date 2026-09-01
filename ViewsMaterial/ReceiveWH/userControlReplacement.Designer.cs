namespace RawMat.ViewsMaterial.ReceiveMat
{
    partial class userControlReplacement
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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_inv_no = new Bunifu.Framework.UI.BunifuMaterialTextbox();
            this.tb_mcode = new Bunifu.Framework.UI.BunifuMaterialTextbox();
            this.tb_qty = new Bunifu.Framework.UI.BunifuMaterialTextbox();
            this.dtg_receiveMatRep = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.dtp_recDate = new Bunifu.Framework.UI.BunifuDatepicker();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_matName = new WindowsFormsControlLibrary1.BunifuCustomTextbox();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_vendor = new WindowsFormsControlLibrary1.BunifuCustomTextbox();
            this.bt_okRep = new System.Windows.Forms.Button();
            this.lb_top = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dtg_receiveMatRep)).BeginInit();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label3.Location = new System.Drawing.Point(6, 114);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(119, 25);
            this.label3.TabIndex = 3;
            this.label3.Text = "Invoice No.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label4.Location = new System.Drawing.Point(384, 63);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(88, 25);
            this.label4.TabIndex = 4;
            this.label4.Text = "M-Code";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label5.Location = new System.Drawing.Point(384, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 25);
            this.label5.TabIndex = 5;
            this.label5.Text = "Q\'Ty";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label6.Location = new System.Drawing.Point(542, 114);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(81, 25);
            this.label6.TabIndex = 6;
            this.label6.Text = "Vendor";
            // 
            // tb_inv_no
            // 
            this.tb_inv_no.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tb_inv_no.Enabled = false;
            this.tb_inv_no.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.tb_inv_no.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tb_inv_no.HintForeColor = System.Drawing.Color.Empty;
            this.tb_inv_no.HintText = "";
            this.tb_inv_no.isPassword = false;
            this.tb_inv_no.LineFocusedColor = System.Drawing.Color.Blue;
            this.tb_inv_no.LineIdleColor = System.Drawing.Color.Gray;
            this.tb_inv_no.LineMouseHoverColor = System.Drawing.Color.Blue;
            this.tb_inv_no.LineThickness = 4;
            this.tb_inv_no.Location = new System.Drawing.Point(156, 110);
            this.tb_inv_no.Margin = new System.Windows.Forms.Padding(4);
            this.tb_inv_no.Name = "tb_inv_no";
            this.tb_inv_no.Size = new System.Drawing.Size(147, 29);
            this.tb_inv_no.TabIndex = 8;
            this.tb_inv_no.Text = "Replacement";
            this.tb_inv_no.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // tb_mcode
            // 
            this.tb_mcode.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tb_mcode.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.tb_mcode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tb_mcode.HintForeColor = System.Drawing.Color.Empty;
            this.tb_mcode.HintText = "";
            this.tb_mcode.isPassword = false;
            this.tb_mcode.LineFocusedColor = System.Drawing.Color.Blue;
            this.tb_mcode.LineIdleColor = System.Drawing.Color.Gray;
            this.tb_mcode.LineMouseHoverColor = System.Drawing.Color.Blue;
            this.tb_mcode.LineThickness = 4;
            this.tb_mcode.Location = new System.Drawing.Point(479, 59);
            this.tb_mcode.Margin = new System.Windows.Forms.Padding(4);
            this.tb_mcode.Name = "tb_mcode";
            this.tb_mcode.Size = new System.Drawing.Size(214, 29);
            this.tb_mcode.TabIndex = 9;
            this.tb_mcode.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.tb_mcode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tb_mcode_KeyUp);
            // 
            // tb_qty
            // 
            this.tb_qty.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.tb_qty.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.tb_qty.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.tb_qty.HintForeColor = System.Drawing.Color.Empty;
            this.tb_qty.HintText = "";
            this.tb_qty.isPassword = false;
            this.tb_qty.LineFocusedColor = System.Drawing.Color.Blue;
            this.tb_qty.LineIdleColor = System.Drawing.Color.Gray;
            this.tb_qty.LineMouseHoverColor = System.Drawing.Color.Blue;
            this.tb_qty.LineThickness = 4;
            this.tb_qty.Location = new System.Drawing.Point(447, 110);
            this.tb_qty.Margin = new System.Windows.Forms.Padding(4);
            this.tb_qty.Name = "tb_qty";
            this.tb_qty.Size = new System.Drawing.Size(88, 29);
            this.tb_qty.TabIndex = 10;
            this.tb_qty.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            this.tb_qty.OnValueChanged += new System.EventHandler(this.tb_qty_OnValueChanged);
            this.tb_qty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_qty_KeyPress);
            // 
            // dtg_receiveMatRep
            // 
            this.dtg_receiveMatRep.AllowUserToAddRows = false;
            this.dtg_receiveMatRep.AllowUserToDeleteRows = false;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtg_receiveMatRep.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle3;
            this.dtg_receiveMatRep.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtg_receiveMatRep.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtg_receiveMatRep.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.DarkGreen;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.Cyan;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtg_receiveMatRep.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.dtg_receiveMatRep.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtg_receiveMatRep.DoubleBuffered = true;
            this.dtg_receiveMatRep.EnableHeadersVisualStyles = false;
            this.dtg_receiveMatRep.HeaderBgColor = System.Drawing.Color.DarkGreen;
            this.dtg_receiveMatRep.HeaderForeColor = System.Drawing.Color.Cyan;
            this.dtg_receiveMatRep.Location = new System.Drawing.Point(54, 160);
            this.dtg_receiveMatRep.Name = "dtg_receiveMatRep";
            this.dtg_receiveMatRep.ReadOnly = true;
            this.dtg_receiveMatRep.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dtg_receiveMatRep.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dtg_receiveMatRep.Size = new System.Drawing.Size(983, 428);
            this.dtg_receiveMatRep.TabIndex = 17;
            this.dtg_receiveMatRep.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dtg_receiveMatRep_CellClick);
            // 
            // dtp_recDate
            // 
            this.dtp_recDate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(103)))), ((int)(((byte)(92)))));
            this.dtp_recDate.BorderRadius = 0;
            this.dtp_recDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.dtp_recDate.ForeColor = System.Drawing.Color.LavenderBlush;
            this.dtp_recDate.Format = System.Windows.Forms.DateTimePickerFormat.Long;
            this.dtp_recDate.FormatCustom = null;
            this.dtp_recDate.Location = new System.Drawing.Point(156, 55);
            this.dtp_recDate.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.dtp_recDate.Name = "dtp_recDate";
            this.dtp_recDate.Size = new System.Drawing.Size(219, 38);
            this.dtp_recDate.TabIndex = 21;
            this.dtp_recDate.Value = new System.DateTime(2024, 9, 19, 12, 9, 58, 27);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(6, 63);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 25);
            this.label1.TabIndex = 20;
            this.label1.Text = "Receive Date";
            // 
            // tb_matName
            // 
            this.tb_matName.BorderColor = System.Drawing.Color.SeaGreen;
            this.tb_matName.Location = new System.Drawing.Point(857, 63);
            this.tb_matName.Multiline = true;
            this.tb_matName.Name = "tb_matName";
            this.tb_matName.ReadOnly = true;
            this.tb_matName.Size = new System.Drawing.Size(231, 28);
            this.tb_matName.TabIndex = 23;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F);
            this.label2.Location = new System.Drawing.Point(700, 63);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(151, 25);
            this.label2.TabIndex = 24;
            this.label2.Text = "Material Name";
            // 
            // tb_vendor
            // 
            this.tb_vendor.BorderColor = System.Drawing.Color.SeaGreen;
            this.tb_vendor.Location = new System.Drawing.Point(629, 110);
            this.tb_vendor.Multiline = true;
            this.tb_vendor.Name = "tb_vendor";
            this.tb_vendor.ReadOnly = true;
            this.tb_vendor.Size = new System.Drawing.Size(236, 29);
            this.tb_vendor.TabIndex = 25;
            // 
            // bt_okRep
            // 
            this.bt_okRep.BackColor = System.Drawing.Color.Lime;
            this.bt_okRep.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.bt_okRep.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_okRep.Location = new System.Drawing.Point(908, 97);
            this.bt_okRep.Name = "bt_okRep";
            this.bt_okRep.Size = new System.Drawing.Size(129, 57);
            this.bt_okRep.TabIndex = 26;
            this.bt_okRep.Text = "OK";
            this.bt_okRep.UseVisualStyleBackColor = false;
            this.bt_okRep.Click += new System.EventHandler(this.bt_okRep_Click);
            // 
            // lb_top
            // 
            this.lb_top.Dock = System.Windows.Forms.DockStyle.Top;
            this.lb_top.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold);
            this.lb_top.ForeColor = System.Drawing.Color.DarkRed;
            this.lb_top.Location = new System.Drawing.Point(0, 0);
            this.lb_top.Name = "lb_top";
            this.lb_top.Size = new System.Drawing.Size(1115, 38);
            this.lb_top.TabIndex = 27;
            this.lb_top.Text = "Receive WH Issue Replacement";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // userControlReplacement
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightPink;
            this.Controls.Add(this.lb_top);
            this.Controls.Add(this.bt_okRep);
            this.Controls.Add(this.tb_vendor);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tb_matName);
            this.Controls.Add(this.dtp_recDate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtg_receiveMatRep);
            this.Controls.Add(this.tb_qty);
            this.Controls.Add(this.tb_mcode);
            this.Controls.Add(this.tb_inv_no);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "userControlReplacement";
            this.Size = new System.Drawing.Size(1115, 600);
            this.Load += new System.EventHandler(this.userControlReplacement_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dtg_receiveMatRep)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private Bunifu.Framework.UI.BunifuMaterialTextbox tb_inv_no;
        private Bunifu.Framework.UI.BunifuMaterialTextbox tb_mcode;
        private Bunifu.Framework.UI.BunifuMaterialTextbox tb_qty;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtg_receiveMatRep;
        private Bunifu.Framework.UI.BunifuDatepicker dtp_recDate;
        private System.Windows.Forms.Label label1;
        private WindowsFormsControlLibrary1.BunifuCustomTextbox tb_matName;
        private System.Windows.Forms.Label label2;
        private WindowsFormsControlLibrary1.BunifuCustomTextbox tb_vendor;
        private System.Windows.Forms.Button bt_okRep;
        private System.Windows.Forms.Label lb_top;
    }
}
