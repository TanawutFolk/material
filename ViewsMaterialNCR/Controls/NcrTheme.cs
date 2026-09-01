using System.Drawing;
using System.Drawing.Drawing2D;

namespace RawMat.ViewsMaterialNCR.Controls
{
    /// <summary>
    /// สีและรูปทรงกลางของหน้าจอ NCR ทั้งหมด
    ///
    /// ทุกตัวใน Controls/ ดึงสีจากที่นี่ที่เดียว เวลาแก้ธีมจะได้ไม่ต้องไล่แก้ทีละไฟล์
    /// </summary>
    internal static class NcrTheme
    {
        public static readonly Color Accent = Color.FromArgb(0, 94, 184);
        public static readonly Color AccentDark = Color.FromArgb(0, 63, 122);
        public static readonly Color Border = Color.FromArgb(214, 222, 232);
        public static readonly Color CardBorder = Color.FromArgb(227, 233, 242);
        public static readonly Color Text = Color.FromArgb(51, 65, 79);
        public static readonly Color Muted = Color.FromArgb(107, 122, 140);
        public static readonly Color Disabled = Color.FromArgb(183, 193, 203);
        public static readonly Color HeaderBack = Color.FromArgb(247, 249, 252);
        public static readonly Color HeaderText = Color.FromArgb(44, 62, 80);
        public static readonly Color GridLine = Color.FromArgb(232, 236, 241);
        public static readonly Color RowSelected = Color.FromArgb(234, 243, 254);
        public static readonly Color PageBack = Color.FromArgb(236, 244, 253);

        public const string FontName = "Microsoft YaHei UI";

        /// <summary>
        /// สี่เหลี่ยมมุมมน ใช้ทั้งวาดขอบการ์ด ปุ่มหน้า และ badge สถานะ
        /// ผู้เรียกต้อง dispose เอง
        /// </summary>
        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
