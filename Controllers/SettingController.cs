using RawMat.Models;
using BusinessData.Property;
using System.Data;
using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RawMat.Controllers
{
    internal class SettingController
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        SettingModels _model = new SettingModels();

        // ไปที่ไฟล์ SettingControllers.cs
        public DataTable SearchSamplingData(string samplingType, string mCode)
        {
            DataTable result = new DataTable();
            OutputOnDbProperty resultData = new OutputOnDbProperty();

            try
            {
                SettingProperty.SamplingSettingModel searchParam = new SettingProperty.SamplingSettingModel();
                searchParam.M_Code = mCode; // เปลี่ยนจาก M_CODE เป็น M_Code

                switch (samplingType)
                {
                    case "regular":
                        resultData = _model.SearchRegularSamplingAll(searchParam);
                        break;
                    case "function":
                        resultData = _model.SearchFunctionSamplingAll(searchParam);
                        break;
                    case "dimension":
                        resultData = _model.SearchDimensionSamplingAll(searchParam);
                        break;
                    case "appearance":
                        resultData = _model.SearchAppearanceSamplingAll(searchParam);
                        break;
                }

                if (resultData.StatusOnDb == true)
                {
                    result = resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return result;
        }

        // ==========================================
        // ส่วนดึงข้อมูล Master สำหรับ Dropdown
        // ==========================================

        public DataTable GetMasterSamplingType()
        {
            DataTable result = new DataTable();
            OutputOnDbProperty resultData = new OutputOnDbProperty();
            try
            {
                resultData = _model.GetMasterSamplingType();
                if (resultData.StatusOnDb == true)
                {
                    result = resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        public DataTable GetMasterStrictnessType()
        {
            DataTable result = new DataTable();
            OutputOnDbProperty resultData = new OutputOnDbProperty();
            try
            {
                resultData = _model.GetMasterStrictnessType();
                if (resultData.StatusOnDb == true)
                {
                    result = resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }

        public DataTable GetMasterStrictnessLevel()
        {
            DataTable result = new DataTable();
            OutputOnDbProperty resultData = new OutputOnDbProperty();
            try
            {
                resultData = _model.GetMasterStrictnessLevel();
                if (resultData.StatusOnDb == true)
                {
                    result = resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return result;
        }
        public bool UpdateSamplingData(SettingProperty.SamplingSettingModel dataItem, string category)
        {
            bool isSuccess = false;
            OutputOnDbProperty resultData = new OutputOnDbProperty();
            try
            {
                resultData = _model.UpdateSamplingData(dataItem, category);
                if (resultData.StatusOnDb == true)
                {
                    isSuccess = true;
                }
                else
                {
                    MessageBox.Show(resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return isSuccess;
        }
    }

}
