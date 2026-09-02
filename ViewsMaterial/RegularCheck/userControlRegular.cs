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
using System.Configuration;
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

namespace RawMat.ViewsMaterial.RegularCheck
{
    public partial class userControlRegular : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        public event Action OnReleaseMutex;
        public event EventHandler BackToARequested;
        public event Action<string> RequestReleaseMutex;
        public event Action OnClose;
        //public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        private readonly SettingControllers settingController = new SettingControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();
        //LowLevelKeyboardHook keyHook = new LowLevelKeyboardHook();


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

        private List<Image> regularImages;
        private int currentRegularImageIndex = 0;
        private Image _defaultImage = null; // ถ้าไม่ต้องการ placeholder จริง
        private readonly Dictionary<string, DataTable> equipmentSerialSourceByType = new Dictionary<string, DataTable>();
        private Form recordLoadingForm;

        public userControlRegular()
        {
            InitializeComponent();
            // ปิดการโฟกัสอัตโนมัติของ DataGridView
            dtg_regular.TabStop = false;
            dtg_regular.DataError += dtg_regular_DataError;

            // ตั้งค่าให้ UserControl ไม่โฟกัสอัตโนมัติ
            this.SetStyle(ControlStyles.Selectable, false);

            // ปิดการรับโฟกัส
            this.TabStop = false;
        }

