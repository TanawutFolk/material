using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Net;
using System.Diagnostics;
using RawMat.Login;
using RawMat.Property;
using RawMat.Views;
using RawMat.Views.Main;
using RawMat.Views.Menu;
using RawMat.Views.RegularCheck;
using RawMat.Views.PackingCheck;
using RawMat.Views.FunctionCheck;
using Bunifu.Framework.UI;
using RawMat.Controllers;
using System.Net.Sockets;
using RawMat.Views.ReceiveWH;
using System.Reflection.Emit;
using System.Diagnostics.Eventing.Reader;
using System.IO.Pipes;
using System.Threading;
using RawMat.Utilities;
using RawMat.Views.CustomMsg;
using static RawMat.Property.QAdataProperty;
using RawMat.Views.DimensionCheck;
using RawMat.Views.InspDataCheck;
using RawMat.Views.AppearCheck;
using RawMat.Views.ReceiveMat;
using RawMat.Views.Setting;
namespace RawMat
{
    public partial class frmMain : Form 
    {
        private INavigationService _navigationService;
        public EmployeeProperty empProp = new EmployeeProperty();
        public EmployeeController empCon = new EmployeeController();
        public QAdataControllers qaCon = new QAdataControllers();
        public QAdataProperty qaProp = new QAdataProperty();

        private BackgroundWorker bgWorker = new BackgroundWorker();
        private Random random = new Random(); // สร้างออบเจกต์ Random
        private bool isTestMode = true; // กำหนดว่าจะเป็นโหมดทดสอบหรือโหมดจริง
        public Dictionary<string, Mutex> reportMutexes = new Dictionary<string, Mutex>();
       

        public frmMain()
        {
            InitializeComponent();
            ConfigureHeaderStatusButtons();
            _navigationService = new NavigationService(panelMain); // สร้าง NavigationService
            LoadLoginControl();

            //for test
            //ProcStatus status;

            ////qaProp.inProcStatus = "1";
            ////qaProp.inProcStatus = "6";
            //qaProp.inProcStatus = "99";

            //bool parsed = int.TryParse(qaProp.inProcStatus, out int statusId) && Enum.IsDefined(typeof(ProcStatus), statusId);
            //status = parsed ? (ProcStatus)statusId : ProcStatus.NG; // ค่าเริ่มต้นเป็น NG ถ้าแปลงไม่ได้

            //switch (status)
            //{
            //    case ProcStatus.OK:
            //        CustomMsgBoxBase.ShowCustomMessageBox(
            //            "Record Regular งาน OK เรียบร้อยแล้ว",
            //            "สำเร็จ",
            //            CustomMsgBoxBase.MessageBoxIconType.OK);
            //        break;
            //    case ProcStatus.Pending:
            //        CustomMsgBoxBase.ShowCustomMessageBox(
            //            "Record Regular พบงาน ถูก PENDING",
            //            "สำเร็จ",
            //            CustomMsgBoxBase.MessageBoxIconType.Pending);
            //        break;
            //    default:
            //        CustomMsgBoxBase.ShowCustomMessageBox(
            //                $"Report No. : QA25-0000x นี้" + "\n" + "path ที่ไม่พบ : " + "\\\\192.168.2.100\\12_qa\\01_Material\\01_Improvement project\\03_Pictures for program\\สิ่งที่ต้องเตรียม\"",
            //                "กรุณาตรวจสอบ!", CustomMsgBoxBase.MessageBoxIconType.NG, fontSize: 24f);
            //        break;
            //}

        }

        private void ConfigureHeaderStatusButtons()
        {
            BunifuFlatButton[] statusButtons =
            {
                bt_status_rec_pending,
                bt_status_packing_check_pending,
                bt_status_regular_pending,
                bt_status_data_pending,
                bt_status_function_pending,
                bt_status_dimension_pending,
                bt_appear_pending
            };

            const int leftMargin = 3;
            const int topMargin = 4;
            const int buttonWidth = 150;
            const int buttonHeight = 100;

            for (int i = 0; i < statusButtons.Length; i++)
            {
                statusButtons[i].Location = new Point(leftMargin + (buttonWidth * i), topMargin);
                statusButtons[i].Size = new Size(buttonWidth, buttonHeight);
                statusButtons[i].Margin = Padding.Empty;
                statusButtons[i].TextAlign = ContentAlignment.MiddleCenter;
                statusButtons[i].IconVisible = false;
                statusButtons[i].IconRightVisible = false;
                statusButtons[i].IconMarginLeft = 0;
                statusButtons[i].IconMarginRight = 0;
            }
        }

