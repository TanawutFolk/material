using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Windows.Forms;

namespace RawMat.Views.Setting.form
{
    public partial class frmAddNewNgMode : Form
    {
        private readonly SettingControllers _controller = new SettingControllers();

        public frmAddNewNgMode()
        {
            InitializeComponent();

            Load += frmAddNewNgMode_Load;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void frmAddNewNgMode_Load(object sender, EventArgs e)
        {
            Text = "Add New NG Mode";
            StartPosition = FormStartPosition.CenterParent;
            txtSerial.Focus();
        }

        private bool ValidateBeforeSave()
        {
            if (string.IsNullOrWhiteSpace(txtSerial.Text))
            {
                MessageBox.Show("Please input NG Mode", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSerial.Focus();
                return false;
            }

            if (txtSerial.Text.Trim().Length > 100)
            {
                MessageBox.Show("NG Mode must not exceed 100 characters", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSerial.Focus();
                return false;
            }

            return true;
        }

        private SettingProperty GetDataFromScreen()
        {
            return new SettingProperty
            {
                NG_Mode = txtSerial.Text.Trim()
            };
        }

        private void SaveNgMode()
        {
            if (!ValidateBeforeSave()) return;

            using (var frm = new frmConfirm("Are you sure you want to save ?"))
            {
                if (frm.ShowDialog(this) != DialogResult.Yes) return;
            }

            bool result = _controller.SaveNgModeSetting(GetDataFromScreen());

            if (!result)
            {
                MessageBox.Show("Save NG Mode Setting Failed", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Save NG Mode Setting", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e) => SaveNgMode();

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
