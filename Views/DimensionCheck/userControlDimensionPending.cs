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

namespace RawMat.Views.DimensionCheck
{
    public partial class userControlDimensionPending : UserControl
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

        private bool isUpdating = false; // ตัวแปรควบคุมเพื่อป้องกันการเรียกซ้ำ

        // Dictionary เพื่อเก็บ VALUE ของแต่ละ POINT_ORDER และ SAMPLING_NO
        private Dictionary<string, Dictionary<string, decimal>> pointValues = new Dictionary<string, Dictionary<string, decimal>>();

        private List<Image> dimensionImages;
        private int currentDimensionImageIndex = 0;
        private Image _defaultImage = null; // ถ้าไม่ต้องการ placeholder จริง

        public userControlDimensionPending()
        {
            InitializeComponent();
        }

        private void userControlDimensionPending_Load(object sender, EventArgs e)
        {

            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");

            dtg_dimension.CellEndEdit -= dtg_dimension_CellEndEdit;
            dtg_dimension.CellValidating -= dtg_dimension_CellValidating;
            dtg_dimension.CellFormatting -= dtg_dimension_CellFormatting;
            dtg_dimension.CellFormatting += dtg_dimension_CellFormatting;

            //tb_pageMax.Text = ""; //มาจาก info_Dimension_sampling 
            //tb_pageCount.Text = ""; // 1 record 2 record จนถึง pageMax
            lb_sampName.Text = propQA.SAMPLING_QTY + " " + propQA.SAMPLING_NAME;

            DataTable dtDimPending = conQA.SearchDimensionDataPending(propQA);

            if (dtDimPending.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบ data Dimension ที่ Pending ใน db_Dimension_data");
                return;
            }


            DataTable dtAllSum = new DataTable();


            if (propQA.SAMPLING_TYPE == "4" || propQA.SAMPLING_TYPE == "3")
            {
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

                foreach (DataRow dtRow in dtDimPending.Rows)
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

                picbox_cavity.Image = imgCls.LoadCavityImage(propQA.M_CODE);
                picbox_dim.Image = imgCls.LoadDimensionImage(propQA.M_CODE);

            }
            else
            {
                gb_cavity.Visible = false;

                picbox_dim.Location = new System.Drawing.Point(231, 120);
                picbox_dim.Size = new Size(815, 442);
                picbox_dim.Image = imgCls.LoadDimensionImage(propQA.M_CODE);

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

                foreach (DataRow dtRow in dtDimPending.Rows)
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

            dtg_dimension.DataSource = dtAllSum;

            string[] hiddenColumns = { "POINT_CAL", "POINT_ORDER", "EQUIPMENT_TYPE", "POINT_JUDGE", "TOTAL_JUDGE" , "EQUIPMENT_SERIAL" };
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

            UpdateReadOnlyCells();

            originalDataTable = (DataTable)dtg_dimension.DataSource;
            bindingSource.DataSource = originalDataTable;
            dtg_dimension.DataSource = bindingSource;

           

            // เปลี่ยน HeaderText
            if (dtg_dimension.Columns.Contains("CAVITY_NAME")) dtg_dimension.Columns["CAVITY_NAME"].HeaderText = "CAV.";
            if (dtg_dimension.Columns.Contains("SAMPLING_NO")) dtg_dimension.Columns["SAMPLING_NO"].HeaderText = "SAMPLE";
            if (dtg_dimension.Columns.Contains("POINT_NAME")) dtg_dimension.Columns["POINT_NAME"].HeaderText = "CHECKPOINT";
            if (dtg_dimension.Columns.Contains("EQUIPMENT_SERIAL")) dtg_dimension.Columns["EQUIPMENT_SERIAL"].HeaderText = "EQ_SN";
            if (dtg_dimension.Columns.Contains("EQUIPMENT_NAME")) dtg_dimension.Columns["EQUIPMENT_NAME"].HeaderText = "EQ_NAME ";
            if (dtg_dimension.Columns.Contains("CRITERIA_MIN")) dtg_dimension.Columns["CRITERIA_MIN"].HeaderText = "MIN";
            if (dtg_dimension.Columns.Contains("CRITERIA_MAX")) dtg_dimension.Columns["CRITERIA_MAX"].HeaderText = "MAX";

            totalPages = originalDataTable.AsEnumerable()
                .Select(row => row["POINT_ORDER"].ToString())
                .Distinct()
                .Count();

            ShowPage(currentPage);

            dtg_dimension.CellEndEdit += dtg_dimension_CellEndEdit;
            dtg_dimension.CellValidating += dtg_dimension_CellValidating;
            dtg_dimension.DataBindingComplete += dtg_dimension_DataBindingComplete;

            this.Disposed += UserControlDimension_Disposed;
            this.Focus();

        }

        //private void ShowPage(int page)
        //{
        //    bindingSource.Filter = $"POINT_ORDER = '{page}'"; // กรองเฉพาะแถวที่มี POINT_ORDER ตรงกับหน้า
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

                        if (columnName == "VALUE")
                        {
                            string pointCal = row["POINT_CAL"]?.ToString();
                            if (string.IsNullOrEmpty(pointCal) || pointCal == "0")
                            {
                                CustomMsgBoxBase.ShowCustomMessageBox($"พบเซลล์ว่างในหน้า {pageNumber}, Sampling No {samplingNo}, คอลัมน์ {columnName}",
                                   "คำเตือน", CustomMsgBoxBase.MessageBoxIconType.Warning);
                                return false;
                            }
                        }
                        else
                        {

                            CustomMsgBoxBase.ShowCustomMessageBox($"พบเซลล์ว่างในหน้า {pageNumber}, Sampling No {samplingNo}, คอลัมน์ {columnName}",
                                "คำเตือน", CustomMsgBoxBase.MessageBoxIconType.Warning);
                        }
                    }
                }
            }
            return true;
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

        private bool IsValidDecimal(string input)
        {
            // เช็คว่าค่าที่ใส่เป็นตัวเลข และมีจุดทศนิยมไม่เกิน 1 จุด
            return decimal.TryParse(input, out _) && input.Count(c => c == '.') <= 1;
        }

        private void dtg_dimension_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dtg_dimension.Columns[e.ColumnIndex].Name == "VALUE")
            {

                CalculatePointValues();

                //DataGridViewRow row = dtg_dimension.Rows[e.RowIndex];

                //if (row.Cells["CRITERIA_MIN"].Value != null &&
                //    row.Cells["CRITERIA_MAX"].Value != null &&
                //    row.Cells["VALUE"].Value != null &&
                //    !DBNull.Value.Equals(row.Cells["CRITERIA_MIN"].Value) &&
                //    !DBNull.Value.Equals(row.Cells["CRITERIA_MAX"].Value) &&
                //    !DBNull.Value.Equals(row.Cells["VALUE"].Value))
                //{
                //    decimal min = Convert.ToDecimal(row.Cells["CRITERIA_MIN"].Value);
                //    decimal max = Convert.ToDecimal(row.Cells["CRITERIA_MAX"].Value);
                //    decimal value;

                //    if (decimal.TryParse(row.Cells["VALUE"].Value.ToString(), out value))
                //    {
                //        // คำนวณ Point_Judge (1 ถ้าอยู่ในช่วง min-max, 0 ถ้านอกช่วง)
                //        row.Cells["POINT_JUDGE"].Value = (value >= min && value <= max) ? 1 : 0;
                //    }
                //    else
                //    {
                //        row.Cells["POINT_JUDGE"].Value = DBNull.Value; // ถ้าค่าไม่ถูกต้อง ให้เป็นค่าว่าง
                //    }

                //    // คำนวณ Total_Judge
                //    CalculateTotalJudge();
                //}
            }
        }

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
                dtRow["TOTAL_JUDGE"] = value;
            }
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

            propQA.dtgDimData = new DataGridView();
            propQA.dtgDimData.DataSource = originalDataTable;


            if (conQA.InsertDimensionData(propQA) == true)
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
                        case ProcStatus.NG:
                            CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Dimension พบงาน ถูก NG",
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
                    bt_status_Dimension_pending_Click();
                    return;
                }
                else
                {
                    CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Dimension ไม่ได้ กรุณากด record อีกครั้ง",
                                "ข้อผิดพลาด",
                                CustomMsgBoxBase.MessageBoxIconType.NG);
                    return;
                }
            }
            else
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                                "Record Dimension ไม่ได้ กรุณากด record อีกครั้ง",
                                "ข้อผิดพลาด",
                                CustomMsgBoxBase.MessageBoxIconType.NG);
                return;
            }

        }

        //Dimension pending 
        private void bt_status_Dimension_pending_Click()
        {
            userControlSelectDimensionPending usrSelectDimPending = new userControlSelectDimensionPending();
            usrSelectDimPending.Dock = DockStyle.Fill;
            usrSelectDimPending.propQA = propQA;

            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
                //Control[] foundPanels = this.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    // เคลียร์และเพิ่ม UserControl ใหม่
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrSelectDimPending);
                    usrSelectDimPending.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

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
        private void UserControlDimension_Disposed(object sender, EventArgs e)
        {
            // Dispose logic เดิม
            if (dimensionImages != null)
            {
                foreach (var img in dimensionImages)
                {
                    img?.Dispose();
                }
                dimensionImages.Clear();
                dimensionImages = null;
            }

            // Dispose อื่นๆ ถ้ามี (เช่น materialImages, cavityImages ถ้า hold list ไว้)

            // Unsubscribe event เพื่อป้องกัน memory leak
            this.Disposed -= UserControlDimension_Disposed;
        }



    }
}
