using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawMat.Property
{
    public class SettingProperty
    {
        // --- Master Setting ---
        public string M_CODE { get; set; }
        public string Keep_Data_Need { get; set; }
        public string Packing_Check_Mode { get; set; }
        public string Regular_Check_Need { get; set; }
        public string Regular_Check_Ref { get; set; }
        public string Function_Check_Need { get; set; }
        public string Dimension_Check_Need { get; set; }
        public string Appearance_Check_Need { get; set; }
        public string INUSE { get; set; }

        // --- Search & Meta ---
        public string Search_M_CODE { get; set; }
        public string Search_Status { get; set; }

        // --- Tab 1: Regular Sampling ---
        public string Reg_Cavity_Qty { get; set; }
        public string Reg_Cavity_Name { get; set; }
        public string Reg_Strictness_Level { get; set; }
        public string Reg_Sampling_Qty { get; set; }
        public string Reg_Strictness_Type { get; set; }
        public string Reg_Sampling_Type { get; set; }

        // --- Tab 2: Function Sampling ---
        public string Func_Cavity_Qty { get; set; }
        public string Func_Cavity_Name { get; set; }
        public string Func_Strictness_Level { get; set; }
        public string Func_Sampling_Qty { get; set; }
        public string Func_Strictness_Type { get; set; }
        public string Func_Sampling_Type { get; set; }

        // --- Tab 3: Dimension Sampling ---
        public string Dim_Cavity_Qty { get; set; }
        public string Dim_Cavity_Name { get; set; }
        public string Dim_Strictness_Level { get; set; }
        public string Dim_Sampling_Qty { get; set; }
        public string Dim_Strictness_Type { get; set; }
        public string Dim_Sampling_Type { get; set; }

        // --- Tab 4: Appearance Sampling ---
        public string App_Cavity_Qty { get; set; }
        public string App_Cavity_Name { get; set; }
        public string App_Strictness_Level { get; set; }
        public string App_Sampling_Qty { get; set; }
        public string App_Strictness_Type { get; set; }
        public string App_Sampling_Type { get; set; }

        // --- Equipment Set ---
        public DataTable RegularEquipment { get; set; }
        public DataTable FunctionEquipment { get; set; }
        public DataTable FunctionChecks { get; set; }
        public DataTable DimensionEquipment { get; set; }

        // --- Employee Setting ---
        public string Employee_ID { get; set; }
        public string Employee_FirstName { get; set; }
        public string Employee_LastName { get; set; }
        public string Employee_Name { get; set; }
        public string Employee_Level_ID { get; set; }
        public string Employee_Level_Name { get; set; }
        public string Phone_Ext { get; set; }
        public string Search_Employee_ID { get; set; }
        public string Search_Employee_Level_ID { get; set; }

        // --- Equipment Type Setting ---
        public string Equipment_Type { get; set; }
        public string Equipment_Name { get; set; }
        public string Equipment_Serial_ID { get; set; }
        public string Equipment_Serial { get; set; }
        public string Search_Equipment_Type { get; set; }
        public string Search_Equipment_Name { get; set; }

        // --- NG Mode Setting ---
        public string NG_Mode_ID { get; set; }
        public string NG_Mode { get; set; }
        public string Search_NG_Mode { get; set; }
        public string IsActive { get; set; }
    }
}
