using RawMat.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Drawing.Text;
using RawMat.Utilities;


namespace RawMat.Login
{
    public partial class userControlProfile : UserControl
    {
        public event EventHandler Logout;
        public event Action ClearPanelRequested;
        private imgCls _imgHelper;
        //public EmployeeProperty employee = new EmployeeProperty();
        //EmployeeProperty employee = EmployeeManager.CurrentEmployee;

        public userControlProfile(EmployeeProperty employee)
        {
            InitializeComponent();
            _imgHelper = new imgCls(); // สร้างอ็อบเจกต์ imgCls

            lb_empProfile.Text = employee.EMP_CODE;

            lb_nameProfile.Text = "";
            lb_position.Text = "";

            //lb_nameProfile.Text = employee.EMP_FULL_NAME.Split(' ')[0];
            //lb_position.Text = employee.EMP_POSITION;
            try
            {
                pb_profile.Controls.Add(_imgHelper.LoadEmployeeImage(fileName: employee.EMP_CODE));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"พบข้อผิดพลาดในการเพิ่มภาพพนักงาน: {ex.Message}");
                pb_profile.Controls.Add(_imgHelper.CreateDefaultPictureBox());
            }

            
        }

        private void bt_logout_Click(object sender, EventArgs e)
        {
            // แสดง MessageBox เตือนการล็อกเอาท์
            DialogResult result = MessageBox.Show(
                "คุณต้องการออกจากระบบหรือไม่? ข้อมูลที่ยังไม่ได้บันทึกอาจสูญหายได้",
                "ยืนยันการล็อกเอาท์",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            // ตรวจสอบการตอบรับจากผู้ใช้
            if (result == DialogResult.Yes)
            {
                // ถ้าผู้ใช้เลือก Yes ให้ทำการล็อกเอาท์
                
                EmployeeManager.CurrentEmployee = null;

                if (ClearPanelRequested != null)
                {
                    ClearPanelRequested.Invoke();
                    Console.WriteLine("ClearPanelRequested invoked");
                }
                else
                {
                    MessageBox.Show("ClearPanelRequested is not subscribed.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                Logout?.Invoke(this, EventArgs.Empty);
            }
            // ถ้าผู้ใช้เลือก No จะไม่ทำการล็อกเอาท์

        }


        //public static PictureBox empImage(string emp_id)
        //{
        //    // ดึงพาธโฟลเดอร์จาก app.config
        //    string folderPath = ConfigurationManager.AppSettings["EmpImgPath"];

        //    if (string.IsNullOrEmpty(folderPath))
        //    {
        //        throw new Exception("Image folder path is not configured in app.config.");
        //    }

        //    // สร้างพาธสำหรับการค้นหาไฟล์ที่มีชื่อเดียวกัน แต่รองรับหลายนามสกุล
        //    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".bmp", ".gif" }; // สามารถเพิ่มหรือลดนามสกุลได้

        //    foreach (string extension in allowedExtensions)
        //    {
        //        string fullPath = Path.Combine(folderPath, emp_id + extension);

        //        // ตรวจสอบว่ามีไฟล์อยู่ในพาธหรือไม่
        //        if (File.Exists(fullPath))
        //        {
        //            try
        //            {
        //                Image image = Image.FromFile(fullPath);
        //                Size pictureBoxSize = new Size(100, 100);
        //                // สร้าง PictureBox และตั้งค่าขนาด
        //                PictureBox pictureBox = new PictureBox
        //                {
        //                    Size = pictureBoxSize,
        //                    Image = image,
        //                    SizeMode = PictureBoxSizeMode.Zoom // ปรับขนาดรูปภาพให้พอดีกับ PictureBox
        //                };

        //                return pictureBox;

        //            }
        //            catch (Exception ex)
        //            {
        //                throw new Exception($"Error loading image file: {fullPath}", ex);
        //            }
        //        }
        //    }

        //    // หากไม่พบไฟล์ที่มีชื่อและนามสกุลใด ๆ ให้ส่งกลับเป็น null
        //    throw new FileNotFoundException($"Image file with name {emp_id} not found in {folderPath}");
        //}




    }
}
