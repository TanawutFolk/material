using BusinessData.Property;
using RawMat.Property;
using RawMat.Services;

namespace RawMat.Models
{
    public class SettingModels
    {
        OutputOnDbProperty resultData = new OutputOnDbProperty();
        SettingServices services = new SettingServices();

        public OutputOnDbProperty SearchInspectionSettingList(SettingProperty dataItem)
        {
            resultData = services.SearchInspectionSettingList(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchInspectionSettingByMCode(SettingProperty dataItem)
        {
            resultData = services.SearchInspectionSettingByMCode(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchMCodeInMES(SettingProperty dataItem)
        {
            resultData = services.SearchMCodeInMES(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CountInspectionSettingByMCode(SettingProperty dataItem)
        {
            resultData = services.CountInspectionSettingByMCode(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertInspectionSetting(SettingProperty dataItem)
        {
            resultData = services.InsertInspectionSetting(dataItem);
            return resultData;
        }

        public OutputOnDbProperty UpdateInspectionSetting(SettingProperty dataItem)
        {
            resultData = services.UpdateInspectionSetting(dataItem);
            return resultData;
        }
        // --- เพิ่มโค้ดส่วนนี้ต่อท้ายก่อนปิดปีกกาของคลาส SettingModels ---

        public OutputOnDbProperty GetSamplingTypeList()
        {
            resultData = services.GetSamplingTypeList();
            return resultData;
        }

        public OutputOnDbProperty GetStrictnessTypeList()
        {
            resultData = services.GetStrictnessTypeList();
            return resultData;
        }

        public OutputOnDbProperty GetStrictnessLevelList()
        {
            resultData = services.GetStrictnessLevelList();
            return resultData;
        }
    }
}