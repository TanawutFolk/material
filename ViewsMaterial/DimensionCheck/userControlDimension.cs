using Microsoft.Office.Interop.Excel;
using MySqlX.XDevAPI.Relational;
using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.ViewsMaterial.CustomMsg;
using RawMat.ViewsMaterial.PackingCheck;
using RawMat.ViewsMaterial.RegularCheck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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

namespace RawMat.ViewsMaterial.DimensionCheck
{
    public partial class userControlDimension : UserControl, IInspectionScreen
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

        // ร่าง : ปุ่ม tb_record เป็น Record Data ตอนยังกรอกไม่ครบ และเป็น Save and Continue เมื่อครบ (Utilities/InspectionDraft.cs)
        private const string DraftDataTable = "DIMENSION";
        private const string DraftCavityTable = "CAVITY";
        private const string DraftLotNoField = "LOT_NO";
        // key เดียวกับที่ InsertDimensionData ใช้แยกแถวใน db_dimension_data
        private static readonly string[] DraftKeyColumns = { "CAVITY_NAME", "SAMPLING_NO", "POINT_ORDER" };
        private static readonly string[] DraftValueColumns = { "VALUE", "EQUIPMENT_SERIAL" };
        private DataSet _loadedDraft;
        // ค่าบนจอตอนที่ถือว่าเก็บแล้ว (เปิดหน้าเสร็จ / กด Record Data) ใช้เทียบตอนออกว่ามีค่าที่ยังไม่ได้ Record ไหม
        private string _recordedDraft;

