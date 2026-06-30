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
        private int selectedPackCount = 1;
        private int selectedPackSequence = 0;
        private int selectedBatchSampleTotal = 0;
        private int selectedPackingValue = 0;
        private int selectedLotSize = 0;
        private int selectedPackSavedQtyAtSelection = 0;
        private int samplePerPackQty = 0;
        private int currentEntryMaxQty = 0;

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
        private DataTable ngModeList;

        public userControlAppear()
        {

            InitializeComponent();
            ngModeList = NgModeHelper.LoadNgModeList();

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
            dtg_packing_size_appear.CurrentCellDirtyStateChanged += dtg_packing_size_appear_CurrentCellDirtyStateChanged;
            dtg_packing_size_appear.CellValueChanged += dtg_packing_size_appear_CellValueChanged;

            dtg_show_appear.CellValueChanged += dtg_show_appear_CellValueChanged;
            dtg_show_appear.CellFormatting += dtg_show_appear_CellFormatting;
            dtg_show_appear.Paint += dtg_show_appear_Paint;
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

            dtg_show_appear.AlternatingRowsDefaultCellStyle.BackColor = Color.White;

            // ใน userControlAppear_Load หรือ constructor หลัง InitializeComponent()
            dtg_ngMode.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;  // Enter edit เฉพาะเมื่อพิมพ์หรือ F2, ไม่ auto-enter
            dtg_ngMode.StandardTab = true;  // Allow tab navigation without edit issues
            dtg_ngMode.VirtualMode = false;  // ใช้ normal mode ถ้าไม่ virtual
            dtg_ngMode.AllowUserToAddRows = true;  // ป้องกัน auto-add row ที่ conflict


        }


        private List<string> GetReportLotNoList()
        {
            List<string> lotNoList = new List<string>();
            if (propQA.dtLotNo != null && propQA.dtLotNo.Columns.Contains("LOT_NO"))
            {
                foreach (DataRow row in propQA.dtLotNo.Rows)
                {
                    string lotNo = row["LOT_NO"]?.ToString()?.Trim();
                    if (!string.IsNullOrWhiteSpace(lotNo) && !lotNoList.Contains(lotNo))
                    {
                        lotNoList.Add(lotNo);
                    }
                }
            }

            return lotNoList;
        }

        private List<string> GetReportLotNoOptions()
        {
            List<string> options = new List<string> { string.Empty };
            options.AddRange(GetReportLotNoList());
            return options;
        }

        private string GetDefaultLotNoForPackingRow()
        {
            List<string> lotNoList = GetReportLotNoList();
            return lotNoList.Count == 1 ? lotNoList[0] : string.Empty;
        }

        private bool TrySetSelectedPackingLotNo(DataGridViewRow selectedRow)
        {
            if (selectedRow == null || !dtg_packing_size_appear.Columns.Contains("LOT_NO"))
            {
                MessageBox.Show("ไม่พบช่อง Lot No. ในรายการ Appearance", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            string lotNo = selectedRow.Cells["LOT_NO"].Value?.ToString()?.Trim() ?? string.Empty;
            List<string> lotNoList = GetReportLotNoList();
            if (lotNoList.Count == 0)
            {
                MessageBox.Show("ไม่พบ Lot No. จาก Packing สำหรับ Report นี้", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(lotNo) || !lotNoList.Contains(lotNo))
            {
                MessageBox.Show("กรุณาเลือก Lot No. ของ row ที่จะตรวจ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                dtg_packing_size_appear.CurrentCell = selectedRow.Cells["LOT_NO"];
                dtg_packing_size_appear.BeginEdit(true);
                return false;
            }

            propQA.Lot_No = lotNo;
            return true;
        }

        private bool SaveAppearanceProcessLotNo()
        {
            if (string.IsNullOrWhiteSpace(propQA.Lot_No))
            {
                MessageBox.Show("กรุณาเลือก Lot No. ก่อนบันทึก Appearance", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            propQA.process = "Appearance_Check";
            if (!conQA.UpdateReportProcessLotNo(propQA))
            {
                MessageBox.Show("ไม่สามารถบันทึก Lot No. ของ Appearance ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
        private void userControlAppear_Load(object sender, EventArgs e)
        {

            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_inspQty.Text = "Inspection Qty : " + propQA.inspQty;
            lb_lotSize.Text = "Lot Size : " + propQA.Qty;
            ConfigureAllInspectionCount();
            //lb_sampName.Text = propQA.SAMPLING_QTY + " " + propQA.SAMPLING_NAME;
            propQA.EMP_ID = employee.EMP_CODE;
            DataTable dt = new DataTable();

            dtg_packing_size_appear.SuspendLayout();

            try
            {
                // 1. ดึงข้อมูลดิบ
                DataTable rawDt = conQA.SearchSampleSize(propQA);

                // 2. แปลงข้อมูลให้ปลอดภัย (Clean Data Type)
                DataTable safeDt = BuildPackingSelectionTable(ConvertToSafeDataTable(rawDt));

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
            bool isAllAppearance = IsAllAppearanceMode();
            dtg_packing_size_appear.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dtg_packing_size_appear.AllowUserToAddRows = false;
            dtg_packing_size_appear.ReadOnly = false;

            if (dtg_packing_size_appear.Columns["DISPLAY_NO"] != null)
            {
                dtg_packing_size_appear.Columns["DISPLAY_NO"].HeaderText = "ลำดับที่";
                dtg_packing_size_appear.Columns["DISPLAY_NO"].ReadOnly = true;
                dtg_packing_size_appear.Columns["DISPLAY_NO"].Visible = !isAllAppearance;
            }

            if (dtg_packing_size_appear.Columns["PACKING_VALUE"] != null)
            {
                dtg_packing_size_appear.Columns["PACKING_VALUE"].HeaderText = "Packing Size";
                dtg_packing_size_appear.Columns["PACKING_VALUE"].ReadOnly = true;
            }

            if (dtg_packing_size_appear.Columns["CUMULATIVE_QTY"] != null)
            {
                dtg_packing_size_appear.Columns["CUMULATIVE_QTY"].Visible = false;
            }

            if (dtg_packing_size_appear.Columns["QTY_SELECT"] != null)
            {
                dtg_packing_size_appear.Columns["QTY_SELECT"].HeaderText = isAllAppearance
                    ? "จำนวนตรวจสอบ"
                    : "จำนวนตรวจสอบ / Packing";
                dtg_packing_size_appear.Columns["QTY_SELECT"].ReadOnly = true;
            }

            if (dtg_packing_size_appear.Columns["QTY_OK"] != null)
            {
                dtg_packing_size_appear.Columns["QTY_OK"].Visible = false;
            }

            if (dtg_packing_size_appear.Columns["QTY_NG"] != null)
            {
                dtg_packing_size_appear.Columns["QTY_NG"].Visible = false;
            }

            if (dtg_packing_size_appear.Columns["JUDGE_LOT_SIZE"] != null)
            {
                dtg_packing_size_appear.Columns["JUDGE_LOT_SIZE"].Visible = false;
            }

            if (dtg_packing_size_appear.Columns["STATUS_TEXT"] != null)
            {
                dtg_packing_size_appear.Columns["STATUS_TEXT"].HeaderText = "สถานะ";
                dtg_packing_size_appear.Columns["STATUS_TEXT"].ReadOnly = true;
            }

            if (dtg_packing_size_appear.Columns["VALUE"] != null)
            {
                dtg_packing_size_appear.Columns["VALUE"].HeaderText = "ตัว/แพ๊ค";
                dtg_packing_size_appear.Columns["VALUE"].ReadOnly = true;
                dtg_packing_size_appear.Columns["VALUE"].Visible = false;

                // *** เพิ่ม: Safe format สำหรับ int columns ***
                dtg_packing_size_appear.Columns["VALUE"].DefaultCellStyle.Format = "N0";  // No decimal
                dtg_packing_size_appear.Columns["VALUE"].DefaultCellStyle.NullValue = "0";  // Null แสดง 0

            }

            if (dtg_packing_size_appear.Columns["PACK_COUNT"] != null)
            {
                dtg_packing_size_appear.Columns["PACK_COUNT"].HeaderText = "จำนวนแพ็ค";
                dtg_packing_size_appear.Columns["PACK_COUNT"].ReadOnly = true;
                dtg_packing_size_appear.Columns["PACK_COUNT"].Visible = false;
            }

            if (dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"] != null)
            {
                dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"].HeaderText = "เหลือตรวจ";
                dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"].ReadOnly = true;
                dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"].Visible = false;

                // *** เพิ่ม: การจัดการ NullValue สำหรับ int ***
                dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"].DefaultCellStyle.NullValue = 0;
                dtg_packing_size_appear.Columns["REMAIN_PACKING_SIZE"].DefaultCellStyle.Format = "N0";

            }

            if (dtg_packing_size_appear.Columns["BATCH"] != null)
            {
                dtg_packing_size_appear.Columns["BATCH"].HeaderText = "ชุดที่";
                dtg_packing_size_appear.Columns["BATCH"].ReadOnly = true;
                dtg_packing_size_appear.Columns["BATCH"].Visible = false;

                // *** เพิ่ม: สำหรับ string ***
                //dtg_packing_size_appear.Columns["BATCH"].DefaultCellStyle.NullValue = "";  // Null แสดง empty
            }

            if (dtg_packing_size_appear.Columns["PACKING_SIZE"] != null)
            {
                //dtg_packing_size_appear.Columns["PACKING_SIZE"].Visible = false;
                dtg_packing_size_appear.Columns["PACKING_SIZE"].HeaderText = "ต้องตรวจทั้งหมด";
                dtg_packing_size_appear.Columns["PACKING_SIZE"].ReadOnly = true;
                dtg_packing_size_appear.Columns["PACKING_SIZE"].Visible = false;
            }

            ConfigurePackingLotNoColumn();

            string[] hiddenColumns = { "COUNT", "ORIGINAL_PACK_COUNT", "TOTAL_PACKING_SIZE", "LOT_SIZE", "IS_SELECTABLE" };
            foreach (string colName in hiddenColumns)
            {
                if (dtg_packing_size_appear.Columns[colName] != null)
                {
                    dtg_packing_size_appear.Columns[colName].Visible = false;
                }
            }

            dtg_packing_size_appear.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtg_packing_size_appear.ColumnHeadersHeight = 42;
            if (!isAllAppearance)
            {
                SetPackingColumnDisplay("DISPLAY_NO", 0, 55);
                SetPackingColumnDisplay("PACKING_VALUE", 1, 90);
                SetPackingColumnDisplay("QTY_SELECT", 2, 135);
                SetPackingColumnDisplay("LOT_NO", 3, 120);
                SetPackingColumnDisplay("STATUS_TEXT", 4, 90);
            }
            else
            {
                SetPackingColumnDisplay("PACKING_VALUE", 0, 90);
                SetPackingColumnDisplay("QTY_SELECT", 1, 135);
                SetPackingColumnDisplay("LOT_NO", 2, 120);
                SetPackingColumnDisplay("STATUS_TEXT", 3, 90);
            }

            foreach (DataGridViewColumn column in dtg_packing_size_appear.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.ReadOnly = column.Name != "LOT_NO";
            }

            UpdatePackingSelectButtonState();
            UpdatePackingCountLabel();
            dtg_packing_size_appear.Refresh(); // Force update UI
            //dtg_packing_size_appear.Columns["NUMBER"].Visible = false;
        }

        private void ConfigurePackingLotNoColumn()
        {
            if (!dtg_packing_size_appear.Columns.Contains("LOT_NO"))
            {
                return;
            }

            DataGridViewColumn existingColumn = dtg_packing_size_appear.Columns["LOT_NO"];
            DataGridViewComboBoxColumn lotColumn = existingColumn as DataGridViewComboBoxColumn;
            if (lotColumn == null)
            {
                int displayIndex = existingColumn.DisplayIndex;
                int columnIndex = existingColumn.Index;
                dtg_packing_size_appear.Columns.Remove(existingColumn);

                lotColumn = new DataGridViewComboBoxColumn
                {
                    Name = "LOT_NO",
                    DataPropertyName = "LOT_NO",
                    HeaderText = "Lot No.",
                    FlatStyle = FlatStyle.Popup,
                    DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                    DisplayStyleForCurrentCellOnly = false,
                    ReadOnly = false
                };
                dtg_packing_size_appear.Columns.Insert(columnIndex, lotColumn);
                lotColumn.DisplayIndex = displayIndex;
            }

            lotColumn.DataSource = GetReportLotNoOptions();
            lotColumn.HeaderText = "Lot No.";
            lotColumn.ReadOnly = false;
            lotColumn.DefaultCellStyle.BackColor = Color.White;
            lotColumn.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        private void SetPackingColumnDisplay(string columnName, int displayIndex, float fillWeight)
        {
            if (!dtg_packing_size_appear.Columns.Contains(columnName))
            {
                return;
            }

            DataGridViewColumn column = dtg_packing_size_appear.Columns[columnName];
            column.Visible = true;
            column.DisplayIndex = displayIndex;
            column.FillWeight = fillWeight;
        }

        private void UpdatePackingCountLabel()
        {
            int inspectedQty = 0;
            int totalQty = 0;

            if (dtg_packing_size_appear.DataSource is DataTable dt)
            {
                foreach (DataRow row in dt.Rows)
                {
                    int plannedQty = ParseInt(row["QTY_SELECT"]);
                    int remainingQty = ParseInt(row["REMAIN_PACKING_SIZE"]);
                    totalQty += plannedQty;
                    inspectedQty += Math.Max(plannedQty - remainingQty, 0);
                }
            }

            // All mode: แสดง Lot Size เป็นเป้าหมาย, ไม่ใช่ผลรวม Packing
            if (IsAllAppearanceMode())
            {
                int lotSize = ParseInt(propQA.Qty);
                if (lotSize <= 0) lotSize = totalQty;
                lbCount.Text = $"{inspectedQty} / {lotSize}";
            }
            else
            {
                lbCount.Text = $"{inspectedQty} / {totalQty}";
            }

            if (IsAllAppearanceMode())
            {
                int targetQty = GetAppearanceAcceptanceTargetQty();
                if (targetQty <= 0)
                {
                    targetQty = totalQty;
                }

                lb_CountAll.Text = $"{inspectedQty} / {targetQty}";
            }
        }

        private void ConfigureAllInspectionCount()
        {
            bool visible = IsAllAppearanceMode();
            lb_CountAll.Visible = visible;
            label3.Visible = visible;

            if (!visible)
            {
                return;
            }

            lb_CountAll.ForeColor = Color.Blue;
            int targetQty = GetAppearanceAcceptanceTargetQty();
            lb_CountAll.Text = $"0 / {targetQty}";
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
                dtg_show_appear.CellFormatting -= dtg_show_appear_CellFormatting;
                dtg_show_appear.Paint -= dtg_show_appear_Paint;
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

        private void SetSelectedBatchSamplingContext(DataGridViewRow selectedRow)
        {
            selectedPackCount = Math.Max(ParseIntSafe(selectedRow.Cells["ORIGINAL_PACK_COUNT"].Value), 1);
            selectedPackSequence = ParseIntSafe(selectedRow.Cells["DISPLAY_NO"].Value);
            selectedPackingValue = ParseIntSafe(selectedRow.Cells["VALUE"].Value);
            selectedLotSize = ParseIntSafe(selectedRow.Cells["LOT_SIZE"].Value);
            selectedBatchSampleTotal = ParseIntSafe(selectedRow.Cells["TOTAL_PACKING_SIZE"].Value);
            maxQty = selectedBatchSampleTotal;
            currentMaxQty = ParseIntSafe(selectedRow.Cells["REMAIN_PACKING_SIZE"].Value);
            samplePerPackQty = ParseIntSafe(selectedRow.Cells["PACKING_SIZE"].Value);

            if (samplePerPackQty <= 0)
            {
                samplePerPackQty = currentMaxQty;
            }

            currentEntryMaxQty = CalculateEntryLimitForRemaining(currentMaxQty);
        }

        private void ResetBatchSamplingContext()
        {
            currentMaxQty = 0;
            maxQty = 0;
            selectedPackCount = 1;
            selectedPackSequence = 0;
            selectedBatchSampleTotal = 0;
            selectedPackingValue = 0;
            selectedLotSize = 0;
            selectedPackSavedQtyAtSelection = 0;
            samplePerPackQty = 0;
            currentEntryMaxQty = 0;
        }

        private int GetPerPackLimit()
        {
            if (samplePerPackQty > 0)
            {
                return samplePerPackQty;
            }

            return maxQty > 0 ? maxQty : currentMaxQty;
        }

        private int CalculateEntryLimitForRemaining(int remainingQty)
        {
            if (remainingQty <= 0)
            {
                return 0;
            }

            int perPackLimit = GetPerPackLimit();
            return perPackLimit > 0 ? Math.Min(perPackLimit, remainingQty) : remainingQty;
        }

        private void UpdateEntryLimitFromInspectedQty(int inspectedQty)
        {
            currentMaxQty = Math.Max(maxQty - inspectedQty, 0);
            currentEntryMaxQty = CalculateEntryLimitForRemaining(currentMaxQty);
        }

        private int GetInspectedQtyFromCurrentGrid()
        {
            if (dtg_show_appear?.DataSource is DataTable dt)
            {
                return GetInspectedQtyFromTable(dt);
            }

            return 0;
        }

        private int GetInspectedQtyFromTable(DataTable dt)
        {
            if (dt == null)
            {
                return 0;
            }

            int inspectedQty = 0;
            foreach (DataRow row in dt.Rows)
            {
                string judge = row.Table.Columns.Contains("JUDGE") ? row["JUDGE"]?.ToString() ?? "" : "";
                if (string.IsNullOrWhiteSpace(judge) || judge.Equals("ERR", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                inspectedQty += row.Table.Columns.Contains("QTY_SELECT") ? ParseInt(row["QTY_SELECT"]) : 0;
            }

            return inspectedQty;
        }

        private int GetRemainingQtyFromPackingGrid()
        {
            if (!(dtg_packing_size_appear?.DataSource is DataTable dt) || !dt.Columns.Contains("REMAIN_PACKING_SIZE"))
            {
                return currentMaxQty;
            }

            int remainingQty = 0;
            foreach (DataRow row in dt.Rows)
            {
                remainingQty += ParseInt(row["REMAIN_PACKING_SIZE"]);
            }

            return remainingQty;
        }

        private int GetLatestRemainingInspectionQty()
        {
            DataTable latestPackingData = BuildPackingSelectionTable(ConvertToSafeDataTable(conQA.SearchSampleSize(propQA)));
            if (latestPackingData == null || !latestPackingData.Columns.Contains("REMAIN_PACKING_SIZE"))
            {
                return GetRemainingQtyFromPackingGrid();
            }

            int remainingQty = 0;
            foreach (DataRow row in latestPackingData.Rows)
            {
                remainingQty += ParseInt(row["REMAIN_PACKING_SIZE"]);
            }

            return remainingQty;
        }

        private bool IsCurrentBatchComplete(int inspectedQty)
        {
            return maxQty > 0 && inspectedQty >= maxQty;
        }

        private void PrepareForNextBatchSelection(bool showCompletedPackCount = false)
        {
            RefreshPackingGrid();
            ShowSavedAppearDataForCurrentBatch();
            if (showCompletedPackCount)
            {

            }
            else
            {
            }
            CloseNgMode();
            dtg_packing_size_appear.ClearSelection();
            dtg_packing_size_appear.Focus();
        }

        private DataTable CreateAppearancePlanTable()
        {
            DataTable dataSource = new DataTable();
            dataSource.Columns.Add("DISPLAY_NO", typeof(int));
            dataSource.Columns.Add("PACKING_VALUE", typeof(int));
            dataSource.Columns.Add("LOT_NO", typeof(string));
            dataSource.Columns.Add("CUMULATIVE_QTY", typeof(int));
            dataSource.Columns.Add("QTY_SELECT", typeof(string));
            dataSource.Columns.Add("QTY_OK", typeof(string));
            dataSource.Columns.Add("QTY_NG", typeof(string));
            dataSource.Columns.Add("JUDGE_LOT_SIZE", typeof(string));
            dataSource.Columns.Add("ROW_STATE", typeof(string));

            dataSource.Columns.Add("APPEARANCE_DATE", typeof(string));
            dataSource.Columns.Add("BATCH", typeof(string));
            dataSource.Columns.Add("COUNT", typeof(int));
            dataSource.Columns.Add("JUDGE", typeof(string));
            dataSource.Columns.Add("EMP_ID", typeof(string));
            return dataSource;
        }

        private DataTable BuildAppearancePlanDataSource(DataTable savedData, bool includeOpenRows = true)
        {
            DataTable dataSource = CreateAppearancePlanTable();
            DataRow[] savedRows = savedData == null
                ? new DataRow[0]
                : savedData.AsEnumerable()
                    .OrderBy(r => ParseInt(r["COUNT"]))
                    .ToArray();

            int cumulativeQty = 0;
            int rowNo = 1;

            foreach (DataRow savedRow in savedRows)
            {
                int qtySelect = ParseInt(savedRow["QTY_SELECT"]);
                cumulativeQty += qtySelect;
                AddAppearancePlanRow(
                    dataSource,
                    rowNo,
                    cumulativeQty,
                    qtySelect,
                    ParseInt(savedRow["QTY_OK"]),
                    ParseInt(savedRow["QTY_NG"]),
                    savedRow["JUDGE"]?.ToString() ?? "",
                    "SAVED",
                    savedRow["APPEARANCE_DATE"]?.ToString() ?? "",
                    ParseInt(savedRow["COUNT"]), savedData.Columns.Contains("LOT_NO") ? savedRow["LOT_NO"]?.ToString() ?? "" : "");

                if (savedData.Columns.Contains("EMP_ID"))
                {
                    dataSource.Rows[dataSource.Rows.Count - 1]["EMP_ID"] = savedRow["EMP_ID"]?.ToString() ?? "";
                }

                rowNo++;
            }

            if (!includeOpenRows)
            {
                MarkLastPlanRowLotSize(dataSource);
                UpdateEntryLimitFromInspectedQty(GetSavedQtyFromPlanTable(dataSource));
                return dataSource;
            }

            int remaining = Math.Max(maxQty - cumulativeQty, 0);
            if (remaining > 0)
            {
                int inputQty = CalculateEntryLimitForRemaining(remaining);
                AddAppearancePlanRow(
                    dataSource,
                    rowNo,
                    cumulativeQty,
                    inputQty,
                    0,
                    0,
                    "",
                    "INPUT",
                    DateTime.Now.ToString("dd-MMM-yyyy"),
                    rowNo);

                rowNo++;
                remaining = Math.Max(remaining - inputQty, 0);
            }

            while (remaining > 0)
            {
                int planQty = CalculateEntryLimitForRemaining(remaining);
                if (planQty <= 0)
                {
                    break;
                }

                remaining = Math.Max(remaining - planQty, 0);
                AddAppearancePlanRow(
                    dataSource,
                    rowNo,
                    cumulativeQty,
                    planQty,
                    0,
                    0,
                    "",
                    "PLAN",
                    "",
                    rowNo);

                rowNo++;
            }

            MarkLastPlanRowLotSize(dataSource);
            UpdateEntryLimitFromInspectedQty(GetSavedQtyFromPlanTable(dataSource));
            return dataSource;
        }

        private void AddAppearancePlanRow(DataTable dataSource, int rowNo, int cumulativeQty, int qtySelect, int qtyOk, int qtyNg, string judge, string rowState, string appearanceDate, int count, string lotNo = "")
        {
            DataRow row = dataSource.NewRow();
            row["DISPLAY_NO"] = rowNo;
            row["PACKING_VALUE"] = selectedPackingValue;
            row["LOT_NO"] = string.IsNullOrWhiteSpace(lotNo) ? propQA.Lot_No ?? string.Empty : lotNo;
            row["CUMULATIVE_QTY"] = cumulativeQty;
            row["QTY_SELECT"] = qtySelect.ToString();
            row["QTY_OK"] = qtyOk > 0 ? qtyOk.ToString() : "";
            row["QTY_NG"] = qtyNg > 0 ? qtyNg.ToString() : "";
            row["JUDGE_LOT_SIZE"] = "";
            row["ROW_STATE"] = rowState;
            row["APPEARANCE_DATE"] = appearanceDate;
            row["BATCH"] = propQA.BATCH;
            row["COUNT"] = count;
            row["JUDGE"] = GetJudgeDisplayText(judge, qtyOk, qtyNg);
            row["EMP_ID"] = propQA.EMP_ID;
            dataSource.Rows.Add(row);
        }

        private string GetJudgeDisplayText(string judge, int qtyOk = 0, int qtyNg = 0)
        {
            string value = judge?.Trim() ?? "";
            if (value == "1")
            {
                return qtyOk > 0 ? $"Accept{Environment.NewLine}{qtyOk} pcs." : "Accept";
            }

            if (value == "0")
            {
                return "Pending";
            }

            return value;
        }

        private void MarkLastPlanRowLotSize(DataTable dataSource)
        {
            if (dataSource.Rows.Count == 0 || selectedLotSize <= 0)
            {
                return;
            }

            dataSource.Rows[dataSource.Rows.Count - 1]["JUDGE_LOT_SIZE"] = selectedLotSize.ToString();
        }

        private int GetSavedQtyFromPlanTable(DataTable dt)
        {
            if (dt == null || !dt.Columns.Contains("ROW_STATE"))
            {
                return 0;
            }

            int qty = 0;
            foreach (DataRow row in dt.Rows)
            {
                if ((row["ROW_STATE"]?.ToString() ?? "") == "SAVED")
                {
                    qty += ParseInt(row["QTY_SELECT"]);
                }
            }
            return qty;
        }

        private DataRow GetActiveInputDataRow()
        {
            if (!(dtg_show_appear?.DataSource is DataTable dt) || !dt.Columns.Contains("ROW_STATE"))
            {
                return null;
            }

            foreach (DataRow row in dt.Rows)
            {
                if ((row["ROW_STATE"]?.ToString() ?? "") == "INPUT")
                {
                    return row;
                }
            }

            return null;
        }

        private int GetActiveInputGridRowIndex()
        {
            if (dtg_show_appear?.Rows == null || !dtg_show_appear.Columns.Contains("ROW_STATE"))
            {
                return -1;
            }

            for (int i = 0; i < dtg_show_appear.Rows.Count; i++)
            {
                string state = dtg_show_appear.Rows[i].Cells["ROW_STATE"].Value?.ToString() ?? "";
                if (state == "INPUT")
                {
                    return i;
                }
            }

            return -1;
        }

        private bool IsInputGridRow(int rowIndex)
        {
            if (rowIndex < 0 || dtg_show_appear == null || !dtg_show_appear.Columns.Contains("ROW_STATE"))
            {
                return false;
            }

            return (dtg_show_appear.Rows[rowIndex].Cells["ROW_STATE"].Value?.ToString() ?? "") == "INPUT";
        }

        private DataTable CreatePackingSelectionTable()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("DISPLAY_NO", typeof(int));
            dt.Columns.Add("PACKING_VALUE", typeof(int));
            dt.Columns.Add("LOT_NO", typeof(string));
            dt.Columns.Add("CUMULATIVE_QTY", typeof(int));
            dt.Columns.Add("QTY_SELECT", typeof(int));
            dt.Columns.Add("QTY_OK", typeof(string));
            dt.Columns.Add("QTY_NG", typeof(string));
            dt.Columns.Add("JUDGE_LOT_SIZE", typeof(string));
            dt.Columns.Add("STATUS_TEXT", typeof(string));

            dt.Columns.Add("BATCH", typeof(string));
            dt.Columns.Add("COUNT", typeof(int));
            dt.Columns.Add("VALUE", typeof(int));
            dt.Columns.Add("PACK_COUNT", typeof(int));
            dt.Columns.Add("ORIGINAL_PACK_COUNT", typeof(int));
            dt.Columns.Add("PACKING_SIZE", typeof(int));
            dt.Columns.Add("TOTAL_PACKING_SIZE", typeof(int));
            dt.Columns.Add("REMAIN_PACKING_SIZE", typeof(int));
            dt.Columns.Add("LOT_SIZE", typeof(int));
            dt.Columns.Add("IS_SELECTABLE", typeof(bool));
            return dt;
        }

        private DataTable BuildPackingSelectionTable(DataTable packingDt)
        {
            DataTable expandedDt = CreatePackingSelectionTable();
            if (packingDt == null)
            {
                return expandedDt;
            }

            string originalBatch = propQA.BATCH;

            foreach (DataRow packingRow in packingDt.Rows)
            {
                string batch = packingRow["BATCH"]?.ToString() ?? "";
                int packCount = Math.Max(ParseIntSafe(packingRow["PACK_COUNT"]), 1);
int packingValue = ParseIntSafe(packingRow["VALUE"]);
int lotSize = packingValue * packCount;
// เช็คเงื่อนไขโหมด All
int totalSampleQty = IsAllAppearanceMode() ? lotSize : ParseIntSafe(packingRow["PACKING_SIZE"]);
int perPackQty = (int)Math.Ceiling(totalSampleQty / (double)packCount);

                propQA.BATCH = batch;
                DataTable savedData = conQA.SearchAppearData(propQA);
                Dictionary<int, int> savedQtyByCount = new Dictionary<int, int>();
                Dictionary<int, int> savedOkByCount = new Dictionary<int, int>();
                Dictionary<int, int> savedNgByCount = new Dictionary<int, int>();

                if (savedData != null)
                {
                    foreach (DataRow savedRow in savedData.Rows)
                    {
                        int count = ParseInt(savedRow["COUNT"]);
                        int qty = ParseInt(savedRow["QTY_SELECT"]);

                        if (!savedQtyByCount.ContainsKey(count))
                        {
                            savedQtyByCount[count] = 0;
                            savedOkByCount[count] = 0;
                            savedNgByCount[count] = 0;
                        }

                        savedQtyByCount[count] += qty;
                        savedOkByCount[count] += ParseInt(savedRow["QTY_OK"]);
                        savedNgByCount[count] += ParseInt(savedRow["QTY_NG"]);
                    }
                }

                int cumulativeQty = 0;
                int remainingPlanQty = totalSampleQty;

                for (int packSeq = 1; packSeq <= packCount && remainingPlanQty > 0; packSeq++)
                {
                    int planQty = Math.Min(perPackQty, remainingPlanQty);
                    remainingPlanQty -= planQty;
                    int savedQty = savedQtyByCount.ContainsKey(packSeq) ? savedQtyByCount[packSeq] : 0;
                    int savedOk = savedOkByCount.ContainsKey(packSeq) ? savedOkByCount[packSeq] : 0;
                    int savedNg = savedNgByCount.ContainsKey(packSeq) ? savedNgByCount[packSeq] : 0;
                    int remainQty = Math.Max(planQty - savedQty, 0);
                    bool isSelectable = remainQty > 0;
                    cumulativeQty += savedQty;
                    string statusText = remainQty <= 0 ? "เสร็จแล้ว" : "รอตรวจ";

                    DataRow expandedRow = expandedDt.NewRow();
                    expandedRow["DISPLAY_NO"] = packSeq;
                    expandedRow["PACKING_VALUE"] = packingValue;
                    expandedRow["LOT_NO"] = GetDefaultLotNoForPackingRow();
                    expandedRow["CUMULATIVE_QTY"] = savedQty > 0 ? (object)cumulativeQty : DBNull.Value;
                    expandedRow["QTY_SELECT"] = planQty;
                    expandedRow["QTY_OK"] = savedOk > 0 ? savedOk.ToString() : "";
                    expandedRow["QTY_NG"] = savedNg > 0 ? savedNg.ToString() : "";
                    expandedRow["JUDGE_LOT_SIZE"] = packSeq == packCount ? lotSize.ToString() : "";
                    expandedRow["STATUS_TEXT"] = statusText;
                    expandedRow["BATCH"] = batch;
                    expandedRow["COUNT"] = packSeq;
                    expandedRow["VALUE"] = packingValue;
                    expandedRow["PACK_COUNT"] = 1;
                    expandedRow["ORIGINAL_PACK_COUNT"] = packCount;
                    expandedRow["PACKING_SIZE"] = planQty;
                    expandedRow["TOTAL_PACKING_SIZE"] = totalSampleQty;
                    expandedRow["REMAIN_PACKING_SIZE"] = remainQty;
                    expandedRow["LOT_SIZE"] = lotSize;
                    expandedRow["IS_SELECTABLE"] = isSelectable;
                    expandedDt.Rows.Add(expandedRow);
                }
            }

            propQA.BATCH = originalBatch;
            return expandedDt;
        }

        private DataTable BuildSinglePackInputDataSource(DataGridViewRow selectedRow)
        {
            DataTable dataSource = CreateAppearancePlanTable();
            int displayNo = ParseIntSafe(selectedRow.Cells["DISPLAY_NO"].Value);
            int cumulativeQty = ParseIntSafe(selectedRow.Cells["CUMULATIVE_QTY"].Value);
            int qtySelect = ParseIntSafe(selectedRow.Cells["QTY_SELECT"].Value);
            int count = ParseIntSafe(selectedRow.Cells["COUNT"].Value);
            bool isAllAppearance = IsAllAppearanceMode();

            AddAppearancePlanRow(
                dataSource,
                displayNo,
                cumulativeQty,
                isAllAppearance ? 0 : qtySelect,
                0,
                0,
                "",
                "INPUT",
                DateTime.Now.ToString("dd-MMM-yyyy"),
                count);

            if (isAllAppearance && dataSource.Rows.Count > 0)
            {
                dataSource.Rows[0]["QTY_SELECT"] = "";
                currentEntryMaxQty = qtySelect;
            }

            if (selectedRow.Cells["JUDGE_LOT_SIZE"].Value != null)
            {
                dataSource.Rows[0]["JUDGE_LOT_SIZE"] = selectedRow.Cells["JUDGE_LOT_SIZE"].Value.ToString();
            }

            return dataSource;
        }

        private DataTable BuildSelectedPackInputDataSource(DataGridViewRow selectedRow, DataTable savedData)
        {
            DataTable dataSource = CreateAppearancePlanTable();
            int displayNo = ParseIntSafe(selectedRow.Cells["DISPLAY_NO"].Value);
            int count = ParseIntSafe(selectedRow.Cells["COUNT"].Value);
            int remainingQty = ParseIntSafe(selectedRow.Cells["REMAIN_PACKING_SIZE"].Value);
            bool isAllAppearance = IsAllAppearanceMode();
            int inputQty = isAllAppearance ? remainingQty : CalculateEntryLimitForRemaining(remainingQty);
            int cumulativeQty = 0;

            IEnumerable<DataRow> savedRows = Enumerable.Empty<DataRow>();
            if (savedData != null && savedData.Columns.Contains("COUNT"))
            {
                savedRows = savedData.AsEnumerable()
                    .Where(row => ParseInt(row["COUNT"]) == count)
                    .OrderBy(row => row["APPEARANCE_DATE"]?.ToString() ?? "");
            }

            foreach (DataRow savedRow in savedRows)
            {
                int savedQty = ParseInt(savedRow["QTY_SELECT"]);
                cumulativeQty += savedQty;
                AddAppearancePlanRow(
                    dataSource,
                    displayNo,
                    cumulativeQty,
                    savedQty,
                    ParseInt(savedRow["QTY_OK"]),
                    ParseInt(savedRow["QTY_NG"]),
                    savedRow["JUDGE"]?.ToString() ?? "",
                    "SAVED",
                    savedRow["APPEARANCE_DATE"]?.ToString() ?? "",
                    count, savedData.Columns.Contains("LOT_NO") ? savedRow["LOT_NO"]?.ToString() ?? "" : "");

                if (savedData.Columns.Contains("EMP_ID"))
                {
                    dataSource.Rows[dataSource.Rows.Count - 1]["EMP_ID"] = savedRow["EMP_ID"]?.ToString() ?? "";
                }
            }

            selectedPackSavedQtyAtSelection = cumulativeQty;

            if (inputQty > 0)
            {
                AddAppearancePlanRow(
                    dataSource,
                    displayNo,
                    cumulativeQty,
                    isAllAppearance ? 0 : inputQty,
                    0,
                    0,
                    "",
                    "INPUT",
                    DateTime.Now.ToString("dd-MMM-yyyy"),
                    count);

                if (isAllAppearance)
                {
                    dataSource.Rows[dataSource.Rows.Count - 1]["QTY_SELECT"] = "";
                }
            }

            if (selectedRow.Cells["JUDGE_LOT_SIZE"].Value != null && dataSource.Rows.Count > 0)
            {
                dataSource.Rows[dataSource.Rows.Count - 1]["JUDGE_LOT_SIZE"] = selectedRow.Cells["JUDGE_LOT_SIZE"].Value.ToString();
            }

            UpdateEntryLimitFromInspectedQty(cumulativeQty);
            currentMaxQty = remainingQty;
            currentEntryMaxQty = inputQty;
            return dataSource;
        }


        private int GetCompletedPackCountFromPackingGrid()
        {
            if (!(dtg_packing_size_appear?.DataSource is DataTable dt) || !dt.Columns.Contains("REMAIN_PACKING_SIZE"))
            {
                return 0;
            }

            int completedPackCount = 0;
            foreach (DataRow row in dt.Rows)
            {
                if (ParseInt(row["REMAIN_PACKING_SIZE"]) <= 0)
                {
                    completedPackCount++;
                }
            }

            return completedPackCount;
        }

        private void bt_Select_Click(object sender, EventArgs e)
        {
            dtg_packing_size_appear.EndEdit();

            if (dtg_packing_size_appear.SelectedRows.Count == 0)
            {
                return;
            }

            var selectedRow = dtg_packing_size_appear.SelectedRows[0];
            if (!TrySetSelectedPackingLotNo(selectedRow))
            {
                UpdatePackingSelectButtonState();
                return;
            }

            dtg_packing_size_appear.Enabled = false;
            bt_select_packing_size_appear.Enabled = false;
            propQA.BATCH = selectedRow.Cells["BATCH"].Value.ToString();

            SetSelectedBatchSamplingContext(selectedRow);

            DataTable savedData = conQA.SearchAppearData(propQA);
            DataTable dataSource = BuildSelectedPackInputDataSource(selectedRow, savedData);
            selectedRow.Cells["STATUS_TEXT"].Value = "รอตรวจ";

            // ล้าง DataSource ก่อนเพื่อรีเซ็ต grid (ป้องกันคอลัมน์เก่าค้าง)
            dtg_show_appear.DataSource = null;
            dtg_show_appear.AutoGenerateColumns = true;
            dtg_show_appear.DataSource = dataSource;

            gb_input.Enabled = currentMaxQty > 0;
            ApplyRowReadOnly();  // ให้เฉพาะแถวสุดท้ายแก้ไขได้
            dtg_show_appear.Refresh();

            int inputRowIndex = GetActiveInputGridRowIndex();
            if (inputRowIndex >= 0 && dtg_show_appear.Columns["QTY_OK"] != null)
            {
                dtg_show_appear.CurrentCell = dtg_show_appear.Rows[inputRowIndex].Cells["QTY_OK"];
            }

            // Refresh UI สำหรับ NG mode (ปิดก่อน)
            isNgModeActive = false;
            gb_ngMode.Enabled = false;
            totalNgRequired = 0;
            InitializeNgModeDataTable();

            tb_record.Enabled = GetActiveInputGridRowIndex() >= 0;

        }

        private void dtg_packing_size_appear_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dtg_packing_size_appear.IsCurrentCellDirty)
            {
                dtg_packing_size_appear.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dtg_packing_size_appear_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || _suppressEvents)
            {
                return;
            }

            if (dtg_packing_size_appear.Columns[e.ColumnIndex].Name == "LOT_NO")
            {
                UpdatePackingSelectButtonState();
            }
        }

        private void UpdatePackingSelectButtonState()
        {
            if (dtg_packing_size_appear == null || bt_select_packing_size_appear == null)
            {
                return;
            }

            if (dtg_packing_size_appear.SelectedRows.Count == 0)
            {
                bt_select_packing_size_appear.Enabled = false;
                return;
            }

            DataGridViewRow selectedRow = dtg_packing_size_appear.SelectedRows[0];
            int remainQty = 0;
            if (dtg_packing_size_appear.Columns.Contains("REMAIN_PACKING_SIZE"))
            {
                object remainValue = selectedRow.Cells["REMAIN_PACKING_SIZE"].Value;
                if (remainValue != null && remainValue != DBNull.Value)
                {
                    int.TryParse(remainValue.ToString(), out remainQty);
                }
            }

            string lotNo = dtg_packing_size_appear.Columns.Contains("LOT_NO")
                ? selectedRow.Cells["LOT_NO"].Value?.ToString()?.Trim() ?? string.Empty
                : string.Empty;

            bt_select_packing_size_appear.Enabled = remainQty > 0 && GetReportLotNoList().Contains(lotNo);
        }
        private void dtg_packing_size_appear_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dtg_packing_size_appear.Columns[e.ColumnIndex].Name == "STATUS_TEXT")
            {
                string status = e.Value?.ToString() ?? "";
                if (status == "เสร็จแล้ว")
                {
                    e.CellStyle.BackColor = Color.LightGray;
                    e.CellStyle.ForeColor = Color.Black;
                }
                else
                {
                    e.CellStyle.BackColor = Color.White;
                    e.CellStyle.ForeColor = Color.DimGray;
                }
                return;
            }

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
            if (_suppressEvents)
            {
                return;
            }

            UpdatePackingSelectButtonState();
        }

        // 4. เพิ่ม dtg_packing_size_appear_MouseDown - สำหรับ handle click/selection บน cell (e.g., REMAIN_PACKING_SIZE) เพื่อให้ focus/edit ได้ทันที
        // *** MouseDown: Prevent loop โดย end-edit ก่อน click ***
        private void dtg_packing_size_appear_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && dtg_packing_size_appear.HitTest(e.X, e.Y).Type == DataGridViewHitTestType.Cell)
            {
                DataGridView.HitTestInfo hit = dtg_packing_size_appear.HitTest(e.X, e.Y);
                if (hit.ColumnIndex >= 0 && hit.RowIndex >= 0 &&
                    (dtg_packing_size_appear.Columns[hit.ColumnIndex].Name == "REMAIN_PACKING_SIZE" || dtg_packing_size_appear.Columns[hit.ColumnIndex].Name == "LOT_NO"))
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
            bool isAllAppearance = IsAllAppearanceMode();

            if (dtg_show_appear.Columns["DISPLAY_NO"] != null)
            {
                dtg_show_appear.Columns["DISPLAY_NO"].HeaderText = "ลำดับที่";
                dtg_show_appear.Columns["DISPLAY_NO"].ReadOnly = true;
                dtg_show_appear.Columns["DISPLAY_NO"].Width = 70;
                dtg_show_appear.Columns["DISPLAY_NO"].FillWeight = 85;
                dtg_show_appear.Columns["DISPLAY_NO"].DisplayIndex = 0;
                dtg_show_appear.Columns["DISPLAY_NO"].Visible = !isAllAppearance;
            }

            if (dtg_show_appear.Columns["PACKING_VALUE"] != null)
            {
                dtg_show_appear.Columns["PACKING_VALUE"].HeaderText = "Packing Size";
                dtg_show_appear.Columns["PACKING_VALUE"].ReadOnly = true;
                dtg_show_appear.Columns["PACKING_VALUE"].Visible = false;
            }

            
            if (dtg_show_appear.Columns["LOT_NO"] != null)
            {
                dtg_show_appear.Columns["LOT_NO"].HeaderText = "Lot No.";
                dtg_show_appear.Columns["LOT_NO"].ReadOnly = true;
                dtg_show_appear.Columns["LOT_NO"].Visible = true;
                dtg_show_appear.Columns["LOT_NO"].FillWeight = 105;
                dtg_show_appear.Columns["LOT_NO"].DisplayIndex = 1;
            }

            if (dtg_show_appear.Columns["CUMULATIVE_QTY"] != null)
            {
                dtg_show_appear.Columns["CUMULATIVE_QTY"].HeaderText = "จำนวนรวม";
                dtg_show_appear.Columns["CUMULATIVE_QTY"].ReadOnly = true;
                dtg_show_appear.Columns["CUMULATIVE_QTY"].Visible = false;
            }

            if (dtg_show_appear.Columns["APPEARANCE_DATE"] != null)
            {
                dtg_show_appear.Columns["APPEARANCE_DATE"].HeaderText = "วันที่";
                dtg_show_appear.Columns["APPEARANCE_DATE"].ReadOnly = true;
                dtg_show_appear.Columns["APPEARANCE_DATE"].Visible = isAllAppearance;
                if (isAllAppearance)
                {
                    dtg_show_appear.Columns["APPEARANCE_DATE"].FillWeight = 95;
                    dtg_show_appear.Columns["APPEARANCE_DATE"].DisplayIndex = 0;
                }
            }

            if (dtg_show_appear.Columns["BATCH"] != null)
            {
                dtg_show_appear.Columns["BATCH"].HeaderText = "ชุดที่";
                dtg_show_appear.Columns["BATCH"].ReadOnly = true;
                dtg_show_appear.Columns["BATCH"].Visible = false;
            }

            if (dtg_show_appear.Columns["COUNT"] != null)
            {
                dtg_show_appear.Columns["COUNT"].HeaderText = "ครั้งที่";
                dtg_show_appear.Columns["COUNT"].ReadOnly = true;
                dtg_show_appear.Columns["COUNT"].Visible = false;
            }

            if (dtg_show_appear.Columns["QTY_SELECT"] != null)
            {
                dtg_show_appear.Columns["QTY_SELECT"].HeaderText = isAllAppearance
                    ? "จำนวนตรวจสอบ"
                    : "จำนวนตรวจสอบ/Packing";
                dtg_show_appear.Columns["QTY_SELECT"].ReadOnly = !isAllAppearance;
                dtg_show_appear.Columns["QTY_SELECT"].FillWeight = 105;
                dtg_show_appear.Columns["QTY_SELECT"].DisplayIndex = 2;
            }

            if (dtg_show_appear.Columns["QTY_OK"] != null)
            {
                dtg_show_appear.Columns["QTY_OK"].HeaderText = "OK";
                dtg_show_appear.Columns["QTY_OK"].ReadOnly = false;
                dtg_show_appear.Columns["QTY_OK"].FillWeight = isAllAppearance ? 80 : 85;
                dtg_show_appear.Columns["QTY_OK"].DisplayIndex = 3;
            }

            if (dtg_show_appear.Columns["QTY_NG"] != null)
            {
                dtg_show_appear.Columns["QTY_NG"].HeaderText = "Pending";
                dtg_show_appear.Columns["QTY_NG"].ReadOnly = false;
                dtg_show_appear.Columns["QTY_NG"].FillWeight = isAllAppearance ? 80 : 85;
                dtg_show_appear.Columns["QTY_NG"].DisplayIndex = 4;
            }

            if (dtg_show_appear.Columns["JUDGE"] != null)
            {
                dtg_show_appear.Columns["JUDGE"].HeaderText = "ผล";
                dtg_show_appear.Columns["JUDGE"].ReadOnly = true;  // Editable only in last row
                dtg_show_appear.Columns["JUDGE"].DisplayIndex = 5;
                dtg_show_appear.Columns["JUDGE"].Visible = isAllAppearance;
                dtg_show_appear.Columns["JUDGE"].FillWeight = 90;
                dtg_show_appear.Columns["JUDGE"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
                dtg_show_appear.Columns["JUDGE"].DefaultCellStyle.WrapMode =
                    DataGridViewTriState.True;
            }

            if (dtg_show_appear.Columns["JUDGE_LOT_SIZE"] != null)
            {
                dtg_show_appear.Columns["JUDGE_LOT_SIZE"].HeaderText = "Appearance Judgement";
                dtg_show_appear.Columns["JUDGE_LOT_SIZE"].ReadOnly = true;
                dtg_show_appear.Columns["JUDGE_LOT_SIZE"].Visible = !isAllAppearance;
                dtg_show_appear.Columns["JUDGE_LOT_SIZE"].FillWeight = 95;
                dtg_show_appear.Columns["JUDGE_LOT_SIZE"].DisplayIndex = 5;
                dtg_show_appear.Columns["JUDGE_LOT_SIZE"].DefaultCellStyle.Alignment =
                    DataGridViewContentAlignment.MiddleCenter;
                dtg_show_appear.Columns["JUDGE_LOT_SIZE"].DefaultCellStyle.WrapMode =
                    DataGridViewTriState.True;
            }

            if (dtg_show_appear.Columns["ROW_STATE"] != null)
            {
                dtg_show_appear.Columns["ROW_STATE"].Visible = false;
            }

            if (dtg_show_appear.Columns["EMP_ID"] != null)
            {
                dtg_show_appear.Columns["EMP_ID"].Visible = false;
            }

            // Make all rows except last read-only, and hide headers if needed (set RowHeadersVisible = false)
            dtg_show_appear.RowHeadersVisible = false;  // ซ่อน HeaderText ด้านซ้าย
            dtg_show_appear.AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            dtg_show_appear.AutoGenerateColumns = false;
            dtg_show_appear.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtg_show_appear.ColumnHeadersHeight = 42;

            ApplyRowReadOnly();
            dtg_show_appear.Invalidate();
        }

        private void dtg_show_appear_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            string columnName = dtg_show_appear.Columns[e.ColumnIndex].Name;
            if ((IsAllAppearanceMode() && columnName == "JUDGE")
                || (!IsAllAppearanceMode() && columnName == "JUDGE_LOT_SIZE"))
            {
                e.Value = "";
                e.FormattingApplied = true;
            }
        }

        private void dtg_show_appear_Paint(object sender, PaintEventArgs e)
        {
            string resultColumnName = IsAllAppearanceMode() ? "JUDGE" : "JUDGE_LOT_SIZE";
            if (dtg_show_appear.Columns[resultColumnName] == null
                || !dtg_show_appear.Columns[resultColumnName].Visible)
            {
                return;
            }

            DataGridViewColumn resultColumn = dtg_show_appear.Columns[resultColumnName];
            Rectangle columnRectangle = dtg_show_appear.GetColumnDisplayRectangle(resultColumn.Index, true);
            if (columnRectangle.Width <= 0)
            {
                return;
            }

            int top = dtg_show_appear.ColumnHeadersHeight;
            int bottom = dtg_show_appear.ClientSize.Height;
            if (dtg_show_appear.Controls.OfType<HScrollBar>().Any(scrollBar => scrollBar.Visible))
            {
                bottom -= SystemInformation.HorizontalScrollBarHeight;
            }

            Rectangle mergedRectangle = new Rectangle(
                columnRectangle.X,
                top,
                columnRectangle.Width,
                Math.Max(bottom - top, 0));

            if (mergedRectangle.Height <= 0)
            {
                return;
            }

            string summaryText = GetAppearanceSummaryText();
            using (SolidBrush backgroundBrush = new SolidBrush(dtg_show_appear.BackgroundColor))
            using (Pen borderPen = new Pen(Color.Black))
            using (Font resultFont = new Font(dtg_show_appear.Font.FontFamily, 14F, FontStyle.Regular))
            {
                e.Graphics.FillRectangle(backgroundBrush, mergedRectangle);
                e.Graphics.DrawRectangle(
                    borderPen,
                    mergedRectangle.X,
                    mergedRectangle.Y,
                    Math.Max(mergedRectangle.Width - 1, 0),
                    Math.Max(mergedRectangle.Height - 1, 0));

                TextRenderer.DrawText(
                    e.Graphics,
                    summaryText,
                    resultFont,
                    mergedRectangle,
                    Color.Blue,
                    TextFormatFlags.HorizontalCenter
                    | TextFormatFlags.VerticalCenter
                    | TextFormatFlags.WordBreak
                    | TextFormatFlags.NoPadding);
            }
        }

        private string GetAppearanceSummaryText()
        {
            if (!(dtg_show_appear.DataSource is DataTable dataSource))
            {
                return "";
            }

            int inspectedQty = 0;
            int totalPending = 0;
            int targetQty = GetAppearanceAcceptanceTargetQty();
            int plannedTotalQty = 0;

            if (dtg_packing_size_appear.DataSource is DataTable packingData)
            {
                foreach (DataRow row in packingData.Rows)
                {
                    int plannedQty = ParseInt(row["QTY_SELECT"]);
                    int remainingQty = ParseInt(row["REMAIN_PACKING_SIZE"]);
                    plannedTotalQty += plannedQty;
                    inspectedQty += Math.Max(plannedQty - remainingQty, 0);
                    totalPending += ParseInt(row["QTY_NG"]);
                }
            }

            if (targetQty <= 0)
            {
                targetQty = plannedTotalQty;
            }

            if (dataSource.Columns.Contains("ROW_STATE"))
            {
                foreach (DataRow row in dataSource.Rows)
                {
                    if (!string.Equals(row["ROW_STATE"]?.ToString(), "INPUT", StringComparison.Ordinal))
                    {
                        continue;
                    }

                    int qtySelect = ParseInt(row["QTY_SELECT"]);
                    int qtyOk = ParseInt(row["QTY_OK"]);
                    int qtyPending = ParseInt(row["QTY_NG"]);
                    if (qtySelect > 0 && qtyOk + qtyPending == qtySelect)
                    {
                        inspectedQty += qtySelect;
                        totalPending += qtyPending;
                    }
                }
            }

            if (totalPending > 0)
            {
                return $"Pending{Environment.NewLine}{totalPending} pcs.";
            }

            if (targetQty <= 0 || inspectedQty < targetQty)
            {
                return "";
            }

            int lotSize = ParseInt(propQA.Qty);
            if (lotSize <= 0)
            {
                lotSize = selectedLotSize;
            }

            return lotSize > 0
                ? $"Accept{Environment.NewLine}{lotSize} pcs."
                : "";
        }

        private bool IsAllAppearanceMode()
        {
            return propQA.SAMPLING_TYPE == "1"
                || string.Equals(propQA.SAMPLING_NAME?.Trim(), "All", StringComparison.OrdinalIgnoreCase);
        }

        private int GetAppearanceAcceptanceTargetQty()
        {
            return IsAllAppearanceMode()
                ? ParseInt(propQA.Qty)
                : ParseInt(propQA.inspQty);
        }

        // Apply ReadOnly to all rows except the last one
        private void ApplyRowReadOnly()
        {
            for (int i = 0; i < dtg_show_appear.Rows.Count; i++)
            {
                DataGridViewRow row = dtg_show_appear.Rows[i];
                row.ReadOnly = true;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    cell.ReadOnly = true;
                    cell.Style.BackColor = Color.FromArgb(245, 245, 245);
                    cell.Style.ForeColor = Color.Black;
                    cell.Style.SelectionBackColor = Color.Gainsboro;
                    cell.Style.SelectionForeColor = Color.Black;
                }
            }

            int inputRowIndex = GetActiveInputGridRowIndex();
            if (inputRowIndex >= 0)
            {
                DataGridViewRow inputRow = dtg_show_appear.Rows[inputRowIndex];
                inputRow.ReadOnly = false;
                inputRow.DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);

                string[] editableColumns = IsAllAppearanceMode()
                    ? new[] { "QTY_SELECT", "QTY_OK", "QTY_NG" }
                    : new[] { "QTY_OK", "QTY_NG" };
                foreach (string colName in editableColumns)
                {
                    if (dtg_show_appear.Columns[colName] == null)
                    {
                        continue;
                    }

                    DataGridViewCell cell = inputRow.Cells[colName];
                    cell.ReadOnly = false;
                    cell.Style.BackColor = Color.White;
                    cell.Style.ForeColor = Color.Black;
                    cell.Style.SelectionBackColor = SystemColors.Highlight;
                    cell.Style.SelectionForeColor = SystemColors.HighlightText;
                }
            }
        }

        // เพิ่ม method เพื่อ init DataTable สำหรับ dtg_ngMode (ว่างเปล่า)
        private void InitializeNgModeDataTable()
        {
            dtg_ngMode.DataSource = CreateNgModeDataTable(0);
            ConfigureNgModeGridColumns();
            dtg_ngMode.AllowUserToAddRows = true; // ควบคุมด้วย code
        }

        private DataTable CreateNgModeDataTable(int requiredNgQty)
        {
            DataTable ngDt = new DataTable();
            ngDt.Columns.Add("QTY_NG", typeof(string));
            ngDt.Columns.Add(NgModeHelper.ColumnName, typeof(string));
            ngDt.Columns.Add("NG_DETAIL", typeof(string));
            ngDt.Columns.Add("TOTAL_PENDING", typeof(string));
            ngDt.Columns.Add("TOTAL_OK", typeof(string));
            ngDt.Columns.Add("TOTAL_NG", typeof(string));

            if (requiredNgQty > 0)
            {
                AddNgModeInputRow(ngDt, requiredNgQty);
            }

            return ngDt;
        }

        private void AddNgModeInputRow(DataTable ngDt, int qtyNg = 0)
        {
            DataRow newRow = ngDt.NewRow();
            newRow["QTY_NG"] = qtyNg > 0 ? qtyNg.ToString() : string.Empty;
            newRow[NgModeHelper.ColumnName] = string.Empty;
            newRow["NG_DETAIL"] = string.Empty;
            newRow["TOTAL_PENDING"] = string.Empty;
            newRow["TOTAL_OK"] = string.Empty;
            newRow["TOTAL_NG"] = string.Empty;
            ngDt.Rows.Add(newRow);
        }

        private int GetCurrentInputQty(string columnName)
        {
            DataRow inputRow = GetActiveInputDataRow();
            if (inputRow == null || !inputRow.Table.Columns.Contains(columnName))
            {
                return 0;
            }

            return ParseInt(inputRow[columnName]);
        }

        private void AddReadonlyNgSummaryColumn(string name, string headerText, int width)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = headerText,
                DataPropertyName = name,
                ReadOnly = true,
                Width = width,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = { BackColor = Color.WhiteSmoke, Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            dtg_ngMode.Columns.Add(column);
        }

        private void ConfigureNgModeGridColumns()
        {
            dtg_ngMode.AutoGenerateColumns = false;
            dtg_ngMode.Columns.Clear();

            DataGridViewTextBoxColumn qtyColumn = new DataGridViewTextBoxColumn
            {
                Name = "QTY_NG",
                HeaderText = totalNgRequired > 0 ? totalNgRequired.ToString() : string.Empty,
                DataPropertyName = "QTY_NG",
                Visible = true,
                ReadOnly = false,
                Width = 75,
                DisplayIndex = 1,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            dtg_ngMode.Columns.Add(qtyColumn);

            DataGridViewComboBoxColumn ngModeColumn = new DataGridViewComboBoxColumn
            {
                Name = NgModeHelper.ColumnName,
                HeaderText = "Q'ty Pending",
                DataPropertyName = NgModeHelper.ColumnName,
                DataSource = ngModeList,
                DisplayMember = "TEXT",
                ValueMember = "VALUE",
                DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton,
                FlatStyle = FlatStyle.Flat,
                Width = 155,
                DisplayIndex = 0
            };
            dtg_ngMode.Columns.Add(ngModeColumn);

            AddReadonlyNgSummaryColumn("TOTAL_PENDING", $"{GetCurrentInputQty("QTY_NG")}\r\npending", 85);
            AddReadonlyNgSummaryColumn("TOTAL_OK", $"{GetCurrentInputQty("QTY_OK")}\r\nOK", 80);
            AddReadonlyNgSummaryColumn("TOTAL_NG", $"{GetCurrentInputQty("QTY_NG")}\r\nNG", 80);

            DataGridViewTextBoxColumn detailColumn = new DataGridViewTextBoxColumn
            {
                Name = "NG_DETAIL",
                HeaderText = "NG_DETAIL",
                DataPropertyName = "NG_DETAIL",
                Visible = false
            };
            dtg_ngMode.Columns.Add(detailColumn);

            dtg_ngMode.AllowUserToAddRows = true;
            dtg_ngMode.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dtg_ngMode.ColumnHeadersHeight = 46;
            dtg_ngMode.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtg_ngMode.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True;
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

                string mode = rowData.Table.Columns.Contains(NgModeHelper.ColumnName) && rowData[NgModeHelper.ColumnName] != DBNull.Value
                    ? rowData[NgModeHelper.ColumnName].ToString().Trim()
                    : "";
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
                    dtg_ngMode.Rows[firstEmptyRowIndex].Cells[NgModeHelper.ColumnName].Style.BackColor = Color.Red;
                    dtg_ngMode.CurrentCell = dtg_ngMode.Rows[firstEmptyRowIndex].Cells[NgModeHelper.ColumnName];
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

            // Validation: QTY_SELECT ไม่เกินจำนวนที่ควรหยิบต่อครั้ง/ต่อ pack
            if (qtySelect > currentEntryMaxQty)
            {
                MessageBox.Show($"จำนวนที่เลือกตรวจ ({qtySelect}) เกินจำนวนที่ควรหยิบต่อครั้ง ({currentEntryMaxQty}) แล้วค่ะ กรุณาเลือกให้น้อยลง");
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
            dtg_show_appear.EndEdit();
            dtg_ngMode.EndEdit();

            DataTable dt = (DataTable)dtg_show_appear.DataSource;
            if (dt == null || dt.Rows.Count == 0) return;

            DataRow inputRow = GetActiveInputDataRow();
            if (inputRow == null)
            {
                MessageBox.Show("ไม่มีแถวที่พร้อมบันทึก กรุณาเลือกชุดตรวจใหม่", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string batch = inputRow["BATCH"].ToString();

            // Validate ข้อมูลพื้นฐานใน input row
            if (inputRow["QTY_SELECT"] == DBNull.Value || !int.TryParse(inputRow["QTY_SELECT"].ToString(), out int qtySelect) || qtySelect <= 0)
            {
                MessageBox.Show("กรุณากรอก QTY_SELECT ให้ถูกต้อง (มากกว่า 0)", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int qtyOK = ParseInt(inputRow["QTY_OK"]);
            int qtyNG = ParseInt(inputRow["QTY_NG"]);

            if (qtyOK + qtyNG != qtySelect)
            {
                MessageBox.Show("QTY_OK + QTY_NG ต้องเท่ากับ QTY_SELECT", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (qtySelect > currentEntryMaxQty)
            {
                MessageBox.Show($"จำนวนที่เลือกตรวจ ({qtySelect}) เกินจำนวนที่ควรหยิบต่อครั้ง ({currentEntryMaxQty}) สำหรับชุด {batch}", "Exceed Per Pack", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string judge = (qtyNG > 0) ? "0" : "1"; // 0=NG, 1=OK

            if (qtyNG > 0 && !ValidateNgModeDetails(qtyNG))
            {
                return;
            }

            if (!SaveAppearanceProcessLotNo())
            {
                return;
            }

            string empId = employee.EMP_CODE ?? ""; // Use EMP_CODE as per load
            DateTime inputDate = DateTime.Now;
            int newCount = Convert.ToInt32(inputRow["COUNT"]);

            // Step 1: Requery ข้อมูลล่าสุดจาก DB เพื่อจัดการ Multi-Task (เช็ค concurrent update)
            propQA.BATCH = batch;
            DataTable latestData = conQA.SearchAppearData(propQA); // filter โดย REPORT_NO และ BATCH, INUSE=1
            int currentSumSelect = 0;
            int selectedPackSavedQty = 0;

            if (latestData != null)
            {
                foreach (DataRow row in latestData.Rows)
                {
                    int savedCount = Convert.ToInt32(row["COUNT"]);
                    int savedQty = Convert.ToInt32(row["QTY_SELECT"]);
                    currentSumSelect += savedQty;
                    if (savedCount == newCount)
                    {
                        selectedPackSavedQty += savedQty;
                    }
                }
            }

            if (selectedPackSavedQty > selectedPackSavedQtyAtSelection)
            {
                MessageBox.Show(
                    $"แพ็คที่ {newCount} มีผู้ใช้อื่นบันทึกข้อมูลไปแล้ว กรุณาเลือกแพ็คใหม่",
                    "ข้อมูลถูกอัปเดตแล้ว",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                PrepareForNextBatchSelection();
                return;
            }

            int selectedPackRemainingQty = Math.Max(samplePerPackQty - selectedPackSavedQty, 0);
            currentMaxQty = selectedPackRemainingQty;
            currentEntryMaxQty = CalculateEntryLimitForRemaining(currentMaxQty);

            if (selectedPackRemainingQty <= 0)
            {
                MessageBox.Show($"แพ็คที่ {newCount} ตรวจครบแล้ว กรุณาเลือกแพ็คอื่น", "Pack Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PrepareForNextBatchSelection();
                return;
            }

            if (currentEntryMaxQty <= 0)
            {
                MessageBox.Show($"ชุด {batch} ตรวจครบแล้ว กรุณาเลือกชุดอื่น", "Batch Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                PrepareForNextBatchSelection();
                return;
            }

            if (qtySelect > currentEntryMaxQty)
            {
                MessageBox.Show($"จำนวนที่เลือกตรวจ ({qtySelect}) เกินจำนวนที่ควรหยิบต่อครั้ง ({currentEntryMaxQty}) สำหรับชุด {batch}", "Exceed Per Pack", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshAppearData();
                return;
            }

            int projectedSelectedPackQty = selectedPackSavedQty + qtySelect;
            if (projectedSelectedPackQty > samplePerPackQty)
            {
                MessageBox.Show($"ผลรวมของแพ็คที่ {newCount} ({projectedSelectedPackQty}) เกินจำนวนที่ต้องตรวจของแพ็คนี้ ({samplePerPackQty})", "Exceed Pack", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                RefreshAppearData();
                return;
            }

            // เช็ค projected sum QTY_SELECT ไม่เกินยอดตรวจรวมของ batch
            int projectedSumSelect = currentSumSelect + qtySelect;
            if (projectedSumSelect > maxQty)
            {
                MessageBox.Show($"ผลรวม QTY_SELECT ({projectedSumSelect}) เกินจำนวนที่ต้องตรวจทั้งหมด ({maxQty}) สำหรับชุด {batch}", "Exceed Limit", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                SyncAllNgDetailText();
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
                int latestRemainingQty = GetLatestRemainingInspectionQty();
                bool isAllComplete = latestRemainingQty <= 0;
                bool isBatchComplete = projectedSelectedPackQty >= samplePerPackQty;

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

                        if (isBatchComplete)
                        {
                            PrepareForNextBatchSelection(true);
                        }
                        else
                        {
                            PrepareForNextBatchSelection(true);
                        }
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

                        using (frmAlert alert = new frmAlert())
                        {
                            alert.ShowDialog(FindForm());
                        }

                        bt_Appear_Click(); // Back to select
                    }
                    else
                    {
                        // Not complete: Continue normally
                        MessageBox.Show(isBatchComplete ? "ชุดนี้ตรวจครบแล้ว กรุณาเลือกทำชุดอื่นต่อ" : "ทำต่อได้ปกติ", "ทำต่อได้ปกติ", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        if (isBatchComplete)
                        {
                            PrepareForNextBatchSelection(true);
                        }
                        else
                        {
                            PrepareForNextBatchSelection(true);
                        }
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
            DataTable dt = BuildPackingSelectionTable(ConvertToSafeDataTable(conQA.SearchSampleSize(propQA)));
            dtg_packing_size_appear.DataSource = dt;
            dtg_packing_size_appear_DataBindingComplete(dtg_packing_size_appear, new DataGridViewBindingCompleteEventArgs(ListChangedType.Reset));
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

        private void ShowSavedAppearDataForCurrentBatch()
        {
            DataTable savedData = conQA.SearchAppearData(propQA);
            DataTable dataSource = BuildAppearancePlanDataSource(savedData, false);

            dtg_show_appear.DataSource = null;
            dtg_show_appear.AutoGenerateColumns = true;
            dtg_show_appear.DataSource = dataSource;
            ApplyRowReadOnly();

            tb_record.Enabled = false;
            gb_input.Enabled = false;
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
            DataTable dataSource = BuildAppearancePlanDataSource(refreshedDt);

            if (currentMaxQty <= 0)
            {
                dtg_show_appear.AutoGenerateColumns = true;
                dtg_show_appear.DataSource = dataSource;
                ApplyRowReadOnly();
                tb_record.Enabled = false;
                gb_input.Enabled = false;
                return;
            }

            dtg_show_appear.AutoGenerateColumns = true;
            dtg_show_appear.DataSource = dataSource;
            ApplyRowReadOnly();
            tb_record.Enabled = true;
        }

        private void dtg_show_appear_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // 1. กรอง Event ที่ไม่จำเป็น
            if (_suppressEvents || e.RowIndex < 0 || e.ColumnIndex < 0) return;
            if (!IsInputGridRow(e.RowIndex)) return;

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

                // กฏที่ 1: Select ต้องไม่เกินจำนวนที่ควรหยิบต่อครั้ง/ต่อ pack
                if (qtySelect > currentEntryMaxQty)
                {
                    isValid = false;
                    errorMsg = $"จำนวนที่เลือก ({qtySelect}) เกินจำนวนที่ควรหยิบต่อครั้ง ({currentEntryMaxQty})";
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
                        row.Cells["JUDGE"].Value = judge == "NG"
                            ? "Pending"
                            : $"Accept{Environment.NewLine}{qtyOK} pcs.";
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
                if (IsAllAppearanceMode())
                {
                    dtg_show_appear.Invalidate();
                }
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
            if (IsInputGridRow(e.RowIndex) && e.ColumnIndex >= 0)
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
            if (_suppressEvents || e.RowIndex < 0 || !IsInputGridRow(e.RowIndex)) return;

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
                if (colName == "QTY_SELECT" && parsedValue > currentEntryMaxQty)
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

            // Reset สถานะอื่นๆ ถ้าจำเป็น
            ResetBatchSamplingContext();
            propQA.BATCH = string.Empty;
            CloseNgMode();

            // Clear selection ใน grid ด้านบนถ้าต้องการ (optional)
            dtg_packing_size_appear.ClearSelection();

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
            lb_Qty.Text = $"ระบุอาการเสียแล้ว: 0 / {totalNgRequired} ชิ้น";

            dtg_ngMode.SuspendLayout();
            try
            {
                if (dtg_ngMode.IsCurrentCellInEditMode)
                {
                    dtg_ngMode.CancelEdit();
                }

                dtg_ngMode.DataSource = null;
                dtg_ngMode.Rows.Clear();
                dtg_ngMode.Columns.Clear();
                dtg_ngMode.ClearSelection();
                dtg_ngMode.CurrentCell = null;

                DataTable ngDt = CreateNgModeDataTable(requiredNgQty);
                dtg_ngMode.AutoGenerateColumns = false;
                dtg_ngMode.DataSource = ngDt;
                ConfigureNgModeGridColumns();
                dtg_ngMode.ReadOnly = false;
            }
            finally
            {
                dtg_ngMode.ResumeLayout();
            }

            dtg_ngMode.Refresh();

            if (dtg_ngMode.Rows.Count > 0 && dtg_ngMode.Columns[NgModeHelper.ColumnName] != null)
            {
                dtg_ngMode.CurrentCell = dtg_ngMode.Rows[0].Cells[NgModeHelper.ColumnName];
                dtg_ngMode.BeginEdit(true);
            }
        }
        private void CloseNgMode()
        {
            isNgModeActive = false;
            gb_ngMode.Enabled = false;
            dtg_ngMode.DataSource = null;  // Clear data
            totalNgRequired = 0;
            lb_Qty.Text = "ระบุอาการเสียแล้ว: 0 / 0 ชิ้น";
        }

        // Handle การเปลี่ยน QTY_NG ใน dtg_show_appear (เฉพาะเมื่อ JUDGE == "NG")
        private void HandleNgQtyChange(int newQtyNg)
        {
            if (isNgModeActive && newQtyNg != totalNgRequired)
            {
                totalNgRequired = newQtyNg;
                dtg_ngMode.DataSource = CreateNgModeDataTable(newQtyNg);
                ConfigureNgModeGridColumns();
                lb_Qty.Text = $"ระบุอาการเสียแล้ว: 0 / {totalNgRequired} ชิ้น";

                if (dtg_ngMode.Rows.Count > 0 && dtg_ngMode.Columns[NgModeHelper.ColumnName] != null)
                {
                    dtg_ngMode.CurrentCell = dtg_ngMode.Rows[0].Cells[NgModeHelper.ColumnName];
                }
            }
        }
        private bool ValidateNgModeDetails(int requiredNgQty)
        {
            if (!isNgModeActive || dtg_ngMode.DataSource == null)
            {
                MessageBox.Show("พบ Pending แล้ว กรุณาระบุ NG MODE ให้ครบก่อนบันทึก", "NG MODE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                OpenNgMode(requiredNgQty);
                return false;
            }

            dtg_ngMode.EndEdit();

            DataTable ngDt = (DataTable)dtg_ngMode.DataSource;
            if (ngDt.Rows.Count == 0)
            {
                dtg_ngMode.DataSource = CreateNgModeDataTable(requiredNgQty);
                ConfigureNgModeGridColumns();
                ngDt = (DataTable)dtg_ngMode.DataSource;
            }

            int sumNg = 0;
            int completedRows = 0;

            for (int i = 0; i < ngDt.Rows.Count; i++)
            {
                DataRow row = ngDt.Rows[i];
                int qty = ParseInt(row["QTY_NG"]);
                string mode = row.Table.Columns.Contains(NgModeHelper.ColumnName) && row[NgModeHelper.ColumnName] != DBNull.Value
                    ? row[NgModeHelper.ColumnName].ToString().Trim()
                    : "";

                if (qty <= 0 && string.IsNullOrEmpty(mode))
                {
                    continue;
                }

                if (qty <= 0)
                {
                    MessageBox.Show("กรุณากรอก QTY NG ให้มากกว่า 0", "NG MODE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FocusNgModeCell(i, "QTY_NG");
                    return false;
                }

                if (string.IsNullOrEmpty(mode))
                {
                    MessageBox.Show("กรุณาเลือกอาการเสียให้ครบทุกแถวที่กรอก QTY NG", "NG MODE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    FocusNgModeCell(i, NgModeHelper.ColumnName);
                    return false;
                }

                sumNg += qty;
                completedRows++;
                SyncNgDetailText(row);
            }

            if (completedRows == 0)
            {
                MessageBox.Show("กรุณากรอก QTY NG และเลือก NG MODE อย่างน้อย 1 แถว", "NG MODE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusNgModeCell(0, "QTY_NG");
                return false;
            }

            if (sumNg != requiredNgQty)
            {
                MessageBox.Show($"ผลรวม QTY NG ({sumNg}) ต้องเท่ากับ Pending ({requiredNgQty})", "NG MODE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                FocusNgModeCell(0, "QTY_NG");
                return false;
            }

            UpdateNgSumDisplay();
            return true;
        }
        private void FocusNgModeCell(int rowIndex, string columnName)
        {
            if (dtg_ngMode.Columns[columnName] == null || rowIndex < 0 || rowIndex >= dtg_ngMode.Rows.Count)
            {
                return;
            }

            DataGridViewCell cell = dtg_ngMode.Rows[rowIndex].Cells[columnName];
            cell.Style.BackColor = Color.MistyRose;
            dtg_ngMode.ClearSelection();
            dtg_ngMode.CurrentCell = cell;
            dtg_ngMode.BeginEdit(true);
        }

        private void SyncNgDetailText(DataRow row)
        {
            if (row == null || !row.Table.Columns.Contains(NgModeHelper.ColumnName) || !row.Table.Columns.Contains("NG_DETAIL"))
            {
                return;
            }

            string modeId = row[NgModeHelper.ColumnName] == DBNull.Value ? "" : row[NgModeHelper.ColumnName].ToString();
            if (string.IsNullOrWhiteSpace(modeId))
            {
                row["NG_DETAIL"] = string.Empty;
                return;
            }

            DataRow match = ngModeList?.AsEnumerable().FirstOrDefault(item => item["VALUE"].ToString() == modeId);
            row["NG_DETAIL"] = match == null ? modeId : match["TEXT"].ToString();
        }

        private void SyncAllNgDetailText()
        {
            if (!(dtg_ngMode.DataSource is DataTable ngDt))
            {
                return;
            }

            foreach (DataRow row in ngDt.Rows)
            {
                SyncNgDetailText(row);
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
            if (!isNgModeActive || !(dtg_ngMode.DataSource is DataTable ngDt)) return;
            if (ngDt.Rows.Count >= totalNgRequired) return;

            int remainingQty = Math.Max(totalNgRequired - GetNgSum(), 0);
            AddNgModeInputRow(ngDt, remainingQty);
            UpdateNgSum();
            UpdateNgSumDisplay();
        }
        private int GetCompletedNgModeQty()
        {
            if (!(dtg_ngMode.DataSource is DataTable ngDt)) return 0;

            int completed = 0;
            foreach (DataRow row in ngDt.Rows)
            {
                string mode = row.Table.Columns.Contains(NgModeHelper.ColumnName) && row[NgModeHelper.ColumnName] != DBNull.Value
                    ? row[NgModeHelper.ColumnName].ToString().Trim()
                    : "";
                if (!string.IsNullOrEmpty(mode))
                {
                    completed += ParseInt(row["QTY_NG"]);
                }
            }
            return completed;
        }

        // Update sum display (สมมติมี Label lb_ngSum)
        private void UpdateNgSumDisplay()
        {
            int completedNg = GetCompletedNgModeQty();
            lb_Qty.Text = $"ระบุอาการเสียแล้ว: {completedNg} / {totalNgRequired} ชิ้น";
        }

        // Event for adding rows and focusing after edit ends (defer focus to avoid reentrancy)
        private void dtg_ngMode_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || _suppressNgEvents) return;
            if (!(dtg_ngMode.DataSource is DataTable ngDt) || e.RowIndex >= ngDt.Rows.Count) return;

            string columnName = dtg_ngMode.Columns[e.ColumnIndex].Name;
            DataRow row = ngDt.Rows[e.RowIndex];

            if (columnName == NgModeHelper.ColumnName)
            {
                SyncNgDetailText(row);
                dtg_ngMode.Rows[e.RowIndex].Cells[NgModeHelper.ColumnName].Style.BackColor = Color.White;
            }
            else if (columnName == "QTY_NG")
            {
                dtg_ngMode.Rows[e.RowIndex].Cells["QTY_NG"].Style.BackColor = Color.White;
            }

            ClearAllRedHighlights();
            UpdateNgSum();
            UpdateNgSumDisplay();
        }
        // Updated UpdateNgSum (remove non-numeric validation, only calculate sum and check MODE for button enable)
        private void UpdateNgSum()
        {
            DataTable ngDt = (DataTable)dtg_ngMode.DataSource;
            if (ngDt == null || !isNgModeActive) return;

            int totalNgSum = GetNgSum();
            int completedRows = 0;
            bool allRowsComplete = true;

            foreach (DataRow row in ngDt.Rows)
            {
                int qty = ParseInt(row["QTY_NG"]);
                string mode = row.Table.Columns.Contains(NgModeHelper.ColumnName) && row[NgModeHelper.ColumnName] != DBNull.Value
                    ? row[NgModeHelper.ColumnName].ToString().Trim()
                    : "";

                if (qty <= 0 && string.IsNullOrEmpty(mode))
                {
                    continue;
                }

                if (qty <= 0 || string.IsNullOrEmpty(mode))
                {
                    allRowsComplete = false;
                    break;
                }

                completedRows++;
            }

            tb_record.Enabled = totalNgRequired > 0
                && totalNgSum == totalNgRequired
                && completedRows > 0
                && allRowsComplete;
            dtg_ngMode.AllowUserToAddRows = true;
        }
        // Updated GetNgSum เพื่อ accuracy (calculate from DataTable after validation)
        private int GetNgSum()
        {
            DataTable ngDt = (DataTable)dtg_ngMode.DataSource;
            if (ngDt == null) return 0;

            int sum = 0;
            foreach (DataRow row in ngDt.Rows)
            {
                sum += ParseInt(row["QTY_NG"]);
            }
            return sum;
        }
        // Event สำหรับ RowValidating คล้าย packing_size (validate ก่อน leave row)
        private void dtg_ngMode_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
            if(e.RowIndex < 0 || dtg_ngMode.Rows[e.RowIndex].IsNewRow) return;

            var row = dtg_ngMode.Rows[e.RowIndex];
            var qtyCell = row.Cells["QTY_NG"].Value;
            var modeCell = row.Cells[NgModeHelper.ColumnName].Value;

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
                row.Cells[NgModeHelper.ColumnName].Style.BackColor = Color.White;
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
                tb.KeyPress -= qtyTextBox_KeyPress;
                string colName = dtg_show_appear.Columns[dtg_show_appear.CurrentCell.ColumnIndex].Name;
                if (colName == "QTY_SELECT" || colName == "QTY_OK" || colName == "QTY_NG")
                {
                    tb.KeyPress += qtyTextBox_KeyPress;  // Subscribe: Block non-digits
                }
            }
        }

        // *** สำหรับ dtg_ngMode ***
        private void dtg_ngMode_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            System.Windows.Forms.TextBox tb = e.Control as System.Windows.Forms.TextBox;
            if (tb == null || dtg_ngMode.CurrentCell == null) return;

            tb.KeyPress -= qtyTextBox_KeyPress;
            if (dtg_ngMode.CurrentCell.ColumnIndex == dtg_ngMode.Columns["QTY_NG"].Index)
            {
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
