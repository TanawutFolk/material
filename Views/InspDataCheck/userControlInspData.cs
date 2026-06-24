using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static RawMat.frmMain;
using static RawMat.Property.QAdataProperty;
using AxAcroPDFLib;
using PdfiumViewer;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using Microsoft.Office.Core;
using RawMat.Views.CustomMsg;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace RawMat.Views.InspDataCheck
{
    public partial class userControlInspData : UserControl
    {
        [System.Runtime.InteropServices.DllImport("ole32.dll")]
        static extern void CoFreeUnusedLibraries();

        public event Action<UserControl> AddUserControlRequested;

        public event EventHandler BackToARequested;

        public event Action OnClose;

        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;
        imgCls imgCls = new imgCls();

        private BindingSource bindingSource = new BindingSource();
        private DataTable originalDataTable;
        private int currentPage = 1;
        private int totalPages = 1;
        private frmMain mainForm;

        private IParent parent;
        public delegate void UserControlDisposedEventHandler(object sender, string reportNo);
        public event UserControlDisposedEventHandler UserControlDisposed;
        
        private PdfRenderer pdfRenderer = new PdfRenderer();
        //private PdfDocument pdfDocument;// เพิ่มตัวแปรเพื่อเก็บ PdfDocument
        AxAcroPDF pdf_data_check = new AxAcroPDF();

        public userControlInspData()
        {

            InitializeComponent();


        }

        private void userControlData_Load(object sender, EventArgs e)
        {
            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_lotSize.Text = "Lot Size : " + propQA.Qty;

            string keepDataFolderPath = ConfigurationManager.AppSettings["KeepDataPath"];
            string file = System.IO.Path.Combine(keepDataFolderPath, propQA.dtReceiveDate.ToString("yyyy"), propQA.dtReceiveDate.ToString("yyyyMMdd") + "_" + propQA.M_CODE + ".pdf");

            try
            {
              
                
                //pdf_data_check.CreateControl(); // Ensure the control is initialized
                panel_pdf.Visible = true;
                panel_pdf.Controls.Add(pdf_data_check);
                pdf_data_check.Dock = DockStyle.Fill;
                pdf_data_check.Visible = true; // Set visible before loading

                
                 
                pdf_data_check.src = file ;
                pdf_data_check.LoadFile(file);
                pdf_data_check.setShowToolbar(false);
                pdf_data_check.BringToFront(); // Bring to front for visibility
                pdf_data_check.Show();
                //panel_pdf.Refresh();
                pdf_data_check.Focus();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading PDF: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


            // test find data

            //string text = ExtractTextFromPdf(file);

            //// ตรวจสอบว่ามีคำว่า "Manufacture" และ "SONY" หรือไม่
            //if (text.Contains(propQA.M_CODE))
            //{
            //    MessageBox.Show(propQA.M_CODE);
            //    MessageBox.Show(text);
            //}
            //else
            //{
            //    MessageBox.Show("ไม่พบคำที่ต้องการใน PDF");
            //}  

            List<int> resultPages = SearchPdf(file, propQA.M_CODE);

            if (resultPages.Count > 0)
            {
                Console.WriteLine($"พบคำว่า '{propQA.M_CODE}' ในหน้าที่: {string.Join(", ", resultPages)}");
            }
            else
            {
                Console.WriteLine($"ไม่พบคำว่า '{propQA.M_CODE}'");
            }

        }
    

        public static List<int> SearchPdf(string filePath, string searchText)
        {
            List<int> foundPages = new List<int>();

            using (PdfReader reader = new PdfReader(filePath))
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    string pageText = PdfTextExtractor.GetTextFromPage(reader, page, new SimpleTextExtractionStrategy());

                    if (pageText.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        foundPages.Add(page);
                    }
                }
            }
            return foundPages;
        }

        public static string ExtractTextFromPdf(string pdfPath)
        {
            StringBuilder text = new StringBuilder();
            using (PdfReader reader = new PdfReader(pdfPath))
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    text.Append(PdfTextExtractor.GetTextFromPage(reader, page));
                }
            }
            
            return text.ToString();
        }


        private void bt_back_Click(object sender, EventArgs e)
        {
            propQA.inProcStatus = ((int)ProcStatus.Unfinished).ToString();
            propQA.reportStatus = ((int)ProcStatus.Unfinished).ToString();

            if (!conQA.UpdateStatus(propQA))
            {
                MessageBox.Show("ไม่สามารถเปลี่ยนสถานะกลับเป็น Unfinished ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (!conQA.DeleteReportActive(propQA))
            {
                MessageBox.Show("ไม่สามารถเพิ่ม report no กับ IP ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            pdf_data_check.Dispose();
            pdf_data_check = null;

            CoFreeUnusedLibraries(); // เรียกหลังจาก dispose

            bt_data_Click();
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
                    panelMain.Controls.Add(usrConSelectData);
                    usrConSelectData.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void tb_detail_TextChanged(object sender, EventArgs e)
        {
            lb_length_detail.Text = $"{tb_data_detail.Text.Length} /255";
            if (tb_data_detail.Text.Length > 255)
            {
                MessageBox.Show("ข้อความห้ามเกิน 255 อักขระ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb_data_detail.Text = tb_data_detail.Text.Substring(0, 255);
                tb_data_detail.SelectionStart = tb_data_detail.Text.Length;
            }
        }

        private void bt_confirm_Click(object sender, EventArgs e)
        {

            if (rb_ng.Checked)
            {
                if (string.IsNullOrWhiteSpace(tb_data_detail.Text))
                {
                    MessageBox.Show($"กรุณากรอกข้อมูลในช่องรายละเอียด เนื่องจากเลือก NG สำหรับ Judgment เอกสาร Inspection", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return ;
                }

                propQA.judge = "6";

                if (employee.EMP_LEVEL == "1") propQA.judge = "0";
            }
            else if (rb_ok.Checked)
            {
                propQA.judge = "1";
            }
            else
            {
                MessageBox.Show($"กรุณาเลือก OK หรือ NG ก่อนบันทึก", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return ;
            }

            propQA.data_detail = tb_data_detail.Text;
            propQA.EMP_ID = employee.EMP_CODE;

            if (conQA.InsertUpdateInspData(propQA) == false)
            {
                MessageBox.Show($"ไม่สามารถ record Inspection Data ได้ กรุณาลองใหม่อีกครั้ง", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            else
            {
                if (propQA.judge == ((int)ProcStatus.OK).ToString() && IsEndAtDataResultReport())
                {
                    if (!SetRegularWaitingApprove())
                    {
                        MessageBox.Show("ไม่สามารถ update Regular เป็น Waiting Approve หลัง Data Result ได้", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }


                ProcStatus status;

                bool parsed = int.TryParse(propQA.judge, out int statusId) && Enum.IsDefined(typeof(ProcStatus), statusId);
                status = parsed ? (ProcStatus)statusId : ProcStatus.NG; // ค่าเริ่มต้นเป็น NG ถ้าแปลงไม่ได้

                switch (status)
                {
                    case ProcStatus.OK:
                        CustomMsgBoxBase.ShowCustomMessageBox(
                            "Record Inspection Data งาน OK เรียบร้อยแล้ว",
                            "สำเร็จ",
                            CustomMsgBoxBase.MessageBoxIconType.OK);
                        break;
                    case ProcStatus.Pending:
                        CustomMsgBoxBase.ShowCustomMessageBox(
                            "Record  Inspection Data พบงาน ถูก PENDING",
                            "สำเร็จ",
                            CustomMsgBoxBase.MessageBoxIconType.Pending);
                        break;
                    case ProcStatus.NG:
                        CustomMsgBoxBase.ShowCustomMessageBox(
                            "Record  Inspection Data พบงาน ถูก NG",
                            "สำเร็จ",
                            CustomMsgBoxBase.MessageBoxIconType.NG);
                        break;
                    default:
                        CustomMsgBoxBase.ShowCustomMessageBox(
                            "สถานะไม่รู้จัก",
                            "ข้อผิดพลาด",
                            CustomMsgBoxBase.MessageBoxIconType.Question);
                        break;
                }

                loadstatus();
                bt_data_Click();

            }


        }


        private bool IsEndAtDataResultReport()
        {
            try
            {
                return conQA.NeedKeepData(propQA) == 1
                    && conQA.NeedFunctionCheck(propQA) != 1
                    && conQA.NeedDimensionCheck(propQA) != 1
                    && conQA.NeedAppearCheck(propQA) != 1;
            }
            catch
            {
                return false;
            }
        }

        private bool SetRegularWaitingApprove()
        {
            string currentProcess = propQA.process;
            string currentInProcStatus = propQA.inProcStatus;
            string currentReportStatus = propQA.reportStatus;

            try
            {
                propQA.process = "Regular_Check";
                propQA.inProcStatus = ((int)ProcStatus.WaitingApprove).ToString();
                propQA.reportStatus = ((int)ProcStatus.WaitingApprove).ToString();

                if (!conQA.UpdateReportStatus(propQA))
                {
                    return false;
                }

                return true;
            }
            finally
            {
                propQA.process = currentProcess;
                propQA.inProcStatus = currentInProcStatus;
                propQA.reportStatus = currentReportStatus;
            }
        }

        private void loadstatus()
        {
            if (this.ParentForm is frmMain mainForm)
            {
                mainForm.LoadStatus();
            }
        }
    }
}
