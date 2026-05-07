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
    public partial class frmSetting : Form
    {
        private SettingController _controller = new SettingController();
        public frmSetting()
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
            // 1. รับค่าจาก Dropdown (ใช้ .ToLower() เพื่อให้ตัวพิมพ์เล็กตรงกับ "regular", "function" ใน Controller)
            string selectedSampling = cboSampling.Text.ToLower();

            // 2. รับค่าจากช่องกรอก M Code
            string searchMCode = txtMcode.Text.Trim();

            // 3. ส่งไปให้ Controller ไปดึง DataTable จาก Database
            DataTable dt = _controller.SearchSamplingData(selectedSampling, searchMCode);

            // 4. เอาข้อมูลยัดใส่ BunifuCustomDataGrid
            dataGridResult.DataSource = dt;


            dataGridResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;

            dataGridResult.AllowUserToAddRows = false;

            dataGridResult.RowHeadersVisible = false;

            dataGridResult.ColumnHeadersVisible = true; 
            dataGridResult.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;

            dataGridResult.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            dataGridResult.ColumnHeadersHeight = 40;


            if (!dataGridResult.Columns.Contains("btnEdit"))
            {
                DataGridViewButtonColumn editCol = new DataGridViewButtonColumn();
                editCol.Name = "btnEdit";
                editCol.HeaderText = "Action";      
                editCol.Text = "Edit";              
                editCol.UseColumnTextForButtonValue = true;
                editCol.FlatStyle = FlatStyle.Flat;   

                dataGridResult.Columns.Add(editCol);
            }

            dataGridResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // ----------------------------------------
        }

        private void dataGridResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && dataGridResult.Columns[e.ColumnIndex].Name == "btnEdit")
            {
                DataGridViewRow row = dataGridResult.Rows[e.RowIndex];

                string mCode = row.Cells["M-Code"].Value.ToString();
                string cavityQty = row.Cells["Cavity Qty"].Value.ToString();
                string samplingType = row.Cells["Sampling Type"].Value.ToString();
                string samplingQty = row.Cells["Sampling Qty"].Value.ToString();
                string strictnessType = row.Cells["Strictness Type"].Value.ToString();
                string strictnessLevel = row.Cells["Strictness Level"].Value.ToString();
                string cavityName = row.Cells["Cavity Name"].Value.ToString();
                string category = cboSampling.Text;

                frmEdit frmEdit = new frmEdit(category, mCode, cavityQty, samplingType, samplingQty, strictnessType, strictnessLevel, cavityName);

                if (frmEdit.ShowDialog() == DialogResult.OK)
                {
                    btnSearch_Click(null, null);
                }
            }
        }
    }
}
