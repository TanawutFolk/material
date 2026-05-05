using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//----- Project ------
using RawMat.Property;
using RawMat.SQLFactory;
//----- BusinessData------
using BusinessData.Interface;
using BusinessData.Property;

namespace RawMat.Services
{
    internal class SettingService : DatabaseAction<SettingsProperty>
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        SettingSQL sqlFactory = new SettingSQL();

        private string sql;

        // เติม Parameter เข้าไปในวงเล็บทุกอัน
        public OutputOnDbProperty SearchRegularSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchRegularSamplingAll(dataItem); // ส่งค่า dataItem ต่อให้ SQL
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchFunctionSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchFunctionSamplingAll(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchDimensionSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchDimensionSamplingAll(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchAppearanceSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchAppearanceSamplingAll(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }
        //------ Override Method ---------
        public override OutputOnDbProperty Delete(SettingsProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Insert(SettingsProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search(SettingsProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search()
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Update(SettingsProperty dataItem)
        {
            throw new NotImplementedException();
        }
    }
}