        private void AddUserControlAdmin()
        {

            //panelMain.Controls.Clear();
            //panelMenu.Controls.Clear();

            bt_setting.Visible = true;

            userControlWH usrConWH = new userControlWH();

            usrConWH.AddUserControlRequested += AddUserControlToPanel;
            usrConWH.Dock = DockStyle.Fill;
            panelWH.Controls.Add(usrConWH);

            userControlMenu usrMenu = new userControlMenu();

            ReorderButtons(usrMenu);
            usrMenu.AddUserControlRequested += AddUserControlToPanel;
            usrMenu.Dock = DockStyle.Fill;
            panelMenu.Controls.Add(usrMenu);

            panelMainHome();

        }

        private void AddUserControlFullOperator()
        {

            //panelMain.Controls.Clear();
            //panelMenu.Controls.Clear();

            bt_setting.Visible = false;
            bt_home.Visible = false;

            userControlWH usrConWH = new userControlWH();

            usrConWH.AddUserControlRequested += AddUserControlToPanel;
            usrConWH.Dock = DockStyle.Fill;
            panelWH.Controls.Add(usrConWH);

            userControlMenu usrMenu = new userControlMenu();

            ReorderButtons(usrMenu);
            usrMenu.AddUserControlRequested += AddUserControlToPanel;
            usrMenu.Dock = DockStyle.Fill;
            panelMenu.Controls.Add(usrMenu);

            panelMainHome();

        }

        private void AddUserControlRecWHOperator()
        {

            //panelMain.Controls.Clear();
            //panelMenu.Controls.Clear();

            bt_setting.Visible = false;
            bt_home.Visible = false;

            userControlWH usrConWH = new userControlWH();

            usrConWH.AddUserControlRequested += AddUserControlToPanel;
            usrConWH.Dock = DockStyle.Fill;
            panelWH.Controls.Add(usrConWH);


        }


        private void AddUserControlInspOperator()
        {

            //panelMain.Controls.Clear();
            //panelMenu.Controls.Clear();

            bt_setting.Visible = false;
            bt_home.Visible = false;

            userControlMenu usrMenu = new userControlMenu();

            ReorderButtons(usrMenu);
            usrMenu.AddUserControlRequested += AddUserControlToPanel;
            usrMenu.Dock = DockStyle.Fill;
            panelMenu.Controls.Add(usrMenu);
        }

        private void AddUserControlAdminBack()
        {
            bt_setting.Visible = true;
            LoadWHAndMenuPanels();
        }

        private void AddUserControlFullOperatorBack()
        {
            bt_setting.Visible = false;
            bt_home.Visible = false;
            LoadWHAndMenuPanels();
        }

        private void AddUserControlRecWHOperatorBack()
        {
            bt_setting.Visible = false;
            bt_home.Visible = false;

            userControlWH usrConWH = new userControlWH();
            usrConWH.AddUserControlRequested += AddUserControlToPanel;
            usrConWH.Dock = DockStyle.Fill;
            panelWH.Controls.Add(usrConWH);
        }

        private void AddUserControlInspOperatorBack()
        {
            bt_setting.Visible = false;
            bt_home.Visible = false;

            userControlMenu usrMenu = new userControlMenu();
            ReorderButtons(usrMenu);
            usrMenu.AddUserControlRequested += AddUserControlToPanel;
            usrMenu.Dock = DockStyle.Fill;
            panelMenu.Controls.Add(usrMenu);
        }

        private void LoadWHAndMenuPanels()
        {
            userControlWH usrConWH = new userControlWH();
            usrConWH.AddUserControlRequested += AddUserControlToPanel;
            usrConWH.Dock = DockStyle.Fill;
            panelWH.Controls.Add(usrConWH);

            userControlMenu usrMenu = new userControlMenu();
            ReorderButtons(usrMenu);
            usrMenu.AddUserControlRequested += AddUserControlToPanel;
            usrMenu.Dock = DockStyle.Fill;
            panelMenu.Controls.Add(usrMenu);
        }

