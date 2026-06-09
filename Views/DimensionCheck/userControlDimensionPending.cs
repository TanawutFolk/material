using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.CustomMsg;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static RawMat.Property.QAdataProperty;

namespace RawMat.Views.DimensionCheck
{
    public partial class userControlDimensionPending : UserControl
    {
        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler BackToARequested;

        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        private readonly SettingControllers settingController = new SettingControllers();

        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();

        private readonly BindingSource bindingSource = new BindingSource();
        private DataTable originalDataTable;

        private int currentPage = 1;
        private int totalPages = 1;

        private bool isUpdating = false;

        // Key = CAVITY_NAME + SAMPLING_NO + POINT_ORDER
        private readonly Dictionary<string, decimal> pointValues = new Dictionary<string, decimal>();

        private List<Image> dimensionImages;
        private int currentDimensionImageIndex = 0;
        private readonly Dictionary<string, DataTable> equipmentSerialSourceByType = new Dictionary<string, DataTable>();

        public userControlDimensionPending()
        {
            InitializeComponent();
        }

        private void userControlDimensionPending_Load(object sender, EventArgs e)
        {
            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_sampName.Text = propQA.SAMPLING_NAME == "Fix"
                ? $"Quantity {propQA.SAMPLING_QTY} Pcs."
                : $"{propQA.SAMPLING_QTY} {propQA.SAMPLING_NAME}";

            DetachGridEvents();

            DataTable dtDimPending = conQA.SearchDimensionDataPending(propQA);

            if (dtDimPending == null || dtDimPending.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบ data Dimension ที่ Pending ใน db_Dimension_data");
                return;
            }

            originalDataTable = BuildDimensionDataTable(dtDimPending);

            bindingSource.DataSource = originalDataTable;
            dtg_dimension.DataSource = bindingSource;

            ConfigureGrid();
            AttachGridEvents();

            totalPages = originalDataTable.AsEnumerable()
                .Select(row => row["POINT_ORDER"].ToString())
                .Distinct()
                .Count();

            if (totalPages <= 0)
            {
                totalPages = 1;
            }

            currentPage = 1;
            ShowPage(currentPage);

            this.Disposed -= UserControlDimension_Disposed;
            this.Disposed += UserControlDimension_Disposed;

            this.Focus();
        }

