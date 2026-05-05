using RawMat.Controllers;
using RawMat.Property;
using RawMat.Utilities;
using RawMat.Views.Main;
using RawMat.Views.PackingCheck;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Views.PackingCheck
{
    public partial class userControlPackingCheckPending : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();
        private IParent parent;

        public userControlPackingCheckPending()
        {
            InitializeComponent();

        }

        private void userControlPackingCheckPending_Load(object sender, EventArgs e)
        {
            //userControlSelectReport usrControlSelect = new userControlSelectReport();

            //usrControlSelect.Dock = DockStyle.Fill;
            //usrControlSelect.propQA = new QAdataProperty();

            //usrControlSelect.propQA.labelProcess = "Select Report for : " + bt_rec_pack.LabelText;
            //usrControlSelect.propQA.process = "Check_Packing";
            //usrControlSelect.propQA.prevProcess = "Receive_WH";
            propQA.process = "Packing_Check";

            DataTable dt = new DataTable();
            dt = conQA.SearchForOperatePending(propQA);

            dtg_packingCheckPending.DataSource = dt;

            dtg_packingCheckPending.Columns["process_status_id"].Visible = false;
            dtg_packingCheckPending.Columns["Issue_Date"].Visible = false;
            //usrControlSelect.propQA.dtgRawMat = new DataGridView();

            //// แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            //foreach (DataRow row in dt.Rows)
            //{
            //    if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
            //    {
            //        row["Status"] = "READY";
            //    }
            //}

            //usrControlSelect.propQA.dtgRawMat.DataSource = dt;

            // Handle SaveRequested Event
            //usrControlSelect.SaveRequested += (s, args) =>
            //{
            //    // Remove UserControl
            //    this.Controls.Remove(usrControlSelect);

            //    // Show Parent Control (เช่น Panel ก่อนหน้า)
            //    this.Visible = true;
            //};

            //if(this.Contains(usrControlSelect) == false)
            //{
            //    this.Controls.Add(usrControlSelect);
            //}
            // Raise the event
            //AddUserControlRequested?.Invoke(usrControlSelect);
        }

        private void dtg_packingCheckPending_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dtg_packingCheckPending.Rows.Count && e.ColumnIndex >= 0 && e.ColumnIndex < dtg_packingCheckPending.Columns.Count)
            {

                propQA.Report_No = dtg_packingCheckPending.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                propQA.Invoice_No = dtg_packingCheckPending.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                propQA.M_CODE = dtg_packingCheckPending.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                propQA.Material_Name = dtg_packingCheckPending.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                propQA.Qty = dtg_packingCheckPending.Rows[e.RowIndex].Cells["Lot Size"].Value.ToString();
                //usrPack.propQA.Receive_Date = dtg_reportSelect.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString();
                propQA.dtReceiveDate = DateTime.Parse(dtg_packingCheckPending.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());
                propQA.dtIssueDate = DateTime.Parse(dtg_packingCheckPending.Rows[e.RowIndex].Cells["Issue_Date"].Value.ToString());


                propQA.packing_check_mode = conQA.PackingCheckMode(propQA);

                if (propQA.packing_check_mode == "2")
                {
                    userControlPackingPrint usrPrint = new userControlPackingPrint(parent);
                    usrPrint.Dock = DockStyle.Fill;
                    usrPrint.propQA = propQA;

                    this.Controls.Clear();
                    this.Controls.Add(usrPrint);
                }
                else
                {
                    userControlPackingCheck usrPack = new userControlPackingCheck(parent);
                    usrPack.Dock = DockStyle.Fill;
                    usrPack.propQA = propQA;

                    this.Controls.Clear();
                    this.Controls.Add(usrPack);
                }

            }

        }
    }
}
