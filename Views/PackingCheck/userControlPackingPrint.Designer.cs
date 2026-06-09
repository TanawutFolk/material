namespace RawMat.Views.PackingCheck
{
    partial class userControlPackingPrint
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(userControlPackingPrint));
            this.lb_top = new System.Windows.Forms.Label();
            this.lb_recDate = new System.Windows.Forms.Label();
            this.lb_invoice = new System.Windows.Forms.Label();
            this.lb_lotSize = new System.Windows.Forms.Label();
            this.lb_mcode = new System.Windows.Forms.Label();
            this.lb_reportNo = new System.Windows.Forms.Label();
            this.tlp_methods = new System.Windows.Forms.TableLayoutPanel();
            this.lb_method4 = new System.Windows.Forms.Label();
            this.pb_packing4 = new System.Windows.Forms.PictureBox();
            this.bt_print = new System.Windows.Forms.Button();
            this.lblPrinterName = new System.Windows.Forms.Label();
            this.bt_back = new Bunifu.Framework.UI.BunifuFlatButton();
            this.tlp_methods.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_packing4)).BeginInit();
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
            this.lb_top.TabIndex = 29;
            this.lb_top.Text = "Packing Check";
            this.lb_top.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // lb_recDate
            // 
            this.lb_recDate.AutoSize = true;
            this.lb_recDate.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_recDate.Location = new System.Drawing.Point(35, 77);
            this.lb_recDate.Name = "lb_recDate";
            this.lb_recDate.Size = new System.Drawing.Size(267, 19);
            this.lb_recDate.TabIndex = 47;
            this.lb_recDate.Text = "RECIEVE DATE : XXXXXXXXXXX";
            // 
            // lb_invoice
            // 
            this.lb_invoice.AutoSize = true;
            this.lb_invoice.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_invoice.Location = new System.Drawing.Point(433, 77);
            this.lb_invoice.Name = "lb_invoice";
            this.lb_invoice.Size = new System.Drawing.Size(216, 19);
            this.lb_invoice.TabIndex = 46;
            this.lb_invoice.Text = "INVOICE : XXXXXXXXXXX";
            // 
            // lb_lotSize
            // 
            this.lb_lotSize.AutoSize = true;
            this.lb_lotSize.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_lotSize.Location = new System.Drawing.Point(765, 77);
            this.lb_lotSize.Name = "lb_lotSize";
            this.lb_lotSize.Size = new System.Drawing.Size(83, 19);
            this.lb_lotSize.TabIndex = 45;
            this.lb_lotSize.Text = "LOT SIZE";
            // 
            // lb_mcode
            // 
            this.lb_mcode.AutoSize = true;
            this.lb_mcode.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_mcode.Location = new System.Drawing.Point(433, 38);
            this.lb_mcode.Name = "lb_mcode";
            this.lb_mcode.Size = new System.Drawing.Size(231, 19);
            this.lb_mcode.TabIndex = 44;
            this.lb_mcode.Text = "M-CODE : MATERIAL NAME";
            // 
            // lb_reportNo
            // 
            this.lb_reportNo.AutoSize = true;
            this.lb_reportNo.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_reportNo.Location = new System.Drawing.Point(35, 38);
            this.lb_reportNo.Name = "lb_reportNo";
            this.lb_reportNo.Size = new System.Drawing.Size(216, 19);
            this.lb_reportNo.TabIndex = 43;
            this.lb_reportNo.Text = "Report No : QAYY-XXXXX";
            // 
            // tlp_methods
            // 
            this.tlp_methods.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.tlp_methods.ColumnCount = 1;
            this.tlp_methods.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlp_methods.Controls.Add(this.lb_method4, 0, 0);
            this.tlp_methods.Controls.Add(this.pb_packing4, 0, 1);
            this.tlp_methods.Location = new System.Drawing.Point(39, 109);
            this.tlp_methods.Name = "tlp_methods";
            this.tlp_methods.RowCount = 2;
            this.tlp_methods.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 3.878116F));
            this.tlp_methods.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 27.28532F));
            this.tlp_methods.Size = new System.Drawing.Size(714, 451);
            this.tlp_methods.TabIndex = 48;
            // 
            // lb_method4
            // 
            this.lb_method4.AutoSize = true;
            this.lb_method4.BackColor = System.Drawing.Color.Aquamarine;
            this.lb_method4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lb_method4.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb_method4.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lb_method4.Location = new System.Drawing.Point(4, 1);
            this.lb_method4.Name = "lb_method4";
            this.lb_method4.Size = new System.Drawing.Size(706, 55);
            this.lb_method4.TabIndex = 1;
            this.lb_method4.Text = "lb_method4";
            // 
            // pb_packing4
            // 
            this.pb_packing4.Image = global::RawMat.Properties.Resources.no_photo;
            this.pb_packing4.Location = new System.Drawing.Point(4, 60);
            this.pb_packing4.Name = "pb_packing4";
            this.pb_packing4.Size = new System.Drawing.Size(706, 387);
            this.pb_packing4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_packing4.TabIndex = 10;
            this.pb_packing4.TabStop = false;
            // 
            // bt_print
            // 
            this.bt_print.Location = new System.Drawing.Point(769, 483);
            this.bt_print.Name = "bt_print";
            this.bt_print.Size = new System.Drawing.Size(175, 73);
            this.bt_print.TabIndex = 51;
            this.bt_print.Text = "PRINT";
            this.bt_print.UseVisualStyleBackColor = true;
            this.bt_print.Click += new System.EventHandler(this.bt_print_Click);
            // 
            // lblPrinterName
            // 
            this.lblPrinterName.AutoSize = true;
            this.lblPrinterName.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrinterName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(251)))), ((int)(((byte)(28)))), ((int)(((byte)(177)))));
            this.lblPrinterName.ImageAlign = System.Drawing.ContentAlignment.TopLeft;
            this.lblPrinterName.Location = new System.Drawing.Point(766, 38);
            this.lblPrinterName.Name = "lblPrinterName";
            this.lblPrinterName.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.lblPrinterName.Size = new System.Drawing.Size(95, 18);
            this.lblPrinterName.TabIndex = 52;
            this.lblPrinterName.Text = "Printer Name";
            this.lblPrinterName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bt_back
            // 
            this.bt_back.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.bt_back.BackColor = System.Drawing.Color.Goldenrod;
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
            this.bt_back.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.bt_back.Name = "bt_back";
            this.bt_back.Normalcolor = System.Drawing.Color.Goldenrod;
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
            // userControlPackingPrint
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.LemonChiffon;
            this.Controls.Add(this.bt_back);
            this.Controls.Add(this.lblPrinterName);
            this.Controls.Add(this.bt_print);
            this.Controls.Add(this.tlp_methods);
            this.Controls.Add(this.lb_recDate);
            this.Controls.Add(this.lb_invoice);
            this.Controls.Add(this.lb_lotSize);
            this.Controls.Add(this.lb_mcode);
            this.Controls.Add(this.lb_reportNo);
            this.Controls.Add(this.lb_top);
            this.Name = "userControlPackingPrint";
            this.Size = new System.Drawing.Size(1115, 888);
            this.Load += new System.EventHandler(this.userControlPackingPrint_Load);
            this.tlp_methods.ResumeLayout(false);
            this.tlp_methods.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_packing4)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lb_top;
        private System.Windows.Forms.Label lb_recDate;
        private System.Windows.Forms.Label lb_invoice;
        private System.Windows.Forms.Label lb_lotSize;
        private System.Windows.Forms.Label lb_mcode;
        private System.Windows.Forms.Label lb_reportNo;
        private System.Windows.Forms.TableLayoutPanel tlp_methods;
        private System.Windows.Forms.Label lb_method4;
        private System.Windows.Forms.PictureBox pb_packing4;
        private System.Windows.Forms.Button bt_print;
        private System.Windows.Forms.Label lblPrinterName;
        private Bunifu.Framework.UI.BunifuFlatButton bt_back;
    }
}