        public userControlDimension(IParent parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        // เมธอดสำหรับรับ mutexKey จาก userControlSelectRegular
        public void SetMutexKey(string mutexKey)
        {
            currentMutexKey = mutexKey;
        }

        private void tb_record_Click(object sender, EventArgs e)
        {

            // บันทึกค่าที่กำลังแก้ไขใน DataGridView
            CommitGridEdits();

            // ใบที่ต้อง confirm cavity ก่อนจะยังไม่มีตาราง แต่จำนวนต่อ cavity ที่กรอกไว้ก็ Save เก็บได้
            if (originalDataTable == null || originalDataTable.Rows.Count == 0)
            {
                if (RequiresCavityInput())
                {
                    SaveDraft(true);
                    return;
                }

                MessageBox.Show("ยังไม่พบ data ที่จะทำการ Dimension");
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
            if (!InspectionDraft.ConfirmContinue(this, "Dimension"))
            {
                SaveDraft(true);
                return;
            }

            // เก็บร่างล่าสุดไว้ก่อนส่งผล ถ้าบันทึกพังกลางทางค่าที่กรอกยังอยู่
            // และต้องทำก่อนลูปข้างล่าง เพราะลูปเขียน ID ทับ S/N ใน originalDataTable
            // ถ้า Save หลังจากนั้นร่างจะได้เลข ID แทน S/N
            SaveDraft(false);

            // Lot ที่เลือก ลง db_report_status.Dimension_Check_Lot_No ผ่าน UpdateReportStatusLotNo ข้างล่าง
            // เดิมไม่ได้อ่านจาก combo เลย ใบที่ผ่าน Dimension จึงมี Lot ว่างทุกใบ
            if (!TrySetSelectedLotNo())
            {
                MessageBox.Show("กรุณาเลือก Lot No. ก่อนบันทึก Dimension", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
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

            string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
            try 
            {
                if (conQA.InsertDimensionData(propQA) == true)
                {
                    // ผลจริงลง db_dimension_data แล้ว ร่างไม่ต้องใช้อีก
                    // ถ้าค้างไว้ ใบที่ถูกส่งกลับมาตรวจซ้ำจะโหลดค่ารอบเก่ากลับมาให้
                    conQA.DeleteInspectionDraft(propQA);

                    if (propQA.TOTAL_STATUS == "0")
                    {
                        propQA.inProcStatus = "6";
                        //propQA.reportStatus = "6";
                    }
                    else
                    {
                        propQA.inProcStatus = "1";
                        propQA.reportStatus = "1";
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
                                    "Record Dimension งาน OK เรียบร้อยแล้ว" ,
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

                if (!conQA.DeleteReportActive(propQA))
                {
                    MessageBox.Show("ไม่สามารถคืนสถานะ report no ด้วย ip เครื่องนี้ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                RequestReleaseMutex?.Invoke(mutexKey);
            }
            return;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
            RequestReleaseMutex?.Invoke(mutexKey);
            base.OnHandleDestroyed(e);
        }

        private void userControlDimension_Load(object sender, EventArgs e)
        {
         
            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_lotSize.Text = "Lot Size :" + propQA.Qty;
            //lb_lotNo.Text = "Lot No. : " + propQA.Lot_No;
            lb_lotNo.Text = "Lot No. : ";// + propQA.dtLotNo;
            //Lot No. : XXXXXXXX

            //lb_lotNo.Text = "Lot No. : " + propQA.Lot_No;
            //cb_lotNo.Items.Clear();
            //if (!string.IsNullOrWhiteSpace(propQA.dtLot_No))
            //{
            //    string[] items = propQA.dtLot_No.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //    foreach (string item in items)
            //    {
            //        cb_lotNo.Items.Add(item.Trim()); // เพิ่มแต่ละรายการลงใน ComboBox
            //    }

            //    // ถ้ามีรายการเดียวใน ComboBox ให้เลือกอัตโนมัติ
            //    if (cb_lotNo.Items.Count == 1)
            //    {
            //        cb_lotNo.SelectedIndex = 0; // เลือกรายการแรก (และรายการเดียว) อัตโนมัติ
            //    }
            //    else
            //    {
            //        cb_lotNo.SelectedIndex = -1; // รีเซ็ตถ้ามีมากกว่า 1 รายการ
            //    }

            //}
            //else
            //{
            //    cb_lotNo.SelectedIndex = -1; // รีเซ็ตถ้าไม่มีข้อมูล
            //}

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

            //tb_pageMax.Text = ""; //มาจาก info_regular_sampling 
            //tb_pageCount.Text = ""; // 1 record 2 record จนถึง pageMax
            lb_sampName.Text = propQA.SAMPLING_QTY + " " + propQA.SAMPLING_NAME;

            bool requiresCavityInput = RequiresCavityInput();
            gb_cavity.Visible = requiresCavityInput;
            lb_TotalCavity.Visible = requiresCavityInput;
            bt_confirmCavity.Enabled = requiresCavityInput;
            bt_confirmCavity.TabStop = requiresCavityInput;

            if (requiresCavityInput)
            {
                lb_TotalCavity.Text = "Total Cavity : " + GetTotalCavitySamplingQty();
                gb_cavity.Text = GetCavityRuleText();

                picbox_cavity.Image = imgCls.LoadCavityImage(propQA.M_CODE);
                picbox_dim.Image = imgCls.LoadDimensionImage(propQA.M_CODE);


                dtg_cavity.DataSource = propQA.dtCavity;

                // ห้าม sort : ลำดับ cavity มาจาก master (Cavity_Name เช่น "M,N,O,P") และมีความหมายในตัว
                // ตอนสร้างแถวตัวอย่างโค้ดวน dtg_cavity.Rows ตามลำดับที่แสดงบนจอ ถ้าผู้ใช้เผลอคลิกหัวคอลัมน์
                // SAMPLING_NO จะถูกจับคู่กับ cavity ผิดตัวแล้วบันทึกลงฐานข้อมูลแบบนั้นเลย
                foreach (DataGridViewColumn cavityColumn in dtg_cavity.Columns)
                {
                    cavityColumn.SortMode = DataGridViewColumnSortMode.NotSortable;
                }


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
                // ไม่ต้อง confirm cavity ก็ไม่ต้องเว้นที่ว่างไว้ให้กรอบ ดันรูปขึ้นมาเต็มพื้นที่แทน
                // Designer ตั้ง default เป็น layout แบบมี cavity ไว้ (userControlDimension.Designer.cs:298)
                picbox_dim.Location = new System.Drawing.Point(17, 113);
                picbox_dim.Size = new System.Drawing.Size(1076, 556);

                picbox_dim.Image = imgCls.LoadDimensionImage(propQA.M_CODE);

                GenerateDataTableDimension(null, Convert.ToInt32(propQA.SAMPLING_QTY));

            }

            // ค่าที่เคย Save ไว้ตอนยังกรอกไม่ครบ ดึงกลับมาเติมให้ทำต่อได้
            LoadDraft();

            cb_lotNo.SelectedIndexChanged -= cb_lotNo_SelectedIndexChanged;
            cb_lotNo.SelectedIndexChanged += cb_lotNo_SelectedIndexChanged;
            dtg_dimension.CellBeginEdit -= RequireLotNo_CellBeginEdit;
            dtg_dimension.CellBeginEdit += RequireLotNo_CellBeginEdit;
            dtg_cavity.CellBeginEdit -= RequireLotNo_CellBeginEdit;
            dtg_cavity.CellBeginEdit += RequireLotNo_CellBeginEdit;
            InspectionDraft.HighlightLotNo(cb_lotNo);

            UpdateRecordButtonMode();
            _recordedDraft = InspectionDraft.Serialize(BuildDraft());
        }

        private void cb_lotNo_SelectedIndexChanged(object sender, EventArgs e)
        {
            InspectionDraft.HighlightLotNo(cb_lotNo);
            UpdateRecordButtonMode();
        }

        // ต้องเลือก Lot No. ก่อนเริ่มกรอกช่องใด ๆ ทั้งตารางค่าวัดและกรอบ Cavity (user ขอ 2026-09-29)
        private void RequireLotNo_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (!InspectionDraft.EnsureLotNoSelected(cb_lotNo))
            {
                e.Cancel = true;
            }
        }

        // Lot ที่เลือกบนหัวหน้าจอ ใช้ตอนส่งผล : มีรายการเดียวเลือกให้เอง
        private bool TrySetSelectedLotNo()
        {
            if (cb_lotNo.SelectedItem == null && cb_lotNo.Items.Count == 1)
            {
                cb_lotNo.SelectedIndex = 0;
            }

            propQA.Lot_No = cb_lotNo.SelectedItem?.ToString() ?? string.Empty;
            return !string.IsNullOrWhiteSpace(propQA.Lot_No);
        }

        private void LoadDraft()
        {
            _loadedDraft = InspectionDraft.Load(conQA, propQA);
            if (_loadedDraft == null)
            {
                return;
            }

            string lotNo = InspectionDraft.GetField(_loadedDraft, DraftLotNoField);
            if (!string.IsNullOrWhiteSpace(lotNo) && cb_lotNo.Items.Contains(lotNo))
            {
                cb_lotNo.SelectedItem = lotNo;
            }

            if (RequiresCavityInput() && propQA.dtCavity != null && _loadedDraft.Tables.Contains(DraftCavityTable))
            {
                InspectionDraft.Restore(propQA.dtCavity, _loadedDraft.Tables[DraftCavityTable],
                    new[] { "CAVITY_NAME" }, new[] { "SAMPLING_QTY" });

                // มีตารางค่าวัดในร่างแปลว่าเคย confirm cavity ไปแล้ว ทำให้เหมือนกด confirm อีกครั้ง
                // ถ้ายอดรวมไม่ตรงแล้ว (master เปลี่ยน) ปล่อยให้ผู้ตรวจแก้แล้วกด confirm เอง ค่าวัดจะเติมให้ตอนนั้น
                bool hadDataTable = _loadedDraft.Tables.Contains(DraftDataTable)
                    && _loadedDraft.Tables[DraftDataTable].Rows.Count > 0;

                if (hadDataTable && IsCavityInputComplete())
                {
                    dtg_cavity.ReadOnly = true;
                    GenerateDataTableDimension(dtg_cavity, 0);
                }
            }

            ApplyDraftValues();
        }

        private void ApplyDraftValues()
        {
            if (_loadedDraft == null || originalDataTable == null || !_loadedDraft.Tables.Contains(DraftDataTable))
            {
                return;
            }

            // เขียนทีละแถวแล้วให้ตารางวาดใหม่ทุกครั้งจะช้าและเคอร์เซอร์เด้ง ปิด event แล้ววาดทีเดียวตอนจบ
            bool raiseEvents = bindingSource.RaiseListChangedEvents;
            bindingSource.RaiseListChangedEvents = false;

            try
            {
                InspectionDraft.Restore(originalDataTable, _loadedDraft.Tables[DraftDataTable], DraftKeyColumns, DraftValueColumns);
                RecalculateJudges();
                EquipmentSerialColumn.AddMissingItems(dtg_dimension);
            }
            finally
            {
                bindingSource.RaiseListChangedEvents = raiseEvents;
            }

            // DataBindingComplete จะตั้งช่อง OK/NG และสีแถว NG ให้ใหม่
            bindingSource.ResetBindings(false);

            // เติมครั้งเดียวพอ ถ้ากด confirm cavity ซ้ำไม่ควรได้ค่าจากร่างเก่ากลับมาทับ
            _loadedDraft = null;
        }

        // ค่าที่เติมจากร่างไม่ได้ผ่าน CellEndEdit จึงต้องคิดผลตัดสินเองด้วยเกณฑ์เดียวกัน
        private void RecalculateJudges()
        {
            foreach (DataRow row in originalDataTable.Rows)
            {
                if (!TryParseDecimal(row["VALUE"], out decimal value)
                    || !TryParseDecimal(row["CRITERIA_MIN"], out decimal min)
                    || !TryParseDecimal(row["CRITERIA_MAX"], out decimal max))
                {
                    continue;
                }

                row["POINT_JUDGE"] = (value >= min && value <= max) ? "1" : "0";
            }

            CalculatePieceDifference();
            CalculateTotalJudge();
        }

        private DataSet BuildDraft()
        {
            var draft = new DataSet("DRAFT");

            if (originalDataTable != null)
            {
                draft.Tables.Add(InspectionDraft.Snapshot(originalDataTable, DraftDataTable,
                    "CAVITY_NAME", "SAMPLING_NO", "POINT_ORDER", "VALUE", "EQUIPMENT_SERIAL"));
            }

            if (RequiresCavityInput() && propQA.dtCavity != null)
            {
                draft.Tables.Add(InspectionDraft.Snapshot(propQA.dtCavity, DraftCavityTable, "CAVITY_NAME", "SAMPLING_QTY"));
            }

            InspectionDraft.SetField(draft, DraftLotNoField, cb_lotNo.SelectedItem?.ToString());

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
                InspectionDraft.ShowSaved(DescribeEmptyInputCell());
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
            bool unrecorded = dtg_dimension.IsCurrentCellDirty || dtg_cavity.IsCurrentCellDirty
                || InspectionDraft.Serialize(BuildDraft()) != _recordedDraft;

            return !unrecorded || InspectionDraft.ConfirmLeaveUnrecorded(this, "Dimension");
        }

        private void CommitGridEdits()
        {
            if (dtg_dimension.IsCurrentCellDirty || dtg_dimension.IsCurrentRowDirty)
            {
                dtg_dimension.EndEdit(); // จบการแก้ไขเซลล์ปัจจุบัน
                dtg_dimension.CommitEdit(DataGridViewDataErrorContexts.Commit); // บันทึกค่าลง DataSource
                bindingSource.EndEdit(); // บันทึกค่าลงใน BindingSource (ถ้าใช้)
            }

            if (dtg_cavity.IsCurrentCellDirty)
            {
                dtg_cavity.EndEdit();
            }
        }

        // ครบ = เลือก Lot แล้ว สร้างตารางแล้ว และทุกช่องที่ผู้ตรวจต้องกรอกมีค่า
        private bool IsInputComplete()
        {
            return InspectionDraft.HasLotNo(cb_lotNo)
                && originalDataTable != null
                && originalDataTable.Rows.Count > 0
                && !TryFindEmptyInputCell(originalDataTable, out _, out _);
        }

        private void UpdateRecordButtonMode()
        {
            InspectionDraft.ApplyButtonMode(tb_record, IsInputComplete());
        }

        // เงื่อนไขเดียวกับ bt_confirmCavity_Click แต่ไม่เด้งข้อความ ใช้ตอนเติมจากร่าง
        private bool IsCavityInputComplete()
        {
            int totalQty = 0;

            foreach (DataRow row in propQA.dtCavity.Rows)
            {
                if (string.IsNullOrWhiteSpace(row["CAVITY_NAME"]?.ToString())
                    || !int.TryParse(row["SAMPLING_QTY"]?.ToString(), out int qty))
                {
                    return false;
                }

                totalQty += qty;
            }

            return totalQty == GetTotalCavitySamplingQty();
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
                    //MessageBox.Show("กรุณากรอกค่า ห้ามปล่อยว่าง", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //e.Cancel = true; // ไม่ให้ผู้ใช้เปลี่ยนแปลงค่า
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
            // Regex pattern: ^-?\d+(\.\d+)?(-)?$
            // -? : เครื่องหมายลบ "-" หน้าตัวเลข (มีหรือไม่มีก็ได้)
            // \d+ : ตัวเลข 1 หลักขึ้นไป
            // (\.\d+)? : จุดทศนิยมและตัวเลข 1 หลักขึ้นไป (มีหรือไม่มีก็ได้)
            // -? : เครื่องหมายลบ "-" หลังตัวเลข (มีหรือไม่มีก็ได้)
            // $ : สิ้นสุดสตริง


            string pattern = @"^-?\d+(\.\d+)?(-)?$";
            return Regex.IsMatch(input, pattern);
        }

        // S/N ที่มีอยู่แล้วในระบบ โหลดครั้งเดียวแล้วใช้ทุกแถว
        private DataTable _equipmentSerials;

        private void ApplyEquipmentSerialDropdown()
        {
            if (_equipmentSerials == null)
            {
                _equipmentSerials = conQA.EquipmentSerialList();
            }

            EquipmentSerialColumn.Apply(dtg_dimension, _equipmentSerials);
        }

        private void dtg_dimension_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            EquipmentSerialColumn.HandleEditingControlShowing(dtg_dimension, e);
            InspectionDraft.WatchEditingControl(e, EditingValue_Changed);
        }

        // พิมพ์หรือเลือกค่าในตาราง : เปลี่ยนปุ่มทันที ไม่ต้องรอกด Enter
        private void EditingValue_Changed(object sender, EventArgs e)
        {
            UpdateRecordButtonMode();
        }

        private void dtg_dimension_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            EquipmentSerialColumn.HandleDataError(dtg_dimension, e);
        }

        private void dtg_dimension_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE")
            {
                DataGridViewRow row = dtg_dimension.Rows[e.RowIndex];

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

                    if (TryParseDecimal(row.Cells["VALUE"].Value, out value))
                    {
                        // คำนวณ Point_Judge (1 ถ้าอยู่ในช่วง min-max, 0 ถ้านอกช่วง)
                        row.Cells["POINT_JUDGE"].Value = (value >= min && value <= max) ? 1 : 0;
                    }
                    else
                    {
                        row.Cells["POINT_JUDGE"].Value = DBNull.Value; // ถ้าค่าไม่ถูกต้อง ให้เป็นค่าว่าง
                    }

                    // ผลต่างต้องคิดใหม่ทั้งชิ้น เพราะแก้จุดเดียว MAX/MIN ก็เปลี่ยนได้
                    CalculatePieceDifference();

                    // คำนวณ Total_Judge
                    CalculateTotalJudge();
                }

                // แสดงผล NG ให้ผู้ตรวจเห็นทันทีหลังจบการกรอก แม้เซลล์นั้นยังถูกเลือกอยู่
                UpdateDimensionRowColor(row);
            }

            // ตรวจสอบว่ากำลังแก้ไข COLUMN "EQUIPMENT_SERIAL" หรือไม่
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

            UpdateRecordButtonMode();

        }

        private static void UpdateDimensionRowColor(DataGridViewRow row)
        {
            bool hasValue = row.Cells["VALUE"].Value != null &&
                !DBNull.Value.Equals(row.Cells["VALUE"].Value) &&
                !string.IsNullOrWhiteSpace(row.Cells["VALUE"].Value.ToString());
            bool isNg = hasValue && row.Cells["POINT_JUDGE"].Value?.ToString() == "0";

            // ใช้ Color.Empty ตอนคืนค่า เพื่อให้แถวกลับไปรับสีปกติ/สีสลับแถวจาก DataGridView
            row.DefaultCellStyle.BackColor = isNg ? Color.Red : Color.Empty;
            row.DefaultCellStyle.SelectionBackColor = isNg ? Color.Red : Color.Empty;
            row.DefaultCellStyle.SelectionForeColor = isNg ? Color.Black : Color.Empty;
        }

        private void dtg_dimension_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (!dtg_dimension.Columns.Contains("VALUE") ||
                !dtg_dimension.Columns.Contains("POINT_JUDGE"))
            {
                return;
            }

            foreach (DataGridViewRow row in dtg_dimension.Rows)
            {
                if (!row.IsNewRow)
                {
                    ConfigureDimensionValueCell(row);
                    UpdateDimensionRowColor(row);
                }
            }
        }

        private static void ConfigureDimensionValueCell(DataGridViewRow row)
        {
            if (row?.DataGridView == null ||
                !row.DataGridView.Columns.Contains("VALUE") ||
                !row.DataGridView.Columns.Contains("CRITERIA_MIN"))
            {
                return;
            }

            DataGridViewCell currentCell = row.Cells["VALUE"];
            object currentValue = currentCell.Value;

            if (Utilities.PointJudgeType.IsPassFail(row))
            {
                if (currentCell is DataGridViewComboBoxCell ||
                    !TryParseDecimal(row.Cells["CRITERIA_MIN"].Value, out decimal minValue))
                {
                    return;
                }

                string okValue = minValue.ToString(CultureInfo.InvariantCulture);
                string ngValue = (minValue - 1m).ToString(CultureInfo.InvariantCulture);
                var choices = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>(string.Empty, string.Empty),
                    new KeyValuePair<string, string>(ngValue, "NG"),
                    new KeyValuePair<string, string>(okValue, "OK")
                };

                string existingValue = currentValue == null || currentValue == DBNull.Value
                    ? string.Empty
                    : currentValue.ToString();

                if (!string.IsNullOrWhiteSpace(existingValue) &&
                    !choices.Any(choice => choice.Key == existingValue))
                {
                    choices.Add(new KeyValuePair<string, string>(existingValue, existingValue));
                }

                var comboBoxCell = new DataGridViewComboBoxCell
                {
                    DataSource = choices,
                    ValueMember = "Key",
                    DisplayMember = "Value",
                    Value = existingValue
                };

                // อ้างด้วยชื่อคอลัมน์เสมอ ป้องกัน index เคลื่อนเมื่อ EQ_SN ถูกเปลี่ยนเป็น ComboBoxColumn
                row.Cells["VALUE"] = comboBoxCell;
                return;
            }

            if (currentCell is DataGridViewComboBoxCell)
            {
                row.Cells["VALUE"] = new DataGridViewTextBoxCell { Value = currentValue };
            }
        }

        /// <summary>
        /// แบบ All วัดจุดเดียวกันหลายตำแหน่งบนชิ้นเดียว จึงต้องดูความสม่ำเสมอภายในชิ้นด้วย
        /// ทุกจุดอยู่ในช่วง MIN-MAX แล้วยังไม่พอ ผลต่างสูงสุด-ต่ำสุดต้องไม่เกินเกณฑ์
        /// (เกณฑ์เดียวกับใบจริง QA26-002 ที่เขียนว่า Dif (MAX-MIN) ≤ 0.2032 mm)
        /// </summary>
        private void CalculatePieceDifference()
        {
            if (!IsAllDimensionMode()) return;
            if (originalDataTable == null) return;
            if (!originalDataTable.Columns.Contains("PIECE_JUDGE")) return;
            if (_isCalculatingDifference) return;

            // การเขียนค่าลง DataTable จะปลุก ListChanged ของ BindingSource
            // ซึ่งกรองอยู่ (Filter = POINT_ORDER) พอ refresh ตารางจะเด้งไปแถวอื่น
            // ทำให้ตัวเลขที่พิมพ์ต่อไปลงผิดแถว ยิ่งพิมพ์หลายหลักยิ่งมีจังหวะให้เด้ง
            // จึงต้องปิด event ระหว่างเขียน แล้วค่อยวาดใหม่ทีเดียวตอนจบ
            _isCalculatingDifference = true;

            DataGridViewCell currentCell = dtg_dimension.CurrentCell;
            bool raiseEvents = bindingSource.RaiseListChangedEvents;
            bindingSource.RaiseListChangedEvents = false;

            try
            {
                // CellEndEdit จบเฉพาะ editor ของ DataGridView แต่ DataRowView อาจยังอยู่ในสถานะ edit
                // ต้อง commit ก่อน ไม่เช่นนั้นการไล่อ่าน originalDataTable อาจยังเห็นค่าเดิม
                // ทำให้ MAX และ MIN เท่ากันและ DIFF แสดงเป็น 0
                bindingSource.EndEdit();
                CalculatePieceDifferenceCore();
            }
            finally
            {
                bindingSource.RaiseListChangedEvents = raiseEvents;
                _isCalculatingDifference = false;

                // วาดใหม่โดยไม่ rebind ตำแหน่งเคอร์เซอร์จะได้ไม่ขยับ
                dtg_dimension.Invalidate();

                if (currentCell != null && !currentCell.IsInEditMode
                    && currentCell.RowIndex < dtg_dimension.Rows.Count
                    && dtg_dimension.CurrentCell != currentCell)
                {
                    dtg_dimension.CurrentCell = currentCell;
                }
            }
        }

        private bool _isCalculatingDifference;

        private void CalculatePieceDifferenceCore()
        {

            // ต้องอ่านจาก DataTable ไม่ใช่จากตารางบนหน้าจอ
            // เพราะหน้าจอกรองไว้ทีละจุด (ShowPage ตั้ง Filter = POINT_ORDER)
            // ถ้าอ่านจากหน้าจอจะเห็นแค่จุดเดียวต่อชิ้น MAX-MIN เลยได้ 0 เสมอ
            var pieces = originalDataTable.Rows
                .Cast<DataRow>()
                .GroupBy(row => row["SAMPLING_NO"]?.ToString());

            foreach (var piece in pieces)
            {
                // จุดที่ตัดสิน OK/NG (MIN = MAX) ไม่ใช่ค่าที่วัดออกมาเป็นตัวเลข
                // เก็บลง DB เป็น 0/1 ถ้าเอามาคิดผลต่างด้วยจะลากช่วงเพี้ยนไปทั้งชิ้น
                // เช่นจุดวัดจริง 6.20-6.45 ปนกับ 1 จาก OK/NG จะได้ผลต่าง 5.45 ตกทุกครั้ง
                List<DataRow> rows = piece.ToList();
                List<DataRow> measurable = rows.Where(IsMeasurablePoint).ToList();
                var values = new List<decimal>();

                foreach (DataRow row in measurable)
                {
                    if (TryParseDecimal(row["VALUE"], out decimal value))
                    {
                        values.Add(value);
                    }
                }

                // ผลต่างต้องมีอย่างน้อย 2 จุดถึงจะมีความหมาย
                // ถ้ามีจุดเดียวแล้วโชว์ 0 จะอ่านเหมือนว่าผลต่างเป็นศูนย์คือดีมาก
                // ทั้งที่ยังวัดไม่พอจะบอกอะไรได้
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

                // เกณฑ์ต้องมาจากจุดที่วัดเป็นตัวเลข ถ้าหยิบจุด OK/NG มาจะได้ (1-1)/2 = 0
                // แล้วเงื่อนไข tolerance > 0 จะทำให้ไม่ตัดสินอะไรเลย
                decimal tolerance = GetDifferenceTolerance(measurable[0]);
                bool complete = values.Count == measurable.Count;

                // ผลตัดสิน : ยิ่งกรอกเพิ่ม ผลต่างมีแต่จะกว้างขึ้น ลดลงไม่ได้
                // เกินเกณฑ์เมื่อไหร่จึงฟันธง NG ได้ทันทีโดยไม่ต้องรอครบ
                // แต่จะบอกว่าผ่านต้องรอครบทุกจุดก่อน เพราะจุดที่ยังไม่กรอกอาจดันให้เกิน
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
                    // ผลต่างโชว์ตั้งแต่กรอกไม่ครบ ผู้ตรวจจะได้เห็นว่ากว้างขึ้นแค่ไหนระหว่างทาง
                    row["DIFFERENCE"] = difference;
                    row["TOLERANCE"] = tolerance;
                    row["PIECE_JUDGE"] = judge == null ? (object)DBNull.Value : judge;
                }
            }
        }

