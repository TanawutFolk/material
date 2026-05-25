using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Windows.Forms;

namespace RawMat.Views.Setting.form
{
    public partial class frmAddEquipment : Form
    {
        private readonly SettingControllers _controller = new SettingControllers();

        public frmAddEquipment()
        {
            InitializeComponent();

            Load += frmAddEquipment_Load;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void frmAddEquipment_Load(object sender, EventArgs e)
        {
            Text = "Add New Equipment";
            StartPosition = FormStartPosition.CenterParent;
            txtEquipmentName.Focus();
        }

        private bool ValidateBeforeSave()
        {
            if (string.IsNullOrWhiteSpace(txtEquipmentName.Text))
            {
                MessageBox.Show("กรุณาระบุ Equipment Name", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEquipmentName.Focus();
                return false;
            }

            if (txtEquipmentName.Text.Trim().Length > 30)
            {
                MessageBox.Show("Equipment Name ต้องไม่เกิน 30 ตัวอักษร", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEquipmentName.Focus();
                return false;
            }

            return true;
        }

        private SettingProperty GetDataFromScreen()
        {
            return new SettingProperty
            {
                Equipment_Name = txtEquipmentName.Text.Trim()
            };
        }

        private void SaveEquipment()
        {
            if (!ValidateBeforeSave()) return;

            using (var frm = new frmConfirm("Are you sure you want to save ?"))
            {
                if (frm.ShowDialog(this) != DialogResult.Yes) return;
            }

            bool result = _controller.SaveEquipmentTypeSetting(GetDataFromScreen());

            if (!result)
            {
                MessageBox.Show("Save Equipment Setting Failed", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Save Equipment Setting", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e) => SaveEquipment();

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
