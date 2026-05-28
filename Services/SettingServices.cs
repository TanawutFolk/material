using BusinessData.Interface;
using BusinessData.Property;
using RawMat.Property;
using RawMat.SQLFactory;
using System;
using System.Collections.Generic;
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

        public OutputOnDbProperty SaveRegularEquipmentSetting(SettingProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sqlList = sqlFactory.SaveRegularEquipmentSetting(dataItem);
            _resultData = base.InsertBySqlList(sqlList);
            return _resultData;
        }

        public OutputOnDbProperty SaveDimensionEquipmentSetting(SettingProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sqlList = sqlFactory.SaveDimensionEquipmentSetting(dataItem);
            _resultData = base.InsertBySqlList(sqlList);
            return _resultData;
        }

        public OutputOnDbProperty SearchEmployeeSettingList(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchEmployeeSettingList(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchEmployeeSettingByEmployeeID(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchEmployeeSettingByEmployeeID(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchEmployeeNameFromPerson(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchEmployeeNameFromPerson(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CountEmployeeSettingByEmployeeID(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CountEmployeeSettingByEmployeeID(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertEmployeeSetting(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.InsertEmployeeSetting(dataItem);
            _resultData = base.InsertBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty UpdateEmployeeSetting(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateEmployeeSetting(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty DeleteEmployeeSetting(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.DeleteEmployeeSetting(dataItem);
            _resultData = base.DeleteBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty GetEmployeeLevelList()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.GetEmployeeLevelList();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchEquipmentTypeSettingList(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchEquipmentTypeSettingList(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchEquipmentTypeSettingByEquipmentType(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchEquipmentTypeSettingByEquipmentType(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CountEquipmentTypeSettingByEquipmentType(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CountEquipmentTypeSettingByEquipmentType(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertEquipmentTypeSetting(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.InsertEquipmentTypeSetting(dataItem);
            _resultData = base.InsertBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty UpdateEquipmentTypeSetting(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateEquipmentTypeSetting(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty DeleteEquipmentTypeSetting(SettingProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.DeleteEquipmentTypeSetting(dataItem);
            _resultData = base.DeleteBySql(sql);
            return _resultData;
        }
    }
}
