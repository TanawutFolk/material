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

            // (แถม) จัดให้ความกว้างคอลัมน์พอดีกับพื้นที่ตาราง
            dataGridResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.DisplayedCells;
            // 1. ตัดบรรทัดสุดท้ายออก (ปิดโหมดไม่ให้ User พิมพ์เพิ่มแถวใหม่เอง แถวที่มี * จะหายไป)
            dataGridResult.AllowUserToAddRows = false;

            // 2. เอา Column ว่างๆ ด้านหน้าสุดออก
            dataGridResult.RowHeadersVisible = false;

            // 3. จัดการ HeadColumn (หัวตาราง)
            dataGridResult.ColumnHeadersVisible = true; // เปิดให้โชว์หัวตาราง
                                                        // ตั้งค่าสีตัวหนังสือให้เป็น "สีขาว" จะได้ตัดกับพื้นหลังสีเขียว
            dataGridResult.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            // (แถม) ปรับฟอนต์หัวตารางให้เป็นตัวหนา และปรับขนาดให้ดูง่ายขึ้น
            dataGridResult.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // (แถม) ปรับความสูงของหัวตารางนิดหน่อย จะได้ไม่ดูอึดอัด
            dataGridResult.ColumnHeadersHeight = 40;

            // เช็คก่อนว่ามีคอลัมน์ปุ่ม Edit หรือยัง จะได้ไม่สร้างซ้ำเวลาปุ่ม Search ถูกกดหลายรอบ
            if (!dataGridResult.Columns.Contains("btnEdit"))
            {
                DataGridViewButtonColumn editCol = new DataGridViewButtonColumn();
                editCol.Name = "btnEdit";
                editCol.HeaderText = "Action";        // ชื่อหัวตาราง
                editCol.Text = "Edit";                // ข้อความบนปุ่ม
                editCol.UseColumnTextForButtonValue = true; // บังคับให้ปุ่มโชว์คำว่า Edit ทุกแถว
                editCol.FlatStyle = FlatStyle.Flat;   // ทำให้ปุ่มดูแบนๆ สวยเข้ากับ UI

                // เพิ่มคอลัมน์นี้เข้าไปใน DataGrid (เอาไว้ขวาสุด)
                dataGridResult.Columns.Add(editCol);
            }

            dataGridResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            // ----------------------------------------
        }

        private void dataGridResult_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // เช็คว่า User คลิกที่คอลัมน์ชื่อ "btnEdit" (ปุ่ม Action) และไม่ได้คลิกโดนหัวตาราง (RowIndex >= 0)
            if (e.RowIndex >= 0 && dataGridResult.Columns[e.ColumnIndex].Name == "btnEdit")
            {
                // 1. ชี้ไปที่แถว (Row) ที่ User คลิก
                DataGridViewRow row = dataGridResult.Rows[e.RowIndex];

                // 2. ดึงข้อมูลแต่ละคอลัมน์ออกมา (อ้างอิงชื่อคอลัมน์ตาม AS ใน SQL ที่เราเขียนไว้)
                string mCode = row.Cells["M-Code"].Value.ToString();
                string cavityQty = row.Cells["Cavity Qty"].Value.ToString();
                string samplingType = row.Cells["Sampling Type"].Value.ToString();
                string samplingQty = row.Cells["Sampling Qty"].Value.ToString();
                string strictnessType = row.Cells["Strictness Type"].Value.ToString();
                string strictnessLevel = row.Cells["Strictness Level"].Value.ToString();
                string cavityName = row.Cells["Cavity Name"].Value.ToString();

                // ดึงประเภท Sampling (จาก Dropdown ค้นหาหน้าหลัก) เพื่อจะได้รู้ว่ากำลังแก้ตารางไหน
                string category = cboSampling.Text;

                // 3. เปิดหน้าฟอร์ม Edit พร้อมกับโยนข้อมูลทั้งหมดเข้าไป
                frmEdit frmEdit = new frmEdit(category, mCode, cavityQty, samplingType, samplingQty, strictnessType, strictnessLevel, cavityName);

                // ใช้ ShowDialog() เพื่อบังคับให้แก้หน้าย่อยให้เสร็จก่อน ถึงจะกลับมากดหน้าหลักได้
                frmEdit.ShowDialog();
            }
        }
    }
}
