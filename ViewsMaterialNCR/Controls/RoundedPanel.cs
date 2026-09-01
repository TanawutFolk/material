using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RawMat.ViewsMaterialNCR.Controls
{
    /// <summary>
    /// การ์ดพื้นขาวมุมมน ใช้เป็นกรอบนอกของทุกหน้าใน panelContent
    ///
    /// Region เป็นตัวตัดมุมให้ลูกที่ Dock อยู่ข้างใน (grid, pager) โดนตัดตามไปด้วย
    /// ส่วนเส้นขอบวาดทับแบบ anti-alias อีกที มุมจะได้ไม่หยัก
    /// </summary>
    public class RoundedPanel : Panel
    {
        private int _cornerRadius = 10;
        private Color _borderColor = NcrTheme.CardBorder;

        public RoundedPanel()
        {
            BackColor = Color.White;
            SetStyle(ControlStyles.ResizeRedraw, true);
        }

        [DefaultValue(10)]
        [Description("รัศมีมุมของการ์ด")]
        public int CornerRadius
        {
            get { return _cornerRadius; }
            set
            {
                if (_cornerRadius == value) return;
                _cornerRadius = value;
                UpdateRegion();
                Invalidate();
            }
        }

        [Description("สีเส้นขอบการ์ด")]
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                if (_borderColor == value) return;
                _borderColor = value;
                Invalidate();
            }
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            UpdateRegion();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = NcrTheme.RoundedRect(
                new Rectangle(0, 0, Width - 1, Height - 1), _cornerRadius))
            using (Pen border = new Pen(_borderColor))
            {
                e.Graphics.DrawPath(border, path);
            }
        }

        private void UpdateRegion()
        {
            if (Width <= 0 || Height <= 0) return;

            using (GraphicsPath path = NcrTheme.RoundedRect(ClientRectangle, _cornerRadius))
            {
                Region = new Region(path);
            }
        }
    }
}
