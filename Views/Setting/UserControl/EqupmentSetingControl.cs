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
    public partial class EqupmentSetingControl : System.Windows.Forms.UserControl
    {
        private readonly SettingControllers _controller = new SettingControllers();

        private const string ColAction = "Action";
        private const string ColSerialId = "Serial ID";
        private const string ColEquipmentType = "Equipment Type";
        private const string ColEquipmentName = "Equipment Name";
        private const string ColEquipmentSerial = "Equipment Serial";

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
            dtgEmployeeSetting.CellContentClick += dtgEmployeeSetting_CellContentClick;
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

        private void ApplyColumnFormat()
        {
            SetColumnWidth(ColAction, 80);
            SetColumnVisible(ColSerialId, false);
            SetColumnVisible(ColEquipmentType, false);
            SetColumnFill(ColEquipmentName, 55);
            SetColumnFill(ColEquipmentSerial, 45);
            SetColumnAlignment(ColAction, DataGridViewContentAlignment.MiddleCenter);
            SetColumnAlignment(ColEquipmentName, DataGridViewContentAlignment.MiddleCenter);
            SetColumnAlignment(ColEquipmentSerial, DataGridViewContentAlignment.MiddleCenter);
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

        private void btnAddNewEquipment_Click(object sender, EventArgs e)
        {
            using (var frm = new frmAddEquipment())
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

            string equipmentType = GetEquipmentTypeFromRow(e.RowIndex);
            string equipmentName = GetEquipmentNameFromRow(e.RowIndex);
            string equipmentSerialId = GetEquipmentSerialIdFromRow(e.RowIndex);

            if (string.IsNullOrWhiteSpace(equipmentType))
                return;

            string equipmentSerial = GetCellText(e.RowIndex, ColEquipmentSerial);
            SettingGridActionMenu.Show(
                dtgEmployeeSetting,
                e.ColumnIndex,
                e.RowIndex,
                () => EditEquipment(equipmentType, equipmentName, equipmentSerialId, equipmentSerial),
                () => DeleteEquipment(equipmentType, equipmentName, equipmentSerialId));
        }

        private void EditEquipment(string equipmentType, string equipmentName, string equipmentSerialId, string equipmentSerial)
        {
            using (var frm = new frmAddEquipment(equipmentType, equipmentName, equipmentSerialId, equipmentSerial))
            {
                if (frm.ShowDialog(this) == DialogResult.OK)
                    LoadData();
            }
        }

        private void DeleteEquipment(string equipmentType, string equipmentName, string equipmentSerialId)
        {
            using (var frm = new frmConfirm("Are you sure you want to delete ?"))
            {
                if (frm.ShowDialog(this) != DialogResult.Yes)
                    return;
            }

            var dataItem = new SettingProperty
            {
                Equipment_Type = equipmentType,
                Equipment_Name = equipmentName,
                Equipment_Serial_ID = equipmentSerialId
            };

            if (!_controller.DeleteEquipmentTypeSetting(dataItem))
            {
                MessageBox.Show("Delete Equipment Setting Failed", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Delete Equipment Setting", "Success",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadData();
        }

        private string GetEquipmentTypeFromRow(int rowIndex) => GetCellText(rowIndex, ColEquipmentType);

        private string GetEquipmentNameFromRow(int rowIndex) => GetCellText(rowIndex, ColEquipmentName);

        private string GetEquipmentSerialIdFromRow(int rowIndex) => GetCellText(rowIndex, ColSerialId);

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
