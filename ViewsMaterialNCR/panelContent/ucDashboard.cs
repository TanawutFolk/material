using RawMat.ViewsMaterialNCR.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.ViewsMaterialNCR.panelContent
{
    public partial class ucDashboard : UserControl
    {
        // สร้างด้วยโค้ด ไม่ใส่ไว้ใน .Designer.cs
        //
        // WinForms designer สร้าง user control ที่ซ้อนหลายชั้นแบบนี้ไม่สำเร็จ
        // (ucDashboard -> ucNcrTable -> RoundedPanel / StyledDataGrid / ucPager)
        // แล้วทุกครั้งที่ save หน้านี้ มันจะเขียน InitializeComponent ใหม่ทั้งก้อน
        // โดยตัดตัวที่สร้างไม่สำเร็จทิ้ง ตัวแปรเลยกลายเป็น null ตอนรัน
        //
        // พอย้ายมาสร้างตรงนี้ designer แตะไม่ได้ ส่วน pn_dtg ยังเป็น Panel ธรรมดา
        // ที่ย้าย/ปรับขนาดใน designer ได้ตามปกติ
        private readonly ucNcrTable dtgDataNCR = new ucNcrTable { Dock = DockStyle.Fill };

        public ucDashboard()
        {
            InitializeComponent();

            pn_dtg.Controls.Add(dtgDataNCR);

            // TODO: เปลี่ยนเป็น query จริงเมื่อมีตาราง NCR ใน DB
            // TODO: ยังไม่ได้ต่อ event ปุ่ม action หน้านี้ รอ form รายละเอียดกับ data layer
            dtgDataNCR.Bind(NcrSampleData.Create());
        }
    }
}
