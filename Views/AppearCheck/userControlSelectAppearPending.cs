using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Views.AppearCheck
{
    public partial class userControlSelectAppearPending : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();

        public userControlSelectAppearPending()
        {
            InitializeComponent();
            ConfigureReportGrid();
        }

        private void userControlSelectAppearPending_Load(object sender, EventArgs e)
        {
            propQA.process = "Appearance_Check";

            DataTable dt = conQA.SearchForAppearPending();
            dtg_appearPending.DataSource = dt;

            HideColumn("process_status_id");
            HideColumn("Issue_Date");
            ApplyReportGridColumnLayout();
        }

        private void dtg_appearPending_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dtg_appearPending.Rows.Count ||
                e.ColumnIndex < 0 || e.ColumnIndex >= dtg_appearPending.Columns.Count)
            {
                return;
            }

            propQA.process = "Appearance_Check";
            propQA.Report_No = GetCellValue(e.RowIndex, "Report No.");
            propQA.Regular_No = GetCellValue(e.RowIndex, "Regular No");
            propQA.Invoice_No = GetCellValue(e.RowIndex, "Invoice No.");
            propQA.M_CODE = GetCellValue(e.RowIndex, "M-CODE");
            propQA.Material_Name = GetCellValue(e.RowIndex, "Material Name");
            propQA.Qty = GetCellValue(e.RowIndex, "Lot Size");
            propQA.Vendor_Name = GetCellValue(e.RowIndex, "Vendor");
            propQA.inspQty = GetCellValue(e.RowIndex, "Inspection Qty");

            if (!DateTime.TryParse(GetCellValue(e.RowIndex, "Receive Date"), out DateTime receiveDate))
            {
                MessageBox.Show("Receive Date is invalid.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            propQA.dtReceiveDate = receiveDate;

            if (DateTime.TryParse(GetCellValue(e.RowIndex, "Issue_Date"), out DateTime issueDate))
            {
                propQA.dtIssueDate = issueDate;
            }

            if (!LoadAppearanceSampling())
            {
                return;
            }

            userControlAppearPending usrAppearPending = new userControlAppearPending();
            usrAppearPending.Dock = DockStyle.Fill;
            usrAppearPending.propQA = propQA;

            this.Controls.Clear();
            this.Controls.Add(usrAppearPending);
        }

        private bool LoadAppearanceSampling()
        {
            propQA.dtAppSamp = conQA.AppearSampling(propQA);
            if (propQA.dtAppSamp == null || propQA.dtAppSamp.Rows.Count == 0)
            {
                MessageBox.Show("Appearance sampling setting was not found.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DataRow samplingRow = propQA.dtAppSamp.Rows[0];
            propQA.SAMPLING_TYPE = samplingRow["sampling_type"].ToString();
            propQA.SAMPLING_NAME = samplingRow["sampling_type_name"].ToString().Trim();
            propQA.CAVITY_QTY = samplingRow["Cavity_Qty"].ToString();
            propQA.SAMPLING_QTY = samplingRow["Sampling_Qty"].ToString();
            propQA.Allow_Continue = samplingRow["Allow_Continue"].ToString();
            propQA.CAVITY_NAME = samplingRow["Cavity_Name"].ToString();
            propQA.Cavity_Name_List = new List<string>();

            int.TryParse(propQA.CAVITY_QTY?.Trim(), out int cavityQty);
            bool hasCavity = cavityQty > 0;

            if (propQA.SAMPLING_TYPE == "4" && !hasCavity)
            {
                MessageBox.Show("Cavity setting is required for M-CODE: " + propQA.M_CODE, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (propQA.SAMPLING_TYPE == "4" &&
                (string.IsNullOrWhiteSpace(propQA.CAVITY_NAME) || propQA.CAVITY_NAME == "0"))
            {
                MessageBox.Show("Cavity name setting is required for M-CODE: " + propQA.M_CODE, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if ((propQA.SAMPLING_TYPE != "3" && propQA.SAMPLING_TYPE != "1") &&
                (propQA.SAMPLING_QTY == "0" || string.IsNullOrWhiteSpace(propQA.SAMPLING_QTY)))
            {
                MessageBox.Show("Sampling qty setting is required for M-CODE: " + propQA.M_CODE, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (propQA.SAMPLING_TYPE == "2" && hasCavity)
            {
                MessageBox.Show("Cavity setting is not allowed for this sampling type. M-CODE: " + propQA.M_CODE, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            if (propQA.SAMPLING_TYPE == "1" || propQA.SAMPLING_TYPE == "3" || propQA.SAMPLING_TYPE == "5")
            {
                return true;
            }

            MessageBox.Show("This Appearance sampling type is not supported. M-CODE: " + propQA.M_CODE, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private string GetCellValue(int rowIndex, string columnName)
        {
            if (!dtg_appearPending.Columns.Contains(columnName))
            {
                return string.Empty;
            }

            object value = dtg_appearPending.Rows[rowIndex].Cells[columnName].Value;
            return value == null || value == DBNull.Value ? string.Empty : value.ToString();
        }

        private void ConfigureReportGrid()
        {
            dtg_appearPending.AutoGenerateColumns = true;
            dtg_appearPending.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtg_appearPending.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dtg_appearPending.RowHeadersVisible = false;
            dtg_appearPending.AllowUserToResizeRows = false;
            dtg_appearPending.MultiSelect = false;
            dtg_appearPending.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dtg_appearPending.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtg_appearPending.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        private void HideColumn(string columnName)
        {
            if (dtg_appearPending.Columns.Contains(columnName))
            {
                dtg_appearPending.Columns[columnName].Visible = false;
            }
        }

        private void ApplyReportGridColumnLayout()
        {
            foreach (DataGridViewColumn column in dtg_appearPending.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                column.MinimumWidth = 70;
                column.FillWeight = 100;
            }

            SetColumnLayout("Receive Date", 95, 90, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Report No.", 115, 100, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("M-CODE", 110, 95, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Invoice No.", 120, 105, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Lot Size", 80, 75, DataGridViewContentAlignment.MiddleRight);
            SetColumnLayout("Inspection Qty", 95, 90, DataGridViewContentAlignment.MiddleRight);
            SetColumnLayout("Vendor", 145, 130, DataGridViewContentAlignment.MiddleLeft);
            SetColumnLayout("Material Name", 220, 205, DataGridViewContentAlignment.MiddleLeft);
            SetColumnLayout("Status", 90, 80, DataGridViewContentAlignment.MiddleCenter);
        }

        private void SetColumnLayout(string columnName, float fillWeight, int minimumWidth, DataGridViewContentAlignment alignment)
        {
            if (!dtg_appearPending.Columns.Contains(columnName))
            {
                return;
            }

            DataGridViewColumn column = dtg_appearPending.Columns[columnName];
            column.FillWeight = fillWeight;
            column.MinimumWidth = minimumWidth;
            column.DefaultCellStyle.Alignment = alignment;
        }
    }
}