        private DataTable BuildDimensionDataTable(DataTable dtDimPending)
        {
            DataTable dtAllSum = new DataTable();

            bool hasCavity = int.TryParse(propQA.CAVITY_QTY, out int cavityQty) &&
                             cavityQty > 0 &&
                             dtDimPending != null &&
                             dtDimPending.Columns.Contains("CAVITY_NAME") &&
                             dtDimPending.AsEnumerable().Any(row =>
                                 !row.IsNull("CAVITY_NAME") &&
                                 !string.IsNullOrWhiteSpace(row["CAVITY_NAME"].ToString()));

            if (hasCavity)
            {
                dtAllSum.Columns.Add("CAVITY_NAME", typeof(string));
            }
            else
            {
                gb_cavity.Visible = false;

                picbox_dim.Location = new Point(231, 120);
                picbox_dim.Size = new Size(815, 442);
            }

            dtAllSum.Columns.Add("SAMPLING_NO", typeof(int));
            dtAllSum.Columns.Add("POINT_ORDER", typeof(string));
            dtAllSum.Columns.Add("POINT_CAL", typeof(string));
            dtAllSum.Columns.Add("EQUIPMENT_SERIAL", typeof(string));
            dtAllSum.Columns.Add("EQUIPMENT_TYPE", typeof(string));
            dtAllSum.Columns.Add("EQUIPMENT_NAME", typeof(string));
            dtAllSum.Columns.Add("POINT_NAME", typeof(string));
            dtAllSum.Columns.Add("VALUE", typeof(string));
            dtAllSum.Columns.Add("CRITERIA_MIN", typeof(double));
            dtAllSum.Columns.Add("CRITERIA_MAX", typeof(double));
            dtAllSum.Columns.Add("POINT_JUDGE", typeof(string));
            dtAllSum.Columns.Add("TOTAL_JUDGE", typeof(string));

            foreach (DataRow dtRow in dtDimPending.Rows)
            {
                if (hasCavity)
                {
                    dtAllSum.Rows.Add(
                        GetString(dtRow, "CAVITY_NAME"),
                        ToInt(GetString(dtRow, "SAMPLING_NO")),
                        GetString(dtRow, "POINT_ORDER"),
                        GetString(dtRow, "POINT_CAL"),
                        GetString(dtRow, "EQUIPMENT_SERIAL_ID"),
                        GetString(dtRow, "EQUIPMENT_TYPE"),
                        GetString(dtRow, "EQUIPMENT_NAME"),
                        GetString(dtRow, "POINT_NAME"),
                        GetString(dtRow, "VALUE"),
                        ToDouble(dtRow["CRITERIA_MIN"]),
                        ToDouble(dtRow["CRITERIA_MAX"]),
                        "",
                        ""
                    );
                }
                else
                {
                    dtAllSum.Rows.Add(
                        ToInt(GetString(dtRow, "SAMPLING_NO")),
                        GetString(dtRow, "POINT_ORDER"),
                        GetString(dtRow, "POINT_CAL"),
                        GetString(dtRow, "EQUIPMENT_SERIAL_ID"),
                        GetString(dtRow, "EQUIPMENT_TYPE"),
                        GetString(dtRow, "EQUIPMENT_NAME"),
                        GetString(dtRow, "POINT_NAME"),
                        GetString(dtRow, "VALUE"),
                        ToDouble(dtRow["CRITERIA_MIN"]),
                        ToDouble(dtRow["CRITERIA_MAX"]),
                        "",
                        ""
                    );
                }
            }

            if (hasCavity)
            {
                picbox_cavity.Image = imgCls.LoadCavityImage(propQA.M_CODE);
            }

            picbox_dim.Image = imgCls.LoadDimensionImage(propQA.M_CODE);

            return dtAllSum;
        }

        private void ConfigureGrid()
        {
            string[] hiddenColumns =
            {
                "POINT_CAL",
                "POINT_ORDER",
                "EQUIPMENT_TYPE",
                "POINT_JUDGE",
                "TOTAL_JUDGE"
            };

            foreach (string col in hiddenColumns)
            {
                if (dtg_dimension.Columns.Contains(col))
                {
                    dtg_dimension.Columns[col].Visible = false;
                }
            }

            foreach (DataGridViewColumn column in dtg_dimension.Columns)
            {
                column.ReadOnly = column.Name != "VALUE" && column.Name != "EQUIPMENT_SERIAL";
            }

            if (dtg_dimension.Columns.Contains("CAVITY_NAME"))
            {
                dtg_dimension.Columns["CAVITY_NAME"].HeaderText = "CAV.";
            }

            if (dtg_dimension.Columns.Contains("SAMPLING_NO"))
            {
                dtg_dimension.Columns["SAMPLING_NO"].HeaderText = "SAMPLE";
            }

            if (dtg_dimension.Columns.Contains("POINT_NAME"))
            {
                dtg_dimension.Columns["POINT_NAME"].HeaderText = "CHECKPOINT";
            }

            if (dtg_dimension.Columns.Contains("EQUIPMENT_SERIAL"))
            {
                dtg_dimension.Columns["EQUIPMENT_SERIAL"].HeaderText = "EQ_SN";
            }

            if (dtg_dimension.Columns.Contains("EQUIPMENT_NAME"))
            {
                dtg_dimension.Columns["EQUIPMENT_NAME"].HeaderText = "EQ_NAME";
            }

            if (dtg_dimension.Columns.Contains("CRITERIA_MIN"))
            {
                dtg_dimension.Columns["CRITERIA_MIN"].HeaderText = "MIN";
            }

            if (dtg_dimension.Columns.Contains("CRITERIA_MAX"))
            {
                dtg_dimension.Columns["CRITERIA_MAX"].HeaderText = "MAX";
            }

            UpdateReadOnlyCells();
        }