        private void dtg_regular_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
        }

        // เมธอดสำหรับรับ mutexKey จาก userControlSelectRegular
        //public void SetMutexKey(string mutexKey)
        //{
        //    currentMutexKey = mutexKey;
        //}

        private void tb_record_Click(object sender, EventArgs e)
        {

            // บันทึกค่าที่กำลังแก้ไขใน DataGridView
            if (dtg_regular.IsCurrentCellDirty || dtg_regular.IsCurrentRowDirty)
            {
                dtg_regular.EndEdit(); // จบการแก้ไขเซลล์ปัจจุบัน
                dtg_regular.CommitEdit(DataGridViewDataErrorContexts.Commit); // บันทึกค่าลง DataSource
                bindingSource.EndEdit(); // บันทึกค่าลงใน BindingSource (ถ้าใช้)
            }

            if (dtg_regular.Rows.Count == 0)
            {
                MessageBox.Show("ยังไม่พบ data ที่จะทำการ Regular");
                return;
            }

            if (!IsDataTableValid(originalDataTable)) // ตรวจสอบจาก DataTable แทน
            {
                return; // ไม่ทำต่อถ้ามีเซลล์ว่าง
            }

            //if (!IsDataGridViewValid(dtg_regular))
            //{
            //    return; // ไม่ทำต่อถ้ามีเซลล์ว่าง
            //}

            ShowRecordLoading();
            try
            {
                propQA.TOTAL_STATUS = "1";
                propQA.EMP_ID = employee.EMP_CODE;
                if (!TrySetSelectedLotNo())
                {
                    MessageBox.Show("กรุณาเลือก Lot No. ก่อนบันทึก Regular", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                DataTable regularDataToSave = originalDataTable.Copy();

                // ใช้ DataTable สำเนาสำหรับบันทึก เพราะคอลัมน์ EQUIPMENT_SERIAL บนหน้าจอใช้ serial text
                // แต่ db_regular_data ต้องเก็บ EQUIPMENT_SERIAL_ID
                foreach (DataRow row in regularDataToSave.Rows)
                {
                    propQA.EQUIPMENT_SERIAL = row["EQUIPMENT_SERIAL"]?.ToString();
                    propQA.EQUIPMENT_TYPE_ID = row["EQUIPMENT_TYPE"]?.ToString();

                    if (!string.IsNullOrEmpty(propQA.EQUIPMENT_SERIAL) && !string.IsNullOrEmpty(propQA.EQUIPMENT_TYPE_ID))
                    {
                        int id = conQA.InsertEquipmentSerial(propQA);
                        row["EQUIPMENT_SERIAL"] = id;
                    }

                    propQA.TOTAL_STATUS = (Convert.ToInt32(row["TOTAL_JUDGE"]?.ToString()) * Convert.ToInt32(propQA.TOTAL_STATUS)).ToString();
                }

                propQA.dtgRegData = new DataGridView();
                propQA.dtgRegData.DataSource = regularDataToSave;

                //string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
                if (conQA.InsertRegularData(propQA) == true)
                {
                    bool endAtRegularReport = IsEndAtRegularReport();

                    if (propQA.TOTAL_STATUS == "0")
                    {
                        propQA.inProcStatus = ((int)ProcStatus.Pending).ToString();
                        propQA.reportStatus = ((int)ProcStatus.Pending).ToString();
                    }
                    else if (endAtRegularReport)
                    {
                        propQA.inProcStatus = ((int)ProcStatus.WaitingApprove).ToString();
                        propQA.reportStatus = ((int)ProcStatus.WaitingApprove).ToString();
                    }
                    else
                    {
                        propQA.inProcStatus = ((int)ProcStatus.OK).ToString();
                        propQA.reportStatus = ((int)ProcStatus.OK).ToString();
                    }

                    if (conQA.UpdateReportStatusLotNo(propQA) == true)
                    {
                        if (propQA.TOTAL_STATUS != "0")
                        {
                            if (endAtRegularReport)
                            {
                                if (!conQA.UpdateReportStatus(propQA))
                                {
                                    MessageBox.Show("ไม่สามารถ update report status เป็น Waiting Approve ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }
                            else if (!OpenNextProcessAfterRegular())
                            {
                                MessageBox.Show("ไม่สามารถเปิด process ถัดไปหลัง Regular ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            PrepareEndAtRegularExcel();
                        }

                        HideRecordLoading();

                        ProcStatus status;

                        bool parsed = int.TryParse(propQA.inProcStatus, out int statusId) && Enum.IsDefined(typeof(ProcStatus), statusId);
                        status = parsed ? (ProcStatus)statusId : ProcStatus.NG; // ค่าเริ่มต้นเป็น NG ถ้าแปลงไม่ได้

                        switch (status)
                        {
                            case ProcStatus.OK:
                                CustomMsgBoxBase.ShowCustomMessageBox(
                                    "Record Regular งาน OK เรียบร้อยแล้ว" ,
                                    "สำเร็จ",
                                    CustomMsgBoxBase.MessageBoxIconType.OK);
                                break;
                            case ProcStatus.Pending:
                                CustomMsgBoxBase.ShowCustomMessageBox(
                                    "Record Regular พบงาน ถูก PENDING",
                                    "สำเร็จ",
                                    CustomMsgBoxBase.MessageBoxIconType.Pending);
                                break;
                            case ProcStatus.WaitingApprove:
                                CustomMsgBoxBase.ShowCustomMessageBox(
                                    "Record Regular งาน OK และรอ Approve",
                                    "สำเร็จ",
                                    CustomMsgBoxBase.MessageBoxIconType.OK);
                                break;
                            default:
                                CustomMsgBoxBase.ShowCustomMessageBox(
                                    "สถานะไม่รู้จัก",
                                    "ข้อผิดพลาด",
                                    CustomMsgBoxBase.MessageBoxIconType.Pending);
                                break;
                        }

                        
                        bt_reg_Click();
                    
                    }
                    else
                    {
                        HideRecordLoading();
                        CustomMsgBoxBase.ShowCustomMessageBox("ไม่สามารถ record data ลง database ได้", "ข้อผิดพลาด", CustomMsgBoxBase.MessageBoxIconType.NG);

                    }
                }
                else
                {
                    HideRecordLoading();
                    CustomMsgBoxBase.ShowCustomMessageBox("ไม่สามารถ record data ลง database ได้", "ข้อผิดพลาด", CustomMsgBoxBase.MessageBoxIconType.NG);
                }
            }
            finally
            {
                HideRecordLoading();

                loadstatus();

                if (!conQA.DeleteReportActive(propQA))
                {
                    MessageBox.Show("ไม่สามารถคืนสถานะ report no ด้วย ip เครื่องนี้ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                //RequestReleaseMutex?.Invoke(mutexKey);
            }
            return;
        }

        private void ShowRecordLoading()
        {
            if (recordLoadingForm != null)
            {
                return;
            }

            tb_record.Enabled = false;
            Enabled = false;
            Cursor = Cursors.WaitCursor;

            var messageLabel = new System.Windows.Forms.Label
            {
                Dock = DockStyle.Top,
                Height = 62,
                Text = "กำลังบันทึกข้อมูล Regular\nกรุณารอสักครู่...",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new System.Drawing.Font("Tahoma", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 85, 60)
            };

            recordLoadingForm = new Form
            {
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterScreen,
                ClientSize = new Size(360, 62),
                ControlBox = false,
                ShowInTaskbar = false,
                TopMost = true,
                BackColor = Color.White,
                Text = "กำลังบันทึกข้อมูล"
            };
            recordLoadingForm.Controls.Add(messageLabel);

            Form owner = FindForm();
            if (owner != null)
            {
                recordLoadingForm.Show(owner);
            }
            else
            {
                recordLoadingForm.Show();
            }

            recordLoadingForm.Refresh();
            System.Windows.Forms.Application.DoEvents();
        }

        private void HideRecordLoading()
        {
            if (recordLoadingForm != null)
            {
                recordLoadingForm.Close();
                recordLoadingForm.Dispose();
                recordLoadingForm = null;
            }

            Enabled = true;
            tb_record.Enabled = true;
            Cursor = Cursors.Default;
        }

        private bool IsEndAtRegularReport()
        {
            if (propQA.TOTAL_STATUS == "0")
            {
                return false;
            }

            try
            {
                return conQA.NeedKeepData(propQA) != 1
                    && conQA.NeedFunctionCheck(propQA) != 1
                    && conQA.NeedDimensionCheck(propQA) != 1
                    && conQA.NeedAppearCheck(propQA) != 1;
            }
            catch
            {
                return false;
            }
        }

        private bool OpenNextProcessAfterRegular()
        {
            try
            {
                if (conQA.NeedKeepData(propQA) == 1)
                {
                    return UpdateNextProcessStatus("Inspection_Data_Check", ProcStatus.Unfinished);
                }

                if (conQA.NeedFunctionCheck(propQA) == 1)
                {
                    return UpdateNextProcessStatus("Function_Check", ProcStatus.Unfinished);
                }

                if (conQA.NeedDimensionCheck(propQA) == 1)
                {
                    return UpdateNextProcessStatus("Dimension_Check", ProcStatus.Unfinished);
                }

                if (conQA.NeedAppearCheck(propQA) == 1)
                {
                    return UpdateNextProcessStatus("Appearance_Check", ProcStatus.Unfinished);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool UpdateNextProcessStatus(string process, ProcStatus status)
        {
            string currentProcess = propQA.process;
            string currentInProcStatus = propQA.inProcStatus;
            string currentReportStatus = propQA.reportStatus;

            try
            {
                propQA.process = process;
                propQA.inProcStatus = ((int)status).ToString();
                propQA.reportStatus = ((int)status).ToString();

                return conQA.UpdateStatus(propQA);
            }
            finally
            {
                propQA.process = currentProcess;
                propQA.inProcStatus = currentInProcStatus;
                propQA.reportStatus = currentReportStatus;
            }
        }

        private void PrepareEndAtRegularExcel()
        {
            EnsureRegularExcelHeaderData();

            DataTable formatMap = null;
            string regularFormatId = ConfigurationManager.AppSettings["RegularReportFormatId"];
            if (!string.IsNullOrWhiteSpace(regularFormatId))
            {
                propQA.FORMAT_REPORT_ID = regularFormatId.Trim();
                formatMap = conQA.SearchFormatReport(propQA);
            }

            ExportExcell.CreateWaitApprovedExcel(propQA, originalDataTable, propQA.FORMAT_REPORT_NAME, formatMap);
        }

        private bool TrySetSelectedLotNo()
        {
            if (cb_lotNo.SelectedItem == null && cb_lotNo.Items.Count == 1)
            {
                cb_lotNo.SelectedIndex = 0;
            }

            propQA.Lot_No = cb_lotNo.SelectedItem?.ToString() ?? cb_lotNo.Text?.Trim() ?? propQA.Lot_No ?? string.Empty;
            return !string.IsNullOrWhiteSpace(propQA.Lot_No);
        }

        private void EnsureRegularExcelHeaderData()
        {
            TrySetSelectedLotNo();

            if (!string.IsNullOrWhiteSpace(propQA.SAMPLING_QTY))
            {
                return;
            }

            if (originalDataTable == null || !originalDataTable.Columns.Contains("SAMPLING_NO"))
            {
                return;
            }

            int samplingQty = originalDataTable.AsEnumerable()
                .Select(row => row["SAMPLING_NO"]?.ToString())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Distinct()
                .Count();

            if (samplingQty > 0)
            {
                propQA.SAMPLING_QTY = samplingQty.ToString();
            }
        }

        //protected override void OnHandleDestroyed(EventArgs e)
        //{
        //   // string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
        //   // RequestReleaseMutex?.Invoke(mutexKey);
        //    base.OnHandleDestroyed(e);
        //}

        private async void userControlRegular_Load(object sender, EventArgs e)
        {
            // ปิด AutoScroll ชั่วคราวขณะโหลด
            this.AutoScroll = false;

            lb_regularNo.Text = "Regular No : " + propQA.Regular_No;
            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
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
            lb_sampName.Text = propQA.SAMPLING_NAME == "Fix"
                ? $"Quantity {propQA.SAMPLING_QTY} Pcs."
                : $"{propQA.SAMPLING_QTY} {propQA.SAMPLING_NAME}";

            bool requiresCavityInput = RequiresCavityInput();
            gb_cavity.Visible = requiresCavityInput;
            lb_TotalCavity.Visible = requiresCavityInput;
            bt_confirmCavity.Enabled = requiresCavityInput;
            bt_confirmCavity.TabStop = requiresCavityInput;

            if (!requiresCavityInput)
            {
                picbox_reg.Location = new System.Drawing.Point(17, 113);
                picbox_reg.Size = new Size(1076, 556);
            }

            regularImages = await imgCls.LoadImagesAsync("RegularPath", propQA.M_CODE);
            currentRegularImageIndex = 0;

            if (regularImages != null && regularImages.Count > 0)
            {
                picbox_reg.Image = regularImages[0];
            }
            else
            {
                // Fallback: LoadImages จัดการ single แล้ว ถ้าไม่มีจะ return empty list
                picbox_reg.Image = _defaultImage; // หรือ null ถ้าไม่มี default
            }

            if (requiresCavityInput)
            {
                lb_TotalCavity.Visible = true;
                lb_TotalCavity.Text = "Total Cavity : " + GetTotalCavitySamplingQty();

                picbox_cavity.Image = imgCls.LoadSingleImage("CavityPath", propQA.M_CODE);
                //picbox_reg.Image = imgCls.LoadRegularImage(propQA.M_CODE);


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

                picbox_reg.Location = new System.Drawing.Point(17, 113);
                picbox_reg.Size = new Size(1076, 556);

                GenerateDataTableRegular(null, Convert.ToInt32(propQA.SAMPLING_QTY));
              
            }

            // หลังจากโหลดข้อมูลเสร็จ
            this.AutoScroll = true;

            // รีเซ็ตตำแหน่ง Scrollbar ไปด้านบน
            this.ScrollControlIntoView(lb_top);

            this.Focus();

            this.AutoScrollPosition = new System.Drawing.Point(0, 0);
            this.VerticalScroll.Value = 0;
        }

        private void dtg_regular_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // เช็คว่ากำลังแก้ไขคอลัมน์ "Value"
            if (dtg_regular.Columns[e.ColumnIndex].Name == "VALUE")
            {

                if (dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell)
                {
                    return;
                }

                string input = e.FormattedValue.ToString();

                if (string.IsNullOrWhiteSpace(input))
                {
                    return;
                }

                // ตรวจสอบว่าเป็นตัวเลข และต้องไม่มีจุดเกิน 1 จุด และไม่สามารถเป็นค่าติดลบได้
                if (!IsValidDecimal(input))
                {
                     MessageBox.Show("กรุณากรอกตัวเลขเท่านั้น และไม่สามารถมีจุดทศนิยมมากกว่า 1 จุดได้ และไม่สามารถเป็นค่าติดลบได้",
                            "Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                    // ใช้ BeginInvoke เพื่อตั้งค่า null หลังจากเหตุการณ์ CellValidating เสร็จสิ้น
                    this.BeginInvoke((MethodInvoker)delegate
                    {
                        dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = null;
                        dtg_regular.EndEdit();
                    });
                }
                


            }
        }

        private bool IsValidDecimal(string input)
        {
            // Pattern ที่อนุญาตเฉพาะตัวเลขบวกและมีจุดทศนิยมได้ไม่เกิน 1 จุด
            string pattern = @"^\d+(\.\d+)?$";
            return Regex.IsMatch(input, pattern);
        }

        private void dtg_regular_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            
            // ตรวจสอบว่ากำลังแก้ไข COLUMN "EQUIPMENT_SERIAL" หรือไม่
            if (dtg_regular.Columns[e.ColumnIndex].Name == "EQUIPMENT_SERIAL")
            {
                // ดึง DataTable จาก DataGridView
                BindingSource bs = dtg_regular.DataSource as BindingSource;
                DataTable dtData = bs != null ? (DataTable)bs.DataSource : dtg_regular.DataSource as DataTable;
                if (dtData == null) return;

                // ดึงค่า EQUIPMENT_SERIAL และ EQUIPMENT_TYPE จากแถวที่แก้ไข
                string newSerial = dtg_regular.Rows[e.RowIndex].Cells["EQUIPMENT_SERIAL"].Value?.ToString();
                string eqType = dtg_regular.Rows[e.RowIndex].Cells["EQUIPMENT_TYPE"].Value?.ToString();

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
                    dtg_regular.Refresh();
                }
            }

            // รีเซ็ตสถานะ
            //LowLevelKeyboardHook.IsEditingValue = false;
            //_hasTextChanged = false;
            //_isKeyboardInputDetected = false;

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

        private void bt_confirmCavity_Click(object sender, EventArgs e)
        {
            if (!RequiresCavityInput())
            {
                return;
            }

            dtg_cavity.EndEdit();

            // ตรวจสอบว่าจำนวนเป็นเลขตั้งแต่ 0 ขึ้นไปทุกแถว
            int totalQty = 0;
            foreach (DataGridViewRow row in dtg_cavity.Rows)
            {
                if (row.IsNewRow) continue;

                if (int.TryParse(row.Cells["SAMPLING_QTY"].Value?.ToString(), out int qty) && qty >= 0)
                {
                    totalQty += qty;
                }
                else
                {
                    MessageBox.Show("กรุณากรอกจำนวน Cavity เป็นตัวเลขตั้งแต่ 0 ขึ้นไปทุกแถว!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            int expectedQty = GetTotalCavitySamplingQty();

            // ตรวจสอบว่าผลรวมตรงกับที่ต้องการ
            if (totalQty != expectedQty)
            {
                MessageBox.Show(
                    $"ผลรวมของ QTY ต้องได้ {expectedQty}  (ปัจจุบัน: {totalQty})",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            // ล็อก dtg_cavity ไม่ให้แก้ไขข้อมูล
            dtg_cavity.ReadOnly = true;

            GenerateDataTableRegular(dtg_cavity, 0);
        }

        private bool RequiresCavityInput()
        {
            int.TryParse(propQA.CAVITY_QTY?.Trim(), out int cavityQty);
            return propQA.SAMPLING_TYPE == "4"
                || (propQA.SAMPLING_TYPE == "3" && cavityQty > 0);
        }

        // ยอดรวมที่ต้องเก็บทั้งใบ = ผลรวมของ SAMPLING_QTY ทุก cavity
        // Type 4 : Sampling_Qty ใน master เป็นจำนวน "ต่อ cavity" ต้องคูณจำนวน cavity เอง
        // Type 3 : ยกพื้นเป็น cavityQty x qtyPerCavity ไปแล้วตอนเลือกใบ (userControlSelectRegular.cs:327) ห้ามคูณซ้ำ
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

            dtg_regular.DataSource = bindingSource;

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
                if (string.IsNullOrWhiteSpace(value) || !int.TryParse(value, out int qty) || qty < 0)
                {
                    e.Cancel = true; // ยกเลิกการออกจากเซลล์โดยไม่แสดงข้อความ
                }
            }
        }

        private void GenerateDataTableRegular(DataGridView dtgCavity, int sampQty)
        {
            dtg_regular.CellEndEdit -= dtg_regular_CellEndEdit;
            dtg_regular.CellValidating -= dtg_regular_CellValidating;
           
            dtg_regular.CellFormatting -= dtg_regular_CellFormatting;
            dtg_regular.CellFormatting += dtg_regular_CellFormatting;

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

            int qtySampCounter = 1;

            if (dtgCavity == null)
            {
                for (int i = 0; i < sampQty; i++)
                {
                    int qtySampNo = qtySampCounter++;

                    foreach (DataRow measureRow in propQA.dtRegEq.Rows)
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

                        foreach (DataRow measureRow in propQA.dtRegEq.Rows)
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
                                null, null
                            );
                        }
                    }
                }
            }

            dtg_regular.DataSource = dtAllSum;

            // ซ่อนคอลัมน์ที่ไม่ต้องการแสดง
            string[] hiddenColumns = { "POINT_CAL", "POINT_ORDER", "EQUIPMENT_TYPE", "POINT_JUDGE", "TOTAL_JUDGE", "JUDGE_TYPE" };
            foreach (var col in hiddenColumns)
            {
                if (dtg_regular.Columns.Contains(col))
                {
                    dtg_regular.Columns[col].Visible = false;
                }
            }

            // บันทึกข้อมูลต้นฉบับ
            originalDataTable = (DataTable)dtg_regular.DataSource;
            bindingSource.DataSource = originalDataTable;
            dtg_regular.DataSource = bindingSource;

            // ทำให้คอลัมน์ที่ไม่ใช่ "VALUE" และ "EQUIPMENT_SERIAL" เป็น ReadOnly
            foreach (DataGridViewColumn column in dtg_regular.Columns)
            {
                column.ReadOnly = (column.Name != "VALUE" && column.Name != "EQUIPMENT_SERIAL");
            }

            // เปลี่ยน HeaderText
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

            dtg_regular.CellEndEdit += dtg_regular_CellEndEdit;
            dtg_regular.EditingControlShowing += (s2, e2) => EquipmentSerialColumn.HandleEditingControlShowing(dtg_regular, e2);
            dtg_regular.DataError += (s2, e2) => EquipmentSerialColumn.HandleDataError(dtg_regular, e2);
            dtg_regular.CellValidating += dtg_regular_CellValidating;
            // dtg_regular.EditingControlShowing += dtg_regular_EditingControlShowing;


            // เพิ่มบรรทัดนี้หลังจากตั้งค่า DataSource
            dtg_regular.ClearSelection();
            dtg_regular.CurrentCell = null;

            // ปิดการโฟกัสอัตโนมัติ
            dtg_regular.TabStop = false;

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

            bt_reg_Click();
        }

        public void bt_reg_Click()
        {

            //string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
            //parent?.ReleaseReportMutex(mutexKey);

            userControlSelectRegular usrConSelectReg = new userControlSelectRegular()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()
            };

            usrConSelectReg.Dock = DockStyle.Fill;
            usrConSelectReg.propQA = new QAdataProperty();

            usrConSelectReg.propQA.labelProcess = "Select Report for : Regular Check";
            usrConSelectReg.propQA.process = "Regular_Check";
            usrConSelectReg.propQA.prevProcess = "Packing_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpRegular(usrConSelectReg.propQA);
            usrConSelectReg.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectReg.propQA.dtgRawMat.DataSource = dt;

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
                    panelMain.Controls.Add(usrConSelectReg);
                    usrConSelectReg.BringToFront();
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

        /// <summary>ค่าตั้งจากหน้า Setting ไม่มีก็ถือว่าวัดเป็นตัวเลขตามค่าเริ่มต้นของคอลัมน์</summary>
        private static string GetJudgeType(DataRow measureRow)
        {
            if (!measureRow.Table.Columns.Contains(Utilities.PointJudgeType.ColumnName))
            {
                return Utilities.PointJudgeType.Numeric.ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            return measureRow[Utilities.PointJudgeType.ColumnName]?.ToString();
        }

        private void dtg_regular_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dtg_regular.Columns[e.ColumnIndex].Name == "VALUE")
            {
                // ตรวจสอบว่าข้อมูลใน CRITERIA_MIN และ CRITERIA_MAX มีค่า
                if (dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value != null &&
                    dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MAX"].Value != null)
                {
                    decimal minValue = Convert.ToDecimal(dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value);

                    // อ่านจากค่าที่ตั้งไว้ ไม่เดาจากเกณฑ์ ดู Utilities/PointJudgeType.cs
                    if (Utilities.PointJudgeType.IsPassFail(dtg_regular.Rows[e.RowIndex]))
                    {
                        // ตรวจสอบว่าเซลล์ VALUE ยังไม่ใช่ ComboBoxCell
                        if (!(dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell))
                        {
                            DataGridViewComboBoxCell comboBoxCell = new DataGridViewComboBoxCell();
                            // OK เก็บค่าที่อยู่ในเกณฑ์พอดี NG เก็บค่าที่ต่ำกว่าเกณฑ์ 1 หน่วย
                            // ตอนตัดสิน POINT_JUDGE ใช้ value >= min && value <= max จะได้ถูกทั้งคู่
                            // ไม่ว่าเกณฑ์จะตั้งไว้ที่ 1/1 หรือเลขอื่น
                            string okValue = minValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
                            string ngValue = (minValue - 1).ToString(System.Globalization.CultureInfo.InvariantCulture);

                            comboBoxCell.DataSource = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("", ""),  // ช่องว่าง
                        new KeyValuePair<string, string>(ngValue, "NG"),
                        new KeyValuePair<string, string>(okValue, "OK")
                    };
                            comboBoxCell.ValueMember = "Key";
                            comboBoxCell.DisplayMember = "Value";

                            // ใช้ BeginInvoke เพื่อหลีกเลี่ยงการเรียก CellFormatting ซ้ำ
                            this.BeginInvoke((MethodInvoker)delegate
                            {
                                // ตรวจสอบว่า RowIndex และ ColumnIndex ไม่เกินขอบเขตของ DataGridView
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
                        // ถ้าไม่ตรงเงื่อนไข ให้ใช้ TextBoxCell
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

        //private void userControlRegular_ParentChanged(object sender, EventArgs e)
        //{
        //    RequestReleaseMutex?.Invoke($"Global\\ReportLock_{propQA.Report_No}_{propQA.process}");
        //}

        //// เมธอดสำหรับปล่อย Mutex
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

        private void dtg_regular_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

            foreach (DataGridViewRow row in dtg_regular.Rows)
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

        //private void dtg_regular_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        //{
        //    if (dtg_regular.CurrentCell.ColumnIndex == dtg_regular.Columns["VALUE"].Index)
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
           
        //        Console.WriteLine($"Typing session ended (on leave). Duration:");
         

           
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

            

          
        //            //_isKeyboardInputDetected = true;
        //            //MessageBox.Show("ไม่ควรพิมพ์ข้อมูลดังกล่าวด้วย keyboard", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //            //if (textBox != null)
        //            //{
        //            //    textBox.Text = string.Empty;
        //            //}
        //            //dtg_regular.CurrentCell.Value = null;
        //            //dtg_regular.EndEdit();
                
        //}

        private void dtg_regular_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dtg_regular.Columns[e.ColumnIndex].Name == "VALUE")
            {
                DataGridViewRow row = dtg_regular.Rows[e.RowIndex];

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

                    // ลบส่วน dispose ออก เพื่อป้องกันการ dispose Image ใน list
                    // if (picbox_func.Image != null)
                    // {
                    //     picbox_func.Image.Dispose();
                    //     picbox_func.Image = null;
                    // }
                    picbox_reg.Image = regularImages[currentRegularImageIndex];

                    return true; // บอกว่าจัดการ key แล้ว ไม่ให้ไปต่อ
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (regularImages != null)
                {
                    foreach (var img in regularImages)
                    {
                        img?.Dispose();
                    }
                    regularImages.Clear();
                    regularImages = null;
                }
                // dispose อื่นๆ ถ้ามี
            }
            base.Dispose(disposing);
        }


    }
}
