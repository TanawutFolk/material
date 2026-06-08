using RawMat.Controllers;
using RawMat.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Views.RegularCheck
{
    public partial class userControlSelectRegularPending : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        public userControlSelectRegularPending()
        {
            InitializeComponent();
        }

        private void userControlSelectRegularPending_Load(object sender, EventArgs e)
        {
            propQA.process = "Regular_Check";

            DataTable dt = new DataTable();
            dt = conQA.SearchForRegularPending(propQA);

            dtg_regularPending.DataSource = dt;

            dtg_regularPending.Columns["process_status_id"].Visible = false;
            dtg_regularPending.Columns["Issue_Date"].Visible = false;
        }

        private void dtg_regularPending_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dtg_regularPending.Rows.Count && e.ColumnIndex >= 0 && e.ColumnIndex < dtg_regularPending.Columns.Count)
            {

                propQA.Regular_No = dtg_regularPending.Rows[e.RowIndex].Cells["Regular No"].Value.ToString();
                propQA.Report_No = dtg_regularPending.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                propQA.Invoice_No = dtg_regularPending.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                propQA.M_CODE = dtg_regularPending.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                propQA.Material_Name = dtg_regularPending.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                propQA.Qty = GetCellValue(e.RowIndex, "Lot Size");
                propQA.Vendor_Name = GetCellValue(e.RowIndex, "Vendor");
                propQA.dtReceiveDate = DateTime.Parse(dtg_regularPending.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());

                //if (dtg_regularPending.Columns[e.ColumnIndex].Name == "REF")
                //{
                //    propQA.CHECK_REGULAR_REF = dtg_regularPending.Rows[e.RowIndex].Cells["Check_Regular_Ref"].Value.ToString();
                //    DataTable dtRef = new DataTable();
                //    dtRef = conQA.SearchRegularRef(propQA);
                //    if (dtRef.Rows.Count == 0 || dtRef == null)
                //    {
                //        MessageBox.Show("ยังไม่พบ data ที่จะทำการ Reference ข้อมูล");
                //        return;
                //    }
                //    else
                //    {
                //        propQA.mRef = dtRef.Rows[0]["M_CODE"].ToString();
                //        propQA.mSelect = propQA.M_CODE;

                //        DataTable checkConRegularRef = conQA.CheckConditionRegularRef(propQA);
                //        if (checkConRegularRef == null)
                //        {
                //            return;
                //        }

                //        if (checkConRegularRef.Rows[0]["mSelect"].ToString() == string.Empty || checkConRegularRef.Rows[0]["mRef"].ToString() == string.Empty)
                //        {
                //            MessageBox.Show("ยังไม่พบ data การ Reference ข้อมูลของ M-code : " + propQA.mRef + "กับ M-code " + propQA.mSelect);
                //            return;
                //        }

                //        if (checkConRegularRef.Rows[0]["Compare_Result"].ToString() == "NOT MATCH")
                //        {
                //            MessageBox.Show("ข้อมูลของ M-code : " + propQA.mRef + "กับ M-code " + propQA.mSelect + " ไม่ Match กัน");
                //            return;
                //        }

                //        var result = MessageBox.Show(
                //      "ต้องการ Ref หรือไม่ ",
                //      "ยืนยันการดำเนินการ",
                //      MessageBoxButtons.YesNo,
                //      MessageBoxIcon.Warning
                //        );

                //        if (result == DialogResult.No)
                //        {
                //            // หยุดการทำงาน (หรือคืนค่าเดิม ถ้าจำเป็น)
                //            return;
                //        }
                //        else
                //        {
                //            //key report_NO => update  
                //            //update dtRef[RI]

                //            //update status report = 1 inprocess , 1  reportstatus
                //        }
                //        //mcode ref 
                //        //mcode 

                //    }
                //    //


                //    MessageBox.Show("Reference Ready");
                //    return;
                //}

                //if (dtg_regularPending.Rows[e.RowIndex].Cells["process_id"].Value.ToString() == "2")
                //{

                //    // แสดงข้อความถามผู้ใช้
                //    var result = MessageBox.Show(
                //        "มีคนทำงานอยู่ หรือยังไม่เสร็จ คุณต้องการทำต่อหรือไม่?",
                //        "ยืนยันการดำเนินการ",
                //        MessageBoxButtons.YesNo,
                //        MessageBoxIcon.Warning
                //    );

                //    if (result == DialogResult.No)
                //    {
                //        // หยุดการทำงาน (หรือคืนค่าเดิม ถ้าจำเป็น)
                //        return;
                //    }

                //}

                //regular sampling type

                propQA.dtRegSamp = conQA.RegularSampling(propQA);
                if (propQA.dtRegSamp == null)
                {
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

                        }
                        else if (propQA.SAMPLING_TYPE == "3")
                        {


                            //คำนวณ Strictness Table sampling  จาก lotsize 
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

                userControlRegularPending usrRegPending = new userControlRegularPending();
                usrRegPending.Dock = DockStyle.Fill;
                usrRegPending.propQA = propQA;

                this.Controls.Clear();
                this.Controls.Add(usrRegPending);

            }
        }

        private string GetCellValue(int rowIndex, string columnName)
        {
            if (!dtg_regularPending.Columns.Contains(columnName))
            {
                return string.Empty;
            }

            object value = dtg_regularPending.Rows[rowIndex].Cells[columnName].Value;
            return value == null || value == DBNull.Value ? string.Empty : value.ToString();
        }
    }
}
