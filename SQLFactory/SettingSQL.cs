using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Data;

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
                value.Equals("NEED", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("ACTIVE", StringComparison.OrdinalIgnoreCase) ||
                value.Equals("TRUE", StringComparison.OrdinalIgnoreCase))
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

        private string GetSqlValue(DataRow row, string columnName)
        {
            if (row == null || row.RowState == DataRowState.Deleted || !row.Table.Columns.Contains(columnName))
            {
                return "";
            }

            return row[columnName]?.ToString() ?? "";
        }

        private List<string> BuildSaveEquipmentSettingSql(string tableName, string mCode, DataTable equipmentRows)
        {
            var sqlList = new List<string>
            {
                $"DELETE FROM {tableName} WHERE M_CODE = '{CleanSqlText(mCode)}';"
            };

            if (equipmentRows == null)
            {
                return sqlList;
            }

            foreach (DataRow row in equipmentRows.Rows)
            {
                if (row.RowState == DataRowState.Deleted)
                {
                    continue;
                }

                string pointOrder = GetSqlValue(row, "POINT_ORDER");
                string equipmentType = GetSqlValue(row, "EQUIPMENT_TYPE");
                string pointName = GetSqlValue(row, "POINT_NAME");
                string pointCal = GetSqlValue(row, "POINT_CAL");
                string criteriaMin = GetSqlValue(row, "CRITERIA_MIN");
                string criteriaMax = GetSqlValue(row, "CRITERIA_MAX");

                if (string.IsNullOrWhiteSpace(pointOrder) &&
                    string.IsNullOrWhiteSpace(equipmentType) &&
                    string.IsNullOrWhiteSpace(pointName) &&
                    string.IsNullOrWhiteSpace(criteriaMin) &&
                    string.IsNullOrWhiteSpace(criteriaMax))
                {
                    continue;
                }

                sqlList.Add($@"
                    INSERT INTO {tableName}
                    (
                        M_CODE,
                        POINT_ORDER,
                        EQUIPMENT_TYPE,
                        POINT_NAME,
                        POINT_CAL,
                        CRITERIA_MIN,
                        CRITERIA_MAX
                    )
                    VALUES
                    (
                        '{CleanSqlText(mCode)}',
                        {ToSqlTextOrNull(pointOrder)},
                        {ToSqlTextOrNull(equipmentType)},
                        {ToSqlTextOrNull(pointName)},
                        {ToSqlTextOrNull(string.IsNullOrWhiteSpace(pointCal) ? "0" : pointCal)},
                        {ToSqlTextOrNull(criteriaMin)},
                        {ToSqlTextOrNull(criteriaMax)}
                    );");
            }

            return sqlList;
        }

        public string SearchInspectionSettingList(SettingProperty dataItem)
        {
            string mCodeSearch = dataItem == null ? "" : CleanSqlText(dataItem.Search_M_CODE);
            string statusSearch = dataItem == null ? "" : CleanSqlText(dataItem.Search_Status);
            sql = @"
                SELECT 
                    a.M_CODE AS `M Code`,
                    CASE WHEN a.Keep_Data_Need = 1 THEN 'Check' ELSE 'No' END AS `Data Result`,
                    CASE WHEN a.Packing_Check_Mode = 1 THEN 'Check' ELSE 'No' END AS `Packing Check`,
                    CASE WHEN a.Regular_Check_Need = 1 THEN 'Check' ELSE 'No' END AS `Regular Check`,
                
                    CASE WHEN a.Function_Check_Need = 1 THEN 'Check' ELSE 'No' END AS `Function Check`,
                    CASE WHEN a.Dimension_Check_Need = 1 THEN 'Check' ELSE 'No' END AS `Dimension Check`,
                    CASE WHEN a.Appearance_Check_Need = 1 THEN 'Check' ELSE 'No' END AS `Appearance Check`,
                    CASE WHEN IFNULL(a.INUSE, 1) = 1 THEN 'Active' ELSE 'InActive' END AS `Status`
                FROM info_mat_inspection_list a
                WHERE 1=1 ";

            // ถ้ามีการพิมพ์คำค้นหา ให้เพิ่มเงื่อนไข LIKE เข้าไป
            if (!string.IsNullOrWhiteSpace(mCodeSearch))
            {
                sql += $" AND a.M_CODE LIKE '%{mCodeSearch}%' ";
            }
            if (!string.IsNullOrWhiteSpace(statusSearch))
            {
                sql += $" AND IFNULL(a.INUSE, 1) = '{statusSearch}' ";
            }
            return sql;
        }

        public string SearchInspectionSettingByMCode(SettingProperty dataItem)
        {
            sql = @"
                SELECT 
                    a.M_CODE AS `M Code`,
                    CASE WHEN a.Keep_Data_Need = 1 THEN 'Yes' ELSE 'No' END AS `Data Result`,
                    CASE WHEN a.Packing_Check_Mode = 1 THEN 'Yes' ELSE 'No' END AS `Packing Check`,
                    CASE WHEN a.Regular_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Regular Check`,
                    a.Regular_Check_Ref AS `Regular Ref`,
                    CASE WHEN a.Function_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Function Check`,
                    CASE WHEN a.Dimension_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Dimension Check`,
                    CASE WHEN a.Appearance_Check_Need = 1 THEN 'Yes' ELSE 'No' END AS `Appearance Check`,
                    IFNULL(a.INUSE, 1) AS INUSE,

                    -- เรียงลำดับคอลัมน์ให้ตรงเป๊ะตาม Database
                    -- 1. Regular
                    r.Cavity_Qty AS Reg_Cavity_Qty, r.Sampling_Type AS Reg_Sampling_Type, r.Sampling_Qty AS Reg_Sampling_Qty, 
                    r.Strictness_Type AS Reg_Strictness_Type, r.Strictness_Level AS Reg_Strictness_Level, r.Cavity_Name AS Reg_Cavity_Name,

                    -- 2. Function
                    f.Cavity_Qty AS Func_Cavity_Qty, f.Sampling_Type AS Func_Sampling_Type, f.Sampling_Qty AS Func_Sampling_Qty, 
                    f.Strictness_Type AS Func_Strictness_Type, f.Strictness_Level AS Func_Strictness_Level, f.Cavity_Name AS Func_Cavity_Name,

                    -- 3. Dimension
                    d.Cavity_Qty AS Dim_Cavity_Qty, d.Sampling_Type AS Dim_Sampling_Type, d.Sampling_Qty AS Dim_Sampling_Qty, 
                    d.Strictness_Type AS Dim_Strictness_Type, d.Strictness_Level AS Dim_Strictness_Level, d.Cavity_Name AS Dim_Cavity_Name,

                    -- 4. Appearance
                    ap.Cavity_Qty AS App_Cavity_Qty, ap.Sampling_Type AS App_Sampling_Type, ap.Sampling_Qty AS App_Sampling_Qty, 
                    ap.Strictness_Type AS App_Strictness_Type, ap.Strictness_Level AS App_Strictness_Level, ap.Cavity_Name AS App_Cavity_Name

                FROM info_mat_inspection_list a
                LEFT JOIN info_regular_sampling r ON a.M_CODE = r.M_Code
                LEFT JOIN info_function_sampling f ON a.M_CODE = f.M_Code
                LEFT JOIN info_dimension_sampling d ON a.M_CODE = d.M_Code
                LEFT JOIN info_appearance_sampling ap ON a.M_CODE = ap.M_Code
                WHERE a.M_CODE = 'dataItem.M_CODE' ";

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
                FROM item_manufacturing b
                JOIN vendor c 
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
                FROM info_mat_inspection_list
                WHERE M_CODE = 'dataItem.M_CODE';
            ";

            sql = sql.Replace("dataItem.M_CODE", CleanSqlText(dataItem.M_CODE));

            return sql;
        }

        public string InsertInspectionSetting(SettingProperty dataItem)
        {
            sql = @"
                INSERT INTO info_mat_inspection_list
                (
                    M_CODE,
                    Keep_Data_Need,
                    Regular_Check_Need,
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
                    dataItem.Packing_Check_Mode,
                    dataItem.Function_Check_Need,
                    dataItem.Dimension_Check_Need,
                    dataItem.Appearance_Check_Need,
                    dataItem.INUSE
                );

                -- เรียงลำดับคอลัมน์ตอนบันทึกให้ตรงเป๊ะ
                INSERT INTO info_regular_sampling (M_Code, Cavity_Qty, Sampling_Type, Sampling_Qty, Strictness_Type, Strictness_Level, Cavity_Name)
                VALUES ('dataItem.M_CODE', dataItem.Reg_Cavity_Qty, dataItem.Reg_Sampling_Type, dataItem.Reg_Sampling_Qty, dataItem.Reg_Strictness_Type, dataItem.Reg_Strictness_Level, dataItem.Reg_Cavity_Name)
                ON DUPLICATE KEY UPDATE Cavity_Qty=VALUES(Cavity_Qty), Sampling_Type=VALUES(Sampling_Type), Sampling_Qty=VALUES(Sampling_Qty), Strictness_Type=VALUES(Strictness_Type), Strictness_Level=VALUES(Strictness_Level), Cavity_Name=VALUES(Cavity_Name);

                DELETE FROM info_function_sampling
                WHERE M_Code = 'dataItem.M_CODE'
                  AND dataItem.Function_Check_Need = 0;

                INSERT INTO info_function_sampling (M_Code, Cavity_Qty, Sampling_Type, Sampling_Qty, Strictness_Type, Strictness_Level, Cavity_Name)
                SELECT 'dataItem.M_CODE', dataItem.Func_Cavity_Qty, dataItem.Func_Sampling_Type, dataItem.Func_Sampling_Qty, dataItem.Func_Strictness_Type, dataItem.Func_Strictness_Level, dataItem.Func_Cavity_Name
                FROM DUAL
                WHERE dataItem.Function_Check_Need = 1
                ON DUPLICATE KEY UPDATE Cavity_Qty=VALUES(Cavity_Qty), Sampling_Type=VALUES(Sampling_Type), Sampling_Qty=VALUES(Sampling_Qty), Strictness_Type=VALUES(Strictness_Type), Strictness_Level=VALUES(Strictness_Level), Cavity_Name=VALUES(Cavity_Name);

                INSERT INTO info_dimension_sampling (M_Code, Cavity_Qty, Sampling_Type, Sampling_Qty, Strictness_Type, Strictness_Level, Cavity_Name)
                VALUES ('dataItem.M_CODE', dataItem.Dim_Cavity_Qty, dataItem.Dim_Sampling_Type, dataItem.Dim_Sampling_Qty, dataItem.Dim_Strictness_Type, dataItem.Dim_Strictness_Level, dataItem.Dim_Cavity_Name)
                ON DUPLICATE KEY UPDATE Cavity_Qty=VALUES(Cavity_Qty), Sampling_Type=VALUES(Sampling_Type), Sampling_Qty=VALUES(Sampling_Qty), Strictness_Type=VALUES(Strictness_Type), Strictness_Level=VALUES(Strictness_Level), Cavity_Name=VALUES(Cavity_Name);

                INSERT INTO info_appearance_sampling (M_Code, Cavity_Qty, Sampling_Type, Sampling_Qty, Strictness_Type, Strictness_Level, Cavity_Name)
                VALUES ('dataItem.M_CODE', dataItem.App_Cavity_Qty, dataItem.App_Sampling_Type, dataItem.App_Sampling_Qty, dataItem.App_Strictness_Type, dataItem.App_Strictness_Level, dataItem.App_Cavity_Name)
                ON DUPLICATE KEY UPDATE Cavity_Qty=VALUES(Cavity_Qty), Sampling_Type=VALUES(Sampling_Type), Sampling_Qty=VALUES(Sampling_Qty), Strictness_Type=VALUES(Strictness_Type), Strictness_Level=VALUES(Strictness_Level), Cavity_Name=VALUES(Cavity_Name);
            ";

            // Helper: ใส่ 0 แทนถ้าค่าว่าง (สำหรับ DB smallint)
            Func<string, string> toZero = v => string.IsNullOrWhiteSpace(v) ? "0" : v;

            // --- Master Logic ---
            sql = sql.Replace("dataItem.M_CODE", CleanSqlText(dataItem.M_CODE));
            sql = sql.Replace("dataItem.Keep_Data_Need", ToBitValue(dataItem.Keep_Data_Need));
            sql = sql.Replace("dataItem.Regular_Check_Need", ToBitValue(dataItem.Regular_Check_Need));
            sql = sql.Replace("dataItem.Packing_Check_Mode", ToBitValue(dataItem.Packing_Check_Mode));
            sql = sql.Replace("dataItem.Function_Check_Need", ToBitValue(dataItem.Function_Check_Need));
            sql = sql.Replace("dataItem.Dimension_Check_Need", ToBitValue(dataItem.Dimension_Check_Need));
            sql = sql.Replace("dataItem.Appearance_Check_Need", ToBitValue(dataItem.Appearance_Check_Need));
            sql = sql.Replace("dataItem.INUSE", ToBitValue(dataItem.INUSE));

            // --- Tab 1: Regular ---
            sql = sql.Replace("dataItem.Reg_Cavity_Qty", toZero(dataItem.Reg_Cavity_Qty));
            sql = sql.Replace("dataItem.Reg_Sampling_Type", toZero(dataItem.Reg_Sampling_Type));
            sql = sql.Replace("dataItem.Reg_Sampling_Qty", toZero(dataItem.Reg_Sampling_Qty));
            sql = sql.Replace("dataItem.Reg_Strictness_Type", toZero(dataItem.Reg_Strictness_Type));
            sql = sql.Replace("dataItem.Reg_Strictness_Level", toZero(dataItem.Reg_Strictness_Level));
            sql = sql.Replace("dataItem.Reg_Cavity_Name", ToSqlTextOrNull(dataItem.Reg_Cavity_Name));

            // --- Tab 2: Function ---
            sql = sql.Replace("dataItem.Func_Cavity_Qty", toZero(dataItem.Func_Cavity_Qty));
            sql = sql.Replace("dataItem.Func_Sampling_Type", toZero(dataItem.Func_Sampling_Type));
            sql = sql.Replace("dataItem.Func_Sampling_Qty", toZero(dataItem.Func_Sampling_Qty));
            sql = sql.Replace("dataItem.Func_Strictness_Type", toZero(dataItem.Func_Strictness_Type));
            sql = sql.Replace("dataItem.Func_Strictness_Level", toZero(dataItem.Func_Strictness_Level));
            sql = sql.Replace("dataItem.Func_Cavity_Name", ToSqlTextOrNull(dataItem.Func_Cavity_Name));

            // --- Tab 3: Dimension ---
            sql = sql.Replace("dataItem.Dim_Cavity_Qty", toZero(dataItem.Dim_Cavity_Qty));
            sql = sql.Replace("dataItem.Dim_Sampling_Type", toZero(dataItem.Dim_Sampling_Type));
            sql = sql.Replace("dataItem.Dim_Sampling_Qty", toZero(dataItem.Dim_Sampling_Qty));
            sql = sql.Replace("dataItem.Dim_Strictness_Type", toZero(dataItem.Dim_Strictness_Type));
            sql = sql.Replace("dataItem.Dim_Strictness_Level", toZero(dataItem.Dim_Strictness_Level));
            sql = sql.Replace("dataItem.Dim_Cavity_Name", ToSqlTextOrNull(dataItem.Dim_Cavity_Name));

            // --- Tab 4: Appearance ---
            sql = sql.Replace("dataItem.App_Cavity_Qty", toZero(dataItem.App_Cavity_Qty));
            sql = sql.Replace("dataItem.App_Sampling_Type", toZero(dataItem.App_Sampling_Type));
            sql = sql.Replace("dataItem.App_Sampling_Qty", toZero(dataItem.App_Sampling_Qty));
            sql = sql.Replace("dataItem.App_Strictness_Type", toZero(dataItem.App_Strictness_Type));
            sql = sql.Replace("dataItem.App_Strictness_Level", toZero(dataItem.App_Strictness_Level));
            sql = sql.Replace("dataItem.App_Cavity_Name", ToSqlTextOrNull(dataItem.App_Cavity_Name));

            return sql;
        }

        public string UpdateInspectionSetting(SettingProperty dataItem)
        {
            sql = @"
                UPDATE info_mat_inspection_list
                SET Keep_Data_Need = dataItem.Keep_Data_Need, 
                    Regular_Check_Need = dataItem.Regular_Check_Need, 
                    Packing_Check_Mode = dataItem.Packing_Check_Mode, 
                    Regular_Check_Ref = dataItem.Regular_Check_Ref,
                    Function_Check_Need = dataItem.Function_Check_Need, 
                    Dimension_Check_Need = dataItem.Dimension_Check_Need, 
                    Appearance_Check_Need = dataItem.Appearance_Check_Need, 
                    INUSE = dataItem.INUSE
                WHERE M_CODE = 'dataItem.M_CODE';

                -- เรียงลำดับคอลัมน์ตอนบันทึกให้ตรงเป๊ะ
                INSERT INTO info_regular_sampling (M_Code, Cavity_Qty, Sampling_Type, Sampling_Qty, Strictness_Type, Strictness_Level, Cavity_Name)
                VALUES ('dataItem.M_CODE', dataItem.Reg_Cavity_Qty, dataItem.Reg_Sampling_Type, dataItem.Reg_Sampling_Qty, dataItem.Reg_Strictness_Type, dataItem.Reg_Strictness_Level, dataItem.Reg_Cavity_Name)
                ON DUPLICATE KEY UPDATE Cavity_Qty=VALUES(Cavity_Qty), Sampling_Type=VALUES(Sampling_Type), Sampling_Qty=VALUES(Sampling_Qty), Strictness_Type=VALUES(Strictness_Type), Strictness_Level=VALUES(Strictness_Level), Cavity_Name=VALUES(Cavity_Name);

                DELETE FROM info_function_sampling
                WHERE M_Code = 'dataItem.M_CODE'
                  AND dataItem.Function_Check_Need = 0;

                INSERT INTO info_function_sampling (M_Code, Cavity_Qty, Sampling_Type, Sampling_Qty, Strictness_Type, Strictness_Level, Cavity_Name)
                SELECT 'dataItem.M_CODE', dataItem.Func_Cavity_Qty, dataItem.Func_Sampling_Type, dataItem.Func_Sampling_Qty, dataItem.Func_Strictness_Type, dataItem.Func_Strictness_Level, dataItem.Func_Cavity_Name
                FROM DUAL
                WHERE dataItem.Function_Check_Need = 1
                ON DUPLICATE KEY UPDATE Cavity_Qty=VALUES(Cavity_Qty), Sampling_Type=VALUES(Sampling_Type), Sampling_Qty=VALUES(Sampling_Qty), Strictness_Type=VALUES(Strictness_Type), Strictness_Level=VALUES(Strictness_Level), Cavity_Name=VALUES(Cavity_Name);

                INSERT INTO info_dimension_sampling (M_Code, Cavity_Qty, Sampling_Type, Sampling_Qty, Strictness_Type, Strictness_Level, Cavity_Name)
                VALUES ('dataItem.M_CODE', dataItem.Dim_Cavity_Qty, dataItem.Dim_Sampling_Type, dataItem.Dim_Sampling_Qty, dataItem.Dim_Strictness_Type, dataItem.Dim_Strictness_Level, dataItem.Dim_Cavity_Name)
                ON DUPLICATE KEY UPDATE Cavity_Qty=VALUES(Cavity_Qty), Sampling_Type=VALUES(Sampling_Type), Sampling_Qty=VALUES(Sampling_Qty), Strictness_Type=VALUES(Strictness_Type), Strictness_Level=VALUES(Strictness_Level), Cavity_Name=VALUES(Cavity_Name);

                INSERT INTO info_appearance_sampling (M_Code, Cavity_Qty, Sampling_Type, Sampling_Qty, Strictness_Type, Strictness_Level, Cavity_Name)
                VALUES ('dataItem.M_CODE', dataItem.App_Cavity_Qty, dataItem.App_Sampling_Type, dataItem.App_Sampling_Qty, dataItem.App_Strictness_Type, dataItem.App_Strictness_Level, dataItem.App_Cavity_Name)
                ON DUPLICATE KEY UPDATE Cavity_Qty=VALUES(Cavity_Qty), Sampling_Type=VALUES(Sampling_Type), Sampling_Qty=VALUES(Sampling_Qty), Strictness_Type=VALUES(Strictness_Type), Strictness_Level=VALUES(Strictness_Level), Cavity_Name=VALUES(Cavity_Name);
            ";

            // Helper: ใส่ 0 แทนถ้าค่าว่าง (สำหรับ DB smallint)
            Func<string, string> toZero = v => string.IsNullOrWhiteSpace(v) ? "0" : v;

            // --- Master Logic ---
            sql = sql.Replace("dataItem.M_CODE", CleanSqlText(dataItem.M_CODE));
            sql = sql.Replace("dataItem.Keep_Data_Need", ToBitValue(dataItem.Keep_Data_Need));
            sql = sql.Replace("dataItem.Regular_Check_Need", ToBitValue(dataItem.Regular_Check_Need));
            sql = sql.Replace("dataItem.Packing_Check_Mode", ToBitValue(dataItem.Packing_Check_Mode));
            sql = sql.Replace("dataItem.Regular_Check_Ref", ToSqlTextOrNull(dataItem.Regular_Check_Ref));
            sql = sql.Replace("dataItem.Function_Check_Need", ToBitValue(dataItem.Function_Check_Need));
            sql = sql.Replace("dataItem.Dimension_Check_Need", ToBitValue(dataItem.Dimension_Check_Need));
            sql = sql.Replace("dataItem.Appearance_Check_Need", ToBitValue(dataItem.Appearance_Check_Need));
            sql = sql.Replace("dataItem.INUSE", ToBitValue(dataItem.INUSE));

            // --- Tab 1: Regular ---
            sql = sql.Replace("dataItem.Reg_Cavity_Qty", toZero(dataItem.Reg_Cavity_Qty));
            sql = sql.Replace("dataItem.Reg_Sampling_Type", toZero(dataItem.Reg_Sampling_Type));
            sql = sql.Replace("dataItem.Reg_Sampling_Qty", toZero(dataItem.Reg_Sampling_Qty));
            sql = sql.Replace("dataItem.Reg_Strictness_Type", toZero(dataItem.Reg_Strictness_Type));
            sql = sql.Replace("dataItem.Reg_Strictness_Level", toZero(dataItem.Reg_Strictness_Level));
            sql = sql.Replace("dataItem.Reg_Cavity_Name", ToSqlTextOrNull(dataItem.Reg_Cavity_Name));

            // --- Tab 2: Function ---
            sql = sql.Replace("dataItem.Func_Cavity_Qty", toZero(dataItem.Func_Cavity_Qty));
            sql = sql.Replace("dataItem.Func_Sampling_Type", toZero(dataItem.Func_Sampling_Type));
            sql = sql.Replace("dataItem.Func_Sampling_Qty", toZero(dataItem.Func_Sampling_Qty));
            sql = sql.Replace("dataItem.Func_Strictness_Type", toZero(dataItem.Func_Strictness_Type));
            sql = sql.Replace("dataItem.Func_Strictness_Level", toZero(dataItem.Func_Strictness_Level));
            sql = sql.Replace("dataItem.Func_Cavity_Name", ToSqlTextOrNull(dataItem.Func_Cavity_Name));

            // --- Tab 3: Dimension ---
            sql = sql.Replace("dataItem.Dim_Cavity_Qty", toZero(dataItem.Dim_Cavity_Qty));
            sql = sql.Replace("dataItem.Dim_Sampling_Type", toZero(dataItem.Dim_Sampling_Type));
            sql = sql.Replace("dataItem.Dim_Sampling_Qty", toZero(dataItem.Dim_Sampling_Qty));
            sql = sql.Replace("dataItem.Dim_Strictness_Type", toZero(dataItem.Dim_Strictness_Type));
            sql = sql.Replace("dataItem.Dim_Strictness_Level", toZero(dataItem.Dim_Strictness_Level));
            sql = sql.Replace("dataItem.Dim_Cavity_Name", ToSqlTextOrNull(dataItem.Dim_Cavity_Name));

            // --- Tab 4: Appearance ---
            sql = sql.Replace("dataItem.App_Cavity_Qty", toZero(dataItem.App_Cavity_Qty));
            sql = sql.Replace("dataItem.App_Sampling_Type", toZero(dataItem.App_Sampling_Type));
            sql = sql.Replace("dataItem.App_Sampling_Qty", toZero(dataItem.App_Sampling_Qty));
            sql = sql.Replace("dataItem.App_Strictness_Type", toZero(dataItem.App_Strictness_Type));
            sql = sql.Replace("dataItem.App_Strictness_Level", toZero(dataItem.App_Strictness_Level));
            sql = sql.Replace("dataItem.App_Cavity_Name", ToSqlTextOrNull(dataItem.App_Cavity_Name));

            return sql;
        }

        public string GetSamplingTypeList()
        {
            sql = @"
                SELECT 
                    sampling_type AS VALUE,
                    sampling_type_name AS TEXT 
                FROM info_sampling_type 
                ORDER BY sampling_type ASC;
            ";

            return sql;
        }

        public string GetStrictnessTypeList()
        {
            sql = @"
                SELECT 
                    Strictness_Name AS TEXT,
                    Strictness_Type AS VALUE
                FROM info_strictness_type
                ORDER BY Strictness_Type ASC;
            ";

            return sql;
        }

        public string GetStrictnessLevelList()
        {
            sql = @"
                SELECT 
                    Strictness_Level AS VALUE, 
                    Strictness_Level_Name AS TEXT 
                FROM info_strictness_level 
                ORDER BY Strictness_Level ASC;
            ";

            return sql;
        }

        public string GetNgModeList()
        {
            sql = @"
                SELECT
                    ID AS VALUE,
                    NG_Mode AS TEXT
                FROM info_ngmode
                WHERE IFNULL(IsActive, 1) = 1
                ORDER BY NG_Mode ASC;
            ";

            return sql;
        }

        public string SearchNgModeSettingList(SettingProperty dataItem)
        {
            string ngModeSearch = dataItem == null ? "" : CleanSqlText(dataItem.Search_NG_Mode);

            sql = @"
                SELECT
                    ID AS `ID`,
                    NG_Mode AS `NG Mode`,
                    CASE WHEN IFNULL(IsActive, 1) = 1 THEN 'Active' ELSE 'InActive' END AS `Status`,
                    Create_Date AS `Create Date`
                FROM info_ngmode
                WHERE 1=1 
                AND IsActive = 1";

            if (!string.IsNullOrWhiteSpace(ngModeSearch))
            {
                sql += $" AND NG_Mode LIKE '%{ngModeSearch}%' ";
            }

            sql += " ORDER BY IFNULL(IsActive, 1) DESC, NG_Mode ASC;";
            return sql;
        }

        public string CountNgModeSettingByName(SettingProperty dataItem)
        {
            sql = @"
                SELECT COUNT(*) AS CNT
                FROM info_ngmode
                WHERE NG_Mode = dataItem.NG_Mode;";

            sql = sql.Replace("dataItem.NG_Mode", ToSqlTextOrNull(dataItem.NG_Mode));
            return sql;
        }

        public string InsertNgModeSetting(SettingProperty dataItem)
        {
            sql = @"
                INSERT INTO info_ngmode
                (
                    NG_Mode,
                    IsActive,
                    Create_Date
                )
                VALUES
                (
                    dataItem.NG_Mode,
                    1,
                    NOW()
                );";

            sql = sql.Replace("dataItem.NG_Mode", ToSqlTextOrNull(dataItem.NG_Mode));
            return sql;
        }

        public string UpdateNgModeSetting(SettingProperty dataItem)
        {
            if (!string.IsNullOrWhiteSpace(dataItem.NG_Mode_ID))
            {
                sql = @"
                    UPDATE info_ngmode
                    SET NG_Mode = dataItem.NG_Mode,
                        IsActive = 1
                    WHERE ID = dataItem.NG_Mode_ID;";

                sql = sql.Replace("dataItem.NG_Mode_ID", ToSqlSmallIntOrNull(dataItem.NG_Mode_ID));
            }
            else
            {
                sql = @"
                    UPDATE info_ngmode
                    SET IsActive = 1
                    WHERE NG_Mode = dataItem.NG_Mode;";
            }

            sql = sql.Replace("dataItem.NG_Mode", ToSqlTextOrNull(dataItem.NG_Mode));
            return sql;
        }

        public string DeleteNgModeSetting(SettingProperty dataItem)
        {
            sql = @"
                UPDATE info_ngmode
                SET IsActive = 0
                WHERE ID = dataItem.NG_Mode_ID;";

            sql = sql.Replace("dataItem.NG_Mode_ID", ToSqlSmallIntOrNull(dataItem.NG_Mode_ID));
            return sql;
        }

        //-------------------------------------- Equipment
        public string SearchRegularEquipmentSetting(SettingProperty dataItem)
        {
            sql = @"
            SELECT 
                a.M_CODE,
                a.POINT_ORDER,
                a.EQUIPMENT_TYPE,
                b.Equipment_Name,
                a.POINT_NAME,
                a.POINT_CAL,
                a.CRITERIA_MIN,
                a.CRITERIA_MAX
            FROM info_regular_equipment a
            LEFT JOIN info_equipment_type b 
                ON a.EQUIPMENT_TYPE = b.Equipment_Type
            WHERE a.M_CODE = 'dataItem.M_CODE'
            ORDER BY a.POINT_ORDER ASC;
          ";

            sql = sql.Replace("dataItem.M_CODE", CleanSqlText(dataItem.M_CODE));

            return sql;
        }

        public string SearchDimensionEquipmentSetting(SettingProperty dataItem)
        {
            sql = @"
            SELECT 
                a.M_CODE,
                a.POINT_ORDER,
                a.EQUIPMENT_TYPE,
                b.Equipment_Name,
                a.POINT_NAME,
                a.POINT_CAL,
                a.CRITERIA_MIN,
                a.CRITERIA_MAX
            FROM info_dimension_equipment a
            LEFT JOIN info_equipment_type b 
                ON a.EQUIPMENT_TYPE = b.Equipment_Type
            WHERE a.M_CODE = 'dataItem.M_CODE'
            ORDER BY a.POINT_ORDER ASC;
          ";

            sql = sql.Replace("dataItem.M_CODE", CleanSqlText(dataItem.M_CODE));

            return sql;
        }

        public string GetEquipmentTypeList()
        {
            sql = @"
            SELECT 
                Equipment_Type,
                Equipment_Name
            FROM info_equipment_type
            ORDER BY Equipment_Type ASC;
          ";

            return sql;
        }

        public List<string> SaveRegularEquipmentSetting(SettingProperty dataItem)
        {
            return BuildSaveEquipmentSettingSql("info_regular_equipment", dataItem.M_CODE, dataItem.RegularEquipment);
        }

        public List<string> SaveDimensionEquipmentSetting(SettingProperty dataItem)
        {
            return BuildSaveEquipmentSettingSql("info_dimension_equipment", dataItem.M_CODE, dataItem.DimensionEquipment);
        }
        //--------------- Employee Setting ---------------------------
        public string SearchEmployeeSettingList(SettingProperty dataItem)
        {
            string employeeSearch = dataItem == null ? "" : CleanSqlText(dataItem.Search_Employee_ID);
            string levelSearch = dataItem == null ? "" : CleanSqlText(dataItem.Search_Employee_Level_ID);

            sql = @"
                SELECT
                    a.Employee_ID AS `Employee ID`,
                    a.Employee_FirstName AS `Employee FirstName`,
                    a.Employee_LastName AS `Employee LastName`,
                    a.Employee_Level_ID AS `Employee Level ID`,
                    b.Employee_Level_Name AS `Employee Level Name`
                FROM info_employee a
                LEFT JOIN info_employee_level b
                    ON a.Employee_Level_ID = b.Employee_Level_ID
                WHERE 1=1 ";

            if (!string.IsNullOrWhiteSpace(employeeSearch))
            {
                sql += $" AND a.Employee_ID LIKE '%{employeeSearch}%' ";
            }

            if (short.TryParse(levelSearch, out short employeeLevelId))
            {
                sql += $" AND a.Employee_Level_ID = {employeeLevelId} ";
            }

            sql += " ORDER BY a.Employee_ID ASC;";
            return sql;
        }

        public string SearchEmployeeSettingByEmployeeID(SettingProperty dataItem)
        {
            sql = @"
                SELECT
                    a.Employee_ID,
                    a.Employee_FirstName,
                    a.Employee_LastName,
                    a.Employee_Level_ID,
                    b.Employee_Level_Name
                FROM info_employee a
                LEFT JOIN info_employee_level b
                    ON a.Employee_Level_ID = b.Employee_Level_ID
                WHERE a.Employee_ID = 'dataItem.Employee_ID';";

            sql = sql.Replace("dataItem.Employee_ID", CleanSqlText(dataItem.Employee_ID));
            return sql;
        }

        public string SearchEmployeeNameFromPerson(SettingProperty dataItem)
        {
            sql = @"
                SELECT
                    empCode,
                    empName,
                    empSurname
                FROM person.member_fed
                WHERE empCode = 'dataItem.Employee_ID';";

            sql = sql.Replace("dataItem.Employee_ID", CleanSqlText(dataItem.Employee_ID));
            return sql;
        }

        public string CountEmployeeSettingByEmployeeID(SettingProperty dataItem)
        {
            sql = @"
                SELECT COUNT(*) AS CNT
                FROM info_employee
                WHERE Employee_ID = 'dataItem.Employee_ID';";

            sql = sql.Replace("dataItem.Employee_ID", CleanSqlText(dataItem.Employee_ID));
            return sql;
        }

        public string InsertEmployeeSetting(SettingProperty dataItem)
        {
            sql = @"
                INSERT INTO info_employee
                (
                    Employee_ID,
                    Employee_FirstName,
                    Employee_LastName,
                    Employee_Level_ID
                )
                VALUES
                (
                    'dataItem.Employee_ID',
                    dataItem.Employee_FirstName,
                    dataItem.Employee_LastName,
                    dataItem.Employee_Level_ID
                );";

            sql = sql.Replace("dataItem.Employee_ID", CleanSqlText(dataItem.Employee_ID));
            sql = sql.Replace("dataItem.Employee_FirstName", ToSqlTextOrNull(dataItem.Employee_FirstName));
            sql = sql.Replace("dataItem.Employee_LastName", ToSqlTextOrNull(dataItem.Employee_LastName));
            sql = sql.Replace("dataItem.Employee_Level_ID", ToSqlSmallIntOrNull(dataItem.Employee_Level_ID));
            return sql;
        }

        public string UpdateEmployeeSetting(SettingProperty dataItem)
        {
            sql = @"
                UPDATE info_employee
                SET Employee_FirstName = dataItem.Employee_FirstName,
                    Employee_LastName = dataItem.Employee_LastName,
                    Employee_Level_ID = dataItem.Employee_Level_ID
                WHERE Employee_ID = 'dataItem.Employee_ID';";

            sql = sql.Replace("dataItem.Employee_ID", CleanSqlText(dataItem.Employee_ID));
            sql = sql.Replace("dataItem.Employee_FirstName", ToSqlTextOrNull(dataItem.Employee_FirstName));
            sql = sql.Replace("dataItem.Employee_LastName", ToSqlTextOrNull(dataItem.Employee_LastName));
            sql = sql.Replace("dataItem.Employee_Level_ID", ToSqlSmallIntOrNull(dataItem.Employee_Level_ID));
            return sql;
        }

        public string DeleteEmployeeSetting(SettingProperty dataItem)
        {
            sql = @"
                DELETE FROM info_employee
                WHERE Employee_ID = 'dataItem.Employee_ID';";

            sql = sql.Replace("dataItem.Employee_ID", CleanSqlText(dataItem.Employee_ID));
            return sql;
        }

        public string GetEmployeeLevelList()
        {
            sql = @"
                SELECT
                    Employee_Level_ID AS VALUE,
                    Employee_Level_Name AS TEXT
                FROM info_employee_level
                ORDER BY Employee_Level_ID ASC;";

            return sql;
        }

        private string ToSqlSmallIntOrNull(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "NULL";
            }

            return short.TryParse(value.Trim(), out short number) ? number.ToString() : "NULL";
        }

        //--------------- Equipment Add ---------------------------
        public string SearchEquipmentTypeSettingList(SettingProperty dataItem)
        {
            string equipmentTypeSearch = dataItem == null ? "" : CleanSqlText(dataItem.Search_Equipment_Type);
            string equipmentNameSearch = dataItem == null ? "" : CleanSqlText(dataItem.Search_Equipment_Name);

            sql = @"
                SELECT
                    s.ID AS `Serial ID`,
                    t.Equipment_Type AS `Equipment Type`,
                    t.Equipment_Name AS `Equipment Name`,
                    s.EQUIPMENT_SERIAL AS `Equipment Serial`
                FROM info_equipment_type t
                LEFT JOIN info_equipment_serial s
                    ON t.Equipment_Type = s.EQUIPMENT_TYPE_ID
                WHERE 1=1 ";

            if (short.TryParse(equipmentTypeSearch, out short equipmentType))
            {
                sql += $" AND t.Equipment_Type = {equipmentType} ";
            }

            if (!string.IsNullOrWhiteSpace(equipmentNameSearch))
            {
                sql += $" AND (t.Equipment_Name LIKE '%{equipmentNameSearch}%' OR s.EQUIPMENT_SERIAL LIKE '%{equipmentNameSearch}%') ";
            }

            sql += " ORDER BY t.Equipment_Type ASC, s.ID ASC;";
            return sql;
        }

        public string SearchEquipmentTypeSettingByEquipmentType(SettingProperty dataItem)
        {
            sql = @"
                SELECT
                    t.Equipment_Type,
                    t.Equipment_Name,
                    s.ID AS Equipment_Serial_ID,
                    s.EQUIPMENT_SERIAL AS Equipment_Serial
                FROM info_equipment_type t
                LEFT JOIN info_equipment_serial s
                    ON t.Equipment_Type = s.EQUIPMENT_TYPE_ID
                WHERE t.Equipment_Type = dataItem.Equipment_Type;";

            sql = sql.Replace("dataItem.Equipment_Type", ToSqlSmallIntOrNull(dataItem.Equipment_Type));
            return sql;
        }

        public string CountEquipmentTypeSettingByEquipmentType(SettingProperty dataItem)
        {
            sql = @"
                SELECT COUNT(*) AS CNT
                FROM info_equipment_type
                WHERE Equipment_Type = dataItem.Equipment_Type;";

            sql = sql.Replace("dataItem.Equipment_Type", ToSqlSmallIntOrNull(dataItem.Equipment_Type));
            return sql;
        }

        public string InsertEquipmentTypeSetting(SettingProperty dataItem)
        {
            sql = @"
                INSERT INTO info_equipment_type
                (
                    Equipment_Type,
                    Equipment_Name
                )
                SELECT
                    dataItem.Equipment_Type,
                    dataItem.Equipment_Name
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM info_equipment_type x
                    WHERE x.Equipment_Name = dataItem.Equipment_Name
                );

                INSERT INTO info_equipment_serial
                (
                    EQUIPMENT_TYPE_ID,
                    EQUIPMENT_SERIAL
                )
                SELECT
                    (
                        SELECT x.Equipment_Type
                        FROM info_equipment_type x
                        WHERE x.Equipment_Name = dataItem.Equipment_Name
                        ORDER BY x.Equipment_Type DESC
                        LIMIT 1
                    ),
                    dataItem.Equipment_Serial
                WHERE dataItem.Equipment_Serial IS NOT NULL;";

            sql = sql.Replace("dataItem.Equipment_Type", ToSqlSmallIntOrNextEquipmentType(dataItem.Equipment_Type));
            sql = sql.Replace("dataItem.Equipment_Name", ToSqlTextOrNull(dataItem.Equipment_Name));
            sql = sql.Replace("dataItem.Equipment_Serial", ToSqlTextOrNull(dataItem.Equipment_Serial));
            return sql;
        }

        public string UpdateEquipmentTypeSetting(SettingProperty dataItem)
        {
            sql = @"
                UPDATE info_equipment_type
                SET Equipment_Name = dataItem.Equipment_Name
                WHERE Equipment_Type = dataItem.Equipment_Type;

                UPDATE info_equipment_serial
                SET EQUIPMENT_SERIAL = dataItem.Equipment_Serial
                WHERE ID = dataItem.Equipment_Serial_ID;

                INSERT INTO info_equipment_serial (EQUIPMENT_TYPE_ID, EQUIPMENT_SERIAL)
                SELECT dataItem.Equipment_Type, dataItem.Equipment_Serial
                WHERE dataItem.Equipment_Serial_ID IS NULL
                  AND dataItem.Equipment_Serial IS NOT NULL;";

            sql = sql.Replace("dataItem.Equipment_Type", ToSqlSmallIntOrNull(dataItem.Equipment_Type));
            sql = sql.Replace("dataItem.Equipment_Name", ToSqlTextOrNull(dataItem.Equipment_Name));
            sql = sql.Replace("dataItem.Equipment_Serial_ID", ToSqlSmallIntOrNull(dataItem.Equipment_Serial_ID));
            sql = sql.Replace("dataItem.Equipment_Serial", ToSqlTextOrNull(dataItem.Equipment_Serial));
            return sql;
        }

        public string DeleteEquipmentTypeSetting(SettingProperty dataItem)
        {
            sql = @"
                DELETE FROM info_equipment_serial
                WHERE ID = dataItem.Equipment_Serial_ID;

                DELETE FROM info_equipment_type
                WHERE Equipment_Type = dataItem.Equipment_Type
                  AND dataItem.Equipment_Serial_ID IS NULL;";

            sql = sql.Replace("dataItem.Equipment_Type", ToSqlSmallIntOrNull(dataItem.Equipment_Type));
            sql = sql.Replace("dataItem.Equipment_Serial_ID", ToSqlSmallIntOrNull(dataItem.Equipment_Serial_ID));
            return sql;
        }

        private string ToSqlSmallIntOrNextEquipmentType(string value)
        {
            if (short.TryParse(value?.Trim(), out short number))
            {
                return number.ToString();
            }

            return "(SELECT IFNULL(MAX(x.Equipment_Type) + 1, 0) FROM info_equipment_type x)";
        }
    }
}
