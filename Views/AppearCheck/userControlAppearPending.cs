using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.CustomMsg;
using System;
using System.Collections.Generic;
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
            dtg_ngMode.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dtg_ngMode.MultiSelect = false;
            dtg_ngMode.DataError += dtg_ngMode_DataError;
            dtg_ngMode.CurrentCellDirtyStateChanged += dtg_ngMode_CurrentCellDirtyStateChanged;
            dtg_ngMode.CellValueChanged += dtg_ngMode_CellValueChanged;

            QTY_NG.DataPropertyName = "QTY_NG";
            QTY_NG.ReadOnly = true;
            QTY_NG.FillWeight = 65;

            NG_MODE.DataPropertyName = "NG_MODE";
            NG_MODE.ReadOnly = true;
            NG_MODE.FillWeight = 150;

            NOTE.DataPropertyName = "NOTE";
            NOTE.ReadOnly = true;
            NOTE.FillWeight = 190;

            JUDGEMENT.DataPropertyName = "JUDGEMENT";
            JUDGEMENT.ReadOnly = true;
            JUDGEMENT.FillWeight = 70;

            RESULT.DataPropertyName = "RESULT";
            RESULT.DataSource = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("", ""),
                new KeyValuePair<string, string>("1", "OK"),
                new KeyValuePair<string, string>("0", "NG")
            };
            RESULT.ValueMember = "Key";
            RESULT.DisplayMember = "Value";
            RESULT.DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton;
            RESULT.FlatStyle = FlatStyle.Flat;
            RESULT.FillWeight = 80;
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
        }

        private DataTable BuildPendingReviewTable(DataTable source)
        {
            DataTable table = new DataTable();
            table.Columns.Add("QTY_NG", typeof(int));
            table.Columns.Add("NG_MODE", typeof(string));
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
            propQA.process = "Appearance_Check";
            propQA.EMP_ID = employee.EMP_CODE;
            propQA.inProcStatus = hasNg
                ? ((int)QAdataProperty.ProcStatus.NG).ToString()
                : ((int)QAdataProperty.ProcStatus.Finished).ToString();
            propQA.reportStatus = propQA.inProcStatus;
            propQA.TOTAL_STATUS = propQA.inProcStatus;

            if (!conQA.UpdateReportStatus(propQA))
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                    "Record Appearance pending status failed.",
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
                CustomMsgBoxBase.ShowCustomMessageBox(
                    "Record Appearance pending as OK completed.",
                    "Success",
                    CustomMsgBoxBase.MessageBoxIconType.OK);
            }

            loadstatus();
            bt_status_appear_pending_Click();
        }

        private bool ValidateResultSelection()
        {
            foreach (DataGridViewRow row in dtg_ngMode.Rows)
            {
                if (row.IsNewRow)
                {
                    continue;
                }

                string result = row.Cells["RESULT"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(result))
                {
                    MessageBox.Show("Please select RESULT for every pending row.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    dtg_ngMode.CurrentCell = row.Cells["RESULT"];
                    dtg_ngMode.BeginEdit(true);
                    return false;
                }
            }

            return true;
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
