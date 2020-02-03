using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Diagnostics;
using System.Configuration;

namespace BC.Integration.AppService.TemplateHipSingletonOnRamp
{
    class DbAccess
    {
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
        #region External Doc Tracking DB calls

        /// <summary>
        /// This method is used to return the doc id and completed timestamp for the last doc that was successfully processed.
        /// </summary>
        /// <param name="connectionString">DB connection string</param>
        /// <param name="processName">Process name</param>
        /// <param name="docType">Doc Type ('Sales', 'Inventory', ...)</param>
        /// <returns>string containing 'Doc ID | Doc Timestamp'</returns>
        public string GetLastDocProcessed(string connectionString, string processName, string docType)
        {
            string connString;
            connString = connectionString;
            Trace.WriteLine(tracingPrefix + "GetLastDocProcessed method connection string value: '" + connString + "'.");
            
            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            OdbcDataReader dataReader = null;
            string lastOrderTimestamp = "";
            string lastOrderId = "";

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ExternalDocTrackingGet ( '" + processName + "', '" + docType + "')";
                cmd.Connection.Open();
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    lastOrderTimestamp = dataReader["LastDocTimestamp"].ToString();
                    lastOrderId = dataReader["LastDocId"].ToString();
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + "Exception occurred trying to connect to the DB BC.Integration.AppService.TemplateHipSingletonOnRamp.DbAccess.GetLastDocProcessed. Exception message: " + ex.Message);
                throw new Exception("Exception occurred trying to connect to the DB BC.Integration.AppService.TemplateHipSingletonOnRamp.DbAccess.GetLastDocProcessed. Exception message: ", ex);
            }
            finally
            {
                dataReader.Close();
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
            Trace.WriteLine(tracingPrefix + "GetLastDocProcessed method method returned: " + lastOrderId + "|" + lastOrderTimestamp);

            return lastOrderId + "|" + lastOrderTimestamp;
        }

        /// <summary>
        /// Used to update the last doc data for a specified process name and doc type.
        /// </summary>
        /// <param name="connectionString">DB connection string</param>
        /// <param name="processName">process name</param>
        /// <param name="docType">Doc Type ('Sales', 'Inventory', ...)</param>
        /// <param name="lastDocId">Doc ID to be used to update the record</param>
        /// <param name="lastDocTimestamp">Date to be used to update the record</param>
        public void UpdateLastDocProcessed(string connectionString, string processName, string docType, string lastDocId, string lastDocTimestamp)
        {
            string connString;
            connString = connectionString;
            Trace.WriteLine(tracingPrefix + "UpdateLastDocProcessed method connection string value: '" + connString + "'.");

            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ExternalDocTrackingUpdate ( '" + processName + "', '" + docType + "', '" + 
                                                                        lastDocId + "', '" + lastDocTimestamp +"')";
                cmd.Connection.Open();
                int rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + "Exception occurred trying to connect to the DB BC.Integration.AppService.TemplateHipSingletonOnRamp.DbAccess.UpdateLastDocProcessed. Exception message: " + ex.Message);
                throw new Exception("Exception occurred trying to connect to the DB BC.Integration.AppService.TemplateHipSingletonOnRamp.DbAccess.UpdateLastDocProcessed. Exception message: ", ex);
            }
            finally
            {
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }

        public void InsertExternalDocPendingProcessing(string connectionString, string processName, string docType, string docId,
                                                        string message, string exceptionMessage)
        {
            string connString;
            message = message.Replace("\'", "`");
            exceptionMessage = exceptionMessage.Replace("\'", "`");
            connString = connectionString;
            Trace.WriteLine(tracingPrefix + "InsertExternalDocPendingProcessing method connection string value: '" + connString + "'.");

            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ExternalDocPendingProcessingInsert ( '" + processName + "', '" + docId + "', '" + docType + "', '" + message + "', '" + exceptionMessage + "')";
                cmd.Connection.Open();
                int rows = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + "Exception occurred trying to connect to the DB BC.Integration.AppService.TemplateHipSingletonOnRamp.DbAccess.InsertExternalDocPendingProcessing. Exception message: " + ex.Message);
                throw new Exception("Exception occurred trying to connect to the DB BC.Integration.AppService.TemplateHipSingletonOnRamp.DbAccess.InsertExternalDocPendingProcessing.", ex);
            }
            finally
            {
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }



        #endregion
    }
}
