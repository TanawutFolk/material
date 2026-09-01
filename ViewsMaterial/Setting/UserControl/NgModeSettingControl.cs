using RawMat.Controllers;
using RawMat.Property;
using RawMat.ViewsMaterial.Setting.form;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.ViewsMaterial.Setting.UserControl
{
    public partial class NgModeSettingControl : System.Windows.Forms.UserControl
    {
        private readonly SettingControllers _controller = new SettingControllers();

        private const string ColAction = "Action";
        private const string ColId = "ID";
        private const string ColNgMode = "NG Mode";
        private const string ColStatus = "Status";
        private const string ColCreateDate = "Create Date";

        private static readonly Color HeaderBackColor = Color.ForestGreen;
        private static readonly Color HeaderForeColor = Color.White;
        private static readonly Color SelectionBackColor = Color.Pink;
        private static readonly Color AlternateRowBackColor = Color.FromArgb(245, 250, 245);

        private bool _gridConfigured;
        private bool _isLoadingData;

        public NgModeSettingControl()
        {
            InitializeComponent();

            Load += NgModeSettingControl_Load;
            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            btnAddNewNgMode.Click += btnAddNewNgMode_Click;
            dtgEmployeeSetting.CellContentClick += dtgEmployeeSetting_CellContentClick;
            txtNgModeSearch.Text = "";
        }

        private void NgModeSettingControl_Load(object sender, EventArgs e)
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

            string searchNgMode = txtNgModeSearch.Text.Trim();

            Task.Run(() => FetchData(searchNgMode))
                .ContinueWith(task =>
                {
                    if (IsDisposed || Disposing) return;

                    try
                    {
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(
                                task.Exception?.GetBaseException().Message ?? "Load NG Mode setting failed.",
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

        private DataTable FetchData(string searchNgMode)
        {
            var filter = new SettingProperty
            {
                Search_NG_Mode = searchNgMode
            };

            return _controller.SearchNgModeSettingList(filter);
        }

        private void BindGrid(DataTable dt)
        {
            var grid = dtgEmployeeSetting;
            grid.SuspendLayout();
            try
            {
                grid.DataSource = dt;
                EnsureActionButtonColumn();
                ApplyColumnFormat();
            }
            finally
            {
                grid.ResumeLayout();
            }
        }

        private void EnsureActionButtonColumn()
        {
            if (dtgEmployeeSetting.Columns.Contains(ColAction))
            {
                dtgEmployeeSetting.Columns[ColAction].DisplayIndex = 0;
                return;
            }

            var btn = new DataGridViewButtonColumn
            {
                Name = ColAction,
                HeaderText = "",
                Text = "Action",
                UseColumnTextForButtonValue = true,
                Width = 80
            };

            dtgEmployeeSetting.Columns.Insert(0, btn);
        }

        private void SetColumnVisible(string name, bool visible)
        {
            DataGridViewColumn col = FindColumn(name);
            if (col != null)
                col.Visible = visible;
        }

        private void ApplyColumnFormat()
        {
            // Show Action + NG Mode
            SetColumnVisible(ColAction, true);
            SetColumnVisible(ColNgMode, true);

            // ซ่อนอันอื่น
            SetColumnVisible(ColId, true); // โชว์เพิ่ม
            SetColumnVisible(ColStatus, false);
            SetColumnVisible(ColCreateDate, false);

            SetColumnWidth(ColAction, 80);
            SetColumnWidth(ColId, 70);
            SetColumnFill(ColNgMode, 100);

            DataGridViewColumn idColumn = FindColumn(ColId);
            if (idColumn != null)
                idColumn.HeaderText = "No.";

            SetColumnAlignment(ColAction, DataGridViewContentAlignment.MiddleCenter);
            SetColumnAlignment(ColId, DataGridViewContentAlignment.MiddleCenter);
            SetColumnAlignment(ColNgMode, DataGridViewContentAlignment.MiddleCenter);
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

        private void SetColumnAlignment(string name, DataGridViewContentAlignment alignment)
        {
            DataGridViewColumn col = FindColumn(name);
            if (col != null) col.DefaultCellStyle.Alignment = alignment;
        }

        private void btnSearch_Click(object sender, EventArgs e) => LoadData();

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtNgModeSearch.Text = "";
            LoadData();
        }

        private void btnAddNewNgMode_Click(object sender, EventArgs e)
        {
            using (var frm = new frmAddNewNgMode())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
        }

        private void dtgEmployeeSetting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dtgEmployeeSetting.Columns[e.ColumnIndex].Name != ColAction)
                return;

            string id = GetCellText(e.RowIndex, ColId);
            string ngMode = GetCellText(e.RowIndex, ColNgMode);
            string status = GetCellText(e.RowIndex, ColStatus);

            if (string.IsNullOrWhiteSpace(id) || status.Equals("InActive", StringComparison.OrdinalIgnoreCase))
                return;

            SettingGridActionMenu.Show(
                dtgEmployeeSetting,
                e.ColumnIndex,
                e.RowIndex,
                () => EditNgMode(id, ngMode),
                () => DeleteNgMode(id, ngMode));
        }

        private void EditNgMode(string id, string ngMode)
        {
            using (var frm = new frmAddNewNgMode(id, ngMode))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
        }

        private void DeleteNgMode(string id, string ngMode)
        {
            using (var frm = new frmConfirm($"Delete NG Mode '{ngMode}' ?"))
            {
                if (frm.ShowDialog(this) != DialogResult.Yes)
                    return;
            }

            var dataItem = new SettingProperty
            {
                NG_Mode_ID = id
            };

            if (!_controller.DeleteNgModeSetting(dataItem))
            {
                MessageBox.Show("Delete NG Mode Setting Failed", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Delete NG Mode Setting", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
        }

        private string GetCellText(int rowIndex, string columnName)
        {
            if (!dtgEmployeeSetting.Columns.Contains(columnName))
                return string.Empty;

            object value = dtgEmployeeSetting.Rows[rowIndex].Cells[columnName].Value;
            return value == null || value == DBNull.Value
                ? string.Empty
                : Convert.ToString(value).Trim();
        }
    }
}
