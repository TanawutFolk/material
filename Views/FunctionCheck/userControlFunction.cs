using MySqlX.XDevAPI.Common;
using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.CustomMsg;
using RawMat.Views.RegularCheck;
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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RawMat.Views.FunctionCheck
{
    public partial class userControlFunction : UserControl
    {
        public event Action<UserControl> AddUserControlRequested;
        public event Action OnReleaseMutex;
        public event EventHandler BackToARequested;
        public event Action<string> RequestReleaseMutex;
        public event Action OnClose;
        //public event EventHandler SaveRequested;
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

        private List<Image> functionImages;
        private int currentFunctionImageIndex = 0;
        private Image _defaultImage = null; // ถ้าไม่ต้องการ placeholder จริง

        public userControlFunction()
        {

            InitializeComponent();
            dtg_cavity.CellValidating += dtg_cavity_CellValidating;
            dtg_cavity.EditingControlShowing += dtg_cavity_EditingControlShowing;
            dtg_function.DataError += dtg_function_DataError;

        }

        private void dtg_function_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
        }

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

        //        // หยุดและกำจัด Timer
        //        if (checkTimer != null)
        //        {
        //            checkTimer.Stop();
        //            checkTimer.Dispose();
        //        }

        //        string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
        //        RequestReleaseMutex?.Invoke(mutexKey);
        //    }
        //    base.Dispose(disposing);
        //}

        private async void userControlFunction_Load(object sender, EventArgs e)
        {
            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_lotSize.Text = "Lot Size :" + propQA.Qty;

            lb_sampName.Text = propQA.SAMPLING_NAME == "Fix"
                ? $"Quantity {propQA.SAMPLING_QTY} Pcs."
                : $"{propQA.SAMPLING_QTY} {propQA.SAMPLING_NAME}";

            // โหลดรูป Function แบบ async (สำหรับ pagination ด้วย list ถ้ามีหลายรูป)
            functionImages = await imgCls.LoadImagesAsync("FunctionPath", propQA.M_CODE);
            currentFunctionImageIndex = 0;

            if (functionImages != null && functionImages.Count > 0)
            {
                picbox_func.Image = functionImages[0];
            }
            else
            {
                // Fallback: LoadImages จัดการ single แล้ว ถ้าไม่มีจะ return empty list
                picbox_func.Image = _defaultImage; // หรือ null ถ้าไม่มี default
            }

            // โหลดรูป Material (ใช้รูปแรก ถ้ามีหลายรูปจะ hold list สำหรับอนาคต แต่ตอนนี้ใช้แค่ตัวแรก)
            picbox_mat.Image = imgCls.LoadSingleImage("MaterialPath", propQA.M_CODE);
           

            if (propQA.SAMPLING_TYPE == "4" || (propQA.SAMPLING_TYPE == "3" && Convert.ToInt32(propQA.CAVITY_QTY) != 0))
            {
                lb_TotalCavity.Visible = true;
                lb_TotalCavity.Text = "Total Cavity : " + propQA.SAMPLING_QTY;

                // โหลดรูป Cavity (สมมติ refactor แล้ว ใช้ LoadImages แทน LoadCavityImage)
                picbox_cavity.Image = imgCls.LoadSingleImage("CavityPath", propQA.M_CODE); // สมมติมี key "CavityPath" ใน app.config
                dtg_cavity.DataSource = propQA.dtCavity;

                // ตรวจสอบว่ามีคอลัมน์ "CAVITY_NAME" หรือยัง
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

                GenerateDataTableFunction(null, Convert.ToInt32(propQA.SAMPLING_QTY));
            }

            // เริ่มต้นและตั้งค่า Timer
            checkTimer = new System.Windows.Forms.Timer();
            checkTimer.Interval = 60000; // 1 นาที (60,000 มิลลิวินาที) - ถ้า 3 นาทีให้แก้เป็น 180000
            checkTimer.Tick += CheckTimer_Tick;
            checkTimer.Start();

            // ตั้งค่า focus ให้ UserControl เพื่อให้ ProcessCmdKey ทำงานทันทีหลัง load
            this.Focus();
        }

        // Event Handler สำหรับ Timer
        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (conQA.CheckReportStatus(propQA) == false)
            {
                CustomMsgBoxBase.ShowCustomMessageBox($"พบงานที่ติด Pending จาก process อื่น", "แจ้งเตือน", CustomMsgBoxBase.MessageBoxIconType.NG);
                bt_function_Click();
                checkTimer.Stop();
            }
        }

      
        private void GenerateDataTableFunction(DataGridView dtgCavity, int sampQty)
        {

            dtg_function.CellEndEdit -= dtg_function_CellEndEdit;


            DataTable dtAllSum = new DataTable();

            if (dtgCavity != null)
            {
                dtAllSum.Columns.Add("CAVITY_NAME", typeof(string)); // ใช้เฉพาะ Code B
            }

            dtAllSum.Columns.Add("SAMPLING_NO", typeof(int));
            dtAllSum.Columns.Add("LOT_NO", typeof(string));
            dtAllSum.Columns.Add("JUDGE", typeof(string));
            dtAllSum.Columns.Add("POINT_JUDGE", typeof(string));
            dtAllSum.Columns.Add("TOTAL_JUDGE", typeof(string));
            dtAllSum.Columns.Add("REMARK", typeof(string));

            int qtySampCounter = 1;

            if (dtgCavity == null)
            {
                for (int i = 0; i < sampQty; i++)
                {
                    int qtySampNo = qtySampCounter++;

                    //foreach (DataRow judgeRow in propQA.dtFuncJudge.Rows)
                    //{
                        dtAllSum.Rows.Add(
                            qtySampNo,
                            //measureRow["POINT_ORDER"].ToString(),
                            //measureRow["POINT_CAL"].ToString(),
                            null,
                            //measureRow["EQUIPMENT_TYPE"].ToString(),
                            //measureRow["EQUIPMENT_NAME"].ToString(),
                            //measureRow["POINT_NAME"].ToString(),
                            null,
                            //Convert.ToDouble(measureRow["CRITERIA_MIN"]),
                            //Convert.ToDouble(measureRow["CRITERIA_MAX"]),
                            null, null , null
                        );
                    //}
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

                        //foreach (DataRow judgeRow in propQA.dtFuncJudge.Rows)
                        //{
                            dtAllSum.Rows.Add(
                                name,
                                qtySampNo,
                                //measureRow["POINT_ORDER"].ToString(),
                                //measureRow["POINT_CAL"].ToString(),
                                null,
                                //measureRow["EQUIPMENT_TYPE"].ToString(),
                                //measureRow["EQUIPMENT_NAME"].ToString(),
                                //measureRow["POINT_NAME"].ToString(),
                                null,
                                //Convert.ToDouble(measureRow["CRITERIA_MIN"]),
                                //Convert.ToDouble(measureRow["CRITERIA_MAX"]),
                                null, null , null
                            );
                        //}
                    }
                }
            }

            dtg_function.DataSource = dtAllSum;

            // ซ่อนคอลัมน์ที่ไม่ต้องการแสดง show ก่อน เดี๋ยวค่อยมา ซ่อน 2025-04-01 preecha j.
            //string[] hiddenColumns = { "POINT_JUDGE", "TOTAL_JUDGE" };
            //foreach (var col in hiddenColumns)
            //{
            //    if (dtg_function.Columns.Contains(col))
            //    {
            //        dtg_function.Columns[col].Visible = false;
            //    }
            //}

            // บันทึกข้อมูลต้นฉบับ
            originalDataTable = (DataTable)dtg_function.DataSource;
            bindingSource.DataSource = originalDataTable;
            dtg_function.DataSource = bindingSource;

            // ซ่อนคอลัมน์ที่ไม่ต้องการแสดง
            string[] hiddenColumns = { "POINT_JUDGE", "TOTAL_JUDGE" };
            foreach (var col in hiddenColumns)
            {
                if (dtg_function.Columns.Contains(col))
                {
                    dtg_function.Columns[col].Visible = false;
                }
            }


            // ทำให้คอลัมน์ที่ไม่ใช่ "VALUE" และ "EQUIPMENT_SERIAL" เป็น ReadOnly
            foreach (DataGridViewColumn column in dtg_function.Columns)
            {
                column.ReadOnly = (column.Name != "JUDGE");
            }

            // เปลี่ยน HeaderText
            if (dtg_function.Columns.Contains("CAVITY_NAME")) dtg_function.Columns["CAVITY_NAME"].HeaderText = "CAV.";
            if (dtg_function.Columns.Contains("SAMPLING_NO")) dtg_function.Columns["SAMPLING_NO"].HeaderText = "SAMPLE";

            // เปลี่ยนคอลัมน์ LOT_NO เป็น ComboBox
            if (dtg_function.Columns.Contains("LOT_NO"))
            {
                // สร้าง List เพื่อเก็บข้อมูลจากคอลัมน์ LOT_NO
                List<string> lotNoList = new List<string>();

                // ลบคอลัมน์ LOT_NO เดิม
                dtg_function.Columns.Remove("LOT_NO");

                // ตรวจสอบว่า propQA.dtLotNo ไม่ใช่ null และมีข้อมูล
                if (propQA.dtLotNo != null && propQA.dtLotNo.Rows.Count > 0)
                {
                    // วนลูปเพื่อดึงข้อมูลจากคอลัมน์ LOT_NO
                    foreach (DataRow row in propQA.dtLotNo.Rows)
                    {
                        string lotNo = row["LOT_NO"]?.ToString()?.Trim();
                        if (!string.IsNullOrWhiteSpace(lotNo))
                        {
                            lotNoList.Add(lotNo);
                        }
                    }
                }

                // เพิ่มคอลัมน์ ComboBox ใหม่
                DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn
                {
                    Name = "LOT_NO",
                    HeaderText = "LOT NO",
                    DataPropertyName = "LOT_NO", // เชื่อมโยงกับคอลัมน์ใน DataTable
                    DataSource = lotNoList,
                    FlatStyle = FlatStyle.Flat // ปรับสไตล์เพื่อให้สีพื้นหลังมีผลมากขึ้น

                };
                dtg_function.Columns.Add(comboBoxColumn);

                // ตั้งค่าการเลือกอัตโนมัติถ้ามีข้อมูลเพียง 1 รายการ
                if (propQA.dtLotNo.Rows.Count == 1)
                {
                    string singleLotNo = propQA.dtLotNo.Rows[0]["LOT_NO"].ToString();
                    // ตั้งค่าเริ่มต้นสำหรับเซลล์ในคอลัมน์ LOT_NO
                    foreach (DataGridViewRow row in dtg_function.Rows)
                    {
                        if (row.Cells["LOT_NO"] is DataGridViewComboBoxCell comboCell)
                        {
                            comboCell.Value = singleLotNo;
                        }
                    }
                }

            }

            // เปลี่ยนคอลัมน์ VALUE เป็น ComboBox
            if (dtg_function.Columns.Contains("JUDGE"))
            {
                // ลบคอลัมน์ VALUE เดิม
                dtg_function.Columns.Remove("JUDGE");

                // เพิ่มคอลัมน์ ComboBox ใหม่
                DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn
                {
                    Name = "JUDGE",
                    HeaderText = "JUDGE",
                    DataPropertyName = "JUDGE", // เชื่อมโยงกับคอลัมน์ใน DataTable
                    DataSource = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("", ""), // ช่องว่าง
                new KeyValuePair<string, string>("0", "NG"),
                new KeyValuePair<string, string>("1", "OK")
            },
                    ValueMember = "Key",
                    DisplayMember = "Value" ,
                    FlatStyle = FlatStyle.Flat // ปรับสไตล์เพื่อให้สีพื้นหลังมีผลมากขึ้น
                };
                dtg_function.Columns.Add(comboBoxColumn);
            }

            // เปลี่ยนคอลัมน์ VALUE เป็น ComboBox ถ้า CRITERIA_MIN และ CRITERIA_MAX เป็น 1
            if (dtg_function.Columns.Contains("REMARK"))
            {
                // ลบคอลัมน์ VALUE เดิม
                dtg_function.Columns.Remove("REMARK");

                // เพิ่มคอลัมน์ ComboBox ใหม่
                DataGridViewTextBoxColumn textColumn = new DataGridViewTextBoxColumn
                {
                    Name = "REMARK",
                    HeaderText = "REMARK",
                    DataPropertyName = "REMARK"
                };

                dtg_function.Columns.Add(textColumn);
            }

            dtg_function.CellEndEdit += dtg_function_CellEndEdit;
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
                    CustomMsgBoxBase.ShowCustomMessageBox("กรุณากรอกจำนวน Cavity เป็นตัวเลขตั้งแต่ 0 ขึ้นไปทุกแถว!", "คำเตือน", CustomMsgBoxBase.MessageBoxIconType.Warning);
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


            GenerateDataTableFunction(dtg_cavity, 0);
        }

        private void dtg_cavity_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || dtg_cavity.Columns[e.ColumnIndex].Name != "SAMPLING_QTY")
            {
                return;
            }

            string value = e.FormattedValue?.ToString();
            if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out int qty) || qty < 0)
            {
                e.Cancel = true;
            }
        }

        private void dtg_cavity_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (e.Control is System.Windows.Forms.TextBox textBox)
            {
                textBox.KeyPress -= dtg_cavity_TextBox_KeyPress;

                if (dtg_cavity.CurrentCell != null &&
                    dtg_cavity.Columns[dtg_cavity.CurrentCell.ColumnIndex].Name == "SAMPLING_QTY")
                {
                    textBox.KeyPress += dtg_cavity_TextBox_KeyPress;
                }
            }
        }

        private void dtg_cavity_TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void dtg_function_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dtg_function.Columns[e.ColumnIndex].Name == "JUDGE")
            {
                DataGridViewRow row = dtg_function.Rows[e.RowIndex];

                if (
                    row.Cells["JUDGE"].Value != null &&
                    !DBNull.Value.Equals(row.Cells["JUDGE"].Value))
                {
                    //decimal min = Convert.ToDecimal(row.Cells["CRITERIA_MIN"].Value);
                    //decimal max = Convert.ToDecimal(row.Cells["CRITERIA_MAX"].Value);
                    //decimal value;

                    if (row.Cells["JUDGE"].Value.ToString() == "0")
                    {
                        // คำนวณ Point_Judge (1 ถ้าอยู่ในช่วง min-max, 0 ถ้านอกช่วง)
                        row.Cells["POINT_JUDGE"].Value = "0";
                    }
                    else if (row.Cells["JUDGE"].Value.ToString() == "1")
                    {
                        row.Cells["POINT_JUDGE"].Value = "1";
                    }
                    else
                    {
                        row.Cells["POINT_JUDGE"].Value = DBNull.Value; // ถ้าค่าไม่ถูกต้อง ให้เป็นค่าว่าง
                    }

                    // คำนวณ Total_Judge
                    CalculateTotalJudge();
                }
            }
        }

        private void CalculateTotalJudge()
        {
            foreach (DataRow dtRow in originalDataTable.Rows)
            {
                //if (!row.IsNewRow)
                //{
                // ถ้ามีแถวใดมี Point_Judge = 0 ให้ Total_Judge เป็น 0 ทั้งหมด
                if (dtRow["POINT_JUDGE"] != null && dtRow["POINT_JUDGE"].ToString() == "0")
                {
                    SetTotalJudge(0);
                    return;
                }
                //}
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

        private void ShowCustomMessageBox(string message, string title, CustomMsgBoxBase.MessageBoxIconType icon)
        {
            var msgBox = new CustomMsgBox();
            msgBox.SetMessage(message, title);
            msgBox.SetIcon(icon);
            msgBox.ShowDialog();
        }

        private void tb_record_Click(object sender, EventArgs e)
        {

            if (dtg_function.IsCurrentCellDirty || dtg_function.IsCurrentRowDirty)
            {
                dtg_function.EndEdit(); // จบการแก้ไขเซลล์ปัจจุบัน
                dtg_function.CommitEdit(DataGridViewDataErrorContexts.Commit); // บันทึกค่าลง DataSource
                bindingSource.EndEdit(); // บันทึกค่าลงใน BindingSource (ถ้าใช้)
            }

            if (dtg_function.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบ data ที่จะทำการ Record Function");
                return;
            }

            if (!IsDataGridViewValid(dtg_function)) // ตรวจสอบจาก DataTable แทน
            {
                return; // ไม่ทำต่อถ้ามีเซลล์ว่าง
            }

            propQA.TOTAL_STATUS = "1";
            propQA.EMP_ID = employee.EMP_CODE;

            propQA.dtFuncData = originalDataTable;

            foreach (DataRow row in originalDataTable.Rows)
            {
                propQA.TOTAL_STATUS = (Convert.ToInt32(row["TOTAL_JUDGE"]?.ToString()) * Convert.ToInt32(propQA.TOTAL_STATUS)).ToString();
            }

            if (propQA.TOTAL_STATUS == "0")
            {
                propQA.inProcStatus = "6";
                //propQA.reportStatus = "6";
            }
            else
            {
                propQA.inProcStatus = "1";
                //propQA.reportStatus = "1";
            }

            try
            {
                if (conQA.InsertFunctionData(propQA) == true)
                {
                    ProcStatus status;

                    bool parsed = int.TryParse(propQA.inProcStatus, out int statusId) && Enum.IsDefined(typeof(ProcStatus), statusId);
                    status = parsed ? (ProcStatus)statusId : ProcStatus.NG; // ค่าเริ่มต้นเป็น NG ถ้าแปลงไม่ได้



                    switch (status)
                    {
                        case ProcStatus.OK:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Function งาน OK เรียบร้อยแล้ว",
                                "สำเร็จ",
                                CustomMsgBoxBase.MessageBoxIconType.OK);
                            break;
                        case ProcStatus.Pending:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Function พบงาน ถูก PENDING",
                                "สำเร็จ",
                                CustomMsgBoxBase.MessageBoxIconType.Pending);
                            break;
                        default:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "สถานะไม่รู้จัก",
                                "ข้อผิดพลาด",
                                CustomMsgBoxBase.MessageBoxIconType.Question);
                            break;
                    }
                    loadstatus();
                    bt_function_Click();
                  
                }
                else
                {
                    CustomMsgBoxBase.ShowCustomMessageBox("ไม่สามารถ record data ลง database ได้", "ข้อผิดพลาด", CustomMsgBoxBase.MessageBoxIconType.NG);
                    //string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
                    //RequestReleaseMutex?.Invoke(mutexKey);
                }
            }
            finally 
            {

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
    

        private bool IsDataGridViewValid(DataGridView dtg)
        {
            foreach (DataGridViewRow row in dtg.Rows)
            {
                if (row.IsNewRow) continue;

                var valueCell = row.Cells["JUDGE"];
                var remarkCell = row.Cells["REMARK"];

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!dtg.Columns[cell.ColumnIndex].Visible) continue;

                    // Skip REMARK column for the general empty check
                    if (cell.ColumnIndex == dtg.Columns["REMARK"].Index) continue;

                    // Check if the cell (excluding REMARK) is empty
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        CustomMsgBoxBase.ShowCustomMessageBox($"พบเซลล์ว่างในแถวที่ {row.Index + 1} คอลัมน์ {dtg.Columns[cell.ColumnIndex].HeaderText}", "คำเตือน", CustomMsgBoxBase.MessageBoxIconType.Warning);
                        dtg.CurrentCell = cell;
                        return false;
                    }
                }

                // Specific validation for REMARK when VALUE is "NG"
                if (valueCell.Value != null && valueCell.Value.ToString() == "0")
                {
                    if (remarkCell.Value == null || string.IsNullOrWhiteSpace(remarkCell.Value.ToString()))
                    {
                        CustomMsgBoxBase.ShowCustomMessageBox($"กรุณากรอก REMARK สำหรับแถวที่ {row.Index + 1} ซึ่งมี VALUE เป็น NG!", "คำเตือน", CustomMsgBoxBase.MessageBoxIconType.Warning);
                        dtg.CurrentCell = remarkCell;
                        return false;
                    }
                }

                // Check REMARK length (applies whether VALUE is "NG" or "OK", if REMARK is not empty)
                if (remarkCell.Value != null && !string.IsNullOrWhiteSpace(remarkCell.Value.ToString()))
                {
                    string remarkText = remarkCell.Value.ToString();
                    if (remarkText.Length > 255)
                    {
                        CustomMsgBoxBase.ShowCustomMessageBox($"REMARK ในแถวที่ {row.Index + 1} มีความยาวเกิน 255 ตัวอักษร (ปัจจุบัน: {remarkText.Length} ตัวอักษร)!", "คำเตือน", CustomMsgBoxBase.MessageBoxIconType.Warning);
                        dtg.CurrentCell = remarkCell;
                        return false;
                    }
                }
            }

            return true;
        }

        public void bt_function_Click()
        {


            userControlSelectFunction usrConSelectFunc = new userControlSelectFunction()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()
            };

            usrConSelectFunc.Dock = DockStyle.Fill;
            usrConSelectFunc.propQA = new QAdataProperty();

            usrConSelectFunc.propQA.labelProcess = "Select Report for : Function Check";
            usrConSelectFunc.propQA.process = "Function_Check";
            usrConSelectFunc.propQA.prevProcess = "Inspection_Data_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpFunction(usrConSelectFunc.propQA);
            usrConSelectFunc.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectFunc.propQA.dtgRawMat.DataSource = dt;

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
                    panelMain.Controls.Add(usrConSelectFunc);
                    usrConSelectFunc.BringToFront();
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


        private void bt_back_Click(object sender, EventArgs e)
        {
            //update database unfinished
            propQA.inProcStatus = ((int)ProcStatus.Unfinished).ToString();
            propQA.reportStatus = ((int)ProcStatus.Unfinished).ToString();

            if (conQA.UpdateStatus(propQA) == false)
            {
                MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Unfinished ได้");
            }

            if (!conQA.DeleteReportActive(propQA))
            {
                MessageBox.Show("ไม่สามารถเพิ่ม report no กับ IP ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            bt_function_Click();
        }

        private void loadstatus()
        {
            if (this.ParentForm is frmMain mainForm)
            {
                mainForm.LoadStatus();
            }
        }

        private void dtg_function_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dtg_function.Rows)
            {
                // ตรวจสอบว่ามีค่าใน VALUE และ POINT_JUDGE หรือไม่
                if (row.Cells["JUDGE"].Value != null &&
                    !string.IsNullOrWhiteSpace(row.Cells["JUDGE"].Value.ToString()) &&
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

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (functionImages != null && functionImages.Count > 1)
            {
                if (keyData == Keys.PageUp || keyData == Keys.PageDown)
                {
                    if (keyData == Keys.PageUp)
                    {
                        currentFunctionImageIndex = (currentFunctionImageIndex - 1 + functionImages.Count) % functionImages.Count;
                    }
                    else
                    {
                        currentFunctionImageIndex = (currentFunctionImageIndex + 1) % functionImages.Count;
                    }

                    // ลบส่วน dispose ออก เพื่อป้องกันการ dispose Image ใน list
                    // if (picbox_func.Image != null)
                    // {
                    //     picbox_func.Image.Dispose();
                    //     picbox_func.Image = null;
                    // }
                    picbox_func.Image = functionImages[currentFunctionImageIndex];

                    return true; // บอกว่าจัดการ key แล้ว ไม่ให้ไปต่อ
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (functionImages != null)
                {
                    foreach (var img in functionImages)
                    {
                        img?.Dispose();
                    }
                    functionImages.Clear();
                    functionImages = null;
                }
                // dispose อื่นๆ ถ้ามี
            }
            base.Dispose(disposing);
        }

    }
}
