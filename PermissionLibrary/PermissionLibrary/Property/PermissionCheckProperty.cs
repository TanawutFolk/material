using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PermissionLibrary.Property
{
    public class PermissionCheckProperty
    {
        public bool Status { get; set; }
        public string Message { get; set; }
        public string ClassOnDb { get; set; }
        public string MethodOnDb { get; set; }
        // public DataTable ResultOnDb { get; set; }
        public string TotalCountOnDb { get; set; }
        public List<PermissionCheckResult> ResultOnDb { get; set; }
    }   
        public class PermissionCheckResult
        {
            public string PERMISSION_ID { get; set; }
            public string USER_NAME { get; set; }
            public string USER_GROUP_NAME { get; set; }
            public string MENU_NAME { get; set; }
            public string APPLICATION_NAME { get; set; }
            public bool IS_CREATE { get; set; }
            public bool IS_UPDATE { get; set; }
            public bool IS_DELETE { get; set; }
            public bool IS_SEARCH { get; set; }
        }
    



}
