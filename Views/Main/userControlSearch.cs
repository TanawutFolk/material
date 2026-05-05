using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
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

namespace RawMat.Views.Main
{
    public partial class userControlSearch : UserControl
    {
        QAdataControllers conQA = new QAdataControllers();
        QAdataProperty propQA = new QAdataProperty();
        docCls doc = new docCls();
        private DataTable receiveMatData; // เพิ่มตัวแปรนี้เพื่อเก็บข้อมูล
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        private DateTime today;

        // ตัวแปรนับจำนวนการคลิก
        private int clickCount = 0;

        public userControlSearch()
        {
            InitializeComponent();
          
        }


        private void bt_findData_Click(object sender, EventArgs e)
        {

        }

        private void userControlSearch_Load(object sender, EventArgs e)
        {

            bt_export.Visible = employee?.EMP_LEVEL == "1";

            today = conQA.SearchToday();
            dtp_recDateSearch.Value = today;



            receiveMatData = conQA.SearchReceiveMatAll();
            //null
            if (receiveMatData == null)
            {
                return;
            }    
            dtg_receiveMatSearch.DataSource = receiveMatData;

            dtg_receiveMatSearch.Sort(dtg_receiveMatSearch.Columns["Report No"], ListSortDirection.Descending);


            // Assuming cb_vendorSearch is a ComboBox control
            cb_vendorSearch.Items.Clear();
            var uniqueVendors = new HashSet<string>();
            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueVendors.Add(row["Vendor Name"].ToString());
            }
            cb_vendorSearch.Items.AddRange(uniqueVendors.ToArray());

            // Set ComboBox to DropDownList style and enable auto-complete
            cb_vendorSearch.DropDownStyle = ComboBoxStyle.DropDown;
            cb_vendorSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb_vendorSearch.AutoCompleteSource = AutoCompleteSource.ListItems;


            cb_repSearch.Items.Clear();
            var uniqueRep = new HashSet<string>();

            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueRep.Add(row["Report No"].ToString());
            }
            cb_repSearch.Items.AddRange(uniqueRep.ToArray());

            // Set ComboBox to DropDownList style and enable auto-complete
            cb_repSearch.DropDownStyle = ComboBoxStyle.DropDown;
            cb_repSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb_repSearch.AutoCompleteSource = AutoCompleteSource.ListItems;


