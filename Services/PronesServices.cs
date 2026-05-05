
using RawMat.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PronesConnection.Interface;
using PronesConnection.Property;
using RawMat.SQLFactory;
using System.Configuration;

namespace RawMat.Services
{
    public class PronesServices : DatabaseAction<PronesProperty>
    {

        OutputOnDbProperty resultData = new OutputOnDbProperty();
        PronessSQL sqlFactory = new PronessSQL();

        public static string strConnection;
        private string sql;

        public List<PronesProperty> _listDB = new List<PronesProperty>();

        public OutputOnDbProperty SearchPrones(PronesProperty dataItem)
        {
            sql = sqlFactory.SearchPrones(dataItem);
            resultData = base.SearchBySql(sql);
            return resultData;
        }

        public OutputOnDbProperty SearchRecDate(PronesProperty dataItem)
        {
            sql = sqlFactory.SearchRecDate(dataItem);
            resultData = base.SearchBySql(sql);
            return resultData;
        }



        public override OutputOnDbProperty Delete(PronesProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Insert(PronesProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search(PronesProperty dataItem)
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Search()
        {
            throw new NotImplementedException();
        }

        public override OutputOnDbProperty Update(PronesProperty dataItem)
        {
            throw new NotImplementedException();
        }
    }
}
