﻿using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.ViewsMaterial.CustomMsg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RawMat.Property.QAdataProperty;

namespace RawMat.ViewsMaterial.RegularCheck
{
    public partial class userControlRegularPending : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler BackToARequested;
        //public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        private readonly SettingControllers settingController = new SettingControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();

        private BindingSource bindingSource = new BindingSource();
        private DataTable originalDataTable;
        private int currentPage = 1;
        private int totalPages = 1;

        // หน้า Pending โหลดมาเฉพาะจุดที่ตก เลขจุดจึงไม่ได้เริ่มที่ 1 และไม่ติดกัน
        // เช่นตกจุดเดียวที่ POINT_ORDER = 11 ก็จะมีหน้าเดียวที่เลขจุดคือ 11
        // เดิมกรองด้วยเลขหน้าตรงๆ (POINT_ORDER = 1) จึงไม่เจออะไรเลย ตารางว่างเปล่า
        // จึงต้องเก็บเลขจุดที่มีจริงไว้ แล้วเดินหน้าตามลำดับในลิสต์แทน
        private readonly List<string> pageOrders = new List<string>();


        private List<Image> regularImages;
        private int currentRegularImageIndex = 0;
        private Image _defaultImage = null;
        private readonly Dictionary<string, DataTable> equipmentSerialSourceByType = new Dictionary<string, DataTable>();

        public userControlRegularPending()
        {
            InitializeComponent();
            dtg_regular.DataError += dtg_regular_DataError;
        }

