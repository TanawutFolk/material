using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace RawMat.SQLFactory
{
    internal class SettingSQL
    {
        private string sql;

        // Regular Sampling 
        public string SearchRegularSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            sql = $@"SELECT 
                        a.M_Code AS `M-Code`, 
                        a.Cavity_Qty AS `Cavity Qty`,
                        a.Cavity_Name AS `Cavity Name`,
                        t.Sampling_Type_Name AS `Sampling Type`,
                        a.Sampling_Qty AS `Sampling Qty`, 
                        st.Strictness_Name AS `Strictness Type`, 
                        sl.Strictness_Level_Name AS `Strictness Level`
                        
                    FROM info_regular_sampling a
                    LEFT JOIN info_sampling_type t ON a.sampling_type = t.sampling_type
                    LEFT JOIN info_strictness_type st ON a.Strictness_Type = st.Strictness_Type
                    LEFT JOIN info_strictness_level sl ON a.Strictness_Level = sl.Strictness_Level
                    WHERE a.M_Code LIKE '%{dataItem.M_Code}%'
                    ORDER BY a.M_Code ASC";
            return sql;
        }

        // Function Sampling 
        public string SearchFunctionSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            sql = $@"SELECT 
                        a.M_Code AS `M-Code`, 
                        a.Cavity_Qty AS `Cavity Qty`,
                        t.Sampling_Type_Name AS `Sampling Type`,
                        a.Sampling_Qty AS `Sampling Qty`, 
                        st.Strictness_Name AS `Strictness Type`, 
                        sl.Strictness_Level_Name AS `Strictness Level`, 
                        a.Cavity_Name AS `Cavity Name`
                    FROM info_function_sampling a
                    LEFT JOIN info_sampling_type t ON a.sampling_type = t.sampling_type
                    LEFT JOIN info_strictness_type st ON a.Strictness_Type = st.Strictness_Type
                    LEFT JOIN info_strictness_level sl ON a.Strictness_Level = sl.Strictness_Level
                    WHERE a.M_Code LIKE '%{dataItem.M_Code}%'
                    ORDER BY a.M_Code ASC";
            return sql;
        }

        // Dimension Sampling 
        public string SearchDimensionSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            sql = $@"SELECT 
                        a.M_Code AS `M-Code`, 
                        a.Cavity_Qty AS `Cavity Qty`,
                        t.Sampling_Type_Name AS `Sampling Type`,
                        a.Sampling_Qty AS `Sampling Qty`, 
                        st.Strictness_Name AS `Strictness Type`, 
                        sl.Strictness_Level_Name AS `Strictness Level`, 
                        a.Cavity_Name AS `Cavity Name`
                    FROM info_dimension_sampling a
                    LEFT JOIN info_sampling_type t ON a.sampling_type = t.sampling_type
                    LEFT JOIN info_strictness_type st ON a.Strictness_Type = st.Strictness_Type
                    LEFT JOIN info_strictness_level sl ON a.Strictness_Level = sl.Strictness_Level
                    WHERE a.M_Code LIKE '%{dataItem.M_Code}%'
                    ORDER BY a.M_Code ASC";
            return sql;
        }

        // Appearance Sampling 
        public string SearchAppearanceSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            sql = $@"SELECT 
                        a.M_Code AS `M-Code`, 
                        a.Cavity_Qty AS `Cavity Qty`,
                        t.Sampling_Type_Name AS `Sampling Type`,
                        a.Sampling_Qty AS `Sampling Qty`, 
                        st.Strictness_Name AS `Strictness Type`, 
                        sl.Strictness_Level_Name AS `Strictness Level`, 
                        a.Cavity_Name AS `Cavity Name`
                    FROM info_appearance_sampling a
                    LEFT JOIN info_sampling_type t ON a.sampling_type = t.sampling_type
                    LEFT JOIN info_strictness_type st ON a.Strictness_Type = st.Strictness_Type
                    LEFT JOIN info_strictness_level sl ON a.Strictness_Level = sl.Strictness_Level
                    WHERE a.M_Code LIKE '%{dataItem.M_Code}%'
                    ORDER BY a.M_Code ASC";
            return sql;
        }
        // dropdown Sampling Type
        public string GetMasterSamplingType()
        {
            return "SELECT sampling_type, Sampling_Type_Name FROM info_sampling_type ORDER BY sampling_type ASC";
        }

        // dropdown Strictness Type
        public string GetMasterStrictnessType()
        {
            return "SELECT Strictness_Type, Strictness_Name FROM info_strictness_type ORDER BY Strictness_Type ASC";
        }

        // dropdown Strictness Level
        public string GetMasterStrictnessLevel()
        {
            return "SELECT Strictness_Level, Strictness_Level_Name FROM info_strictness_level ORDER BY Strictness_Level ASC";
        }

        // ---------------- Update ----------------
        public string UpdateSamplingData(SettingProperty.SamplingSettingModel dataItem, string category)
        {
            string tableName = "";

            // เช็คว่า User กำลังแก้ข้อมูลของตารางไหน
            switch (category.ToLower())
            {
                case "regular": tableName = "info_regular_sampling"; break;
                case "function": tableName = "info_function_sampling"; break;
                case "dimension": tableName = "info_dimension_sampling"; break;
                case "appearance": tableName = "info_appearance_sampling"; break;
            }

            // สร้างคำสั่ง UPDATE
            sql = $@"UPDATE {tableName} SET 
                Cavity_Qty = '{dataItem.Cavity_Qty}',
                sampling_type = '{dataItem.Sampling_Type}',
                Sampling_Qty = '{dataItem.Sampling_Qty}',
                Strictness_Type = '{dataItem.Strictness_Type}',
                Strictness_Level = '{dataItem.Strictness_Level}',
                Cavity_Name = '{dataItem.Cavity_Name}'
             WHERE M_Code = '{dataItem.M_Code}'";

            return sql;
        }


        // ------------------------------ M CODE Setting ------------------------------
        //public string Search_M_Code()
        //{
        //    sql= 
        //        Return sql
        //}
    }
}