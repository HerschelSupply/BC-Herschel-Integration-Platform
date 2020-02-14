///Aug 2018: Code updated to include a data push to a New Relic queue for HIP exception and instrumentaion monitoring
///Test change
///Another change
///Hello
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using BC.Integration.Interfaces;
using BC.Integration.Utility;
using System.IO;
using System.Xml;
using Microsoft.Practices.Unity;


namespace BC.Integration.Utility
{
    public class InstrumentationDB : IInstrumentation
    {
        #region Properties
        IConfiguration config;
        IPublishService publishService;
        UnityContainer container;
        List<string> urls;
        private string queueName = ""; //populated by activation code and used by New Relic code
        private DateTime activationTimestamp;
        private DateTime activationEndTimestamp;

        private string instrumentationConnectionStr = "";
        private List<KeyValuePair<string, string>> configuration = null;
        private string logFilePath = "C:\\HIP Logs";
        private string exceptionFilePath = "C:\\HIP Logs";
        private List<Activity> activityLogMsgs = new List<Activity>();
        private bool tracingEnabled = false;
        private AuroraConnector connector;

        /// <summary>
        /// used to provide the instumentation with a local and global configuration data.  The
        /// setter also calls the setup method to configure the component after the configuration
        /// collection has been populated.
        /// </summary>
        public List<KeyValuePair<string, string>> Configuration
        {
            get => configuration;
            set
            {
                if (configuration == null)
                {
                    configuration = value;
                    Setup();
                }
                else
                {
                    configuration = value;
                    SetLocalVariables();
                }
                connector = new AuroraConnector(configuration);
            }
        }
        #endregion
        
        public InstrumentationDB()
        {
        }

        private void Setup()
        {
            try
            {
                if (!Directory.Exists(logFilePath))
                    Directory.CreateDirectory(logFilePath);
                if (!Directory.Exists(exceptionFilePath))
                    Directory.CreateDirectory(exceptionFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured in Corp.Integration.Utility.InstrumentationDB.Setup trying to verify and if " +
                    "necessary, create the logging folders.  The Log File Path: " + logFilePath + ", Exception File Path: " + exceptionFilePath, ex);
            }

            SetLocalVariables();

        }

        private void SetLocalVariables()
        {
            instrumentationConnectionStr = Utilities.GetConfigurationValue(configuration, "InstrumentationConnectionStr");
            logFilePath = Utilities.GetConfigurationValue(configuration, "LogFilePath");
            exceptionFilePath = Utilities.GetConfigurationValue(configuration, "ExceptionFilePath");
            string val = Utilities.GetConfigurationValue(configuration, "TracingEnabled");
            if (val != "")
            {
                if (val.ToLower() == "true" || val.ToLower() == "false")
                    tracingEnabled = Convert.ToBoolean(val);
                if (val == "1" || val == "0")
                    tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
            }
        }

        #region Log Activity
        /// <summary>
        /// Failure to log should not return an exception message as instumentation is a non critical path so should never 
        /// cause a message to fail. This method sets the destination in the DB to be an empty string and retries to '0'.
        /// </summary>
        /// <param name="msg">Serialized Msg Envelope</param>
        /// <param name="level">Normal or Verbose</param>
        public void LogActivity(XmlDocument msg)
        {
            try
            {
                activityLogMsgs.Add(new Activity(msg, "", 0));
                //Since we are currently not concerned about throughput more accurate timestamping would be prefered.
                //FlushActivity(); 
            }
            catch (Exception exc)
            {
                LogMessagingException("An exception occured trying to generate the activity log message from the xml document.(Corp.Integration.Utility.InstrumentationDB.LogActivity method call)", msg, exc);
            }
        }

        /// <summary>
        /// Failure to log should not return an exception message as instumentation is a non critical path so should never 
        /// cause a message to fail.
        /// </summary>
        /// <param name="msg">Serialized Msg Envelope</param>
        /// <param name="level">Normal or Verbose</param>
        /// <param name="destination">The destination the post operation send the message too.</param>
        /// <param name="retries">Number of retries the Post Operation had to preform.</param>
        public void LogActivity(XmlDocument msg, string destination, int retries)
        {
            try
            {
                activityLogMsgs.Add(new Activity(msg, destination, retries));
                //Since we are currently not concerned about throughput more accurate timestamping would be prefered.
                //FlushActivity();
            }
            catch (Exception exc)
            {
                LogMessagingException("An exception occured trying to generate the activity log message from the xml document (Corp.Integration.Utility.InstrumentationDB.LogActivity method call).  The additional parameters were; destination- " + destination + ", retries - " + retries, msg, exc);
            }
        }

