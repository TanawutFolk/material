using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Data;
using System.Windows.Forms;

namespace RawMat.Views.Setting.form
{
    public partial class frmEditEmployee : Form
    {
        private readonly SettingControllers _controller = new SettingControllers();
        private readonly string _employeeId;
        private readonly bool _isEditMode;

        public frmEditEmployee()
            : this("")
        {
        }

        public frmEditEmployee(string employeeId)
        {
            InitializeComponent();

            _employeeId = employeeId?.Trim() ?? "";
            _isEditMode = !string.IsNullOrWhiteSpace(_employeeId);

            Load += frmEditEmployee_Load;
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void frmEditEmployee_Load(object sender, EventArgs e)
        {
            Text = _isEditMode ? "Edit Employee" : "Add New Employee";
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            BindEmployeeLevel();

            if (_isEditMode)
            {
                LoadEmployee(_employeeId);
                txtEmpCode.Enabled = false;
            }
            else
            {
                txtEmpCode.Enabled = true;
                txtEmpCode.Focus();
            }
        }

        private void BindEmployeeLevel()
        {
            DataTable source = _controller.GetEmployeeLevelList();

            var dt = new DataTable();
            dt.Columns.Add("TEXT");
            dt.Columns.Add("VALUE");

            if (source != null)
            {
                foreach (DataRow row in source.Rows)
                {
                    dt.Rows.Add(
                        Convert.ToString(row["TEXT"]),
                        Convert.ToString(row["VALUE"]));
                }
            }

            cboEmployeeLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            cboEmployeeLevel.DataSource = dt;
            cboEmployeeLevel.DisplayMember = "TEXT";
            cboEmployeeLevel.ValueMember = "VALUE";

            if (dt.Rows.Count > 0)
                cboEmployeeLevel.SelectedIndex = 0;
        }

        private void LoadEmployee(string employeeId)
        {
            var dataItem = new SettingProperty
            {
                Employee_ID = employeeId
            };

            DataTable dt = _controller.SearchEmployeeSettingByEmployeeID(dataItem);
            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบข้อมูล Employee นี้", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.Cancel;
                Close();
                return;
            }

            DataRow row = dt.Rows[0];
            txtEmpCode.Text = Convert.ToString(row["Employee_ID"]).Trim();
            txtPhoneExt.Text = Convert.ToString(row["Phone_Ext"]).Trim();
            SetComboValue(cboEmployeeLevel, Convert.ToString(row["Employee_Level_ID"]).Trim());
        }

        private SettingProperty GetDataFromScreen()
        {
            return new SettingProperty
            {
                Employee_ID = txtEmpCode.Text.Trim(),
                Employee_Level_ID = GetComboValue(cboEmployeeLevel),
                Phone_Ext = txtPhoneExt.Text.Trim()
            };
        }

        private bool ValidateBeforeSave()
        {
            if (string.IsNullOrWhiteSpace(txtEmpCode.Text))
            {
                MessageBox.Show("กรุณาระบุ Employee Code", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmpCode.Focus();
                return false;
            }

            if (txtEmpCode.Text.Trim().Length > 20)
            {
                MessageBox.Show("Employee Code ต้องไม่เกิน 20 ตัวอักษร", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtEmpCode.Focus();
                return false;
            }

            if (cboEmployeeLevel.SelectedIndex < 0 || string.IsNullOrWhiteSpace(GetComboValue(cboEmployeeLevel)))
            {
                MessageBox.Show("กรุณาเลือก Employee Level", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cboEmployeeLevel.Focus();
                return false;
            }

            if (txtPhoneExt.Text.Trim().Length > 10)
            {
                MessageBox.Show("Phone Ext ต้องไม่เกิน 10 ตัวอักษร", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPhoneExt.Focus();
                return false;
            }

            return true;
        }

        private void SaveEmployee()
        {
            if (!ValidateBeforeSave()) return;

            using (var frm = new frmConfirm("Are you sure you want to save ?"))
            {
                if (frm.ShowDialog(this) != DialogResult.Yes) return;
            }

            bool result = _controller.SaveEmployeeSetting(GetDataFromScreen());

            if (!result)
            {
                MessageBox.Show("Save Employee Setting Failed", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Save Employee Setting", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }

        private static string GetComboValue(ComboBox cbo)
        {
            if (cbo.DataSource == null)
                return cbo.Text.Trim();

            return cbo.SelectedValue == null ? "" : cbo.SelectedValue.ToString().Trim();
        }

        private static void SetComboValue(ComboBox cbo, string value)
        {
            value = value?.Trim() ?? "";

            if (cbo.DataSource == null)
            {
                cbo.Text = value;
                return;
            }

            cbo.SelectedValue = value;
            if (cbo.SelectedIndex == -1)
                cbo.SelectedIndex = 0;
        }

        private void btnSave_Click(object sender, EventArgs e) => SaveEmployee();

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
