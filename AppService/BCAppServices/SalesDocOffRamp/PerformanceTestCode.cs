using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Odbc;
using System.Diagnostics;
using BC.Integration.Utility;

namespace BC.Integration.AppService.BC
{
    public class PerfTest
    {
        private List<KeyValuePair<string, string>> configuration = null;
        private string connection = "";
        private string level = "";
        private int retryInterval = 0;
        private int retryCount = 0;
        private Exception retryExceptions = null;
        private bool tracingEnabled = false;

        /// <summary>
        /// Class constructor used to pass configuration data.
        /// </summary>
        /// <param name="configuration">Collection of configuration data needed to connect to and manage the database interaction.</param>
        public PerfTest(List<KeyValuePair<string, string>> configuration)
        {
            this.configuration = configuration;
            try
            {
                connection = Utilities.GetConfigurationValue(configuration, "InstrumentationConn");
                level = Utilities.GetConfigurationValue(configuration, "InstrumentationLevel");
                retryCount = Convert.ToInt16(Utilities.GetConfigurationValue(configuration, "DatabaseRetryCount"));
                retryInterval = Convert.ToInt16(Utilities.GetConfigurationValue(configuration, "DatabaseRetryInterval"));
                string val = Utilities.GetConfigurationValue(configuration, "TracingEnabled");
                if (val != "")
                {
                    if (val.ToLower() == "true" || val.ToLower() == "false")
                        tracingEnabled = Convert.ToBoolean(val);
                    if (val == "1" || val == "0")
                        tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in the AuroraConnector constructor while trying to set the configuration values.", ex);
            }
        }


        public void LogMsgGuid(Guid msgId)
        {
            OdbcConnection conn = new OdbcConnection(connection);
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call PerfTestInsert ('" + msgId.ToString() + "')";
                int rowcount = 0;
                bool isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                if (!isSuccess)
                    throw new Exception("Failed to insert PertTest record after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                if (rowcount != 1)
                    throw new Exception("Failed to insert PertTest record.  The query reported " + rowcount + " rows were updated.");
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred in the BC.Integration.AppService.BC.PerfTest.LogMsgGuid() with a parameter: " + msgId.ToString(), ex);
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
        }

        /// <summary>
        /// Wraps the ODBC command ExecuteNonQuery call to Aurora to return the values needed for retry logic
        /// </summary>
        /// <param name="cmd">Pre configured command object to use to run nonquery command</param>
        /// <param name="rowcount">Success or failure of the query call. Did one row get updated?</param>
        /// <returns>Bool to identify if the call was successfully made without exception.</returns>
        private bool ExecuteNonQuery(OdbcCommand cmd, out int rowcount)
        {
            try
            {
                cmd.Connection.Open();
                rowcount = cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception occurred trying to connect to the DB BC.Integration.AppService.BC.PerfTest.ExecuteNonQuery");
                if (retryExceptions == null)
                    retryExceptions = new Exception("Error tring to insert data into the DB (BC.Integration.AppService.BC.PerfTest.ExecuteNonQuery)", ex);
                else
                    retryExceptions = new Exception(ex.Message, retryExceptions);
                rowcount = 0;
                return false;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

    }
}