        /// <summary>เกณฑ์ผลต่างที่ยอมได้ = ครึ่งหนึ่งของช่วง MIN-MAX เช่น 6.1468~6.5532 ได้ 0.2032</summary>
        /// <summary>
        /// จุดที่ตัดสินผ่าน/ไม่ผ่านด้วย Jig หรือ Gauge ไม่ใช่ค่าที่วัดได้ จึงเอามาคิดผลต่างไม่ได้
        /// หน้าจอก็ใช้ค่าเดียวกันนี้สลับช่องกรอกเป็น Dropdown OK/NG (CellFormatting)
        /// </summary>
        private static bool IsMeasurablePoint(DataRow row)
        {
            return Utilities.PointJudgeType.IsNumeric(row);
        }

        /// <summary>ค่าตั้งจากหน้า Setting ไม่มีก็ถือว่าวัดเป็นตัวเลขตามค่าเริ่มต้นของคอลัมน์</summary>
        private static string GetJudgeType(DataRow measureRow)
        {
            if (!measureRow.Table.Columns.Contains(Utilities.PointJudgeType.ColumnName))
            {
                return Utilities.PointJudgeType.Numeric.ToString(CultureInfo.InvariantCulture);
            }

            return measureRow[Utilities.PointJudgeType.ColumnName]?.ToString();
        }

