using System;
using System.Collections.Generic;

using System.Xml;
using System.Data.Odbc;
using System.Data;
using System.Threading;
using System.Diagnostics;
using BC.Integration.Utility;
using BC.Integration;

namespace BC.Integration.Utility
{
    public class AuroraConnector
    {
        #region variables
        private List<KeyValuePair<string, string>> configuration = null;
        private string connection = "";
        private string level = "";
        private int retryInterval = 0;
        private int retryCount = 0;
        private Exception retryExceptions = null;
        private bool tracingEnabled = false;
        #endregion

        /// <summary>
        /// Class constructor used to pass configuration data.
        /// </summary>
        /// <param name="configuration">Collection of configuration data needed to connect to and manage the database interaction.</param>
        public AuroraConnector (List<KeyValuePair<string, string>> configuration)
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

        #region Log Activities
        /// <summary>
        /// Used to log activities that have been collected in the activities list collection
        /// </summary>
        /// <param name="activities">List of activities</param>
        public void LogActivities(List<Activity> activities)
        {
            OdbcConnection conn = new OdbcConnection(connection);
            OdbcCommand cmd = new OdbcCommand { Connection = conn };

            try
            {
                foreach (Activity act in activities)
                {
                    XmlDocument msgXml = act.Message;
                    string destination = act.Destination;
                    int retries = act.Retries;
                        //NOTE: ODBC does not appear to recognise the use of Stored Procedures in this connector.  Examples are
                        //all inline SQL. Hence the code calls stored procs as inline calls.
                        MessageManager mgr = new MessageManager();
                        MsgEnvelope msgOm = mgr.ConvertHipEnvelopeToOm(msgXml);
                        string interchangeId = msgOm.Interchange.InterchangeId.ToString();
                        msgOm.DecodeBody(); //make sure body is decoded before logging

                        bool exists = false;
                        int rowcount = 0;
                        bool isSuccess;
                        //Check for interchange record
                        cmd.CommandText = "Call InterchangeExists ('" + interchangeId + "')";
                        isSuccess = Retry.Execute(() => ExistsExecuteScalar(cmd, out exists), new TimeSpan(0, 0, retryInterval), retryCount, true);
                        if (!isSuccess)
                            throw new Exception("Failed to connect with the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds. (Interchange Exists call, Interchange record: " + interchangeId + ")", retryExceptions);
                        if (!exists) //insert Interchange record
                        {
                            cmd.CommandText = "Call InterchangeInsert ('" + interchangeId +
                                "', '" + msgOm.Interchange.Timestamp.ToString("yyyy-MM-dd HH:mm:ss") +
                                "', '" + msgOm.Interchange.ProcessName +
                                "', '" + msgOm.Interchange.EntryPoint +
                                "', 'Messaging" +
                                "', '" + msgOm.Interchange.BatchId + "')";
                            isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                            if (!isSuccess)
                                throw new Exception("Failed to connect with the DB to insert an Interchange record (" + interchangeId + ") after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                            if (rowcount != 1)
                                throw new Exception("Failed to update the DB when trying to insert an Interchange record (" + interchangeId + ").  The query reported " + rowcount + " rows were updated.");
                        }

                        //Check for service record
                        cmd.CommandText = "Call ServiceExists ('" + msgOm.Service.ServiceInstanceId.ToString() + "')";
                        isSuccess = Retry.Execute(() => ExistsExecuteScalar(cmd, out exists), new TimeSpan(0, 0, retryInterval), retryCount, true);
                        if (!isSuccess)
                            throw new Exception("Failed to connect with the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds. (Service Exists call, Service record: " + msgOm.Service.ServiceInstanceId.ToString() + ")", retryExceptions);

                        if (!exists) //insert service record
                        {
                            cmd.CommandText = "Call ServiceInsert ('" + msgOm.Service.ServiceInstanceId.ToString() +
                                "', '" + msgOm.Interchange.InterchangeId.ToString() +
                                "', '" + msgOm.Service.ServiceID +
                                "', '" + msgOm.Service.Version +
                                "', '" + msgOm.Service.ServiceOperationId +
                                "', '" + msgOm.Service.ServiceOperationInstanceId.ToString() +
                                "', '" + Environment.MachineName + "-" + GetServerIpAddress() + "')";
                            isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                            if (!isSuccess)
                                throw new Exception("Failed to connect with the DB to insert an Service record (" + msgOm.Service.ServiceInstanceId.ToString() + ") after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                            if (rowcount != 1)
                                throw new Exception("Failed to update the DB when trying to insert an Service record (" + msgOm.Service.ServiceInstanceId.ToString() + ").  The query reported " + rowcount + " rows were updated.");
                        }

                        //Check for service post record
                        if (msgOm.Service.ServicePostOperationInstanceId.ToString() != "")
                        {
                            cmd.CommandText = "Call ServicePostOpExists ('" + msgOm.Service.ServicePostOperationInstanceId.ToString() + "')";
                            isSuccess = Retry.Execute(() => ExistsExecuteScalar(cmd, out exists), new TimeSpan(0, 0, retryInterval), retryCount, true);
                            if (!isSuccess)
                                throw new Exception("Failed to connect with the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds. (Service Post Operation Exists call, Service Post Operation record: " + msgOm.Service.ServicePostOperationInstanceId.ToString() + ")", retryExceptions);

                            if (!exists) //insert service post record
                            {
                                cmd.CommandText = "Call ServicePostOpInsert ('" + msgOm.Service.ServicePostOperationInstanceId.ToString() +
                                    "', '" + msgOm.Service.ServiceInstanceId.ToString() +
                                    "', '" + msgOm.Service.ServicePostOperationtId +
                                    "', '" + destination +
                                    "', '" + retries + "')";
                                isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                                if (!isSuccess)
                                    throw new Exception("Failed to connect with the DB to insert an Service Post Operation record (" + msgOm.Service.ServicePostOperationInstanceId.ToString() + ") after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                                if (rowcount != 1)
                                    throw new Exception("Failed to update the DB when trying to insert an Service Post Operation record (" + msgOm.Service.ServicePostOperationInstanceId.ToString() + ").  The query reported " + rowcount + " rows were updated.");
                            }
                        }

                        //Check to see if message already exists.  This should never happen so raise an error if it does.
                        cmd.CommandText = "Call MessageExists ('" + msgOm.Msg.Id.ToString() + "')";
                        isSuccess = Retry.Execute(() => ExistsExecuteScalar(cmd, out exists), new TimeSpan(0, 0, retryInterval), retryCount, true);
                        if (!isSuccess)
                            throw new Exception("Failed to connect with the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds. (Message Exists call, Message record: " + msgOm.Msg.Id.ToString() + ")", retryExceptions);

                        if (!exists)
                        {
                            string filters = "";
                            if (msgOm.Msg.FilterKeyValuePairs != null)
                                filters = msgOm.Msg.FilterKeyValuePairs.ToString();
                            string processes = "";
                            if (msgOm.Msg.ProcessKeyValuePairs != null)
                                processes = msgOm.Msg.ProcessKeyValuePairs.ToString();
                            string body = "";
                            if (level.ToLower() == "verbose")
                                body = msgOm.Body.Replace("'", "");
                            int processEnd = 0;
                            if (msgOm.Msg.ProcessEnd)
                                processEnd = 1;

                            cmd.CommandText = "Call MessageInsert ('" + msgOm.Msg.Id.ToString() +
                                        "', '" + msgOm.Service.ServiceInstanceId.ToString() +
                                        "', '" + msgOm.Service.ServicePostOperationInstanceId.ToString() +
                                        "', '" + msgOm.Msg.Type +
                                        "', '" + msgOm.Msg.Version.ToString() +
                                        "', '" + msgOm.Msg.ParentMsgId.ToString() +
                                        "', '" + msgOm.Msg.Topic +
                                        "', '" + filters +
                                        "', '" + processes +
                                        "', '" + processEnd +
                                        "', '" + msgOm.Msg.MessageSplitIndex +
                                        "', '" + msgOm.Msg.DocumentId.Replace("'", "") +
                                        "', '" + body + "')";
                            isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                            if (!isSuccess)
                                throw new Exception("Failed to connect with the DB to insert an Message record (" + msgOm.Msg.Id.ToString() + ") after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                            if (rowcount != 1)
                                throw new Exception("Failed to update the DB when trying to insert an Message record (" + msgOm.Msg.Id.ToString() + ").  The query reported " + rowcount + " rows were updated.");
                        }
                        else
                            throw new DataException("Trying to add a message that already exists.  This suggests the code is logging the same message twice or failing to update the message envelope. InterchangeID: " + interchangeId + ", MessageID: " + msgOm.Msg.Id.ToString());
                }//End Foreach loop
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
        }
        //public void LogActivities(List<Activity> activities)
        //{
        //    foreach(Activity act in activities)
        //    {
        //        LogActivity(act.Message, act.Destination, act.Retries);
        //    }
        //}

        /// <summary>
        /// Log an activity to the database.  This method should be primarily used at the service post operation.
        /// </summary>
        /// <param name="msgXml">Message being processed in an XML document format</param>
        /// <param name="destination">Destination the service will post the message too.</param>
        /// <param name="retries">number of retries needed to send message.</param>
        public void LogActivity(XmlDocument msgXml, string destination, int retries)
        {
            Activity act = new Activity(msgXml, destination, retries);
            List<Activity> activities = new List<Activity>();
            activities.Add(act);
            LogActivities(activities);

            //OdbcConnection conn = new OdbcConnection(connection);
            //OdbcCommand cmd = new OdbcCommand();
            //try
            //{
            //    //NOTE: ODBC does not appear to recognise the use of Stored Procedures in this connector.  Examples are
            //    //all inline SQL. Hence the code calls stored procs as inline calls.
            //    MessageManager mgr = new MessageManager();
            //    MsgEnvelope msgOm = mgr.ConvertHipEnvelopeToOm(msgXml);
            //    string interchangeId = msgOm.Interchange.InterchangeId.ToString();
            //    msgOm.DecodeBody(); //make sure body is decoded before logging

            //    cmd.Connection = conn;

            //    bool exists = false;
            //    int rowcount = 0;
            //    bool isSuccess;
            //    //Check for interchange record
            //    cmd.CommandText = "Call InterchangeExists ('" + interchangeId + "')";
            //    isSuccess = Retry.Execute(() => ExistsExecuteScalar(cmd, out exists), new TimeSpan(0, 0, retryInterval), retryCount, true);
            //    if (!isSuccess)
            //        throw new Exception("Failed to connect with the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds. (Interchange Exists call, Interchange record: " + interchangeId + ")", retryExceptions);
            //    if (!exists) //insert Interchange record
            //    {
            //        cmd.CommandText = "Call InterchangeInsert ('" + interchangeId +
            //            "', '" + msgOm.Interchange.Timestamp.ToString("yyyy-MM-dd HH:mm:ss") +
            //            "', '" + msgOm.Interchange.ProcessName +
            //            "', '" + msgOm.Interchange.EntryPoint +
            //            "', 'Messaging" +
            //            "', '" + msgOm.Interchange.BatchId + "')";
            //        isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
            //        if (!isSuccess)
            //            throw new Exception("Failed to connect with the DB to insert an Interchange record (" + interchangeId + ") after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
            //        if (rowcount != 1)
            //            throw new Exception("Failed to update the DB when trying to insert an Interchange record (" + interchangeId + ").  The query reported " + rowcount + " rows were updated.");
            //    }

            //    //Check for service record
            //    cmd.CommandText = "Call ServiceExists ('" + msgOm.Service.ServiceInstanceId.ToString() + "')";
            //    isSuccess = Retry.Execute(() => ExistsExecuteScalar(cmd, out exists), new TimeSpan(0, 0, retryInterval), retryCount, true);
            //    if (!isSuccess)
            //        throw new Exception("Failed to connect with the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds. (Service Exists call, Service record: " + msgOm.Service.ServiceInstanceId.ToString() + ")", retryExceptions);

            //    if (!exists) //insert service record
            //    {
            //        cmd.CommandText = "Call ServiceInsert ('" + msgOm.Service.ServiceInstanceId.ToString() +
            //            "', '" + msgOm.Interchange.InterchangeId.ToString() +
            //            "', '" + msgOm.Service.ServiceID +
            //            "', '" + msgOm.Service.Version +
            //            "', '" + msgOm.Service.ServiceOperationId +
            //            "', '" + msgOm.Service.ServiceOperationInstanceId.ToString() +
            //            "', '" + Environment.MachineName + "-" + GetServerIpAddress() + "')";
            //        isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
            //        if (!isSuccess)
            //            throw new Exception("Failed to connect with the DB to insert an Service record (" + msgOm.Service.ServiceInstanceId.ToString() + ") after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
            //        if (rowcount != 1)
            //            throw new Exception("Failed to update the DB when trying to insert an Service record (" + msgOm.Service.ServiceInstanceId.ToString() + ").  The query reported " + rowcount + " rows were updated.");
            //    }

            //    //Check for service post record
            //    if (msgOm.Service.ServicePostOperationInstanceId.ToString() != "")
            //    {
            //        cmd.CommandText = "Call ServicePostOpExists ('" + msgOm.Service.ServicePostOperationInstanceId.ToString() + "')";
            //        isSuccess = Retry.Execute(() => ExistsExecuteScalar(cmd, out exists), new TimeSpan(0, 0, retryInterval), retryCount, true);
            //        if (!isSuccess)
            //            throw new Exception("Failed to connect with the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds. (Service Post Operation Exists call, Service Post Operation record: " + msgOm.Service.ServicePostOperationInstanceId.ToString() + ")", retryExceptions);

            //        if (!exists) //insert service post record
            //        {
            //            cmd.CommandText = "Call ServicePostOpInsert ('" + msgOm.Service.ServicePostOperationInstanceId.ToString() +
            //                "', '" + msgOm.Service.ServiceInstanceId.ToString() +
            //                "', '" + msgOm.Service.ServicePostOperationtId +
            //                "', '" + destination +
            //                "', '" + retries + "')";
            //            isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
            //            if (!isSuccess)
            //                throw new Exception("Failed to connect with the DB to insert an Service Post Operation record (" + msgOm.Service.ServicePostOperationInstanceId.ToString() + ") after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
            //            if (rowcount != 1)
            //                throw new Exception("Failed to update the DB when trying to insert an Service Post Operation record (" + msgOm.Service.ServicePostOperationInstanceId.ToString() + ").  The query reported " + rowcount + " rows were updated.");
            //        }
            //    }

            //    //Check to see if message already exists.  This should never happen so raise an error if it does.
            //    cmd.CommandText = "Call MessageExists ('" + msgOm.Msg.Id.ToString() + "')";
            //    isSuccess = Retry.Execute(() => ExistsExecuteScalar(cmd, out exists), new TimeSpan(0, 0, retryInterval), retryCount, true);
            //    if (!isSuccess)
            //        throw new Exception("Failed to connect with the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds. (Message Exists call, Message record: " + msgOm.Msg.Id.ToString() + ")", retryExceptions);

            //    if (!exists)
            //    {
            //        string filters = "";
            //        if (msgOm.Msg.FilterKeyValuePairs != null)
            //            filters = msgOm.Msg.FilterKeyValuePairs.ToString();
            //        string processes = "";
            //        if (msgOm.Msg.ProcessKeyValuePairs != null)
            //            processes = msgOm.Msg.ProcessKeyValuePairs.ToString();
            //        string body = "";
            //        if (level.ToLower() == "verbose")
            //            body = msgOm.Body.Replace("'", "");
            //        int processEnd = 0;
            //        if (msgOm.Msg.ProcessEnd)
            //            processEnd = 1;

            //        cmd.CommandText = "Call MessageInsert ('" + msgOm.Msg.Id.ToString() +
            //                    "', '" + msgOm.Service.ServiceInstanceId.ToString() +
            //                    "', '" + msgOm.Service.ServicePostOperationInstanceId.ToString() +
            //                    "', '" + msgOm.Msg.Type +
            //                    "', '" + msgOm.Msg.Version.ToString() +
            //                    "', '" + msgOm.Msg.ParentMsgId.ToString() +
            //                    "', '" + msgOm.Msg.Topic +
            //                    "', '" + filters +
            //                    "', '" + processes +
            //                    "', '" + processEnd +
            //                    "', '" + msgOm.Msg.MessageSplitIndex +
            //                    "', '" + msgOm.Msg.DocumentId.Replace("'", "") +
            //                    "', '" + body + "')";
            //        isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
            //        if (!isSuccess)
            //            throw new Exception("Failed to connect with the DB to insert an Message record (" + msgOm.Msg.Id.ToString() + ") after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
            //        if (rowcount != 1)
            //            throw new Exception("Failed to update the DB when trying to insert an Message record (" + msgOm.Msg.Id.ToString() + ").  The query reported " + rowcount + " rows were updated.");
            //    }
            //    else
            //        throw new DataException("Trying to add a message that already exists.  This suggests the code is logging the same message twice or failing to update the message envelope. InterchangeID: " + interchangeId + ", MessageID: " + msgOm.Msg.Id.ToString());
            //}
            //finally
            //{
            //    conn.Dispose();
            //    cmd.Dispose();
            //}
        }
        #endregion

        #region Log Exceptions
        public void LogExceptions(Guid interchangeId, Guid msgId, Guid parentMsgId, string docId, 
                                    string processName, string msg, string exceptionMsg, string stackTrace)
        {
            OdbcConnection conn = new OdbcConnection(connection);
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                //If there are single quotes in the strings it will cause the exception logging to fail.
                exceptionMsg = exceptionMsg.Replace("'", "");
                stackTrace = stackTrace.Replace("'", "");
                msg = msg.Replace("'", "");

                cmd.Connection = conn;
                cmd.CommandText = "Call ExceptionInsert2 ('" + interchangeId.ToString() +
                                                        "', '" + msgId.ToString() +
                                                        "', '" + parentMsgId.ToString() +
                                                        "', '" + docId +
                                                        "', '" + processName +
                                                        "', '" + msg +
                                                        "', '" + exceptionMsg +
                                                        "', '" + stackTrace + 
                                                        "', '" + Environment.MachineName + "-" + GetServerIpAddress() + "')";
                int rowcount = 0;
                bool isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                if (!isSuccess)
                    throw new Exception("Failed to insert exception record after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                if (rowcount != 1)
                    throw new Exception("Failed to insert exception record.  The query reported " + rowcount + " rows were updated.");
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
        }

        /// <summary>
        /// General exception logging method
        /// </summary>
        /// <param name="msg">Message from point the exception was raised.</param>
        /// <param name="exceptionMsg">Exception message from the exception captured, including inner exceptions.</param>
        /// <param name="stackTrace">Stack trace from captured exception.</param>
        public void LogExceptions(string msg, string exceptionMsg, string stackTrace)
        {
            OdbcConnection conn = new OdbcConnection(connection);
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
                if (msg != null)
                {
                    msg = msg.Replace("'", "");
                }
                else
                {
                    msg = "No message available";
                }

                cmd.Connection = conn;
                cmd.CommandText = "Call ExceptionInsert ('', '', '', '" + msg +
                                                        "', '" + exceptionMsg +
                                                        "', '" + stackTrace +
                                                        "', '" + Environment.MachineName + "-" + GetServerIpAddress() + "')";
                int rowcount = 0;
                bool isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                if (!isSuccess)
                    throw new Exception("Failed to insert exception record after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                if (rowcount != 1)
                    throw new Exception("Failed to insert exception record.  The query reported " + rowcount + " rows were updated.");
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
        }

        #endregion

        #region Log Activations

        public void LogActivation(string serviceName, Guid activationGuid, bool isBlocked)
        {
            OdbcConnection conn = new OdbcConnection(connection);
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ActivationLogInsert ('" + activationGuid.ToString() +
                                                        "', '" + Environment.MachineName + "-" + GetServerIpAddress() +
                                                        "', '" + serviceName +
                                                        "', '" + Convert.ToInt16(isBlocked) + "')";
                int rowcount = 0;
                bool isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                if (!isSuccess)
                    throw new Exception("Failed to insert LogActivation record after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                if (rowcount != 1)
                    throw new Exception("Failed to insert LogActivation record.  The query reported " + rowcount + " rows were updated.");
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
        }

        public void LogActivationEnd(Guid activationGuid)
        {
            OdbcConnection conn = new OdbcConnection(connection);
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ActivationLogEndUpdate ('" + activationGuid.ToString() + "')";
                int rowcount = 0;
                bool isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                if (!isSuccess)
                {
                    Trace.WriteLineIf(tracingEnabled, "Failed to update LogActivationEnd record (" + activationGuid.ToString() + ")");
                    throw new Exception("Failed to update LogActivationEnd record (" + activationGuid.ToString() + ") after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                }
                if (rowcount != 1)
                    throw new Exception("Failed to update LogActivationEnd record (" + activationGuid.ToString() + ").  The query reported " + rowcount + " rows were updated.");
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
        }

        public void AssociateActivationWithInterchages(Guid activationGuid, Guid interchageId)
        {
            OdbcConnection conn = new OdbcConnection(connection);
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ActivationInterchangeJoinInsert ('" + activationGuid.ToString() +
                                                        "', '" + interchageId.ToString() + "')";
                int rowcount = 0;
                bool isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                if (!isSuccess)
                    throw new Exception("Failed to insert ActivationInterchangeJoin record after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                if (rowcount != 1)
                    throw new Exception("Failed to insert ActivationInterchangeJoin record.  The query reported " + rowcount + " rows were updated.");
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
        }

        #endregion

        #region Notifications

        public void LogNotification(string processName, string serviceID, Guid messageId, string issueCategory, string issueMessage, string documentId)
        {
            OdbcConnection conn = new OdbcConnection(connection);
            OdbcCommand cmd = new OdbcCommand();
            try
            {
                //If there are single quotes in the strings it will cause the exception logging to fail.
                issueMessage = issueMessage.Replace("'", "");
                documentId = documentId.Replace("'", "");

                cmd.Connection = conn;
                cmd.CommandText = "Call NotificationLogInsert ('" + processName +
                                                        "', '" + serviceID +
                                                        "', '" + messageId.ToString() +
                                                        "', '" + issueCategory +
                                                        "', '" + issueMessage +
                                                        "', '" + documentId + "')";
                int rowcount = 0;
                bool isSuccess = Retry.Execute(() => ExecuteNonQuery(cmd, out rowcount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                if (!isSuccess)
                    throw new Exception("Failed to insert notification log record after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
                if (rowcount != 1)
                    throw new Exception("Failed to insert notification log record.  The query reported " + rowcount + " rows were updated.");
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }

        }

        #endregion


        #region Helper Methods

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
                Trace.WriteLine("Exception occured trying to connect to the DB Corp.Integration.Utility.AuroraConnector.ExecuteNonQuery");
                if (retryExceptions == null)
                    retryExceptions = new Exception("Error tring to insert data into the DB (Corp.Integration.Utility.AuroraConnector.ExecuteNonQuery)", ex);
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

        /// <summary>
        /// Wraps ODBC command ExecuteScalar call to Aurora to return the values needed for the retry logic
        /// </summary>
        /// <param name="cmd">Pre configured command object to use to run nonquery command</param>
        /// <param name="result">Result of the Exists DB call</param>
        /// <returns>Bool to identify if the call was successfully made without exception.</returns>
        private bool ExistsExecuteScalar(OdbcCommand cmd, out bool result)
        {
            try
            {
                cmd.Connection.Open();
                result = Convert.ToBoolean(cmd.ExecuteScalar());
                return true;
            }
            catch (Exception ex)
            {
                
                Trace.WriteLineIf(tracingEnabled, "Exception occured trying to connect to the DB 'Corp.Integration.Utility.AuroraConnector.ExistsExecuteScalar'");
                if (retryExceptions == null)
                    retryExceptions = new Exception("Error tring to insert data into the DB", ex);
                else
                    retryExceptions = new Exception(ex.Message, retryExceptions);
                result = false;
                return false;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        /// <summary>
        /// Wrapper for the GetServerIpAddress call to manage the exceptions
        /// </summary>
        /// <returns>Server IP address</returns>
        private string GetServerIpAddress()
        {
            try
            {
                return  Utilities.GetServerIpAddress(); 
            }
            catch (Exception ex)
            {
                //If no IP is available log an exception and return a 'Unavailable' string.
                LogExceptions("The GetServerIpAddress method was not able to resolve the servers IP address.", ex.Message, ex.StackTrace);
                return "Unavailable";
            }
        }

        #endregion
    }

 }
