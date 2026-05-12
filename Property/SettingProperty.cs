using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawMat.Property
{
    internal class SettingProperty
    {
        public class SamplingSettingModel
        {
            public string M_Code { get; set; }
            public short Cavity_Qty { get; set; }
            public short Sampling_Type { get; set; }
            public short Sampling_Qty { get; set; }
            public short Strictness_Type { get; set; }
            public short Strictness_Level { get; set; }
            public string Cavity_Name { get; set; }
        }
        public class MCodeSettingModel
        {
            public string M_CODE { get; set; }
            public string Keep_Data_Need { get; set; }
            public string Regular_Check_Need { get; set; }
            public string Regular_Check_Ref { get; set; }
            public string Packing_Check_Mode { get; set; }
            public string Finction_Check_Need { get; set; }
            public string Dimension_Check_Need { get; set; }
            public string Appearance_Check_Need { get; set; }
            public string INUSE { get; set; }


        }
    }
}