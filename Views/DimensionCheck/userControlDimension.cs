using Microsoft.Office.Interop.Excel;
using MySqlX.XDevAPI.Relational;
using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.CustomMsg;
using RawMat.Views.PackingCheck;
using RawMat.Views.RegularCheck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static RawMat.frmMain;
using static RawMat.Property.QAdataProperty;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Action = System.Action;
using DataTable = System.Data.DataTable;
using TextBox = System.Windows.Forms.TextBox;

namespace RawMat.Views.DimensionCheck
{
    public partial class userControlDimension : UserControl
    {
        public event Action<UserControl> AddUserControlRequested;
        public event Action OnReleaseMutex;
        public event EventHandler BackToARequested;
        public event Action<string> RequestReleaseMutex;
        public event Action OnClose;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        private readonly SettingControllers settingController = new SettingControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();

        private BindingSource bindingSource = new BindingSource();
        private DataTable originalDataTable;
        private int currentPage = 1;
        private int totalPages = 1;
        private frmMain mainForm;
        private string currentMutexKey; // ?????????? mutexKey
                                        // Add this event delegate at the top of your userControlRegular class:
        private IParent parent;
        public delegate void UserControlDisposedEventHandler(object sender, string reportNo);
        public event UserControlDisposedEventHandler UserControlDisposed;

        private System.Windows.Forms.Timer checkTimer;

        private List<Image> dimensionImages;
        private int currentDimensionImageIndex = 0;
        private Image _defaultImage = null; // ????????????? placeholder ????
        private readonly Dictionary<string, DataTable> equipmentSerialSourceByType = new Dictionary<string, DataTable>();
        private bool _isNavigatingAway;

        // Dictionary ????????? VALUE ???????? POINT_ORDER ??? SAMPLING_NO
        private Dictionary<string, Dictionary<string, decimal>> pointValues = new Dictionary<string, Dictionary<string, decimal>>();

        public userControlDimension(IParent parent)
        {
            InitializeComponent();
            this.parent = parent;

            dtg_dimension.TabStop = false;
            dtg_dimension.DataError += dtg_dimension_DataError;
            // ?????????? UserControl ?????????????????
            this.SetStyle(ControlStyles.Selectable, false);

            // ??????????????
            this.TabStop = false;

        }

        private void dtg_dimension_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
        }

        private void tb_record_Click(object sender, EventArgs e)
        {

            // ???????????????????????? DataGridView
            if (dtg_dimension.IsCurrentCellDirty || dtg_dimension.IsCurrentRowDirty)
            {
                dtg_dimension.EndEdit(); // ???????????????????????
                dtg_dimension.CommitEdit(DataGridViewDataErrorContexts.Commit); // ??????????? DataSource
                bindingSource.EndEdit(); // ????????????? BindingSource (??????)
            }

            if (dtg_dimension.Rows.Count == 0)
            {
                MessageBox.Show("???????? data ?????????? Dimension");
                return;
            }

            if (!IsDataTableValid(originalDataTable)) // ?????????? DataTable ???
            {
                return; // ??????????????????????
            }

            propQA.TOTAL_STATUS = "1";
            propQA.EMP_ID = employee.EMP_CODE;

            // ? ????????? originalDataTable ???????????????????????????????????
            foreach (DataRow row in originalDataTable.Rows)
            {
                propQA.EQUIPMENT_SERIAL = row["EQUIPMENT_SERIAL"]?.ToString();
                propQA.EQUIPMENT_TYPE_ID = row["EQUIPMENT_TYPE"]?.ToString();

                if (!string.IsNullOrEmpty(propQA.EQUIPMENT_SERIAL) && !string.IsNullOrEmpty(propQA.EQUIPMENT_TYPE_ID))
                {
                    int id = conQA.InsertEquipmentSerial(propQA);
                    row["EQUIPMENT_SERIAL"] = id; // ? ????????? ID ????????? DataTable
                }

                propQA.TOTAL_STATUS = (Convert.ToInt32(row["TOTAL_JUDGE"]?.ToString()) * Convert.ToInt32(propQA.TOTAL_STATUS)).ToString();
            }

            propQA.dtgDimData = new DataGridView();
            propQA.dtgDimData.DataSource = originalDataTable;

            try
            {
                if (conQA.InsertDimensionData(propQA) == true)
                {
                    if (propQA.TOTAL_STATUS == "0")
                    {
                        propQA.inProcStatus = "6";
                    }
                    else
                    {
                        propQA.inProcStatus = "1";
                    }

                    if (conQA.UpdateReportStatusLotNo(propQA) == true)
                    {
                        ProcStatus status;
                        bool parsed = int.TryParse(propQA.inProcStatus, out int statusId) && Enum.IsDefined(typeof(ProcStatus), statusId);
                        status = parsed ? (ProcStatus)statusId : ProcStatus.NG; // ??????????????? NG ?????????????

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
                            default:
                                CustomMsgBoxBase.ShowCustomMessageBox(
                                    "??????????????",
                                    "??????????",
                                    CustomMsgBoxBase.MessageBoxIconType.Pending);
                                break;
                        }

                        loadstatus();
                        bt_dim_Click();

                    }
                    else
                    {
                        CustomMsgBoxBase.ShowCustomMessageBox("????????? record data ?? database ???", "??????????", CustomMsgBoxBase.MessageBoxIconType.NG);

                    }
                }
                else
                {
                    CustomMsgBoxBase.ShowCustomMessageBox("????????? record data ?? database ???", "??????????", CustomMsgBoxBase.MessageBoxIconType.NG);
                }
            }
            finally
            {

                loadstatus();

                propQA.reportStatus = conQA.ReportFDA_Status(propQA);
                if (!conQA.UpdateReportStatus(propQA))
                {
                    MessageBox.Show("????????? update report status ???", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!conQA.DeleteReportActive(propQA))
                {
                    MessageBox.Show("????????????????? report no ???? ip ?????????????", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // ???????????? Timer
                if (checkTimer != null)
                {
                    checkTimer.Stop();
                    checkTimer.Dispose();
                }


            }
            return;
        }

        private async void userControlDimension_Load(object sender, EventArgs e)
        {
            bool hasCavity = int.TryParse(propQA.CAVITY_QTY, out int cavityQty) &&
                             cavityQty > 0 &&
                             propQA.dtCavity != null &&
                             propQA.dtCavity.Rows.Count > 0;

            // Set the final layout before loading images so the picture does not jump.
            gb_cavity.Visible = hasCavity;
            lb_TotalCavity.Visible = hasCavity;
            bt_confirmCavity.Enabled = hasCavity;
            picbox_dim.Location = hasCavity
                ? new System.Drawing.Point(17, 330)
                : new System.Drawing.Point(17, 113);
            picbox_dim.Size = hasCavity
                ? new Size(1076, 339)
                : new Size(1076, 556);

            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_lotSize.Text = "Lot Size :" + propQA.Qty;
            lb_lotNo.Text = "Lot No. : ";

            cb_lotNo.Items.Clear();

            // ?????????? propQA.dtLotNo ?????? null ??????????????
            if (propQA.dtLotNo != null && propQA.dtLotNo.Rows.Count > 0)
            {
                // ?????????????? DataTable ??????????? LOT_NO
                foreach (DataRow row in propQA.dtLotNo.Rows)
                {
                    string lotNo = row["LOT_NO"]?.ToString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(lotNo))
                    {
                        cb_lotNo.Items.Add(lotNo); // ????? LOT_NO ???? ComboBox
                    }
                }

                // ?????????????????? ComboBox ?????????????????
                if (cb_lotNo.Items.Count == 1)
                {
                    cb_lotNo.SelectedIndex = 0; // ?????????????? (??????????????) ?????????
                }
                else
                {
                    cb_lotNo.SelectedIndex = -1; // ?????????????????? 1 ??????
                }
            }
            else
            {
                cb_lotNo.SelectedIndex = -1; // ????????????????????
            }

            lb_sampName.Text = propQA.SAMPLING_NAME == "Fix"
                ? $"Quantity {propQA.SAMPLING_QTY} Pcs."
                : $"{propQA.SAMPLING_QTY} {propQA.SAMPLING_NAME}";

            // ??????? Function ??? async (?????? pagination ???? list ????????????)
            dimensionImages = await imgCls.LoadImagesAsync("DimensionPath", propQA.M_CODE);
            currentDimensionImageIndex = 0;

            if (dimensionImages != null && dimensionImages.Count > 0)
            {
                picbox_dim.Image = dimensionImages[0];
            }
            else
            {
                // Fallback: LoadImages ?????? single ???? ?????????? return empty list
                picbox_dim.Image = _defaultImage; // ???? null ???????? default
            }


            if (hasCavity)
            {
                lb_TotalCavity.Text = "Total Cavity : " + propQA.SAMPLING_QTY;

                picbox_cavity.Image = imgCls.LoadSingleImage("CavityPath", propQA.M_CODE);
                //picbox_dim.Image = imgCls.LoadDimensionImage(propQA.M_CODE);


                dtg_cavity.DataSource = propQA.dtCavity;


                // ??????????????????? "DATA_NO" ???????
                if (dtg_cavity.Columns["CAVITY_NAME"] != null)
                {
                    dtg_cavity.Columns["CAVITY_NAME"].HeaderText = "???????????";
                    dtg_cavity.Columns["CAVITY_NAME"].ReadOnly = true;
                }



                if (dtg_cavity.Columns["SAMPLING_QTY"] != null)
                {
                    dtg_cavity.Columns["SAMPLING_QTY"].HeaderText = "?????";
                }


            }
            else
            {
                if (!int.TryParse(propQA.SAMPLING_QTY, out int samplingQty) || samplingQty <= 0)
                {
                    MessageBox.Show("?????????? Sampling ?????? Dimension ??? M-CODE : " + propQA.M_CODE);
                    return;
                }

                if (propQA.dtDimEq == null || propQA.dtDimEq.Rows.Count == 0)
                {
                    MessageBox.Show("????? Dimension Equipment/Checkpoint ??? M-CODE : " + propQA.M_CODE);
                    return;
                }

                GenerateDataTableDimension(null, samplingQty);

            }

            // ?????????????????? Timer
            checkTimer = new System.Windows.Forms.Timer();
            checkTimer.Interval = 60000; // 3 ???? (180,000 ???????????)
            checkTimer.Tick += CheckTimer_Tick;
            checkTimer.Start();

            // ??????????????????????
            this.AutoScroll = true;

            // ????????????? Scrollbar ????????
            this.ScrollControlIntoView(lb_top);

            this.Focus();

            this.AutoScrollPosition = new System.Drawing.Point(0, 0);
            this.VerticalScroll.Value = 0;

        }

        // Event Handler ?????? Timer
        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (conQA.CheckReportStatus(propQA) == false)
            {
                CustomMsgBoxBase.ShowCustomMessageBox($"??????????? Pending ??? process ????", "?????????", CustomMsgBoxBase.MessageBoxIconType.NG);
                bt_dim_Click();
                checkTimer.Stop();
            }
        }

