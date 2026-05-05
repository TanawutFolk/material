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
            dataGridResult.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
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
            // เช็คว่า User ไม่ได้เผลอไปคลิกโดนหัวตาราง (e.RowIndex >= 0)
            // และเช็คว่าคอลัมน์ที่คลิก คือคอลัมน์ "btnEdit" ที่เราเพิ่งสร้างใช่ไหม
            if (e.RowIndex >= 0 && dataGridResult.Columns[e.ColumnIndex].Name == "btnEdit")
            {
                // ดึงค่า M_Code จากบรรทัดที่ถูกคลิกออกมา (เพื่อส่งไปให้ฟอร์ม Edit ค้นหาข้อมูลต่อ)
                string selectedMCode = dataGridResult.Rows[e.RowIndex].Cells["M_Code"].Value.ToString();

                // (สมมติ) เปิดฟอร์มสำหรับ Edit
                // frmEditSetting frmEdit = new frmEditSetting(selectedMCode);
                // frmEdit.ShowDialog();

                // ลองโชว์ข้อความดูก่อนว่าดึงค่า M Code มาได้ถูกต้องไหม
                MessageBox.Show("คุณต้องการแก้ไขข้อมูลของ M Code : " + selectedMCode, "Edit Data");
            }
        }
    }
}
