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
            if (dataItem.process == "Appearance_Check")
            {
                sql = @"select count(distinct COALESCE(CAST(p.APPEARANCE_ID AS CHAR), CONCAT(p.REPORT_NO, '|', COALESCE(CAST(p.BATCH AS CHAR), ''), '|', COALESCE(CAST(p.COUNT AS CHAR), ''), '|', COALESCE(p.LOT_NO, '')))) as `cnt`
                        from db_appearance_pending p
                        join db_report_status a on (p.REPORT_NO = a.Report_No)
                        join db_receive_mat b on (a.Report_No = b.Report_No)
                        join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
                        where p.RESULT IS NULL
                          and COALESCE(p.QTY_NG, 0) > 0
                          and e.Appearance_Check_Need = 1
                        ";

                return sql;
            }

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
            sql = @"SELECT b.Receive_Date as `Receive Date` , b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
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
            sql = @"SELECT b.Receive_Date as `Receive Date` , b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
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
                sql = sql.Replace("dataItem.dim_check_status", $"'{dataItem.dim_check_status}'");
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
            sql = @"SELECT a.POINT_ORDER ,a.EQUIPMENT_TYPE , b.Equipment_Name , a.POINT_NAME , a.POINT_CAL , a.CRITERIA_MIN , a.CRITERIA_MAX , a.JUDGE_TYPE 
                    FROM `info_regular_equipment` a
                    JOIN info_equipment_type b on (a.EQUIPMENT_TYPE = b.Equipment_Type)
                    WHERE M_Code = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        // รายการ S/N ที่มีอยู่แล้ว ใช้เติม Dropdown ให้ช่อง EQ_SN เลือกแทนการพิมพ์เอง
        public string EquipmentSerialList()
        {
            return @"SELECT s.`ID`, s.`EQUIPMENT_SERIAL`, s.`EQUIPMENT_TYPE_ID`
                     FROM `info_equipment_serial` s
                     WHERE s.`EQUIPMENT_SERIAL` IS NOT NULL
                       AND TRIM(s.`EQUIPMENT_SERIAL`) <> ''
                     ORDER BY s.`EQUIPMENT_TYPE_ID`, s.`EQUIPMENT_SERIAL`";
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
            sql = @"select a.SAMPLING_NO  , a.CAVITY_NAME, c.POINT_CAL , c.POINT_ORDER , c.POINT_NAME ,a.EQUIPMENT_SERIAL_ID , d.Equipment_Type , d.Equipment_Name , a.`VALUE` , c.CRITERIA_MIN , c.CRITERIA_MAX , c.JUDGE_TYPE
                    
                    from db_regular_data a 
                    join db_receive_mat b on (a.REGULAR_NO = b.Regular_No)
                    join info_regular_equipment c on (b.M_Code = c.M_CODE  and a.POINT_ORDER = c.POINT_ORDER)
                    join info_equipment_type d on (c.EQUIPMENT_TYPE = d.Equipment_Type)

                    where a.REGULAR_NO = 'dataItem.Regular_No' and inuse = 1 and JUDGE= 0
   
            ";

            sql = sql.Replace("dataItem.Regular_No", dataItem.Regular_No);

            return sql;

        }

        public string SearchRegularReportData(QAdataProperty dataItem)

        {
            sql = @"select a.SAMPLING_NO, a.CAVITY_NAME, c.POINT_CAL, c.POINT_ORDER, c.POINT_NAME,
                    a.EQUIPMENT_SERIAL_ID, d.Equipment_Type, d.Equipment_Name, a.`VALUE`,
                    c.CRITERIA_MIN, c.CRITERIA_MAX, a.JUDGE AS POINT_JUDGE
                    from db_regular_data a
                    join db_receive_mat b on (a.REGULAR_NO = b.Regular_No)
                    join info_regular_equipment c on (b.M_Code = c.M_CODE and a.POINT_ORDER = c.POINT_ORDER)
                    left join info_equipment_type d on (c.EQUIPMENT_TYPE = d.Equipment_Type)
                    where a.REGULAR_NO = 'dataItem.Regular_No' and a.INUSE = 1
                    order by a.SAMPLING_NO, c.POINT_ORDER, a.CAVITY_NAME
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
            sql = @"SELECT b.Receive_Date as `Receive Date` , b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
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

        public string FunctionEquipment(QAdataProperty dataItem)
        {
            sql = @"SELECT a.POINT_ORDER, a.EQUIPMENT_TYPE, b.Equipment_Name,
                           a.POINT_NAME, a.POINT_CAL, a.CRITERIA_MIN,
                           a.CRITERIA_MAX, a.UNIT
                    FROM info_function_equipment a
                    LEFT JOIN info_equipment_type b
                      ON a.EQUIPMENT_TYPE = b.Equipment_Type
                    WHERE a.M_CODE = 'dataItem.M_CODE'
                    ORDER BY a.POINT_ORDER ASC";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE.Replace("'", "''"));
            return sql;
        }

        public string FunctionCheckMethods(QAdataProperty dataItem)
        {
            return $@"SELECT ID, CHECK_ORDER, CHECK_DETAIL
                      FROM info_function_check_method
                      WHERE M_CODE = {ToSqlTextValue(dataItem.M_CODE)}
                        AND INUSE = 1
                      ORDER BY CHECK_ORDER";
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

        public string UpdateReportProcessLotNo(QAdataProperty dataItem)
        {
            sql = @"UPDATE `db_report_status`
                    SET `dataItem.process_Lot_No` = dataItem.LotNo
                    WHERE `Report_No` = dataItem.Report_No";

            sql = sql.Replace("dataItem.process", dataItem.process);
            sql = sql.Replace("dataItem.LotNo", ToSqlTextValue(dataItem.Lot_No));
            sql = sql.Replace("dataItem.Report_No", ToSqlTextValue(dataItem.Report_No));

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

                sqlList.Add($"UPDATE `db_function_check_result` SET `INUSE` = 0 " +
                            $"WHERE `REPORT_NO` = '{dataItem.Report_No}' " +
                            $"AND `SAMPLING_NO` = '{row["SAMPLING_NO"]}';");

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
                object equipmentSerialId = row.Table.Columns.Contains("EQUIPMENT_SERIAL_ID")
                    ? row["EQUIPMENT_SERIAL_ID"]
                    : null;
                var equipmentSerialIdValue = ToSqlIntOrNull(equipmentSerialId);

                // ?????? INSERT ?????? Derived Table
                var insertQuery = $"INSERT INTO `db_function_data` (`REPORT_NO`, `CAVITY_NAME`, `SAMPLING_NO`, `LOT_NO`, `EQUIPMENT_SERIAL_ID`, `COUNT`, `JUDGE`, `REMARK`, `EMP_ID`, `FUNCTION_DATE`, `INUSE`) " +
                                  $"VALUES ('{dataItem.Report_No}', {cavityNameValue}, '{row["SAMPLING_NO"].ToString()}', '{row["LOT_NO"].ToString()}', " +
                                  $"{equipmentSerialIdValue}, {countSubQuery}, '{row["POINT_JUDGE"].ToString()}', {remarkValue}, '{dataItem.EMP_ID}', NOW(), 1);";
                sqlList.Add(insertQuery);

                if (dataItem.dtFuncCheck != null && dataItem.dtFuncCheck.Rows.Count > 0)
                {
                    var insertedCount = $"(SELECT MAX(d.`COUNT`) FROM `db_function_data` d " +
                                        $"WHERE d.`REPORT_NO` = '{dataItem.Report_No}' " +
                                        $"AND d.`SAMPLING_NO` = '{row["SAMPLING_NO"]}')";

                    foreach (DataRow check in dataItem.dtFuncCheck.Rows)
                    {
                        string checkId = check["ID"]?.ToString();
                        string checkOrder = check["CHECK_ORDER"]?.ToString();
                        string checkDetail = check["CHECK_DETAIL"]?.ToString();
                        string resultColumn = "FUNCTION_CHECK_" + checkId;
                        if (!row.Table.Columns.Contains(resultColumn)) continue;

                        string detailJudge = ToSqlIntOrNull(row[resultColumn]);
                        sqlList.Add($@"INSERT INTO `db_function_check_result`
                            (`REPORT_NO`, `COUNT`, `SAMPLING_NO`, `CAVITY_NAME`, `LOT_NO`,
                             `FUNCTION_CHECK_ID`, `CHECK_ORDER`, `CHECK_DETAIL`, `JUDGE`,
                             `REMARK`, `EMP_ID`, `FUNCTION_DATE`, `INUSE`)
                            VALUES ({ToSqlTextValue(dataItem.Report_No)}, {insertedCount},
                                    {ToSqlIntOrNull(row["SAMPLING_NO"])}, {cavityNameValue},
                                    {ToSqlTextValue(row["LOT_NO"])}, {ToSqlIntOrNull(checkId)},
                                    {ToSqlIntOrNull(checkOrder)}, {ToSqlTextValue(checkDetail)},
                                    {detailJudge}, {remarkValue}, {ToSqlTextValue(dataItem.EMP_ID)}, NOW(), 1);");
                    }
                }
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
            sql = @"SELECT b.Receive_Date as `Receive Date` , b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
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
            sql = @"select a.SAMPLING_NO, a.CAVITY_NAME, a.LOT_NO, a.JUDGE, a.REMARK,
                           a.EMP_ID, a.FUNCTION_DATE, a.EQUIPMENT_SERIAL_ID,
                           s.EQUIPMENT_SERIAL,
                           COALESCE(s.EQUIPMENT_TYPE_ID, f.EQUIPMENT_TYPE) AS EQUIPMENT_TYPE,
                           COALESCE(t.Equipment_Name, ft.Equipment_Name) AS EQUIPMENT_NAME
               
                   from db_function_data a 
                   join db_receive_mat b on (a.Report_No = b.Report_No)
                   left join info_equipment_serial s on a.EQUIPMENT_SERIAL_ID = s.ID
                   left join info_equipment_type t on s.EQUIPMENT_TYPE_ID = t.Equipment_Type
                   left join info_function_equipment f
                     on b.M_Code = f.M_CODE and f.POINT_ORDER = 1
                   left join info_equipment_type ft on f.EQUIPMENT_TYPE = ft.Equipment_Type
              
                   where a.Report_No = 'dataItem.Report_No' and inuse = 1 and JUDGE= 0
   
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;

        }

        public string SearchFunctionCheckResultPending(QAdataProperty dataItem)
        {
            return $@"SELECT r.REPORT_NO, r.`COUNT`, r.SAMPLING_NO, r.CAVITY_NAME,
                             r.LOT_NO, r.FUNCTION_CHECK_ID, r.CHECK_ORDER,
                             r.CHECK_DETAIL, r.JUDGE, r.REMARK
                      FROM db_function_check_result r
                      JOIN db_function_data f
                        ON r.REPORT_NO = f.REPORT_NO
                       AND r.`COUNT` = f.`COUNT`
                       AND r.SAMPLING_NO = f.SAMPLING_NO
                       AND r.INUSE = f.INUSE
                      WHERE r.REPORT_NO = {ToSqlTextValue(dataItem.Report_No)}
                        AND r.INUSE = 1
                        AND f.JUDGE = 0
                      ORDER BY r.SAMPLING_NO, r.CHECK_ORDER";
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

        // อ่านสถานะของ process ที่กำลังทำอยู่ (คอลัมน์ชื่อเดียวกับ dataItem.process เช่น `Appearance_Check`)
        // เดิมอ่าน `report_status` ซึ่งเป็นคอลัมน์รวมทั้งใบที่ทุก process เขียนทับกันได้
        // ทำให้ Appearance ถูกบล็อกเพราะ Function/Dimension ติด Pending ทั้งที่ยังไม่ได้เริ่มตรวจ
        public string CheckReportStatus(QAdataProperty dataItem)
        {
            sql = @"SELECT `dataItem.process` AS proc_status
            FROM `db_report_status`
            where Report_No = 'dataItem.Report_No'";

            sql = sql.Replace("dataItem.process", dataItem.process);
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
            sql = @"SELECT a.POINT_ORDER ,a.EQUIPMENT_TYPE , b.Equipment_Name , a.POINT_NAME , a.POINT_CAL , a.CRITERIA_MIN , a.CRITERIA_MAX , a.JUDGE_TYPE 
                    FROM `info_dimension_equipment` a
                    JOIN info_equipment_type b on (a.EQUIPMENT_TYPE = b.Equipment_Type)
                    WHERE M_Code = 'dataItem.M_CODE'";

            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        public string SearchForDimensionPending(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
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
            sql = @"select a.SAMPLING_NO  , a.CAVITY_NAME, c.POINT_CAL , c.POINT_ORDER , c.POINT_NAME ,a.EQUIPMENT_SERIAL_ID , d.Equipment_Type , d.Equipment_Name , a.`VALUE` , c.CRITERIA_MIN , c.CRITERIA_MAX , c.JUDGE_TYPE
              
              from db_dimension_data a 
              join db_receive_mat b on (a.Report_No = b.Report_No)
              join info_dimension_equipment c on (b.M_Code = c.M_CODE  and a.POINT_ORDER = c.POINT_ORDER)
              join info_equipment_type d on (c.EQUIPMENT_TYPE = d.Equipment_Type)

              where a.Report_No = 'dataItem.Report_No' and inuse = 1 dataItem.judgeFilter
   
            ";

            // ปกติหน้า Pending โชว์เฉพาะจุดที่ตก Admin จะได้แก้เฉพาะจุดนั้น
            // แต่แบบ All ตัดสินจากผลต่างของทุกจุดในชิ้นเดียวกัน จึงต้องดึงมาทั้งชิ้น
            //   1) ชิ้นที่ทุกจุดอยู่ในเกณฑ์ แต่ผลต่างกว้างเกิน จะไม่มีแถว JUDGE = 0 เลย
            //      กรองไว้จะเปิดหน้ามาแล้วว่างเปล่า ทั้งที่ใบนั้นตกจริง
            //   2) คิดผลต่างใหม่ต้องใช้ค่าทุกจุด ถ้าโหลดมาแค่จุดที่ตก ผลต่างจะผิด
            string judgeFilter = dataItem.SAMPLING_TYPE == "1" ? "" : "and JUDGE = 0";

            sql = sql.Replace("dataItem.judgeFilter", judgeFilter);
            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;

        }

        public string SearchForOpDimension(QAdataProperty dataItem)
        {
            sql = @"SELECT b.Receive_Date as `Receive Date` , b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
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

            sqlList.AddRange(BuildDimensionPieceJudgeSql(dataItem, dt));

            return sqlList;
        }

        /// <summary>
        /// ผลตัดสินราย "ชิ้น" ของแบบ All : ผลต่างของทุกจุดในชิ้นเดียวกันต้องไม่เกินเกณฑ์
        /// เก็บ TOLERANCE ที่ใช้ตัดสินไว้ด้วย ไม่ให้ไปคำนวณใหม่ตอน export
        /// ถ้าเกณฑ์ใน master ถูกแก้ทีหลัง ใบเก่าจะยังตัดสินด้วยเกณฑ์เดิม
        /// </summary>
        private List<string> BuildDimensionPieceJudgeSql(QAdataProperty dataItem, DataTable dt)
        {
            var sqlList = new List<string>();

            // โหมดอื่นไม่มีเกณฑ์นี้ ตารางหน้าจอจึงไม่มีคอลัมน์พวกนี้
            if (!dt.Columns.Contains("PIECE_JUDGE") || !dt.Columns.Contains("DIFFERENCE"))
            {
                return sqlList;
            }

            string reportNo = ToSqlTextValue(dataItem.Report_No);
            string empId = ToSqlTextValue(dataItem.EMP_ID);
            var done = new HashSet<string>();

            foreach (DataRow row in dt.Rows)
            {
                string samplingNo = ToSqlTextValue(row["SAMPLING_NO"]);

                // ทุกจุดในชิ้นเดียวกันถือค่าเดียวกัน เก็บแถวเดียวพอ
                if (!done.Add(samplingNo))
                {
                    continue;
                }

                // ยังกรอกไม่ครบทุกจุด ยังไม่มีผลตัดสินให้บันทึก
                if (row["PIECE_JUDGE"] == null || row["PIECE_JUDGE"] == DBNull.Value)
                {
                    continue;
                }

                string difference = ToSqlNumberValue(row["DIFFERENCE"]);
                string tolerance = dt.Columns.Contains("TOLERANCE") ? ToSqlNumberValue(row["TOLERANCE"]) : "NULL";
                string judge = ToSqlNumberValue(row["PIECE_JUDGE"]);
                string cavityValue = dt.Columns.Contains("CAVITY_NAME") ? ToSqlValue(row["CAVITY_NAME"]) : "NULL";

                sqlList.Add($@"
            UPDATE `db_dimension_piece_judge`
            SET `INUSE` = 0
            WHERE `REPORT_NO` = {reportNo}
              AND `SAMPLING_NO` = {samplingNo};");

                sqlList.Add($@"
            INSERT INTO `db_dimension_piece_judge`
            (
                `REPORT_NO`,
                `SAMPLING_NO`,
                `COUNT`,
                `CAVITY_NAME`,
                `DIFFERENCE`,
                `TOLERANCE`,
                `JUDGE`,
                `EMP_ID`,
                `JUDGE_DATE`,
                `INUSE`
            )
            SELECT
                {reportNo},
                {samplingNo},
                COALESCE(MAX(`COUNT`), 0) + 1,
                {cavityValue},
                {difference},
                {tolerance},
                {judge},
                {empId},
                NOW(),
                1
            FROM `db_dimension_piece_judge`
            WHERE `REPORT_NO` = {reportNo}
              AND `SAMPLING_NO` = {samplingNo};");
            }

            return sqlList;
        }

        // ตัวเลขที่ต้องลง DB เป็นชนิดตัวเลข ไม่ใช่ข้อความ ค่าว่างให้เป็น NULL
        private static string ToSqlNumberValue(object value)
        {
            if (value == null || value == DBNull.Value || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return "NULL";
            }

            return decimal.TryParse(value.ToString(), out decimal number)
                ? number.ToString(System.Globalization.CultureInfo.InvariantCulture)
                : "NULL";
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
            sql = @"SELECT b.Receive_Date as `Receive Date` , b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
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
            sql = @"SELECT b.Receive_Date as `Receive Date` , b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , 
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
            sql = @"SELECT b.Receive_Date as `Receive Date` , b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , b.Lot_Size as `Lot Size` ,
                    a.Appearance_Check_Lot_No as `Lot No.`, b.Inspection_Qty as `Inspection Qty` , d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , a.dataItem.process as `process_status_id` , iStatus.STATUS_NAME as `Status` , b.Issue_Date
    
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
            sql = @"SELECT `APPEARANCE_ID`,
                           COALESCE(`APPEARANCE_DATE`, DATE(`UPDATETIME`)) AS APPEARANCE_DATE,
                           `BATCH`,
                           `COUNT`,
                           LOT_NO,
                           QTY_SELECT,
                           QTY_OK,
                           QTY_NG,
                           EMP_ID,
                           JUDGE
                    FROM `db_appearance_data`
                    WHERE REPORT_NO = 'dataItem.Report_No'
                      AND BATCH = 'dataItem.Batch'
                      AND INUSE = 1
                    ORDER BY `COUNT`, `APPEARANCE_ID`";

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
                        CASE
                            WHEN COALESCE(s.sampling_type, 0) = 1 THEN (p.`VALUE` * p.PACK_COUNT)
                            ELSE p.packing_size
                        END - COALESCE(SUM(a.qty_select), 0) as `REMAIN_PACKING_SIZE`,
                        p.`PACKING_SIZE` as `PACKING_SIZE`
                FROM 
                    db_packing_size p
                LEFT JOIN
                    db_receive_mat r
                    ON p.report_no = r.report_no
                LEFT JOIN
                    info_appearance_sampling s
                    ON r.M_Code = s.M_CODE
                LEFT JOIN 
                    db_appearance_data a 
                    ON p.report_no = a.report_no 
                    AND p.BATCH = a.BATCH AND a.inuse = 1

                WHERE p.REPORT_NO = 'dataItem.Report_No'

                GROUP BY 
                    p.report_no,
                    p.BATCH,
                    p.PACK_COUNT,
                    p.`VALUE`,
                    p.PACKING_SIZE,
                    s.sampling_type;
                    ";

            sql = sql.Replace("dataItem.Report_No", dataItem.Report_No);

            return sql;
        }
        public string InsertAppearData(QAdataProperty dataItem)
        {

            sql = @"INSERT INTO `db_appearance_data` (`REPORT_NO`, `BATCH`, `COUNT`, `LOT_NO`, `QTY_SELECT`, `QTY_OK`, `QTY_NG`, `EMP_ID`, `JUDGE`, `APPEARANCE_DATE`, `UPDATETIME`, `INUSE`) " +
            $"VALUES ({ToSqlTextValue(dataItem.Report_No)}, {ToSqlIntOrNull(dataItem.BATCH)}, {ToSqlIntOrNull(dataItem.COUNT)}, {ToSqlTextValue(dataItem.Lot_No)}, {ToSqlIntOrNull(dataItem.QTY_SELECT)}, {ToSqlIntOrNull(dataItem.QTY_OK)}, {ToSqlIntOrNull(dataItem.QTY_NG)}, {ToSqlTextValue(dataItem.EMP_ID)}, {ToSqlIntOrNull(dataItem.judge)}, CURDATE(), NOW(), 1)";

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
                      AND COALESCE(LOT_NO, '') = {ToSqlTextValue(dataItem.Lot_No)}
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
                int qtyNg = 0;
                if (!row.Table.Columns.Contains("QTY_NG") || !int.TryParse(row["QTY_NG"]?.ToString(), out qtyNg) || qtyNg <= 0)
                {
                    continue;
                }

                string ngDetail = row.Table.Columns.Contains("NG_DETAIL") ? row["NG_DETAIL"].ToString().Replace("'", "''") : "";
                string ngModeId = ToSqlIntOrNull(row.Table.Columns.Contains("NG_MODE_ID") ? row["NG_MODE_ID"] : null);
                string appearanceId = ToSqlLongOrNull(dataItem.APPEARANCE_ID);
                sql = $"INSERT INTO `db_appearance_pending`(`APPEARANCE_ID`, `REPORT_NO`, `BATCH`, `COUNT`, `LOT_NO`, `NG_COUNT`, `QTY_NG`, `NG_DETAIL`, `NG_MODE_ID`, `APPEARANCE_DATE`, `UPDATETIME`) " +
                      $"VALUES({appearanceId}, {ToSqlTextValue(dataItem.Report_No)}, {ToSqlIntOrNull(dataItem.BATCH)}, {ToSqlIntOrNull(dataItem.COUNT)}, {ToSqlTextValue(dataItem.Lot_No)}, {ngCount}, {qtyNg}, '{ngDetail}', {ngModeId}, NOW(), NOW())";

                sqlList.Add(sql);
                ngCount++;
            }
            return sqlList;

        }

        public List<string> UpdateAppearPendingReview(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            DataTable dt = (DataTable)dataItem.dtg_ngMode.DataSource;

            // APPEARANCE_ID ที่ถูกแตะในรอบนี้ ต้องคิดยอดใหม่กลับ db_appearance_data
            List<string> appearanceIds = new List<string>();

            foreach (DataRow row in dt.Rows)
            {
                string pendingId = ToSqlLongOrNull(row.Table.Columns.Contains("APPEARANCE_PENDING_ID") ? row["APPEARANCE_PENDING_ID"] : null);
                if (pendingId == "NULL")
                {
                    continue;
                }

                // ช่อง OK ว่าง = ยังไม่ตัดสิน (UI กันไว้อีกชั้นแล้ว แต่กันเผื่อไว้ตรงนี้ด้วย)
                string okText = row.Table.Columns.Contains("OK_QTY") ? row["OK_QTY"]?.ToString() : null;
                if (string.IsNullOrWhiteSpace(okText) || !int.TryParse(okText, out int okQty))
                {
                    continue;
                }

                int qtyNg = 0;
                if (row.Table.Columns.Contains("QTY_NG"))
                {
                    int.TryParse(row["QTY_NG"]?.ToString(), out qtyNg);
                }

                if (okQty < 0) okQty = 0;
                if (okQty > qtyNg) okQty = qtyNg;

                // RESULT ยังเป็น "สรุปผล" เหมือนเดิม เพื่อให้ query ที่ใช้ `RESULT IS NULL`
                // เป็นตัวชี้ว่ายังรอ admin ทำงานได้ต่อโดยไม่ต้องแก้
                int result = okQty >= qtyNg ? 1 : 0;

                sql = $@"UPDATE `db_appearance_pending`
                         SET `RESULT` = {result},
                             `REVIEW_OK_QTY` = {okQty},
                             `REVIEW_EMP_ID` = {ToSqlTextValue(dataItem.EMP_ID)},
                             `REVIEW_DATE` = NOW(),
                             `UPDATETIME` = NOW()
                         WHERE `APPEARANCE_PENDING_ID` = {pendingId}
                           AND `RESULT` IS NULL";
                sqlList.Add(sql);

                string appearanceId = ToSqlLongOrNull(row.Table.Columns.Contains("APPEARANCE_ID") ? row["APPEARANCE_ID"] : null);
                if (appearanceId != "NULL" && !appearanceIds.Contains(appearanceId))
                {
                    appearanceIds.Add(appearanceId);
                }
            }

            // ต้องอยู่หลัง UPDATE ข้างบน — UpdateBySqlList รันเรียงกันใน transaction เดียว
            // จึงเห็น REVIEW_OK_QTY ที่เพิ่งเขียน
            foreach (string appearanceId in appearanceIds)
            {
                sqlList.Add(RecalcAppearDataFromPending(appearanceId));
            }

            sqlList.Add(UpdateReportStatus(dataItem));

            return sqlList;
        }

        // เขียนผลการ review กลับ db_appearance_data — ชิ้นที่ admin ตัดสินว่า OK จริง
        // ต้องย้ายจากฝั่ง NG มาเป็น OK มิฉะนั้นยอดในใบตรวจจะค้างเป็น NG ตลอด
        //
        // คิดใหม่ทั้งก้อนจาก db_appearance_pending (ไม่ใช่บวกเพิ่มจากของเดิม) จึงรันซ้ำได้
        // ยอดไม่บวกทบ — อาศัย invariant ที่ยืนยันกับข้อมูลจริงแล้วว่า
        //   QTY_SELECT = QTY_OK + QTY_NG  และ  QTY_NG = SUM(pending.QTY_NG)
        private string RecalcAppearDataFromPending(string appearanceId)
        {
            return $@"UPDATE `db_appearance_data` a
                      JOIN (
                            SELECT `APPEARANCE_ID`,
                                   SUM(COALESCE(`QTY_NG`, 0)) - SUM(COALESCE(`REVIEW_OK_QTY`, 0)) AS REMAIN_NG
                            FROM `db_appearance_pending`
                            WHERE `APPEARANCE_ID` = {appearanceId}
                            GROUP BY `APPEARANCE_ID`
                           ) p ON a.`APPEARANCE_ID` = p.`APPEARANCE_ID`
                      SET a.`QTY_NG`     = p.REMAIN_NG,
                          a.`QTY_OK`     = a.`QTY_SELECT` - p.REMAIN_NG,
                          a.`JUDGE`      = CASE WHEN p.REMAIN_NG = 0 THEN 1 ELSE 0 END,
                          a.`UPDATETIME` = NOW()
                      WHERE a.`APPEARANCE_ID` = {appearanceId}
                        AND a.`INUSE` = 1";
        }
        public string GetTotalInspected (QAdataProperty dataItem)
        {
            sql = $"SELECT sum(QTY_SELECT) FROM `db_appearance_data` WHERE REPORT_NO = '{dataItem.Report_No}' and inuse = 1";

            return sql;
        }

        // นับรายการ NG ที่ยังรอ admin ตัดสิน (ใช้กันไม่ให้ปิดงานทั้งที่ยังมี pending ค้าง)
        public string CountAppearPendingUnreviewed(QAdataProperty dataItem)
        {
            sql = $@"SELECT COUNT(*) AS `CNT`
                     FROM `db_appearance_pending`
                     WHERE `REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                       AND `RESULT` IS NULL
                       AND COALESCE(`QTY_NG`, 0) > 0";

            return sql;
        }


        public string SearchForAppearPending()
        {
            // รวมเป็น 1 แถวต่อ 1 Report — อาการเดียวกันจากคนละแพ็ค/คนละครั้งบันทึกจะถูกยุบรวมกัน
            // รายละเอียดรายอาการไปดูที่หน้า Pending (SearchAppearPendingData ดึงทั้ง Report เมื่อไม่ส่ง APPEARANCE_ID)
            sql = @"SELECT b.Receive_Date as `Receive Date` , b.Regular_No as `Regular No`, a.Report_No as `Report No.` ,
                      COALESCE(GROUP_CONCAT(DISTINCT NULLIF(p.LOT_NO, '') ORDER BY p.LOT_NO SEPARATOR ', '), '') as `Lot No.`,
                      SUM(COALESCE(p.QTY_NG, 0)) as `Pending Qty`, COUNT(*) as `Pending Items`,
                      b.M_Code as `M-CODE` , b.Invoice_No as `Invoice No.` , b.Lot_Size as `Lot Size` , b.Inspection_Qty as `Inspection Qty` ,
                      d.VENDOR_NAME as `Vendor` , c.ITEM_EXTERNAL_SHORT_NAME as `Material Name` , 6 as `process_status_id` , 'Pending' as `Status` , b.Issue_Date

               FROM `db_report_status` a

               join db_receive_mat b on (a.Report_No = b.Report_No)
               join mes.item_manufacturing c on (b.M_Code = c.ITEM_CODE_FOR_SUPPORT_MES)
               join mes.vendor d on (c.VENDOR_ID = d.VENDOR_ID)
               join info_mat_inspection_list e on (b.M_Code = e.M_CODE)
               join db_appearance_pending p on (a.Report_No = p.REPORT_NO and p.RESULT IS NULL)
               where e.Appearance_Check_Need = 1
                 and COALESCE(p.QTY_NG, 0) > 0
               group by b.Receive_Date, b.Regular_No, a.Report_No, b.M_Code, b.Invoice_No,
                        b.Lot_Size, b.Inspection_Qty, d.VENDOR_NAME, c.ITEM_EXTERNAL_SHORT_NAME,
                        b.Issue_Date
               order by b.Receive_Date, a.Report_No
               ";

            return sql;
        }



        public string SearchAppearPendingData(QAdataProperty dataItem)
        {
            string appearanceId = ToSqlLongOrNull(dataItem.APPEARANCE_ID);
            sql = $@"SELECT
                        p.APPEARANCE_PENDING_ID,
                        p.APPEARANCE_ID,
                        p.REPORT_NO,
                        p.BATCH,
                        p.COUNT,
                        p.NG_COUNT,
                        COALESCE(p.LOT_NO, a.LOT_NO, '') AS LOT_NO,
                        p.QTY_NG,
                        COALESCE(n.NG_Mode, p.NG_DETAIL, '') AS NG_MODE,
                        p.NG_DETAIL AS NOTE,
                        '' AS JUDGEMENT,
                        COALESCE(CAST(p.RESULT AS CHAR), '') AS RESULT,
                        COALESCE(CAST(p.REVIEW_OK_QTY AS CHAR), '') AS REVIEW_OK_QTY,
                        a.QTY_SELECT,
                        a.QTY_OK,
                        a.QTY_NG AS TOTAL_QTY_NG,
                        a.EMP_ID,
                        a.APPEARANCE_DATE
                    FROM db_appearance_pending p
                    LEFT JOIN db_appearance_data a
                        ON p.APPEARANCE_ID = a.APPEARANCE_ID
                    LEFT JOIN info_ngmode n
                        ON p.NG_MODE_ID = n.ID
                    WHERE p.REPORT_NO = {ToSqlTextValue(dataItem.Report_No)}
                      AND p.RESULT IS NULL
                      AND COALESCE(p.QTY_NG, 0) > 0
                      AND ({appearanceId} IS NULL OR p.APPEARANCE_ID = {appearanceId})
                    ORDER BY p.BATCH, p.COUNT, p.NG_COUNT";

            return sql;
        }

        public List<string> DeleteReportHistory(QAdataProperty dataItem)
        {
            string reportNo = ToSqlTextValue(dataItem.Report_No);
            string regularNo = ToSqlTextValue(dataItem.Regular_No);

            return new List<string>
            {
                $"DELETE FROM `db_appearance_pending` WHERE `REPORT_NO` = {reportNo}",
                $"DELETE FROM `db_appearance_data` WHERE `REPORT_NO` = {reportNo}",
                $"DELETE FROM `db_inspection_data` WHERE `REPORT_NO` = {reportNo}",
                $"DELETE FROM `db_dimension_data` WHERE `REPORT_NO` = {reportNo}",
                $"DELETE FROM `db_function_check_result` WHERE `REPORT_NO` = {reportNo}",
                $"DELETE FROM `db_function_data` WHERE `REPORT_NO` = {reportNo}",
                $"DELETE FROM `db_regular_data` WHERE `REGULAR_NO` = {regularNo}",
                $"DELETE rd FROM `db_regular_data` rd JOIN `db_receive_mat` rm ON rd.`REGULAR_NO` = rm.`Regular_No` WHERE rm.`Report_No` = {reportNo}",
                $"DELETE FROM `db_packing_check` WHERE `REPORT_NO` = {reportNo}",
                $"DELETE FROM `db_packing_size` WHERE `REPORT_NO` = {reportNo}",
                $"DELETE FROM `db_report_lot_no` WHERE `REPORT_NO` = {reportNo}",
                $"DELETE FROM `info_report_active` WHERE `REPORT_NO` = {reportNo}",
                $"DELETE FROM `db_report_status` WHERE `Report_No` = {reportNo}",
                $"DELETE FROM `db_receive_mat` WHERE `Report_No` = {reportNo}"
            };
        }

        // ---------- ชุด query สำหรับ FM-QA-B08-F Receiving Inspection Check Sheet ----------

        // หัวเอกสาร + flag ว่าต้องตรวจอะไรบ้าง (ใช้ตัดสินว่าบล็อกไหนเทา บล็อกไหนขาว)
        public string B08Header(QAdataProperty dataItem)
        {
            return $@"SELECT r.`Report_No`, r.`M_Code`, r.`Material_Name`, r.`Receive_Date`, r.`Invoice_No`,
                             r.`Vendor_Name`, r.`Lot_Size`, r.`Inspection_Qty`, r.`Regular_No`,
                             s.`Emp_Receive_WH`, s.`Receive_WH_Date`,
                             COALESCE(e.`Keep_Data_Need`, 0)        AS `Keep_Data_Need`,
                             COALESCE(e.`Regular_Check_Need`, 0)    AS `Regular_Check_Need`,
                             COALESCE(e.`Function_Check_Need`, 0)   AS `Function_Check_Need`,
                             COALESCE(e.`Dimension_Check_Need`, 0)  AS `Dimension_Check_Need`,
                             COALESCE(e.`Appearance_Check_Need`, 0) AS `Appearance_Check_Need`,
                             ref.`Reference`
                      FROM `db_receive_mat` r
                      LEFT JOIN `db_report_status` s ON r.`Report_No` = s.`Report_No`
                      LEFT JOIN `info_mat_inspection_list` e ON r.`M_Code` = e.`M_CODE`
                      LEFT JOIN `info_reference` ref ON r.`M_Code` = ref.`M_Code`
                      WHERE r.`Report_No` = {ToSqlTextValue(dataItem.Report_No)}";
        }

        // บรรจุภัณฑ์ 3 บรรทัด : info_method ID 1-3 ตรงกับข้อความในฟอร์ม
        // LEFT JOIN เพื่อให้ได้ครบ 3 บรรทัดเสมอ ถึงยังไม่ได้ตรวจ
        public string B08PackingCheck(QAdataProperty dataItem)
        {
            return $@"SELECT m.`ID` AS `METHOD_ID`, m.`DETAIL_METHOD`,
                             p.`JUDGMENT`, p.`DETAIL_JUDGE`, p.`EMP_PACKING_CHECK`, p.`UpdateTime`
                      FROM `info_method` m
                      LEFT JOIN `db_packing_check` p
                             ON p.`METHOD_ID` = m.`ID`
                            AND p.`REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                            AND p.`COUNT` = (SELECT MAX(x.`COUNT`) FROM `db_packing_check` x
                                             WHERE x.`REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                                               AND x.`METHOD_ID` = m.`ID`)
                      WHERE m.`ID` IN (1, 2, 3)
                      ORDER BY m.`ID`";
        }

        // ขนาดบรรจุ : VALUE X PACK_COUNT
        public string B08PackingSize(QAdataProperty dataItem)
        {
            return $@"SELECT `BATCH`, `VALUE`, `PACK_COUNT`, `PACKING_SIZE`
                      FROM `db_packing_size`
                      WHERE `Report_No` = {ToSqlTextValue(dataItem.Report_No)}
                      ORDER BY `BATCH`";
        }

        public string B08LotNo(QAdataProperty dataItem)
        {
            return $@"SELECT `LOT_NO`
                      FROM `db_report_lot_no`
                      WHERE `REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                      ORDER BY `LOT_NO`";
        }

        // 1 แถวต่อ 1 รอบตรวจ เรียงตามเวลาที่บันทึกจริง
        public string B08AppearanceData(QAdataProperty dataItem)
        {
            return $@"SELECT a.`APPEARANCE_ID`, a.`BATCH`, a.`COUNT`, a.`LOT_NO`,
                             a.`QTY_SELECT`, a.`QTY_OK`, a.`QTY_NG`, a.`JUDGE`,
                             a.`EMP_ID`,
                             TRIM(CONCAT(COALESCE(emp.`Employee_FirstName`, ''), ' ', COALESCE(emp.`Employee_LastName`, ''))) AS `EMP_NAME`,
                             COALESCE(a.`APPEARANCE_DATE`, DATE(a.`UPDATETIME`)) AS `APPEARANCE_DATE`,
                             a.`UPDATETIME`
                      FROM `db_appearance_data` a
                      LEFT JOIN `info_employee` emp ON a.`EMP_ID` = emp.`Employee_ID`
                      WHERE a.`REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                        AND a.`INUSE` = 1
                      ORDER BY a.`APPEARANCE_ID`";
        }

        // ค่าที่ใช้ตัดสินว่า M-CODE นี้ต้องใช้ฟอร์ม B08 แบบไหน และโซนกลางต้องยืดแค่ไหน
        // dataItem.Qty = Lot Size ใช้เปิดตาราง info_strictness หาจำนวนตัวอย่างที่ต้องตรวจ
        public string B08Sampling(QAdataProperty dataItem)
        {
            string mCode = ToSqlTextValue(dataItem.M_CODE);
            string lotSize = ToSqlIntOrNull(dataItem.Qty);

            return $@"SELECT d.`Sampling_Type`      AS `DIM_SAMPLING_TYPE`,
                             d.`Cavity_Qty`         AS `DIM_CAVITY_QTY`,
                             d.`Sampling_Qty`       AS `DIM_SAMPLING_QTY`,
                             d.`Strictness_Type`    AS `DIM_STRICTNESS_TYPE`,
                             d.`Strictness_Level`   AS `DIM_STRICTNESS_LEVEL`,
                             f.`Sampling_Type`      AS `FUNC_SAMPLING_TYPE`,
                             f.`Cavity_Qty`         AS `FUNC_CAVITY_QTY`,
                             f.`Sampling_Qty`       AS `FUNC_SAMPLING_QTY`,
                             (SELECT s.`Sampling_Qty`
                                FROM `info_strictness` s
                               WHERE s.`Strictness_Type`  = d.`Strictness_Type`
                                 AND s.`Strictness_Level` = d.`Strictness_Level`
                                 AND COALESCE({lotSize}, 0) BETWEEN s.`Min_Qty` AND s.`Max_Qty`
                               LIMIT 1)                AS `STRICTNESS_SAMPLING_QTY`,
                             (SELECT COUNT(*)
                                FROM `info_dimension_equipment` de
                               WHERE de.`M_CODE` = {mCode}) AS `DIM_POINT_COUNT`
                      FROM `info_dimension_sampling` d
                      LEFT JOIN `info_function_sampling` f ON f.`M_Code` = d.`M_Code`
                      WHERE d.`M_Code` = {mCode}";
        }

        // รายการ NG ที่ค้าง/ตัดสินแล้ว ใช้เติมตาราง Pending detail ท้ายเอกสาร
        public string B08AppearancePending(QAdataProperty dataItem)
        {
            return $@"SELECT p.`APPEARANCE_PENDING_ID`, p.`APPEARANCE_ID`, p.`BATCH`, p.`COUNT`, p.`LOT_NO`,
                             p.`NG_DETAIL`, p.`NG_MODE_ID`, n.`NG_Mode`,
                             p.`QTY_NG`, p.`RESULT`, p.`REVIEW_OK_QTY`,
                             (COALESCE(p.`QTY_NG`, 0) - COALESCE(p.`REVIEW_OK_QTY`, 0)) AS `REMAIN_NG`
                      FROM `db_appearance_pending` p
                      LEFT JOIN `info_ngmode` n ON p.`NG_MODE_ID` = n.`ID`
                      WHERE p.`REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                        AND COALESCE(p.`QTY_NG`, 0) > 0
                      ORDER BY p.`APPEARANCE_PENDING_ID`";
        }

        // ---------- Function / Dimension สำหรับ FM-QA-B08-F ----------

        // จุดวัด Dimension ของ M-CODE (master) : ชื่อจุด เกณฑ์ และเครื่องมือที่ใช้
        public string B08DimensionPoints(QAdataProperty dataItem)
        {
            return $@"SELECT d.`POINT_ORDER`, d.`POINT_NAME`, d.`POINT_CAL`,
                             d.`CRITERIA_MIN`, d.`CRITERIA_MAX`, d.`UNIT`, d.`JUDGE_TYPE`,
                             d.`EQUIPMENT_TYPE`, t.`Equipment_Name`
                      FROM `info_dimension_equipment` d
                      LEFT JOIN `info_equipment_type` t ON d.`EQUIPMENT_TYPE` = t.`Equipment_Type`
                      WHERE d.`M_CODE` = {ToSqlTextValue(dataItem.M_CODE)}
                      ORDER BY d.`POINT_ORDER`";
        }

        // ค่าที่วัดได้จริง 1 แถวต่อ 1 จุด ต่อ 1 ตัวอย่าง
        public string B08DimensionData(QAdataProperty dataItem)
        {
            return $@"SELECT v.`SAMPLING_NO`, v.`POINT_ORDER`, v.`CAVITY_NAME`,
                             v.`VALUE`, v.`JUDGE`, v.`EQUIPMENT_SERIAL_ID`, s.`EQUIPMENT_SERIAL`
                      FROM `db_dimension_data` v
                      LEFT JOIN `info_equipment_serial` s ON v.`EQUIPMENT_SERIAL_ID` = s.`ID`
                      WHERE v.`REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                        AND v.`INUSE` = 1
                      ORDER BY v.`SAMPLING_NO`, v.`POINT_ORDER`";
        }

        // เครื่องมือวัดที่ใช้จริงในใบนี้ ใช้เติมแถว Equipment S/N หัวบล็อก Dimension
        public string B08DimensionEquipment(QAdataProperty dataItem)
        {
            return $@"SELECT DISTINCT t.`Equipment_Name`, s.`EQUIPMENT_SERIAL`
                      FROM `db_dimension_data` v
                      JOIN `info_equipment_serial` s ON v.`EQUIPMENT_SERIAL_ID` = s.`ID`
                      LEFT JOIN `info_equipment_type` t ON s.`EQUIPMENT_TYPE_ID` = t.`Equipment_Type`
                      WHERE v.`REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                        AND v.`INUSE` = 1
                      ORDER BY t.`Equipment_Name`, s.`EQUIPMENT_SERIAL`";
        }

        // ผลตัดสินรายชิ้นของแบบ All : ผลต่างกับเกณฑ์ที่ใช้ตอนบันทึก
        public string B08DimensionPieceJudge(QAdataProperty dataItem)
        {
            return $@"SELECT p.`SAMPLING_NO`, p.`CAVITY_NAME`, p.`DIFFERENCE`, p.`TOLERANCE`, p.`JUDGE`
                      FROM `db_dimension_piece_judge` p
                      WHERE p.`REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                        AND p.`INUSE` = 1
                      ORDER BY p.`SAMPLING_NO`";
        }

        // ผลตรวจ Function พร้อม Equipment ที่ใช้จริง (ถ้า M-CODE นี้กำหนดไว้)
        public string B08FunctionData(QAdataProperty dataItem)
        {
            return $@"SELECT v.`SAMPLING_NO`, v.`COUNT`, v.`CAVITY_NAME`, v.`LOT_NO`,
                             v.`JUDGE`, v.`REMARK`, v.`EMP_ID`, v.`EQUIPMENT_SERIAL_ID`,
                             s.`EQUIPMENT_SERIAL`,
                             COALESCE(t.`Equipment_Name`, ft.`Equipment_Name`) AS `Equipment_Name`,
                             COALESCE(
                                 NULLIF((SELECT GROUP_CONCAT(DISTINCT cr.`CHECK_DETAIL`
                                                             ORDER BY cr.`CHECK_ORDER` SEPARATOR '\n')
                                         FROM `db_function_check_result` cr
                                         WHERE cr.`REPORT_NO` = v.`REPORT_NO`
                                           AND cr.`COUNT` = v.`COUNT`
                                           AND cr.`SAMPLING_NO` = v.`SAMPLING_NO`
                                           AND cr.`INUSE` = 1), ''),
                                 NULLIF((SELECT GROUP_CONCAT(cm.`CHECK_DETAIL`
                                                             ORDER BY cm.`CHECK_ORDER` SEPARATOR '\n')
                                         FROM `info_function_check_method` cm
                                         WHERE cm.`M_CODE` = {ToSqlTextValue(dataItem.M_CODE)}
                                           AND cm.`INUSE` = 1), ''),
                                 f.`POINT_NAME`
                             ) AS `FUNCTION_METHOD`
                      FROM `db_function_data` v
                      LEFT JOIN `info_equipment_serial` s ON v.`EQUIPMENT_SERIAL_ID` = s.`ID`
                      LEFT JOIN `info_equipment_type` t ON s.`EQUIPMENT_TYPE_ID` = t.`Equipment_Type`
                      LEFT JOIN `info_function_equipment` f
                             ON f.`M_CODE` = {ToSqlTextValue(dataItem.M_CODE)}
                            AND f.`POINT_ORDER` = 1
                      LEFT JOIN `info_equipment_type` ft ON f.`EQUIPMENT_TYPE` = ft.`Equipment_Type`
                      WHERE v.`REPORT_NO` = {ToSqlTextValue(dataItem.Report_No)}
                        AND v.`INUSE` = 1
                      ORDER BY v.`SAMPLING_NO`";
        }
    }
}
