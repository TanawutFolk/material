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
    public class QAdataModels
    {
        OutputOnDbProperty resultData = new OutputOnDbProperty();
        QAdataServices services = new QAdataServices();

        public OutputOnDbProperty SearchReceiveMatAll()
        {
            resultData = services.SearchReceiveMatAll();
            return resultData;
        }

        public OutputOnDbProperty SearchReceiveMatStatusProcess()
        {
            resultData = services.SearchReceiveMatStatusProcess();
            return resultData;
        }

        public OutputOnDbProperty InsertReceiveRefreshLog(QAdataProperty dataItem)
        {
            resultData = services.InsertReceiveRefreshLog(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchLatestReceiveRefreshLog()
        {
            resultData = services.SearchLatestReceiveRefreshLog();
            return resultData;
        }

        public OutputOnDbProperty SearchInspectionList(QAdataProperty dataItem)
        {
            resultData = services.SearchInspectionList(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchActiveInspectionList()
        {
            resultData = services.SearchActiveInspectionList();
            return resultData;
        }

        public OutputOnDbProperty SearchInspListxSmartFFT(QAdataProperty dataItem)
        {
            resultData = services.SearchInspListxSmartFFT(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchMcodeSmartFFTOnly(QAdataProperty dataItem)
        {
            resultData = services.SearchMcodeSmartFFTOnly(dataItem);
            return resultData;
        }

        public OutputOnDbProperty checkReceiveMat(QAdataProperty dataItem)
        {
            resultData = services.checkReceiveMat(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchToday()
        {
            resultData = services.SearchToday();
            return resultData;
        }

        public OutputOnDbProperty SearchReportNoMax()
        {
            resultData = services.SearchReportNoMax();
            return resultData;
        }

        public OutputOnDbProperty SearchRegularNoMax()
        {
            resultData = services.SearchRegularNoMax();
            return resultData;
        }

        public OutputOnDbProperty NeedKeepData(QAdataProperty dataItem)
        {
            resultData = services.NeedKeepData(dataItem);
            return resultData;
        }

        public OutputOnDbProperty NeedRegularCheck(QAdataProperty dataItem)
        {
            resultData = services.NeedRegularCheck(dataItem);
            return resultData;
        }

        public OutputOnDbProperty NeedFunctionCheck(QAdataProperty dataItem)
        {
            resultData = services.NeedFunctionCheck(dataItem);
            return resultData;
        }

        public OutputOnDbProperty PackingCheckMode(QAdataProperty dataItem)
        {
            resultData = services.PackingCheckMode(dataItem);
            return resultData;
        }

        public OutputOnDbProperty DetailMethod(QAdataProperty dataItem)
        {
            resultData = services.DetailMethod(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchFormatReport(QAdataProperty dataItem)
        {
            resultData = services.SearchFormatReport(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CheckThisMonthRegular(QAdataProperty dataItem)
        {
            resultData = services.CheckThisMonthRegular(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CheckStatus(QAdataProperty dataItem)
        {
            resultData = services.CheckStatus(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchReceiveMatStatusByReceiveDate(QAdataProperty dataItem)
        {
            resultData = services.SearchReceiveMatStatusByReceiveDate(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CheckStatusReplacement(QAdataProperty dataItem)
        {
            resultData = services.CheckStatusReplacement(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CountProcessStatusPending(QAdataProperty dataItem)
        {
            resultData = services.CountProcessStatusPending(dataItem);
            return resultData;
        }
        public OutputOnDbProperty CountPackingCheck(QAdataProperty dataItem)
        {
            resultData = services.CountPackingCheck(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CountMaxPackingCheck(QAdataProperty dataItem)
        {
            resultData = services.CountMaxPackingCheck(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CountReportLotNo(QAdataProperty dataItem)
        {
            resultData = services.CountReportLotNo(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CountPackingSize(QAdataProperty dataItem)
        {
            resultData = services.CountPackingSize(dataItem);
            return resultData;
        }


        public OutputOnDbProperty PackingCheck(QAdataProperty dataItem)
        {
            resultData = services.PackingCheck(dataItem);
            return resultData;
        }

        public OutputOnDbProperty ReportLot(QAdataProperty dataItem)
        {
            resultData = services.ReportLot(dataItem);
            return resultData;
        }

        public OutputOnDbProperty PackingSize(QAdataProperty dataItem)
        {
            resultData = services.PackingSize(dataItem);
            return resultData;
        }


        public OutputOnDbProperty SearchProcessStatusPending(QAdataProperty dataItem)
        {
            resultData = services.SearchProcessStatusPending(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchReplacement()
        {
            resultData = services.SearchReplacement();
            return resultData;
        }

        public OutputOnDbProperty SearchForOpPackingCheck(QAdataProperty dataItem)
        {
            resultData = services.SearchForOpPackingCheck(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForOpRegular(QAdataProperty dataItem)
        {
            resultData = services.SearchForOpRegular(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForOperatePending(QAdataProperty dataItem)
        {
            resultData = services.SearchForOperatePending(dataItem);
            return resultData;
        }

        public OutputOnDbProperty UpdateStatus(QAdataProperty dataItem)
        {
            resultData = services.UpdateStatus(dataItem);
            return resultData;
        }

        public OutputOnDbProperty UpdateRegularNo(QAdataProperty dataItem)
        {
            resultData = services.UpdateRegularNo(dataItem);
            return resultData;
        }

        public OutputOnDbProperty UpdateDataReceiveWH(QAdataProperty dataItem)
        {
            resultData = services.UpdateDataReceiveWH(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertReportStatusAndReceiveMatAll(QAdataProperty dataItem)
        {
            resultData = services.InsertReportStatusAndReceiveMatAll(dataItem);
            return resultData;
        }
        public OutputOnDbProperty InsertReportStatusAndReceiveMat(QAdataProperty dataItem)
        {
            resultData = services.InsertReportStatusAndReceiveMat(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertPackingSize(QAdataProperty dataItem)
        {
            resultData = services.InsertPackingSize(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertPackingCheck(QAdataProperty dataItem)
        {
            resultData = services.InsertPackingCheck(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertReportLotNo(QAdataProperty dataItem)
        {
            resultData = services.InsertReportLotNo(dataItem);
            return resultData;
        }

        //public OutputOnDbProperty UpdatePackingCheck(QAdataProperty dataItem)
        //{
        //    resultData = services.UpdatePackingCheck(dataItem);
        //    return resultData;
        //}
        public OutputOnDbProperty RegularSampling(QAdataProperty dataItem)
        {
            resultData = services.RegularSampling(dataItem);
            return resultData;
        }

        public OutputOnDbProperty RegularEquipment(QAdataProperty dataItem)
        {
            resultData = services.RegularEquipment(dataItem);
            return resultData;
        }



        public OutputOnDbProperty UpdateReportLotNo(QAdataProperty dataItem)
        {
            resultData = services.UpdateReportLotNo(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertEquipmentSerial(QAdataProperty dataItem)
        {
            resultData = services.InsertEquipmentSerial(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertRegularData(QAdataProperty dataItem)
        {
            resultData = services.InsertRegularData(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchRegularRef(QAdataProperty dataItem)
        {
            resultData = services.SearchRegularRef(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchReferenceByMCode(QAdataProperty dataItem)
        {
            resultData = services.SearchReferenceByMCode(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CheckConditionRegularRef(QAdataProperty dataItem)
        {
            resultData = services.CheckConditionRegularRef(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForRegularPending(QAdataProperty dataItem)
        {
            resultData = services.SearchForRegularPending(dataItem);
            return resultData;
        }


        public OutputOnDbProperty SearchRegularDataPending(QAdataProperty dataItem)
        {
            resultData = services.SearchRegularDataPending(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchRegularReportData(QAdataProperty dataItem)
        {
            resultData = services.SearchRegularReportData(dataItem);
            return resultData;
        }
        public OutputOnDbProperty UpdateRegularRef(QAdataProperty dataItem)
        {
            resultData = services.UpdateRegularRef(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForOpFunction(QAdataProperty dataItem)
        {
            resultData = services.SearchForOpFunction(dataItem);
            return resultData;
        }

        public OutputOnDbProperty FunctionSampling(QAdataProperty dataItem)
        {
            resultData = services.FunctionSampling(dataItem);
            return resultData;
        }

        public OutputOnDbProperty FunctionSampQtyLotSize(QAdataProperty dataItem)
        {
            resultData = services.FunctionSampQtyLotSize(dataItem);
            return resultData;
        }

        public OutputOnDbProperty UpdateReportStatusLotNo(QAdataProperty dataItem)
        {
            resultData = services.UpdateReportStatusLotNo(dataItem);
            return resultData;
        }

        public OutputOnDbProperty UpdateReportProcessLotNo(QAdataProperty dataItem)
        {
            resultData = services.UpdateReportProcessLotNo(dataItem);
            return resultData;
        }


        public OutputOnDbProperty InsertReportLotNoList(QAdataProperty dataItem)
        {
            resultData = services.InsertReportLotNoList(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertFunctionData(QAdataProperty dataItem)
        {
            resultData = services.InsertFunctionData(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForFunctionPending(QAdataProperty dataItem)
        {
            resultData = services.SearchForFunctionPending(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchFunctionDataPending(QAdataProperty dataItem)
        {
            resultData = services.SearchFunctionDataPending(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchReportActive(QAdataProperty dataItem)
        {
            resultData = services.SearchReportActive(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertReportActive(QAdataProperty dataItem)
        {
            resultData = services.InsertReportActive(dataItem);
            return resultData;
        }

        public OutputOnDbProperty DeleteReportActive(QAdataProperty dataItem)
        {
            resultData = services.DeleteReportActive(dataItem);
            return resultData;
        }


        public OutputOnDbProperty DeleteReportHistory(QAdataProperty dataItem)
        {
            resultData = services.DeleteReportHistory(dataItem);
            return resultData;
        }
        public OutputOnDbProperty CheckReportStatus(QAdataProperty dataItem)
        {
            resultData = services.CheckReportStatus(dataItem);
            return resultData;
        }

        public OutputOnDbProperty ReportFDA_Status(QAdataProperty dataItem)
        {
            resultData = services.ReportFDA_Status(dataItem);
            return resultData;
        }

        public OutputOnDbProperty DimensionSampling(QAdataProperty dataItem)
        {
            resultData = services.DimensionSampling(dataItem);
            return resultData;
        }

        public OutputOnDbProperty DimensionEquipment(QAdataProperty dataItem)
        {
            resultData = services.DimensionEquipment(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForDimensionPending(QAdataProperty dataItem)
        {
            resultData = services.SearchForDimensionPending(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchDimensionDataPending(QAdataProperty dataItem)
        {
            resultData = services.SearchDimensionDataPending(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForOpDimension(QAdataProperty dataItem)
        {
            resultData = services.SearchForOpDimension(dataItem);
            return resultData;
        }

        public OutputOnDbProperty DimensionSampQtyLotSize(QAdataProperty dataItem)
        {
            resultData = services.DimensionSampQtyLotSize(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertDimensionData(QAdataProperty dataItem)
        {
            resultData = services.InsertDimensionData(dataItem);
            return resultData;
        }

        public OutputOnDbProperty NeedDimensionCheck(QAdataProperty dataItem)
        {
            resultData = services.NeedDimensionCheck(dataItem);
            return resultData;
        }

        public OutputOnDbProperty UpdateReportStatus(QAdataProperty dataItem)
        {
            resultData = services.UpdateReportStatus(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForOpData(QAdataProperty dataItem)
        {
            resultData = services.SearchForOpData(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertUpdateInspData(QAdataProperty dataItem)
        {
            resultData = services.InsertUpdateInspData(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForInspDataPending(QAdataProperty dataItem)
        {
            resultData = services.SearchForInspDataPending(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchDataInspDataPending(QAdataProperty dataItem)
        {
            resultData = services.SearchDataInspDataPending(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForOpAppear(QAdataProperty dataItem)
        {
            resultData = services.SearchForOpAppear(dataItem);
            return resultData;
        }

        public OutputOnDbProperty NeedAppearCheck(QAdataProperty dataItem)
        {
            resultData = services.NeedAppearCheck(dataItem);
            return resultData;
        }

        public OutputOnDbProperty AppearSampQtyLotSize(QAdataProperty dataItem)
        {
            resultData = services.AppearSampQtyLotSize(dataItem);
            return resultData;
        }

        public OutputOnDbProperty AppearSampling(QAdataProperty dataItem)
        {
            resultData = services.AppearSampling(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchPackingSize(QAdataProperty dataItem)
        {
            resultData = services.SearchPackingSize(dataItem);
            return resultData;
        }

        public OutputOnDbProperty UpdateInspQtyAppear(QAdataProperty dataItem)
        {
            resultData = services.UpdateInspQtyAppear(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchAppearData(QAdataProperty dataItem)
        {
            resultData = services.SearchAppearData(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchSampleSize(QAdataProperty dataItem)
        {
            resultData = services.SearchSampleSize(dataItem);
            return resultData;
        }


        public OutputOnDbProperty InsertAppearPendingDetail(QAdataProperty dataItem)
        {
            resultData = services.InsertAppearPendingDetail(dataItem);
            return resultData;
        }


        public OutputOnDbProperty SearchAppearPendingData(QAdataProperty dataItem)
        {
            resultData = services.SearchAppearPendingData(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchForAppearPending()
        {
            resultData = services.SearchForAppearPending();
            return resultData;
        }
        public OutputOnDbProperty GetTotalInspected(QAdataProperty dataItem)
        {
            resultData = services.GetTotalInspected(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertAppearData(QAdataProperty dataItem)
        {
            resultData = services.InsertAppearData(dataItem);
            return resultData;
        }

        public OutputOnDbProperty GetLatestAppearDataId(QAdataProperty dataItem)
        {
            resultData = services.GetLatestAppearDataId(dataItem);
            return resultData;
        }

    }
}
