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
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlMES"].ConnectionString;
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

        // --- เพิ่มโค้ดส่วนนี้ต่อท้ายก่อนปิดปีกกาของคลาส SettingServices ---

        public OutputOnDbProperty GetSamplingTypeList()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.GetSamplingTypeList();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty GetStrictnessTypeList()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.GetStrictnessTypeList();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty GetStrictnessLevelList()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.GetStrictnessLevelList();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }
        public OutputOnDbProperty SearchRegularEquipmentSetting(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchRegularEquipmentSetting(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchDimensionEquipmentSetting(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchDimensionEquipmentSetting(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty GetEquipmentTypeList()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.GetEquipmentTypeList();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }
    }
}