        private void AddUserControlGuest()
        {

            panelMenu.Controls.Clear();
            panelWH.Controls.Clear();

            bt_setting.Visible = false;

        }

 
        public void AddUserControlToPanel(UserControl userControl)
        {
            // ปล่อย Mutex ของ UserControl เดิม (ถ้ามี)
            foreach (Control ctrl in panelMain.Controls)
            {
                //if (ctrl is userControlRegular regularControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{regularControl.propQA.Report_No}_{regularControl.propQA.process}";
                //    ReleaseReportMutex(mutexKey);
                //}
                //else if (ctrl is userControlFunction functionControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{functionControl.propQA.Report_No}_{functionControl.propQA.process}";
                //    ReleaseReportMutex(mutexKey);
                //}
                //else if (ctrl is userControlDimension dimensionControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{dimensionControl.propQA.Report_No}_{dimensionControl.propQA.process}";
                //    ReleaseReportMutex(mutexKey);
                //}
                //else if (ctrl is userControlSelectRegular selectRegularControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{selectRegularControl.propQA.Report_No}_{selectRegularControl.propQA.process}";
                //    ReleaseReportMutex(mutexKey);
                //}
                //else if (ctrl is userControlSelectFunction selectFunctionControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{selectFunctionControl.propQA.Report_No}_{selectFunctionControl.propQA.process}";
                //    ReleaseReportMutex(mutexKey);
                //}
                //else if (ctrl is userControlSelectDimension selectDimensionControl)
                //{
                //    string mutexKey = $"Global\\ReportLock_{selectDimensionControl.propQA.Report_No}_{selectDimensionControl.propQA.process}";
                //    ReleaseReportMutex(mutexKey);
                //}


            }

            // ใช้ NavigationService เพื่อเปลี่ยนหน้า
            _navigationService.NavigateTo(userControl);

            // Subscribe to the event for when userControlRegular is disposed/removed
            if (userControl is userControlRegular regularCheck)
            {

                //regular.UserControlDisposed -= UserControlRegular_UserControlDisposed;
                //regular.UserControlDisposed += UserControlRegular_UserControlDisposed;

                regularCheck.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                regularCheck.AddUserControlRequested += AddUserControlToPanel;

            }
            else if (userControl is userControlSelectRegular selectRegularCheck)
            {

                //regular.UserControlDisposed -= UserControlRegular_UserControlDisposed;
                //regular.UserControlDisposed += UserControlRegular_UserControlDisposed;

                selectRegularCheck.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                selectRegularCheck.AddUserControlRequested += AddUserControlToPanel;

            }
            // เพิ่มการสมัครสมาชิก AddUserControlRequested สำหรับ userControlSelectPackingCheck
            else if (userControl is userControlSelectPackingCheck selectPackingCheck)
            {
                selectPackingCheck.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                selectPackingCheck.AddUserControlRequested += AddUserControlToPanel;
            }
            else if (userControl is userControlPackingCheck packingCheck)
            {
                packingCheck.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                packingCheck.AddUserControlRequested += AddUserControlToPanel;
            }
            else if (userControl is userControlMenu menu)
            {
                menu.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                menu.AddUserControlRequested += AddUserControlToPanel;
            }
            else if (userControl is userControlFunction functionCheck)
            {

                //regular.UserControlDisposed -= UserControlRegular_UserControlDisposed;
                //regular.UserControlDisposed += UserControlRegular_UserControlDisposed;

                functionCheck.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                functionCheck.AddUserControlRequested += AddUserControlToPanel;

            }
            else if (userControl is userControlSelectFunction selectFunctionCheck)
            {

                //regular.UserControlDisposed -= UserControlRegular_UserControlDisposed;
                //regular.UserControlDisposed += UserControlRegular_UserControlDisposed;

                selectFunctionCheck.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                selectFunctionCheck.AddUserControlRequested += AddUserControlToPanel;

            }
            else if (userControl is userControlDimension dimensionCheck)
            {

                //regular.UserControlDisposed -= UserControlRegular_UserControlDisposed;
                //regular.UserControlDisposed += UserControlRegular_UserControlDisposed;

                dimensionCheck.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                dimensionCheck.AddUserControlRequested += AddUserControlToPanel;

            }
            else if (userControl is userControlSelectDimension selectDimensionCheck)
            {

                //regular.UserControlDisposed -= UserControlRegular_UserControlDisposed;
                //regular.UserControlDisposed += UserControlRegular_UserControlDisposed;

                selectDimensionCheck.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                selectDimensionCheck.AddUserControlRequested += AddUserControlToPanel;

            }
            else if (userControl is userControlAppear appearCheck)
            {

                //regular.UserControlDisposed -= UserControlRegular_UserControlDisposed;
                //regular.UserControlDisposed += UserControlRegular_UserControlDisposed;

                appearCheck.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                appearCheck.AddUserControlRequested += AddUserControlToPanel;

            }
            else if (userControl is userControlSelectAppear selectAppearCheck)
            {

                //regular.UserControlDisposed -= UserControlRegular_UserControlDisposed;
                //regular.UserControlDisposed += UserControlRegular_UserControlDisposed;

                selectAppearCheck.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                selectAppearCheck.AddUserControlRequested += AddUserControlToPanel;

            }
            else if (userControl is userControlReplacement usrReplacement)
            {

                //regular.UserControlDisposed -= UserControlRegular_UserControlDisposed;
                //regular.UserControlDisposed += UserControlRegular_UserControlDisposed;

                usrReplacement.AddUserControlRequested -= AddUserControlToPanel; // ป้องกันการสมัครซ้ำ
                usrReplacement.AddUserControlRequested += AddUserControlToPanel;

            }
        }