        /// <summary>
        /// The code tries to log the data to the instumentation and exception database.  If that fails it will first try to log to the event log,
        /// then try to lof to an txt file.
        /// It takes all the activity logs in the activityLogMsgs property and writes them to the log store.
        /// </summary>
        public void FlushActivity()
        {
            try
            {
                //AuroraConnector connector = new AuroraConnector(configuration);
                connector.LogActivities(activityLogMsgs);
            }
            catch (Exception exc)
            {
                LogGeneralException("An exception occurred when Corp.Integration.Utility.InstrumentationDB.FlushActivity method was called.", exc);
            }
            //Send data to New Relic queue.
            //Clear log file incase flush gets called again.
            activityLogMsgs = new List<Activity>();
        }
        
        #endregion

        #region Exception Logging
        public void LogGeneralException(string exceptionMessage, Exception ex)
        {
            LogMessagingException(exceptionMessage, null, ex);
        }

        public void LogMessagingException(string exceptionMessage, XmlDocument msgXml, Exception ex)
        {
            string exStr = "";
            string stack = "";
            if (ex != null)
            {
                exStr = "\r\n" + exceptionMessage + "\r\nCaught Exception Message:\r\n" + ex.Message + "\r\nInner Exceptions:\r\n";
                while (ex.InnerException != null)
                {
                    exStr += ex.InnerException.Message + "\r\n";
                    ex = ex.InnerException;
                }
                if (ex.StackTrace != null) //Stack trace can still be null even if the exception is not.
                {
                    stack = ex.StackTrace;
                }
            }

            try
            {
                //AuroraConnector connector = new AuroraConnector(configuration);
                //On-ramp messages will not have parent msg ID's
                Guid pId = new Guid();
                string docId = "";
                string processName = "";

                if (msgXml != null)
                {
                    if (msgXml.SelectSingleNode("/MsgEnvelope/Msg/ParentMsgId").InnerText != "")
                        pId = new Guid(msgXml.SelectSingleNode("/MsgEnvelope/Msg/ParentMsgId").InnerText);
                    if (msgXml.SelectSingleNode("/MsgEnvelope/Msg/DocId").InnerText != "")
                        docId = msgXml.SelectSingleNode("/MsgEnvelope/Msg/DocId").InnerText;
                    if (msgXml.SelectSingleNode("/MsgEnvelope/Interchange/ProcessName").InnerText != "")
                        processName = msgXml.SelectSingleNode("/MsgEnvelope/Interchange/ProcessName").InnerText;


                    connector.LogExceptions(new Guid(msgXml.SelectSingleNode("/MsgEnvelope/Interchange/Id").InnerText),
                                    new Guid(msgXml.SelectSingleNode("/MsgEnvelope/Msg/Id").InnerText),
                                    pId,
                                    docId,
                                    processName,
                                    msgXml.OuterXml, exStr, stack);

                }
                else
                {
                    connector.LogExceptions("", exStr, stack);

                }

            }
            catch (Exception exc)
            {
                string rec = "";
                if (msgXml != null)
                    rec = "\r\nMessage that was being processed when error occurred. " + CreateLoggingString(msgXml) + "\r\n";
                Trace.WriteLine("INSTRUMENTATION: Exception occurred when trying to write an exception to the DB.  Original exception message: " + exStr + " Secondary exception: " + exc.Message);
                LogExceptionLocally("The originating exception that was being logged was -" + exStr + rec, exc);
            }
        }

