using BusinessData.Interface;
using BusinessData.Property;
using CommonClassLibrary.SQLFactorys;
using System;
using System.Collections.Generic;

namespace CommonClassLibrary.Services
{
    public class CommonService : DatabaseAction<string>
    {
        CommonSQLFactory _sqlFactory = new CommonSQLFactory();
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        string sql;
        private List<string> listSql;

        public override OutputOnDbProperty Delete(string dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Insert(string dataItem)
        {
            throw new NotImplementedException();
        }

        public OutputOnDbProperty getDateTimeNow()
        {
            sql = _sqlFactory.getDateTimeNow();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty getDateTimeNowByFormat(string formatDatetime)
        {
            sql = _sqlFactory.getDateTimeNowByFormat(formatDatetime);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty getDateTimeNowYMD()
        {
            sql = _sqlFactory.getDateTimeNowYMD();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }
        public OutputOnDbProperty getYearNow(int countYear)
        {
            sql = _sqlFactory.getYearNow(countYear);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public override OutputOnDbProperty Search(string dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search()
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Update(string dataItem)
        {
            throw new NotImplementedException();
        }
    }
}