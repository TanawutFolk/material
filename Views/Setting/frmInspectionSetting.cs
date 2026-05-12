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
            LoadInspectionSettingList();
        }
        private void LoadInspectionSettingList()
        {
            SettingProperty dataItem = new SettingProperty();
            dataItem.Search_M_CODE = txtMCodeSearch.Text.Trim();

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

            dtgInspectionSetting.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            // Header Style
            dtgInspectionSetting.EnableHeadersVisualStyles = false;
            dtgInspectionSetting.ColumnHeadersDefaultCellStyle.BackColor = Color.Red;
            dtgInspectionSetting.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dtgInspectionSetting.ColumnHeadersDefaultCellStyle.Font = new Font(dtgInspectionSetting.Font, FontStyle.Bold);
            dtgInspectionSetting.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtgInspectionSetting.ColumnHeadersHeight = 35;
            dtgInspectionSetting.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            // Row Style
            dtgInspectionSetting.DefaultCellStyle.ForeColor = Color.Black;
            dtgInspectionSetting.DefaultCellStyle.BackColor = Color.White;
            dtgInspectionSetting.DefaultCellStyle.SelectionBackColor = Color.LightCoral;
            dtgInspectionSetting.DefaultCellStyle.SelectionForeColor = Color.Black;

            // Grid Style
            dtgInspectionSetting.BackgroundColor = Color.White;
            dtgInspectionSetting.GridColor = Color.LightGray;
            dtgInspectionSetting.BorderStyle = BorderStyle.None;

            AddEditButtonColumn();

            // Column Width
            SetColumnWidth("Edit", 70);

            SetColumnWidth("M Code", 90);
            SetColumnWidth("Material Name", 200);
            SetColumnWidth("Vendor", 254);

            SetColumnWidth("Keep Data", 60);
            SetColumnWidth("Packing Check", 70);
            SetColumnWidth("Regular Check", 75);
            SetColumnWidth("Regular Ref", 60);
            SetColumnWidth("Function Check", 75);
            SetColumnWidth("Dimension Check", 75);
            SetColumnWidth("Appearance Check", 75);
            SetColumnWidth("Status", 60);

            SetColumnCenter("Edit");
            SetColumnCenter("Keep Data");
            SetColumnCenter("Packing Check");
            SetColumnCenter("Regular Check");
            SetColumnCenter("Regular Ref");
            SetColumnCenter("Function Check");
            SetColumnCenter("Dimension Check");
            SetColumnCenter("Appearance Check");
            SetColumnCenter("Status");
        }

        private void AddEditButtonColumn()
        {
            if (dtgInspectionSetting.Columns.Contains("Edit"))
            {
                return;
            }

            DataGridViewButtonColumn btnEdit = new DataGridViewButtonColumn();
            btnEdit.Name = "Edit";
            btnEdit.HeaderText = "";
            btnEdit.Text = "Edit";
            btnEdit.UseColumnTextForButtonValue = true;
            btnEdit.Width = 70;

            dtgInspectionSetting.Columns.Insert(0, btnEdit);
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

            if (dtgInspectionSetting.Columns[e.ColumnIndex].Name != "Edit")
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
