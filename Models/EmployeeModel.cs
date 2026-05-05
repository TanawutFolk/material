using BusinessData.Property;
using RawMat.Property;
using RawMat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawMat.Models
{
    public class EmployeeModel
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        EmployeeServices _service = new EmployeeServices();
        public OutputOnDbProperty SearchEmpCode(EmployeeProperty dataItem)
        {
            _resultData = _service.SearchEmpCode(dataItem);
            return _resultData;
        }

        public OutputOnDbProperty SearchEmpLevel(EmployeeProperty dataItem)
        {
            _resultData = _service.SearchEmpLevel(dataItem);
            return _resultData;
        }

    }
}
