using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.ViewsMaterial.CustomMsg;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RawMat.ViewsMaterial.AppearCheck
{
    public partial class userControlAppearPending : UserControl
    {
        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler BackToARequested;

        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();

        private readonly EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        private readonly imgCls imgCls = new imgCls();
        private DataTable pendingData;

        // กันเหตุการณ์วนซ้ำตอนเราเขียนค่าคอลัมน์อื่นกลับเข้าแถวเดียวกัน
        private bool suspendGridEvents;

        public userControlAppearPending()
        {
            InitializeComponent();
            ConfigurePendingGrid();
        }

        private void userControlAppearPending_Load(object sender, EventArgs e)
        {
            propQA.process = "Appearance_Check";

            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_lotSize.Text = "Lot Size : " + propQA.Qty;
            lb_inspQty.Text = "Inspection Qty : " + propQA.inspQty;

            LoadInspectionImages();
            LoadPendingData();
        }

        private void ConfigurePendingGrid()
        {
            dtg_ngMode.AutoGenerateColumns = false;
            dtg_ngMode.AllowUserToAddRows = false;
            dtg_ngMode.AllowUserToDeleteRows = false;
            dtg_ngMode.AllowUserToResizeColumns = false;
            dtg_ngMode.AllowUserToResizeRows = false;
            dtg_ngMode.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtg_ngMode.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dtg_ngMode.MultiSelect = false;
            // เฉพาะช่อง OK ที่แก้ได้ คอลัมน์อื่นตั้ง ReadOnly รายคอลัมน์
            dtg_ngMode.ReadOnly = false;
            dtg_ngMode.EditMode = DataGridViewEditMode.EditOnEnter;
            dtg_ngMode.RowHeadersVisible = false;
            dtg_ngMode.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dtg_ngMode.EnableHeadersVisualStyles = false;
            dtg_ngMode.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dtg_ngMode.ColumnHeadersHeight = 54;
            dtg_ngMode.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtg_ngMode.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11F, FontStyle.Bold);
            dtg_ngMode.DefaultCellStyle.Font = new Font("Tahoma", 10F, FontStyle.Regular);
            dtg_ngMode.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtg_ngMode.DataError += dtg_ngMode_DataError;
            dtg_ngMode.CellValidating += dtg_ngMode_CellValidating;
            dtg_ngMode.CellValueChanged += dtg_ngMode_CellValueChanged;
            dtg_ngMode.CurrentCellDirtyStateChanged += dtg_ngMode_CurrentCellDirtyStateChanged;

            dtg_ngMode.Columns.Clear();
            dtg_ngMode.Columns.Add(CreatePendingTextColumn("LOT_NO", "LOT_NO", "Lot No.", 90, DataGridViewContentAlignment.MiddleCenter));
            dtg_ngMode.Columns.Add(CreatePendingTextColumn("NG_MODE", "NG_MODE", "Q'ty Pending", 145, DataGridViewContentAlignment.MiddleLeft));
            dtg_ngMode.Columns.Add(CreatePendingTextColumn("QTY_NG", "QTY_NG", string.Empty, 70, DataGridViewContentAlignment.MiddleCenter));
            dtg_ngMode.Columns.Add(CreatePendingTextColumn("OK_QTY", "OK_QTY", "0\r\nOK", 70, DataGridViewContentAlignment.MiddleCenter, readOnly: false));
            dtg_ngMode.Columns.Add(CreatePendingTextColumn("NG_REVIEW_QTY", "NG_REVIEW_QTY", "0\r\nNG", 70, DataGridViewContentAlignment.MiddleCenter));

            ApplyReviewColumnStyle();
        }

        private DataGridViewTextBoxColumn CreatePendingTextColumn(string name, string dataPropertyName, string headerText, float fillWeight, DataGridViewContentAlignment alignment, bool readOnly = true)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
            {
                Name = name,
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                FillWeight = fillWeight,
                MinimumWidth = 60,
                ReadOnly = readOnly,
                SortMode = DataGridViewColumnSortMode.NotSortable
            };
            column.DefaultCellStyle.Alignment = alignment;
            return column;
        }

        private void LoadInspectionImages()
        {
            if (int.TryParse(propQA.CAVITY_QTY, out int cavityQty) && cavityQty > 0)
            {
                picbox_cavity.Image = imgCls.LoadCavityImage(propQA.M_CODE);
            }
            else
            {
                gb_cavity.Visible = false;
                picbox_Appear.Location = new Point(72, 115);
            }

            picbox_Appear.Image = imgCls.LoadAppearImage(propQA.M_CODE);
        }

        private void LoadPendingData()
        {
            DataTable rawPendingData = conQA.SearchAppearPendingData(propQA);
            if (rawPendingData == null || rawPendingData.Rows.Count == 0)
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                    "Appearance pending detail was not found.",
                    "Warning",
                    CustomMsgBoxBase.MessageBoxIconType.Warning);
                return;
            }

            pendingData = BuildPendingReviewTable(rawPendingData);

            suspendGridEvents = true;
            try
            {
                dtg_ngMode.DataSource = pendingData;
            }
            finally
            {
                suspendGridEvents = false;
            }

            UpdateReviewTotals();
            ApplyReviewGridStyle();
        }

        private DataTable BuildPendingReviewTable(DataTable source)
        {
            DataTable table = new DataTable();
            table.Columns.Add("LOT_NO", typeof(string));
            table.Columns.Add("QTY_NG", typeof(int));
            table.Columns.Add("NG_MODE", typeof(string));
            table.Columns.Add("OK_QTY", typeof(string));
            table.Columns.Add("NG_REVIEW_QTY", typeof(string));
            table.Columns.Add("NOTE", typeof(string));
            table.Columns.Add("JUDGEMENT", typeof(bool));
            table.Columns.Add("RESULT", typeof(string));

            table.Columns.Add("APPEARANCE_PENDING_ID", typeof(string));
            table.Columns.Add("APPEARANCE_ID", typeof(string));
            table.Columns.Add("BATCH", typeof(string));
            table.Columns.Add("COUNT", typeof(string));

            foreach (DataRow sourceRow in source.Rows)
            {
                string batch = GetString(sourceRow, "BATCH");
                string count = GetString(sourceRow, "COUNT");
                string note = GetString(sourceRow, "NOTE");
                string lotNo = GetString(sourceRow, "LOT_NO");

                int qtyNg = ParseInt(sourceRow["QTY_NG"]);
                string reviewOk = GetString(sourceRow, "REVIEW_OK_QTY");

                DataRow row = table.NewRow();
                row["LOT_NO"] = lotNo;
                row["QTY_NG"] = qtyNg;
                row["NG_MODE"] = GetString(sourceRow, "NG_MODE");
                row["OK_QTY"] = "";
                row["NG_REVIEW_QTY"] = "";
                row["NOTE"] = BuildNoteText(batch, count, lotNo, note);
                row["JUDGEMENT"] = false;
                row["RESULT"] = "";
                row["APPEARANCE_PENDING_ID"] = GetString(sourceRow, "APPEARANCE_PENDING_ID");
                row["APPEARANCE_ID"] = GetString(sourceRow, "APPEARANCE_ID");
                row["BATCH"] = batch;
                row["COUNT"] = count;
                table.Rows.Add(row);

                // เผื่อกรณีเปิดรายการที่เคย review ไว้แล้ว ให้ค่าเดิมขึ้นมาให้เห็น
                if (!string.IsNullOrWhiteSpace(reviewOk))
                {
                    ApplyPendingReviewQty(row, ParseInt(reviewOk));
                }
            }

            return table;
        }

        private string BuildNoteText(string batch, string count, string lotNo, string note)
        {
            string prefix = $"Batch {batch}, Count {count}, Lot {lotNo}";
            if (string.IsNullOrWhiteSpace(note))
            {
                return prefix;
            }

            return prefix + " - " + note;
        }

        // admin กรอกจำนวนที่ตัดสินว่า OK จริง — ที่เหลือของแถวนั้นถือเป็น NG อัตโนมัติ
        private void ApplyPendingReviewQty(DataRow row, int okQty)
        {
            int qtyPending = ParseInt(row["QTY_NG"]);

            if (okQty < 0) okQty = 0;
            if (okQty > qtyPending) okQty = qtyPending;

            int ngQty = qtyPending - okQty;

            row["OK_QTY"] = okQty.ToString();
            row["NG_REVIEW_QTY"] = ngQty.ToString();
            // RESULT = สรุปผลของทั้งแถว ให้ตรงกับที่ฝั่ง DB คาดหวัง
            row["RESULT"] = ngQty == 0 ? "1" : "0";
            row["JUDGEMENT"] = ngQty == 0;
        }

        private void UpdateReviewTotals()
        {
            int totalQty = 0;
            int okQty = 0;
            int ngQty = 0;

            if (pendingData != null)
            {
                foreach (DataRow row in pendingData.Rows)
                {
                    totalQty += ParseInt(row["QTY_NG"]);

                    // ยังไม่กรอก = ยังไม่นับเข้ายอดสรุป
                    if (string.IsNullOrWhiteSpace(row["OK_QTY"]?.ToString()))
                    {
                        continue;
                    }

                    okQty += ParseInt(row["OK_QTY"]);
                    ngQty += ParseInt(row["NG_REVIEW_QTY"]);
                }
            }

            if (dtg_ngMode.Columns.Contains("QTY_NG"))
            {
                dtg_ngMode.Columns["QTY_NG"].HeaderText = totalQty.ToString();
            }
            if (dtg_ngMode.Columns.Contains("OK_QTY"))
            {
                dtg_ngMode.Columns["OK_QTY"].HeaderText = okQty + "\r\nOK";
            }
            if (dtg_ngMode.Columns.Contains("NG_REVIEW_QTY"))
            {
                dtg_ngMode.Columns["NG_REVIEW_QTY"].HeaderText = ngQty + "\r\nNG";
            }
        }

        private void ApplyReviewColumnStyle()
        {
            if (dtg_ngMode.Columns.Contains("LOT_NO"))
            {
                dtg_ngMode.Columns["LOT_NO"].HeaderCell.Style.BackColor = Color.White;
                dtg_ngMode.Columns["LOT_NO"].DefaultCellStyle.BackColor = Color.FromArgb(245, 245, 245);
            }
            if (dtg_ngMode.Columns.Contains("NG_MODE"))
            {
                dtg_ngMode.Columns["NG_MODE"].HeaderCell.Style.BackColor = Color.FromArgb(255, 224, 192);
                dtg_ngMode.Columns["NG_MODE"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            }
            if (dtg_ngMode.Columns.Contains("QTY_NG"))
            {
                dtg_ngMode.Columns["QTY_NG"].HeaderCell.Style.BackColor = Color.White;
                dtg_ngMode.Columns["QTY_NG"].DefaultCellStyle.BackColor = Color.FromArgb(235, 235, 235);
            }
            if (dtg_ngMode.Columns.Contains("OK_QTY"))
            {
                dtg_ngMode.Columns["OK_QTY"].HeaderCell.Style.ForeColor = Color.Blue;
                dtg_ngMode.Columns["OK_QTY"].DefaultCellStyle.ForeColor = Color.Blue;
            }
            if (dtg_ngMode.Columns.Contains("NG_REVIEW_QTY"))
            {
                dtg_ngMode.Columns["NG_REVIEW_QTY"].HeaderCell.Style.ForeColor = Color.Red;
                dtg_ngMode.Columns["NG_REVIEW_QTY"].DefaultCellStyle.ForeColor = Color.Red;
            }
        }

        private void ApplyReviewGridStyle()
        {
            ApplyReviewColumnStyle();
            if (pendingData == null)
            {
                return;
            }

            for (int rowIndex = 0; rowIndex < dtg_ngMode.Rows.Count && rowIndex < pendingData.Rows.Count; rowIndex++)
            {
                DataRow dataRow = pendingData.Rows[rowIndex];
                DataGridViewRow gridRow = dtg_ngMode.Rows[rowIndex];

                // แถวที่แบ่งครึ่ง (เช่น 2 OK 1 NG) จะติดสีทั้งสองฝั่ง
                bool reviewed = !string.IsNullOrWhiteSpace(dataRow["OK_QTY"]?.ToString());
                bool hasOk = reviewed && ParseInt(dataRow["OK_QTY"]) > 0;
                bool hasNg = reviewed && ParseInt(dataRow["NG_REVIEW_QTY"]) > 0;

                gridRow.Cells["OK_QTY"].Style.BackColor = hasOk ? Color.FromArgb(255, 255, 192) : Color.White;
                gridRow.Cells["NG_REVIEW_QTY"].Style.BackColor = hasNg ? Color.FromArgb(255, 235, 235) : Color.White;
            }
        }

        private void dtg_ngMode_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            // ให้ CellValueChanged ยิงทันทีที่พิมพ์เสร็จ ไม่ต้องรอย้ายแถว
            if (dtg_ngMode.IsCurrentCellDirty)
            {
                dtg_ngMode.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dtg_ngMode_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (suspendGridEvents || e.RowIndex < 0 || e.ColumnIndex < 0 || pendingData == null)
            {
                return;
            }

            if (dtg_ngMode.Columns[e.ColumnIndex].Name != "OK_QTY" || e.RowIndex >= pendingData.Rows.Count)
            {
                return;
            }

            string text = e.FormattedValue?.ToString()?.Trim() ?? string.Empty;
            if (text.Length == 0)
            {
                return;
            }

            int qtyPending = ParseInt(pendingData.Rows[e.RowIndex]["QTY_NG"]);

            if (!int.TryParse(text, out int okQty) || okQty < 0 || okQty > qtyPending)
            {
                MessageBox.Show(
                    $"Enter a whole number between 0 and {qtyPending}.",
                    "Warning",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                e.Cancel = true;
            }
        }

        private void dtg_ngMode_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (suspendGridEvents || e.RowIndex < 0 || e.ColumnIndex < 0 || pendingData == null)
            {
                return;
            }

            if (dtg_ngMode.Columns[e.ColumnIndex].Name != "OK_QTY" || e.RowIndex >= pendingData.Rows.Count)
            {
                return;
            }

            DataRow dataRow = pendingData.Rows[e.RowIndex];
            string text = dataRow["OK_QTY"]?.ToString()?.Trim() ?? string.Empty;

            suspendGridEvents = true;
            try
            {
                if (text.Length == 0)
                {
                    // ล้างช่อง = กลับไปเป็น "ยังไม่ตัดสิน"
                    dataRow["OK_QTY"] = string.Empty;
                    dataRow["NG_REVIEW_QTY"] = string.Empty;
                    dataRow["RESULT"] = string.Empty;
                    dataRow["JUDGEMENT"] = false;
                }
                else
                {
                    ApplyPendingReviewQty(dataRow, ParseInt(text));
                }
            }
            finally
            {
                suspendGridEvents = false;
            }

            UpdateReviewTotals();
            ApplyReviewGridStyle();
        }

        private void dtg_ngMode_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
            e.Cancel = false;
        }

        private void bt_record_Click(object sender, EventArgs e)
        {
            dtg_ngMode.EndEdit();

            if (pendingData == null || pendingData.Rows.Count == 0)
            {
                MessageBox.Show("No pending data to record.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!ValidateResultSelection())
            {
                return;
            }

            // ตัดสินจากจำนวนจริง ไม่ใช่ผลรวมของแถว — แถวเดียวมีได้ทั้ง OK และ NG
            bool hasOk = pendingData.AsEnumerable().Any(row => ParseInt(row["OK_QTY"]) > 0);
            bool hasNg = pendingData.AsEnumerable().Any(row => ParseInt(row["NG_REVIEW_QTY"]) > 0);
            bool isInspectionComplete = IsAppearanceInspectionComplete();
            propQA.process = "Appearance_Check";
            propQA.EMP_ID = employee.EMP_CODE;
            propQA.dtg_ngMode = dtg_ngMode;
            propQA.inProcStatus = GetReviewProcessStatus(hasNg, isInspectionComplete);
            propQA.reportStatus = propQA.inProcStatus;
            propQA.TOTAL_STATUS = propQA.inProcStatus;

            if (!conQA.UpdateAppearPendingReview(propQA))
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                    "Record Appearance pending failed.",
                    "Error",
                    CustomMsgBoxBase.MessageBoxIconType.NG);
                return;
            }

            if (hasNg)
            {
                ShowPendingAlert(hasOk);
            }
            else
            {
                string successMessage = isInspectionComplete
                    ? "Record Appearance pending as OK completed."
                    : "Record Appearance pending as OK completed. Appearance inspection can continue.";
                CustomMsgBoxBase.ShowCustomMessageBox(
                    successMessage,
                    "Success",
                    CustomMsgBoxBase.MessageBoxIconType.OK);
            }

            loadstatus();
            bt_status_appear_pending_Click();
        }

        private void ShowPendingAlert(bool showPrintLabel)
        {
            string alertText = showPrintLabel
                ? "\u0E2D\u0E22\u0E48\u0E32\u0E25\u0E37\u0E21 Print Label \u0E41\u0E25\u0E30 Move Prones"
                : "\u0E2D\u0E22\u0E48\u0E32\u0E25\u0E37\u0E21 Move Prones";

            using (frmAlertPending alertForm = new frmAlertPending(alertText, showPrintLabel))
            {
                alertForm.ShowDialog(FindForm());
            }
        }

        private bool ValidateResultSelection()
        {
            if (pendingData == null)
            {
                return false;
            }

            for (int rowIndex = 0; rowIndex < pendingData.Rows.Count; rowIndex++)
            {
                DataRow dataRow = pendingData.Rows[rowIndex];
                string okText = dataRow["OK_QTY"]?.ToString()?.Trim() ?? string.Empty;
                int qtyPending = ParseInt(dataRow["QTY_NG"]);

                bool valid = okText.Length > 0
                    && int.TryParse(okText, out int okQty)
                    && okQty >= 0
                    && okQty <= qtyPending;

                if (!valid)
                {
                    MessageBox.Show(
                        "Enter the OK quantity for every pending row (0 means the whole row stays NG).",
                        "Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    if (rowIndex < dtg_ngMode.Rows.Count && dtg_ngMode.Columns.Contains("OK_QTY"))
                    {
                        dtg_ngMode.CurrentCell = dtg_ngMode.Rows[rowIndex].Cells["OK_QTY"];
                    }
                    return false;
                }
            }

            return true;
        }

        private string GetReviewProcessStatus(bool hasNg, bool isInspectionComplete)
        {
            if (hasNg)
            {
                return ((int)QAdataProperty.ProcStatus.NG).ToString();
            }

            return isInspectionComplete
                ? ((int)QAdataProperty.ProcStatus.Finished).ToString()
                : ((int)QAdataProperty.ProcStatus.Unfinished).ToString();
        }

        private bool IsAppearanceInspectionComplete()
        {
            int targetQty = ParseInt(propQA.inspQty);
            if (targetQty <= 0)
            {
                targetQty = ParseInt(propQA.Qty);
            }

            if (targetQty <= 0)
            {
                return false;
            }

            try
            {
                return conQA.GetTotalInspected(propQA) >= targetQty;
            }
            catch
            {
                return false;
            }
        }

        private void bt_status_appear_pending_Click()
        {
            userControlSelectAppearPending usrSelectAppearPending = new userControlSelectAppearPending();
            usrSelectAppearPending.Dock = DockStyle.Fill;
            usrSelectAppearPending.propQA = propQA;

            Form mainForm = this.FindForm();
            if (mainForm == null)
            {
                return;
            }

            Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
            if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
            {
                panelMain.Controls.Clear();
                panelMain.Controls.Add(usrSelectAppearPending);
                usrSelectAppearPending.BringToFront();
            }
            else
            {
                MessageBox.Show("Main panel was not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void loadstatus()
        {
            if (this.ParentForm is frmMain mainForm)
            {
                mainForm.LoadStatus();
            }
        }

        private string GetString(DataRow row, string columnName)
        {
            if (row == null || !row.Table.Columns.Contains(columnName) || row[columnName] == DBNull.Value)
            {
                return string.Empty;
            }

            return row[columnName].ToString();
        }

        private int ParseInt(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return 0;
            }

            return int.TryParse(value.ToString(), out int number) ? number : 0;
        }
    }
}
