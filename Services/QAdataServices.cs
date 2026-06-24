using BusinessData.Interface;
using BusinessData.Property;
using RawMat.Property;
using RawMat.SQLFactory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawMat.Services
{
    public class QAdataServices : DatabaseAction<QAdataProperty>
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        QAdataSQL sqlFactory = new QAdataSQL();

        private string sql;

        public override OutputOnDbProperty Delete(QAdataProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Insert(QAdataProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search(QAdataProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search()
        {
            throw new NotImplementedException();
        }

        public OutputOnDbProperty SearchReceiveMatAll()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchReceiveMatAll();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchReceiveMatStatusProcess()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchReceiveMatStatusProcess();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertReceiveRefreshLog(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.InsertReceiveRefreshLog(dataItem);
            _resultData = base.InsertBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchLatestReceiveRefreshLog()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchLatestReceiveRefreshLog();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchInspectionList(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchInspectionList(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchActiveInspectionList()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchActiveInspectionList();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchInspListxSmartFFT(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchInspListxSmartFFT(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }



        public OutputOnDbProperty SearchMcodeSmartFFTOnly(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchMcodeSmartFFTOnly(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty checkReceiveMat(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.checkReceiveMat(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CheckStatus(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CheckStatus(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchReceiveMatStatusByReceiveDate(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchReceiveMatStatusByReceiveDate(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CheckStatusReplacement(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CheckStatusReplacement(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CountProcessStatusPending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CountProcessStatusPending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CountPackingCheck(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CountPackingCheck(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CountMaxPackingCheck(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CountMaxPackingCheck(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CountPackingSize(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CountPackingSize(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CountReportLotNo(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CountReportLotNo(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchProcessStatusPending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchProcessStatusPending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchReplacement()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchReplacement();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchForOpPackingCheck(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForOpPackingCheck(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchForOpRegular(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForOpRegular(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }



        public OutputOnDbProperty SearchForOperatePending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForOperatePending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty UpdateStatus(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateStatus(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty UpdateRegularNo(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateRegularNo(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty UpdateDataReceiveWH(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateDataReceiveWH(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }


        public OutputOnDbProperty SearchToday()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchToday();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchReportNoMax()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchReportNoMax();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchRegularNoMax()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchRegularNoMax();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty NeedKeepData(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.NeedKeepData(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty NeedFunctionCheck(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.NeedFunctionCheck(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty PackingCheck(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.PackingCheck(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty ReportLot(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.ReportLot(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty PackingSize(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.PackingSize(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty NeedRegularCheck(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.NeedRegularCheck(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty PackingCheckMode(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.PackingCheckMode(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty DetailMethod(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.DetailMethod(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchFormatReport(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchFormatReport(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }


        public OutputOnDbProperty CheckThisMonthRegular(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CheckThisMonthRegular(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertReportStatusAndReceiveMatAll(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sqlList = sqlFactory.InsertReportStatusAndReceiveMatAll(dataItem);

            _resultData = base.InsertBySqlList(sqlList);
            return _resultData;
        }

        public OutputOnDbProperty InsertReportStatusAndReceiveMat(QAdataProperty dataItem)
        {

            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sql = sqlFactory.InsertReportStatusAndReceiveMat(dataItem);

            _resultData = base.InsertBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertPackingSize(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sqlList = sqlFactory.InsertPackingSize(dataItem);

            _resultData = base.InsertBySqlList(sqlList);
            return _resultData;
        }

        public OutputOnDbProperty InsertPackingCheck(QAdataProperty dataItem)
        {

            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sql = sqlFactory.InsertPackingCheck(dataItem);

            _resultData = base.InsertBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertReportLotNo(QAdataProperty dataItem)
        {

            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sql = sqlFactory.InsertReportLotNo(dataItem);

            _resultData = base.InsertBySql(sql);
            return _resultData;
        }

        //public OutputOnDbProperty UpdatePackingCheck(QAdataProperty dataItem)
        //{
        //    strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
        //    sql = sqlFactory.UpdatePackingCheck(dataItem);
        //    _resultData = base.UpdateBySql(sql);
        //    return _resultData;
        //}

        public OutputOnDbProperty UpdateReportLotNo(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateReportLotNo(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }


        public OutputOnDbProperty RegularSampling(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.RegularSampling(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty RegularEquipment(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.RegularEquipment(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertEquipmentSerial(QAdataProperty dataItem)
        {

            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sql = sqlFactory.InsertEquipmentSerial(dataItem);

            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertRegularData(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sqlList = sqlFactory.InsertRegularData(dataItem);

            _resultData = base.InsertBySqlList(sqlList);
            return _resultData;
        }

        public OutputOnDbProperty SearchRegularRef(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchRegularRef(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchReferenceByMCode(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchReferenceByMCode(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CheckConditionRegularRef(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CheckConditionRegularRef(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchForRegularPending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForRegularPending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchRegularDataPending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchRegularDataPending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchRegularReportData(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchRegularReportData(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }
        public OutputOnDbProperty UpdateRegularRef(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateRegularRef(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchForOpFunction(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForOpFunction(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }


        public OutputOnDbProperty FunctionSampling(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.FunctionSampling(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty FunctionSampQtyLotSize(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.FunctionSampQtyLotSize(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }
        public OutputOnDbProperty InsertReportLotNoList(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sqlList = sqlFactory.InsertReportLotNoList(dataItem);

            _resultData = base.InsertBySqlList(sqlList);
            return _resultData;
        }

        public OutputOnDbProperty UpdateReportStatusLotNo(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateReportStatusLotNo(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertFunctionData(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sqlList = sqlFactory.InsertFunctionData(dataItem);

            _resultData = base.InsertBySqlList(sqlList);
            return _resultData;
        }

        public OutputOnDbProperty SearchForFunctionPending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForFunctionPending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchFunctionDataPending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchFunctionDataPending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchReportActive(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchReportActive(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertReportActive(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.InsertReportActive(dataItem);
            _resultData = base.InsertBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty DeleteReportActive(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.DeleteReportActive(dataItem);
            _resultData = base.DeleteBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty CheckReportStatus(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.CheckReportStatus(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty ReportFDA_Status(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.ReportFDA_Status(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty DimensionSampling(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.DimensionSampling(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty DimensionEquipment(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.DimensionEquipment(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchForDimensionPending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForDimensionPending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchDimensionDataPending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchDimensionDataPending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchForOpDimension(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForOpDimension(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty DimensionSampQtyLotSize(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.DimensionSampQtyLotSize(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertDimensionData(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sqlList = sqlFactory.InsertDimensionData(dataItem);

            _resultData = base.InsertBySqlList(sqlList);
            return _resultData;
        }

        public OutputOnDbProperty NeedDimensionCheck(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.NeedDimensionCheck(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty UpdateReportStatus(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateReportStatus(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchForOpData(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForOpData(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertUpdateInspData(QAdataProperty dataItem)
        {

            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sql = sqlFactory.InsertUpdateInspData(dataItem);

            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchForInspDataPending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForInspDataPending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchDataInspDataPending(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchDataInspDataPending(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchForOpAppear(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForOpAppear(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty NeedAppearCheck(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.NeedAppearCheck(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty AppearSampQtyLotSize(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.AppearSampQtyLotSize(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty AppearSampling(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.AppearSampling(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty UpdateInspQtyAppear(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.UpdateInspQtyAppear(dataItem);
            _resultData = base.UpdateBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchPackingSize(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchPackingSize(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchAppearData(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchAppearData(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchSampleSize(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchSampleSize(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }


        public OutputOnDbProperty InsertAppearPendingDetail(QAdataProperty dataItem)
        {
            List<string> sqlList = new List<string>();
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;

            sqlList = sqlFactory.InsertAppearPendingDetail(dataItem);

            _resultData = base.InsertBySqlList(sqlList);
            return _resultData;
        }

        public OutputOnDbProperty GetTotalInspected(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.GetTotalInspected(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty InsertAppearData(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.InsertAppearData(dataItem);
            _resultData = base.InsertBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty GetLatestAppearDataId(QAdataProperty dataItem)
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.GetLatestAppearDataId(dataItem);
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public OutputOnDbProperty SearchForAppearPending()
        {
            strConnection = ConfigurationManager.ConnectionStrings["ConnectionStrMysqlQA"].ConnectionString;
            sql = sqlFactory.SearchForAppearPending();
            _resultData = base.SearchBySql(sql);
            return _resultData;
        }

        public override OutputOnDbProperty Update(QAdataProperty dataItem)
        {
            throw new NotImplementedException();
        }
    }
}
