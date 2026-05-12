using Bunifu.Framework.UI;
using BunifuAnimatorNS;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;           // สำหรับ DataTable
using RawMat.Controllers;    // สำหรับ SettingController

namespace RawMat.Views.Setting
{
    public partial class frmSamplingSetting : Form
    {
        private SettingController _controller = new SettingController();
        public frmSamplingSetting()
        {
            InitializeComponent();
        }







        private void frmSetting_Load(object sender, EventArgs e)
        {
            cboSampling.Items.Clear();
            cboSampling.Items.Add("regular");
            cboSampling.Items.Add("function");
            cboSampling.Items.Add("dimension");
            cboSampling.Items.Add("appearance");
            cboSampling.SelectedIndex = 0;
            btnSearch_Click(sender, e);
        }

        private void btnClear_Click_1(object sender, EventArgs e)
        {
            cboSampling.SelectedIndex = 0;
            txtMcode.Text = "";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string selectedSampling = cboSampling.Text.ToLower();
            string searchMCode = txtMcode.Text.Trim();

            DataTable dt = _controller.SearchSamplingData(selectedSampling, searchMCode);

            dataGridResult.DataSource = null;
            dataGridResult.Columns.Clear();

            dataGridResult.AutoGenerateColumns = true;
            dataGridResult.DataSource = dt;

            dataGridResult.AllowUserToAddRows = false;
            dataGridResult.RowHeadersVisible = false;

            dataGridResult.ColumnHeadersVisible = true;
            dataGridResult.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dataGridResult.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dataGridResult.ColumnHeadersHeight = 40;

            DataGridViewButtonColumn editCol = new DataGridViewButtonColumn();
            editCol.Name = "btnEdit";
            editCol.HeaderText = "Action";
            editCol.Text = "Edit";
            editCol.UseColumnTextForButtonValue = true;
            editCol.FlatStyle = FlatStyle.Flat;

            dataGridResult.Columns.Add(editCol);

            if (dataGridResult.Columns.Contains("M-Code"))
            {
                dataGridResult.Columns["M-Code"].DisplayIndex = 0;
            }

            if (dataGridResult.Columns.Contains("Cavity Qty"))
            {
                dataGridResult.Columns["Cavity Qty"].DisplayIndex = 1;
            }

            if (dataGridResult.Columns.Contains("Cavity Name"))
            {
                dataGridResult.Columns["Cavity Name"].DisplayIndex = 2;
            }

            if (dataGridResult.Columns.Contains("Sampling Type"))
            {
                dataGridResult.Columns["Sampling Type"].DisplayIndex = 3;
            }

            if (dataGridResult.Columns.Contains("Sampling Qty"))
            {
                dataGridResult.Columns["Sampling Qty"].DisplayIndex = 4;
            }

            if (dataGridResult.Columns.Contains("Strictness Type"))
            {
                dataGridResult.Columns["Strictness Type"].DisplayIndex = 5;
            }

            if (dataGridResult.Columns.Contains("Strictness Level"))
            {
                dataGridResult.Columns["Strictness Level"].DisplayIndex = 6;
            }

            dataGridResult.Columns["btnEdit"].DisplayIndex = dataGridResult.Columns.Count - 1;

            dataGridResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void dataGridResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (dataGridResult.Columns[e.ColumnIndex].Name == "btnEdit")
            {
                DataGridViewRow row = dataGridResult.Rows[e.RowIndex];

                string mCode = row.Cells["M-Code"].Value.ToString();
                string cavityQty = row.Cells["Cavity Qty"].Value.ToString();
                string cavityName = row.Cells["Cavity Name"].Value.ToString();
                string samplingType = row.Cells["Sampling Type"].Value.ToString();
                string samplingQty = row.Cells["Sampling Qty"].Value.ToString();
                string strictnessType = row.Cells["Strictness Type"].Value.ToString();
                string strictnessLevel = row.Cells["Strictness Level"].Value.ToString();
                string category = cboSampling.Text;

                frmSamplingEdit frmEdit = new frmSamplingEdit(
                    category,
                    mCode,
                    cavityQty,
                    samplingType,
                    samplingQty,
                    strictnessType,
                    strictnessLevel,
                    cavityName
                );

                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    btnSearch_Click(null, null);
                }
            }
        }
    }
}