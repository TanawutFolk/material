using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SqlKata;
using SqlKata.Compilers;
using SqlKata.Execution;
using MySqlX.XDevAPI.Relational;
using System.Linq.Expressions;

namespace RawMat.SQLFactory
{
    public class QAdataSQL
    {
        private string sql;

        public string SearchReceiveMatAll()
            
        {
            sql = @"SELECT a.Report_No as `Report No`, a.Regular_No as `Regular No` , a.Receive_Date as `Receive Date`, a.M_Code as `M-CODE`, 
                    a.Invoice_No as `Invoice No`, a.Lot_Size as `Lot Size`, c.VENDOR_NAME as `Vendor Name`, a.Emp_Issue_Report as `Issued By`,
                    docStatus.STATUS_NAME as `Keep Data`,
                    whStatus.STATUS_NAME as `Receive WH`,
                    chkpStatus.STATUS_NAME as `Packing Check`,
                    regStatus.STATUS_NAME as `Regular Check`,
                    inspStatus.STATUS_NAME as `Inspection Data Check`,
                    funcStatus.STATUS_NAME as `Function Check`,
                    dimStatus.STATUS_NAME as `Dimension Check`,
                    appStatus.STATUS_NAME as `Appearance Check`,
                    d.Regular_Check

                    FROM `db_receive_mat` a 

                    JOIN mes.item_manufacturing b ON (a.M_Code = b.ITEM_CODE_FOR_SUPPORT_MES)
                    JOIN mes.vendor c ON (b.VENDOR_ID = c.VENDOR_ID)
                    JOIN db_report_status d ON (a.Report_No = d.Report_No)
                    LEFT JOIN info_status docStatus ON (d.Keep_data = docStatus.ID)
                    LEFT JOIN info_status whStatus ON (d.Receive_WH = whStatus.ID)
                    LEFT JOIN info_status chkpStatus ON (d.Packing_Check = chkpStatus.ID)
                    LEFT JOIN info_status regStatus ON (d.Regular_Check = regStatus.ID)
                    LEFT JOIN info_status inspStatus ON (d.Inspection_Data_Check = inspStatus.ID)
                    LEFT JOIN info_status funcStatus ON (d.Function_Check = funcStatus.ID)
                    LEFT JOIN info_status dimStatus ON (d.Dimension_Check = dimStatus.ID)
                    LEFT JOIN info_status appStatus ON (d.Appearance_Check = appStatus.ID)
                    ORDER BY a.Report_No DESC
                    ";

            return sql; 
        
        }

        public string SearchReceiveMatStatusProcess()
        {
            sql = @"
                    SELECT 
                        a.Report_No AS `Report No`, 
                        a.Receive_Date AS `Receive Date`, 
                        a.M_Code AS `M-CODE`, 
                        a.Invoice_No AS `Invoice No`, 
                        a.Lot_Size AS `Lot Size`, 
                        c.VENDOR_NAME AS `Vendor Name`, 
                        a.Emp_Issue_Report AS `Issued By`,
        
                        -- ? ????? `Status` (????????? Pending ???? Working)
                        CASE 
                            WHEN 'Working' IN (
                                docStatus.STATUS_NAME, whStatus.STATUS_NAME, chkpStatus.STATUS_NAME, 
                                regStatus.STATUS_NAME, funcStatus.STATUS_NAME, dimStatus.STATUS_NAME, appStatus.STATUS_NAME
                            ) THEN 'Working'
                            WHEN 'Pending' IN (
                                docStatus.STATUS_NAME, whStatus.STATUS_NAME, chkpStatus.STATUS_NAME, 
                                regStatus.STATUS_NAME, funcStatus.STATUS_NAME, dimStatus.STATUS_NAME, appStatus.STATUS_NAME
                            ) THEN 'Pending'
                        ELSE ''
                        END AS `Status`

                    FROM `db_receive_mat` a 

                    JOIN mes.item_manufacturing b ON (a.M_Code = b.ITEM_CODE_FOR_SUPPORT_MES)
                    JOIN mes.vendor c ON (b.VENDOR_ID = c.VENDOR_ID)
                    JOIN db_report_status d ON (a.Report_No = d.Report_No)
                    LEFT JOIN info_status docStatus ON (d.Keep_Data = docStatus.ID)
                    LEFT JOIN info_status whStatus ON (d.Receive_WH = whStatus.ID)
                    LEFT JOIN info_status chkpStatus ON (d.Packing_Check = chkpStatus.ID)
                    LEFT JOIN info_status regStatus ON (d.Regular_Check = regStatus.ID)
                    LEFT JOIN info_status funcStatus ON (d.Function_Check = funcStatus.ID)
                    LEFT JOIN info_status dimStatus ON (d.Dimension_Check = dimStatus.ID)
                    LEFT JOIN info_status appStatus ON (d.Appearance_Check = appStatus.ID)

                    -- ?? ?????????????? `WHERE` ?????????????? `Pending` ??? `Working`
                    WHERE 
                        (docStatus.STATUS_NAME IN ('Pending', 'Working') OR
                         whStatus.STATUS_NAME IN ('Pending', 'Working') OR
                         chkpStatus.STATUS_NAME IN ('Pending', 'Working') OR
                         regStatus.STATUS_NAME IN ('Pending', 'Working') OR
                         funcStatus.STATUS_NAME IN ('Pending', 'Working') OR
                         dimStatus.STATUS_NAME IN ('Pending', 'Working') OR
                         appStatus.STATUS_NAME IN ('Pending', 'Working'))
                    ORDER BY a.Report_No DESC
                    ";

            return sql;
        }

        public string InsertReceiveRefreshLog(QAdataProperty dataItem)
        {
            return $@"INSERT INTO `db_receive_refresh_log`
                        (`REFRESH_TYPE`, `EMP_ID`, `RECEIVE_DATE`, `COMPUTER_NAME`, `REFRESH_AT`)
                      VALUES
                        ({ToSqlTextValue(dataItem.REFRESH_TYPE)},
                         {ToSqlTextValue(dataItem.EMP_ID)},
                         {ToSqlTextValue(dataItem.dtReceiveDate.ToString("yyyy-MM-dd"))},
                         {ToSqlValue(dataItem.MY_COMPUTER_NAME)},
                         NOW())";
        }

        public string SearchLatestReceiveRefreshLog()
        {
            return @"SELECT `REFRESH_TYPE`, `EMP_ID`, `RECEIVE_DATE`, `COMPUTER_NAME`, `REFRESH_AT`
                     FROM `db_receive_refresh_log`
                     ORDER BY `ID` DESC
                     LIMIT 1";
        }


        public string SearchVendorSmartFFT()
        {
            sql = @"SELECT VENDOR_ID , VENDOR_NAME
                    FROM `vendor`";

            return sql;
        }

        public string SearchInspectionList(QAdataProperty dataItem)
        {
            //I_model ??? mathName
            sql = @"select COUNT(*) AS CNT from info_mat_inspection_list where TRIM(M_CODE) = 'dataItem.M_CODE' and INUSE = 1 
                    ";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE.Trim());

            return sql;
        }

        public string SearchActiveInspectionList()
        {
            sql = @"SELECT DISTINCT
                        TRIM(a.M_CODE) AS M_CODE,
                        c.VENDOR_NAME,
                        b.ITEM_EXTERNAL_SHORT_NAME AS material_name
                    FROM info_mat_inspection_list a
                    JOIN mes.item_manufacturing b ON TRIM(a.M_CODE) = TRIM(b.ITEM_CODE_FOR_SUPPORT_MES)
                    JOIN mes.vendor c ON b.VENDOR_ID = c.VENDOR_ID
                    WHERE a.INUSE = 1";

            return sql;
        }

        //public string SearchMcodeSmartFFT(QAdataProperty dataItem)
        //{
        //    //I_model ??? mathName
        //    sql = @" select b.VENDOR_ID , c.VENDOR_NAME

        //            from item_manufacturing b
        //            join vendor c on (b.VENDOR_ID = c.VENDOR_ID)

        //            where b.Item_code_for_support_mes = 'dataItem.M_CODE'
        //            ";


        //    sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

        //    return sql;
        //}

