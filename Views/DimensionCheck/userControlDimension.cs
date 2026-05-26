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
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();

        private BindingSource bindingSource = new BindingSource();
        private DataTable originalDataTable;
        private int currentPage = 1;
        private int totalPages = 1;
        private frmMain mainForm;
        private string currentMutexKey; // ตัวแปรเก็บ mutexKey
                                        // Add this event delegate at the top of your userControlRegular class:
        private IParent parent;
        public delegate void UserControlDisposedEventHandler(object sender, string reportNo);
        public event UserControlDisposedEventHandler UserControlDisposed;

        private System.Windows.Forms.Timer checkTimer;

        private List<Image> dimensionImages;
        private int currentDimensionImageIndex = 0;
        private Image _defaultImage = null; // ถ้าไม่ต้องการ placeholder จริง

        // Dictionary เพื่อเก็บ VALUE ของแต่ละ POINT_ORDER และ SAMPLING_NO
        private Dictionary<string, Dictionary<string, decimal>> pointValues = new Dictionary<string, Dictionary<string, decimal>>();

        public userControlDimension(IParent parent)
        {
            InitializeComponent();
            this.parent = parent;

            dtg_dimension.TabStop = false;
            // ตั้งค่าให้ UserControl ไม่โฟกัสอัตโนมัติ
            this.SetStyle(ControlStyles.Selectable, false);

            // ปิดการรับโฟกัส
            this.TabStop = false;

        }

        private void tb_record_Click(object sender, EventArgs e)
        {

            // บันทึกค่าที่กำลังแก้ไขใน DataGridView
            if (dtg_dimension.IsCurrentCellDirty || dtg_dimension.IsCurrentRowDirty)
            {
                dtg_dimension.EndEdit(); // จบการแก้ไขเซลล์ปัจจุบัน
                dtg_dimension.CommitEdit(DataGridViewDataErrorContexts.Commit); // บันทึกค่าลง DataSource
                bindingSource.EndEdit(); // บันทึกค่าลงใน BindingSource (ถ้าใช้)
            }

            if (dtg_dimension.Rows.Count == 0)
            {
                MessageBox.Show("ยังไม่พบ data ที่จะทำการ Dimension");
                return;
            }

            if (!IsDataTableValid(originalDataTable)) // ตรวจสอบจาก DataTable แทน
            {
                return; // ไม่ทำต่อถ้ามีเซลล์ว่าง
            }

            propQA.TOTAL_STATUS = "1";
            propQA.EMP_ID = employee.EMP_CODE;

            // ✅ วนลูปผ่าน originalDataTable เพื่อให้แน่ใจว่าใช้ข้อมูลจากทุกหน้า
            foreach (DataRow row in originalDataTable.Rows)
            {
                propQA.EQUIPMENT_SERIAL = row["EQUIPMENT_SERIAL"]?.ToString();
                propQA.EQUIPMENT_TYPE_ID = row["EQUIPMENT_TYPE"]?.ToString();

                if (!string.IsNullOrEmpty(propQA.EQUIPMENT_SERIAL) && !string.IsNullOrEmpty(propQA.EQUIPMENT_TYPE_ID))
                {
                    int id = conQA.InsertEquipmentSerial(propQA);
                    row["EQUIPMENT_SERIAL"] = id; // ✅ อัปเดตค่า ID กลับไปที่ DataTable
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
                        status = parsed ? (ProcStatus)statusId : ProcStatus.NG; // ค่าเริ่มต้นเป็น NG ถ้าแปลงไม่ได้

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
                            default:
                                CustomMsgBoxBase.ShowCustomMessageBox(
                                    "สถานะไม่รู้จัก",
                                    "ข้อผิดพลาด",
                                    CustomMsgBoxBase.MessageBoxIconType.Pending);
                                break;
                        }

                        loadstatus();
                        bt_dim_Click();

                    }
                    else
                    {
                        CustomMsgBoxBase.ShowCustomMessageBox("ไม่สามารถ record data ลง database ได้", "ข้อผิดพลาด", CustomMsgBoxBase.MessageBoxIconType.NG);

                    }
                }
                else
                {
                    CustomMsgBoxBase.ShowCustomMessageBox("ไม่สามารถ record data ลง database ได้", "ข้อผิดพลาด", CustomMsgBoxBase.MessageBoxIconType.NG);
                }
            }
            finally
            {

                loadstatus();

                propQA.reportStatus = conQA.ReportFDA_Status(propQA);
                if (!conQA.UpdateReportStatus(propQA))
                {
                    MessageBox.Show("ไม่สามารถ update report status ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                if (!conQA.DeleteReportActive(propQA))
                {
                    MessageBox.Show("ไม่สามารถคืนสถานะ report no ด้วย ip เครื่องนี้ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                // หยุดและกำจัด Timer
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

            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_lotSize.Text = "Lot Size :" + propQA.Qty;
            lb_lotNo.Text = "Lot No. : ";

            cb_lotNo.Items.Clear();

            // ตรวจสอบว่า propQA.dtLotNo ไม่ใช่ null และมีแถวข้อมูล
            if (propQA.dtLotNo != null && propQA.dtLotNo.Rows.Count > 0)
            {
                // วนลูปผ่านแถวใน DataTable เพื่อดึงค่า LOT_NO
                foreach (DataRow row in propQA.dtLotNo.Rows)
                {
                    string lotNo = row["LOT_NO"]?.ToString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(lotNo))
                    {
                        cb_lotNo.Items.Add(lotNo); // เพิ่ม LOT_NO ลงใน ComboBox
                    }
                }

                // ถ้ามีรายการเดียวใน ComboBox ให้เลือกอัตโนมัติ
                if (cb_lotNo.Items.Count == 1)
                {
                    cb_lotNo.SelectedIndex = 0; // เลือกรายการแรก (และรายการเดียว) อัตโนมัติ
                }
                else
                {
                    cb_lotNo.SelectedIndex = -1; // รีเซ็ตถ้ามีมากกว่า 1 รายการ
                }
            }
            else
            {
                cb_lotNo.SelectedIndex = -1; // รีเซ็ตถ้าไม่มีข้อมูล
            }

            lb_sampName.Text = propQA.SAMPLING_QTY + " " + propQA.SAMPLING_NAME;

            // โหลดรูป Function แบบ async (สำหรับ pagination ด้วย list ถ้ามีหลายรูป)
            dimensionImages = await imgCls.LoadImagesAsync("DimensionPath", propQA.M_CODE);
            currentDimensionImageIndex = 0;

            if (dimensionImages != null && dimensionImages.Count > 0)
            {
                picbox_dim.Image = dimensionImages[0];
            }
            else
            {
                // Fallback: LoadImages จัดการ single แล้ว ถ้าไม่มีจะ return empty list
                picbox_dim.Image = _defaultImage; // หรือ null ถ้าไม่มี default
            }


            if (propQA.SAMPLING_TYPE == "4" || (propQA.SAMPLING_TYPE == "3" && Convert.ToInt32(propQA.CAVITY_QTY) != 0))
            {
                lb_TotalCavity.Visible = true;
                lb_TotalCavity.Text = "Total Cavity : " + propQA.SAMPLING_QTY;

                picbox_cavity.Image = imgCls.LoadSingleImage("CavityPath", propQA.M_CODE);
                //picbox_dim.Image = imgCls.LoadDimensionImage(propQA.M_CODE);


                dtg_cavity.DataSource = propQA.dtCavity;


                // ตรวจสอบว่ามีคอลัมน์ "DATA_NO" หรือยัง
                if (dtg_cavity.Columns["CAVITY_NAME"] != null)
                {
                    dtg_cavity.Columns["CAVITY_NAME"].HeaderText = "ชื่อคาวิตี้";
                    dtg_cavity.Columns["CAVITY_NAME"].ReadOnly = true;
                }



                if (dtg_cavity.Columns["SAMPLING_QTY"] != null)
                {
                    dtg_cavity.Columns["SAMPLING_QTY"].HeaderText = "จำนวน";
                }


            }
            else
            {
                gb_cavity.Visible = false;
                lb_TotalCavity.Visible = false;
                
                picbox_dim.Location = new System.Drawing.Point(17, 113);
                picbox_dim.Size = new Size(1076, 556);

                GenerateDataTableDimension(null, Convert.ToInt32(propQA.SAMPLING_QTY));

            }

            // เริ่มต้นและตั้งค่า Timer
            checkTimer = new System.Windows.Forms.Timer();
            checkTimer.Interval = 60000; // 3 นาที (180,000 มิลลิวินาที)
            checkTimer.Tick += CheckTimer_Tick;
            checkTimer.Start();

            // หลังจากโหลดข้อมูลเสร็จ
            this.AutoScroll = true;

            // รีเซ็ตตำแหน่ง Scrollbar ไปด้านบน
            this.ScrollControlIntoView(lb_top);

            this.Focus();

            this.AutoScrollPosition = new System.Drawing.Point(0, 0);
            this.VerticalScroll.Value = 0;

        }

        // Event Handler สำหรับ Timer
        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (conQA.CheckReportStatus(propQA) == false)
            {
                CustomMsgBoxBase.ShowCustomMessageBox($"พบงานที่ติด Pending จาก process อื่น", "แจ้งเตือน", CustomMsgBoxBase.MessageBoxIconType.NG);
                bt_dim_Click();
                checkTimer.Stop();
            }
        }

        private void dtg_dimension_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // เช็คว่ากำลังแก้ไขคอลัมน์ "Value"
            if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE")
            {

                if (dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
                {
                    return;
                }

                string input = e.FormattedValue.ToString();

                // ถ้าเว้นว่างไว้ ให้เตือนและไม่ให้ผ่าน
                if (string.IsNullOrWhiteSpace(input))
                {
                    return;
                }

                // ตรวจสอบว่าเป็นตัวเลข และต้องไม่มีจุดเกิน 1 จุด
                if (!IsValidDecimal(input))
                {
                    MessageBox.Show("กรุณากรอกตัวเลขเท่านั้น และไม่สามารถมีจุดทศนิยมมากกว่า 1 จุดได้", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // ยกเลิกการเปลี่ยนแปลงค่า
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
                // ดึง DataTable จาก DataGridView
                BindingSource bs = dtg_dimension.DataSource as BindingSource;
                DataTable dtData = bs != null ? (DataTable)bs.DataSource : dtg_dimension.DataSource as DataTable;
                if (dtData == null) return;

                // ดึงค่า EQUIPMENT_SERIAL และ EQUIPMENT_TYPE จากแถวที่แก้ไข
                string newSerial = dtg_dimension.Rows[e.RowIndex].Cells["EQUIPMENT_SERIAL"].Value?.ToString();
                string eqType = dtg_dimension.Rows[e.RowIndex].Cells["EQUIPMENT_TYPE"].Value?.ToString();

                // ตรวจสอบว่ามีค่าใหม่หรือไม่
                if (!string.IsNullOrEmpty(newSerial) && !string.IsNullOrEmpty(eqType))
                {
                    // อัปเดตทุกแถวที่มี EQUIPMENT_TYPE เดียวกัน
                    foreach (DataRow row in dtData.Rows)
                    {
                        if (row["EQUIPMENT_TYPE"].ToString() == eqType)
                        {
                            row["EQUIPMENT_SERIAL"] = newSerial;
                        }
                    }

                    // รีเฟรช DataGridView เพื่อให้ข้อมูลอัปเดต
                    bs?.ResetBindings(false);
                    dtg_dimension.Refresh();
                }
            }

            // เรียกใช้การคำนวณ VALUE เมื่อมีการกรอก VALUE
            if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE")
            {
                CalculatePointValues();
            }
        }

        //private void CalculatePointValues()
        //{
        //    // ล้างค่าเก่าใน Dictionary
        //    pointValues.Clear();

        //    // เก็บ VALUE ของแต่ละ POINT_ORDER ที่กรอกแล้วจาก originalDataTable
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

        //    // คำนวณ VALUE สำหรับ POINT_ORDER ที่มี POINT_CAL ไม่ใช่ "0"
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

        //    // อัปเดต UI
        //    dtg_dimension.Refresh();
        //}

        //private void CalculatePointValues()
        //{
        //    // ล้างค่าเก่าใน Dictionary
        //    pointValues.Clear();

        //    // เก็บ VALUE ของแต่ละ POINT_ORDER ที่ EQUIPMENT_TYPE = 0 (กรอกได้) จากข้อมูลที่แสดง
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

        //    // คำนวณ VALUE สำหรับ POINT_ORDER ปัจจุบันถ้า EQUIPMENT_TYPE != "0"
        //    string currentEquipmentType = dtg_dimension.Rows[0].Cells["EQUIPMENT_TYPE"].Value?.ToString();
        //    if (!string.IsNullOrEmpty(currentEquipmentType) && currentEquipmentType != "0")
        //    {
        //        // สมมติว่า EQUIPMENT_TYPE เก็บข้อมูลที่บ่งบอก POINT_ORDER ที่จะบวก (เช่น "1+2")
        //        string[] orders = currentEquipmentType.Split('+'); // ใช้ EQUIPMENT_TYPE เป็นตัวบ่งชี้การบวก
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
        //                row.Cells["VALUE"].Value = sum.ToString(); // ตั้งค่า VALUE เดียวกันสำหรับทุกแถวในหน้า
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

        //    // อัปเดต UI
        //    dtg_dimension.Refresh();
        //}

        private void CalculatePointValues()
        {
            if (isUpdating) return; // ป้องกันการเรียกซ้ำ

            // ล้างค่าเก่าใน Dictionary
            pointValues.Clear();

            // เก็บ VALUE ของทุก POINT_ORDER และ SAMPLING_NO ที่มีค่าไม่ว่างจาก originalDataTable
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

            // คำนวณ VALUE สำหรับทุกแถวใน originalDataTable
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
                    // คำนวณเมื่อ EQUIPMENT_TYPE เป็น 0 และ POINT_CAL มีการบวก
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
                                break; // ออกจากลูปทันทีเมื่อพบข้อมูลขาดหาย
                            }
                        }

                        if (canCalculate)
                        {
                            row["VALUE"] = sum.ToString();
                            Console.WriteLine($"Setting VALUE to {sum} for POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}");
                            // ตรวจสอบกับ CRITERIA_MIN และ CRITERIA_MAX
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
                        // ถ้า POINT_CAL เป็น "0" หรือไม่มีค่า ใช้ VALUE เดิมที่กรอก
                        Console.WriteLine($"No calculation needed or invalid POINT_CAL for POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}");
                    }
                }
            }
            finally
            {
                isUpdating = false;
                bindingSource.ResetBindings(false); // รีเฟรช UI ด้วยข้อมูลที่อัปเดต
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
            // ถ้าทุกแถวเป็น 1 ให้ Total_Judge เป็น 1
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
            dtg_cavity.EndEdit();

            int totalQty = 0;

            // ตรวจสอบว่าจำนวนเป็นเลขตั้งแต่ 0 ขึ้นไปทุกแถว
            foreach (DataGridViewRow row in dtg_cavity.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                string samplingQty = row.Cells["SAMPLING_QTY"].Value?.ToString();

                if (!int.TryParse(samplingQty, out int qty) || qty < 0)
                {
                    MessageBox.Show("กรุณากรอกจำนวน Cavity เป็นตัวเลขตั้งแต่ 0 ขึ้นไปทุกแถว!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                totalQty += qty;
            }

            if (totalQty != Convert.ToInt32(propQA.SAMPLING_QTY))
            {
                MessageBox.Show($"ผลรวมของ QTY ต้องได้ {Convert.ToInt32(propQA.SAMPLING_QTY)}  (ปัจจุบัน: {totalQty})", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ล็อก dtg_data ไม่ให้แก้ไขข้อมูล
            dtg_cavity.ReadOnly = true;


            GenerateDataTableDimension(dtg_cavity, 0);

        }

        //// ฟังก์ชันแสดงเฉพาะแถวที่เป็น POINT_ORDER ของหน้าปัจจุบัน
        //private void ShowPage(int page)
        //{
        //    bindingSource.Filter = $"POINT_ORDER = '{page}'"; // กรองเฉพาะแถวที่มี POINT_ORDER ตรงกับหน้า
        //    CalculatePointValues(); // คำนวณใหม่ทุกครั้งที่เปลี่ยนหน้า
        //    UpdateReadOnlyCells();
        //    lb_page.Text = $"{page}/{totalPages}"; // แสดงหน้า (1/8)
        //}

        private void ShowPage(int pageNumber)
        {
            currentPage = pageNumber;
            bindingSource.Filter = $"POINT_ORDER = '{pageNumber}'"; // กรองเฉพาะหน้า
            dtg_dimension.DataSource = bindingSource; // อัปเดต DataGridView
            dtg_dimension.Refresh(); // รีเฟรชเพื่อให้แน่ใจว่าแสดงข้อมูลล่าสุด
            CalculatePointValues(); // คำนวณใหม่ทุกครั้งที่เปลี่ยนหน้า
            UpdateReadOnlyCells();
            lb_page.Text = $"{pageNumber}/{totalPages}";
            Console.WriteLine($"Switched to page {pageNumber}, filter applied: {bindingSource.Filter}");
        }

        private void UpdateGrid()
        {
            if (originalDataTable == null) return;

            // กรองข้อมูลจาก DataTable เดิมตาม POINT_ORDER ปัจจุบัน
            var filteredData = originalDataTable.AsEnumerable()
                .Where(row => Convert.ToInt32(row["POINT_ORDER"]) == currentPage);

            if (filteredData.Any())
            {
                bindingSource.DataSource = filteredData.CopyToDataTable();
            }
            else
            {
                bindingSource.DataSource = new DataTable(); // ถ้าไม่มีข้อมูลให้ตั้งเป็น DataTable เปล่า
            }

            dtg_dimension.DataSource = bindingSource;

            // อัปเดต Label แสดงสถานะ
            lb_page.Text = $"Page {currentPage} / {totalPages}";

            // ปิดการใช้งานปุ่ม Prev / Next ถ้าถึงขอบ
            bt_prev.Enabled = currentPage > 1;
            bt_next.Enabled = currentPage < totalPages;
        }

        private void dtg_cavity_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            if (e.ColumnIndex == dtg_cavity.Columns["SAMPLING_QTY"].Index)
            {
                string value = e.FormattedValue?.ToString();
                // ตรวจสอบว่าเป็นตัวเลขตั้งแต่ 0 ขึ้นไป และห้ามเว้นว่าง
                if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out int qty) || qty < 0)
                {
                    e.Cancel = true; // ยกเลิกการออกจากเซลล์โดยไม่แสดงข้อความ
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
                dtAllSum.Columns.Add("CAVITY_NAME", typeof(string)); // ใช้เฉพาะ Code B
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

            // ซ่อนคอลัมน์ที่ไม่ต้องการแสดง
            string[] hiddenColumns = { "POINT_CAL", "POINT_ORDER", "EQUIPMENT_TYPE", "POINT_JUDGE", "TOTAL_JUDGE" };
            foreach (var col in hiddenColumns)
            {
                if (dtg_dimension.Columns.Contains(col))
                {
                    dtg_dimension.Columns[col].Visible = false;
                }
            }


            // ทำให้คอลัมน์ที่ไม่ใช่ "VALUE" และ "EQUIPMENT_SERIAL" เป็น ReadOnly
            foreach (DataGridViewColumn column in dtg_dimension.Columns)
            {
                column.ReadOnly = (column.Name != "VALUE" && column.Name != "EQUIPMENT_SERIAL");
            }

            //// ตั้งค่า ReadOnly สำหรับเซลล์ VALUE ที่มี POINT_CAL ไม่ใช่ "0"
            //foreach (DataGridViewRow row in dtg_dimension.Rows)
            //{
            //    string pointCal = row.Cells["POINT_CAL"].Value?.ToString();
            //    if (!string.IsNullOrEmpty(pointCal) && pointCal != "0")
            //    {
            //        row.Cells["VALUE"].ReadOnly = true;
            //    }
            //}

            // ตั้งค่า ReadOnly สำหรับเซลล์ทันทีหลังโหลดข้อมูล
            UpdateReadOnlyCells();


            // บันทึกข้อมูลต้นฉบับ
            //originalDataTable = (DataTable)dtg_dimension.DataSource;
            //bindingSource.DataSource = originalDataTable;
            //dtg_dimension.DataSource = bindingSource;

            originalDataTable = dtAllSum.Copy(); // ใช้ Copy เพื่อให้แน่ใจว่าเป็นข้อมูลดิบ
            bindingSource.DataSource = originalDataTable;
            dtg_dimension.DataSource = bindingSource;

            // คำนวณจำนวน POINT_ORDER ที่มีทั้งหมด
            totalPages = originalDataTable.AsEnumerable()
                .Select(row => row["POINT_ORDER"].ToString())
                .Distinct()
                .Count();

            ShowPage(currentPage);

            // เปลี่ยน HeaderText
            if (dtg_dimension.Columns.Contains("CAVITY_NAME")) dtg_dimension.Columns["CAVITY_NAME"].HeaderText = "CAV.";
            if (dtg_dimension.Columns.Contains("SAMPLING_NO")) dtg_dimension.Columns["SAMPLING_NO"].HeaderText = "SAMPLE";
            if (dtg_dimension.Columns.Contains("POINT_NAME")) dtg_dimension.Columns["POINT_NAME"].HeaderText = "CHECKPOINT";
            if (dtg_dimension.Columns.Contains("EQUIPMENT_SERIAL")) dtg_dimension.Columns["EQUIPMENT_SERIAL"].HeaderText = "EQ_SN";
            if (dtg_dimension.Columns.Contains("EQUIPMENT_NAME")) dtg_dimension.Columns["EQUIPMENT_NAME"].HeaderText = "EQ_NAME ";
            if (dtg_dimension.Columns.Contains("CRITERIA_MIN")) dtg_dimension.Columns["CRITERIA_MIN"].HeaderText = "MIN";
            if (dtg_dimension.Columns.Contains("CRITERIA_MAX")) dtg_dimension.Columns["CRITERIA_MAX"].HeaderText = "MAX";

            // ตรวจสอบว่า VALUE ควรเป็น ComboBox หรือไม่
            //dtg_regular.CellFormatting += (sender, e) =>
            //{
            //    if (dtg_regular.Columns[e.ColumnIndex].Name == "VALUE")
            //    {
            //        double minValue = Convert.ToDouble(dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value);
            //        double maxValue = Convert.ToDouble(dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MAX"].Value);

            //        if (minValue == 1 && maxValue == 1)
            //        {
            //            // ใช้ ComboBoxColumn
            //            DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
            //            comboBoxCell.DataSource = new List<KeyValuePair<string, string>>()
            //            {
            //                new KeyValuePair<string, string>("", ""), // Null ค่าเป็นช่องว่าง
            //                new KeyValuePair<string, string>("0", "NG"),
            //                new KeyValuePair<string, string>("1", "OK")
            //            };
            //            comboBoxCell.ValueMember = "Key";
            //            comboBoxCell.DisplayMember = "Value";

            //            dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] = comboBoxCell;
            //        }
            //        else
            //        {
            //            // ใช้ TextBoxColumn
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
                // ดึงหมายเลขหน้า (POINT_ORDER)
                int pageNumber = row["POINT_ORDER"] != DBNull.Value ? Convert.ToInt32(row["POINT_ORDER"]) : 0;

                // ดึงค่า Sampling No (อ้างอิงแทน Row Index)
                string samplingNo = row["SAMPLING_NO"] != DBNull.Value ? row["SAMPLING_NO"].ToString() : "N/A";

                foreach (DataColumn column in table.Columns)
                {
                    if (row[column] == DBNull.Value || string.IsNullOrWhiteSpace(row[column].ToString()))
                    {
                        string columnName = column.ColumnName; // ชื่อคอลัมน์

                        if (columnName == "EQUIPMENT_SERIAL")
                        {
                            CustomMsgBoxBase.ShowCustomMessageBox($"พบเซลล์ว่างในหน้า {pageNumber}, Sample {samplingNo}, คอลัมน์ EQ_SN",
                                "คำเตือน", CustomMsgBoxBase.MessageBoxIconType.Warning);
                        }
                        else if (columnName == "VALUE")
                        {
                            string pointCal = row["POINT_CAL"]?.ToString();
                            if (string.IsNullOrEmpty(pointCal) || pointCal == "0")
                            {
                                CustomMsgBoxBase.ShowCustomMessageBox($"พบเซลล์ว่างในหน้า {pageNumber}, Sample {samplingNo}, คอลัมน์ {columnName}",
                                   "คำเตือน", CustomMsgBoxBase.MessageBoxIconType.Warning);
                                return false;
                            }
                        }
                        else
                        {

                            CustomMsgBoxBase.ShowCustomMessageBox($"พบเซลล์ว่างในหน้า {pageNumber}, Sample {samplingNo}, คอลัมน์ {columnName}",
                                "คำเตือน", CustomMsgBoxBase.MessageBoxIconType.Warning);
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
                // ข้ามแถวใหม่ที่ยังไม่เพิ่มข้อมูล (AllowUserToAddRows = true)
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    // ตรวจสอบค่าในเซลล์
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        CustomMsgBoxBase.ShowCustomMessageBox($"พบเซลล์ว่างในแถวที่ {row.Index + 1} คอลัมน์ {dtg.Columns[cell.ColumnIndex].HeaderText}", "คำเตือน", CustomMsgBoxBase.MessageBoxIconType.Warning);
                        dtg.CurrentCell = cell; // ตั้งค่าให้เซลล์ที่ว่างเป็น Active
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
                    // ลบ Event เดิม (ถ้ามี) เพื่อป้องกันการซ้ำซ้อน
                    textBox.KeyPress -= TextBox_KeyPress;

                    // เพิ่ม Event ใหม่
                    textBox.KeyPress += TextBox_KeyPress;
                }


            }

        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (dtg_cavity.CurrentCell.ColumnIndex == dtg_cavity.Columns["SAMPLING_QTY"].Index)
            {
                // อนุญาตเฉพาะตัวเลขและปุ่มควบคุม (เช่น Backspace, Delete)
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true; // ยกเลิกอักขระที่ไม่ใช่ตัวเลข
                }
            }
        }

        private void bt_back_Click(object sender, EventArgs e)
        {
            propQA.inProcStatus = ((int)ProcStatus.Unfinished).ToString();
            propQA.reportStatus = ((int)ProcStatus.Unfinished).ToString();

            if (!conQA.UpdateStatus(propQA))
            {
                MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Unfinished ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!conQA.DeleteReportActive(propQA))
            {
                MessageBox.Show("ไม่สามารถเพิ่ม report no กับ IP ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            bt_dim_Click();
        }

        public void bt_dim_Click()
        {

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

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
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
                    // เคลียร์และเพิ่ม UserControl ใหม่

                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrConSelectDim);
                    usrConSelectDim.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            //else
            //{
            //    Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
            //    //Control[] foundPanels = this.Controls.Find("panelMain", true);

            //    if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
            //    {
            //        // เคลียร์และเพิ่ม UserControl ใหม่

            //        panelMain.Controls.Clear();
            //        panelMain.Controls.Add(usrConSelectReg);
            //        usrConSelectReg.BringToFront();
            //    }
            //    else
            //    {
            //        MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    }
            //}



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
                    row.Cells["VALUE"].ReadOnly = false; // เพื่อให้แน่ใจว่าเซลล์อื่นยังแก้ไขได้
                }
            }
        }

     

        private decimal CalculateSumForPoint(DataRow row)
        {
            string pointCal = row["POINT_CAL"]?.ToString();
            if (string.IsNullOrEmpty(pointCal) || pointCal == "0")
            {
                return 0; // ไม่ต้องคำนวณถ้า POINT_CAL เป็น 0 หรือว่าง
            }

            decimal sum = 0;
            string[] orders = pointCal.Split('+'); // แยก POINT_ORDER ที่จะบวกกัน (เช่น "1+2+3")
            bool canCalculate = true;

            foreach (string order in orders)
            {
                string trimmedOrder = order.Trim();
                // ค้นหาแถวใน originalDataTable ที่ตรงกับ POINT_ORDER
                var relatedRows = originalDataTable.AsEnumerable()
                    .Where(r => r["POINT_ORDER"].ToString() == trimmedOrder && r["VALUE"] != DBNull.Value);

                if (relatedRows.Any())
                {
                    decimal value = relatedRows.Select(r => Convert.ToDecimal(r["VALUE"])).FirstOrDefault();
                    sum += value;
                }
                else
                {
                    canCalculate = false; // ถ้าไม่มีข้อมูลให้คำนวณได้
                    break;
                }
            }

            return canCalculate ? sum : 0; // คืนค่า 0 ถ้าคำนวณไม่ได้
        }

        //private void dtg_regular_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{
        //    DataGridViewTextBoxEditingControl textBox = e.Control as DataGridViewTextBoxEditingControl;
        //    if (textBox != null)
        //    {
        //        // ตรวจสอบคอลัมน์ที่ต้องการเปลี่ยนเป็น ComboBox
        //        int columnIndex = dtg_regular.CurrentCell.ColumnIndex;
        //        if (dtg_regular.Columns[columnIndex].Name == "VALUE")
        //        {
        //            // เปลี่ยนเป็น ComboBox
        //            DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
        //            comboBoxCell.DataSource = new List<KeyValuePair<string, string>>()
        //    {
        //        new KeyValuePair<string, string>("", null),
        //        new KeyValuePair<string, string>("NG", "0"),
        //        new KeyValuePair<string, string>("OK", "1")
        //    };
        //            comboBoxCell.DisplayMember = "Key";
        //            comboBoxCell.ValueMember = "Value";

        //            // แทนที่เซลล์ แต่ต้องใช้ BeginInvoke เพื่อป้องกัน StackOverflow
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
                // ตรวจสอบว่าข้อมูลใน CRITERIA_MIN และ CRITERIA_MAX มีค่า
                if (dtg_dimension.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value != null &&
                    dtg_dimension.Rows[e.RowIndex].Cells["CRITERIA_MAX"].Value != null)
                {
                    double minValue = Convert.ToDouble(dtg_dimension.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value);
                    double maxValue = Convert.ToDouble(dtg_dimension.Rows[e.RowIndex].Cells["CRITERIA_MAX"].Value);

                    // เงื่อนไข: ถ้า CRITERIA_MIN == 1 && CRITERIA_MAX == 1 ให้ใช้ ComboBoxCell
                    if (minValue == 1 && maxValue == 1)
                    {
                        // ตรวจสอบว่าเซลล์ VALUE ยังไม่ใช่ ComboBoxCell
                        if (!(dtg_dimension.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell))
                        {
                            DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
                            comboBoxCell.DataSource = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("", ""),  // ช่องว่าง
                        new KeyValuePair<string, string>("0", "NG"),
                        new KeyValuePair<string, string>("1", "OK")
                    };
                            comboBoxCell.ValueMember = "Key";
                            comboBoxCell.DisplayMember = "Value";

                            // ใช้ BeginInvoke เพื่อหลีกเลี่ยงการเรียก CellFormatting ซ้ำ
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                // ตรวจสอบว่า RowIndex และ ColumnIndex ไม่เกินขอบเขตของ DataGridView
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
                        // ถ้าไม่ตรงเงื่อนไข ให้ใช้ TextBoxCell
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

            UpdateReadOnlyCells(); // เรียกอัปเดต ReadOnly หลังการผูกข้อมูล

            foreach (DataGridViewRow row in dtg_dimension.Rows)
            {
                // ตรวจสอบว่ามีค่าใน VALUE และ POINT_JUDGE หรือไม่
                if (row.Cells["VALUE"].Value != null &&
                    !string.IsNullOrWhiteSpace(row.Cells["VALUE"].Value.ToString()) &&
                    row.Cells["POINT_JUDGE"].Value != null &&
                    row.Cells["POINT_JUDGE"].Value.ToString() == "0")
                {
                    row.DefaultCellStyle.BackColor = Color.Red; // เปลี่ยนสีเป็นแดงถ้า POINT_JUDGE = "0"
                }
                else
                {
                    row.DefaultCellStyle.BackColor = Color.White; // คืนสีพื้นหลังเป็นสีขาว (หรือสีปกติ)
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
        //    //    // รีเซ็ตสถานะเมื่อออกจากเซลล์

        //    //}
        //}
        // next function
        //private void TextBox_TextChanged(object sender, EventArgs e)
        //{

        //    TextBox textBox = sender as TextBox;




        //    //_isKeyboardInputDetected = true;
        //    //MessageBox.Show("ไม่ควรพิมพ์ข้อมูลดังกล่าวด้วย keyboard", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

        //        // คำนวณ POINT_JUDGE เมื่อค่าเปลี่ยนแปลง
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

        //        // อัปเดต originalDataTable ด้วยค่าใหม่จาก dtg_dimension โดยใช้ POINT_ORDER เป็นคีย์
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

        //    // เรียกใช้การคำนวณ VALUE เมื่อ VALUE เปลี่ยนแปลง
        //    CalculatePointValues();
        //}

        private bool isUpdating = false; // ตัวแปรควบคุมเพื่อป้องกันการเรียกซ้ำ

        private void dtg_dimension_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE" && !isUpdating)
            {
                // บล็อกการเรียกซ้ำ
                isUpdating = true;
                try
                {
                    DataGridViewRow row = dtg_dimension.Rows[e.RowIndex];
                    string pointOrder = row.Cells["POINT_ORDER"].Value?.ToString() ?? "";
                    string samplingNo = row.Cells["SAMPLING_NO"].Value?.ToString() ?? "";

                    // ตรวจสอบค่า VALUE ปัจจุบัน
                    string valueStr = row.Cells["VALUE"].Value?.ToString() ?? "";
                    if (string.IsNullOrWhiteSpace(valueStr))
                    {
                        // ถ้าเป็นว่าง ตั้งค่าใน originalDataTable เป็น DBNull และข้ามการคำนวณ
                        DataRow[] matchingRows = originalDataTable.Select($"POINT_ORDER = '{pointOrder}' AND SAMPLING_NO = {samplingNo}");
                        if (matchingRows.Length > 0)
                        {
                            matchingRows[0]["VALUE"] = DBNull.Value;
                            Console.WriteLine($"Updated originalDataTable: POINT_ORDER={pointOrder}, SAMPLING_NO={samplingNo}, VALUE=null");
                        }
                        return; // ข้ามการคำนวณต่อไป
                    }

                    // คำนวณ POINT_JUDGE เมื่อค่าเปลี่ยนแปลง
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

                    // อัปเดต originalDataTable ด้วยค่าใหม่
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

                    // เรียกคำนวณใหม่
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
        //        // บังคับซิงโครไนซ์ข้อมูลทันที
        //        dtg_dimension.EndEdit();
        //        bindingSource.EndEdit();

        //        DataGridViewRow row = dtg_dimension.Rows[e.RowIndex];
        //        string pointOrder = row.Cells["POINT_ORDER"].Value?.ToString();
        //        string samplingNo = row.Cells["SAMPLING_NO"].Value?.ToString();

        //        // คำนวณ POINT_JUDGE เมื่อค่าเปลี่ยนแปลง
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

        //        // อัปเดตผ่าน bindingSource เพื่อป้องกันการเรียกซ้ำ
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

        //        // เรียกคำนวณใหม่ทันทีหลังอัปเดต
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

                    // ลบส่วน dispose ออก เพื่อป้องกันการ dispose Image ใน list
                    // if (picbox_func.Image != null)
                    // {
                    //     picbox_func.Image.Dispose();
                    //     picbox_func.Image = null;
                    // }
                    picbox_dim.Image = dimensionImages[currentDimensionImageIndex];

                    return true; // บอกว่าจัดการ key แล้ว ไม่ให้ไปต่อ
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
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
                // dispose อื่นๆ ถ้ามี
            }
            base.Dispose(disposing);
        }

        //private void userControlRegular_ParentChanged(object sender, EventArgs e)
        //{
        //    RequestReleaseMutex?.Invoke($"Global\\ReportLock_{propQA.Report_No}_{propQA.process}");
        //}

        // เมธอดสำหรับปล่อย Mutex
        //private void ReleaseReportMutex(string mutexKey)
        //{
        //    if (!string.IsNullOrEmpty(currentMutexKey) && mainForm != null)
        //    {
        //        mainForm.ReleaseReportMutex(currentMutexKey);
        //        currentMutexKey = null; // รีเซ็ต mutexKey
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
