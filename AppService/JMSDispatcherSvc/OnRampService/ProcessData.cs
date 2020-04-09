using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using BC.Integration.Interfaces;
using BC.Integration.Utility;
using System.Xml;
using System.IO;
using BC.Integration.AppService.DispatcherSvcOnRamp;
using System.Security.AccessControl;

namespace BC.Integration.AppService.JMSDispatcherSvc
{
    public class Process
    {

        #region Properties

        private List<KeyValuePair<string, string>> configuration = null;

        //Dependency Injection Components
        IConfiguration config;
        IInstrumentation instrumentation;
        IPublishService publishService;
        UnityContainer container;

        //Key service Properties that should never change from design time
        private string processName = ConfigurationManager.AppSettings["ProcessName"];
        private string serviceId = ConfigurationManager.AppSettings["ServiceId"];
        private string serviceOperationId = ConfigurationManager.AppSettings["ServiceOperationId"];
        private string servicePostOperationId = ConfigurationManager.AppSettings["ServicePostOperationId"];
        private string centralConfigConnString = ConfigurationManager.AppSettings["CentralConfigConnString"];
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string prefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";

        //These rest of the properties could be overridden by the central config store at the service ot process level


        //Msg Envelope properties
        private string msgType = ConfigurationManager.AppSettings["ServiceId"];
        private string filterKeyValuePairs = "";
        private string processKeyValuePairs = "";
        private string topic = "";

        //Messaging Queue Properties
        private string queueUrl = "";

        private MessageManager msgMgr = new MessageManager();
        private string messageFormat = "xml";
        private decimal serviceVersion = 1;
        private decimal messageVersion = 1;
        private bool tracingEnabled = false;
        private bool localFileSource = true;
        private string exceptionMessageFolderPath = "";
        private string tracingMessageFolderPath = "";
        private string pickupFileFolderPath = "";
        private string inProgressFolderPath = "";
        private string completedFolderPath = "";
        private string returnFolderPath = "";
        private string exchangeFolderPath = "";
        private string cancelledFolderPath = "";
        private string archiveFolderPath = "";
        private string process = "";
        private string activationGuid_ = "";

        #endregion

        #region Configuration methods
        /// <summary>
        /// Retrives the service and global central config values from the store.  The service properties are applied to the 
        /// service first, so global properties will override all properties.
        /// </summary>
        /// <param name="config">The configuration class to implement the retrieval of the central configuration</param>
        private void OverrideConfigProperties(IConfiguration config)
        {
            //This is an On-Ramp so we know process name from the beginning so no reset needed.
            configuration = config.GetConfiguration(serviceId, processName);
            PopulateLocalVaribale();
        }

        /// <summary>
        /// This method populates local variables with the data from the configuration key/value pair collection.  It is
        /// called by the OverrideConfigProperties method to populated the variables with local and central values.
        /// </summary>
        private void PopulateLocalVaribale()
        {
            messageFormat = Utilities.GetConfigurationValue(configuration, "MessageFormat");
            serviceVersion = Convert.ToDecimal(Utilities.GetConfigurationValue(configuration, "ServiceVersion"));
            messageVersion = Convert.ToDecimal(Utilities.GetConfigurationValue(configuration, "MessageVersion"));
            exceptionMessageFolderPath = Utilities.GetConfigurationValue(configuration, "ExceptionMessageFolderPath");
            tracingMessageFolderPath = Utilities.GetConfigurationValue(configuration, "TracingMessageFolderPath");
            pickupFileFolderPath = Utilities.GetConfigurationValue(configuration, "PickupFileFolderPath");
            inProgressFolderPath = Utilities.GetConfigurationValue(configuration, "InProgressFolderPath");
            exchangeFolderPath = Utilities.GetConfigurationValue(configuration, "ExchangeFolderPath");
            completedFolderPath = Utilities.GetConfigurationValue(configuration, "CompletedFolderPath");
            returnFolderPath = Utilities.GetConfigurationValue(configuration, "ReturnFolderPath");
            cancelledFolderPath = Utilities.GetConfigurationValue(configuration, "CancelledFolderPath");
            archiveFolderPath = Utilities.GetConfigurationValue(configuration, "ArchiveFolderPath");
            msgType = Utilities.GetConfigurationValue(configuration, "MsgType");
            filterKeyValuePairs = Utilities.GetConfigurationValue(configuration, "FilterKeyValuePairs");
            processKeyValuePairs = Utilities.GetConfigurationValue(configuration, "ProcessKeyValuePairs");
            topic = Utilities.GetConfigurationValue(configuration, "Topic");
            queueUrl = Utilities.GetConfigurationValue(configuration, "QueueUrl");
            process = Utilities.GetConfigurationValue(configuration, "ProcessName");

            prefix = process + ": ";

            string val = Utilities.GetConfigurationValue(configuration, "TracingEnabled");
            if (val != "")
            {
                if (val.ToLower() == "true" || val.ToLower() == "false")
                {
                    tracingEnabled = Convert.ToBoolean(val);
                }
                if (val == "1" || val == "0")
                {
                    tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
                }
            }

            string val2 = Utilities.GetConfigurationValue(configuration, "LocalFileSource");
            if (val2 != "")
            {
                if (val2.ToLower() == "true" || val2.ToLower() == "false")
                {
                    localFileSource = Convert.ToBoolean(val2);
                }
                if (val2 == "1" || val2 == "0")
                {
                    localFileSource = Convert.ToBoolean(Convert.ToInt16(val2));
                }
            }
        }

