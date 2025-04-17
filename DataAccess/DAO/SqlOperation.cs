using Microsoft.Data.SqlClient;

namespace DataAccess.DAO
{
    public class SqlOperation
    {
        public string procedureName { get; set; }
        public List<SqlParameter> parameters;

        public SqlOperation()
        {
            parameters = new List<SqlParameter>();
        }

        public void AddVarcharParameter( string paramName, string paramValue)
        {
            parameters.Add(new SqlParameter("@" + paramName, paramValue));
        }

        public void AddIntegerParameter(string paramName, int paramValue)
        {
            parameters.Add(new SqlParameter("@" + paramName, paramValue));
        }

        public void AddDateTimeParameter(string paramName, DateTime paramValue)
        {
            parameters.Add(new SqlParameter("@" + paramName, paramValue));
        }

        public void AddDoubleParameter(string paramName, Double paramValue)
        {
            parameters.Add(new SqlParameter("@" + paramName, paramValue));
        }

        public void AddBooleanParameter(string paramName, bool paramValue)
        {
            parameters.Add(new SqlParameter("@" + paramName, paramValue));
        }
    }
}