        private void dtg_regular_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
        }

        private async void userControlRegularPending_Load(object sender, EventArgs e)
        {

            lb_regularNo.Text = "Regular No : " + propQA.Regular_No;
            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");

            dtg_regular.CellEndEdit -= dtg_regular_CellEndEdit;
            dtg_regular.CellValidating -= dtg_regular_CellValidating;
            dtg_regular.CellFormatting -= dtg_regular_CellFormatting;
            dtg_regular.CellFormatting += dtg_regular_CellFormatting;

            //tb_pageMax.Text = "";
            //tb_pageCount.Text = ""; // 1 record 2 record ????? pageMax
            lb_sampName.Text = propQA.SAMPLING_NAME == "Fix"
                ? $"Quantity {propQA.SAMPLING_QTY} Pcs."
                : $"{propQA.SAMPLING_QTY} {propQA.SAMPLING_NAME}";

            regularImages = await imgCls.LoadImagesAsync("RegularPath", propQA.M_CODE);
            currentRegularImageIndex = 0;

            if (regularImages != null && regularImages.Count > 0)
            {
                picbox_reg.Image = regularImages[0];
            }
            else
            {
                // Fallback: LoadImages ?????? single ???? ?????????? return empty list
                picbox_reg.Image = _defaultImage;
            }


            DataTable dtRegPending = conQA.SearchRegularDataPending(propQA);

            if (dtRegPending.Rows.Count == 0)
            {
                MessageBox.Show("????? data regular ??? Pending ?? db_regular_data");
                return;
            }


            DataTable dtAllSum = new DataTable();


            if (propQA.SAMPLING_TYPE == "4" || propQA.SAMPLING_TYPE == "3")
            {

                picbox_cavity.Image = imgCls.LoadSingleImage("CavityPath", propQA.M_CODE);

                dtAllSum.Columns.Add("CAVITY_NAME", typeof(string));
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

                foreach (DataRow dtRow in dtRegPending.Rows)
                {

                    dtAllSum.Rows.Add(dtRow["CAVITY_NAME"].ToString(),
                    dtRow["SAMPLING_NO"].ToString(),
                    dtRow["POINT_ORDER"].ToString(),
                    dtRow["POINT_CAL"].ToString(),
                    GetSerialTextById(dtRow["EQUIPMENT_TYPE"].ToString(), dtRow["EQUIPMENT_SERIAL_ID"].ToString()),
                    dtRow["EQUIPMENT_TYPE"].ToString(),
                    dtRow["EQUIPMENT_NAME"].ToString(),
                    dtRow["POINT_NAME"].ToString(),
                    NumberDisplay.Trim(dtRow["VALUE"]),
                    Convert.ToDecimal(dtRow["CRITERIA_MIN"]),
                    Convert.ToDecimal(dtRow["CRITERIA_MAX"]),
                    GetJudgeType(dtRow),
                    "0", "0"
                    );

                }


            }
            else
            {
                gb_cavity.Visible = false;

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

                foreach (DataRow dtRow in dtRegPending.Rows)
                {

                    dtAllSum.Rows.Add(
                    dtRow["SAMPLING_NO"].ToString(),
                    dtRow["POINT_ORDER"].ToString(),
                    dtRow["POINT_CAL"].ToString(),
                    GetSerialTextById(dtRow["EQUIPMENT_TYPE"].ToString(), dtRow["EQUIPMENT_SERIAL_ID"].ToString()),
                    dtRow["EQUIPMENT_TYPE"].ToString(),
                    dtRow["EQUIPMENT_NAME"].ToString(),
                    dtRow["POINT_NAME"].ToString(),
                    NumberDisplay.Trim(dtRow["VALUE"]),
                    Convert.ToDecimal(dtRow["CRITERIA_MIN"]),
                    Convert.ToDecimal(dtRow["CRITERIA_MAX"]),
                    GetJudgeType(dtRow),
                    "0", "0"
                    );

                }

            }

            dtg_regular.DataSource = dtAllSum;

            string[] hiddenColumns = { "POINT_CAL", "POINT_ORDER", "EQUIPMENT_TYPE", "POINT_JUDGE", "TOTAL_JUDGE", "JUDGE_TYPE" };
            foreach (var col in hiddenColumns)
            {
                if (dtg_regular.Columns.Contains(col))
                {
                    dtg_regular.Columns[col].Visible = false;
                }
            }

            originalDataTable = (DataTable)dtg_regular.DataSource;
            bindingSource.DataSource = originalDataTable;
            dtg_regular.DataSource = bindingSource;

            foreach (DataGridViewColumn column in dtg_regular.Columns)
            {
                column.ReadOnly = (column.Name != "VALUE" && column.Name != "EQUIPMENT_SERIAL");
            }

            if (dtg_regular.Columns.Contains("CAVITY_NAME")) dtg_regular.Columns["CAVITY_NAME"].HeaderText = "CAV.";
            if (dtg_regular.Columns.Contains("SAMPLING_NO")) dtg_regular.Columns["SAMPLING_NO"].HeaderText = "SAMPLE";
            if (dtg_regular.Columns.Contains("POINT_NAME")) dtg_regular.Columns["POINT_NAME"].HeaderText = "CHECKPOINT";
            if (dtg_regular.Columns.Contains("EQUIPMENT_SERIAL")) dtg_regular.Columns["EQUIPMENT_SERIAL"].HeaderText = "EQ_SN";

            // ช่อง EQ_SN ให้เลือกจาก S/N ที่มีอยู่แล้ว แทนการพิมพ์เองซึ่งทำให้ master มีค่าซ้ำ
            EquipmentSerialColumn.Apply(dtg_regular, conQA.EquipmentSerialList());
            if (dtg_regular.Columns.Contains("EQUIPMENT_NAME")) dtg_regular.Columns["EQUIPMENT_NAME"].HeaderText = "EQ_NAME ";
            if (dtg_regular.Columns.Contains("CRITERIA_MIN"))
            {
                dtg_regular.Columns["CRITERIA_MIN"].HeaderText = "MIN";
                dtg_regular.Columns["CRITERIA_MIN"].DefaultCellStyle.Format = NumberDisplay.GridFormat;
            }
            if (dtg_regular.Columns.Contains("CRITERIA_MAX"))
            {
                dtg_regular.Columns["CRITERIA_MAX"].HeaderText = "MAX";
                dtg_regular.Columns["CRITERIA_MAX"].DefaultCellStyle.Format = NumberDisplay.GridFormat;
            }

            ApplyColumnWidths();

            // เก็บเลขจุดที่โหลดมาจริง เรียงตามค่าตัวเลข ไม่ใช่ตามตัวอักษร (2 ต้องมาก่อน 10)
            pageOrders.Clear();
            pageOrders.AddRange(originalDataTable.AsEnumerable()
                .Select(row => row["POINT_ORDER"].ToString())
                .Distinct()
                .OrderBy(order => int.TryParse(order, out int number) ? number : int.MaxValue)
                .ThenBy(order => order));

            totalPages = pageOrders.Count;

            ShowPage(currentPage);

            dtg_regular.CellEndEdit += dtg_regular_CellEndEdit;
            dtg_regular.EditingControlShowing += (s2, e2) => EquipmentSerialColumn.HandleEditingControlShowing(dtg_regular, e2);
            dtg_regular.DataError += (s2, e2) => EquipmentSerialColumn.HandleDataError(dtg_regular, e2);
            dtg_regular.CellValidating += dtg_regular_CellValidating;

            this.Disposed += UserControlRegular_Disposed;

            this.Focus();

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
            if (!dtg_regular.Columns.Contains(columnName)) { return; }

            dtg_regular.Columns[columnName].FillWeight = weight;
        }

        private void ShowPage(int page)
        {
            if (pageOrders.Count == 0)
            {
                bindingSource.Filter = null;
                lb_page.Text = "0/0";
                return;
            }

            // page คือลำดับหน้า ไม่ใช่เลขจุด ต้องแปลงก่อนเสมอ
            string pointOrder = pageOrders[Math.Min(Math.Max(page, 1), pageOrders.Count) - 1];

            bindingSource.Filter = $"POINT_ORDER = '{pointOrder}'";
            lb_page.Text = $"{page}/{totalPages}";
        }

        private void bt_prev_Click(object sender, EventArgs e)
        {
            if (currentPage > 1)
            {
                currentPage--;
                ShowPage(currentPage);
                //UpdateGrid();
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



        private bool IsDataTableValid(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                int pageNumber = row["POINT_ORDER"] != DBNull.Value ? Convert.ToInt32(row["POINT_ORDER"]) : 0;

                string samplingNo = row["SAMPLING_NO"] != DBNull.Value ? row["SAMPLING_NO"].ToString() : "N/A";

                foreach (DataColumn column in table.Columns)
                {
                    if (row[column] == DBNull.Value || string.IsNullOrWhiteSpace(row[column].ToString()))
                    {
                        string columnName = column.ColumnName;

                        MessageBox.Show($"????????????????? {pageNumber}, Sampling No {samplingNo}, ??????? {columnName}",
                            "???????", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            return true;
        }


        /// <summary>ค่าตั้งจากหน้า Setting ไม่มีก็ถือว่าวัดเป็นตัวเลขตามค่าเริ่มต้นของคอลัมน์</summary>
        private static string GetJudgeType(DataRow sourceRow)
        {
            if (!sourceRow.Table.Columns.Contains(Utilities.PointJudgeType.ColumnName))
            {
                return Utilities.PointJudgeType.Numeric.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            return sourceRow[Utilities.PointJudgeType.ColumnName]?.ToString();
        }

        private void dtg_regular_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dtg_regular.Columns[e.ColumnIndex].Name == "VALUE")
            {
                if (dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value != null &&
                    dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MAX"].Value != null)
                {
                    decimal minValue = Convert.ToDecimal(dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value);

                    // อ่านจากค่าที่ตั้งไว้ ไม่เดาจากเกณฑ์ ดู Utilities/PointJudgeType.cs
                    if (Utilities.PointJudgeType.IsPassFail(dtg_regular.Rows[e.RowIndex]))
                    {
                        if (!(dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell))
                        {
                            DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
                            // ต้องใช้ค่าตามเกณฑ์จริง ไม่ผูกกับ 1/0 ให้ตรงกับหน้ากรอกค่า
                            // ไม่งั้นจุดที่ตั้งเกณฑ์เป็นเลขอื่นจะโชว์ช่องว่างเพราะหาค่าเดิมในรายการไม่เจอ
                            string okValue = minValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            string ngValue = (minValue - 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

                            comboBoxCell.DataSource = new List<KeyValuePair<string, string>>()
             {
                 new KeyValuePair<string, string>("", ""),
                 new KeyValuePair<string, string>(ngValue, "NG"),
                 new KeyValuePair<string, string>(okValue, "OK")
             };
                            comboBoxCell.ValueMember = "Key";
                            comboBoxCell.DisplayMember = "Value";

                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                if (e.RowIndex >= 0 && e.RowIndex < dtg_regular.Rows.Count &&
                                    e.ColumnIndex >= 0 && e.ColumnIndex < dtg_regular.Columns.Count)
                                {
                                    dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] = comboBoxCell;
                                }
                            });
                        }
                    }
                    else
                    {
                        if (!(dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewTextBoxCell))
                        {
                            DataGridViewTextBoxCell textBoxCell = new DataGridViewTextBoxCell();
                            textBoxCell.Value = dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] = textBoxCell;
                            });
                        }
                    }
                }
            }

        }

        private void dtg_regular_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (dtg_regular.Columns[e.ColumnIndex].Name == "VALUE")
            {

                if (dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
                {
                    return;
                }

                string input = e.FormattedValue.ToString();

                if (string.IsNullOrWhiteSpace(input))
                {
                    //MessageBox.Show("???????????? ?????????????", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //e.Cancel = true;
                    return;
                }

                if (!IsValidDecimal(input))
                {
                    MessageBox.Show("??????????????????????? ?????????????????????????????? 1 ??????", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                }
            }
        }

        private bool IsValidDecimal(string input)
        {
            return decimal.TryParse(input, out _) && input.Count(c => c == '.') <= 1;
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

        private void dtg_regular_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dtg_regular.Columns[e.ColumnIndex].Name == "EQUIPMENT_SERIAL")
            {
                BindingSource bs = dtg_regular.DataSource as BindingSource;
                DataTable dtData = bs != null ? (DataTable)bs.DataSource : dtg_regular.DataSource as DataTable;
                if (dtData == null) return;

                string newSerial = dtg_regular.Rows[e.RowIndex].Cells["EQUIPMENT_SERIAL"].Value?.ToString();
                string eqType = dtg_regular.Rows[e.RowIndex].Cells["EQUIPMENT_TYPE"].Value?.ToString();

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
                    dtg_regular.Refresh();
                }

                return;
            }

            if (dtg_regular.Columns[e.ColumnIndex].Name == "VALUE")
            {
                DataGridViewRow row = dtg_regular.Rows[e.RowIndex];

                if (row.Cells["CRITERIA_MIN"].Value != null &&
                    row.Cells["CRITERIA_MAX"].Value != null &&
                    row.Cells["VALUE"].Value != null &&
                    !DBNull.Value.Equals(row.Cells["CRITERIA_MIN"].Value) &&
                    !DBNull.Value.Equals(row.Cells["CRITERIA_MAX"].Value) &&
                    !DBNull.Value.Equals(row.Cells["VALUE"].Value))
                {
                    decimal min = Convert.ToDecimal(row.Cells["CRITERIA_MIN"].Value);
                    decimal max = Convert.ToDecimal(row.Cells["CRITERIA_MAX"].Value);
                    decimal value;

                    if (decimal.TryParse(row.Cells["VALUE"].Value.ToString(), out value))
                    {
                        row.Cells["POINT_JUDGE"].Value = (value >= min && value <= max) ? 1 : 0;
                    }
                    else
                    {
                        row.Cells["POINT_JUDGE"].Value = DBNull.Value;
                    }

                    CalculateTotalJudge();
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

        private void CalculateTotalJudge()
        {
            foreach (DataRow dtRow in originalDataTable.Rows)
            {
                if (dtRow["POINT_JUDGE"] != null && dtRow["POINT_JUDGE"].ToString() == "0")
                {
                    SetTotalJudge(0);
                    return;
                }

            }
            SetTotalJudge(1);
        }

        private void SetTotalJudge(int value)
        {
            foreach (DataRow dtRow in originalDataTable.Rows)
            {
                dtRow["TOTAL_JUDGE"] = value;
            }
        }

        private void tb_record_Click(object sender, EventArgs e)
        {
            if (dtg_regular.IsCurrentCellDirty || dtg_regular.IsCurrentRowDirty)
            {
                dtg_regular.EndEdit();
                dtg_regular.CommitEdit(DataGridViewDataErrorContexts.Commit);
                bindingSource.EndEdit();
            }


            if (!IsDataTableValid(originalDataTable))
            {
                return;
            }

            propQA.TOTAL_STATUS = "1";
            propQA.EMP_ID = employee.EMP_CODE;

            // ? ????????? originalDataTable ???????????????????????????????????
            foreach (DataRow row in originalDataTable.Rows)
            {

                propQA.TOTAL_STATUS = (Convert.ToInt32(row["TOTAL_JUDGE"]?.ToString()) * Convert.ToInt32(propQA.TOTAL_STATUS)).ToString();
            }

            DataTable regularDataToSave = originalDataTable.Copy();

            foreach (DataRow row in regularDataToSave.Rows)
            {
                propQA.EQUIPMENT_SERIAL = row["EQUIPMENT_SERIAL"]?.ToString();
                propQA.EQUIPMENT_TYPE_ID = row["EQUIPMENT_TYPE"]?.ToString();

                if (!string.IsNullOrEmpty(propQA.EQUIPMENT_SERIAL) && !string.IsNullOrEmpty(propQA.EQUIPMENT_TYPE_ID))
                {
                    int id = conQA.InsertEquipmentSerial(propQA);
                    row["EQUIPMENT_SERIAL"] = id;
                }
            }

            propQA.dtgRegData = new DataGridView();
            propQA.dtgRegData.DataSource = regularDataToSave;


            if (conQA.InsertRegularData(propQA) == true)
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
                    //this.Controls.Clear();

                    ProcStatus status;

                    bool parsed = int.TryParse(propQA.inProcStatus, out int statusId) && Enum.IsDefined(typeof(ProcStatus), statusId);
                    status = parsed ? (ProcStatus)statusId : ProcStatus.NG;

                    switch (status)
                    {
                        case ProcStatus.OK:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Regular ??? OK ?????????????",
                                "??????",
                                CustomMsgBoxBase.MessageBoxIconType.OK);
                            break;
                        case ProcStatus.NG:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Regular ????? ??? NG",
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
                    bt_status_regular_pending_Click();
                    return;
                }
                else
                {
                    CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record regular status ?????? ??????? record ????????",
                                "??????????",
                                CustomMsgBoxBase.MessageBoxIconType.NG);
                    return;
                }
            }
            else
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record regular ?????? ??????? record ????????",
                                "??????????",
                                CustomMsgBoxBase.MessageBoxIconType.NG);
                return;
            }

        }

        //regular pending 
        private void bt_status_regular_pending_Click()
        {
            userControlSelectRegularPending usrSelectRegPending = new userControlSelectRegularPending();
            usrSelectRegPending.Dock = DockStyle.Fill;
            usrSelectRegPending.propQA = propQA;

            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
                //Control[] foundPanels = this.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrSelectRegPending);
                    usrSelectRegPending.BringToFront();
                }
                else
                {
                    MessageBox.Show("????? ?????????? panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                dtg_regular.ContainsFocus &&
                dtg_regular.CurrentCell != null &&
                dtg_regular.CurrentCell.OwningColumn.Name == "VALUE" &&
                dtg_regular.CurrentCell is DataGridViewTextBoxCell &&
                !dtg_regular.CurrentCell.ReadOnly)
            {
                int currentRowIndex = dtg_regular.CurrentCell.RowIndex;

                if (!dtg_regular.EndEdit())
                {
                    return true;
                }

                bindingSource.EndEdit();
                BeginInvoke(new Action(() => MoveToNextRegularValueRow(currentRowIndex)));
                return true;
            }

            if (regularImages != null && regularImages.Count > 1)
            {
                if (keyData == Keys.PageUp || keyData == Keys.PageDown)
                {
                    if (keyData == Keys.PageUp)
                    {
                        currentRegularImageIndex = (currentRegularImageIndex - 1 + regularImages.Count) % regularImages.Count;
                    }
                    else
                    {
                        currentRegularImageIndex = (currentRegularImageIndex + 1) % regularImages.Count;
                    }

                    // if (picbox_func.Image != null)
                    // {
                    //     picbox_func.Image.Dispose();
                    //     picbox_func.Image = null;
                    // }
                    picbox_reg.Image = regularImages[currentRegularImageIndex];

                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MoveToNextRegularValueRow(int currentRowIndex)
        {
            if (IsDisposed ||
                dtg_regular.IsDisposed ||
                !dtg_regular.IsHandleCreated ||
                !dtg_regular.Columns.Contains("VALUE"))
            {
                return;
            }

            for (int rowIndex = currentRowIndex + 1; rowIndex < dtg_regular.Rows.Count; rowIndex++)
            {
                DataGridViewCell valueCell = dtg_regular.Rows[rowIndex].Cells["VALUE"];
                if (!valueCell.Visible || valueCell.ReadOnly || valueCell is DataGridViewComboBoxCell)
                {
                    continue;
                }

                dtg_regular.CurrentCell = valueCell;
                dtg_regular.Focus();
                dtg_regular.BeginEdit(true);
                return;
            }
        }

        private void UserControlRegular_Disposed(object sender, EventArgs e)
        {
            // Dispose logic ????
            if (regularImages != null)
            {
                foreach (var img in regularImages)
                {
                    img?.Dispose();
                }
                regularImages.Clear();
                regularImages = null;
            }

            // Dispose ????? ????? (???? materialImages, cavityImages ??? hold list ???)

            // Unsubscribe event ???????????? memory leak
            this.Disposed -= UserControlRegular_Disposed;
        }


    }
}
