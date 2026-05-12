using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawMat.Property
{
    public class SettingProperty
    {
        // Search
        public string Search_M_CODE { get; set; }

        // Master Setting
        public string M_CODE { get; set; }
        public string Material_Name { get; set; }
        public string Vendor_ID { get; set; }
        public string Vendor_Name { get; set; }

        public string Keep_Data_Need { get; set; }
        public string Packing_Check_Mode { get; set; }
        public string Regular_Check_Need { get; set; }
        public string Regular_Check_Ref { get; set; }
        public string Function_Check_Need { get; set; }
        public string Dimension_Check_Need { get; set; }
        public string Appearance_Check_Need { get; set; }

        public string INUSE { get; set; }
        public string EMP_ID { get; set; }
    }
}
