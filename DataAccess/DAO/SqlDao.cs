using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.DAO
{
    public class SqlDao
    {
        private readonly string _connectionString;

        private SqlDao()
        {
            // Get connection string 
            _connectionString = "Server=tcp:employee-perks-wilmer-server.database.windows.net,1433;Initial Catalog=Proyecto_Software_2_DB;Persist Security Info=False;User ID=lavarus;Password=W.w72v8siFVP7..;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=300;";//"Server=localhost;Database=master;Trusted_Connection=True;TrustServerCertificate=True;";//Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");

            // Fallback 
            if (string.IsNullOrEmpty(_connectionString))
            {
                _connectionString = "Server=localhost; Database=Proyecto_Software_2_DB; Trusted_Connection=True; TrustServerCertificate=true";
                Console.WriteLine("Warning: Using default connection string. Set DB_CONNECTION_STRING environment variable for production.");
            }
        }


        #region Singleton
        private static SqlDao instance;

        public static SqlDao GetInstance()
        {
            if (instance == null)
                instance = new SqlDao();
            return instance;
        }

        #endregion


        public void ExcecuteStoredProcedure(SqlOperation operation)
        {
            //crear conexion 
            SqlConnection conn = new SqlConnection(_connectionString);

            //armar query
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = operation.procedureName;

            //agregar parametros
            foreach (var p in operation.parameters)
            {
                command.Parameters.Add(p);
            }

            //abrir conexion
            conn.Open();
            //ejecutar query
            command.ExecuteNonQuery();
            //cerrar conexion
            conn.Close();

        }

        //R ead
        public List<Dictionary<string, object>> ExecuteStoredProcedureWithQuery(SqlOperation operation)
        {

            List<Dictionary<string, object>> listResults = new List<Dictionary<string, object>>();

            SqlConnection conn = new SqlConnection(_connectionString);
            SqlCommand command = new SqlCommand();
            command.Connection = conn;
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = operation.procedureName;


            //agregar parametros
            foreach (var p in operation.parameters)
            {
                command.Parameters.Add(p);
            }

            //abrir conexion
            conn.Open();

            SqlDataReader reader = command.ExecuteReader();

            //recorrer el reader y construir el diccionario
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    Dictionary<string, object> dictObj = new Dictionary<string, object>();

                    //Construir matriz fila por fila

                    for (var fieldCount = 0; fieldCount < reader.FieldCount; fieldCount++)
                    {
                        dictObj.Add(reader.GetName(fieldCount), reader.GetValue(fieldCount));
                    }
                    listResults.Add(dictObj);
                }
            }

            //cerrar conexion
            conn.Close();

            return listResults;

        }

    }
}
