using RawMat.Controllers;
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
using static Humanizer.On;
using static RawMat.Property.QAdataProperty;

namespace RawMat.ViewsMaterial.FunctionCheck
{
    public partial class userControlFunctionPending : UserControl
    {
        private const string FunctionCheckColumnPrefix = "FUNCTION_CHECK_";
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

        private List<Image> functionImages;
        private int currentFunctionImageIndex = 0;
        private Image _defaultImage = null; // ถ้าไม่ต้องการ placeholder จริง

        public userControlFunctionPending()
        {
            InitializeComponent();
            dtg_function.DataError += dtg_function_DataError;
        }

        private void dtg_function_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
        }

        private async void userControlFunctionPending_Load(object sender, EventArgs e)
        {
            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_lotSize.Text = "Lot Size : " + propQA.Qty;

            dtg_function.CellEndEdit -= dtg_function_CellEndEdit;

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
            picbox_cavity.Image = imgCls.LoadSingleImage("CavityPath", propQA.M_CODE); // สมมติมี key "CavityPath" ใน app.config

            DataTable dtFuncPending = conQA.SearchFunctionDataPending(propQA);
            DataTable dtFuncCheckResult = conQA.SearchFunctionCheckResultPending(propQA);

            if (dtFuncPending.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบ data function ที่ Pending ใน db_function_data");
                return;
            }

            DataTable dtAllSum = new DataTable();



            if (propQA.SAMPLING_TYPE == "4" || propQA.SAMPLING_TYPE == "3")
            {

                picbox_cavity.Image = imgCls.LoadCavityImage(propQA.M_CODE);
                //picbox_func.Image = imgCls.LoadFunctionImage(propQA.M_CODE);
                //picbox_mat.Image = imgCls.LoadMaterialImage(propQA.M_CODE);



                dtAllSum.Columns.Add("CAVITY_NAME", typeof(string));
                dtAllSum.Columns.Add("SAMPLING_NO", typeof(int));
                dtAllSum.Columns.Add("LOT_NO", typeof(string));
                dtAllSum.Columns.Add("EQUIPMENT_SERIAL_ID", typeof(int));
                dtAllSum.Columns.Add("EQUIPMENT_SERIAL", typeof(string));
                dtAllSum.Columns.Add("EQUIPMENT_TYPE", typeof(string));
                dtAllSum.Columns.Add("EQUIPMENT_NAME", typeof(string));
                dtAllSum.Columns.Add("JUDGE", typeof(string)); // เก็บเป็น string เพื่อให้สอดคล้องกับ Key
                dtAllSum.Columns.Add("REMARK", typeof(string));
                dtAllSum.Columns.Add("EMP_ID", typeof(string));
                dtAllSum.Columns.Add("FUNCTION_DATE", typeof(string));
                dtAllSum.Columns.Add("POINT_JUDGE", typeof(string));
                dtAllSum.Columns.Add("TOTAL_JUDGE", typeof(string));

                foreach (DataRow dtRow in dtFuncPending.Rows)
                {
                    // แปลงค่า JUDGE ให้ตรงกับ Key ใน ComboBox
                    string judgeValue = dtRow["JUDGE"].ToString().Trim();
                    if (judgeValue == "NG" || judgeValue == "0") judgeValue = "0";
                    else if (judgeValue == "OK" || judgeValue == "1") judgeValue = "1";
                    else if (string.IsNullOrEmpty(judgeValue)) judgeValue = "";
                    else judgeValue = ""; // ถ้าไม่รู้จักให้เป็นค่าว่าง

                    dtAllSum.Rows.Add(dtRow["CAVITY_NAME"].ToString(),
                        dtRow["SAMPLING_NO"].ToString(),
                        dtRow["LOT_NO"],
                        dtRow["EQUIPMENT_SERIAL_ID"],
                        dtRow["EQUIPMENT_SERIAL"],
                        dtRow["EQUIPMENT_TYPE"],
                        dtRow["EQUIPMENT_NAME"],
                        judgeValue, // ใช้ค่า judgeValue ที่แปลงแล้ว
                        dtRow["REMARK"].ToString(),
                        dtRow["EMP_ID"].ToString(),
                        dtRow["FUNCTION_DATE"].ToString(),
                        "0", "0");
                }
            }
            else
            {
                gb_cavity.Visible = false;

                dtAllSum.Columns.Add("SAMPLING_NO", typeof(int));
                dtAllSum.Columns.Add("LOT_NO", typeof(string));
                dtAllSum.Columns.Add("EQUIPMENT_SERIAL_ID", typeof(int));
                dtAllSum.Columns.Add("EQUIPMENT_SERIAL", typeof(string));
                dtAllSum.Columns.Add("EQUIPMENT_TYPE", typeof(string));
                dtAllSum.Columns.Add("EQUIPMENT_NAME", typeof(string));
                dtAllSum.Columns.Add("JUDGE", typeof(string)); // เก็บเป็น string เพื่อให้สอดคล้องกับ Key
                dtAllSum.Columns.Add("REMARK", typeof(string));
                dtAllSum.Columns.Add("EMP_ID", typeof(string));
                dtAllSum.Columns.Add("FUNCTION_DATE", typeof(string));
                dtAllSum.Columns.Add("POINT_JUDGE", typeof(string));
                dtAllSum.Columns.Add("TOTAL_JUDGE", typeof(string));

                foreach (DataRow dtRow in dtFuncPending.Rows)
                {
                    // แปลงค่า JUDGE ให้ตรงกับ Key ใน ComboBox
                    string judgeValue = dtRow["JUDGE"].ToString().Trim();
                    if (judgeValue == "NG" || judgeValue == "0") judgeValue = "0";
                    else if (judgeValue == "OK" || judgeValue == "1") judgeValue = "1";
                    else if (string.IsNullOrEmpty(judgeValue)) judgeValue = "";
                    else judgeValue = ""; // ถ้าไม่รู้จักให้เป็นค่าว่าง

                    dtAllSum.Rows.Add(
                        dtRow["SAMPLING_NO"].ToString(),
                        dtRow["LOT_NO"],
                        dtRow["EQUIPMENT_SERIAL_ID"],
                        dtRow["EQUIPMENT_SERIAL"],
                        dtRow["EQUIPMENT_TYPE"],
                        dtRow["EQUIPMENT_NAME"],
                        judgeValue, // ใช้ค่า judgeValue ที่แปลงแล้ว
                        dtRow["REMARK"].ToString(),
                        dtRow["EMP_ID"].ToString(),
                        dtRow["FUNCTION_DATE"].ToString(),
                        "0", "0");
                }
            }

            bool hasFunctionChecks = dtFuncCheckResult != null && dtFuncCheckResult.Rows.Count > 0;
            if (hasFunctionChecks)
            {
                propQA.dtFuncCheck = BuildFunctionCheckMethodTable(dtFuncCheckResult);
                foreach (DataRow check in propQA.dtFuncCheck.Rows)
                {
                    string columnName = GetFunctionCheckColumnName(check);
                    dtAllSum.Columns.Add(columnName, typeof(string));
                }

                foreach (DataRow sample in dtAllSum.Rows)
                {
                    string samplingNo = sample["SAMPLING_NO"].ToString();
                    foreach (DataRow result in dtFuncCheckResult.Rows)
                    {
                        if (result["SAMPLING_NO"].ToString() != samplingNo) continue;
                        string columnName = FunctionCheckColumnPrefix + result["FUNCTION_CHECK_ID"];
                        if (dtAllSum.Columns.Contains(columnName))
                        {
                            sample[columnName] = result["JUDGE"];
                        }
                    }
                }
            }

            dtg_function.DataSource = dtAllSum;

            bool hasEquipment = dtFuncPending.AsEnumerable().Any(row =>
                !string.IsNullOrWhiteSpace(row["EQUIPMENT_TYPE"]?.ToString()) ||
                !string.IsNullOrWhiteSpace(row["EQUIPMENT_SERIAL"]?.ToString()));

            string[] hiddenColumns = { "POINT_JUDGE", "TOTAL_JUDGE", "LOT_NO", "EQUIPMENT_SERIAL_ID", "EQUIPMENT_TYPE" };
            foreach (var col in hiddenColumns)
            {
                if (dtg_function.Columns.Contains(col))
                {
                    dtg_function.Columns[col].Visible = false;
                }
            }

            originalDataTable = (DataTable)dtg_function.DataSource;
            bindingSource.DataSource = originalDataTable;
            dtg_function.DataSource = bindingSource;

            // ทำให้คอลัมน์ที่ไม่ใช่ "JUDGE" เป็น ReadOnly
            foreach (DataGridViewColumn column in dtg_function.Columns)
            {
                column.ReadOnly = column.Name != "JUDGE" &&
                                  !column.Name.StartsWith(FunctionCheckColumnPrefix, StringComparison.Ordinal);
            }

            dtg_function.Columns["JUDGE"].Visible = !hasFunctionChecks;

            // เปลี่ยน HeaderText
            if (dtg_function.Columns.Contains("CAVITY_NAME")) dtg_function.Columns["CAVITY_NAME"].HeaderText = "CAV.";
            if (dtg_function.Columns.Contains("SAMPLING_NO")) dtg_function.Columns["SAMPLING_NO"].HeaderText = "SAMPLE";
            dtg_function.Columns["EQUIPMENT_SERIAL"].Visible = hasEquipment;
            dtg_function.Columns["EQUIPMENT_NAME"].Visible = hasEquipment;
            dtg_function.Columns["EQUIPMENT_SERIAL"].HeaderText = "EQ_SN";
            dtg_function.Columns["EQUIPMENT_NAME"].HeaderText = "EQ_NAME";

            // แปลงคอลัมน์ JUDGE เป็น ComboBox โดยใช้คอลัมน์เดิม
            if (dtg_function.Columns.Contains("JUDGE") && !hasFunctionChecks)
            {
                // ลบคอลัมน์ JUDGE เดิมออกจาก DataGridView
                DataGridViewColumn oldColumn = dtg_function.Columns["JUDGE"];
                int columnIndex = oldColumn.Index;
                dtg_function.Columns.Remove("JUDGE");

                // สร้าง DataGridViewComboBoxColumn
                DataGridViewComboBoxColumn comboBoxColumn = new DataGridViewComboBoxColumn
                {
                    Name = "JUDGE",
                    HeaderText = "YES / NO",
                    DataPropertyName = "JUDGE", // เชื่อมโยงกับคอลัมน์ JUDGE ใน DataTable
                    DataSource = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("", ""),
                new KeyValuePair<string, string>("0", "NO"),
                new KeyValuePair<string, string>("1", "YES")
            },
                    ValueMember = "Key",
                    DisplayMember = "Value"
                };

                // เพิ่ม ComboBox กลับไปที่ตำแหน่งเดิม
                dtg_function.Columns.Insert(columnIndex,comboBoxColumn);
            }

