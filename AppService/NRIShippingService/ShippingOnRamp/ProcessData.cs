using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using BC.Integration.Interfaces;
using BC.Integration.Utility;
using System.Xml;
using System.Net;
using System.IO;
using System.Text;
using BC.Integration.Canonical.NRI;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace BC.Integration.AppService.NriShippingOnRampService
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
        
        //These rest of the properties could be overridden by the central config store at the service ot process level
        private string archiveFolder = "";
        private string exceptionMessageFolderPath = "";
        private string tracingMessageFolderPath = "";
        private string pickupConfirmationFolderPath = "";
        private string pickupCancellationFolderPath = "";
        //Msg Envelope properties
        private string msgType = "";
        private string filter = "";
        private string process = "";
        
        //Messaging Queue Properties
        private string queueUrl = "";

        private MessageManager msgMgr = new MessageManager();
        private string batchName = "";
        private string documentNumber;
        private XmlDocument outgoingMessage;
        private string messageFormat = "xml";
        private int serviceVersion = 1;
        private int messageVersion = 1;
        private bool tracingEnabled = false;
        private string activeUrl = "";
        private string activeKey = "";
        private string siteMapping = "Logic";

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
            archiveFolder = Utilities.GetConfigurationValue(configuration, "ArchiveFolder");
            exceptionMessageFolderPath = Utilities.GetConfigurationValue(configuration, "ExceptionMessageFolderPath");
            tracingMessageFolderPath = Utilities.GetConfigurationValue(configuration, "TracingMessageFolderPath");
            pickupConfirmationFolderPath = Utilities.GetConfigurationValue(configuration, "PickupConfirmationFolderPath");
            pickupCancellationFolderPath = Utilities.GetConfigurationValue(configuration, "PickupCancellationFolderPath");
            msgType = Utilities.GetConfigurationValue(configuration, "MsgType");
            filter = Utilities.GetConfigurationValue(configuration, "Filter");
            process = Utilities.GetConfigurationValue(configuration, "Process");
            queueUrl = Utilities.GetConfigurationValue(configuration, "QueueUrl");
            string val = Utilities.GetConfigurationValue(configuration, "TracingEnabled");
            if (val != "")
            {
                if (val.ToLower() == "true" || val.ToLower() == "false")
                    tracingEnabled = Convert.ToBoolean(val);
                if (val == "1" || val == "0")
                    tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
            }
            siteMapping = Utilities.GetConfigurationValue(configuration, "SiteMapping");
        }

        #endregion

        /// <summary>
        /// Process Data is the main method in the BC.Integration.AppService.NriShippingOnRampService class.  It implements the following steps;
        /// -Resolves the Unity DI classes.
        /// -Overrides local property setting with the central configuration using the IConfiguration component.
        /// -Connect to the source system and checks to see if there are available files
        /// -Download data
        /// -Log activity
        /// -Process the data and spliting it into seperate messages
        /// -Log activity for each message
        /// </summary>
        public void ProcessData(string activationGuid)
        {
            try
            {
                CreateDiComponents();
                try
                {
                    //Log Activation Start
                    instrumentation.LogActivation(serviceId, new Guid(activationGuid.Substring(0, 36)), false);

                    int collectionCount = 6;
                    string nriData;
                    //There is not a batch number for this process so use the activation GUID.
                    batchName = activationGuid;

                    //_____________________________________________________________________________________________
                    //The code is going to need to make 4 calls.  Since the API's are split between US and Canada each
                    //region will need to be called for shipment confirmations and cancellations.
                    string canConfirmUrl = Utilities.GetConfigurationValue(configuration, "CanConfirmUrl"); 
                    string canCancelUrl = Utilities.GetConfigurationValue(configuration, "CanCancelUrl"); 
                    string canAuthorization = Utilities.GetConfigurationValue(configuration, "CanAuthKey");
                    string usaConfirmUrl = Utilities.GetConfigurationValue(configuration, "UsaConfirmUrl"); 
                    string usaCancelUrl = Utilities.GetConfigurationValue(configuration, "UsaCancelUrl"); 
                    string usaAuthorization = Utilities.GetConfigurationValue(configuration, "UsaAuthKey");

                    for (int i = 0; i < collectionCount; i++)
                    {
                        nriData = "";
                         if (i == 0) //Get Canadian Shipping Confirmations
                         {
                             nriData = GetSourceData(canConfirmUrl, canAuthorization);
                             batchName = "NRI Canadian Shipment Confirmation API";
                             activeUrl = canConfirmUrl;
                             activeKey = canAuthorization;
                         }
                         else if (i == 1) //Get US Shipping Confirmations
                         {
                             nriData = GetSourceData(usaConfirmUrl, usaAuthorization);
                             batchName = "NRI US Shipment Confirmation API";
                             activeUrl = usaConfirmUrl;
                             activeKey = usaAuthorization;
                         }
                         else if (i == 2)  //Get local Confirmation Shipments
                         {
                             nriData = GetFileSourceData(pickupConfirmationFolderPath);
                             batchName = "Local confirmation file processing";
                             activeUrl = "Not Applicable";
                             activeKey = "Not Applicable"; 
                         }
                          else
                        if (i == 3) //Get Canadian Cancelled Shipments
                        {
                            nriData = GetSourceData(canCancelUrl, canAuthorization);
                            batchName = "NRI Canadian Cancelled Shipping Orders API";
                            activeUrl = canCancelUrl;
                            activeKey = canAuthorization;
                        }
                        else if (i == 4)  //Get US Cancelled Shipments
                        {
                            nriData = GetSourceData(usaCancelUrl, usaAuthorization);
                            batchName = "NRI US Cancelled Shipping Orders API";
                            activeUrl = usaCancelUrl;
                            activeKey = usaAuthorization;
                        }
                       else //Get local Cancelled Shipments
                        {
                            nriData = GetFileSourceData(pickupCancellationFolderPath);
                            batchName = "Local cancellation file processing";
                            activeUrl = "Not Applicable";
                            activeKey = "Not Applicable"; 
                        }

                        //Process message collection if it contains one or more messages
                        if (nriData != "")
                        {
                            Trace.WriteLineIf(tracingEnabled, "NriShippingOnRampServiceBC: BC.Integration.AppService.NriShippingOnRampServiceBC data retrieved found. Batch name: " + batchName);
                            //Create initial msg envelope to represent the start of the service process and call Instrumentation.
                            XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, msgType, messageVersion, messageFormat, batchName);
                            instrumentation.LogActivity(msg);

                            //Log Activation Association with Interchange
                            if(tracingEnabled) //Only track this if tracing is enabled.  This is due to the performance impact of making the ODBC connections
                                instrumentation.AssociateActivationWithInterchages(new Guid(activationGuid.Substring(0, 36)), msgMgr.EntryPointEnvelope.Interchange.InterchangeId);

                            if (i < 3)
                                ProcessShipmentConfirmData(nriData, msgMgr.EntryPointEnvelope.Msg.Id);
                            else
                                ProcessCancelledOrdersData(nriData, msgMgr.EntryPointEnvelope.Msg.Id);

                        } //end if

                    }//end for loop

                    //Log Activation End (This step will be skipped if an exception occurs)
                    instrumentation.LogActivationEnd(new Guid(activationGuid.Substring(0, 36)));
                    Trace.WriteLineIf(tracingEnabled, "End processing all batches.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("NriShippingOnRampServiceBC: An exception was raised when calling the BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessData method. Exception message: " + ex.Message);
                    instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessData method.", ex);
                }
                finally
                {
                    instrumentation.FlushActivity();
                    container.Dispose();
                }
            }
            catch (Exception ex)
            {
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                Trace.WriteLine("NriShippingOnRampServiceBC: An exception was raised when calling the BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessData method trying to resolve the Unity DI components. Exception message: " + ex.Message);
                throw new Exception("An exception occured in the BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessData method trying to resolve the Unity DI components.", ex);
            }
        }

        /// <summary>
        /// Process shipment confirmation data received from NRI
        /// </summary>
        /// <param name="nriShipmentData">shipment confirmation data</param>
        private void ProcessShipmentConfirmData (string nriShipmentData, Guid messageId)
        {
            List<string> processedOrderIds = new List<string>();
            List<Nri.ShippingConfirmation.ShippingConfirmation> list = new List<Nri.ShippingConfirmation.ShippingConfirmation>();
            try
            {
                //Deserialize the data.
                list = Nri.ShippingConfirmation.ShippingConfirmation.FromJson(nriShipmentData);
            }
            catch (Exception ex)
            {
                SaveMessageToFile(nriShipmentData, serviceId + ".Deserialization", true);
                Trace.WriteLine("NriShippingOnRampServiceBC: An exception occurred while deserializing shipment confirmation data in BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessShipmentConfirmData. Exception message: " + ex.Message);
                instrumentation.LogGeneralException("An exception occured while deserializing shipment confirmation data in the " +
                    "BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessShipmentConfirmData method. The current process is - " + processName +
                    ".", ex);
                return;
            }

            Trace.WriteLineIf(tracingEnabled, "###NriShippingOnRampServiceBC (Confirmations): Start adding the order collection to the queue.  Number of orders to process is " + list.Count + " orders.");
            int i = 0;
            foreach (Nri.ShippingConfirmation.ShippingConfirmation message in list)
            {
                try
                {
                    if (message.BillToCustomerCode != "ONLNBF001")  //we are not going to process Borderfree orders
                    {
                        //Set document number in envelope to support tracking and troubleshooting.
                        if (message.ClientReferenceNumber1 == null || message.ClientReferenceNumber1 == "")
                            documentNumber = "NRI OrderID " + message.OrderId;
                        else
                            documentNumber = message.ClientReferenceNumber1;
                        Trace.WriteLineIf(tracingEnabled, "NriShippingOnRampServiceBC: Processing shipment confirmation message collection.  Processing document number: " + documentNumber);

                        if (tracingEnabled)
                        {
                            string site = "";
                            if (message.Data.ClientWarehouseCode == null || message.Data.ClientWarehouseCode == "")
                                site = "unknown";
                            else
                                site = message.Data.ClientWarehouseCode;
                            SaveMessageToFile(Nri.ShippingConfirmation.Serialize.OrderToJson(message), serviceId + "." + documentNumber + "." + site, false);
                        }

                        //Map to the canonical structure
                       
                        string canonicalMsg;

                        try
                        {
                            canonicalMsg = Mapper.ConvertShipmentConfirm(message, siteMapping);

                        }
                        catch (Exception ex)
                        {
                            string filename = serviceId;
                            string docId = "unknown";
                            if (message.ClientReferenceNumber1 != null || message.ClientReferenceNumber1 != "")
                            {
                                filename += "." + message.ClientReferenceNumber1;
                                docId = message.ClientReferenceNumber1;
                            }
                            if (message.Data != null)
                            {
                                if (message.ClientWarehouseCode != null || message.ClientWarehouseCode != "")
                                {
                                    filename += "." + message.ClientWarehouseCode;
                                }
                            }

                            instrumentation.LogNotification(processName, serviceId, messageId, "Mapping", ex.Message, docId);

                            SaveMessageToFile(ex.Message + "\r\n" + Nri.ShippingConfirmation.Serialize.OrderToJson(message),
                            filename + ".Mapping", true);
                            throw new Exception("An exception occurred while mapping the shipping confirmation " + message.ClientReferenceNumber1 + " to the canonicalShippingConfirmation message type " +
                                "in BC.Integration.AppService.NriShippingOnRampServiceBC.Mapper.ConvertShipmentConfirm.  A copy of the failed message has been saved to the file system.", ex);
                        }

                        //Create envelope and add canonical message to the envelope
                        HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filter);
                        //outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, "", filterCol, null,  canonicalMsg, i, null, documentNumber);
                        outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, "", filterCol, null, canonicalMsg.ToString(), i, null, documentNumber);

                        //Place message on the message bus
                        int retryCount = 0;
                        publishService.Configuration = configuration;
                        publishService.Push(queueUrl, outgoingMessage.InnerXml, out retryCount);

                        //Add order ID to a list for PUT call logic to confirm processing of message
                        processedOrderIds.Add(message.OrderId);

                        //Log Activity
                        instrumentation.LogActivity(outgoingMessage, queueUrl, retryCount);
                        Trace.WriteLineIf(tracingEnabled, "NriShippingOnRampServiceBC: End processing: " + documentNumber);
                        i++;
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("NriShippingOnRampServiceBC: An exception occurred while processing the shipping confirmation " + message.OrderId + " in BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessShipmentConfirmData. Exception message: " + ex.Message);
                    instrumentation.LogMessagingException("An exception occurred while processing the shipping confirmation " + message.OrderId + " in the " +
                        "BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessShipmentConfirmData method. The current process is - " + processName +
                        " document number being processed is " + documentNumber + ".", outgoingMessage, ex);
                }
            } //End Foreach Loop

            //Confirm the processing of the orders by making a PUT call
            ConfirmSuccessfullProcessing(processedOrderIds);
            processedOrderIds.Clear();
            Trace.WriteLineIf(tracingEnabled, "End processing " + batchName );

        }

        /// <summary>
        /// Process cancelled order data received from NRI
        /// </summary>
        /// <param name="nriCancelledOrderData">Cancelled order data</param>
        private void ProcessCancelledOrdersData (string nriCancelledOrderData, Guid messageId)
        {
            List<string> processedOrderIds = new List<string>();
            List<Nri.CancelledShippingOrder.CancelledShippingOrder> list = new List<Nri.CancelledShippingOrder.CancelledShippingOrder>();
            try
            {
                list = Nri.CancelledShippingOrder.CancelledShippingOrder.FromJson(nriCancelledOrderData);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("NriShippingOnRampServiceBC: An exception occurred while deserializing cancelled order data in BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessCancelledOrdersData. Exception message: " + ex.Message);
                instrumentation.LogGeneralException("An exception occured while deserializing cancelled order data in the " +
                    "BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessCancelledOrdersData method. The current process is - " + processName +
                    ".", ex);
                SaveMessageToFile(nriCancelledOrderData, "Deserialization", true);
                return;
            }

            Trace.WriteLineIf(tracingEnabled, "###NriShippingOnRampServiceBC (Cancellations): Start adding the order collection to the queue.  Number of orders to process is " + list.Count + " orders.");
            int i = 0;
            foreach (Nri.CancelledShippingOrder.CancelledShippingOrder message in list)
            {
                try
                {
                    //Set document number in envelope to support tracking and troubleshooting.
                    if (message.ClientReferenceNumber1 == null || message.ClientReferenceNumber1 == "")
                        documentNumber = "NRI OrderID " + message.OrderId;
                    else
                        documentNumber = message.ClientReferenceNumber1;
                   
                    Trace.WriteLineIf(tracingEnabled, "NriShippingOnRampServiceBC: Processing cancelled shipment order collection.  Processing document number: " + documentNumber);

                    if (tracingEnabled)
                    {
                        string site = "";
                        if (message.Data != null)
                        {
                            if (message.Data.WarehouseCode != null || message.Data.WarehouseCode != "")
                                site = message.Data.WarehouseCode;
                            else
                                site = "unknown";
                        }
                        SaveMessageToFile(Nri.CancelledShippingOrder.Serialize.OrderToJson(message), serviceId + "." + documentNumber + "." + site, false);
                    }
                    //Map to the canonical structure
                    string canonicalMsg;
                    
                    try
                    {
                        canonicalMsg = Mapper.ConvertCancelled(message);
                    }
                    catch (Exception ex)
                    {
                        string filename = serviceId;
                        string docId = "unknown";
                        if (message.ClientReferenceNumber1 != null || message.ClientReferenceNumber1 != "")
                        {
                            filename += "." + message.ClientReferenceNumber1;
                            docId = message.ClientReferenceNumber1;
                        }
                        if (message.Data != null)
                        {
                            if (message.Data.WarehouseCode != null || message.Data.WarehouseCode != "")
                            {
                                filename += "." + message.Data.WarehouseCode;
                            }
                        }

                        instrumentation.LogNotification(processName, serviceId, messageId, "Mapping", ex.Message, docId);

                        SaveMessageToFile(ex.Message + "\r\n" + Nri.CancelledShippingOrder.Serialize.OrderToJson(message),
                            filename + ".Mapping", true);
                        throw new Exception("An exception occurred while mapping the cancelled shipping order " + message.ClientReferenceNumber1 + " to the canonicalShippingConfirmation message type " +
                            "in BC.Integration.AppService.NriShippingOnRampServiceBC.Mapper.ConvertShipmentConfirm.  A copy of the failed message has been saved to the file system.", ex);
                    }
                   

                    //Create envelope and add canonical message to the envelope
                    HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filter);
                    HipKeyValuePairCollection processCol = new HipKeyValuePairCollection(process);
                    outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, "", filterCol, null, canonicalMsg.ToString(), i, null, documentNumber);

                    //Place message on the message bus
                    int retryCount = 0;
                    publishService.Configuration = configuration;
                    publishService.Push(queueUrl, outgoingMessage.InnerXml, out retryCount);

                    //Add order ID to a list for PUT call logic to confirm processing of message
                    processedOrderIds.Add(message.OrderId);

                    //Log Activity
                    instrumentation.LogActivity(outgoingMessage, queueUrl, retryCount);
                    Trace.WriteLineIf(tracingEnabled, "End processing: " + documentNumber);
                    i++;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("NriShippingOnRampServiceBC: An exception occurred while processing cancelled shipping orders " + message.OrderId + "  in BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessCancelledOrdersData. Exception message: " + ex.Message);
                    instrumentation.LogMessagingException("An exception occurred while processing cancelled shipping orders " + message.OrderId + " in the " +
                        "BC.Integration.AppService.NriShippingOnRampServiceBC.Process.ProcessCancelledOrdersData method. The current process is - " + processName +
                        " document number being processed is " + documentNumber + ".", outgoingMessage, ex);
                }
            } //end foreach

            //Confirm the processing of the orders by making a PUT call
            ConfirmSuccessfullProcessing(processedOrderIds);
            processedOrderIds.Clear();
            Trace.WriteLineIf(tracingEnabled, "End processing " + batchName);
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
        /// The code is responsible for calling the GET meothod of the NRI endpoint and retrieving the shipment orders. The 
        /// code logs errors and return empty string on failure.  It also returns an empty string if no data is returned ([]).
        /// </summary>
        /// <param name="url">NRI endpoint</param>
        /// <param name="authorization">authorization key in base 64</param>
        /// <returns></returns>
        private string GetSourceData(string url, string authorization)
        {
            string nriData;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.ContentType = "application/json";
            request.Headers.Add(HttpRequestHeader.Authorization, authorization);

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    nriData = reader.ReadToEnd();
                }
                if (nriData == "[]")
                    return "";
                else
                    return nriData;
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("401"))
                {
                    instrumentation.LogGeneralException("An exception occurred in the BC.Integration.AppService.NriShippingOnRampServiceBC.GetSourceData method when trying to access the " +
                        url + " URL.", new Exception("401 Unauthorized Access", ex));
                    return "";
                }
                if (ex.Message.Contains("The remote name could not be resolved"))
                {
                    instrumentation.LogGeneralException("An exception occurred in the BC.Integration.AppService.NriShippingOnRampServiceBC.GetSourceData method when trying to access the " +
                        url + " URL.", new Exception("Please check network access", ex));
                    return "";
                }
                if (ex.Message.Contains("operation has timed out"))
                {
                    instrumentation.LogGeneralException("A Time Out exception occurred in the BC.Integration.AppService.NriShippingOnRampServiceBC.GetSourceData method when trying to access the NRI " +
                        url + " URL.", ex);
                    return "";
                }
                //Unable to provide any additional info so return exception;
                return "";
            }
            catch (Exception ex)
            {
                instrumentation.LogGeneralException("An exception occurred in the BC.Integration.AppService.NriShippingOnRampServiceBC.GetSourceData method when trying to access the " +
                    url + " URL.", new Exception("Unknown exception occured.  Please review inner exception for details.", ex));
                return "";
            }
        }

        /// <summary>
        /// This method allows the pickup of files from a local folder.
        /// </summary>
        /// <param name="pickupMessageFolderPath">Folder path to pickup the files</param>
        /// <returns></returns>
        private string GetFileSourceData(string pickupMessageFolderPath)
        {
            string nriData = "[";

            string path = pickupMessageFolderPath.Replace("#ProcessName#", processName);

            var fileNames = Directory.EnumerateFiles(path);
            foreach (string file in fileNames)
            {
                Trace.WriteLineIf(tracingEnabled, "Get file messages.  Filename: " + file);
                string fileText = File.ReadAllText(file);
                nriData += fileText.Substring(fileText.IndexOf("{"), fileText.LastIndexOf("}") - fileText.IndexOf("{") + 1) + ",";
                //Remove file after processing
                File.Delete(file);  
            }
            nriData = nriData.TrimEnd(',') + "]";
            if (nriData == "[]")
                nriData = "";

            return nriData;
        }


        /// <summary>
        /// Makes a PUT call to the orderconfirmation/confirmation API to register the successful completion of the 
        /// shipment confirmation.
        /// This method will use the active URL and Key variables to connect to the appropriate API.
        /// </summary>
        /// <param name="orderIds">List of order ID'd that have been processed successfully</param>
        private void ConfirmSuccessfullProcessing(List<string> orderIds)
        {
            if (activeUrl == "Not Applicable") //These are local files not orders form the API
                return;

            Trace.WriteLineIf(tracingEnabled, "###NriShippingOnRampServiceBC: Start making the PUT call for " + orderIds.Count + " orders.");
            //Data format [{'OrderID': '" + orderId + "'}]";
            string data = "["; 
            foreach (string id in orderIds)
            {
                data += "{'OrderID': '" + id + "'},";
            }
            data = data.TrimEnd(',');
            data += "]";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(activeUrl);

            request.Headers.Add(HttpRequestHeader.Authorization, activeKey);
            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.Method = "POST";

            byte[] postBytes = Encoding.UTF8.GetBytes(data);
            request.ContentLength = postBytes.Length;

            Stream requestStream = request.GetRequestStream();
            try
            {
                requestStream.Write(postBytes, 0, postBytes.Length);
                requestStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Message.Contains("401"))
                {
                    instrumentation.LogGeneralException("A 401 Unauthorized Access exception occurred in the BC.Integration.AppService.NriShippingOnRampServiceBC.ConfirmSuccessfullProcessing method when trying to access the " +
                        activeUrl + " URL.  This failure will result in duplicate messages being processed.", new Exception("401 Unauthorized Access", ex));
                }
                else if (ex.Message.Contains("The remote name could not be resolved"))
                {
                    instrumentation.LogGeneralException("A network access exception occurred in the BC.Integration.AppService.NriShippingOnRampServiceBC.ConfirmSuccessfullProcessing method when trying to access the " +
                        activeUrl + " URL.  This failure will result in duplicate messages being processed.", ex);
                }
                else
                {
                    
                   instrumentation.LogGeneralException("An exception occurred in the BC.Integration.AppService.NriShippingOnRampServiceBC.ConfirmSuccessfullProcessing method when trying to access the " +
                   activeUrl + " URL.  This failure will result in duplicate messages being processed and will unable to mark a shipment RECEIVED.", new Exception("Unknown exception occured.  Please review inner exception for details.", ex));
                }
                
                instrumentation.LogNotification("NriApiPostUsaCan", serviceId,new Guid(), "Connectivity", "Unable to mark a shipment RECEIVED", string.Join(",", orderIds.ToArray()));

            }//Unable to provide any additional info so return exception;
            finally
            {
                requestStream.Dispose();
            }
            Trace.WriteLineIf(tracingEnabled, "###NriShippingOnRampServiceBC: End making the PUT call.");
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