        private void AttachGridEvents()
        {
            dtg_dimension.CellEndEdit += dtg_dimension_CellEndEdit;
            dtg_dimension.CellValidating += dtg_dimension_CellValidating;
            dtg_dimension.CellFormatting += dtg_dimension_CellFormatting;
            dtg_dimension.DataBindingComplete += dtg_dimension_DataBindingComplete;
            dtg_dimension.CellValueChanged += dtg_dimension_CellValueChanged;
            dtg_dimension.CurrentCellDirtyStateChanged += dtg_dimension_CurrentCellDirtyStateChanged;
        }

        private void DetachGridEvents()
        {
            dtg_dimension.CellEndEdit -= dtg_dimension_CellEndEdit;
            dtg_dimension.CellValidating -= dtg_dimension_CellValidating;
            dtg_dimension.CellFormatting -= dtg_dimension_CellFormatting;
            dtg_dimension.DataBindingComplete -= dtg_dimension_DataBindingComplete;
            dtg_dimension.CellValueChanged -= dtg_dimension_CellValueChanged;
            dtg_dimension.CurrentCellDirtyStateChanged -= dtg_dimension_CurrentCellDirtyStateChanged;
        }

        private void ShowPage(int pageNumber)
        {
            currentPage = pageNumber;

            bindingSource.Filter = $"POINT_ORDER = '{pageNumber}'";
            dtg_dimension.DataSource = bindingSource;

            CalculatePointValues();
            UpdateReadOnlyCells();
            ApplyEquipmentSerialComboBoxes();

            lb_page.Text = $"{pageNumber}/{totalPages}";
            dtg_dimension.Refresh();
        }

