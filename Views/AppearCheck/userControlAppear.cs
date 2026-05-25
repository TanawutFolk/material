using MySqlX.XDevAPI.Common;
using Org.BouncyCastle.Asn1.Crmf;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Markup;
using static RawMat.Property.QAdataProperty;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace RawMat.Views.AppearCheck
{
    public partial class userControlAppear : UserControl
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

        // Setup empty grid for input (first row with count=1, last row editable)
        private int currentMaxQty = 0;
        private int maxQty = 0;

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

        // Flag to prevent recursive event calls
        private bool _suppressEvents = false;
        // Events สำหรับ dtg_ngMode
        private bool _suppressNgEvents = false;
        private bool _suppressFormatting = false;

        private int totalNgRequired = 0;  // เก็บ QTY_NG จากด้านซ้าย
        private bool isNgModeActive = false;

        public userControlAppear()
        {

            InitializeComponent();

            // เปิด Double Buffered ลดการกระพริบ/ภาพซ้อน
            EnableDoubleBuffered(dtg_packing_size_appear);
            EnableDoubleBuffered(dtg_show_appear);
            EnableDoubleBuffered(dtg_ngMode);

            // *** สำคัญ: ปิด AutoGenerateColumns สำหรับ dtg_packing_size_appear เพื่อ manual control ***
            //dtg_packing_size_appear.AutoGenerateColumns = false;
            dtg_packing_size_appear.AutoGenerateColumns = true;
            dtg_packing_size_appear.Columns.Clear();
            dtg_packing_size_appear.DataBindingComplete += dtg_packing_size_appear_DataBindingComplete;  // Subscribe ถ้าต้องการ
            dtg_packing_size_appear.DataError += dtg_packing_size_appear_DataError;
            dtg_packing_size_appear.CellValidating += dtg_packing_size_appear_CellValidating;

            dtg_show_appear.CellValueChanged += dtg_show_appear_CellValueChanged;
            dtg_show_appear.CellValidating += dtg_show_appear_CellValidating;
            dtg_show_appear.CurrentCellChanged += dtg_show_appear_CurrentCellChanged;
            dtg_show_appear.CellEndEdit += dtg_show_appear_CellEndEdit;

            // *** เพิ่มบรรทัดนี้ ***
            dtg_show_appear.DataError += dtg_show_appear_DataError;  // Suppress error dialog

            dtg_ngMode.AutoGenerateColumns = false;
            dtg_ngMode.CellValueChanged += dtg_ngMode_CellValueChanged;
            dtg_ngMode.CellValidating += dtg_ngMode_CellValidating;
            dtg_ngMode.CellEndEdit += dtg_ngMode_CellEndEdit;
            dtg_ngMode.RowValidating += dtg_ngMode_RowValidating;
            dtg_ngMode.DataError += dtg_ngMode_DataError;

            // *** เพิ่ม events สำหรับ block non-digit input ***
            dtg_show_appear.EditingControlShowing += dtg_show_appear_EditingControlShowing;
            dtg_ngMode.EditingControlShowing += dtg_ngMode_EditingControlShowing;

            gb_ngMode.Enabled = false;
            gb_ngMode.Visible = true;  // หรือ false ถ้าต้องการ hide จนกว่าจะ NG

            // ใน userControlAppear_Load หรือ constructor หลัง InitializeComponent()
            dtg_ngMode.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;  // Enter edit เฉพาะเมื่อพิมพ์หรือ F2, ไม่ auto-enter
            dtg_ngMode.StandardTab = true;  // Allow tab navigation without edit issues
            dtg_ngMode.VirtualMode = false;  // ใช้ normal mode ถ้าไม่ virtual
            dtg_ngMode.AllowUserToAddRows = false;  // ป้องกัน auto-add row ที่ conflict


        }


        private void userControlAppear_Load(object sender, EventArgs e)
        {

            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_inspQty.Text = "Inspection Qty : " + propQA.inspQty;
            lb_lotSize.Text = "Lot Size : " + propQA.Qty;
            //lb_sampName.Text = propQA.SAMPLING_QTY + " " + propQA.SAMPLING_NAME;
            propQA.EMP_ID = employee.EMP_CODE;
            DataTable dt = new DataTable();

            dtg_packing_size_appear.SuspendLayout();

            try
            {
                // 1. ดึงข้อมูลดิบ
                DataTable rawDt = conQA.SearchSampleSize(propQA);

                // 2. แปลงข้อมูลให้ปลอดภัย (Clean Data Type)
                DataTable safeDt = ConvertToSafeDataTable(rawDt);

                // 3. Bind Data
                dtg_packing_size_appear.DataSource = null; // ล้างก่อน
                dtg_packing_size_appear.AutoGenerateColumns = true;

                if (dtg_packing_size_appear.InvokeRequired)
                {
                    dtg_packing_size_appear.Invoke(new Action(() =>
                    {
                        dtg_packing_size_appear.DataSource = safeDt;
                    }));
                }
                else
                {
                    dtg_packing_size_appear.DataSource = safeDt;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Bind Error: {ex}");
            }
            finally
            {
                dtg_packing_size_appear.ResumeLayout(); // กลับมาวาดต่อ
            }
            //DataTable dt_show = new DataTable();
            //dt_show = conQA.SearchAppearData(propQA);

            //if (dt_show.Rows.Count == 0 || dt_show == null)
            //{
            //    dtg_show_appear.Rows.Clear(); // ล้างแถวเท่านั้น ถ้าไม่ต้องการลบคอลัมน์
            //}
            //else
            //{

            //    // กำหนดคอลัมน์ด้วย DataPropertyName
            //    dtg_show_appear.Columns.Clear();


            //    // ตั้ง DataSource
            //    dtg_show_appear.DataSource = dt_show;
            //    //dtg_show_appear.DataBindingComplete += dtg_show_appear_DataBindingComplete;

            //}


            if (propQA.SAMPLING_TYPE == "3")
            {
                if (Convert.ToInt32(propQA.CAVITY_QTY) != 0)
                {
                    picbox_cavity.Image = imgCls.LoadCavityImage(propQA.M_CODE);
                }
                else
                {
                    gb_cavity.Visible = false;

                    picbox_Appear.Location = new System.Drawing.Point(286, 113);
                    picbox_Appear.Size = new Size(807, 442);
                }

                picbox_Appear.Image = imgCls.LoadAppearImage(propQA.M_CODE);
                gb_pack.Enabled = true;
                gb_input.Enabled = false;

                //picbox_mat.Image = imgCls.LoadMaterialImage(propQA.M_CODE);


            }
            else if (propQA.SAMPLING_TYPE == "1" || propQA.SAMPLING_TYPE == "5")
            {
                gb_cavity.Visible = false;

                picbox_Appear.Location = new System.Drawing.Point(286, 113);
                picbox_Appear.Size = new Size(807, 442);
                picbox_Appear.Image = imgCls.LoadAppearImage(propQA.M_CODE);

                gb_pack.Enabled = true;
                gb_input.Enabled = false;


                //GenerateDataTableFunction(null, Convert.ToInt32(propQA.SAMPLING_QTY));

            }
            else
            {
                CustomMsgBoxBase.ShowCustomMessageBox($"ยังไม่มี Setting Sampling Type นี้ ในการดำเนินการ", "แจ้งเตือน", CustomMsgBoxBase.MessageBoxIconType.NG);
                bt_Appear_Click();
            }


            //query data to dtg 

            // เริ่มต้นและตั้งค่า Timer
            checkTimer = new System.Windows.Forms.Timer();
            checkTimer.Interval = 60000; // 3 นาที (60,000 มิลลิวินาที)
            checkTimer.Tick += CheckTimer_Tick;
            checkTimer.Start();

        }

        //// *** Manual create columns สำหรับ dtg_packing_size_appear ***
        //private void CreatePackingColumns()
        //{
        //    dtg_packing_size_appear.Columns.Clear();

        //    // BATCH (string, read-only)
        //    DataGridViewTextBoxColumn colBatch = new DataGridViewTextBoxColumn();
        //    colBatch.Name = "BATCH";
        //    colBatch.DataPropertyName = "BATCH";
        //    colBatch.HeaderText = "ชุดที่";
        //    colBatch.ReadOnly = true;
        //    colBatch.ValueType = typeof(string);
        //    colBatch.DefaultCellStyle.NullValue = "";  // Null show empty
        //    dtg_packing_size_appear.Columns.Add(colBatch);

        //    // PACK_COUNT (int, read-only)
        //    DataGridViewTextBoxColumn colPack = new DataGridViewTextBoxColumn();
        //    colPack.Name = "PACK_COUNT";
        //    colPack.DataPropertyName = "PACK_COUNT";
        //    colPack.HeaderText = "แพ๊ค";
        //    colPack.ReadOnly = true;
        //    colPack.ValueType = typeof(int);
        //    colPack.DefaultCellStyle.Format = "N0";
        //    colPack.DefaultCellStyle.NullValue = 0;
        //    dtg_packing_size_appear.Columns.Add(colPack);


        //    // VALUE column (int, read-only)
        //    DataGridViewTextBoxColumn colValue = new DataGridViewTextBoxColumn();
        //    colValue.Name = "VALUE";
        //    colValue.DataPropertyName = "VALUE";
        //    colValue.HeaderText = "ตัว/แพ๊ค";
        //    colValue.ReadOnly = true;
        //    colValue.ValueType = typeof(int);  // Explicit int
        //    colValue.DefaultCellStyle.Format = "N0";  // No decimal
        //    colValue.DefaultCellStyle.NullValue = 0;  // Null show 0
        //    dtg_packing_size_appear.Columns.Add(colValue);

        //    // REMAIN_PACKING_SIZE (int, read-only)
        //    DataGridViewTextBoxColumn colPackingSize = new DataGridViewTextBoxColumn();
        //    colPackingSize.Name = "PACKING_SIZE";
        //    colPackingSize.DataPropertyName = "PACKING_SIZE";
        //    colPackingSize.HeaderText = "จากทั้งหมด";
        //    colPackingSize.ReadOnly = true;
        //    colPackingSize.ValueType = typeof(int);
        //    colPackingSize.DefaultCellStyle.Format = "N0";
        //    colPackingSize.DefaultCellStyle.NullValue = 0;
        //    dtg_packing_size_appear.Columns.Add(colPackingSize);

        //    // REMAIN_PACKING_SIZE (int, read-only)
        //    DataGridViewTextBoxColumn colRemain = new DataGridViewTextBoxColumn();
        //    colRemain.Name = "REMAIN_PACKING_SIZE";
        //    colRemain.DataPropertyName = "REMAIN_PACKING_SIZE";
        //    colRemain.HeaderText = "สุ่มตรวจ";
        //    colRemain.ReadOnly = true;
        //    colRemain.ValueType = typeof(int);
        //    colRemain.DefaultCellStyle.Format = "N0";
        //    colRemain.DefaultCellStyle.NullValue = 0;
        //    dtg_packing_size_appear.Columns.Add(colRemain);

        //}

        // Helper method
        public static void EnableDoubleBuffered(Control control)
        {
            System.Reflection.PropertyInfo prop = typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            prop.SetValue(control, true, null);
        }

        private void dtg_packing_size_appear_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

            
            dtg_packing_size_appear.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dtg_packing_size_appear.AllowUserToAddRows = false;
            dtg_packing_size_appear.ReadOnly = false; // Grid หลัก editable

            if (dtg_packing_size_appear.Columns["VALUE"] != null)
            {
                dtg_packing_size_appear.Columns["VALUE"].HeaderText = "ตัว/แพ๊ค";
                dtg_packing_size_appear.Columns["VALUE"].ReadOnly = true;

                // *** เพิ่ม: Safe format สำหรับ int columns ***
                dtg_packing_size_appear.Columns["VALUE"].DefaultCellStyle.Format = "N0";  // No decimal
                dtg_packing_size_appear.Columns["VALUE"].DefaultCellStyle.NullValue = "0";  // Null แสดง 0

            }

            if (dtg_packing_size_appear.Columns["PACK_COUNT"] != null)
            {
                dtg_packing_size_appear.Columns["PACK_COUNT"].HeaderText = "จำนวนแพ็ค";
                dtg_packing_size_appear.Columns["PACK_COUNT"].ReadOnly = true;
            }

            if (dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"] != null)
            {
                dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"].HeaderText = "เหลือตรวจ";
                dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"].ReadOnly = true;

                // *** เพิ่ม: การจัดการ NullValue สำหรับ int ***
                dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"].DefaultCellStyle.NullValue = 0;
                dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"].DefaultCellStyle.Format = "N0";

            }

            if (dtg_packing_size_appear.Columns["BATCH"] != null)
            {
                dtg_packing_size_appear.Columns["BATCH"].HeaderText = "ชุดที่";
                dtg_packing_size_appear.Columns["BATCH"].ReadOnly = true;

                // *** เพิ่ม: สำหรับ string ***
                //dtg_packing_size_appear.Columns["BATCH"].DefaultCellStyle.NullValue = "";  // Null แสดง empty
            }

            if (dtg_packing_size_appear.Columns["PACKING_SIZE"] != null)
            {
                //dtg_packing_size_appear.Columns["PACKING_SIZE"].Visible = false;
                dtg_packing_size_appear.Columns["PACKING_SIZE"].HeaderText = "ต้องตรวจทั้งหมด";
                dtg_packing_size_appear.Columns["PACKING_SIZE"].ReadOnly = true;
            }

             dtg_packing_size_appear.Refresh(); // Force update UI
            //dtg_packing_size_appear.Columns["NUMBER"].Visible = false;
            dtg_packing_size_appear.DataBindingComplete -= dtg_packing_size_appear_DataBindingComplete;  // Subscribe ถ้าต้องการ
        }

        // Event Handler สำหรับ Timer
        private void CheckTimer_Tick(object sender, EventArgs e)
        {
            if (conQA.CheckReportStatus(propQA) == false)
            {
                CustomMsgBoxBase.ShowCustomMessageBox($"พบงานที่ติด Pending จาก process อื่น", "แจ้งเตือน", CustomMsgBoxBase.MessageBoxIconType.NG);
                bt_Appear_Click();
                checkTimer.Stop();
            }
        }


        public void bt_Appear_Click()
        {

            userControlSelectAppear usrConSelectAppear = new userControlSelectAppear()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()
            };

            usrConSelectAppear.Dock = DockStyle.Fill;
            usrConSelectAppear.propQA = new QAdataProperty();

            usrConSelectAppear.propQA.labelProcess = "Select Report for : Appearance Check";
            usrConSelectAppear.propQA.process = "Appearance_Check";
            usrConSelectAppear.propQA.prevProcess = "Inspection_Data_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpAppear(usrConSelectAppear.propQA);
            usrConSelectAppear.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectAppear.propQA.dtgRawMat.DataSource = dt;

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
                    panelMain.Controls.Add(usrConSelectAppear);
                    usrConSelectAppear.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }



        // Optional: สร้าง method สำหรับ cleanup manual (ถ้าต้องการเรียกก่อน Dispose)
        private void CleanupEvents()
        {
            if (dtg_show_appear != null)
            {
                dtg_show_appear.CellValueChanged -= dtg_show_appear_CellValueChanged;
                dtg_show_appear.CellValidating -= dtg_show_appear_CellValidating;
                dtg_show_appear.CurrentCellChanged -= dtg_show_appear_CurrentCellChanged;
                dtg_show_appear.CellEndEdit -= dtg_show_appear_CellEndEdit;
            }

            // ... existing ...
            if (dtg_ngMode != null)
            {
                dtg_ngMode.CellValueChanged -= dtg_ngMode_CellValueChanged;
                dtg_ngMode.CellValidating -= dtg_ngMode_CellValidating;
                dtg_ngMode.CellEndEdit -= dtg_ngMode_CellEndEdit;
                dtg_ngMode.RowValidating -= dtg_ngMode_RowValidating;
                dtg_ngMode.DataError -= dtg_ngMode_DataError;

                // *** เพิ่ม ***
                dtg_ngMode.EditingControlShowing -= dtg_ngMode_EditingControlShowing;

            }

            if (dtg_packing_size_appear != null)
            {
                dtg_packing_size_appear.DataError -= dtg_packing_size_appear_DataError;
                dtg_packing_size_appear.EditingControlShowing -= dtg_packing_size_appear_EditingControlShowing;
                dtg_packing_size_appear.MouseDown -= dtg_packing_size_appear_MouseDown;
                // DataBindingComplete if subscribed
            }


        }

        private void bt_back_Click(object sender, EventArgs e)
        {
            CleanupEvents();  // Unsubscribe manual ก่อน switch
            bt_Appear_Click();
        }

        private void loadstatus()
        {
            if (this.ParentForm is frmMain mainForm)
            {
                mainForm.LoadStatus();
            }
        }

        private void UpdateCurrentTaskLabel()
        {
            if (lb_currentTask == null) return;

            int inspectedInBatch = 0;
            if (dtg_show_appear?.DataSource is DataTable dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    string judge = row.Table.Columns.Contains("JUDGE") ? row["JUDGE"]?.ToString() ?? "" : "";
                    if (string.IsNullOrWhiteSpace(judge)) continue;
                    inspectedInBatch += ParseInt(row["QTY_SELECT"]);
                }
            }

            int remaining = Math.Max(maxQty - inspectedInBatch, 0);
            lb_currentTask.Text = $"กำลังตรวจชุดที่ {propQA.BATCH} | ต้องตรวจทั้งหมด {maxQty} ชิ้น | เหลือตรวจ {remaining} ชิ้น";
        }

        private void ResetCurrentTaskLabel()
        {
            if (lb_currentTask != null)
            {
                lb_currentTask.Text = "เลือกชุดตรวจเพื่อเริ่มกรอกผล";
            }

            if (label2 != null)
            {
                label2.Text = "ระบุอาการเสียแล้ว: 0 / 0 ชิ้น";
            }
        }

        private void bt_Select_Click(object sender, EventArgs e)
        {
            if (dtg_packing_size_appear.SelectedRows.Count == 0)
            {
                return; // ไม่มีแถวที่เลือก ไม่อยากให้ทำต่อ
            }
            else
            {
                dtg_packing_size_appear.Enabled = false;
                bt_select_packing_size_appear.Enabled = false;
            }

            var selectedRow = dtg_packing_size_appear.SelectedRows[0];
            propQA.BATCH = selectedRow.Cells["BATCH"].Value.ToString();

            //propQA.PACKING_SIZE = selectedRow.Cells["PACKING_SIZE"].Value.ToString(); // Total fixed for batch
            //propQA.REMAIN_PACKING_SIZE = selectedRow.Cells["REMAIN_PACKING_SIZE"].Value.ToString();

            maxQty = Convert.ToInt32(selectedRow.Cells["PACKING_SIZE"].Value.ToString()); // Total fixed for batch
            currentMaxQty = Convert.ToInt32(selectedRow.Cells["REMAIN_PACKING_SIZE"].Value.ToString()); // Remaining to inspect
            //currentMaxQty = Convert.ToInt32(propQA.REMAIN_PACKING_SIZE); // Remaining to inspect

            DataTable dt_show = conQA.SearchAppearData(propQA);


            // สร้าง DataTable ใหม่เสมอเพื่อควบคุมโครงสร้างคอลัมน์ให้คงที่และเรียงลำดับถูกต้อง (APPEARANCE_DATE อยู่ด้านหน้า)
            DataTable dataSource = new DataTable();
            string[] requiredColumns = { "APPEARANCE_DATE", "BATCH", "COUNT", "QTY_SELECT", "QTY_OK", "QTY_NG", "JUDGE" };
            foreach (string colName in requiredColumns)
            {
                dataSource.Columns.Add(colName, typeof(string));
            }

            // คัดลอกข้อมูลจาก dt_show ถ้ามี (เฉพาะคอลัมน์ที่ตรงกัน เพื่อหลีกเลี่ยง duplicates)
            if (dt_show != null && dt_show.Rows.Count > 0)
            {
                foreach (DataRow oldRow in dt_show.Rows)
                {
                    DataRow copiedRow = dataSource.NewRow();  // เปลี่ยนชื่อเพื่อหลีกเลี่ยง conflict
                    foreach (string colName in requiredColumns)
                    {
                        if (dt_show.Columns.Contains(colName))
                        {
                            copiedRow[colName] = oldRow[colName];
                        }
                        // ถ้าคอลัมน์ไม่มีใน dt_show จะเป็น DBNull อัตโนมัติ
                    }
                    dataSource.Rows.Add(copiedRow);
                }
            }

            // คำนวณ lastCount อย่างปลอดภัย
            int lastCount = 0;
            if (dataSource.Rows.Count > 0)
            {
                object lastCountObj = dataSource.Rows[dataSource.Rows.Count - 1]["COUNT"];
                if (lastCountObj != DBNull.Value && int.TryParse(lastCountObj.ToString(), out int parsedCount))
                {
                    lastCount = parsedCount;
                }
            }

            // เพิ่มแถวใหม่สำหรับ input เสมอ
            string dateStr = DateTime.Now.ToString("dd-MMM-yyyy");
            DataRow newRow = dataSource.NewRow();
            newRow["APPEARANCE_DATE"] = dateStr;
            newRow["BATCH"] = propQA.BATCH;
            newRow["COUNT"] = (lastCount + 1).ToString();
            newRow["QTY_SELECT"] = DBNull.Value;
            newRow["QTY_OK"] = DBNull.Value;
            newRow["QTY_NG"] = DBNull.Value;
            newRow["JUDGE"] = DBNull.Value;
            dataSource.Rows.Add(newRow);

            // ล้าง DataSource ก่อนเพื่อรีเซ็ต grid (ป้องกันคอลัมน์เก่าค้าง)
            dtg_show_appear.DataSource = null;
            dtg_show_appear.DataSource = dataSource;

            gb_input.Enabled = true;
            ApplyRowReadOnly();  // ให้เฉพาะแถวสุดท้ายแก้ไขได้
            UpdateCurrentTaskLabel();
            dtg_show_appear.Refresh();

            // โฟกัสที่เซลล์ JUDGE ของแถวสุดท้าย
            if (dtg_show_appear.Rows.Count > 0 && dtg_show_appear.Columns["JUDGE"] != null)
            {
                dtg_show_appear.CurrentCell = dtg_show_appear.Rows[dtg_show_appear.Rows.Count - 1].Cells["JUDGE"];
            }

            // Refresh UI สำหรับ NG mode (ปิดก่อน)
            isNgModeActive = false;
            gb_ngMode.Enabled = false;
            totalNgRequired = 0;
            InitializeNgModeDataTable();

            tb_record.Enabled = false; // Disable จนกว่าจะ ready

        }

        private void dtg_packing_size_appear_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            // ตรวจสอบว่าเป็นคอลัมน์ REMAIN_PACKING_SIZE หรือไม่
            if (dtg_packing_size_appear.Columns[e.ColumnIndex].Name == "REMAIN_PACKING_SIZE")
            {
                if (e.Value != null && int.TryParse(e.Value.ToString(), out int val))
                {
                    if (val == 0)
                    {
                        e.CellStyle.BackColor = Color.LightGray;
                        e.CellStyle.ForeColor = Color.Black; // *** ต้องมั่นใจว่าเป็นสี Black หรือสีที่ตัดกับพื้นหลัง ***
                    }
                    else
                    {
                        e.CellStyle.BackColor = Color.White;
                        e.CellStyle.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void dtg_packing_size_appear_SelectionChanged(object sender, EventArgs e)
        {
            // ป้องกันการทำงานซ้อนกัน
            if (_suppressEvents) return;

            // ถ้าไม่มีการเลือกแถว ให้ปิดปุ่มและจบการทำงาน
            if (dtg_packing_size_appear.SelectedRows.Count == 0)
            {
                bt_select_packing_size_appear.Enabled = false;
                label3.Text = "เลือกชุดที่มีจำนวนเหลือตรวจมากกว่า 0";
                return;
            }

            try
            {
                // 1. ดึงแถวที่เลือกมา
                var selectedRow = dtg_packing_size_appear.SelectedRows[0];

                // 2. ดึงค่าจากคอลัมน์ "REMAIN_PACKING_SIZE" (สุ่มตรวจ) โดยตรง
                var cellValue = selectedRow.Cells["REMAIN_PACKING_SIZE"].Value;
                int remainQty = 0;

                // 3. แปลงค่าอย่างปลอดภัย (กัน Null/Error)
                if (cellValue != null && cellValue != DBNull.Value)
                {
                    int.TryParse(cellValue.ToString(), out remainQty);
                }

                // 4. ตรรกะการเปิดปุ่ม: 
                // ถ้าจำนวนที่เหลือ (remainQty) มากกว่า 0 -> ให้กดเลือกทำได้
                if (remainQty > 0)
                {
                    bt_select_packing_size_appear.Enabled = true;
                    string batch = selectedRow.Cells["BATCH"].Value?.ToString() ?? "";
                    int totalQty = ParseIntSafe(selectedRow.Cells["PACKING_SIZE"].Value);
                    label3.Text = $"เลือกชุดที่ {batch}: เหลือตรวจ {remainQty} / {totalQty} ชิ้น";
                }
                else
                {
                    // ถ้าเป็น 0 (ตรวจหมดแล้ว) -> ปิดปุ่ม ห้ามเลือกทำซ้ำ
                    bt_select_packing_size_appear.Enabled = false;
                    string batch = selectedRow.Cells["BATCH"].Value?.ToString() ?? "";
                    label3.Text = $"ชุดที่ {batch} ตรวจครบแล้ว กรุณาเลือกชุดอื่น";

                    // (Optional) ถ้าอยากให้มันเด้งออกจากการเลือกด้วย ให้ใช้ ClearSelection
                    // แต่ระวัง Loop นรก ถ้าใช้บรรทัดล่างนี้ ต้องมั่นใจว่าจัดการ Flag ดีๆ
                    // _suppressEvents = true;
                    // dtg_packing_size_appear.ClearSelection();
                    // _suppressEvents = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Selection Error: " + ex.Message);
                bt_select_packing_size_appear.Enabled = false;
                label3.Text = "ไม่สามารถอ่านจำนวนเหลือตรวจได้";
            }
        }

        // 4. เพิ่ม dtg_packing_size_appear_MouseDown - สำหรับ handle click/selection บน cell (e.g., REMAIN_PACKING_SIZE) เพื่อให้ focus/edit ได้ทันที
        // *** MouseDown: Prevent loop โดย end-edit ก่อน click ***
        private void dtg_packing_size_appear_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dtg_packing_size_appear.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.Cell)
            {
                DataGridView.HitTestInfo hit = dtg_packing_size_appear.HitTest(e.X, e.Y);
                if (hit.ColumnIndex >= 0 && hit.RowIndex >= 0 &&
                    dtg_packing_size_appear.Columns[hit.ColumnIndex].Name == "REMAIN_PACKING_SIZE")
                {
                    dtg_packing_size_appear.CurrentCell = dtg_packing_size_appear.Rows[hit.RowIndex].Cells[hit.ColumnIndex];
                    dtg_packing_size_appear.BeginEdit(true); // Start edit ถ้า click column นี้
                }
            }
        }

        // 3. dtg_packing_size_appear_MouseWheel - เก็บไว้สำหรับ scroll lag (ถ้าปัญหา scroll wheel)
        //private void dtg_packing_size_appear_MouseWheel(object sender, MouseEventArgs e)
        //{
        //    _suppressFormatting = true;

        //    var timer = new System.Windows.Forms.Timer { Interval = 100 };
        //    timer.Tick += (s, args) =>
        //    {
        //        _suppressFormatting = false;
        //        timer.Stop();
        //        timer.Dispose();
        //        dtg_packing_size_appear.Invalidate();
        //    };
        //    timer.Start();
        //}

        //// Adjusted AddNewInputRow to handle appending with next COUNT and given BATCH
        //private void AddNewInputRow(DataTable dt, string batchValue)
        //{
        //    int nextCount = dt.Rows.Count > 0 ? Convert.ToInt32(dt.Rows[dt.Rows.Count - 1]["COUNT"]) + 1 : 1;

        //    DataRow newRow = dt.NewRow();
        //    newRow["INPUT_DATE"] = DateTime.Now.ToString("dd-MMM-yyyy");
        //    newRow["BATCH"] = batchValue;
        //    newRow["COUNT"] = nextCount;
        //    newRow["QTY_SELECT"] = DBNull.Value;
        //    newRow["QTY_OK"] = DBNull.Value;
        //    newRow["QTY_NG"] = DBNull.Value;
        //    newRow["JUDGE"] = DBNull.Value;  // Empty for editing
        //    dt.Rows.Add(newRow);

        //    // Refresh DataSource to trigger events
        //    dtg_show_appear.DataSource = dt;
        //}

        private void dtg_show_appear_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dtg_show_appear.Columns["APPEARANCE_DATE"] != null)
            {
                dtg_show_appear.Columns["APPEARANCE_DATE"].HeaderText = "วันที่";
                dtg_show_appear.Columns["APPEARANCE_DATE"].ReadOnly = true;
            }

            if (dtg_show_appear.Columns["BATCH"] != null)
            {
                dtg_show_appear.Columns["BATCH"].HeaderText = "ชุดที่";
                dtg_show_appear.Columns["BATCH"].ReadOnly = true;
            }

            if (dtg_show_appear.Columns["COUNT"] != null)
            {
                dtg_show_appear.Columns["COUNT"].HeaderText = "ครั้งที่";
                dtg_show_appear.Columns["COUNT"].ReadOnly = true;
            }

            if (dtg_show_appear.Columns["QTY_SELECT"] != null)
            {
                dtg_show_appear.Columns["QTY_SELECT"].HeaderText = "จำนวนที่เลือก";
                dtg_show_appear.Columns["QTY_SELECT"].ReadOnly = false;
            }

            if (dtg_show_appear.Columns["QTY_OK"] != null)
            {
                dtg_show_appear.Columns["QTY_OK"].HeaderText = "จำนวนงานดี";
                dtg_show_appear.Columns["QTY_OK"].ReadOnly = false;
            }

            if (dtg_show_appear.Columns["QTY_NG"] != null)
            {
                dtg_show_appear.Columns["QTY_NG"].HeaderText = "จำนวนงานเสีย";
                dtg_show_appear.Columns["QTY_NG"].ReadOnly = false;
            }

            if (dtg_show_appear.Columns["JUDGE"] != null)
            {
                dtg_show_appear.Columns["JUDGE"].HeaderText = "ผล";
                dtg_show_appear.Columns["JUDGE"].ReadOnly = true;  // Editable only in last row
            }

            // Make all rows except last read-only, and hide headers if needed (set RowHeadersVisible = false)
            dtg_show_appear.RowHeadersVisible = false;  // ซ่อน HeaderText ด้านซ้าย
            dtg_show_appear.AutoGenerateColumns = false;

            ApplyRowReadOnly();
        }

        // Apply ReadOnly to all rows except the last one
        private void ApplyRowReadOnly()
        {
            for (int i = 0; i < dtg_show_appear.Rows.Count - 1; i++)  // All but last row
            {
                dtg_show_appear.Rows[i].ReadOnly = true;  // Lock entire row (overrides column settings for previous rows)
            }

            if (dtg_show_appear.Rows.Count > 0)
            {
                // Last row: Unlock row to allow editing in editable columns (white background)
                dtg_show_appear.Rows[dtg_show_appear.Rows.Count - 1].ReadOnly = false;
                dtg_show_appear.Rows[dtg_show_appear.Rows.Count - 1].DefaultCellStyle.BackColor = Color.White;
            }
        }

        // เพิ่ม method เพื่อ init DataTable สำหรับ dtg_ngMode (ว่างเปล่า)
        private void InitializeNgModeDataTable()
        {
            DataTable ngDt = new DataTable();
            ngDt.Columns.Add("QTY_NG", typeof(int));
            ngDt.Columns.Add("NG_DETAIL", typeof(string));
            dtg_ngMode.DataSource = ngDt;
            ConfigureNgModeGridColumns();
            dtg_ngMode.AllowUserToAddRows = false; // ควบคุมด้วย code
        }

        private void ConfigureNgModeGridColumns()
        {
            if (dtg_ngMode.Columns["QTY_NG"] != null)
            {
                dtg_ngMode.Columns["QTY_NG"].HeaderText = "QTY NG";
            }

            if (dtg_ngMode.Columns["NG_DETAIL"] != null)
            {
                dtg_ngMode.Columns["NG_DETAIL"].HeaderText = "NG MODE";
            }
        }

        private void tb_record_Click(object sender, EventArgs e)
        {
            SaveAppearData();
        }

        private void tb_record_Click_backup(object sender, EventArgs e)
        {

            dtg_show_appear.EndEdit();

            if (dtg_show_appear.Rows.Count == 0)
            {
                MessageBox.Show("ไม่มีข้อมูลให้บันทึก");
                return;
            }

            // Commit any pending edits before reading values
            
            DataTable dataSource = (DataTable)dtg_show_appear.DataSource;
            DataRow lastDataRow = dataSource.Rows[dataSource.Rows.Count - 1];

            // ดึงค่าจากแถวสุดท้าย (จัดการ DBNull)
            int qtySelect = lastDataRow["QTY_SELECT"] is DBNull ? 0 : Convert.ToInt32(lastDataRow["QTY_SELECT"]);
            int qtyOK = lastDataRow["QTY_OK"] is DBNull ? 0 : Convert.ToInt32(lastDataRow["QTY_OK"]);
            int qtyNG = lastDataRow["QTY_NG"] is DBNull ? 0 : Convert.ToInt32(lastDataRow["QTY_NG"]);

            // ถ้า JUDGE != "NG" ไม่ต้องเช็ค NG Mode
            if (lastDataRow["JUDGE"].ToString() != "NG")
            {
                // ... existing save logic ...
                return;
            }

            // เช็ค NG Mode
            if (!isNgModeActive || dtg_ngMode.DataSource == null)
            {
                MessageBox.Show("กรุณาเปิด NG Mode และกรอกข้อมูล");
                return;
            }

            DataTable ngDt = (DataTable)dtg_ngMode.DataSource;
            int sumNg = 0;
            bool hasEmptyMode = false;
            int firstEmptyRowIndex = -1;

            foreach (DataRow rowData in ngDt.Rows)
            {
                int qty = rowData["QTY_NG"] is DBNull ? 0 : Convert.ToInt32(rowData["QTY_NG"]);
                sumNg += qty;

                string mode = rowData["NG_DETAIL"] is DBNull ? "" : rowData["NG_DETAIL"].ToString().Trim();
                if (qty > 0 && string.IsNullOrEmpty(mode))
                {
                    hasEmptyMode = true;
                    // Find row index for highlight
                    for (int i = 0; i < dtg_ngMode.Rows.Count; i++)
                    {
                        if (dtg_ngMode.Rows[i].DataBoundItem == rowData)
                        {
                            firstEmptyRowIndex = i;
                            break;
                        }
                    }
                    break;
                }
            }

            if (hasEmptyMode)
            {
                MessageBox.Show("กรุณากรอก NG MODE ในทุกแถวที่กรอก QTY NG แล้ว");

                // Highlight first empty MODE cell red
                if (firstEmptyRowIndex >= 0)
                {
                    dtg_ngMode.Rows[firstEmptyRowIndex].Cells["NG_DETAIL"].Style.BackColor = Color.Red;
                    dtg_ngMode.CurrentCell = dtg_ngMode.Rows[firstEmptyRowIndex].Cells["NG_DETAIL"];
                    dtg_ngMode.BeginEdit(true);  // Enter edit mode
                }
                return;
            }

            if (sumNg != totalNgRequired)
            {
                MessageBox.Show($"ผลรวม QTY NG ({sumNg}) ต้องเท่ากับจำนวนงานเสีย ({totalNgRequired})");
                return;
            }

            // Validation: QTY_SELECT ต้อง > 0
            if (qtySelect <= 0)
            {
                MessageBox.Show("จำนวนที่เลือกตรวจต้องมากกว่า 0");
                dtg_show_appear.CurrentCell = dtg_show_appear.Rows[dtg_show_appear.Rows.Count - 1].Cells["QTY_SELECT"];
                return;
            }

            // Validation: QTY_SELECT ไม่เกิน currentMaxQty (จาก grid ด้านบน)
            if (qtySelect > currentMaxQty)
            {
                MessageBox.Show($"จำนวนที่เลือกตรวจ ({qtySelect}) เกินจำนวนที่เหลือ ({currentMaxQty}) แล้วค่ะ กรุณาเลือกให้น้อยลง");
                dtg_show_appear.CurrentCell = dtg_show_appear.Rows[dtg_show_appear.Rows.Count - 1].Cells["QTY_SELECT"];
                return;
            }

            // Validation: QTY_OK + QTY_NG == QTY_SELECT (force equality for record)
            if (qtyOK + qtyNG != qtySelect)
            {
                MessageBox.Show($"จำนวนที่ OK ({qtyOK}) + NG ({qtyNG}) ต้องเท่ากับจำนวนที่เลือกตรวจ ({qtySelect}) แล้วค่ะ กรุณาปรับให้รวมกันเท่ากัน");
                // โฟกัสไปที่ QTY_OK หรือ QTY_NG เพื่อแก้ไข
                if (qtyOK > 0) dtg_show_appear.CurrentCell = dtg_show_appear.Rows[dtg_show_appear.Rows.Count - 1].Cells["QTY_OK"];
                else dtg_show_appear.CurrentCell = dtg_show_appear.Rows[dtg_show_appear.Rows.Count - 1].Cells["QTY_NG"];
                return;
            }

            // Set JUDGE อัตโนมัติตาม requirement: NG if qtyNG >=1, else OK
            lastDataRow["JUDGE"] = (qtyNG >= 1) ? "NG" : "OK";

            // Refresh grid เพื่อแสดง JUDGE ใหม่
            dtg_show_appear.Refresh();

            // TODO: บันทึกข้อมูลลงฐานข้อมูล เช่น conQA.SaveAppearData(dataSource, propQA.Batch);
            // MessageBox.Show("บันทึกข้อมูลเรียบร้อย");

            // Optional: หลังบันทึก เพิ่มแถวใหม่สำหรับ input ถัดไป (หรือ clear ถ้าต้องการ)
            // AddNewInputRow(); // method ที่ copy จาก bt_Select แต่ไม่ load dt_show
        }

        // Method หลักสำหรับ Save/Insert ข้อมูล (จัดการ Multi-Task ด้วยการ requery ก่อน insert)
        private void SaveAppearData()
        {
            DataTable dt = (DataTable)dtg_show_appear.DataSource;
            if (dt == null || dt.Rows.Count == 0) return;

            // สมมติ row สุดท้ายคือ row ที่จะบันทึก (input row)
            DataRow inputRow = dt.Rows[dt.Rows.Count - 1];
            string batch = inputRow["BATCH"].ToString();

            // Validate ข้อมูลพื้นฐานใน input row
            if (inputRow["QTY_SELECT"] == DBNull.Value || !int.TryParse(inputRow["QTY_SELECT"].ToString(), out int qtySelect) || qtySelect <= 0)
            {
                MessageBox.Show("กรุณากรอก QTY_SELECT ให้ถูกต้อง (มากกว่า 0)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int qtyOK = inputRow["QTY_OK"] == DBNull.Value ? 0 : Convert.ToInt32(inputRow["QTY_OK"]);
            int qtyNG = inputRow["QTY_NG"] == DBNull.Value ? 0 : Convert.ToInt32(inputRow["QTY_NG"]);

            if (qtyOK + qtyNG != qtySelect)
            {
                MessageBox.Show("QTY_OK + QTY_NG ต้องเท่ากับ QTY_SELECT", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string judge = (qtyNG > 0) ? "0" : "1"; // 0=NG, 1=OK
            string empId = employee.EMP_CODE ?? ""; // Use EMP_CODE as per load
            DateTime inputDate = DateTime.Now;
            int newCount = Convert.ToInt32(inputRow["COUNT"]);

            // Step 1: Requery ข้อมูลล่าสุดจาก DB เพื่อจัดการ Multi-Task (เช็ค concurrent update)
            DataTable latestData = conQA.SearchAppearData(propQA); // Assume method นี้ filter โดย REPORT_NO และ BATCH, INUSE=1
            int currentSumSelect = 0;
            int latestMaxCount = 0;
            bool hasExistingNG = false;

            foreach (DataRow row in latestData.Rows)
            {
                currentSumSelect += Convert.ToInt32(row["QTY_SELECT"]);
                latestMaxCount = Math.Max(latestMaxCount, Convert.ToInt32(row["COUNT"]));
                if (row["JUDGE"].ToString() == "0") // ถ้ามี NG จากก่อนหน้า
                {
                    hasExistingNG = true;
                }
            }

            // ถ้า COUNT ใน input row ไม่ match latestMaxCount +1 (concurrent insert แล้ว)
            if (newCount != latestMaxCount + 1)
            {
                MessageBox.Show($"ข้อมูลถูกอัพเดทโดยผู้ใช้อื่นแล้ว COUNT ใหม่ต้องเป็น {latestMaxCount + 1} กรุณา refresh และกรอกใหม่", "Concurrent Update", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshAppearData(); // เพิ่ม method เพื่อ refresh dtg_show_appear
                return;
            }

            // ถ้ามี NG จาก existing data ห้ามบันทึก
            if (hasExistingNG)
            {
                MessageBox.Show("พบข้อมูล NG จากผู้ใช้อื่นแล้ว ห้ามบันทึกต่อ", "NG Detected", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            // เช็ค projected sum QTY_SELECT ไม่เกิน REMAIN_PACKING_SIZE (currentMaxQty)
            int projectedSumSelect = currentSumSelect + qtySelect;
            if (projectedSumSelect > currentMaxQty)
            {
                MessageBox.Show($"ผลรวม QTY_SELECT ({projectedSumSelect}) เกินจำนวนสุ่มตรวจที่เหลือ ({currentMaxQty}) สำหรับชุด {batch}", "Exceed Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Set properties for insert
            propQA.BATCH = batch;
            propQA.COUNT = newCount.ToString();
            propQA.QTY_SELECT = qtySelect.ToString();
            propQA.QTY_OK = qtyOK.ToString();
            propQA.QTY_NG = qtyNG.ToString();
            propQA.judge = judge;


            // Step 2: Insert ข้อมูลหลักลง DB
            bool insertSuccess = conQA.InsertAppearData(propQA);

            if (!insertSuccess)
            {
                MessageBox.Show("ไม่สามารถบันทึกข้อมูลได้ กรุณาลองใหม่", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // Step 3: ถ้า QTY_NG > 0, Insert รายละเอียด NG
            bool ngDetailsSuccess = true;
            if (qtyNG > 0)
            {
                propQA.dtg_ngMode = dtg_ngMode; // Set for controller
                ngDetailsSuccess = conQA.InsertAppearPendingDetail(propQA);
                if (!ngDetailsSuccess)
                {
                    //MessageBox.Show("บันทึกข้อมูลหลักสำเร็จ แต่รายละเอียด NG ล้มเหลว กรุณาตรวจสอบ", "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

            // Step 4: Post-insert logic based on judge and Allow_Continue
            if (insertSuccess && ngDetailsSuccess)
            {
                // Requery total inspected for this report (across all batches, INUSE=1)
                int totalInspected = conQA.GetTotalInspected(propQA); // SUM(QTY_SELECT) WHERE REPORT_NO=..., INUSE=1
                bool isAllComplete = totalInspected >= Convert.ToInt32(propQA.inspQty); // Total inspection qty

                if (judge == "0") // NG
                {
                    

                    if (propQA.Allow_Continue == "0")
                    {
                        // No continue: Immediate pending

                        // Set status to Pending for NG
                        propQA.TOTAL_STATUS = "6"; // Pending
                        conQA.UpdateReportStatus(propQA); // Update report status

                        MessageBox.Show("พบสิ่งผิดปกติ งานถูกตั้งเป็น Pending", "พบสิ่งผิดปกติ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        bt_Appear_Click(); // Back to select
                    }
                    else
                    {
                        // Allow continue: Check if NG details complete or not
                        // Assume: If NG details inserted but need review, stay and update grid
                        // For now, since details saved, message and stay (or check if all NG modes filled)
                        // Set status to Pending for NG
                        propQA.TOTAL_STATUS = "8"; // Not Finished yet
                        conQA.UpdateReportStatus(propQA); // Update report status

                        MessageBox.Show("ทำต่อได้ แต่พบสิ่งผิดปกติ กรุณาตรวจสอบเพิ่มเติม", "ทำต่อได้ แต่พบสิ่งผิดปกติ", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                        // Update grid for next count if batch not full
                        RefreshAppearData(); // Refresh to add new input row if needed
                        CloseNgMode();
                                             // Stay in current screen, enable for next input
                    }



                }
                else // OK (judge == "1")
                {
                    if (isAllComplete)
                    {
                        // Complete: Set final status if needed (assume "9" for complete, but use existing logic)
                        propQA.TOTAL_STATUS = "9"; // Complete - adjust if needed
                        conQA.UpdateReportStatus(propQA);
                        MessageBox.Show("ทำครบแล้ว", "ทำครบแล้ว", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        bt_Appear_Click(); // Back to select
                    }
                    else
                    {
                        // Not complete: Continue normally
                        MessageBox.Show("ทำต่อได้ปกติ", "ทำต่อได้ปกติ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Clear for next: Refresh packing if switching batch, but since per batch, refresh show grid
                        RefreshAppearData(); // Add new input row
                                             // Reset NG mode
                        isNgModeActive = false;
                        gb_ngMode.Enabled = false;
                        InitializeNgModeDataTable();
                        // Stay in screen
                    }
                }

                // Common: Success message if not already shown
                if (propQA.Allow_Continue == "1" || judge == "1")
                {
                    // Optional: General success
                    // MessageBox.Show("บันทึกข้อมูลสำเร็จ", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

        }

        // Helper: Refresh packing grid with latest data (assume SearchSampleSize updates REMAIN if needed)
        private void RefreshPackingGrid()
        {
            DataTable dt = ConvertToSafeDataTable(conQA.SearchSampleSize(propQA));
            dtg_packing_size_appear.DataSource = dt;
            if (dtg_packing_size_appear.Columns["PACK_COUNT"] != null) dtg_packing_size_appear.Columns["PACK_COUNT"].HeaderText = "จำนวนแพ็ค";
            if (dtg_packing_size_appear.Columns["VALUE"] != null) dtg_packing_size_appear.Columns["VALUE"].HeaderText = "ตัว/แพ๊ค";
            if (dtg_packing_size_appear.Columns["PACKING_SIZE"] != null) dtg_packing_size_appear.Columns["PACKING_SIZE"].HeaderText = "ต้องตรวจทั้งหมด";
            if (dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"] != null) dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"].HeaderText = "เหลือตรวจ";
            dtg_packing_size_appear.Enabled = true;
            bt_select_packing_size_appear.Enabled = true;
        }

        // Helper: Clear show grid to empty state (no rows, ready for new batch)
        private void ClearShowGridForNextBatch()
        {
            DataTable emptyDt = new DataTable();
            string[] requiredColumns = { "APPEARANCE_DATE", "BATCH", "COUNT", "QTY_SELECT", "QTY_OK", "QTY_NG", "JUDGE", "EMP_ID" };
            foreach (string colName in requiredColumns)
            {
                emptyDt.Columns.Add(colName, typeof(string));
            }
            dtg_show_appear.DataSource = emptyDt;
            tb_record.Enabled = false;
            gb_input.Enabled = false; // Disable input until select new batch
        }

       

        // Helper method สำหรับจัดการ error state (ERR, red row, disable button)
        private void HandleValidationError(DataRow currentRow, int rowIndex)
        {
            _suppressEvents = true;
            try
            {
                currentRow["JUDGE"] = "ERR";
                dtg_show_appear.Rows[rowIndex].DefaultCellStyle.BackColor = Color.Red;
                tb_record.Enabled = false;  // Disable ปุ่ม Record
            }
            finally
            {
                _suppressEvents = false;
            }
        }

        // Helper method เพื่อ refresh ข้อมูลจาก DB
        private void RefreshAppearData()
        {
            DataTable refreshedDt = conQA.SearchAppearData(propQA);
            // สร้าง DataTable ใหม่เหมือนใน bt_Select_Click
            DataTable dataSource = new DataTable();

            // 1. กำหนดชนิดข้อมูลที่ถูกต้อง
            dataSource.Columns.Add("APPEARANCE_DATE", typeof(string));
            dataSource.Columns.Add("BATCH", typeof(string));
            dataSource.Columns.Add("COUNT", typeof(int));           // เปลี่ยนเป็น int
            dataSource.Columns.Add("QTY_SELECT", typeof(int));      // เปลี่ยนเป็น int
            dataSource.Columns.Add("QTY_OK", typeof(int));          // เปลี่ยนเป็น int
            dataSource.Columns.Add("QTY_NG", typeof(int));          // เปลี่ยนเป็น int
            dataSource.Columns.Add("JUDGE", typeof(string));
            dataSource.Columns.Add("EMP_ID", typeof(string));

            foreach (DataRow row in refreshedDt.Rows)
            {
                DataRow newRow = dataSource.NewRow();

                // 2. ใช้ Convert.ToInt32() และจัดการ DBNull
                newRow["APPEARANCE_DATE"] = row["APPEARANCE_DATE"]?.ToString() ?? DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                newRow["BATCH"] = row["BATCH"]?.ToString() ?? "";
                newRow["COUNT"] = row["COUNT"] is DBNull ? 0 : Convert.ToInt32(row["COUNT"]);
                newRow["QTY_SELECT"] = row["QTY_SELECT"] is DBNull ? 0 : Convert.ToInt32(row["QTY_SELECT"]);
                newRow["QTY_OK"] = row["QTY_OK"] is DBNull ? 0 : Convert.ToInt32(row["QTY_OK"]);
                newRow["QTY_NG"] = row["QTY_NG"] is DBNull ? 0 : Convert.ToInt32(row["QTY_NG"]);
                newRow["JUDGE"] = row["JUDGE"]?.ToString() ?? "1";
                newRow["EMP_ID"] = row["EMP_ID"]?.ToString() ?? "";
                dataSource.Rows.Add(newRow);
            }

            // เพิ่ม row ใหม่สำหรับ input ถ้าต้องการ (COUNT = max +1)
            DataRow newInputRow = dataSource.NewRow();
            newInputRow["APPEARANCE_DATE"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            newInputRow["BATCH"] = propQA.BATCH;

            // คำนวณ maxCount จากข้อมูลที่เพิ่งโหลดมา
            int maxCount = dataSource.Rows.Count > 0 ? dataSource.AsEnumerable().Max(r => r.Field<int>("COUNT")) : 0;

            newInputRow["COUNT"] = (maxCount + 1); // เป็น int
            newInputRow["QTY_SELECT"] = 0;         // **ใช้ 0 แทน DBNull**
            newInputRow["QTY_OK"] = 0;             // **ใช้ 0 แทน DBNull**
            newInputRow["QTY_NG"] = 0;             // **ใช้ 0 แทน DBNull**
            newInputRow["JUDGE"] = string.Empty;
            newInputRow["EMP_ID"] = propQA.EMP_ID;
            dataSource.Rows.Add(newInputRow);

            dtg_show_appear.DataSource = dataSource;
            ApplyRowReadOnly();
            UpdateCurrentTaskLabel();
        }

        private void dtg_show_appear_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 1. กรอง Event ที่ไม่จำเป็น
            if (_suppressEvents || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (e.RowIndex != dtg_show_appear.Rows.Count - 1) return; // เช็คแค่แถวสุดท้าย (Input Row)

            // Ignore changes to JUDGE itself to prevent loop
            if (dtg_show_appear.Columns[e.ColumnIndex].Name == "JUDGE") return;

            _suppressEvents = true; // ล็อค Event ไม่ให้ทำงานซ้อน
            try
            {
                var row = dtg_show_appear.Rows[e.RowIndex];

                // 2. ดึงค่าและแปลงค่าอย่างปลอดภัย (ใช้ 0 ถ้า Null/Error)
                int qtySelect = ParseInt(row.Cells["QTY_SELECT"].Value);
                int qtyOK = ParseInt(row.Cells["QTY_OK"].Value);
                int qtyNG = ParseInt(row.Cells["QTY_NG"].Value);

                // ถ้ามีการแก้ QTY_NG ให้ update logic ของ NG Mode (ถ้ามี function นี้)
                if (dtg_show_appear.Columns[e.ColumnIndex].Name == "QTY_NG")
                {
                    HandleNgQtyChange(qtyNG); // (Method ของคุณที่มีอยู่แล้ว)
                }

                // 3. Logic การตรวจสอบ (Validation Logic)
                bool isValid = true;
                string errorMsg = "";
                string targetColName = ""; // คอลัมน์ที่จะดีดกลับไปถ้าผิด

                // กฏที่ 1: Select ต้องไม่เกิน Max
                if (qtySelect > currentMaxQty)
                {
                    isValid = false;
                    errorMsg = $"จำนวนที่เลือก ({qtySelect}) เกินจำนวนที่เหลือ ({currentMaxQty})";
                    targetColName = "QTY_SELECT";
                }
                // กฏที่ 2: OK + NG ต้องเท่ากับ Select (เฉพาะเมื่อ Select > 0)
                else if (qtySelect > 0 && (qtyOK + qtyNG) != qtySelect)
                {
                    // เราจะไม่ Error ทันทีที่พิมพ์ Select เสร็จ (เพราะยังไม่ได้พิมพ์ OK/NG)
                    // แต่จะเช็คเมื่อพิมพ์ครบ หรือ เกิน
                    if ((qtyOK + qtyNG) > qtySelect)
                    {
                        isValid = false;
                        errorMsg = "ผลรวม OK+NG เกินจำนวนที่เลือก";
                        targetColName = (qtyOK > 0) ? "QTY_OK" : "QTY_NG";
                    }
                    else
                    {
                        // กรณีผลรวมยังไม่ครบ ถือว่ายังไม่เสร็จ ไม่ใช่ Error (แค่ยัง Judge ไม่ได้)
                        // ปล่อยผ่านไปก่อน แต่ JUDGE จะยังไม่ออก
                    }
                }
                // กฏที่ 3: Select ห้ามเป็น 0 (ที่เป็นจุดเกิด Error ของคุณ)
                else if (qtySelect == 0 && dtg_show_appear.Columns[e.ColumnIndex].Name == "QTY_SELECT")
                {
                    // เช็คเฉพาะเมื่อแก้ช่อง Select แล้วใส่ 0
                    isValid = false;
                    // errorMsg = "จำนวนที่เลือกต้องมากกว่า 0"; // ไม่ต้องโชว์ msg ก็ได้ถ้ารำคาญ
                    targetColName = "QTY_SELECT";
                }

                // 4. การแสดงผลและการย้าย Cursor (หัวใจสำคัญของการแก้ Reentrant)
                if (!isValid)
                {
                    row.DefaultCellStyle.BackColor = Color.MistyRose; // สีแดงอ่อนๆ เตือน
                    row.Cells["JUDGE"].Value = "ERR";
                    tb_record.Enabled = false;

                    // *** แก้ไขจุด Reentrant Error ตรงนี้ ***
                    if (!string.IsNullOrEmpty(targetColName))
                    {
                        // ใช้ BeginInvoke เพื่อดีด Cursor กลับไปหลังจากจบ Event นี้
                        this.BeginInvoke(new Action(() =>
                        {
                            if (dtg_show_appear.Rows.Count > e.RowIndex)
                            {
                                dtg_show_appear.CurrentCell = dtg_show_appear.Rows[e.RowIndex].Cells[targetColName];
                                dtg_show_appear.BeginEdit(true); // เปิดโหมดพิมพ์ทันที เพื่อความลื่นไหล
                            }
                        }));
                    }

                    if (!string.IsNullOrEmpty(errorMsg))
                    {
                        // แสดง Tooltip หรือ lblStatus แทน MessageBox จะลื่นไหลกว่า
                        // MessageBox.Show(errorMsg); 
                    }
                }
                else
                {
                    // 5. กรณีข้อมูลถูกต้อง (Valid Case)
                    row.DefaultCellStyle.BackColor = Color.White;

                    // คำนวณ Judge เฉพาะเมื่อผลรวมถูกต้องเป๊ะๆ
                    if (qtySelect > 0 && (qtyOK + qtyNG) == qtySelect)
                    {
                        string judge = (qtyNG > 0) ? "NG" : "OK";
                        row.Cells["JUDGE"].Value = judge;
                        UpdateCurrentTaskLabel();
                        tb_record.Enabled = true;

                        // Logic เปิด/ปิด NG Mode
                        if (judge == "NG")
                        {
                            totalNgRequired = qtyNG;
                            OpenNgMode(totalNgRequired);
                            tb_record.Enabled = false; // ต้องไปกรอก NG details ก่อน
                        }
                        else
                        {
                            CloseNgMode();
                            tb_record.Enabled = true;
                        }
                    }
                    else
                    {
                        // ข้อมูลยังไม่ครบ (เช่น พิมพ์ Select 10 แต่ OK 0 NG 0)
                        row.Cells["JUDGE"].Value = "";
                        UpdateCurrentTaskLabel();
                        tb_record.Enabled = false;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Calc Error: " + ex.Message);
            }
            finally
            {
                _suppressEvents = false; // ปลดล็อคเสมอ
            }
        }

        // Helper function เล็กๆ เพื่อ Code ที่สะอาดขึ้น
        private int ParseInt(object value)
        {
            if (value == null || value == DBNull.Value) return 0;
            return int.TryParse(value.ToString(), out int i) ? i : 0;
        }

        // เพิ่ม event นี้เพื่อ handle การคลิกนอกเซลล์หรือ tab out เพื่อ commit changes
        private void dtg_show_appear_CurrentCellChanged(object sender, EventArgs e)
        {
            // เปลี่ยน EndEdit เป็น CancelEdit เพื่อยกเลิกค่าที่ไม่สมบูรณ์ระหว่าง Scroll
            if (dtg_show_appear.IsCurrentCellInEditMode)
            {
                try
                {
                    dtg_show_appear.CancelEdit();  // ยกเลิกการแก้ไขที่ยังค้างอยู่
                }
                catch
                {
                    // อาจเกิด error ถ้า Grid กำลังอยู่ในสถานะวาดซ้ำซ้อน 
                    // ให้ EndEdit เป็น Fallback
                    dtg_show_appear.EndEdit();
                }
            }
        }

        // And add this method if using CellEndEdit
        private void dtg_show_appear_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == dtg_show_appear.Rows.Count - 1 && e.ColumnIndex >= 0)
            {
                // Trigger validation after edit ends
                dtg_show_appear_CellValueChanged(sender, e);
            }
        }

        //// ปรับ dtg_show_appear_CellValidating
        //private void dtg_show_appear_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        //{
        //    if (_suppressEvents || e.RowIndex < 0 || e.RowIndex != dtg_show_appear.Rows.Count - 1) return;
        //    if (e.ColumnIndex < 0) return;

        //    DataGridViewColumn column = dtg_show_appear.Columns[e.ColumnIndex];
        //    string inputValue = e.FormattedValue?.ToString() ?? "";
        //    DataTable dataSource = (DataTable)dtg_show_appear.DataSource;
        //    if (dataSource == null || e.RowIndex >= dataSource.Rows.Count) return;

        //    DataRow currentRow = dataSource.Rows[e.RowIndex];

        //    if (column.Name == "QTY_SELECT" || column.Name == "QTY_OK" || column.Name == "QTY_NG")
        //    {
        //        // *** เพิ่ม Regex check: ต้องเป็น digits เท่านั้น (no decimal, no letters) ***
        //        if (!string.IsNullOrEmpty(inputValue) && !Regex.IsMatch(inputValue, @"^\d*$"))
        //        {
        //            e.Cancel = true;
        //            HandleValidationError(currentRow, e.RowIndex);
        //            // Optional: MessageBox.Show("กรุณากรอกเฉพาะตัวเลข (ไม่มีจุดทศนิยมหรือตัวอักษร)", "Warning");
        //            return;
        //        }

        //        if (!int.TryParse(inputValue, out int parsedValue))
        //        {
        //            e.Cancel = true;
        //            HandleValidationError(currentRow, e.RowIndex);
        //            return;
        //        }

        //        if (parsedValue < 0)
        //        {
        //            e.Cancel = true;
        //            HandleValidationError(currentRow, e.RowIndex);
        //            return;
        //        }

        //        // Existing range check...
        //        if (column.Name == "QTY_SELECT" && parsedValue > currentMaxQty)
        //        {
        //            // Handle as before
        //        }
        //    }
        //}

        private void dtg_show_appear_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (_suppressEvents || e.RowIndex < 0 || e.RowIndex != dtg_show_appear.Rows.Count - 1) return;

            string colName = dtg_show_appear.Columns[e.ColumnIndex].Name;
            string inputValue = e.FormattedValue?.ToString() ?? "";

            if (colName == "QTY_SELECT" || colName == "QTY_OK" || colName == "QTY_NG")
            {
                int parsedValue = 0;  // *** แก้: Declare ก่อน ด้วย default 0 ***

                // *** แก้: ใช้ out parsedValue (assign ถ้าสำเร็จ, else 0) ***
                if (!string.IsNullOrEmpty(inputValue) &&
                    (!int.TryParse(inputValue, out parsedValue) || parsedValue < 0))
                {
                    e.Cancel = true;
                    // Optional: HandleValidationError(currentRow, e.RowIndex);  // ถ้ามี method นี้
                    // MessageBox.Show("กรุณากรอกเฉพาะตัวเลขบวก", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Existing range check (safe แล้วเพราะ parsedValue assigned)
                if (colName == "QTY_SELECT" && parsedValue > currentMaxQty)
                {
                    e.Cancel = true;
                    // Handle as before (e.g., MessageBox or clear cell)
                    return;
                }
            }
        }

        private void bt_Clear_Click(object sender, EventArgs e)
        {
            // ปลด lock grid ด้านบนเพื่อให้เลือกใหม่ได้
            RefreshPackingGrid();

            // Clear ข้อมูลด้านล่าง: ล้าง DataSource และรีเซ็ต grid
            dtg_show_appear.DataSource = null;
            dtg_show_appear.Rows.Clear();  // ล้าง rows ถ้าจำเป็น (backup)
            dtg_show_appear.Refresh();

            // Disable groupbox และปุ่มต่างๆ (เช่น Record Data ถ้ามี)
            gb_input.Enabled = false;
            // ถ้ามีปุ่ม Record Data: bt_Record.Enabled = false; (ปรับชื่อปุ่มจริง)
            // ถ้ามีปุ่มอื่นๆ ที่เกี่ยวข้อง: bt_Other.Enabled = false;

            // Reset สถานะอื่นๆ ถ้าจำเป็น (เช่น currentMaxQty = 0;)
            currentMaxQty = 0;
            maxQty = 0;
            propQA.BATCH = string.Empty;
            ResetCurrentTaskLabel();
            CloseNgMode();

            // Clear selection ใน grid ด้านบนถ้าต้องการ (optional)
            dtg_packing_size_appear.ClearSelection();
            label3.Text = "เลือกชุดที่มีจำนวนเหลือตรวจมากกว่า 0";

            // โฟกัสกลับไปที่ grid ด้านบนเพื่อเลือกใหม่
            dtg_packing_size_appear.Focus();
        }

        private void userControlAppear_Leave(object sender, EventArgs e)
        {
            CleanupEvents();  // Optional: ถ้าต้องการ unsubscribe เมื่อ focus ออกจาก control
        }

        private void userControlAppear_VisibleChanged(object sender, EventArgs e)
        {
            if (!this.Visible)
            {
                CleanupEvents();  // Unsubscribe เมื่อ hidden (ก่อน switch)
            }
        }

        private void OpenNgMode(int requiredNgQty)
        {
            totalNgRequired = requiredNgQty;
            isNgModeActive = true;
            gb_ngMode.Enabled = true;
            label2.Text = $"ระบุอาการเสียแล้ว: 0 / {totalNgRequired} ชิ้น";

            // Suspend layout และ binding ก่อน reset
            dtg_ngMode.SuspendLayout();
            if (dtg_ngMode.DataSource != null)
            {
                var cm = (CurrencyManager)this.BindingContext[dtg_ngMode.DataSource];
                if (cm != null)
                {
                    cm.SuspendBinding();
                }
            }

            // ย้าย DataSource = null ขึ้นก่อนเพื่อตัด binding ทันที
            dtg_ngMode.DataSource = null;

            // Force cancel edit ถ้ากำลัง edit (ใช้ CancelEdit เพื่อไม่ commit)
            if (dtg_ngMode.IsCurrentCellInEditMode)
            {
                dtg_ngMode.CancelEdit();  // ยกเลิก edit โดยไม่ commit
            }

            // Clear rows/columns/selection ก่อน set CurrentCell
            dtg_ngMode.Rows.Clear();
            dtg_ngMode.Columns.Clear();
            dtg_ngMode.ClearSelection();

            // Set CurrentCell = null หลัง clear (wrap try-catch ถ้ายัง error)
            try
            {
                dtg_ngMode.CurrentCell = null;
            }
            catch (InvalidOperationException)
            {
                // Fallback: ถ้า error ยังเกิด, ignore และ refresh แทน (grid จะ reset เองหลัง DataSource = null)
                dtg_ngMode.Refresh();
            }

            // Refresh เพื่อ clean state
            dtg_ngMode.Refresh();

            // Resume binding
            if (dtg_ngMode.DataSource != null)
            {
                var cm = (CurrencyManager)this.BindingContext[dtg_ngMode.DataSource];
                if (cm != null) cm.ResumeBinding();
            }

            // สร้าง DataTable ใหม่
            DataTable ngDt = new DataTable();
            ngDt.Columns.Add("QTY_NG", typeof(int));
            ngDt.Columns.Add("NG_DETAIL", typeof(string));

            // เพิ่ม row ว่าง
            for (int i = 0; i < 1; i++)
            {
                DataRow newRow = ngDt.NewRow();
                newRow["QTY_NG"] = 0;
                newRow["NG_DETAIL"] = string.Empty;
                ngDt.Rows.Add(newRow);
            }

            // Set DataSource หลัง reset เสร็จ
            dtg_ngMode.DataSource = ngDt;

            // Auto-generate columns จาก DataTable
            dtg_ngMode.AutoGenerateColumns = true;
            ConfigureNgModeGridColumns();

            // Make editable
            if (dtg_ngMode.Rows.Count > 0)
            {
                dtg_ngMode.Rows[0].ReadOnly = false;
            }

            // Resume layout
            dtg_ngMode.ResumeLayout();

            // Final refresh
            dtg_ngMode.Refresh();
        }

        private void CloseNgMode()
        {
            isNgModeActive = false;
            gb_ngMode.Enabled = false;
            dtg_ngMode.DataSource = null;  // Clear data
            totalNgRequired = 0;
            label2.Text = "ระบุอาการเสียแล้ว: 0 / 0 ชิ้น";
        }

        // Handle การเปลี่ยน QTY_NG ใน dtg_show_appear (เฉพาะเมื่อ JUDGE == "NG")
        private void HandleNgQtyChange(int newQtyNg)
        {
            if (isNgModeActive && newQtyNg != totalNgRequired)
            {
                // Clear dtg_ngMode แต่ keep open
                DataTable currentNgDt = (DataTable)dtg_ngMode.DataSource;
                if (currentNgDt != null)
                {
                    currentNgDt.Clear();
                    // Re-add empty rows if needed
                    for (int i = 0; i < 1; i++)
                    {
                        DataRow newRow = currentNgDt.NewRow();
                        newRow["QTY_NG"] = DBNull.Value;
                        newRow["NG_DETAIL"] = string.Empty;
                        currentNgDt.Rows.Add(newRow);
                    }
                }
                totalNgRequired = newQtyNg;
            }
        }

        // Events สำหรับ dtg_ngMode
        private void dtg_ngMode_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (_suppressNgEvents || e.RowIndex < 0 || e.ColumnIndex < 0) return;

            DataTable ngDt = (DataTable)dtg_ngMode.DataSource;
            if (ngDt == null) return;

            _suppressNgEvents = true;
            try
            {
                // Call UpdateNgSum to check sum and enable button if equals total
                UpdateNgSum();

                UpdateNgSumDisplay();  // Update UI sum
            }
            finally
            {
                _suppressNgEvents = false;
            }
        }

        //private void dtg_ngMode_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        //{
        //    if (_suppressNgEvents || e.RowIndex < 0) return;

        //    if (e.ColumnIndex == dtg_ngMode.Columns["QTY_NG"].Index)
        //    {
        //        string inputValue = e.FormattedValue?.ToString() ?? "";
        //        if (!int.TryParse(inputValue, out int qty) || qty <= 0)
        //        {
        //            e.Cancel = true;
        //            // ไม่แสดง MessageBox ที่นี่ เพื่อรอ commit แล้วค่อย validate ใน ValueChanged
        //        }
        //    }
        //    else if (e.ColumnIndex == dtg_ngMode.Columns["NG_MODE"].Index)
        //    {
        //        // Optional: Validate NG_MODE not empty, but defer to record time
        //    }
        //}

        // Optional: ปุ่ม Add Row ใน NG Mode
        private void bt_addNgRow_Click(object sender, EventArgs e)
        {
            if (!isNgModeActive) return;

            DataTable ngDt = (DataTable)dtg_ngMode.DataSource;
            DataRow newRow = ngDt.NewRow();
            newRow["QTY_NG"] = 0;
            newRow["NG_DETAIL"] = string.Empty;
            ngDt.Rows.Add(newRow);
        }

        // Update sum display (สมมติมี Label lb_ngSum)
        private void UpdateNgSumDisplay()
        {
            int sumNg = GetNgSum();  // คำนวณ sum ครั้งเดียวจาก GetNgSum

            label2.Text = $"ระบุอาการเสียแล้ว: {sumNg} / {totalNgRequired} ชิ้น";

            // Button logic ย้ายมาที่นี่เพื่อ consistency (enable ถ้า sum == total, disable ถ้า > หรือ < )
        }

        // Event for adding rows and focusing after edit ends (defer focus to avoid reentrancy)
        private void dtg_ngMode_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || _suppressNgEvents) return;

            DataTable ngDt = (DataTable)dtg_ngMode.DataSource;
            if (ngDt == null || e.RowIndex >= ngDt.Rows.Count) return;

            DataRow row = ngDt.Rows[e.RowIndex];

            // Force end/cancel edit before proceeding
            if (dtg_ngMode.IsCurrentCellInEditMode)
            {
                try
                {
                    dtg_ngMode.EndEdit();
                }
                catch
                {
                    dtg_ngMode.CancelEdit();
                }
            }

            _suppressNgEvents = true;
            try
            {
                // If QTY_NG edited, focus to NG detail of same row
                if (e.ColumnIndex == dtg_ngMode.Columns["QTY_NG"].Index && row["QTY_NG"] != DBNull.Value)
                {
                    // Clear red for NG detail
                    dtg_ngMode.Rows[e.RowIndex].Cells["NG_DETAIL"].Style.BackColor = Color.White;

                    // Defer focus to NG detail with timer to avoid reentrancy
                    System.Windows.Forms.Timer focusTimer = new System.Windows.Forms.Timer();
                    focusTimer.Interval = 50;
                    focusTimer.Tick += (s, args) =>
                    {
                        focusTimer.Stop();
                        focusTimer.Dispose();

                        if (dtg_ngMode.Rows.Count > e.RowIndex && dtg_ngMode.Rows[e.RowIndex].Cells["NG_DETAIL"] != null)
                        {
                            try
                            {
                                dtg_ngMode.ClearSelection();
                                dtg_ngMode.CurrentCell = dtg_ngMode.Rows[e.RowIndex].Cells["NG_DETAIL"];
                            }
                            catch
                            {
                                // Ignore - user can click manually
                            }
                        }
                    };
                    focusTimer.Start();
                }
                // If NG detail edited, check if complete and add row if needed
                else if (e.ColumnIndex == dtg_ngMode.Columns["NG_DETAIL"].Index)
                {
                    string mode = row["NG_DETAIL"]?.ToString()?.Trim() ?? "";
                    object qtyObj = row["QTY_NG"];

                    // Clear red if complete
                    if (qtyObj != DBNull.Value && Convert.ToInt32(qtyObj) > 0 && !string.IsNullOrEmpty(mode))
                    {
                        dtg_ngMode.Rows[e.RowIndex].Cells["NG_DETAIL"].Style.BackColor = Color.White;
                        ClearAllRedHighlights();
                    }

                    UpdateNgSum();  // Update sum

                    // Add row only if current row complete and sum < total
                    if (qtyObj != DBNull.Value && Convert.ToInt32(qtyObj) > 0 && !string.IsNullOrEmpty(mode) && GetNgSum() < totalNgRequired)
                    {
                        DataRow newRow = ngDt.NewRow();
                        newRow["QTY_NG"] = DBNull.Value;
                        newRow["NG_DETAIL"] = string.Empty;
                        ngDt.Rows.Add(newRow);

                        // Scroll to new row
                        dtg_ngMode.FirstDisplayedScrollingRowIndex = e.RowIndex + 1;
                    }
                }
            }
            finally
            {
                _suppressNgEvents = false;
            }

            UpdateNgSumDisplay();  // Update UI
        }

        // Updated UpdateNgSum (remove non-numeric validation, only calculate sum and check MODE for button enable)
        private void UpdateNgSum()
        {
            DataTable ngDt = (DataTable)dtg_ngMode.DataSource;
            if (ngDt == null || !isNgModeActive) return;

            int totalNgSum = GetNgSum();  // ใช้ GetNgSum เพื่อ consistency

            // Check if all MODE are filled for rows with QTY >0
            bool allModesFilled = true;
            for (int i = 0; i < ngDt.Rows.Count; i++)
            {
                DataRow row = ngDt.Rows[i];
                int qty = row["QTY_NG"] is DBNull ? 0 : Convert.ToInt32(row["QTY_NG"]);
                string mode = row["NG_DETAIL"] is DBNull ? "" : row["NG_DETAIL"].ToString().Trim();
                if (qty > 0 && string.IsNullOrEmpty(mode))
                {
                    allModesFilled = false;
                    break;
                }
            }

            // ถ้า total > required, warn และ remove last row (คล้าย packing_size)
            if (totalNgSum > totalNgRequired)
            {
                MessageBox.Show($"ผลรวม QTY NG ({totalNgSum}) เกินจำนวนงานเสียที่กำหนด ({totalNgRequired})", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                dtg_ngMode.BeginInvoke(new Action(() =>
                {
                    _suppressNgEvents = true;  // Suppress events during removal
                    try
                    {
                        while (ngDt.Rows.Count > 0 && GetNgSum() > totalNgRequired)
                        {
                            DataRow lastRow = ngDt.Rows[ngDt.Rows.Count - 1];
                            if (!lastRow.HasVersion(DataRowVersion.Original))  // Check if not new row
                            {
                                ngDt.Rows.Remove(lastRow);
                            }
                            else
                            {
                                break;
                            }
                        }
                        dtg_ngMode.AllowUserToAddRows = true;  // Re-enable
                    }
                    finally
                    {
                        _suppressNgEvents = false;
                    }
                    UpdateNgSumDisplay();  // Refresh sum after remove
                }));
                tb_record.Enabled = false;  // Disable when over
            }
            else if (totalNgSum == totalNgRequired && allModesFilled)
            {
                // พอดีและ MODE ครบ: ปิด add row และ enable record
                dtg_ngMode.AllowUserToAddRows = false;
                tb_record.Enabled = true;  // Enable record
            }
            else
            {
                // ยังไม่พอ หรือ MODE ไม่ครบ: เปิด add row และ disable record
                dtg_ngMode.AllowUserToAddRows = true;
                tb_record.Enabled = false;  // Disable จนกว่าจะพอดีและ MODE ครบ
            }
        }


        // Updated GetNgSum เพื่อ accuracy (calculate from DataTable after validation)
        private int GetNgSum()
        {
            DataTable ngDt = (DataTable)dtg_ngMode.DataSource;
            if (ngDt == null) return 0;

            int sum = 0;
            foreach (DataRow row in ngDt.Rows)
            {
                if (row["QTY_NG"] != DBNull.Value)
                {
                    int qty = Convert.ToInt32(row["QTY_NG"]);
                    if (qty > 0) sum += qty;  // Only add if >0
                }
            }
            return sum;
        }

        // Event สำหรับ RowValidating คล้าย packing_size (validate ก่อน leave row)
        private void dtg_ngMode_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if(e.RowIndex < 0 || dtg_ngMode.Rows[e.RowIndex].IsNewRow) return;

            var row = dtg_ngMode.Rows[e.RowIndex];
            var qtyCell = row.Cells["QTY_NG"].Value;
            var modeCell = row.Cells["NG_DETAIL"].Value;

            if (qtyCell != null && int.TryParse(qtyCell.ToString(), out int qty) && qty > 0 &&
                string.IsNullOrWhiteSpace(modeCell?.ToString()))
            {
                // ไม่ e.Cancel = true; (ให้ user leave แล้ว warn ใน ValueChanged)
                // MessageBox.Show("กรุณากรอก NG MODE", "Warning");
            }
        }

        // Helper method เพื่อ clear all red highlights ใน NG detail cells
        private void ClearAllRedHighlights()
        {
            foreach (DataGridViewRow row in dtg_ngMode.Rows)
            {
                row.Cells["NG_DETAIL"].Style.BackColor = Color.White;
            }
        }

        // *** ปรับ DataError Handler สำหรับ dtg_ngMode: Suppress กว้างขึ้น ***
        private void dtg_ngMode_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Suppress ทุก FormatException + wrong type
            if (e.Exception != null && (
                e.Exception is FormatException ||
                e.Exception is InvalidCastException ||
                e.Exception.Message.Contains("wrong type") ||
                e.Context == DataGridViewDataErrorContexts.Commit ||
                e.Context == DataGridViewDataErrorContexts.Formatting))
            {
                e.Cancel = true;
                e.ThrowException = false;

                // Focus กลับ
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    dtg_ngMode.CurrentCell = dtg_ngMode[e.ColumnIndex, e.RowIndex];
                    dtg_ngMode.BeginEdit(true);
                }
                return;
            }
            e.Cancel = false;
        }

        // ปรับ dtg_ngMode_CellValidating (เพิ่ม Regex เหมือนกัน)
        private void dtg_ngMode_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (_suppressNgEvents || e.RowIndex < 0) return;

            if (e.ColumnIndex == dtg_ngMode.Columns["QTY_NG"].Index)
            {
                string inputValue = e.FormattedValue?.ToString() ?? "";

                int qty = 0;  // *** แก้: Declare ก่อน ***

                if (!string.IsNullOrEmpty(inputValue) &&
                    (!int.TryParse(inputValue, out qty) || qty <= 0))  // *** ใช้ out qty ***
                {
                    e.Cancel = true;
                    // MessageBox.Show("QTY NG ต้องมากกว่า 0", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }

        // *** DataError Handler สำหรับ dtg_show_appear: Suppress ทุก FormatException ***
        private void dtg_show_appear_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // Suppress dialog ถ้าเป็น parsing/format error (ครอบคลุมทุก case ที่ "wrong type")
            if (e.Exception != null && (
                e.Exception is FormatException ||
                e.Exception is InvalidCastException ||
                e.Exception.Message.Contains("wrong type") ||
                e.Context == DataGridViewDataErrorContexts.Commit ||
                e.Context == DataGridViewDataErrorContexts.Formatting))
            {
                e.Cancel = true;  // ยกเลิก event ไม่ให้ dialog ขึ้น
                e.ThrowException = false;  // ไม่ throw exception ต่อ

                // *** เพิ่ม: ล้างค่าใน EditingControl (ถ้ามี) ***
                if (dtg_show_appear.EditingControl != null && dtg_show_appear.EditingControl is System.Windows.Forms.TextBox tb)
                {
                    tb.Text = "0"; // ตั้งค่าเริ่มต้นที่ปลอดภัย
                }

                // *** เพิ่ม: พยายามตั้งค่าใน DataRow เป็น 0 (ถ้าทำได้) ***
                try
                {
                    // ถ้าเป็นคอลัมน์ int ให้ตั้งค่าเป็น 0
                    if (dtg_show_appear.Columns[e.ColumnIndex].ValueType == typeof(int))
                    {
                        // บังคับให้ commit ค่า 0 ที่เพิ่งใส่เข้าไป (อาจต้องใช้ BeginInvoke ถ้าอยู่ใน context ที่ไวต่อ threading)
                        dtg_show_appear.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = 0;
                    }
                }
                catch { /* Ignore error during cleanup */ }

                // Optional: Focus กลับ cell เพื่อแก้ไข (ไม่ให้ user สับสน)
                if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                {
                    dtg_show_appear.CurrentCell = dtg_show_appear[e.ColumnIndex, e.RowIndex];
                    dtg_show_appear.BeginEdit(true);
                }
                return;
            }
            // ถ้า error อื่น (critical) ให้ default
            e.Cancel = false;
        }

        // *** สำหรับ dtg_show_appear ***
        private void dtg_show_appear_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            System.Windows.Forms.TextBox tb = e.Control as System.Windows.Forms.TextBox;
            if (tb != null && dtg_show_appear.CurrentCell != null)
            {
                string colName = dtg_show_appear.Columns[dtg_show_appear.CurrentCell.ColumnIndex].Name;
                if (colName == "QTY_SELECT" || colName == "QTY_OK" || colName == "QTY_NG")
                {
                    tb.KeyPress -= qtyTextBox_KeyPress;  // Unsubscribe to avoid multiples
                    tb.KeyPress += qtyTextBox_KeyPress;  // Subscribe: Block non-digits
                }
            }
        }

        // *** สำหรับ dtg_ngMode ***
        private void dtg_ngMode_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            System.Windows.Forms.TextBox tb = e.Control as System.Windows.Forms.TextBox;
            if (tb != null && dtg_ngMode.CurrentCell.ColumnIndex == dtg_ngMode.Columns["QTY_NG"].Index)
            {
                tb.KeyPress -= qtyTextBox_KeyPress;
                tb.KeyPress += qtyTextBox_KeyPress;
            }
        }

        // *** Shared KeyPress: Block non-digits (digits + control keys เท่านั้น) ***
        private void qtyTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;  // Block non-digit
            }
            if (e.KeyChar == '.') e.Handled = true;  // Block decimal
        }

        // ใน dtg_packing_size_appear_DataError:

        private void dtg_packing_size_appear_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // เรา Clean Data มาดีแล้ว แต่ถ้ายังหลุดมา ให้แค่ Cancel Dialog พอ
            if (e.Exception != null)
            {
                e.Cancel = true;          // หยุด Dialog
                e.ThrowException = false; // ไม่พ่น Error ต่อ
            }
        }

        // *** EditingControlShowing สำหรับ dtg_packing_size_appear: Block non-digit ใน numeric columns ***
        private void dtg_packing_size_appear_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            System.Windows.Forms.TextBox tb = e.Control as System.Windows.Forms.TextBox;
            if (tb != null)
            {
                string colName = dtg_packing_size_appear.Columns[dtg_packing_size_appear.CurrentCell.ColumnIndex].Name;
                if (colName == "PACK_COUNT" || colName == "REMAIN_PACKING_SIZE" || colName == "VALUE")
                {
                    tb.KeyPress -= qtyTextBox_KeyPress;
                    tb.KeyPress += qtyTextBox_KeyPress;
                }
            }
        }

        private void dtg_packing_size_appear_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            string colName = dtg_packing_size_appear.Columns[e.ColumnIndex].Name;

            // ตรวจสอบเฉพาะคอลัมน์ตัวเลข
            if (colName == "PACK_COUNT" || colName == "REMAIN_PACKING_SIZE" || colName == "VALUE")
            {
                string inputValue = e.FormattedValue?.ToString() ?? "";

                // ถ้าว่าง ให้ผ่านไป (เดี๋ยว CellFormatting แสดงเป็น 0 เอง หรือ DataError จัดการ)
                if (string.IsNullOrEmpty(inputValue)) return;

                // ถ้าไม่ใช่ตัวเลข ให้ Block
                if (!int.TryParse(inputValue, out int val) || val < 0)
                {
                    e.Cancel = true; // ห้ามออกจากเซลล์จนกว่าจะถูก
                    MessageBox.Show("กรุณากรอกตัวเลขที่ถูกต้อง", "Input Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        // เพิ่ม Method นี้ใน Class userControlAppear
        private DataTable ConvertToSafeDataTable(DataTable originalDt)
        {
            DataTable safeDt = new DataTable();

            // 1. สร้างโครงสร้าง Column ใหม่ที่ระบุ Type ชัดเจน
            safeDt.Columns.Add("BATCH", typeof(string));
            safeDt.Columns.Add("PACK_COUNT", typeof(int));           // บังคับ int
            safeDt.Columns.Add("VALUE", typeof(int));                // บังคับ int
            safeDt.Columns.Add("PACKING_SIZE", typeof(int));         // บังคับ int
            safeDt.Columns.Add("REMAIN_PACKING_SIZE", typeof(int));  // บังคับ int

            if (originalDt == null || originalDt.Rows.Count == 0) return safeDt;

            // 2. วนลูปยัดข้อมูลและแปลง Type ให้ปลอดภัย
            foreach (DataRow oldRow in originalDt.Rows)
            {
                DataRow newRow = safeDt.NewRow();

                // BATCH
                newRow["BATCH"] = oldRow["BATCH"]?.ToString() ?? "";

                // PACK_COUNT
                newRow["PACK_COUNT"] = ParseIntSafe(oldRow["PACK_COUNT"]);

                // VALUE
                newRow["VALUE"] = ParseIntSafe(oldRow["VALUE"]);

                // PACKING_SIZE
                newRow["PACKING_SIZE"] = ParseIntSafe(oldRow["PACKING_SIZE"]);

                // REMAIN_PACKING_SIZE (ตัวปัญหา)
                newRow["REMAIN_PACKING_SIZE"] = ParseIntSafe(oldRow["REMAIN_PACKING_SIZE"]);

                safeDt.Rows.Add(newRow);
            }

            return safeDt;
        }

        // Helper เล็กๆ สำหรับแปลงค่าเป็น int อย่างปลอดภัย (Null/String ว่าง จะได้ 0)
        private int ParseIntSafe(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return 0;
            }
            if (int.TryParse(value.ToString(), out int result))
            {
                return result;
            }
            return 0; // ถ้าแปลงไม่ได้ให้เป็น 0
        }

    }
}
