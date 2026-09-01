using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RawMat.ViewsMaterialNCR.Controls
{
    /// <summary>
    /// แถบแบ่งหน้าท้ายตาราง: จำนวนที่แสดง + ปุ่มเลขหน้า + จำนวนต่อหน้า
    ///
    /// ตัวนี้ไม่รู้จักข้อมูลเลย รู้แค่จำนวนแถวทั้งหมด เอาไปแปะหน้าไหนก็ได้
    /// วิธีใช้: subscribe PageChanged แล้วเรียก Bind(จำนวนแถวทั้งหมด)
    /// ใน handler อ่าน FirstRowIndex กับ RowsOnPage ไปตัดข้อมูลเอง
    /// </summary>
    public partial class ucPager : UserControl
    {
        private static readonly Font PagerFont = new Font(NcrTheme.FontName, 9F);

        private int _totalRows;
        private int _pageSize = 10;
        private int _currentPage = 1;

        public ucPager()
        {
            InitializeComponent();

            cmb_PageSize.Items.AddRange(new object[] { 10, 25, 50, 100 });
            cmb_PageSize.SelectedItem = _pageSize;

            // ตอนวางอยู่บน designer ไม่ต้องสร้างปุ่มจริงหรือผูก event
            // ปล่อยให้ designer เห็นแค่กล่องเปล่าจะปลอดภัยกว่า
            // (DesignMode ใน constructor ยังเป็น false เสมอ ต้องถาม LicenseManager)
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return;

            cmb_PageSize.SelectedIndexChanged += cmb_PageSize_SelectedIndexChanged;
            Resize += (s, e) => LayoutBar();

            Rebuild();
        }

        [Description("เกิดทุกครั้งที่หน้าหรือจำนวนต่อหน้าเปลี่ยน รวมถึงตอนเรียก Bind")]
        public event EventHandler PageChanged;

        [Browsable(false)]
        public int CurrentPage
        {
            get { return _currentPage; }
        }

        [Browsable(false)]
        public int TotalRows
        {
            get { return _totalRows; }
        }

        [Browsable(false)]
        public int TotalPages
        {
            get
            {
                int pages = (_totalRows + _pageSize - 1) / _pageSize;
                return pages < 1 ? 1 : pages;
            }
        }

        /// <summary>ลำดับแถวแรกของหน้านี้ นับจาก 0</summary>
        [Browsable(false)]
        public int FirstRowIndex
        {
            get { return (_currentPage - 1) * _pageSize; }
        }

        /// <summary>จำนวนแถวที่หน้านี้แสดงจริง หน้าสุดท้ายจะน้อยกว่า PageSize</summary>
        [Browsable(false)]
        public int RowsOnPage
        {
            get { return Math.Max(0, Math.Min(_pageSize, _totalRows - FirstRowIndex)); }
        }

        [Category("NCR"), DefaultValue(10)]
        [Description("จำนวนแถวต่อหน้า ค่าที่ไม่มีในรายการจะถูกเพิ่มให้")]
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                if (value < 1) return;
                if (!cmb_PageSize.Items.Contains(value)) cmb_PageSize.Items.Add(value);
                cmb_PageSize.SelectedItem = value; // ให้ handler เป็นคนอัปเดตสถานะ
            }
        }

        /// <summary>ตั้งจำนวนแถวทั้งหมดใหม่ กลับไปหน้า 1 แล้วยิง PageChanged</summary>
        public void Bind(int totalRows)
        {
            _totalRows = Math.Max(0, totalRows);
            _currentPage = 1;
            Rebuild();
            OnPageChanged();
        }

        public void GoTo(int page)
        {
            int clamped = Math.Min(Math.Max(page, 1), TotalPages);
            if (clamped == _currentPage) return;

            _currentPage = clamped;
            Rebuild();
            OnPageChanged();
        }

        protected virtual void OnPageChanged()
        {
            EventHandler handler = PageChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        private void cmb_PageSize_SelectedIndexChanged(object sender, EventArgs e)
        {
            _pageSize = Convert.ToInt32(cmb_PageSize.SelectedItem);
            _currentPage = 1;
            Rebuild();
            OnPageChanged();
        }

        private void Rebuild()
        {
            int first = FirstRowIndex;
            int count = RowsOnPage;
            lb_Summary.Text = string.Format("Showing {0} to {1} of {2} entries",
                _totalRows == 0 ? 0 : first + 1, first + count, _totalRows);

            BuildButtons();
            LayoutBar();
        }

        private void BuildButtons()
        {
            flp_Pager.SuspendLayout();
            while (flp_Pager.Controls.Count > 0)
            {
                Control old = flp_Pager.Controls[0];
                flp_Pager.Controls.Remove(old);
                old.Dispose();
            }

            int total = TotalPages;
            flp_Pager.Controls.Add(MakeButton("«", false, _currentPage > 1, (s, e) => GoTo(1)));
            flp_Pager.Controls.Add(MakeButton("‹", false, _currentPage > 1, (s, e) => GoTo(_currentPage - 1)));

            // แสดงเลขหน้าไม่เกินห้าปุ่ม โดยให้หน้าปัจจุบันอยู่กลาง
            int start = Math.Max(1, _currentPage - 2);
            int end = Math.Min(total, start + 4);
            start = Math.Max(1, end - 4);
            for (int n = start; n <= end; n++)
            {
                int target = n; // ตัวแปรลูปใช้ร่วมกัน ต้องคัดลอกไว้ต่อปุ่ม
                flp_Pager.Controls.Add(MakeButton(
                    n.ToString(), n == _currentPage, true, (s, e) => GoTo(target)));
            }

            flp_Pager.Controls.Add(MakeButton("›", false, _currentPage < total, (s, e) => GoTo(_currentPage + 1)));
            flp_Pager.Controls.Add(MakeButton("»", false, _currentPage < total, (s, e) => GoTo(total)));

            flp_Pager.ResumeLayout();
        }

        private Button MakeButton(string text, bool active, bool enabled, EventHandler onClick)
        {
            Button button = new Button
            {
                Text = text,
                Size = new Size(32, 32),
                Margin = new Padding(3, 0, 0, 0),
                FlatStyle = FlatStyle.Flat,
                Font = PagerFont,
                Enabled = enabled,
                Cursor = enabled ? Cursors.Hand : Cursors.Default,
                Tag = active,
                TabStop = false,
            };
            button.FlatAppearance.BorderSize = 0;
            button.Paint += Button_Paint;
            button.Click += onClick;
            return button;
        }

        // Button ทำมุมมนเองไม่ได้ เลยทาสีทับหน้าปุ่มทั้งใบ
        private void Button_Paint(object sender, PaintEventArgs e)
        {
            Button button = (Button)sender;
            bool active = (bool)button.Tag;

            e.Graphics.Clear(button.Parent.BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle face = new Rectangle(0, 0, button.Width - 1, button.Height - 1);
            using (GraphicsPath path = NcrTheme.RoundedRect(face, 5))
            using (SolidBrush fill = new SolidBrush(active ? NcrTheme.Accent : Color.White))
            using (Pen border = new Pen(active ? NcrTheme.Accent : NcrTheme.Border))
            {
                e.Graphics.FillPath(fill, path);
                e.Graphics.DrawPath(border, path);
            }

            Color foreColor = active
                ? Color.White
                : (button.Enabled ? NcrTheme.Text : NcrTheme.Disabled);

            TextRenderer.DrawText(e.Graphics, button.Text, button.Font, face, foreColor,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        // แถบปุ่มกว้างไม่คงที่ตามจำนวนหน้า Anchor เลยเอาไม่อยู่
        // (Anchor ขยับตอนพ่อเปลี่ยนขนาด ไม่ใช่ตอนตัวเองเปลี่ยนขนาด)
        private void LayoutBar()
        {
            pn_PageSize.Left = Width - pn_PageSize.Width - 16;
            pn_PageSize.Top = (Height - pn_PageSize.Height) / 2;

            flp_Pager.Left = pn_PageSize.Left - flp_Pager.Width - 24;
            flp_Pager.Top = (Height - flp_Pager.Height) / 2;

            lb_Summary.Top = (Height - lb_Summary.Height) / 2;
        }
    }
}
