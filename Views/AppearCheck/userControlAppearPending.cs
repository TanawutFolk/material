using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.CustomMsg;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace RawMat.Views.AppearCheck
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
            dtg_ngMode.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtg_ngMode.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dtg_ngMode.MultiSelect = false;
            dtg_ngMode.ReadOnly = true;
            dtg_ngMode.EditMode = DataGridViewEditMode.EditProgrammatically;
            dtg_ngMode.RowHeadersVisible = false;
            dtg_ngMode.EnableHeadersVisualStyles = false;
            dtg_ngMode.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dtg_ngMode.ColumnHeadersHeight = 54;
            dtg_ngMode.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtg_ngMode.ColumnHeadersDefaultCellStyle.Font = new Font("Tahoma", 11F, FontStyle.Bold);
            dtg_ngMode.DefaultCellStyle.Font = new Font("Tahoma", 10F, FontStyle.Regular);
            dtg_ngMode.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtg_ngMode.DataError += dtg_ngMode_DataError;
            dtg_ngMode.CellClick += dtg_ngMode_CellClick;

            dtg_ngMode.Columns.Clear();
            dtg_ngMode.Columns.Add(CreatePendingTextColumn("NG_MODE", "NG_MODE", "Q'ty Pending", 145, DataGridViewContentAlignment.MiddleLeft));
            dtg_ngMode.Columns.Add(CreatePendingTextColumn("QTY_NG", "QTY_NG", string.Empty, 70, DataGridViewContentAlignment.MiddleCenter));
            dtg_ngMode.Columns.Add(CreatePendingTextColumn("OK_QTY", "OK_QTY", "0\r\nOK", 70, DataGridViewContentAlignment.MiddleCenter));
            dtg_ngMode.Columns.Add(CreatePendingTextColumn("NG_REVIEW_QTY", "NG_REVIEW_QTY", "0\r\nNG", 70, DataGridViewContentAlignment.MiddleCenter));

            ApplyReviewColumnStyle();
        }

        private DataGridViewTextBoxColumn CreatePendingTextColumn(string name, string dataPropertyName, string headerText, float fillWeight, DataGridViewContentAlignment alignment)
        {
            DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn
            {
                Name = name,
                DataPropertyName = dataPropertyName,
                HeaderText = headerText,
                FillWeight = fillWeight,
                MinimumWidth = 60,
                ReadOnly = true,
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
            dtg_ngMode.DataSource = pendingData;
            UpdateReviewTotals();
            ApplyReviewGridStyle();
        }

        private DataTable BuildPendingReviewTable(DataTable source)
        {
            DataTable table = new DataTable();
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

                DataRow row = table.NewRow();
                row["QTY_NG"] = ParseInt(sourceRow["QTY_NG"]);
                row["NG_MODE"] = GetString(sourceRow, "NG_MODE");
                row["OK_QTY"] = "";
                row["NG_REVIEW_QTY"] = "";
                row["NOTE"] = BuildNoteText(batch, count, note);
                row["JUDGEMENT"] = false;
                row["RESULT"] = "";
                row["APPEARANCE_PENDING_ID"] = GetString(sourceRow, "APPEARANCE_PENDING_ID");
                row["APPEARANCE_ID"] = GetString(sourceRow, "APPEARANCE_ID");
                row["BATCH"] = batch;
                row["COUNT"] = count;
                table.Rows.Add(row);
            }

            return table;
        }

        private string BuildNoteText(string batch, string count, string note)
        {
            string prefix = $"Batch {batch}, Count {count}";
            if (string.IsNullOrWhiteSpace(note))
            {
                return prefix;
            }

            return prefix + " - " + note;
        }

        private void dtg_ngMode_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || pendingData == null)
            {
                return;
            }

            string columnName = dtg_ngMode.Columns[e.ColumnIndex].Name;
            if (columnName == "OK_QTY")
            {
                SetPendingReviewResult(e.RowIndex, "1");
            }
            else if (columnName == "NG_REVIEW_QTY")
            {
                SetPendingReviewResult(e.RowIndex, "0");
            }
        }

        private void SetPendingReviewResult(int rowIndex, string result)
        {
            if (rowIndex < 0 || rowIndex >= pendingData.Rows.Count)
            {
                return;
            }

            ApplyPendingReviewResult(pendingData.Rows[rowIndex], result);
            UpdateReviewTotals();
            ApplyReviewGridStyle();
        }

        private void ApplyPendingReviewResult(DataRow row, string result)
        {
            int qtyPending = ParseInt(row["QTY_NG"]);
            row["RESULT"] = result;
            row["JUDGEMENT"] = result == "1";
            row["OK_QTY"] = result == "1" ? qtyPending.ToString() : string.Empty;
            row["NG_REVIEW_QTY"] = result == "0" ? qtyPending.ToString() : string.Empty;
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
                    int qtyPending = ParseInt(row["QTY_NG"]);
                    totalQty += qtyPending;

                    string result = row["RESULT"]?.ToString() ?? string.Empty;
                    if (result == "1")
                    {
                        okQty += qtyPending;
                    }
                    else if (result == "0")
                    {
                        ngQty += qtyPending;
                    }
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
                string result = pendingData.Rows[rowIndex]["RESULT"]?.ToString() ?? string.Empty;
                DataGridViewRow gridRow = dtg_ngMode.Rows[rowIndex];

                gridRow.Cells["OK_QTY"].Style.BackColor = result == "1" ? Color.FromArgb(255, 255, 192) : Color.White;
                gridRow.Cells["NG_REVIEW_QTY"].Style.BackColor = result == "0" ? Color.FromArgb(255, 235, 235) : Color.White;
            }
        }

        private void dtg_ngMode_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dtg_ngMode.IsCurrentCellDirty)
            {
                dtg_ngMode.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void dtg_ngMode_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || pendingData == null)
            {
                return;
            }

            if (dtg_ngMode.Columns[e.ColumnIndex].Name != "RESULT")
            {
                return;
            }

            DataGridViewRow row = dtg_ngMode.Rows[e.RowIndex];
            string result = row.Cells["RESULT"].Value?.ToString() ?? "";
            row.Cells["JUDGEMENT"].Value = result == "1";
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

            bool hasNg = pendingData.AsEnumerable().Any(row => row["RESULT"].ToString() == "0");
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
                CustomMsgBoxBase.ShowCustomMessageBox(
                    "Record Appearance pending as NG completed.",
                    "Success",
                    CustomMsgBoxBase.MessageBoxIconType.NG);
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

        private bool ValidateResultSelection()
        {
            if (pendingData == null)
            {
                return false;
            }

            for (int rowIndex = 0; rowIndex < pendingData.Rows.Count; rowIndex++)
            {
                string result = pendingData.Rows[rowIndex]["RESULT"]?.ToString();
                if (string.IsNullOrWhiteSpace(result))
                {
                    MessageBox.Show("Please select OK or NG for every pending row.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
