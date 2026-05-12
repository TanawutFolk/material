using BusinessData.Interface;
using BusinessData.Property;
using RawMat.Property;
using RawMat.SQLFactory;
using System;
using System.Configuration;

namespace RawMat.Services
{
    public class SettingServices : DatabaseAction<SettingProperty>
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        SettingSQL sqlFactory = new SettingSQL();

        private string sql;

        public override OutputOnDbProperty Delete(SettingProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Insert(SettingProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search(SettingProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search()
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Update(SettingProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public OutputOnDbProperty SearchInspectionSettingList(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchInspectionSettingList(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchInspectionSettingByMCode(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchInspectionSettingByMCode(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchMCodeInMES(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchMCodeInMES(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CountInspectionSettingByMCode(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CountInspectionSettingByMCode(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertInspectionSetting(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.InsertInspectionSetting(dataItem);
            _resultData = base.InsertBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty UpdateInspectionSetting(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateInspectionSetting(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }
    }
}