using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using PronesConnection.Property;
using Oracle.ManagedDataAccess.Client;

namespace PronesConnection.Models
{
    public class TransMysqlModels
    {
        private OracleConnection conTrans;
        private OracleCommand userCommand = new OracleCommand();
        private OracleTransaction trans;
        public static string PronesConnectionString;
        private OutputOnDbProperty dataOutPut;

        protected TransMysqlModels()
        {
            PronesConnectionString = "Data Source = (DESCRIPTION = (ADDRESS_LIST = (ADDRESS = (PROTOCOL = TCP)(HOST = 192.168.0.101)(PORT = 1521)))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = PRONES))); User Id = FFT_USER; Password = fS5pjWXGh; ";
        }

        protected OutputOnDbProperty TransConnection()
        {
            try
            {
                conTrans = new OracleConnection(PronesConnectionString);
                conTrans.Open();
                trans = conTrans.BeginTransaction(IsolationLevel.ReadCommitted);     //*** Start  
                userCommand.Connection = conTrans;
                userCommand.Transaction = trans;  //*** Command & Transaction ***// 

                dataOutPut = new OutputOnDbProperty
                {
                    StatusOnDb = true,
                    MessageOnDb = "Connect Database Success",
                    ClassOnDb = "Class Name: TransOracleModels",
                    MethodOnDb = "Method Name: TransConnection",
                    ResultOnDb = null
                };
            }
            catch (Exception er)
            {
                dataOutPut = new OutputOnDbProperty
                {
                    StatusOnDb = false,
                    ClassOnDb = "Class Name: TransOracleModels",
                    MethodOnDb = "Method Name: TransConnection",
                    MessageOnDb = "Connect Database Error ==> " + er.ToString(),
                    ResultOnDb = null
                };
            }

            return dataOutPut;
        }

        protected OutputOnDbProperty TransSelectCommand(string sql)
        {
            OracleConnection connecter = new OracleConnection(PronesConnectionString);

            DataTable tableResult = new DataTable();

            try
            {
                connecter.Open();

                OracleDataAdapter userDataAdaptor = new OracleDataAdapter(sql, connecter);

                userDataAdaptor.Fill(tableResult);

                connecter.Close();
                connecter.Dispose();

                dataOutPut = new OutputOnDbProperty
                {
                    StatusOnDb = true,
                    ClassOnDb = "Class Name: TransOracleModels",
                    MethodOnDb = "Method Name: TransSelectCommand",
                    MessageOnDb = "",
                    ResultOnDb = tableResult
                };

            }
            catch (Exception er)
            {
                dataOutPut = new OutputOnDbProperty
                {
                    StatusOnDb = false,
                    ClassOnDb = "Class Name: TransOracleModels",
                    MethodOnDb = "Method Name: TransSelectCommand",
                    MessageOnDb = "Select SQL Error ==> " + er.ToString() + "<br>" + sql,
                    ResultOnDb = null
                };
            }

            return dataOutPut;
        }



        protected OutputOnDbProperty TransExecuteCommand(string sql)
        {
            try
            {
                userCommand.CommandText = sql;
                userCommand.CommandType = CommandType.Text;
                userCommand.ExecuteNonQuery();

                dataOutPut = new OutputOnDbProperty
                {
                    StatusOnDb = true,
                    ClassOnDb = "Class Name: TransOracleModels",
                    MethodOnDb = "Method Name: TransExecuteCommand",
                    MessageOnDb = "",
                    ResultOnDb = null
                };
            }
            catch (Exception er)
            {
                dataOutPut = new OutputOnDbProperty
                {
                    StatusOnDb = false,
                    ClassOnDb = "Class Name: TransOracleModels",
                    MethodOnDb = "Method Name: TransExecuteCommand",
                    MessageOnDb = "Execute SQL Command Error ==> " + er.ToString(),
                    ResultOnDb = null
                };
            }

            return dataOutPut;
        }

        protected void TransCommit()
        {
            trans.Commit();
            conTrans.Close();
            conTrans.Dispose();
        }

        protected void TransRolback()
        {
            trans.Rollback();
            conTrans.Close();
            conTrans.Dispose();
        }

        protected void TransClose()
        {

            conTrans.Close();
            conTrans.Dispose();
        }

        protected string GetDBName()
        {
            string result = "";
            if (conTrans != null)
            {
                result = conTrans.DataSource;
            }
            return result;
        }

    }
}