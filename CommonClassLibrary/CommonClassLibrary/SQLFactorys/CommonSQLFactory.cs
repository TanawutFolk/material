namespace CommonClassLibrary.SQLFactorys
{
    public class CommonSQLFactory
    {
        string sql = "";
        public string getDateTimeNow()
        {
            sql = "select date_format(now(), '%Y-%m-%d %H:%i:%s')";
            return sql;
        }

        public string getDateTimeNowByFormat(string formatDatetime)
        {
            sql = "select date_format(now(), 'dataItem.formatDatetime')";
            sql = sql.Replace("dataItem.formatDatetime", formatDatetime);
            return sql;
        }

        public string getDateTimeNowYMD()
        {
            sql = "select date_format(now(), '%Y%m%d')";
            return sql;
        }

        public string getYearNow(int countYear)
        {
            if (countYear == 2)
            {
                sql = "select date_format(now(), '%y')";
            }
            else if (countYear == 4)
            {
                sql = "select date_format(now(), '%Y')";
            }

            return sql;
        }

    }
}