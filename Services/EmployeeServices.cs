using BusinessData.Interface;
using BusinessData.Property;
using RawMat.Property;
using RawMat.SQLFactory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawMat.Services
{
    public class EmployeeServices : DatabaseAction<EmployeeProperty>
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        EmployeeSQL _sqlFactory = new EmployeeSQL();
        private string sql;

        public OutputOnDbProperty SearchEmpCode(EmployeeProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysql"].ConnectionString;
            sql = _sqlFactory.SearchEmpCode(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchEmpLevel(EmployeeProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = _sqlFactory.SearchEmpLevel(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public override OutputOnDbProperty Delete(EmployeeProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Insert(EmployeeProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search(EmployeeProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search()
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Update(EmployeeProperty dataItem)
        {
            throw new NotImplementedException();
        }
    }
}
