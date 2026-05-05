using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.CustomMsg;
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

namespace RawMat.Views.RegularCheck
{
    public partial class userControlRegularPending : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler BackToARequested;
        //public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();

        private BindingSource bindingSource = new BindingSource();
        private DataTable originalDataTable;
        private int currentPage = 1;
        private int totalPages = 1;

        private List<Image> regularImages;
        private int currentRegularImageIndex = 0;
        private Image _defaultImage = null; // ถ้าไม่ต้องการ placeholder จริง

        public userControlRegularPending()
        {
            InitializeComponent();
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

            //tb_pageMax.Text = ""; //มาจาก info_regular_sampling 
            //tb_pageCount.Text = ""; // 1 record 2 record จนถึง pageMax
            lb_sampName.Text = propQA.SAMPLING_QTY + " " + propQA.SAMPLING_NAME;

            // โหลดรูป Function แบบ async (สำหรับ pagination ด้วย list ถ้ามีหลายรูป)
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


            DataTable dtRegPending = conQA.SearchRegularDataPending(propQA);

            if (dtRegPending.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบ data regular ที่ Pending ใน db_regular_data");
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
                dtAllSum.Columns.Add("CRITERIA_MIN", typeof(double));
                dtAllSum.Columns.Add("CRITERIA_MAX", typeof(double));
                dtAllSum.Columns.Add("POINT_JUDGE", typeof(string));
                dtAllSum.Columns.Add("TOTAL_JUDGE", typeof(string));

                foreach (DataRow dtRow in dtRegPending.Rows)
                {

                    dtAllSum.Rows.Add(dtRow["CAVITY_NAME"].ToString(),
                    dtRow["SAMPLING_NO"].ToString(),
                    dtRow["POINT_ORDER"].ToString(),
                    dtRow["POINT_CAL"].ToString(),
                    dtRow["EQUIPMENT_SERIAL_ID"].ToString(),
                    dtRow["EQUIPMENT_TYPE"].ToString(),
                    dtRow["EQUIPMENT_NAME"].ToString(),
                    dtRow["POINT_NAME"].ToString(),
                    dtRow["VALUE"].ToString(),
                    Convert.ToDouble(dtRow["CRITERIA_MIN"]),
                    Convert.ToDouble(dtRow["CRITERIA_MAX"]),
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
                dtAllSum.Columns.Add("CRITERIA_MIN", typeof(double));
                dtAllSum.Columns.Add("CRITERIA_MAX", typeof(double));
                dtAllSum.Columns.Add("POINT_JUDGE", typeof(string));
                dtAllSum.Columns.Add("TOTAL_JUDGE", typeof(string));

                foreach (DataRow dtRow in dtRegPending.Rows)
                {

                    dtAllSum.Rows.Add(
                    dtRow["SAMPLING_NO"].ToString(),
                    dtRow["POINT_ORDER"].ToString(),
                    dtRow["POINT_CAL"].ToString(),
                    dtRow["EQUIPMENT_SERIAL_ID"].ToString(),
                    dtRow["EQUIPMENT_TYPE"].ToString(),
                    dtRow["EQUIPMENT_NAME"].ToString(),
                    dtRow["POINT_NAME"].ToString(),
                    dtRow["VALUE"].ToString(),
                    Convert.ToDouble(dtRow["CRITERIA_MIN"]),
                    Convert.ToDouble(dtRow["CRITERIA_MAX"]),
                    "0", "0"
                    );

                }

            }

            dtg_regular.DataSource = dtAllSum;

            string[] hiddenColumns = { "POINT_CAL", "POINT_ORDER", "EQUIPMENT_TYPE", "POINT_JUDGE", "TOTAL_JUDGE" , "EQUIPMENT_SERIAL" };
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
            if (dtg_regular.Columns.Contains("EQUIPMENT_NAME")) dtg_regular.Columns["EQUIPMENT_NAME"].HeaderText = "EQ_NAME ";
            if (dtg_regular.Columns.Contains("CRITERIA_MIN")) dtg_regular.Columns["CRITERIA_MIN"].HeaderText = "MIN";
            if (dtg_regular.Columns.Contains("CRITERIA_MAX")) dtg_regular.Columns["CRITERIA_MAX"].HeaderText = "MAX";

            totalPages = originalDataTable.AsEnumerable()
                .Select(row => row["POINT_ORDER"].ToString())
                .Distinct()
                .Count();

            ShowPage(currentPage);

            dtg_regular.CellEndEdit += dtg_regular_CellEndEdit;
            dtg_regular.CellValidating += dtg_regular_CellValidating;

            this.Disposed += UserControlRegular_Disposed;

            this.Focus();

        }

        private void ShowPage(int page)
        {
            bindingSource.Filter = $"POINT_ORDER = '{page}'"; // กรองเฉพาะแถวที่มี POINT_ORDER ตรงกับหน้า
            lb_page.Text = $"{page}/{totalPages}"; // แสดงหน้า (1/8)
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
                // ดึงหมายเลขหน้า (POINT_ORDER)
                int pageNumber = row["POINT_ORDER"] != DBNull.Value ? Convert.ToInt32(row["POINT_ORDER"]) : 0;

                // ดึงค่า Sampling No (อ้างอิงแทน Row Index)
                string samplingNo = row["SAMPLING_NO"] != DBNull.Value ? row["SAMPLING_NO"].ToString() : "N/A";

                foreach (DataColumn column in table.Columns)
                {
                    if (row[column] == DBNull.Value || string.IsNullOrWhiteSpace(row[column].ToString()))
                    {
                        string columnName = column.ColumnName; // ชื่อคอลัมน์

                        MessageBox.Show($"พบเซลล์ว่างในหน้า {pageNumber}, Sampling No {samplingNo}, คอลัมน์ {columnName}",
                            "คำเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return false;
                    }
                }
            }
            return true;
        }


        private void dtg_regular_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dtg_regular.Columns[e.ColumnIndex].Name == "VALUE")
            {
                // ตรวจสอบว่าข้อมูลใน CRITERIA_MIN และ CRITERIA_MAX มีค่า
                if (dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value != null &&
                    dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MAX"].Value != null)
                {
                    double minValue = Convert.ToDouble(dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MIN"].Value);
                    double maxValue = Convert.ToDouble(dtg_regular.Rows[e.RowIndex].Cells["CRITERIA_MAX"].Value);

                    // เงื่อนไข: ถ้า CRITERIA_MIN == 1 && CRITERIA_MAX == 1 ให้ใช้ ComboBoxCell
                    if (minValue == 1 && maxValue == 1)
                    {
                        // ตรวจสอบว่าเซลล์ VALUE ยังไม่ใช่ ComboBoxCell
                        if (!(dtg_regular.Rows[e.RowIndex].Cells[e.ColumnIndex] is DataGridViewComboBoxCell))
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
            // เช็คว่าค่าที่ใส่เป็นตัวเลข และมีจุดทศนิยมไม่เกิน 1 จุด
            return decimal.TryParse(input, out _) && input.Count(c => c == '.') <= 1;
        }

        private void dtg_regular_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
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
                        // คำนวณ Point_Judge (1 ถ้าอยู่ในช่วง min-max, 0 ถ้านอกช่วง)
                        row.Cells["POINT_JUDGE"].Value = (value >= min && value <= max) ? 1 : 0;
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
                dtRow["TOTAL_JUDGE"] = value;
            }
        }

        private void tb_record_Click(object sender, EventArgs e)
        {
            // บันทึกค่าที่กำลังแก้ไขใน DataGridView
            if (dtg_regular.IsCurrentCellDirty || dtg_regular.IsCurrentRowDirty)
            {
                dtg_regular.EndEdit(); // จบการแก้ไขเซลล์ปัจจุบัน
                dtg_regular.CommitEdit(DataGridViewDataErrorContexts.Commit); // บันทึกค่าลง DataSource
                bindingSource.EndEdit(); // บันทึกค่าลงใน BindingSource (ถ้าใช้)
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

                propQA.TOTAL_STATUS = (Convert.ToInt32(row["TOTAL_JUDGE"]?.ToString()) * Convert.ToInt32(propQA.TOTAL_STATUS)).ToString();
            }

            propQA.dtgRegData = new DataGridView();
            propQA.dtgRegData.DataSource = originalDataTable;


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
                    status = parsed ? (ProcStatus)statusId : ProcStatus.NG; // ค่าเริ่มต้นเป็น NG ถ้าแปลงไม่ได้

                    switch (status)
                    {
                        case ProcStatus.OK:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Regular งาน OK เรียบร้อยแล้ว",
                                "สำเร็จ",
                                CustomMsgBoxBase.MessageBoxIconType.OK);
                            break;
                        case ProcStatus.NG:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Regular พบงาน ถูก NG",
                                "สำเร็จ",
                                CustomMsgBoxBase.MessageBoxIconType.NG);
                            break;
                        default:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "สถานะไม่รู้จัก",
                                "ข้อผิดพลาด",
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
                                "Record regular status ไม่ได้ กรุณากด record อีกครั้ง",
                                "ข้อผิดพลาด",
                                CustomMsgBoxBase.MessageBoxIconType.NG);
                    return;
                }
            }
            else
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record regular ไม่ได้ กรุณากด record อีกครั้ง",
                                "ข้อผิดพลาด",
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
                    // เคลียร์และเพิ่ม UserControl ใหม่
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrSelectRegPending);
                    usrSelectRegPending.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        private void UserControlRegular_Disposed(object sender, EventArgs e)
        {
            // Dispose logic เดิม
            if (regularImages != null)
            {
                foreach (var img in regularImages)
                {
                    img?.Dispose();
                }
                regularImages.Clear();
                regularImages = null;
            }

            // Dispose อื่นๆ ถ้ามี (เช่น materialImages, cavityImages ถ้า hold list ไว้)

            // Unsubscribe event เพื่อป้องกัน memory leak
            this.Disposed -= UserControlRegular_Disposed;
        }


    }
}
