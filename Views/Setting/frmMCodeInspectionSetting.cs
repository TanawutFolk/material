using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Views.Setting
{
    public partial class frmMCodeInspectionSetting : Form
    {
        SettingControllers _controller = new SettingControllers();

        private string _mCode = "";
        private bool _isEditMode = false;

        public frmMCodeInspectionSetting()
        {
            InitializeComponent();
        }
        public frmMCodeInspectionSetting(string mCode)
        {
            InitializeComponent();

            _mCode = mCode;
            _isEditMode = true;
        }
        private void frmMCodeInspectionSetting_Load(object sender, EventArgs e)
        {
            BindCombo();
            ClearInput();

            if (_isEditMode)
            {
                LoadInspectionSettingByMCode(_mCode);
                txtMCode.Enabled = false;
            }
            else
            {
                txtMCode.Enabled = true;
                txtMCode.Focus();
            }
        }
        private void BindCombo()
        {
            BindYesNoCombo(cboKeepData);
            BindPackingCheckCombo();
            BindYesNoCombo(cboRegularCheck);
            BindYesNoCombo(cboFunctionCheck);
            BindYesNoCombo(cboDimensionCheck);
            BindYesNoCombo(cboAppearanceCheck);
        }
        private void BindYesNoCombo(ComboBox cbo)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TEXT");
            dt.Columns.Add("VALUE");

            dt.Rows.Add("No", "0");
            dt.Rows.Add("Yes", "1");

            cbo.DataSource = dt;
            cbo.DisplayMember = "TEXT";
            cbo.ValueMember = "VALUE";
            cbo.SelectedValue = "0";
        }
        private void BindPackingCheckCombo()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("TEXT");
            dt.Columns.Add("VALUE");

            dt.Rows.Add("No", "0");
            dt.Rows.Add("Yes", "1");

            cboPackingCheck.DataSource = dt;
            cboPackingCheck.DisplayMember = "TEXT";
            cboPackingCheck.ValueMember = "VALUE";
            cboPackingCheck.SelectedValue = "0";
        }
        private void ClearInput()
        {
            txtMCode.Text = "";

            SetComboValue(cboKeepData, "0");
            SetComboValue(cboPackingCheck, "0");
            SetComboValue(cboRegularCheck, "0");
            SetComboValue(cboFunctionCheck, "0");
            SetComboValue(cboDimensionCheck, "0");
            SetComboValue(cboAppearanceCheck, "0");
        }
        private void LoadInspectionSettingByMCode(string mCode)
        {
            if (string.IsNullOrWhiteSpace(mCode))
            {
                MessageBox.Show("ไม่พบ M Code ที่ต้องการแก้ไข", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SettingProperty dataItem = new SettingProperty();
            dataItem.M_CODE = mCode.Trim();

            DataTable dt = _controller.SearchInspectionSettingByMCode(dataItem);

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบข้อมูล Inspection Setting ของ M Code นี้", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DataRow row = dt.Rows[0];

            txtMCode.Text = mCode;

            SetComboValue(cboKeepData, row["Keep Data"].ToString());
            SetComboValue(cboPackingCheck, row["Packing Check"].ToString());
            SetComboValue(cboRegularCheck, row["Regular Check"].ToString());
            SetComboValue(cboFunctionCheck, row["Function Check"].ToString());
            SetComboValue(cboDimensionCheck, row["Dimension Check"].ToString());
            SetComboValue(cboAppearanceCheck, row["Appearance Check"].ToString());
        }
        private void SetComboValue(ComboBox cbo, string value)
        {
            if (cbo.DataSource == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(value))
            {
                cbo.SelectedValue = "0";
                return;
            }

            value = value.Trim();

            if (value.Equals("Yes", StringComparison.OrdinalIgnoreCase))
            {
                cbo.SelectedValue = "1";
                return;
            }

            if (value.Equals("No", StringComparison.OrdinalIgnoreCase))
            {
                cbo.SelectedValue = "0";
                return;
            }

            cbo.SelectedValue = value;
        }
        private string GetComboValue(ComboBox cbo)
        {
            if (cbo.SelectedValue == null)
            {
                return "0";
            }

            return cbo.SelectedValue.ToString();
        }
        private SettingProperty GetDataFromScreen()
        {
            SettingProperty dataItem = new SettingProperty();

            dataItem.M_CODE = txtMCode.Text.Trim();
            dataItem.Keep_Data_Need = GetComboValue(cboKeepData);
            dataItem.Packing_Check_Mode = GetComboValue(cboPackingCheck);
            dataItem.Regular_Check_Need = GetComboValue(cboRegularCheck);
            dataItem.Function_Check_Need = GetComboValue(cboFunctionCheck);
            dataItem.Dimension_Check_Need = GetComboValue(cboDimensionCheck);
            dataItem.Appearance_Check_Need = GetComboValue(cboAppearanceCheck);
            dataItem.INUSE = "1";

            return dataItem;
        }

        private bool ValidateBeforeSave()
        {
            if (string.IsNullOrWhiteSpace(txtMCode.Text))
            {
                MessageBox.Show("กรุณาระบุ M Code", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMCode.Focus();
                return false;
            }

            if (GetComboValue(cboKeepData) == "0" &&
                GetComboValue(cboPackingCheck) == "0" &&
                GetComboValue(cboRegularCheck) == "0" &&
                GetComboValue(cboFunctionCheck) == "0" &&
                GetComboValue(cboDimensionCheck) == "0" &&
                GetComboValue(cboAppearanceCheck) == "0")
            {
                DialogResult confirm = MessageBox.Show(
                    "M Code นี้ยังไม่ได้เลือก Check ใด ๆ เลย ต้องการบันทึกต่อหรือไม่?",
                    "Confirm",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question
                );

                if (confirm == DialogResult.No)
                {
                    return false;
                }
            }

            return true;
        }

        private bool CheckMCodeInMES()
        {
            SettingProperty dataItem = new SettingProperty();
            dataItem.M_CODE = txtMCode.Text.Trim();

            DataTable dt = _controller.SearchMCodeInMES(dataItem);

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบ M Code นี้ใน MES กรุณาตรวจสอบหรือแจ้งฝ่ายที่เกี่ยวข้อง", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveInspectionSetting()
        {
            if (!ValidateBeforeSave())
            {
                return;
            }

            if (!CheckMCodeInMES())
            {
                return;
            }

            SettingProperty dataItem = GetDataFromScreen();

            Boolean result = _controller.SaveInspectionSetting(dataItem);

            if (result)
            {
                MessageBox.Show("บันทึก Inspection Setting สำเร็จ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveInspectionSetting();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void txtMCode_Leave(object sender, EventArgs e)
        {
            if (_isEditMode)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(txtMCode.Text))
            {
                return;
            }

            CheckMCodeInMES();
        }

        //private void frmMCodeInspectionSetting_Load_1(object sender, EventArgs e)
        //{

        //}
    }
}

