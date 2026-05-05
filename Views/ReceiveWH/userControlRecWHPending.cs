using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.CustomMsg;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Views.ReceiveWH
{
    public partial class userControlRecWHPending : UserControl
    {
        QAdataControllers qaCon = new QAdataControllers();
        
        imgCls imgCls = new imgCls();
        string keepDataFolderPath = ConfigurationManager.AppSettings["KeepDataPath"];
        string monitorFolderPath = ConfigurationManager.AppSettings["MonitorPath"];


        public userControlRecWHPending()
        {
            InitializeComponent();
        }

        private void userControlRecWHPending_Load(object sender, EventArgs e)
        {
            QAdataProperty qaProp = new QAdataProperty();
            qaProp.process = "keep_data";

            //dtg_recWHPending.DataSource = qaCon.SearchProcessStatusPending(qaProp);

            dtg_recWHPending.DataSource = qaCon.SearchProcessStatusPending(qaProp);
            dtg_recWHPending.DataBindingComplete += dgv_DataBindingComplete;
            //show datagrid
            //add picture
            //
        }



        private void dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (!dtg_recWHPending.Columns.Contains("STATUS"))
            {
                DataGridViewImageColumn idColumn = new DataGridViewImageColumn
                {
                    Name = "STATUS",
                    HeaderText = "STATUS",
                    ReadOnly = false
                };
                dtg_recWHPending.Columns.Add(idColumn);
            }

            if (!dtg_recWHPending.Columns.Contains("CANCEL"))
            {
                DataGridViewImageColumn cancelColumn = new DataGridViewImageColumn
                {
                    Name = "CANCEL",
                    HeaderText = "CANCEL",
                    ReadOnly = false

                };
                dtg_recWHPending.Columns.Add(cancelColumn);
            }

            foreach (DataGridViewRow row in dtg_recWHPending.Rows)
            {
                if (row.IsNewRow) continue;

                if (row.Cells["keep_data"].Value.ToString() == "6")
                {
                    row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/stop.png"), 24, 24);
                }


                if (row.Cells["Invoice_No"].Value.ToString() != "Replacement")
                {
                    row.Cells["CANCEL"].Value = imgCls.ResizeImage(Image.FromFile("img/gray.png"), row.Cells["CANCEL"].Size.Width, row.Cells["CANCEL"].Size.Height);
                    //row.Cells["CANCEL"].Style.BackColor = Color.Gray;
                    //row.Cells["CANCEL"].Style.SelectionBackColor = Color.Gray;
                    //row.Cells["CANCEL"].Value = imgCls.ResizeImage(Image.FromFile("img/gray.png"), 24, 24);
                }
                else
                {
                    row.Cells["CANCEL"].Value = imgCls.ResizeImage(Image.FromFile("img/delete.png"), 24, 24);
                    row.Cells["CANCEL"].ReadOnly = false;
                    row.Cells["CANCEL"].Style.ForeColor = Color.Black;
                    row.Cells["CANCEL"].Style.SelectionForeColor = Color.Black;
                    row.Cells["CANCEL"].Style.BackColor = Color.White;
                    row.Cells["CANCEL"].Style.SelectionBackColor = Color.White;
                }

            }

            dtg_recWHPending.Columns["keep_data"].Visible = false;
            dtg_recWHPending.Columns["Report_Type"].Visible = false;
        }

        private void dtg_recWHPending_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dtg_recWHPending_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Ensure the clicked row is within the valid range
            if (e.RowIndex >= 0 && e.RowIndex < dtg_recWHPending.Rows.Count && e.ColumnIndex >= 0 && e.ColumnIndex < dtg_recWHPending.Columns.Count)
            {
                QAdataProperty qaProp = new QAdataProperty();


                DateTime.TryParse(dtg_recWHPending.Rows[e.RowIndex].Cells["Receive_Date"].Value.ToString(), out DateTime dateValue);
                qaProp.dtReceiveDate = dateValue;
                qaProp.M_CODE = dtg_recWHPending.Rows[e.RowIndex].Cells["M_Code"].Value.ToString();
                qaProp.Vendor_Name = dtg_recWHPending.Rows[e.RowIndex].Cells["Vendor_Name"].Value.ToString();
                qaProp.Report_No = dtg_recWHPending.Rows[e.RowIndex].Cells["Report_No"].Value.ToString();
                qaProp.Report_Type = dtg_recWHPending.Rows[e.RowIndex].Cells["Report_Type"].Value.ToString();
                
                // Check if the clicked column is "STATUS"
                if (dtg_recWHPending.Columns[e.ColumnIndex].Name == "STATUS")
                {
                    //string subfolderVendor = qaProp.Vendor_Name;

                    //if (!string.IsNullOrEmpty(subfolderVendor) && subfolderVendor.EndsWith("."))
                    //{
                    //    subfolderVendor = subfolderVendor.Substring(0, subfolderVendor.Length - 1);
                    //}


                    string file = "";
                    if (qaProp.Report_Type == "1")
                    {
                        if (dtg_recWHPending.Rows[e.RowIndex].Cells["Invoice_No"].Value.ToString() == "Replacement")
                        {
                            file = Path.Combine(keepDataFolderPath, qaProp.dtReceiveDate.ToString("yyyy"), qaProp.dtReceiveDate.ToString("yyyyMMdd") + "_" + qaProp.M_CODE + "_Replacement" + ".pdf");
                        }
                        else
                        {
                            file = Path.Combine(keepDataFolderPath, qaProp.dtReceiveDate.ToString("yyyy"), qaProp.dtReceiveDate.ToString("yyyyMMdd") + "_" + qaProp.M_CODE + ".pdf");
                        }
                    }
                    else if (qaProp.Report_Type == "2")
                    {
                        file = Path.Combine(monitorFolderPath, qaProp.M_CODE + ".pdf");
                    }
                    else
                    {
                        MessageBox.Show("ไม่พบ Report Type ที่ต้องการ");
                        return;
                    }

                    if (File.Exists(file))
                    {
                        qaProp.process = "keep_data";
                        qaProp.inProcStatus = "1";
                        qaProp.reportStatus = "1";
                        if (qaCon.UpdateStatus(qaProp) == true)
                        {
                            
                           // qaProp.process = "Receive_WH";
                           // qaProp.inProcStatus = "1";
                           // qaProp.reportStatus = "1";

                           //if (qaCon.UpdateStatus(qaProp) == true) ;
                           // {
                            CustomMsgBoxBase.ShowCustomMessageBox(
                            "เรียบร้อยแล้ว M-Code : " + qaProp.M_CODE + " พบ Data Inspection",
                            "เรียบร้อยแล้ว",
                            CustomMsgBoxBase.MessageBoxIconType.OK);
                            //}

                            loadstatus();
                            dtg_recWHPending.Rows.RemoveAt(e.RowIndex);
                        }
                        else
                        {
                            CustomMsgBoxBase.ShowCustomMessageBox(
                              "M-Code :" + qaProp.M_CODE + " พบดาต้าเบส Error โปรดลองอีกครั้ง",
                              "ไม่สำเร็จ",
                              CustomMsgBoxBase.MessageBoxIconType.NG, backColor: Color.Red); 
                        }
                    }
                    else
                    {
                        CustomMsgBoxBase.ShowCustomMessageBox(
                            $"Report No. : {qaProp.Report_No} นี้" + "\n" + "path ที่ไม่พบ : " + file,
                            "กรุณาตรวจสอบ!", CustomMsgBoxBase.MessageBoxIconType.NG,fontSize: 12f, backColor: Color.Red);
                    }

                    return;
                }

                if (dtg_recWHPending.Columns[e.ColumnIndex].Name == "CANCEL" && dtg_recWHPending.Rows[e.RowIndex].Cells["Invoice_No"].Value.ToString() == "Replacement")
                {
                    // Show confirmation dialog
                    bool result = CustomMsgBoxBase.ShowCustomMessageBox(
                        $"คุณต้องการยกเลิก Report No:{qaProp.Report_No} นี้หรือไม่?",
                        "ยืนยันการยกเลิก",
                        CustomMsgBoxBase.MessageBoxIconType.Question,
                        CustomMsgBoxBase.MessageBoxDialogType.YesNo
                    );

                    if (result)
                    {

                        qaProp.process = "keep_data";
                        qaProp.inProcStatus = "5";
                        qaProp.reportStatus = "5";

                        bool isDoc = qaCon.UpdateStatus(qaProp);

                        qaProp.process = "Receive_WH";
                        qaProp.inProcStatus = "";
                        qaProp.reportStatus = "5";

                        bool isWH = qaCon.UpdateStatus(qaProp);

                        if (isDoc == true && isWH == true)
                        {
                            CustomMsgBoxBase.ShowCustomMessageBox("Cancel data M-Code : " + qaProp.M_CODE + "\n" + "Report No." + qaProp.Report_No + " เรียบร้อยแล้ว", 
                                "สำเร็จ", 
                                CustomMsgBoxBase.MessageBoxIconType.OK);

                            loadstatus();
                            dtg_recWHPending.Rows.RemoveAt(e.RowIndex);
                        }
                        else
                        {
                            CustomMsgBoxBase.ShowCustomMessageBox("Cancel data ล้มเหลว ที่ report :" + qaProp.Report_No, "ไม่สำเร็จ", CustomMsgBoxBase.MessageBoxIconType.NG, backColor: Color.Red);
                        }

                    }
                }
            }
        }
        private void loadstatus()
        {
            if (this.ParentForm is frmMain mainForm)
            {
                mainForm.LoadStatus();
            }
        }

        private void dtg_recWHPending_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
