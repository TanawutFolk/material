using RawMat.Controllers;
using RawMat.Property;
using RawMat.SQLFactory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RawMat.ViewsMaterial.Setting
{
    public partial class frmMCodeInspectionSetting : Form
    {
        // ─── Constants ────────────────────────────────────────────────────────────
        private const string VALUE_YES = "1";
        private const string VALUE_NO = "0";
        private const string VALUE_ACTIVE = "1";
        private const string DISPLAY_DASH = "-";
        private const string SAMPLING_TABLE_TXT = "Sampling Table";
        private const string SAMPLING_FIX_TXT = "Fix";
        private const string SAMPLING_ALL_TXT = "All";
        private const string SAMPLING_CAVITY_TXT = "Pc/Cavity";
        private const string SAMPLING_PERCENT_TXT = "% Lot Size Receive";
        private const string UNIT_PCS = "Pcs";
        private const string UNIT_PCS_PER_CAVITY = "Pcs / Cavity";
        private const string UNIT_PCS_PER_CAVITY_MIN = "Cavity ≥ ... Pcs";
        private const string UNIT_PERCENT = "% ของ Lot";

        // ─── Fields ───────────────────────────────────────────────────────────────
        private readonly SettingControllers _controller = new SettingControllers();
        private DataTable dtRegularEquipment = new DataTable();
        private DataTable dtFunctionEquipment = new DataTable();
        private DataTable dtFunctionChecks = new DataTable();
        private DataTable dtDimensionEquipment = new DataTable();
        private DataTable dtEquipmentType = new DataTable();
        private DataTable dtJudgeType = new DataTable();

        private readonly string _mCode;
        private readonly bool _isEditMode;
        private bool _isSyncingCavity;

        // ─── Tab descriptor — ผูก ComboBox และ TextBox แต่ละ Tab ไว้ด้วยกัน ──────
        private struct TabDescriptor
        {
            public ComboBox CboInspectionLevel;
            public ComboBox CboNormalReduce;
            public ComboBox CboS1;
            public TextBox TxtInspectionQty;
            public ComboBox CboInspectionQty;
            public Label LbQtyUnit;
            public string SamplingTypeField;
            public string SamplingQtyField;
            public string StrictnessTypeField;
            public string StrictnessLevelField;
        }

        private TabDescriptor[] _tabs;

        // ─── Constructors ─────────────────────────────────────────────────────────
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

        // ─── Form Load ────────────────────────────────────────────────────────────
        private void frmMCodeInspectionSetting_Load(object sender, EventArgs e)
        {
            InitTabDescriptors();

            // ต้องสร้าง Dropdown จำนวนต่อ Cavity ก่อนผูก Event และก่อนโหลดค่าเดิม
            // ไม่เช่นนั้น Event จะจำ TabDescriptor ตอนที่ CboInspectionQty ยังเป็น null:
            // TextBox ถูกซ่อนเมื่อเลือก Sampling Table / Pc/Cavity แต่ไม่มี Dropdown แสดงแทน
            for (int i = 0; i < _tabs.Length; i++)
            {
                _tabs[i].CboInspectionQty = CreateQtyCombo(_tabs[i].TxtInspectionQty);
            }

            BindAllCombos();
            BindEvents();
            ClearInput();

            LoadEquipmentTypeList();
            SetupEquipmentGrid(dtgRegularEquipment);
            SetupEquipmentGrid(dtgFunctionEquipment);
            SetupFunctionCheckGrid();
            SetupEquipmentGrid(dtgDimensionEquipment);

            dtgRegularEquipment.DataError += dtgEquipment_DataError;
            dtgFunctionEquipment.DataError += dtgEquipment_DataError;
            dtgDimensionEquipment.DataError += dtgEquipment_DataError;
            dtgRegularEquipment.CellFormatting += dtgRegularEquipment_CellFormatting;
            dtgRegularEquipment.CellContentClick += dtgEquipment_CellContentClick;
            dtgFunctionEquipment.CellContentClick += dtgEquipment_CellContentClick;
            dtgDimensionEquipment.CellContentClick += dtgEquipment_CellContentClick;
            dtgRegularEquipment.UserDeletingRow += dtgEquipment_UserDeletingRow;
            dtgFunctionEquipment.UserDeletingRow += dtgEquipment_UserDeletingRow;
            dtgDimensionEquipment.UserDeletingRow += dtgEquipment_UserDeletingRow;
            dtgFunctionEquipment.DefaultValuesNeeded += dtgFunctionEquipment_DefaultValuesNeeded;
            dtgRegularEquipment.DefaultValuesNeeded += dtgEquipment_DefaultValuesNeeded;
            dtgDimensionEquipment.DefaultValuesNeeded += dtgEquipment_DefaultValuesNeeded;
            dtg_function_Check.CellContentClick += dtgFunctionCheck_CellContentClick;
            dtg_function_Check.UserDeletingRow += dtgEquipment_UserDeletingRow;
            dtg_function_Check.DefaultValuesNeeded += dtgFunctionCheck_DefaultValuesNeeded;

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
                dtRegularEquipment = CreateEquipmentTable();
                dtFunctionEquipment = CreateEquipmentTable();
                dtFunctionChecks = CreateFunctionCheckTable();
                dtDimensionEquipment = CreateEquipmentTable();
                dtgRegularEquipment.DataSource = dtRegularEquipment;
                dtgFunctionEquipment.DataSource = dtFunctionEquipment;
                dtg_function_Check.DataSource = dtFunctionChecks;
                dtgDimensionEquipment.DataSource = dtDimensionEquipment;
            }

            SetAllCheckTabStatus();

            foreach (var tab in _tabs)
            {
                ApplyInspectionLevelRules(tab);
            }
        }

        // ─── Tab Descriptors ──────────────────────────────────────────────────────
        /// <summary>ผูก TabDescriptor array กับ Control แต่ละ Tab เพื่อลด code ซ้ำ</summary>
        private void InitTabDescriptors()
        {
            _tabs = new[]
            {
                new TabDescriptor
                {
                    CboInspectionLevel  = cboInscpectionLeveltab1,
                    CboNormalReduce     = cboNormalReducetab1,
                    CboS1               = cboS1tab1,
                    TxtInspectionQty    = txtInspectionQtytab1,
                    LbQtyUnit           = lbForQty,
                    SamplingTypeField   = "Reg_Sampling_Type",
                    SamplingQtyField    = "Reg_Sampling_Qty",
                    StrictnessTypeField = "Reg_Strictness_Type",
                    StrictnessLevelField= "Reg_Strictness_Level"
                },
                new TabDescriptor
                {
                    CboInspectionLevel  = cboInscpectionLeveltab2,
                    CboNormalReduce     = cboNormalReducetab2,
                    CboS1               = cboS1tab2,
                    TxtInspectionQty    = txtInspectionQtytab2,
                    LbQtyUnit           = label18,
                    SamplingTypeField   = "Func_Sampling_Type",
                    SamplingQtyField    = "Func_Sampling_Qty",
                    StrictnessTypeField = "Func_Strictness_Type",
                    StrictnessLevelField= "Func_Strictness_Level"
                },
                new TabDescriptor
                {
                    CboInspectionLevel  = cboInscpectionLeveltab3,
                    CboNormalReduce     = cboNormalReducetab3,
                    CboS1               = cboS1tab3,
                    TxtInspectionQty    = txtInspectionQtytab3,
                    LbQtyUnit           = label24,
                    SamplingTypeField   = "Dim_Sampling_Type",
                    SamplingQtyField    = "Dim_Sampling_Qty",
                    StrictnessTypeField = "Dim_Strictness_Type",
                    StrictnessLevelField= "Dim_Strictness_Level"
                },
                new TabDescriptor
                {
                    CboInspectionLevel  = cboInscpectionLeveltab4,
                    CboNormalReduce     = cboNormalReducetab4,
                    CboS1               = cboS1tab4,
                    TxtInspectionQty    = txtInspectionQtytab4,
                    LbQtyUnit           = label30,
                    SamplingTypeField   = "App_Sampling_Type",
                    SamplingQtyField    = "App_Sampling_Qty",
                    StrictnessTypeField = "App_Strictness_Type",
                    StrictnessLevelField= "App_Strictness_Level"
                }
            };
        }

        // ─── Combo Binding ────────────────────────────────────────────────────────
        private void BindAllCombos()
        {
            // Check combos — แสดงเป็น Check / No
            foreach (var cbo in new[] { cboRegularCheck, cboFunctionCheck,
                                         cboDimensionCheck, cboAppearanceCheck, cboPackingCheck })
            {
                BindSimpleCombo(cbo, new[] { ("No", VALUE_NO), ("Check", VALUE_YES) });
            }

            // Keep Data — แสดงเป็น Keep / No
            BindSimpleCombo(cboKeepData, new[] { ("No", VALUE_NO), ("Keep", VALUE_YES) });

            BindSimpleCombo(cboStatus, new[] { ("Active", VALUE_ACTIVE), ("InActive", VALUE_NO) });

            // Master lists from DB
            DataTable dtSamplingType = _controller.GetSamplingTypeList();
            DataTable dtStrictnessType = _controller.GetStrictnessTypeList();
            DataTable dtStrictnessLevel = _controller.GetStrictnessLevelList();

            foreach (var tab in _tabs)
            {
                BindMasterToCombo(tab.CboInspectionLevel, dtSamplingType);
                BindMasterToCombo(tab.CboNormalReduce, dtStrictnessType);
                BindMasterToCombo(tab.CboS1, dtStrictnessLevel);
            }
        }

        /// <summary>ผูก ComboBox ด้วย tuple array แทนการสร้าง DataTable ซ้ำ</summary>
        private static void BindSimpleCombo(ComboBox cbo, (string Text, string Value)[] items)
        {
            var dt = new DataTable();
            dt.Columns.Add("TEXT");
            dt.Columns.Add("VALUE");

            foreach (var (text, value) in items)
            {
                dt.Rows.Add(text, value);
            }

            cbo.DataSource = dt;
            cbo.DisplayMember = "TEXT";
            cbo.ValueMember = "VALUE";
            cbo.SelectedValue = items[0].Value; // Default = first item
        }

        private static void BindMasterToCombo(ComboBox cbo, DataTable dt)
        {
            if (dt == null) return;
            cbo.DataSource = dt.Copy(); // Copy เพื่อแยก DataSource แต่ละ ComboBox
            cbo.DisplayMember = "TEXT";
            cbo.ValueMember = "VALUE";
        }

        // ─── Event Binding ────────────────────────────────────────────────────────
        private void BindEvents()
        {
            // Tab enable/disable ตาม Check combo
            foreach (var (cbo, tab) in new[]
            {
                (cboRegularCheck,    tabRegularCheckDetails),
                (cboFunctionCheck,   tabFunctionCheckDetails),
                (cboDimensionCheck,  tabDimensionCheckDetails),
                (cboAppearanceCheck, tabAppearanceCheckDetails)
            })
            {
                var capturedCbo = cbo;
                var capturedTab = tab; // capture สำหรับ lambda
                capturedCbo.SelectedIndexChanged += (_, __) => SetCheckTabStatus(capturedCbo, capturedTab);
            }

            // Strictness + หน่วย Qty ตาม Inspection Level
            foreach (var t in _tabs)
            {
                var captured = t;
                captured.CboInspectionLevel.SelectedIndexChanged += (_, __) =>
                    ApplyInspectionLevelRules(captured);
            }

            // Cavity sync (Qty + Name) ข้าม Tab
            var cavityQtyBoxes = new[] { txtQtyCavityTab1, txtQtyCavityTab2, txtQtyCavityTab3, txtQtyCavityTab4 };
            var cavityNameBoxes = new[] { txtCavityNameTab1, txtCavityNameTab2, txtCavityNameTab3, txtCavityNameTab4 };

            foreach (var txt in cavityQtyBoxes)
            {
                var src = txt;
                src.TextChanged += (_, __) => SyncCavityGroup(src, cavityQtyBoxes);
            }
            foreach (var txt in cavityNameBoxes)
            {
                var src = txt;
                src.TextChanged += (_, __) => SyncCavityGroup(src, cavityNameBoxes);
            }
        }

        private void SetAllCheckTabStatus()
        {
            SetCheckTabStatus(cboRegularCheck, tabRegularCheckDetails);
            SetCheckTabStatus(cboFunctionCheck, tabFunctionCheckDetails);
            SetCheckTabStatus(cboDimensionCheck, tabDimensionCheckDetails);
            SetCheckTabStatus(cboAppearanceCheck, tabAppearanceCheckDetails);
        }

        private void SetCheckTabStatus(ComboBox cbo, TabPage tab)
        {
            tab.Enabled = IsYesComboValue(cbo);
        }

        private static bool IsYesComboValue(ComboBox cbo)
        {
            string value = GetComboValue(cbo).Trim();
            return value == VALUE_YES || IsAffirmativeText(cbo.Text.Trim());
        }

        /// <summary>ข้อความฝั่งจอที่หมายถึง "เลือก" — Yes (ค่าเดิมจาก DB), Check, Keep</summary>
        private static bool IsAffirmativeText(string text)
        {
            return text.Equals("Yes", StringComparison.OrdinalIgnoreCase)
                || text.Equals("Check", StringComparison.OrdinalIgnoreCase)
                || text.Equals("Keep", StringComparison.OrdinalIgnoreCase);
        }

        private void SyncCavityGroup(TextBox source, TextBox[] group)
        {
            if (_isSyncingCavity) return;
            _isSyncingCavity = true;

            foreach (var target in group.Where(t => t != source))
            {
                target.Text = source.Text;
            }

            _isSyncingCavity = false;
        }

        // ─── Clear / Load ─────────────────────────────────────────────────────────
        private void ClearInput()
        {
            txtMCode.Text = "";

            foreach (var cbo in new[] { cboKeepData, cboPackingCheck, cboRegularCheck,
                                         cboFunctionCheck, cboDimensionCheck, cboAppearanceCheck })
            {
                SetComboValue(cbo, VALUE_NO);
            }

            SetComboValue(cboStatus, VALUE_ACTIVE);

            txtQtyCavityTab1.Text = DISPLAY_DASH;
            txtInspectionQtytab1.Text = DISPLAY_DASH;
            txtCavityNameTab1.Text = DISPLAY_DASH;
        }

        private void LoadInspectionSettingByMCode(string mCode)
        {
            var dataItem = new SettingProperty { M_CODE = mCode.Trim() };
            DataTable dt = _controller.SearchInspectionSettingByMCode(dataItem);
            if (dt == null || dt.Rows.Count == 0) return;

            DataRow row = dt.Rows[0];
            txtMCode.Text = mCode;

            // Master
            SetComboValue(cboKeepData, GetRowValue(row, "Data Result", "Keep Data"));
            SetComboValue(cboPackingCheck, row["Packing Check"].ToString());
            SetComboValue(cboRegularCheck, row["Regular Check"].ToString());
            SetComboValue(cboFunctionCheck, row["Function Check"].ToString());
            SetComboValue(cboDimensionCheck, row["Dimension Check"].ToString());
            SetComboValue(cboAppearanceCheck, row["Appearance Check"].ToString());

            // Status — รองรับทั้ง INUSE และ Status column
            string statusCol = row.Table.Columns.Contains("INUSE") ? "INUSE"
                             : row.Table.Columns.Contains("Status") ? "Status"
                             : null;
            SetComboValue(cboStatus, statusCol != null ? NormalizeStatusValue(row[statusCol]) : VALUE_ACTIVE);

            // Common Cavity (ใช้ค่าแรกที่ไม่ใช่ 0 หรือว่างจาก 4 Tab)
            txtQtyCavityTab1.Text = GetFirstCavityValue(row, "Reg_Cavity_Qty", "Func_Cavity_Qty", "Dim_Cavity_Qty", "App_Cavity_Qty");
            txtCavityNameTab1.Text = GetFirstCavityValue(row, "Reg_Cavity_Name", "Func_Cavity_Name", "Dim_Cavity_Name", "App_Cavity_Name");

            // Do not display stale detail rows for checks that are disabled.
            var checkCombos = new[]
            {
                cboRegularCheck,
                cboFunctionCheck,
                cboDimensionCheck,
                cboAppearanceCheck
            };

            for (int i = 0; i < _tabs.Length; i++)
            {
                var tab = _tabs[i];
                if (!IsYesComboValue(checkCombos[i]))
                {
                    tab.CboInspectionLevel.SelectedIndex = -1;
                    SetSamplingQty(tab, DISPLAY_DASH);
                    tab.CboNormalReduce.SelectedIndex = -1;
                    tab.CboS1.SelectedIndex = -1;
                    continue;
                }

                SetComboValue(tab.CboInspectionLevel, row[tab.SamplingTypeField].ToString());
                SetSamplingQty(tab, DisplayDashIfZeroOrEmpty(row[tab.SamplingQtyField]));
                SetComboValue(tab.CboNormalReduce, row[tab.StrictnessTypeField].ToString());
                SetComboValue(tab.CboS1, row[tab.StrictnessLevelField].ToString());
            }
        }

        // ─── Get/Set Screen Values ────────────────────────────────────────────────
        private static string GetRowValue(DataRow row, params string[] columnNames)
        {
            if (row == null || columnNames == null)
            {
                return string.Empty;
            }

            foreach (string columnName in columnNames)
            {
                if (row.Table.Columns.Contains(columnName))
                {
                    return row[columnName]?.ToString() ?? string.Empty;
                }
            }

            return string.Empty;
        }

        private static void SetComboValue(ComboBox cbo, string value)
        {
            if (cbo.DataSource == null)
            {
                cbo.Text = value ?? "";
                return;
            }

            // Normalize Yes/Check/Keep/No → 1/0
            value = value?.Trim() ?? VALUE_NO;
            if (IsAffirmativeText(value)) value = VALUE_YES;
            else if (value.Equals("No", StringComparison.OrdinalIgnoreCase)) value = VALUE_NO;

            // ลอง set ด้วย int ก่อน (ส่วนใหญ่ ValueMember เป็นตัวเลข)
            if (int.TryParse(value, out int intVal))
                cbo.SelectedValue = intVal;

            // Fallback ถ้ายังไม่เจอ
            if (cbo.SelectedIndex == -1)
                cbo.SelectedValue = value;
        }

        private static string GetComboValue(ComboBox cbo)
        {
            if (cbo.DataSource == null)
                return string.IsNullOrWhiteSpace(cbo.Text) ? VALUE_NO : cbo.Text;

            return cbo.SelectedValue?.ToString() ?? VALUE_NO;
        }

        private SettingProperty GetDataFromScreen()
        {
            string commonQty = GetNumberValueFromTextBox(txtQtyCavityTab1);
            string commonName = GetTextValueFromTextBox(txtCavityNameTab1);

            var dataItem = new SettingProperty
            {
                M_CODE = txtMCode.Text.Trim(),
                Keep_Data_Need = GetComboValue(cboKeepData),
                Packing_Check_Mode = GetComboValue(cboPackingCheck),
                Regular_Check_Need = GetComboValue(cboRegularCheck),
                Function_Check_Need = GetComboValue(cboFunctionCheck),
                Dimension_Check_Need = GetComboValue(cboDimensionCheck),
                Appearance_Check_Need = GetComboValue(cboAppearanceCheck),
                INUSE = GetComboValue(cboStatus),

                // Regular
                Reg_Cavity_Qty = commonQty,
                Reg_Sampling_Type = GetComboValue(cboInscpectionLeveltab1),
                Reg_Sampling_Qty = GetSamplingQty(_tabs[0]),
                Reg_Strictness_Type = GetComboValue(cboNormalReducetab1),
                Reg_Strictness_Level = GetComboValue(cboS1tab1),
                Reg_Cavity_Name = commonName,

                // Function
                Func_Cavity_Qty = commonQty,
                Func_Sampling_Type = GetComboValue(cboInscpectionLeveltab2),
                Func_Sampling_Qty = GetSamplingQty(_tabs[1]),
                Func_Strictness_Type = GetComboValue(cboNormalReducetab2),
                Func_Strictness_Level = GetComboValue(cboS1tab2),
                Func_Cavity_Name = commonName,

                // Dimension
                Dim_Cavity_Qty = commonQty,
                Dim_Sampling_Type = GetComboValue(cboInscpectionLeveltab3),
                Dim_Sampling_Qty = GetSamplingQty(_tabs[2]),
                Dim_Strictness_Type = GetComboValue(cboNormalReducetab3),
                Dim_Strictness_Level = GetComboValue(cboS1tab3),
                Dim_Cavity_Name = commonName,

                // Appearance
                App_Cavity_Qty = commonQty,
                App_Sampling_Type = GetComboValue(cboInscpectionLeveltab4),
                App_Sampling_Qty = GetSamplingQty(_tabs[3]),
                App_Strictness_Type = GetComboValue(cboNormalReducetab4),
                App_Strictness_Level = GetComboValue(cboS1tab4),
                App_Cavity_Name = commonName,

                RegularEquipment = GetEquipmentTableFromGrid(dtgRegularEquipment),
                FunctionEquipment = GetEquipmentTableFromGrid(dtgFunctionEquipment),
                FunctionChecks = GetFunctionCheckTableFromGrid(),
                DimensionEquipment = GetEquipmentTableFromGrid(dtgDimensionEquipment)
            };

            return dataItem;
        }

        private DataTable CreateEquipmentTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("M_CODE");
            dt.Columns.Add("POINT_ORDER");
            dt.Columns.Add("EQUIPMENT_TYPE");
            dt.Columns.Add("POINT_NAME");
            dt.Columns.Add("POINT_CAL");
            dt.Columns.Add("CRITERIA_MIN");
            dt.Columns.Add("CRITERIA_MAX");
            dt.Columns.Add("UNIT");
            dt.Columns.Add("JUDGE_TYPE");
            return dt;
        }

        private static DataTable CreateFunctionCheckTable()
        {
            var dt = new DataTable();
            dt.Columns.Add("ID");
            dt.Columns.Add("M_CODE");
            dt.Columns.Add("CHECK_ORDER");
            dt.Columns.Add("CHECK_DETAIL");
            return dt;
        }

        private DataTable GetFunctionCheckTableFromGrid()
        {
            dtg_function_Check.EndEdit();
            if (dtg_function_Check.DataSource != null)
            {
                BindingContext[dtg_function_Check.DataSource]?.EndCurrentEdit();
            }

            var result = CreateFunctionCheckTable();
            foreach (DataGridViewRow gridRow in dtg_function_Check.Rows)
            {
                if (gridRow.IsNewRow || IsEmptyFunctionCheckRow(gridRow)) continue;

                DataRow row = result.NewRow();
                row["ID"] = GetGridCellText(gridRow, "ID");
                row["M_CODE"] = txtMCode.Text.Trim();
                row["CHECK_ORDER"] = GetGridCellText(gridRow, "CHECK_ORDER");
                row["CHECK_DETAIL"] = GetGridCellText(gridRow, "CHECK_DETAIL");
                result.Rows.Add(row);
            }
            return result;
        }

        private static bool IsEmptyFunctionCheckRow(DataGridViewRow row)
        {
            return string.IsNullOrWhiteSpace(GetGridCellText(row, "CHECK_ORDER")) &&
                   string.IsNullOrWhiteSpace(GetGridCellText(row, "CHECK_DETAIL"));
        }

        private DataTable GetEquipmentTableFromGrid(DataGridView dtg)
        {
            dtg.EndEdit();
            if (dtg.DataSource != null)
            {
                BindingContext[dtg.DataSource]?.EndCurrentEdit();
            }

            var result = CreateEquipmentTable();

            foreach (DataGridViewRow gridRow in dtg.Rows)
            {
                if (gridRow.IsNewRow || IsEmptyEquipmentRow(gridRow))
                {
                    continue;
                }

                var row = result.NewRow();
                row["M_CODE"] = txtMCode.Text.Trim();
                row["POINT_ORDER"] = GetGridCellText(gridRow, "POINT_ORDER");
                row["EQUIPMENT_TYPE"] = GetGridCellText(gridRow, "EQUIPMENT_TYPE");
                row["POINT_NAME"] = GetGridCellText(gridRow, "POINT_NAME");
                row["POINT_CAL"] = GetGridCellText(gridRow, "POINT_CAL");
                row["CRITERIA_MIN"] = GetGridCellText(gridRow, "CRITERIA_MIN");
                row["CRITERIA_MAX"] = GetGridCellText(gridRow, "CRITERIA_MAX");
                row["UNIT"] = GetGridCellText(gridRow, "UNIT");
                row["JUDGE_TYPE"] = GetGridCellText(gridRow, "JUDGE_TYPE");
                result.Rows.Add(row);
            }

            return result;
        }

        private static string GetGridCellText(DataGridViewRow row, string columnName)
        {
            return row.Cells[columnName].Value?.ToString().Trim() ?? "";
        }

        private static bool IsEmptyEquipmentRow(DataGridViewRow row)
        {
            foreach (var columnName in new[] { "POINT_ORDER", "EQUIPMENT_TYPE", "POINT_NAME", "CRITERIA_MIN", "CRITERIA_MAX" })
            {
                if (!string.IsNullOrWhiteSpace(GetGridCellText(row, columnName)))
                {
                    return false;
                }
            }

            return true;
        }

        // ─── Validation & Save ────────────────────────────────────────────────────
        private bool ValidateBeforeSave()
        {
            if (string.IsNullOrWhiteSpace(txtMCode.Text))
            {
                MessageBox.Show("กรุณาระบุ M Code", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtMCode.Focus();
                return false;
            }

            bool nothingChecked = new[] { cboKeepData, cboPackingCheck, cboRegularCheck,
                                           cboFunctionCheck, cboDimensionCheck, cboAppearanceCheck }
                                  .All(c => GetComboValue(c) == VALUE_NO);

            if (nothingChecked)
            {
                var confirm = MessageBox.Show(
                    "M Code นี้ยังไม่ได้เลือก Check ใด ๆ เลย ต้องการบันทึกต่อหรือไม่?",
                    "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirm == DialogResult.No) return false;
            }

            if (!ValidateEquipmentGrid(dtgRegularEquipment, "Regular Equipment Set")) return false;
            if (!ValidateEquipmentGrid(dtgFunctionEquipment, "Function Equipment Set")) return false;
            if (!ValidateEquipmentGrid(dtgDimensionEquipment, "Dimension Equipment Set")) return false;
            if (!ValidateFunctionCheckGrid()) return false;

            int functionEquipmentCount = dtgFunctionEquipment.Rows
                .Cast<DataGridViewRow>()
                .Count(row => !row.IsNewRow && !IsEmptyEquipmentRow(row));
            if (functionEquipmentCount > 1)
            {
                MessageBox.Show("Function Equipment Set รองรับ Equipment ได้ 1 รายการต่อ M Code", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtgFunctionEquipment.Focus();
                return false;
            }

            if (!ConfirmJudgeTypeMismatch(dtgRegularEquipment, "Regular Equipment Set")) return false;
            if (!ConfirmJudgeTypeMismatch(dtgDimensionEquipment, "Dimension Equipment Set")) return false;

            return true;
        }

        /// <summary>
        /// ช่อง Judge บอกว่าจุดนั้นวัดเป็นตัวเลข หรือตัดสินผ่าน/ไม่ผ่านด้วย Jig หรือ Gauge
        /// เตือนเฉพาะกรณีที่ตั้ง Numeric ทั้งที่ Min = Max เพราะผู้ตรวจจะได้ช่องพิมพ์ตัวเลข
        /// แล้วมีค่าเดียวเท่านั้นที่ผ่าน แทบทุกครั้งคือตั้งใจจะให้เป็น Pass/Fail แต่ลืมเปลี่ยน
        /// (ตั้ง Pass/Fail ทั้งที่ Min ไม่เท่า Max ไม่เป็นไร เพราะ OK เก็บค่า Min ซึ่งอยู่ในเกณฑ์)
        /// </summary>
        private bool ConfirmJudgeTypeMismatch(DataGridView dtg, string title)
        {
            var mismatched = new List<string>();

            foreach (DataGridViewRow row in dtg.Rows)
            {
                if (row.IsNewRow || IsEmptyEquipmentRow(row)) continue;

                string minText = GetGridCellText(row, "CRITERIA_MIN");
                string maxText = GetGridCellText(row, "CRITERIA_MAX");

                if (!decimal.TryParse(minText, out decimal min)) continue;
                if (!decimal.TryParse(maxText, out decimal max)) continue;
                if (min != max) continue;
                if (Utilities.PointJudgeType.IsPassFail(row)) continue;

                string pointName = GetGridCellText(row, "POINT_NAME");
                mismatched.Add(string.IsNullOrWhiteSpace(pointName)
                    ? "Order " + GetGridCellText(row, "POINT_ORDER") + "  (" + minText + ")"
                    : pointName + "  (" + minText + ")");
            }

            if (mismatched.Count == 0) return true;

            string message = title + Environment.NewLine + Environment.NewLine +
                "จุดต่อไปนี้ตั้ง Min = Max แต่ช่อง Judge เป็น Numeric" + Environment.NewLine +
                "ผู้ตรวจจะได้ช่องพิมพ์ตัวเลข แล้วมีค่าเดียวเท่านั้นที่ผ่าน" + Environment.NewLine +
                "ถ้าตั้งใจให้เลือก OK / NG ให้เปลี่ยนช่อง Judge เป็น Pass/Fail" + Environment.NewLine + Environment.NewLine +
                string.Join(Environment.NewLine, mismatched);

            using (var frm = new frmConfirm(message, null, "ใช่ ตั้งใจ"))
            {
                return frm.ShowDialog(this) == DialogResult.Yes;
            }
        }

        private bool ValidateFunctionCheckGrid()
        {
            dtg_function_Check.EndEdit();
            var usedOrders = new HashSet<int>();

            foreach (DataGridViewRow row in dtg_function_Check.Rows)
            {
                if (row.IsNewRow || IsEmptyFunctionCheckRow(row)) continue;

                string orderText = GetGridCellText(row, "CHECK_ORDER");
                string detail = GetGridCellText(row, "CHECK_DETAIL");
                if (!int.TryParse(orderText, out int order) || order <= 0)
                {
                    MessageBox.Show("Function Check Method: Order ต้องเป็นเลขจำนวนเต็มมากกว่า 0",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtg_function_Check.CurrentCell = row.Cells["CHECK_ORDER"];
                    return false;
                }
                if (!usedOrders.Add(order))
                {
                    MessageBox.Show($"Function Check Method: Order {order} ซ้ำกัน",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtg_function_Check.CurrentCell = row.Cells["CHECK_ORDER"];
                    return false;
                }
                if (string.IsNullOrWhiteSpace(detail))
                {
                    MessageBox.Show("Function Check Method: กรุณาระบุวิธีการทดสอบ",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtg_function_Check.CurrentCell = row.Cells["CHECK_DETAIL"];
                    return false;
                }
                if (detail.Length > 255)
                {
                    MessageBox.Show("Function Check Method ยาวเกิน 255 ตัวอักษร",
                        "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtg_function_Check.CurrentCell = row.Cells["CHECK_DETAIL"];
                    return false;
                }
            }
            return true;
        }

        private bool ValidateEquipmentGrid(DataGridView dtg, string gridName)
        {
            dtg.EndEdit();

            foreach (DataGridViewRow row in dtg.Rows)
            {
                if (row.IsNewRow || IsEmptyEquipmentRow(row))
                {
                    continue;
                }

                string pointOrder = GetGridCellText(row, "POINT_ORDER");
                string equipmentType = GetGridCellText(row, "EQUIPMENT_TYPE");

                if (string.IsNullOrWhiteSpace(pointOrder))
                {
                    MessageBox.Show($"{gridName}: กรุณาระบุ Order", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtg.CurrentCell = row.Cells["POINT_ORDER"];
                    dtg.Focus();
                    return false;
                }

                if (!int.TryParse(pointOrder, out _))
                {
                    MessageBox.Show($"{gridName}: Order ต้องเป็นตัวเลข", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtg.CurrentCell = row.Cells["POINT_ORDER"];
                    dtg.Focus();
                    return false;
                }

                if (string.IsNullOrWhiteSpace(equipmentType))
                {
                    MessageBox.Show($"{gridName}: กรุณาเลือก Equipment", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtg.CurrentCell = row.Cells["EQUIPMENT_TYPE"];
                    dtg.Focus();
                    return false;
                }
            }

            return true;
        }

        private bool CheckMCodeInMES()
        {
            var dataItem = new SettingProperty { M_CODE = txtMCode.Text.Trim() };
            DataTable dt = _controller.SearchMCodeInMES(dataItem);

            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบ M Code นี้ใน MES กรุณาตรวจสอบหรือแจ้งฝ่ายที่เกี่ยวข้อง",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private void SaveInspectionSetting()
        {
            if (!ValidateBeforeSave() || !CheckMCodeInMES()) return;

            using (var frm = new frmConfirm("Are you sure you want to save ?"))
            {
                if (frm.ShowDialog(this) != DialogResult.Yes) return;
            }

            var dataItem = GetDataFromScreen();
            bool result = _controller.SaveInspectionSetting(dataItem);

            if (result)
            {
                result = _controller.SaveRegularEquipmentSetting(dataItem)
                      && _controller.SaveFunctionEquipmentSetting(dataItem)
                      && _controller.SaveFunctionCheckSetting(dataItem)
                      && _controller.SaveDimensionEquipmentSetting(dataItem);
            }

            if (result)
            {
                MessageBox.Show("Save Inspection Setting", "Success",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        // ─── Grid Setup ───────────────────────────────────────────────────────────
        private void SetupEquipmentGrid(DataGridView dtg)
        {
            dtg.AutoGenerateColumns = false;
            dtg.AllowUserToAddRows = true;
            dtg.AllowUserToDeleteRows = true;
            dtg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dtg.MultiSelect = false;
            dtg.RowHeadersVisible = false;

            dtg.EnableHeadersVisualStyles = false;
            var headerStyle = dtg.ColumnHeadersDefaultCellStyle;
            headerStyle.BackColor = Color.ForestGreen;
            headerStyle.ForeColor = Color.White;
            headerStyle.Font = dtg.Font;
            headerStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtg.ColumnHeadersHeight = 35;
            dtg.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            var cellStyle = dtg.DefaultCellStyle;
            cellStyle.Font = dtg.Font;
            cellStyle.ForeColor = SystemColors.ControlText;
            cellStyle.BackColor = SystemColors.Window;
            cellStyle.SelectionBackColor = Color.LightGreen;
            cellStyle.SelectionForeColor = Color.Black;

            dtg.Columns.Clear();
            dtg.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "M_CODE", HeaderText = "M Code", DataPropertyName = "M_CODE", Width = 100, ReadOnly = true, Visible = false },
                new DataGridViewTextBoxColumn { Name = "POINT_ORDER", HeaderText = "Order", DataPropertyName = "POINT_ORDER", Width = 60 },
                new DataGridViewComboBoxColumn
                {
                    Name = "EQUIPMENT_TYPE",
                    HeaderText = "Equipment",
                    DataPropertyName = "EQUIPMENT_TYPE",
                    DataSource = dtEquipmentType,
                    DisplayMember = "Equipment_Name",
                    ValueMember = "Equipment_Type",
                    // ชื่อเครื่องมือยาว เช่น Pin Gauge NO GO / Vernier Caliper
                    // ให้กินพื้นที่ที่เหลือจากคอลัมน์ความกว้างคงที่ ยืดตามขนาดหน้าต่าง
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                    MinimumWidth = 180,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                },
                new DataGridViewTextBoxColumn { Name = "POINT_NAME", HeaderText = "Point Name", DataPropertyName = "POINT_NAME", Width = 160 },
                new DataGridViewTextBoxColumn { Name = "POINT_CAL", HeaderText = "Point Cal", DataPropertyName = "POINT_CAL", Visible = false },
                new DataGridViewTextBoxColumn { Name = "CRITERIA_MIN", HeaderText = "Min", DataPropertyName = "CRITERIA_MIN", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "CRITERIA_MAX", HeaderText = "Max", DataPropertyName = "CRITERIA_MAX", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "UNIT", HeaderText = "Unit", DataPropertyName = "UNIT", Width = 60 },
                new DataGridViewComboBoxColumn
                {
                    // เดิมระบบเดาจาก Min = Max ว่าเป็นจุดตัดสิน OK/NG
                    // ทำเป็นช่องให้เลือกชัดเจน จะได้ย้อนดูได้ว่าตั้งใจให้วัดแบบไหน
                    Name = "JUDGE_TYPE",
                    HeaderText = "Judge",
                    DataPropertyName = "JUDGE_TYPE",
                    DataSource = dtJudgeType,
                    DisplayMember = "JUDGE_TYPE_NAME",
                    ValueMember = "JUDGE_TYPE",
                    Width = 110,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                },
                new DataGridViewButtonColumn
                {
                    Name = "ROW_ACTION",
                    HeaderText = "Delete",
                    Text = "Delete",
                    UseColumnTextForButtonValue = true,
                    Width = 70
                }
            );

            if (dtg == dtgDimensionEquipment || dtg == dtgRegularEquipment)
            {
                // เกณฑ์เก็บ decimal(12,6) ตัดศูนย์ท้ายทิ้ง เช่น 1.150000 -> 1.15
                // รับถึง 6 ตำแหน่งเท่าความละเอียดจริงของคอลัมน์
                // ช่องแก้ไขจึงไม่มีทางโดนปัดค่าทิ้งตอน Save
                dtg.Columns["CRITERIA_MIN"].DefaultCellStyle.Format = Utilities.NumberDisplay.GridFormat;
                dtg.Columns["CRITERIA_MAX"].DefaultCellStyle.Format = Utilities.NumberDisplay.GridFormat;
            }

            if (dtg == dtgFunctionEquipment)
            {
                dtg.Columns["POINT_ORDER"].Visible = false;
                dtg.Columns["POINT_NAME"].Visible = false;
                dtg.Columns["CRITERIA_MIN"].Visible = false;
                dtg.Columns["CRITERIA_MAX"].Visible = false;
                dtg.Columns["UNIT"].Visible = false;
                // Function ตรวจด้วยหัวข้อ Check Method ไม่มีจุดวัดที่ตัดสินเป็น OK/NG
                // ตาราง info_function_equipment จึงไม่มีคอลัมน์นี้
                dtg.Columns["JUDGE_TYPE"].Visible = false;
            }
        }

        private void SetupFunctionCheckGrid()
        {
            dtg_function_Check.AutoGenerateColumns = false;
            dtg_function_Check.AllowUserToAddRows = true;
            dtg_function_Check.AllowUserToDeleteRows = true;
            dtg_function_Check.RowHeadersVisible = false;
            dtg_function_Check.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dtg_function_Check.MultiSelect = false;
            dtg_function_Check.EnableHeadersVisualStyles = false;
            dtg_function_Check.ColumnHeadersDefaultCellStyle.BackColor = Color.ForestGreen;
            dtg_function_Check.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dtg_function_Check.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtg_function_Check.ColumnHeadersHeight = 35;
            // ต้องปิด AutoSize ด้วย ไม่งั้นความสูงที่ตั้งไว้จะถูกทับกลับเป็นค่า default
            // (ตารางเครื่องมือตัวอื่นปิดไว้แล้วที่ SetupEquipmentGrid หัวจึงสูง 35 จริง)
            dtg_function_Check.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dtg_function_Check.Columns.Clear();
            dtg_function_Check.Columns.AddRange(
                new DataGridViewTextBoxColumn { Name = "ID", DataPropertyName = "ID", Visible = false },
                new DataGridViewTextBoxColumn { Name = "M_CODE", DataPropertyName = "M_CODE", Visible = false },
                new DataGridViewTextBoxColumn { Name = "CHECK_ORDER", HeaderText = "Order", DataPropertyName = "CHECK_ORDER", Width = 52 },
                new DataGridViewTextBoxColumn { Name = "CHECK_DETAIL", HeaderText = "Test Method", DataPropertyName = "CHECK_DETAIL", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill },
                new DataGridViewButtonColumn { Name = "ROW_ACTION", HeaderText = "", Text = "Delete", UseColumnTextForButtonValue = true, Width = 55 }
            );
        }

        private void dtgRegularEquipment_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || e.Value == null) return;

            string columnName = dtgRegularEquipment.Columns[e.ColumnIndex].Name;
            if (columnName != "CRITERIA_MIN" && columnName != "CRITERIA_MAX") return;

            if (decimal.TryParse(e.Value.ToString(), out decimal value))
            {
                e.Value = value.ToString("0.000###");
                e.FormattingApplied = true;
            }
        }

        private void dtgFunctionCheck_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            int nextOrder = 1;
            foreach (DataGridViewRow row in dtg_function_Check.Rows)
            {
                if (!row.IsNewRow && int.TryParse(GetGridCellText(row, "CHECK_ORDER"), out int order))
                {
                    nextOrder = Math.Max(nextOrder, order + 1);
                }
            }
            e.Row.Cells["CHECK_ORDER"].Value = nextOrder;
        }

        private void dtgFunctionCheck_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dtg_function_Check.Columns[e.ColumnIndex].Name != "ROW_ACTION" ||
                dtg_function_Check.Rows[e.RowIndex].IsNewRow) return;
            DeleteEquipmentRow(dtg_function_Check, e.RowIndex);
        }

        /// <summary>
        /// แถวใหม่ต้องมีค่า Judge ตั้งไว้ก่อน ไม่งั้น ComboBox จะว่างแล้วคนตั้งค่าไม่รู้ว่าต้องเลือก
        /// ค่าเริ่มต้นคือวัดเป็นตัวเลข ตรงกับ DEFAULT ของคอลัมน์ในฐานข้อมูล
        /// </summary>
        private void dtgEquipment_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            // ต้องหยิบค่าจาก DataSource ของ ComboBox มาตรงๆ
            // ถ้าใส่ int ธรรมดาจะไม่ตรงชนิดกับ ValueMember (smallint -> Int16) แล้วช่องจะว่าง
            foreach (DataRow judge in dtJudgeType.Rows)
            {
                if (Convert.ToInt32(judge["JUDGE_TYPE"]) != Utilities.PointJudgeType.Numeric) continue;

                e.Row.Cells["JUDGE_TYPE"].Value = judge["JUDGE_TYPE"];
                return;
            }
        }

        private void dtgFunctionEquipment_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells["POINT_ORDER"].Value = "1";
            e.Row.Cells["POINT_NAME"].Value = "Function";
            e.Row.Cells["POINT_CAL"].Value = "0";
        }

        private void dtgEquipment_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var dtg = sender as DataGridView;
            if (dtg == null || dtg.Columns[e.ColumnIndex].Name != "ROW_ACTION") return;
            if (dtg.Rows[e.RowIndex].IsNewRow) return;

            DeleteEquipmentRow(dtg, e.RowIndex);
        }

        private void DeleteEquipmentRow(DataGridView dtg, int rowIndex)
        {
            string itemName = dtg == dtg_function_Check ? "วิธีการทดสอบ Function" : "Equipment";
            var confirm = MessageBox.Show($"ต้องการลบ {itemName} แถวนี้หรือไม่?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            dtg.Rows.RemoveAt(rowIndex);
        }

        private void dtgEquipment_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            string itemName = sender == dtg_function_Check ? "วิธีการทดสอบ Function" : "Equipment";
            var confirm = MessageBox.Show($"ต้องการลบ {itemName} แถวนี้หรือไม่?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            e.Cancel = confirm != DialogResult.Yes;
        }

        private void LoadEquipmentTypeList()
        {
            dtEquipmentType = _controller.GetEquipmentTypeList() ?? new DataTable();
            dtJudgeType = _controller.GetJudgeTypeList() ?? new DataTable();
        }

        private void LoadEquipmentSetting(string mCode)
        {
            if (string.IsNullOrWhiteSpace(mCode))
            {
                dtgRegularEquipment.DataSource = null;
                dtgFunctionEquipment.DataSource = null;
                dtg_function_Check.DataSource = null;
                dtgDimensionEquipment.DataSource = null;
                return;
            }

            var dataItem = new SettingProperty { M_CODE = mCode.Trim() };

            dtRegularEquipment = _controller.SearchRegularEquipmentSetting(dataItem);
            dtFunctionEquipment = _controller.SearchFunctionEquipmentSetting(dataItem);
            dtFunctionChecks = _controller.SearchFunctionCheckSetting(dataItem);
            dtDimensionEquipment = _controller.SearchDimensionEquipmentSetting(dataItem);

            if (dtRegularEquipment == null) dtRegularEquipment = CreateEquipmentTable();
            if (dtFunctionEquipment == null) dtFunctionEquipment = CreateEquipmentTable();
            if (dtFunctionChecks == null) dtFunctionChecks = CreateFunctionCheckTable();
            if (dtDimensionEquipment == null) dtDimensionEquipment = CreateEquipmentTable();

            dtgRegularEquipment.DataSource = dtRegularEquipment;
            dtgFunctionEquipment.DataSource = dtFunctionEquipment;
            dtg_function_Check.DataSource = dtFunctionChecks;
            dtgDimensionEquipment.DataSource = dtDimensionEquipment;
        }

        // ─── Inspection Level Rules ───────────────────────────────────────────────
        /// <summary>ปรับ Control ทุกตัวที่ขึ้นกับ Inspection Level — Strictness + หน่วยของ Qty</summary>
        private static void ApplyInspectionLevelRules(TabDescriptor tab)
        {
            SetStrictnessFieldStatus(tab.CboInspectionLevel, tab.CboNormalReduce, tab.CboS1);
            SetQtyUnitLabel(tab);
        }

        /// <summary>ตัวเลือกจำนวนต่อ cavity — ค่าที่ใช้จริงทั้งระบบมีแค่ 0 / 1 / 2</summary>
        private class QtyChoice
        {
            public readonly string Value;
            private readonly string _text;

            public QtyChoice(string value, string text)
            {
                Value = value;
                _text = text;
            }

            public override string ToString()
            {
                return _text;
            }
        }

        /// <summary>วาง Dropdown ทับตำแหน่งช่องกรอก สลับโชว์ทีละตัวตาม Inspection Type</summary>
        private static ComboBox CreateQtyCombo(TextBox anchor)
        {
            var combo = new ComboBox
            {
                Name = anchor.Name + "Combo",
                Location = anchor.Location,
                Size = anchor.Size,
                Font = anchor.Font,
                DropDownStyle = ComboBoxStyle.DropDownList,
                Visible = false
            };

            // ข้อความจริงตั้งตอนเลือก Inspection Type เพราะสองแบบเขียนไม่เหมือนกัน
            // เอกสารจริงเขียน Pc/Cavity ว่า "1Pc./Cavity" ส่วน Sampling Table ว่า "Cavity ≥2 Pcs."
            SetQtyChoiceText(combo, false);

            anchor.Parent.Controls.Add(combo);
            combo.BringToFront();

            return combo;
        }

        /// <summary>
        /// Pc/Cavity เป็นจำนวนตายตัว ส่วน Sampling Table เป็นขั้นต่ำที่ต้องไปชนกับตาราง AQL อีกที
        /// เอกสารจริงจึงเขียนคนละแบบ ตัวเลือกต้องสะกดให้ตรงกัน ไม่งั้นคนตั้งค่าเข้าใจผิด
        /// </summary>
        private static void SetQtyChoiceText(ComboBox combo, bool isMinimum)
        {
            if (combo == null) return;

            string selected = (combo.SelectedItem as QtyChoice)?.Value;

            combo.Items.Clear();
            combo.Items.Add(new QtyChoice("0", "ไม่มี Cavity"));

            for (int qty = 1; qty <= 2; qty++)
            {
                string unit = qty == 1 ? "Pc." : "Pcs.";

                combo.Items.Add(new QtyChoice(
                    qty.ToString(),
                    isMinimum ? $"Cavity ≥ {qty} {unit}" : $"{qty} {unit} / Cavity"));
            }

            if (selected == null) return;

            for (int i = 0; i < combo.Items.Count; i++)
            {
                if (((QtyChoice)combo.Items[i]).Value == selected)
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        /// <summary>อ่านค่าจากตัวที่กำลังโชว์อยู่ ไม่ใช่จากช่องกรอกเสมอ</summary>
        private static string GetSamplingQty(TabDescriptor tab)
        {
            if (tab.CboInspectionQty != null && tab.CboInspectionQty.Visible)
            {
                var choice = tab.CboInspectionQty.SelectedItem as QtyChoice;
                return choice == null ? "0" : choice.Value;
            }

            return GetNumberValueFromTextBox(tab.TxtInspectionQty);
        }

        /// <summary>ใส่ค่าเดิมลงทั้งสองตัว จะได้สลับไปมาแล้วค่าไม่หาย</summary>
        private static void SetSamplingQty(TabDescriptor tab, string value)
        {
            tab.TxtInspectionQty.Text = value;

            if (tab.CboInspectionQty == null) return;

            string wanted = value == DISPLAY_DASH ? "0" : value.Trim();
            tab.CboInspectionQty.SelectedIndex = -1;

            for (int i = 0; i < tab.CboInspectionQty.Items.Count; i++)
            {
                if (((QtyChoice)tab.CboInspectionQty.Items[i]).Value == wanted)
                {
                    tab.CboInspectionQty.SelectedIndex = i;
                    return;
                }
            }
        }

        /// <summary>
        /// ตัวเลขในช่องเดียวกันหมายความคนละอย่างตาม Inspection Type ที่เลือก
        /// ถ้าไม่บอกหน่วยไว้ คนตั้งค่าจะแยกไม่ออกว่าใส่จำนวนชิ้น จำนวนต่อ cavity หรือเปอร์เซ็นต์
        /// </summary>
        private static void SetQtyUnitLabel(TabDescriptor tab)
        {
            if (tab.LbQtyUnit == null) return;

            string level = tab.CboInspectionLevel.Text.Trim();
            string unit;
            bool useQty = true;
            bool usePerCavity = false;

            if (level.Equals(SAMPLING_FIX_TXT, StringComparison.OrdinalIgnoreCase))
            {
                unit = UNIT_PCS;
            }
            else if (level.Equals(SAMPLING_CAVITY_TXT, StringComparison.OrdinalIgnoreCase))
            {
                // Pc/Cavity ไม่ใช้ตาราง AQL เลย จำนวนที่ต้องเก็บ = Cavity_Qty x ค่าในช่องนี้ ตรงๆ
                // ยืนยันจาก DB : type 4 ทุกตัวตั้ง Strictness_Type/Level เป็น 0 (N/A)
                unit = UNIT_PCS_PER_CAVITY;
                usePerCavity = true;
            }
            else if (level.Equals(SAMPLING_TABLE_TXT, StringComparison.OrdinalIgnoreCase))
            {
                // Sampling Table เอาค่านี้ไปชนกับตาราง AQL อีกชั้น (Math.Max)
                // เลขในช่องจึงเป็นแค่ขั้นต่ำ ผลจริงอาจมากกว่าถ้าตารางสั่งมากกว่า
                unit = UNIT_PCS_PER_CAVITY_MIN;
                usePerCavity = true;
            }
            else if (level.Equals(SAMPLING_PERCENT_TXT, StringComparison.OrdinalIgnoreCase))
            {
                unit = UNIT_PERCENT;
            }
            else
            {
                // All = ตรวจทุกชิ้นใน Lot , N/A = ยังไม่ได้ตั้งค่า ทั้งสองแบบไม่ใช้ช่องนี้
                unit = level.Equals(SAMPLING_ALL_TXT, StringComparison.OrdinalIgnoreCase)
                    ? "ตรวจทุกชิ้น"
                    : "";
                useQty = false;
            }

            tab.LbQtyUnit.Text = unit;

            if (usePerCavity)
            {
                SetQtyChoiceText(tab.CboInspectionQty, unit == UNIT_PCS_PER_CAVITY_MIN);
            }

            // จำนวนต่อ cavity มีแค่ 0/1/2 ให้เลือกจาก Dropdown กันพิมพ์ผิด
            // ส่วน Fix กับ % เป็นตัวเลขอิสระ ต้องพิมพ์เอง
            if (tab.CboInspectionQty != null)
            {
                tab.CboInspectionQty.Visible = useQty && usePerCavity;
            }

            if (tab.TxtInspectionQty == null) return;

            tab.TxtInspectionQty.Visible = !usePerCavity;
            tab.TxtInspectionQty.Enabled = useQty;

            if (!useQty)
            {
                tab.TxtInspectionQty.Text = "";
            }
        }

        private static void SetStrictnessFieldStatus(ComboBox cboLevel, ComboBox cboNormalReduce, ComboBox cboS1)
        {
            bool isSamplingTable = cboLevel.Text.Trim().Equals(SAMPLING_TABLE_TXT, StringComparison.OrdinalIgnoreCase)
                                || (cboLevel.SelectedValue?.ToString().Trim()
                                       .Equals(SAMPLING_TABLE_TXT, StringComparison.OrdinalIgnoreCase) ?? false);

            cboNormalReduce.Enabled = isSamplingTable;
            cboS1.Enabled = isSamplingTable;

            if (!isSamplingTable)
            {
                cboNormalReduce.SelectedIndex = -1;
                cboS1.SelectedIndex = -1;
            }
        }

        // ─── Helper Methods ───────────────────────────────────────────────────────
        private static string DisplayDashIfZeroOrEmpty(object value)
        {
            if (value == null || value == DBNull.Value) return DISPLAY_DASH;
            string text = value.ToString().Trim();
            return string.IsNullOrWhiteSpace(text) || text == "0" ? DISPLAY_DASH : text;
        }

        private static string GetFirstCavityValue(DataRow row, params string[] columnNames)
        {
            foreach (string col in columnNames)
            {
                if (!row.Table.Columns.Contains(col)) continue;
                string text = row[col]?.ToString().Trim();
                if (!string.IsNullOrWhiteSpace(text) && text != "0" && text != DISPLAY_DASH)
                    return text;
            }
            return DISPLAY_DASH;
        }

        private static string GetNumberValueFromTextBox(TextBox txt)
        {
            string text = txt.Text.Trim();
            return string.IsNullOrWhiteSpace(text) || text == DISPLAY_DASH ? "0" : text;
        }

        private static string GetTextValueFromTextBox(TextBox txt)
        {
            string text = txt.Text.Trim();
            return text == DISPLAY_DASH ? "" : text;
        }

        private static string NormalizeStatusValue(object value)
        {
            if (value == null || value == DBNull.Value) return VALUE_ACTIVE;
            string text = value.ToString().Trim();

            return (text == "1"
                || text.Equals("Active", StringComparison.OrdinalIgnoreCase)
                || text.Equals("Yes", StringComparison.OrdinalIgnoreCase))
                ? VALUE_ACTIVE : VALUE_NO;
        }

        // ─── Event Handlers ───────────────────────────────────────────────────────
        private void btnSave_Click_1(object sender, EventArgs e) => SaveInspectionSetting();
        private void btnCancel_Click(object sender, EventArgs e) { this.DialogResult = DialogResult.Cancel; this.Close(); }
        private void dtgEquipment_DataError(object sender, DataGridViewDataErrorEventArgs e) => e.ThrowException = false;

        private void txtMCode_Leave(object sender, EventArgs e)
        {
            if (!_isEditMode && !string.IsNullOrWhiteSpace(txtMCode.Text))
                CheckMCodeInMES();
        }

        private TabPage GetFirstEnabledTab()
        {
            return new[] { tabRegularCheckDetails, tabFunctionCheckDetails,
                           tabDimensionCheckDetails, tabAppearanceCheckDetails }
                   .FirstOrDefault(t => t.Enabled);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
