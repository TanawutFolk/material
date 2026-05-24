using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.PackingCheck;
using RawMat.Views.RegularCheck;
using RawMat.Views.DimensionCheck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using static RawMat.frmMain;
using RawMat.Views.CustomMsg;
using static RawMat.Property.QAdataProperty;
using MySqlX.XDevAPI.Common;
using static RawMat.Views.CustomMsg.CustomMsgBoxBase;

namespace RawMat.Views.DimensionCheck
{
    public partial class userControlSelectDimension : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        //public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();
        private userControlDimension parentControl;
        private frmMain parentMain;
        private NetworkInfoCls netInfo = new NetworkInfoCls();
        //private Dictionary<string, Mutex> reportMutexes = new Dictionary<string, Mutex>();
        private IParent parent;

        //public userControlSelectDimension(userControlDimension parent)
        //{
        //    InitializeComponent();
        //    this.parentControl = parent; // เก็บค่า parent ไว้ใช้
        //}

        // Constructor สำหรับ MainForm
        //public userControlSelectDimension(frmMain parent)
        //{
        //    InitializeComponent();
        //    this.parentMain = parent;
        //}

        public userControlSelectDimension()
        {

            InitializeComponent();

        }

        private int GetCavitySamplingQty(int totalSamplingQty, int cavityQty, int cavityIndex)
        {
            if (cavityQty <= 0)
            {
                return 0;
            }

            int baseQty = totalSamplingQty / cavityQty;
            int remainder = totalSamplingQty % cavityQty;

            return baseQty + (cavityIndex < remainder ? 1 : 0);
        }


        private void userControlSelectDimension_Load(object sender, EventArgs e)
        {
            lb_process.Text = propQA.labelProcess.Replace("\n", " ");
            dtg_reportSelect.DataSource = propQA.dtgRawMat.DataSource;
            dtg_reportSelect.DataBindingComplete += dtg_reportSelect_DataBindingComplete;

            //dtg_reportSelect.Columns["process_id"].Visible = false;
            //dtg_reportSelect.Columns["Issue_Date"].Visible = false;



        }