        public string SearchInspListxSmartFFT(QAdataProperty dataItem)
        {

            sql = "SELECT b.`VENDOR_ID` , c.`VENDOR_NAME` , b.ITEM_EXTERNAL_SHORT_NAME as `material_name` from `info_mat_inspection_list` a " +
                "JOIN `mes`.`item_manufacturing` b on (a.`M_Code` = b.`Item_code_for_support_mes`) " +
                "JOIN `mes`.`vendor` c on (b.`VENDOR_ID` = c.`VENDOR_ID`) " +
                "WHERE TRIM(a.`M_CODE`) = '" + dataItem.M_CODE.Trim() + "' ;";


           //sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string SearchMcodeSmartFFTOnly(QAdataProperty dataItem)
        {

            sql = @"SELECT b.`VENDOR_ID` , c.`VENDOR_NAME` , b.ITEM_EXTERNAL_SHORT_NAME as `material_name` 
                    from `mes`.`item_manufacturing` b
                    JOIN `mes`.`vendor` c on(b.`VENDOR_ID` = c.`VENDOR_ID`) 
                    WHERE b.ITEM_CODE_FOR_SUPPORT_MES =  'dataItem.M_CODE'";


            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string SearchMcodeSmartFFTreal(QAdataProperty dataItem)
        {
           
            sql = @" select b.VENDOR_ID , c.VENDOR_NAME
                    from info_mat_inspection_list a
                    join mes.item_manufacturing b on (a.M_Code = b.Item_code_for_support_mes)
                    join mes.vendor c on (b.VENDOR_ID = c.VENDOR_ID)

                    where a.M_CODE = 'dataItem.M_CODE'
                    ";


            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string checkReceiveMat(QAdataProperty dataItem)
        {

            sql = @"SELECT COUNT(*) as cnt 
                    FROM `db_receive_mat`
                    where Receive_Date = 'dataItem.Receive_Date' and M_Code = 'dataItem.M_CODE' and Invoice_No = 'dataItem.Invoice_No'

                    ";

            sql = sql.Replace("dataItem.Receive_Date", dataItem.Receive_Date);
            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);
            sql = sql.Replace("dataItem.Invoice_No", dataItem.Invoice_No);

            return sql;
        }

        
        public string SearchToday()
        {
            sql = @"Select NOW() as `Today`";

            return sql;
        }

        public string SearchReportNoMax()
        {
            sql = @"SELECT max(Report_No) as `LAST_REPORT_NO`
                    FROM `db_report_status`";   

            return sql;
        }

        public string SearchRegularNoMax()
        {
            sql = @"SELECT max(Regular_No) as `LAST_REGULAR_NO`
                    FROM `db_receive_mat`";

            return sql;
        }

        public string NeedKeepData(QAdataProperty dataItem)
        {
            sql = @"SELECT `Keep_Data_Need` FROM `info_mat_inspection_list`
                    where M_CODE = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string NeedRegularCheck(QAdataProperty dataItem)
        {
            sql = @"SELECT `Regular_Check_Need` FROM `info_mat_inspection_list`
                    where M_CODE = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string NeedFunctionCheck(QAdataProperty dataItem)
        {
            sql = @"SELECT `Function_Check_Need` FROM `info_mat_inspection_list`
              where M_CODE = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string CheckThisMonthRegular(QAdataProperty dataItem)
        {
            sql = @"
                    SELECT Count(*) As CNT
                    FROM db_receive_mat
                    WHERE Regular_No is not NULL
                    AND Receive_Date LIKE 'dataItem.dtReceiveDate%'
                    AND M_Code = 'dataItem.M_CODE'
                    AND Invoice_No <> 'Replacement'
                    AND Report_Type = '1'
                   ";
            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);
            sql = sql.Replace("dataItem.dtReceiveDate", dataItem.dtReceiveDate.ToString("yyyy-MM"));
            return sql;
        }

        public string CheckStatus(QAdataProperty dataItem)
        {

            sql = @"SELECT b.`Report_No` , b.`dataItem.process` as `dataItem.process`
                    FROM `db_receive_mat` a 
                    join `db_report_status` b on (a.Report_No = b.Report_No)
                    where a.Receive_Date = 'dataItem.Receive_Date' and a.M_Code = 'dataItem.M_CODE' and a.Invoice_No = 'dataItem.Invoice_No'
                    ";

            //sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.Receive_Date", dataItem.Receive_Date);
            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);
            sql = sql.Replace("dataItem.Invoice_No", dataItem.Invoice_No);
            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string SearchReceiveMatStatusByReceiveDate(QAdataProperty dataItem)
        {
            sql = @"SELECT
                        a.Report_No,
                        TRIM(a.M_Code) AS M_CODE,
                        a.Invoice_No,
                        b.Receive_WH
                    FROM db_receive_mat a
                    JOIN db_report_status b ON a.Report_No = b.Report_No
                    WHERE a.Receive_Date = 'dataItem.Receive_Date'";

            sql = sql.Replace("dataItem.Receive_Date", dataItem.Receive_Date);

            return sql;
        }

        public string CheckStatusReplacement(QAdataProperty dataItem)
        {

            sql = @"SELECT b.`Report_No` , b.`dataItem.process` as `dataItem.process`
                    FROM `db_receive_mat` a 
                    join `db_report_status` b on (a.Report_No = b.Report_No)
                    where a.Report_No = 'dataItem.Report_No' and a.Receive_Date = 'dataItem.Receive_Date' and a.M_Code = 'dataItem.M_CODE' and a.Invoice_No = 'dataItem.Invoice_No'
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.Receive_Date", dataItem.Receive_Date);
            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);
            sql = sql.Replace("dataItem.Invoice_No", dataItem.Invoice_No);
            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string PackingCheckMode(QAdataProperty dataItem)
        {
            sql = @"SELECT PACKING_CHECK_MODE 
                    FROM info_mat_inspection_list
                    WHERE M_CODE = 'dataItem.M_CODE';
                    ";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);
            return sql;
        }

        public string CountProcessStatusPending(QAdataProperty dataItem)
        {
            sql = @"select count(*) as `cnt`
                    from db_report_status
                    where `dataItem.process` = 6
                    ";

            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;

        }

        public string SearchProcessStatusPending(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date , a.Report_No , b.M_Code , b.Invoice_No  , b.Material_Name , b.Vendor_Name , b.Lot_Size as `Qty` ,  
                    b.Report_Type ,c.Report_Type_Name as `Report Type` , a.dataItem.process
                    FROM `db_report_status` a
                    join db_receive_mat b on (a.Report_No = b.Report_No)
                    join info_report_type c on (b.Report_Type = c.Report_Type)
                    where a.dataItem.process = 6
                    ";

            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string SearchReplacement()
        {
            sql = "SELECT  b.Receive_Date  , a.Report_No ,  b.M_Code  , b.Invoice_No , b.Material_Name  , b.Vendor_Name , b.Lot_Size as `Qty` , b.Report_Type ,c.Report_Type_Name as `Report Type` , a.Keep_Data , a.Receive_WH " +
                "FROM `db_report_status` a " +
                "join db_receive_mat b on (a.Report_No = b.Report_No) " +
                "join info_report_type c on (b.Report_Type = c.Report_Type) " +
                "where (B.Invoice_No = 'Replacement' or b.Report_Type = 2) and a.Receive_WH = 8";
                return sql;
        }

        public string SearchForOpPackingCheck(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
                    b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date
                    
                    FROM `db_report_status` a
                    join db_receive_mat b on (a.Report_No = b.Report_No)
                    join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
                    join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
                    
                    LEFT JOIN info_status iStatus ON (a.dataItem.process = iStatus.ID)

                    where dataItem.prevProcess = 1 and (dataItem.process is NULL or dataItem.process = 2 or dataItem.process = 8 ) 
                    ";

            sql = sql.Replace("dataItem.prevProcess", dataItem.prevProcess);
            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string SearchForOpRegular(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` ,b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
                            b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , 
                            c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , 
                            iStatus.STATUS_NAME as `Status` , b.Issue_Date ,e.Regular_Check_Ref

                            FROM `db_report_status` a
                            join db_receive_mat b on (a.Report_No = b.Report_No)
                            join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
                            join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
                            join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
                            LEFT JOIN info_status iStatus ON (a.dataItem.process = iStatus.ID)

                            where (a.dataItem.prevProcess = 1 and a.dataItem.prevProcess is NOT NULL) 
                                    and (a.dataItem.process is NULL or dataItem.process = 2 or dataItem.process = 8) 
                                    and (b.Regular_No is NOT NULL)
               ";

            sql = sql.Replace("dataItem.prevProcess", dataItem.prevProcess);
            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }


        public string SearchForOperatePending(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
                    b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date
                    
                    FROM `db_report_status` a
                    join db_receive_mat b on (a.Report_No = b.Report_No)
                    join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
                    join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
                    
                    LEFT JOIN info_status iStatus ON (dataItem.process = iStatus.ID)

                    where (dataItem.process = 6) 
                    ";

            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string CountPackingCheck(QAdataProperty dataItem)
        {
            sql = @"SELECT COUNT(*) AS `cnt`
                    FROM `db_packing_check`
                    WHERE `Report_No` = 'dataItem.Report_No' 
                      AND `METHOD_ID` = 'dataItem.METHOD_ID'
                      AND `Count` = (SELECT MAX(`Count`) 
                                     FROM `db_packing_check` 
                                     WHERE `Report_No` = 'dataItem.Report_No' 
                                       AND `METHOD_ID` = 'dataItem.METHOD_ID')
                      AND `judgment` = 'dataItem.judge';
                    ";
            
            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.METHOD_ID", dataItem.METHOD_ID);
            sql = sql.Replace("dataItem.judge", dataItem.judge);

            return sql;

        }

        public string DetailMethod(QAdataProperty dataItem)
        {
            sql = @"SELECT DETAIL_METHOD  
                    FROM info_method 
                    WHERE ID = 'dataItem.METHOD_ID' 
                    ";

            sql = sql.Replace("dataItem.METHOD_ID", dataItem.METHOD_ID);
            return sql;
        }

        public string SearchFormatReport(QAdataProperty dataItem)
        {
            sql = @"SELECT CELL , CELL_NAME 
                    FROM info_format_report 
                    WHERE FORMAT_REPORT_ID ='dataItem.FORMAT_REPORT_ID'
                    ";

            sql = sql.Replace("dataItem.FORMAT_REPORT_ID", dataItem.FORMAT_REPORT_ID);
            return sql;
        }

        public string CountMaxPackingCheck(QAdataProperty dataItem)
        {
            sql = @"SELECT COUNT(*) AS `cnt`
                    FROM `db_packing_check`
                    WHERE `Report_No` = 'dataItem.Report_No' 
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;

        }

        public string PackingCheck(QAdataProperty dataItem)
        {
            sql = @"SELECT p.METHOD_ID, p.`COUNT`, p.JUDGMENT , p.DETAIL_JUDGE
                    FROM db_packing_check p
                    INNER JOIN (
                        SELECT METHOD_ID, MAX(`COUNT`) AS max_count
                        FROM db_packing_check
                        WHERE REPORT_NO = 'dataItem.Report_No'
                        GROUP BY METHOD_ID
                    ) max_counts
                    ON p.METHOD_ID = max_counts.METHOD_ID AND p.`COUNT` = max_counts.max_count
                    WHERE p.REPORT_NO = 'dataItem.Report_No';
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }

        public string CountReportLotNo(QAdataProperty dataItem)
        {
            sql = @"SELECT count(*) `cnt`
                    FROM db_report_lot_no
                    WHERE `Report_No` = 'dataItem.Report_No'
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;

        }

        public string ReportLot(QAdataProperty dataItem)
        {
            sql = @"SELECT *
                    FROM db_report_lot_no
                    WHERE `Report_No` = 'dataItem.Report_No'
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }

        public string CountPackingSize(QAdataProperty dataItem)
        {
            sql = @"SELECT count(*) `cnt`
                    FROM db_packing_size
                    WHERE `Report_No` = 'dataItem.Report_No'
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }

        public string PackingSize(QAdataProperty dataItem)
        {
            sql = @"SELECT `BATCH` , VALUE , PACK_COUNT
                    FROM db_packing_size
                    WHERE `Report_No` = 'dataItem.Report_No'
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }

        public List<string> InsertPackingSize(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            int i = 0;
            foreach (DataGridViewRow row in dataItem.dtgPackingSize.Rows)
            {

                // ????????????????????????
                if (row.Cells["VALUE"].Value == null || row.Cells["PACK_COUNT"].Value == null)
                {
                    continue; // ?????????????????????
                }

                sql = @"INSERT INTO `db_packing_size`(`Report_No`, `BATCH`, `VALUE`, `PACK_COUNT` , `PACKING_SIZE`) 
                        VALUES ('dataItem.Report_No', 'dataItem.Batch', 'dataItem.Value', 'dataItem.Pack_Count' , dataItem.Packing_Size);";

                sql = sql.Replace("dataItem.Batch", (i + 1).ToString());
                sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
                sql = sql.Replace("dataItem.Value", row.Cells["VALUE"].Value.ToString());
                sql = sql.Replace("dataItem.Pack_Count", row.Cells["PACK_COUNT"].Value.ToString());

                if (dataItem.Packing_Size_Cal_List == null || dataItem.Packing_Size_Cal_List.Count == 0)
                {
                    sql = sql.Replace("dataItem.Packing_Size", "NULL");
                }
                else
                {
                    sql = sql.Replace("dataItem.Packing_Size", $"'{dataItem.Packing_Size_Cal_List[i].ToString()}'");
                }

                sqlList.Add(sql);

                i++;
            }

            return sqlList;
        }


        public string UpdateStatus(QAdataProperty dataItem)
        {
            sql = @"UPDATE `db_report_status` 
                    SET `dataItem.process` = dataItem.inProcStatus , `Report_Status` = dataItem.reportStatus  WHERE `Report_No` = 'dataItem.Report_No'";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.process", dataItem.process);

            // ?????????? reg_check_status ????????????? NULL
            if (string.IsNullOrEmpty(dataItem.inProcStatus))
            {
                sql = sql.Replace("dataItem.inProcStatus", "NULL");
            }
            else
            {
                sql = sql.Replace("dataItem.inProcStatus", $"'{dataItem.inProcStatus}'");
            }

            sql = sql.Replace("dataItem.reportStatus", dataItem.reportStatus);
            return sql;
        }


        public string UpdateStatusCancel(QAdataProperty dataItem)
        {
            sql = @"UPDATE `db_report_status` 
                    SET `Keep_Data` = 5, `Receive_WH` = 5 , `Report_Status` = 5  WHERE `Report_No` = 'dataItem.Report_No'";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }

        public string UpdateDataReceiveWH(QAdataProperty dataItem)
        {
            sql = @" UPDATE `db_report_status` 
                    SET `EMP_RECEIVE_WH` = 'dataItem.EMP_ID', `RECEIVE_WH_DATE` = NOW() , `Receive_WH` = 'dataItem.inProcStatus' , `Report_Status` = 'dataItem.reportStatus'
                    WHERE `Report_No` = 'dataItem.Report_No'
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.EMP_ID", dataItem.EMP_ID);
            sql = sql.Replace("dataItem.inProcStatus", dataItem.inProcStatus);
            sql = sql.Replace("dataItem.reportStatus", dataItem.reportStatus);
            return sql;
        }


        public string UpdateRegularNo(QAdataProperty dataItem)
        {
            sql = @" UPDATE `db_receive_mat` SET `Regular_No` = 'dataItem.Regular_No' 
                    WHERE `Report_No` = 'dataItem.Report_No'";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.Regular_No", dataItem.Regular_No);

            return sql;
        }

        public List<string> InsertReportStatusAndReceiveMatAll(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();

            foreach (DataGridViewRow row in dataItem.dtgRawMat.Rows)
            {
                //if (row.IsNewRow) continue; // ???????????? (New Row)

            
                sql = @"INSERT INTO `db_receive_mat`(`Report_No`, `Report_Type`, `Regular_No`, `M_Code`, `Material_Name`, `Invoice_No`, `Vendor_Name`, `Lot_Size` ,`Receive_Date`) 
                     VALUES ('dataItem.Report_No', 1, NULL, 'dataItem.M_CODE', 'dataItem.Material_Name', 'dataItem.Invoice_No', 'dataItem.Vendor_Name', 'dataItem.Qty' , 'dataItem.Receive_Date');
                    
                     INSERT INTO `db_report_status`(`Report_No`, `Keep_Data`, `Receive_WH`, `Report_Status`) 
                     VALUES ('dataItem.Report_No', 2, 2, 2);
                    ";

                

                sql = sql.Replace("dataItem.M_CODE", row.Cells["M_CODE"].Value.ToString());
                sql = sql.Replace("dataItem.Report_No", row.Cells["REPORT_NO"].Value.ToString());
                sql = sql.Replace("dataItem.Material_Name", row.Cells["PART_NAME"].Value.ToString());
                sql = sql.Replace("dataItem.Invoice_No", row.Cells["INVOICE_NO"].Value.ToString());
                sql = sql.Replace("dataItem.Vendor_Name", row.Cells["VENDOR"].Value.ToString());
                sql = sql.Replace("dataItem.Qty", row.Cells["GR_QTY"].Value.ToString());
                sql = sql.Replace("dataItem.Receive_Date", dataItem.Receive_Date);

                sqlList.Add(sql);
            }
            return sqlList;
        }

        public string InsertReportStatusAndReceiveMat(QAdataProperty dataItem)
        {

            sql = @"INSERT INTO `db_receive_mat`(`Report_No`, `Report_Type`, `Regular_No`, `M_Code`, `Material_Name`, `Invoice_No`, `Vendor_Name`, `Lot_Size` ,`Receive_Date` , `Emp_Issue_Report` ,`Issue_Date`) 
       VALUES ('dataItem.Report_No', 'dataItem.Report_Type', NULL, 'dataItem.M_CODE', 'dataItem.Material_Name', 'dataItem.Invoice_No', 'dataItem.Vendor_Name', 'dataItem.Qty' , 'dataItem.Receive_Date' , 'dataItem.EMP_ID' , NOW());
       
       INSERT INTO `db_report_status`(`Report_No`, `Keep_Data`, `Receive_WH`, `Regular_Check`, `Inspection_Data_Check` ,`Function_Check` ,`Dimension_Check` ,`Appearance_Check` , `Report_Status`) 
       VALUES ('dataItem.Report_No', dataItem.keep_data_status, 'dataItem.inProcStatus', dataItem.reg_check_status , dataItem.data_check_status , dataItem.func_check_status , dataItem.dim_check_status , dataItem.app_check_status , 'dataItem.reportStatus');
   ";


            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);
            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.Material_Name", dataItem.Material_Name);
            sql = sql.Replace("dataItem.Report_Type", dataItem.Report_Type);
            sql = sql.Replace("dataItem.Invoice_No", dataItem.Invoice_No);
            sql = sql.Replace("dataItem.Vendor_Name", dataItem.Vendor_Name);
            sql = sql.Replace("dataItem.Qty", dataItem.Qty);
            sql = sql.Replace("dataItem.Receive_Date", dataItem.Receive_Date);
            sql = sql.Replace("dataItem.EMP_ID", dataItem.EMP_ID);

            sql = sql.Replace("dataItem.inProcStatus", dataItem.inProcStatus);
            sql = sql.Replace("dataItem.reportStatus", dataItem.reportStatus);

            // ?????????? keep_data_status ????????????? NULL
            if (string.IsNullOrEmpty(dataItem.keep_data_status))
            {
                sql = sql.Replace("dataItem.keep_data_status", "NULL");
            }
            else
            {
                sql = sql.Replace("dataItem.keep_data_status", $"'{dataItem.keep_data_status}'");
            }

            // ?????????? reg_check_status ????????????? NULL
            if (string.IsNullOrEmpty(dataItem.reg_check_status))
            {
                sql = sql.Replace("dataItem.reg_check_status", "NULL");
            }
            else
            {
                sql = sql.Replace("dataItem.reg_check_status", $"'{dataItem.reg_check_status}'");
            }

            // ?????????? function_check_status ????????????? NULL
            if (string.IsNullOrEmpty(dataItem.func_check_status))
            {
                sql = sql.Replace("dataItem.func_check_status", "NULL");
            }
            else
            {
                sql = sql.Replace("dataItem.func_check_status", $"'{dataItem.func_check_status}'");
            }

            // ?????????? dimension_check_status ????????????? NULL
            if (string.IsNullOrEmpty(dataItem.dim_check_status))
            {
                sql = sql.Replace("dataItem.dim_check_status", "NULL");
            }
            else
            {
                sql = sql.Replace("dataItem.dim_check_status", $"'{dataItem.reg_check_status}'");
            }

            // ?????????? data_check_status ????????????? NULL
            if (string.IsNullOrEmpty(dataItem.data_check_status))
            {
                sql = sql.Replace("dataItem.data_check_status", "NULL");
            }
            else
            {
                sql = sql.Replace("dataItem.data_check_status", $"'{dataItem.data_check_status}'");
            }

            // ?????????? app_check_status ????????????? NULL
            if (string.IsNullOrEmpty(dataItem.app_check_status))
            {
                sql = sql.Replace("dataItem.app_check_status", "NULL");
            }
            else
            {
                sql = sql.Replace("dataItem.app_check_status", $"'{dataItem.app_check_status}'");
            }

            return sql;
        }



        public string InsertPackingCheck(QAdataProperty dataItem)
        {
            //var compiler = new MySqlCompiler();
            var queries = new List<string>();

            // ???? SET @nextCount
            // ????? raw SQL ???????????????????
            var setCountQuery = $"SET @nextCount = (SELECT COALESCE(MAX(`COUNT`), 0) + 1 FROM `db_packing_check` WHERE `REPORT_NO` = '{dataItem.Report_No}' AND `METHOD_ID` = '{dataItem.METHOD_ID}')";
            queries.Add(setCountQuery);

            // ???? INSERT
            // ????? raw SQL ???????????????????
            var detailJudgeValue = string.IsNullOrEmpty(dataItem.detail_Method) ? "NULL" : $"'{dataItem.detail_Method.Replace("'", "''")}'";
            var insertQuery = $"INSERT INTO `db_packing_check` (`REPORT_NO`, `METHOD_ID`, `COUNT`, `DETAIL_JUDGE`, `JUDGMENT`, `EMP_PACKING_CHECK`) " +
                              $"VALUES ('{dataItem.Report_No}', '{dataItem.METHOD_ID}', @nextCount, {detailJudgeValue}, '{dataItem.judge}', '{dataItem.EMP_ID}')";
            queries.Add(insertQuery);

            // ??? queries ???? string ?????
            var sql = string.Join(";\n", queries);
            return sql;
        }


        public string InsertReportLotNo(QAdataProperty dataItem)
        {
            sql = @"INSERT INTO `db_report_lot_no`(`REPORT_NO`, `LOT_NO`) VALUES ('dataItem.Report_No', 'dataItem.Lot_No');";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.Lot_No", dataItem.Lot_No);

            // ?????????? Lot_No ????????????? NULL
            //if (string.IsNullOrEmpty(dataItem.Lot_No))
            //{
            //    sql = sql.Replace("dataItem.Lot_No", "NULL");
            //}
            //else
            //{
            //    sql = sql.Replace("dataItem.Lot_No", $"'{dataItem.Lot_No}'");
            //}

            return sql;

        }


        public string UpdateReportLotNo(QAdataProperty dataItem)
        {
            sql = @"
                    UPDATE `db_report_lot_no`
                    SET 
                        `LOT_NO` = @LotNo
                    WHERE 
                        `REPORT_NO` = @ReportNo;
                    ";

            // ?????? parameter ?????? SQL
            sql = sql.Replace("@ReportNo", $"'{dataItem.Report_No}'");
            sql = sql.Replace("@LotNo", $"'{dataItem.Lot_No}'");

            return sql;
        }

        public string RegularSampling(QAdataProperty dataItem)
        {
            sql = @"SELECT a.sampling_type , b.sampling_type_name , a.Cavity_Qty , a.Sampling_Qty , a.Cavity_Name
                    FROM info_regular_sampling a 
                    JOIN info_sampling_type  b on a.sampling_type = b.sampling_type
                    WHERE M_Code = 'dataItem.M_CODE'";



            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string RegularEquipment(QAdataProperty dataItem)
        {
            sql = @"SELECT a.POINT_ORDER ,a.EQUIPMENT_TYPE , b.Equipment_Name , a.POINT_NAME , a.POINT_CAL , a.CRITERIA_MIN , a.CRITERIA_MAX 
                    FROM `info_regular_equipment` a
                    JOIN info_equipment_type b on (a.EQUIPMENT_TYPE = b.Equipment_Type)
                    WHERE M_Code = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string InsertEquipmentSerial(QAdataProperty dataItem)
        {
            sql = @"ALTER TABLE `info_equipment_serial`
                  AUTO_INCREMENT = 1;
	
                INSERT INTO `info_equipment_serial` (EQUIPMENT_SERIAL, EQUIPMENT_TYPE_ID)
                SELECT 'dataItem.EQUIPMENT_SERIAL', 'dataItem.EQUIPMENT_TYPE_ID'
                FROM DUAL
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM `info_equipment_serial`
                    WHERE EQUIPMENT_SERIAL = 'dataItem.EQUIPMENT_SERIAL' and EQUIPMENT_TYPE_ID = 'dataItem.EQUIPMENT_TYPE_ID'
                );


                SELECT id AS id
                FROM `info_equipment_serial`
                WHERE EQUIPMENT_SERIAL = 'dataItem.EQUIPMENT_SERIAL' and EQUIPMENT_TYPE_ID = 'dataItem.EQUIPMENT_TYPE_ID';";

            sql = sql.Replace("dataItem.EQUIPMENT_SERIAL", dataItem.EQUIPMENT_SERIAL);
            sql = sql.Replace("dataItem.EQUIPMENT_TYPE_ID", dataItem.EQUIPMENT_TYPE_ID);

            return sql;

        }

        public List<string> InsertRegularData(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            DataTable dt = (DataTable)dataItem.dtgRegData.DataSource;

            foreach (DataRow row in dt.Rows)
            {

                //sql = @"INSERT INTO `db_regular_data`(`REGULAR_NO`, `CAVITY_NAME`, `SAMPLING_NO`, `EQUIPMENT_SERIAL_ID`, `POINT_ORDER`, `VALUE`, `JUDGE`,`EMP_ID`,`REGULAR_DATE`,`INUSE`) 
                //        VALUES ('dataItem.REGULAR_NO',dataItem.CAVITY_NAME , 'dataItem.SAMPLING_NO', 'dataItem.EQUIPMENT_SERIAL_ID', 'dataItem.POINT_ORDER', 'dataItem.VALUE', 'dataItem.JUDGE', 'dataItem.EMP_ID' ,NOW() , 1);";

                sql = @"
                     -- ?????? `INUSE` ???????????????????????? 0
                        UPDATE `db_regular_data`
                        SET `INUSE` = 0
                        WHERE `REGULAR_NO` = 'dataItem.REGULAR_NO'
                        AND `POINT_ORDER` = 'dataItem.POINT_ORDER'
                        AND `SAMPLING_NO` = 'dataItem.SAMPLING_NO';

                        -- ????????????????? `COUNT` ????????????? 1 ??? `INUSE = 1`
                        INSERT INTO `db_regular_data` (`REGULAR_NO`, `CAVITY_NAME`, `SAMPLING_NO`, `COUNT`, `EQUIPMENT_SERIAL_ID`, `POINT_ORDER`, `VALUE`, `JUDGE`, `EMP_ID`, `REGULAR_DATE`, `INUSE`)
                        SELECT 
                            'dataItem.REGULAR_NO',
                            dataItem.CAVITY_NAME,
                            'dataItem.SAMPLING_NO',
                            COALESCE(MAX(`COUNT`), 0) + 1,  -- ????? COUNT ?????????????? 1
                            'dataItem.EQUIPMENT_SERIAL_ID',
                            'dataItem.POINT_ORDER',
                            'dataItem.VALUE',
                            'dataItem.JUDGE',
                            'dataItem.EMP_ID',
                            NOW(),
                            1  -- ?????????? INUSE = 1
                        FROM `db_regular_data`
                        WHERE `REGULAR_NO` = 'dataItem.REGULAR_NO'
                        AND `POINT_ORDER` = 'dataItem.POINT_ORDER'
                        AND `SAMPLING_NO` = 'dataItem.SAMPLING_NO';
                        
                        ";

                sql = sql.Replace("dataItem.REGULAR_NO", dataItem.Regular_No);
                sql = sql.Replace("dataItem.EMP_ID", dataItem.EMP_ID);
                sql = sql.Replace("dataItem.SAMPLING_NO", row["SAMPLING_NO"].ToString());
                sql = sql.Replace("dataItem.EQUIPMENT_SERIAL_ID", row["EQUIPMENT_SERIAL"].ToString());
                sql = sql.Replace("dataItem.POINT_ORDER", row["POINT_ORDER"].ToString());
                sql = sql.Replace("dataItem.VALUE", row["VALUE"].ToString());
                sql = sql.Replace("dataItem.JUDGE", row["POINT_JUDGE"].ToString());

                // ????????????????? CAVITY_NAME ???????? DataTable ???????
                if (dt.Columns.Contains("CAVITY_NAME"))
                {
                    // ?????????? row["CAVITY_NAME"] ???? null ???????????????
                    string cavityName = row["CAVITY_NAME"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["CAVITY_NAME"].ToString())
                        ? $"'{row["CAVITY_NAME"].ToString()}'"
                        : "NULL";
                    sql = sql.Replace("dataItem.CAVITY_NAME", cavityName);
                }
                else
                {
                    sql = sql.Replace("dataItem.CAVITY_NAME", "NULL");
                }
                sqlList.Add(sql);
            }

            return sqlList;
        }

        public string SearchRegularRef(QAdataProperty dataItem)
        {
            sql = @"SELECT a.Report_No , a.Regular_No , c.Regular_Check , a.M_Code
                    FROM `db_receive_mat` a 
                    join info_mat_inspection_list b on (a.M_Code = b.M_CODE)
                    join db_report_status c on (a.Report_No = c.Report_No)
                    where b.Regular_Check_Ref = 'dataItem.REGULAR_CHECK_REF' and Receive_Date = 'dataItem.Receive_Date' and C.Regular_Check = 1
                    ORDER BY Regular_No ASC 
                    ";

            sql = sql.Replace("dataItem.REGULAR_CHECK_REF", dataItem.REGULAR_CHECK_REF);
            sql = sql.Replace("dataItem.Receive_Date", dataItem.dtReceiveDate.ToString("yyyy-MM-dd"));

            return sql;
        }

        public string SearchReferenceByMCode(QAdataProperty dataItem)
        {
            sql = @"SELECT `Reference` AS `REFERENCE`
                    FROM `info_reference`
                    WHERE `M_Code` = dataItem.M_CODE
                    LIMIT 1";

            sql = sql.Replace("dataItem.M_CODE", ToSqlTextValue(dataItem.M_CODE));

            return sql;
        }

        public string CheckConditionRegularRef(QAdataProperty dataItem)
        {
            sql = @"SELECT 
            'dataItem.mSelect' AS mSelect, 
            'dataItem.mRef' AS mRef,
                    CASE 
                        WHEN GROUP_CONCAT(DISTINCT IFNULL(a.POINT_ORDER, 'NULL') ORDER BY a.POINT_ORDER) = 
                            GROUP_CONCAT(DISTINCT IFNULL(b.POINT_ORDER, 'NULL') ORDER BY b.POINT_ORDER)
                        AND GROUP_CONCAT(DISTINCT IFNULL(a.EQUIPMENT_TYPE, 'NULL') ORDER BY a.EQUIPMENT_TYPE) = 
                            GROUP_CONCAT(DISTINCT IFNULL(b.EQUIPMENT_TYPE, 'NULL') ORDER BY b.EQUIPMENT_TYPE)
                        THEN 'MATCH' 
                        ELSE 'NOT MATCH' 
                    END AS Compare_Result
                FROM info_regular_equipment a
                JOIN info_regular_equipment b 
                ON a.M_CODE = 'dataItem.mSelect' AND b.M_CODE = 'dataItem.mRef';";

            sql = sql.Replace("dataItem.mSelect", dataItem.mSelect);
            sql = sql.Replace("dataItem.mRef", dataItem.mRef);

            return sql;
        }

        public string SearchForRegularPending(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` ,b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
                     b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date
             #,e.Check_Regular_Ref
                     FROM `db_report_status` a
                     join db_receive_mat b on (a.Report_No = b.Report_No)
                     join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
                     join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
                     join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
                     LEFT JOIN info_status iStatus ON (a.dataItem.process = iStatus.ID)

                     where (a.dataItem.process = 6) 
        ";

            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string SearchRegularDataPending(QAdataProperty dataItem)

        {
            sql = @"select a.SAMPLING_NO  , a.CAVITY_NAME, c.POINT_CAL , c.POINT_ORDER , c.POINT_NAME ,a.EQUIPMENT_SERIAL_ID , d.Equipment_Type , d.Equipment_Name , a.`VALUE` , c.CRITERIA_MIN , c.CRITERIA_MAX
                    
                    from db_regular_data a 
                    join db_receive_mat b on (a.REGULAR_NO = b.Regular_No)
                    join info_regular_equipment c on (b.M_Code = c.M_CODE  and a.POINT_ORDER = c.POINT_ORDER)
                    join info_equipment_type d on (c.EQUIPMENT_TYPE = d.Equipment_Type)

                    where a.REGULAR_NO = 'dataItem.Regular_No' and inuse = 1 and JUDGE= 0
   
            ";

            sql = sql.Replace("dataItem.Regular_No", dataItem.Regular_No);

            return sql;

        }

        public string UpdateRegularRef(QAdataProperty dataItem)
        {
            sql = @"
                    UPDATE  `db_receive_mat`
                    SET `Regular_No` = 'dataItem.REGULAR_NO_REF' WHERE `Report_No` = 'dataItem.Report_No' ;  

     
                    UPDATE `db_report_status` 
                    SET `dataItem.process` = dataItem.inProcStatus , `Report_Status` = dataItem.reportStatus  WHERE `Report_No` = 'dataItem.Report_No';
            ";

            sql = sql.Replace("dataItem.REGULAR_NO_REF", dataItem.REGULAR_NO_REF);
            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.process", dataItem.process);
            sql = sql.Replace("dataItem.inProcStatus", dataItem.inProcStatus);
            sql = sql.Replace("dataItem.reportStatus", dataItem.reportStatus);
            return sql;
        }

        //????? prevProcess ?????????????? skip = 3
        public string SearchForOpFunction(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
               b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date
               
               FROM `db_report_status` a
               join db_receive_mat b on (a.Report_No = b.Report_No)
               join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
               join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
               join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
               
               LEFT JOIN info_status iStatus ON (a.dataItem.process = iStatus.ID)

               where ((a.dataItem.prevProcess = 3 and a.Packing_Check = 1) or (a.Inspection_Data_Check = 1 and e.Keep_Data_Need = 1))
                       and (a.dataItem.process is NULL or a.dataItem.process = 8 or a.dataItem.process = 2 ) and (a.report_status != 6 or a.report_status != 0)
                       and (e.dataItem.process_Need = 1)
               ";

            sql = sql.Replace("dataItem.prevProcess", dataItem.prevProcess);
            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }
      

        public string FunctionSampling(QAdataProperty dataItem)
        {

            sql = @"SELECT a.sampling_type , b.sampling_type_name , a.Cavity_Qty , a.Sampling_Qty , a.Cavity_Name
                    FROM info_function_sampling a 
                    JOIN info_sampling_type  b on a.sampling_type = b.sampling_type
                    WHERE M_Code = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string FunctionSampQtyLotSize(QAdataProperty dataItem)
        {

            sql = @"SELECT m.Lot_Size, d.Min_Qty, d.Max_Qty, d.Sampling_Qty
                    FROM db_receive_mat m 
                    JOIN info_function_sampling a ON a.M_Code = m.M_Code
                    JOIN info_strictness_type b ON a.Strictness_Type = b.Strictness_Type
                    JOIN info_strictness_level c ON a.Strictness_Level = c.Strictness_Level
                    JOIN info_strictness d ON b.Strictness_Type = d.Strictness_Type AND c.Strictness_Level = d.Strictness_Level
                    WHERE m.REPORT_NO = 'dataItem.Report_No'
                    AND m.lot_size BETWEEN d.Min_Qty AND d.Max_Qty
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }

        public string UpdateReportStatusLotNo(QAdataProperty dataItem)
        {
            sql = @"UPDATE `db_report_status` 
             SET `dataItem.process_Lot_No` = 'dataItem.LotNo' , `dataItem.process` = dataItem.inProcStatus    WHERE `Report_No` = 'dataItem.Report_No'";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.process", dataItem.process);
            sql = sql.Replace("dataItem.LotNo", dataItem.Lot_No);
            sql = sql.Replace("dataItem.inProcStatus", dataItem.inProcStatus);

            return sql;

        }

        public List<string> InsertReportLotNoList(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            DataTable dt = dataItem.dtLotNo;

            foreach (DataRow row in dt.Rows)
            {
                sql = @"INSERT IGNORE INTO `db_report_lot_no`(`REPORT_NO`, `LOT_NO`) VALUES ('dataItem.Report_No', 'dataItem.Lot_No');";
    
                sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
                sql = sql.Replace("dataItem.Lot_No", row["Lot_No"].ToString());

                sqlList.Add(sql);

            }
            return sqlList;
        }

        //public List<string> InsertFunctionData(QAdataProperty dataItem)
        //{
        //    List<string> sqlList = new List<string>();

        //    foreach (DataRow row in dataItem.dtFuncData.Rows)
        //    {

        //        //sql = @"INSERT INTO `db_regular_data`(`REGULAR_NO`, `CAVITY_NAME`, `SAMPLING_NO`, `EQUIPMENT_SERIAL_ID`, `POINT_ORDER`, `VALUE`, `JUDGE`,`EMP_ID`,`REGULAR_DATE`,`INUSE`) 
        //        //        VALUES ('dataItem.REGULAR_NO',dataItem.CAVITY_NAME , 'dataItem.SAMPLING_NO', 'dataItem.EQUIPMENT_SERIAL_ID', 'dataItem.POINT_ORDER', 'dataItem.VALUE', 'dataItem.JUDGE', 'dataItem.EMP_ID' ,NOW() , 1);";

        //        sql = @"
        //         -- ?????? `INUSE` ???????????????????????? 0
        //            UPDATE `db_function_data`
        //            SET `INUSE` = 0
        //            WHERE `REPORT_NO` = 'dataItem.REPORT_NO'
        //            AND `SAMPLING_NO` = 'dataItem.SAMPLING_NO';

        //            -- ????????????????? `COUNT` ????????????? 1 ??? `INUSE = 1`
        //            INSERT INTO `db_regular_data` (`REPORT_NO`, `CAVITY_NAME`, `SAMPLING_NO`, `LOT_NO` , `COUNT`, `JUDGE`, `REMARK` , `EMP_ID`, `FUNCTION_DATE`, `INUSE`)
        //            SELECT 
        //                'dataItem.REPORT_NO',
        //                dataItem.CAVITY_NAME,
        //                'dataItem.SAMPLING_NO',
        //                'dataItem.LOT_NO',
        //                COALESCE(MAX(`COUNT`), 0) + 1,  -- ????? COUNT ?????????????? 1
        //                'dataItem.JUDGE',
        //                'dataItem.REMARK',
        //                'dataItem.EMP_ID',
        //                NOW(),
        //                1  -- ?????????? INUSE = 1
        //            FROM `db_function_data`
        //            WHERE `REPORT_NO` = 'dataItem.REPORT_NO'
        //            AND `SAMPLING_NO` = 'dataItem.SAMPLING_NO';

        //            ";

        //        sql = sql.Replace("dataItem.REPORT_NO", dataItem.Report_No);
        //        sql = sql.Replace("dataItem.EMP_ID", dataItem.EMP_ID);
        //        sql = sql.Replace("dataItem.SAMPLING_NO", row["SAMPLING_NO"].ToString());
        //        sql = sql.Replace("dataItem.LOT_NO", row["LOT_NO"].ToString());
        //        sql = sql.Replace("dataItem.JUDGE", row["POINT_JUDGE"].ToString());

        //        // ????????????????? CAVITY_NAME ???????? DataTable ???????
        //        if (dataItem.dtFuncData.Columns.Contains("CAVITY_NAME"))
        //        {
        //            // ?????????? row["CAVITY_NAME"] ???? null ???????????????
        //            string cavityName = row["CAVITY_NAME"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["CAVITY_NAME"].ToString())
        //                ? $"'{row["CAVITY_NAME"].ToString()}'"
        //                : "NULL";
        //            sql = sql.Replace("dataItem.CAVITY_NAME", cavityName);
        //        }
        //        else
        //        {
        //            sql = sql.Replace("dataItem.CAVITY_NAME", "NULL");
        //        }

        //        // ????????????????? CAVITY_NAME ???????? DataTable ???????
        //        if (dataItem.dtFuncData.Columns.Contains("REMARK"))
        //        {
        //            // ?????????? row["CAVITY_NAME"] ???? null ???????????????
        //            string remark = row["REMARK"] != DBNull.Value && !string.IsNullOrWhiteSpace(row["REMARK"].ToString())
        //                ? $"'{row["REMARK"].ToString()}'"
        //                : "NULL";
        //            sql = sql.Replace("dataItem.REMARK", remark);
        //        }
        //        else
        //        {
        //            sql = sql.Replace("dataItem.REMARK", "NULL");
        //        }

        //        sqlList.Add(sql);
        //    }

        //    return sqlList;
        //}

        public List<string> InsertFunctionData(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();

            foreach (DataRow row in dataItem.dtFuncData.Rows)
            {
                // ????????? INUSE ???? 0 ????????????????
                var updateQuery = $"UPDATE `db_function_data` " +
                                  $"SET `INUSE` = 0 " +
                                  $"WHERE `REPORT_NO` = '{dataItem.Report_No}' " +
                                  $"AND `SAMPLING_NO` = '{row["SAMPLING_NO"].ToString()}';";
                sqlList.Add(updateQuery);

                // ??? Derived Table ???????? Error 1093
                var countSubQuery = $"(SELECT COALESCE(MAX(tmp.`COUNT`), 0) + 1 FROM " +
                                    $"(SELECT `COUNT` FROM `db_function_data` WHERE `REPORT_NO` = '{dataItem.Report_No}' " +
                                    $"AND `SAMPLING_NO` = '{row["SAMPLING_NO"].ToString()}') AS tmp)";

                // ??????????? CAVITY_NAME ??? REMARK
                var cavityNameValue = row.Table.Columns.Contains("CAVITY_NAME") && row["CAVITY_NAME"] != DBNull.Value
                    ? $"'{row["CAVITY_NAME"].ToString().Replace("'", "''")}'"
                    : "NULL";
                var remarkValue = row.Table.Columns.Contains("REMARK") && row["REMARK"] != DBNull.Value
                    ? $"'{row["REMARK"].ToString().Replace("'", "''")}'"
                    : "NULL";

                // ?????? INSERT ?????? Derived Table
                var insertQuery = $"INSERT INTO `db_function_data` (`REPORT_NO`, `CAVITY_NAME`, `SAMPLING_NO`, `LOT_NO`, `COUNT`, `JUDGE`, `REMARK`, `EMP_ID`, `FUNCTION_DATE`, `INUSE`) " +
                                  $"VALUES ('{dataItem.Report_No}', {cavityNameValue}, '{row["SAMPLING_NO"].ToString()}', '{row["LOT_NO"].ToString()}', " +
                                  $"{countSubQuery}, '{row["POINT_JUDGE"].ToString()}', {remarkValue}, '{dataItem.EMP_ID}', NOW(), 1);";
                sqlList.Add(insertQuery);
            }

            // ??????????? Report
            var updateStatusQuery = $"UPDATE `db_report_status` " +
                                    $"SET `Function_Check` = '{dataItem.inProcStatus}' " +
                                    $"WHERE `Report_No` = '{dataItem.Report_No}';";
            sqlList.Add(updateStatusQuery);

            return sqlList;
        }

        public string SearchForFunctionPending(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
                     b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date
    
                     FROM `db_report_status` a
                     join db_receive_mat b on (a.Report_No = b.Report_No)
                     join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
                     join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
                     join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
                     LEFT JOIN info_status iStatus ON (a.dataItem.process = iStatus.ID)

                     where (a.dataItem.process = 6) 
                    ";

            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string SearchFunctionDataPending(QAdataProperty dataItem)

        {
            sql = @"select a.SAMPLING_NO  , a.CAVITY_NAME , a.`JUDGE` , a.`REMARK` , a.EMP_ID , a.FUNCTION_DATE
               
                   from db_function_data a 
                   join db_receive_mat b on (a.Report_No = b.Report_No)
              
                   where a.Report_No = 'dataItem.Report_No' and inuse = 1 and JUDGE= 0
   
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;

        }

        public string SearchReportActive(QAdataProperty dataItem)
        {
            sql = @"SELECT `Active_User` , COMPUTER_NAME 
                    FROM info_report_active 
                    WHERE Report_No = 'dataItem.Report_No' and PROCESS = 'dataItem.process' ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string InsertReportActive(QAdataProperty dataItem)
        {
            sql = @"INSERT INTO `info_report_active`(`REPORT_NO`,`PROCESS` , `Active_User` , `COMPUTER_NAME`) VALUES ('dataItem.Report_No', 'dataItem.process' , 'dataItem.myIPv4' , 'dataItem.MY_COMPUTER_NAME');
 ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.process", dataItem.process);
            sql = sql.Replace("dataItem.myIPv4", dataItem.myIPv4);
            sql = sql.Replace("dataItem.MY_COMPUTER_NAME", dataItem.MY_COMPUTER_NAME);


            return sql;

        }

        public string DeleteReportActive(QAdataProperty dataItem)
        {
            sql = @"DELETE FROM `info_report_active` WHERE `Report_No` = 'dataItem.Report_No' and PROCESS = 'dataItem.process' ;";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;

        }

        public string CheckReportStatus(QAdataProperty dataItem)
        {
            sql = @"SELECT report_status
            FROM `db_report_status`
            where Report_No = 'dataItem.Report_No'";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }

        public string ReportFDA_Status(QAdataProperty dataItem)
        {
            sql = @"SELECT Function_Check , Dimension_Check , Appearance_Check 
            FROM `db_report_status` 
            where Report_No = 'dataItem.Report_No' ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }

        public string UpdateReportStatus(QAdataProperty dataItem)
        {
            string reportStatus = string.IsNullOrWhiteSpace(dataItem.reportStatus)
                ? dataItem.TOTAL_STATUS
                : dataItem.reportStatus;

            string processStatus = string.IsNullOrWhiteSpace(dataItem.inProcStatus)
                ? reportStatus
                : dataItem.inProcStatus;

            if (string.IsNullOrWhiteSpace(dataItem.process))
            {
                sql = @"UPDATE `db_report_status` 
      SET `report_status` = dataItem.reportStatus
      WHERE `Report_No` = 'dataItem.Report_No'";
            }
            else
            {
                sql = @"UPDATE `db_report_status` 
      SET `report_status` = dataItem.reportStatus,
          `dataItem.process` = dataItem.inProcStatus
      WHERE `Report_No` = 'dataItem.Report_No'";

                sql = sql.Replace("dataItem.process", dataItem.process);
                sql = sql.Replace("dataItem.inProcStatus", ToSqlIntOrNull(processStatus));
            }

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.reportStatus", ToSqlIntOrNull(reportStatus));

            return sql;

        }

        public string DimensionSampling(QAdataProperty dataItem)
        {
            sql = @"SELECT a.sampling_type , b.sampling_type_name , a.Cavity_Qty , a.Sampling_Qty , a.Cavity_Name
              FROM info_dimension_sampling a 
              JOIN info_sampling_type  b on a.sampling_type = b.sampling_type
              WHERE M_Code = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string DimensionEquipment(QAdataProperty dataItem)
        {
            sql = @"SELECT a.POINT_ORDER ,a.EQUIPMENT_TYPE , b.Equipment_Name , a.POINT_NAME , a.POINT_CAL , a.CRITERIA_MIN , a.CRITERIA_MAX 
                    FROM `info_dimension_equipment` a
                    JOIN info_equipment_type b on (a.EQUIPMENT_TYPE = b.Equipment_Type)
                    WHERE M_Code = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string SearchForDimensionPending(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
               b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date

               FROM `db_report_status` a

               join db_receive_mat b on (a.Report_No = b.Report_No)
               join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
               join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
               join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
               LEFT JOIN info_status iStatus ON (a.dataItem.process = iStatus.ID)

               where (a.dataItem.process = 6) 
               ";

            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string SearchDimensionDataPending(QAdataProperty dataItem)

        {
            sql = @"select a.SAMPLING_NO  , a.CAVITY_NAME, c.POINT_CAL , c.POINT_ORDER , c.POINT_NAME ,a.EQUIPMENT_SERIAL_ID , d.Equipment_Type , d.Equipment_Name , a.`VALUE` , c.CRITERIA_MIN , c.CRITERIA_MAX
              
              from db_dimension_data a 
              join db_receive_mat b on (a.Report_No = b.Report_No)
              join info_dimension_equipment c on (b.M_Code = c.M_CODE  and a.POINT_ORDER = c.POINT_ORDER)
              join info_equipment_type d on (c.EQUIPMENT_TYPE = d.Equipment_Type)

              where a.Report_No = 'dataItem.Report_No' and inuse = 1 and JUDGE= 0
   
            ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;

        }

        public string SearchForOpDimension(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
          b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date
          
          FROM `db_report_status` a
          join db_receive_mat b on (a.Report_No = b.Report_No)
          join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
          join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
          join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
          
          LEFT JOIN info_status iStatus ON (a.dataItem.process = iStatus.ID)

          where ((a.dataItem.prevProcess = 3 and a.Packing_Check = 1) or (a.dataItem.prevProcess = 1 and e.Keep_Data_Need = 1))
                  and (a.dataItem.process is NULL or a.dataItem.process = 8 or a.dataItem.process = 2) and (a.report_status != 6 or a.report_status != 0)
                  and (e.dataItem.process_Need = 1)
          ";

            sql = sql.Replace("dataItem.prevProcess", dataItem.prevProcess);
            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string DimensionSampQtyLotSize(QAdataProperty dataItem)
        {

            sql = @"SELECT m.Lot_Size, d.Min_Qty, d.Max_Qty, d.Sampling_Qty
                    FROM db_receive_mat m 
                    JOIN info_dimension_sampling a ON a.M_Code = m.M_Code
                    JOIN info_strictness_type b ON a.Strictness_Type = b.Strictness_Type
                    JOIN info_strictness_level c ON a.Strictness_Level = c.Strictness_Level
                    JOIN info_strictness d ON b.Strictness_Type = d.Strictness_Type AND c.Strictness_Level = d.Strictness_Level
                    WHERE m.REPORT_NO = 'dataItem.Report_No'
                    AND m.lot_size BETWEEN d.Min_Qty AND d.Max_Qty
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }

        private string ToSqlValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "NULL";
            }

            string text = value.ToString();

            if (string.IsNullOrWhiteSpace(text))
            {
                return "NULL";
            }

            return $"'{text.Replace("'", "''")}'";
        }

        private string ToSqlTextValue(object value)
        {
            string text = value == null || value == DBNull.Value ? string.Empty : value.ToString();
            return $"'{text.Replace("'", "''")}'";
        }

        private string ToSqlIntOrNull(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "NULL";
            }

            return int.TryParse(value.ToString(), out int number) ? number.ToString() : "NULL";
        }

        private string ToSqlLongOrNull(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "NULL";
            }

            return long.TryParse(value.ToString(), out long number) ? number.ToString() : "NULL";
        }

        public List<string> InsertDimensionData(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();

            if (dataItem == null || dataItem.dtgDimData == null || dataItem.dtgDimData.DataSource == null)
            {
                return sqlList;
            }

            DataTable dt = (DataTable)dataItem.dtgDimData.DataSource;

            foreach (DataRow row in dt.Rows)
            {
                string reportNo = ToSqlTextValue(dataItem.Report_No);
                string empId = ToSqlTextValue(dataItem.EMP_ID);

                string samplingNo = ToSqlTextValue(row["SAMPLING_NO"]);
                string pointOrder = ToSqlTextValue(row["POINT_ORDER"]);
                string value = ToSqlTextValue(row["VALUE"]);
                string judge = ToSqlTextValue(row["POINT_JUDGE"]);
                string equipmentSerial = ToSqlTextValue(row["EQUIPMENT_SERIAL"]);

                string cavityValue = "NULL";

                if (dt.Columns.Contains("CAVITY_NAME"))
                {
                    cavityValue = ToSqlValue(row["CAVITY_NAME"]);
                }

                string cavityWhere;

                if (cavityValue == "NULL")
                {
                    cavityWhere = "(`CAVITY_NAME` IS NULL OR `CAVITY_NAME` = '')";
                }
                else
                {
                    cavityWhere = $"`CAVITY_NAME` = {cavityValue}";
                }

                sql = $@"
            UPDATE `db_dimension_data`
            SET `INUSE` = 0
            WHERE `Report_No` = {reportNo}
              AND {cavityWhere}
              AND `POINT_ORDER` = {pointOrder}
              AND `SAMPLING_NO` = {samplingNo};

            INSERT INTO `db_dimension_data`
            (
                `Report_No`,
                `CAVITY_NAME`,
                `SAMPLING_NO`,
                `COUNT`,
                `EQUIPMENT_SERIAL_ID`,
                `POINT_ORDER`,
                `VALUE`,
                `JUDGE`,
                `EMP_ID`,
                `DIMENSION_DATE`,
                `INUSE`
            )
            SELECT
                {reportNo},
                {cavityValue},
                {samplingNo},
                COALESCE(MAX(`COUNT`), 0) + 1,
                {equipmentSerial},
                {pointOrder},
                {value},
                {judge},
                {empId},
                NOW(),
                1
            FROM `db_dimension_data`
            WHERE `Report_No` = {reportNo}
              AND {cavityWhere}
              AND `POINT_ORDER` = {pointOrder}
              AND `SAMPLING_NO` = {samplingNo};
        ";

                sqlList.Add(sql);
            }

            return sqlList;
        }


        public string NeedDimensionCheck(QAdataProperty dataItem)
        {
            sql = @"SELECT `Dimension_Check_Need` FROM `info_mat_inspection_list`
       where M_CODE = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string SearchForOpData(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
             b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date
         
             FROM `db_report_status` a
             join db_receive_mat b on (a.Report_No = b.Report_No)
             join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
             join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
             join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
         
             LEFT JOIN info_status iStatus ON (a.dataItem.process = iStatus.ID)

             where ((a.dataItem.prevProcess = 3 and a.Packing_Check = 1) or (a.dataItem.prevProcess = 1 and e.dataItem.prevProcess_Need = 1))
                     and (a.dataItem.process is NULL or a.dataItem.process = 8 or a.dataItem.process = 2 ) and (a.report_status != 6 or a.report_status != 0)
                     and (e.Keep_Data_Need = 1)
             ";

            sql = sql.Replace("dataItem.prevProcess", dataItem.prevProcess);
            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string InsertUpdateInspData(QAdataProperty dataItem)
        {
            //var compiler = new MySqlCompiler();
            var queries = new List<string>();
            var setUpdateInuse = $"UPDATE `db_inspection_data` SET `INUSE` = 0 WHERE `REPORT_NO` = '{dataItem.Report_No}'";
            queries.Add(setUpdateInuse);

            // ???? SET @nextCount"
            // ????? raw SQL ???????????????????
            var setCountQuery = $"SET @nextCount = (SELECT COALESCE(MAX(`COUNT`), 0) + 1 FROM `db_inspection_data` WHERE `REPORT_NO` = '{dataItem.Report_No}')";
            queries.Add(setCountQuery);

            // ???? INSERT
            // ????? raw SQL ???????????????????
            var detailJudgeValue = string.IsNullOrEmpty(dataItem.data_detail) ? "NULL" : $"'{dataItem.data_detail.Replace("'", "''")}'";
            var insertQuery = $"INSERT INTO `db_inspection_data` (`REPORT_NO`, `COUNT`, `REMARK`, `JUDGE`, `EMP_ID` , `INSPECTION_DATA_DATE`, `INUSE`) " +
                              $"VALUES ('{dataItem.Report_No}', @nextCount, {detailJudgeValue}, '{dataItem.judge}', '{dataItem.EMP_ID}' , NOW() , 1 )";
            queries.Add(insertQuery);

            // ??????????? Report
            var updateStatusQuery = $"UPDATE `db_report_status` " +
                                $"SET `inspection_data_Check` = '{dataItem.judge}' , `report_status` = '{dataItem.judge}' " + 
                            $"WHERE `Report_No` = '{dataItem.Report_No}'";
            queries.Add(updateStatusQuery);

            // ??? queries ???? string ?????
            var sql = string.Join(";\n", queries);
            return sql;
        }

        public string SearchForInspDataPending(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
               b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date
    
               FROM `db_report_status` a
               join db_receive_mat b on (a.Report_No = b.Report_No)
               join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
               join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
               join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
               LEFT JOIN info_status iStatus ON (a.dataItem.process = iStatus.ID)

               where (a.dataItem.process = 6) 
              ";

            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string SearchDataInspDataPending(QAdataProperty dataItem)

        {
            sql = @"select a.`JUDGE` , a.`REMARK` , a.EMP_ID , a.inspection_data_date
         
             from db_inspection_data a 
             join db_receive_mat b on (a.Report_No = b.Report_No)
        
             where a.Report_No = 'dataItem.Report_No' and inuse = 1 and (JUDGE = 0 or JUDGE = 6)
   
              ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;

        }

        public string SearchForOpAppear(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , b.Lot_Size as `Lot Size` ,
                    b.Inspection_Qty as `Inspection Qty` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date
    
                    FROM `db_report_status` a
                    join db_receive_mat b on (a.Report_No = b.Report_No)
                    join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
                    join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
                    join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
    
                    LEFT JOIN info_status iStatus ON (a.dataItem.process = iStatus.ID)

                    where ((a.dataItem.prevProcess = 3 and a.Packing_Check = 1) or (a.dataItem.prevProcess = 1 and e.Keep_Data_Need = 1))
                            and (a.dataItem.process is NULL or a.dataItem.process = 8 or a.dataItem.process = 2) and (a.report_status != 6 or a.report_status != 0)
                            and (e.dataItem.process_Need = 1) and b.Inspection_Qty IS NOT NULL
                    ";

            sql = sql.Replace("dataItem.prevProcess", dataItem.prevProcess);
            sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

        public string NeedAppearCheck(QAdataProperty dataItem)
        {
            sql = @"SELECT `Appearance_Check_Need` FROM `info_mat_inspection_list`
                    where M_CODE = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string AppearSampQtyLotSize(QAdataProperty dataItem)
        {

            sql = @"SELECT d.Min_Qty, d.Max_Qty, d.Sampling_Qty
             FROM info_appearance_sampling a 
             JOIN info_strictness_type b ON a.Strictness_Type = b.Strictness_Type
             JOIN info_strictness_level c ON a.Strictness_Level = c.Strictness_Level
             JOIN info_strictness d ON b.Strictness_Type = d.Strictness_Type AND c.Strictness_Level = d.Strictness_Level
             WHERE a.M_Code = 'dataItem.M_CODE'
             AND dataItem.VALUE BETWEEN d.Min_Qty AND d.Max_Qty
             ";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);
            sql = sql.Replace("dataItem.VALUE", dataItem.VALUE);
            return sql;
        }

        public string AppearSampling(QAdataProperty dataItem)
        {
            sql = @"SELECT a.sampling_type , b.sampling_type_name , a.Cavity_Qty , a.Sampling_Qty , a.Cavity_Name , b.Allow_Continue
                   FROM info_appearance_sampling a 
                   JOIN info_sampling_type  b on a.sampling_type = b.sampling_type
                   WHERE M_Code = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string UpdateInspQtyAppear(QAdataProperty dataItem)
        {
            sql = @"UPDATE `db_receive_mat` 
                    SET `Inspection_Qty` = dataItem.inspQty WHERE `Report_No` = 'dataItem.Report_No'";

            sql = sql.Replace("dataItem.inspQty", dataItem.inspQty);
            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
           
            return sql;

        }

        public string SearchPackingSize(QAdataProperty dataItem)
        {
            sql = @"select `BATCH` , `VALUE` , `PACK_COUNT` , PACKING_SIZE
        from db_packing_size
        where REPORT_NO = 'dataItem.Report_No'";

        sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;

        }

        public string SearchAppearData(QAdataProperty dataItem)
        {
            bool isAllAppearance = dataItem.SAMPLING_TYPE == "1"
                || string.Equals(dataItem.SAMPLING_NAME?.Trim(), "All", StringComparison.OrdinalIgnoreCase);

            if (isAllAppearance)
            {
                sql = @"SELECT MIN(`APPEARANCE_ID`) AS APPEARANCE_ID,
                               COALESCE(`APPEARANCE_DATE`, DATE(`UPDATETIME`)) AS APPEARANCE_DATE,
                               `BATCH`,
                               MIN(`COUNT`) AS `COUNT`,
                               SUM(COALESCE(`QTY_SELECT`, 0)) AS QTY_SELECT,
                               SUM(COALESCE(`QTY_OK`, 0)) AS QTY_OK,
                               SUM(COALESCE(`QTY_NG`, 0)) AS QTY_NG,
                               EMP_ID,
                               CASE
                                   WHEN SUM(COALESCE(`QTY_NG`, 0)) > 0 THEN 0
                                   ELSE 1
                               END AS JUDGE
                        FROM `db_appearance_data`
                        WHERE REPORT_NO = 'dataItem.Report_No'
                          AND BATCH = 'dataItem.Batch'
                          AND INUSE = 1
                        GROUP BY
                            COALESCE(`APPEARANCE_DATE`, DATE(`UPDATETIME`)),
                            `BATCH`,
                            `EMP_ID`
                        ORDER BY
                            COALESCE(`APPEARANCE_DATE`, DATE(`UPDATETIME`)),
                            `EMP_ID`";
            }
            else
            {
                sql = @"SELECT `APPEARANCE_ID`,
                               COALESCE(`APPEARANCE_DATE`, DATE(`UPDATETIME`)) AS APPEARANCE_DATE,
                               `BATCH`,
                               `COUNT`,
                               QTY_SELECT,
                               QTY_OK,
                               QTY_NG,
                               EMP_ID,
                               JUDGE
                        FROM `db_appearance_data`
                        WHERE REPORT_NO = 'dataItem.Report_No'
                          AND BATCH = 'dataItem.Batch'
                          AND INUSE = 1";
            }

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);
            sql = sql.Replace("dataItem.Batch", dataItem.BATCH);
            return sql;

        }

        public string SearchSampleSize(QAdataProperty dataItem)
        {
            sql = @"SELECT 
                        p.BATCH as `BATCH`,
                        p.PACK_COUNT as `PACK_COUNT`,
                        p.`VALUE` as `VALUE` ,
                        p.packing_size - COALESCE(SUM(a.qty_select), 0) as `REMAIN_PACKING_SIZE`,
                        p.`PACKING_SIZE` as `PACKING_SIZE`
                FROM 
                    db_packing_size p
                LEFT JOIN 
                    db_appearance_data a 
                    ON p.report_no = a.report_no 
                    AND p.BATCH = a.BATCH AND a.inuse = 1

                WHERE p.REPORT_NO = 'dataItem.Report_No'

                GROUP BY 
                    p.report_no,
                    p.BATCH;
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }

        public string InsertAppearData(QAdataProperty dataItem)
        {

            sql = @"INSERT INTO `db_appearance_data` (`REPORT_NO`, `BATCH`, `COUNT`, `QTY_SELECT`, `QTY_OK`, `QTY_NG`, `EMP_ID`, `JUDGE`, `APPEARANCE_DATE`, `UPDATETIME`, `INUSE`) " +
            $"VALUES ({ToSqlTextValue(dataItem.Report_No)}, {ToSqlIntOrNull(dataItem.BATCH)}, {ToSqlIntOrNull(dataItem.COUNT)}, {ToSqlIntOrNull(dataItem.QTY_SELECT)}, {ToSqlIntOrNull(dataItem.QTY_OK)}, {ToSqlIntOrNull(dataItem.QTY_NG)}, {ToSqlTextValue(dataItem.EMP_ID)}, {ToSqlIntOrNull(dataItem.judge)}, CURDATE(), NOW(), 1)";

            return sql;

        }

        public string GetLatestAppearDataId(QAdataProperty dataItem)
        {
            sql = $@"SELECT `APPEARANCE_ID`
                    FROM `db_appearance_data`
                    WHERE `REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                      AND `BATCH` = {ToSqlIntOrNull(dataItem.BATCH)}
                      AND `COUNT` = {ToSqlIntOrNull(dataItem.COUNT)}
                      AND `EMP_ID` = {ToSqlTextValue(dataItem.EMP_ID)}
                      AND `APPEARANCE_DATE` = CURDATE()
                      AND `INUSE` = 1
                    ORDER BY `APPEARANCE_ID` DESC
                    LIMIT 1";

            return sql;
        }

        public List<string> InsertAppearPendingDetail(QAdataProperty dataItem)
        {
            int ngCount = 0;
            List<string> sqlList = new List<string>();
            DataTable dt = (DataTable)dataItem.dtg_ngMode.DataSource;

            foreach (DataRow row in dt.Rows)
            {

                string ngDetail = row.Table.Columns.Contains("NG_DETAIL") ? row["NG_DETAIL"].ToString().Replace("'", "''") : "";
                string ngModeId = ToSqlIntOrNull(row.Table.Columns.Contains("NG_MODE_ID") ? row["NG_MODE_ID"] : null);
                string appearanceId = ToSqlLongOrNull(dataItem.APPEARANCE_ID);
                sql = $"INSERT INTO `db_appearance_pending`(`APPEARANCE_ID`, `REPORT_NO`, `BATCH`, `COUNT`, `NG_COUNT`, `QTY_NG`, `NG_DETAIL`, `NG_MODE_ID`, `APPEARANCE_DATE`, `UPDATETIME`) " +
                      $"VALUES({appearanceId}, {ToSqlTextValue(dataItem.Report_No)}, {ToSqlIntOrNull(dataItem.BATCH)}, {ToSqlIntOrNull(dataItem.COUNT)}, {ngCount}, {ToSqlIntOrNull(row["QTY_NG"])}, '{ngDetail}', {ngModeId}, NOW(), NOW())";

                sqlList.Add(sql);
                ngCount++;
            }
            return sqlList;

        }

        public string GetTotalInspected (QAdataProperty dataItem)
        {
            sql = $"SELECT sum(QTY_SELECT) FROM `db_appearance_data` WHERE REPORT_NO = '{dataItem.Report_No}' and inuse = 1";

            return sql;
        }


        public string SearchForAppearPending()
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
               b.Lot_Size as `Lot Size` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.Appearance_Check as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date

               FROM `db_report_status` a

               join db_receive_mat b on (a.Report_No = b.Report_No)
               join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
               join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
               join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
               LEFT JOIN info_status iStatus ON (a.Appearance_Check = iStatus.ID)

               where (a.Appearance_Check = 6) 
               ";

            //sql = sql.Replace("dataItem.process", dataItem.process);

            return sql;
        }

    }
}
