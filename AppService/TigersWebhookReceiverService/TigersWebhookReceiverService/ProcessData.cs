using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using BC.Integration.Interfaces;
using BC.Integration.Utility;
using System.ServiceModel;
using System.Xml;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using BC.Integration.AppService.TigersWebhookReceiverService.Models;
using BC.Integration.AppService.TigersWebhookReceiverService.Managers;
using System.Security.Claims;
using System.ServiceModel.Web;
using System.Net.Http;

namespace BC.Integration.AppService.TigersWebhookReceiverService
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
        //Msg Envelope properties
        private string msgType = "";
        private string filter = "";
        private string process = "";

        //Messaging Queue Properties
        private string queueUrlOL = "";
        private string queueUrlWS = "";

        private MessageManager msgMgr = new MessageManager();
        WebOperationContext ctx = WebOperationContext.Current;
        private string batchName = "";
        private string documentNumber;
        private XmlDocument outgoingMessage;
        private string messageFormat = "JSON";
        private int serviceVersion = 1;
        private int messageVersion = 1;
        private bool tracingEnabled = false;
        private string activeUrl = "";
        private string activeKey = "";
        private string siteMapping = "Logic";
        public string tracingPrefix;
        string shipmentString = "";


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
            msgType = Utilities.GetConfigurationValue(configuration, "MsgType");
            filter = Utilities.GetConfigurationValue(configuration, "Filter");
            process = Utilities.GetConfigurationValue(configuration, "Process");
            queueUrlOL = Utilities.GetConfigurationValue(configuration, "QueueUrlOL");
            queueUrlWS = Utilities.GetConfigurationValue(configuration, "QueueUrlWS");
            string val1 = Utilities.GetConfigurationValue(configuration, "TracingEnabled");
            tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";

            if (val1 != "")
            {
                if (val1.ToLower() == "true" || val1.ToLower() == "false")
                    tracingEnabled = Convert.ToBoolean(val1);
                if (val1 == "1" || val1 == "0")
                    tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val1));
            }

            siteMapping = Utilities.GetConfigurationValue(configuration, "SiteMapping");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        private JWTContainerModel GetJWTContainerModel(string name, string email)
        {
            return new JWTContainerModel()
            {
                Claims = new Claim[]
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.Email, email)
                }
            };
        }

        /// <summary>
        /// Authenticate bearer key from Tigers. If sucessfull, save the receiving message to file folder.
        /// </summary>
        /// <param name="activationGuid"></param>
        /// <returns></returns>
        public string AuthenticateJWTToken(string activationGuid)
        {
            try
            {
                var bearerToken = WebOperationContext.Current.IncomingRequest.Headers["Authorization"].Substring(6).Trim();

                IAuthContainerModel model = GetJWTContainerModel(ConfigurationManager.AppSettings["FName"], ConfigurationManager.AppSettings["LName"]);
                IAuthService authService = new JWTService(model.SecretKey);

                string token = authService.GenerateToken(model);

                if (!authService.IsTokenValid(token) && !bearerToken.Equals(token))
                {
                    //return shipmentString;
                    throw new UnauthorizedAccessException();
                }
                else
                {
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Access granted");
                    var aa = OperationContext.Current.RequestContext.RequestMessage.GetHashCode();

                    var inputStream = OperationContext.Current.RequestContext.RequestMessage.GetBody<Stream>();
                    var sr = new StreamReader(inputStream, Encoding.UTF8);
                    shipmentString = sr.ReadToEnd();

                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Received input stream" + shipmentString);

                    //Write to Text file
                    SaveMessageToFile(shipmentString, serviceId + "." + DateTime.Now.ToString("yyyy-MM-dd"), false);

                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " File generated");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "EXCEPTION generated" + ex);
                instrumentation.LogGeneralException("An exception occured while processing a message in the " +
                  "BC.Integration.AppService.TigersWebhookReceiverService.ProcessData.AuthenticateJWTToken method. The current Tigers file being processed is - " + batchName + "  This transaction in the file has not been processed.", ex);
                instrumentation.LogNotification(process, serviceId, Guid.Parse(activationGuid), "Connectivity", "Unable to receive message from Tigers Webhook. " + ex, "");
                ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
            }
            return shipmentString;
        }


        #endregion

        /// <summary>
        /// Process Data is the main method in the BC.Integration.AppService.TigersWebhookReceiverService class.  It implements the following steps;
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
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Entered ProcessData ");
            try
            {
                CreateDiComponents();
                try
                {
                    //Log Activation Start
                    instrumentation.LogActivation(serviceId, new Guid(activationGuid.Substring(0, 36)), false);

                    shipmentString = AuthenticateJWTToken(activationGuid);
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Passed Authentication  ");
                    //File.WriteAllText(@"C:\Temp\ TigersMsg_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") +".txt" , shipmentString);

                    if (!String.IsNullOrEmpty(shipmentString))
                    {
                        try
                        {
                            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.TigersShippingOnRampService service initializing. ");

                            ShipmentConfirmation shipment = JsonConvert.DeserializeObject<ShipmentConfirmation>(shipmentString);

                            documentNumber = shipment.salesOrderRef;

                            //There is not a batch number for this process so use the activation GUID.
                            batchName = activationGuid;

                            //Process message collection if it contains one or more messages
                            if (shipment != null)
                            {
                                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.TigersWebhookReceiverService data retrieved found. Batch name: " + batchName);
                                if (shipment.locationNumber == "60" || shipment.addresses[0].locationNumber == "60")
                                {
                                    processName = "TigersShippingConfirmationWS";
                                }
                                //Create initial msg envelope to represent the start of the service process and call Instrumenation.
                                XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, msgType, messageVersion, messageFormat, batchName);
                                instrumentation.LogActivity(msg);

                                //Log Activation Association with Interchange
                                if (tracingEnabled) //Only track this if tracing is enabled.  This is due to the performance impact of making the ODBC connections
                                {
                                    instrumentation.AssociateActivationWithInterchages(new Guid(activationGuid.Substring(0, 36)), msgMgr.EntryPointEnvelope.Interchange.InterchangeId);
                                }
                                if (shipment.cancellations != null && shipment.cancellations.Count > 0) //json is for Cancelled shipments
                                {
                                    shipment.cancellations.RemoveAll(x => x.documentType != "SO"); //Remove PO and RO

                                    ProcessCancelledOrdersData(shipment, msgMgr.EntryPointEnvelope.Msg.Id);
                                }
                                else
                                {
                                    ProcessShipmentConfirmData(shipment, msgMgr.EntryPointEnvelope.Msg.Id);
                                }
                            }

                            //Log Activation End (This step will be skipped if an exception occurs)
                            instrumentation.LogActivationEnd(new Guid(activationGuid.Substring(0, 36)));
                            Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End processing all batches.");
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " An exception was raised when calling the BC.Integration.AppService.TigersWebhookReceiverService.Process.ProcessData method. Exception message: " + ex.Message);
                            instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.AppService.TigersWebhookReceiverService.Process.ProcessData method.", ex);
                            throw new Exception("An exception occured in the BC.Integration.AppService.TigersWebhookReceiverService.Process.ProcessData method trying to authenticate JWT token.", ex);
                        }
                        finally
                        {
                            instrumentation.FlushActivity();
                            container.Dispose();
                        }
                        ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.OK;
                    }
                }
                catch (Exception ex)
                {
                    //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "  An exception was raised when calling the BC.Integration.AppService.TigersWebhookReceiverService.Process.ProcessData method trying to authenticate JWT token. Exception message: " + ex.Message);
                    throw new Exception("An exception occured in the BC.Integration.AppService.TigersWebhookReceiverService.Process.ProcessData method trying to authenticate JWT token.", ex);
                }
            }
            catch (Exception ex)
            {
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "  An exception was raised when calling the BC.Integration.AppService.TigersWebhookReceiverService.Process.ProcessData method trying to resolve the Unity DI components. Exception message: " + ex.Message);
                instrumentation.LogGeneralException("An ", ex);
                if (String.IsNullOrEmpty(shipmentString))
                {
                    ctx.OutgoingResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                }
                else
                {
                    instrumentation.LogNotification(process, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "API", shipmentString, documentNumber);
                }
            }
        }

        /// <summary>
        /// Process shipment confirmation data received from Tigers 
        /// </summary>
        /// <param name="tigersShipmentData">shipment confirmation data</param>
        private void ProcessShipmentConfirmData(ShipmentConfirmation shipment, Guid messageId)
        {

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + "(Confirmations): Start adding the order collection to the queue.  Number of orders to process is " + "" + " orders.");
            //int i = 0;
            try
            {
                //Set document number in envelope to support tracking and troubleshooting.
                if (shipment.salesOrderRef != null || shipment.salesOrderRef != "")
                {
                    documentNumber = shipment.salesOrderRef;
                }
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Processing shipment confirmation message collection.  Processing document number: " + documentNumber);

                if (tracingEnabled)
                {
                    string site = "";
                    if (shipment.locationNumber == null || shipment.locationNumber == "")
                    {
                        site = shipment.addresses[0].locationNumber;   //For some reason Tigers doesn't send location number at Header level but in address field.
                    }
                    else
                    {
                        site = shipment.locationNumber;
                    }
                }

                //Map to the canonical structure
                string canonicalMsg;
                try
                {
                    canonicalMsg = Mapper.ConvertShipmentConfirm(shipment, siteMapping);
                }
                catch (Exception ex)
                {
                    string filename = serviceId;
                    string docId = "unknown";
                    if (shipment.salesOrderRef != null || shipment.salesOrderRef != "")
                    {
                        filename += "." + shipment.salesOrderRef;
                        docId = shipment.salesOrderRef;
                    }
                    if (shipment.locationNumber != null || shipment.locationNumber != "")
                    {
                        filename += "." + shipment.locationNumber;
                    }

                    instrumentation.LogNotification(processName, serviceId, messageId, "Mapping", ex.Message, docId);

                    SaveMessageToFile(ex.Message + "\r\n" + Serialize.OrderToJson(shipment), filename + ".Mapping", true);

                    throw new Exception("An exception occurred while mapping the shipping confirmation " + shipment.salesOrderRef + " to the canonicalShippingConfirmation message type " +
                        "in BC.Integration.AppService.TigersWebhookReceiverService.Mapper.ConvertShipmentConfirm.  A copy of the failed message has been saved to the file system.", ex);
                }

                //Create envelope and add canonical message to the envelope
                HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filter);
                outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, "", filterCol, null, canonicalMsg.ToString(), null, null, documentNumber);

                //Place message on the message bus
                int retryCount = 0;
                publishService.Configuration = configuration;

                if (canonicalMsg.Contains("site\":\"60\""))   //if it's wholesale then move it to different queue
                {
                    publishService.Push(queueUrlWS, outgoingMessage.InnerXml, out retryCount);

                    //Log Activity
                    instrumentation.LogActivity(outgoingMessage, queueUrlWS, retryCount);
                    Trace.WriteLineIf(tracingEnabled,tracingPrefix," End processing: " + documentNumber);
                } 
                else
                {
                    publishService.Push(queueUrlOL, outgoingMessage.InnerXml, out retryCount);

                    //Log Activity
                    instrumentation.LogActivity(outgoingMessage, queueUrlOL, retryCount);
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix, " End processing: " + documentNumber);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "  An exception occurred while processing the shipping confirmation " + "" + " in BC.Integration.AppService.TigersWebhookReceiverService.Process.ProcessShipmentConfirmData. Exception message: " + ex.Message);
                instrumentation.LogMessagingException("An exception occurred while processing the shipping confirmation " + "" + " in the " +
                    "BC.Integration.AppService.TigersWebhookReceiverService.Process.ProcessShipmentConfirmData method. The current process is - " + processName +
                    " document number being processed is " + documentNumber + ".", outgoingMessage, ex);
                throw new Exception();
            }
            Trace.WriteLineIf(tracingEnabled, tracingPrefix, "End processing " + batchName);

        }

        /// <summary>
        /// Process cancelled order data received from Tigers
        /// </summary>
        /// <param name="TigersCancelledOrderData">Cancelled order data</param>
        private void ProcessCancelledOrdersData(ShipmentConfirmation message, Guid messageId)
        {

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " (Cancellations): Start adding the order collection to the queue.  Number of orders to process is " + "list.Count" + " orders.");
            try
            {
                foreach (Cancellation cancelMessage in message.cancellations)
                {
                    //Set document number in envelope to support tracking and troubleshooting.
                    if (cancelMessage.documentRef != null && cancelMessage.documentRef != "")
                    {
                        documentNumber = cancelMessage.documentRef;
                    }

                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Processing cancelled shipment order collection.  Processing document number: " + documentNumber);
                    if (tracingEnabled)
                    {
                        string site = "";
                        if (message.locationNumber != null)
                        {
                            if (message.locationNumber != null || message.locationNumber != "")
                                site = message.locationNumber;
                            else
                                site = "unknown";
                        }
                        SaveMessageToFile(Serialize.OrderToJson(message), serviceId + "." + documentNumber + "." + site, false);
                    }
                    //Map to the canonical structure
                    string canonicalMsg;
                    try
                    {
                        canonicalMsg = Mapper.ConvertCancelled(cancelMessage, message.locationNumber);
                    }
                    catch (Exception ex)
                    {
                        string filename = serviceId;
                        string docId = "unknown";
                        if (cancelMessage.documentRef != null || cancelMessage.documentRef != "")
                        {
                            filename += "." + cancelMessage.documentRef;
                            docId = cancelMessage.documentRef;
                        }
                        if (message.locationNumber != null)
                        {
                            if (message.locationNumber != null || message.locationNumber != "")
                            {
                                filename += "." + message.locationNumber;
                            }
                        }

                        instrumentation.LogNotification(processName, serviceId, messageId, "Mapping", ex.Message, docId);

                        SaveMessageToFile(ex.Message + "\r\n" + Serialize.OrderToJson(message),
                            filename + ".Mapping", true);
                        throw new Exception("An exception occurred while mapping the cancelled shipping order " + cancelMessage.documentRef + " to the canonicalShippingConfirmation message type " +
                            "in BC.Integration.AppService.TigersWebhookReceiverService.Mapper.ConvertShipmentConfirm.  A copy of the failed message has been saved to the file system.", ex);
                    }

                    //Create envelope and add canonical message to the envelope
                    HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filter);
                    HipKeyValuePairCollection processCol = new HipKeyValuePairCollection(process);
                    outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, "", filterCol, null, canonicalMsg, null, null, documentNumber);

                    //Place message on the message bus
                    int retryCount = 0;
                    publishService.Configuration = configuration;

                    if (canonicalMsg.Contains("site\":60"))   //if it's wholesale then move it to different queue
                    {
                        publishService.Push(queueUrlWS, outgoingMessage.InnerXml, out retryCount);

                        //Log Activity
                        instrumentation.LogActivity(outgoingMessage, queueUrlWS, retryCount);
                        Trace.WriteLineIf(tracingEnabled, "End processing: " + documentNumber);
                    }
                    else
                    {
                        publishService.Push(queueUrlOL, outgoingMessage.InnerXml, out retryCount);

                        //Log Activity
                        instrumentation.LogActivity(outgoingMessage, queueUrlOL, retryCount);
                        Trace.WriteLineIf(tracingEnabled, "End processing: " + documentNumber);
                    }
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "  An exception occurred while processing cancelled shipping orders " + "message.OrderId" + "  in BC.Integration.AppService.TigersWebhookReceiverService.Process.ProcessCancelledOrdersData. Exception message: " + ex.Message);
                instrumentation.LogMessagingException("An exception occurred while processing cancelled shipping orders " + "message.OrderId" + " in the " +
                    "BC.Integration.AppService.TigersWebhookReceiverService.Process.ProcessCancelledOrdersData method. The current process is - " + processName +
                    " document number being processed is " + documentNumber + ".", outgoingMessage, ex);
            }

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End processing " + batchName);
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
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Get file messages.  Filename: " + file);
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

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Start making the PUT call for " + orderIds.Count + " orders.");
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
                    instrumentation.LogGeneralException("A 401 Unauthorized Access exception occurred in the BC.Integration.AppService.TigersWebhookReceiverService.ConfirmSuccessfullProcessing method when trying to access the " +
                        activeUrl + " URL.  This failure will result in duplicate messages being processed.", new Exception("401 Unauthorized Access", ex));
                }
                else if (ex.Message.Contains("The remote name could not be resolved"))
                {
                    instrumentation.LogGeneralException("A network access exception occurred in the BC.Integration.AppService.TigersWebhookReceiverService.ConfirmSuccessfullProcessing method when trying to access the " +
                        activeUrl + " URL.  This failure will result in duplicate messages being processed.", ex);
                }
                else
                {

                    instrumentation.LogGeneralException("An exception occurred in the BC.Integration.AppService.TigersWebhookReceiverService.ConfirmSuccessfullProcessing method when trying to access the " +
                    activeUrl + " URL.  This failure will result in duplicate messages being processed and will unable to mark a shipment RECEIVED.", new Exception("Unknown exception occured.  Please review inner exception for details.", ex));
                }

                instrumentation.LogNotification("NriApiPostUsaCan", serviceId, new Guid(), "Connectivity", "Unable to mark a shipment RECEIVED", string.Join(",", orderIds.ToArray()));

            }//Unable to provide any additional info so return exception;
            finally
            {
                requestStream.Dispose();
            }
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End making the PUT call.");
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