         private string CreateLoggingString(XmlDocument msg)
        {
            try
            {
                string str = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss") + " Server: " + Environment.MachineName + ", " +
                    " ProcessName: " + msg.SelectSingleNode("/MsgEnvelope/Interchange/ProcessName").InnerText + ", ";
                try
                {
                    //DateTime date = DateTime.ParseExact(msg.SelectSingleNode("/MsgEnvelope/Interchange/Timestamp").InnerText, "yyyy-MM-ddThh:mm:ss.fffffffzzz", System.Globalization.CultureInfo.InvariantCulture);
                    //str += "Interchange Timestamp: " +  date.ToString("yyyy-MM-dd hh:mm:ss") + ", ";
                    string date = msg.SelectSingleNode("/MsgEnvelope/Interchange/Timestamp").InnerText;
                    str += "Interchange Timestamp: " +  date.Substring(0, date.IndexOf('.')) + ", ";
                }
                catch
                {
                    str += "Interchange Timestamp: " + msg.SelectSingleNode("/MsgEnvelope/Interchange/Timestamp").InnerText + ", ";
                }
                str += "Interchange: " + msg.SelectSingleNode("/MsgEnvelope/Interchange/Id").InnerText + ", ";
                str += "Split Index: " + msg.SelectSingleNode("/MsgEnvelope/Msg/MessageSplitIndex").InnerText + ", ";
                str += "Service: " + msg.SelectSingleNode("/MsgEnvelope/Svc/SvcId").InnerText + ", ";
                str += "Service Operation: " + msg.SelectSingleNode("/MsgEnvelope/Svc/SvcOpId").InnerText + ", ";
                str += "Service Post Operation: " + msg.SelectSingleNode("/MsgEnvelope/Svc/SvcPostId").InnerText + ", ";
                str += "Service ID: " + msg.SelectSingleNode("/MsgEnvelope/Svc/SvcInstId").InnerText + ", ";
                str += "Service Operation ID: " + msg.SelectSingleNode("/MsgEnvelope/Svc/SvcOpInstId").InnerText + ", ";
                str += "Service Post Operation ID: " + msg.SelectSingleNode("/MsgEnvelope/Svc/SvcPostInstId").InnerText + ", ";
                str += "Msg Type: " + msg.SelectSingleNode("/MsgEnvelope/Msg/Type").InnerText + ", ";
                str += "Msg ID: " + msg.SelectSingleNode("/MsgEnvelope/Msg/Id").InnerText + ", ";
                str += "Msg Parent ID: " + msg.SelectSingleNode("/MsgEnvelope/Msg/ParentMsgId").InnerText + ".";
                return str;
            }
            catch (Exception ex)
            {
                //Instrumentation can't cause a message to fail so must consume all exceptions and handle the best it can.
                throw new Exception("Reading the XmlDocument object supplied with the exception resulted in an exception: \r\n" + ex.Message + "\r\n", ex);
            }
        }

        /// <summary>
        /// If the main logging methods raise exceptions log lovally to file...
        /// </summary>
        /// <param name="msg">The message to log to the local repository</param>
        /// <param name="ex">Exception object that was thrown when the component failured to log the data</param>
        private void LogExceptionLocally(string msg, Exception ex)
        {
            //Get exception data
            string exStr = "   -" + ex.Message + "\r\n   -";
            while (ex.InnerException != null)
            {
                exStr += "   -" + ex.InnerException.Message + "\r\n";
                ex = ex.InnerException;
            }
            try
            {
                File.AppendAllText(logFilePath + "\\ExceptionLog" + DateTime.Now.ToString("yyyyMMdd") + ".txt",
                    "\r\n" + DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd hh:mm:ss") + ": An exception was thrown while trying to log an exception on " + Environment.MachineName 
                    + "\r\n" + msg + "\r\nThe exceptions that were raised while trying to log the original exception were:\r\n" + exStr);
            }
            catch (Exception exc)
            {
                Trace.WriteLine("INSTRUMENTATION: Exception occurred when trying to write an exception Local File.  Exception Message: " + exc.Message);
                //At this stage there is nothing we can do if we can't communicate with the file system.
            }
        }
        #endregion

        #region Write to File
        public void WriteMsgToFile(string path, string batchFilename, XmlDocument msgXml, string documentType, string processStep)
        {
            try
            {
                //Get MsgID if available
                string msgId = "";
                if (msgXml.SelectSingleNode(@"/MsgEnvelope/Msg/Id") != null)
                    msgId = msgXml.SelectSingleNode(@"/MsgEnvelope/Msg/Id").InnerText;
                else
                    msgId = Guid.NewGuid().ToString();

                path += "\\" + DateTime.Now.ToString("yyyyMMdd");
                if (batchFilename != null)
                    path += "\\" + batchFilename;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                using (StreamWriter fs = new StreamWriter(path + "\\" + documentType + " " + processStep + " " + msgId + ".xml"))
                {
                    fs.Write(msgXml.InnerXml);
                }
            }
            catch (Exception ex)
            {
                LogMessagingException("An exception was raised while trying to write the message (XmlDocument) to file (Corp.Integration.Utility.InstrumentationDB.WriteMsgToFile method call).  The parameters are; path: " + path + ", batchFileName: " + batchFilename + ", documentType: " + documentType + ", processStep: " + processStep, msgXml, ex);
            }
        }

