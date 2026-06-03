using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.PackingCheck;
using RawMat.Views.RegularCheck;
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
using static RawMat.Views.CustomMsg.CustomMsgBoxBase;

namespace RawMat.Views.RegularCheck
{
    public partial class userControlSelectRegular : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        //public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();
        private userControlRegular parentControl;
        private frmMain parentMain;
        private NetworkInfoCls netInfo = new NetworkInfoCls();
        //private Dictionary<string, Mutex> reportMutexes = new Dictionary<string, Mutex>();
        private IParent parent;

        //public userControlSelectRegular(userControlRegular parent)
        //{
        //    InitializeComponent();
        //    this.parentControl = parent; // เก็บค่า parent ไว้ใช้
        //}

        // Constructor สำหรับ MainForm
        //public userControlSelectRegular(frmMain parent)
        //{
        //    InitializeComponent();
        //    this.parentMain = parent;
        //}

        public userControlSelectRegular( )
        {

            InitializeComponent();
            ConfigureReportGrid();

        }


        private void userControlSelectRegular_Load(object sender, EventArgs e)
        {
            lb_process.Text = propQA.labelProcess.Replace("\n", " ");
            dtg_reportSelect.DataSource = propQA.dtgRawMat.DataSource;

            //dtg_reportSelect.Columns["process_id"].Visible = false;
            //dtg_reportSelect.Columns["Issue_Date"].Visible = false;



        }

