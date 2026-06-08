namespace RawMat.Views.Setting
{
    partial class frmSetting
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.pn_SettingMenu = new System.Windows.Forms.Panel();
            this.pn_buttonSwap = new System.Windows.Forms.Panel();
            this.btnNgModeSetting = new System.Windows.Forms.Button();
            this.btnEquipmentSetting = new System.Windows.Forms.Button();
            this.btn_InspectionSetting = new System.Windows.Forms.Button();
            this.btn_EmployeeSetting = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.pn_Content = new System.Windows.Forms.Panel();
            this.pn_SettingMenu.SuspendLayout();
            this.pn_buttonSwap.SuspendLayout();
            this.SuspendLayout();
            // 
            // pn_SettingMenu
            // 
            this.pn_SettingMenu.BackColor = System.Drawing.Color.White;
            this.pn_SettingMenu.Controls.Add(this.pn_buttonSwap);
            this.pn_SettingMenu.Controls.Add(this.label4);
            this.pn_SettingMenu.Location = new System.Drawing.Point(4, 5);
            this.pn_SettingMenu.Margin = new System.Windows.Forms.Padding(2);
            this.pn_SettingMenu.Name = "pn_SettingMenu";
            this.pn_SettingMenu.Size = new System.Drawing.Size(122, 727);
            this.pn_SettingMenu.TabIndex = 1;
            // 
            // pn_buttonSwap
            // 
            this.pn_buttonSwap.Controls.Add(this.btnNgModeSetting);
            this.pn_buttonSwap.Controls.Add(this.btnEquipmentSetting);
            this.pn_buttonSwap.Controls.Add(this.btn_InspectionSetting);
            this.pn_buttonSwap.Controls.Add(this.btn_EmployeeSetting);
            this.pn_buttonSwap.Location = new System.Drawing.Point(3, 136);
            this.pn_buttonSwap.Name = "pn_buttonSwap";
            this.pn_buttonSwap.Size = new System.Drawing.Size(136, 229);
            this.pn_buttonSwap.TabIndex = 4;
            // 
            // btnNgModeSetting
            // 
            this.btnNgModeSetting.BackColor = System.Drawing.Color.White;
            this.btnNgModeSetting.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnNgModeSetting.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnNgModeSetting.Location = new System.Drawing.Point(-6, 158);
            this.btnNgModeSetting.Name = "btnNgModeSetting";
            this.btnNgModeSetting.Size = new System.Drawing.Size(130, 51);
            this.btnNgModeSetting.TabIndex = 5;
            this.btnNgModeSetting.Text = "NG Mode";
            this.btnNgModeSetting.UseVisualStyleBackColor = false;
            this.btnNgModeSetting.Click += new System.EventHandler(this.btnNgModeSetting_Click);
            // 
            // btnEquipmentSetting
            // 
            this.btnEquipmentSetting.BackColor = System.Drawing.Color.White;
            this.btnEquipmentSetting.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEquipmentSetting.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Bold);
            this.btnEquipmentSetting.Location = new System.Drawing.Point(-6, 108);
            this.btnEquipmentSetting.Name = "btnEquipmentSetting";
            this.btnEquipmentSetting.Size = new System.Drawing.Size(130, 51);
            this.btnEquipmentSetting.TabIndex = 4;
            this.btnEquipmentSetting.Text = "Equipments";
            this.btnEquipmentSetting.UseVisualStyleBackColor = false;
            this.btnEquipmentSetting.Click += new System.EventHandler(this.btnEquipmentSetting_Click);
            // 
            // btn_InspectionSetting
            // 
            this.btn_InspectionSetting.BackColor = System.Drawing.Color.Pink;
            this.btn_InspectionSetting.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_InspectionSetting.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Bold);
            this.btn_InspectionSetting.Location = new System.Drawing.Point(-7, 8);
            this.btn_InspectionSetting.Name = "btn_InspectionSetting";
            this.btn_InspectionSetting.Size = new System.Drawing.Size(130, 51);
            this.btn_InspectionSetting.TabIndex = 1;
            this.btn_InspectionSetting.Text = "M-Code Inspection ";
            this.btn_InspectionSetting.UseVisualStyleBackColor = false;
            this.btn_InspectionSetting.Click += new System.EventHandler(this.btn_InspectionSetting_Click);
            // 
            // btn_EmployeeSetting
            // 
            this.btn_EmployeeSetting.BackColor = System.Drawing.Color.White;
            this.btn_EmployeeSetting.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btn_EmployeeSetting.Font = new System.Drawing.Font("Microsoft YaHei", 11.25F, System.Drawing.FontStyle.Bold);
            this.btn_EmployeeSetting.Location = new System.Drawing.Point(-6, 58);
            this.btn_EmployeeSetting.Name = "btn_EmployeeSetting";
            this.btn_EmployeeSetting.Size = new System.Drawing.Size(130, 51);
            this.btn_EmployeeSetting.TabIndex = 3;
            this.btn_EmployeeSetting.Text = "Employees";
            this.btn_EmployeeSetting.UseVisualStyleBackColor = false;
            this.btn_EmployeeSetting.Click += new System.EventHandler(this.btn_EmployeeSetting_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft YaHei", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(20, 17);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(81, 52);
            this.label4.TabIndex = 0;
            this.label4.Text = "Setting\r\n Menu";
            // 
            // pn_Content
            // 
            this.pn_Content.Location = new System.Drawing.Point(132, 2);
            this.pn_Content.Name = "pn_Content";
            this.pn_Content.Size = new System.Drawing.Size(1073, 726);
            this.pn_Content.TabIndex = 3;
            // 
            // frmSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Pink;
            this.ClientSize = new System.Drawing.Size(1209, 735);
            this.Controls.Add(this.pn_Content);
            this.Controls.Add(this.pn_SettingMenu);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmSetting";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "M Code Inspection Setting";
            this.Load += new System.EventHandler(this.frmInspectionSetting_Load);
            this.pn_SettingMenu.ResumeLayout(false);
            this.pn_SettingMenu.PerformLayout();
            this.pn_buttonSwap.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pn_SettingMenu;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_InspectionSetting;
        private System.Windows.Forms.Panel pn_Content;
        private System.Windows.Forms.Button btn_EmployeeSetting;
        private System.Windows.Forms.Panel pn_buttonSwap;
        private System.Windows.Forms.Button btnEquipmentSetting;
        private System.Windows.Forms.Button btnNgModeSetting;
    }
}
