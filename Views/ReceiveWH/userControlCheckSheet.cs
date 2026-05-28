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
using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using System.Net.NetworkInformation;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using RawMat.Login;
using RawMat.Views.CustomMsg;
namespace RawMat.Views.ReceiveWH
{
    public partial class userControlCheckSheet : UserControl
    {
        PronesControllers conPrones = new PronesControllers();
        QAdataControllers conQA = new QAdataControllers();
        imgCls imgCls = new imgCls();
        private BackgroundWorker bgWorker = new BackgroundWorker();
        //private BackgroundWorker bgWorkerHeavy; // สำหรับงานหนัก
        private Timer loadingTimer; // เพิ่ม Timer เพื่อควบคุมการอัปเดต UI
        EmployeeProperty employee = EmployeeManager.CurrentEmployee;


        public userControlCheckSheet()
        {
            InitializeComponent();
            //bgWorker.DoWork += BgWorker_DoWork;
            //bgWorker.RunWorkerCompleted += BgWorker_RunWorkerCompleted;
            //bgWorker.WorkerReportsProgress = false;
            //bgWorker.WorkerSupportsCancellation = false;

            loadingTimer = new Timer();
            loadingTimer.Interval = 100;
            loadingTimer.Tick += LoadingTimer_Tick;

        }

        // Event Handler ของ Timer
        private void LoadingTimer_Tick(object sender, EventArgs e)
        {
            if (!pgbOkSearch.Visible)
            {
                return;
            }

            if (pgbOkSearch.Value < 95)
            {
                pgbOkSearch.Value = Math.Min(pgbOkSearch.Value + 2, 95);
            }
        }

