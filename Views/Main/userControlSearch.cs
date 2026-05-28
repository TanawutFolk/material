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
        private bool isLoadingData;

        // ตัวแปรนับจำนวนการคลิก
        private int clickCount = 0;

        public userControlSearch()
        {
            InitializeComponent();
            dtg_receiveMatSearch.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            dtg_receiveMatSearch.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }


        private void bt_findData_Click(object sender, EventArgs e)
        {

        }

        private async void userControlSearch_Load(object sender, EventArgs e)
        {

            bt_export.Visible = employee?.EMP_LEVEL == "1";
            WireGridEvents();
            await LoadReceiveMatDataAsync(false);
        }

        private async Task LoadReceiveMatDataAsync(bool statusProcessOnly)
        {
            if (isLoadingData)
            {
                return;
            }

            isLoadingData = true;
            SetFilterControlsEnabled(false);
            dtg_receiveMatSearch.DataSource = null;

            try
            {
                DataTable data = null;
                DateTime dbToday = DateTime.Now;

                await Task.Run(() =>
                {
                    QAdataControllers controller = new QAdataControllers();
                    dbToday = controller.SearchToday();
                    data = statusProcessOnly
                        ? controller.SearchReceiveMatStatusProcess()
                        : controller.SearchReceiveMatAll();
                });

                if (IsDisposed)
                {
                    return;
                }

                today = dbToday;
                dtp_recDateSearch.Value = today;
                receiveMatData = data;

                BindReceiveMatData();
            }
            finally
            {
                isLoadingData = false;
                SetFilterControlsEnabled(true);
            }
        }

        private void BindReceiveMatData()
        {
            if (receiveMatData == null)
            {
                return;
            }

            dtg_receiveMatSearch.SuspendLayout();
            dtg_receiveMatSearch.DataSource = receiveMatData;
            dtg_receiveMatSearch.ResumeLayout();

            if (dtg_receiveMatSearch.Columns.Contains("Regular_Check"))
            {
                dtg_receiveMatSearch.Columns["Regular_Check"].Visible = false;
            }

            UpdateComboBoxItems();
        }

        private void WireGridEvents()
        {
            dtg_receiveMatSearch.CellFormatting -= dtg_receiveMatSearch_CellFormatting;
            dtg_receiveMatSearch.CellMouseEnter -= dtg_receiveMatSearch_CellMouseEnter;
            dtg_receiveMatSearch.CellMouseLeave -= dtg_receiveMatSearch_CellMouseLeave;
            dtg_receiveMatSearch.CellClick -= dtg_receiveMatSearch_CellClick;

            dtg_receiveMatSearch.CellFormatting += dtg_receiveMatSearch_CellFormatting;
            dtg_receiveMatSearch.CellMouseEnter += dtg_receiveMatSearch_CellMouseEnter;
            dtg_receiveMatSearch.CellMouseLeave += dtg_receiveMatSearch_CellMouseLeave;
            dtg_receiveMatSearch.CellClick += dtg_receiveMatSearch_CellClick;
        }

        private void SetFilterControlsEnabled(bool enabled)
        {
            cb_vendorSearch.Enabled = enabled;
            cb_repSearch.Enabled = enabled;
            cb_mCode.Enabled = enabled;
            dtp_recDateSearch.Enabled = enabled;
            rb_all.Enabled = enabled;
            rb_statusProcess.Enabled = enabled;
            rbSpecificDate.Enabled = enabled;
            rbMonthYear.Enabled = enabled;
        }

        private void SetComboBoxAutoComplete(ComboBox comboBox)
        {
            comboBox.DropDownStyle = ComboBoxStyle.DropDown;
            comboBox.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            comboBox.AutoCompleteSource = AutoCompleteSource.ListItems;
        }

        private void SetComboBoxItems(ComboBox comboBox, IEnumerable<string> values)
        {
            comboBox.BeginUpdate();
            comboBox.Items.Clear();
            comboBox.Items.AddRange(values.Where(value => !string.IsNullOrWhiteSpace(value))
                                          .Distinct()
                                          .OrderBy(value => value)
                                          .ToArray());
            comboBox.EndUpdate();
            SetComboBoxAutoComplete(comboBox);
        }

        private void ApplyFilteredRows(IEnumerable<DataRow> filteredRows)
        {
            DataRow[] rows = filteredRows.ToArray();
            dtg_receiveMatSearch.DataSource = rows.Length > 0 ? rows.CopyToDataTable() : null;
        }

        private string GetRowText(DataRow row, string columnName)
        {
            return row.Table.Columns.Contains(columnName) ? row[columnName]?.ToString() ?? string.Empty : string.Empty;
        }

        private DateTime? GetRowDate(DataRow row, string columnName)
        {
            if (!row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return null;
            }

            return Convert.ToDateTime(row[columnName]);
        }

        private bool HasReceiveMatData()
        {
            return receiveMatData != null && receiveMatData.Rows.Count > 0;
        }


        // ทำให้คอลัมน์ "Regular No" ดูเหมือนลิงก์
        private void dtg_receiveMatSearch_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex < 0)
            {
                return;
            }

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

                    // เปิด Regular Report เมื่อรอ approve หรือ approved แล้ว
                    if (regularCheck == ((int)QAdataProperty.ProcStatus.WaitingApprove).ToString()
                        || regularCheck == ((int)QAdataProperty.ProcStatus.OK).ToString())
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

        private async void RefreshData()
        {
            await LoadReceiveMatDataAsync(rb_statusProcess.Checked);
        }

        // เมธอดสำหรับอัปเดต ComboBox
        private void UpdateComboBoxItems()
        {
            if (!HasReceiveMatData())
            {
                cb_vendorSearch.Items.Clear();
                cb_repSearch.Items.Clear();
                cb_mCode.Items.Clear();
                return;
            }

            SetComboBoxItems(cb_vendorSearch, receiveMatData.AsEnumerable().Select(row => GetRowText(row, "Vendor Name")));
            SetComboBoxItems(cb_repSearch, receiveMatData.AsEnumerable().Select(row => GetRowText(row, "Report No")));
            SetComboBoxItems(cb_mCode, receiveMatData.AsEnumerable().Select(row => GetRowText(row, "M-CODE")));
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
            if (isLoadingData || !HasReceiveMatData())
            {
                return;
            }

            string filterText = cb_vendorSearch.Text;
            var filteredRows = receiveMatData.AsEnumerable()
                .Where(row => GetRowText(row, "Vendor Name").IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            ApplyFilteredRows(filteredRows);
        }

        private void cb_repSearch_TextChanged(object sender, EventArgs e)
        {
            if (isLoadingData || !HasReceiveMatData())
            {
                return;
            }

            string filterText = cb_repSearch.Text;
            var filteredRows = receiveMatData.AsEnumerable()
                .Where(row => GetRowText(row, "Report No").IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            ApplyFilteredRows(filteredRows);
        }

        private void dtp_recDateSearch_onValueChanged(object sender, EventArgs e)
        {
            if (isLoadingData || !HasReceiveMatData())
            {
                return;
            }

            DateTime selectedDate = dtp_recDateSearch.Value;

            var filteredRows = receiveMatData.AsEnumerable();

            if (rbSpecificDate.Checked)
            {
                filteredRows = filteredRows.Where(row => GetRowDate(row, "Receive Date")?.Date == selectedDate.Date);
            }
            else if (rbMonthYear.Checked)
            {
                int selectedMonth = selectedDate.Month;
                int selectedYear = selectedDate.Year;
                filteredRows = filteredRows.Where(row =>
                {
                    DateTime? receiveDate = GetRowDate(row, "Receive Date");
                    return receiveDate.HasValue
                        && receiveDate.Value.Month == selectedMonth
                        && receiveDate.Value.Year == selectedYear;
                });
            }
            else
            {
                dtg_receiveMatSearch.DataSource = null;
                return;
            }

            ApplyFilteredRows(filteredRows);
        }

        //Mcode_onValueChanged


        private async void rb_all_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingData || !rb_all.Checked)
            {
                return;
            }

            await LoadReceiveMatDataAsync(false);
        }

        private async void rb_statusProcess_CheckedChanged(object sender, EventArgs e)
        {
            if (isLoadingData || !rb_statusProcess.Checked)
            {
                return;
            }

            await LoadReceiveMatDataAsync(true);
        }

        private void cb_mCode_TextChanged(object sender, EventArgs e)
        {
            if (isLoadingData || !HasReceiveMatData())
            {
                return;
            }

            string filterText = cb_mCode.Text;
            var filteredRows = receiveMatData.AsEnumerable()
                .Where(row => GetRowText(row, "M-CODE").IndexOf(filterText, StringComparison.OrdinalIgnoreCase) >= 0);

            ApplyFilteredRows(filteredRows);
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
