using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RawMat.ViewsMaterialNCR.Controls
{
    /// <summary>
    /// ตารางของหน้า NCR ทุกหน้า
    ///
    /// รับผิดชอบเฉพาะ "หน้าตากับการคลิก" เท่านั้น คือสไตล์ทั้งชุด, badge สถานะ,
    /// ปุ่ม 👁 / ⋮ ท้ายแถว และการแปลงคลิกให้เป็น event
    /// ส่วนคอลัมน์กับข้อมูลยังเป็นของแต่ละหน้าเหมือนเดิม
    ///
    /// วิธีใช้: ลากลง designer แล้วตั้ง StatusColumnName / ActionColumnName
    /// ให้ตรงกับชื่อคอลัมน์ จากนั้น subscribe event ที่ต้องการ
    /// แต่ละแถวต้องเก็บ DataRow ต้นทางไว้ใน Tag เพื่อให้ event ส่งกลับมาได้
    /// </summary>
    public class StyledDataGrid : Bunifu.Framework.UI.BunifuCustomDataGrid
    {
        private static readonly Font BadgeFont = new Font(NcrTheme.FontName, 8.25F, FontStyle.Bold);

        private readonly Dictionary<string, Color> _statusColors =
            new Dictionary<string, Color>(StringComparer.OrdinalIgnoreCase);

        private readonly ContextMenuStrip _rowMenu;
        private DataGridViewRow _menuRow;

        private string _statusColumnName = string.Empty;
        private string _actionColumnName = string.Empty;

        public StyledDataGrid()
        {
            // ค่าเริ่มต้น หน้าไหนมีสถานะอื่นก็เพิ่มผ่าน StatusColors ได้
            // ค่าที่ไม่รู้จักตกไปสีเทาแทนที่จะพัง
            _statusColors["Open"] = Color.FromArgb(229, 57, 53);
            _statusColors["In Review"] = Color.FromArgb(232, 151, 12);
            _statusColors["Closed"] = Color.FromArgb(22, 163, 74);

            _rowMenu = new ContextMenuStrip { Font = new Font(NcrTheme.FontName, 9F) };
            _rowMenu.Items.Add("Edit", null, (s, e) => Raise(EditClicked, _menuRow));
            _rowMenu.Items.Add("Export", null, (s, e) => Raise(ExportClicked, _menuRow));
            _rowMenu.Items.Add(new ToolStripSeparator());
            _rowMenu.Items.Add("Delete", null, (s, e) => Raise(DeleteClicked, _menuRow));

            ApplyStyle();
        }

        [Category("NCR"), DefaultValue("")]
        [Description("ชื่อคอลัมน์ที่จะวาดเป็น badge สถานะ เว้นว่างถ้าไม่มี")]
        public string StatusColumnName
        {
            get { return _statusColumnName; }
            set { _statusColumnName = value ?? string.Empty; Invalidate(); }
        }

        [Category("NCR"), DefaultValue("")]
        [Description("ชื่อคอลัมน์ที่จะวาดปุ่ม 👁 / ⋮ เว้นว่างถ้าไม่มี")]
        public string ActionColumnName
        {
            get { return _actionColumnName; }
            set { _actionColumnName = value ?? string.Empty; Invalidate(); }
        }

        [Browsable(false)]
        [Description("สีของแต่ละสถานะ ค่าที่ไม่อยู่ในนี้จะวาดเป็นสีเทา")]
        public IDictionary<string, Color> StatusColors
        {
            get { return _statusColors; }
        }

        public event EventHandler<RowActionEventArgs> ViewClicked;
        public event EventHandler<RowActionEventArgs> EditClicked;
        public event EventHandler<RowActionEventArgs> DeleteClicked;
        public event EventHandler<RowActionEventArgs> ExportClicked;

        private void ApplyStyle()
        {
            // เหตุผลเดียวที่สืบทอดจาก BunifuCustomDataGrid แทน DataGridView เปล่าๆ
            // คือ property ตัวนี้ ถ้าไม่เปิด การวาด badge ทุกเซลล์จะกระพริบตอน scroll
            DoubleBuffered = true;

            AllowUserToAddRows = false;
            AllowUserToDeleteRows = false;
            AllowUserToOrderColumns = false;
            AllowUserToResizeColumns = false;
            AllowUserToResizeRows = false;
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            BackgroundColor = Color.White;
            BorderStyle = BorderStyle.None;
            CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            ColumnHeadersHeight = 44;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            EnableHeadersVisualStyles = false;
            GridColor = NcrTheme.GridLine;
            MultiSelect = false;
            ReadOnly = true;
            RowHeadersVisible = false;
            RowTemplate.Height = 36;
            SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            DefaultCellStyle.Font = new Font(NcrTheme.FontName, 9F);
            DefaultCellStyle.BackColor = Color.White;
            DefaultCellStyle.ForeColor = NcrTheme.Text;
            DefaultCellStyle.SelectionBackColor = NcrTheme.RowSelected;
            DefaultCellStyle.SelectionForeColor = NcrTheme.Text;
            DefaultCellStyle.Padding = new Padding(10, 0, 10, 0);

            // BunifuCustomDataGrid เปิดแถบสลับสีมาให้ ธีมนี้เป็นขาวล้วน
            AlternatingRowsDefaultCellStyle.BackColor = Color.White;
            AlternatingRowsDefaultCellStyle.ForeColor = NcrTheme.Text;
            AlternatingRowsDefaultCellStyle.SelectionBackColor = NcrTheme.RowSelected;
            AlternatingRowsDefaultCellStyle.SelectionForeColor = NcrTheme.Text;

            ColumnHeadersDefaultCellStyle.Font = new Font(NcrTheme.FontName, 9.75F, FontStyle.Bold);
            ColumnHeadersDefaultCellStyle.BackColor = NcrTheme.HeaderBack;
            ColumnHeadersDefaultCellStyle.ForeColor = NcrTheme.HeaderText;
            ColumnHeadersDefaultCellStyle.SelectionBackColor = NcrTheme.HeaderBack;
            ColumnHeadersDefaultCellStyle.SelectionForeColor = NcrTheme.HeaderText;
            ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 0, 10, 0);
            ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
        }

        private bool IsColumn(int index, string name)
        {
            return name.Length > 0 && index >= 0 && Columns[index].Name == name;
        }

        private void Raise(EventHandler<RowActionEventArgs> handler, DataGridViewRow row)
        {
            if (handler != null && row != null) handler(this, new RowActionEventArgs(row));
        }

        protected override void OnCellPainting(DataGridViewCellPaintingEventArgs e)
        {
            base.OnCellPainting(e);
            if (e.Handled || e.RowIndex < 0) return; // หัวตารางวาดตามปกติ

            if (IsColumn(e.ColumnIndex, _statusColumnName))
            {
                PaintStatusBadge(e);
                e.Handled = true;
            }
            else if (IsColumn(e.ColumnIndex, _actionColumnName))
            {
                PaintActionIcons(e);
                e.Handled = true;
            }
        }

        private void PaintStatusBadge(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);

            string status = Convert.ToString(e.Value) ?? string.Empty;
            if (status.Length == 0) return;

            Color accent;
            if (!_statusColors.TryGetValue(status, out accent)) accent = NcrTheme.Muted;

            Size text = TextRenderer.MeasureText(status, BadgeFont);
            Rectangle badge = new Rectangle(
                e.CellBounds.X + 10,
                e.CellBounds.Y + (e.CellBounds.Height - 24) / 2,
                text.Width + 20,
                24);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (GraphicsPath path = NcrTheme.RoundedRect(badge, 6))
            using (SolidBrush fill = new SolidBrush(Color.FromArgb(26, accent)))
            using (Pen border = new Pen(accent))
            {
                e.Graphics.FillPath(fill, path);
                e.Graphics.DrawPath(border, path);
            }

            TextRenderer.DrawText(e.Graphics, status, BadgeFont, badge, accent,
                TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
        }

        private void PaintActionIcons(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle[] icons = ActionIconRects(e.CellBounds);
            using (Pen box = new Pen(NcrTheme.Border))
            {
                foreach (Rectangle icon in icons)
                {
                    using (GraphicsPath path = NcrTheme.RoundedRect(
                        new Rectangle(icon.X, icon.Y, icon.Width - 1, icon.Height - 1), 5))
                    {
                        e.Graphics.DrawPath(box, path);
                    }
                }
            }

            DrawEyeIcon(e.Graphics, icons[0]);
            DrawKebabIcon(e.Graphics, icons[1]);
        }

        /// <summary>
        /// กรอบปุ่มสองอัน วัดจากสี่เหลี่ยมที่ส่งเข้ามา
        /// ตอนวาดส่งกรอบเซลล์จริง ตอนเช็คคลิกส่งกรอบที่อิงมุมเซลล์
        /// คำนวณจากที่เดียวกัน ตำแหน่งที่เห็นกับที่คลิกโดนเลยเลื่อนหลุดจากกันไม่ได้
        /// </summary>
        private static Rectangle[] ActionIconRects(Rectangle cell)
        {
            const int size = 28;
            const int gap = 6;
            int y = cell.Y + (cell.Height - size) / 2;
            int x = cell.X + 8;

            return new Rectangle[]
            {
                new Rectangle(x, y, size, size),
                new Rectangle(x + size + gap, y, size, size),
            };
        }

        private static void DrawEyeIcon(Graphics g, Rectangle box)
        {
            using (Pen pen = new Pen(NcrTheme.Muted, 1.4F))
            {
                g.DrawEllipse(pen, box.X + 6, box.Y + 9, 16, 10);
            }
            using (SolidBrush brush = new SolidBrush(NcrTheme.Muted))
            {
                g.FillEllipse(brush, box.X + 11, box.Y + 11, 6, 6);
            }
        }

        private static void DrawKebabIcon(Graphics g, Rectangle box)
        {
            using (SolidBrush brush = new SolidBrush(NcrTheme.Muted))
            {
                for (int i = 0; i < 3; i++)
                {
                    g.FillEllipse(brush, box.X + 12.5F, box.Y + 8F + i * 5.5F, 3.5F, 3.5F);
                }
            }
        }

        private int HitIcon(DataGridViewCellMouseEventArgs e)
        {
            Rectangle cell = GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            Rectangle[] icons = ActionIconRects(new Rectangle(0, 0, cell.Width, cell.Height));

            if (icons[0].Contains(e.X, e.Y)) return 0;
            if (icons[1].Contains(e.X, e.Y)) return 1;
            return -1;
        }

        protected override void OnCellMouseClick(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseClick(e);
            if (e.RowIndex < 0 || !IsColumn(e.ColumnIndex, _actionColumnName)) return;

            int icon = HitIcon(e);
            if (icon == 0)
            {
                Raise(ViewClicked, Rows[e.RowIndex]);
            }
            else if (icon == 1)
            {
                _menuRow = Rows[e.RowIndex];
                _rowMenu.Show(this, PointToClient(Cursor.Position));
            }
        }

        protected override void OnCellMouseMove(DataGridViewCellMouseEventArgs e)
        {
            base.OnCellMouseMove(e);

            bool onIcon = e.RowIndex >= 0
                && IsColumn(e.ColumnIndex, _actionColumnName)
                && HitIcon(e) >= 0;

            Cursor = onIcon ? Cursors.Hand : Cursors.Default;
        }

        protected override void OnCellMouseLeave(DataGridViewCellEventArgs e)
        {
            base.OnCellMouseLeave(e);
            Cursor = Cursors.Default;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && _rowMenu != null) _rowMenu.Dispose();
            base.Dispose(disposing);
        }
    }

    /// <summary>
    /// แถวที่ถูกกดปุ่ม action พร้อม DataRow ต้นทางที่หน้าเก็บไว้ใน Tag
    /// </summary>
    public class RowActionEventArgs : EventArgs
    {
        public RowActionEventArgs(DataGridViewRow gridRow)
        {
            GridRow = gridRow;
        }

        public DataGridViewRow GridRow { get; private set; }

        public DataRow Row
        {
            get { return GridRow == null ? null : GridRow.Tag as DataRow; }
        }
    }
}
