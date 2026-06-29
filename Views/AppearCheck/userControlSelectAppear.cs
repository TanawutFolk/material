using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.PackingCheck;
using RawMat.Views.RegularCheck;
using RawMat.Views.FunctionCheck;
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
using static RawMat.Property.QAdataProperty;
using RawMat.Views.CustomMsg;

namespace RawMat.Views.AppearCheck
{
    public partial class userControlSelectAppear : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        //public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();
        private userControlAppear parentControl;
        private frmMain parentMain;
        private NetworkInfoCls netInfo = new NetworkInfoCls();
        //private Dictionary<string, Mutex> reportMutexes = new Dictionary<string, Mutex>();
        private IParent parent;

        public userControlSelectAppear()
        {

            InitializeComponent();
            ConfigureReportGrid();

        }


        private void userControlSelectAppear_Load(object sender, EventArgs e)
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

                propQA.Report_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                propQA.Invoice_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                propQA.M_CODE = dtg_reportSelect.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                propQA.Material_Name = dtg_reportSelect.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                propQA.dtReceiveDate = DateTime.Parse(dtg_reportSelect.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());
                propQA.inspQty = dtg_reportSelect.Rows[e.RowIndex].Cells["Inspection Qty"].Value.ToString();
                propQA.Qty = dtg_reportSelect.Rows[e.RowIndex].Cells["Lot Size"].Value.ToString();
                propQA.Lot_No = string.Empty;
                if (dtg_reportSelect.Columns.Contains("Lot No.") && dtg_reportSelect.Rows[e.RowIndex].Cells["Lot No."].Value != null)
                {
                    propQA.Lot_No = dtg_reportSelect.Rows[e.RowIndex].Cells["Lot No."].Value.ToString();
                }


                try
                {

                    //regular sampling type
                    propQA.dtLotNo = conQA.ReportLot(propQA);

                    propQA.dtAppSamp = conQA.AppearSampling(propQA);
                    if (propQA.dtAppSamp == null || propQA.dtAppSamp.Rows.Count == 0)
                    {
                        MessageBox.Show("ไม่พบการ Sampling ที่จะนำไปใช้ในหน้า Appearance Check ");
                        return;
                    }
                    else 
                    {
                        propQA.SAMPLING_TYPE = propQA.dtAppSamp.Rows[0]["sampling_type"].ToString();
                        propQA.SAMPLING_NAME = propQA.dtAppSamp.Rows[0]["sampling_type_name"].ToString().Trim();
                        propQA.CAVITY_QTY = propQA.dtAppSamp.Rows[0]["Cavity_Qty"].ToString();
                        propQA.SAMPLING_QTY = propQA.dtAppSamp.Rows[0]["Sampling_Qty"].ToString();
                        propQA.Allow_Continue = propQA.dtAppSamp.Rows[0]["Allow_Continue"].ToString();

                        propQA.Cavity_Name_List = new List<string>();

                        propQA.CAVITY_NAME = propQA.dtAppSamp.Rows[0]["Cavity_Name"].ToString();
                        int.TryParse(propQA.CAVITY_QTY?.Trim(), out int cavityQty);
                        bool hasCavity = cavityQty > 0;


                        if (propQA.SAMPLING_TYPE == "4" && !hasCavity)
                        {
                            MessageBox.Show("ต้องมีการ Setting จำนวน Cavity ของ M-CODE : " + propQA.M_CODE);
                            return;
                        }
                        else if (propQA.SAMPLING_TYPE == "4" &&
                                 (string.IsNullOrWhiteSpace(propQA.CAVITY_NAME) || propQA.CAVITY_NAME == "0"))
                        {
                            MessageBox.Show("ต้องมีการ Setting จำนวน Cavity_Name ของ M-CODE : " + propQA.M_CODE);
                            return;
                        }
                        else if ((propQA.SAMPLING_TYPE != "3" && propQA.SAMPLING_TYPE != "1") && (propQA.SAMPLING_QTY == "0" || propQA.SAMPLING_QTY == string.Empty))
                        {
                            MessageBox.Show("ต้องมีการ Setting จำนวน Sampling อย่างน้อย 1 ตัว ของ M-CODE : " + propQA.M_CODE);
                            return;
                        }
                        else if (propQA.SAMPLING_TYPE == "2" && hasCavity)
                        {
                            MessageBox.Show("ต้องไม่มีการ Setting จำนวน Cavity ของ M-CODE : " + propQA.M_CODE);
                            return;
                        }
                        else
                        {
                            // ok 
                            //dtg 
                            if (propQA.SAMPLING_TYPE == "1")
                            {

                            }
                            else if (propQA.SAMPLING_TYPE == "3")
                            {

                            }
                            else if (propQA.SAMPLING_TYPE == "5")
                            {
                                //ใช้อันที่หยิบมา
                            }
                            else
                            {
                                MessageBox.Show("ไม่สามารถเข้าไปทำการ Appearance ได้ กรุณา check sampling type ของ m-code :" + propQA.M_CODE);
                                return;
                            }

                        }

                    }

                    var parentForm = this.FindForm() as frmMain;
                    parentForm?.VisibleControl();

                    userControlAppear usrAppear = new userControlAppear()
                    {
                        Dock = DockStyle.Fill,
                        propQA = propQA
                    };

                    SwitchUserControl(usrAppear);
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
                if (currentControl is userControlAppear)
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


            if (dtg_reportSelect.Columns.Contains("process_status_id"))
            {
                dtg_reportSelect.Columns["process_status_id"].Visible = false;
            }

            if (dtg_reportSelect.Columns.Contains("Issue_Date"))
            {
                dtg_reportSelect.Columns["Issue_Date"].Visible = false;
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

            SetColumnLayout("Receive Date", 95, 90, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Report No.", 115, 100, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("M-CODE", 110, 95, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Invoice No.", 120, 105, DataGridViewContentAlignment.MiddleCenter);
            SetColumnLayout("Lot Size", 80, 75, DataGridViewContentAlignment.MiddleRight);
            SetColumnLayout("Inspection Qty", 95, 90, DataGridViewContentAlignment.MiddleRight);
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

        public void bt_Appear_Click()
        {
            userControlSelectAppear usrConSelectAppear = new userControlSelectAppear();

            usrConSelectAppear.Dock = DockStyle.Fill;
            usrConSelectAppear.propQA = new QAdataProperty();

            usrConSelectAppear.propQA.labelProcess = "Select Report for : Appearance Check";
            usrConSelectAppear.propQA.process = "Appearance_Check";
            usrConSelectAppear.propQA.prevProcess = "Inspection_Data_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpAppear(usrConSelectAppear.propQA);
            usrConSelectAppear.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectAppear.propQA.dtgRawMat.DataSource = dt;
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
                    panelMain.Controls.Add(usrConSelectAppear);
                    usrConSelectAppear.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

    }
}
