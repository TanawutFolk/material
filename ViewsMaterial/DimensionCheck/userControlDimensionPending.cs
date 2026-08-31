﻿using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.ViewsMaterial.CustomMsg;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using static RawMat.Property.QAdataProperty;

namespace RawMat.ViewsMaterial.DimensionCheck
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

        // หน้า Pending โหลดมาเฉพาะจุดที่ตก เลขจุดจึงไม่ได้เริ่มที่ 1 และไม่ติดกัน
        // เช่นตกจุดเดียวที่ POINT_ORDER = 11 ก็จะมีหน้าเดียวที่เลขจุดคือ 11
        // เดิมกรองด้วยเลขหน้าตรงๆ (POINT_ORDER = 1) จึงไม่เจออะไรเลย ตารางว่างเปล่า
        // จึงต้องเก็บเลขจุดที่มีจริงไว้ แล้วเดินหน้าตามลำดับในลิสต์แทน
        private readonly List<string> pageOrders = new List<string>();


        private bool isUpdating = false;

        // Key = CAVITY_NAME + SAMPLING_NO + POINT_ORDER
        private readonly Dictionary<string, decimal> pointValues = new Dictionary<string, decimal>();

        private List<Image> dimensionImages;
        private int currentDimensionImageIndex = 0;
        private readonly Dictionary<string, DataTable> equipmentSerialSourceByType = new Dictionary<string, DataTable>();

        public userControlDimensionPending()
        {
            InitializeComponent();
            dtg_dimension.DataError += dtg_dimension_DataError;
        }

        private void dtg_dimension_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
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
                MessageBox.Show("????? data Dimension ??? Pending ?? db_Dimension_data");
                return;
            }

            originalDataTable = BuildDimensionDataTable(dtDimPending);

            bindingSource.DataSource = originalDataTable;
            dtg_dimension.DataSource = bindingSource;

            ConfigureGrid();
            AttachGridEvents();

            // เก็บเลขจุดที่โหลดมาจริง เรียงตามค่าตัวเลข ไม่ใช่ตามตัวอักษร (2 ต้องมาก่อน 10)
            pageOrders.Clear();
            pageOrders.AddRange(originalDataTable.AsEnumerable()
                .Select(row => row["POINT_ORDER"].ToString())
                .Distinct()
                .OrderBy(order => int.TryParse(order, out int number) ? number : int.MaxValue)
                .ThenBy(order => order));

            totalPages = pageOrders.Count;

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
            dtAllSum.Columns.Add("CRITERIA_MIN", typeof(decimal));
            dtAllSum.Columns.Add("CRITERIA_MAX", typeof(decimal));
            dtAllSum.Columns.Add("JUDGE_TYPE", typeof(string));
            dtAllSum.Columns.Add("POINT_JUDGE", typeof(string));
            dtAllSum.Columns.Add("TOTAL_JUDGE", typeof(string));
            dtAllSum.Columns.Add("DIFFERENCE", typeof(decimal));
            dtAllSum.Columns.Add("TOLERANCE", typeof(decimal));
            dtAllSum.Columns.Add("PIECE_JUDGE", typeof(string));

            foreach (DataRow dtRow in dtDimPending.Rows)
            {
                if (hasCavity)
                {
                    dtAllSum.Rows.Add(
                        GetString(dtRow, "CAVITY_NAME"),
                        ToInt(GetString(dtRow, "SAMPLING_NO")),
                        GetString(dtRow, "POINT_ORDER"),
                        GetString(dtRow, "POINT_CAL"),
                        GetSerialTextById(GetString(dtRow, "EQUIPMENT_TYPE"), GetString(dtRow, "EQUIPMENT_SERIAL_ID")),
                        GetString(dtRow, "EQUIPMENT_TYPE"),
                        GetString(dtRow, "EQUIPMENT_NAME"),
                        GetString(dtRow, "POINT_NAME"),
                        NumberDisplay.Trim(GetString(dtRow, "VALUE")),
                        ToDecimal(dtRow["CRITERIA_MIN"]),
                        ToDecimal(dtRow["CRITERIA_MAX"]),
                        GetJudgeType(dtRow),
                        "",
                        "",
                        null, null, null
                    );
                }
                else
                {
                    dtAllSum.Rows.Add(
                        ToInt(GetString(dtRow, "SAMPLING_NO")),
                        GetString(dtRow, "POINT_ORDER"),
                        GetString(dtRow, "POINT_CAL"),
                        GetSerialTextById(GetString(dtRow, "EQUIPMENT_TYPE"), GetString(dtRow, "EQUIPMENT_SERIAL_ID")),
                        GetString(dtRow, "EQUIPMENT_TYPE"),
                        GetString(dtRow, "EQUIPMENT_NAME"),
                        GetString(dtRow, "POINT_NAME"),
                        NumberDisplay.Trim(GetString(dtRow, "VALUE")),
                        ToDecimal(dtRow["CRITERIA_MIN"]),
                        ToDecimal(dtRow["CRITERIA_MAX"]),
                        GetJudgeType(dtRow),
                        "",
                        "",
                        null, null, null
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
                "TOTAL_JUDGE",
                "JUDGE_TYPE",
                "PIECE_JUDGE",
                "TOLERANCE"
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

            // ช่อง EQ_SN ให้เลือกจาก S/N ที่มีอยู่แล้ว แทนการพิมพ์เองซึ่งทำให้ master มีค่าซ้ำ
            EquipmentSerialColumn.Apply(dtg_dimension, conQA.EquipmentSerialList());
            }

            if (dtg_dimension.Columns.Contains("EQUIPMENT_NAME"))
            {
                dtg_dimension.Columns["EQUIPMENT_NAME"].HeaderText = "EQ_NAME";
            }

            if (dtg_dimension.Columns.Contains("CRITERIA_MIN"))
            {
                dtg_dimension.Columns["CRITERIA_MIN"].HeaderText = "MIN";
                dtg_dimension.Columns["CRITERIA_MIN"].DefaultCellStyle.Format = NumberDisplay.GridFormat;
            }

            if (dtg_dimension.Columns.Contains("CRITERIA_MAX"))
            {
                dtg_dimension.Columns["CRITERIA_MAX"].HeaderText = "MAX";
                dtg_dimension.Columns["CRITERIA_MAX"].DefaultCellStyle.Format = NumberDisplay.GridFormat;
            }

            // ผลต่างภายในชิ้นใช้ตัดสินเฉพาะแบบ All โหมดอื่นไม่มีเกณฑ์นี้จึงซ่อนไว้
            // Admin ต้องเห็นเหมือนที่ผู้ตรวจเห็น ไม่งั้นจะงงว่าทำไมทุกจุดอยู่ในเกณฑ์แล้วยังไม่ผ่าน
            if (dtg_dimension.Columns.Contains("DIFFERENCE"))
            {
                dtg_dimension.Columns["DIFFERENCE"].Visible = IsAllDimensionMode();
                dtg_dimension.Columns["DIFFERENCE"].HeaderText = "DIFF";
                dtg_dimension.Columns["DIFFERENCE"].DefaultCellStyle.Format = NumberDisplay.GridFormat;
            }

            ApplyColumnWidths();
            UpdateReadOnlyCells();
        }

        private void AttachGridEvents()
        {
            dtg_dimension.CellEndEdit += dtg_dimension_CellEndEdit;
            dtg_dimension.EditingControlShowing += (s2, e2) => EquipmentSerialColumn.HandleEditingControlShowing(dtg_dimension, e2);
            dtg_dimension.DataError += (s2, e2) => EquipmentSerialColumn.HandleDataError(dtg_dimension, e2);
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


        /// <summary>
        /// ตารางตั้ง AutoSizeColumnsMode = Fill ความกว้างจึงคุมด้วย FillWeight ไม่ใช่ Width
        /// ค่าเริ่มต้นทุกคอลัมน์เท่ากันที่ 100 จึงกว้างเท่ากันหมด
        ///
        /// CAV. กับ SAMPLE เก็บแค่ตัวอักษรเดียวกับเลขหลักเดียว ลดครึ่งหนึ่งเหลือ 50
        /// ที่เหลือ 100 ยกไปให้ EQ_SN กับ EQ_NAME อย่างละ 50 เพราะชื่อเครื่องมือกับ S/N ยาวจนโดนตัด
        /// น้ำหนักรวมเท่าเดิม คอลัมน์อื่นจึงกว้างเท่าเดิม
        /// </summary>
        private void ApplyColumnWidths()
        {
            SetFillWeight("CAVITY_NAME", 50);
            SetFillWeight("SAMPLING_NO", 70);
            SetFillWeight("EQUIPMENT_SERIAL", 140);
            SetFillWeight("EQUIPMENT_NAME", 140);
        }

        private void SetFillWeight(string columnName, float weight)
        {
            if (!dtg_dimension.Columns.Contains(columnName)) { return; }

            dtg_dimension.Columns[columnName].FillWeight = weight;
        }

        private void ShowPage(int pageNumber)
        {
            currentPage = pageNumber;

            if (pageOrders.Count == 0)
            {
                bindingSource.Filter = null;
                lb_page.Text = "0/0";
                return;
            }

            // pageNumber คือลำดับหน้า ไม่ใช่เลขจุด ต้องแปลงก่อนเสมอ
            string pointOrder = pageOrders[Math.Min(Math.Max(pageNumber, 1), pageOrders.Count) - 1];

            bindingSource.Filter = $"POINT_ORDER = '{pointOrder}'";
            dtg_dimension.DataSource = bindingSource;

            CalculatePointValues();
            UpdateReadOnlyCells();

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

            if (!decimal.TryParse(row.Cells["CRITERIA_MIN"].Value.ToString(), out decimal minValue) ||
                !decimal.TryParse(row.Cells["CRITERIA_MAX"].Value.ToString(), out decimal maxValue))
            {
                return;
            }

            // อ่านจากค่าที่ตั้งไว้ ไม่เดาจากเกณฑ์ ดู Utilities/PointJudgeType.cs
            if (Utilities.PointJudgeType.IsPassFail(row))
            {
                if (row.Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
                {
                    return;
                }

                object currentValue = row.Cells[e.ColumnIndex].Value;

                DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
                // ต้องใช้ค่าตามเกณฑ์จริง ไม่ผูกกับ 1/0 ให้ตรงกับหน้ากรอกค่า
                // ไม่งั้นจุดที่ตั้งเกณฑ์เป็นเลขอื่นจะโชว์ช่องว่างเพราะหาค่าเดิมในรายการไม่เจอ
                string okValue = minValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                string ngValue = (minValue - 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

                comboBoxCell.DataSource = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("", ""),
                    new KeyValuePair<string, string>(ngValue, "NG"),
                    new KeyValuePair<string, string>(okValue, "OK")
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
                    "??????????????????????? ?????????????????????????????? 1 ??????",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                e.Cancel = true;
            }
        }

        private string GetSerialTextById(string equipmentType, string serialId)
        {
            if (string.IsNullOrWhiteSpace(serialId)) return "";

            System.Data.DataTable serialSource = GetEquipmentSerialSource(equipmentType);
            var row = serialSource.AsEnumerable().FirstOrDefault(r => string.Equals(r["ID"].ToString(), serialId, System.StringComparison.OrdinalIgnoreCase));
            if (row != null) return row["TEXT"].ToString();

            row = serialSource.AsEnumerable().FirstOrDefault(r => string.Equals(r["VALUE"].ToString(), serialId, System.StringComparison.OrdinalIgnoreCase));
            return row != null ? row["TEXT"].ToString() : serialId;
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
            serialSource.Rows.Add("", "", "");

            if (source != null)
            {
                foreach (DataRow row in source.Rows)
                {
                    string serialId = row["Equipment_Serial_ID"]?.ToString()?.Trim();
                    string serial = row["Equipment_Serial"]?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(serialId) || string.IsNullOrWhiteSpace(serial)) continue;

                    bool exists = serialSource.AsEnumerable()
                        .Any(r => string.Equals(r["VALUE"].ToString(), serial, StringComparison.OrdinalIgnoreCase));

                    if (!exists)
                    {
                        serialSource.Rows.Add(serial, serial, serialId);
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
            table.Columns.Add("ID", typeof(string));
            return table;
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

                // 1) ??????? VALUE ??????????????????? ??? key ??????? Cavity ????
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

                // 2) ???????????? ???? POINT_CAL = 1+2+3 ?????? Cavity + Sample
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

                CalculatePieceDifference();
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

                // แบบ All ผลต่างเกินเกณฑ์ก็ตกทั้งใบเหมือนกัน
                if (originalDataTable.Columns.Contains("PIECE_JUDGE")
                    && row["PIECE_JUDGE"] != null
                    && row["PIECE_JUDGE"].ToString() == "0")
                {
                    SetTotalJudge(0);
                    return;
                }
            }

            SetTotalJudge(1);
        }

        private bool IsAllDimensionMode()
        {
            return propQA.SAMPLING_TYPE == "1";
        }

        /// <summary>
        /// ผลต่างของทุกจุดในชิ้นเดียวกันต้องไม่เกินครึ่งหนึ่งของช่วง MIN-MAX
        /// ต้องคิดใหม่ทุกครั้งที่ Admin แก้ค่า ไม่งั้นชิ้นที่ผลต่างเกินจะหลุดผ่านหน้านี้ไปได้
        /// ตรรกะเดียวกับ userControlDimension ตั้งใจให้ทั้งสองหน้าตัดสินตรงกัน
        /// </summary>
        private void CalculatePieceDifference()
        {
            if (!IsAllDimensionMode()) return;
            if (originalDataTable == null) return;
            if (!originalDataTable.Columns.Contains("PIECE_JUDGE")) return;

            // ต้องอ่านจาก DataTable ไม่ใช่จากตารางบนหน้าจอ
            // เพราะหน้าจอกรองไว้ทีละจุด (ShowPage ตั้ง Filter = POINT_ORDER)
            // ถ้าอ่านจากหน้าจอจะเห็นแค่จุดเดียวต่อชิ้น MAX-MIN เลยได้ 0 เสมอ
            var pieces = originalDataTable.Rows
                .Cast<DataRow>()
                .GroupBy(row => row["SAMPLING_NO"]?.ToString());

            foreach (var piece in pieces)
            {
                // จุดที่ตัดสิน OK/NG ไม่ใช่ค่าที่วัดออกมาเป็นตัวเลข เก็บลง DB เป็น 0/1
                // ถ้าเอามาคิดผลต่างด้วยจะลากช่วงเพี้ยนไปทั้งชิ้น
                List<DataRow> rows = piece.ToList();
                List<DataRow> measurable = rows.Where(Utilities.PointJudgeType.IsNumeric).ToList();
                var values = new List<decimal>();

                foreach (DataRow row in measurable)
                {
                    if (TryParseDecimalInvariant(row["VALUE"], out decimal value))
                    {
                        values.Add(value);
                    }
                }

                // ผลต่างต้องมีอย่างน้อย 2 จุดถึงจะมีความหมาย
                // ถ้ามีจุดเดียวแล้วโชว์ 0 จะอ่านเหมือนว่าผลต่างเป็นศูนย์คือดีมาก
                if (values.Count < 2)
                {
                    foreach (DataRow row in rows)
                    {
                        row["DIFFERENCE"] = DBNull.Value;
                        row["TOLERANCE"] = DBNull.Value;
                        row["PIECE_JUDGE"] = DBNull.Value;
                    }

                    continue;
                }

                decimal difference = values.Max() - values.Min();
                decimal tolerance = GetDifferenceTolerance(measurable[0]);
                bool complete = values.Count == measurable.Count;

                // เกินเกณฑ์เมื่อไหร่ฟันธง NG ได้ทันที เพราะกรอกเพิ่มมีแต่จะกว้างขึ้น
                // แต่จะบอกว่าผ่านต้องรอครบทุกจุดก่อน
                string judge = null;

                if (tolerance > 0 && difference > tolerance)
                {
                    judge = "0";
                }
                else if (complete)
                {
                    judge = "1";
                }

                foreach (DataRow row in rows)
                {
                    row["DIFFERENCE"] = difference;
                    row["TOLERANCE"] = tolerance;
                    row["PIECE_JUDGE"] = judge == null ? (object)DBNull.Value : judge;
                }
            }
        }

        /// <summary>เกณฑ์ผลต่างที่ยอมได้ = ครึ่งหนึ่งของช่วง MIN-MAX เช่น 6.1468~6.5532 ได้ 0.2032</summary>
        private static decimal GetDifferenceTolerance(DataRow row)
        {
            // ใช้ decimal ไม่ใช้ double เพราะ 6.5532 - 6.1468 ด้วย double ได้ 0.40640000000000053
            // แล้วเคสที่ผลต่างเท่าเกณฑ์พอดีจะตัดสินพลาด
            if (!TryParseDecimalInvariant(row["CRITERIA_MIN"], out decimal min)) return 0;
            if (!TryParseDecimalInvariant(row["CRITERIA_MAX"], out decimal max)) return 0;

            return (max - min) / 2m;
        }

        private static bool TryParseDecimalInvariant(object rawValue, out decimal value)
        {
            string text = rawValue == null || rawValue == DBNull.Value
                ? string.Empty
                : rawValue.ToString().Trim();

            // ช่องกรอกรับจุดทศนิยม จึงอ่านแบบ Invariant ก่อน แล้วค่อยรองรับ Culture ของเครื่อง
            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value)
                || decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
        }

        /// <summary>ค่าตั้งจากหน้า Setting ไม่มีก็ถือว่าวัดเป็นตัวเลขตามค่าเริ่มต้นของคอลัมน์</summary>
        private static string GetJudgeType(DataRow sourceRow)
        {
            if (!sourceRow.Table.Columns.Contains(Utilities.PointJudgeType.ColumnName))
            {
                return Utilities.PointJudgeType.Numeric.ToString(CultureInfo.InvariantCulture);
            }

            return sourceRow[Utilities.PointJudgeType.ColumnName]?.ToString();
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
                    "??????????? Dimension",
                    "???????",
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
                        $"?? Cavity ????: ???? {pageNumber}, Sampling No {samplingNo}",
                        "???????",
                        CustomMsgBoxBase.MessageBoxIconType.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    CustomMsgBoxBase.ShowCustomMessageBox(
                        $"?? VALUE ????: Cavity {cavityName}, ???? {pageNumber}, Sampling No {samplingNo}",
                        "???????",
                        CustomMsgBoxBase.MessageBoxIconType.Warning);
                    return false;
                }

                if (string.IsNullOrWhiteSpace(pointJudge))
                {
                    CustomMsgBoxBase.ShowCustomMessageBox(
                        $"?????????????? Judge: Cavity {cavityName}, ???? {pageNumber}, Sampling No {samplingNo}",
                        "???????",
                        CustomMsgBoxBase.MessageBoxIconType.Warning);
                    return false;
                }

                if (equipmentType != "0" && string.IsNullOrWhiteSpace(equipmentSerial))
                {
                    CustomMsgBoxBase.ShowCustomMessageBox(
                        $"?? Equipment Serial ????: Cavity {cavityName}, ???? {pageNumber}, Sampling No {samplingNo}",
                        "???????",
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

            DataTable dimensionDataToSave = originalDataTable.Copy();

            foreach (DataRow row in dimensionDataToSave.Rows)
            {
                propQA.EQUIPMENT_SERIAL = row["EQUIPMENT_SERIAL"]?.ToString();
                propQA.EQUIPMENT_TYPE_ID = row["EQUIPMENT_TYPE"]?.ToString();

                if (!string.IsNullOrEmpty(propQA.EQUIPMENT_SERIAL) && !string.IsNullOrEmpty(propQA.EQUIPMENT_TYPE_ID))
                {
                    int id = conQA.InsertEquipmentSerial(propQA);
                    row["EQUIPMENT_SERIAL"] = id;
                }
            }

            propQA.dtgDimData = new DataGridView();
            propQA.dtgDimData.DataSource = dimensionDataToSave;

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
                                "Record Dimension ??? OK ?????????????",
                                "??????",
                                CustomMsgBoxBase.MessageBoxIconType.OK);
                            break;

                        case ProcStatus.Pending:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Dimension ????? ??? PENDING",
                                "??????",
                                CustomMsgBoxBase.MessageBoxIconType.Pending);
                            break;

                        case ProcStatus.NG:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Dimension ????? ??? NG",
                                "??????",
                                CustomMsgBoxBase.MessageBoxIconType.NG);
                            break;

                        default:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "??????????????",
                                "??????????",
                                CustomMsgBoxBase.MessageBoxIconType.Pending);
                            break;
                    }

                    loadstatus();
                    bt_status_Dimension_pending_Click();
                    return;
                }

                CustomMsgBoxBase.ShowCustomMessageBox(
                    "Record Dimension ?????? ??????? record ????????",
                    "??????????",
                    CustomMsgBoxBase.MessageBoxIconType.NG);
                return;
            }

            CustomMsgBoxBase.ShowCustomMessageBox(
                "Record Dimension ?????? ??????? record ????????",
                "??????????",
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
                        "????? ?????????? panelMain",
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

        private decimal ToDecimal(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }

            if (decimal.TryParse(value.ToString(), out decimal result))
            {
                return result;
            }

            return 0;
        }
    }
}
