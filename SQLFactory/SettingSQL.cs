using RawMat.Property;
using System;

namespace RawMat.SQLFactory
{
    public class SettingSQL
    {
        private string sql;

        private string CleanSqlText(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? "" : value.Trim().Replace("'", "''");
        }

        private string ToBitValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "0";
            }

            value = value.Trim();

            if (value == "1" ||
                value.Equals("YES", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("Y", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("NEED", StringComparison.OrdinalIgnoreCase))
            {
                return "1";
            }

            return "0";
        }

        private string ToSqlTextOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "NULL";
            }

            return $"'{CleanSqlText(value)}'";
        }

        public string SearchInspectionSettingList(SettingProperty dataItem)
        {
            string mCodeSearch = dataItem == null ? "" : CleanSqlText(dataItem.Search_M_CODE);

            sql = @"
            SELECT 
                a.M_CODE AS `M Code`,
                CASE WHEN a.Keep_Data_Need = 1 THEN 'Yes' ELSE 'No' END AS `Keep Data`,
                CASE WHEN a.Packing_Check_Mode = 1 THEN 'Yes' ELSE 'NO' END AS `Packing Check`,
                CASE WHEN a.Regular_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Regular Check`,
                a.Regular_Check_Ref AS `Regular Ref`,
                CASE WHEN a.Function_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Function Check`,
                CASE WHEN a.Dimension_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Dimension Check`,
                CASE WHEN a.Appearance_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Appearance Check`,
                CASE WHEN a.INUSE = 1 THEN 'Active' ELSE 'Inactive' END AS `Status`
            FROM qa_system.info_mat_inspection_list a
            JOIN mes.item_manufacturing b 
                ON a.M_CODE = b.ITEM_CODE_FOR_SUPPORT_MES
            JOIN mes.vendor c 
                ON b.VENDOR_ID = c.VENDOR_ID
            WHERE 1=1 ";

            // ถ้ามีการพิมพ์คำค้นหา ให้เพิ่มเงื่อนไข LIKE เข้าไป
            if (!string.IsNullOrWhiteSpace(mCodeSearch))
            {
                sql += $" AND a.M_CODE LIKE '%{mCodeSearch}%' ";
            }

            return sql;
        }

        public string SearchInspectionSettingByMCode(SettingProperty dataItem)
        {
            sql = @"
            SELECT 
                a.M_CODE AS `M Code`,
                CASE WHEN a.Keep_Data_Need = 1 THEN 'Yes' ELSE 'No' END AS `Keep Data`,
                a.Packing_Check_Mode AS `Packing Check`,
                CASE WHEN a.Regular_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Regular Check`,
                a.Regular_Check_Ref AS `Regular Ref`,
                CASE WHEN a.Function_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Function Check`,
                CASE WHEN a.Dimension_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Dimension Check`,
                CASE WHEN a.Appearance_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Appearance Check`,
                CASE WHEN a.INUSE = 1 THEN 'Active' ELSE 'Inactive' END AS `Status`
            FROM qa_system.info_mat_inspection_list a
            JOIN mes.item_manufacturing b 
                ON a.M_CODE = b.ITEM_CODE_FOR_SUPPORT_MES
            JOIN mes.vendor c 
                ON b.VENDOR_ID = c.VENDOR_ID
            WHERE a.M_CODE = 'dataItem.M_CODE'  
           "; 

            sql = sql.Replace("dataItem.M_CODE", CleanSqlText(dataItem.M_CODE));

            return sql;
        }

        public string SearchMCodeInMES(SettingProperty dataItem)
        {
            sql = @"
                    SELECT 
                        b.ITEM_CODE_FOR_SUPPORT_MES AS M_CODE,
                        b.ITEM_EXTERNAL_SHORT_NAME AS Material_Name,
                        c.VENDOR_ID,
                        c.VENDOR_NAME
                    FROM mes.item_manufacturing b
                    JOIN mes.vendor c 
                        ON b.VENDOR_ID = c.VENDOR_ID
                    WHERE b.ITEM_CODE_FOR_SUPPORT_MES = 'dataItem.M_CODE';
                   ";

            sql = sql.Replace("dataItem.M_CODE", CleanSqlText(dataItem.M_CODE));

            return sql;
        }

        public string CountInspectionSettingByMCode(SettingProperty dataItem)
        {
            sql = @"
                    SELECT COUNT(*) AS CNT
                    FROM qa_system.info_mat_inspection_list
                    WHERE M_CODE = 'dataItem.M_CODE'
                      AND IFNULL(INUSE, 1) = 1;
                   ";

            sql = sql.Replace("dataItem.M_CODE", CleanSqlText(dataItem.M_CODE));

            return sql;
        }

        public string InsertInspectionSetting(SettingProperty dataItem)
        {
            sql = @"
                    INSERT INTO qa_system.info_mat_inspection_list
                    (
                        M_CODE,
                        Keep_Data_Need,
                        Regular_Check_Need,
                        Regular_Check_Ref,
                        Packing_Check_Mode,
                        Function_Check_Need,
                        Dimension_Check_Need,
                        Appearance_Check_Need,
                        INUSE
                    )
                    VALUES
                    (
                        'dataItem.M_CODE',
                        dataItem.Keep_Data_Need,
                        dataItem.Regular_Check_Need,
                        dataItem.Regular_Check_Ref,
                        dataItem.Packing_Check_Mode,
                        dataItem.Function_Check_Need,
                        dataItem.Dimension_Check_Need,
                        dataItem.Appearance_Check_Need,
                        1
                    );
                   ";

            sql = sql.Replace("dataItem.M_CODE", CleanSqlText(dataItem.M_CODE));
            sql = sql.Replace("dataItem.Keep_Data_Need", ToBitValue(dataItem.Keep_Data_Need));
            sql = sql.Replace("dataItem.Regular_Check_Need", ToBitValue(dataItem.Regular_Check_Need));
            sql = sql.Replace("dataItem.Regular_Check_Ref", ToSqlTextOrNull(dataItem.Regular_Check_Ref));
            sql = sql.Replace("dataItem.Packing_Check_Mode", ToSqlTextOrNull(dataItem.Packing_Check_Mode));
            sql = sql.Replace("dataItem.Function_Check_Need", ToBitValue(dataItem.Function_Check_Need));
            sql = sql.Replace("dataItem.Dimension_Check_Need", ToBitValue(dataItem.Dimension_Check_Need));
            sql = sql.Replace("dataItem.Appearance_Check_Need", ToBitValue(dataItem.Appearance_Check_Need));

            return sql;
        }

        public string UpdateInspectionSetting(SettingProperty dataItem)
        {
            sql = @"
                    UPDATE qa_system.info_mat_inspection_list
                    SET 
                        Keep_Data_Need = dataItem.Keep_Data_Need,
                        Regular_Check_Need = dataItem.Regular_Check_Need,
                        Regular_Check_Ref = dataItem.Regular_Check_Ref,
                        Packing_Check_Mode = dataItem.Packing_Check_Mode,
                        Function_Check_Need = dataItem.Function_Check_Need,
                        Dimension_Check_Need = dataItem.Dimension_Check_Need,
                        Appearance_Check_Need = dataItem.Appearance_Check_Need,
                        INUSE = 1
                    WHERE M_CODE = 'dataItem.M_CODE'
                      AND IFNULL(INUSE, 1) = 1;
                   ";

            sql = sql.Replace("dataItem.M_CODE", CleanSqlText(dataItem.M_CODE));
            sql = sql.Replace("dataItem.Keep_Data_Need", ToBitValue(dataItem.Keep_Data_Need));
            sql = sql.Replace("dataItem.Regular_Check_Need", ToBitValue(dataItem.Regular_Check_Need));
            sql = sql.Replace("dataItem.Regular_Check_Ref", ToSqlTextOrNull(dataItem.Regular_Check_Ref));
            sql = sql.Replace("dataItem.Packing_Check_Mode", ToSqlTextOrNull(dataItem.Packing_Check_Mode));
            sql = sql.Replace("dataItem.Function_Check_Need", ToBitValue(dataItem.Function_Check_Need));
            sql = sql.Replace("dataItem.Dimension_Check_Need", ToBitValue(dataItem.Dimension_Check_Need));
            sql = sql.Replace("dataItem.Appearance_Check_Need", ToBitValue(dataItem.Appearance_Check_Need));

            return sql;
        }
    }
}