        private void LoadLoginControl()
        {
            userControlLogin loginControl = new userControlLogin();
            loginControl.Dock = DockStyle.Fill;
            loginControl.LoginSuccess += LoginControl_LoginSuccess;
            //this.FormClosing += frmMain_FormClosing;

            panelLogin.Controls.Clear(); // ลบ UserControl เดิม
            panelLogin.Controls.Add(loginControl); // เพิ่ม LoginControl ไปที่ Panel

            userControlSearch usrSearch = new userControlSearch();
            _navigationService.NavigateTo(usrSearch); // ใช้ NavigationService



        }

        private void LoadProfileControl(EmployeeProperty employee)
        {
            userControlProfile profileControl = new userControlProfile(employee);
            profileControl.Dock = DockStyle.Fill;
            profileControl.Logout += ProfileControl_Logout;

            panelLogin.Controls.Clear(); // ลบ UserControl เดิม
            panelLogin.Controls.Add(profileControl); // เพิ่ม ProfileControl ไปที่ Panel

            profileControl.ClearPanelRequested += () => ClearPanel(); // ใช้ NavigationService

        }

        private void LoginControl_LoginSuccess(object sender, EventArgs e)
        {
            empProp = ((userControlLogin)sender).employee;
            LoadProfileControl(empProp); // เมื่อเข้าสู่ระบบสำเร็จ สลับไปหน้าโปรไฟล์

            empCon.SearchEmpLevel(empProp);

            // Set the current employee
            EmployeeManager.CurrentEmployee = empProp;

            panelMain.Controls.Clear();
            panelMenu.Controls.Clear();

            this.FormClosing -= frmMain_FormClosing;
            this.FormClosing += frmMain_NotClosing;

            ControlLevel(empProp);
            LoadStatusAsync();

            

        }

        public void ControlLevel(EmployeeProperty empProp)
        {
            panelHeader2.Enabled = true;
            bt_home.Visible = true;

            if (empProp.EMP_LEVEL == "1")
            //if level 1
            {
                AddUserControlAdmin();
            }
            else if (empProp.EMP_LEVEL == "2")
            {
                AddUserControlFullOperator();
            }
            else if (empProp.EMP_LEVEL == "3")
            {
                AddUserControlRecWHOperator();
            }
            else if (empProp.EMP_LEVEL == "4")
            {
                AddUserControlInspOperator();
            }
            else
            {
                //AddUserControlGuest();
                CustomMsgBoxBase.ShowCustomMessageBox(
                                "พนักงานไม่มี Skill ไม่สามารถเข้าปฏิบัติงานได้",
                                "ข้อผิดพลาด",
                                CustomMsgBoxBase.MessageBoxIconType.NG);

                ProfileControl_Logout(this, EventArgs.Empty);
            }
        }
        public void ControlBackLevel(EmployeeProperty empProp)
        {
            panelHeader2.Enabled = true;
            bt_home.Visible = true;

            panelWH.Controls.Clear();
            panelMenu.Controls.Clear();

            if (empProp.EMP_LEVEL == "1")
            //if level 1
            {
                AddUserControlAdminBack();
            }
            else if (empProp.EMP_LEVEL == "2")
            {
                AddUserControlFullOperatorBack();
            }
            else if (empProp.EMP_LEVEL == "3")
            {
                AddUserControlRecWHOperatorBack();
            }
            else if (empProp.EMP_LEVEL == "4")
            {
                AddUserControlInspOperatorBack();
            }
            else
            {
                //AddUserControlGuest();
                CustomMsgBoxBase.ShowCustomMessageBox(
                                "พนักงานไม่มี Skill ไม่สามารถเข้าปฏิบัติงานได้",
                                "ข้อผิดพลาด",
                                CustomMsgBoxBase.MessageBoxIconType.NG);

                ProfileControl_Logout(this, EventArgs.Empty);
            }
        }
        public void VisibleControl()
        {
            panelWH.Controls.Clear();
            panelMenu.Controls.Clear();
            panelHeader2.Enabled = false;
         
        }

