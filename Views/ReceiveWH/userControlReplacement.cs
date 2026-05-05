using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RawMat.Utilities;
using System.Configuration;
using System.IO;
using MySqlX.XDevAPI.Relational;
using RawMat.Views.CustomMsg;


namespace RawMat.Views.ReceiveMat
{
    public partial class userControlReplacement : UserControl
    {
        DataTable dt = new DataTable();
        public QAdataProperty propQA = new QAdataProperty();
        public PronesControllers conPrones = new PronesControllers();
        public QAdataControllers conQA = new QAdataControllers();

        string keepDataFolderPath = ConfigurationManager.AppSettings["KeepDataPath"];
        string TempFolderPath = ConfigurationManager.AppSettings["TempPath"];

        EmployeeProperty employee = EmployeeManager.CurrentEmployee;

        imgCls imgCls = new imgCls();
        public event Action<UserControl> AddUserControlRequested;

        public userControlReplacement()
        {
            InitializeComponent();

            //dtg_receiveMatRep.DataBindingComplete -= dgv_RepDataBindingComplete;
            dtg_receiveMatRep.DataSource = conQA.SearchReplacement();
            dtg_receiveMatRep.DataBindingComplete += dgv_RepDataBindingComplete;
        }



        private void bt_okRep_Click(object sender, EventArgs e)
        {
            // Clear the DataGridView rows
            //dtg_receiveMat.Rows.Clear();

            // Check if all textboxes have data
            if (string.IsNullOrWhiteSpace(tb_mcode.Text))
            {
                MessageBox.Show("Please enter M-Code", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(tb_matName.Text))
            {
                MessageBox.Show("Please enter Material Name", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(tb_vendor.Text))
            {
                MessageBox.Show("Please enter Vendor Name", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(tb_qty.Text))
            {
                MessageBox.Show("Please enter Quantity", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Generate new report number
            QAdataProperty dataItem = new QAdataProperty
            {
                dtReceiveDate = dtp_recDate.Value,
                dtToday = conQA.SearchToday()
            };

            //propQA.dtToday = conQA.SearchToday();

            string newReportNo = conQA.PrefixReportRunNumber(dataItem);

            //// Show details of each textbox
            //string details = $"M-Code: {tb_mcode.Text}\nMaterial Name: {tb_matName.Text}\nVendor Name: {tb_vendor.Text}\nQuantity: {tb_qty.Text}";

            //// Show confirmation dialog
            //DialogResult result = MessageBox.Show($"The data will be processed. Do you want to continue?\n" + $"New Report Number: {newReportNo}\n\nDetails:\n{details}", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (result == DialogResult.No)
            //{
            //    return;
            //}


            //// Show details of each textbox
            //string details = $"M-Code: {tb_mcode.Text}\nMaterial Name: {tb_matName.Text}\nVendor Name: {tb_vendor.Text}\nQuantity: {tb_qty.Text}";

            // Show confirmation dialog using CustomMsgBoxYesNo
            using (CustomMsgBoxYesNo msgBox = new CustomMsgBoxYesNo())
            {
                // Set message - assuming the form has a Label control named lblMessage
                // You might need to add this Label control to your CustomMsgBoxYesNo form designer
                msgBox.Message = $"Create Check Sheet\n" +
                                       $"Report No.: {newReportNo} หรือไม่";
                msgBox.Icon = CustomMsgBoxBase.MessageBoxIconType.Question;
                msgBox.Title = "กรุณายืนยัน";
                msgBox.ShowDialog();

                if (!msgBox.IsYesClicked)
                {
                    return;
                }
            }

            dataItem.M_CODE = tb_mcode.Text;
            dataItem.Report_No = newReportNo;
            dataItem.Invoice_No = tb_inv_no.Text;
            dataItem.Vendor_Name = tb_vendor.Text;
            dataItem.Material_Name = tb_matName.Text;
            dataItem.Qty = tb_qty.Text;
            dataItem.process = "Keep_Data";
            dataItem.Receive_Date = dataItem.dtReceiveDate.ToString("yyyy-MM-dd");
            dataItem.EMP_ID = employee.EMP_CODE;

           
            int foundList = conQA.SearchInspectionList(dataItem);
            if (foundList == 1)
            {

                //มีใน zz5 , check ว่าต้อง skip document/regular
                int keep = conQA.NeedKeepData(dataItem);
                dataItem.process = "Keep_Data";

                //check spec m-code keep-data?
                if (keep == 1)
                {
                    string keepDataFolderPath = ConfigurationManager.AppSettings["KeepDataPath"];
                    string fileKeepData = Path.Combine(keepDataFolderPath, dataItem.dtReceiveDate.ToString("yyyy"), dataItem.dtReceiveDate.ToString("yyyyMMdd") + "_" + dataItem.M_CODE + "_Replacement" + ".pdf");
                    if (File.Exists(fileKeepData))
                    {
                        dataItem.inProcStatus = "8";
                        dataItem.keep_data_status = "1";
                        dataItem.data_check_status = "";
                        dataItem.reportStatus = "8";
                        dataItem.Report_Type = "1";
                    }
                    else
                    {
                        dataItem.inProcStatus = "6";
                        dataItem.keep_data_status = "6";
                        dataItem.reportStatus = "6";
                        dataItem.Report_Type = "1";
                        //MessageBox.Show("Please check " + dataItem.M_CODE + "_replacement", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        //return;
                    }
                }
                else
                {
                    dataItem.inProcStatus = "8";
                    dataItem.keep_data_status = "3";
                    dataItem.data_check_status = "3";
                    dataItem.reportStatus = "8";
                    dataItem.Report_Type = "1";
                    //ยังขาด total
                    //
                    //if (conQA.UpdateStatus(dataItem) == true)
                    //{
                    //    //row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/cart1.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                    //}
                }

                //skip regular
                dataItem.reg_check_status = "3";


                int makeFunc = conQA.NeedFunctionCheck(dataItem);

                if (makeFunc == 1)
                {
                    dataItem.func_check_status = "";
                }
                else
                {
                    dataItem.func_check_status = "3";
                }

                int makeDim = conQA.NeedDimensionCheck(dataItem);

                if (makeDim == 1)
                {
                    dataItem.dim_check_status = "";
                }
                else
                {
                    dataItem.dim_check_status = "3";
                }

                int makeApp = conQA.NeedAppearCheck(dataItem);

                if (makeApp == 1)
                {
                    dataItem.app_check_status = "";
                }
                else
                {
                    dataItem.app_check_status = "3";
                }


            }
            else
            {
    
                // เป็น temp => เป็น monitor 
                string TempFolderPath = ConfigurationManager.AppSettings["MonitorPath"];
                //string fileTemp = Path.Combine(TempFolderPath, dataItem.dtReceiveDate.ToString("yyyy"), dataItem.Vendor_Name, dataItem.dtReceiveDate.ToString("yyyyMMdd") + "-" + dataItem.M_CODE + "_Replacement" + ".pdf");
                string fileTemp = Path.Combine(TempFolderPath, dataItem.M_CODE + ".pdf");

                if (File.Exists(fileTemp))
                {
                    dataItem.inProcStatus = "8";
                    dataItem.reportStatus = "8";
                    dataItem.keep_data_status = "1";
                    dataItem.reg_check_status = "3";
                    dataItem.dim_check_status = "3";
                    dataItem.func_check_status = "3";
                    dataItem.app_check_status = "3";
                    dataItem.Report_Type = "2";

                }
                else
                {
                    dataItem.inProcStatus = "6";
                    dataItem.reportStatus = "6";
                    dataItem.keep_data_status = "6";
                    dataItem.reg_check_status = "3";
                    dataItem.dim_check_status = "3";
                    dataItem.func_check_status = "3";
                    dataItem.app_check_status = "3";
                    dataItem.Report_Type = "2";

                    //insert ไป แล้วติด ไม่เจอไฟล์

                    //MessageBox.Show("Please check " + dataItem.M_CODE + "TempPath :" + fileTemp, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    //return;
                }
            }

            if (conQA.InsertReportStatusAndReceiveMat(dataItem) == true)
            {
                //conQA.UpdateStatus(dataItem);
                dtg_receiveMatRep.DataSource = conQA.SearchReplacement();
                using (CustomMsgBox msgBox = new CustomMsgBox())
                {
                    // Set message - assuming the form has a Label control named lblMessage
                    // You might need to add this Label control to your CustomMsgBox form designer
                    msgBox.Message = $"เสร็จแล้ว";
                    msgBox.Icon = CustomMsgBoxBase.MessageBoxIconType.OK;
                    msgBox.ShowDialog();

                }
                clearData();
            }
            else
            {
                MessageBox.Show("ไม่สามารถนำ Report No:" + dataItem.Report_No + " เข้า database ได้"); 
            }

            // Call LoadStatus from frmMain
            var parentForm = this.FindForm() as frmMain;
            parentForm?.LoadStatus();
        }

        private void dgv_RepDataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (!dtg_receiveMatRep.Columns.Contains("STATUS"))
            {
                // สร้างคอลัมน์ใหม่
                DataGridViewImageColumn idColumn = new DataGridViewImageColumn
                {
                    Name = "STATUS", // ชื่อของคอลัมน์
                    HeaderText = "STATUS", // ข้อความหัวคอลัมน์
                    ReadOnly = false // สามารถแก้ไขได้ (หรือจะตั้งเป็น true ถ้าต้องการให้แก้ไขไม่ได้)
                };

                // เพิ่มคอลัมน์ลงใน DataGridView
                dtg_receiveMatRep.Columns.Add(idColumn); // เพิ่มที่ตำแหน่งแรก
            }

            foreach (DataGridViewRow row in dtg_receiveMatRep.Rows)
            {
                if (row.IsNewRow) continue;
                if (row.Cells["keep_data"].Value.ToString() == "0" || row.Cells["keep_data"].Value.ToString() == "6")
                {
                    row.Cells["keep_data"].Value = imgCls.ResizeImage(Image.FromFile("img/stop.png"), 24, 24);
                }
                else if ((row.Cells["Receive_WH"].Value.ToString() == "8" || row.Cells["keep_data"].Value.ToString() == "3") && row.Cells["Report_Type"].Value.ToString() == "1")
                {
                    row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/cart1.png"), 24, 24);
                }
                else if ((row.Cells["Receive_WH"].Value.ToString() == "8" || row.Cells["keep_data"].Value.ToString() == "3") && row.Cells["Report_Type"].Value.ToString() == "2")
                {
                    row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/cart2.png"), 24, 24);
                }
                else
                {
                    dtg_receiveMatRep.Rows.Remove(row);
                }
            }

            dtg_receiveMatRep.Columns["keep_data"].Visible = false;
            dtg_receiveMatRep.Columns["Receive_WH"].Visible = false;
            dtg_receiveMatRep.Columns["Report_Type"].Visible = false;
        }


        private void tb_mcode_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.KeyCode == Keys.Enter)
            {
                QAdataProperty dataItem = new QAdataProperty()
                {
                    M_CODE = tb_mcode.Text.ToString()
                };

                dt = conQA.SearchMcodeSmartFFTOnly(dataItem);

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        this.tb_matName.Text = dr["material_name"].ToString();
                        this.tb_vendor.Text = dr["VENDOR_NAME"].ToString();
                        //this.txt_OrderReason.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Please check M-Code!!!!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }
        }

        private void userControlReplacement_Load(object sender, EventArgs e)
        {
            //date time จาก database โดยตรง
            DateTime today = conQA.SearchToday();
            dtp_recDate.Value = today;

            //show data ที่เป็น replacement 
            
        }

        private void dtg_receiveMatRep_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // ตรวจสอบว่าแถวที่คลิกเป็นแถวที่สามารถลบได้
            if (e.RowIndex >= 0 && e.RowIndex < dtg_receiveMatRep.Rows.Count)
            {
                QAdataProperty qaProp = new QAdataProperty();

                qaProp.EMP_ID = employee.EMP_CODE;

                qaProp.Receive_Date = Convert.ToDateTime(dtg_receiveMatRep.Rows[e.RowIndex].Cells["Receive_Date"].Value).ToString("yyyy-MM-dd");
                // process
                qaProp.process = "keep_data";

                // report_No
                qaProp.Report_No = dtg_receiveMatRep.Rows[e.RowIndex].Cells["REPORT_NO"].Value.ToString();

                // mcode
                qaProp.M_CODE = dtg_receiveMatRep.Rows[e.RowIndex].Cells["M_CODE"].Value.ToString();

                // invoice
                qaProp.Invoice_No = dtg_receiveMatRep.Rows[e.RowIndex].Cells["INVOICE_NO"].Value.ToString();

                DataTable dt = new DataTable();
                dt = conQA.CheckStatusReplacement(qaProp);

                if (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(qaProp?.process))
                {
                    // 1=ok , 3=skipProcess
                    if (dt.Rows[0][qaProp.process].ToString() == "1" || dt.Rows[0][qaProp.process].ToString() == "3")
                    {
                        qaProp.inProcStatus = "1";
                        qaProp.reportStatus = "1";
                        
                        if (conQA.UpdateDataReceiveWH(qaProp) == true)
                        {
                            CustomMsgBoxBase.ShowCustomMessageBox(
                               "เก็บ data M-Code : " + qaProp.M_CODE + " เรียบร้อยแล้ว",
                               "เรียบร้อยแล้ว",
                               CustomMsgBoxBase.MessageBoxIconType.OK);

                            dtg_receiveMatRep.DataSource = conQA.SearchReplacement();
                        }
                        else
                        {
                            CustomMsgBoxBase.ShowCustomMessageBox(
                               "เก็บ data :" + qaProp.Report_No + " ไม่สำเร็จ",
                               "ไม่สำเร็จ",
                               CustomMsgBoxBase.MessageBoxIconType.NG);
                            //MessageBox.Show("Update report no :" + qaProp.Report_No + " ลงดาต้าเบสไม่สำเร็จ");
                            // row.Cells["INS_DATA"].Value = imgCls.ResizeImage(Image.FromFile("img/stop.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                        }
                    }
                }
            }
        }

        void clearData()
        {
            tb_mcode.Text = "";
            tb_matName.Text = "";
            tb_vendor.Text = "";
            tb_qty.Text = "";
        }

        private void tb_qty_KeyPress(object sender, KeyPressEventArgs e)
        {
            // อนุญาตให้กดเฉพาะตัวเลข (0-9) และปุ่ม Backspace
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // ไม่ให้พิมพ์ตัวอักษรที่ไม่ใช่ตัวเลข
            }
        }

        private void tb_qty_OnValueChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(tb_qty.Text, "[^0-9]"))
            {
                MessageBox.Show("กรุณากรอกตัวเลขเท่านั้น!", "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                tb_qty.Text = System.Text.RegularExpressions.Regex.Replace(tb_qty.Text, "[^0-9]", "");
            }
        }
    }
}
