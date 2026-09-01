using CommonClassLibrary;
using PermissionLibrary;
using RawMat.Controllers;
using RawMat.Property;
using RawMat.ViewsLogin;
using RawMat.ViewsMaterial.CustomMsg;
using RawMat.ViewsMaterialNCR;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Login
{
    /// <summary>
    /// หน้าแรกของโปรแกรม ทำสองหน้าที่ในฟอร์มเดียว
    /// ตอนเปิดคือหน้า login พอผ่านแล้วซ่อนชุด login ทิ้ง เหลือ pn_Mat กับ pn_ncr
    /// เป็นหน้าเลือกว่าจะเข้าระบบไหน
    ///
    /// ตัวตรวจสอบยกมาจาก ViewsMaterial/Login/userControlLogin.cs ทั้งชุด
    /// รวมถึงที่ SignIn ยังถูกปิดไว้ด้วย เพื่อให้พฤติกรรมเหมือนของเดิมทุกอย่าง
    /// </summary>
    public partial class frmLogin : Form
    {
        private readonly SecurityCenterControllers securityCenterControllers = new SecurityCenterControllers();
        private readonly EmployeeController employeeController = new EmployeeController();

        private readonly ucMaterial materialTile = new ucMaterial { Dock = DockStyle.Fill };
        private readonly ucNCR ncrTile = new ucNCR { Dock = DockStyle.Fill };

        private EmployeeProperty employee;
        private Form module;

        public frmLogin()
        {
            InitializeComponent();

            btn_Login.Click += btn_Login_Click;

            // btn_Login เป็น UserControl ของ Bunifu ไม่ใช่ IButtonControl เลยตั้ง AcceptButton ไม่ได้
            // และ BunifuMetroTextbox ก็ไม่ส่ง KeyDown ของ TextBox ข้างในออกมา
            // ดักที่ระดับฟอร์มแทน กด Enter จากช่องไหนก็ login ได้เหมือนกัน
            KeyPreview = true;
            KeyDown += frmLogin_KeyDown;

            materialTile.Clicked += materialTile_Clicked;
            ncrTile.Clicked += ncrTile_Clicked;
        }

        private void frmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            // ผ่าน login แล้วปุ่มจะถูกซ่อน หน้านี้กลายเป็นเมนู Enter ไม่ต้องทำอะไรต่อ
            if (e.KeyCode != Keys.Enter || !btn_Login.Visible) return;

            e.Handled = true;
            e.SuppressKeyPress = true;
            btn_Login_Click(this, EventArgs.Empty);
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            string empCode = bunifuMetroTextbox1.Text.Trim();

            if (string.IsNullOrEmpty(empCode))
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                    "กรุณาใส่รหัสพนักงาน",
                    "แจ้งเตือน",
                    CustomMsgBoxBase.MessageBoxIconType.Question);

                return;
            }

            employee = employeeController.SearchEmpCode(new EmployeeProperty { EMP_CODE = empCode });
            if (employee == null)
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                    "ไม่พบรหัสพนักงานนี้ในฐานข้อมูล",
                    "แจ้งเตือน",
                    CustomMsgBoxBase.MessageBoxIconType.Question);

                bunifuMetroTextbox1.Text = "";
                return;
            }

            // เปิดตอนใช้จริง
            //PermissionLibrary.Property.SignInProperty signprop = securityCenterControllers.SignIn(empCode, bunifuMetroTextbox2.Text);

            PermissionLibrary.Property.SignInProperty signprop = new PermissionLibrary.Property.SignInProperty();
            signprop.Status = true;

            if (signprop.Status != true)
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                    "รหัสพนักงานนี้ไม่สามารถเข้าใช้งานในส่วนนี้ได้ \nกรุณาลองใหม่อีกครั้ง",
                    "แจ้งเตือน",
                    CustomMsgBoxBase.MessageBoxIconType.NG);

                return;
            }

            CommonClassLibraryGlobal.OPERATOR_ID = empCode;
            CommonClassLibraryGlobal.Login_Status = true;

            // ระดับสิทธิ์ยัดกลับเข้า employee ตัวเดิม frmMain ใช้ค่านี้ตอนจัดเมนู
            employeeController.SearchEmpLevel(employee);
            EmployeeManager.CurrentEmployee = employee;

            ShowMenu();
        }

        /// <summary>ซ่อนชุด login ทั้งหมด แล้ววางไทล์ลงสองแผงที่เหลือ</summary>
        private void ShowMenu()
        {
            foreach (Control control in Controls)
            {
                if (control != pn_Mat && control != pn_ncr) control.Visible = false;
            }

            // pictureBox1 เป็นลูกของ pn_Mat ไม่ใช่ของฟอร์ม ลูปข้างบนเลยไม่โดน
            pictureBox1.Visible = false;

            AddTile(pn_Mat, materialTile);
            AddTile(pn_ncr, ncrTile);
        }

        // Controls.Add วางตัวใหม่ไว้ท้าย collection ซึ่งเป็นล่างสุดของ z-order
        // ถ้าไม่ BringToFront ไทล์จะไปอยู่หลังของเดิมที่อยู่ในแผง
        private static void AddTile(Panel panel, UserControl tile)
        {
            panel.Controls.Add(tile);
            tile.BringToFront();
        }

        private void materialTile_Clicked(object sender, EventArgs e)
        {
            OpenModule(new frmMain());
        }

        private void ncrTile_Clicked(object sender, EventArgs e)
        {
            OpenModule(new frmMainNCR());
        }

        /// <summary>
        /// ซ่อนเมนูไว้เฉยๆ ไม่ปิด เพราะฟอร์มนี้เป็น main form ของ Application.Run
        /// ปิดเมื่อไหร่โปรแกรมจบทันที ปิดโมดูลแล้วจึงกลับมาที่เมนูโดยไม่ต้อง login ใหม่
        /// </summary>
        private void OpenModule(Form form)
        {
            if (module != null && !module.IsDisposed)
            {
                module.Activate();
                return;
            }

            module = form;
            form.FormClosed += module_FormClosed;

            Hide();
            form.Show();
        }

        private void module_FormClosed(object sender, FormClosedEventArgs e)
        {
            module = null;
            Show();
        }
    }
}