        private async void bt_okCheckSheet_Click(object sender, EventArgs e)
        {
            //picLoading.Visible = true;
            //picLoading.Visible = true;
            //bgWorker.RunWorkerAsync(); // ให้ BackgroundWorker ทำงาน //Test
            SetSearchProgressVisible(true);
            bool success = false;

            try
            {
                success = await Task.Run(() => ProcessData()); // เรียกฟังก์ชัน async
            }
            finally
            {
                SetSearchProgressVisible(false);
            }

            if (success)
            {
                //MessageBox.Show("การประมวลผลเสร็จสิ้นเรียบร้อย", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void SetSearchProgressVisible(bool isVisible)
        {
            pgbOkSearch.Style = ProgressBarStyle.Continuous;
            pgbOkSearch.Minimum = 0;
            pgbOkSearch.Maximum = 100;

            if (isVisible)
            {
                pgbOkSearch.Value = 0;
                pgbOkSearch.Visible = true;
                loadingTimer.Start();
            }
            else
            {
                loadingTimer.Stop();
                pgbOkSearch.Value = 100;
                pgbOkSearch.Visible = true;
            }

            bt_okCheckSheet.Enabled = !isVisible;
        }

        private bool ProcessData()
        {
            try
            {
                PronesProperty propPrones = new PronesProperty();
                QAdataProperty propQA = new QAdataProperty();

                propPrones.rec_date = dtp_recDate.Value.ToString("yyyy-MM-dd");
                propQA.dtReceiveDate = dtp_recDate.Value;
                propQA.dtToday = conQA.SearchToday();
                propQA.Receive_Date = propPrones.rec_date;
                propQA.EMP_ID = employee.EMP_CODE;
                propQA.Report_Type = "1";

                DataTable dataSource = conPrones.SearchRecDate(propPrones);
                if (dataSource.Rows.Count == 0)
                {
                    Invoke(new Action(() => MessageBox.Show("ไม่พบ Data ที่จะ Inspection ในวัน " + dtp_recDate.Value.ToString("yyyy-MM-dd"))));
                    return false;
                }

                DataTable inspectionMaster = conQA.SearchActiveInspectionList();
                if (inspectionMaster == null || inspectionMaster.Rows.Count == 0)
                {
                    Invoke(new Action(() => MessageBox.Show("ไม่พบข้อมูล Inspection List ที่เปิดใช้งานอยู่")));
                    return false;
                }

                DataTable inspectionDataSource = FilterInspectionReceiveRows(dataSource, inspectionMaster);
                if (inspectionDataSource.Rows.Count == 0)
                {
                    Invoke(new Action(() => MessageBox.Show("ไม่พบ M-CODE ที่อยู่ใน Inspection List ในวัน " + dtp_recDate.Value.ToString("yyyy-MM-dd"))));
                    return false;
                }

                bool updateSuccess = false;

                Invoke(new Action(() =>
                {
                    dtg_receiveMat.AutoGenerateColumns = false;
                    dtg_receiveMat.Columns["M_CODE"].DataPropertyName = "ITEM_CD";
                    dtg_receiveMat.Columns["INVOICE_NO"].DataPropertyName = "INVOICE_NO";
                    dtg_receiveMat.Columns["PART_NAME"].DataPropertyName = "ITEM_DESC";
                    dtg_receiveMat.Columns["VENDOR"].DataPropertyName = "DL_DESC";
                    dtg_receiveMat.Columns["GR_QTY"].DataPropertyName = "GR_QTY";
                    dtg_receiveMat.DataSource = inspectionDataSource;
                    // เรียก UpdateDataGridViewWithImage และเก็บผลลัพธ์
                    updateSuccess = UpdateDataGridViewWithImage(dtg_receiveMat, "M_CODE", "STATUS", inspectionMaster);

                    // ดำเนินการต่อเฉพาะเมื่อ updateSuccess เป็น true
                    if (updateSuccess)
                    {
                        screenReceived(dtg_receiveMat, propQA);
                    }
                }));

                // หาก UpdateDataGridViewWithImage ล้มเหลว (เช่น Qty ไม่ถูกต้อง) หยุดที่นี่
                if (!updateSuccess)
                {
                    return false;
                }

                return GenerateIdsForDataGridView(dtg_receiveMat, propQA);
            }
            catch (Exception ex)
            {
                Invoke(new Action(() => MessageBox.Show("Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                return false;
            }
        }

        private DataTable FilterInspectionReceiveRows(DataTable receiveData, DataTable inspectionMaster)
        {
            DataTable filteredData = receiveData.Clone();
            Dictionary<string, DataRow> inspectionMap = BuildInspectionMap(inspectionMaster);

            foreach (DataRow row in receiveData.Rows)
            {
                string mCode = row["ITEM_CD"]?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(mCode))
                {
                    continue;
                }

                if (inspectionMap.ContainsKey(mCode))
                {
                    filteredData.ImportRow(row);
                }
            }

            return filteredData;
        }

        private Dictionary<string, DataRow> BuildInspectionMap(DataTable inspectionMaster)
        {
            Dictionary<string, DataRow> inspectionMap = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);

            if (inspectionMaster == null || !inspectionMaster.Columns.Contains("M_CODE"))
            {
                return inspectionMap;
            }

            foreach (DataRow row in inspectionMaster.Rows)
            {
                string mCode = row["M_CODE"]?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(mCode) || inspectionMap.ContainsKey(mCode))
                {
                    continue;
                }

                inspectionMap.Add(mCode, row);
            }

            return inspectionMap;
        }

        private void BgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            PronesProperty propPrones = new PronesProperty();
            QAdataProperty propQA = new QAdataProperty();

            propPrones.rec_date = dtp_recDate.Value.ToString("yyyy-MM-dd");
            propQA.dtReceiveDate = dtp_recDate.Value;
            propQA.dtToday = conQA.SearchToday();
            propQA.Receive_Date = propPrones.rec_date;
            propQA.EMP_ID = employee.EMP_CODE;
            propQA.Report_Type = "1";

            DataTable dataSource = conPrones.SearchRecDate(propPrones);
            //e.Result = new Tuple<DataTable, QAdataProperty>(dataSource, propQA);
            if (dataSource.Rows.Count == 0)
            {
                MessageBox.Show("ไม่พบ Data ที่จะ Inspection ในวัน " + dtp_recDate.Value.ToString("yyyy-MM-dd"));
                e.Result = false;
                //picLoading.Visible = false;
                return;
            }

            dtg_receiveMat.AutoGenerateColumns = false;
            dtg_receiveMat.Columns["M_CODE"].DataPropertyName = "ITEM_CD";
            dtg_receiveMat.Columns["INVOICE_NO"].DataPropertyName = "INVOICE_NO";
            dtg_receiveMat.Columns["PART_NAME"].DataPropertyName = "ITEM_DESC";
            dtg_receiveMat.Columns["VENDOR"].DataPropertyName = "DL_DESC";
            dtg_receiveMat.Columns["GR_QTY"].DataPropertyName = "GR_QTY";

            

            
            Invoke(new Action(() =>
            {

                dtg_receiveMat.DataSource = dataSource;

                UpdateDataGridViewWithImage(dtg_receiveMat, "M_CODE", "STATUS");

                screenReceived(dtg_receiveMat, propQA);

                
            }));

            bool success = GenerateIdsForDataGridView(dtg_receiveMat, propQA);

            e.Result = success;

        }


        private void BgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Error: " + e.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetSearchProgressVisible(false);
                return;
            }

