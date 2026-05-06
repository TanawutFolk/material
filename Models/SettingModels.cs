using BusinessData.Property;
using Microsoft.Office.Interop.Excel;
using RawMat.Property;
using RawMat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RawMat.Models
{
    internal class SettingModels
    {
        OutputOnDbProperty resultData = new OutputOnDbProperty();
        SettingService services = new SettingService();

        // เติม SettingProperty.SamplingSettingModel dataItem ในวงเล็บทุกอัน
        public OutputOnDbProperty SearchRegularSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            resultData = services.SearchRegularSamplingAll(dataItem); // ส่งต่อไป Service
            return resultData;
        }

        public OutputOnDbProperty SearchFunctionSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            resultData = services.SearchFunctionSamplingAll(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchDimensionSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            resultData = services.SearchDimensionSamplingAll(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchAppearanceSamplingAll(SettingProperty.SamplingSettingModel dataItem)
        {
            resultData = services.SearchAppearanceSamplingAll(dataItem);
            return resultData;
        }

        // ==========================================
        // ส่วนดึงข้อมูล Master สำหรับ Dropdown
        // ==========================================

        public OutputOnDbProperty GetMasterSamplingType()
        {
            resultData = services.GetMasterSamplingType();
            return resultData;
        }

        public OutputOnDbProperty GetMasterStrictnessType()
        {
            resultData = services.GetMasterStrictnessType();
            return resultData;
        }

        public OutputOnDbProperty GetMasterStrictnessLevel()
        {
            resultData = services.GetMasterStrictnessLevel();
            return resultData;
        }

        public OutputOnDbProperty UpdateSamplingData(SettingProperty.SamplingSettingModel dataItem, string category)
        {
            resultData = services.UpdateSamplingData(dataItem, category);
            return resultData;
        }
    }
}
