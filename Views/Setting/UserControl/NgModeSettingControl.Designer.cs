namespace RawMat.Views.Setting.UserControl
{
    partial class NgModeSettingControl
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            this.btnClear = new Bunifu.Framework.UI.BunifuFlatButton();
            this.panel3 = new System.Windows.Forms.Panel();
            this.dtgEmployeeSetting = new Bunifu.Framework.UI.BunifuCustomDataGrid();
            this.btnAddNewNgMode = new Bunifu.Framework.UI.BunifuFlatButton();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearch = new Bunifu.Framework.UI.BunifuFlatButton();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.txtNgModeSearch = new Bunifu.Framework.UI.BunifuMaterialTextbox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtgEmployeeSetting)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnClear
            // 
            this.btnClear.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnClear.BackColor = System.Drawing.Color.DarkGray;
            this.btnClear.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnClear.BorderRadius = 0;
            this.btnClear.ButtonText = "Clear";
            this.btnClear.DisabledColor = System.Drawing.Color.Gray;
            this.btnClear.Iconcolor = System.Drawing.Color.Transparent;
            this.btnClear.Iconimage = null;
            this.btnClear.Iconimage_right = null;
            this.btnClear.Iconimage_right_Selected = null;
            this.btnClear.Iconimage_Selected = null;
            this.btnClear.IconMarginLeft = 0;
            this.btnClear.IconMarginRight = 0;
            this.btnClear.IconRightVisible = true;
            this.btnClear.IconRightZoom = 0D;
            this.btnClear.IconVisible = true;
            this.btnClear.IconZoom = 90D;
            this.btnClear.IsTab = false;
            this.btnClear.Location = new System.Drawing.Point(679, 36);
            this.btnClear.Name = "btnClear";
            this.btnClear.Normalcolor = System.Drawing.Color.DarkGray;
            this.btnClear.OnHovercolor = System.Drawing.Color.Pink;
            this.btnClear.OnHoverTextColor = System.Drawing.Color.Red;
            this.btnClear.selected = false;
            this.btnClear.Size = new System.Drawing.Size(82, 27);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "Clear";
            this.btnClear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnClear.Textcolor = System.Drawing.Color.White;
            this.btnClear.TextFont = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.White;
            this.panel3.Controls.Add(this.dtgEmployeeSetting);
            this.panel3.Location = new System.Drawing.Point(3, 130);
            this.panel3.Margin = new System.Windows.Forms.Padding(2);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1066, 590);
            this.panel3.TabIndex = 11;
            // 
            // dtgEmployeeSetting
            // 
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.dtgEmployeeSetting.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            this.dtgEmployeeSetting.BackgroundColor = System.Drawing.Color.Gainsboro;
            this.dtgEmployeeSetting.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dtgEmployeeSetting.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.SeaGreen;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Century Gothic", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.SeaGreen;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtgEmployeeSetting.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.dtgEmployeeSetting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dtgEmployeeSetting.DoubleBuffered = true;
            this.dtgEmployeeSetting.EnableHeadersVisualStyles = false;
            this.dtgEmployeeSetting.HeaderBgColor = System.Drawing.Color.SeaGreen;
            this.dtgEmployeeSetting.HeaderForeColor = System.Drawing.Color.SeaGreen;
            this.dtgEmployeeSetting.Location = new System.Drawing.Point(10, 8);
            this.dtgEmployeeSetting.Margin = new System.Windows.Forms.Padding(2);
            this.dtgEmployeeSetting.Name = "dtgEmployeeSetting";
            this.dtgEmployeeSetting.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Microsoft YaHei", 10.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dtgEmployeeSetting.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dtgEmployeeSetting.RowHeadersVisible = false;
            this.dtgEmployeeSetting.RowTemplate.Height = 24;
            this.dtgEmployeeSetting.Size = new System.Drawing.Size(1046, 570);
            this.dtgEmployeeSetting.TabIndex = 0;
            // 
            // btnAddNewNgMode
            // 
            this.btnAddNewNgMode.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnAddNewNgMode.BackColor = System.Drawing.Color.Pink;
            this.btnAddNewNgMode.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnAddNewNgMode.BorderRadius = 0;
            this.btnAddNewNgMode.ButtonText = "+ Add New Ng Mode";
            this.btnAddNewNgMode.DisabledColor = System.Drawing.Color.Gray;
            this.btnAddNewNgMode.Iconcolor = System.Drawing.Color.Transparent;
            this.btnAddNewNgMode.Iconimage = null;
            this.btnAddNewNgMode.Iconimage_right = null;
            this.btnAddNewNgMode.Iconimage_right_Selected = null;
            this.btnAddNewNgMode.Iconimage_Selected = null;
            this.btnAddNewNgMode.IconMarginLeft = 0;
            this.btnAddNewNgMode.IconMarginRight = 0;
            this.btnAddNewNgMode.IconRightVisible = true;
            this.btnAddNewNgMode.IconRightZoom = 0D;
            this.btnAddNewNgMode.IconVisible = true;
            this.btnAddNewNgMode.IconZoom = 90D;
            this.btnAddNewNgMode.IsTab = false;
            this.btnAddNewNgMode.Location = new System.Drawing.Point(856, 11);
            this.btnAddNewNgMode.Name = "btnAddNewNgMode";
            this.btnAddNewNgMode.Normalcolor = System.Drawing.Color.Pink;
            this.btnAddNewNgMode.OnHovercolor = System.Drawing.Color.Pink;
            this.btnAddNewNgMode.OnHoverTextColor = System.Drawing.Color.Red;
            this.btnAddNewNgMode.selected = false;
            this.btnAddNewNgMode.Size = new System.Drawing.Size(200, 27);
            this.btnAddNewNgMode.TabIndex = 8;
            this.btnAddNewNgMode.Text = "+ Add New Ng Mode";
            this.btnAddNewNgMode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnAddNewNgMode.Textcolor = System.Drawing.Color.White;
            this.btnAddNewNgMode.TextFont = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(421, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(180, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "NG Mode Setting";
            // 
            // btnSearch
            // 
            this.btnSearch.Activecolor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(139)))), ((int)(((byte)(87)))));
            this.btnSearch.BackColor = System.Drawing.Color.Red;
            this.btnSearch.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnSearch.BorderRadius = 0;
            this.btnSearch.ButtonText = "Search";
            this.btnSearch.DisabledColor = System.Drawing.Color.Gray;
            this.btnSearch.Iconcolor = System.Drawing.Color.Transparent;
            this.btnSearch.Iconimage = null;
            this.btnSearch.Iconimage_right = null;
            this.btnSearch.Iconimage_right_Selected = null;
            this.btnSearch.Iconimage_Selected = null;
            this.btnSearch.IconMarginLeft = 0;
            this.btnSearch.IconMarginRight = 0;
            this.btnSearch.IconRightVisible = true;
            this.btnSearch.IconRightZoom = 0D;
            this.btnSearch.IconVisible = true;
            this.btnSearch.IconZoom = 90D;
            this.btnSearch.IsTab = false;
            this.btnSearch.Location = new System.Drawing.Point(519, 36);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Normalcolor = System.Drawing.Color.Red;
            this.btnSearch.OnHovercolor = System.Drawing.Color.Pink;
            this.btnSearch.OnHoverTextColor = System.Drawing.Color.Red;
            this.btnSearch.selected = false;
            this.btnSearch.Size = new System.Drawing.Size(133, 27);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.btnSearch.Textcolor = System.Drawing.Color.White;
            this.btnSearch.TextFont = new System.Drawing.Font("Microsoft YaHei", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.White;
            this.panel2.Controls.Add(this.btnAddNewNgMode);
            this.panel2.Controls.Add(this.btnClear);
            this.panel2.Controls.Add(this.btnSearch);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.txtNgModeSearch);
            this.panel2.Location = new System.Drawing.Point(3, 42);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1066, 84);
            this.panel2.TabIndex = 10;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft YaHei", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(13, 36);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(105, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "NG Mode";
            // 
            // txtNgModeSearch
            // 
            this.txtNgModeSearch.Font = new System.Drawing.Font("Century Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNgModeSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.txtNgModeSearch.HintForeColor = System.Drawing.Color.Empty;
            this.txtNgModeSearch.HintText = "NG Mode name . . .";
            this.txtNgModeSearch.isPassword = false;
            this.txtNgModeSearch.LineFocusedColor = System.Drawing.Color.Red;
            this.txtNgModeSearch.LineIdleColor = System.Drawing.Color.Gray;
            this.txtNgModeSearch.LineMouseHoverColor = System.Drawing.Color.Pink;
            this.txtNgModeSearch.LineThickness = 3;
            this.txtNgModeSearch.Location = new System.Drawing.Point(125, 28);
            this.txtNgModeSearch.Margin = new System.Windows.Forms.Padding(4);
            this.txtNgModeSearch.Name = "txtNgModeSearch";
            this.txtNgModeSearch.Size = new System.Drawing.Size(200, 35);
            this.txtNgModeSearch.TabIndex = 0;
            this.txtNgModeSearch.Text = "equipment name . . .";
            this.txtNgModeSearch.TextAlign = System.Windows.Forms.HorizontalAlignment.Left;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(3, 6);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1066, 32);
            this.panel1.TabIndex = 9;
            // 
            // NgModeSettingControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "NgModeSettingControl";
            this.Size = new System.Drawing.Size(1073, 726);
            this.panel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dtgEmployeeSetting)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Bunifu.Framework.UI.BunifuFlatButton btnClear;
        private System.Windows.Forms.Panel panel3;
        private Bunifu.Framework.UI.BunifuCustomDataGrid dtgEmployeeSetting;
        private Bunifu.Framework.UI.BunifuFlatButton btnAddNewNgMode;
        private System.Windows.Forms.Label label1;
        private Bunifu.Framework.UI.BunifuFlatButton btnSearch;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private Bunifu.Framework.UI.BunifuMaterialTextbox txtNgModeSearch;
        private System.Windows.Forms.Panel panel1;
    }
}
