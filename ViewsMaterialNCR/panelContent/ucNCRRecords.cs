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
    public partial class ucNCRRecords : UserControl
    {
        // สร้างด้วยโค้ดด้วยเหตุผลเดียวกับใน ucDashboard
        // ถ้าปล่อยไว้ใน .Designer.cs จะโดน designer ลบทิ้งตอนเปิดหน้านี้ใน design view
        private readonly ucNcrTable table = new ucNcrTable { Dock = DockStyle.Fill };

        public ucNCRRecords()
        {
            InitializeComponent();

            Controls.Add(table);

            table.ViewClicked += (s, e) => ShowDetail(e.Row);
            table.EditClicked += (s, e) => EditRecord(e.Row);
            table.DeleteClicked += (s, e) => DeleteRecord(e.Row);
            table.ExportClicked += (s, e) => ExportRecord(e.Row);

            // TODO: เปลี่ยนเป็น query จริงเมื่อมีตาราง NCR ใน DB
            table.Bind(NcrSampleData.Create());
        }

        // TODO: ยังไม่ได้สร้าง form รายละเอียด NCR
        private void ShowDetail(DataRow row)
        {
        }

        // TODO: รอ data layer ของ NCR
        private void EditRecord(DataRow row)
        {
        }

        // TODO: รอ data layer ของ NCR
        private void DeleteRecord(DataRow row)
        {
        }

        // TODO: รอ data layer ของ NCR
        private void ExportRecord(DataRow row)
        {
        }
    }
}
