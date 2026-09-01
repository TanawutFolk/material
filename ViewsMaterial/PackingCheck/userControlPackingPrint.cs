using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.IO;
using Microsoft.Office.Interop.Excel;
using RawMat.ViewsMaterial.Main;
using RawMat.ViewsMaterial.CustomMsg;
using static RawMat.Property.QAdataProperty;

namespace RawMat.ViewsMaterial.PackingCheck
{
    public partial class userControlPackingPrint : UserControl
    {
        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler BackToARequested;
        public event Action<string> RequestReleaseMutex;

        //public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();
        printerCls printerCls = new printerCls();
        private IParent parent;

        public userControlPackingPrint(IParent parent)
        {
            InitializeComponent();
            this.parent = parent;
        }

        private void userControlPackingPrint_Load(object sender, EventArgs e)
        {
            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_lotSize.Text = "Lot Size :" + propQA.Qty;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");

            pb_packing4.Image = imgCls.LoadPackingImage(propQA.M_CODE);
            //pb_packing2.Image= LoadPackingImage("Packing2");
            //pb_packing3.Image = LoadPackingImage("Packing3");

            propQA.METHOD_ID = "4";
            lb_method4.Text = conQA.DetailMethod(propQA);

            //propQA.METHOD_ID = "2";
            //lb_method5.Text = "";

            //propQA.METHOD_ID = "3";
            //lb_method6.Text = "";

            //lblPrinterName.Text = printerCls.GetDefaultPrinter();

            string PrinterName = "";
            int nCheck = 0;
            nCheck = printerCls.checkPrinter(out PrinterName);

            //lblPrinterName.Text = "Printer Name : " + PrinterName;

            if (nCheck == 1)
            {
                lblPrinterName.ForeColor = Color.Green;
                lblPrinterName.Text = "Printer Name : " + PrinterName + " is Ready";
            }
            else
            {
                lblPrinterName.ForeColor = Color.Red;
                lblPrinterName.Text = "Printer Name : " + PrinterName + " is Problem";
            }

            //if (employee.EMP_LEVEL == "1")
            //{
            //    //bt_print.Visible = false;
            //    //bt_save.Visible = true;
            //    //lblPrinterName
            //}
            //else
            //{
            //    bt_print.Visible = true;
            //    bt_save.Visible = false;
            //}



        }


