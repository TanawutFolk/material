using BusinessData.Interface;
using BusinessData.Property;
using CommonClassLibrary.Propertys;
using CommonClassLibrary.SQLFactorys;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace CommonClassLibrary.Services
{
    public class EmployeeService : DatabaseAction<EmployeeProperty>
    {
        EmployeeSQLFactory _sqlFactory = new EmployeeSQLFactory();
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        string sql;
        private List<string> listSql;


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
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionLogin"].ConnectionString;
            sql = _sqlFactory.SearchOne(dataItem);
            _resultData = base.SearchBySql(sql);
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysql"].ConnectionString;
            return _resultData;
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