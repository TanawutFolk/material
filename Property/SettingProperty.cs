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
    }
}
