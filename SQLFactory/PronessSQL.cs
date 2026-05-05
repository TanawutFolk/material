using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RawMat.Property;

namespace RawMat.SQLFactory
{
    public class PronessSQL
    {
        private string sql;
        public string SearchPrones(PronesProperty dataItem)
        {
            //I_model คือ mathName
            sql = @"
                SELECT 
                        T_PM_MS.I_ITEM_CD, 
                        T_PM_MS.I_ITEM_DESC, 
                        T_PM_MS.I_DRW_NO, 
                        T_PM_MS.I_MODEL
                FROM FFT.T_PM_MS

                LEFT JOIN FFT.V_UNIT_MS
                ON I_UNIT_CD = T_PM_MS.I_CNV_UNIT_CD

                WHERE T_PM_MS.I_ITEM_CD = 'dataItem.M_CODE'";


            sql = sql.Replace("dataItem.M_CODE", dataItem.M_CODE);

            return sql;
        }

        //
        public string SearchRecDate(PronesProperty dataItem)
        {
            //I_model คือ mathName
            sql = @"
                SELECT RTRIM(T_ACP_TR.I_ITEM_CD) Item_Cd,RTRIM(T_ACP_TR.I_INV_NO) Invoice_No,
                    T_ACP_TR.I_ITEM_DESC Item_Desc,VENDOR.I_DL_ARG_DESC DL_Desc, T_ACP_TR.I_ACP_QTY GR_Qty
                    FROM 
                    FFT.T_ACP_TR T_ACP_TR
                    LEFT JOIN FFT.T_TRADE_MS VENDOR ON VENDOR.I_DL_TYPE = '03' AND T_ACP_TR.I_IND_DEST_CD = VENDOR.I_DL_CD
                    WHERE
                    T_ACP_TR.I_PO_CLS = '00' 
                    AND TO_CHAR( T_ACP_TR.I_ACP_DATE, 'YYYY-MM-DD' ) = 'dataItem.rec_date'
                    ";


            sql = sql.Replace("dataItem.rec_date", dataItem.rec_date);

            return sql;
        }

     

    }
}
