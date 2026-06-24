using AxAcroPDFLib;
using PdfiumViewer;
using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Remoting.Messaging;
using RawMat.Views.CustomMsg;
using static RawMat.Property.QAdataProperty;

namespace RawMat.Views.InspDataCheck
{
    public partial class userControlInspDataPending : UserControl
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
        private PdfDocument pdfDocument;// เพิ่มตัวแปรเพื่อเก็บ PdfDocument
        AxAcroPDF pdf_data_check = new AxAcroPDF();

        public userControlInspDataPending()
        {
            InitializeComponent();
        }

        private void userControlInspDataPending_Load(object sender, EventArgs e)
        {
            lb_reportNo.Text = "Report No. : " + propQA.Report_No;
            lb_invoice.Text = "Invoice : " + propQA.Invoice_No;
            lb_mcode.Text = propQA.M_CODE + " : " + propQA.Material_Name;
            lb_recDate.Text = "Receive Date : " + propQA.dtReceiveDate.ToString("dd-MMM-yyyy");
            lb_lotSize.Text = "Lot Size :" + propQA.Qty;

            DataTable dtInspDataPending = conQA.SearchDataInspDataPending(propQA);

            if (dtInspDataPending.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบ data inspection data ที่ Pending ใน db_inspection_data");
                return;
            }

            lb_emp_op.Text = "EMP ID : " + dtInspDataPending.Rows[0]["EMP_ID"].ToString();
            //lb_insp_date.Text = dtInspDataPending.Rows[0]["INSPECTION_DATA_DATE"].ToString("dd-MMM-yyyy");

            object inspectionDateObj = dtInspDataPending.Rows[0]["INSPECTION_DATA_DATE"];
            if (inspectionDateObj != null && inspectionDateObj != DBNull.Value)
            {
                if (DateTime.TryParse(inspectionDateObj.ToString(), out DateTime inspectionDate))
                {
                    lb_insp_date.Text = "INSP DATE : " + inspectionDate.ToString("dd-MMM-yyyy");
                }
                else
                {
                    lb_insp_date.Text = "INSP DATE : วันที่ไม่ถูกต้อง";
                    // สามารถเพิ่มการบันทึกข้อผิดพลาด (logging) หรือการจัดการเพิ่มเติมตามความต้องการ
                }
            }
            else
            {
                lb_insp_date.Text = "INSP DATE : ไม่มีข้อมูลวันที่";
            }

            gb_data_judge.Enabled = false;
            gb_data_qa_judge.Enabled = true;

            if(dtInspDataPending.Rows[0]["JUDGE"].ToString() == "0" || dtInspDataPending.Rows[0]["JUDGE"].ToString() == "6")
            {
                rb_ng.Checked = true;
            }
            else if (dtInspDataPending.Rows[0]["JUDGE"].ToString() == "1")
            {
                rb_ok.Checked = true;
            }
            else
            {
                rb_ng.Checked = true;
            }

            tb_data_detail.Text = dtInspDataPending.Rows[0]["REMARK"].ToString();
            tb_data_qa_detail.Text = dtInspDataPending.Rows[0]["REMARK"].ToString();

            try
            {
                string keepDataFolderPath = ConfigurationManager.AppSettings["KeepDataPath"];
                string file = Path.Combine(keepDataFolderPath, propQA.dtReceiveDate.ToString("yyyy"), propQA.dtReceiveDate.ToString("yyyyMMdd") + "_" + propQA.M_CODE + ".pdf");

                //pdf_data_check.CreateControl(); // Ensure the control is initialized
                panel_pdf.Visible = true;
                panel_pdf.Controls.Add(pdf_data_check);
                pdf_data_check.Dock = DockStyle.Fill;
                pdf_data_check.Visible = true; // Set visible before loading


                pdf_data_check.src = file;
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
                return;
            }

        }

        private void bt_confirm_Click(object sender, EventArgs e)
        {

            if (rb_qa_ng.Checked)
            {
                if (string.IsNullOrWhiteSpace(tb_data_qa_detail.Text))
                {
                    MessageBox.Show($"กรุณากรอกข้อมูลในช่องรายละเอียด เนื่องจากเลือก NG สำหรับ Judgment เอกสาร Inspection", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                propQA.judge = "6";

                if (employee.EMP_LEVEL == "1") propQA.judge = "0";
            }
            else if (rb_qa_ok.Checked)
            {
                propQA.judge = "1";
            }
            else
            {
                MessageBox.Show($"กรุณาเลือก OK หรือ NG ก่อนบันทึก", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            propQA.data_detail = tb_data_qa_detail.Text;
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

                pdf_data_check.Dispose();
                pdf_data_check = null;

                CoFreeUnusedLibraries(); // เรียกหลังจาก dispose

                loadstatus();
                bt_status_data_pending_Click();
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

        private void bt_status_data_pending_Click()
        {
            userControlSelectInspDataPending usrSelectInspDataPending = new userControlSelectInspDataPending();
            usrSelectInspDataPending.Dock = DockStyle.Fill;
            usrSelectInspDataPending.propQA = propQA;


            Form mainForm = this.FindForm();

            if (mainForm != null)
            {
                Control[] foundPanels = mainForm.Controls.Find("panelMain", true);
                //Control[] foundPanels = this.Controls.Find("panelMain", true);

                if (foundPanels.Length > 0 && foundPanels[0] is Panel panelMain)
                {
                    // เคลียร์และเพิ่ม UserControl ใหม่
                    this.Controls.Clear();
                    this.Controls.Add(usrSelectInspDataPending);
                    usrSelectInspDataPending.BringToFront();
                }
                else
                {
                    MessageBox.Show("ไม่พบ หน้าจอหลัก panelMain", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tb_data_qa_detail_TextChanged(object sender, EventArgs e)
        {
            lb_length_qa_detail.Text = $"{tb_data_qa_detail.Text.Length} /255";
            if (tb_data_detail.Text.Length > 255)
            {
                MessageBox.Show("ข้อความห้ามเกิน 255 อักขระ", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb_data_qa_detail.Text = tb_data_qa_detail.Text.Substring(0, 255);
                tb_data_qa_detail.SelectionStart = tb_data_qa_detail.Text.Length;
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
