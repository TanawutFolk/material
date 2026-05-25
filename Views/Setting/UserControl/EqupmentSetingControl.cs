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
    public partial class EqupmentSetingControl : UserControl
    {
        private readonly SettingControllers _controller = new SettingControllers();

        private const string ColEquipmentType = "Equipment Type";
        private const string ColEquipmentName = "Equipment Name";

        private static readonly Color HeaderBackColor = Color.ForestGreen;
        private static readonly Color HeaderForeColor = Color.White;
        private static readonly Color SelectionBackColor = Color.Pink;
        private static readonly Color AlternateRowBackColor = Color.FromArgb(245, 250, 245);

        private bool _gridConfigured;
        private bool _isLoadingData;

        public EqupmentSetingControl()
        {
            InitializeComponent();

            Load += EqupmentSetingControl_Load;
            btnSearch.Click += btnSearch_Click;
            btnClear.Click += btnClear_Click;
            btnAddNewEquipment.Click += btnAddNewEquipment_Click;
        }

        private void EqupmentSetingControl_Load(object sender, EventArgs e)
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

            string searchEquipmentName = txtMCodeSearch.Text.Trim();

            Task.Run(() => FetchData(searchEquipmentName))
                .ContinueWith(task =>
                {
                    if (IsDisposed || Disposing) return;

                    try
                    {
                        if (task.IsFaulted)
                        {
                            MessageBox.Show(
                                task.Exception?.GetBaseException().Message ?? "Load equipment setting failed.",
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

        private DataTable FetchData(string searchEquipmentName)
        {
            var filter = new SettingProperty
            {
                Search_Equipment_Name = searchEquipmentName
            };

            return _controller.SearchEquipmentTypeSettingList(filter);
        }

        private void BindGrid(DataTable dt)
        {
            var grid = dtgEmployeeSetting;
            grid.SuspendLayout();
            try
            {
                grid.DataSource = dt;
                ApplyColumnFormat();
            }
            finally
            {
                grid.ResumeLayout();
            }
        }

        private void ApplyColumnFormat()
        {
            SetColumnFill(ColEquipmentType, 25);
            SetColumnFill(ColEquipmentName, 75);
            SetColumnAlignment(ColEquipmentType, DataGridViewContentAlignment.MiddleCenter);
            SetColumnAlignment(ColEquipmentName, DataGridViewContentAlignment.MiddleCenter);
        }

        private DataGridViewColumn FindColumn(string name) =>
            dtgEmployeeSetting.Columns.Contains(name)
                ? dtgEmployeeSetting.Columns[name]
                : null;

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
            txtMCodeSearch.Text = "";
            LoadData();
        }

        private void btnAddNewEquipment_Click(object sender, EventArgs e)
        {
            using (var frm = new frmAddEquipment())
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
        }
    }
}
