using CommonClassLibrary.Propertys;

namespace CommonClassLibrary.SQLFactorys
{
    public class EmployeeSQLFactory
    {
        string sql = "";
        string tableName = CommonClassLibraryGlobal.DB_OPERATOR;

        public string SearchOne(EmployeeProperty dataItem)
        {
            sql = @"SELECT * FROM tableName 
                             where EmpCode = 'dataItem.EmpCode' 
                             AND EmpCodeResign is null ";

            sql = sql.Replace("tableName", tableName);
            sql = sql.Replace("dataItem.EmpCode", dataItem.EmpCode);
            return sql;
        }
    }
}