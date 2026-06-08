using System;
using System.Drawing;
using System.Windows.Forms;

namespace RawMat.Views.Setting
{
    public partial class frmSetting : Form
    {
        private static readonly Color ActiveButtonColor = Color.Pink;
        private static readonly Color InactiveButtonColor = Color.White;
        private static readonly Color ActiveTextColor = Color.Black;
        private static readonly Color InactiveTextColor = Color.Black;

        public frmSetting()
        {
            InitializeComponent();
        }

        private void frmInspectionSetting_Load(object sender, EventArgs e)
        {
            ShowInspectionSetting();
        }

        private void ShowInspectionSetting()
        {
            ShowContent(new InspectionSettingControl());
            SetActiveButton(btn_InspectionSetting);
        }

        private void ShowEmployeeSetting()
        {
            ShowContent(new EmployeeSettingControl());
            SetActiveButton(btn_EmployeeSetting);
        }

        private void ShowEquipmentSetting()
        {
            ShowContent(new EqupmentSetingControl());
            SetActiveButton(btnEquipmentSetting);
        }

        private void ShowNgModeSetting()
        {
            ShowContent(new RawMat.Views.Setting.UserControl.NgModeSettingControl());
            SetActiveButton(btnNgModeSetting);
        }

        private void ShowContent(System.Windows.Forms.UserControl control)
        {
            pn_Content.SuspendLayout();
            try
            {
                foreach (Control oldControl in pn_Content.Controls)
                {
                    oldControl.Dispose();
                }

                pn_Content.Controls.Clear();
                control.Dock = DockStyle.Fill;
                pn_Content.Controls.Add(control);
                control.BringToFront();
                control.Show();
            }
            finally
            {
                pn_Content.ResumeLayout();
            }
        }

        private void SetActiveButton(Button selectedButton)
        {
            foreach (Control control in pn_buttonSwap.Controls)
            {
                if (control is Button button)
                {
                    bool isActive = button == selectedButton;
                    button.BackColor = isActive ? ActiveButtonColor : InactiveButtonColor;
                    button.ForeColor = isActive ? ActiveTextColor : InactiveTextColor;
                    button.UseVisualStyleBackColor = false;
                }
            }
        }

        private void btn_InspectionSetting_Click(object sender, EventArgs e) => ShowInspectionSetting();
        private void btn_EmployeeSetting_Click(object sender, EventArgs e) => ShowEmployeeSetting();
        private void btnEquipmentSetting_Click(object sender, EventArgs e) => ShowEquipmentSetting();
        private void btnNgModeSetting_Click(object sender, EventArgs e) => ShowNgModeSetting();
    }
}