        private void dtg_dimension_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (_isNavigatingAway)
            {
                return;
            }

            // ???????????????????????? "Value"
            if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE")
            {

                if (dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
                {
                    return;
                }

                string input = e.FormattedValue.ToString();

                // ?????????????? ?????????????????????
                if (string.IsNullOrWhiteSpace(input))
                {
                    return;
                }

                // ???????????????????? ??????????????????? 1 ???
                if (!IsValidDecimal(input))
                {
                    MessageBox.Show("??????????????????????? ?????????????????????????????? 1 ??????", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // ???????????????????????
                }
            }
        }

        private bool IsValidDecimal(string input)
        {
            string pattern = @"^-?\d+(\.\d+)?(-)?$";
            return Regex.IsMatch(input, pattern);
        }

        private void dtg_dimension_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dtg_dimension.Columns[e.ColumnIndex].Name == "EQUIPMENT_SERIAL")
            {
                // ??? DataTable ??? DataGridView
                BindingSource bs = dtg_dimension.DataSource as BindingSource;
                DataTable dtData = bs != null ? (DataTable)bs.DataSource : dtg_dimension.DataSource as DataTable;
                if (dtData == null) return;

                // ?????? EQUIPMENT_SERIAL ??? EQUIPMENT_TYPE ??????????????
                string newSerial = dtg_dimension.Rows[e.RowIndex].Cells["EQUIPMENT_SERIAL"].Value?.ToString();
                string eqType = dtg_dimension.Rows[e.RowIndex].Cells["EQUIPMENT_TYPE"].Value?.ToString();

                // ??????????????????????????
                if (!string.IsNullOrEmpty(newSerial) && !string.IsNullOrEmpty(eqType))
                {
                    // ????????????????? EQUIPMENT_TYPE ????????
                    foreach (DataRow row in dtData.Rows)
                    {
                        if (row["EQUIPMENT_TYPE"].ToString() == eqType)
                        {
                            row["EQUIPMENT_SERIAL"] = newSerial;
                        }
                    }

                    // ?????? DataGridView ????????????????????
                    bs?.ResetBindings(false);
                    dtg_dimension.Refresh();
                    ApplyEquipmentSerialComboBoxes();
                }
            }