        #endregion

        /// <summary>
        /// Process Data is the main method in the BC.Integration.AppService.JMSDispatcherSvc class.  It implements the following steps;
        /// -Resolves the Unity DI classes.
        /// -Overrides local property setting with the central configuration using the IConfiguration component.
        /// -Connect to the source system and checks to see if there are available files
        /// -Download data
        /// -Log activity
        /// -Process the data and spliting it into seperate messages
        /// -Log activity for each message
        /// </summary>
        public bool ProcessData(string activationGuid)
        {
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + ".ProcessData() method is initializaing.");
            activationGuid_ = activationGuid;
            bool success = false;

            try
            {
                CreateDiComponents();
                try
                {

                    //Get data from Source System or Local File Pickup
                    if (localFileSource == true)
                    {
                        //Get local file data
                        success = ProcessFiles(pickupFileFolderPath);
                    }
                    else
                    {

                    }

                }
                catch (Exception ex)
                {
                    Trace.WriteLine(prefix + "An exception was raised when calling the BC.Integration.AppService.JMSDispatcherSvc.Process.ProcessData method. Exception message: " + ex.Message);
                    instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.AppService.JMSDispatcherSvc.Process.ProcessData method.", ex);
                }
                finally
                {
                    instrumentation.FlushActivity();
                    container.Dispose();
                }
                Trace.WriteLineIf(tracingEnabled, prefix + "End processing data.");

            }
            catch (Exception ex)
            {
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                Trace.WriteLine(prefix + "An exception was raised when calling the BC.Integration.AppService.JMSDispatcherSvc.Process.ProcessData method trying to resolve the Unity DI components. Exception message: " + ex.Message);
                instrumentation.LogGeneralException("An exception occured in the BC.Integration.AppService.JMSDispatcherSvc.Process.ProcessData method trying to resolve the Unity DI components.", ex);
            }
            return success;
        }

        /// <summary>
        /// Works through a collection of URLs from the routing table and publishes the message to each queue.
        /// NOTE: The way this code is written if a message fails to be delivered to a queue all the process will stop
        /// and no additional queue will get a message.  The likely hood of one SQS queue being unavailable but not the 
        /// other one is low, so it maybe a non-issue, but in some scenarios it maybe important to consider.
        /// </summary>
        /// <param name="msgString">Message inc. envelope</param>
        /// <param name="filterCol">Filter collection for additional routing selection</param>
        private void PublishMessage(string msgString, HipKeyValuePairCollection filterCol)
        {
            int retryCount = 0;
            //Get SQS Queue URL's
            List<string> urls = config.GetDestinationQueueUrls(processName, serviceId, servicePostOperationId, msgType, filterCol.ConvertToKeyValuePairs());
            foreach (string url in urls)
            {
                publishService.Push(url, msgString, out retryCount);
                //adding comment
            }
        }
        private bool MoveFileTo(string filename, string destination)
        {
            bool success = false;
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + ".MoveFileTo() method is initializaing.  file : " + filename + " . Destination : " + destination);
            var fileNames = Directory.EnumerateFiles(pickupFileFolderPath.Replace("#ProcessName#", processName));
            string jms = Path.GetFileName(filename);
            FileAttributes attributes = File.GetAttributes(filename);