            bool success = (bool)e.Result;
            SetSearchProgressVisible(false);

            if (!success)
            {
                // ไม่ต้องทำอะไรเพิ่มเติม เพราะ UI และฐานข้อมูลไม่เปลี่ยนแปลง
                return;
            }

        }

        private void screenReceived(DataGridView dataGridView , QAdataProperty dataItem)
        {
            dataItem.process = "Receive_WH";

            if (!dataGridView.Columns.Contains("REPORT_NO"))
            {
                // สร้างคอลัมน์ใหม่
                DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn
                {
                    Name = "REPORT_NO", // ชื่อของคอลัมน์
                    HeaderText = "Report No.", // ข้อความหัวคอลัมน์
                    ReadOnly = false // สามารถแก้ไขได้ (หรือจะตั้งเป็น true ถ้าต้องการให้แก้ไขไม่ได้)
                };

                // เพิ่มคอลัมน์ลงใน DataGridView
                dataGridView.Columns.Insert(0, idColumn); // เพิ่มที่ตำแหน่งแรก
            }

            // สร้างรายการแถวที่จะลบ
            List<DataGridViewRow> rowsToDelete = new List<DataGridViewRow>();
            Dictionary<string, DataRow> receiveStatusMap = BuildReceiveStatusMap(conQA.SearchReceiveMatStatusByReceiveDate(dataItem));

            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                string mCode = row.Cells["M_CODE"].Value?.ToString();
                string invoiceNo = row.Cells["INVOICE_NO"].Value?.ToString();
                string receiveKey = BuildReceiveKey(mCode, invoiceNo);

                if(receiveStatusMap.TryGetValue(receiveKey, out DataRow receiveRow))
                {
                    if (receiveRow["Receive_WH"].ToString() == "1")
                    {
                        // ถ้าไม่เจอข้อมูลในฐานข้อมูล ให้เตรียมลบแถวนี้ออก
                        rowsToDelete.Add(row);
                    }
                    else
                    {
                        row.Cells["REPORT_NO"].Value = receiveRow["REPORT_NO"].ToString();
                    }
                }
                else
                {
                    continue;
                }

            }

