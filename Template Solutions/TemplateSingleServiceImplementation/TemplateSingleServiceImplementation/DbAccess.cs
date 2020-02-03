using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Configuration;
using System.Data.Odbc;

namespace BC.Integration.AppService.ProjectName.TemplateProject
{
    public static class DbAccess
    {
        private static string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private static string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
        /// <summary>
        /// Sample DB Access method.....
        /// </summary>
        /// <param name="procName">The proc name.</param>
        /// <returns></returns>
        public static List<DateTime> GetDates(string procName)
        {
            List<DateTime> dates = new List<DateTime>();
            string connString;
            try
            {
                connString = ConfigurationManager.AppSettings["SourceConnectivity"];
                Trace.WriteLine(tracingPrefix + "GetDates, connection string value: '" + connString + "'.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + "BC.Integration.AppService.ProjectName.TemplateProject.GetDates could not connect to the DB as the" +
                    " AggregationSourceConnectivity appsetting is not set in the configuration file.");
                throw new Exception("BC.Integration.AppService.ProjectName.TemplateProject.GetDates could not connect to the DB as the" +
                    " AggregationSourceConnectivity appsetting is not set in the configuration file.", ex);
            }
            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            OdbcDataReader dataReader = null;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call " + procName;
                cmd.Connection.Open();
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    while(dataReader.Read())
                    {
                        dates.Add(Convert.ToDateTime(dataReader[0]));
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + "Exception occurred trying to connect to the DB BC.Integration.AppService.ProjectName.TemplateProject.GetDates. Exception message: " + ex.Message);
                throw new Exception("Exception occurred trying to connect to the DB BC.Integration.AppService.ProjectName.TemplateProject.GetDates. Exception message: ", ex);
            }
            finally
            {
                dataReader.Close();
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
            return dates;
        }


    }
}