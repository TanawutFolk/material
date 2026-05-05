using BusinessData.Property;
using CommonClassLibrary.Models;
using System;
using System.Windows.Forms;

namespace CommonClassLibrary.Controllers
{
    public class CommonController
    {
        OutputOnDbProperty resultData = new OutputOnDbProperty();
        CommonModel model = new CommonModel();

        public string getDateTimeNow()
        {
            string result = "";
            try
            {
                resultData = model.getDateTimeNow();
                if (resultData.StatusOnDb == true)
                {
                    if (resultData.ResultOnDb.Rows.Count > 0)
                    {
                        result = resultData.ResultOnDb.Rows[0][0].ToString();
                    }
                }
                else
                {
                    MessageBox.Show(resultData.MessageOnDb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        public DateTime getDateTimeNowTypeDateTime()
        {
            DateTime _result = DateTime.Now;
            string _datetimenow = "";
            try
            {
                resultData = model.getDateTimeNow();
                if (resultData.StatusOnDb == true)
                {
                    if (resultData.ResultOnDb.Rows.Count > 0)
                    {
                        _datetimenow = resultData.ResultOnDb.Rows[0][0].ToString();
                        _result = Convert.ToDateTime(_datetimenow);
                    }
                }
                else
                {
                    MessageBox.Show(resultData.MessageOnDb, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return _result;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return _result;
            }
        }

        public string getDateTimeNowByFormat(string formatDatetime)
        {
            string result = "";

            try
            {
                resultData = model.getDateTimeNowByFormat(formatDatetime);
                if (resultData.StatusOnDb == true)
                {
                    if (resultData.ResultOnDb.Rows.Count > 0)
                    {
                        result = resultData.ResultOnDb.Rows[0][0].ToString();
                    }
                }
                else
                {
                    MessageBox.Show(resultData.MessageOnDb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        public string getDateTimeNowYMD()
        {
            string result = "";
            try
            {
                resultData = model.getDateTimeNowYMD();
                if (resultData.StatusOnDb == true)
                {
                    if (resultData.ResultOnDb.Rows.Count > 0)
                    {
                        result = resultData.ResultOnDb.Rows[0][0].ToString();
                    }
                }
                else
                {
                    MessageBox.Show(resultData.MessageOnDb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }

        public string getYearNow(int countYear)
        {
            string result = "";
            try
            {
                resultData = model.getYearNow(countYear);
                if (resultData.StatusOnDb == true)
                {
                    if (resultData.ResultOnDb.Rows.Count > 0)
                    {
                        result = resultData.ResultOnDb.Rows[0][0].ToString();
                    }
                }
                else
                {
                    MessageBox.Show(resultData.MessageOnDb);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            return result;
        }

    }
}