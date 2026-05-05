using RawMat.Models;
using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PronesConnection.Property;
using System.Data;
using System.Windows.Forms;

namespace RawMat.Controllers
{
    public class PronesControllers
    {
        OutputOnDbProperty resultData = new OutputOnDbProperty();
        PronesModels models = new PronesModels();
        PronesProperty _conn = new PronesProperty();


        public DataTable SearchMath(PronesProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                resultData = models.SearchPrones(dataItem);
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

        public DataTable SearchRecDate(PronesProperty dataItem)
        {

            DataTable result = new DataTable();
            try
            {
                resultData = models.SearchRecDate(dataItem);
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

      


    }
}
