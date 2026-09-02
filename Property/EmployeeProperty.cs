using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawMat.Property
{
    public static class EmployeeManager
    {
        public static EmployeeProperty CurrentEmployee { get; set; } = new EmployeeProperty();
    }

    public class EmployeeProperty
    {
        public string EMP_CODE { get; set; }
        public string EMP_FULL_NAME { get; set; }
        public string EMP_NAME { get; set; }
        public string EMP_SURNAME { get; set; }
        public string EMP_POSITION { get; set; }
        public string EMP_SECTION { get; set; }
        public string EMP_LEVEL { get; set; }
        public string EMP_LEVEL_NAME { get; set; }

    }
}