        private void dtg_reportSelect_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dtg_reportSelect.Rows.Count && e.ColumnIndex >= 0 && e.ColumnIndex < dtg_reportSelect.Columns.Count)
            {

                propQA.Report_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                propQA.Invoice_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                propQA.M_CODE = dtg_reportSelect.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                propQA.Material_Name = dtg_reportSelect.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                propQA.dtReceiveDate = DateTime.Parse(dtg_reportSelect.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());
                //propQA.Lot_No = dtg_reportSelect.Rows[e.RowIndex].Cells["LOT_NO"].Value.ToString();
                propQA.Qty = dtg_reportSelect.Rows[e.RowIndex].Cells["Lot Size"].Value.ToString();

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
                //if (dtg_reportSelect.Rows[e.RowIndex].Cells["process_id"].Value.ToString() == "2" && (employee.EMP_LEVEL == "1" || employee.EMP_LEVEL == "2" || employee.EMP_LEVEL == "4"))
                //{

                //    bool result = CustomMsgBoxBase.ShowCustomMessageBox(
                //         "มีคนทำงานอยู่คุณต้องการเปลี่ยนสถานะงานนี้เป็น UNFINISHED/ยังไม่เสร็จ หรือไม่?",
                //             "ยืนยันการดำเนินการ",
                //        CustomMsgBoxBase.MessageBoxIconType.Question,
                //        CustomMsgBoxBase.MessageBoxDialogType.YesNo);

                //    if (result == true)
                //    {
                //        // update status UNFINISHED
                //        propQA.inProcStatus = ((int)ProcStatus.Unfinished).ToString();
                //        propQA.reportStatus = ((int)ProcStatus.Unfinished).ToString();

                //        if (conQA.UpdateStatus(propQA) == false)
                //        {
                //            MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Working ได้");
                //        }

                //        bt_reg_Click();
                //        return;
                //    }
                //    else
                //    {
                //        return;
                //    }


                //}
                //else if (dtg_reportSelect.Rows[e.RowIndex].Cells["process_id"].Value.ToString() == "2" && (employee.EMP_LEVEL == "3"))
                //{
                //    CustomMsgBoxBase.ShowCustomMessageBox(
                //              "มีคนทำงานอยู่ ไม่สามารถดำเนินการได้",
                //               "ยืนยันการดำเนินการ",
                //              CustomMsgBoxBase.MessageBoxIconType.OK);
                //    return;
                //}
                //else
                //{
                //    //ดำเนินการ working
                //    propQA.inProcStatus = ((int)ProcStatus.Working).ToString();
                //    propQA.reportStatus = ((int)ProcStatus.Working).ToString();

                //    if (conQA.UpdateStatus(propQA) == false)
                //    {
                //        MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Working ได้");
                //    }
                //}




                // สร้าง Mutex Name แบบ Unique (ใช้ Report No. และ Process Name)

                //string mutexKey = $"Global\\ReportLock_{propQA.Report_No}_{propQA.process}";


                //if (parent == null)
                //{
                //    MessageBox.Show("ไม่สามารถสร้างระบบป้องกันการถูกแก้ไขได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    return;
                //}

                // เรียก frmMain เพื่อสร้าง Mutex
                //if (!parent.TryCreateMutex(mutexKey, out Mutex _))
                //{
                //    MessageBox.Show("รายงานนี้กำลังถูกแก้ไขโดยผู้ใช้อื่น");
                //    return;
                //}




                try
                {
                    // บันทึก Mutex ใน Dictionary
                    propQA.dtLotNo = new DataTable();
                    propQA.dtLotNo = conQA.ReportLot(propQA);

                    //propQA.Dimension_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Dimension No"].Value.ToString();
                    //propQA.Report_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                    //propQA.Invoice_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                    //propQA.M_CODE = dtg_reportSelect.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                    //propQA.Material_Name = dtg_reportSelect.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                    //propQA.dtReceiveDate = DateTime.Parse(dtg_reportSelect.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());

                    //propQA.inProcStatus = "2";
                    //propQA.reportStatus = "2";


                    //Dimension sampling type

                    propQA.dtDimSamp = conQA.DimensionSampling(propQA);
                    if (propQA.dtDimSamp == null)
                    {
                        return;
                    }
                    else
                    {
                        propQA.SAMPLING_TYPE = propQA.dtDimSamp.Rows[0]["sampling_type"].ToString();
                        propQA.SAMPLING_NAME = propQA.dtDimSamp.Rows[0]["sampling_type_name"].ToString().Trim();
                        propQA.CAVITY_QTY = propQA.dtDimSamp.Rows[0]["Cavity_Qty"].ToString();
                        propQA.SAMPLING_QTY = propQA.dtDimSamp.Rows[0]["Sampling_Qty"].ToString();
                        propQA.Cavity_Name_List = new List<string>();

                        propQA.CAVITY_NAME = propQA.dtDimSamp.Rows[0]["Cavity_Name"].ToString();


                        if (propQA.SAMPLING_TYPE == "4" && (propQA.CAVITY_QTY == "0" || propQA.CAVITY_QTY == string.Empty))
                        {
                            MessageBox.Show("ต้องมีการ Setting จำนวน Cavity ของ M-CODE : " + propQA.M_CODE);
                            return;
                        }
                        else if (propQA.SAMPLING_TYPE == "4" && (propQA.CAVITY_NAME == "0" || propQA.CAVITY_QTY == string.Empty))
                        {
                            MessageBox.Show("ต้องมีการ Setting จำนวน Cavity_Name ของ M-CODE : " + propQA.M_CODE);
                            return;
                        }
                        else if ((propQA.SAMPLING_TYPE != "3") && (propQA.SAMPLING_QTY == "0" || propQA.SAMPLING_QTY == string.Empty))
                        {
                            MessageBox.Show("ต้องมีการ Setting จำนวน Sampling อย่างน้อย 1 ตัว ของ M-CODE : " + propQA.M_CODE);
                            return;
                        }
                        else if (propQA.SAMPLING_TYPE == "2" && (propQA.CAVITY_QTY != "0"))
                        {
                            MessageBox.Show("ต้องไม่มีการ Setting จำนวน Cavity ของ M-CODE : " + propQA.M_CODE);
                            return;
                        }

                        else
                        {
                            // ok 
                            //dtg 
                            if (propQA.SAMPLING_TYPE == "4")
                            {
                                propQA.Cavity_Name_List = propQA.CAVITY_NAME.Split(',').ToList();

                                propQA.dtCavity = new DataTable();
                                if (!propQA.dtCavity.Columns.Contains("CAVITY_NAME"))
                                {

                                    propQA.dtCavity.Columns.Add("CAVITY_NAME", typeof(string));


                                }

                                if (!propQA.dtCavity.Columns.Contains("SAMPLING_QTY"))
                                {

                                    propQA.dtCavity.Columns.Add("SAMPLING_QTY", typeof(int));


                                }

                                for (int i = 0; i < Convert.ToInt32(propQA.CAVITY_QTY); i++)
                                {
                                    // เพิ่มข้อมูลทั้ง 2 คอลัมน์ในแถวเดียวกัน
                                    propQA.dtCavity.Rows.Add(new object[] { propQA.Cavity_Name_List[i].ToString(), propQA.SAMPLING_QTY });
                                }


                            }
                            else if (propQA.SAMPLING_TYPE == "3")
                            {

                                propQA.Cavity_Name_List = propQA.CAVITY_NAME.Split(',').ToList();

                                propQA.dtCavity = new DataTable();
                                if (!propQA.dtCavity.Columns.Contains("CAVITY_NAME"))
                                {

                                    propQA.dtCavity.Columns.Add("CAVITY_NAME", typeof(string));


                                }

                                if (!propQA.dtCavity.Columns.Contains("SAMPLING_QTY"))
                                {

                                    propQA.dtCavity.Columns.Add("SAMPLING_QTY", typeof(int));


                                }

                                //คำนวณ Strictness Table sampling  จาก lotsize 
                                DataTable dtSampLot = new DataTable();
                                dtSampLot = conQA.DimensionSampQtyLotSize(propQA);

                                if (dtSampLot.Rows.Count == 0)
                                {
                                    MessageBox.Show("ไม่พบข้อมูลการ Sampling Qty จาก " + propQA.SAMPLING_NAME + " ของ m-code :" + propQA.M_CODE);
                                    return;
                                }
                                else
                                {
                                    //Cavity ≥ 1 pc , น่าจะนำ Sampling_Qty  setting เป็น 1 / Cavity ≥2  น่าจะนำ Sampling_Qty  setting เป็น 2 แล้วนำมา คูณกับ data ที่ไปวนในตาราง AQL
                                    propQA.SAMPLING_QTY = dtSampLot.Rows[0]["Sampling_Qty"].ToString();
                                }

                                if(Convert.ToInt32(propQA.CAVITY_QTY) != 0)
                                {
                                    int sampCavity = Convert.ToInt32(propQA.CAVITY_QTY) * Convert.ToInt32(propQA.dtDimSamp.Rows[0]["Sampling_Qty"].ToString());

                                    if (Convert.ToInt32(propQA.SAMPLING_QTY) <= sampCavity)
                                    {
                                        propQA.SAMPLING_QTY = sampCavity.ToString();
                                    }

                                    for (int i = 0; i < Convert.ToInt32(propQA.CAVITY_QTY); i++)
                                    {
                                        int cavitySamplingQty = GetCavitySamplingQty(
                                            Convert.ToInt32(propQA.SAMPLING_QTY),
                                            Convert.ToInt32(propQA.CAVITY_QTY),
                                            i);

                                        // เพิ่มข้อมูลทั้ง 2 คอลัมน์ในแถวเดียวกัน
                                        propQA.dtCavity.Rows.Add(new object[] { propQA.Cavity_Name_List[i].ToString(), cavitySamplingQty });
                                    }

                                }

                            }
                            else if (propQA.SAMPLING_TYPE == "2")
                            {
                               //ใช้อันที่หยิบมา
                            }
                            else
                            {
                                MessageBox.Show("ไม่สามารถเข้าไปทำการ Dimension ได้ กรุณา check sampling type ของ m-code :" + propQA.M_CODE);
                                return;
                            }

                        }

                    }


                    //Dimension equipment
                    propQA.dtDimEq = conQA.DimensionEquipment(propQA);
                    if (propQA.dtDimEq == null)
                    {
                        return;
                    }

                    if (!propQA.dtDimEq.Columns.Contains("VALUE"))
                    {
                        propQA.dtDimEq.Columns.Add("VALUE", typeof(string));
                    }

                    if (!propQA.dtDimEq.Columns.Contains("POINT_JUDGE"))
                    {
                        propQA.dtDimEq.Columns.Add("POINT_JUDGE", typeof(string));
                    }

                    if (!propQA.dtDimEq.Columns.Contains("TOTAL_JUDGE"))
                    {
                        propQA.dtDimEq.Columns.Add("TOTAL_JUDGE", typeof(string));
                    }
                    //if (conQA.UpdateStatus(propQA) == true)
                    //{
     


                    // ผูก Event โดยใช้ mutexKey แทน Report_No
                    //usrReg.OnReleaseMutex += () => ReleaseReportMutex(mutexKey);

                    //usrDim.RequestReleaseMutex += (key) => parent.ReleaseReportMutex(key);

                    //this.Controls.Clear();
                    //this.Controls.Add(usrReg);


                    // in cell_click
                    if (string.IsNullOrWhiteSpace(propQA.reportIP))
                    {
                        //

                        if (conQA.InsertReportActive(propQA) == false)
                        {
                            MessageBox.Show("ไม่สามารถเพิ่ม report no กับ IP ได้");
                            return;
                        }


                        //update ==> working
                        //ดำเนินการ working
                        propQA.inProcStatus = ((int)ProcStatus.Working).ToString();
                        propQA.reportStatus = ((int)ProcStatus.Working).ToString();

                        if (conQA.UpdateStatus(propQA) == false)
                        {
                            MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Working ได้");
                            return;
                        }

                    }
                    else if (propQA.myIPv4 == propQA.reportIP)
                    {
                        //update ==> working
                        //ดำเนินการ working
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

                        // ตรวจสอบว่าเป็น Admin หรือไม่ (EMP_LEVEL == 1 หรือ 2)
                        if (employee.EMP_LEVEL == "1" || employee.EMP_LEVEL == "2")
                        {
                            message = "ต้องการปลดล็อคหรือไม่?";
                            bool result = CustomMsgBoxBase.ShowCustomMessageBox(
                                message,
                                "แจ้งเตือน",
                                CustomMsgBoxBase.MessageBoxIconType.Question,
                                MessageBoxDialogType.YesNo); // มี Yes/No

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

                                    bt_dim_Click();
                                    return;
                                    //// Insert ข้อมูลใหม่สำหรับผู้ใช้งานปัจจุบัน
                                    //if (conQA.InsertReportActive(propQA) == false)
                                    //{
                                    //    MessageBox.Show("ไม่สามารถเพิ่ม report no กับ IP ได้");
                                    //    return;
                                    //}

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
                    }


                    var parentForm = this.FindForm() as frmMain;
                    parentForm?.VisibleControl();

                    userControlDimension usrDim = new userControlDimension(parent)
                    {
                        Dock = DockStyle.Fill,
                        propQA = propQA
                    };

                    SwitchUserControl(usrDim);
                    //}
                    //else
                    //{
                    //    return;
                    //}


                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                    //parentMain.ReleaseReportMutex(mutexKey); // ปล่อย Mutex ถ้ามีข้อผิดพลาด
                }
                //finally
                //{
                //    if (!reportMutexes.ContainsKey(mutexKey))
                //    {
                //        reportMutexes[mutexKey] = mutex;  // เก็บ Mutex ไว้เพื่อไม่ให้ถูกปล่อย
                //    }
                //}
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
                if (currentControl is userControlDimension)
                {
                    //ReleaseReportMutex(currentReportNo);
                }
            }

            // แสดง UserControl ใหม่
            
            this.Controls.Clear();
            newControl.Dock = DockStyle.Fill;
            this.Controls.Add(newControl);
        }

        //private void ReleaseReportMutex(string mutexKey)
        //{
        //    if (reportMutexes.ContainsKey(mutexKey))
        //    {
        //        try
        //        {
        //            reportMutexes[mutexKey].ReleaseMutex();
        //            reportMutexes[mutexKey].Dispose();
        //        }
        //        catch (ApplicationException)
        //        {
        //            // Mutex ถูกปล่อยไปแล้ว
        //        }
        //        reportMutexes.Remove(mutexKey);
        //    }
        //}

        //private void ReleaseReportMutex(string mutexKey)
        //{
        //    parentMain?.ReleaseReportMutex(mutexKey);
        //}

        //private void UserControlDimension_RequestReleaseMutex(string mutexKey)
        //{
        //    // เมื่อได้รับคำขอจาก userControlDimension ให้ปล่อย Mutex
        //    ReleaseReportMutex(mutexKey);
        //}

        private void dtg_reportSelect_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            //if (!dtg_reportSelect.Columns.Contains("Ref"))
            //{
            //    DataGridViewImageColumn refColumn = new DataGridViewImageColumn
            //    {
            //        Name = "REF", // ชื่อของคอลัมน์
            //        HeaderText = "Ref", // ข้อความหัวคอลัมน์
            //        ReadOnly = false // สามารถแก้ไขได้ (หรือจะตั้งเป็น true ถ้าต้องการให้แก้ไขไม่ได้)
            //    };
            //    dtg_reportSelect.Columns.Add(refColumn);
            //}

            //foreach (DataGridViewRow row in dtg_reportSelect.Rows)
            //{
            //    if (row.Cells["Dimension_Check_Ref"].Value.ToString() == "1")
            //    {
            //        row.Cells["REF"].Value = imgCls.ResizeImage(Image.FromFile("img/ref.png"), 24, 24);
            //    }
            //    else
            //    {
            //        row.Cells["REF"].Value = imgCls.ResizeImage(Image.FromFile("img/gray.png"), row.Cells["REF"].Size.Width, row.Cells["REF"].Size.Height);
            //    }
            //}

            dtg_reportSelect.Columns["process_status_id"].Visible = false;
            dtg_reportSelect.Columns["Issue_Date"].Visible = false;
            //dtg_reportSelect.Columns["LOT_NO"].Visible = false;
            //dtg_reportSelect.Columns["Dimension_Check_Ref"].Visible = false;
        }

        public void bt_dim_Click()
        {
            userControlSelectDimension usrConSelectDim = new userControlSelectDimension();

            usrConSelectDim.Dock = DockStyle.Fill;
            usrConSelectDim.propQA = new QAdataProperty();

            usrConSelectDim.propQA.labelProcess = "Select Report for : Dimension Check";
            usrConSelectDim.propQA.process = "Dimension_Check";
            usrConSelectDim.propQA.prevProcess = "Inspection_Data_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpDimension(usrConSelectDim.propQA);
            usrConSelectDim.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectDim.propQA.dtgRawMat.DataSource = dt;

            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
                //Control[] foundPanels = this.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    // เคลียร์และเพิ่ม UserControl ใหม่
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrConSelectDim);
                    usrConSelectDim.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

    }
}