            if (hasFunctionChecks)
            {
                foreach (DataRow check in propQA.dtFuncCheck.Rows)
                {
                    ReplaceWithYesNoColumn(GetFunctionCheckColumnName(check), check["CHECK_DETAIL"]?.ToString());
                }
            }

            // กำหนดค่า "0 NG" (Key = "0") ให้ทุกแถวในคอลัมน์ JUDGE
            foreach (DataGridViewRow row in dtg_function.Rows)
            {
                if (!hasFunctionChecks &&
                    (row.Cells["JUDGE"].Value == null || string.IsNullOrEmpty(row.Cells["JUDGE"].Value.ToString())))
                {
                    row.Cells["JUDGE"].Value = "0"; // กำหนดให้เป็น "0" ซึ่งจะแสดง "0 NG"
                }
            }

            dtg_function.CellEndEdit += dtg_function_CellEndEdit;
            this.Disposed += UserControlFunction_Disposed;

            this.Focus();
        }

        private void dtg_function_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            string editedColumn = dtg_function.Columns[e.ColumnIndex].Name;
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

        private static DataTable BuildFunctionCheckMethodTable(DataTable results)
        {
            var methods = new DataTable();
            methods.Columns.Add("ID");
            methods.Columns.Add("CHECK_ORDER");
            methods.Columns.Add("CHECK_DETAIL");

            var seen = new HashSet<string>();
            foreach (DataRow result in results.Select("", "CHECK_ORDER ASC"))
            {
                string id = result["FUNCTION_CHECK_ID"].ToString();
                if (!seen.Add(id)) continue;
                methods.Rows.Add(id, result["CHECK_ORDER"], result["CHECK_DETAIL"]);
            }
            return methods;
        }

        private static string GetFunctionCheckColumnName(DataRow check)
        {
            return FunctionCheckColumnPrefix + check["ID"];
        }

        private void ReplaceWithYesNoColumn(string columnName, string headerText)
        {
            if (!dtg_function.Columns.Contains(columnName)) return;
            int index = dtg_function.Columns[columnName].Index;
            dtg_function.Columns.Remove(columnName);
            dtg_function.Columns.Insert(index, new DataGridViewComboBoxColumn
            {
                Name = columnName,
                DataPropertyName = columnName,
                HeaderText = headerText,
                DataSource = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("", ""),
                    new KeyValuePair<string, string>("0", "NO"),
                    new KeyValuePair<string, string>("1", "YES")
                },
                ValueMember = "Key",
                DisplayMember = "Value",
                MinimumWidth = 150
            });
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
                propQA.inProcStatus = "0";
                propQA.reportStatus = "0";
            }
            else
            {
                propQA.inProcStatus = "1";
                propQA.reportStatus = "1";
            }

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
                    case ProcStatus.NG:
                        CustomMsgBoxBase.ShowCustomMessageBox(
                            "Record Function พบงาน NG",
                            "สำเร็จ",
                            CustomMsgBoxBase.MessageBoxIconType.NG);
                        break;
                    default:
                        CustomMsgBoxBase.ShowCustomMessageBox(
                            "สถานะไม่รู้จัก",
                            "ข้อผิดพลาด",
                            CustomMsgBoxBase.MessageBoxIconType.Question);
                        break;
                }

                loadstatus();
                bt_status_function_pending_Click();
                return;
            }
            else
            {
                CustomMsgBoxBase.ShowCustomMessageBox("ไม่สามารถ record function data ได้ กรุณากดอีกครั้ง", "ข้อผิดพลาด", CustomMsgBoxBase.MessageBoxIconType.NG);
                //string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";
                //RequestReleaseMutex?.Invoke(mutexKey);
                return;
            }
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
                        MessageBox.Show($"พบเซลล์ว่างในแถวที่ {row.Index + 1} คอลัมน์ {dtg.Columns[cell.ColumnIndex].HeaderText}", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dtg.CurrentCell = cell;
                        return false;
                    }
                }

                // Specific validation for REMARK when VALUE is "NG"
                if (valueCell.Value != null && valueCell.Value.ToString() == "0")
                {
                    if (remarkCell.Value == null || string.IsNullOrWhiteSpace(remarkCell.Value.ToString()))
                    {
                        MessageBox.Show($"กรุณากรอก REMARK สำหรับแถวที่ {row.Index + 1} ซึ่งมี VALUE เป็น NG!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                        MessageBox.Show($"REMARK ในแถวที่ {row.Index + 1} มีความยาวเกิน 255 ตัวอักษร (ปัจจุบัน: {remarkText.Length} ตัวอักษร)!", "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dtg.CurrentCell = remarkCell;
                        return false;
                    }
                }
            }

            return true;
        }

        private void bt_status_function_pending_Click()
        {
            userControlSelectFunctionPending usrSelectFuncPending = new userControlSelectFunctionPending();
            usrSelectFuncPending.Dock = DockStyle.Fill;
            usrSelectFuncPending.propQA = propQA;

            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
                //Control[] foundPanels = this.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    // เคลียร์และเพิ่ม UserControl ใหม่
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrSelectFuncPending);
                    usrSelectFuncPending.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

    

        private void loadstatus()
        {
            if (this.ParentForm is frmMain mainForm)
            {
                mainForm.LoadStatus();
            }
        }

        // เพิ่ม method ใหม่นี้ใน class
        private void UserControlFunction_Disposed(object sender, EventArgs e)
        {
            // Dispose logic เดิม
            if (functionImages != null)
            {
                foreach (var img in functionImages)
                {
                    img?.Dispose();
                }
                functionImages.Clear();
                functionImages = null;
            }

            // Dispose อื่นๆ ถ้ามี (เช่น materialImages, cavityImages ถ้า hold list ไว้)

            // Unsubscribe event เพื่อป้องกัน memory leak
            this.Disposed -= UserControlFunction_Disposed;
        }

    }
}
