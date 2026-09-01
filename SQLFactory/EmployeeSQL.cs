using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawMat.SQLFactory
{
    public class EmployeeSQL
    {
        private string sql;

        public string SearchEmpCode(EmployeeProperty dataItem)
        {
            sql = @"
                    SELECT
	                    EmpCode
	                  , CONCAT(EmpName,' ',EmpSurname) AS EmpFullName
	                  , EmpName
	                  , EmpSurname
	                  , EmpPosition
	                  , EmpSection
                    FROM
	                    mfg.info_employees
                    WHERE
	                    EmpCodeResign IS NULL
                    AND EmpCode = 'dataItem.EMP_CODE'
                    ";
            sql = sql.Replace("dataItem.EMP_CODE", dataItem.EMP_CODE);
            return sql;
        }

        public string SearchEmpLevel(EmployeeProperty dataItem)
        {
            sql = @"
                    SELECT a.`Employee_ID` , a.`Employee_Level_ID` , b.`Employee_Level_Name`
                    FROM `info_employee` a 
                    join `info_employee_level` b ON (a.`Employee_Level_ID` = b.`Employee_Level_ID`)
                    WHERE
                    a.`Employee_ID` = 'dataItem.EMP_CODE'
                    ";
            sql = sql.Replace("dataItem.EMP_CODE", dataItem.EMP_CODE);
            return sql;
        }

    }
}