            // ???????????????? VALUE ?????????????? VALUE
            if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE")
            {
                CalculatePointValues();
            }
        }

        //private void CalculatePointValues()
        //{
        //    // ????????????? Dictionary
        //    pointValues.Clear();

        //    // ???? VALUE ???????? POINT_ORDER ?????????????? originalDataTable
        //    foreach (DataRow row in originalDataTable.Rows)
        //    {
        //        string pointOrder = row["POINT_ORDER"].ToString();
        //        string valueStr = row["VALUE"]?.ToString();
        //        string pointCal = row["POINT_CAL"]?.ToString();

        //        if (string.IsNullOrEmpty(pointCal) || pointCal == "0")
        //        {
        //            if (!string.IsNullOrWhiteSpace(valueStr) && decimal.TryParse(valueStr, out decimal value))
        //            {
        //                pointValues[pointOrder] = value;
        //            }
        //        }
        //    }

        //    // ????? VALUE ?????? POINT_ORDER ????? POINT_CAL ?????? "0"
        //    foreach (DataRow row in originalDataTable.Rows)
        //    {
        //        string pointCal = row["POINT_CAL"]?.ToString();
        //        if (!string.IsNullOrEmpty(pointCal) && pointCal != "0")
        //        {
        //            string[] orders = pointCal.Split('+');
        //            decimal sum = 0;
        //            bool canCalculate = true;

        //            foreach (string order in orders)
        //            {
        //                string trimmedOrder = order.Trim();
        //                if (pointValues.ContainsKey(trimmedOrder))
        //                {
        //                    sum += pointValues[trimmedOrder];
        //                }
        //                else
        //                {
        //                    canCalculate = false;
        //                    break;
        //                }
        //            }

        //            if (canCalculate)
        //            {
        //                row["VALUE"] = sum.ToString();
        //            }
        //            else
        //            {
        //                row["VALUE"] = DBNull.Value;
        //            }
        //        }
        //    }

        //    // ?????? UI
        //    dtg_dimension.Refresh();
        //}

        //private void CalculatePointValues()
        //{
        //    // ????????????? Dictionary
        //    pointValues.Clear();

        //    // ???? VALUE ???????? POINT_ORDER ??? EQUIPMENT_TYPE = 0 (???????) ????????????????
        //    int currentPage = Convert.ToInt32(bindingSource.Filter.Replace("POINT_ORDER = '", "").Replace("'", ""));
        //    foreach (DataGridViewRow row in dtg_dimension.Rows)
        //    {
        //        string pointOrder = row.Cells["POINT_ORDER"].Value?.ToString();
        //        string valueStr = row.Cells["VALUE"].Value?.ToString();
        //        string equipmentType = row.Cells["EQUIPMENT_TYPE"].Value?.ToString();

        //        if (equipmentType == "0" && pointOrder == currentPage.ToString())
        //        {
        //            if (!string.IsNullOrWhiteSpace(valueStr) && decimal.TryParse(valueStr, out decimal value))
        //            {
        //                pointValues[pointOrder] = value;
        //            }
        //        }
        //    }

        //    // ????? VALUE ?????? POINT_ORDER ??????????? EQUIPMENT_TYPE != "0"
        //    string currentEquipmentType = dtg_dimension.Rows[0].Cells["EQUIPMENT_TYPE"].Value?.ToString();
        //    if (!string.IsNullOrEmpty(currentEquipmentType) && currentEquipmentType != "0")
        //    {
        //        // ???????? EQUIPMENT_TYPE ??????????????????? POINT_ORDER ???????? (???? "1+2")
        //        string[] orders = currentEquipmentType.Split('+'); // ??? EQUIPMENT_TYPE ???????????????????
        //        decimal sum = 0;
        //        bool canCalculate = true;

        //        foreach (string order in orders)
        //        {
        //            string trimmedOrder = order.Trim();
        //            if (pointValues.ContainsKey(trimmedOrder))
        //            {
        //                sum += pointValues[trimmedOrder];
        //            }
        //            else
        //            {
        //                canCalculate = false;
        //                break;
        //            }
        //        }

        //        if (canCalculate)
        //        {
        //            foreach (DataGridViewRow row in dtg_dimension.Rows)
        //            {
        //                row.Cells["VALUE"].Value = sum.ToString(); // ??????? VALUE ??????????????????????????
        //            }
        //        }
        //        else
        //        {
        //            foreach (DataGridViewRow row in dtg_dimension.Rows)
        //            {
        //                row.Cells["VALUE"].Value = DBNull.Value;
        //            }
        //        }
        //    }

        //    // ?????? UI
        //    dtg_dimension.Refresh();
        //}

        private void CalculatePointValues()
        {
            if (isUpdating) return; // ??????????????????

            // ????????????? Dictionary
            pointValues.Clear();

            // ???? VALUE ?????? POINT_ORDER ??? SAMPLING_NO ?????????????????? originalDataTable
            Console.WriteLine("Dumping originalDataTable before calculation:");
            foreach (DataRow row in originalDataTable.Rows)
            {
                string pointOrder = row["POINT_ORDER"]?.ToString() ?? "";
                string samplingNo = row["SAMPLING_NO"]?.ToString() ?? "";
                string valueStr = row["VALUE"]?.ToString();
                string equipmentType = row["EQUIPMENT_TYPE"]?.ToString();

                string key = $"{samplingNo}_{pointOrder}";
                if (!string.IsNullOrWhiteSpace(valueStr) && decimal.TryParse(valueStr, out decimal value))
                {
                    if (!pointValues.ContainsKey(key))
                        pointValues[key] = new Dictionary<string, decimal>();
                    pointValues[key][pointOrder] = value;
                    Console.WriteLine($"Stored pointValues[{key}][{pointOrder}] = {value}, EQUIPMENT_TYPE = {equipmentType}");
                }
                else
                {
                    Console.WriteLine($"Skipped pointValues[{key}][{pointOrder}], VALUE = {valueStr}, EQUIPMENT_TYPE = {equipmentType}");
                }
            }

            // ????? VALUE ?????????????? originalDataTable
            Console.WriteLine($"Calculating for all pages, total rows in originalDataTable: {originalDataTable.Rows.Count}");

            isUpdating = true;

            try
            {
                foreach (DataRow row in originalDataTable.Rows)
                {
                    string pointCal = row["POINT_CAL"]?.ToString() ?? "";
                    string equipmentType = row["EQUIPMENT_TYPE"]?.ToString();
                    string pointOrder = row["POINT_ORDER"]?.ToString() ?? "";
                    string samplingNo = row["SAMPLING_NO"]?.ToString() ?? "";

                    Console.WriteLine($"Processing row (POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}), POINT_CAL = {pointCal}, EQUIPMENT_TYPE = {equipmentType}");

                    string key = $"{samplingNo}_{pointOrder}";
                    // ?????????? EQUIPMENT_TYPE ???? 0 ??? POINT_CAL ????????
                    if (equipmentType == "0" && !string.IsNullOrEmpty(pointCal) && pointCal.Contains("+"))
                    {
                        string[] orders = pointCal.Split('+');
                        decimal sum = 0;
                        bool canCalculate = true;

                        Console.WriteLine($"Calculating for POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}, PointCal: {pointCal}");
                        foreach (string order in orders)
                        {
                            string trimmedOrder = order.Trim();
                            string depKey = $"{samplingNo}_{trimmedOrder}";
                            Console.WriteLine($"Checking depKey={depKey}, trimmedOrder={trimmedOrder}");

                            if (pointValues.ContainsKey(depKey) && pointValues[depKey].ContainsKey(trimmedOrder))
                            {
                                sum += pointValues[depKey][trimmedOrder];
                                Console.WriteLine($"Adding {trimmedOrder}: {pointValues[depKey][trimmedOrder]}, Sum: {sum}");
                            }
                            else
                            {
                                canCalculate = false;
                                Console.WriteLine($"Missing value for {depKey}[{trimmedOrder}]");
                                break; // ?????????????????????????????????
                            }
                        }

                        if (canCalculate)
                        {
                            row["VALUE"] = sum.ToString();
                            Console.WriteLine($"Setting VALUE to {sum} for POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}");
                            // ?????????? CRITERIA_MIN ??? CRITERIA_MAX
                            if (row["CRITERIA_MIN"] != DBNull.Value && row["CRITERIA_MAX"] != DBNull.Value)
                            {
                                decimal min = Convert.ToDecimal(row["CRITERIA_MIN"]);
                                decimal max = Convert.ToDecimal(row["CRITERIA_MAX"]);
                                row["POINT_JUDGE"] = (sum >= min && sum <= max) ? 1 : 0;
                                Console.WriteLine($"Set POINT_JUDGE to {(sum >= min && sum <= max ? 1 : 0)} for sum={sum}, min={min}, max={max}");
                            }

                        }
                        else
                        {
                            row["VALUE"] = DBNull.Value;
                            row["POINT_JUDGE"] = DBNull.Value;
                            Console.WriteLine($"Cannot calculate, setting VALUE to null for POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}");
                        }
                    }
                    else
                    {
                        // ??? POINT_CAL ???? "0" ???????????? ??? VALUE ???????????
                        Console.WriteLine($"No calculation needed or invalid POINT_CAL for POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}");
                    }
                }
            }
            finally
            {
                isUpdating = false;
                bindingSource.ResetBindings(false); // ?????? UI ???????????????????
                Console.WriteLine("Calculation completed for all pages");
            }
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
            // ????????????? 1 ??? Total_Judge ???? 1
            SetTotalJudge(1);
        }

        private void SetTotalJudge(int value)
        {
            foreach (DataRow dtRow in originalDataTable.Rows)
            {
                //if (!row.IsNewRow)
                //{
                dtRow["TOTAL_JUDGE"] = value;
                //}
            }
        }

        private void bt_confirmCavity_Click(object sender, EventArgs e)
        {
            bool hasCavity = int.TryParse(propQA.CAVITY_QTY, out int cavityQty) &&
                             cavityQty > 0 &&
                             propQA.dtCavity != null &&
                             propQA.dtCavity.Rows.Count > 0;

            if (!hasCavity)
            {
                return;
            }

            dtg_cavity.EndEdit();

            int totalQty = 0;

            // ????????????????????????????? 0 ????????????
            foreach (DataGridViewRow row in dtg_cavity.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                string samplingQty = row.Cells["SAMPLING_QTY"].Value?.ToString();

                if (!int.TryParse(samplingQty, out int qty) || qty < 0)
                {
                    MessageBox.Show("?????????????? Cavity ????????????????? 0 ????????????!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                totalQty += qty;
            }

            if (totalQty != Convert.ToInt32(propQA.SAMPLING_QTY))
            {
                MessageBox.Show($"???????? QTY ??????? {Convert.ToInt32(propQA.SAMPLING_QTY)}  (????????: {totalQty})", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ???? dtg_data ?????????????????
            dtg_cavity.ReadOnly = true;


            GenerateDataTableDimension(dtg_cavity, 0);

        }

        //// ??????????????????????????? POINT_ORDER ???????????????
        //private void ShowPage(int page)
        //{
        //    bindingSource.Filter = $"POINT_ORDER = '{page}'"; // ????????????????? POINT_ORDER ??????????
        //    CalculatePointValues(); // ???????????????????????????????
        //    UpdateReadOnlyCells();
        //    lb_page.Text = $"{page}/{totalPages}"; // ???????? (1/8)
        //}

        private void ShowPage(int pageNumber)
        {
            currentPage = pageNumber;
            bindingSource.Filter = $"POINT_ORDER = '{pageNumber}'"; // ?????????????
            dtg_dimension.DataSource = bindingSource; // ?????? DataGridView
            dtg_dimension.Refresh(); // ??????????????????????????????????????
            CalculatePointValues(); // ???????????????????????????????
            UpdateReadOnlyCells();
            ApplyEquipmentSerialComboBoxes();
            lb_page.Text = $"{pageNumber}/{totalPages}";
            Console.WriteLine($"Switched to page {pageNumber}, filter applied: {bindingSource.Filter}");
        }

        private void UpdateGrid()
        {
            if (originalDataTable == null) return;

            // ????????????? DataTable ??????? POINT_ORDER ????????
            var filteredData = originalDataTable.AsEnumerable()
                .Where(row => Convert.ToInt32(row["POINT_ORDER"]) == currentPage);

            if (filteredData.Any())
            {
                bindingSource.DataSource = filteredData.CopyToDataTable();
            }
            else
            {
                bindingSource.DataSource = new DataTable(); // ????????????????????????? DataTable ?????
            }

            dtg_dimension.DataSource = bindingSource;

            // ?????? Label ?????????
            lb_page.Text = $"Page {currentPage} / {totalPages}";

            // ???????????????? Prev / Next ?????????
            bt_prev.Enabled = currentPage > 1;
            bt_next.Enabled = currentPage < totalPages;
        }

        private void dtg_cavity_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (_isNavigatingAway)
            {
                return;
            }

            if (e.ColumnIndex == dtg_cavity.Columns["SAMPLING_QTY"].Index)
            {
                string value = e.FormattedValue?.ToString();
                // ??????????????????????????? 0 ?????? ???????????????
                if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out int qty) || qty < 0)
                {
                    e.Cancel = true; // ?????????????????????????????????????
                }
            }
        }

        private void GenerateDataTableDimension(DataGridView dtgCavity, int sampQty)
        {
            dtg_dimension.CellEndEdit -= dtg_dimension_CellEndEdit;
            dtg_dimension.CellValidating -= dtg_dimension_CellValidating;
            dtg_dimension.CellFormatting -= dtg_dimension_CellFormatting;
            dtg_dimension.CellFormatting += dtg_dimension_CellFormatting;

            DataTable dtAllSum = new DataTable();

            if (dtgCavity != null)
            {
                dtAllSum.Columns.Add("CAVITY_NAME", typeof(string)); // ???????? Code B
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

            int qtySampCounter = 1;

            Console.WriteLine("Dumping propQA.dtDimEq for debugging:");
            foreach (DataRow row in propQA.dtDimEq.Rows)
            {
                Console.WriteLine($"POINT_ORDER: {row["POINT_ORDER"]}, POINT_CAL: {row["POINT_CAL"]}, EQUIPMENT_TYPE: {row["EQUIPMENT_TYPE"]}");
            }

            if (dtgCavity == null)
            {
                for (int i = 0; i < sampQty; i++)
                {
                    int qtySampNo = qtySampCounter++;

                    foreach (DataRow measureRow in propQA.dtDimEq.Rows)
                    {
                        string pointOrder = measureRow["POINT_ORDER"].ToString();
                        string pointCal = measureRow["POINT_CAL"].ToString();
                        string equipmentType = measureRow["EQUIPMENT_TYPE"].ToString();

                        dtAllSum.Rows.Add(
                            qtySampNo,
                            pointOrder,
                            pointCal,
                            null,
                            equipmentType,
                            measureRow["EQUIPMENT_NAME"].ToString(),
                            measureRow["POINT_NAME"].ToString(),
                            null,
                            Convert.ToDouble(measureRow["CRITERIA_MIN"]),
                            Convert.ToDouble(measureRow["CRITERIA_MAX"]),
                            null, null
                        );
                    }
                }
            }
            else
            {
                foreach (DataGridViewRow row in dtgCavity.Rows)
                {
                    string name = row.Cells["CAVITY_NAME"].Value.ToString();
                    int qty = Convert.ToInt32(row.Cells["SAMPLING_QTY"].Value);

                    for (int i = 0; i < qty; i++)
                    {
                        int qtySampNo = qtySampCounter++;

                        foreach (DataRow measureRow in propQA.dtDimEq.Rows)
                        {
                            string pointOrder = measureRow["POINT_ORDER"].ToString();
                            string pointCal = measureRow["POINT_CAL"].ToString();
                            string equipmentType = measureRow["EQUIPMENT_TYPE"].ToString();

                            dtAllSum.Rows.Add(
                                name,
                                qtySampNo,
                                pointOrder,
                                pointCal,
                                null,
                                equipmentType,
                                measureRow["EQUIPMENT_NAME"].ToString(),
                                measureRow["POINT_NAME"].ToString(),
                                null,
                                Convert.ToDouble(measureRow["CRITERIA_MIN"]),
                                Convert.ToDouble(measureRow["CRITERIA_MAX"]),
                                null, null
                            );
                        }
                    }
                }
            }

            dtg_dimension.DataSource = dtAllSum;

            // ????????????????????????????
            string[] hiddenColumns = { "POINT_CAL", "POINT_ORDER", "EQUIPMENT_TYPE", "POINT_JUDGE", "TOTAL_JUDGE" };
            foreach (var col in hiddenColumns)
            {
                if (dtg_dimension.Columns.Contains(col))
                {
                    dtg_dimension.Columns[col].Visible = false;
                }
            }


            // ????????????????????? "VALUE" ??? "EQUIPMENT_SERIAL" ???? ReadOnly
            foreach (DataGridViewColumn column in dtg_dimension.Columns)
            {
                column.ReadOnly = (column.Name != "VALUE" && column.Name != "EQUIPMENT_SERIAL");
            }

            //// ??????? ReadOnly ??????????? VALUE ????? POINT_CAL ?????? "0"
            //foreach (DataGridViewRow row in dtg_dimension.Rows)
            //{
            //    string pointCal = row.Cells["POINT_CAL"].Value?.ToString();
            //    if (!string.IsNullOrEmpty(pointCal) && pointCal != "0")
            //    {
            //        row.Cells["VALUE"].ReadOnly = true;
            //    }
            //}

            // ??????? ReadOnly ??????????????????????????????
            UpdateReadOnlyCells();


            // ???????????????????
            //originalDataTable = (DataTable)dtg_dimension.DataSource;
            //bindingSource.DataSource = originalDataTable;
            //dtg_dimension.DataSource = bindingSource;

            originalDataTable = dtAllSum.Copy(); // ??? Copy ?????????????????????????????
            bindingSource.DataSource = originalDataTable;
            dtg_dimension.DataSource = bindingSource;

            // ?????????? POINT_ORDER ????????????
            totalPages = originalDataTable.AsEnumerable()
                .Select(row => row["POINT_ORDER"].ToString())
                .Distinct()
                .Count();

            ShowPage(currentPage);

            // ??????? HeaderText
            if (dtg_dimension.Columns.Contains("CAVITY_NAME")) dtg_dimension.Columns["CAVITY_NAME"].HeaderText = "CAV.";
            if (dtg_dimension.Columns.Contains("SAMPLING_NO")) dtg_dimension.Columns["SAMPLING_NO"].HeaderText = "SAMPLE";
            if (dtg_dimension.Columns.Contains("POINT_NAME")) dtg_dimension.Columns["POINT_NAME"].HeaderText = "CHECKPOINT";
            if (dtg_dimension.Columns.Contains("EQUIPMENT_SERIAL")) dtg_dimension.Columns["EQUIPMENT_SERIAL"].HeaderText = "EQ_SN";
            if (dtg_dimension.Columns.Contains("EQUIPMENT_NAME")) dtg_dimension.Columns["EQUIPMENT_NAME"].HeaderText = "EQ_NAME ";
            if (dtg_dimension.Columns.Contains("CRITERIA_MIN")) dtg_dimension.Columns["CRITERIA_MIN"].HeaderText = "MIN";
            if (dtg_dimension.Columns.Contains("CRITERIA_MAX")) dtg_dimension.Columns["CRITERIA_MAX"].HeaderText = "MAX";

            // ?????????? VALUE ??????? ComboBox ???????
            //dtg_regular.CellFormatting += (sender, e) =>
            //{
            //    if (dtg_regular.Columns[e.ColumnIndex].Name == "VALUE")
            //    {
            //        double minValue = Convert.ToDouble(dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value);
            //        double maxValue = Convert.ToDouble(dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MAX"].Value);

            //        if (minValue == 1 && maxValue == 1)
            //        {
            //            // ??? ComboBoxColumn
            //            DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
            //            comboBoxCell.DataSource = new List<KeyValuePair<string, string>>()
            //            {
            //                new KeyValuePair<string, string>("", ""), // Null ???????????????
            //                new KeyValuePair<string, string>("0", "NG"),
            //                new KeyValuePair<string, string>("1", "OK")
            //            };
            //            comboBoxCell.ValueMember = "Key";
            //            comboBoxCell.DisplayMember = "Value";

            //            dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] = comboBoxCell;
            //        }
            //        else
            //        {
            //            // ??? TextBoxColumn
            //            DataGridViewTextBoxCell textBoxCell = new DataGridViewTextBoxCell();
            //            textBoxCell.Value = dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            //            dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] = textBoxCell;
            //        }
            //    }
            //};



            dtg_dimension.CellEndEdit += dtg_dimension_CellEndEdit;
            dtg_dimension.CellValidating += dtg_dimension_CellValidating;
            dtg_dimension.DataBindingComplete += dtg_dimension_DataBindingComplete;
            // dtg_regular.EditingControlShowing += dtg_regular_EditingControlShowing;

        }




        private bool IsDataTableValid(DataTable table)
        {
            foreach (DataRow row in table.Rows)
            {
                // ?????????????? (POINT_ORDER)
                int pageNumber = row["POINT_ORDER"] != DBNull.Value ? Convert.ToInt32(row["POINT_ORDER"]) : 0;

                // ?????? Sampling No (?????????? Row Index)
                string samplingNo = row["SAMPLING_NO"] != DBNull.Value ? row["SAMPLING_NO"].ToString() : "N/A";

                foreach (DataColumn column in table.Columns)
                {
                    if (row[column] == DBNull.Value || string.IsNullOrWhiteSpace(row[column].ToString()))
                    {
                        string columnName = column.ColumnName; // ???????????

                        if (columnName == "EQUIPMENT_SERIAL")
                        {
                            CustomMsgBoxBase.ShowCustomMessageBox($"????????????????? {pageNumber}, Sample {samplingNo}, ??????? EQ_SN",
                                "???????", CustomMsgBoxBase.MessageBoxIconType.Warning);
                        }
                        else if (columnName == "VALUE")
                        {
                            string pointCal = row["POINT_CAL"]?.ToString();
                            if (string.IsNullOrEmpty(pointCal) || pointCal == "0")
                            {
                                CustomMsgBoxBase.ShowCustomMessageBox($"????????????????? {pageNumber}, Sample {samplingNo}, ??????? {columnName}",
                                   "???????", CustomMsgBoxBase.MessageBoxIconType.Warning);
                                return false;
                            }
                        }
                        else
                        {

                            CustomMsgBoxBase.ShowCustomMessageBox($"????????????????? {pageNumber}, Sample {samplingNo}, ??????? {columnName}",
                                "???????", CustomMsgBoxBase.MessageBoxIconType.Warning);
                        }
                        return false;
                    }
                }
            }
            return true;
        }

        private bool IsDataGridViewValid(DataGridView dtg)
        {
            foreach (DataGridViewRow row in dtg.Rows)
            {
                // ??????????????????????????????? (AllowUserToAddRows = true)
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    // ?????????????????
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        CustomMsgBoxBase.ShowCustomMessageBox($"??????????????????? {row.Index + 1} ??????? {dtg.Columns[cell.ColumnIndex].HeaderText}", "???????", CustomMsgBoxBase.MessageBoxIconType.Warning);
                        dtg.CurrentCell = cell; // ?????????????????????????? Active
                        return false;
                    }
                }
            }
            return true;
        }

        private void dtg_cavity_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dtg_cavity.CurrentCell.ColumnIndex == dtg_cavity.Columns["SAMPLING_QTY"].Index)
            {

                if (e.Control is TextBox textBox)
                {
                    // ?? Event ???? (?????) ??????????????????????
                    textBox.KeyPress -= TextBox_KeyPress;

                    // ????? Event ????
                    textBox.KeyPress += TextBox_KeyPress;
                }


            }

        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dtg_cavity.CurrentCell.ColumnIndex == dtg_cavity.Columns["SAMPLING_QTY"].Index)
            {
                // ?????????????????????????????? (???? Backspace, Delete)
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true; // ???????????????????????????
                }
            }
        }

        private void bt_back_Click(object sender, EventArgs e)
        {
            propQA.inProcStatus = ((int)ProcStatus.Unfinished).ToString();
            propQA.reportStatus = ((int)ProcStatus.Unfinished).ToString();

            if (!conQA.UpdateStatus(propQA))
            {
                MessageBox.Show("????????????????????????????? Unfinished ???", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!conQA.DeleteReportActive(propQA))
            {
                MessageBox.Show("?????????????? report no ??? IP ???", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            bt_dim_Click();
        }

        public void bt_dim_Click()
        {
            PrepareForNavigation();

            userControlSelectDimension usrConSelectDim = new userControlSelectDimension()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()
            };

            usrConSelectDim.Dock = DockStyle.Fill;
            usrConSelectDim.propQA = new QAdataProperty();

            usrConSelectDim.propQA.labelProcess = "Select Report for : Dimension Check";
            usrConSelectDim.propQA.process = "Dimension_Check";
            usrConSelectDim.propQA.prevProcess = "Inspection_Data_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpDimension(usrConSelectDim.propQA);
            usrConSelectDim.propQA.dtgRawMat = new DataGridView();

            // ????????????????? "Status" ??????? null ?????????? "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectDim.propQA.dtgRawMat.DataSource = dt;

            var parentForm = this.FindForm() as frmMain;
            parentForm?.ControlBackLevel(employee);

            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
                //Control[] foundPanels = this.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    // ??????????????? UserControl ????

                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrConSelectDim);
                    usrConSelectDim.BringToFront();
                }
                else
                {
                    MessageBox.Show("????? ?????????? panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //else
            //{
            //    Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
            //    //Control[] foundPanels = this.Controls.Find("panelMain", true);

            //    if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
            //    {
            //        // ??????????????? UserControl ????

            //        panelMain.Controls.Clear();
            //        panelMain.Controls.Add(usrConSelectReg);
            //        usrConSelectReg.BringToFront();
            //    }
            //    else
            //    {
            //        MessageBox.Show("????? ?????????? panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}



        }

        private void PrepareForNavigation()
        {
            _isNavigatingAway = true;

            if (checkTimer != null)
            {
                checkTimer.Stop();
            }

            CancelGridEdit(dtg_dimension);
            CancelGridEdit(dtg_cavity);
            bindingSource?.CancelEdit();
        }

        private void CancelGridEdit(DataGridView grid)
        {
            if (grid == null || grid.IsDisposed)
            {
                return;
            }

            try
            {
                grid.CancelEdit();

                if (grid.DataSource != null)
                {
                    BindingContext[grid.DataSource]?.CancelCurrentEdit();
                }
            }
            catch (InvalidOperationException)
            {
                // The grid is already being detached from its binding context.
            }
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

        void UpdateReadOnlyCells()
        {
            foreach (DataGridViewRow row in dtg_dimension.Rows)
            {
                string pointCal = row.Cells["POINT_CAL"].Value?.ToString();
                if (!string.IsNullOrEmpty(pointCal) && pointCal != "0")
                {
                    row.Cells["VALUE"].ReadOnly = true;
                }
                else
                {
                    row.Cells["VALUE"].ReadOnly = false; // ????????????????????????????????????
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

                EnsureCurrentSerialExists(serialSource, currentValue);

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
                    string serial = row["Equipment_Serial"]?.ToString()?.Trim();
                    if (string.IsNullOrWhiteSpace(serial)) continue;

                    bool exists = serialSource.AsEnumerable()
                        .Any(r => string.Equals(r["VALUE"].ToString(), serial, StringComparison.OrdinalIgnoreCase));

                    if (!exists)
                    {
                        serialSource.Rows.Add(serial, serial);
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

        private static void EnsureCurrentSerialExists(DataTable serialSource, object currentValue)
        {
            string serial = currentValue == null || currentValue == DBNull.Value
                ? ""
                : currentValue.ToString().Trim();

            if (string.IsNullOrWhiteSpace(serial)) return;

            bool exists = serialSource.AsEnumerable()
                .Any(row => string.Equals(row["VALUE"].ToString(), serial, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                serialSource.Rows.Add(serial, serial);
            }
        }

 

        private decimal CalculateSumForPoint(DataRow row)
        {
            string pointCal = row["POINT_CAL"]?.ToString();
            if (string.IsNullOrEmpty(pointCal) || pointCal == "0")
            {
                return 0; // ??????????????? POINT_CAL ???? 0 ????????
            }

            decimal sum = 0;
            string[] orders = pointCal.Split('+'); // ??? POINT_ORDER ??????????? (???? "1+2+3")
            bool canCalculate = true;

            foreach (string order in orders)
            {
                string trimmedOrder = order.Trim();
                // ?????????? originalDataTable ????????? POINT_ORDER
                var relatedRows = originalDataTable.AsEnumerable()
                    .Where(r => r["POINT_ORDER"].ToString() == trimmedOrder && r["VALUE"] != DBNull.Value);

                if (relatedRows.Any())
                {
                    decimal value = relatedRows.Select(r => Convert.ToDecimal(r["VALUE"])).FirstOrDefault();
                    sum += value;
                }
                else
                {
                    canCalculate = false; // ?????????????????????????
                    break;
                }
            }

            return canCalculate ? sum : 0; // ?????? 0 ??????????????
        }

        //private void dtg_regular_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{
        //    DataGridViewTextBoxEditingControl textBox = e.Control as DataGridViewTextBoxEditingControl;
        //    if (textBox != null)
        //    {
        //        // ??????????????????????????????????? ComboBox
        //        int columnIndex = dtg_regular.CurrentCell.ColumnIndex;
        //        if (dtg_regular.Columns[columnIndex].Name == "VALUE")
        //        {
        //            // ??????????? ComboBox
        //            DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
        //            comboBoxCell.DataSource = new List<KeyValuePair<string, string>>()
        //    {
        //        new KeyValuePair<string, string>("", null),
        //        new KeyValuePair<string, string>("NG", "0"),
        //        new KeyValuePair<string, string>("OK", "1")
        //    };
        //            comboBoxCell.DisplayMember = "Key";
        //            comboBoxCell.ValueMember = "Value";

        //            // ??????????? ?????????? BeginInvoke ???????????? StackOverflow
        //            this.BeginInvoke((MethodInvoker)delegate
        //            {
        //                dtg_regular.Rows[dtg_regular.CurrentCell.RowIndex].Cells[columnIndex] = comboBoxCell;
        //            });
        //        }
        //    }
        //}

        private void dtg_dimension_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE")
            {
                // ?????????????????? CRITERIA_MIN ??? CRITERIA_MAX ?????
                if (dtg_dimension.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value != null &&
                    dtg_dimension.Rows[e.RowIndex].Cells["CRITERIA_MAX"].Value != null)
                {
                    double minValue = Convert.ToDouble(dtg_dimension.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value);
                    double maxValue = Convert.ToDouble(dtg_dimension.Rows[e.RowIndex].Cells["CRITERIA_MAX"].Value);

                    // ????????: ??? CRITERIA_MIN == 1 && CRITERIA_MAX == 1 ?????? ComboBoxCell
                    if (minValue == 1 && maxValue == 1)
                    {
                        // ??????????????? VALUE ????????? ComboBoxCell
                        if (!(dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell))
                        {
                            DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
                            comboBoxCell.DataSource = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("", ""),  // ????????
                        new KeyValuePair<string, string>("0", "NG"),
                        new KeyValuePair<string, string>("1", "OK")
                    };
                            comboBoxCell.ValueMember = "Key";
                            comboBoxCell.DisplayMember = "Value";

                            // ??? BeginInvoke ??????????????????????? CellFormatting ???
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                // ?????????? RowIndex ??? ColumnIndex ???????????????? DataGridView
                                if (e.RowIndex >= 0 && e.RowIndex < dtg_dimension.Rows.Count &&
                                    e.ColumnIndex >= 0 && e.ColumnIndex < dtg_dimension.Columns.Count)
                                {
                                    dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex] = comboBoxCell;
                                }
                            });
                        }
                    }
                    else
                    {
                        // ????????????????? ?????? TextBoxCell
                        if (!(dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewTextBoxCell))
                        {
                            DataGridViewTextBoxCell textBoxCell = new DataGridViewTextBoxCell();
                            textBoxCell.Value = dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex] = textBoxCell;
                            });
                        }
                    }
                }
            }

        }

        private void dtg_dimension_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

            UpdateReadOnlyCells(); // ??????????? ReadOnly ????????????????
            ApplyEquipmentSerialComboBoxes();

            foreach (DataGridViewRow row in dtg_dimension.Rows)
            {
                // ????????????????? VALUE ??? POINT_JUDGE ???????
                if (row.Cells["VALUE"].Value != null &&
                    !string.IsNullOrWhiteSpace(row.Cells["VALUE"].Value.ToString()) &&
                    row.Cells["POINT_JUDGE"].Value != null &&
                    row.Cells["POINT_JUDGE"].Value.ToString() == "0")
                {
                    row.DefaultCellStyle.BackColor = Color.Red; // ??????????????????? POINT_JUDGE = "0"
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White; // ?????????????????????? (??????????)
                }
            }
        }

        //private void dtg_dimension_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{
        //    if (dtg_dimension.CurrentCell.ColumnIndex == dtg_dimension.Columns["VALUE"].Index)
        //    {


        //        if (e.Control is TextBox textBox)
        //        {
        //            textBox.Leave -= TextBox_Leave;
        //            textBox.TextChanged -= TextBox_TextChanged;

        //            textBox.Leave += TextBox_Leave;
        //            textBox.TextChanged += TextBox_TextChanged;
        //        }
        //    }
        //}

        //private void TextBox_Leave(object sender, EventArgs e)
        //{

        //    Console.WriteLine($"Typing session ended (on leave). Duration:");



        //}

        //private void dtg_regular_CellLeave(object sender, DataGridViewCellEventArgs e)
        //{
        //    //if (dtg_regular.Columns[e.ColumnIndex].Name == "VALUE")
        //    //{
        //    //    // ???????????????????????????

        //    //}
        //}
        // next function
        //private void TextBox_TextChanged(object sender, EventArgs e)
        //{

        //    TextBox textBox = sender as TextBox;




        //    //_isKeyboardInputDetected = true;
        //    //MessageBox.Show("????????????????????????????? keyboard", "???????", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //    //if (textBox != null)
        //    //{
        //    //    textBox.Text = string.Empty;
        //    //}
        //    //dtg_regular.CurrentCell.Value = null;
        //    //dtg_regular.EndEdit();

        //}

        //private void dtg_dimension_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE")
        //    {
        //        DataGridViewRow row = dtg_dimension.Rows[e.RowIndex];
        //        string pointOrder = row.Cells["POINT_ORDER"].Value?.ToString();
        //        int rowIndex = row.Index;

        //        // ????? POINT_JUDGE ???????????????????
        //        if (row.Cells["CRITERIA_MIN"].Value != null &&
        //            row.Cells["CRITERIA_MAX"].Value != null &&
        //            row.Cells["VALUE"].Value != null &&
        //            !DBNull.Value.Equals(row.Cells["CRITERIA_MIN"].Value) &&
        //            !DBNull.Value.Equals(row.Cells["CRITERIA_MAX"].Value) &&
        //            !DBNull.Value.Equals(row.Cells["VALUE"].Value))
        //        {
        //            decimal min = Convert.ToDecimal(row.Cells["CRITERIA_MIN"].Value);
        //            decimal max = Convert.ToDecimal(row.Cells["CRITERIA_MAX"].Value);
        //            decimal value;

        //            if (decimal.TryParse(row.Cells["VALUE"].Value.ToString(), out value))
        //            {
        //                row.Cells["POINT_JUDGE"].Value = (value >= min && value <= max) ? 1 : 0;
        //            }
        //            else
        //            {
        //                row.Cells["POINT_JUDGE"].Value = DBNull.Value;
        //            }

        //            CalculateTotalJudge();
        //        }

        //        // ?????? originalDataTable ?????????????? dtg_dimension ?????? POINT_ORDER ????????
        //        if (originalDataTable != null)
        //        {
        //            foreach (DataRow dataRow in originalDataTable.Rows)
        //            {
        //                if (dataRow["POINT_ORDER"].ToString() == pointOrder)
        //                {
        //                    dataRow["VALUE"] = row.Cells["VALUE"].Value;
        //                    Console.WriteLine($"original POINT_ORDER {pointOrder} with VALUE: {row.Cells["VALUE"].Value}");
        //                    break;
        //                }
        //            }
        //        }

        //    }

        //    // ???????????????? VALUE ????? VALUE ???????????
        //    CalculatePointValues();
        //}

        private bool isUpdating = false; // ???????????????????????????????????

        private void dtg_dimension_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE" && !isUpdating)
            {
                // ????????????????
                isUpdating = true;
                try
                {
                    DataGridViewRow row = dtg_dimension.Rows[e.RowIndex];
                    string pointOrder = row.Cells["POINT_ORDER"].Value?.ToString() ?? "";
                    string samplingNo = row.Cells["SAMPLING_NO"].Value?.ToString() ?? "";

                    // ?????????? VALUE ????????
                    string valueStr = row.Cells["VALUE"].Value?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(valueStr))
                    {
                        // ??????????? ????????? originalDataTable ???? DBNull ???????????????
                        DataRow[] matchingRows = originalDataTable.Select($"POINT_ORDER = '{pointOrder}' AND SAMPLING_NO = {samplingNo}");
                        if (matchingRows.Length > 0)
                        {
                            matchingRows[0]["VALUE"] = DBNull.Value;
                            Console.WriteLine($"Updated originalDataTable: POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}, VALUE=null");
                        }
                        return; // ?????????????????
                    }

                    // ????? POINT_JUDGE ???????????????????
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

                        if (decimal.TryParse(valueStr, out value))
                        {
                            row.Cells["POINT_JUDGE"].Value = (value >= min && value <= max) ? 1 : 0;
                        }
                        else
                        {
                            row.Cells["POINT_JUDGE"].Value = DBNull.Value;
                        }

                        CalculateTotalJudge();
                    }

                    // ?????? originalDataTable ???????????
                    DataRow[] rows = originalDataTable.Select($"POINT_ORDER = '{pointOrder}' AND SAMPLING_NO = {samplingNo}");
                    if (rows.Length > 0)
                    {
                        rows[0]["VALUE"] = valueStr;
                        Console.WriteLine($"Updated originalDataTable: POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}, VALUE={valueStr}");
                    }
                    else
                    {
                        Console.WriteLine($"No matching row found in originalDataTable for POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}");
                    }

                    // ??????????????
                    CalculatePointValues();
                }
                finally
                {
                    isUpdating = false;
                }
            }
        }

        //private void dtg_dimension_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE" && !isUpdating)
        //    {
        //        // ???????????????????????????
        //        dtg_dimension.EndEdit();
        //        bindingSource.EndEdit();

        //        DataGridViewRow row = dtg_dimension.Rows[e.RowIndex];
        //        string pointOrder = row.Cells["POINT_ORDER"].Value?.ToString();
        //        string samplingNo = row.Cells["SAMPLING_NO"].Value?.ToString();

        //        // ????? POINT_JUDGE ???????????????????
        //        if (row.Cells["CRITERIA_MIN"].Value != null &&
        //            row.Cells["CRITERIA_MAX"].Value != null &&
        //            row.Cells["VALUE"].Value != null &&
        //            !DBNull.Value.Equals(row.Cells["CRITERIA_MIN"].Value) &&
        //            !DBNull.Value.Equals(row.Cells["CRITERIA_MAX"].Value) &&
        //            !DBNull.Value.Equals(row.Cells["VALUE"].Value))
        //        {
        //            decimal min = Convert.ToDecimal(row.Cells["CRITERIA_MIN"].Value);
        //            decimal max = Convert.ToDecimal(row.Cells["CRITERIA_MAX"].Value);
        //            decimal value;

        //            if (decimal.TryParse(row.Cells["VALUE"].Value.ToString(), out value))
        //            {
        //                row.Cells["POINT_JUDGE"].Value = (value >= min && value <= max) ? 1 : 0;
        //            }
        //            else
        //            {
        //                row.Cells["POINT_JUDGE"].Value = DBNull.Value;
        //            }

        //            CalculateTotalJudge();
        //        }

        //        // ?????????? bindingSource ???????????????????????
        //        isUpdating = true;
        //        try
        //        {
        //            DataRow[] rows = originalDataTable.Select($"POINT_ORDER = '{pointOrder}' AND SAMPLING_NO = {samplingNo}");
        //            if (rows.Length > 0)
        //            {
        //                rows[0]["VALUE"] = row.Cells["VALUE"].Value;
        //                Console.WriteLine($"Updated originalDataTable: POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}, VALUE={row.Cells["VALUE"].Value}");
        //            }
        //            else
        //            {
        //                Console.WriteLine($"No matching row found in originalDataTable for POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}");
        //            }
        //        }
        //        finally
        //        {
        //            isUpdating = false;
        //        }

        //        // ?????????????????????????????
        //        CalculatePointValues();
        //    }
        //}

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
                BeginInvoke(new Action(() => MoveToNextValueRow(currentRowIndex)));
                return true;
            }

            if (dimensionImages != null && dimensionImages.Count > 1)
            {
                if (keyData == Keys.PageUp || keyData == Keys.PageDown)
                {
                    if (keyData == Keys.PageUp)
                    {
                        currentDimensionImageIndex = (currentDimensionImageIndex - 1 + dimensionImages.Count) % dimensionImages.Count;
                    }
                    else
                    {
                        currentDimensionImageIndex = (currentDimensionImageIndex + 1) % dimensionImages.Count;
                    }

                    // ?????? dispose ??? ??????????????? dispose Image ?? list
                    // if (picbox_func.Image != null)
                    // {
                    //     picbox_func.Image.Dispose();
                    //     picbox_func.Image = null;
                    // }
                    picbox_dim.Image = dimensionImages[currentDimensionImageIndex];

                    return true; // ???????????? key ???? ???????????
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void MoveToNextValueRow(int currentRowIndex)
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (dimensionImages != null)
                {
                    foreach (var img in dimensionImages)
                    {
                        img?.Dispose();
                    }
                    dimensionImages.Clear();
                    dimensionImages = null;
                }
                // dispose ????? ?????
            }
            base.Dispose(disposing);
        }

        //private void userControlRegular_ParentChanged(object sender, EventArgs e)
        //{
        //    RequestReleaseMutex?.Invoke($"Global\\ReportLock_{propQA.Report_No}_{propQA.process}");
        //}

        // ???????????????? Mutex
        //private void ReleaseReportMutex(string mutexKey)
        //{
        //    if (!string.IsNullOrEmpty(currentMutexKey) && mainForm != null)
        //    {
        //        mainForm.ReleaseReportMutex(currentMutexKey);
        //        currentMutexKey = null; // ?????? mutexKey
        //    }
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        if (components != null)
        //        {
        //            components.Dispose();
        //        }
        //        string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
        //        RequestReleaseMutex?.Invoke(mutexKey);
        //    }
        //    base.Dispose(disposing);
        //}

        //private void InitializeComponent()
        //{
        //    this.SuspendLayout();
        //    // 
        //    // userControlDimension
        //    // 
        //    this.BackColor = System.Drawing.Color.Aquamarine;
        //    this.Name = "userControlDimension";
        //    this.ResumeLayout(false);

        //}


        // next function


    }
}
