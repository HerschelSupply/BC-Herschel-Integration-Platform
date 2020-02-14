using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace BC.Integration.Utility.NotificationSvc
{
    public class NotificationSvcDbConnection
    {
        private OdbcConnection connection;
        private List<NotificationLog> unProcessedNotificationLogs;
        private List<NotificationLog> processedNotificationLogs;
        List<string> notificationConfiguration;

        #region DB Connection

        //Constructor
        public NotificationSvcDbConnection()
        {
            Initialize();
        }

        //InitializeNotification values
        private void Initialize()
        {
            try
            {
                string connectionString;
                connectionString = System.Configuration.ConfigurationManager.AppSettings["DBConnectionString"];
                connection = new OdbcConnection(connectionString);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BC.Integration.Utility.NotificationSvc.NotificationSvcDbConnection service exception occurred. Exception message: " + ex.Message);
                throw new Exception("Exception* occured while initializing NotificationSvcDbConnection", ex);
            }
        }

        //open connection to database
        private bool OpenConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (OdbcException ex)
            {
                Trace.WriteLine("BC.Integration.Utility.NotificationSvc.NotificationSvcDbConnection.OpenConnection method exception occurred. Exception message: " + ex.Message);
                return false;
            }
        }

        //Close connection
        private bool CloseConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (OdbcException ex)
            {
                Trace.WriteLine("BC.Integration.Utility.NotificationSvc.NotificationSvcDbConnection.CloseConnection method exception occurred. Exception message: " + ex.Message);
                return false;
            }
        }

        #endregion

        #region Get Notifications

        //Get processed or unprocessed notification logs
        public List<NotificationLog> GetNotificationLogs(int isProcessed, string processName, string serviceId, string issueCategory)
        {
            try
            {
                Trace.WriteLine("'" + DateTime.Now + "Exception - Before get notification Logs 4");
                if (this.OpenConnection() == true)
                {
                    OdbcCommand cmd;
                    OdbcParameter prm;
                    DataTable dt;
                    OdbcDataAdapter adapter;

                    //create command and assign the query and connection from the constructor
                    cmd = new OdbcCommand("{call NotificationLogGet(?,?,?,?)}", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    prm = cmd.Parameters.AddWithValue("@IsProcessed", isProcessed);
                    prm = cmd.Parameters.AddWithValue("@ProcessName", processName);
                    prm = cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    prm = cmd.Parameters.AddWithValue("@IssueCategory", issueCategory);

                    adapter = new OdbcDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    //Create List<NotificationLog> from instance of NotificationLog class
                    unProcessedNotificationLogs = dt.AsEnumerable().Select(m => new NotificationLog()
                    {
                        NotificationLogId = m.Field<int>("NotificationLogId"),
                        NotificationId = m.Field<int?>("NotificationId"),
                        ProcessName = m.Field<string>("ProcessName"),
                        ServiceId = m.Field<string>("ServiceId"),
                        MessageId = m.Field<string>("MessageId"),
                        IssueCategory = m.Field<string>("IssueCategory"),
                        IssueMessage = m.Field<string>("IssueMessage"),
                        IsProcessed = Convert.ToInt16(m.Field<object>("IsProcessed")),
                        DocumentId = m.Field<string>("DocumentId"),

                    }).ToList();

                    //close Connection
                    this.CloseConnection();

                    //return list to be displayed
                    return unProcessedNotificationLogs;
                }
                else
                {
                    return unProcessedNotificationLogs;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in GetNotificationLogs() method while trying to retrieve unprocessed notification logs.'ProcessName:" + processName + "','ServiceId:" + serviceId + "','IssueCatagory:" + issueCategory + "'", ex);

            }
        }

        //Get processed or unprocessed notification logs
        public List<NotificationLog> GetNotificationLogs(int isProcessed, string processName, string serviceId, string issueCategory, int pastProcessCount)
        {
            try
            {
                Trace.WriteLine("'" + DateTime.Now + "Exception - Before get notification Logs 5 ");
                if (this.OpenConnection() == true)
                {
                    OdbcCommand cmd;
                    OdbcParameter prm;
                    DataTable dt;
                    OdbcDataAdapter adapter;

                    //create command and assign the query and connection from the constructor
                    cmd = new OdbcCommand("{call NotificationLogProcessedDateGet(?,?,?,?,?)}", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    prm = cmd.Parameters.AddWithValue("@IsProcessed", isProcessed);
                    prm = cmd.Parameters.AddWithValue("@ProcessName", processName);
                    prm = cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                    prm = cmd.Parameters.AddWithValue("@IssueCategory", issueCategory);
                    prm = cmd.Parameters.AddWithValue("@PastProcessCount", pastProcessCount);

                    adapter = new OdbcDataAdapter(cmd);
                    dt = new DataTable();
                    adapter.Fill(dt);

                    List<NotificationLog> notificationprocessedTimeStamp;

                    //Create List<NotificationLog> from instance of NotificationLog class
                    notificationprocessedTimeStamp = dt.AsEnumerable().Select(m => new NotificationLog()
                    {
                        ProcessedTimestamp = m.Field<DateTime?>("ProcessedTimestamp"),
                    }).ToList();

                    if (notificationprocessedTimeStamp != null && notificationprocessedTimeStamp.Count() > 0)
                    {
                        DateTime? aa = notificationprocessedTimeStamp[notificationprocessedTimeStamp.Count - 1].ProcessedTimestamp;
                    }
                    if (notificationprocessedTimeStamp.Any())
                    {
                        //create command and assign the query and connection from the constructor
                        cmd = new OdbcCommand("{call NotificationLogProcessedDataGet(?,?,?,?,?)}", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        prm = cmd.Parameters.AddWithValue("@IsProcessed", isProcessed);
                        prm = cmd.Parameters.AddWithValue("@ProcessName", processName);
                        prm = cmd.Parameters.AddWithValue("@ServiceId", serviceId);
                        prm = cmd.Parameters.AddWithValue("@IssueCategory", issueCategory);
                        prm = cmd.Parameters.AddWithValue("@ProcessedTimestamp", notificationprocessedTimeStamp[notificationprocessedTimeStamp.Count - 1].ProcessedTimestamp);

                        adapter = new OdbcDataAdapter(cmd);
                        dt = new DataTable();
                        adapter.Fill(dt);

                        //Create List<NotificationLog> from instance of NotificationLog class
                        processedNotificationLogs = dt.AsEnumerable().Select(m => new NotificationLog()
                        {
                            NotificationLogId = m.Field<int>("NotificationLogId"),
                            NotificationId = m.Field<int?>("NotificationId"),
                            ProcessName = m.Field<string>("ProcessName"),
                            ServiceId = m.Field<string>("ServiceId"),
                            MessageId = m.Field<string>("MessageId"),
                            IssueCategory = m.Field<string>("IssueCategory"),
                            IssueMessage = m.Field<string>("IssueMessage"),
                            IsProcessed = Convert.ToInt16(m.Field<object>("IsProcessed")),
                            DocumentId = m.Field<string>("DocumentId"),
                        }).ToList();
                    }
                    this.CloseConnection();

                    //return list to be displayed
                    return processedNotificationLogs;
                }
                else
                {
                    return processedNotificationLogs;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in GetNotificationLogs() method while trying to retrieve already processed notification logs.'ProcessName:" + processName + "','ServiceId:" + serviceId + "','IssueCatagory:" + issueCategory + "'", ex);

            }
        }

        #endregion

        #region Get Notification Configuration

        //Retreive notification configurations
        public List<string> GetNotificationConfiguration(string processName, string serviceId, string issueCategory)
        {
            try
            {
                Trace.WriteLine("'" + DateTime.Now + "Exception - Before get notification configuration"+ processName + serviceId  + issueCategory);

                //Create a list to store the result
                notificationConfiguration = new List<string>();

                //Open connection
                if (this.OpenConnection() == true)
                {
                    OdbcCommand cmd;
                    OdbcParameter prm;
                    OdbcDataReader dr;

                    //create command and assign the query and connection from the constructor
                    cmd = new OdbcCommand("{call NotificationConfigurationGet(?,?,?)}", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    prm = cmd.Parameters.Add("processName", OdbcType.VarChar, 45);
                    prm.Value = processName;
                    prm = cmd.Parameters.Add("serviceId", OdbcType.VarChar, 45);
                    prm.Value = serviceId;
                    prm = cmd.Parameters.Add("issueCategory", OdbcType.VarChar, 45);
                    prm.Value = issueCategory;
                    prm.Direction = ParameterDirection.Input;

                    dr = cmd.ExecuteReader();

                    //Read the data and store them in the list
                    while (dr.Read())
                    {
                        notificationConfiguration.Add(dr["DistributionList"] + "");
                        notificationConfiguration.Add(dr["CcDistributionList"] + "");
                        notificationConfiguration.Add(dr["EmailSubjectTemplate"] + "");
                        notificationConfiguration.Add(dr["EmailBodyTemplate"] + "");
                        notificationConfiguration.Add(dr["SendWhenNoNotifications"] + "");
                        notificationConfiguration.Add(dr["AltEmailBodyTemplate"] + "");
                        notificationConfiguration.Add(dr["ExcItemsInPastNotifications"] + "");
                        notificationConfiguration.Add(dr["NotificationConfigurationId"] + "");
                    }

                    //close Data Reader
                    dr.Close();

                    //close Connection
                    this.CloseConnection();

                    //return list to be displayed
                    return notificationConfiguration;
                }
                else
                {
                    return notificationConfiguration;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in GetNotificationConfiguration() method while trying to retrieve configuration values.'ProcessName:" + processName + "','ServiceId:" + serviceId + "','IssueCatagory:" + issueCategory + "'", ex);

            }
        }

        #endregion 

        #region Inserts

        //Insert statement
        public int InsertNotification(int notificationConfigurationId, string emailSubject, string emailBody, string distributionList, string ccDistributionList)
        {
            try
            {
                Trace.WriteLine("'" + DateTime.Now + "Exception - Insert  notification ");
                //open connection
                if (this.OpenConnection() == true)
                {
                    //create command and assign the query and connection from the constructor
                    OdbcCommand cmd = new OdbcCommand();

                    cmd = new OdbcCommand("{Call NotificationInsert(?,?,?,?,?)}", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("NotificationConfigurationId", notificationConfigurationId);
                    cmd.Parameters.AddWithValue("EmailSubject", emailSubject);
                    cmd.Parameters.AddWithValue("EmailBody", emailBody);
                    cmd.Parameters.AddWithValue("DistributionList", distributionList);
                    cmd.Parameters.AddWithValue("CcDistributionList", ccDistributionList);

                    //Execute command
                    //cmd.ExecuteNonQuery();
                    int result = Convert.ToInt32(cmd.ExecuteScalar());

                    //close connection
                    this.CloseConnection();
                    return result;
                }
                else
                {
                    return 0;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in InsertNotification() method while trying to insert Notifications", ex);
            }
        }

        #endregion

        #region Updates

        //Update statement
        public void UpdateNotificationLog(int isProcessed, int notificationId, List<int> notificationLogIds)
        {
            try
            {
                Trace.WriteLine("'" + DateTime.Now + "Exception - Update notification ");

                foreach (var notificationLogId in notificationLogIds)
                {
                    using (OdbcCommand cmd = new OdbcCommand("{Call NotificationLogIsProcessedUpdate(?,?,?)}"))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Connection = connection;
                        cmd.Parameters.AddWithValue("IsProcessed", 1);
                        cmd.Parameters.AddWithValue("NotificationId", notificationId);
                        cmd.Parameters.AddWithValue("notificationLogId", notificationLogId);
                        connection.Open();
                        cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in UpdateNotificationLog() method while trying to update Notification logs", ex);
            }
        }

        #endregion
    }
}