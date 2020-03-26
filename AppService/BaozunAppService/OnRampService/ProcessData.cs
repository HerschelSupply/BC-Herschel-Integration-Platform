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
using BC.Integration.Canonical.RetailChannel;
using System.Linq;

namespace BC.Integration.AppService.BaozunOnRamp
{
    public class Process
    {

        #region Properties

        private List<KeyValuePair<string, string>> configuration = null;

        //Dependency Injection Components
        IConfiguration config;
        IInstrumentation instrumentation;
        IPublishService publishService;
        IFtpIntegrator integrator;
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

        //private string batchData = "";
        private MessageManager msgMgr = new MessageManager();
        private string batchName = "";
        //private string documentNumber;
        private XmlDocument outgoingMessage;
        private string messageFormat = "xml";
        private int serviceVersion = 1;
        private int messageVersion = 1;
        private bool tracingEnabled = false;
        private bool localFileSource = false;
        private string exceptionMessageFolderPath = "";
        private string tracingMessageFolderPath = "";
        private string pickupFileFolderPath = "";

        private string mapSiteId = "";
        private string mapCurrencyCode = "";
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
            mapSiteId = Utilities.GetConfigurationValue(configuration, "MapSiteId");
            mapCurrencyCode = Utilities.GetConfigurationValue(configuration, "MapCurrencyCode");

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

                    string data;
                    //_____________________________________________________________________________________________
                    //Add code to get data from source system.  This will usally be a collection of messages.

