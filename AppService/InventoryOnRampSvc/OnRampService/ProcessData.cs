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
using BC.Integration.Canonical.Inventory.Ep;
using System.Linq;
using BC.Integration.APICalls;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BC.Integration.InventoryOnRamp
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
        //CR7: Tracing prefix
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
        //CR8: Dynamic Routing
        List<string> urls;

        //These rest of the properties could be overridden by the central config store at the service ot process level
        private string archiveFolder = "";
        
        //Msg Envelope properties
        private string msgType = "";
        private string filterKeyValuePairs = "";
        private string processKeyValuePairs = "";
        private string topic = "";
       
        //Messaging Queue Properties
        private string queueUrl = "";

        private MessageManager msgMgr = new MessageManager();
        private string batchName = "";
        private XmlDocument outgoingMessage;
        private string messageFormat = "xml";
        private int serviceVersion = 1;
        private int messageVersion = 1;
        private bool tracingEnabled = false;
        private bool localFileSource = false;
        private string exceptionMessageFolderPath = "";
        private string tracingMessageFolderPath = "";
        private string pickupFileFolderPath = "";

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
            pickupFileFolderPath = Utilities.GetConfigurationValue(configuration, "PickupFileFolderPath");
            msgType = Utilities.GetConfigurationValue(configuration, "MsgType");
            filterKeyValuePairs = Utilities.GetConfigurationValue(configuration, "FilterKeyValuePairs");
            processKeyValuePairs = Utilities.GetConfigurationValue(configuration, "ProcessKeyValuePairs");
            topic = Utilities.GetConfigurationValue(configuration, "Topic");
            //queueUrl = Utilities.GetConfigurationValue(configuration, "QueueUrl");

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
        /// Process Data is the main method in the BC.Integration.AppService.TemplateHipOnRamp class.  It implements the following steps;
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
                    string data = "";

                    //Get data from Source System or Local File Pickup
                    if (!localFileSource)
                    {
                        try
                        {
                           
                            //Get data from BC
                            API_Calls APIcalls = new API_Calls();

                            string json = @"[{
                                'location': '11',
                                'division': 'HSC',
                                'upc': '828432006540',
                                'product_id':'10026-00055-OS',
                                'open':'5',
                                'picked':'0',
                                'invoiced':'0',
                                'unshipped':'0',
                                'qoh':'100',
                                'available_qoh':'100',
                                'ots_inventory':'95',
                                'extraFields':'t1'
                             },
                            {
                                'location': '11',
                                'division': 'HSC',
                                'upc': '828432010295',
                                'product_id':'10005-00001-OS',
                                'open':'10',
                                'picked':'0',
                                'invoiced':'0',
                                'unshipped':'0',
                                'qoh':'100',
                                'available_qoh':'100',
                                'ots_inventory':'90',
                                'extraFields':'t2'
                             } ,
                            {
                                'location': '21',
                                'division': 'HSC',
                                'upc': '828432010295',
                                'product_id':'10005-00001-OS',
                                'open':'10',
                                'picked':'0',
                                'invoiced':'0',
                                'unshipped':'0',
                                'qoh':'100',
                                'available_qoh':'100',
                                'ots_inventory':'90',
                                'extraFields':'t3'
                             }
                            ]";

                          // data = JArray.Parse(json);
                            //JArray data =  APIcalls.GetCutSoldByLocation();

                            Inventory[] items = JsonConvert.DeserializeObject<Inventory[]>(json);
                            var groupedByLoc = items.GroupBy(i => i.Location).ToList();

                            foreach (var item in groupedByLoc)
                            {
                                string location = item.FirstOrDefault().Location;
                                string jsonMsg = JsonConvert.SerializeObject(item);

                                ProcessSourceData(jsonMsg, location, activationGuid);
                                 Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Data found. ");
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Exception occurred while trying to retrieve and process Inventory data from BC API. (BC.Integration.InventoryOnRamp.Process.ProcessData method)", ex);
                        }
                    }
                    else
                    {
                      try
                        {
                            //Get local file data
                            data = GetFileSourceData(pickupFileFolderPath);
                            batchName = "LocalFile";

                            Inventory[] items = JsonConvert.DeserializeObject<Inventory[]>(data);
                            var groupedByLoc = items.GroupBy(i => i.Location).ToList();

                            foreach (var item in groupedByLoc)
                            {
                                string location = item.FirstOrDefault().Location;
                                string jsonMsg = JsonConvert.SerializeObject(item);

                                ProcessSourceData(jsonMsg, location, activationGuid);
                                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Files found.  Filename: " + batchName);
                            }                            

                            
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Exception occurred while trying to retrieve and process Inventory local files. (BC.Integration.InventoryOnRamp.Process.ProcessData method)", ex);
                        }
                    }

                    //______________________________________________________________________________________________

                    //Log Activation End (This step will be skipped if an exception occurs)
                    instrumentation.LogActivationEnd(new Guid(activationGuid.Substring(0, 36)));
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End processing Inventory data.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + " An exception was raised when calling the BC.Integration.InventoryOnRamp.Process.ProcessData method. Exception message: " + ex.Message);
                    instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.InventoryOnRamp.Process.ProcessData method.", ex);
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
                Trace.WriteLine(tracingExceptionPrefix + " An exception was raised when calling the BC.Integration.InventoryOnRamp.Process.ProcessData method trying to resolve the Unity DI components. Exception message: " + ex.Message);
                throw new Exception("An exception occured in the BC.Integration.InventoryOnRamp.Process.ProcessData method trying to resolve the Unity DI components.", ex);
            }
        }

        private void ProcessSourceData(string canonicalMsg, string location, string activationGuid)
        {
                        
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.InventoryOnRamp.Process.ProcessFile.  Start Processing Source data.");
            //Create initial msg envelope to represent the start of the service process and call Instrumenation
            XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, msgType, messageVersion, messageFormat, batchName);
            instrumentation.LogActivity(msg);
            //Log Activation Association with Interchange
            instrumentation.AssociateActivationWithInterchages(new Guid(activationGuid.Substring(0, 36)), msgMgr.EntryPointEnvelope.Interchange.InterchangeId);

           
                try
                {

                //Call a method to process each transaction and map to the Sales Document
                //string canonicalMsg =  MapInventoryToCanonical(data);

                    //Create envelope and add canonical message to the envelope
                    HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filterKeyValuePairs);
                    HipKeyValuePairCollection processCol = new HipKeyValuePairCollection(processKeyValuePairs);
                    outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, topic, filterCol, processCol, canonicalMsg, 1, null, "");

                    int retryCount = 0;
                    //Place message on the message bus
                    //CR8: Update Dynamic Routing
                    PublishMessage(outgoingMessage.InnerXml, filterCol);

                    //Save a copy of message
                    if (tracingEnabled)
                    {
                    string textBody = "";

                    textBody = canonicalMsg;
                    SaveMessageToFile(textBody, serviceId + "."  , false); 

                        /*string site = "TM";
                        string textBody = "RAW\r\n";
                        foreach (OrderLineItem item in transaction)
                        {
                            textBody += item.ConvertToCsv() + "\r\n";
                        }
                        textBody += "\r\nXML\r\n" + invoice.ConvertInvoiceToString(invoice);

                        SaveMessageToFile(textBody, serviceId + "." + transactionNum + "." + site, false);*/
                    }

                    //Log Activity
                    instrumentation.LogActivity(outgoingMessage, queueUrl, retryCount);
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End processing location: " + location);
                    
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + " An exception occured while processing data in BC.Integration.InventoryOnRamp.Process.ProcessSourceData. Exception message: " + ex.Message);
                    instrumentation.LogMessagingException("An exception occured while processing a message in the " +
                        "BC.Integration.InventoryOnRamp.Process.ProcessSourceData method. The current process is - " + processName +
                        "Processing SiteId: "+ location, outgoingMessage, ex);
                }
            
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

        /*public  string MapInventoryToCanonical(JToken data)
        {
            try
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Start mapping the Inventory transaction");
                Inventory inv = new Inventory();
                //Populate the obj

                inv.Location = data.Value<string>("location");
                inv.Division = data.Value<string>("division");
                inv.Upc = data.Value<string>("upc");
                inv.Product_id = data.Value<string>("product_id");
                inv.Open = data.Value<int>("open");
                inv.Picked = data.Value<int>("picked");
                inv.Invoiced = data.Value<int>("invoiced");
                inv.Unshipped = data.Value<int>("unshipped");
                inv.Qoh = data.Value<int>("qoh");
                inv.Available_qoh = data.Value<int>("available_qoh");
                inv.Ots_inventory = data.Value<int>("ots_inventory");


                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End mapping the Inventory transaction");
                //return inv;
                return SerializeCanonicalInventory.ToJson(inv);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + " An error occured while trying to map the Inventory message to the CanonicalInventory message");
                throw new Exception("An error occured while trying to map the Inventory message to the CanonicalInventory message" +
                    "in the BC.Integration.InventoryOnRamp.MapInventoryToCanonical method. The SKU is: " + data.Value<string>("Product_id"), ex);
            }
        }*/

        
        /// <summary>
        /// CR8: Dynamic Routing
        /// Works through a collection of URLs from the routing table and publishes the message to each queue.
        /// </summary>
        /// <param name="msgString">Message inc. envelope</param>
        /// <param name="filterCol">Filter collection for additional routing selection</param>
        private void PublishMessage(string msgString, HipKeyValuePairCollection filterCol)
        {
            int retryCount = 0;
            string strUrls = "";
            if (urls == null) //Only want to call the GetDestinationQueueUrls for the first message.
            {
                //Get SQS Queue URL's
                urls = config.GetDestinationQueueUrls(processName, serviceId, servicePostOperationId, msgType, filterCol.ConvertToKeyValuePairs());
            }

            foreach (string url in urls)
            {
                publishService.Push(url, msgString, out retryCount);
                strUrls += url.Substring(url.LastIndexOf('/') + 1) + ",";
            }
            //Set queueUrl for the instrumentation
            queueUrl = strUrls.TrimEnd(',');
        }


        /// <summary>
        /// This method allows the pickup of files from a local folder.
        /// </summary>
        /// <param name="pickupMessageFolderPath">Folder path to pickup the files</param>
        /// <returns></returns>
        private string GetFileSourceData(string pickupMessageFolderPath)
        {
            string data = "";

            string path = pickupMessageFolderPath.Replace("#ProcessName#", processName);

            var fileNames = Directory.EnumerateFiles(path);
            foreach (string file in fileNames)
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Get file messages.  Filename: " + file);
               
                    string fileText = File.ReadAllText(file);
                    fileText = fileText.TrimEnd('\r', '\n');
                    data += fileText + "\r\n";
                    //Remove file after processing
               
               File.Delete(file);
            }
            data = data.Trim();

            return data;
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

    }
}