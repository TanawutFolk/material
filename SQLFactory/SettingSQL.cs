using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawMat.SQLFactory
{
    internal class SettingSQL
    {
        private string sql;

        // 1. ดึงข้อมูล Regular Sampling ตาม M_CODE
        public string SearchRegularSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            sql = $@"SELECT 
                        M_Code, 
                        Cavity_Qty,
                        sampling_type AS Sampling_Type,
                        Sampling_Qty, 
                        Strictness_Type, 
                        Strictness_Level, 
                        Cavity_Name
                    FROM 
                        info_regular_sampling
                    WHERE 
                        M_Code LIKE '%{dataItem.M_Code}%'
                    ORDER BY M_Code ASC";
            return sql;
        }

        // 2. ดึงข้อมูล Function Sampling ตาม M_CODE
        public string SearchFunctionSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            sql = $@"SELECT 
                        M_Code, 
                        Cavity_Qty,
                        sampling_type AS Sampling_Type,
                        Sampling_Qty, 
                        Strictness_Type, 
                        Strictness_Level, 
                        Cavity_Name
                    FROM 
                        info_function_sampling
                    WHERE 
                        M_Code LIKE '%{dataItem.M_Code}%'
                    ORDER BY M_Code ASC";
            return sql;
        }

        // 3. ดึงข้อมูล Dimension Sampling ตาม M_CODE
        public string SearchDimensionSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            sql = $@"SELECT 
                        M_Code, 
                        Cavity_Qty,
                        sampling_type AS Sampling_Type,
                        Sampling_Qty, 
                        Strictness_Type, 
                        Strictness_Level, 
                        Cavity_Name
                    FROM 
                        info_dimension_sampling
                    WHERE 
                        M_Code LIKE '%{dataItem.M_Code}%'
                    ORDER BY M_Code ASC";
            return sql;
        }

        // 4. ดึงข้อมูล Appearance Sampling ตาม M_CODE
        public string SearchAppearanceSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            sql = $@"SELECT 
                        M_Code, 
                        Cavity_Qty,
                        sampling_type AS Sampling_Type,
                        Sampling_Qty, 
                        Strictness_Type, 
                        Strictness_Level, 
                        Cavity_Name
                    FROM 
                        info_appearance_sampling
                    WHERE 
                        M_Code LIKE '%{dataItem.M_Code}%'
                    ORDER BY M_Code ASC";
            return sql;
        }
    }
}
