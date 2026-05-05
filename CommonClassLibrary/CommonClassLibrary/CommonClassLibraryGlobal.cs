using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

namespace CommonClassLibrary
{
    public static class CommonClassLibraryGlobal
    {
        //DB Name
        public static readonly string DB_OPERATOR = "info_employees";

        //IP and Host
        public static string HOST_NAME = "";
        public static string IP = "";

        public static string OPERATOR_ID = "";
        public static string Login_TYPE_FORM = "";

        public static string FORM_ACTION_MODE = "";

        public static double chkDouble;
        public static int chkInt;

        public static bool Login_Status;

        public static void showSuccess(string msg)
        {
            MessageBox.Show(msg, "การแจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void showError(string msg)
        {
            MessageBox.Show(msg, "เกิดข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void showSaveDatabaseSuccess()
        {
            MessageBox.Show("บันทึกข้อมูลสำเร็จ", "การแจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void showSaveDatabaseError()
        {
            MessageBox.Show("บันทึกข้อมูลไม่สำเร็จ กรุณาลองใหม่อีกครั้ง", "เกิดข้อผิดพลาด", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static string GetLocalHostName()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.HostName;
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        public static void DELAY(double ms)
        {
            Stopwatch stw = new Stopwatch();
            stw.Reset();
            stw.Start();

            while (stw.ElapsedMilliseconds < ms)
            {
                System.Threading.Thread.Sleep(5);
                Application.DoEvents();
            }
            stw.Stop();
            stw = null;
        }

        public static void DELAY_OPHIR(double ms)
        {
            Stopwatch stw = new Stopwatch();
            stw.Reset();
            stw.Start();
            while (stw.ElapsedMilliseconds < ms)
            {
                Application.DoEvents();
            }
            stw.Stop();
        }

        public static void DELAY_SAFETY(double ms)
        {
            Stopwatch stw = new Stopwatch();
            stw.Reset();
            stw.Start();

            while (stw.ElapsedMilliseconds < ms)
            {
                //System.Threading.Thread.Sleep(5);
                Application.DoEvents();
            }
            stw.Stop();
        }

    }
}