using BusinessData.Property;
using CommonClassLibrary.Models;
using CommonClassLibrary.Propertys;
using System;
using System.Windows.Forms;

namespace CommonClassLibrary.Controllers
{
    public class EmployeeController
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        EmployeeModel _model = new EmployeeModel();

        public EmployeeProperty SearchOne(EmployeeProperty dataitem)
        {

            EmployeeProperty employeeProperty = null;

            try
            {
                _resultData = _model.SearchOne(dataitem);
                if (_resultData.StatusOnDb == true)
                {
                    if (_resultData.ResultOnDb.Rows.Count > 0)
                    {
                        employeeProperty = new EmployeeProperty
                        {
                            EmpCode = _resultData.ResultOnDb.Rows[0]["EmpCode"].ToString(),
                            EmpName = _resultData.ResultOnDb.Rows[0]["EmpName"].ToString(),
                            EmpSurname = _resultData.ResultOnDb.Rows[0]["EmpSurname"].ToString(),
                            EmpPosition = _resultData.ResultOnDb.Rows[0]["EmpPosition"].ToString(),
                            EmpDept = _resultData.ResultOnDb.Rows[0]["EmpDept"].ToString(),
                            EmpEmail = _resultData.ResultOnDb.Rows[0]["EmpEmail"].ToString(),
                            EmpCodeResign = _resultData.ResultOnDb.Rows[0]["EmpCodeResign"].ToString(),
                            EmpPcode = _resultData.ResultOnDb.Rows[0]["EmpPcode"].ToString(),
                            EmpStartwork = _resultData.ResultOnDb.Rows[0]["EmpStartwork"].ToString(),
                            UpdateTime = _resultData.ResultOnDb.Rows[0]["UpdateTime"].ToString()

                        };

                    }
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return employeeProperty;
        }
    }

}