        private void bt_print_Click(object sender, EventArgs e)
        {
            propQA.EMP_ID = employee.EMP_CODE;
            
            //ใช้ตอน มีดาต้าชื่อจาก login จริง
            //propQA.EMP_NAME = employee.EMP_FULL_NAME.Split(' ')[0]; ;

            propQA.process = "Packing_Check";

            //if (rb_ng_method4.Checked)
            //{
            //    // ตรวจสอบว่าช่อง tb_detail ว่างหรือไม่
            //    if (string.IsNullOrWhiteSpace(tb_detail_method4.Text))
            //    {
            //        MessageBox.Show("กรุณากรอกข้อมูลในช่องรายละเอียด เนื่องจากเลือก NG", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return; // หยุดการทำงานของปุ่มบันทึก
            //    }

            //    propQA.judge = "6";
            //    propQA.METHOD_ID = "4";

            //    propQA.detail_Method = tb_detail_method4.Text;

            //    if (conQA.CountPackingCheck(propQA) == 0)
            //    {
            //        conQA.InsertPackingCheck(propQA);
            //    }

            //    if (conQA.UpdateStatus(propQA) == true)
            //    {

            //    }

            //    this.Controls.Clear();
            //    bt_rec_pack_Click();

            //}
            //else if (rb_ok_method4.Checked)
            //{
            //    propQA.judge = "1";
            //    propQA.reportStatus = "1";
            //    propQA.inProcStatus = "1";

            //}
            //else
            //{
            //    MessageBox.Show($"เกิดข้อผิดพลาด: กรุณาเลือก OK หรือ NG เพื่อทำการ Confirm", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}


            propQA.FORMAT_REPORT_ID = "2";
            System.Data.DataTable dt = conQA.SearchFormatReport(propQA);


            if (dt == null || dt.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบข้อมูลในฐานข้อมูล Format Report", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string templatePath = ConfigurationManager.AppSettings["ReportFormatFile"];
            string file = propQA.M_CODE ;
            string full = Path.Combine(templatePath, file);
            string temp = Path.Combine(templatePath, "temp" , propQA.M_CODE + ".xls");

            string fileExcel = FindExcelFile(full);

            if (string.IsNullOrEmpty(fileExcel))
            {
                MessageBox.Show("ไม่พบไฟล์ Template.xlsx หรือ Template.xls");
                return;
            }

            //
            string recordPath = ConfigurationManager.AppSettings["ReportRecord"];
            propQA.dtToday = conQA.SearchToday();

            string fullPathRecord = Path.Combine(recordPath , propQA.dtToday.ToString("yyyy"),"RECORD","T01,D16");
            string fileNameRecord = propQA.Report_No + "_" + propQA.M_CODE + "_" + propQA.dtReceiveDate.ToString("yyyy-MM-dd") + ".xls";

            if (string.IsNullOrEmpty(fullPathRecord))
            {
                MessageBox.Show("ไม่พบไฟล์ Template.xlsx หรือ Template.xls");
                return;
            }

            string fullFileRecord = Path.Combine(fullPathRecord, fileNameRecord);

            Microsoft.Office.Interop.Excel.Application excelApp = null;
            Workbook workbook = null;

            try
            {
                excelApp = new Microsoft.Office.Interop.Excel.Application();
                excelApp.DisplayAlerts = false; // ปิดการแสดงข้อความแจ้งเตือนทั้งหมด
                excelApp.AlertBeforeOverwriting = false; // ปิดการเตือนก่อนทับไฟล์
                excelApp.Visible = true;

                workbook = excelApp.Workbooks.Open(fileExcel);
                Worksheet worksheet = (Worksheet)workbook.Sheets[1]; // ใช้ Sheet ที่ 1

                // เติมข้อมูลใน Excel ตามที่กำหนดไว้ในฐานข้อมูล
                foreach (DataRow row in dt.Rows)
                {
                    string cell = row["cell"].ToString(); // อ่านค่าคอลัมน์ cell เช่น "V3"
                    string cellName = row["cell_name"].ToString(); // อ่านค่าคอลัมน์ cell_name เช่น "Report_No"
                    string value = GetValueForCell(cellName , propQA); // ดึงค่าของ cell_name ที่ระบุ

                    // เติมค่าในเซลล์ Excel
                    worksheet.Range[cell].Value = value;
                }

                // บันทึกไฟล์ Excel
                //SaveFileDialog saveDialog = new SaveFileDialog();
                //saveDialog.Filter = "Excel 2007+|*.xlsx|Excel 97-2003|*.xls";

                //if (saveDialog.ShowDialog() == DialogResult.OK)
                //{
                // กำหนดรูปแบบการบันทึกตามนามสกุลที่เลือก
                Microsoft.Office.Interop.Excel.XlFileFormat format = Path.GetExtension(fileExcel).ToLower() == ".xls"
                    ? Microsoft.Office.Interop.Excel.XlFileFormat.xlExcel8
                    : Microsoft.Office.Interop.Excel.XlFileFormat.xlOpenXMLWorkbook;

                workbook.SaveAs(fullFileRecord, format);
                //}


                //ปริ้่นอันนี้เป็นการปริ้นหน้าหลัง
                //workbook.PrintOut();



                int totalPages = worksheet.PageSetup.Pages.Count;
                if (totalPages == 0)
                {
                    MessageBox.Show($"ไม่พบหน้าที่จะ print กรุณาตรวจสอบไฟล์ template: {fileExcel}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    // พิมพ์หน้า 1 (1 สำเนา)
                    for (int page = 1; page <= totalPages; page++)
                    {
                        worksheet.PrintOut(
                            From: page,
                            To: page,
                            Copies: 1,
                            Preview: false
                        );
                    }
                }



                workbook.Close(false);
                excelApp.Quit();
                //workbook.Close(false);
                //excelApp.Quit();

                CustomMsgBoxBase.ShowCustomMessageBox(
               message: "Print Inspection Check Sheet เสร็จแล้ว",
               title: "สำเร็จ",
               icon: CustomMsgBoxBase.MessageBoxIconType.OK);

                //insert update

                propQA.judge = "1";
                propQA.reportStatus = "1";
                propQA.inProcStatus = "1";
                propQA.METHOD_ID = "4";
                //propQA.detail_Method = tb_detail_method4.Text;
                propQA.detail_Method = "";

                if (conQA.CountPackingCheck(propQA) == 0)
                {
                    conQA.InsertPackingCheck(propQA);
                }

                if (conQA.UpdateStatus(propQA) == true)
                {

                }

                loadstatus();

                this.Controls.Clear();
                bt_rec_pack_Click();

                // ส่งคำสั่งพิมพ์
                //var printExcelApp = new Microsoft.Office.Interop.Excel.Application();
                //Workbook printWorkbook = printExcelApp.Workbooks.Open(temp);
                //printWorkbook.PrintOut();
                //printWorkbook.Close(false);
                //printExcelApp.Quit();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {

                // 2. ปิด Workbook
                if (workbook != null)
                {
                    try
                    {
                        workbook.Close(false);
                    }
                    catch { } // จับ Exception หากถูกปิดไปแล้ว

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                    workbook = null;
                }

                // 3. ปิด Application
                if (excelApp != null)
                {
                    try
                    {
                        excelApp.Quit();
                    }
                    catch { } // จับ Exception หากถูกปิดไปแล้ว

                    System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);
                    excelApp = null;
                }

                // 4. คืนหน่วยความจำ
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        // ดึงค่าจาก cell_name
        private string GetValueForCell(string cellName , QAdataProperty dataItem)
        {
            // คุณสามารถเขียนโค้ดเพื่อดึงค่าจริงตาม cell_name ได้จากฐานข้อมูลหรือที่อื่น
            switch (cellName)
            {
                case "Report_No": return dataItem.Report_No;
                case "Receive_Date": return dataItem.dtReceiveDate.ToString("dd/MM/yyyy");
                case "Invoice_No": return dataItem.Invoice_No;
                case "Issue_EMP_ID": return dataItem.EMP_ID;
                case "Issue_Date": return dataItem.dtIssueDate.ToString("dd/MM/yyyy");
                case "Issue_EMP_NAME": return dataItem.EMP_NAME;
                case "Qty": return dataItem.Qty;
                default: return string.Empty;
            }
        }

        private string FindExcelFile(string basePath)
        {
            if (File.Exists(basePath + ".xlsx")) return basePath + ".xlsx";
            if (File.Exists(basePath + ".xls")) return basePath + ".xls";
            return null;
        }

        public void bt_rec_pack_Click()
        {

            userControlSelectPackingCheck usrControlSelectPackingCheck = new userControlSelectPackingCheck();

            usrControlSelectPackingCheck.Dock = DockStyle.Fill;
            usrControlSelectPackingCheck.propQA = new QAdataProperty();

            usrControlSelectPackingCheck.propQA.labelProcess = "Select Report for : " + "packing check";
            usrControlSelectPackingCheck.propQA.process = "Packing_Check";
            usrControlSelectPackingCheck.propQA.prevProcess = "Receive_WH";

            System.Data.DataTable dt = new System.Data.DataTable();
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
        }

        private void bt_save_Click(object sender, EventArgs e)
        {
            propQA.EMP_ID = employee.EMP_CODE;

            //ใช้ตอน มีดาต้าชื่อจาก login จริง
            //propQA.EMP_NAME = employee.EMP_FULL_NAME.Split(' ')[0]; ;

            propQA.process = "Packing_Check";

            //if (rb_ng_method4.Checked)
            //{
            //    // ตรวจสอบว่าช่อง tb_detail ว่างหรือไม่
            //    if (string.IsNullOrWhiteSpace(tb_detail_method4.Text))
            //    {
            //        MessageBox.Show("กรุณากรอกข้อมูลในช่องรายละเอียด เนื่องจากเลือก NG", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return; // หยุดการทำงานของปุ่มบันทึก
            //    }

            //    propQA.judge = "0";
            //    propQA.reportStatus = "0";
            //    propQA.inProcStatus = "0";

            //    propQA.METHOD_ID = "4";

            //    propQA.detail_Method = tb_detail_method4.Text;

            //    if (conQA.CountPackingCheck(propQA) == 0)
            //    {
            //        conQA.InsertPackingCheck(propQA);
            //    }

            //    if (conQA.UpdateStatus(propQA) == true)
            //    {

            //    }

            //    userControlPackingCheckPending usrPackCheckPend = new userControlPackingCheckPending();
            //    usrPackCheckPend.Dock = DockStyle.Fill;

            //    this.Controls.Clear();
            //    this.Controls.Add(usrPackCheckPend);
            //    return;

            //}
            //else if (rb_ok_method4.Checked)
            //{
            //    propQA.judge = "2";
            //    propQA.reportStatus = "2";
            //    propQA.inProcStatus = "2";

            //    propQA.METHOD_ID = "4";

            //    propQA.detail_Method = tb_detail_method4.Text;

            //    if (conQA.CountPackingCheck(propQA) == 0)
            //    {
            //        conQA.InsertPackingCheck(propQA);
            //    }

            //    if (conQA.UpdateStatus(propQA) == true)
            //    {

            //    }

            //    userControlPackingCheckPending usrPackCheckPend = new userControlPackingCheckPending();
            //    usrPackCheckPend.Dock = DockStyle.Fill;

            //    this.Controls.Clear();
            //    this.Controls.Add(usrPackCheckPend);
            //    return;
            //}
            //else
            //{
            //    MessageBox.Show($"เกิดข้อผิดพลาด: กรุณาเลือก OK หรือ NG เพื่อทำการ Confirm", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    return;
            //}

        }

        private void bt_back_Click(object sender, EventArgs e)
        {
            propQA.inProcStatus = ((int)ProcStatus.Unfinished).ToString();
            propQA.reportStatus = ((int)ProcStatus.Unfinished).ToString();

            if (conQA.UpdateStatus(propQA) == false)
            {
                MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Unfinished ได้");
            }

            bt_rec_pack_Click();

        }

        private void loadstatus()
        {
            if (this.ParentForm is frmMain mainForm)
            {
                mainForm.LoadStatus();
            }
        }

        //private void tb_detail_method4_TextChanged(object sender, EventArgs e)
        //{
        //    // Trim ข้อความใน TextBox
        //    //tb_detail_method1.Text = tb_detail_method1.Text.Trim();

        //    // ให้เคอร์เซอร์อยู่ท้ายสุดหลัง Trim
        //    //tb_detail_method1.SelectionStart = tb_detail_method1.Text.Length;

        //    // อัปเดต Label
        //    tb_detail_method4.Text = $"{tb_detail_method4.Text.Length} /255";

        //    // เช็คความยาวไม่เกิน 255 อักขระ
        //    if (tb_detail_method4.Text.Length > 255)
        //    {
        //        MessageBox.Show("ข้อความห้ามเกิน 255 อักขระ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        //        // ตัดข้อความที่เกินออก
        //        tb_detail_method4.Text = tb_detail_method4.Text.Substring(0, 255);
        //        tb_detail_method4.SelectionStart = tb_detail_method4.Text.Length; // ให้เคอร์เซอร์อยู่ท้ายสุด
        //    }
        //}
    }
}
