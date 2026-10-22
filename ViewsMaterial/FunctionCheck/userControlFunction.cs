using MySqlX.XDevAPI.Common;
using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.ViewsMaterial.CustomMsg;
using RawMat.ViewsMaterial.RegularCheck;
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

namespace RawMat.ViewsMaterial.FunctionCheck
{
    public partial class userControlFunction : UserControl, IInspectionScreen
    {
        private const string FunctionCheckColumnPrefix = "FUNCTION_CHECK_";
        private const int FunctionGridHeightFull = 351;
        private const int FunctionCheckColumnWidth = 260;
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

        // ร่าง : ปุ่ม tb_record เป็น Record Data ตอนยังกรอกไม่ครบ และเป็น Save and Continue เมื่อครบ (Utilities/InspectionDraft.cs)
        private const string DraftDataTable = "FUNCTION";
        private const string DraftCavityTable = "CAVITY";
        // Function มี 1 แถวต่อ 1 ชิ้น
        private static readonly string[] DraftKeyColumns = { "CAVITY_NAME", "SAMPLING_NO" };
        private DataSet _loadedDraft;
        // ค่าบนจอตอนที่ถือว่าเก็บแล้ว (เปิดหน้าเสร็จ / กด Record Data) ใช้เทียบตอนออกว่ามีค่าที่ยังไม่ได้ Record ไหม
        private string _recordedDraft;

        public userControlFunction()
        {

            InitializeComponent();
            dtg_cavity.CellValidating += dtg_cavity_CellValidating;
            dtg_cavity.EditingControlShowing += dtg_cavity_EditingControlShowing;
            dtg_function.EditingControlShowing += dtg_function_EditingControlShowing;
            dtg_function.DataError += dtg_function_DataError;
            dtg_function.CellMouseDown += OpenComboBoxOnSingleMouseClick;
            dtg_function.CellBeginEdit += RequireRowLotNo_CellBeginEdit;

            // กันเคสที่ออกจากหน้านี้โดยไม่ผ่าน bt_function_Click (เช่น กด Home / logout
            // ซึ่ง NavigationService จะ dispose control ทิ้งเลย) ให้หยุด timer เสมอ
            this.Disposed += userControlFunction_Disposed;

        }

        private void userControlFunction_Disposed(object sender, EventArgs e)
        {
            StopStatusWatchdog();
        }

        // หยุด timer เฝ้าสถานะให้ขาดจาก control ที่กำลังจะถูกทิ้ง
        // ถ้าไม่หยุด Windows.Forms.Timer ที่ยัง Enabled จะถูก root ไว้ด้วย native window ของตัวเอง
        // -> เดินต่อพร้อม propQA.Report_No ใบเก่า แล้วไปเด้งเตือนใส่ report ใบอื่นที่เปิดทีหลัง
        private void StopStatusWatchdog()
        {
            if (checkTimer == null)
            {
                return;
            }

            checkTimer.Stop();
            checkTimer.Tick -= CheckTimer_Tick;
            checkTimer.Dispose();
            checkTimer = null;
        }

        private void dtg_function_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            EquipmentSerialColumn.HandleDataError(dtg_function, e);
            e.ThrowException = false;
            e.Cancel = false;
        }