                    //Get data from Source System or Local File Pickup
                    if(!localFileSource)
                    {
                        try
                        {
                            //Use FTP integrator component to get file from FTP server
                            integrator.CreateSession(configuration);

                            //Get data from source system
                            while (integrator.GetFile())
                            {
                                data = integrator.AvailableFileText;
                                batchName = integrator.AvailableFilename.Substring(0, integrator.AvailableFilename.IndexOf('.'));
                                if (batchName.Substring(0, 2) != "ST")
                                {
                                    ProcessFile(data, activationGuid);
                                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Files found.  Filename: " + batchName);
                                }
                                else
                                {
                                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Sales Tender files found and ignored.  Filename: " + batchName);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Exception occurred while trying to retrieve and process Baozun CSV Sales files. (BC.Integration.AppService.BaozunOnRamp.Process.ProcessData method)", ex);
                        }
                        finally
                        {
                            integrator.CloseSession();
                        }
                    }
                    else
                    {
                        try
                        {
                            //Get local file data
                            data = GetFileSourceData(pickupFileFolderPath);
                            ProcessFile(data, activationGuid);
                            batchName = "LocalFile";
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Exception occurred while trying to retrieve and process Baozun Local Sales files. (BC.Integration.AppService.BaozunOnRamp.Process.ProcessData method)", ex);
                        }
                    }

                    //______________________________________________________________________________________________

                    //Log Activation End (This step will be skipped if an exception occurs)
                    instrumentation.LogActivationEnd(new Guid(activationGuid.Substring(0, 36)));
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End processing Baozun CSV batch file.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + " An exception was raised when calling the BC.Integration.AppService.BaozunOnRamp.Process.ProcessData method. Exception message: " + ex.Message);
                    instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.AppService.BaozunOnRamp.Process.ProcessData method.", ex);
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
                Trace.WriteLine(tracingExceptionPrefix + " An exception was raised when calling the BC.Integration.AppService.BaozunOnRamp.Process.ProcessData method trying to resolve the Unity DI components. Exception message: " + ex.Message);
                throw new Exception("An exception occured in the BC.Integration.AppService.BaozunOnRamp.Process.ProcessData method trying to resolve the Unity DI components.", ex);
            }
        }

        private void ProcessFile(string data, string activationGuid)
        {
            List<string> failedTransNums = new List<string>();
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.BaozunOnRamp.Process.ProcessFile.  Start Processing File.");
            //Create initial msg envelope to represent the start of the service process and call Instrumenation
            XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, msgType, messageVersion, messageFormat, batchName);
            instrumentation.LogActivity(msg);
            //Log Activation Association with Interchange
            instrumentation.AssociateActivationWithInterchages(new Guid(activationGuid.Substring(0, 36)), msgMgr.EntryPointEnvelope.Interchange.InterchangeId);

            //Load each line into an object model then loop through getting each associated line item to create a complete transaction
            //Generate a collection of lines
            Orders orders = new Orders();
            string[] colLines = data.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            int i = 1;
            foreach (string line in colLines)
            {
                try
                {
                    orders.AddLine(line);
                }
                catch (Exception ex)
                {
                    //If we get a badly formed line, we need to make sure all lines for that transaction number are removed.
                    //Identify transaction number of failed line
                    string failedTransNum = line.Substring(0, line.IndexOf(','));
                    failedTransNums.Add(failedTransNum);
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Transaction Number: " + failedTransNum + " failed to be converted from the CSV data.  Exception message: " + ex.Message);
                    instrumentation.LogGeneralException("An exception occured while processing a message in the " +
                        "BC.Integration.AppService.BaozunOnRamp.Process.ProcessFile method. The current Baozun file being processed is - " + batchName +
                        ", transaction number being processed is " + failedTransNum + " and the error occured on line " + i + " in the file.  This transaction in the file has not been processed.", ex);
                    //Notification Log entry...
                    instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "ConversionFromCSV", 
                        "CSV filename: " + batchName + " failed with the following error, " + ex.Message, failedTransNum);
                }
                i++;
            }
            List<string> distinctTransactions = orders.DistinctTransactionNumbers();
            i = 1;
            foreach (string transactionNum in distinctTransactions)
            {
                try
                {
                    //Skip if transaction number exists in failedTransNums collection.
                    if(HasFailedLineItems(transactionNum, failedTransNums))
                    {
                        Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Transaction Number: " + transactionNum + " was skipped, as the trasaction number was in the failed line item collection.");
                        continue;
                    }
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Processing Baozun file.  Transaction Number: " + transactionNum);
                    //Get all the associated OrderLineItems
                    List<OrderLineItem> transaction = orders.OrderLineItems(transactionNum);

                    //Call a method to process each transaction and map to the Sales Document
                    Invoice invoice = MapInvoices(transaction);

                    //Create envelope and add canonical message to the envelope
                    HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filterKeyValuePairs);
                    HipKeyValuePairCollection processCol = new HipKeyValuePairCollection(processKeyValuePairs);
                    outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, topic, filterCol, processCol, invoice.ConvertInvoiceToString(invoice), i, null, invoice.Header.InvoiceNumber);

                    int retryCount = 0;
                    //Place message on the message bus
                    //CR8: Update Dynamic Routing
                    PublishMessage(outgoingMessage.InnerXml, filterCol);

                    //Save a copy of message
                    if (tracingEnabled)
                    {
                        string site = "TM";
                        string textBody = "RAW\r\n";
                        foreach (OrderLineItem item in transaction)
                        {
                            textBody += item.ConvertToCsv() + "\r\n";
                        }
                        textBody += "\r\nXML\r\n" + invoice.ConvertInvoiceToString(invoice);

                        SaveMessageToFile(textBody, serviceId + "." + transactionNum + "." + site, false);
                    }

                    //Log Activity
                    instrumentation.LogActivity(outgoingMessage, queueUrl, retryCount);
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End processing: " + transactionNum);
                    i++;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + " An exception occured while processing data in BC.Integration.AppService.BaozunOnRamp.Process.ProcessFile. Exception message: " + ex.Message);

                    instrumentation.LogMessagingException("An exception occured while processing a message in the " +
                        "BC.Integration.AppService.BaozunOnRamp.Process.ProcessFile method. The current process is - " + processName +
                        " document number being processed is " + transactionNum + ".", outgoingMessage, ex);

                    instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "ProcessFile",
                    "DocumentId: " + transactionNum + " failed with the following error, " + ex.Message, transactionNum);
                }
            }
        }

        /// <summary>
        /// Checks a collection of failed transaction numbers against the current transaction number.  If there is a match the method
        /// returns false.
        /// </summary>
        /// <param name="transactionNum">Current Transaction Number</param>
        /// <param name="failedTransNums">Collection of failed transaction numbers</param>
        /// <returns>Boolean</returns>
        private bool HasFailedLineItems(string transactionNum, List<string> failedTransNums)
        {
            foreach(string num in failedTransNums)
            {
                if(transactionNum == num)
                {
                    return true;
                }
            }
            return false;
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

        private Invoice MapInvoices(List<OrderLineItem> transaction)
        {
            try
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Start mapping the Baozun transaction");
                Invoice doc = new Invoice();
                //Populate the message header
                doc.Process = processName;
                doc.Header = new InvoiceHeader();
                doc.Header.InvoiceNumber = transaction[0].TransactionNumber;
                doc.Header.Currency = mapCurrencyCode;
                doc.Header.SiteId = mapSiteId; //Baozun specific site number
                //Delivery date will not include time as this increases risk of bad data impacting integration.  
                //The date will also be the local date in China not converted to PST.  Approach signed off by Tanya.
                doc.Header.InvoiceDate = transaction[0].DeliveryDate;
                doc.Header.SlipCode = transaction[0].SlipCode; // Also known as Baozun TMall number
                if (transaction[0].TransactionType.ToUpper() == "S" || transaction[0].TransactionType.ToUpper() == "E")
                    doc.Header.InvoiceType = "Sales";
                else
                    doc.Header.InvoiceType = "Return";

                decimal freightCost = 0;
                //decimal itemDiscounts = 0;
                decimal total = 0;
                int productCount = 0;
                bool isConsolidationRequired = false;

                //Create the child collections
                //Baozun should only ever have a single shipment with one or multiple line items in a transaction.
                //A Return or Exchange transaction could have a line split by the return condition for the same 
                //product.  Need to determine unique product count as item count could differ from return SKU's
                //due to the return condition.
                var items = from item in transaction
                            select item.ProductCode;
                var distinctProductCodes = items.Distinct();
                productCount = distinctProductCodes.Count();
                //if product count is < transaction count then will need to call line item consolidation code after message complete.
                if (productCount < transaction.Count)
                    isConsolidationRequired = true;

                doc.LineItems = new List<InvoiceLineItem>();

                //Loop through the line items in the transaction and create the associated items in the canonical msg
                //NOTE: Sales transactions are split by line item
                //Return transaction can also be split by return or exchange condition (Good, Damaged or Pending)
                for (int i = 0; i < transaction.Count; i++)
                {
                    total += transaction[i].LineSumAmount;
                    //itemDiscounts += (transaction[i].Discount * transaction[i].Quantity);
                    //As per Bernard's email Dec 6th '17.  Logistic cost is duplicated on each line item. This is a customer charge.
                    freightCost = transaction[i].LogisticCost;  //Summing logic removed.

                    InvoiceLineItem item = new InvoiceLineItem();
                    item.ItemNumber = transaction[i].ProductCode;
                    item.UnitPrice = transaction[i].UnitPrice;
                    item.ActualPrice = transaction[i].ActualPrice;
                    item.LineItemTotalAmount = Math.Abs(transaction[i].LineSumAmount);
                    item.Quantity = Math.Abs(transaction[i].Quantity);
                    item.Discount = Math.Abs(transaction[i].Discount);
                    item.ExtendedPrice = item.ActualPrice * item.Quantity;

                    //If the transaction represents a Return or Exception add the return node and populate
                    if (transaction[0].TransactionType.ToUpper() == "R")
                    {
                        //EP terminology
                        //PERFECT in CM UI = < ReceivedState > OrderReturnReceivedState_Defect </ ReceivedState >
                        //DEFECTIVE in CM UI = < ReceivedState > OrderReturnReceivedState_Damaged </ ReceivedState >
                        //Note that no value will be returned for a 'Physical return not required' RMA
                        item.ReturnedInPerfectCondition = transaction[i].ReturnGood;
                        item.ReturnedDamaged = transaction[i].ReturnDamaged;
                        item.ReturnedInUnknownCondition = transaction[i].ReturnUnknown;
                    }
                    doc.LineItems.Add(item);
                }

                //Add the calculated fields to the header.
                doc.Header.FreightCost = Math.Abs(freightCost);
                //doc.Header.TotalLineItemDiscounts = Math.Abs(itemDiscounts);
                doc.Header.TotalAmount = Math.Abs(total) + doc.Header.FreightCost;
              
                if (isConsolidationRequired)
                    doc = InvoiceLineItemConsolidation(doc, distinctProductCodes);

                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End mapping the Baozun transaction");
                return doc;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + " An error occured while trying to map the Baozun batch message to the SalesChannelInvoice message");
                throw new Exception("An error occured while trying to map the Baozun batch message to the SalesChannelInvoice message" +
                    "in the BC.Integration.AppService.Baozun.SalesDocumentOnRamp.MapInvoices method. The Baozun transaction " +
                    "number is: " + transaction[0].TransactionNumber, ex);
            }
        }

        /// <summary>
        /// This code will consolidate the line items in return or exchange scenario's where returned items are in deifferent conditions.
        /// Since we should rarely get multiple line items returned in different conditions. It was seen as easier to consolidate 
        /// the line items than complicate the simple looping code that the majority of scenarios will use.
        /// </summary>
        /// <param name="doc">Sales Document object created form the Baozun transactional data</param>
        /// <param name="distinctProductCodes">Distinct product codes to use in the consolidation process</param>
        private Invoice InvoiceLineItemConsolidation(Invoice doc, IEnumerable<string> distinctProductCodes)
        {
            try
            {
                int distinctProductCodeCount = distinctProductCodes.Count();
                List<InvoiceLineItem> newLineItems = new List<InvoiceLineItem>();

                int i = 0;
                foreach (string code in distinctProductCodes)
                {
                    int quantity = 0;
                    decimal totalLineItemAmt = 0;
                    int returnedInPerfectCondition = 0;
                    int returnedDamaged = 0;
                    int returnedInUnknownCondition = 0;
                    InvoiceLineItem newItem = new InvoiceLineItem();

                    foreach (InvoiceLineItem item in doc.LineItems)
                    {
                        if (item.ItemNumber == code)
                        {
                            quantity += item.Quantity;
                            totalLineItemAmt += item.LineItemTotalAmount;
                            if (item.ReturnedInPerfectCondition != 0)
                                returnedInPerfectCondition = item.ReturnedInPerfectCondition;
                            else if (item.ReturnedDamaged != 0)
                                returnedDamaged = item.ReturnedDamaged;
                            else if (item.ReturnedInUnknownCondition != 0)
                                returnedInUnknownCondition = item.ReturnedInUnknownCondition;

                            newItem = item;
                        }
                    }
                    newItem.Quantity = quantity;
                    newItem.LineItemTotalAmount = totalLineItemAmt;
                    newItem.ReturnedInPerfectCondition = returnedInPerfectCondition;
                    newItem.ReturnedDamaged = returnedDamaged;
                    newItem.ReturnedInUnknownCondition = returnedInUnknownCondition;
                    newLineItems.Add(newItem);
                    i++;
                }
                doc.LineItems = newLineItems;
                return doc;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occured while trying to consolidate a Baozun return message to the SalesChannelInvoice message " +
                    "in the BC.Integration.AppService.Baozun.SalesDocumentOnRamp.InvoiceLineItemConsolidation method. The Baozun transaction " +
                    "number is: " + doc.Header.InvoiceNumber, ex);
            }
        }

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
                if (file.Substring(file.LastIndexOf("\\") + 1, 2).ToUpper() != "ST")
                {
                    string fileText = File.ReadAllText(file);
                    fileText = fileText.TrimEnd('\r', '\n');
                    data += fileText + "\r\n";
                    //Remove file after processing
                }
                else
                {
                    Trace.WriteLineIf(tracingEnabled, tracingExceptionPrefix + " File skipped as the name starts with ST signifying it is a Sales Tender file.  Filename: " + file);
                    instrumentation.LogGeneralException("File skipped as the name starts with ST signifying it is a Sales Tender file.  Filename: " + file, new Exception(""));
                }
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
            integrator = container.Resolve<IFtpIntegrator>();
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