        private void ProfileControl_Logout(object sender, EventArgs e)
        {
            empProp = new EmployeeProperty();
            LoadLoginControl();

            this.FormClosing += frmMain_FormClosing;
            this.FormClosing -= frmMain_NotClosing;
        }


        private void ReorderButtons(UserControl usc)
        {
            int currentY = 0; // เริ่มต้นที่ตำแหน่ง Y = 0

            // เรียงลำดับคอนโทรลใน Panel ตามตำแหน่ง Y ก่อน
            var sortedControls = usc.Controls.OfType<BunifuTileButton>()
                                               .Where(btn => btn.Visible)
                                               .OrderBy(btn => btn.Top)
                                               .ToList();

            foreach (BunifuTileButton btn in sortedControls)
            {
                btn.Location = new Point(0, currentY); // ตั้งตำแหน่งใหม่
                currentY += btn.Height; // ย้ายตำแหน่ง Y ถัดไปลงมา
            }
        
            
        }
       

        private void bt_home_Click(object sender, EventArgs e)
        {
            ControlLevel(empProp);
            //_navigationService.Clear(); // ใช้ NavigationService เพื่อเคลียร์หน้า
            panelMainHome();
        }

        private void panelMainHome()
        {
            userControlSearch usrSearch = new userControlSearch();
            _navigationService.NavigateTo(usrSearch); // ใช้ NavigationService
        }



     
        public void LoadStatus()
        {
            LoadStatusAsync();
        }

        public async void LoadStatusAsync()
        {
            SetStatusButtonText(null, null, null, null, null, null, null);

            int receiveWhPending = 0;
            int packingPending = 0;
            int regularPending = 0;
            int functionPending = 0;
            int dimensionPending = 0;
            int inspDataPending = 0;
            int appearancePending = 0;

            try
            {
                await Task.Run(() =>
                {
                    receiveWhPending = CountPending("Keep_Data");
                    packingPending = CountPending("Packing_Check");
                    regularPending = CountPending("Regular_Check");
                    functionPending = CountPending("Function_Check");
                    dimensionPending = CountPending("Dimension_Check");
                    inspDataPending = CountPending("Inspection_Data_Check");
                    appearancePending = CountPending("Appearance_Check");
                });
            }
            catch
            {
                receiveWhPending = 0;
                packingPending = 0;
                regularPending = 0;
                functionPending = 0;
                dimensionPending = 0;
                inspDataPending = 0;
                appearancePending = 0;
            }

            if (IsDisposed)
            {
                return;
            }

            SetStatusButtonText(receiveWhPending, packingPending, regularPending, functionPending, dimensionPending, inspDataPending, appearancePending);
        }

        private int CountPending(string process)
        {
            QAdataProperty countProp = new QAdataProperty
            {
                process = process
            };

            return new QAdataControllers().CountProcessStatusPending(countProp);
        }

        private void SetStatusButtonText(int? receiveWhPending, int? packingPending, int? regularPending, int? functionPending, int? dimensionPending, int? inspDataPending, int? appearancePending)
        {
            string receiveWhText = receiveWhPending.HasValue ? receiveWhPending.Value.ToString() : "...";
            string packingText = packingPending.HasValue ? packingPending.Value.ToString() : "...";
            string regularText = regularPending.HasValue ? regularPending.Value.ToString() : "...";
            string functionText = functionPending.HasValue ? functionPending.Value.ToString() : "...";
            string dimensionText = dimensionPending.HasValue ? dimensionPending.Value.ToString() : "...";
            string inspDataText = inspDataPending.HasValue ? inspDataPending.Value.ToString() : "...";
            string appearanceText = appearancePending.HasValue ? appearancePending.Value.ToString() : "...";

            ConfigureHeaderStatusButtons();
            SetStatusButtonText(bt_status_rec_pending, "Receive WH", receiveWhText);
            SetStatusButtonText(bt_status_packing_check_pending, "Packing Check", packingText);
            SetStatusButtonText(bt_status_regular_pending, "Regular Check", regularText);
            SetStatusButtonText(bt_status_function_pending, "Function Check", functionText);
            SetStatusButtonText(bt_status_dimension_pending, "Dimension Check", dimensionText);
            SetStatusButtonText(bt_status_data_pending, "Insp. Data Check", inspDataText);
            SetStatusButtonText(bt_appear_pending, "Appearance", appearanceText);
        }

