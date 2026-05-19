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

namespace RawMat.Views.Setting
{
    public partial class frmInspectionSetting : Form
    {
        SettingControllers _controller = new SettingControllers();
        public frmInspectionSetting()
        {
            InitializeComponent();
        }

        private void frmInspectionSetting_Load(object sender, EventArgs e)
        {
            BindStatusCombo();
            LoadInspectionSettingList();
        }
        private void LoadInspectionSettingList()
        {
            SettingProperty dataItem = new SettingProperty();
            dataItem.Search_M_CODE = txtMCodeSearch.Text.Trim();
            dataItem.Search_Status = Convert.ToString(cboStatus.SelectedValue);


            DataTable dt = _controller.SearchInspectionSettingList(dataItem);
            dtgInspectionSetting.DataSource = dt;

            FormatInspectionSettingGrid();
        }
        private void FormatInspectionSettingGrid()
        {
            if (dtgInspectionSetting.Columns.Count == 0)
            {
                return;
            }

            dtgInspectionSetting.ReadOnly = true;
            dtgInspectionSetting.AllowUserToAddRows = false;
            dtgInspectionSetting.AllowUserToDeleteRows = false;
            dtgInspectionSetting.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dtgInspectionSetting.MultiSelect = false;
            dtgInspectionSetting.RowHeadersVisible = false;

            dtgInspectionSetting.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            dtgInspectionSetting.AllowUserToResizeRows = false;

            // Header Style
            dtgInspectionSetting.EnableHeadersVisualStyles = false;
            dtgInspectionSetting.ColumnHeadersDefaultCellStyle.BackColor = Color.ForestGreen;
            dtgInspectionSetting.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dtgInspectionSetting.ColumnHeadersDefaultCellStyle.Font =
                                                 new Font("Segoe UI", 9, FontStyle.Bold);
            dtgInspectionSetting.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtgInspectionSetting.ColumnHeadersHeight = 35;
            dtgInspectionSetting.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Row Style
            dtgInspectionSetting.DefaultCellStyle.ForeColor = Color.Black;
            dtgInspectionSetting.DefaultCellStyle.BackColor = Color.White;
            dtgInspectionSetting.DefaultCellStyle.SelectionBackColor = Color.FromArgb(210, 240, 210);
            dtgInspectionSetting.DefaultCellStyle.SelectionForeColor = Color.Black;
            dtgInspectionSetting.DefaultCellStyle.Font = dtgInspectionSetting.Font;

            dtgInspectionSetting.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(245, 250, 245);

            // Grid Style
            dtgInspectionSetting.BackgroundColor = Color.White;
            dtgInspectionSetting.GridColor = Color.LightGray;
            dtgInspectionSetting.BorderStyle = BorderStyle.None;
            dtgInspectionSetting.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dtgInspectionSetting.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            AddEditButtonColumn();

            // Column Width
            SetColumnWidth("Revise", 80);
            SetColumnWidth("M Code", 110);
            SetColumnWidth("Keep Data", 110);
            SetColumnWidth("Packing Check", 110);
            SetColumnWidth("Regular Check", 110);
            SetColumnWidth("Regular Ref", 110);
            SetColumnWidth("Function Check", 110);
            SetColumnWidth("Dimension Check", 110);
            SetColumnWidth("Appearance Check", 110);
            SetColumnWidth("Status", 80);

            // Center Align
            SetColumnCenter("Revise");
            SetColumnCenter("Keep Data");
            SetColumnCenter("Packing Check");
            SetColumnCenter("Regular Check");
            SetColumnCenter("Regular Ref");
            SetColumnCenter("Function Check");
            SetColumnCenter("Dimension Check");
            SetColumnCenter("Appearance Check");
            SetColumnCenter("Status");

            dtgInspectionSetting.CellFormatting -= dtgInspectionSetting_CellFormatting;
            dtgInspectionSetting.CellFormatting += dtgInspectionSetting_CellFormatting;
        }

        private void dtgInspectionSetting_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (dtgInspectionSetting.Columns[e.ColumnIndex].Name != "Status")
            {
                return;
            }