        private static decimal GetDifferenceTolerance(DataRow row)
        {
            // ใช้ decimal ไม่ใช้ double เพราะ 6.5532 - 6.1468 ด้วย double ได้ 0.40640000000000053
            // แล้วเคสที่ผลต่างเท่าเกณฑ์พอดีจะตัดสินพลาด
            if (!TryParseDecimal(row["CRITERIA_MIN"], out decimal min)) return 0;
            if (!TryParseDecimal(row["CRITERIA_MAX"], out decimal max)) return 0;

            return (max - min) / 2m;
        }

        private static bool TryParseDecimal(object rawValue, out decimal value)
        {
            string text = rawValue == null || rawValue == DBNull.Value
                ? string.Empty
                : rawValue.ToString().Trim();

            // ช่องกรอกรับจุดทศนิยม จึงอ่านแบบ Invariant ก่อน แล้วค่อยรองรับ Culture ของเครื่อง
            return decimal.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value)
                || decimal.TryParse(text, NumberStyles.Number, CultureInfo.CurrentCulture, out value);
        }

        private bool IsAllDimensionMode()
        {
            return propQA.SAMPLING_TYPE == "1";
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

                // แบบ All ผลต่างเกินเกณฑ์ก็ตกทั้งใบเหมือนกัน
                if (originalDataTable.Columns.Contains("PIECE_JUDGE")
                    && dtRow["PIECE_JUDGE"] != null
                    && dtRow["PIECE_JUDGE"].ToString() == "0")
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

        // ใบที่ต้องให้ผู้ตรวจกระจายจำนวนลงแต่ละ cavity ก่อน ถึงจะสร้างตารางกรอกค่าได้
        // เกณฑ์เดียวกับ Regular (userControlRegular.cs:777) และ Function (userControlFunction.cs:191)
        // type 4 = แบ่งตาม cavity เสมอ , type 3 = แบ่งเมื่อ master ตั้ง Cavity_Qty ไว้
        private bool RequiresCavityInput()
        {
            int.TryParse(propQA.CAVITY_QTY?.Trim(), out int cavityQty);
            return propQA.SAMPLING_TYPE == "4"
                || (propQA.SAMPLING_TYPE == "3" && cavityQty > 0);
        }

        // ยอดรวมที่ต้องเก็บทั้งใบ = ผลรวมของ SAMPLING_QTY ทุก cavity
        // Type 4 : Sampling_Qty ใน master เป็นจำนวน "ต่อ cavity" ต้องคูณจำนวน cavity เอง
        // Type 3 : ยกพื้นเป็น cavityQty x qtyPerCavity ไปแล้วตอนเลือกใบ (userControlSelectDimension.cs:334) ห้ามคูณซ้ำ
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
            if (!RequiresCavityInput())
            {
                return;
            }

            if (!InspectionDraft.EnsureLotNoSelected(cb_lotNo))
            {
                return;
            }

            // ค่าที่ยังค้างอยู่ในช่องที่กำลังพิมพ์ยังไม่ถูก commit ลง cell ถ้าไม่ปิด edit ก่อน
            // จะถูกนับเป็นช่องว่างแล้วเด้ง warning ทั้งที่ผู้ใช้กรอกครบแล้ว (เหมือน userControlRegular.cs:738)
            dtg_cavity.EndEdit();

            // ตรวจสอบว่าไม่มี Cell ว่าง
            foreach (DataGridViewRow row in dtg_cavity.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        MessageBox.Show("กรุณากรอกข้อมูลให้ครบทุกช่อง!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

            }

            // คำนวณผลรวมของ QTY
            int totalQty = 0;
            foreach (DataGridViewRow row in dtg_cavity.Rows)
            {

                if (int.TryParse(row.Cells["SAMPLING_QTY"].Value?.ToString(), out int qty))
                {
                    totalQty += qty;
                }

            }

            int expectedQty = GetTotalCavitySamplingQty();

            // ตรวจสอบว่าผลรวมตรงกับที่ต้องการ
            if (totalQty != expectedQty)
            {
                MessageBox.Show($"ผลรวมของ QTY ต้องได้ {expectedQty}  (ปัจจุบัน: {totalQty})", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // ล็อก dtg_data ไม่ให้แก้ไขข้อมูล
            dtg_cavity.ReadOnly = true;


            GenerateDataTableDimension(dtg_cavity, 0);

            // ร่างที่ยอดรวม cavity ไม่ตรงตอนเปิดหน้า จะเติมค่าวัดให้ตอนนี้แทน
            ApplyDraftValues();

            UpdateRecordButtonMode();
        }

        // ฟังก์ชันแสดงเฉพาะแถวที่เป็น POINT_ORDER ของหน้าปัจจุบัน
        private void ShowPage(int page)
        {
            bindingSource.Filter = $"POINT_ORDER = '{page}'"; // กรองเฉพาะแถวที่มี POINT_ORDER ตรงกับหน้า
            lb_page.Text = $"{page}/{totalPages}"; // แสดงหน้า (1/8)
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
                // ตรวจสอบว่าเป็นตัวเลขหรือไม่
                if (!int.TryParse(e.FormattedValue.ToString(), out _))
                {
                    e.Cancel = true; // ยกเลิกการออกจากเซลล์โดยไม่แสดงข้อความ
                }
            }
            else if (e.ColumnIndex == dtg_cavity.Columns["CAVITY_NAME"].Index)
            {
                string value = e.FormattedValue.ToString().Trim().ToUpper(); // แปลงเป็นตัวพิมพ์ใหญ่

                // ตรวจสอบความยาว 1 ตัว และเป็นตัวเลขหรือตัวอักษร

                if (value == "")
                {
                    return;
                }

                // ตรวจสอบความยาว 1 ตัว และต้องเป็นตัวอักษรภาษาอังกฤษหรือตัวเลข
                if (value.Length != 1 || !Regex.IsMatch(value, "^[A-Z0-9]$"))
                {
                    MessageBox.Show("กรุณากรอกตัวอักษรภาษาอังกฤษหรือเลข 0-9 เท่านั้น และต้องมีความยาว 1 ตัว", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true;
                    return;
                }

                // ตรวจสอบค่าซ้ำในคอลัมน์ Name
                for (int i = 0; i < dtg_cavity.Rows.Count; i++)
                {
                    if (i != e.RowIndex) // ข้ามแถวที่กำลังแก้ไข
                    {
                        var cell = dtg_cavity.Rows[i].Cells["CAVITY_NAME"].Value;
                        string existingValue = cell != null ? cell.ToString().Trim().ToUpper() : "";

                        // ข้ามแถวที่ไม่มีค่า
                        if (string.IsNullOrEmpty(existingValue))
                            continue;

                        if (existingValue == value)
                        {
                            MessageBox.Show("ค่าซ้ำ! กรุณากรอกค่าที่ไม่ซ้ำกัน", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            e.Cancel = true;
                            return;
                        }
                    }
                }

                // ตั้งค่าค่าใหม่ให้เป็นตัวพิมพ์ใหญ่
                //dtg_cavity.Rows[e.RowIndex].Cells["CAVITY_NAME"].Value = value;

            }


        }

        private void GenerateDataTableDimension(DataGridView dtgCavity, int sampQty)
        {
            dtg_dimension.CellEndEdit -= dtg_dimension_CellEndEdit;
            dtg_dimension.CellValidating -= dtg_dimension_CellValidating;
           
            dtg_dimension.DataBindingComplete -= dtg_dimension_DataBindingComplete;
            dtg_dimension.DataBindingComplete += dtg_dimension_DataBindingComplete;

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
            dtAllSum.Columns.Add("CRITERIA_MIN", typeof(decimal));
            dtAllSum.Columns.Add("CRITERIA_MAX", typeof(decimal));
            dtAllSum.Columns.Add("JUDGE_TYPE", typeof(string));
            dtAllSum.Columns.Add("POINT_JUDGE", typeof(string));
            dtAllSum.Columns.Add("TOTAL_JUDGE", typeof(string));
            dtAllSum.Columns.Add("DIFFERENCE", typeof(decimal));
            dtAllSum.Columns.Add("TOLERANCE", typeof(decimal));
            dtAllSum.Columns.Add("PIECE_JUDGE", typeof(string));

            int qtySampCounter = 1;

            if (dtgCavity == null)
            {
                for (int i = 0; i < sampQty; i++)
                {
                    int qtySampNo = qtySampCounter++;

                    foreach (DataRow measureRow in propQA.dtDimEq.Rows)
                    {
                        dtAllSum.Rows.Add(
                            qtySampNo,
                            measureRow["POINT_ORDER"].ToString(),
                            measureRow["POINT_CAL"].ToString(),
                            null,
                            measureRow["EQUIPMENT_TYPE"].ToString(),
                            measureRow["EQUIPMENT_NAME"].ToString(),
                            measureRow["POINT_NAME"].ToString(),
                            null,
                            Convert.ToDecimal(measureRow["CRITERIA_MIN"]),
                            Convert.ToDecimal(measureRow["CRITERIA_MAX"]),
                            GetJudgeType(measureRow),
                            null, null, null, null, null
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
                            dtAllSum.Rows.Add(
                                name,
                                qtySampNo,
                                measureRow["POINT_ORDER"].ToString(),
                                measureRow["POINT_CAL"].ToString(),
                                null,
                                measureRow["EQUIPMENT_TYPE"].ToString(),
                                measureRow["EQUIPMENT_NAME"].ToString(),
                                measureRow["POINT_NAME"].ToString(),
                                null,
                                Convert.ToDecimal(measureRow["CRITERIA_MIN"]),
                                Convert.ToDecimal(measureRow["CRITERIA_MAX"]),
                                GetJudgeType(measureRow),
                                null, null, null, null, null
                            );
                        }
                    }
                }
            }

            dtg_dimension.DataSource = dtAllSum;

            // ซ่อนคอลัมน์ที่ไม่ต้องการแสดง
            string[] hiddenColumns = { "POINT_CAL", "POINT_ORDER", "EQUIPMENT_TYPE", "POINT_JUDGE", "TOTAL_JUDGE", "PIECE_JUDGE", "TOLERANCE", "JUDGE_TYPE" };
            foreach (var col in hiddenColumns)
            {
                if (dtg_dimension.Columns.Contains(col))
                {
                    dtg_dimension.Columns[col].Visible = false;
                }
            }

            // บันทึกข้อมูลต้นฉบับ
            originalDataTable = (DataTable)dtg_dimension.DataSource;
            bindingSource.DataSource = originalDataTable;
            dtg_dimension.DataSource = bindingSource;

            // ทำให้คอลัมน์ที่ไม่ใช่ "VALUE" และ "EQUIPMENT_SERIAL" เป็น ReadOnly
            foreach (DataGridViewColumn column in dtg_dimension.Columns)
            {
                column.ReadOnly = (column.Name != "VALUE" && column.Name != "EQUIPMENT_SERIAL");
            }

            // เปลี่ยน HeaderText
            if (dtg_dimension.Columns.Contains("CAVITY_NAME")) dtg_dimension.Columns["CAVITY_NAME"].HeaderText = "CAV.";
            if (dtg_dimension.Columns.Contains("SAMPLING_NO")) dtg_dimension.Columns["SAMPLING_NO"].HeaderText = "SAMPLE";
            if (dtg_dimension.Columns.Contains("POINT_NAME")) dtg_dimension.Columns["POINT_NAME"].HeaderText = "CHECKPOINT";
            if (dtg_dimension.Columns.Contains("EQUIPMENT_SERIAL")) dtg_dimension.Columns["EQUIPMENT_SERIAL"].HeaderText = "EQ_SN";
            if (dtg_dimension.Columns.Contains("EQUIPMENT_NAME"))
            {
                dtg_dimension.Columns["EQUIPMENT_NAME"].HeaderText = "EQ_NAME";
                // ชื่อเครื่องมือยาวกว่าหัวคอลัมน์มาก เช่น Vernier Caliper / Pin Gauge NO GO
                dtg_dimension.Columns["EQUIPMENT_NAME"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
                dtg_dimension.Columns["EQUIPMENT_NAME"].MinimumWidth = 150;
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
            // ต้องโชว์ให้เห็น ไม่งั้นผู้ตรวจจะงงว่าทำไมทุกจุดอยู่ในเกณฑ์แล้วยังไม่ผ่าน
            if (dtg_dimension.Columns.Contains("DIFFERENCE"))
            {
                dtg_dimension.Columns["DIFFERENCE"].Visible = IsAllDimensionMode();
                dtg_dimension.Columns["DIFFERENCE"].HeaderText = "DIFF";
                dtg_dimension.Columns["DIFFERENCE"].DefaultCellStyle.Format = NumberDisplay.GridFormat;
            }

            ApplyEquipmentSerialDropdown();

            // DataBindingComplete อาจเกิดก่อนที่ EQ_SN จะถูกแทนด้วย ComboBoxColumn
            // ย้ำชนิดเซลล์ VALUE หลังจัดคอลัมน์เสร็จ โดยอ้างชื่อคอลัมน์ ไม่ใช้ index
            foreach (DataGridViewRow row in dtg_dimension.Rows)
            {
                if (!row.IsNewRow)
                {
                    ConfigureDimensionValueCell(row);
                }
            }

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

            // คำนวณจำนวน POINT_ORDER ที่มีทั้งหมด
            totalPages = originalDataTable.AsEnumerable()
                .Select(row => row["POINT_ORDER"].ToString())
                .Distinct()
                .Count();

            ShowPage(currentPage);

            dtg_dimension.CellEndEdit += dtg_dimension_CellEndEdit;
            dtg_dimension.CellValidating += dtg_dimension_CellValidating;
            dtg_dimension.EditingControlShowing += dtg_dimension_EditingControlShowing;
            dtg_dimension.DataError += dtg_dimension_DataError;
           // dtg_regular.EditingControlShowing += dtg_regular_EditingControlShowing;

        }




        // บอกผู้ตรวจตอน Save ว่ายังขาดช่องไหน จะได้กลับมาทำต่อถูกจุด คืน null ถ้าครบแล้ว
        private string DescribeEmptyInputCell()
        {
            // ต้องเลือก Lot ก่อนถึงจะกรอกช่องอื่นได้ จึงบอกเรื่อง Lot ก่อน
            if (!InspectionDraft.HasLotNo(cb_lotNo))
            {
                return InspectionDraft.MissingLotNoText;
            }

            if (originalDataTable == null)
            {
                return InspectionDraft.MissingCavityText;
            }

            if (!TryFindEmptyInputCell(originalDataTable, out DataRow row, out DataColumn column))
            {
                return null;
            }

            return InspectionDraft.DescribeMissingMeasurement(originalDataTable, row, column, totalPages);
        }

        // หาช่องแรกที่ผู้ตรวจยังไม่ได้กรอก ใช้ทั้งตอนเตือนก่อนบันทึก และตอนตัดสินว่าปุ่มเป็น Record Data หรือ Save and Continue
        // ช่องที่กำลังพิมพ์อยู่ใช้ค่าที่พิมพ์ (GetValueAfterEdit) ปุ่มจะได้เปลี่ยนทันทีโดยไม่ต้องกด Enter
        private bool TryFindEmptyInputCell(DataTable table, out DataRow emptyRow, out DataColumn emptyColumn)
        {
            foreach (DataRow row in table.Rows)
            {
                foreach (DataColumn column in table.Columns)
                {
                    // คอลัมน์ที่โปรแกรมคำนวณให้เอง ไม่ใช่ช่องที่ผู้ตรวจต้องกรอก
                    // ว่างได้ตามปกติ เช่นโหมดที่ไม่ใช่ All จะไม่มีค่าผลต่างเลย
                    if (IsCalculatedColumn(column.ColumnName))
                    {
                        continue;
                    }

                    object value = InspectionDraft.GetValueAfterEdit(dtg_dimension, row, column.ColumnName);

                    if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
                    {
                        emptyRow = row;
                        emptyColumn = column;
                        return true;
                    }
                }
            }

            emptyRow = null;
            emptyColumn = null;
            return false;
        }

        private static bool IsCalculatedColumn(string columnName)
        {
            return columnName == "POINT_JUDGE"
                || columnName == "TOTAL_JUDGE"
                || columnName == "DIFFERENCE"
                || columnName == "TOLERANCE"
                || columnName == "PIECE_JUDGE";
        }

        private bool IsDataGridViewValid(DataGridView dtg)
        {
            foreach (DataGridViewRow row in dtg.Rows)
            {
                // ข้ามแถวใหม่ที่ยังไม่เพิ่มข้อมูล (AllowUserToAddRows = true)
                if (row.IsNewRow) continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (IsCalculatedColumn(dtg.Columns[cell.ColumnIndex].Name))
                    {
                        continue;
                    }

                    // ตรวจสอบค่าในเซลล์
                    if (cell.Value == null || string.IsNullOrWhiteSpace(cell.Value.ToString()))
                    {
                        MessageBox.Show($"พบเซลล์ว่างในแถวที่ {row.Index + 1} คอลัมน์ {dtg.Columns[cell.ColumnIndex].HeaderText}", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dtg.CurrentCell = cell; // ตั้งค่าให้เซลล์ที่ว่างเป็น Active
                        return false;
                    }
                }
            }
            return true;
        }

        private void dtg_cavity_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (dtg_cavity.CurrentCell.ColumnIndex == dtg_cavity.Columns["SAMPLING_QTY"].Index ||
                dtg_cavity.CurrentCell.ColumnIndex == dtg_cavity.Columns["CAVITY_NAME"].Index)
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

            var textBox = sender as TextBox;
            int columnIndex = dtg_cavity.CurrentCell.ColumnIndex;

            if (columnIndex == dtg_cavity.Columns["SAMPLING_QTY"].Index)
            {
                // อนุญาตเฉพาะตัวเลขและปุ่มควบคุม (เช่น Backspace, Delete)
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true; // ยกเลิกอักขระที่ไม่ใช่ตัวเลข
                }
            }
            // สำหรับคอลัมน์ CAVITY_NAME
            else if (columnIndex == dtg_cavity.Columns["CAVITY_NAME"].Index)
            {
                // อนุญาตแค่ตัวควบคุม (เช่น Backspace)
                if (char.IsControl(e.KeyChar)) return;

                // อนุญาตเฉพาะตัวเลข (0-9) และตัวอักษรภาษาอังกฤษ (A-Z, a-z)
                if (!(char.IsDigit(e.KeyChar) || (char.IsLetter(e.KeyChar) && e.KeyChar <= 127)))
                {
                    e.Handled = true;
                    return;
                }

                // ตรวจสอบว่าข้อความมีความยาวเกิน 1 ตัวหรือไม่ (ไม่นับส่วนที่เลือกไว้)
                if (textBox.SelectionLength == 0 && textBox.Text.Length >= 1)
                {
                    e.Handled = true;
                }
            }


        }

        private void dtg_cavity_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            if (dtg_cavity.Columns[e.ColumnIndex].Name == "CAVITY_NAME")
            {
                if (e.Value != null)
                {
                    e.Value = e.Value.ToString().Trim().ToUpper(); // แปลงเป็นตัวพิมพ์ใหญ่
                    e.ParsingApplied = true; // แจ้ง DataGridView ว่าเราได้เปลี่ยนค่าแล้ว
                }
            }
        }

        private void bt_back_Click(object sender, EventArgs e)
        {
            // ไม่เก็บร่างให้เอง (เก็บเฉพาะตอนกด Record Data) แต่ถามก่อนถ้ามีค่าที่ยังไม่ได้ Record
            if (!ConfirmLeave()) return;

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

            string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
            parent?.ReleaseReportMutex(mutexKey);

            userControlSelectDimension usrConSelectDim = new userControlSelectDimension()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()
            };

            usrConSelectDim.Dock = DockStyle.Fill;
            usrConSelectDim.propQA = new QAdataProperty();

            usrConSelectDim.propQA.labelProcess = "Select Report for : Dimension Check";
            usrConSelectDim.propQA.process = "Dimension_Check";
            // ต้องเป็น Inspection_Data_Check ให้ตรงกับตอนเข้าจากเมนู (userControlMenu.cs:182)
            // และตอนรีเฟรชในหน้า select (userControlSelectDimension.cs:725)
            // SearchForOpDimension เอาค่านี้ไปแทนเป็นชื่อคอลัมน์ใน where ใส่ผิดคอลัมน์ = ได้รายการคนละชุด
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

        //private void userControlRegular_ParentChanged(object sender, EventArgs e)
        //{
        //    RequestReleaseMutex?.Invoke($"Global\\ReportLock_{propQA.Report_No}_{propQA.process}");
        //}

        // เมธอดสำหรับปล่อย Mutex
        private void ReleaseReportMutex(string mutexKey)
        {
            if (!string.IsNullOrEmpty(currentMutexKey) && parent != null)
            {
                parent.ReleaseReportMutex(currentMutexKey);
                currentMutexKey = null; // รีเซ็ต mutexKey
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
                string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
                RequestReleaseMutex?.Invoke(mutexKey);
            }
            base.Dispose(disposing);
        }

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
