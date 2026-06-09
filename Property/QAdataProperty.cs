using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Property
{
    public class QAdataProperty
    {
        public string M_CODE { get; set; }
        public string Receive_Date { get; set; }
        public string Issue_Date { get; set; }
        public DateTime dtReceiveDate { get; set; }
        public DateTime dtIssueDate { get; set; }

        public string Report_Type { get; set; }
        public string Report_No { get; set; }
        public string Regular_No { get; set; }
        public string Invoice_No { get; set; }
        public string Material_Name { get; set; }
        public string Vendor_Name { get; set; }
        public string Qty { get; set; }
        public string process { get; set; } //process ใน database 
        public string prevProcess { get; set; } //process ก่อนหน้า
        public string labelProcess { get; set; } //process ที่แสดงในหน้าจอ
        public string inProcStatus { get; set; }
        public string reportStatus { get; set; }  // ใช้กับ db_report_status
        public DateTime dtToday { get; set; }
        public DataGridView dtgRawMat { get; set; }
        public DataGridView dtgPackingSize { get; set; }
        public DataTable dtPackingSize { get; set; }
        public string EMP_ID { get; set; }
        public string EMP_NAME { get; set; }
        public string REFRESH_TYPE { get; set; }
        public string rec_wh_status { get; set; }
        public string keep_data_status { get; set; }
        public string reg_check_status { get; set; }
        public string dim_check_status { get; set; }
        public string func_check_status { get; set; }
        public string data_check_status { get; set; }
        public string app_check_status { get; set; }

        // Packing Check
        public string detail_Method { get; set; }

        public string inspQty { get; set; }
        public List<string> Packing_Size_Cal_List { get; set; }
        public string METHOD_ID { get; set; }
        public string judge { get; set; }
        public string Lot_No { get; set; }
        public DataTable dtLotNo { get; set; }
        public string packing_check_mode { get; set; }

        public string FORMAT_REPORT_ID { get; set; }
        public string FORMAT_REPORT_NAME { get; set; }

        // Regular Check
        public string CAVITY_QTY { get; set; }
        public string SAMPLING_TYPE { get; set; }
        public string SAMPLING_NAME { get; set; }
        public string SAMPLING_QTY { get; set; }
        public List<string> Cavity_Name_List { get; set; }
        public string CAVITY_NAME { get; set; }
        public DataTable dtRegSamp { get; set; }
        public DataTable dtRegEq { get; set; }
        public DataTable dtDimEq { get; set; }
        public DataTable dtCavity { get; set; }
        public DataGridView dtgRegData { get; set; }
        public DataGridView dtgDimData { get; set; }
        public string EQUIPMENT_TYPE_ID { get; set; }
        public string EQUIPMENT_SERIAL_ID { get; set; }
        public string EQUIPMENT_SERIAL { get; set; }
        public string TOTAL_STATUS { get; set; }
        public string REGULAR_CHECK_REF { get; set; }
        public string REGULAR_NO_REF { get; set; }
        public string mSelect { get; set; }
        public string mRef { get; set; }

        //Function Check
        public DataTable dtFuncSamp { get; set; }
        public DataTable dtFuncData { get; set; }
        public DataTable dtDimSamp { get; set; }
        public DataTable dtAppSamp { get; set; }
        //Inspection Data Check
        public string data_detail { get; set; }
        public enum ProcStatus
        {
            NG = 0,
            OK = 1,
            Working = 2,
            Skip = 3,
            WaitingApprove = 4,
            Cancel = 5,
            Pending = 6,
            Printed = 7,
            Unfinished = 8, //not finished yet
            Finished = 9
        }

        public string myIPv4 { get; set; }
        public string MY_COMPUTER_NAME { get; set; }
        public string COMPUTER_NAME { get; set; } //VARCHAR(63) สำหรับ hostname เดี่ยว (ครอบคลุมส่วนใหญ่)
        public string myIPv6 { get; set; }
        public string reportIP { get; set; }
        public DataTable dt_report_active { get; set; }
        public string VALUE { get; set; }
        public string BATCH { get; set; }
        public string QTY_SELECT { get; set; }
        public string QTY_OK { get; set; }
        public string QTY_NG { get; set; }
        public string COUNT { get; set; }
        public string APPEARANCE_ID { get; set; }
        public string NG_DETAIL { get; set; }
        public string NG_MODE_ID { get; set; }
        public string PACKING_SIZE { get; set; }
        public string REMAIN_PACKING_SIZE { get; set; }
        public DataGridView dtg_ngMode {get;set;}
        public string Allow_Continue { get; set; }

        
        //public string Lot_Size {get;set;}
    }
}
