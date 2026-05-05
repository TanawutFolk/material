using RawMat.Property;
using RawMat.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PronesConnection.Property;


namespace RawMat.Models
{
    public class PronesModels
    {
        OutputOnDbProperty resultData = new OutputOnDbProperty();
        PronesServices services = new PronesServices();

        public OutputOnDbProperty SearchPrones(PronesProperty dataItem)
        {
            resultData = services.SearchPrones(dataItem);
            return resultData;
        }

        public OutputOnDbProperty SearchRecDate(PronesProperty dataItem)
        {
            resultData = services.SearchRecDate(dataItem);
            return resultData;
        }

       
    }
}
