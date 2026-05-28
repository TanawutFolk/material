using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Data;
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
            BindEquipmentList();
            cboEquipment.Focus();
        }

        private void BindEquipmentList()
        {
            DataTable source = _controller.GetEquipmentTypeList();

            var dt = new DataTable();
            dt.Columns.Add("TEXT");
            dt.Columns.Add("VALUE");

            if (source != null)
            {
                foreach (DataRow row in source.Rows)
                {
                    dt.Rows.Add(
                        Convert.ToString(row["Equipment_Name"]),
                        Convert.ToString(row["Equipment_Type"]));
                }
            }

            cboEquipment.DropDownStyle = ComboBoxStyle.DropDown;
            cboEquipment.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cboEquipment.AutoCompleteSource = AutoCompleteSource.ListItems;
            cboEquipment.DataSource = dt;
            cboEquipment.DisplayMember = "TEXT";
            cboEquipment.ValueMember = "VALUE";
            cboEquipment.SelectedIndex = -1;
            cboEquipment.Text = "";
        }

        private bool ValidateBeforeSave()
        {
            if (string.IsNullOrWhiteSpace(cboEquipment.Text))
            {
                MessageBox.Show("กรุณาระบุ Equipment Name", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboEquipment.Focus();
                return false;
            }

            if (cboEquipment.Text.Trim().Length > 30)
            {
                MessageBox.Show("Equipment Name ต้องไม่เกิน 30 ตัวอักษร", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboEquipment.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtSerial.Text))
            {
                MessageBox.Show("กรุณาระบุ Equipment Serial", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtSerial.Focus();
                return false;
            }

            if (txtSerial.Text.Trim().Length > 255)
            {
                MessageBox.Show("Equipment Serial ต้องไม่เกิน 255 ตัวอักษร", "Warning",
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
                Equipment_Type = GetSelectedEquipmentType(),
                Equipment_Name = cboEquipment.Text.Trim(),
                Equipment_Serial = txtSerial.Text.Trim()
            };
        }

        private string GetSelectedEquipmentType()
        {
            if (cboEquipment.SelectedIndex < 0 || cboEquipment.SelectedValue == null)
                return "";

            string selectedText = Convert.ToString(cboEquipment.GetItemText(cboEquipment.SelectedItem)).Trim();
            if (!selectedText.Equals(cboEquipment.Text.Trim(), StringComparison.OrdinalIgnoreCase))
                return "";

            return cboEquipment.SelectedValue.ToString().Trim();
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
