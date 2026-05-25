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
        //------------------- Equipment

        public OutputOnDbProperty SearchRegularEquipmentSetting(SettingProperty dataItem)
        {
            resultData = services.SearchRegularEquipmentSetting(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchDimensionEquipmentSetting(SettingProperty dataItem)
        {
            resultData = services.SearchDimensionEquipmentSetting(dataItem);
            return resultData;
        }

        public OutputOnDbProperty GetEquipmentTypeList()
        {
            resultData = services.GetEquipmentTypeList();
            return resultData;
        }

        public OutputOnDbProperty SaveRegularEquipmentSetting(SettingProperty dataItem)
        {
            resultData = services.SaveRegularEquipmentSetting(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SaveDimensionEquipmentSetting(SettingProperty dataItem)
        {
            resultData = services.SaveDimensionEquipmentSetting(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchEmployeeSettingList(SettingProperty dataItem)
        {
            resultData = services.SearchEmployeeSettingList(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchEmployeeSettingByEmployeeID(SettingProperty dataItem)
        {
            resultData = services.SearchEmployeeSettingByEmployeeID(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CountEmployeeSettingByEmployeeID(SettingProperty dataItem)
        {
            resultData = services.CountEmployeeSettingByEmployeeID(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertEmployeeSetting(SettingProperty dataItem)
        {
            resultData = services.InsertEmployeeSetting(dataItem);
            return resultData;
        }

        public OutputOnDbProperty UpdateEmployeeSetting(SettingProperty dataItem)
        {
            resultData = services.UpdateEmployeeSetting(dataItem);
            return resultData;
        }

        public OutputOnDbProperty DeleteEmployeeSetting(SettingProperty dataItem)
        {
            resultData = services.DeleteEmployeeSetting(dataItem);
            return resultData;
        }

        public OutputOnDbProperty GetEmployeeLevelList()
        {
            resultData = services.GetEmployeeLevelList();
            return resultData;
        }

        public OutputOnDbProperty SearchEquipmentTypeSettingList(SettingProperty dataItem)
        {
            resultData = services.SearchEquipmentTypeSettingList(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchEquipmentTypeSettingByEquipmentType(SettingProperty dataItem)
        {
            resultData = services.SearchEquipmentTypeSettingByEquipmentType(dataItem);
            return resultData;
        }

        public OutputOnDbProperty CountEquipmentTypeSettingByEquipmentType(SettingProperty dataItem)
        {
            resultData = services.CountEquipmentTypeSettingByEquipmentType(dataItem);
            return resultData;
        }

        public OutputOnDbProperty InsertEquipmentTypeSetting(SettingProperty dataItem)
        {
            resultData = services.InsertEquipmentTypeSetting(dataItem);
            return resultData;
        }

        public OutputOnDbProperty UpdateEquipmentTypeSetting(SettingProperty dataItem)
        {
            resultData = services.UpdateEquipmentTypeSetting(dataItem);
            return resultData;
        }

        public OutputOnDbProperty DeleteEquipmentTypeSetting(SettingProperty dataItem)
        {
            resultData = services.DeleteEquipmentTypeSetting(dataItem);
            return resultData;
        }
    }
}
