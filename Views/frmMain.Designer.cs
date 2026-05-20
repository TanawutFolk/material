namespace RawMat
{
    partial class frmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.panelHeader2 = new System.Windows.Forms.Panel();
            this.bt_refresh = new Bunifu.Framework.UI.BunifuTileButton();
            this.bt_status_dimension_pending = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bt_status_function_pending = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bt_appear_pending = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bt_status_data_pending = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bt_status_packing_check_pending = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bt_status_regular_pending = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bt_status_rec_pending = new Bunifu.Framework.UI.BunifuFlatButton();
            this.bt_setting = new Bunifu.Framework.UI.BunifuTileButton();
            this.panelHeader1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.panelLogin = new System.Windows.Forms.Panel();
            this.bunifuProgressBar1 = new Bunifu.Framework.UI.BunifuProgressBar();
            this.panelWH = new System.Windows.Forms.Panel();
            this.panelMenu = new System.Windows.Forms.Panel();
            this.panelMain = new System.Windows.Forms.Panel();
            this.panelHome = new System.Windows.Forms.Panel();
            this.bt_home = new Bunifu.Framework.UI.BunifuTileButton();
            this.panelHeader2.SuspendLayout();
            this.panelHeader1.SuspendLayout();
            this.panelHome.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelHeader2
            // 
            this.panelHeader2.BackColor = System.Drawing.Color.LightPink;
            this.panelHeader2.Controls.Add(this.bt_refresh);
            this.panelHeader2.Controls.Add(this.bt_status_dimension_pending);
            this.panelHeader2.Controls.Add(this.bt_status_function_pending);
            this.panelHeader2.Controls.Add(this.bt_appear_pending);
            this.panelHeader2.Controls.Add(this.bt_status_data_pending);
            this.panelHeader2.Controls.Add(this.bt_status_packing_check_pending);
            this.panelHeader2.Controls.Add(this.bt_status_regular_pending);
            this.panelHeader2.Controls.Add(this.bt_status_rec_pending);
            this.panelHeader2.Controls.Add(this.bt_setting);
            this.panelHeader2.Location = new System.Drawing.Point(200, 62);
            this.panelHeader2.Margin = new System.Windows.Forms.Padding(4);
            this.panelHeader2.Name = "panelHeader2";
            this.panelHeader2.Size = new System.Drawing.Size(1488, 138);
            this.panelHeader2.TabIndex = 7;
            // 
            // bt_refresh
            // 
            this.bt_refresh.BackColor = System.Drawing.Color.LightPink;
            this.bt_refresh.color = System.Drawing.Color.LightPink;
            this.bt_refresh.colorActive = System.Drawing.Color.HotPink;
            this.bt_refresh.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_refresh.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_refresh.ForeColor = System.Drawing.Color.DarkRed;
            this.bt_refresh.Image = global::RawMat.Properties.Resources.refresh;
            this.bt_refresh.ImagePosition = 10;
            this.bt_refresh.ImageZoom = 40;
            this.bt_refresh.LabelPosition = 15;
            this.bt_refresh.LabelText = "Refresh";
            this.bt_refresh.Location = new System.Drawing.Point(1416, 66);
            this.bt_refresh.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.bt_refresh.Name = "bt_refresh";
            this.bt_refresh.Size = new System.Drawing.Size(61, 62);
            this.bt_refresh.TabIndex = 32;
            this.bt_refresh.Visible = false;
            this.bt_refresh.Click += new System.EventHandler(this.bt_refresh_Click);
            // 
            // bt_status_dimension_pending
            // 
            this.bt_status_dimension_pending.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_dimension_pending.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_dimension_pending.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bt_status_dimension_pending.BorderRadius = 0;
            this.bt_status_dimension_pending.ButtonText = "Dimension Pending \n0 Report";
            this.bt_status_dimension_pending.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_status_dimension_pending.DisabledColor = System.Drawing.Color.Gray;
            this.bt_status_dimension_pending.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_dimension_pending.Iconcolor = System.Drawing.Color.Transparent;
            this.bt_status_dimension_pending.Iconimage = global::RawMat.Properties.Resources.report_approve;
            this.bt_status_dimension_pending.Iconimage_right = null;
            this.bt_status_dimension_pending.Iconimage_right_Selected = null;
            this.bt_status_dimension_pending.Iconimage_Selected = null;
            this.bt_status_dimension_pending.IconMarginLeft = 6;
            this.bt_status_dimension_pending.IconMarginRight = 0;
            this.bt_status_dimension_pending.IconRightVisible = true;
            this.bt_status_dimension_pending.IconRightZoom = 40D;
            this.bt_status_dimension_pending.IconVisible = true;
            this.bt_status_dimension_pending.IconZoom = 40D;
            this.bt_status_dimension_pending.IsTab = false;
            this.bt_status_dimension_pending.Location = new System.Drawing.Point(973, 5);
            this.bt_status_dimension_pending.Margin = new System.Windows.Forms.Padding(0);
            this.bt_status_dimension_pending.Name = "bt_status_dimension_pending";
            this.bt_status_dimension_pending.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_dimension_pending.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_dimension_pending.OnHoverTextColor = System.Drawing.Color.White;
            this.bt_status_dimension_pending.selected = false;
            this.bt_status_dimension_pending.Size = new System.Drawing.Size(224, 123);
            this.bt_status_dimension_pending.TabIndex = 40;
            this.bt_status_dimension_pending.Text = "Dimension Pending \n0 Report";
            this.bt_status_dimension_pending.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_status_dimension_pending.Textcolor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(10)))), ((int)(((byte)(16)))));
            this.bt_status_dimension_pending.TextFont = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_dimension_pending.Click += new System.EventHandler(this.bt_status_dimension_pending_Click);
            // 
            // bt_status_function_pending
            // 
            this.bt_status_function_pending.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_function_pending.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_function_pending.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bt_status_function_pending.BorderRadius = 0;
            this.bt_status_function_pending.ButtonText = "Function Pending \n0 Report";
            this.bt_status_function_pending.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_status_function_pending.DisabledColor = System.Drawing.Color.Gray;
            this.bt_status_function_pending.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_function_pending.Iconcolor = System.Drawing.Color.Transparent;
            this.bt_status_function_pending.Iconimage = global::RawMat.Properties.Resources.report_approve;
            this.bt_status_function_pending.Iconimage_right = null;
            this.bt_status_function_pending.Iconimage_right_Selected = null;
            this.bt_status_function_pending.Iconimage_Selected = null;
            this.bt_status_function_pending.IconMarginLeft = 6;
            this.bt_status_function_pending.IconMarginRight = 0;
            this.bt_status_function_pending.IconRightVisible = true;
            this.bt_status_function_pending.IconRightZoom = 40D;
            this.bt_status_function_pending.IconVisible = true;
            this.bt_status_function_pending.IconZoom = 40D;
            this.bt_status_function_pending.IsTab = false;
            this.bt_status_function_pending.Location = new System.Drawing.Point(791, 5);
            this.bt_status_function_pending.Margin = new System.Windows.Forms.Padding(0);
            this.bt_status_function_pending.Name = "bt_status_function_pending";
            this.bt_status_function_pending.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_function_pending.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_function_pending.OnHoverTextColor = System.Drawing.Color.White;
            this.bt_status_function_pending.selected = false;
            this.bt_status_function_pending.Size = new System.Drawing.Size(195, 123);
            this.bt_status_function_pending.TabIndex = 35;
            this.bt_status_function_pending.Text = "Function Pending \n0 Report";
            this.bt_status_function_pending.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_status_function_pending.Textcolor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(10)))), ((int)(((byte)(16)))));
            this.bt_status_function_pending.TextFont = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_function_pending.Click += new System.EventHandler(this.bt_status_function_pending_Click);
            // 
            // bt_appear_pending
            // 
            this.bt_appear_pending.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_appear_pending.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_appear_pending.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bt_appear_pending.BorderRadius = 0;
            this.bt_appear_pending.ButtonText = "Appearance Pending \n0 report";
            this.bt_appear_pending.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_appear_pending.DisabledColor = System.Drawing.Color.Gray;
            this.bt_appear_pending.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_appear_pending.Iconcolor = System.Drawing.Color.Transparent;
            this.bt_appear_pending.Iconimage = global::RawMat.Properties.Resources.report_approve;
            this.bt_appear_pending.Iconimage_right = null;
            this.bt_appear_pending.Iconimage_right_Selected = null;
            this.bt_appear_pending.Iconimage_Selected = null;
            this.bt_appear_pending.IconMarginLeft = 6;
            this.bt_appear_pending.IconMarginRight = 0;
            this.bt_appear_pending.IconRightVisible = true;
            this.bt_appear_pending.IconRightZoom = 40D;
            this.bt_appear_pending.IconVisible = true;
            this.bt_appear_pending.IconZoom = 40D;
            this.bt_appear_pending.IsTab = false;
            this.bt_appear_pending.Location = new System.Drawing.Point(1197, 5);
            this.bt_appear_pending.Margin = new System.Windows.Forms.Padding(0);
            this.bt_appear_pending.Name = "bt_appear_pending";
            this.bt_appear_pending.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_appear_pending.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_appear_pending.OnHoverTextColor = System.Drawing.Color.White;
            this.bt_appear_pending.selected = false;
            this.bt_appear_pending.Size = new System.Drawing.Size(212, 123);
            this.bt_appear_pending.TabIndex = 39;
            this.bt_appear_pending.Text = "Appearance Pending \n0 report";
            this.bt_appear_pending.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_appear_pending.Textcolor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(10)))), ((int)(((byte)(16)))));
            this.bt_appear_pending.TextFont = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_appear_pending.Click += new System.EventHandler(this.bt_appear_pending_Click);
            // 
            // bt_status_data_pending
            // 
            this.bt_status_data_pending.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_data_pending.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_data_pending.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bt_status_data_pending.BorderRadius = 0;
            this.bt_status_data_pending.ButtonText = "Insp. Data \nPending \n0 report";
            this.bt_status_data_pending.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_status_data_pending.DisabledColor = System.Drawing.Color.Gray;
            this.bt_status_data_pending.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_data_pending.Iconcolor = System.Drawing.Color.Transparent;
            this.bt_status_data_pending.Iconimage = global::RawMat.Properties.Resources.report_approve;
            this.bt_status_data_pending.Iconimage_right = null;
            this.bt_status_data_pending.Iconimage_right_Selected = null;
            this.bt_status_data_pending.Iconimage_Selected = null;
            this.bt_status_data_pending.IconMarginLeft = 6;
            this.bt_status_data_pending.IconMarginRight = 0;
            this.bt_status_data_pending.IconRightVisible = true;
            this.bt_status_data_pending.IconRightZoom = 40D;
            this.bt_status_data_pending.IconVisible = true;
            this.bt_status_data_pending.IconZoom = 40D;
            this.bt_status_data_pending.IsTab = false;
            this.bt_status_data_pending.Location = new System.Drawing.Point(581, 5);
            this.bt_status_data_pending.Margin = new System.Windows.Forms.Padding(0);
            this.bt_status_data_pending.Name = "bt_status_data_pending";
            this.bt_status_data_pending.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_data_pending.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_data_pending.OnHoverTextColor = System.Drawing.Color.White;
            this.bt_status_data_pending.selected = false;
            this.bt_status_data_pending.Size = new System.Drawing.Size(209, 123);
            this.bt_status_data_pending.TabIndex = 38;
            this.bt_status_data_pending.Text = "Insp. Data \nPending \n0 report";
            this.bt_status_data_pending.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_status_data_pending.Textcolor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(10)))), ((int)(((byte)(16)))));
            this.bt_status_data_pending.TextFont = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_data_pending.Click += new System.EventHandler(this.bt_status_data_pending_Click);
            // 
            // bt_status_packing_check_pending
            // 
            this.bt_status_packing_check_pending.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_packing_check_pending.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_packing_check_pending.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bt_status_packing_check_pending.BorderRadius = 0;
            this.bt_status_packing_check_pending.ButtonText = "Packing Check Pending \n0 report";
            this.bt_status_packing_check_pending.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_status_packing_check_pending.DisabledColor = System.Drawing.Color.Gray;
            this.bt_status_packing_check_pending.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_packing_check_pending.Iconcolor = System.Drawing.Color.Transparent;
            this.bt_status_packing_check_pending.Iconimage = global::RawMat.Properties.Resources.report_approve;
            this.bt_status_packing_check_pending.Iconimage_right = null;
            this.bt_status_packing_check_pending.Iconimage_right_Selected = null;
            this.bt_status_packing_check_pending.Iconimage_Selected = null;
            this.bt_status_packing_check_pending.IconMarginLeft = 6;
            this.bt_status_packing_check_pending.IconMarginRight = 0;
            this.bt_status_packing_check_pending.IconRightVisible = true;
            this.bt_status_packing_check_pending.IconRightZoom = 40D;
            this.bt_status_packing_check_pending.IconVisible = true;
            this.bt_status_packing_check_pending.IconZoom = 40D;
            this.bt_status_packing_check_pending.IsTab = false;
            this.bt_status_packing_check_pending.Location = new System.Drawing.Point(183, 5);
            this.bt_status_packing_check_pending.Margin = new System.Windows.Forms.Padding(0);
            this.bt_status_packing_check_pending.Name = "bt_status_packing_check_pending";
            this.bt_status_packing_check_pending.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_packing_check_pending.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_packing_check_pending.OnHoverTextColor = System.Drawing.Color.White;
            this.bt_status_packing_check_pending.selected = false;
            this.bt_status_packing_check_pending.Size = new System.Drawing.Size(203, 123);
            this.bt_status_packing_check_pending.TabIndex = 37;
            this.bt_status_packing_check_pending.Text = "Packing Check Pending \n0 report";
            this.bt_status_packing_check_pending.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_status_packing_check_pending.Textcolor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(10)))), ((int)(((byte)(16)))));
            this.bt_status_packing_check_pending.TextFont = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_packing_check_pending.Click += new System.EventHandler(this.bt_status_packing_check_pending_Click);
            // 
            // bt_status_regular_pending
            // 
            this.bt_status_regular_pending.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_regular_pending.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_regular_pending.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bt_status_regular_pending.BorderRadius = 0;
            this.bt_status_regular_pending.ButtonText = "Regular Pending \n0 report";
            this.bt_status_regular_pending.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_status_regular_pending.DisabledColor = System.Drawing.Color.Gray;
            this.bt_status_regular_pending.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_regular_pending.Iconcolor = System.Drawing.Color.Transparent;
            this.bt_status_regular_pending.Iconimage = global::RawMat.Properties.Resources.report_approve;
            this.bt_status_regular_pending.Iconimage_right = null;
            this.bt_status_regular_pending.Iconimage_right_Selected = null;
            this.bt_status_regular_pending.Iconimage_Selected = null;
            this.bt_status_regular_pending.IconMarginLeft = 6;
            this.bt_status_regular_pending.IconMarginRight = 0;
            this.bt_status_regular_pending.IconRightVisible = true;
            this.bt_status_regular_pending.IconRightZoom = 40D;
            this.bt_status_regular_pending.IconVisible = true;
            this.bt_status_regular_pending.IconZoom = 40D;
            this.bt_status_regular_pending.IsTab = false;
            this.bt_status_regular_pending.Location = new System.Drawing.Point(385, 5);
            this.bt_status_regular_pending.Margin = new System.Windows.Forms.Padding(0);
            this.bt_status_regular_pending.Name = "bt_status_regular_pending";
            this.bt_status_regular_pending.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_regular_pending.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_regular_pending.OnHoverTextColor = System.Drawing.Color.White;
            this.bt_status_regular_pending.selected = false;
            this.bt_status_regular_pending.Size = new System.Drawing.Size(196, 123);
            this.bt_status_regular_pending.TabIndex = 36;
            this.bt_status_regular_pending.Text = "Regular Pending \n0 report";
            this.bt_status_regular_pending.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_status_regular_pending.Textcolor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(10)))), ((int)(((byte)(16)))));
            this.bt_status_regular_pending.TextFont = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_regular_pending.Click += new System.EventHandler(this.bt_status_regular_pending_Click);
            // 
            // bt_status_rec_pending
            // 
            this.bt_status_rec_pending.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_rec_pending.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_rec_pending.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.bt_status_rec_pending.BorderRadius = 0;
            this.bt_status_rec_pending.ButtonText = "Receive WH \nPending \n0 Report";
            this.bt_status_rec_pending.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_status_rec_pending.DisabledColor = System.Drawing.Color.Gray;
            this.bt_status_rec_pending.Font = new System.Drawing.Font("Tahoma", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_rec_pending.Iconcolor = System.Drawing.Color.Transparent;
            this.bt_status_rec_pending.Iconimage = global::RawMat.Properties.Resources.report_approve;
            this.bt_status_rec_pending.Iconimage_right = null;
            this.bt_status_rec_pending.Iconimage_right_Selected = null;
            this.bt_status_rec_pending.Iconimage_Selected = null;
            this.bt_status_rec_pending.IconMarginLeft = 6;
            this.bt_status_rec_pending.IconMarginRight = 0;
            this.bt_status_rec_pending.IconRightVisible = true;
            this.bt_status_rec_pending.IconRightZoom = 40D;
            this.bt_status_rec_pending.IconVisible = true;
            this.bt_status_rec_pending.IconZoom = 40D;
            this.bt_status_rec_pending.IsTab = false;
            this.bt_status_rec_pending.Location = new System.Drawing.Point(4, 5);
            this.bt_status_rec_pending.Margin = new System.Windows.Forms.Padding(0);
            this.bt_status_rec_pending.Name = "bt_status_rec_pending";
            this.bt_status_rec_pending.Normalcolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.bt_status_rec_pending.OnHovercolor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.bt_status_rec_pending.OnHoverTextColor = System.Drawing.Color.White;
            this.bt_status_rec_pending.selected = false;
            this.bt_status_rec_pending.Size = new System.Drawing.Size(197, 123);
            this.bt_status_rec_pending.TabIndex = 17;
            this.bt_status_rec_pending.Text = "Receive WH \nPending \n0 Report";
            this.bt_status_rec_pending.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bt_status_rec_pending.Textcolor = System.Drawing.Color.FromArgb(((int)(((byte)(97)))), ((int)(((byte)(10)))), ((int)(((byte)(16)))));
            this.bt_status_rec_pending.TextFont = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_status_rec_pending.Click += new System.EventHandler(this.bt_status_rec_pending_Click);
            // 
            // bt_setting
            // 
            this.bt_setting.BackColor = System.Drawing.Color.LightPink;
            this.bt_setting.color = System.Drawing.Color.LightPink;
            this.bt_setting.colorActive = System.Drawing.Color.HotPink;
            this.bt_setting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_setting.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_setting.ForeColor = System.Drawing.Color.DarkRed;
            this.bt_setting.Image = global::RawMat.Properties.Resources.setting;
            this.bt_setting.ImagePosition = 10;
            this.bt_setting.ImageZoom = 40;
            this.bt_setting.LabelPosition = 15;
            this.bt_setting.LabelText = "Settings";
            this.bt_setting.Location = new System.Drawing.Point(1416, 6);
            this.bt_setting.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.bt_setting.Name = "bt_setting";
            this.bt_setting.Size = new System.Drawing.Size(61, 62);
            this.bt_setting.TabIndex = 34;
            this.bt_setting.Visible = false;
            this.bt_setting.Click += new System.EventHandler(this.bt_setting_Click);
            // 
            // panelHeader1
            // 
            this.panelHeader1.BackColor = System.Drawing.Color.LightPink;
            this.panelHeader1.Controls.Add(this.label1);
            this.panelHeader1.Location = new System.Drawing.Point(200, 0);
            this.panelHeader1.Margin = new System.Windows.Forms.Padding(4);
            this.panelHeader1.Name = "panelHeader1";
            this.panelHeader1.Size = new System.Drawing.Size(1292, 63);
            this.panelHeader1.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.DarkRed;
            this.label1.Location = new System.Drawing.Point(352, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(780, 41);
            this.label1.TabIndex = 0;
            this.label1.Text = "MATERIAL RECEIVING INSPECTION SYSTEM";
            // 
            // panelLogin
            // 
            this.panelLogin.BackColor = System.Drawing.Color.LightPink;
            this.panelLogin.Location = new System.Drawing.Point(0, 0);
            this.panelLogin.Margin = new System.Windows.Forms.Padding(4);
            this.panelLogin.Name = "panelLogin";
            this.panelLogin.Size = new System.Drawing.Size(200, 295);
            this.panelLogin.TabIndex = 14;
            // 
            // bunifuProgressBar1
            // 
            this.bunifuProgressBar1.BackColor = System.Drawing.Color.Silver;
            this.bunifuProgressBar1.BorderRadius = 5;
            this.bunifuProgressBar1.Location = new System.Drawing.Point(-3, 1109);
            this.bunifuProgressBar1.Margin = new System.Windows.Forms.Padding(5);
            this.bunifuProgressBar1.MaximumValue = 100;
            this.bunifuProgressBar1.Name = "bunifuProgressBar1";
            this.bunifuProgressBar1.ProgressColor = System.Drawing.Color.Teal;
            this.bunifuProgressBar1.Size = new System.Drawing.Size(1691, 27);
            this.bunifuProgressBar1.TabIndex = 15;
            this.bunifuProgressBar1.Value = 0;
            // 
            // panelWH
            // 
            this.panelWH.Location = new System.Drawing.Point(0, 396);
            this.panelWH.Margin = new System.Windows.Forms.Padding(4);
            this.panelWH.Name = "panelWH";
            this.panelWH.Size = new System.Drawing.Size(200, 172);
            this.panelWH.TabIndex = 16;
            // 
            // panelMenu
            // 
            this.panelMenu.Location = new System.Drawing.Point(0, 576);
            this.panelMenu.Margin = new System.Windows.Forms.Padding(4);
            this.panelMenu.Name = "panelMenu";
            this.panelMenu.Size = new System.Drawing.Size(200, 529);
            this.panelMenu.TabIndex = 17;
            // 
            // panelMain
            // 
            this.panelMain.BackColor = System.Drawing.Color.Pink;
            this.panelMain.Location = new System.Drawing.Point(200, 199);
            this.panelMain.Margin = new System.Windows.Forms.Padding(4);
            this.panelMain.Name = "panelMain";
            this.panelMain.Size = new System.Drawing.Size(1487, 906);
            this.panelMain.TabIndex = 18;
            // 
            // panelHome
            // 
            this.panelHome.Controls.Add(this.bt_home);
            this.panelHome.Location = new System.Drawing.Point(1488, 0);
            this.panelHome.Margin = new System.Windows.Forms.Padding(4);
            this.panelHome.Name = "panelHome";
            this.panelHome.Size = new System.Drawing.Size(196, 63);
            this.panelHome.TabIndex = 17;
            // 
            // bt_home
            // 
            this.bt_home.BackColor = System.Drawing.Color.LightPink;
            this.bt_home.color = System.Drawing.Color.LightPink;
            this.bt_home.colorActive = System.Drawing.Color.HotPink;
            this.bt_home.Cursor = System.Windows.Forms.Cursors.Hand;
            this.bt_home.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bt_home.ForeColor = System.Drawing.Color.DarkRed;
            this.bt_home.Image = global::RawMat.Properties.Resources.home;
            this.bt_home.ImagePosition = 5;
            this.bt_home.ImageZoom = 15;
            this.bt_home.LabelPosition = 20;
            this.bt_home.LabelText = "Home";
            this.bt_home.Location = new System.Drawing.Point(0, 0);
            this.bt_home.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.bt_home.Name = "bt_home";
            this.bt_home.Size = new System.Drawing.Size(196, 63);
            this.bt_home.TabIndex = 24;
            this.bt_home.Click += new System.EventHandler(this.bt_home_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Pink;
            this.ClientSize = new System.Drawing.Size(1685, 1134);
            this.Controls.Add(this.panelHome);
            this.Controls.Add(this.panelMain);
            this.Controls.Add(this.panelMenu);
            this.Controls.Add(this.panelWH);
            this.Controls.Add(this.bunifuProgressBar1);
            this.Controls.Add(this.panelLogin);
            this.Controls.Add(this.panelHeader1);
            this.Controls.Add(this.panelHeader2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.Text = "Material Receiving Inspection System";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.panelHeader2.ResumeLayout(false);
            this.panelHeader1.ResumeLayout(false);
            this.panelHeader1.PerformLayout();
            this.panelHome.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel panelHeader2;
        private System.Windows.Forms.Panel panelHeader1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelLogin;
        private Bunifu.Framework.UI.BunifuProgressBar bunifuProgressBar1;
        private System.Windows.Forms.Panel panelWH;
        private System.Windows.Forms.Panel panelMenu;
        private System.Windows.Forms.Panel panelMain;
        private Bunifu.Framework.UI.BunifuFlatButton bt_status_rec_pending;
        private Bunifu.Framework.UI.BunifuTileButton bt_refresh;
        private System.Windows.Forms.Panel panelHome;
        private Bunifu.Framework.UI.BunifuTileButton bt_home;
        private Bunifu.Framework.UI.BunifuTileButton bt_setting;
        private Bunifu.Framework.UI.BunifuFlatButton bt_appear_pending;
        private Bunifu.Framework.UI.BunifuFlatButton bt_status_data_pending;
        private Bunifu.Framework.UI.BunifuFlatButton bt_status_packing_check_pending;
        private Bunifu.Framework.UI.BunifuFlatButton bt_status_regular_pending;
        private Bunifu.Framework.UI.BunifuFlatButton bt_status_function_pending;
        private Bunifu.Framework.UI.BunifuFlatButton bt_status_dimension_pending;
    }
}