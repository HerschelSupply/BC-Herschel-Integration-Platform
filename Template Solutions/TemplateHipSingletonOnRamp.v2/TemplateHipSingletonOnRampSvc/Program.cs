using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using BC.Integration.Interfaces;
using BC.Integration.Utility;
using System.Threading;
using System.Xml;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Text;

namespace BC.Integration.AppService.TemplateHipSingletonOnRamp
{
    class Program
    {
        static void Main(string[] args)
        {
            string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
            string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
            Trace.WriteLine(tracingPrefix + "Service Main method is being executed.");

            if (args.Length != 2)
                throw new Exception("The TemplateHipSingletonOnRamp app service takes an activation GUID string as the only argument.");

            string guid = args[0];
            string processName = args[1];
            Trace.WriteLine(tracingPrefix + "Service executable initiated.  Starting Process Controller.");

            try
            {
                TemplateHipSingletonOnRamp svc = new TemplateHipSingletonOnRamp();
                svc.ProcessController(guid, processName);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + "Exception occurred whilst starting the Process Controller.");
                throw new Exception("An error occurred trying to initialize the TemplateHipSingletonOnRamp service Process Controller. Please review the inner exception for details of the error.", ex);
            }
        }
    }


    public class TemplateHipSingletonOnRamp
    {
        #region Properties
        private static Mutex mutex = null; //Used to block multiple istances working on the same queue

        private List<KeyValuePair<string, string>> configuration = null;

        //Dependency Injection Components
        IConfiguration config;
        IInstrumentation instrumentation;
        IPublishService publishService;
        UnityContainer container;

        //Key service Properties that should never change from design time
        private string processName = "";
        private string serviceId = ConfigurationManager.AppSettings["ServiceId"];
        private string serviceOperationId = ConfigurationManager.AppSettings["ServiceOperationId"];
        private string servicePostOperationId = ConfigurationManager.AppSettings["ServicePostOperationId"];
        private string centralConfigConnString = ConfigurationManager.AppSettings["CentralConfigConnString"];
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";

        //These rest of the properties could be overridden by the central config store at the service ot process level
        private string archiveFolder = "";

        //Msg Envelope properties
        private string msgType = "";
        private string filterKeyValuePairs = "";
        private string processKeyValuePairs = "";
        private string topic = "";

        //Mapping properties
        private string shopIdToSiteIdMapping = "";

        //Messaging Queue Properties
        private string queueUrl = "";

        //private string batchData = "";
        private MessageManager msgMgr = new MessageManager();
        private string batchName = "";
        private string documentNumber;
        private XmlDocument outgoingMessage;
        private string messageFormat = "xml";
        private int serviceVersion = 1;
        private int messageVersion = 1;
        private bool tracingEnabled = false;
        private bool localFileSource = false;
        private string exceptionMessageFolderPath = "";
        private string tracingMessageFolderPath = "";
        private string pickupFileFolderPath = "";

        //LS Throttling and Authentication variables 
        private string lightspeedTokenRefreshUrl = "";
        private string refreshToken = "";
        private string clientKey = "";
        private string clientSecret = "";
        private string lightspeedRootUrl = "";
        private string lightspeedCustomerAccountId = "";

        private string apiBatchSize = "11";
        private decimal bucketSize = 0;
        private decimal bucketLevel = 0;
        private decimal bucketDripRate = 0;
        private int apiGetPoints = 1;
        private int apiPutPoints = 10;
        private string accessToken = "";
        private DateTime accessTokenTimeout = DateTime.Now;
        private DbAccess db = new DbAccess();
        private string externalDocTrackingConn = "";
        private Stopwatch stopwatch = new Stopwatch();

        #endregion

        #region Central Configuration methods
        /// <summary>
        /// Retrives the service and global central config values from the store.  The service properties are applied to the 
        /// service first, so global properties will override all properties.
        /// </summary>
        /// <param name="config">The configuration class to implement the retrieval of the central configuration</param>
        private void OverrideConfigProperties(IConfiguration config)
        {
            configuration = config.GetConfiguration(serviceId, processName);
            PopulateLocalVaribale();
        }

        /// <summary>
        /// Applies the central configurations to the local variables.
        /// </summary>
        private void PopulateLocalVaribale()
        {
            archiveFolder = Utilities.GetConfigurationValue(configuration, "ArchiveFolder");
            exceptionMessageFolderPath = Utilities.GetConfigurationValue(configuration, "ExceptionMessageFolderPath");
            tracingMessageFolderPath = Utilities.GetConfigurationValue(configuration, "TracingMessageFolderPath");
            pickupFileFolderPath = Utilities.GetConfigurationValue(configuration, "PickupFileFolderPath");
            msgType = Utilities.GetConfigurationValue(configuration, "MsgType");
            messageVersion = Convert.ToInt16(Utilities.GetConfigurationValue(configuration, "MessageVersion"));
            filterKeyValuePairs = Utilities.GetConfigurationValue(configuration, "FilterKeyValuePairs");
            processKeyValuePairs = Utilities.GetConfigurationValue(configuration, "ProcessKeyValuePairs");
            topic = Utilities.GetConfigurationValue(configuration, "Topic");
            queueUrl = Utilities.GetConfigurationValue(configuration, "QueueUrl");
            lightspeedTokenRefreshUrl = Utilities.GetConfigurationValue(configuration, "LightspeedTokenRefreshUrl");
            refreshToken = Utilities.GetConfigurationValue(configuration, "RefreshToken");
            clientKey = Utilities.GetConfigurationValue(configuration, "ClientKey");
            clientSecret = Utilities.GetConfigurationValue(configuration, "ClientSecret");
            lightspeedRootUrl = Utilities.GetConfigurationValue(configuration, "LightspeedRootUrl");
            lightspeedCustomerAccountId = Utilities.GetConfigurationValue(configuration, "LightspeedCustomerAccountId");
            externalDocTrackingConn = Utilities.GetConfigurationValue(configuration, "ExternalDocTrackingConn");
            apiBatchSize = Utilities.GetConfigurationValue(configuration, "ApiBatchSize");
            apiGetPoints = Convert.ToInt16(Utilities.GetConfigurationValue(configuration, "ApiGetPoints"));
            apiPutPoints = Convert.ToInt16(Utilities.GetConfigurationValue(configuration, "ApiPutPoints"));
            shopIdToSiteIdMapping = Utilities.GetConfigurationValue(configuration, "ShopIdToSiteIdMapping");

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
        /// This On-Ramp can process to types of messages Sales and Inventory Transfers.  The standard process will be for 
        /// the service to process all sales transactions followed by any inventory transfers.  The On-Ramp can also be
        /// initialized to only process sales or inventory transfers by passing the specific process name.
        /// Process names: Process B or Process A or All
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="process"></param>
        public void ProcessController(string guid, string process)
        {
            try
            {
                CreateDiComponents();
                try
                {
                    //Ensure that this application cannot be started twice to process the same queue.
                    string appName = serviceId + process;
                    bool createNew;
                    mutex = new Mutex(true, appName, out createNew);
                    if (!createNew)
                    {
                        instrumentation.LogActivation(serviceId, new Guid(guid), true);
                        Trace.WriteLine(tracingPrefix + "Service already running for this process (" + process + "). This instance will terminate.");
                        return; //terminates ProcessController method and ends service activation.
                    }
                    Trace.WriteLine(tracingPrefix + "Service starting with a Process Name of " + process + ".");

                    //Log Activation
                    instrumentation.LogActivation(serviceId, new Guid(guid), false);

                    if (process == "Process A")
                    {
                        ProcessData(guid, process);
                    }
                    else if(process == "Process B")
                    {
                        ProcessData(guid, process);
                    }
                    else if(process == "Both")
                    {
                        ProcessData(guid, "Process A");
                        ProcessData(guid, "Process B");
                    }
                    //Log Activation End (This step will be skipped if an exception occurs)
                    instrumentation.LogActivationEnd(new Guid(guid));
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + "Error occurred in the BC.Integration.AppService.TemplateHipSingletonOnRamp.ProcessController method. " +
                        "Exception message: " + ex.Message);
                    instrumentation.LogGeneralException("Error occurred in the BC.Integration.AppService.TemplateHipSingletonOnRamp.ProcessController method " +
                        "processing. Please review inner exceptions for details: ", ex);
                }
                finally
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                    mutex.Dispose();
                }
            }
            catch (Exception ex)
            {
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                Trace.WriteLine(tracingExceptionPrefix + "An exception occurred in the BC.Integration.AppService.TemplateHipSingletonOnRamp ProcessController method trying to resolve the Unity DI components. Exception message: " + ex.Message);
                throw new Exception("An exception occurred in the BC.Integration.AppService.TemplateHipSingletonOnRamp ProcessController method trying to resolve the Unity DI components.", ex);
            }
        }

        /// <summary>
        /// This method controls the flow of requests.  It gets data from the appropriate source and sends it to the appropriate method to
        /// be mapped and sent to the message queue.
        /// </summary>
        /// <param name="activationGuid">This is used to link between the scheduler activation event and the service activity.</param>
        private void ProcessData(string activationGuid, string process)
        {
            processName = process;
            bool continueProcessingFlag = true;
            try
            {
                if (processName == "Process B")
                {
                    Sales orders;
                    //Get data from Source System or Local File Pickup
                    if (!localFileSource)
                    {
                        //Get data from source system (collection size determined by ApiBatchSize in config file)
                        while (continueProcessingFlag)
                        {
                            continueProcessingFlag = GetNextTransaction(out orders);
                            if (orders == null) //If order = null, no documents were returned.
                            {
                                Trace.WriteLine(tracingPrefix + "Count of API records to be processed: 0");
                                break;
                            }
                            if (orders.Sale != null)
                            {
                                Trace.WriteLine(tracingPrefix + "Count of API records to be processed: " + orders.Sale.Length + ".");
                            }
                            else
                            {
                                Trace.WriteLine(tracingPrefix + "Count of API records to be processed: 0");
                            }
                            batchName = "ApiData";
                            //Process sales order
                            ProcessDocuments(orders);
                        }
                    }
                    else //Get local file data
                    {
                        batchName = "LocalFile";
                        orders = GetDataFiles(pickupFileFolderPath);
                        if (orders.Sale != null)
                        {
                            orders.count = 1;
                            ProcessDocuments(orders);
                        }
                    }
                }
                else if (processName == "Process A")
                {
                    throw new NotImplementedException("Only Process B has been implemented. " +
                        " Process A will be implemented in a second phase.");
                }
                else
                {
                    throw new NotImplementedException("Only Process B has been implemented.");
                }

                //Log Activation End (This step will be skipped if an exception occurs)
                instrumentation.LogActivationEnd(new Guid(activationGuid.Substring(0, 36)));
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End processing available API transactions.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the BC.Integration.AppService.TemplateHipSingletonOnRamp.ProcessData method. Exception message: " + ex.Message);
                instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.AppService.TemplateHipSingletonOnRamp.ProcessData method.", ex);
            }
            finally
            {
                instrumentation.FlushActivity();
                container.Dispose();
            }
        }

        #region Document Processing

        /// <summary>
        /// This method is responsible for processing a collection of transactions.  Each transaction needs to be check to 
        /// see if it is complete.  If it is not it, the doc ID will be saved to the ExternalDocPendingProcessing table
        /// for reprocessing later.  After each order is successfully processed the Doc ID will be used to update the 
        /// last processed doc ID in the ExternalDocTracking table.
        /// *** May want to add code to terminate process if the current transaction Timestamp is less that 5 minutes ago.  This
        /// could avoid orders being placed in the ExternalDocPendingProcessing due to the transaction being in process when the 
        /// data pull from the source occurred.***
        /// </summary>
        /// <param name="salesOrder"></param>
        private void ProcessDocuments(Sales orders)
        {
            //Process message collection if it contains one or more messages
            if (orders.count > 0)
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "BC.Integration.AppService.TemplateHipSingletonOnRamp data retrieved.");
                //Create initial msg envelope to represent the start of the service process and call Instrumenation
                XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, msgType, messageVersion, messageFormat, batchName);

                instrumentation.LogActivity(msg);
                //Log Activation Association with Interchange
                int i = 1;
                foreach (SalesSale order in orders.Sale)
                {
                    try
                    {
                        documentNumber = order.saleID; //Set document number to support tracking and troubleshooting.
                        Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Processing message collection.  Document Number: " + documentNumber);

                        try
                        {
                            string canonicalMsg;
                            //Map to the canonical structure
                            try
                            {
                                canonicalMsg = Mapper.Transform();
                            }
                            catch (Exception ex)
                            {
                                instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "On-Ramp", ex.Message, order.saleID);
                                throw new Exception("Exception occurred trying to map from the to the canonical schema.  " +
                                    "The exception occurred in BC.Integration.AppService.TemplateHipSingletonOnRamp.Mapper.Transform method.", ex);
                            }

                            //Create envelope and add canonical message to the envelope
                            HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filterKeyValuePairs);
                            HipKeyValuePairCollection processCol = new HipKeyValuePairCollection(processKeyValuePairs);
                            outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, topic,
                                                                        filterCol, processCol, canonicalMsg, i, null, documentNumber);

                            //Place message on the message bus
                            PublishMessage(outgoingMessage.InnerXml, filterCol);
                        }
                        catch (Exception ex)
                        {
                            db.InsertExternalDocPendingProcessing(externalDocTrackingConn, processName, "Sales", order.saleID, order.ConvertToString(order), ex.Message);
                            instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "On-Ramp", "The message failed to be placed on the message queue.  Please review the exception logs to determine the issue.", order.saleID);
                            Trace.WriteLine(tracingExceptionPrefix + "An exception occurred while mapping and publishing the message in BC.Integration.AppService.TemplateHipSingletonOnRamp.Process.ProcessData. Exception message: " + ex.Message);
                            instrumentation.LogMessagingException("An exception occurred while mapping and publishing the message in " +
                                "BC.Integration.AppService.TemplateHipSingletonOnRamp.Process.ProcessData method. The current process is - " + processName +
                                " document number being processed is " + documentNumber + ".", 
                                outgoingMessage, ex);
                            continue;
                        }
                        finally
                        {
                            //Save a copy of message
                            if (tracingEnabled)
                            {
                                string site = order.shopID;
                                Sales sales = new Sales(order);

                                string data = orders.ConvertTransactionToString(sales);

                                SaveMessageToFile(data, serviceId + "." + order.saleID + "." + site, false);
                            }
                        }

                        //External document tracking updates...
                        if (processName == "Process B")
                        {
                            if (!localFileSource)
                            {
                                //Set the last transaction processed.
                                db.UpdateLastDocProcessed(externalDocTrackingConn, processName, "Sales", order.saleID, 
                                                            order.timeStamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss"));
                            }
                        }

                        //Log Activity
                        int retryCount = 0;
                        instrumentation.LogActivity(outgoingMessage, queueUrl, retryCount);
                        Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End processing: " + documentNumber);
                    }
                    catch (Exception ex)
                    {
                        db.InsertExternalDocPendingProcessing(externalDocTrackingConn, processName, "Sales", order.saleID, outgoingMessage.InnerXml, ex.Message);
                        instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "On-Ramp", "The message failed to be processed by the ProcessDocuments method.  Please review the exception logs to determine the issue.", order.saleID);
                        Trace.WriteLine(tracingExceptionPrefix + "An exception occurred while processing data in BC.Integration.AppService.TemplateHipSingletonOnRamp.Process.ProcessDocuments. Exception message: " + ex.Message);
                        instrumentation.LogMessagingException("An exception occurred while processing a message in the " +
                            "BC.Integration.AppService.TemplateHipSingletonOnRamp.Process.ProcessDocuments method. The current process is - " + processName +
                            " document number being processed is " + documentNumber + ".", outgoingMessage, ex);
                    }
                    i++;
                }
            } //end while loop
        }

        /// <summary>
        /// This method is used to get sales orders from the Lightspeed API.  It checks the ExternalDocIdTracking table to 
        /// determine the start timestamp for getting each batch of orders.  Orders will be limited to 11 orders and the 
        /// first order should be the last order processed.  Return bool will be set to false if this is the last collection
        /// that needs to be processed.
        /// </summary>
        /// <param name="orders">Collection of orders from Lightspeed</param>
        /// <returns>flag to terminate processing once current orders are processed.</returns>
        private bool GetNextTransaction(out Sales orders) //Return sales object collection
        {
            Trace.WriteLine(tracingPrefix + "Run GetNextTransaction method.");
            //Determine next order number and order date to call
            string lastOrder = db.GetLastDocProcessed(externalDocTrackingConn, processName, "Sales");
            string orderId = lastOrder.Substring(0, lastOrder.IndexOf("|"));
            string date = lastOrder.Substring(lastOrder.IndexOf("|") + 1);

            string endPoint = "Some Endpoint";
            string data = GetDataApi(endPoint);

            //_____________  Add code to get transactions from source_______________________________________

            orders = new Sales();

            //_______________________________________________________________________________________________

            if (orders.count < orders.limit)
            {
                return false; //return false to end the looping process.
            }
            return true; 
        }

        /// <summary>
        /// Takes data from a file and converts it to a Sales Order object.  The data is retrieved from the 
        /// standard GetFileSourceData method.
        /// </summary>
        /// <param name="pickupFileFolderPath"></param>
        /// <returns></returns>
        private Sales GetDataFiles(string pickupFileFolderPath)
        {
            Sales orders = new Sales();
            try
            {
                string data = GetFileSourceData(pickupFileFolderPath);
                if (data != "")
                {
                    return orders.ConvertToOm(data);
                }
                return orders;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the BC.Integration.AppService.TemplateHipSingletonOnRamp.GetFileSourceSalesOrders method. " +
                    "The method uses GetFileSourceData to get data from a file and then de-serializes the XML to a Sales object. Exception message: " + ex.Message);
                throw new Exception("An exception was raised when calling the BC.Integration.AppService.TemplateHipSingletonOnRamp.GetFileSourceSalesOrders method. " +
                    "The method uses GetFileSourceData to get data from a file and then de-serializes the XML to a Sales object.", ex);
            }
        }


        #endregion


        /// <summary>
        /// This method gets the data from an API, as defined by the endpoint parameter and returns
        /// the data as a string.
        /// </summary>
        /// <param name="endPoint">Endpoint including serach parameters.</param>
        /// <returns>Lightspeed data as a JSON string</returns>
        private string GetDataApi(string endPoint)
        {
            string responseValue = "";
            return responseValue;
        }

        #region Standard On-Ramp Methods

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
            }
        }

        /// <summary>
        /// This method allows the pickup of files from a local folder.
        /// </summary>
        /// <param name="pickupMessageFolderPath">Folder path to pickup the files</param>
        /// <returns></returns>
        private string GetFileSourceData(string pickupMessageFolderPath)
        {
            //This is setup for JSON data...
            string fileText = "";

            string path = pickupMessageFolderPath.Replace("#ProcessName#", processName);

            var fileNames = Directory.EnumerateFiles(path);
            foreach (string file in fileNames)
            {
                Trace.WriteLineIf(tracingEnabled, "Get file messages.  Filename: " + file);
                fileText = File.ReadAllText(file);
                batchName = file.Substring(file.LastIndexOf('\\') + 1);
                //Remove file after processing
                File.Delete(file);
            }

            return fileText;
        }

        /// <summary>
        /// Helper method to standardize behaviour when saving files and update the process name tags.
        /// </summary>
        /// <param name="message">Message (data) to be saved to files</param>
        /// <param name="fileName">Provide the start of the file name. Format for tracing: 'svcID.Doc#.SiteID', for exceptions: ‘svcID.Doc#.SiteID.ValidationType’
        /// Or ‘SvcID.ValidationType’ depending what variables are available.  Time and '.txt' will be added later. </param>
        /// <param name="exception">Flag to identify which folder path to use (Exception or Tracing)</param>
        private void SaveMessageToFile(string message, string fileName, bool exception)
        {
            if (!tracingEnabled && !exception) //If tracing is not enabled and it is not an exception exit method.
                return;

            string path;
            if (exception)
                path = exceptionMessageFolderPath.Replace("#ProcessName#", processName);
            else
                path = tracingMessageFolderPath.Replace("#ProcessName#", processName);

            //fileName Time and txt will be added by instrumentation.
            instrumentation.WriteMsgToFile(path, null, message, fileName, null);
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
            publishService.Configuration = configuration;
        }

        #endregion
    }

}
