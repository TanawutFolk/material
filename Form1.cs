
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat
{
    public partial class Form1 : Form
    {

        private Random random = new Random(); // สร้างออบเจกต์ Random

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void bt_recordData_Click(object sender, EventArgs e)
        {
            
        }

        // ฟังก์ชันสำหรับอัปเดต UI ด้วยข้อมูลสุ่ม
        private void UpdateButtonsWithRandomData()
        {
            // สร้างเลขสุ่มระหว่าง 0 - 100 สำหรับการแสดงผลในปุ่ม
            int randomValue1 = random.Next(0, 10); // สุ่มตัวเลขระหว่าง 0 ถึง 100
            int randomValue2 = random.Next(0, 10); // สุ่มตัวเลขระหว่าง 0 ถึง 100

            // อัปเดตตัวเลขใน BunifuFlatButton
            bunifuFlatButton1.Text = $"Document A {randomValue1} Report";
            bunifuFlatButton2.Text = $"Document B {randomValue2} Report";
        }

    }
}
