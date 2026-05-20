using RawMat.Controllers;
using RawMat.Property;
using RawMat.SQLFactory;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RawMat.Views.Setting
{
    public partial class frmMCodeInspectionSetting : Form
    {
        // ─── Constants ────────────────────────────────────────────────────────────
        private const string VALUE_YES = "1";
        private const string VALUE_NO = "0";
        private const string VALUE_ACTIVE = "1";
        private const string DISPLAY_DASH = "-";
        private const string SAMPLING_TABLE_TXT = "Sampling Table";

        // ─── Fields ───────────────────────────────────────────────────────────────
        private readonly SettingControllers _controller = new SettingControllers();
        private DataTable dtRegularEquipment = new DataTable();
        private DataTable dtDimensionEquipment = new DataTable();
        private DataTable dtEquipmentType = new DataTable();

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
            BindAllCombos();
            BindEvents();
            ClearInput();

            LoadEquipmentTypeList();
            SetupEquipmentGrid(dtgRegularEquipment);
            SetupEquipmentGrid(dtgDimensionEquipment);

            dtgRegularEquipment.DataError += dtgEquipment_DataError;
            dtgDimensionEquipment.DataError += dtgEquipment_DataError;
            dtgRegularEquipment.CellContentClick += dtgEquipment_CellContentClick;
            dtgDimensionEquipment.CellContentClick += dtgEquipment_CellContentClick;
            dtgRegularEquipment.UserDeletingRow += dtgEquipment_UserDeletingRow;
            dtgDimensionEquipment.UserDeletingRow += dtgEquipment_UserDeletingRow;

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
                dtDimensionEquipment = CreateEquipmentTable();
                dtgRegularEquipment.DataSource = dtRegularEquipment;
                dtgDimensionEquipment.DataSource = dtDimensionEquipment;
            }

            SetAllCheckTabStatus();

            foreach (var tab in _tabs)
            {
                SetStrictnessFieldStatus(tab.CboInspectionLevel, tab.CboNormalReduce, tab.CboS1);
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
            // Yes/No combos
            foreach (var cbo in new[] { cboKeepData, cboRegularCheck, cboFunctionCheck,
                                         cboDimensionCheck, cboAppearanceCheck, cboPackingCheck })
            {
                BindSimpleCombo(cbo, new[] { ("No", VALUE_NO), ("Yes", VALUE_YES) });
            }

            BindSimpleCombo(cboStatus, new[] { ("Active", VALUE_ACTIVE), ("InActive", VALUE_NO) });

            // Master lists from DB
            DataTable dtSamplingType = _controller.GetSamplingTypeList();
            DataTable dtStrictnessType = _controller.GetStrictnessTypeList();
            DataTable dtStrictnessLevel = _controller.GetStrictnessLevelList();

            // TODO: Remove debug MessageBox ก่อน deploy จริง
            MessageBox.Show(
                $"dtStrictnessType null = {dtStrictnessType == null}\n" +
                $"Rows = {dtStrictnessType?.Rows.Count ?? 0}\n" +
                $"Columns = {(dtStrictnessType == null ? "" : string.Join(", ", dtStrictnessType.Columns.Cast<DataColumn>().Select(c => c.ColumnName)))}");

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

            // Strictness enable/disable ตาม Inspection Level
            foreach (var t in _tabs)
            {
                var captured = t;
                captured.CboInspectionLevel.SelectedIndexChanged += (_, __) =>
                    SetStrictnessFieldStatus(captured.CboInspectionLevel,
                                            captured.CboNormalReduce,
                                            captured.CboS1);
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
            return value == VALUE_YES || cbo.Text.Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase);
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
            SetComboValue(cboKeepData, row["Keep Data"].ToString());
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

            // 4 Tabs โดย loop
            foreach (var tab in _tabs)
            {
                SetComboValue(tab.CboInspectionLevel, row[tab.SamplingTypeField].ToString());
                tab.TxtInspectionQty.Text = DisplayDashIfZeroOrEmpty(row[tab.SamplingQtyField]);
                SetComboValue(tab.CboNormalReduce, row[tab.StrictnessTypeField].ToString());
                SetComboValue(tab.CboS1, row[tab.StrictnessLevelField].ToString());
            }
        }

        // ─── Get/Set Screen Values ────────────────────────────────────────────────
        private static void SetComboValue(ComboBox cbo, string value)
        {
            if (cbo.DataSource == null)
            {
                cbo.Text = value ?? "";
                return;
            }

            // Normalize Yes/No → 1/0
            value = value?.Trim() ?? VALUE_NO;
            if (value.Equals("Yes", StringComparison.OrdinalIgnoreCase)) value = VALUE_YES;
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
                Reg_Sampling_Qty = GetNumberValueFromTextBox(txtInspectionQtytab1),
                Reg_Strictness_Type = GetComboValue(cboNormalReducetab1),
                Reg_Strictness_Level = GetComboValue(cboS1tab1),
                Reg_Cavity_Name = commonName,

                // Function
                Func_Cavity_Qty = commonQty,
                Func_Sampling_Type = GetComboValue(cboInscpectionLeveltab2),
                Func_Sampling_Qty = GetNumberValueFromTextBox(txtInspectionQtytab2),
                Func_Strictness_Type = GetComboValue(cboNormalReducetab2),
                Func_Strictness_Level = GetComboValue(cboS1tab2),
                Func_Cavity_Name = commonName,

                // Dimension
                Dim_Cavity_Qty = commonQty,
                Dim_Sampling_Type = GetComboValue(cboInscpectionLeveltab3),
                Dim_Sampling_Qty = GetNumberValueFromTextBox(txtInspectionQtytab3),
                Dim_Strictness_Type = GetComboValue(cboNormalReducetab3),
                Dim_Strictness_Level = GetComboValue(cboS1tab3),
                Dim_Cavity_Name = commonName,

                // Appearance
                App_Cavity_Qty = commonQty,
                App_Sampling_Type = GetComboValue(cboInscpectionLeveltab4),
                App_Sampling_Qty = GetNumberValueFromTextBox(txtInspectionQtytab4),
                App_Strictness_Type = GetComboValue(cboNormalReducetab4),
                App_Strictness_Level = GetComboValue(cboS1tab4),
                App_Cavity_Name = commonName,

                RegularEquipment = GetEquipmentTableFromGrid(dtgRegularEquipment),
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
            return dt;
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
            foreach (var columnName in new[] { "POINT_ORDER", "EQUIPMENT_TYPE", "POINT_NAME", "POINT_CAL", "CRITERIA_MIN", "CRITERIA_MAX" })
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
            if (!ValidateEquipmentGrid(dtgDimensionEquipment, "Dimension Equipment Set")) return false;

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
                    Width = 180,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
                },
                new DataGridViewTextBoxColumn { Name = "POINT_NAME", HeaderText = "Point Name", DataPropertyName = "POINT_NAME", Width = 160 },
                new DataGridViewTextBoxColumn { Name = "POINT_CAL", HeaderText = "Point Cal", DataPropertyName = "POINT_CAL", Width = 120 },
                new DataGridViewTextBoxColumn { Name = "CRITERIA_MIN", HeaderText = "Min", DataPropertyName = "CRITERIA_MIN", Width = 80 },
                new DataGridViewTextBoxColumn { Name = "CRITERIA_MAX", HeaderText = "Max", DataPropertyName = "CRITERIA_MAX", Width = 80 },
                new DataGridViewButtonColumn { Name = "DELETE_ROW", HeaderText = "", Text = "Delete", UseColumnTextForButtonValue = true, Width = 70 }
            );
        }

        private void dtgEquipment_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var dtg = sender as DataGridView;
            if (dtg == null || dtg.Columns[e.ColumnIndex].Name != "DELETE_ROW") return;
            if (dtg.Rows[e.RowIndex].IsNewRow) return;

            var confirm = MessageBox.Show("ต้องการลบ Equipment แถวนี้หรือไม่?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            dtg.Rows.RemoveAt(e.RowIndex);
        }

        private void dtgEquipment_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            var confirm = MessageBox.Show("ต้องการลบ Equipment แถวนี้หรือไม่?",
                "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            e.Cancel = confirm != DialogResult.Yes;
        }

        private void LoadEquipmentTypeList()
        {
            dtEquipmentType = _controller.GetEquipmentTypeList() ?? new DataTable();
        }

        private void LoadEquipmentSetting(string mCode)
        {
            if (string.IsNullOrWhiteSpace(mCode))
            {
                dtgRegularEquipment.DataSource = null;
                dtgDimensionEquipment.DataSource = null;
                return;
            }

            var dataItem = new SettingProperty { M_CODE = mCode.Trim() };

            dtRegularEquipment = _controller.SearchRegularEquipmentSetting(dataItem);
            dtDimensionEquipment = _controller.SearchDimensionEquipmentSetting(dataItem);

            if (dtRegularEquipment == null) dtRegularEquipment = CreateEquipmentTable();
            if (dtDimensionEquipment == null) dtDimensionEquipment = CreateEquipmentTable();

            dtgRegularEquipment.DataSource = dtRegularEquipment;
            dtgDimensionEquipment.DataSource = dtDimensionEquipment;
        }

        // ─── Strictness Field Status ──────────────────────────────────────────────
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
    }
}
