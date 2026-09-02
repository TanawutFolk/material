using RawMat.ViewsMaterialNCR.panelContent;
using RawMat.Property;
using RawMat.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.ViewsMaterialNCR
{
    public partial class frmMainNCR : Form
    {
        private static readonly Color MenuColor = Color.FromArgb(0, 94, 184);
        private static readonly Color MenuActiveColor = Color.FromArgb(0, 63, 122);

        // one sidebar entry: the icon drawn on top of the button and the page it opens
        private sealed class MenuItem
        {
            public PictureBox Icon;
            public Func<UserControl> CreatePage;
            public UserControl Page;
        }

        private readonly Dictionary<Button, MenuItem> _menuItems;
        private Button _activeMenu;

        public frmMainNCR()
        {
            InitializeComponent();
            lb_version.Text =
                ConfigurationManager.AppSettings["programNCRVersion"] ?? "Error Label";
            ShowEmployee();
            _menuItems = new Dictionary<Button, MenuItem>
            {
                { btn_Dashboard,  new MenuItem { Icon = pictureBox_Dashboard,  CreatePage = () => new ucDashboard() } },
                { btn_NCRRecords, new MenuItem { Icon = pictureBox_ncrRecords, CreatePage = () => new ucNCRRecords() } },
                { button3,        new MenuItem { Icon = pictureBox_Approval,   CreatePage = () => new ucApproval() } },
                { button4,        new MenuItem { Icon = pictureBox_Report,     CreatePage = () => new ucReports() } },
                { button5,        new MenuItem { Icon = pictureBox_Setting,    CreatePage = () => new ucSetting() } },
            };

            foreach (KeyValuePair<Button, MenuItem> item in _menuItems)
            {
                // keep the hover/pressed shades identical to the active colour so the
                // selected item does not change tone while the cursor is still on it
                item.Key.FlatAppearance.MouseOverBackColor = MenuActiveColor;
                item.Key.FlatAppearance.MouseDownBackColor = MenuActiveColor;
                item.Key.Click += Menu_Click;
                item.Key.MouseEnter += Menu_MouseEnter;
                item.Key.MouseLeave += Menu_MouseLeave;

                item.Value.Icon.Cursor = Cursors.Hand;
                item.Value.Icon.Click += Menu_Click;
                item.Value.Icon.MouseEnter += Menu_MouseEnter;
                item.Value.Icon.MouseLeave += Menu_MouseLeave;
            }

            // pn_MoveFollow is added after the docked buttons, so it would be painted
            // behind them on every row except the first one
            pn_MoveFollow.BringToFront();

            picb_empPicture.Paint += picb_empPicture_Paint;

            SetActiveMenu(btn_Dashboard);
        }

        // the icon covers part of its button, so a click on it never reaches the button
        private void Menu_Click(object sender, EventArgs e)
        {
            SetActiveMenu(MenuOf(sender));
        }

        private void Menu_MouseEnter(object sender, EventArgs e)
        {
            SetHoverMenu(MenuOf(sender), true);
        }

        private void Menu_MouseLeave(object sender, EventArgs e)
        {
            Button menu = MenuOf(sender);
            if (menu == null) return;

            // the icon is a sibling drawn on top of the button, so moving the cursor
            // between the two raises a MouseLeave even though it never left the row
            if (menu.ClientRectangle.Contains(menu.PointToClient(Cursor.Position))) return;

            SetHoverMenu(menu, false);
        }

        // the button repaints itself with FlatAppearance while the cursor is on it, but
        // not while the cursor is on the icon, so both shades are driven by hand here
        private void SetHoverMenu(Button menu, bool hovered)
        {
            if (menu == null || menu == _activeMenu) return;

            Color color = hovered ? MenuActiveColor : MenuColor;
            menu.BackColor = color;
            _menuItems[menu].Icon.BackColor = color;
        }

        private void SetActiveMenu(Button menu)
        {
            if (menu == null || menu == _activeMenu) return;

            foreach (KeyValuePair<Button, MenuItem> item in _menuItems)
            {
                bool active = item.Key == menu;
                item.Key.BackColor = active ? MenuActiveColor : MenuColor;
                item.Value.Icon.BackColor = active ? MenuActiveColor : MenuColor;
            }

            pn_MoveFollow.Location = new Point(0, menu.Top);
            _activeMenu = menu;

            ShowPage(_menuItems[menu]);
        }

        // a page is built on its first visit and then kept, so coming back to it does
        // not throw away the grid, the filters or anything half filled in
        private void ShowPage(MenuItem item)
        {
            pn_Page.SuspendLayout();

            if (item.Page == null)
            {
                item.Page = item.CreatePage();
                item.Page.Dock = DockStyle.Fill;
                pn_Page.Controls.Add(item.Page);
            }

            foreach (KeyValuePair<Button, MenuItem> other in _menuItems)
            {
                if (other.Value != item && other.Value.Page != null)
                {
                    other.Value.Page.Visible = false;
                }
            }

            item.Page.Visible = true;
            item.Page.BringToFront();

            pn_Page.ResumeLayout();
        }

        // resolves either a menu button or the icon drawn on it back to the button
        private Button MenuOf(object sender)
        {
            Button menu = sender as Button;
            if (menu != null) return menu;

            foreach (KeyValuePair<Button, MenuItem> item in _menuItems)
            {
                if (item.Value.Icon == sender) return item.Key;
            }

            return null;
        }

        // ป้ายมุมขวาบน บอกว่าใครกำลังใช้งานอยู่ frmLogin เซ็ต EmployeeManager ไว้ให้แล้วก่อนเปิดหน้านี้
        private void ShowEmployee()
        {
            EmployeeProperty employee = EmployeeManager.CurrentEmployee ?? new EmployeeProperty();

            lb_empCode.Text = employee.EMP_CODE ?? string.Empty;
            lb_empName.Text = (employee.EMP_NAME + " " + employee.EMP_SURNAME).Trim();

            ShowEmployeePicture(employee.EMP_CODE);
        }

        // รูปอยู่บนแชร์ตาม EmpImgPath ใน App.config ต่อไม่ติดหรือไม่มีไฟล์ก็ห้ามทำให้เปิดหน้าไม่ได้
        // ไม่พบรูปก็ปล่อย Image เป็น null ไว้ ให้ picb_empPicture_Paint วาดวงกลมสีพื้นแทน
        private void ShowEmployeePicture(string empCode)
        {
            if (string.IsNullOrEmpty(empCode)) return;

            try
            {
                Image photo = new imgCls().LoadSingleImageOrNull("EmpImgPath", empCode);
                if (photo == null) return;

                // คัดลอกลง Bitmap ใหม่ทันที เพราะ LoadSingleImageOrNull ปิด FileStream ไปแล้วก่อน return
                // ถ้าถือตัวที่ยังผูกกับ stream ที่ปิดแล้วไว้ มันจะไปพังตอน TextureBrush วาด ซึ่งอยู่ใน Paint
                using (photo)
                {
                    picb_empPicture.Image = new Bitmap(photo);
                }
            }
            catch (Exception ex)
            {
                // รูปไม่ขึ้นไม่ใช่เรื่องที่ต้องหยุดคนทำงาน วงกลมสีพื้นยังอยู่เหมือนเดิม
                Console.WriteLine("โหลดรูปพนักงานไม่สำเร็จ: " + ex.Message);
            }
        }

        // the avatar is filled as an anti-aliased ellipse instead of being clipped by a
        // Region, because a Region cuts on whole pixels and leaves a stair-stepped edge
        private void picb_empPicture_Paint(object sender, PaintEventArgs e)
        {
            Rectangle bounds = picb_empPicture.ClientRectangle;

            // the box is not square, so the circle takes the shorter side and is centred
            int diameter = Math.Min(bounds.Width, bounds.Height);
            Rectangle circle = new Rectangle(
                (bounds.Width - diameter) / 2,
                (bounds.Height - diameter) / 2,
                diameter - 1,
                diameter - 1);

            e.Graphics.Clear(picb_empPicture.Parent.BackColor);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

            Image picture = picb_empPicture.Image;
            if (picture == null)
            {
                // no photo loaded yet - keep the round placeholder rather than a square
                using (SolidBrush brush = new SolidBrush(picb_empPicture.BackColor))
                {
                    e.Graphics.FillEllipse(brush, circle);
                }
                return;
            }

            using (TextureBrush brush = new TextureBrush(picture, WrapMode.TileFlipXY))
            {
                // scale on the longer side so the photo covers the circle and is cropped
                // instead of squashed, then centre what is left over
                float scale = Math.Max((float)diameter / picture.Width, (float)diameter / picture.Height);
                brush.TranslateTransform(
                    circle.X + (diameter - picture.Width * scale) / 2f,
                    circle.Y + (diameter - picture.Height * scale) / 2f);
                brush.ScaleTransform(scale, scale);

                e.Graphics.FillEllipse(brush, circle);
            }
        }

        private void lb_version_Click(object sender, EventArgs e)
        {
            
        }
    }
}