        private void bt_prev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                ShowPage(currentPage);
            }
        }

        private void bt_next_Click(object sender, EventArgs e)
        {
            if (currentPage < totalPages)
            {
                currentPage++;
                ShowPage(currentPage);
            }
        }

        private void UpdateReadOnlyCells()
        {
            foreach (DataGridViewRow row in dtg_dimension.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                string pointCal = row.Cells["POINT_CAL"].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(pointCal) && pointCal != "0")
                {
                    row.Cells["VALUE"].ReadOnly = true;
                }
                else
                {
                    row.Cells["VALUE"].ReadOnly = false;
                }
            }
        }

        private void dtg_dimension_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dtg_dimension.IsCurrentCellDirty)
            {
                dtg_dimension.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dtg_dimension_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dtg_dimension.Columns[e.ColumnIndex].Name != "VALUE")
            {
                return;
            }

            DataGridViewRow row = dtg_dimension.Rows[e.RowIndex];

            if (row.Cells["CRITERIA_MIN"].Value == null || row.Cells["CRITERIA_MAX"].Value == null)
            {
                return;
            }

            if (!double.TryParse(row.Cells["CRITERIA_MIN"].Value.ToString(), out double minValue) ||
                !double.TryParse(row.Cells["CRITERIA_MAX"].Value.ToString(), out double maxValue))
            {
                return;
            }

            if (minValue == 1 && maxValue == 1)
            {
                if (row.Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
                {
                    return;
                }

                object currentValue = row.Cells[e.ColumnIndex].Value;

                DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
                comboBoxCell.DataSource = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("", ""),
                    new KeyValuePair<string, string>("0", "NG"),
                    new KeyValuePair<string, string>("1", "OK")
                };
                comboBoxCell.ValueMember = "Key";
                comboBoxCell.DisplayMember = "Value";
                comboBoxCell.Value = currentValue;

                BeginInvoke((MethodInvoker)delegate
                {
                    if (e.RowIndex >= 0 &&
                        e.RowIndex < dtg_dimension.Rows.Count &&
                        e.ColumnIndex >= 0 &&
                        e.ColumnIndex < dtg_dimension.Columns.Count)
                    {
                        dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex] = comboBoxCell;
                    }
                });
            }
            else
            {
                if (row.Cells[e.ColumnIndex] is DataGridViewTextBoxCell)
                {
                    return;
                }

                object currentValue = row.Cells[e.ColumnIndex].Value;

                DataGridViewTextBoxCell textBoxCell = new DataGridViewTextBoxCell();
                textBoxCell.Value = currentValue;

                BeginInvoke((MethodInvoker)delegate
                {
                    if (e.RowIndex >= 0 &&
                        e.RowIndex < dtg_dimension.Rows.Count &&
                        e.ColumnIndex >= 0 &&
                        e.ColumnIndex < dtg_dimension.Columns.Count)
                    {
                        dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex] = textBoxCell;
                    }
                });
            }
        }

        private void dtg_dimension_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dtg_dimension.Columns[e.ColumnIndex].Name != "VALUE")
            {
                return;
            }

            if (dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
            {
                return;
            }

            string input = e.FormattedValue?.ToString();

            if (string.IsNullOrWhiteSpace(input))
            {
                return;
            }

            if (!IsValidDecimal(input))
            {
                MessageBox.Show(
                    "กรุณากรอกตัวเลขเท่านั้น และไม่สามารถมีจุดทศนิยมมากกว่า 1 จุดได้",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                e.Cancel = true;
            }
        }

        private void dtg_dimension_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE")
            {
                CalculatePointValues();
                return;
            }

            if (dtg_dimension.Columns[e.ColumnIndex].Name == "EQUIPMENT_SERIAL")
            {
                BindingSource bs = dtg_dimension.DataSource as BindingSource;
                DataTable dtData = bs != null ? (DataTable)bs.DataSource : dtg_dimension.DataSource as DataTable;
                if (dtData == null) return;

                string newSerial = dtg_dimension.Rows[e.RowIndex].Cells["EQUIPMENT_SERIAL"].Value?.ToString();
                string eqType = dtg_dimension.Rows[e.RowIndex].Cells["EQUIPMENT_TYPE"].Value?.ToString();

                if (!string.IsNullOrEmpty(newSerial) && !string.IsNullOrEmpty(eqType))
                {
                    foreach (DataRow row in dtData.Rows)
                    {
                        if (row["EQUIPMENT_TYPE"].ToString() == eqType)
                        {
                            row["EQUIPMENT_SERIAL"] = newSerial;
                        }
                    }

                    bs?.ResetBindings(false);
                    dtg_dimension.Refresh();
                    ApplyEquipmentSerialComboBoxes();
                }
            }
        }

        private void dtg_dimension_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (isUpdating)
            {
                return;
            }

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dtg_dimension.Columns[e.ColumnIndex].Name != "VALUE")
            {
                return;
            }

            if (!(dtg_dimension.Rows[e.RowIndex].DataBoundItem is DataRowView rowView))
            {
                return;
            }

            CalculateJudgeForRow(rowView.Row);
            CalculatePointValues();
        }

        private void dtg_dimension_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            UpdateReadOnlyCells();
            ApplyEquipmentSerialComboBoxes();

            foreach (DataGridViewRow row in dtg_dimension.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                string pointJudge = row.Cells["POINT_JUDGE"].Value?.ToString();
                string value = row.Cells["VALUE"].Value?.ToString();

                if (!string.IsNullOrWhiteSpace(value) && pointJudge == "0")
                {
                    row.DefaultCellStyle.BackColor = Color.Red;
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White;
                }
            }
        }

        private void ApplyEquipmentSerialComboBoxes()
        {
            if (!dtg_dimension.Columns.Contains("EQUIPMENT_SERIAL") ||
                !dtg_dimension.Columns.Contains("EQUIPMENT_TYPE"))
            {
                return;
            }

            foreach (DataGridViewRow row in dtg_dimension.Rows)
            {
                if (row.IsNewRow) continue;

                string equipmentType = row.Cells["EQUIPMENT_TYPE"].Value?.ToString();
                DataTable serialSource = GetEquipmentSerialSource(equipmentType);
                object currentValue = row.Cells["EQUIPMENT_SERIAL"].Value;

                EnsureCurrentSerialIdExists(serialSource, currentValue);

                DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell
                {
                    DataSource = serialSource,
                    DisplayMember = "TEXT",
                    ValueMember = "VALUE",
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                    FlatStyle = FlatStyle.Flat
                };

                comboBoxCell.Value = currentValue == null || currentValue == DBNull.Value
                    ? ""
                    : currentValue.ToString();

                row.Cells["EQUIPMENT_SERIAL"] = comboBoxCell;
            }
        }

        private DataTable GetEquipmentSerialSource(string equipmentType)
        {
            string key = equipmentType?.Trim() ?? "";
            if (equipmentSerialSourceByType.ContainsKey(key))
            {
                return equipmentSerialSourceByType[key].Copy();
            }

            DataTable source = settingController.SearchEquipmentTypeSettingByEquipmentType(new SettingProperty
            {
                Equipment_Type = key
            });

            DataTable serialSource = CreateEquipmentSerialSourceTable();
            serialSource.Rows.Add("", "");

            if (source != null)
            {
                foreach (DataRow row in source.Rows)
                {
                    string serialId = row["Equipment_Serial_ID"]?.ToString()?.Trim();
                    string serial = row["Equipment_Serial"]?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(serialId) || string.IsNullOrWhiteSpace(serial)) continue;

                    bool exists = serialSource.AsEnumerable()
                        .Any(r => string.Equals(r["VALUE"].ToString(), serialId, StringComparison.OrdinalIgnoreCase));

                    if (!exists)
                    {
                        serialSource.Rows.Add(serial, serialId);
                    }
                }
            }

            equipmentSerialSourceByType[key] = serialSource;
            return serialSource.Copy();
        }

        private static DataTable CreateEquipmentSerialSourceTable()
        {
            DataTable table = new DataTable();
            table.Columns.Add("TEXT", typeof(string));
            table.Columns.Add("VALUE", typeof(string));
            return table;
        }

        private static void EnsureCurrentSerialIdExists(DataTable serialSource, object currentValue)
        {
            string serialId = currentValue == null || currentValue == DBNull.Value
                ? ""
                : currentValue.ToString().Trim();

            if (string.IsNullOrWhiteSpace(serialId)) return;

            bool exists = serialSource.AsEnumerable()
                .Any(row => string.Equals(row["VALUE"].ToString(), serialId, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                serialSource.Rows.Add(serialId, serialId);
            }
        }

        private void CalculatePointValues()
        {
            if (isUpdating)
            {
                return;
            }

            if (originalDataTable == null)
            {
                return;
            }

            isUpdating = true;

            try
            {
                pointValues.Clear();

                // 1) เก็บค่า VALUE ของแถวที่ไม่ใช่สูตร โดย key ต้องแยก Cavity ด้วย
                foreach (DataRow row in originalDataTable.Rows)
                {
                    if (IsCalculatedPoint(row))
                    {
                        continue;
                    }

                    if (TryGetDecimal(row["VALUE"], out decimal value))
                    {
                        string key = MakePointKey(row);
                        pointValues[key] = value;
                    }
                }

                // 2) คำนวณแถวสูตร เช่น POINT_CAL = 1+2+3 โดยแยก Cavity + Sample
                foreach (DataRow row in originalDataTable.Rows)
                {
                    if (IsCalculatedPoint(row))
                    {
                        string pointCal = row["POINT_CAL"]?.ToString() ?? "";
                        string samplingNo = row["SAMPLING_NO"]?.ToString() ?? "";
                        string cavityName = GetCavityName(row);

                        decimal sum = 0;
                        bool canCalculate = true;

                        string[] orders = pointCal.Split('+');

                        foreach (string order in orders)
                        {
                            string depPointOrder = order.Trim();
                            string depKey = MakePointKey(cavityName, samplingNo, depPointOrder);

                            if (pointValues.ContainsKey(depKey))
                            {
                                sum += pointValues[depKey];
                            }
                            else
                            {
                                canCalculate = false;
                                break;
                            }
                        }

                        if (canCalculate)
                        {
                            row["VALUE"] = sum.ToString();
                            pointValues[MakePointKey(row)] = sum;
                        }
                        else
                        {
                            row["VALUE"] = DBNull.Value;
                            row["POINT_JUDGE"] = DBNull.Value;
                            continue;
                        }
                    }

                    CalculateJudgeForRow(row);
                }

                CalculateTotalJudge();
            }
            finally
            {
                isUpdating = false;
                bindingSource.ResetBindings(false);
                dtg_dimension.Refresh();
            }
        }

        private bool IsCalculatedPoint(DataRow row)
        {
            string equipmentType = row["EQUIPMENT_TYPE"]?.ToString() ?? "";
            string pointCal = row["POINT_CAL"]?.ToString() ?? "";

            return equipmentType == "0" &&
                   !string.IsNullOrWhiteSpace(pointCal) &&
                   pointCal != "0" &&
                   pointCal.Contains("+");
        }

        private void CalculateJudgeForRow(DataRow row)
        {
            if (row == null)
            {
                return;
            }

            if (!TryGetDecimal(row["VALUE"], out decimal value))
            {
                row["POINT_JUDGE"] = DBNull.Value;
                return;
            }

            if (!TryGetDecimal(row["CRITERIA_MIN"], out decimal min) ||
                !TryGetDecimal(row["CRITERIA_MAX"], out decimal max))
            {
                row["POINT_JUDGE"] = DBNull.Value;
                return;
            }

            row["POINT_JUDGE"] = value >= min && value <= max ? "1" : "0";
        }

        private void CalculateTotalJudge()
        {
            if (originalDataTable == null)
            {
                return;
            }

            foreach (DataRow row in originalDataTable.Rows)
            {
                string pointJudge = row["POINT_JUDGE"]?.ToString();

                if (string.IsNullOrWhiteSpace(pointJudge) || pointJudge != "1")
                {
                    SetTotalJudge(0);
                    return;
                }
            }

            SetTotalJudge(1);
        }

        private void SetTotalJudge(int value)
        {
            if (originalDataTable == null)
            {
                return;
            }

            foreach (DataRow row in originalDataTable.Rows)
            {
                row["TOTAL_JUDGE"] = value.ToString();
            }
        }

        private bool IsDataTableValid(DataTable table)
        {
            if (table == null)
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                    "ไม่พบข้อมูล Dimension",
                    "คำเตือน",
                    CustomMsgBoxBase.MessageBoxIconType.Warning);
                return false;
            }

            CalculatePointValues();

            foreach (DataRow row in table.Rows)
            {
                string pageNumber = row["POINT_ORDER"]?.ToString() ?? "N/A";
                string samplingNo = row["SAMPLING_NO"]?.ToString() ?? "N/A";
                string cavityName = table.Columns.Contains("CAVITY_NAME")
                    ? row["CAVITY_NAME"]?.ToString() ?? "N/A"
                    : "N/A";

                string value = row["VALUE"]?.ToString();
                string pointJudge = row["POINT_JUDGE"]?.ToString();
                string equipmentType = row["EQUIPMENT_TYPE"]?.ToString();
                string equipmentSerial = row["EQUIPMENT_SERIAL"]?.ToString();

                if (table.Columns.Contains("CAVITY_NAME") && string.IsNullOrWhiteSpace(cavityName))
                {
                    CustomMsgBoxBase.ShowCustomMessageBox(
                        $"พบ Cavity ว่าง: หน้า {pageNumber}, Sampling No {samplingNo}",
                        "คำเตือน",
                        CustomMsgBoxBase.MessageBoxIconType.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    CustomMsgBoxBase.ShowCustomMessageBox(
                        $"พบ VALUE ว่าง: Cavity {cavityName}, หน้า {pageNumber}, Sampling No {samplingNo}",
                        "คำเตือน",
                        CustomMsgBoxBase.MessageBoxIconType.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(pointJudge))
                {
                    CustomMsgBoxBase.ShowCustomMessageBox(
                        $"ยังไม่ได้คำนวณ Judge: Cavity {cavityName}, หน้า {pageNumber}, Sampling No {samplingNo}",
                        "คำเตือน",
                        CustomMsgBoxBase.MessageBoxIconType.Warning);
                    return false;
                }

                // ถ้าไม่ใช่แถวสูตร ควรมี Equipment Serial
                if (equipmentType != "0" && string.IsNullOrWhiteSpace(equipmentSerial))
                {
                    CustomMsgBoxBase.ShowCustomMessageBox(
                        $"พบ Equipment Serial ว่าง: Cavity {cavityName}, หน้า {pageNumber}, Sampling No {samplingNo}",
                        "คำเตือน",
                        CustomMsgBoxBase.MessageBoxIconType.Warning);
                    return false;
                }
            }

            return true;
        }

        private void tb_record_Click(object sender, EventArgs e)
        {
            if (dtg_dimension.IsCurrentCellDirty || dtg_dimension.IsCurrentRowDirty)
            {
                dtg_dimension.EndEdit();
                dtg_dimension.CommitEdit(DataGridViewDataErrorContexts.Commit);
                bindingSource.EndEdit();
            }

            CalculatePointValues();

            if (!IsDataTableValid(originalDataTable))
            {
                return;
            }

            propQA.TOTAL_STATUS = "1";
            propQA.EMP_ID = employee.EMP_CODE;

            foreach (DataRow row in originalDataTable.Rows)
            {
                string totalJudge = row["TOTAL_JUDGE"]?.ToString();

                if (totalJudge != "1")
                {
                    propQA.TOTAL_STATUS = "0";
                    break;
                }
            }

            propQA.dtgDimData = new DataGridView();
            propQA.dtgDimData.DataSource = originalDataTable;

            if (conQA.InsertDimensionData(propQA) == true)
            {
                if (propQA.TOTAL_STATUS == "0")
                {
                    propQA.inProcStatus = "0";
                    propQA.reportStatus = "0";
                }
                else
                {
                    propQA.inProcStatus = "1";
                    propQA.reportStatus = "1";
                }

                if (conQA.UpdateStatus(propQA) == true)
                {
                    ProcStatus status;

                    bool parsed = int.TryParse(propQA.inProcStatus, out int statusId) &&
                                  Enum.IsDefined(typeof(ProcStatus), statusId);

                    status = parsed ? (ProcStatus)statusId : ProcStatus.NG;

                    switch (status)
                    {
                        case ProcStatus.OK:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Dimension งาน OK เรียบร้อยแล้ว",
                                "สำเร็จ",
                                CustomMsgBoxBase.MessageBoxIconType.OK);
                            break;

                        case ProcStatus.Pending:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Dimension พบงาน ถูก PENDING",
                                "สำเร็จ",
                                CustomMsgBoxBase.MessageBoxIconType.Pending);
                            break;

                        case ProcStatus.NG:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Dimension พบงาน ถูก NG",
                                "สำเร็จ",
                                CustomMsgBoxBase.MessageBoxIconType.NG);
                            break;

                        default:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "สถานะไม่รู้จัก",
                                "ข้อผิดพลาด",
                                CustomMsgBoxBase.MessageBoxIconType.Pending);
                            break;
                    }

                    loadstatus();
                    bt_status_Dimension_pending_Click();
                    return;
                }

                CustomMsgBoxBase.ShowCustomMessageBox(
                    "Record Dimension ไม่ได้ กรุณากด record อีกครั้ง",
                    "ข้อผิดพลาด",
                    CustomMsgBoxBase.MessageBoxIconType.NG);
                return;
            }

            CustomMsgBoxBase.ShowCustomMessageBox(
                "Record Dimension ไม่ได้ กรุณากด record อีกครั้ง",
                "ข้อผิดพลาด",
                CustomMsgBoxBase.MessageBoxIconType.NG);
        }

        private void bt_status_Dimension_pending_Click()
        {
            userControlSelectDimensionPending usrSelectDimPending = new userControlSelectDimensionPending();
            usrSelectDimPending.Dock = DockStyle.Fill;
            usrSelectDimPending.propQA = propQA;

            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrSelectDimPending);
                    usrSelectDimPending.BringToFront();
                }
                else
                {
                    MessageBox.Show(
                        "ไม่พบ หน้าจอหลัก panelMain",
                        "Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        private void loadstatus()
        {
            if (this.ParentForm is frmMain mainForm)
            {
                mainForm.LoadStatus();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Enter &&
                dtg_dimension.ContainsFocus &&
                dtg_dimension.CurrentCell != null &&
                dtg_dimension.CurrentCell.OwningColumn.Name == "VALUE" &&
                dtg_dimension.CurrentCell is DataGridViewTextBoxCell &&
                !dtg_dimension.CurrentCell.ReadOnly)
            {
                int currentRowIndex = dtg_dimension.CurrentCell.RowIndex;

                if (!dtg_dimension.EndEdit())
                {
                    return true;
                }

                bindingSource.EndEdit();
                BeginInvoke(new Action(() => MoveToNextDimensionValueRow(currentRowIndex)));
                return true;
            }

            if (dimensionImages != null && dimensionImages.Count > 1)
            {
                if (keyData == Keys.PageUp || keyData == Keys.PageDown)
                {
                    if (keyData == Keys.PageUp)
                    {
                        currentDimensionImageIndex =
                            (currentDimensionImageIndex - 1 + dimensionImages.Count) % dimensionImages.Count;
                    }
                    else
                    {
                        currentDimensionImageIndex =
                            (currentDimensionImageIndex + 1) % dimensionImages.Count;
                    }

                    picbox_dim.Image = dimensionImages[currentDimensionImageIndex];
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MoveToNextDimensionValueRow(int currentRowIndex)
        {
            if (IsDisposed ||
                dtg_dimension.IsDisposed ||
                !dtg_dimension.IsHandleCreated ||
                !dtg_dimension.Columns.Contains("VALUE"))
            {
                return;
            }

            for (int rowIndex = currentRowIndex + 1; rowIndex < dtg_dimension.Rows.Count; rowIndex++)
            {
                DataGridViewCell valueCell = dtg_dimension.Rows[rowIndex].Cells["VALUE"];
                if (!valueCell.Visible || valueCell.ReadOnly || valueCell is DataGridViewComboBoxCell)
                {
                    continue;
                }

                dtg_dimension.CurrentCell = valueCell;
                dtg_dimension.Focus();
                dtg_dimension.BeginEdit(true);
                return;
            }
        }

        private void UserControlDimension_Disposed(object sender, EventArgs e)
        {
            DetachGridEvents();

            if (dimensionImages != null)
            {
                foreach (Image img in dimensionImages)
                {
                    img?.Dispose();
                }

                dimensionImages.Clear();
                dimensionImages = null;
            }

            this.Disposed -= UserControlDimension_Disposed;
        }

        private string MakePointKey(DataRow row)
        {
            string cavityName = GetCavityName(row);
            string samplingNo = row["SAMPLING_NO"]?.ToString() ?? "";
            string pointOrder = row["POINT_ORDER"]?.ToString() ?? "";

            return MakePointKey(cavityName, samplingNo, pointOrder);
        }

        private string MakePointKey(string cavityName, string samplingNo, string pointOrder)
        {
            return $"{cavityName}_{samplingNo}_{pointOrder}";
        }

        private string GetCavityName(DataRow row)
        {
            if (row == null || originalDataTable == null)
            {
                return "";
            }

            if (!originalDataTable.Columns.Contains("CAVITY_NAME"))
            {
                return "";
            }

            return row["CAVITY_NAME"]?.ToString() ?? "";
        }

        private bool TryGetDecimal(object value, out decimal result)
        {
            result = 0;

            if (value == null || value == DBNull.Value)
            {
                return false;
            }

            return decimal.TryParse(value.ToString(), out result);
        }

        private bool IsValidDecimal(string input)
        {
            return decimal.TryParse(input, out _) && input.Count(c => c == '.') <= 1;
        }

        private string GetString(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName))
            {
                return "";
            }

            if (row[columnName] == null || row[columnName] == DBNull.Value)
            {
                return "";
            }

            return row[columnName].ToString();
        }

        private int ToInt(string value)
        {
            if (int.TryParse(value, out int result))
            {
                return result;
            }

            return 0;
        }

        private double ToDouble(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }

            if (double.TryParse(value.ToString(), out double result))
            {
                return result;
            }

            return 0;
        }
    }
}