        /// <summary>
        /// Persists an message string to the file storage.  The method builds a directory structure including the date and batch 
        /// filename. This method was added to same Json serialized messages.
        /// </summary>
        /// <param name="path">Path to the route file storage location</param>
        /// <param name="batchFilename">If the messages need to be grouped in a batch include the batch filename, otherwise pass null</param>
        /// <param name="msg">message to be persisted</param>
        /// <param name="documentType">The document type being transmitted.  This parameter used to build the message name. Pass null to ignore value.</param>
        /// <param name="processStep">Process step the message was created (onRamp or Post).  This parameter used to build the message name. Pass null to ignore value.</param>
        public void WriteMsgToFile(string path, string batchFilename, string msg, string documentType, string processStep)
        {
            try
            {
                //Get MsgID if available
                string uniqueId = DateTime.Now.ToString("hhmmssss");

                path += "\\" + DateTime.Now.ToString("yyyyMMdd");
                if (batchFilename != null)
                    path += "\\" + batchFilename;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (documentType == null)
                {
                    using (StreamWriter fs = new StreamWriter(path + "\\" + processStep + "." + uniqueId + ".txt"))
                    {
                        fs.Write(msg);
                    }
                }
                else if (processStep == null)
                {
                    using (StreamWriter fs = new StreamWriter(path + "\\" + documentType + "." + uniqueId + ".txt"))
                    {
                        fs.Write(msg);
                    }
                }
                else
                {
                    using (StreamWriter fs = new StreamWriter(path + "\\" + documentType + "." + processStep + "." + uniqueId + ".txt"))
                    {
                        fs.Write(msg);
                    }
                }

            }
            catch (Exception ex)
            {
                LogGeneralException("An exception was raised while trying to write the message (string) to file (Corp.Integration.Utility.InstrumentationDB.WriteMsgToFile method call).  The parameters are; path: " + path + ", batchFileName: " + batchFilename + ", documentType: " + documentType + ", processStep: " + processStep, ex);
            }

        }
        #endregion

        #region Activation logging
        public void LogActivation(string serviceName, Guid activationGuid, bool isBlocked)
        {
            try
            {
                //AuroraConnector connector = new AuroraConnector(configuration);
                connector.LogActivation(serviceName, activationGuid, isBlocked);
                activationTimestamp = DateTime.Now;
                //if (serviceName.IndexOf('(') != -1)
                //{
                //    queueName = serviceName.Substring(serviceName.IndexOf('(')).TrimEnd(')');
                //}
                //else
                //{
                //    queueName = serviceName;
                //}
            }
            catch (Exception exc)
            {
                LogGeneralException("An exception occured when Corp.Integration.Utility.InstrumentationDB.LogActivation method was called.", exc);
            }
        }

        public void LogActivationEnd(Guid activationGuid)
        {
            try
            {
                //AuroraConnector connector = new AuroraConnector(configuration);
                connector.LogActivationEnd(activationGuid);
                activationEndTimestamp = DateTime.Now;
            }
            catch (Exception exc)
            {
                LogGeneralException("An exception occured when Corp.Integration.Utility.InstrumentationDB.LogActivationEnd method was called.", exc);
            }
        }

        public void AssociateActivationWithInterchages(Guid activationGuid, Guid interchageId)
        {
            try
            {
                //AuroraConnector connector = new AuroraConnector(configuration);
                connector.AssociateActivationWithInterchages(activationGuid, interchageId);
            }
            catch (Exception exc)
            {
                LogGeneralException("An exception occured when Corp.Integration.Utility.InstrumentationDB.AssociateActivationWithInterchages method was called.", exc);
            }
        }
        #endregion

        #region Notifications

        public void LogNotification(string processName, string serviceID, Guid messageId, string issueCategory, string issueMessage, string documentId)
        {
            try
            {
                connector.LogNotification(processName, serviceID, messageId, issueCategory, issueMessage, documentId);
            }
            catch (Exception exc)
            {
                LogGeneralException("An exception occured when Corp.Integration.Utility.InstrumentationDB.LogNotification method was called.", exc);
            }
        }

        #endregion

        #region New Relic



        #endregion
    }

    public class Activity
    {
        private XmlDocument msg = new XmlDocument();
        private string destination = "";
        private int retries = 0;

        public XmlDocument Message { get => msg; set => msg = value; }
        public string Destination { get => destination; set => destination = value; }
        public int Retries { get => retries; set => retries = value; }

        public Activity (XmlDocument msg, string destination, int retries)
        {
            Message = msg;
            Destination = destination;
            Retries = retries;
        }
    }
}
