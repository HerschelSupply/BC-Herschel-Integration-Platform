using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Data;
using System.Diagnostics;
using System.Configuration;
using Corp.Integration.Utility;

namespace Corp.Instrumentation.ServerMgmt
{
    class AuroraConnector
    {
        private string connString = ConfigurationManager.AppSettings["ConnectionString"];
        private string serverIp = Utilities.GetServerIpAddress();

        public void RecordServerActivation()
        {
            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ServerStatusLogActivation ('" + Environment.MachineName + "', '" + serverIp + "')";
                cmd.Connection.Open();
                int rowcount = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured while logging server actiation in the AuroraConnector.RecordServerActivation method.", ex);
            }
            finally
            {
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }

        public void RecordServerDeactivation(string targetServerIp)
        {
            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ServerStatusLogDeactivation ('" + Environment.MachineName + "', '" + targetServerIp + "')";
                cmd.Connection.Open();
                int rowcount = cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured while logging server deactiation in the AuroraConnector.RecordServerDeactivation method.", ex);
            }
            finally
            {
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }

        public List<string> GetActiveServers(string serverIp, out bool isCurrentServerMarkedActive)
        {
            isCurrentServerMarkedActive = false;
            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            OdbcDataReader dataReader = null;
            List<string> activeServers = new List<string>();
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ServerStatusGetActiveServers";
                cmd.Connection.Open();
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    int i = 0;
                    while (dataReader.Read())
                    {
                        string ip = dataReader["ServerIp"].ToString();
                        if (ip != serverIp)
                        {
                            activeServers.Add(ip);
                        }
                        else
                        {
                            isCurrentServerMarkedActive = true;
                        }
                        i++;
                    }
                }
                dataReader.Close();
                return activeServers;
            }
            catch (Exception ex)
            {
                throw new Exception("Exception occured while retrieving the active server IP's in the AuroraConnector.GetActiveServers method.", ex);
            }
            finally
            {
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }

        public void LogGeneralException(string exceptionMessage, Exception ex)
        {
            string exceptionMsg = "";
            string stackTrace = "";
            if (ex != null)
            {
                exceptionMsg = "\r\n" + exceptionMessage + "\r\nCaught Exception Message:\r\n" + ex.Message + "\r\nInner Exceptions:\r\n";
                while (ex.InnerException != null)
                {
                    exceptionMsg += ex.InnerException.Message + "\r\n";
                    ex = ex.InnerException;
                }
                if (ex.StackTrace != null) //Stack trace can still be null even if the exception is not.
                {
                    stackTrace = ex.StackTrace;
                }
            }

            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                //If there are single quotes in the strings it will cause the exception logging to fail.
                if (exceptionMsg != null)
                {
                    exceptionMsg = exceptionMsg.Replace("'", "");
                }
                else
                {
                    exceptionMsg = "No exception message available";
                }
                if (stackTrace != null)
                {
                    stackTrace = stackTrace.Replace("'", "");
                }
                else
                {
                    stackTrace = "No stack trace available";
                }

                cmd.Connection = conn;
                cmd.CommandText = "Call ExceptionInsert ('HipSvrMgmtSvc', '', '', '" +
                                                        "', '" + exceptionMsg +
                                                        "', '" + stackTrace +
                                                        "', '" + Environment.MachineName + "-" + serverIp + "')";
                cmd.Connection.Open();
                int rowcount = cmd.ExecuteNonQuery();


            }
            catch (Exception exc)
            {
                throw new Exception("Exception occured while logging an exception to the HIP exception table using the  Corp.Instrumentation.ServerMgmt.AuroraConnector.LogGeneralException (HIP Svr Mgmt Svc) method.", exc);
            }
            finally
            {
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
        }


    }
}