        private void ConfigureReportGrid()
        {
            dtg_reportSelect.AutoGenerateColumns = true;
            dtg_reportSelect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dtg_reportSelect.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            dtg_reportSelect.RowHeadersVisible = false;
            dtg_reportSelect.AllowUserToResizeRows = false;
            dtg_reportSelect.MultiSelect = false;
            dtg_reportSelect.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dtg_reportSelect.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dtg_reportSelect.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
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
                    // บันทึก Mutex ใน Dictionary
                    propQA.dtLotNo = new DataTable();
                    propQA.dtLotNo = conQA.ReportLot(propQA);



                    if (dtg_reportSelect.Columns[e.ColumnIndex].Name == "REF")
                    {
                        propQA.REGULAR_CHECK_REF = dtg_reportSelect.Rows[e.RowIndex].Cells["Regular_Check_Ref"].Value.ToString();
                        //ถ้า = 0 แสดงว่าไม่มีการ ref จะเป็นการกด roll นั้นปกติ
                        if (propQA.REGULAR_CHECK_REF != "0")
                        {
                            DataTable dtRef = new DataTable();
                            dtRef = conQA.SearchRegularRef(propQA);
                            if (dtRef.Rows.Count == 0 || dtRef == null)
                            {
                                MessageBox.Show("ยังไม่พบ data ที่จะทำการ Reference ข้อมูล");
                                return;
                            }
                            else
                            {
                                propQA.mRef = dtRef.Rows[0]["M_CODE"].ToString();
                                propQA.mSelect = propQA.M_CODE;
                                propQA.REGULAR_NO_REF = dtRef.Rows[0]["REGULAR_NO"].ToString();
                                DataTable checkConRegularRef = conQA.CheckConditionRegularRef(propQA);
                                if (checkConRegularRef == null)
                                {
                                    return;
                                }

                                if (checkConRegularRef.Rows[0]["mSelect"].ToString() == string.Empty || checkConRegularRef.Rows[0]["mRef"].ToString() == string.Empty)
                                {
                                    MessageBox.Show("ยังไม่พบ data การ Reference ข้อมูลของ M-code : " + propQA.mRef + "กับ M-code " + propQA.mSelect);
                                    return;
                                }

                                if (checkConRegularRef.Rows[0]["Compare_Result"].ToString() == "NOT MATCH")
                                {
                                    MessageBox.Show("ข้อมูลของ M-code : " + propQA.mRef + "กับ M-code " + propQA.mSelect + " ไม่ Match กัน");
                                    return;
                                }

                                bool result = CustomMsgBoxBase.ShowCustomMessageBox(
                                "ต้องการ Ref หมายเลข : " + propQA.REGULAR_NO_REF + " หรือไม่ ",
                                 "ยืนยันการดำเนินการ",
                                CustomMsgBoxBase.MessageBoxIconType.Question,
                                CustomMsgBoxBase.MessageBoxDialogType.YesNo);

                                if (result == false)
                                {
                                    // no
                                    return;
                                }
                                else
                                {
                                    //yes
                                    propQA.inProcStatus = "1";
                                    propQA.reportStatus = "1";
                                    if (conQA.UpdateRegularRef(propQA) == true)
                                    {
                                       

                                        CustomMsgBoxBase.ShowCustomMessageBox(
                                  "Update Reference เรียบร้อย",
                                   "ดำเนินการสำเร็จ",
                                  CustomMsgBoxBase.MessageBoxIconType.OK);

                                        bt_reg_Click();
                                        //MessageBox.Show("Update Reference เรียบร้อย");
                                        return;
                                    }
                                    else
                                    {
                                        //MessageBox.Show("Update Reference ");
                                        return;
                                    }
                                    //key report_NO => update  
                                    //update dtRef[RI]

                                    //update status report = 1 inprocess , 1  reportstatus
                                }

                            }
                            //
                        }
                    }

                    //propQA.Regular_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Regular No"].Value.ToString();
                    //propQA.Report_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                    //propQA.Invoice_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                    //propQA.M_CODE = dtg_reportSelect.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                    //propQA.Material_Name = dtg_reportSelect.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                    //propQA.dtReceiveDate = DateTime.Parse(dtg_reportSelect.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());

                    //propQA.inProcStatus = "2";
                    //propQA.reportStatus = "2";


                    //regular sampling type

                    propQA.dtRegSamp = conQA.RegularSampling(propQA);
                    if (propQA.dtRegSamp == null || propQA.dtRegSamp.Rows.Count == 0)
                    {
                        MessageBox.Show("ไม่พบข้อมูล regular sampling ที่จะนำไปทำการตรวจสอบ", "ไม่พบข้อมูล", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    else
                    {
                        propQA.SAMPLING_TYPE = propQA.dtRegSamp.Rows[0]["sampling_type"].ToString();
                        propQA.SAMPLING_NAME = propQA.dtRegSamp.Rows[0]["sampling_type_name"].ToString().Trim();
                        propQA.CAVITY_QTY = propQA.dtRegSamp.Rows[0]["Cavity_Qty"].ToString();
                        propQA.SAMPLING_QTY = propQA.dtRegSamp.Rows[0]["Sampling_Qty"].ToString();
                        propQA.Cavity_Name_List = new List<string>();

                        propQA.CAVITY_NAME = propQA.dtRegSamp.Rows[0]["Cavity_Name"].ToString();


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
                        else if (propQA.SAMPLING_QTY == "0" || propQA.SAMPLING_QTY == string.Empty)
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
                                    // ให้ผู้ใช้กรอกจำนวน Sampling ของแต่ละ Cavity เองในหน้า Regular Check
                                    propQA.dtCavity.Rows.Add(new object[] { propQA.Cavity_Name_List[i].ToString(), DBNull.Value });
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

                                DataTable dtSampLot = new DataTable();
                                dtSampLot = conQA.FunctionSampQtyLotSize(propQA);

                                if (dtSampLot.Rows.Count == 0)
                                {
                                    MessageBox.Show("ไม่พบข้อมูลการ Sampling Qty จาก " + propQA.SAMPLING_NAME + " ของ m-code :" + propQA.M_CODE);
                                    return;
                                }
                                else
                                {
                                    propQA.SAMPLING_QTY = dtSampLot.Rows[0]["Sampling_Qty"].ToString();
                                }

                                if (Convert.ToInt32(propQA.CAVITY_QTY) != 0)
                                {
                                    for (int i = 0; i < Convert.ToInt32(propQA.CAVITY_QTY); i++)
                                    {
                                        // ให้ผู้ใช้กรอกจำนวน Sampling ของแต่ละ Cavity เองในหน้า Regular Check
                                        propQA.dtCavity.Rows.Add(new object[] { propQA.Cavity_Name_List[i].ToString(), DBNull.Value });
                                    }
                                }
                            }
                            else if (propQA.SAMPLING_TYPE == "2")
                            {

                            }
                            else
                            {
                                MessageBox.Show("ไม่สามารถเข้าไปทำการ Regular ได้ กรุณา check sampling type ของ m-code :" + propQA.M_CODE);
                                return;
                            }

                        }

                    }


                    //regular equipment
                    propQA.dtRegEq = conQA.RegularEquipment(propQA);
                    if (propQA.dtRegEq == null)
                    {
                        return;
                    }

                    if (!propQA.dtRegEq.Columns.Contains("VALUE"))
                    {
                        propQA.dtRegEq.Columns.Add("VALUE", typeof(string));
                    }

                    if (!propQA.dtRegEq.Columns.Contains("POINT_JUDGE"))
                    {
                        propQA.dtRegEq.Columns.Add("POINT_JUDGE", typeof(string));
                    }

                    if (!propQA.dtRegEq.Columns.Contains("TOTAL_JUDGE"))
                    {
                        propQA.dtRegEq.Columns.Add("TOTAL_JUDGE", typeof(string));
                    }
                    //if (conQA.UpdateStatus(propQA) == true)
                    //{

                    // ผูก Event โดยใช้ mutexKey แทน Report_No
                    //usrReg.OnReleaseMutex += () => ReleaseReportMutex(mutexKey);

                    //usrReg.RequestReleaseMutex += (key) => parent.ReleaseReportMutex(key);

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
                                    bt_reg_Click();
                                    return;
         
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

                    userControlRegular usrReg = new userControlRegular()
                    {
                        Dock = DockStyle.Fill,
                        propQA = propQA
                    };

                    SwitchUserControl(usrReg);


                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                   // parentMain.ReleaseReportMutex(mutexKey); // ปล่อย Mutex ถ้ามีข้อผิดพลาด
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

        //private void UserControlRegular_RequestReleaseMutex(string mutexKey)
        //{
        //    // เมื่อได้รับคำขอจาก userControlRegular ให้ปล่อย Mutex
        //    ReleaseReportMutex(mutexKey);
        //}

        private void dtg_reportSelect_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (!dtg_reportSelect.Columns.Contains("Ref"))
            {
                DataGridViewImageColumn refColumn = new DataGridViewImageColumn
                {
                    Name = "REF", // ชื่อของคอลัมน์
                    HeaderText = "Ref", // ข้อความหัวคอลัมน์
                    ReadOnly = false // สามารถแก้ไขได้ (หรือจะตั้งเป็น true ถ้าต้องการให้แก้ไขไม่ได้)
                };
                dtg_reportSelect.Columns.Add(refColumn);
            }

            foreach (DataGridViewRow row in dtg_reportSelect.Rows)
            {
                if (row.Cells["Regular_Check_Ref"].Value.ToString() == "1")
                {
                    row.Cells["REF"].Value = imgCls.ResizeImage(imgCls.LoadAppImage("ref.png"), 24, 24);
                }
                else
                {
                    row.Cells["REF"].Value = imgCls.ResizeImage(imgCls.LoadAppImage("gray.png"), row.Cells["REF"].Size.Width, row.Cells["REF"].Size.Height);
                }
            }

            if (dtg_reportSelect.Columns.Contains("process_status_id"))
            {
                dtg_reportSelect.Columns["process_status_id"].Visible = false;
            }

            if (dtg_reportSelect.Columns.Contains("Issue_Date"))
            {
                dtg_reportSelect.Columns["Issue_Date"].Visible = false;
            }
            //dtg_reportSelect.Columns["LOT_NO"].Visible = false;
            if (dtg_reportSelect.Columns.Contains("Regular_Check_Ref"))
            {
                dtg_reportSelect.Columns["Regular_Check_Ref"].Visible = false;
            }

            ApplyReportGridColumnLayout();
        }

