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

namespace RawMat.ViewsMaterial.InspDataCheck
{
    public partial class userControlSelectInspDataPending : UserControl
    {

        public event Action<UserControl> AddUserControlRequested;
        public event EventHandler SaveRequested;
        public QAdataProperty propQA = new QAdataProperty();
        public QAdataControllers conQA = new QAdataControllers();

        public userControlSelectInspDataPending()
        {
            InitializeComponent();
        }

        private void userControlSelectInspDataPending_Load(object sender, EventArgs e)
        {
            propQA.process = "Inspection_Data_Check";

            DataTable dt = new DataTable();
            dt = conQA.SearchForInspDataPending(propQA);

            dtg_InspDataPending.DataSource = dt;

            dtg_InspDataPending.Columns["process_status_id"].Visible = false;
            dtg_InspDataPending.Columns["Issue_Date"].Visible = false;
        }

        private void dtg_InspDataPending_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.RowIndex < dtg_InspDataPending.Rows.Count && e.ColumnIndex >= 0 && e.ColumnIndex < dtg_InspDataPending.Columns.Count)
            {

                propQA.Regular_No = dtg_InspDataPending.Rows[e.RowIndex].Cells["Regular No"].Value.ToString();
                propQA.Report_No = dtg_InspDataPending.Rows[e.RowIndex].Cells["Report No."].Value.ToString();
                propQA.Invoice_No = dtg_InspDataPending.Rows[e.RowIndex].Cells["Invoice No."].Value.ToString();
                propQA.M_CODE = dtg_InspDataPending.Rows[e.RowIndex].Cells["M-CODE"].Value.ToString();
                propQA.Material_Name = dtg_InspDataPending.Rows[e.RowIndex].Cells["Material Name"].Value.ToString();
                propQA.dtReceiveDate = DateTime.Parse(dtg_InspDataPending.Rows[e.RowIndex].Cells["Receive Date"].Value.ToString());
                propQA.Qty = dtg_InspDataPending.Rows[e.RowIndex].Cells["Lot Size"].Value.ToString();
                propQA.Vendor_Name = dtg_InspDataPending.Rows[e.RowIndex].Cells["Vendor"].Value.ToString();

                try
                {

                    userControlInspDataPending usrInspDataPending = new userControlInspDataPending();
                    usrInspDataPending.Dock = DockStyle.Fill;
                    usrInspDataPending.propQA = propQA;

                    this.Controls.Clear();
                    this.Controls.Add(usrInspDataPending);


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
