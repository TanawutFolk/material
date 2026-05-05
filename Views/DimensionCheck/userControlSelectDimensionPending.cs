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

namespace RawMat.Views.DimensionCheck
{
    public partial class userControlSelectDimensionPending : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        public userControlSelectDimensionPending()
        {
            InitializeComponent();
        }

        private void userControlSelectDimensionPending_Load(object sender, EventArgs e)
        {
            propQA.process = "Dimension_Check";

            DataTable dt = new DataTable();
            dt = conQA.SearchForDimensionPending(propQA);

            dtg_dimensionPending.DataSource = dt;

            dtg_dimensionPending.Columns["process_status_id"].Visible = false;
            dtg_dimensionPending.Columns["Issue_Date"].Visible = false;
        }

        private void dtg_dimensionPending_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dtg_dimensionPending.Rows.Count && e.ColumnIndex >= 0 && e.ColumnIndex < dtg_dimensionPending.Columns.Count)
            {

                propQA.Report_No = dtg_dimensionPending.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                propQA.Invoice_No = dtg_dimensionPending.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                propQA.M_CODE = dtg_dimensionPending.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                propQA.Material_Name = dtg_dimensionPending.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                propQA.dtReceiveDate = DateTime.Parse(dtg_dimensionPending.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());

                //if (dtg_DimensionPending.Columns[e.ColumnIndex].Name == "REF")
                //{
                //    propQA.CHECK_Dimension_REF = dtg_DimensionPending.Rows[e.RowIndex].Cells["Check_Dimension_Ref"].Value.ToString();
                //    DataTable dtRef = new DataTable();
                //    dtRef = conQA.SearchDimensionRef(propQA);
                //    if (dtRef.Rows.Count == 0 || dtRef == null)
                //    {
                //        MessageBox.Show("ยังไม่พบ data ที่จะทำการ Reference ข้อมูล");
                //        return;
                //    }
                //    else
                //    {
                //        propQA.mRef = dtRef.Rows[0]["M_CODE"].ToString();
                //        propQA.mSelect = propQA.M_CODE;

                //        DataTable checkConDimensionRef = conQA.CheckConditionDimensionRef(propQA);
                //        if (checkConDimensionRef == null)
                //        {
                //            return;
                //        }

                //        if (checkConDimensionRef.Rows[0]["mSelect"].ToString() == string.Empty || checkConDimensionRef.Rows[0]["mRef"].ToString() == string.Empty)
                //        {
                //            MessageBox.Show("ยังไม่พบ data การ Reference ข้อมูลของ M-code : " + propQA.mRef + "กับ M-code " + propQA.mSelect);
                //            return;
                //        }

                //        if (checkConDimensionRef.Rows[0]["Compare_Result"].ToString() == "NOT MATCH")
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

                //if (dtg_DimensionPending.Rows[e.RowIndex].Cells["process_id"].Value.ToString() == "2")
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

                //Dimension sampling type

                propQA.dtDimSamp = conQA.DimensionSampling(propQA);
                if (propQA.dtDimSamp == null)
                {
                    return;
                }
                else
                {
                    propQA.SAMPLING_TYPE = propQA.dtDimSamp.Rows[0]["sampling_type"].ToString();
                    propQA.SAMPLING_NAME = propQA.dtDimSamp.Rows[0]["sampling_type_name"].ToString().Trim();
                    propQA.CAVITY_QTY = propQA.dtDimSamp.Rows[0]["Cavity_Qty"].ToString();
                    propQA.SAMPLING_QTY = propQA.dtDimSamp.Rows[0]["Sampling_Qty"].ToString();

                    if (propQA.SAMPLING_TYPE == "4" && (propQA.CAVITY_QTY == "0" || propQA.CAVITY_QTY == string.Empty))
                    {
                        MessageBox.Show("ต้องมีการ Setting จำนวน Cavity ของ M-CODE : " + propQA.M_CODE);
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


                        }
                        else if (propQA.SAMPLING_TYPE == "2")
                        {

                        }
                        else
                        {
                            MessageBox.Show("ไม่สามารถเข้าไปทำการ Dimension ได้ กรุณา check sampling type ของ m-code :" + propQA.M_CODE);
                            return;
                        }

                    }

                }

                userControlDimensionPending usrDimPending = new userControlDimensionPending();
                usrDimPending.Dock = DockStyle.Fill;
                usrDimPending.propQA = propQA;

                this.Controls.Clear();
                this.Controls.Add(usrDimPending);

            }
        }
    }
}
