using CommonClassLibrary;
using PermissionLibrary;
using PermissionLibrary.Property;
using RawMat.Controllers;
using RawMat.Property;
using RawMat.Views.CustomMsg;
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

    public partial class userControlLogin : UserControl
    {

        public event EventHandler LoginSuccess;

        SecurityCenterControllers securityCenterControllers = new SecurityCenterControllers();
        EmployeeController employeeController = new EmployeeController();
        public EmployeeProperty employee = new EmployeeProperty();

        public userControlLogin()
        {
            InitializeComponent();
            tb_login.KeyDown += tb_login_KeyDown;
        }

        private void tb_login_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                bt_login_Click(this, new EventArgs());
            }
        }

        private void bt_login_Click(object sender, EventArgs e)
        {
            if(string.IsNullOrEmpty(tb_login.Text))
            {

                CustomMsgBoxBase.ShowCustomMessageBox(
                                "กรุณาใส่รหัสพนักงาน",
                                "แจ้งเตือน",
                                CustomMsgBoxBase.MessageBoxIconType.Question);

                return;
            }

            

           
            employee = employeeController.SearchEmpCode(new EmployeeProperty { EMP_CODE = tb_login.Text.Trim() });
            if (employee == null)
            {
                CustomMsgBoxBase.ShowCustomMessageBox(
                              "ไม่พบรหัสพนักงานนี้ในฐานข้อมูล",
                              "แจ้งเตือน",
                              CustomMsgBoxBase.MessageBoxIconType.Question);

                tb_login.Text = "";
                return;
            }

            // เปิดตอนใช้จริง
            //PermissionLibrary.Property.SignInProperty signprop = securityCenterControllers.SignIn(tb_login.Text.Trim(),tb_pass.Text);

            PermissionLibrary.Property.SignInProperty signprop = new PermissionLibrary.Property.SignInProperty();
            signprop.Status = true;


            if (signprop.Status == true)
            {
                CommonClassLibraryGlobal.OPERATOR_ID = tb_login.Text;
                CommonClassLibraryGlobal.Login_Status = true;
                
                

                // เรียก Event เมื่อล็อกอินสำเร็จ
                LoginSuccess?.Invoke(this, EventArgs.Empty);
               
                this.Hide();
            }
            else
            {

                CustomMsgBoxBase.ShowCustomMessageBox(
                             "รหัสพนักงานนี้ไม่สามารถเข้าใช้งานในส่วนนี้ได้ \nกรุณาลองใหม่อีกครั้ง",
                             "แจ้งเตือน",
                             CustomMsgBoxBase.MessageBoxIconType.NG);

                return;
            }
        }
    }
}