        private void ApplyReportGridColumnLayout()
        {
            dtg_reportSelect.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            foreach (DataGridViewColumn column in dtg_reportSelect.Columns)
            {
                column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                column.MinimumWidth = 70;
                column.FillWeight = 100;
                column.SortMode = DataGridViewColumnSortMode.Automatic;
            }

            SetColumnLayout("REF", 45, 42, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Receive Date", 95, 90, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Regular No", 105, 95, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Report No.", 115, 100, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("M-CODE", 110, 95, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Invoice No.", 120, 105, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Lot Size", 80, 75, DataGridViewContentAlignment.MiddleRight);
            SetColumnLayout("Vendor", 145, 130, DataGridViewContentAlignment.MiddleLeft);
            SetColumnLayout("Material Name", 220, 205, DataGridViewContentAlignment.MiddleLeft);
            SetColumnLayout("Status", 90, 80, DataGridViewContentAlignment.MiddleCenter);
        }

        private void SetColumnLayout(string columnName, float fillWeight, int minimumWidth, DataGridViewContentAlignment alignment)
        {
            if (!dtg_reportSelect.Columns.Contains(columnName))
            {
                return;
            }

            DataGridViewColumn column = dtg_reportSelect.Columns[columnName];
            column.FillWeight = fillWeight;
            column.MinimumWidth = minimumWidth;
            column.DefaultCellStyle.Alignment = alignment;
        }

        public void bt_reg_Click()
        {
            userControlSelectRegular usrConSelectReg = new userControlSelectRegular();

            usrConSelectReg.Dock = DockStyle.Fill;
            usrConSelectReg.propQA = new QAdataProperty();

            usrConSelectReg.propQA.labelProcess = "Select Report for : Regular Check";
            usrConSelectReg.propQA.process = "Regular_Check";
            usrConSelectReg.propQA.prevProcess = "Packing_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpRegular(usrConSelectReg.propQA);
            usrConSelectReg.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectReg.propQA.dtgRawMat.DataSource = dt;
            //AddUserControlRequested?.Invoke(usrConSelectReg);

            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
                //Control[] foundPanels = this.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    // เคลียร์และเพิ่ม UserControl ใหม่
                    panelMain.Controls.Clear();
                    panelMain.Controls.Add(usrConSelectReg);
                    usrConSelectReg.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

    }
}