        private void dtg_function_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            EquipmentSerialColumn.HandleEditingControlShowing(dtg_function, e);
            InspectionDraft.WatchEditingControl(e, EditingValue_Changed);
        }

        // พิมพ์หรือเลือกค่าในตาราง : เปลี่ยนปุ่มทันที ไม่ต้องรอกด Enter
        private void EditingValue_Changed(object sender, EventArgs e)
        {
            UpdateRecordButtonMode();
        }

        private void OpenComboBoxOnSingleMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            DataGridView grid = sender as DataGridView;
            DataGridViewCell cell = grid?.Rows[e.RowIndex].Cells[e.ColumnIndex];
            if (cell == null || cell.ReadOnly || !(cell is DataGridViewComboBoxCell))
            {
                return;
            }

            grid.CurrentCell = cell;
            grid.BeginEdit(true);

            grid.BeginInvoke((MethodInvoker)(() =>
            {
                if (!grid.IsDisposed && grid.CurrentCell == cell &&
                    grid.EditingControl is System.Windows.Forms.ComboBox comboBox)
                {
                    comboBox.DroppedDown = true;
                }
            }));
        }

        // ต้องเลือก LOT NO ของแถวก่อนกรอกช่องอื่นในแถวนั้น (user ขอ 2026-09-29)
        // Function เลือก Lot ทีละแถว (แต่ละ sample มาจากคนละ Lot ได้) จึงไม่ได้ใช้ combo บนหัวหน้าจอแบบ Regular / Dimension
        // มี Lot เดียว ตอนสร้างตารางใส่ให้ทุกแถวอยู่แล้ว จะไม่เจอกล่องนี้
        private void RequireRowLotNo_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex < 0 || !dtg_function.Columns.Contains("LOT_NO"))
            {
                return;
            }

            DataGridViewColumn lotColumn = dtg_function.Columns["LOT_NO"];
            if (e.ColumnIndex == lotColumn.Index || !lotColumn.Visible)
            {
                return;
            }

            DataGridViewCell lotCell = dtg_function.Rows[e.RowIndex].Cells[lotColumn.Index];
            if (lotCell.ReadOnly || !string.IsNullOrWhiteSpace(lotCell.Value?.ToString()))
            {
                return;
            }

            e.Cancel = true;

            if ((lotColumn as DataGridViewComboBoxColumn)?.DataSource is System.Collections.ICollection lots && lots.Count == 0)
            {
                MessageBox.Show(InspectionDraft.NoLotNoFromPackingText, "Lot No.", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            MessageBox.Show($"กรุณาเลือก LOT NO ของแถวที่ {e.RowIndex + 1} ก่อนกรอกข้อมูล", "Lot No.", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            // พาไปช่อง LOT NO ของแถวนั้นแล้วเปิดรายการให้เลือก
            // ห้ามตั้ง CurrentCell ใน CellBeginEdit ตรง ๆ (DataGridView ถือว่าเรียกซ้อน) จึงรอให้ event นี้จบก่อน
            if (!dtg_function.IsHandleCreated)
            {
                return;
            }

            dtg_function.BeginInvoke((MethodInvoker)(() =>
            {
                if (dtg_function.IsDisposed || lotCell.DataGridView != dtg_function)
                {
                    return;
                }

                dtg_function.CurrentCell = lotCell;
                dtg_function.BeginEdit(true);

                if (dtg_function.EditingControl is System.Windows.Forms.ComboBox comboBox)
                {
                    comboBox.DroppedDown = true;
                }
            }));
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
                lb_TotalCavity.Text = "Total Cavity : " + GetTotalCavitySamplingQty();
                gb_cavity.Text = GetCavityRuleText();

                // โหลดรูป Cavity (สมมติ refactor แล้ว ใช้ LoadImages แทน LoadCavityImage)
                picbox_cavity.Image = imgCls.LoadSingleImage("CavityPath", propQA.M_CODE); // สมมติมี key "CavityPath" ใน app.config
                dtg_cavity.DataSource = propQA.dtCavity;

                // ห้าม sort : ลำดับ cavity มาจาก master (Cavity_Name เช่น "M,N,O,P") และมีความหมายในตัว
                // ตอนสร้างแถวตัวอย่างโค้ดวน dtg_cavity.Rows ตามลำดับที่แสดงบนจอ ถ้าผู้ใช้เผลอคลิกหัวคอลัมน์
                // SAMPLING_NO จะถูกจับคู่กับ cavity ผิดตัวแล้วบันทึกลงฐานข้อมูลแบบนั้นเลย
                foreach (DataGridViewColumn cavityColumn in dtg_cavity.Columns)
                {
                    cavityColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                }

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

            // ค่าที่เคย Save ไว้ตอนยังกรอกไม่ครบ ดึงกลับมาเติมให้ทำต่อได้
            // ต้องอยู่หลัง await ข้างบน เพราะตารางถูกสร้างหลังโหลดรูปเสร็จ
            LoadDraft();

            UpdateRecordButtonMode();
            _recordedDraft = InspectionDraft.Serialize(BuildDraft());

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
            EnsureNotBlocked(); // ถ้าถูกบล็อกจะเด้งเตือนและพากลับหน้าเลือก report ให้เอง
        }

        // เช็คว่างาน Function ของ report ใบนี้ยังทำต่อได้ไหม
        // คืน true = ทำต่อได้ , false = ถูกบล็อก (เด้งข้อความ + พากลับหน้าเลือก report แล้ว)
        private bool EnsureNotBlocked()
        {
            string status = conQA.GetProcessStatusId(propQA);

            if (!QAdataControllers.IsProcessBlocked(status))
            {
                return true;
            }

            CustomMsgBoxBase.ShowCustomMessageBox(
                QAdataControllers.BuildProcessBlockedMessage(status, propQA.Report_No, "Function Check"),
                "แจ้งเตือน",
                CustomMsgBoxBase.MessageBoxIconType.NG);

            bt_function_Click(); // StopStatusWatchdog() ข้างในหยุด timer ให้แล้ว
            return false;
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
            dtAllSum.Columns.Add("EQUIPMENT_SERIAL_ID", typeof(int));
            dtAllSum.Columns.Add("EQUIPMENT_SERIAL", typeof(string));
            dtAllSum.Columns.Add("EQUIPMENT_TYPE", typeof(string));
            dtAllSum.Columns.Add("EQUIPMENT_NAME", typeof(string));
            dtAllSum.Columns.Add("JUDGE", typeof(string));
            dtAllSum.Columns.Add("POINT_JUDGE", typeof(string));
            dtAllSum.Columns.Add("TOTAL_JUDGE", typeof(string));
            dtAllSum.Columns.Add("REMARK", typeof(string));

            bool hasFunctionChecks = propQA.dtFuncCheck != null && propQA.dtFuncCheck.Rows.Count > 0;
            if (hasFunctionChecks)
            {
                foreach (DataRow check in propQA.dtFuncCheck.Rows)
                {
                    string columnName = GetFunctionCheckColumnName(check);
                    if (!dtAllSum.Columns.Contains(columnName))
                    {
                        dtAllSum.Columns.Add(columnName, typeof(string));
                    }
                }
            }

            int qtySampCounter = 1;
            bool hasEquipment = propQA.dtFuncEq != null && propQA.dtFuncEq.Rows.Count > 0 &&
                                !string.IsNullOrWhiteSpace(propQA.dtFuncEq.Rows[0]["EQUIPMENT_TYPE"]?.ToString());
            string equipmentType = hasEquipment ? propQA.dtFuncEq.Rows[0]["EQUIPMENT_TYPE"].ToString() : null;
            string equipmentName = hasEquipment ? propQA.dtFuncEq.Rows[0]["Equipment_Name"].ToString() : null;

            if (dtgCavity == null)
            {
                for (int i = 0; i < sampQty; i++)
                {
                    DataRow dataRow = dtAllSum.NewRow();
                    dataRow["SAMPLING_NO"] = qtySampCounter++;
                    if (hasEquipment)
                    {
                        dataRow["EQUIPMENT_TYPE"] = equipmentType;
                        dataRow["EQUIPMENT_NAME"] = equipmentName;
                    }
                    dtAllSum.Rows.Add(dataRow);
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
                        DataRow dataRow = dtAllSum.NewRow();
                        dataRow["CAVITY_NAME"] = name;
                        dataRow["SAMPLING_NO"] = qtySampCounter++;
                        if (hasEquipment)
                        {
                            dataRow["EQUIPMENT_TYPE"] = equipmentType;
                            dataRow["EQUIPMENT_NAME"] = equipmentName;
                        }
                        dtAllSum.Rows.Add(dataRow);
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
            string[] hiddenColumns = { "POINT_JUDGE", "TOTAL_JUDGE", "EQUIPMENT_SERIAL_ID", "EQUIPMENT_TYPE" };
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
                column.ReadOnly = column.Name != "JUDGE" &&
                                  !column.Name.StartsWith(FunctionCheckColumnPrefix, StringComparison.Ordinal) &&
                                  !(hasEquipment && column.Name == "EQUIPMENT_SERIAL");
            }

            dtg_function.Columns["JUDGE"].Visible = !hasFunctionChecks;

            dtg_function.Columns["EQUIPMENT_SERIAL"].Visible = hasEquipment;
            dtg_function.Columns["EQUIPMENT_NAME"].Visible = hasEquipment;

            // เปลี่ยน HeaderText
            if (dtg_function.Columns.Contains("CAVITY_NAME")) dtg_function.Columns["CAVITY_NAME"].HeaderText = "CAV.";
            if (dtg_function.Columns.Contains("SAMPLING_NO")) dtg_function.Columns["SAMPLING_NO"].HeaderText = "SAMPLE";
            if (dtg_function.Columns.Contains("EQUIPMENT_SERIAL")) dtg_function.Columns["EQUIPMENT_SERIAL"].HeaderText = "EQ_SN";
            if (dtg_function.Columns.Contains("EQUIPMENT_NAME")) dtg_function.Columns["EQUIPMENT_NAME"].HeaderText = "EQ_NAME";

            if (hasEquipment)
            {
                EquipmentSerialColumn.Apply(dtg_function, conQA.EquipmentSerialList());
            }

            // เปลี่ยนคอลัมน์ LOT_NO เป็น ComboBox
            if (dtg_function.Columns.Contains("LOT_NO"))
            {
                int lotNoColumnIndex = dtg_function.Columns["LOT_NO"].Index;
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
                dtg_function.Columns.Insert(lotNoColumnIndex, comboBoxColumn);

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
            if (dtg_function.Columns.Contains("JUDGE") && !hasFunctionChecks)
            {
                // ลบคอลัมน์ VALUE เดิม
                dtg_function.Columns.Remove("JUDGE");

                // เพิ่มคอลัมน์ ComboBox ใหม่
                DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn
                {
                    Name = "JUDGE",
                    HeaderText = "OK / NG",
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

            if (hasFunctionChecks)
            {
                foreach (DataRow check in propQA.dtFuncCheck.Rows)
                {
                    string columnName = GetFunctionCheckColumnName(check);
                    ReplaceWithYesNoColumn(columnName, check["CHECK_DETAIL"]?.ToString());
                }
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

            ConfigureFunctionGridColumns();

            dtg_function.CellEndEdit += dtg_function_CellEndEdit;
        }

        // ยอดรวมที่ต้องเก็บทั้งใบ = ผลรวมของ SAMPLING_QTY ทุก cavity
        // Type 4 : Sampling_Qty ใน master เป็นจำนวน "ต่อ cavity" ต้องคูณจำนวน cavity เอง
        // Type 3 : ยกพื้นเป็น cavityQty x qtyPerCavity ไปแล้วตอนเลือกใบ (userControlSelectFunction.cs:260) ห้ามคูณซ้ำ
        private int GetTotalCavitySamplingQty()
        {
            int.TryParse(propQA.SAMPLING_QTY?.Trim(), out int samplingQty);

            if (propQA.SAMPLING_TYPE != "4")
            {
                return samplingQty;
            }

            int.TryParse(propQA.CAVITY_QTY?.Trim(), out int cavityQty);

            return cavityQty > 0 ? samplingQty * cavityQty : samplingQty;
        }

        // ข้อความหัวกรอบ Cavity : บอก user ว่าต้องเก็บ cavity ละกี่ตัว
        // อ้างอิง readout ชุดเดียวกับ DatabaseScripts/20260826_fix_shin005_01_sampling.sql:68-74
        // type 4 = cavity ละเท่านี้เป๊ะ , type 3 = cavity ละอย่างน้อยเท่านี้ (ยอดรวมอาจถูกยกขึ้นตามตาราง AQL)
        private string GetCavityRuleText()
        {
            // ไม่ได้ตั้งจำนวนต่อ cavity ไว้ ก็ไม่มีกฎอะไรจะบอก ปล่อยหัวกรอบว่างดีกว่าโชว์คำว่า Cavity ลอย ๆ
            if (!int.TryParse(propQA.SAMPLING_QTY_PER_CAVITY?.Trim(), out int perCavity) || perCavity <= 0)
            {
                return string.Empty;
            }

            string unit = perCavity == 1 ? "Pc." : "Pcs.";

            return propQA.SAMPLING_TYPE == "4"
                ? $"Cavity : {perCavity} {unit}/Cavity"
                : $"Cavity : >= {perCavity} {unit}/Cavity";
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

            int expectedQty = GetTotalCavitySamplingQty();

            if (totalQty != expectedQty)
            {
                MessageBox.Show($"ผลรวมของ QTY ต้องได้ {expectedQty}  (ปัจจุบัน: {totalQty})", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ล็อก dtg_data ไม่ให้แก้ไขข้อมูล
            dtg_cavity.ReadOnly = true;


            GenerateDataTableFunction(dtg_cavity, 0);

            // ร่างที่ยอดรวม cavity ไม่ตรงตอนเปิดหน้า จะเติมค่าให้ตอนนี้แทน
            ApplyDraftValues();

            UpdateRecordButtonMode();
        }

        // เงื่อนไขเดียวกับตอน Load ที่ตัดสินว่าต้องโชว์กรอบ cavity
        private bool RequiresCavityInput()
        {
            int.TryParse(propQA.CAVITY_QTY?.Trim(), out int cavityQty);
            return propQA.SAMPLING_TYPE == "4"
                || (propQA.SAMPLING_TYPE == "3" && cavityQty != 0);
        }

        // เงื่อนไขเดียวกับ bt_confirmCavity_Click แต่ไม่เด้งข้อความ ใช้ตอนเติมจากร่าง
        private bool IsCavityInputComplete()
        {
            int totalQty = 0;

            foreach (DataRow row in propQA.dtCavity.Rows)
            {
                if (!int.TryParse(row["SAMPLING_QTY"]?.ToString(), out int qty) || qty < 0)
                {
                    return false;
                }

                totalQty += qty;
            }

            return totalQty == GetTotalCavitySamplingQty();
        }

        private void LoadDraft()
        {
            _loadedDraft = InspectionDraft.Load(conQA, propQA);
            if (_loadedDraft == null)
            {
                return;
            }

            if (RequiresCavityInput() && propQA.dtCavity != null && _loadedDraft.Tables.Contains(DraftCavityTable))
            {
                InspectionDraft.Restore(propQA.dtCavity, _loadedDraft.Tables[DraftCavityTable],
                    new[] { "CAVITY_NAME" }, new[] { "SAMPLING_QTY" });

                // มีตารางผลตรวจในร่างแปลว่าเคย confirm cavity ไปแล้ว ทำให้เหมือนกด confirm อีกครั้ง
                // ถ้ายอดรวมไม่ตรงแล้ว (master เปลี่ยน) ปล่อยให้ผู้ตรวจแก้แล้วกด confirm เอง ค่าจะเติมให้ตอนนั้น
                bool hadDataTable = _loadedDraft.Tables.Contains(DraftDataTable)
                    && _loadedDraft.Tables[DraftDataTable].Rows.Count > 0;

                if (hadDataTable && IsCavityInputComplete())
                {
                    dtg_cavity.ReadOnly = true;
                    GenerateDataTableFunction(dtg_cavity, 0);
                }
            }

            ApplyDraftValues();
        }

        // คอลัมน์ที่ผู้ตรวจกรอก : ข้อตรวจแต่ละข้อ (FUNCTION_CHECK_<ID>) มีจำนวนไม่เท่ากันแต่ละ M-CODE
        private string[] GetDraftValueColumns()
        {
            var columns = new List<string> { "LOT_NO", "EQUIPMENT_SERIAL", "JUDGE", "REMARK" };

            if (originalDataTable != null)
            {
                columns.AddRange(originalDataTable.Columns.Cast<DataColumn>()
                    .Select(column => column.ColumnName)
                    .Where(name => name.StartsWith(FunctionCheckColumnPrefix, StringComparison.Ordinal)));
            }

            return columns.ToArray();
        }

        private void ApplyDraftValues()
        {
            if (_loadedDraft == null || originalDataTable == null || !_loadedDraft.Tables.Contains(DraftDataTable))
            {
                return;
            }

            bool raiseEvents = bindingSource.RaiseListChangedEvents;
            bindingSource.RaiseListChangedEvents = false;

            try
            {
                InspectionDraft.Restore(originalDataTable, _loadedDraft.Tables[DraftDataTable], DraftKeyColumns, GetDraftValueColumns());
                RecalculateJudges();
                EquipmentSerialColumn.AddMissingItems(dtg_function);
            }
            finally
            {
                bindingSource.RaiseListChangedEvents = raiseEvents;
            }

            // DataBindingComplete จะทาสีแถว NG ให้ใหม่
            bindingSource.ResetBindings(false);

            // เติมครั้งเดียวพอ ถ้ากด confirm cavity ซ้ำไม่ควรได้ค่าจากร่างเก่ากลับมาทับ
            _loadedDraft = null;
        }

        // ค่าที่เติมจากร่างไม่ได้ผ่าน dtg_function_CellEndEdit จึงต้องคิดผลตัดสินเองด้วยเกณฑ์เดียวกัน
        private void RecalculateJudges()
        {
            bool hasFunctionChecks = propQA.dtFuncCheck != null && propQA.dtFuncCheck.Rows.Count > 0;

            foreach (DataRow row in originalDataTable.Rows)
            {
                if (hasFunctionChecks)
                {
                    // เหมือน CalculateSampleFunctionJudge : มีข้อไหน NG = NG , ยังว่างอยู่ = ยังไม่ตัดสิน
                    bool hasBlank = false;
                    bool hasNo = false;

                    foreach (DataRow check in propQA.dtFuncCheck.Rows)
                    {
                        string value = row[GetFunctionCheckColumnName(check)]?.ToString();
                        if (string.IsNullOrWhiteSpace(value)) hasBlank = true;
                        else if (value == "0") hasNo = true;
                    }

                    object judge = hasNo ? (object)"0" : hasBlank ? DBNull.Value : (object)"1";
                    row["JUDGE"] = judge;
                    row["POINT_JUDGE"] = judge;
                }
                else
                {
                    string judge = row["JUDGE"]?.ToString();
                    if (string.IsNullOrWhiteSpace(judge))
                    {
                        continue;
                    }

                    row["POINT_JUDGE"] = judge == "0" || judge == "1" ? (object)judge : DBNull.Value;
                }
            }

            CalculateTotalJudge();
        }

        private DataSet BuildDraft()
        {
            var draft = new DataSet("DRAFT");

            if (originalDataTable != null)
            {
                var columns = new List<string>(DraftKeyColumns);
                columns.AddRange(GetDraftValueColumns());
                draft.Tables.Add(InspectionDraft.Snapshot(originalDataTable, DraftDataTable, columns.ToArray()));
            }

            if (RequiresCavityInput() && propQA.dtCavity != null)
            {
                draft.Tables.Add(InspectionDraft.Snapshot(propQA.dtCavity, DraftCavityTable, "CAVITY_NAME", "SAMPLING_QTY"));
            }

            return draft;
        }

        private bool SaveDraft(bool showMessage)
        {
            DataSet draft = BuildDraft();

            if (!InspectionDraft.Save(conQA, propQA, draft, employee.EMP_CODE))
            {
                return false;
            }

            _recordedDraft = propQA.DRAFT_DATA;
            UpdateRecordButtonMode();

            if (showMessage)
            {
                DataGridViewCell invalidCell = null;
                string problem = originalDataTable == null
                    ? "ยังไม่ได้ Confirm Cavity"
                    : FindInvalidCell(dtg_function, out invalidCell);

                CustomMsgBoxBase.ShowCustomMessageBox(
                    "Save ข้อมูลที่กรอกไว้แล้ว กลับมาทำต่อได้" + (problem == null ? string.Empty : "\n" + problem),
                    "Save",
                    CustomMsgBoxBase.MessageBoxIconType.OK);

                // พาไปช่องที่ยังขาด เหมือนที่ตัวเตือนเดิมเคยทำ
                if (invalidCell != null)
                {
                    dtg_function.CurrentCell = invalidCell;
                }
            }

            return true;
        }

        // กดเมนูอื่น / Logout โดยไม่ผ่านปุ่มย้อนกลับ : NavigationService เรียกก่อน Dispose หน้านี้ (IInspectionScreen)
        // ทำแบบเดียวกับปุ่มย้อนกลับ คือตั้ง Not Finished yet + ปลดล็อกใบ
        // ไม่เก็บร่างให้ ค่าที่ยังไม่ได้กด Record Data จะทิ้งไป (user เลือกให้เก็บเฉพาะตอนกด Record Data)
        public void LeaveScreen()
        {
            InspectionDraft.ReleaseReport(conQA, propQA);
        }

        // ออกจากหน้า (ย้อนกลับ / Home / Logout) : ค่าบนจอไม่ตรงกับตอนเปิดหน้าหรือตอนกด Record Data ครั้งล่าสุด = ถามก่อน
        // false = ผู้ตรวจกด Cancel อยู่หน้าเดิม ค่าที่กรอกยังอยู่บนจอ
        public bool ConfirmLeave()
        {
            if (_recordedDraft == null) return true; // หน้ายังเปิดไม่เสร็จ ยังไม่มีอะไรให้เสีย

            CommitGridEdits();

            // ช่องที่ยัง commit ไม่ได้ (ค่าไม่ผ่าน CellValidating) ก็คือพิมพ์ค้างไว้
            bool unrecorded = dtg_function.IsCurrentCellDirty || dtg_cavity.IsCurrentCellDirty
                || InspectionDraft.Serialize(BuildDraft()) != _recordedDraft;

            return !unrecorded || InspectionDraft.ConfirmLeaveUnrecorded(this, "Function");
        }

        private void CommitGridEdits()
        {
            if (dtg_function.IsCurrentCellDirty || dtg_function.IsCurrentRowDirty)
            {
                dtg_function.EndEdit(); // จบการแก้ไขเซลล์ปัจจุบัน
                dtg_function.CommitEdit(DataGridViewDataErrorContexts.Commit); // บันทึกค่าลง DataSource
                bindingSource.EndEdit(); // บันทึกค่าลงใน BindingSource (ถ้าใช้)
            }

            if (dtg_cavity.IsCurrentCellDirty)
            {
                dtg_cavity.EndEdit();
            }
        }

        // ครบ = สร้างตารางแล้ว และไม่มีช่องที่ FindInvalidCell ฟ้อง
        private bool IsInputComplete()
        {
            return originalDataTable != null
                && dtg_function.Rows.Count > 0
                && FindInvalidCell(dtg_function, out _) == null;
        }

        private void UpdateRecordButtonMode()
        {
            InspectionDraft.ApplyButtonMode(tb_record, IsInputComplete());
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
            ApplyFunctionCellEdit(e);

            // ผลตัดสินต้องคิดเสร็จก่อน ปุ่มถึงจะรู้ว่าแถว NG ต้องมี REMARK หรือยัง
            UpdateRecordButtonMode();
        }

        private void ApplyFunctionCellEdit(DataGridViewCellEventArgs e)
        {
            string editedColumn = dtg_function.Columns[e.ColumnIndex].Name;
            if (dtg_function.Columns[e.ColumnIndex].Name == "EQUIPMENT_SERIAL")
            {
                string serial = dtg_function.Rows[e.RowIndex].Cells["EQUIPMENT_SERIAL"].Value?.ToString();
                foreach (DataRow dataRow in originalDataTable.Rows)
                {
                    dataRow["EQUIPMENT_SERIAL"] = string.IsNullOrWhiteSpace(serial) ? (object)DBNull.Value : serial.Trim();
                    dataRow["EQUIPMENT_SERIAL_ID"] = DBNull.Value;
                }
                return;
            }

            if (editedColumn.StartsWith(FunctionCheckColumnPrefix, StringComparison.Ordinal))
            {
                CalculateSampleFunctionJudge(dtg_function.Rows[e.RowIndex]);
                CalculateTotalJudge();
                return;
            }

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

        private static string GetFunctionCheckColumnName(DataRow check)
        {
            return FunctionCheckColumnPrefix + check["ID"].ToString();
        }

        private void ReplaceWithYesNoColumn(string columnName, string headerText)
        {
            if (!dtg_function.Columns.Contains(columnName)) return;

            int index = dtg_function.Columns[columnName].Index;
            dtg_function.Columns.Remove(columnName);
            var combo = new DataGridViewComboBoxColumn
            {
                Name = columnName,
                DataPropertyName = columnName,
                HeaderText = string.IsNullOrWhiteSpace(headerText) ? "OK / NG" : headerText,
                DataSource = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("", ""),
                    new KeyValuePair<string, string>("0", "NG"),
                    new KeyValuePair<string, string>("1", "OK")
                },
                ValueMember = "Key",
                DisplayMember = "Value",
                FlatStyle = FlatStyle.Flat,
                MinimumWidth = 150
            };
            dtg_function.Columns.Insert(index, combo);
        }

        private void ConfigureFunctionGridColumns()
        {
            dtg_function.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dtg_function.ScrollBars = ScrollBars.Both;
            dtg_function.Height = FunctionGridHeightFull;

            SetFunctionGridColumnWidth("CAVITY_NAME", 110, true);
            SetFunctionGridColumnWidth("SAMPLING_NO", 110, true);
            SetFunctionGridColumnWidth("LOT_NO", 180, true);
            SetFunctionGridColumnWidth("EQUIPMENT_SERIAL", 180);
            SetFunctionGridColumnWidth("EQUIPMENT_NAME", 220);
            SetFunctionGridColumnWidth("JUDGE", 150);

            if (propQA.dtFuncCheck != null)
            {
                foreach (DataRow check in propQA.dtFuncCheck.Rows)
                {
                    SetFunctionGridColumnWidth(GetFunctionCheckColumnName(check), FunctionCheckColumnWidth);
                }
            }

            if (dtg_function.Columns.Contains("REMARK"))
            {
                DataGridViewColumn remarkColumn = dtg_function.Columns["REMARK"];
                remarkColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                remarkColumn.MinimumWidth = 240;
                remarkColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void SetFunctionGridColumnWidth(string columnName, int width, bool frozen = false)
        {
            if (!dtg_function.Columns.Contains(columnName)) return;

            DataGridViewColumn column = dtg_function.Columns[columnName];
            column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            column.Width = width;
            column.SortMode = DataGridViewColumnSortMode.NotSortable;
            column.Frozen = frozen;
        }

        private void CalculateSampleFunctionJudge(DataGridViewRow row)
        {
            bool hasBlank = false;
            bool hasNo = false;

            foreach (DataRow check in propQA.dtFuncCheck.Rows)
            {
                string value = row.Cells[GetFunctionCheckColumnName(check)].Value?.ToString();
                if (string.IsNullOrWhiteSpace(value)) hasBlank = true;
                else if (value == "0") hasNo = true;
            }

            object judge = hasNo ? (object)"0" : hasBlank ? DBNull.Value : (object)"1";
            row.Cells["JUDGE"].Value = judge;
            row.Cells["POINT_JUDGE"].Value = judge;
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
            // กันบันทึกทับงานที่เพิ่งถูกตั้ง Pending / ตัดสิน NG ระหว่างที่ operator กำลังกรอกอยู่
            if (!EnsureNotBlocked())
            {
                return;
            }

            CommitGridEdits();

            // ใบที่ต้อง confirm cavity ก่อนจะยังไม่มีตาราง แต่จำนวนต่อ cavity ที่กรอกไว้ก็ Save เก็บได้
            if (originalDataTable == null || dtg_function.Rows.Count == 0)
            {
                if (RequiresCavityInput())
                {
                    SaveDraft(true);
                    return;
                }

                MessageBox.Show("ไม่พบ data ที่จะทำการ Record Function");
                return;
            }

            // ยังกรอกไม่ครบ ปุ่มเป็น Record Data : เก็บร่างไว้ อยู่หน้าเดิม ไม่ตัดสินผล
            if (!IsInputComplete())
            {
                SaveDraft(true);
                return;
            }

            // ครบแล้ว ปุ่มเป็น Save and Continue : ถามก่อน เพราะข้อความบนปุ่มเปลี่ยนเองทันทีที่กรอกช่องสุดท้าย
            // ผู้ตรวจที่ตั้งใจแค่เก็บไว้ก่อนแล้วค่อยกลับมาทวนจะได้ไม่ส่งผลไปโดยไม่รู้ตัว
            // Cancel = ยังไม่ส่งผล เก็บเป็นร่างไว้ก่อน เหมือนกด Record Data
            if (!InspectionDraft.ConfirmContinue(this, "Function"))
            {
                SaveDraft(true);
                return;
            }

            // เก็บร่างล่าสุดไว้ก่อนส่งผล ถ้าบันทึกพังกลางทางค่าที่กรอกยังอยู่
            SaveDraft(false);

            propQA.TOTAL_STATUS = "1";
            propQA.EMP_ID = employee.EMP_CODE;

            if (!ResolveFunctionEquipmentSerialIds())
            {
                return;
            }

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
                    // ผลจริงลง db_function_data แล้ว ร่างไม่ต้องใช้อีก
                    // ถ้าค้างไว้ ใบที่ถูกส่งกลับมาตรวจซ้ำจะโหลดค่ารอบเก่ากลับมาให้
                    conQA.DeleteInspectionDraft(propQA);

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
                StopStatusWatchdog();


            }
            return;
        }

        private bool ResolveFunctionEquipmentSerialIds()
        {
            if (!dtg_function.Columns.Contains("EQUIPMENT_SERIAL") ||
                !dtg_function.Columns["EQUIPMENT_SERIAL"].Visible)
            {
                foreach (DataRow row in originalDataTable.Rows)
                {
                    row["EQUIPMENT_SERIAL_ID"] = DBNull.Value;
                }
                return true;
            }

            var resolvedIds = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (DataRow row in originalDataTable.Rows)
                {
                    string serial = row["EQUIPMENT_SERIAL"]?.ToString().Trim();
                    string equipmentType = row["EQUIPMENT_TYPE"]?.ToString().Trim();

                    if (string.IsNullOrWhiteSpace(serial))
                    {
                        row["EQUIPMENT_SERIAL_ID"] = DBNull.Value;
                        continue;
                    }

                    string key = equipmentType + "\u001f" + serial;

                    if (!resolvedIds.TryGetValue(key, out int serialId))
                    {
                        propQA.EQUIPMENT_SERIAL = serial;
                        propQA.EQUIPMENT_TYPE_ID = equipmentType;
                        serialId = conQA.InsertEquipmentSerial(propQA);
                        resolvedIds[key] = serialId;
                    }

                    row["EQUIPMENT_SERIAL_ID"] = serialId;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("ไม่สามารถบันทึก Equipment S/N ได้: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    

        // หาช่องแรกที่ยังบันทึกผลไม่ได้ คืนข้อความบอกผู้ตรวจ หรือ null ถ้าครบแล้ว
        // ใช้ทั้งตอนบอกช่องที่ขาดหลัง Save และตอนตัดสินว่าปุ่มเป็น Record Data หรือ Save and Continue
        // ไม่ได้แบ่งหน้าเหมือน Regular / Dimension จึงไล่จากแถวบนจอได้เลย
        private static string FindInvalidCell(DataGridView dtg, out DataGridViewCell invalidCell)
        {
            invalidCell = null;

            // ช่องที่กำลังพิมพ์/เลือกอยู่ยังไม่ถูกเขียนลงเซลล์จนกว่าจะกด Enter จึงอ่านจากตัวแก้ไขแทน ปุ่มจะได้เปลี่ยนทันที
            InspectionDraft.TryGetPendingEdit(dtg, out DataGridViewCell editingCell, out string editingValue);
            string ValueOf(DataGridViewCell cell) => cell == editingCell ? editingValue : cell.Value?.ToString();

            List<DataGridViewColumn> checkColumns = dtg.Columns.Cast<DataGridViewColumn>()
                .Where(column => column.Name.StartsWith(FunctionCheckColumnPrefix, StringComparison.Ordinal))
                .ToList();

            foreach (DataGridViewRow row in dtg.Rows)
            {
                if (row.IsNewRow) continue;

                var remarkCell = row.Cells["REMARK"];

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (!dtg.Columns[cell.ColumnIndex].Visible) continue;

                    // Function Equipment บางชนิดไม่มี Serial Number จึงไม่บังคับ EQ_SN
                    if (dtg.Columns[cell.ColumnIndex].Name == "EQUIPMENT_SERIAL") continue;

                    // Skip REMARK column for the general empty check
                    if (cell.ColumnIndex == dtg.Columns["REMARK"].Index) continue;

                    // Check if the cell (excluding REMARK) is empty
                    if (string.IsNullOrWhiteSpace(ValueOf(cell)))
                    {
                        invalidCell = cell;
                        return $"พบเซลล์ว่างในแถวที่ {row.Index + 1} คอลัมน์ {dtg.Columns[cell.ColumnIndex].HeaderText}";
                    }
                }

                // ผล OK/NG ของแถว : ใบที่มีหลายข้อตรวจ คิดจากทุกข้อแบบเดียวกับ CalculateSampleFunctionJudge
                // (JUDGE ในเซลล์ถูกคิดใหม่ตอนกด Enter เท่านั้น ถ้าอ่านจากเซลล์ตรง ๆ จะช้าไปหนึ่งจังหวะ)
                string judge;
                if (checkColumns.Count > 0)
                {
                    List<string> checks = checkColumns.Select(column => ValueOf(row.Cells[column.Index])).ToList();
                    judge = checks.Any(value => value == "0") ? "0"
                        : checks.Any(string.IsNullOrWhiteSpace) ? null
                        : "1";
                }
                else
                {
                    judge = ValueOf(row.Cells["JUDGE"]);
                }

                string remarkText = ValueOf(remarkCell);

                // Specific validation for REMARK when VALUE is "NG"
                if (judge == "0")
                {
                    if (string.IsNullOrWhiteSpace(remarkText))
                    {
                        invalidCell = remarkCell;
                        return $"กรุณากรอก REMARK สำหรับแถวที่ {row.Index + 1} ซึ่งมี VALUE เป็น NG!";
                    }
                }

                // Check REMARK length (applies whether VALUE is "NG" or "OK", if REMARK is not empty)
                if (!string.IsNullOrWhiteSpace(remarkText))
                {
                    if (remarkText.Length > 255)
                    {
                        invalidCell = remarkCell;
                        return $"REMARK ในแถวที่ {row.Index + 1} มีความยาวเกิน 255 ตัวอักษร (ปัจจุบัน: {remarkText.Length} ตัวอักษร)!";
                    }
                }
            }

            return null;
        }

        public void bt_function_Click()
        {
            // หยุด timer ก่อนออกจากหน้า ไม่งั้นมันจะเดินค้างแล้วเด้งใส่ report ใบถัดไป
            StopStatusWatchdog();


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
            // ไม่เก็บร่างให้เอง (เก็บเฉพาะตอนกด Record Data) แต่ถามก่อนถ้ามีค่าที่ยังไม่ได้ Record
            if (!ConfirmLeave()) return;

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
