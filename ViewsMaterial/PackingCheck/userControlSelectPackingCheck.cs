using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.ViewsMaterial.CustomMsg;
using RawMat.ViewsMaterial.PackingCheck;
using RawMat.ViewsMaterial.ReceiveWH;
using RawMat.ViewsMaterial.RegularCheck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using static RawMat.Property.QAdataProperty;
using RawMat.ViewsMaterial.InspDataCheck;
using static RawMat.ViewsMaterial.CustomMsg.CustomMsgBoxBase;

namespace RawMat.ViewsMaterial.PackingCheck
{
    public partial class userControlSelectPackingCheck : UserControl
    {
        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler SaveRequested;
        public QAdataProperty propQA;
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        public QAdataControllers conQA = new QAdataControllers();
        private IParent parent;
        private NetworkInfoCls netInfo = new NetworkInfoCls();
        public userControlSelectPackingCheck()
        {
            InitializeComponent();

        }

        private void userControlSelectReport_Load(object sender, EventArgs e)
        {
            lb_process.Text = propQA.labelProcess.Replace("\n", " ");
            dtg_reportSelect.DataSource = propQA.dtgRawMat.DataSource;

            dtg_reportSelect.Columns["process_status_id"].Visible = false;
            dtg_reportSelect.Columns["Issue_Date"].Visible = false;

        }

        private void dtg_reportSelect_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex >= 0 && e.RowIndex < dtg_reportSelect.Rows.Count && e.ColumnIndex >= 0 && e.ColumnIndex < dtg_reportSelect.Columns.Count)
            {

                propQA.Report_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                propQA.Invoice_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                propQA.M_CODE = dtg_reportSelect.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                propQA.Material_Name = dtg_reportSelect.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                propQA.Qty = dtg_reportSelect.Rows[e.RowIndex].Cells["Lot Size"].Value.ToString();
                //usrPack.propQA.Receive_Date = dtg_reportSelect.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString();
                propQA.dtReceiveDate = DateTime.Parse(dtg_reportSelect.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());
                propQA.dtIssueDate = DateTime.Parse(dtg_reportSelect.Rows[e.RowIndex].Cells["Issue_Date"].Value.ToString());

                propQA.myIPv4 = netInfo.GetIPActive();
                propQA.MY_COMPUTER_NAME = netInfo.GetComputerName();
                //propQA.COMPUTER_NAME = conQA.SearchReportActive(propQA);

                propQA.dt_report_active = conQA.SearchReportActive(propQA);

                if (propQA.dt_report_active != null)
                {
                    propQA.COMPUTER_NAME = propQA.dt_report_active.Rows[0]["COMPUTER_NAME"].ToString();
                    propQA.reportIP = propQA.dt_report_active.Rows[0]["active_user"].ToString();
                }
                else
                {
                    propQA.COMPUTER_NAME = "";
                    propQA.reportIP = "";
                }


                try
                {
                    // ตรวจสอบว่ามีคนใช้งานอยู่หรือไม่
                    if (!string.IsNullOrWhiteSpace(propQA.reportIP))
                    {
                        // มีคนใช้งานอยู่
                        if (propQA.myIPv4 == propQA.reportIP)
                        {
                            // เป็นตัวเองกำลังใช้งานอยู่
                            propQA.inProcStatus = ((int)ProcStatus.Working).ToString();
                            propQA.reportStatus = ((int)ProcStatus.Working).ToString();

                            if (conQA.UpdateStatus(propQA) == false)
                            {
                                MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Working ได้");
                                return;
                            }
                        }
                        else
                        {
                            // มีคนอื่นใช้งานอยู่
                            string message = $"{propQA.COMPUTER_NAME} กำลังใช้งานอยู่ (IP: {propQA.reportIP})\n";

                            // ตรวจสอบว่าเป็น Admin หรือไม่ (EMP_LEVEL == 1)
                            if (employee.EMP_LEVEL == "1")
                            {
                                message = "ต้องการปลดล็อคหรือไม่?";
                                bool result = CustomMsgBoxBase.ShowCustomMessageBox(
                                    message,
                                    "แจ้งเตือน",
                                    CustomMsgBoxBase.MessageBoxIconType.Question,
                                    MessageBoxDialogType.YesNo); // มี Yes/No

                                if (result == true )
                                {
                                    // ลบข้อมูลในตารางที่ insert ไว้
                                    if (conQA.DeleteReportActive(propQA))
                                    {
                                        //// ลบสำเร็จ ทำการเข้าใช้งาน
                                        propQA.inProcStatus = ((int)ProcStatus.Unfinished).ToString();
                                        propQA.reportStatus = ((int)ProcStatus.Unfinished).ToString();
                                        
                                        if (conQA.UpdateStatus(propQA) == false)
                                        {
                                            MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Working ได้");
                                            return;
                                        }

                                        bt_rec_pack_Click();

                                        return;
                                        //propQA.packing_check_mode = conQA.PackingCheckMode(propQA);

                                        //if (propQA.process == "Packing_Check")
                                        //{
                                        //    var parentForm = this.FindForm() as frmMain;
                                        //    parentForm?.VisibleControl();

                                        //    UserControl nextControl = propQA.packing_check_mode == "2"
                                        //        ? (UserControl)new userControlPackingPrint(parent) { Dock = DockStyle.Fill, propQA = propQA }
                                        //        : (UserControl)new userControlPackingCheck(parent) { Dock = DockStyle.Fill, propQA = propQA };
                                        //    nextControl.Dock = DockStyle.Fill;

                                        //    SwitchUserControl(nextControl);
                                        //}
                                    }
                                    else
                                    {
                                        MessageBox.Show("ไม่สามารถปลดล็อคได้ กรุณาติดต่อผู้ดูแลระบบ");
                                        return;
                                    }
                                }
                                else
                                {
                                    // ผู้ใช้เลือกไม่ปลดล็อค
                                    return;
                                }
                            }
                            else
                            {
                                // ไม่ใช่ Admin แสดงเฉพาะแจ้งเตือน
                                CustomMsgBoxBase.ShowCustomMessageBox(
                                    message + "ไม่สามารถเข้าใช้งานได้",
                                    "แจ้งเตือน",
                                    CustomMsgBoxBase.MessageBoxIconType.Warning);
                                return;
                            }
                        }
                    }
                    else
                    {
                        // ไม่มีคนใช้งานอยู่
                        if (conQA.InsertReportActive(propQA) == false)
                        {
                            MessageBox.Show("ไม่สามารถเพิ่ม report no กับ IP ได้");
                            return;
                        }

                        propQA.inProcStatus = ((int)ProcStatus.Working).ToString();
                        propQA.reportStatus = ((int)ProcStatus.Working).ToString();

                        if (conQA.UpdateStatus(propQA) == false)
                        {
                            MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Working ได้");
                            return;
                        }
                    }

                    propQA.packing_check_mode = conQA.PackingCheckMode(propQA);

                    if (propQA.process == "Packing_Check")
                    {
                        var parentForm = this.FindForm() as frmMain;
                        parentForm?.VisibleControl();

                        UserControl nextControl = propQA.packing_check_mode == "2"
                            ? (UserControl)new userControlPackingPrint(parent) { Dock = DockStyle.Fill, propQA = propQA }
                            : (UserControl)new userControlPackingCheck(parent) { Dock = DockStyle.Fill, propQA = propQA };
                        nextControl.Dock = DockStyle.Fill;

                        SwitchUserControl(nextControl);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                }
            }
        }

