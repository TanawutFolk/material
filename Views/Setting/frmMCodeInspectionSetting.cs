using RawMat.Controllers;
using RawMat.Property;
using RawMat.SQLFactory;
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
        DataTable dtRegularEquipment = new DataTable();
        DataTable dtDimensionEquipment = new DataTable();
        DataTable dtEquipmentType = new DataTable();

        private string _mCode = "";
        private bool _isEditMode = false;

        public frmMCodeInspectionSetting()
        {
            InitializeComponent();

            _mCode = "";
            _isEditMode = false;
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
            BindInspectionLevelEvents();
            ClearInput();

            LoadEquipmentTypeList();

            SetupEquipmentGrid(dtgRegularEquipment);
            SetupEquipmentGrid(dtgDimensionEquipment);

            dtgRegularEquipment.DataError += dtgEquipment_DataError;
            dtgDimensionEquipment.DataError += dtgEquipment_DataError;

            if (_isEditMode)
            {
                LoadInspectionSettingByMCode(_mCode);
                LoadEquipmentSetting(_mCode);
                txtMCode.Enabled = false;
            }
            else
            {
                txtMCode.Enabled = true;
                txtMCode.Focus();
            }

            SetStrictnessFieldStatus(cboInscpectionLeveltab1, cboNormalReducetab1, cboS1tab1);
            SetStrictnessFieldStatus(cboInscpectionLeveltab2, cboNormalReducetab2, cboS1tab2);
            SetStrictnessFieldStatus(cboInscpectionLeveltab3, cboNormalReducetab3, cboS1tab3);
            SetStrictnessFieldStatus(cboInscpectionLeveltab4, cboNormalReducetab4, cboS1tab4);
        }
        private void BindCombo()
        {
            // --- 1. ผูกข้อมูล Master Yes/No (ส่วนบนของหน้าจอ) ---
            BindYesNoCombo(cboKeepData);
            BindPackingCheckCombo();
            BindYesNoCombo(cboRegularCheck);
            BindYesNoCombo(cboFunctionCheck);
            BindYesNoCombo(cboDimensionCheck);
            BindYesNoCombo(cboAppearanceCheck);

            // --- 2. ดึงข้อมูลรายการจาก Database มาเป็น DataTable ---
            // (ใช้ Controller ที่เราเพิ่มฟังก์ชัน GetList ไว้ก่อนหน้านี้)
            DataTable dtSamplingType = _controller.GetSamplingTypeList();   // รายชื่อประเภทการสุ่ม
            DataTable dtStrictnessType = _controller.GetStrictnessTypeList(); // รายชื่อ Normal/Reduce
            MessageBox.Show(
    "dtStrictnessType null = " + (dtStrictnessType == null).ToString() + Environment.NewLine +
    "Rows = " + (dtStrictnessType == null ? "0" : dtStrictnessType.Rows.Count.ToString()) + Environment.NewLine +
    "Columns = " + (dtStrictnessType == null ? "" : string.Join(", ", dtStrictnessType.Columns.Cast<DataColumn>().Select(c => c.ColumnName)))
);
            DataTable dtStrictnessLevel = _controller.GetStrictnessLevelList(); // รายชื่อ Level I, II, III, S-1

            // --- 3. ผูกข้อมูลเข้ากับ ComboBox ใน 4 Tab (เรียงตามลำดับหน้าจอ) ---

            // Tab 1: Regular
            BindMasterToCombo(cboInscpectionLeveltab1, dtSamplingType);
            BindMasterToCombo(cboNormalReducetab1, dtStrictnessType);
            BindMasterToCombo(cboS1tab1, dtStrictnessLevel);

            // Tab 2: Function
            BindMasterToCombo(cboInscpectionLeveltab2, dtSamplingType);
            BindMasterToCombo(cboNormalReducetab2, dtStrictnessType);
            BindMasterToCombo(cboS1tab2, dtStrictnessLevel);

            // Tab 3: Dimension
            BindMasterToCombo(cboInscpectionLeveltab3, dtSamplingType);
            BindMasterToCombo(cboNormalReducetab3, dtStrictnessType);
            BindMasterToCombo(cboS1tab3, dtStrictnessLevel);

            // Tab 4: Appearance
            BindMasterToCombo(cboInscpectionLeveltab4, dtSamplingType);
            BindMasterToCombo(cboNormalReducetab4, dtStrictnessType);
            BindMasterToCombo(cboS1tab4, dtStrictnessLevel);
        }

        // ฟังก์ชันช่วยผูก DataTable เข้ากับ ComboBox
        private void BindMasterToCombo(ComboBox cbo, DataTable dt)
        {
            if (dt == null) return;

            // ต้อง Copy ข้อมูลออกมาเพื่อให้แต่ละ ComboBox แยก DataSource กันเด็ดขาด
            DataTable dtCopy = dt.Copy();

            cbo.DataSource = dtCopy;
            cbo.DisplayMember = "TEXT";  // โชว์ชื่อ (เช่น Normal)
            cbo.ValueMember = "VALUE";   // เก็บ ID (เช่น 1)
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

            // Regular
            txtCavityQtytab1.Text = "-";
            txtInspectionQtytab1.Text = "-";
            txtCavityNametab1.Text = "-";

            // Function
            txtCavityQtytab2.Text = "-";
            txtInspectionQtytab2.Text = "-";
            txtCavityNametab2.Text = "-";

            // Dimension
            txtCavityQtytab3.Text = "-";
            txtInspectionQtytab3.Text = "-";
            txtCavityNametab3.Text = "-";

            // Appearance
            txtCavityQtytab4.Text = "-";
            txtInspectionQtytab4.Text = "-";
            txtCavityNametab4.Text = "-";
        }
        private string DisplayDashIfZeroOrEmpty(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "-";
            }

            string text = value.ToString().Trim();

            if (string.IsNullOrWhiteSpace(text) || text == "0")
            {
                return "-";
            }

            return text;
        }

        private string GetNumberValueFromTextBox(TextBox txt)
        {
            string text = txt.Text.Trim();

            if (string.IsNullOrWhiteSpace(text) || text == "-")
            {
                return "0";
            }

            return text;
        }

        private string GetTextValueFromTextBox(TextBox txt)
        {
            string text = txt.Text.Trim();

            if (text == "-")
            {
                return "";
            }

            return text;
        }


        private void LoadInspectionSettingByMCode(string mCode)
        {
            SettingProperty dataItem = new SettingProperty();
            dataItem.M_CODE = mCode.Trim();

            DataTable dt = _controller.SearchInspectionSettingByMCode(dataItem);
            if (dt == null || dt.Rows.Count == 0) return;

            DataRow row = dt.Rows[0];
            txtMCode.Text = mCode;

            // Master Data
            SetComboValue(cboKeepData, row["Keep Data"].ToString());
            SetComboValue(cboPackingCheck, row["Packing Check"].ToString());
            SetComboValue(cboRegularCheck, row["Regular Check"].ToString());
            SetComboValue(cboFunctionCheck, row["Function Check"].ToString());
            SetComboValue(cboDimensionCheck, row["Dimension Check"].ToString());
            SetComboValue(cboAppearanceCheck, row["Appearance Check"].ToString());

            // 1. Regular
            txtCavityQtytab1.Text = DisplayDashIfZeroOrEmpty(row["Reg_Cavity_Qty"]);
            SetComboValue(cboInscpectionLeveltab1, row["Reg_Sampling_Type"].ToString());
            txtInspectionQtytab1.Text = DisplayDashIfZeroOrEmpty(row["Reg_Sampling_Qty"]);
            SetComboValue(cboNormalReducetab1, row["Reg_Strictness_Type"].ToString());
            SetComboValue(cboS1tab1, row["Reg_Strictness_Level"].ToString());
            txtCavityNametab1.Text = DisplayDashIfZeroOrEmpty(row["Reg_Cavity_Name"]);

            // 2. Function
            txtCavityQtytab2.Text = DisplayDashIfZeroOrEmpty(row["Func_Cavity_Qty"]);
            SetComboValue(cboInscpectionLeveltab2, row["Func_Sampling_Type"].ToString());
            txtInspectionQtytab2.Text = DisplayDashIfZeroOrEmpty(row["Func_Sampling_Qty"]);
            SetComboValue(cboNormalReducetab2, row["Func_Strictness_Type"].ToString());
            SetComboValue(cboS1tab2, row["Func_Strictness_Level"].ToString());
            txtCavityNametab2.Text = DisplayDashIfZeroOrEmpty(row["Func_Cavity_Name"]);

            // 3. Dimension
            txtCavityQtytab3.Text = DisplayDashIfZeroOrEmpty(row["Dim_Cavity_Qty"]);
            SetComboValue(cboInscpectionLeveltab3, row["Dim_Sampling_Type"].ToString());
            txtInspectionQtytab3.Text = DisplayDashIfZeroOrEmpty(row["Dim_Sampling_Qty"]);
            SetComboValue(cboNormalReducetab3, row["Dim_Strictness_Type"].ToString());
            SetComboValue(cboS1tab3, row["Dim_Strictness_Level"].ToString());
            txtCavityNametab3.Text = DisplayDashIfZeroOrEmpty(row["Dim_Cavity_Name"]);

            // 4. Appearance
            txtCavityQtytab4.Text = DisplayDashIfZeroOrEmpty(row["App_Cavity_Qty"]);
            SetComboValue(cboInscpectionLeveltab4, row["App_Sampling_Type"].ToString());
            txtInspectionQtytab4.Text = DisplayDashIfZeroOrEmpty(row["App_Sampling_Qty"]);
            SetComboValue(cboNormalReducetab4, row["App_Strictness_Type"].ToString());
            SetComboValue(cboS1tab4, row["App_Strictness_Level"].ToString());
            txtCavityNametab4.Text = DisplayDashIfZeroOrEmpty(row["App_Cavity_Name"]);
        }
        private void SetComboValue(ComboBox cbo, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                value = "0";
            }
            value = value.Trim();

            if (cbo.DataSource == null)
            {
                cbo.Text = value;
                return;
            }

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

            if (int.TryParse(value, out int intValue))
            {
                cbo.SelectedValue = intValue; 
            }

            if (cbo.SelectedIndex == -1)
            {
                cbo.SelectedValue = value;
            }
        }

        private string GetComboValue(ComboBox cbo)
        {
            // 1. ถ้าไม่มี DataSource ให้ดึงค่าที่พิมพ์อยู่ในช่อง Text ไป Save เลย
            if (cbo.DataSource == null)
            {
                return string.IsNullOrWhiteSpace(cbo.Text) ? "0" : cbo.Text;
            }

            // 2. ถ้ามี DataSource ดึงจาก Value ปกติ
            if (cbo.SelectedValue == null)
            {
                return "0";
            }
            return cbo.SelectedValue.ToString();
        }
        private SettingProperty GetDataFromScreen()
        {
            SettingProperty dataItem = new SettingProperty();

            // Master
            dataItem.M_CODE = txtMCode.Text.Trim();
            dataItem.Keep_Data_Need = GetComboValue(cboKeepData);
            dataItem.Packing_Check_Mode = GetComboValue(cboPackingCheck);
            dataItem.Regular_Check_Need = GetComboValue(cboRegularCheck);
            dataItem.Function_Check_Need = GetComboValue(cboFunctionCheck);
            dataItem.Dimension_Check_Need = GetComboValue(cboDimensionCheck);
            dataItem.Appearance_Check_Need = GetComboValue(cboAppearanceCheck);
            dataItem.INUSE = "1";

            // 1. Regular
            dataItem.Reg_Cavity_Qty = GetNumberValueFromTextBox(txtCavityQtytab1);
            dataItem.Reg_Sampling_Type = GetComboValue(cboInscpectionLeveltab1);
            dataItem.Reg_Sampling_Qty = GetNumberValueFromTextBox(txtInspectionQtytab1);
            dataItem.Reg_Strictness_Type = GetComboValue(cboNormalReducetab1);
            dataItem.Reg_Strictness_Level = GetComboValue(cboS1tab1);
            dataItem.Reg_Cavity_Name = GetTextValueFromTextBox(txtCavityNametab1);

            // 2. Function
            dataItem.Func_Cavity_Qty = GetNumberValueFromTextBox(txtCavityQtytab2);
            dataItem.Func_Sampling_Type = GetComboValue(cboInscpectionLeveltab2);
            dataItem.Func_Sampling_Qty = GetNumberValueFromTextBox(txtInspectionQtytab2);
            dataItem.Func_Strictness_Type = GetComboValue(cboNormalReducetab2);
            dataItem.Func_Strictness_Level = GetComboValue(cboS1tab2);
            dataItem.Func_Cavity_Name = GetTextValueFromTextBox(txtCavityNametab2);

            // 3. Dimension
            dataItem.Dim_Cavity_Qty = GetNumberValueFromTextBox(txtCavityQtytab3);
            dataItem.Dim_Sampling_Type = GetComboValue(cboInscpectionLeveltab3);
            dataItem.Dim_Sampling_Qty = GetNumberValueFromTextBox(txtInspectionQtytab3);
            dataItem.Dim_Strictness_Type = GetComboValue(cboNormalReducetab3);
            dataItem.Dim_Strictness_Level = GetComboValue(cboS1tab3);
            dataItem.Dim_Cavity_Name = GetTextValueFromTextBox(txtCavityNametab3);

            // 4. Appearance
            dataItem.App_Cavity_Qty = GetNumberValueFromTextBox(txtCavityQtytab4);
            dataItem.App_Sampling_Type = GetComboValue(cboInscpectionLeveltab4);
            dataItem.App_Sampling_Qty = GetNumberValueFromTextBox(txtInspectionQtytab4);
            dataItem.App_Strictness_Type = GetComboValue(cboNormalReducetab4);
            dataItem.App_Strictness_Level = GetComboValue(cboS1tab4);
            dataItem.App_Cavity_Name = GetTextValueFromTextBox(txtCavityNametab4);

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

        private void UpdateTabStatus_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabRegularCheckDetails.Enabled = (cboRegularCheck.Text == "Yes");
            tabFunctionCheckDetails.Enabled = (cboFunctionCheck.Text == "Yes");
            tabDimensionCheckDetails.Enabled = (cboDimensionCheck.Text == "Yes");
            tabAppearanceCheckDetails.Enabled = (cboAppearanceCheck.Text == "Yes");
            //UpdateTabStatus_SelectedIndexChanged(null, null);
        }

        private void BindInspectionLevelEvents()
        {
            cboInscpectionLeveltab1.SelectedIndexChanged += cboInscpectionLeveltab_SelectedIndexChanged;
            cboInscpectionLeveltab2.SelectedIndexChanged += cboInscpectionLeveltab_SelectedIndexChanged;
            cboInscpectionLeveltab3.SelectedIndexChanged += cboInscpectionLeveltab_SelectedIndexChanged;
            cboInscpectionLeveltab4.SelectedIndexChanged += cboInscpectionLeveltab_SelectedIndexChanged;
        }

        private void cboInscpectionLeveltab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender == cboInscpectionLeveltab1)
            {
                SetStrictnessFieldStatus(cboInscpectionLeveltab1, cboNormalReducetab1, cboS1tab1);
            }
            else if (sender == cboInscpectionLeveltab2)
            {
                SetStrictnessFieldStatus(cboInscpectionLeveltab2, cboNormalReducetab2, cboS1tab2);
            }
            else if (sender == cboInscpectionLeveltab3)
            {
                SetStrictnessFieldStatus(cboInscpectionLeveltab3, cboNormalReducetab3, cboS1tab3);
            }
            else if (sender == cboInscpectionLeveltab4)
            {
                SetStrictnessFieldStatus(cboInscpectionLeveltab4, cboNormalReducetab4, cboS1tab4);
            }
        }

        private void SetStrictnessFieldStatus(ComboBox cboInspectionLevel, ComboBox cboNormalReduce, ComboBox cboS1)
        {
            bool isStrictnessTable = IsStrictnessTable(cboInspectionLevel);

            cboNormalReduce.Enabled = isStrictnessTable;
            cboS1.Enabled = isStrictnessTable;

            if (!isStrictnessTable)
            {
                cboNormalReduce.SelectedIndex = -1;
                cboS1.SelectedIndex = -1;
            }
        }

        private bool IsStrictnessTable(ComboBox cbo)
        {
            string text = cbo.Text.Trim();
            string value = cbo.SelectedValue == null ? "" : cbo.SelectedValue.ToString().Trim();

            return text.Equals("Strictness Table", StringComparison.OrdinalIgnoreCase)
                || value.Equals("Strictness Table", StringComparison.OrdinalIgnoreCase);
        }

        private void btnSave_Click_1(object sender, EventArgs e)
        {
            SaveInspectionSetting();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void SetupEquipmentGrid(DataGridView dtg)
        {
            dtg.AutoGenerateColumns = false;
            dtg.AllowUserToAddRows = true;
            dtg.AllowUserToDeleteRows = true;
            dtg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dtg.MultiSelect = false;
            dtg.RowHeadersVisible = false;

            dtg.Columns.Clear();

            dtg.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "M_CODE",
                HeaderText = "M Code",
                DataPropertyName = "M_CODE",
                Width = 100,
                ReadOnly = true,
                Visible = false
            });

            dtg.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "POINT_ORDER",
                HeaderText = "Order",
                DataPropertyName = "POINT_ORDER",
                Width = 60
            });

            dtg.Columns.Add(new DataGridViewComboBoxColumn
            {
                Name = "EQUIPMENT_TYPE",
                HeaderText = "Equipment",
                DataPropertyName = "EQUIPMENT_TYPE",
                DataSource = dtEquipmentType,
                DisplayMember = "Equipment_Name",
                ValueMember = "Equipment_Type",
                Width = 180,
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
            });

            dtg.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Equipment_Name",
                HeaderText = "Equipment Name",
                DataPropertyName = "Equipment_Name",
                Width = 160,
                ReadOnly = true
            });

            dtg.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "POINT_NAME",
                HeaderText = "Point Name",
                DataPropertyName = "POINT_NAME",
                Width = 160
            });

            dtg.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "POINT_CAL",
                HeaderText = "Point Cal",
                DataPropertyName = "POINT_CAL",
                Width = 120
            });

            dtg.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CRITERIA_MIN",
                HeaderText = "Min",
                DataPropertyName = "CRITERIA_MIN",
                Width = 80
            });

            dtg.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "CRITERIA_MAX",
                HeaderText = "Max",
                DataPropertyName = "CRITERIA_MAX",
                Width = 80
            });
        }

        private void LoadEquipmentTypeList()
        {
            dtEquipmentType = _controller.GetEquipmentTypeList();

            if (dtEquipmentType == null)
            {
                dtEquipmentType = new DataTable();
            }
        }
        private void LoadEquipmentSetting(string mCode)
        {
            if (string.IsNullOrWhiteSpace(mCode))
            {
                dtgRegularEquipment.DataSource = null;
                dtgDimensionEquipment.DataSource = null;
                return;
            }

            SettingProperty dataItem = new SettingProperty();
            dataItem.M_CODE = mCode.Trim();

            dtRegularEquipment = _controller.SearchRegularEquipmentSetting(dataItem);
            dtDimensionEquipment = _controller.SearchDimensionEquipmentSetting(dataItem);

            dtgRegularEquipment.DataSource = dtRegularEquipment;
            dtgDimensionEquipment.DataSource = dtDimensionEquipment;
        }
        private void dtgEquipment_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }
        private void cboNormalReducetab4_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label28_Click(object sender, EventArgs e)
        {
        }

        private void cboInscpectionLeveltab4_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

        private void label31_Click(object sender, EventArgs e)
        {
        }

        private void label33_Click(object sender, EventArgs e)
        {
        }

        private void txtCavityQtytab4_TextChanged(object sender, EventArgs e)
        {
        }
    }
}

