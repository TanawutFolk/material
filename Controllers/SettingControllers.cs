using BusinessData.Property;
using RawMat.Models;
using RawMat.Property;
using System;
using System.Data;
using System.Windows.Forms;

namespace RawMat.Controllers
{
    public class SettingControllers
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        SettingModels _model = new SettingModels();

        DataTable dtRegularEquipment = new DataTable();
        DataTable dtDimensionEquipment = new DataTable();
        DataTable dtEquipmentType = new DataTable();

        public DataTable SearchInspectionSettingList(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.SearchInspectionSettingList(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public DataTable SearchInspectionSettingByMCode(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.SearchInspectionSettingByMCode(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public DataTable SearchMCodeInMES(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.SearchMCodeInMES(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public int CountInspectionSettingByMCode(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.CountInspectionSettingByMCode(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            if (result == null || result.Rows.Count == 0)
            {
                return 0;
            }

            return Convert.ToInt32(result.Rows[0]["CNT"]);
        }

        public Boolean InsertInspectionSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                _resultData = _model.InsertInspectionSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public Boolean UpdateInspectionSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                _resultData = _model.UpdateInspectionSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public Boolean SaveInspectionSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                int cnt = CountInspectionSettingByMCode(dataItem);

                if (cnt > 0)
                {
                    bl = UpdateInspectionSetting(dataItem);
                }
                else
                {
                    bl = InsertInspectionSetting(dataItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }
        // --- เพิ่มโค้ดส่วนนี้ต่อท้ายก่อนปิดปีกกาของคลาส SettingControllers ---

        public DataTable GetSamplingTypeList()
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.GetSamplingTypeList();
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }
            return result;
        }

        public DataTable GetStrictnessTypeList()
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.GetStrictnessTypeList();
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }
            return result;
        }

        public DataTable GetStrictnessLevelList()
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.GetStrictnessLevelList();
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }
            return result;
        }
        //------------------- Equipment

        public DataTable SearchRegularEquipmentSetting(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.SearchRegularEquipmentSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public DataTable SearchDimensionEquipmentSetting(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.SearchDimensionEquipmentSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public DataTable GetEquipmentTypeList()
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.GetEquipmentTypeList();

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public Boolean SaveRegularEquipmentSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                _resultData = _model.SaveRegularEquipmentSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public Boolean SaveDimensionEquipmentSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                _resultData = _model.SaveDimensionEquipmentSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public DataTable SearchEmployeeSettingList(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.SearchEmployeeSettingList(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public DataTable SearchEmployeeSettingByEmployeeID(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.SearchEmployeeSettingByEmployeeID(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public int CountEmployeeSettingByEmployeeID(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.CountEmployeeSettingByEmployeeID(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            if (result == null || result.Rows.Count == 0)
            {
                return 0;
            }

            return Convert.ToInt32(result.Rows[0]["CNT"]);
        }

        public Boolean InsertEmployeeSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                _resultData = _model.InsertEmployeeSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public Boolean UpdateEmployeeSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                _resultData = _model.UpdateEmployeeSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public Boolean DeleteEmployeeSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                _resultData = _model.DeleteEmployeeSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public Boolean SaveEmployeeSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                int cnt = CountEmployeeSettingByEmployeeID(dataItem);

                if (cnt > 0)
                {
                    bl = UpdateEmployeeSetting(dataItem);
                }
                else
                {
                    bl = InsertEmployeeSetting(dataItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public DataTable GetEmployeeLevelList()
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.GetEmployeeLevelList();

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public DataTable SearchEquipmentTypeSettingList(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.SearchEquipmentTypeSettingList(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public DataTable SearchEquipmentTypeSettingByEquipmentType(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.SearchEquipmentTypeSettingByEquipmentType(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    result = null;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                result = null;
            }

            return result;
        }

        public int CountEquipmentTypeSettingByEquipmentType(SettingProperty dataItem)
        {
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.CountEquipmentTypeSettingByEquipmentType(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0;
            }

            if (result == null || result.Rows.Count == 0)
            {
                return 0;
            }

            return Convert.ToInt32(result.Rows[0]["CNT"]);
        }

        public Boolean InsertEquipmentTypeSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                _resultData = _model.InsertEquipmentTypeSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public Boolean UpdateEquipmentTypeSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                _resultData = _model.UpdateEquipmentTypeSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public Boolean DeleteEquipmentTypeSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                _resultData = _model.DeleteEquipmentTypeSetting(dataItem);

                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }

        public Boolean SaveEquipmentTypeSetting(SettingProperty dataItem)
        {
            Boolean bl = false;

            try
            {
                int cnt = CountEquipmentTypeSettingByEquipmentType(dataItem);

                if (cnt > 0)
                {
                    bl = UpdateEquipmentTypeSetting(dataItem);
                }
                else
                {
                    bl = InsertEquipmentTypeSetting(dataItem);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return bl;
        }
    }
}