        private void SwitchUserControl(UserControl newControl)
        {
            // ตรวจสอบ UserControl ปัจจุบัน
            var currentControl = this.Controls.OfType<UserControl>().FirstOrDefault();
            if (currentControl != null)
            {
                // ถอด UserControl ปัจจุบันออก
                this.Controls.Remove(currentControl);

                // ปล่อย Mutex สำหรับ Report No. ปัจจุบัน
                if (currentControl is userControlRegular)
                {
                    //ReleaseReportMutex(currentReportNo);
                }
            }

            // แสดง UserControl ใหม่

            this.Controls.Clear();
            newControl.Dock = DockStyle.Fill;
            this.Controls.Add(newControl);
        }

        public void bt_rec_pack_Click()
        {

            userControlSelectPackingCheck usrControlSelectPackingCheck = new userControlSelectPackingCheck();

            usrControlSelectPackingCheck.Dock = DockStyle.Fill;
            usrControlSelectPackingCheck.propQA = new QAdataProperty();

            usrControlSelectPackingCheck.propQA.labelProcess = "Select Report for : " + "packing check";
            usrControlSelectPackingCheck.propQA.process = "Packing_Check";
            usrControlSelectPackingCheck.propQA.prevProcess = "Receive_WH";

            DataTable dt = new DataTable();
            dt = conQA.SearchForOpPackingCheck(usrControlSelectPackingCheck.propQA);
            usrControlSelectPackingCheck.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrControlSelectPackingCheck.propQA.dtgRawMat.DataSource = dt;

            var parentForm = this.FindForm() as frmMain;
            parentForm?.ControlBackLevel(employee);

            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
                //Control[] foundPanels = this.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    // เคลียร์และเพิ่ม UserControl ใหม่

                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrControlSelectPackingCheck);
                    usrControlSelectPackingCheck.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            //this.Controls.Add(usrControlSelectPackingCheck);
            //AddUserControlRequested?.Invoke(usrControlSelectPackingCheck);
        }


    }
}
