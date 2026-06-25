using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.RegularCheck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RawMat.frmMain;
using static RawMat.Property.QAdataProperty;
using RawMat.Views.CustomMsg;

namespace RawMat.Views.InspDataCheck
{
    public partial class userControlSelectInspData : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;

        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();
        private userControlRegular parentControl;
        private frmMain parentMain;
        private NetworkInfoCls netInfo = new NetworkInfoCls();


        public userControlSelectInspData()
        {

            InitializeComponent();
        }

        private void userControlSelectInspData_Load(object sender, EventArgs e)
        {
            lb_process.Text = propQA.labelProcess.Replace("\n", " ");
            dtg_reportSelect.DataSource = propQA.dtgRawMat.DataSource;
            dtg_reportSelect.DataBindingComplete += dtg_reportSelect_DataBindingComplete;
        }

        private void dtg_reportSelect_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dtg_reportSelect.Rows.Count && e.ColumnIndex >= 0 && e.ColumnIndex < dtg_reportSelect.Columns.Count)
            {

                propQA.Regular_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Regular No"].Value.ToString();
                propQA.Report_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                propQA.Invoice_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                propQA.M_CODE = dtg_reportSelect.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                propQA.Material_Name = dtg_reportSelect.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                propQA.dtReceiveDate = DateTime.Parse(dtg_reportSelect.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());
                propQA.Qty = dtg_reportSelect.Rows[e.RowIndex].Cells["Lot Size"].Value.ToString();
                propQA.Vendor_Name = dtg_reportSelect.Rows[e.RowIndex].Cells["Vendor"].Value.ToString();


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
                                    CustomMsgBoxBase.MessageBoxDialogType.YesNo); // มี Yes/No

                                if (result == true)
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

                                        bt_data_Click();
                                        return;
                                        //// Insert ข้อมูลใหม่สำหรับผู้ใช้งานปัจจุบัน
                                        //if (conQA.InsertReportActive(propQA) == false)
                                        //{
                                        //    MessageBox.Show("ไม่สามารถเพิ่ม report no กับ IP ได้");
                                        //    return;
                                        //}

                                        //// ไปยังหน้าถัดไป
                                        //var parentForm = this.FindForm() as frmMain;
                                        //parentForm?.VisibleControl();

                                        //userControlInspData InspData = new userControlInspData()
                                        //{
                                        //    Dock = DockStyle.Fill,
                                        //    propQA = propQA
                                        //};

                                        //SwitchUserControl(InspData);
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

                    // กรณีที่สามารถเข้าใช้งานได้
                    var parentFormSuccess = this.FindForm() as frmMain;
                    parentFormSuccess?.VisibleControl();

                    userControlInspData InspDataSuccess = new userControlInspData()
                    {
                        Dock = DockStyle.Fill,
                        propQA = propQA
                    };

                    SwitchUserControl(InspDataSuccess);
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
                if (currentControl is userControlInspData)
                {
                    //ReleaseReportMutex(currentReportNo);
                }
            }

            // แสดง UserControl ใหม่

            this.Controls.Clear();
            newControl.Dock = DockStyle.Fill;
            this.Controls.Add(newControl);
            
        }

        private void dtg_reportSelect_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {

            dtg_reportSelect.Columns["process_status_id"].Visible = false;
            dtg_reportSelect.Columns["Issue_Date"].Visible = false;
            //dtg_reportSelect.Columns["LOT_NO"].Visible = false;
        }

        public void bt_data_Click()
        {
            userControlSelectInspData usrConSelectData = new userControlSelectInspData();

            usrConSelectData.Dock = DockStyle.Fill;
            usrConSelectData.propQA = new QAdataProperty();

            usrConSelectData.propQA.labelProcess = "Select Report for : Inspection Data Check";
            usrConSelectData.propQA.process = "Inspection_Data_Check";
            usrConSelectData.propQA.prevProcess = "Regular_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpData(usrConSelectData.propQA);
            usrConSelectData.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectData.propQA.dtgRawMat.DataSource = dt;

            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
                //Control[] foundPanels = this.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    // เคลียร์และเพิ่ม UserControl ใหม่
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrConSelectData);
                    usrConSelectData.BringToFront();
                    
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

    }

}

