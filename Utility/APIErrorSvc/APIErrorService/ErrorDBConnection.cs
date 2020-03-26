using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Diagnostics;
namespace BC.Integration.APIError
{
    public class ErrorDBConnection
    {
        private OdbcConnection connection;

        #region DB Connection

        //Constructor
        public ErrorDBConnection()
        {
            Initialize();
        }

        public void Initialize()
        {
            try
            {
                string connectionString;
                connectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnectionString"];
                connection = new OdbcConnection(connectionString);

            }
            catch (SqlException ex)
            {
                Trace.WriteLine("Corp.Integration.AppService.APIError service exception occurred. Exception message: " + ex.Message);
                throw new Exception("Exception* occured while initializing ErrorDBConnection", ex);
            }
        }

        public bool Open()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                Trace.WriteLine("Corp.Integration.AppService.APIError.OpenConnection method exception occurred. Exception message: " + ex.Message);
                return false;
            }
        }

        public bool Close()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (SqlException ex)
            {
                Trace.WriteLine("Corp.Integration.AppService.APIError.CloseConnection method exception occurred. Exception message: " + ex.Message);
                return false;
            }
        }

        public OdbcConnection GetConnection()
        {
            return connection;
        }
        #endregion

    }
}