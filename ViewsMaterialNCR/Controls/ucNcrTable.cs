using System;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace RawMat.ViewsMaterialNCR.Controls
{
    /// <summary>
    /// ตารางรายการ NCR ทั้งใบ — การ์ด + คอลัมน์ + ตัวแบ่งหน้า
    ///
    /// หน้าไหนอยากได้ตารางนี้ก็ลากลงไปวาง แล้วเรียก Bind(dt) หน้าเดียว
    /// ทั้ง ucNCRRecords และ ucDashboard ใช้ตัวนี้ร่วมกัน
    ///
    /// DataTable ที่ส่งเข้ามาต้องมีคอลัมน์
    /// NCR_NO, NCR_DATE, SUPPLIER, PART_NO, PROBLEM, OWNER, STATUS
    /// </summary>
    public partial class ucNcrTable : UserControl
    {
        private DataTable _source;

        public ucNcrTable()
        {
            InitializeComponent();

            dtg.ViewClicked += (s, e) => Raise(ViewClicked, e);
            dtg.EditClicked += (s, e) => Raise(EditClicked, e);
            dtg.DeleteClicked += (s, e) => Raise(DeleteClicked, e);
            dtg.ExportClicked += (s, e) => Raise(ExportClicked, e);

            pager.PageChanged += (s, e) => ShowPage();
        }

        public event EventHandler<RowActionEventArgs> ViewClicked;
        public event EventHandler<RowActionEventArgs> EditClicked;
        public event EventHandler<RowActionEventArgs> DeleteClicked;
        public event EventHandler<RowActionEventArgs> ExportClicked;

        /// <summary>เปิดไว้ให้หน้าที่ต้องการปรับ StatusColors หรือความกว้างคอลัมน์เอง</summary>
        [Browsable(false)]
        public StyledDataGrid Grid
        {
            get { return dtg; }
        }

        [Browsable(false)]
        public ucPager Pager
        {
            get { return pager; }
        }

        /// <summary>ข้อมูลทั้งชุด การแบ่งหน้าตัดเอาจากตัวนี้</summary>
        [Browsable(false)]
        public DataTable Source
        {
            get { return _source; }
        }

        public void Bind(DataTable source)
        {
            _source = source;
            pager.Bind(source == null ? 0 : source.Rows.Count); // ยิง PageChanged ต่อ
        }

        /// <summary>
        /// grid เป็น unbound ดันเข้าไปเฉพาะแถวของหน้าปัจจุบัน
        /// แต่ละแถวเก็บ DataRow ต้นทางไว้ใน Tag ให้ event ของปุ่ม action หยิบกลับมาได้
        /// </summary>
        private void ShowPage()
        {
            dtg.SuspendLayout();
            dtg.Rows.Clear();

            if (_source != null)
            {
                int first = pager.FirstRowIndex;
                int last = first + pager.RowsOnPage;
                for (int i = first; i < last; i++)
                {
                    DataRow source = _source.Rows[i];
                    int index = dtg.Rows.Add(
                        source["NCR_NO"],
                        Convert.ToDateTime(source["NCR_DATE"]).ToString("MM/dd/yyyy"),
                        source["SUPPLIER"],
                        source["PART_NO"],
                        source["PROBLEM"],
                        source["OWNER"],
                        source["STATUS"],
                        null);
                    dtg.Rows[index].Tag = source;
                }
            }

            dtg.ClearSelection();
            dtg.ResumeLayout();
        }

        private void Raise(EventHandler<RowActionEventArgs> handler, RowActionEventArgs e)
        {
            if (handler != null) handler(this, e);
        }
    }
}
