using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace RawMat.Views.Setting
{
    public partial class frmSetting : Form
    {
        private readonly SettingControllers _controller = new SettingControllers();

        // ── Column name constants ─────────────────────────────────────────────
        private const string ColRevise = "Revise";
        private const string ColMCode = "M Code";
        private const string ColKeepData = "Keep Data";
        private const string ColPackingCheck = "Packing Check";
        private const string ColRegularCheck = "Regular Check";
        private const string ColRegularRef = "Regular Ref";
        private const string ColFunctionCheck = "Function Check";
        private const string ColDimensionCheck = "Dimension Check";
        private const string ColAppearanceCheck = "Appearance Check";
        private const string ColStatus = "Status";

        // ── Status values ─────────────────────────────────────────────────────
        private const string StatusActive = "1";
        private const string StatusInactive = "0";

        // ── Style constants ───────────────────────────────────────────────────
        private static readonly Color HeaderBackColor = Color.ForestGreen;
        private static readonly Color HeaderForeColor = Color.White;
        private static readonly Color SelectionBackColor = Color.Pink;
        private static readonly Color AlternateRowBackColor = Color.FromArgb(245, 250, 245);
        private static readonly Color ActiveBackColor = Color.FromArgb(210, 245, 210);
        private static readonly Color ActiveForeColor = Color.DarkGreen;
        private static readonly Color InactiveBackColor = Color.Orange;
        private static readonly Color InactiveForeColor = Color.FromArgb(255, 230, 200);

        private static readonly Dictionary<string, int> ColumnWidths = new Dictionary<string, int>
        {
            { ColRevise,          80  },
            { ColMCode,           110 },
            { ColKeepData,        110 },
            { ColPackingCheck,    125 },
            { ColRegularCheck,    125 },
            { ColRegularRef,      110 },
            { ColFunctionCheck,   125 },
            { ColDimensionCheck,  130 },
            { ColAppearanceCheck, 140 },
            { ColStatus,          80  }
        };

        private static readonly string[] CenterColumns =
        {
            ColRevise, ColKeepData, ColPackingCheck, ColRegularCheck,
            ColRegularRef, ColFunctionCheck, ColDimensionCheck,
            ColAppearanceCheck, ColStatus
        };

        private int _statusColumnIndex = -1;

        // ─────────────────────────────────────────────────────────────────────
        public frmSetting()
        {
            InitializeComponent();
        }

        // ── Lifecycle ─────────────────────────────────────────────────────────
        private void frmInspectionSetting_Load(object sender, EventArgs e)
        {
            BindStatusCombo();
            ConfigureGrid();           // renamed: shorter, unambiguous
            LoadData();                // renamed: short verb-noun
        }

        // ── Grid configuration (run ONCE on load) ─────────────────────────────
        private void ConfigureGrid()
        {
            var grid = dtgInspectionSetting;

            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.RowHeadersVisible = false;
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
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
            grid.DefaultCellStyle.Font = grid.Font;

            grid.AlternatingRowsDefaultCellStyle.BackColor = AlternateRowBackColor;

            grid.BackgroundColor = Color.White;
            grid.BorderStyle = BorderStyle.FixedSingle;
            grid.GridColor = Color.DarkGray;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            grid.AdvancedCellBorderStyle.Left = DataGridViewAdvancedCellBorderStyle.Single;
            grid.AdvancedCellBorderStyle.Right = DataGridViewAdvancedCellBorderStyle.Single;
            grid.AdvancedCellBorderStyle.Top = DataGridViewAdvancedCellBorderStyle.Single;
            grid.AdvancedCellBorderStyle.Bottom = DataGridViewAdvancedCellBorderStyle.Single;

            // Subscribe ONCE here — not inside LoadData
            grid.CellFormatting += OnCellFormatting;
        }

        // ── Data loading ──────────────────────────────────────────────────────
        private void LoadData()
        {
            DataTable dt = FetchData();
            BindGrid(dt);
        }

        private DataTable FetchData()
        {
            var filter = new SettingProperty
            {
                Search_M_CODE = txtMCodeSearch.Text.Trim(),
                Search_Status = Convert.ToString(cboStatus.SelectedValue)
            };
            return _controller.SearchInspectionSettingList(filter);
        }

        private void BindGrid(DataTable dt)
        {
            var grid = dtgInspectionSetting;
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

        // ── Column formatting ─────────────────────────────────────────────────
        private void ApplyColumnFormat()
        {
            if (dtgInspectionSetting.Columns.Count == 0)
            {
                _statusColumnIndex = -1;
                return;
            }

            foreach (var pair in ColumnWidths)
                SetColumnWidth(pair.Key, pair.Value);

            foreach (string name in CenterColumns)
                SetColumnAlignment(name, DataGridViewContentAlignment.MiddleCenter);

            DataGridViewColumn statusCol = FindColumn(ColStatus);
            _statusColumnIndex = statusCol?.Index ?? -1;   // null-conditional + null-coalescing
        }

        private void EnsureEditButtonColumn()
        {
            if (dtgInspectionSetting.Columns.Contains(ColRevise))
            {
                dtgInspectionSetting.Columns[ColRevise].DisplayIndex = 0;
                return;
            }

            var btn = new DataGridViewButtonColumn
            {
                Name = ColRevise,
                HeaderText = "",
                Text = "Revise",
                UseColumnTextForButtonValue = true,
                Width = 80
            };
            dtgInspectionSetting.Columns.Insert(0, btn);
        }

        // ── Cell formatting ───────────────────────────────────────────────────
        private void OnCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != _statusColumnIndex)
                return;

            string status = Convert.ToString(e.Value).Trim();
            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            bool isActive = status == StatusActive
                           || status.Equals("Active", StringComparison.OrdinalIgnoreCase);
            bool isInactive = status == StatusInactive
                           || status.Equals("InActive", StringComparison.OrdinalIgnoreCase);

            if (isActive)
            {
                e.CellStyle.BackColor = ActiveBackColor;
                e.CellStyle.ForeColor = ActiveForeColor;
            }
            else if (isInactive)
            {
                e.CellStyle.BackColor = InactiveBackColor;
                e.CellStyle.ForeColor = InactiveForeColor;
            }
            else
            {
                e.CellStyle.BackColor = dtgInspectionSetting.DefaultCellStyle.BackColor;
                e.CellStyle.ForeColor = dtgInspectionSetting.DefaultCellStyle.ForeColor;
            }
        }

        // ── Combo setup ───────────────────────────────────────────────────────
        private void BindStatusCombo()
        {
            var dt = new DataTable();
            dt.Columns.Add("TEXT");
            dt.Columns.Add("VALUE");
            dt.Rows.Add("All", "");
            dt.Rows.Add("Active", StatusActive);
            dt.Rows.Add("InActive", StatusInactive);

            cboStatus.DataSource = dt;
            cboStatus.DisplayMember = "TEXT";
            cboStatus.ValueMember = "VALUE";
            cboStatus.SelectedValue = StatusActive;
        }

        // ── Grid helpers ──────────────────────────────────────────────────────
        private DataGridViewColumn FindColumn(string name) =>
            dtgInspectionSetting.Columns.Contains(name)
                ? dtgInspectionSetting.Columns[name]
                : null;

        private void SetColumnWidth(string name, int width)
        {
            DataGridViewColumn col = FindColumn(name);
            if (col != null) col.Width = width;
        }

        private void SetColumnAlignment(string name, DataGridViewContentAlignment alignment)
        {
            DataGridViewColumn col = FindColumn(name);
            if (col != null) col.DefaultCellStyle.Alignment = alignment;
        }

        // ── UI event handlers ─────────────────────────────────────────────────
        private void btnSearch_Click(object sender, EventArgs e) => LoadData();

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMCodeSearch.Text = "";
            cboStatus.SelectedValue = "";
            LoadData();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            using (var frm = new frmMCodeInspectionSetting())
                frm.ShowDialog();

            LoadData();
        }

        private void dtgInspectionSetting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            if (dtgInspectionSetting.Columns[e.ColumnIndex].Name != ColRevise)
                return;

            string mCode = GetMCodeFromRow(e.RowIndex);
            if (string.IsNullOrEmpty(mCode))
                return;

            using (var frm = new frmMCodeInspectionSetting(mCode))
                frm.ShowDialog();

            LoadData();
        }

        private string GetMCodeFromRow(int rowIndex)
        {
            object value = dtgInspectionSetting.Rows[rowIndex].Cells[ColMCode].Value;
            return (value == null || value == DBNull.Value)
                ? string.Empty
                : Convert.ToString(value).Trim();
        }
    }
}