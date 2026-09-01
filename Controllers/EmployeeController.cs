using BusinessData.Property;
using RawMat.Property;
using RawMat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace RawMat.Controllers
{
    public class EmployeeController
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        EmployeeModel _model = new EmployeeModel();

        
        public EmployeeProperty SearchEmpCode(EmployeeProperty dataItem)
        {
            EmployeeProperty _result = null;
            try
            {
                _resultData = _model.SearchEmpCode(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    if (_resultData.ResultOnDb.Rows.Count > 0)
                    {
                        for (int i = 0; i < _resultData.ResultOnDb.Rows.Count; i++)
                        {
                            _result = new EmployeeProperty
                            {
                                EMP_CODE = _resultData.ResultOnDb.Rows[0]["EmpCode"].ToString(),
                                EMP_FULL_NAME = _resultData.ResultOnDb.Rows[0]["EmpFullName"].ToString(),
                                EMP_NAME = _resultData.ResultOnDb.Rows[0]["EmpName"].ToString(),
                                EMP_SURNAME = _resultData.ResultOnDb.Rows[0]["EmpSurname"].ToString(),
                                EMP_POSITION = _resultData.ResultOnDb.Rows[0]["EmpPosition"].ToString(),
                                EMP_SECTION = _resultData.ResultOnDb.Rows[0]["EmpSection"].ToString()
                            };
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            return _result;
        }

        public EmployeeProperty SearchEmpLevel(EmployeeProperty dataItem)
        {
            //EmployeeProperty _result = null;
            try
            {
                _resultData = _model.SearchEmpLevel(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    if (_resultData.ResultOnDb.Rows.Count > 0)
                    {
                        for (int i = 0; i < _resultData.ResultOnDb.Rows.Count; i++)
                        {

                            dataItem.EMP_CODE = _resultData.ResultOnDb.Rows[0]["Employee_ID"].ToString();
                            dataItem.EMP_LEVEL = _resultData.ResultOnDb.Rows[0]["Employee_Level_ID"].ToString();
                            dataItem.EMP_LEVEL_NAME = _resultData.ResultOnDb.Rows[0]["Employee_Level_Name"].ToString();
                            
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            return dataItem;
        }

    }
}