            try
            {
                if (!((attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly))
                {
                    attributes &= FileAttributes.ReadOnly;
                    File.SetAttributes(filename, attributes);
                    FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    fs.Lock(0, filename.Length);

                    FileStream fsCopy = new FileStream(destination.Replace("#ProcessName#", processName) + "/" + jms, FileMode.OpenOrCreate);
                    fs.CopyTo(fsCopy);

                    fs.Unlock(0, filename.Length);
                    fs.Close();
                    fs.Dispose();

                    fs = new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                    FileStream fsArchive = new FileStream(archiveFolderPath.Replace("#ProcessName#", processName) + "/" + jms, FileMode.OpenOrCreate);
                    fs.CopyTo(fsArchive);
                    fs.Close();

                    File.Delete(filename);

                    success = true;
                }
            }
            catch (Exception ex)
            {
                success = false;
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                Trace.WriteLine(prefix + "An exception was raised when calling the BC.Integration.AppService.JMSDispatcherSvc2.Process.MoveFileTo method" +
                    "trying to resolve the Unity DI components. Exception message: " + ex.Message);
                instrumentation.LogGeneralException(prefix +
                    "An exception was raised when calling the BC.Integration.AppService.JMSDispatcherSvc2.Process.MoveFileTo method", ex);
                instrumentation.LogNotification(processName, serviceId, new Guid(activationGuid_), "JMS", "JMS", filename);

            }
            finally
            {
                if (!success)
                {
                    attributes &= ~FileAttributes.ReadOnly;
                    File.SetAttributes(filename, attributes);
                }                
            }

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + ".MoveFileTo() method is initializaing.  file : " + filename + " . Destination : " + destination);
            return success;
        }
        private bool Dispatch(string file)
        {
            bool success = false;
            bool hasPayment = false;
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + ".Dispatch() method is initializaing.  Filename: " + file);
            XmlDocument JMSFile = new XmlDocument();

            try
            {
                string fileText = File.ReadAllText(file);
                JMSFile.LoadXml(fileText);
                String status = JMSFile.InnerText.Substring(0, 11);

                if (status.Contains("IN_PROGRESS")) //exchange or new order
                {
                    XmlElement order = JMSFile.DocumentElement;
                    XmlNodeList xnList = order.ChildNodes;
                    
                    foreach (XmlNode node in xnList)
                    {
                        if (node.Name.Equals("Payments"))
                        {
                            hasPayment = true;
                            foreach (XmlNode n in node.FirstChild.ChildNodes)
                            {
                                if (n.Name.Contains("PaymentMethod"))
                                {
                                    if (n.FirstChild.InnerText == "EXCHANGE") //if the payment method is exchange
                                    {
                                        success = MoveFileTo(file, exchangeFolderPath);
                                    }
                                    else
                                    {
                                        success = MoveFileTo(file, inProgressFolderPath); //this is a regular new order
                                    }
                                }
                            }
                        }

                    }
                    if (!hasPayment)
                    {
                        success = MoveFileTo(file, inProgressFolderPath); //100% discounted order
                    }
                }
                else if (status.Contains("CANCELLED"))
                { //cancelled

                    success = MoveFileTo(file, cancelledFolderPath);
                }
                else //completed or return
                {
                    XmlElement order = JMSFile.DocumentElement;
                    XmlNodeList xnList = order.ChildNodes;

                    foreach (XmlNode node in xnList)
                    {
                        if (node.Name.Equals("Returns"))
                        {
                            if (node.HasChildNodes)
                            {
                                foreach (XmlNode n in node.FirstChild.ChildNodes)
                                {

                                    if (n.Name.Contains("ReturnType")) // return type is either exchange or return
                                    {
                                        if (!String.IsNullOrEmpty(n.FirstChild.InnerText))
                                        {
                                            //this is a return
                                            success = MoveFileTo(file, returnFolderPath);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                //this is a regular completed message
                                success = MoveFileTo(file, completedFolderPath);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(tracingEnabled, prefix + " EXCEPTION Occurred: " + ex.Message);
                instrumentation.LogGeneralException("Error occurred in the " + prefix + ".Dispatch() method. " +
                   "The dispatching of the received file caused the component to fail and processing to stop. File: " + file, ex);
                instrumentation.LogNotification(processName, serviceId, new Guid(activationGuid_), "JMS", "JMS", file);

                return false;
            }
            finally
            {
                instrumentation.FlushActivity();
                Trace.WriteLineIf(tracingEnabled, prefix + " Finally block called and Dispatch() method complete.");
            }

            Trace.WriteLineIf(tracingEnabled, prefix + ".Dispatch() method completed.");
            return success;
        }

        /// <summary>
        /// This method processes files in a location folder.
        /// </summary>
        /// <param name="pickupMessageFolderPath">Folder path to pickup the files</param>
        /// <returns></returns>
        private bool ProcessFiles(string pickupMessageFolderPath)
        {
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + ".ProcessFiles() method is initializaing.  Source Folder: " + pickupMessageFolderPath);

            bool success = false;
            string path = pickupMessageFolderPath.Replace("#ProcessName#", processName);

            var fileNames = Directory.EnumerateFiles(path);
            foreach (string file in fileNames)
            {
                success = this.Dispatch(file);
            }
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + ".ProcessFiles() method is completed.");
            return success;
        }

        /// <summary>
        /// Instantiates the objects that are used via dependency injection
        /// </summary>
        private void CreateDiComponents()
        {
            //Create Unity container for DI and create DI components
            container = new UnityContainer();
            container.LoadConfiguration();
            config = container.Resolve<IConfiguration>();
            configuration = config.PopulateConfigurationCollectionFromAppConfig();
            OverrideConfigProperties(config);
            instrumentation = container.Resolve<IInstrumentation>();
            instrumentation.Configuration = configuration;
            publishService = container.Resolve<IPublishService>();
        }

    }
}