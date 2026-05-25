using RawMat.Controllers;
using RawMat.Property;
using RawMat.Views.Setting.form;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Views.Setting
{
    public partial class EmployeeSettingControl : UserControl
    {
        private readonly SettingControllers _controller = new SettingControllers();

        private const string ColEdit = "Edit";
        private const string ColEmployeeId = "Employee ID";
        private const string ColEmployeeLevelId = "Employee Level ID";
        private const string ColEmployeeLevelName = "Employee Level Name";
        private const string ColPhoneExt = "Phone Ext";

        private static readonly Color HeaderBackColor = Color.ForestGreen;
        private static readonly Color HeaderForeColor = Color.White;
        private static readonly Color SelectionBackColor = Color.Pink;
        private static readonly Color AlternateRowBackColor = Color.FromArgb(245, 250, 245);

        private bool _gridConfigured;
        private bool _isLoadingData;

        public EmployeeSettingControl()
        {
            InitializeComponent();

            Load += EmployeeSettingControl_Load;
            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            btnAddNewEmployee.Click += btnAddNewEmployee_Click;
            dtgEmployeeSetting.CellContentClick += dtgEmployeeSetting_CellContentClick;
        }

        private void EmployeeSettingControl_Load(object sender, EventArgs e)
        {
            ConfigureGrid();
            LoadData();
        }

        private void ConfigureGrid()
        {
            if (_gridConfigured) return;

            var grid = dtgEmployeeSetting;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.AutoGenerateColumns = true;

            grid.EnableHeadersVisualStyles = false;
            grid.ColumnHeadersDefaultCellStyle.BackColor = HeaderBackColor;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = HeaderForeColor;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid.ColumnHeadersHeight = 35;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grid.DefaultCellStyle.ForeColor = Color.Black;
            grid.DefaultCellStyle.BackColor = Color.White;
            grid.DefaultCellStyle.SelectionBackColor = SelectionBackColor;
            grid.DefaultCellStyle.SelectionForeColor = Color.Black;
            grid.AlternatingRowsDefaultCellStyle.BackColor = AlternateRowBackColor;

            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.GridColor = Color.DarkGray;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            _gridConfigured = true;
        }

        private void LoadData()
        {
            if (_isLoadingData) return;

            _isLoadingData = true;
            btnSearch.Enabled = false;
            Cursor = Cursors.WaitCursor;

            string searchEmployeeId = txtMCodeSearch.Text.Trim();

            Task.Run(() => FetchData(searchEmployeeId))
                .ContinueWith(task =>
                {
                    if (IsDisposed || Disposing) return;

                    try
                    {
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(
                                task.Exception?.GetBaseException().Message ?? "Load employee setting failed.",
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        BindGrid(task.Result);
                    }
                    finally
                    {
                        _isLoadingData = false;
                        btnSearch.Enabled = true;
                        Cursor = Cursors.Default;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private DataTable FetchData(string searchEmployeeId)
        {
            var filter = new SettingProperty
            {
                Search_Employee_ID = searchEmployeeId
            };

            return _controller.SearchEmployeeSettingList(filter);
        }

        private void BindGrid(DataTable dt)
        {
            var grid = dtgEmployeeSetting;
            grid.SuspendLayout();
            try
            {
                grid.DataSource = dt;
                EnsureEditButtonColumn();
                ApplyColumnFormat();
            }
            finally
            {
                grid.ResumeLayout();
            }
        }

        private void EnsureEditButtonColumn()
        {
            if (dtgEmployeeSetting.Columns.Contains(ColEdit))
            {
                dtgEmployeeSetting.Columns[ColEdit].DisplayIndex = 0;
                return;
            }

            var btn = new DataGridViewButtonColumn
            {
                Name = ColEdit,
                HeaderText = "",
                Text = "Edit",
                UseColumnTextForButtonValue = true,
                Width = 80
            };

            dtgEmployeeSetting.Columns.Insert(0, btn);
        }

        private void ApplyColumnFormat()
        {
            SetColumnWidth(ColEdit, 80);
            SetColumnVisible(ColEmployeeLevelId, false);

            SetColumnFill(ColEmployeeId, 30);
            SetColumnFill(ColEmployeeLevelName, 50);
            SetColumnFill(ColPhoneExt, 20);

            SetColumnAlignment(ColEdit, DataGridViewContentAlignment.MiddleCenter);
            SetColumnAlignment(ColEmployeeLevelName, DataGridViewContentAlignment.MiddleCenter);
            SetColumnAlignment(ColPhoneExt, DataGridViewContentAlignment.MiddleCenter);
        }

        private DataGridViewColumn FindColumn(string name) =>
            dtgEmployeeSetting.Columns.Contains(name)
                ? dtgEmployeeSetting.Columns[name]
                : null;

        private void SetColumnWidth(string name, int width)
        {
            DataGridViewColumn col = FindColumn(name);
            if (col != null)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                col.Width = width;
            }
        }

        private void SetColumnFill(string name, float fillWeight)
        {
            DataGridViewColumn col = FindColumn(name);
            if (col != null)
            {
                col.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                col.FillWeight = fillWeight;
            }
        }

        private void SetColumnVisible(string name, bool visible)
        {
            DataGridViewColumn col = FindColumn(name);
            if (col != null) col.Visible = visible;
        }

        private void SetColumnAlignment(string name, DataGridViewContentAlignment alignment)
        {
            DataGridViewColumn col = FindColumn(name);
            if (col != null) col.DefaultCellStyle.Alignment = alignment;
        }

        private void btnSearch_Click(object sender, EventArgs e) => LoadData();

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMCodeSearch.Text = "";
            LoadData();
        }

        private void btnAddNewEmployee_Click(object sender, EventArgs e)
        {
            using (var frm = new frmEditEmployee())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
        }

        private void dtgEmployeeSetting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dtgEmployeeSetting.Columns[e.ColumnIndex].Name != ColEdit)
                return;

            string employeeId = GetEmployeeIdFromRow(e.RowIndex);
            if (string.IsNullOrWhiteSpace(employeeId))
                return;

            using (var frm = new frmEditEmployee(employeeId))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
        }

        private string GetEmployeeIdFromRow(int rowIndex)
        {
            if (!dtgEmployeeSetting.Columns.Contains(ColEmployeeId))
                return string.Empty;

            object value = dtgEmployeeSetting.Rows[rowIndex].Cells[ColEmployeeId].Value;
            return value == null || value == DBNull.Value
                ? string.Empty
                : Convert.ToString(value).Trim();
        }
    }
}