            cb_mCode.Items.Clear();
            var uniqueMcode= new HashSet<string>();

            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueMcode.Add(row["M-CODE"].ToString());
            }
            cb_mCode.Items.AddRange(uniqueMcode.ToArray());

            // Set ComboBox to DropDownList style and enable auto-complete
            cb_mCode.DropDownStyle = ComboBoxStyle.DropDown;
            cb_mCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb_mCode.AutoCompleteSource = AutoCompleteSource.ListItems;

            // ซ่อนคอลัมน์ Regular_Check
            if (dtg_receiveMatSearch.Columns.Contains("Regular_Check"))
            {
                dtg_receiveMatSearch.Columns["Regular_Check"].Visible = false;
            }

            // เพิ่มการตั้งค่า CellFormatting และ CellMouseEnter/Leave
            dtg_receiveMatSearch.CellFormatting += dtg_receiveMatSearch_CellFormatting;
            dtg_receiveMatSearch.CellMouseEnter += dtg_receiveMatSearch_CellMouseEnter;
            dtg_receiveMatSearch.CellMouseLeave += dtg_receiveMatSearch_CellMouseLeave;
            dtg_receiveMatSearch.CellClick += dtg_receiveMatSearch_CellClick;

        }

        // ทำให้คอลัมน์ "Regular No" ดูเหมือนลิงก์
        private void dtg_receiveMatSearch_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (dtg_receiveMatSearch.Columns[e.ColumnIndex].Name == "Regular No" ||
                dtg_receiveMatSearch.Columns[e.ColumnIndex].HeaderText == "Regular No")
            {
                e.CellStyle.Font = new Font(dtg_receiveMatSearch.Font, FontStyle.Underline);
                e.CellStyle.ForeColor = Color.Blue;
                e.CellStyle.SelectionForeColor = Color.Blue;
                e.CellStyle.SelectionBackColor = Color.LightBlue;
            }
        }

        // เปลี่ยน cursor เป็นรูปมือเมื่อเมาส์วางบนคอลัมน์ "Regular No"
        private void dtg_receiveMatSearch_CellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                if (dtg_receiveMatSearch.Columns[e.ColumnIndex].Name == "Regular No" ||
                    dtg_receiveMatSearch.Columns[e.ColumnIndex].HeaderText == "Regular No")
                {
                    dtg_receiveMatSearch.Cursor = Cursors.Hand;
                }
                else
                {
                    dtg_receiveMatSearch.Cursor = Cursors.Default;
                }
            }
        }

        private void dtg_receiveMatSearch_CellMouseLeave(object sender, DataGridViewCellEventArgs e)
        {
            dtg_receiveMatSearch.Cursor = Cursors.Default;
        }

        // จัดการการคลิกที่คอลัมน์ "Regular No"
        private void dtg_receiveMatSearch_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // ตรวจสอบว่าเป็นการคลิกที่คอลัมน์ "Regular No" และไม่ใช่ header row
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                string columnName = dtg_receiveMatSearch.Columns[e.ColumnIndex].Name;
                if (string.IsNullOrEmpty(columnName))
                {
                    columnName = dtg_receiveMatSearch.Columns[e.ColumnIndex].HeaderText;
                }

                if (columnName == "Regular No")
                {
                    // ตรวจสอบ EMP_LEVEL ก่อนเปิดฟอร์ม
                    if (employee?.EMP_LEVEL != "1")
                    {
                        //MessageBox.Show("คุณไม่มีสิทธิ์ในการเข้าถึงฟังก์ชันนี้", "สิทธิ์ไม่เพียงพอ",
                        //    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // วิธีที่ถูกต้อง: ดึงข้อมูลจากแถวที่ถูกเลือกใน DataGridView โดยตรง
                    DataGridViewRow selectedRow = dtg_receiveMatSearch.Rows[e.RowIndex];

                    // ดึงค่า Regular No และ Report No
                    propQA.Regular_No = dtg_receiveMatSearch.Rows[e.RowIndex].Cells["Regular No"].Value?.ToString();
                    propQA.Report_No = dtg_receiveMatSearch.Rows[e.RowIndex].Cells["Report No"].Value?.ToString();

                    // ดึง Regular_Check จาก DataBoundItem (วิธีนี้จะได้ข้อมูลที่ถูกต้องแม้จะมีการ Sort)
                    string regularCheck = "0";

                    // วิธีที่ 1: ใช้ DataBoundItem (แนะนำที่สุด)
                    if (selectedRow.DataBoundItem != null)
                    {
                        DataRowView rowView = selectedRow.DataBoundItem as DataRowView;
                        if (rowView != null && rowView.Row.Table.Columns.Contains("Regular_Check"))
                        {
                            regularCheck = rowView.Row["Regular_Check"]?.ToString() ?? "0";
                        }
                        else
                        {
                            // กรณีเป็น object อื่นๆ
                            var propertyInfo = selectedRow.DataBoundItem.GetType().GetProperty("Regular_Check");
                            if (propertyInfo != null)
                            {
                                regularCheck = propertyInfo.GetValue(selectedRow.DataBoundItem)?.ToString() ?? "0";
                            }
                        }
                    }

                    // วิธีที่ 2: ถ้าวิธีแรกไม่ได้ผล ให้ค้นหาจาก DataTable โดยใช้ Report No หรือ Regular No เป็น key
                    if (regularCheck == "0" && receiveMatData != null && !string.IsNullOrEmpty(propQA.Report_No))
                    {
                        DataRow[] foundRows = receiveMatData.Select($"`Report No` = '{propQA.Report_No.Replace("'", "''")}'");
                        if (foundRows.Length > 0 && foundRows[0].Table.Columns.Contains("Regular_Check"))
                        {
                            regularCheck = foundRows[0]["Regular_Check"]?.ToString() ?? "0";
                        }
                    }

                    // ตรวจสอบว่า d.Regular_Check = 1 หรือไม่
                    if (regularCheck == "1")
                    {
                        using (FormRegularReportStamp stampForm = new FormRegularReportStamp(propQA))
                        {
                            if (stampForm.ShowDialog() == DialogResult.OK)
                            {
                                // TODO: หลังจากอัปเดตสำเร็จ อาจจะ refresh DataGridView
                                MessageBox.Show("อัปเดตข้อมูลเรียบร้อยแล้ว", "สำเร็จ",
                                    MessageBoxButtons.OK, MessageBoxIcon.Information);

                                // รีเฟรชข้อมูล
                                RefreshData();
                            }
                        }
                    }
                    else
                    {
                          MessageBox.Show("สถานะ Regular Check ไม่พร้อมสำหรับการประทับตรา", "แจ้งเตือน",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
        }

        private void RefreshData()
        {
            // รีเฟรชข้อมูลตามสถานะปัจจุบัน
            if (rb_all.Checked)
            {
                receiveMatData = conQA.SearchReceiveMatAll();
            }
            else if (rb_statusProcess.Checked)
            {
                receiveMatData = conQA.SearchReceiveMatStatusProcess();
            }
            else
            {
                receiveMatData = conQA.SearchReceiveMatAll();
            }

            if (receiveMatData != null)
            {
                dtg_receiveMatSearch.DataSource = receiveMatData;
                dtg_receiveMatSearch.Sort(dtg_receiveMatSearch.Columns["Report No"], ListSortDirection.Descending);

                // อัปเดต ComboBox items
                UpdateComboBoxItems();
            }
        }

        // เมธอดสำหรับอัปเดต ComboBox
        private void UpdateComboBoxItems()
        {
            // อัปเดต Vendor
            cb_vendorSearch.Items.Clear();
            var uniqueVendors = new HashSet<string>();
            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueVendors.Add(row["Vendor Name"].ToString());
            }
            cb_vendorSearch.Items.AddRange(uniqueVendors.ToArray());

            // อัปเดต Report No
            cb_repSearch.Items.Clear();
            var uniqueRep = new HashSet<string>();
            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueRep.Add(row["Report No"].ToString());
            }
            cb_repSearch.Items.AddRange(uniqueRep.ToArray());

            // อัปเดต M-Code
            cb_mCode.Items.Clear();
            var uniqueMcode = new HashSet<string>();
            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueMcode.Add(row["M-CODE"].ToString());
            }
            cb_mCode.Items.AddRange(uniqueMcode.ToArray());
        }

        private void SetDatepickerForSpecificDate()
        {
            dtp_recDateSearch.Format = DateTimePickerFormat.Custom;
            dtp_recDateSearch.CustomFormat = "dd/MM/yyyy";
            dtp_recDateSearch.ShowUpDown = false;
            dtp_recDateSearch.MinDate = new DateTime(1900, 1, 1);
            dtp_recDateSearch.MaxDate = new DateTime(2100, 12, 31);
            dtp_recDateSearch.Enabled = true;
            dtp_recDateSearch.Value = today; // ตั้งค่าเริ่มต้นเป็นวันที่ปัจจุบัน
        }

        private void SetDatepickerForMonthYear()
        {
            dtp_recDateSearch.Format = DateTimePickerFormat.Custom;
            dtp_recDateSearch.CustomFormat = "MM/yyyy";
            dtp_recDateSearch.ShowUpDown = true; // แสดงปุ่มขึ้น-ลงเพื่อเปลี่ยนเดือนและปี
            dtp_recDateSearch.MinDate = new DateTime(1900, 1, 1);
            dtp_recDateSearch.MaxDate = new DateTime(2100, 12, 31);
            dtp_recDateSearch.Enabled = true;
            dtp_recDateSearch.Value = new DateTime(today.Year, today.Month, 1); // ตั้งค่าเริ่มต้นเป็นวันที่แรกของเดือนปัจจุบัน
        }





        private void tb_vendorSearch_TextChanged(object sender, EventArgs e)
        {
            string filterText = cb_vendorSearch.Text;
            var filteredRows = receiveMatData.AsEnumerable()
                .Where(row => row.Field<string>("Vendor Name").IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            if (filteredRows.Any())
            {
                DataTable filteredTable = filteredRows.CopyToDataTable();
                dtg_receiveMatSearch.DataSource = filteredTable;
            }
            else
            {
                dtg_receiveMatSearch.DataSource = null; // or handle the empty case as needed
            }
        }

        private void cb_repSearch_TextChanged(object sender, EventArgs e)
        {
            string filterText = cb_repSearch.Text;
            var filteredRows = receiveMatData.AsEnumerable()
                .Where(row => row.Field<string>("Report No").IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            if (filteredRows.Any())
            {
                DataTable filteredTable = filteredRows.CopyToDataTable();
                dtg_receiveMatSearch.DataSource = filteredTable;
            }
            else
            {
                dtg_receiveMatSearch.DataSource = null; // or handle the empty case as needed
            }
        }

        private void dtp_recDateSearch_onValueChanged(object sender, EventArgs e)
        {
            if (receiveMatData == null)
            {
                // Handle the null case, perhaps by logging an error or showing a message to the user
                return;
            }

            DateTime selectedDate = dtp_recDateSearch.Value;

            var filteredRows = receiveMatData.AsEnumerable();

            if (rbSpecificDate.Checked)
            {
                // กรองตามวันที่เต็ม (วัน-เดือน-ปี)
                filteredRows = filteredRows.Where(row => row.Field<DateTime>("Receive Date").Date == selectedDate.Date);
            }
            else if (rbMonthYear.Checked)
            {
                // กรองตามเดือนและปี
                int selectedMonth = selectedDate.Month;
                int selectedYear = selectedDate.Year;
                filteredRows = filteredRows.Where(row => row.Field<DateTime>("Receive Date").Month == selectedMonth &&
                                                       row.Field<DateTime>("Receive Date").Year == selectedYear);
            }
            else
            {
                // Handle case where no radio button is selected (optional)
                dtg_receiveMatSearch.DataSource = null;
                return;
            }

            if (filteredRows.Any())
            {
                DataTable filteredTable = filteredRows.CopyToDataTable();
                dtg_receiveMatSearch.DataSource = filteredTable;
            }
            else
            {
                dtg_receiveMatSearch.DataSource = null; // or handle the empty case as needed
            }
        }

        //Mcode_onValueChanged


        private void rb_all_CheckedChanged(object sender, EventArgs e)
        {
            DateTime today = conQA.SearchToday();
            dtp_recDateSearch.Value = today;

            receiveMatData = conQA.SearchReceiveMatAll();
            dtg_receiveMatSearch.DataSource = receiveMatData;

            // Assuming cb_vendorSearch is a ComboBox control
            cb_vendorSearch.Items.Clear();
            var uniqueVendors = new HashSet<string>();
            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueVendors.Add(row["Vendor Name"].ToString());
            }
            cb_vendorSearch.Items.AddRange(uniqueVendors.ToArray());

            // Set ComboBox to DropDownList style and enable auto-complete
            cb_vendorSearch.DropDownStyle = ComboBoxStyle.DropDown;
            cb_vendorSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb_vendorSearch.AutoCompleteSource = AutoCompleteSource.ListItems;


            cb_repSearch.Items.Clear();
            var uniqueRep = new HashSet<string>();

            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueRep.Add(row["Report No"].ToString());
            }
            cb_repSearch.Items.AddRange(uniqueRep.ToArray());

            // Set ComboBox to DropDownList style and enable auto-complete
            cb_repSearch.DropDownStyle = ComboBoxStyle.DropDown;
            cb_repSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb_repSearch.AutoCompleteSource = AutoCompleteSource.ListItems;


            cb_mCode.Items.Clear();
            var uniqueMcode = new HashSet<string>();

            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueMcode.Add(row["M-CODE"].ToString());
            }
            cb_mCode.Items.AddRange(uniqueMcode.ToArray());

            // Set ComboBox to DropDownList style and enable auto-complete
            cb_mCode.DropDownStyle = ComboBoxStyle.DropDown;
            cb_mCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb_mCode.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void rb_statusProcess_CheckedChanged(object sender, EventArgs e)
        {
            DateTime today = conQA.SearchToday();
            dtp_recDateSearch.Value = today;

            receiveMatData = conQA.SearchReceiveMatStatusProcess();
            dtg_receiveMatSearch.DataSource = receiveMatData;

            // Assuming cb_vendorSearch is a ComboBox control
            cb_vendorSearch.Items.Clear();
            var uniqueVendors = new HashSet<string>();
            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueVendors.Add(row["Vendor Name"].ToString());
            }
            cb_vendorSearch.Items.AddRange(uniqueVendors.ToArray());

            // Set ComboBox to DropDownList style and enable auto-complete
            cb_vendorSearch.DropDownStyle = ComboBoxStyle.DropDown;
            cb_vendorSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb_vendorSearch.AutoCompleteSource = AutoCompleteSource.ListItems;


            cb_repSearch.Items.Clear();
            var uniqueRep = new HashSet<string>();

            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueRep.Add(row["Report No"].ToString());
            }
            cb_repSearch.Items.AddRange(uniqueRep.ToArray());

            // Set ComboBox to DropDownList style and enable auto-complete
            cb_repSearch.DropDownStyle = ComboBoxStyle.DropDown;
            cb_repSearch.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb_repSearch.AutoCompleteSource = AutoCompleteSource.ListItems;


            cb_mCode.Items.Clear();
            var uniqueMcode = new HashSet<string>();

            foreach (DataRow row in receiveMatData.Rows)
            {
                uniqueMcode.Add(row["M-CODE"].ToString());
            }
            cb_mCode.Items.AddRange(uniqueMcode.ToArray());

            // Set ComboBox to DropDownList style and enable auto-complete
            cb_mCode.DropDownStyle = ComboBoxStyle.DropDown;
            cb_mCode.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cb_mCode.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void cb_mCode_TextChanged(object sender, EventArgs e)
        {
            string filterText = cb_mCode.Text;
            var filteredRows = receiveMatData.AsEnumerable()
                .Where(row => row.Field<string>("M-CODE").IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            if (filteredRows.Any())
            {
                DataTable filteredTable = filteredRows.CopyToDataTable();
                dtg_receiveMatSearch.DataSource = filteredTable;
            }
            else
            {
                dtg_receiveMatSearch.DataSource = null; // or handle the empty case as needed
            }
        }

        private void bt_export_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.Title = "Save as CSV File";
            saveFileDialog.FileName = "export.csv";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                doc.ExportToCSV(dtg_receiveMatSearch, saveFileDialog.FileName);
            }
        }

        private void rbSpecificDate_CheckedChanged(object sender, EventArgs e)
        {
            if (rbSpecificDate.Checked)
            {
                SetDatepickerForSpecificDate();
            }
        }

        private void rbMonthYear_CheckedChanged(object sender, EventArgs e)
        {
            if (rbMonthYear.Checked)
            {
                SetDatepickerForMonthYear();
            }
        }

       
    }
}
