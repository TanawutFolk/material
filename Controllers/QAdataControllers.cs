using BusinessData.Property;
using RawMat.Property;
using RawMat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Reflection;
using static RawMat.Property.QAdataProperty;


namespace RawMat.Controllers
{
    public class QAdataControllers
    {
        OutputOnDbProperty _resultData = new OutputOnDbProperty();
        QAdataModels _model = new QAdataModels();

        public DataTable SearchReceiveMatAll()
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchReceiveMatAll();
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

        public DataTable SearchReceiveMatStatusProcess()
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchReceiveMatStatusProcess();
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

        public bool InsertReceiveRefreshLog(QAdataProperty dataItem)
        {
            try
            {
                _resultData = _model.InsertReceiveRefreshLog(dataItem);
                if (_resultData.StatusOnDb)
                {
                    return true;
                }

                MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return false;
        }

        public DataTable SearchLatestReceiveRefreshLog()
        {
            try
            {
                _resultData = _model.SearchLatestReceiveRefreshLog();
                if (_resultData.StatusOnDb)
                {
                    return _resultData.ResultOnDb;
                }

                MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return null;
        }

        public int SearchInspectionList(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchInspectionList(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return Convert.ToInt32(result.Rows[0]["CNT"]);
        }

        public DataTable SearchActiveInspectionList()
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchActiveInspectionList();
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

        public DataTable SearchInspListxSmartFFT(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchInspListxSmartFFT(dataItem);
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

        public DataTable SearchMcodeSmartFFTOnly(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchMcodeSmartFFTOnly(dataItem);
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

        public int checkReceiveMat(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.checkReceiveMat(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception(_resultData.MessageOnDb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            return Convert.ToInt32(result.Rows[0]["CNT"]);
        }

        public int NeedKeepData(QAdataProperty dataItem)
        {
            try
            {
                _resultData = _model.NeedKeepData(dataItem);

                if (!_resultData.StatusOnDb)
                {
                    throw new Exception(_resultData.MessageOnDb); // Throw เดียวพอ
                }

                DataTable result = _resultData.ResultOnDb;

                // ตรวจสอบความปลอดภัยก่อนเข้าถึงข้อมูล
                if (result == null || result.Rows.Count == 0)
                {
                    throw new Exception("ไม่พบข้อมูลในตารางผลลัพธ์");
                }

                if (!result.Columns.Contains("Keep_Data_Need"))
                {
                    throw new Exception("ไม่พบคอลัมน์ 'Keep_Data_Need' ในผลลัพธ์");
                }

                return Convert.ToInt32(result.Rows[0]["Keep_Data_Need"]);
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
               throw;
            }

        }

        public int NeedRegularCheck(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.NeedRegularCheck(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception(_resultData.MessageOnDb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            return Convert.ToInt32(result.Rows[0]["Regular_Check_Need"]);
        }

        public int NeedFunctionCheck(QAdataProperty dataItem)
        {
            try
            {
                _resultData = _model.NeedFunctionCheck(dataItem);

                if (!_resultData.StatusOnDb)
                {
                    throw new Exception(_resultData.MessageOnDb); // Throw เดียวพอ
                }

                DataTable result = _resultData.ResultOnDb;

                // ตรวจสอบความปลอดภัยก่อนเข้าถึงข้อมูล
                if (result == null || result.Rows.Count == 0)
                {
                    throw new Exception("ไม่พบข้อมูลในตารางผลลัพธ์");
                }

                if (!result.Columns.Contains("Function_Check_Need"))
                {
                    throw new Exception("ไม่พบคอลัมน์ 'Function_Check_Need' ในผลลัพธ์");
                }

                return Convert.ToInt32(result.Rows[0]["Function_Check_Need"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

        }

        public int CheckThisMonthRegular(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.CheckThisMonthRegular(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    throw new Exception(_resultData.MessageOnDb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
            return Convert.ToInt32(result.Rows[0]["cnt"]);
        }

        public DateTime SearchToday()
        {
            DateTime dtNow = new DateTime();

            try
            {
                _resultData = _model.SearchToday();
                if (_resultData.StatusOnDb == true)
                {
                    dtNow = Convert.ToDateTime(_resultData.ResultOnDb.Rows[0]["TODAY"]);
                }
                else
                {
                    dtNow = DateTime.Now;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                dtNow = DateTime.Now;
            }
            return dtNow;
        }


        public string PrefixReportRunNumber(QAdataProperty dataItem)
        {
            string strReportNoLast = "";
            string PrefixFormatReport = "QA";
            int Year;
            string CurrentYearPrefix;

            //
            Year = dataItem.dtToday.Year % 100;
            CurrentYearPrefix = $"{PrefixFormatReport}{Year}-";

            try
            {
                _resultData = _model.SearchReportNoMax();
                if (_resultData.StatusOnDb == true)
                {
                    strReportNoLast = _resultData.ResultOnDb.Rows[0]["LAST_REPORT_NO"].ToString();
                    // ดึงเลขท้าย xxxx ออกจาก strReportNoLast
                    var numberPart = strReportNoLast.Replace(CurrentYearPrefix, "");
                    if (int.TryParse(numberPart, out int currentNumber))
                    {
                        // เพิ่มตัวเลขถัดไป
                        var nextNumber = currentNumber + 1;
                        return $"{CurrentYearPrefix}{nextNumber:D4}";
                    }
                }
                else
                {
                    // ถ้าไม่มีข้อมูลในฐานข้อมูล ให้เริ่มต้นที่ -0001
                    return $"{CurrentYearPrefix}0001";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return $"{CurrentYearPrefix}0001";
        }

        public string PrefixRegularRunNumber(QAdataProperty dataItem)
        {
            string strRegularNoLast = "";
            string PrefixFormatRegular = "RI";
            int Year;
            int Month;
            string CurrentYearPrefix;

            //
            Year = dataItem.dtToday.Year % 100;
            Month = dataItem.dtToday.Month;
            CurrentYearPrefix = $"{PrefixFormatRegular}{Year}{Month:D2}-";

            try
            {
                _resultData = _model.SearchRegularNoMax();
                if (_resultData.StatusOnDb == true)
                {
                    strRegularNoLast = _resultData.ResultOnDb.Rows[0]["LAST_Regular_NO"].ToString();
                    // ดึงเลขท้าย xxxx ออกจาก strRegularNoLast
                    var numberPart = strRegularNoLast.Replace(CurrentYearPrefix, "");
                    if (int.TryParse(numberPart, out int currentNumber))
                    {
                        // เพิ่มตัวเลขถัดไป
                        var nextNumber = currentNumber + 1;
                        return $"{CurrentYearPrefix}{nextNumber:D4}";
                    }
                }
                else
                {
                    // ถ้าไม่มีข้อมูลในฐานข้อมูล ให้เริ่มต้นที่ -0001
                    return $"{CurrentYearPrefix}0001";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return $"{CurrentYearPrefix}0001";
        }

        public DataTable CheckStatus(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.CheckStatus(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchReceiveMatStatusByReceiveDate(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchReceiveMatStatusByReceiveDate(dataItem);
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

        public DataTable CheckStatusReplacement(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.CheckStatusReplacement(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public int CountProcessStatusPending(QAdataProperty dataItem)
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.CountProcessStatusPending(dataItem);
                if (_resultData.StatusOnDb == true && _resultData.ResultOnDb != null && _resultData.ResultOnDb.Rows.Count > 0)
                {
                    result = _resultData.ResultOnDb;
                    return Convert.ToInt32(result.Rows[0]["CNT"]);
                }
                else
                {
                    //MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return 0; // หรือค่าอื่นที่เหมาะสมเมื่อไม่มีข้อมูล
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show($"เกิดข้อผิดพลาด: {ex.Message}", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return 0; // หรือค่าอื่นที่เหมาะสมเมื่อเกิด error
            }
        }

        public int CountPackingCheck(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.CountPackingCheck(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return Convert.ToInt32(result.Rows[0]["CNT"]);
        }

        public int CountMaxPackingCheck(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.CountMaxPackingCheck(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return Convert.ToInt32(result.Rows[0]["CNT"]);
        }

        public int CountReportLotNo(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.CountReportLotNo(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return Convert.ToInt32(result.Rows[0]["CNT"]);
        }

        public int CountPackingSize(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.CountPackingSize(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return Convert.ToInt32(result.Rows[0]["CNT"]);
        }


        public DataTable PackingCheck(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.PackingCheck(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable ReportLot(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.ReportLot(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable PackingSize(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.PackingSize(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchProcessStatusPending(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchProcessStatusPending(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }


        public DataTable SearchReplacement()
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchReplacement();
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchForOpPackingCheck(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForOpPackingCheck(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchForOpRegular(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForOpRegular(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }



        public DataTable SearchForOperatePending(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForOperatePending(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public Boolean UpdateStatus(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.UpdateStatus(dataItem);
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

        public Boolean UpdateDataReceiveWH(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.UpdateDataReceiveWH(dataItem);
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

        public Boolean UpdateRegularNo(QAdataProperty dataItem)
        {
            Boolean bl = false;
            try
            {
                _resultData = _model.UpdateRegularNo(dataItem);
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


        public Boolean InsertReportStatusAndReceiveMatAll(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertReportStatusAndReceiveMatAll(dataItem);
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

        public Boolean InsertReportStatusAndReceiveMat(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertReportStatusAndReceiveMat(dataItem);
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

        public Boolean InsertPackingSize(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertPackingSize(dataItem);
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

        public Boolean InsertPackingCheck(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertPackingCheck(dataItem);
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

        public Boolean InsertReportLotNo(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertReportLotNo(dataItem);
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

        //public Boolean UpdatePackingCheck(QAdataProperty dataItem)
        //{
        //    Boolean bl = false;
        //    try
        //    {
        //        _resultData = _model.UpdatePackingCheck(dataItem);
        //        if (_resultData.StatusOnDb == true)
        //        {
        //            bl = _resultData.StatusOnDb;
        //        }
        //        else
        //        {
        //            MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
        //    }
        //    return bl;
        //}

        public Boolean UpdateReportLotNo(QAdataProperty dataItem)
        {
            Boolean bl = false;
            try
            {
                _resultData = _model.UpdateReportLotNo(dataItem);
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

        public string PackingCheckMode(QAdataProperty dataItem)
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.PackingCheckMode(dataItem);
                if (_resultData.StatusOnDb)
                {
                    result = _resultData.ResultOnDb;

                    // ตรวจสอบว่ามีข้อมูลในตารางหรือไม่
                    if (result.Rows.Count > 0)
                    {
                        return result.Rows[0]["Packing_Check_Mode"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("ไม่มีข้อมูลในผลลัพธ์", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return string.Empty; // หรือค่าที่เหมาะสม เช่น "N/A", "Error", เป็นต้น
                    }
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return string.Empty; // หรือค่าที่เหมาะสม
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty; // หรือค่าที่เหมาะสม
            }
        }

        public string DetailMethod(QAdataProperty dataItem)
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.DetailMethod(dataItem);
                if (_resultData.StatusOnDb)
                {
                    result = _resultData.ResultOnDb;

                    // ตรวจสอบว่ามีข้อมูลในตารางหรือไม่
                    if (result.Rows.Count > 0)
                    {
                        return result.Rows[0]["DETAIL_METHOD"].ToString();
                    }
                    else
                    {
                        MessageBox.Show("ไม่มีข้อมูลในผลลัพธ์", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return string.Empty; // หรือค่าที่เหมาะสม เช่น "N/A", "Error", เป็นต้น
                    }
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return string.Empty; // หรือค่าที่เหมาะสม
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty; // หรือค่าที่เหมาะสม
            }
        }

        public DataTable SearchFormatReport(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchFormatReport(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable RegularSampling(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.RegularSampling(dataItem);
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
            }
            return result;
        }

        public DataTable RegularEquipment(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.RegularEquipment(dataItem);
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
            }
            return result;
        }

        public int InsertEquipmentSerial(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.InsertEquipmentSerial(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return Convert.ToInt32(result.Rows[0]["ID"]);
        }

        public Boolean InsertRegularData(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertRegularData(dataItem);
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

        public DataTable SearchRegularRef(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchRegularRef(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public string SearchReferenceByMCode(QAdataProperty dataItem)
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchReferenceByMCode(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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

            if (result == null || result.Rows.Count == 0 || !result.Columns.Contains("REFERENCE"))
            {
                return string.Empty;
            }

            return result.Rows[0]["REFERENCE"]?.ToString() ?? string.Empty;
        }

        public DataTable CheckConditionRegularRef(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.CheckConditionRegularRef(dataItem);
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
            }
            return result;
        }

        public DataTable SearchForRegularPending(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForRegularPending(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchRegularDataPending(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchRegularDataPending(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchRegularReportData(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchRegularReportData(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }
        public Boolean UpdateRegularRef(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.UpdateRegularRef(dataItem);
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

        public DataTable SearchForOpFunction(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForOpFunction(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable FunctionSampling(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.FunctionSampling(dataItem);
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
            }
            return result;
        }

        public DataTable FunctionSampQtyLotSize(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.FunctionSampQtyLotSize(dataItem);
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
            }
            return result;
        }

        public Boolean UpdateReportStatusLotNo(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.UpdateReportStatusLotNo(dataItem);
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

        public Boolean UpdateReportProcessLotNo(QAdataProperty dataItem)
        {
            Boolean bl = false;
            try
            {
                _resultData = _model.UpdateReportProcessLotNo(dataItem);
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


        public Boolean InsertReportLotNoList(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertReportLotNoList(dataItem);
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

        public Boolean InsertFunctionData(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertFunctionData(dataItem);
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

        public DataTable SearchForFunctionPending(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForFunctionPending(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchFunctionDataPending(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchFunctionDataPending(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;

        }

        public DataTable SearchReportActive(QAdataProperty dataItem)
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchReportActive(dataItem);
                if (_resultData.StatusOnDb)
                {
                    result = _resultData.ResultOnDb;

                    // ตรวจสอบว่ามีข้อมูลในตารางหรือไม่
                    if (result.Rows.Count > 0)
                    {
                        return result;
                    }
                    else
                    {
                        //MessageBox.Show("ไม่มีข้อมูลในผลลัพธ์", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return null; // หรือค่าที่เหมาะสม เช่น "N/A", "Error", เป็นต้น
                    }
                }
                else
                {
                    //MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null; // หรือค่าที่เหมาะสม
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null; // หรือค่าที่เหมาะสม
            }
        }

        public Boolean InsertReportActive(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertReportActive(dataItem);
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

        public Boolean DeleteReportActive(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.DeleteReportActive(dataItem);
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


        public Boolean DeleteReportHistory(QAdataProperty dataItem)
        {
            Boolean bl = false;
            try
            {
                _resultData = _model.DeleteReportHistory(dataItem);
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
        public Boolean CheckReportStatus(QAdataProperty dataItem)
        {

            Boolean bl = true;
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.CheckReportStatus(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;

                    string report_status = string.IsNullOrEmpty(result.Rows[0]["report_status"].ToString()) ? "1" : result.Rows[0]["report_status"].ToString();

                    // ตรวจสอบว่ามีค่าใดเป็น 6 (Pending) หรือ 0 (NG) หรือไม่
                    if (report_status == ((int)ProcStatus.Pending).ToString() || report_status == ((int)ProcStatus.NG).ToString())
                    {
                        bl = false;
                    }
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    bl = false; // คืนค่า false หากเกิดข้อผิดพลาดจากฐานข้อมูล
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                bl = false; // คืนค่า false หากเกิด exception
            }

            return bl;
        }

        public string ReportFDA_Status(QAdataProperty dataItem)
        {
            string report_status = "";
            string statusF = ((int)ProcStatus.OK).ToString();
            string statusD = ((int)ProcStatus.OK).ToString();
            string statusA = ((int)ProcStatus.OK).ToString();
            DataTable result = new DataTable();

            try
            {
                _resultData = _model.ReportFDA_Status(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;

                    statusF = string.IsNullOrEmpty(result.Rows[0]["Function_Check"].ToString()) ? ((int)ProcStatus.OK).ToString() : result.Rows[0]["Function_Check"].ToString();
                    statusD = string.IsNullOrEmpty(result.Rows[0]["Dimension_Check"].ToString()) ? ((int)ProcStatus.OK).ToString() : result.Rows[0]["Dimension_Check"].ToString();
                    statusA = string.IsNullOrEmpty(result.Rows[0]["Appearance_Check"].ToString()) ? ((int)ProcStatus.OK).ToString(): result.Rows[0]["Appearance_Check"].ToString();
                }
                else
                {
                    MessageBox.Show(_resultData.MessageOnDb, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return report_status; // Return empty string if database error
                }

                // Rule 1: If any status is Pending (6), return 6
                if (statusF == ((int)ProcStatus.Pending).ToString() || statusD == ((int)ProcStatus.Pending).ToString() || statusA == ((int)ProcStatus.Pending).ToString())
        {
                    report_status = ((int)ProcStatus.Pending).ToString();
                }
                // Rule 2: If any status is Working (2), return 2
                else if (statusF == ((int)ProcStatus.Working).ToString() || statusD == ((int)ProcStatus.Working).ToString() || statusA == ((int)ProcStatus.Working).ToString())
                {
                    report_status = ((int)ProcStatus.Working).ToString();
                }
                // Rule 3: If all statuses are OK (1) or Skip (3), return 1
                else if ((statusF == ((int)ProcStatus.OK).ToString() || statusF == ((int)ProcStatus.Skip).ToString()) &&
                         (statusD == ((int)ProcStatus.OK).ToString() || statusD == ((int)ProcStatus.Skip).ToString()) &&
                         (statusA == ((int)ProcStatus.OK).ToString() || statusA == ((int)ProcStatus.Skip).ToString()))
                {
                    report_status = ((int)ProcStatus.OK).ToString();
                }
                // Default case: Return empty string for unhandled combinations
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return report_status;
        }

        public DataTable DimensionSampling(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.DimensionSampling(dataItem);
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
            }
            return result;
        }

        public DataTable DimensionEquipment(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.DimensionEquipment(dataItem);
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
            }
            return result;
        }

        public DataTable SearchForDimensionPending(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForDimensionPending(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchDimensionDataPending(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchDimensionDataPending(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchForOpDimension(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForOpDimension(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable DimensionSampQtyLotSize(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.DimensionSampQtyLotSize(dataItem);
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
            }
            return result;
        }


        public Boolean InsertDimensionData(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertDimensionData(dataItem);
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

        public int NeedDimensionCheck(QAdataProperty dataItem)
        {
            try
            {
                _resultData = _model.NeedDimensionCheck(dataItem);

                if (!_resultData.StatusOnDb)
                {
                    throw new Exception(_resultData.MessageOnDb); // Throw เดียวพอ
                }

                DataTable result = _resultData.ResultOnDb;

                // ตรวจสอบความปลอดภัยก่อนเข้าถึงข้อมูล
                if (result == null || result.Rows.Count == 0)
                {
                    throw new Exception("ไม่พบข้อมูลในตารางผลลัพธ์");
                }

                if (!result.Columns.Contains("Dimension_Check_Need"))
                {
                    throw new Exception("ไม่พบคอลัมน์ 'Dimension_Check_Need' ในผลลัพธ์");
                }

                return Convert.ToInt32(result.Rows[0]["Dimension_Check_Need"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

        }

        public Boolean UpdateReportStatus(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.UpdateReportStatus(dataItem);
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

        public DataTable SearchForOpData(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForOpData(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public Boolean InsertUpdateInspData(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertUpdateInspData(dataItem);
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

        public DataTable SearchForInspDataPending(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForInspDataPending(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchDataInspDataPending(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchDataInspDataPending(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public DataTable SearchForOpAppear(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForOpAppear(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    result = _resultData.ResultOnDb;
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
            return result;
        }

        public int NeedAppearCheck(QAdataProperty dataItem)
        {
            try
            {
                _resultData = _model.NeedAppearCheck(dataItem);

                if (!_resultData.StatusOnDb)
                {
                    throw new Exception(_resultData.MessageOnDb); // Throw เดียวพอ
                }

                DataTable result = _resultData.ResultOnDb;

                // ตรวจสอบความปลอดภัยก่อนเข้าถึงข้อมูล
                if (result == null || result.Rows.Count == 0)
                {
                    throw new Exception("ไม่พบข้อมูลในตารางผลลัพธ์");
                }

                if (!result.Columns.Contains("Appearance_Check_Need"))
                {
                    throw new Exception("ไม่พบคอลัมน์ 'Appearance_Check_Need' ในผลลัพธ์");
                }

                return Convert.ToInt32(result.Rows[0]["Appearance_Check_Need"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

        }

        public DataTable AppearSampQtyLotSize(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.AppearSampQtyLotSize(dataItem);
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
            }
            return result;
        }

        public DataTable AppearSampling(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.AppearSampling(dataItem);
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
            }
            return result;
        }

        public Boolean UpdateInspQtyAppear(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.UpdateInspQtyAppear(dataItem);
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

        public DataTable SearchPackingSize(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchPackingSize(dataItem);
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
            }
            return result;
        }

        public DataTable SearchAppearData(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchAppearData(dataItem);
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
            }
            return result;
        }

        public DataTable SearchSampleSize(QAdataProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchSampleSize(dataItem);
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
            }
            return result;
        }


        public Boolean InsertAppearPendingDetail(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertAppearPendingDetail(dataItem);
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


        public Boolean UpdateAppearPendingReview(QAdataProperty dataItem)
        {
            Boolean bl = false;
            try
            {
                _resultData = _model.UpdateAppearPendingReview(dataItem);
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
        public DataTable SearchAppearPendingData(QAdataProperty dataItem)
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchAppearPendingData(dataItem);
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
            }
            return result;
        }

        public DataTable SearchForAppearPending()
        {
            DataTable result = new DataTable();
            try
            {
                _resultData = _model.SearchForAppearPending();
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
            }
            return result;
        }
        public int GetTotalInspected(QAdataProperty dataItem)
        {
            try
            {
                _resultData = _model.GetTotalInspected(dataItem);

                if (!_resultData.StatusOnDb)
                {
                    throw new Exception(_resultData.MessageOnDb); // Throw เดียวพอ
                }

                DataTable result = _resultData.ResultOnDb;

                // ตรวจสอบความปลอดภัยก่อนเข้าถึงข้อมูล
                if (result == null || result.Rows.Count == 0)
                {
                    throw new Exception("ไม่พบข้อมูลในตารางผลลัพธ์");
                }

                if (!result.Columns.Contains("sum(QTY_SELECT)"))
                {
                    throw new Exception("ไม่พบคอลัมน์ 'sum(QTY_SELECT)' ในผลลัพธ์");
                }

                return Convert.ToInt32(result.Rows[0]["sum(QTY_SELECT)"]);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }

        }

        public Boolean InsertAppearData(QAdataProperty dataItem)
        {

            Boolean bl = false;
            try
            {
                _resultData = _model.InsertAppearData(dataItem);
                if (_resultData.StatusOnDb == true)
                {
                    bl = _resultData.StatusOnDb;
                    _resultData = _model.GetLatestAppearDataId(dataItem);
                    if (_resultData.StatusOnDb == true
                        && _resultData.ResultOnDb != null
                        && _resultData.ResultOnDb.Rows.Count > 0
                        && _resultData.ResultOnDb.Columns.Contains("APPEARANCE_ID"))
                    {
                        dataItem.APPEARANCE_ID = _resultData.ResultOnDb.Rows[0]["APPEARANCE_ID"].ToString();
                    }
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

    }
}
