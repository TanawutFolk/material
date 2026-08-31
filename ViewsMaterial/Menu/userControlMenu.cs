using Bunifu.Framework.UI;
using RawMat.Controllers;
using RawMat.ViewsMaterial.Main;
using RawMat.ViewsMaterial.ReceiveWH;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using RawMat.Property;
using RawMat.ViewsMaterial.PackingCheck;
using RawMat.ViewsMaterial.RegularCheck;
using System.Threading;
using static RawMat.frmMain;
using RawMat.Utilities;
using RawMat.ViewsMaterial.FunctionCheck;
using RawMat.ViewsMaterial.DimensionCheck;
using RawMat.ViewsMaterial.InspDataCheck;
using RawMat.ViewsMaterial.AppearCheck;

namespace RawMat.ViewsMaterial.Menu
{
    public partial class userControlMenu : UserControl
    {
        public event Action<UserControl> AddUserControlRequested;
        //public event EventHandler SaveRequested;
        private BunifuTileButton activeButton;
        QAdataControllers conQA = new QAdataControllers();
        //private IParent parent;
        private frmMain parentMain;
        //userControlSelectReport usrControlSelect = new userControlSelectReport();

        public userControlMenu()
        {
            InitializeComponent();
            //this.parent = parent;
        }

        public void bt_rec_pack_Click(object sender, EventArgs e)
        {

            userControlSelectPackingCheck usrConPackCheck = new userControlSelectPackingCheck();

            usrConPackCheck.Dock = DockStyle.Fill;
            usrConPackCheck.propQA = new QAdataProperty();

            usrConPackCheck.propQA.labelProcess = "Select Report for : " + bt_rec_pack.LabelText;
            usrConPackCheck.propQA.process = "Packing_Check";
            usrConPackCheck.propQA.prevProcess = "Receive_WH";

            DataTable dt = new DataTable();
            dt = conQA.SearchForOpPackingCheck(usrConPackCheck.propQA);
            usrConPackCheck.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConPackCheck.propQA.dtgRawMat.DataSource = dt;
            AddUserControlRequested?.Invoke(usrConPackCheck);
        }

        public void bt_reg_Click(object sender, EventArgs e)
        {

            // ค้นหา userControlRegular ทุกที่ใน MainForm
            userControlSelectRegular usrConSelectReg = new userControlSelectRegular()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()
            };

            usrConSelectReg.propQA.labelProcess = "Select Report for : " + bt_reg.LabelText;
            usrConSelectReg.propQA.process = "Regular_Check";
            usrConSelectReg.propQA.prevProcess = "Packing_Check";
            
            DataTable dt = new DataTable();

            dt = conQA.SearchForOpRegular(usrConSelectReg.propQA);
            usrConSelectReg.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectReg.propQA.dtgRawMat.DataSource = dt;

            AddUserControlRequested?.Invoke(usrConSelectReg);


        }

        private void bt_function_Click(object sender, EventArgs e)
        {
            userControlSelectFunction usrConSelectFunc = new userControlSelectFunction()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()
            };


        

            usrConSelectFunc.propQA.labelProcess = "Select Report for : " + bt_function.LabelText;
            usrConSelectFunc.propQA.process = "Function_Check";
            //usrConSelectFunc.propQA.prevProcess = "Packing_Check";
            usrConSelectFunc.propQA.prevProcess = "Inspection_Data_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpFunction(usrConSelectFunc.propQA);
            usrConSelectFunc.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectFunc.propQA.dtgRawMat.DataSource = dt;

            AddUserControlRequested?.Invoke(usrConSelectFunc);
        }

        private userControlRegular FindUserControlRegular(Panel searchPanel, HashSet<Control> visited = null)
        {
            if (visited == null)
                visited = new HashSet<Control>();

            if (visited.Contains(searchPanel))
                return null;

            visited.Add(searchPanel);

            foreach (Control ctrl in searchPanel.Controls)
            {
                if (ctrl is userControlRegular regularControl)
                {
                    return regularControl;
                }

                userControlRegular found = FindUserControlRegular(ctrl as Panel, visited);
                if (found != null)
                    return found;
            }

            return null;
        }


        private void bt_dimen_Click(object sender, EventArgs e)
        {
            userControlSelectDimension usrConSelectDim = new userControlSelectDimension()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()
            };


            //แก้ query previous 2025-05-05 preecha_job

            usrConSelectDim.propQA.labelProcess = "Select Report for : " + bt_dimen.LabelText;
            usrConSelectDim.propQA.process = "Dimension_Check";
            //usrConSelectFunc.propQA.prevProcess = "Packing_Check";
            usrConSelectDim.propQA.prevProcess = "Inspection_Data_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpDimension(usrConSelectDim.propQA);
            usrConSelectDim.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectDim.propQA.dtgRawMat.DataSource = dt;

            AddUserControlRequested?.Invoke(usrConSelectDim);
        }

        private void bt_data_Click(object sender, EventArgs e)
        {
            userControlSelectInspData usrConSelectData = new userControlSelectInspData()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()
            };

            usrConSelectData.propQA.labelProcess = "Select Report for : " + bt_data.LabelText;
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

            AddUserControlRequested?.Invoke(usrConSelectData);
        }

        private void bt_appear_Click(object sender, EventArgs e)
        {
            userControlSelectAppear usrConSelectAppear = new userControlSelectAppear()
            {
                Dock = DockStyle.Fill,
                propQA = new QAdataProperty()
            };

            usrConSelectAppear.propQA.labelProcess = "Select Report for : " + bt_appear.LabelText;
            usrConSelectAppear.propQA.process = "Appearance_Check";
            //usrConSelectFunc.propQA.prevProcess = "Packing_Check";
            usrConSelectAppear.propQA.prevProcess = "Inspection_Data_Check";

            DataTable dt = new DataTable();

            dt = conQA.SearchForOpAppear(usrConSelectAppear.propQA);
            usrConSelectAppear.propQA.dtgRawMat = new DataGridView();

            // แก้ไขค่าในคอลัมน์ "Status" หากเป็น null ให้แทนด้วย "Ready"
            foreach (DataRow row in dt.Rows)
            {
                if (row["Status"] == DBNull.Value || string.IsNullOrWhiteSpace(row["Status"].ToString()))
                {
                    row["Status"] = "READY";
                }
            }

            usrConSelectAppear.propQA.dtgRawMat.DataSource = dt;

            AddUserControlRequested?.Invoke(usrConSelectAppear);

        }
    }
}