        private void SetStatusButtonText(BunifuFlatButton button, string processName, string pendingCount)
        {
            string buttonText = $"{processName}\nPending\n{pendingCount} Report";

            button.Text = buttonText;
            button.ButtonText = buttonText;
            button.TextAlign = ContentAlignment.MiddleCenter;
            button.TextFont = new Font("Tahoma", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            button.IconVisible = false;
            button.IconRightVisible = false;
            button.IconMarginLeft = 0;
            button.IconMarginRight = 0;
        }

        private void bt_refresh_Click(object sender, EventArgs e)
        {
            LoadStatusAsync();
        }

        private void bt_status_rec_pending_Click(object sender, EventArgs e)
        {
            if (empProp.EMP_LEVEL == "1")
            {
                userControlRecWHPending usrPend = new userControlRecWHPending();
                usrPend.Dock = DockStyle.Fill;

                panelMain.Controls.Clear();
                panelMain.Controls.Add(usrPend);
            }
        }


        private void bt_status_packing_check_pending_Click(object sender, EventArgs e)
        {
            if (empProp.EMP_LEVEL == "1")
            {
                userControlPackingCheckPending usrPackCheckPend = new userControlPackingCheckPending();
                _navigationService.NavigateTo(usrPackCheckPend); // ใช้ NavigationService
            }
        }

        private void bt_status_regular_pending_Click(object sender, EventArgs e)
        {
            if (empProp.EMP_LEVEL == "1")
            {
                userControlSelectRegularPending usrSelectRegPending = new userControlSelectRegularPending();
                _navigationService.NavigateTo(usrSelectRegPending); // ใช้ NavigationService
            }
        }

        private void bt_status_function_pending_Click(object sender, EventArgs e)
        {
            if (empProp.EMP_LEVEL == "1")
            {
                userControlSelectFunctionPending usrSelectFuncPending = new userControlSelectFunctionPending();
                _navigationService.NavigateTo(usrSelectFuncPending); // ใช้ NavigationService
            }
        }

        //private void UserControlRegular_RequestReleaseMutex(string mutexKey)
        //{
        //    // เมื่อได้รับคำขอจาก userControlRegular ให้ปล่อย Mutex
        //    ReleaseReportMutex(mutexKey);
        //}

        // เมธอดสำหรับสร้างและเพิ่ม Mutex
        //public bool TryCreateMutex(string mutexKey, out Mutex mutex)
        //{
        //    //mutex = new Mutex(true, mutexKey, out bool mutexCreated);
        //    //if (!mutexCreated)
        //    //{
        //    //    mutex.Dispose();
        //    //    return false;
        //    //}
        //    //reportMutexes[mutexKey] = mutex;
        //    //Console.WriteLine($"Mutex {mutexKey} added. Count: {reportMutexes.Count}");
        //    //return true;
        //    return MutexManager.Instance.TryCreateMutex(mutexKey, out mutex);
        //}

        // เมธอดสำหรับปล่อยและลบ Mutex
        //public void ReleaseReportMutex(string mutexKey)
        //{
        //    MutexManager.Instance.ReleaseMutex(mutexKey);
        //    //if (string.IsNullOrEmpty(mutexKey))
        //    //{
        //    //    Console.WriteLine("Mutex key is null or empty.");
        //    //    return;
        //    //}

        //    //if (reportMutexes.ContainsKey(mutexKey))
        //    //{
        //    //    try
        //    //    {
        //    //        reportMutexes[mutexKey].ReleaseMutex();
        //    //        reportMutexes[mutexKey].Dispose();
        //    //        Console.WriteLine($"Mutex {mutexKey} released successfully.");
        //    //    }
        //    //    catch (ApplicationException ex)
        //    //    {
        //    //        Console.WriteLine($"Mutex {mutexKey} already released: {ex.Message}");
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        Console.WriteLine($"Error releasing Mutex {mutexKey}: {ex.Message}");
        //    //    }
        //    //    finally
        //    //    {
        //    //        reportMutexes.Remove(mutexKey);
        //    //        Console.WriteLine($"Mutex {mutexKey} removed. Current Mutex count: {reportMutexes.Count}");
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    Console.WriteLine($"Mutex {mutexKey} not found in reportMutexes. Current Mutex count: {reportMutexes.Count}");
        //    //}
        //}

        // Event handler to automatically release the Mutex when userControlRegular is disposed
        //private void UserControlRegular_UserControlDisposed(object sender, string mutexKey)
        //{
        //    // Release the mutex associated with the report
        //    ReleaseReportMutex(mutexKey);
        //}

        private void ClearPanel()
        {
            //foreach (Control ctrl in panelMain.Controls)
            //{
            //    if (ctrl is userControlRegular regularControl)
            //    {
            //        string mutexKey = $"Global\\ReportLock_{regularControl.propQA.Report_No}_{regularControl.propQA.process}";
            //        ReleaseReportMutex(mutexKey);
            //    }
            //    else if (ctrl is userControlSelectRegular selectControl)
            //    {
            //        string mutexKey = $"Global\\ReportLock_{selectControl.propQA.Report_No}_{selectControl.propQA.process}";
            //        ReleaseReportMutex(mutexKey);
            //    }
            //    ctrl.Dispose();
            //}
            //panelMain.Controls.Clear();

            // ถ้ามี panelProfile หรือ panel อื่น ๆ

            panelMenu.Controls.Clear();
            panelWH.Controls.Clear();

            //foreach (Control ctrl in panelLogin.Controls)
            //{
            //    ctrl.Dispose();
            //}
            //panelLogin.Controls.Clear();

            // รีเซ็ตสถานะอื่น ๆ ถ้ามี
            _navigationService?.Clear();

            // รีเซ็ต empProp
            empProp = new EmployeeProperty(); // หรือ empProp = null; ถ้าต้องการให้เป็น null

        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            //e.Cancel = true; // ยกเลิกการปิดฟอร์ม
            //สร้าง MessageBox แบบ Yes/ No
            DialogResult result = MessageBox.Show(
                "คุณต้องการออกจากโปรแกรมหรือไม่?",
                "ยืนยันการออกจากโปรแกรม",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            // ตรวจสอบผลลัพธ์
            if (result == DialogResult.No)
            {
                e.Cancel = true; // ยกเลิกการปิดฟอร์ม
            }
        }

        private void frmMain_NotClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true; // ยกเลิกการปิดฟอร์ม
            //สร้าง MessageBox แบบ Yes/ No
            //DialogResult result = MessageBox.Show(
            //    "คุณต้องการออกจากโปรแกรมหรือไม่?",
            //    "ยืนยันการออกจากโปรแกรม",
            //    MessageBoxButtons.YesNo,
            //    MessageBoxIcon.Warning);

            //// ตรวจสอบผลลัพธ์
            //if (result == DialogResult.No)
            //{
            //    e.Cancel = true; // ยกเลิกการปิดฟอร์ม
            //}
        }

        private void bt_status_dimension_pending_Click(object sender, EventArgs e)
        {
            if (empProp.EMP_LEVEL == "1")
            {
                userControlSelectDimensionPending usrSelectDimPending = new userControlSelectDimensionPending();
                _navigationService.NavigateTo(usrSelectDimPending); // ใช้ NavigationService
            }
        }

        private void bt_status_data_pending_Click(object sender, EventArgs e)
        {
            if (empProp.EMP_LEVEL == "1")
            {
                userControlSelectInspDataPending usrSelectInspDataPending = new userControlSelectInspDataPending();
                _navigationService.NavigateTo(usrSelectInspDataPending); // ใช้ NavigationService
            }
        }

        private void bt_appear_pending_Click(object sender, EventArgs e)
        {

        }

        private void bt_setting_Click(object sender, EventArgs e)
        {
            if (empProp.EMP_LEVEL == "1")
            {
                // ถ้าเป็น Level 1 ให้เปิดหน้า Setting ได้
                frmSetting FormSetting = new frmSetting();
                FormSetting.ShowDialog();
            }
        }
    }
}
