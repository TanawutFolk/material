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
    }
}