            string status = Convert.ToString(e.Value).Trim();

            if (status == "1" || status.Equals("Active", StringComparison.OrdinalIgnoreCase))
            {
                e.CellStyle.BackColor = Color.FromArgb(210, 245, 210); // เขียวอ่อน
                e.CellStyle.ForeColor = Color.DarkGreen;
            }
            else if (status == "0" || status.Equals("InActive", StringComparison.OrdinalIgnoreCase))
            {
                e.CellStyle.BackColor = Color.Orange; // ส้มอ่อน
                e.CellStyle.ForeColor = Color.FromArgb(255, 230, 200);
            }

            e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        private void AddEditButtonColumn()
        {
            if (dtgInspectionSetting.Columns.Contains("Revise"))
            {
                return;
            }

            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
            btnEdit.Name = "Revise";
            btnEdit.HeaderText = "";
            btnEdit.Text = "Revise";
            btnEdit.UseColumnTextForButtonValue = true;
            btnEdit.Width = 70;

            dtgInspectionSetting.Columns.Insert(0, btnEdit);
        }

        private void BindStatusCombo()
        {
            DataTable dt = new DataTable();

            dt.Columns.Add("TEXT");
            dt.Columns.Add("VALUE");

            dt.Rows.Add("All", "");
            dt.Rows.Add("Active", "1");
            dt.Rows.Add("InActive", "0");

            cboStatus.DataSource = dt;
            cboStatus.DisplayMember = "TEXT";
            cboStatus.ValueMember = "VALUE";
        }

        private void SetColumnWidth(string columnName, int width)
        {
            if (dtgInspectionSetting.Columns.Contains(columnName))
            {
                dtgInspectionSetting.Columns[columnName].Width = width;
            }
        }

        private void SetColumnCenter(string columnName)
        {
            if (dtgInspectionSetting.Columns.Contains(columnName))
            {
                dtgInspectionSetting.Columns[columnName].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadInspectionSettingList();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtMCodeSearch.Text = "";
            LoadInspectionSettingList();
        }

        private void btnAddNew_Click(object sender, EventArgs e)
        {
            frmMCodeInspectionSetting frm = new frmMCodeInspectionSetting();
            frm.ShowDialog();

            LoadInspectionSettingList();
        }

        private void dtgInspectionSetting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (dtgInspectionSetting.Columns[e.ColumnIndex].Name != "Revise")
            {
                return;
            }

            if (dtgInspectionSetting.Rows[e.RowIndex].Cells["M Code"].Value == null)
            {
                return;
            }

            string mCode = dtgInspectionSetting.Rows[e.RowIndex].Cells["M Code"].Value.ToString();

            frmMCodeInspectionSetting frm = new frmMCodeInspectionSetting(mCode);
            frm.ShowDialog();

            LoadInspectionSettingList();
        }
        //private void dtgInspectionSetting_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0)
        //    {
        //        return;
        //    }

        //    if (dtgInspectionSetting.Rows[e.RowIndex].Cells["M Code"].Value == null)
        //    {
        //        return;
        //    }

        //    string mCode = dtgInspectionSetting.Rows[e.RowIndex].Cells["M Code"].Value.ToString();

        //    frmMCodeInspectionSetting frm = new frmMCodeInspectionSetting(mCode);
        //    frm.ShowDialog();

        //    LoadInspectionSettingList();
        //}

        //private void dtgInspectionSetting_CellContentClick(object sender, DataGridViewCellEventArgs e)
        //{
        //    if (e.RowIndex < 0)
        //    {
        //        return;
        //    }

        //    if (dtgInspectionSetting.Rows[e.RowIndex].Cells["M Code"].Value == null)
        //    {
        //        return;
        //    }

        //    string mCode = dtgInspectionSetting.Rows[e.RowIndex].Cells["M Code"].Value.ToString();

        //    frmMCodeInspectionSetting frm = new frmMCodeInspectionSetting(mCode);
        //    frm.ShowDialog();

        //    LoadInspectionSettingList();
        //}
    }
}