            // ลบแถวที่ไม่ต้องการออกจาก DataGridView
            foreach (DataGridViewRow row in rowsToDelete)
            {
                dataGridView.Rows.Remove(row);
            }
        }

        private Dictionary<string, DataRow> BuildReceiveStatusMap(DataTable receiveStatus)
        {
            Dictionary<string, DataRow> receiveStatusMap = new Dictionary<string, DataRow>(StringComparer.OrdinalIgnoreCase);

            if (receiveStatus == null ||
                !receiveStatus.Columns.Contains("M_CODE") ||
                !receiveStatus.Columns.Contains("Invoice_No"))
            {
                return receiveStatusMap;
            }

            foreach (DataRow row in receiveStatus.Rows)
            {
                string receiveKey = BuildReceiveKey(row["M_CODE"]?.ToString(), row["Invoice_No"]?.ToString());
                if (string.IsNullOrWhiteSpace(receiveKey) || receiveStatusMap.ContainsKey(receiveKey))
                {
                    continue;
                }

                receiveStatusMap.Add(receiveKey, row);
            }

            return receiveStatusMap;
        }

        private string BuildReceiveKey(string mCode, string invoiceNo)
        {
            mCode = mCode?.Trim();
            invoiceNo = invoiceNo?.Trim();

            if (string.IsNullOrWhiteSpace(mCode) || string.IsNullOrWhiteSpace(invoiceNo))
            {
                return string.Empty;
            }

            return $"{mCode}|{invoiceNo}";
        }


        private bool GenerateIdsForDataGridView(DataGridView dataGridView, QAdataProperty dataItem)
        {
            //dataItem.process = "Receive_WH";

            if (!dataGridView.Columns.Contains("REPORT_NO"))
            {
                // สร้างคอลัมน์ใหม่
                DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn
                {
                    Name = "REPORT_NO", // ชื่อของคอลัมน์
                    HeaderText = "Report No.", // ข้อความหัวคอลัมน์
                    ReadOnly = false, // สามารถแก้ไขได้ (หรือจะตั้งเป็น true ถ้าต้องการให้แก้ไขไม่ได้)
                    SortMode = DataGridViewColumnSortMode.NotSortable
                };

                // เพิ่มคอลัมน์ลงใน DataGridView
                dataGridView.Columns.Insert(0, idColumn); // เพิ่มที่ตำแหน่งแรก
            }

            // ดึง Last ID ล่าสุดจากฐานข้อมูล lastReportNo

            //Generate Report No
            string strNewReportNo = conQA.PrefixReportRunNumber(dataItem);

            //Generate Regular No
            string strNewRegularNo = conQA.PrefixRegularRunNumber(dataItem);


            // สร้าง Prefix ของปีปัจจุบัน
            string currentPrefixReportNo    = $"QA{dataItem.dtToday.Year % 100}";
            string currentPrefixRegularNo   = $"RI{dataItem.dtToday.Year % 100}{dataItem.dtToday.Month:D2}";

            //// หากไม่มีข้อมูลในฐานข้อมูล ให้เริ่มต้น เลข report no ใหม่
            int nextReportNo = 1; // เริ่มจาก 1
            if (!string.IsNullOrEmpty(strNewReportNo) && strNewReportNo.StartsWith(currentPrefixReportNo))
            {
                // แยกตัวเลขจาก report no
                nextReportNo = int.Parse(strNewReportNo.Substring(strNewReportNo.LastIndexOf('-') + 1));
            }

            //// หากไม่มีข้อมูลในฐานข้อมูล ให้เริ่มต้น เลข regular no ใหม่
            int nextRegularNo = 1; // เริ่มจาก 1
            if (!string.IsNullOrEmpty(strNewRegularNo) && strNewRegularNo.StartsWith(currentPrefixRegularNo))
            {
                // แยกตัวเลขจาก regular no
                nextRegularNo = int.Parse(strNewRegularNo.Substring(strNewRegularNo.LastIndexOf('-') + 1));
            }

            // วนลูปเพื่ออัปเดต DataGridView
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue; // ข้ามแถวเปล่า (New Row)

                // ตรวจสอบว่า REPORT_NO มีค่าเป็น null หรือว่าง
                var reportNoCellValue = row.Cells["REPORT_NO"].Value;
                if (reportNoCellValue != null && !string.IsNullOrEmpty(reportNoCellValue.ToString()))
                {

                    dataItem.M_CODE = row.Cells["M_CODE"].Value.ToString();
                    dataItem.Report_No = row.Cells["REPORT_NO"].Value.ToString();
                    dataItem.Vendor_Name = row.Cells["VENDOR"].Value.ToString();
                    try
                    {
                        int keep = conQA.NeedKeepData(dataItem);
                        dataItem.process = "keep_data";

                        //check spec m-code keep-data?
                        if (keep == 1)
                        {

                            string keepDataFolderPath = ConfigurationManager.AppSettings["KeepDataPath"];
                            //check data
                            string file = Path.Combine(keepDataFolderPath, dataItem.dtReceiveDate.ToString("yyyy"), dataItem.dtReceiveDate.ToString("yyyyMMdd") + "_" + dataItem.M_CODE + ".pdf");
                            //string file = Path.Combine(keepDataFolderPath, dataItem.dtReceiveDate.ToString("yyyy"), dataItem.Vendor_Name, dataItem.dtReceiveDate.ToString("yyyyMMdd") + "_" + dataItem.M_CODE + ".pdf");
                            if (File.Exists(file))
                            {

                                dataItem.inProcStatus = "1";
                                dataItem.reportStatus = "1";
                                if (conQA.UpdateStatus(dataItem) == true)
                                {
                                    row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/cart1.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                                }
                                else
                                {
                                    row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/stop.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                                }
                                //if found == 1
                                //
                                //document 1
                                //image cart1
                                //else
                                //image stop
                            }
                            else
                            {
                                dataItem.inProcStatus = "6";
                                dataItem.reportStatus = "6";
                                if (conQA.UpdateStatus(dataItem) == true)
                                {
                                    row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/stop.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                                }
                                else
                                {
                                    row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/stop.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                                }
                                    
                            }

                        }
                        else
                        {
                            dataItem.inProcStatus = "3";
                            dataItem.reportStatus = "1";
                            //ยังขาด total status
                            if (conQA.UpdateStatus(dataItem) == true)
                            {
                                row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/cart1.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                            }

                            dataItem.process = "Inspection_Data_Check";

                            if (conQA.UpdateStatus(dataItem) == true)
                            {
                               
                            }
                            else
                            {
                                return false;
                            }
                            //else
                            //{
                            //    row.Cells["INS_DATA"].Value = imgCls.ResizeImage(Image.FromFile("img/stop.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                            //}
                        }
                    }
                    catch (Exception ex)
                    {
                        return false;
                    }

                    continue; // ถ้ามีค่าอยู่แล้วให้ข้ามไปยังแถวถัดไป
                }

                
                
                // สร้าง ID ใหม่ เช่น WWE24-0001
                string newId = $"{currentPrefixReportNo}-{nextReportNo:D4}";
                // เพิ่มค่าในคอลัมน์ REPORT_NO
                row.Cells["REPORT_NO"].Value = newId;

                dataItem.M_CODE = row.Cells["M_CODE"].Value.ToString();
                dataItem.Report_No = row.Cells["REPORT_NO"].Value.ToString();
                dataItem.Material_Name = row.Cells["PART_NAME"].Value.ToString();
                dataItem.Invoice_No = row.Cells["INVOICE_NO"].Value.ToString();
                dataItem.Vendor_Name = row.Cells["VENDOR"].Value.ToString();
                dataItem.Qty = row.Cells["GR_QTY"].Value.ToString();

                
                dataItem.keep_data_status = "8";
                dataItem.inProcStatus = "8";
                dataItem.reg_check_status = "8";
                dataItem.reportStatus = "8";


                if (conQA.InsertReportStatusAndReceiveMat(dataItem) == true)
                {

                    int keep = conQA.NeedKeepData(dataItem);
                    dataItem.process = "keep_data";

                    //check spec m-code keep-data?
                    if (keep == 1)
                    {

                        string keepDataFolderPath = ConfigurationManager.AppSettings["KeepDataPath"];
                        //check data
                        dataItem.dtReceiveDate.ToString("yyyyMMdd");

                        //string subfolderVendor = dataItem.Vendor_Name;

                        //if (!string.IsNullOrEmpty(subfolderVendor) && subfolderVendor.EndsWith("."))
                        //{
                        //    subfolderVendor = subfolderVendor.Substring(0, subfolderVendor.Length - 1); //vendor ที่มีจุดท้าย
                        //}

                        string file = Path.Combine(keepDataFolderPath, dataItem.dtReceiveDate.ToString("yyyy"), dataItem.dtReceiveDate.ToString("yyyyMMdd") + "_" + dataItem.M_CODE + ".pdf");
                        if (File.Exists(file))
                        {
                            dataItem.inProcStatus = "1";
                            dataItem.reportStatus = "1";
                            if (conQA.UpdateStatus(dataItem) == true)
                            {
                                row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/cart1.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                            }
                            else
                            {
                                row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/stop.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                            }
                        }
                        else
                        {

                            dataItem.inProcStatus = "6";
                            dataItem.reportStatus = "6";
                            if (conQA.UpdateStatus(dataItem) == true)
                            {
                                row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/stop.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                            }
                            else
                            {
                                row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/stop.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                            }
                        }

                    }
                    else
                    {
                        dataItem.inProcStatus = "3";
                        dataItem.reportStatus = "1";
                        if (conQA.UpdateStatus(dataItem) == true)
                        {
                            row.Cells["STATUS"].Value = imgCls.ResizeImage(Image.FromFile("img/cart1.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                        }

                        dataItem.process = "Inspection_Data_Check";

                        if (conQA.UpdateStatus(dataItem) == true)
                        {

                        }
                        else
                        {
                            return false;
                        }

                    }

                    int makeReg = conQA.NeedRegularCheck(dataItem);

                    if (makeReg == 1)
                    {
                        if (conQA.CheckThisMonthRegular(dataItem) >= 1)
                        {
                            dataItem.process = "Regular_Check";
                            dataItem.inProcStatus = "3";
                            //dataItem.reportStatus = "1";
                            if (conQA.UpdateStatus(dataItem) == true)
                            {

                            }
                            else
                            {
                                return false ;

                            }
                        }
                        else
                        {
                            //null เตรียมทำงาน
                            dataItem.process = "Regular_Check";
                            dataItem.inProcStatus = "";
                            dataItem.Regular_No = $"{currentPrefixRegularNo}-{nextRegularNo:D4}";
                            if (conQA.UpdateRegularNo(dataItem) == true)
                            {
                                nextRegularNo++;
                            }
                            else
                            {
                                return false;
                            }

                            if (conQA.UpdateStatus(dataItem) == true)
                            {

                            }
                            else
                            {
                                return false;

                            }

                        }
                        //strNewRegularNo
                        //
                    }
                    else
                    {
                        dataItem.process = "Regular_Check";
                        dataItem.inProcStatus = "3";
                        //dataItem.reportStatus = "1";
                        if (conQA.UpdateStatus(dataItem) == true)
                        {

                        }
                        else
                        {
                            return false;

                        }
                    }

                    int makeFunc = conQA.NeedFunctionCheck(dataItem);

                    if (makeFunc == 1)
                    {
                        dataItem.process = "Function_Check";
                        dataItem.inProcStatus = "";

                        if (conQA.UpdateStatus(dataItem) == true)
                        {

                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                       
                        dataItem.process = "Function_Check";
                        dataItem.inProcStatus = "3";
                        //dataItem.reportStatus = "1";
                        if (conQA.UpdateStatus(dataItem) == true)
                        {

                        }
                        else
                        {
                            return false;

                        }

                    }

                    int makeDim = conQA.NeedDimensionCheck(dataItem);

                    if (makeDim == 1)
                    {
                        dataItem.process = "Dimension_Check";
                        dataItem.inProcStatus = "";

                        if (conQA.UpdateStatus(dataItem) == true)
                        {

                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {

                        dataItem.process = "Dimension_Check";
                        dataItem.inProcStatus = "3";
                        //dataItem.reportStatus = "1";
                        if (conQA.UpdateStatus(dataItem) == true)
                        {

                        }
                        else
                        {
                            return false;

                        }

                    }

                    int makeApp = conQA.NeedAppearCheck(dataItem);

                    if (makeApp == 1)
                    {
                        dataItem.process = "Appearance_Check";
                        dataItem.inProcStatus = "";

                        if (conQA.UpdateStatus(dataItem) == true)
                        {

                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {

                        dataItem.process = "Appearance_Check";
                        dataItem.inProcStatus = "3";
                        //dataItem.reportStatus = "1";
                        if (conQA.UpdateStatus(dataItem) == true)
                        {

                        }
                        else
                        {
                            return false;

                        }

                    }

                    nextReportNo++; // เพิ่มตัวเลขสำหรับแถวถัดไป
                    continue;
                }
                else
                {
                    //insert
                    return false;
                }
                  
            }

            loadstatus();
            return true;
        }

        public bool UpdateDataGridViewWithImage(DataGridView dataGridView, string columnToSearch, string targetColumn, DataTable inspectionMaster = null)
        {

            QAdataProperty qaProp = new QAdataProperty();
            Dictionary<string, DataRow> inspectionMap = BuildInspectionMap(inspectionMaster ?? conQA.SearchActiveInspectionList());

            // สร้างรายการแถวที่จะลบ
            List<DataGridViewRow> rowsToDelete = new List<DataGridViewRow>();

            // วนลูปแต่ละแถวใน DataGridView
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue; // ข้ามแถวใหม่ที่กำลังสร้าง

                // รับค่าจากคอลัมน์ที่ต้องการใช้ค้นหา
                qaProp.M_CODE = row.Cells[columnToSearch].Value.ToString().Trim();

                if (inspectionMap.TryGetValue(qaProp.M_CODE, out DataRow inspectionRow))
                {
                    if (inspectionRow.Table.Columns.Contains("VENDOR_NAME"))
                    {
                        row.Cells["VENDOR"].Value = inspectionRow["VENDOR_NAME"].ToString();
                    }

                    row.Cells[targetColumn].Value = imgCls.ResizeImage(Image.FromFile("img/cart1.png"), 24, 24); // กำหนดที่อยู่ของไฟล์รูปภาพที่ต้องการใช้
                }
                else
                {
                    // ถ้าไม่เจอข้อมูลในฐานข้อมูล ให้เตรียมลบแถวนี้ออก
                    rowsToDelete.Add(row);
                }
            }

            // ลบแถวที่ไม่ต้องการออกจาก DataGridView
            foreach (DataGridViewRow row in rowsToDelete)
            {
                dataGridView.Rows.Remove(row);
            }

            // ตรวจสอบ Qty ของทุกแถวก่อนเริ่มการประมวลผล
            rowsToDelete.Clear();
            foreach (DataGridViewRow row in dataGridView.Rows)
            {
                if (row.IsNewRow) continue;

                qaProp.Qty = row.Cells["GR_QTY"].Value?.ToString();
                if (string.IsNullOrWhiteSpace(qaProp.Qty) || !int.TryParse(qaProp.Qty, out int qtyValue))
                {
                    MessageBox.Show($"พบ Qty ที่ไม่ถูกต้อง '{qaProp.Qty}' ในแถวที่มี M_CODE: {row.Cells["M_CODE"].Value?.ToString() ?? "N/A"}.\n" +
                                    "การดำเนินการถูกยกเลิก และจะไม่มีการบันทึกหรืออัปเดตข้อมูลใด ๆ",
                                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false; // หยุดและส่ง false กลับ
                }
                else
                {
                    if (qtyValue <= 0)
                    {
                        rowsToDelete.Add(row);
                    }
                }
            }

            foreach (DataGridViewRow row in rowsToDelete)
            {
                dataGridView.Rows.Remove(row);
            }

            return true;

        }

      

        private void dtg_receiveMat_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // ตรวจสอบว่าแถวที่คลิกเป็นแถวที่สามารถลบได้
            if (e.RowIndex >= 0 && e.RowIndex < dtg_receiveMat.Rows.Count)
            {
                QAdataProperty qaProp = new QAdataProperty();

                qaProp.EMP_ID = employee.EMP_CODE;

                // rec_date
                qaProp.dtReceiveDate = dtp_recDate.Value;
                qaProp.Receive_Date = qaProp.dtReceiveDate.ToString("yyyy-MM-dd");
                // process
                qaProp.process = "keep_data";

                // report_No
                qaProp.Report_No = dtg_receiveMat.Rows[e.RowIndex].Cells["REPORT_NO"].Value.ToString();

                // mcode
                qaProp.M_CODE = dtg_receiveMat.Rows[e.RowIndex].Cells["M_CODE"].Value.ToString();

                // invoice
                qaProp.Invoice_No = dtg_receiveMat.Rows[e.RowIndex].Cells["INVOICE_NO"].Value.ToString();

                DataTable dt = new DataTable();
                dt = conQA.CheckStatus(qaProp);

                if (dt != null && dt.Rows.Count > 0 && !string.IsNullOrEmpty(qaProp?.process))
                {

                    string processValue = dt.Rows[0][qaProp.process].ToString();
                    string failMsg;
                    // 1=ok เก็บ , 3=skipProcess ไม่เก็บ
                    // ตรวจสอบว่าค่าเป็น "1" หรือ "3" หรือไม่
                    if (processValue == "1" || processValue == "3")
                    {
                        // --- 1. กำหนดค่าตัวแปรที่จะแสดงผลลัพธ์ตามเงื่อนไข ---
                        //string failMsg;
                        string messagePrefix;
                        Color? msgBackColor = null; // ใช้ Nullable Color

                        if (processValue == "1")
                        {
                            messagePrefix = "พบ data inspection";
                        }
                        else // (processValue == "3")
                        {
                            messagePrefix = "ไม่เก็บ data inspection";
                            //msgBackColor = Color.Orange;
                        }

                        // --- 2. ทำ Logic ส่วนที่เหมือนกัน ---
                        qaProp.inProcStatus = "1";
                        qaProp.reportStatus = "1";

                        if (conQA.UpdateDataReceiveWH(qaProp) == true)
                        {
                            // --- 3. แสดงผลลัพธ์ "สำเร็จ" ---
                            string successMsg = $"เรียบร้อยแล้ว M-Code : {qaProp.M_CODE} {messagePrefix}";

                            CustomMsgBoxBase.ShowCustomMessageBox(
                                    successMsg, "เรียบร้อยแล้ว",
                                    CustomMsgBoxBase.MessageBoxIconType.OK);

                            // ตรวจสอบว่าต้องใส่สีหรือไม่
                            //if (msgBackColor.HasValue)
                            //{
                            //    CustomMsgBoxBase.ShowCustomMessageBox(
                            //        successMsg, "เรียบร้อยแล้ว",
                            //        CustomMsgBoxBase.MessageBoxIconType.OK,
                            //        backColor: msgBackColor.Value);
                            //}
                            //else
                            //{
                            //    CustomMsgBoxBase.ShowCustomMessageBox(
                            //        successMsg, "เรียบร้อยแล้ว",
                            //        CustomMsgBoxBase.MessageBoxIconType.OK);
                            //}

                            loadstatus();
                            dtg_receiveMat.Rows.RemoveAt(e.RowIndex);
                        }
                        else
                        {

                            //if (processValue == "1")
                            //{
                            //    messagePrefix = "พบดาต้าเบส error ลองอีกครั้ง\r\n";
                            //}
                            //else // (processValue == "3")
                            //{
                            //    messagePrefix = "พบดาต้าเบส error ลองอีกครั้ง\r\n";
                            //    //msgBackColor = Color.Orange;
                            //}

                            // --- 4. แสดงผลลัพธ์ "ไม่สำเร็จ" ---
                            failMsg = $"ไม่สำเร็จ M-Code :{qaProp.M_CODE} พบดาต้าเบส error ลองอีกครั้ง";

                            if (msgBackColor.HasValue)
                            {
                                CustomMsgBoxBase.ShowCustomMessageBox(
                                    failMsg, "ไม่สำเร็จ",
                                    CustomMsgBoxBase.MessageBoxIconType.NG,
                                    backColor: Color.Red);
                            }
                            else
                            {
                                CustomMsgBoxBase.ShowCustomMessageBox(
                                    failMsg, "ไม่สำเร็จ",
                                    CustomMsgBoxBase.MessageBoxIconType.NG, backColor: Color.Red);
                            }

                            // row.Cells["INS_DATA"].Value = ... (ส่วนนี้ยังถูก comment ไว้เหมือนเดิม)
                        }
                    }
                    else
                    {
                        failMsg = $"{qaProp.M_CODE} ไม่พบ พบดาต้าเบส error ลองอีกครั้ง";
                        CustomMsgBoxBase.ShowCustomMessageBox(
                                    failMsg, "ไม่สำเร็จ",
                                    CustomMsgBoxBase.MessageBoxIconType.NG, backColor: Color.Red);
                    }
                }
            }
        }

        private void userControlCheckSheet_Load(object sender, EventArgs e)
        {
            //date time จาก database โดยตรง
            DateTime today = conQA.SearchToday();
            dtp_recDate.Value = today;
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
