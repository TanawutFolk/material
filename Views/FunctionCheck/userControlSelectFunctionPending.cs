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

namespace RawMat.Views.FunctionCheck
{
    public partial class userControlSelectFunctionPending : UserControl
    {
        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();

        public userControlSelectFunctionPending()
        {
            InitializeComponent();
        }

        private void userControlSelectFunctionPending_Load(object sender, EventArgs e)
        {
            propQA.process = "Function_Check";
            DataTable dt = new DataTable();
            dt = conQA.SearchForFunctionPending(propQA);

            dtg_functionPending.DataSource = dt;

            dtg_functionPending.Columns["process_status_id"].Visible = false;
            dtg_functionPending.Columns["Issue_Date"].Visible = false;

        }

        private void dtg_functionPending_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dtg_functionPending.Rows.Count && e.ColumnIndex >= 0 && e.ColumnIndex < dtg_functionPending.Columns.Count)
            {

                propQA.Report_No = dtg_functionPending.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                propQA.Invoice_No = dtg_functionPending.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                propQA.M_CODE = dtg_functionPending.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                propQA.Material_Name = dtg_functionPending.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                propQA.dtReceiveDate = DateTime.Parse(dtg_functionPending.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());
                //propQA.Lot_No = dtg_reportSelect.Rows[e.RowIndex].Cells["LOT_NO"].Value.ToString();
                propQA.Qty = dtg_functionPending.Rows[e.RowIndex].Cells["Lot Size"].Value.ToString();


                try
                {


                    propQA.dtFuncSamp = conQA.FunctionSampling(propQA);
                    if (propQA.dtFuncSamp == null || propQA.dtFuncSamp.Rows.Count == 0)
                    {
                        MessageBox.Show("ไม่พบการ sampling ที่จะนำไปใช้ในหน้า function check ");
                        return;
                    }
                    else
                    {
                        propQA.SAMPLING_TYPE = propQA.dtFuncSamp.Rows[0]["sampling_type"].ToString();
                        propQA.SAMPLING_NAME = propQA.dtFuncSamp.Rows[0]["sampling_type_name"].ToString().Trim();
                        propQA.CAVITY_QTY = propQA.dtFuncSamp.Rows[0]["Cavity_Qty"].ToString();
                        propQA.SAMPLING_QTY = propQA.dtFuncSamp.Rows[0]["Sampling_Qty"].ToString();
                        propQA.Cavity_Name_List = new List<string>();

                        propQA.CAVITY_NAME = propQA.dtFuncSamp.Rows[0]["Cavity_Name"].ToString();
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
                        else if ((propQA.SAMPLING_TYPE != "3") && (propQA.SAMPLING_QTY == "0" || propQA.SAMPLING_QTY == string.Empty))
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

                                for (int i = 0; i < Convert.ToInt32(propQA.CAVITY_QTY); i++)
                                {
                                    propQA.dtCavity.Rows.Add(new object[] { propQA.Cavity_Name_List[i].ToString(), DBNull.Value });
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
                                //ใช้อันที่หยิบมา
                            }
                            else
                            {
                                MessageBox.Show("ไม่สามารถเข้าไปทำการ Function ได้ กรุณา check sampling type ของ m-code :" + propQA.M_CODE);
                                return;
                            }

                        }

                    }

                    userControlFunctionPending usrFuncPending = new userControlFunctionPending();
                    usrFuncPending.Dock = DockStyle.Fill;
                    usrFuncPending.propQA = propQA;

                    this.Controls.Clear();
                    this.Controls.Add(usrFuncPending);


                }
                catch (Exception ex)
                {
                    MessageBox.Show("เกิดข้อผิดพลาด: " + ex.Message);
                    return;
                }

            }
        }
    }
}
