using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionLibrary.Property
{
    public class SignInProperty
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string ClassOnDb { get; set; }
        public string MethodOnDb { get; set; }
        public DataTable ResultOnDb { get; set; }
        public string TotalCountOnDb { get; set; }

    }


}
