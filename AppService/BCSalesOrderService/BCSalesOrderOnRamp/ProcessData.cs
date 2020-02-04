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

using System.Text;
using System.Linq;
using BC.Integration.Canonical.SaleChannelOrder.New;
using System.Globalization;
using System.Text.RegularExpressions;

namespace BC.Integration.AppService.BCSalesOrderSvc
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
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
        private string shipToAddressType = ConfigurationManager.AppSettings["ShipToAddressType"];
        private string billToAddressType = ConfigurationManager.AppSettings["BillToAddressType"];



        //These rest of the properties could be overridden by the central config store at the service ot process level
        private string archiveFolder = "";

        //Msg Envelope properties
        private string msgType = "";
        private string filterKeyValuePairs = "";
        private string processKeyValuePairs = "";
        private string topic = "";

        //Messaging Queue Properties
        private string queueUrl = "";

        private string batchData = "";
        private MessageManager msgMgr = new MessageManager();
        private string batchName = "";
        private string documentNumber;
        private XmlDocument outgoingMessage;
        private string messageFormat = "xml";
        private decimal serviceVersion = 1;
        private decimal messageVersion = 1;
        private bool tracingEnabled = false;
        private bool localFileSource = false;
        private string exceptionMessageFolderPath = "";
        private string tracingMessageFolderPath = "";
        private string pickupFileFolderPath = "";

        //SalesOrder salesOrder = new SalesOrder();
        SalesOrders orderList = new SalesOrders();

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
            //messageFormat = Utilities.GetConfigurationValue(configuration, "MessageFormat");
            //serviceVersion = Convert.ToDecimal(Utilities.GetConfigurationValue(configuration, "ServiceVersion"));
            //messageVersion = Convert.ToDecimal(Utilities.GetConfigurationValue(configuration, "MessageVersion"));
            archiveFolder = Utilities.GetConfigurationValue(configuration, "ArchiveFolder");
            exceptionMessageFolderPath = Utilities.GetConfigurationValue(configuration, "ExceptionMessageFolderPath");
            tracingMessageFolderPath = Utilities.GetConfigurationValue(configuration, "TracingMessageFolderPath");
            pickupFileFolderPath = Utilities.GetConfigurationValue(configuration, "PickupSalesOrderFileFolderPath");
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
        /// Process Data is the main method in the BC.Integration.AppService.BCSalesOrderSvc class.  It implements the following steps;
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

                    int msgCount = 0;
                    string data;

                    //Get data from Source System or Local File Pickup
                    if (!localFileSource)
                    {
                        return;
                    }
                    else
                    {
                        try
                        {
                            //Get local file data
                            batchName = "LocalSalesOrderFile";
                            data = GetFileSourceData(pickupFileFolderPath);
                            if (!String.IsNullOrEmpty(data))
                            {
                                ProcessData(data, activationGuid);
                            }
                            
                           

                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Exception occurred while trying to retrieve and process Tigers Local Sales files. (BC.Integration.AppService.BCSalesOrderSvc.Process.ProcessData method)" + ex.ToString() , ex);
                        }
                    }

                    //Log Activation End (This step will be skipped if an exception occurs)
                    instrumentation.LogActivationEnd(new Guid(activationGuid.Substring(0, 36)));
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End processing data.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the BC.Integration.AppService.BCSalesOrderSvc.Process.ProcessData method. Exception message: " + ex.Message);
                    instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.AppService.BCSalesOrderSvc.Process.ProcessData method.", ex);
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
                Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the BC.Integration.AppService.BCSalesOrderSvc.Process.ProcessData method trying to resolve the Unity DI components. Exception message: " + ex.Message);
                throw new Exception("An exception occured in the BC.Integration.AppService.BCSalesOrderSvc.Process.ProcessData method trying to resolve the Unity DI components.", ex);
            }
        }

        /// <summary>
        /// Process incoming GP sales orders to map to canonical 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="activationGuid"></param>
        private void ProcessData(string data, string activationGuid)
        {

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.BCSalesOrderSvc.ProcessData.  Start Processing File.");

            List<string> failedTransNums = new List<string>();

            //Create initial msg envelope to represent the start of the service process and call Instrumenation
            XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, msgType, messageVersion, messageFormat, batchName);
            instrumentation.LogActivity(msg);

            //Log Activation Association with Interchange
            instrumentation.AssociateActivationWithInterchages(new Guid(activationGuid.Substring(0, 36)), msgMgr.EntryPointEnvelope.Interchange.InterchangeId);

            SalesOrders orders = new SalesOrders();

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.BCSalesOrderSvc.ProcessData.  Convert string sales order data to string array.");

            //data = StringToCSVCell(data);
            string[] colLines = data.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);


            /***  TEST ZONE ***/
           

            try
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.BCSalesOrderSvc.ProcessData.  Load SalesOrders object from string[] colLines.");

                orders = AddOrders(colLines);

            }
            catch (Exception ex)
            {
                //If we get a badly formed line, we need to make sure all lines for that transaction number are removed. Identify transaction number of failed line
                string failedTransNum = orders.salesOrders[orders.salesOrders.Count - 1].orderNumber;
                failedTransNums.Add(failedTransNum);
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Transaction Number: " + failedTransNum + " failed to be converted from the CSV data.  Exception message: " + ex.Message);
                instrumentation.LogGeneralException("An exception occured while processing a message in the " +
                    "BC.Integration.AppService.BCSalesOrderSvc.Process.ProcessFile method. The current Tigers file being processed is - " + batchName +
                    ", transaction number being processed is " + failedTransNum + " and the error occured on line " + orders.salesOrders[orders.salesOrders.Count - 1].salesOrderLineItem + " in the file.  This transaction in the file has not been processed.", ex);
                //Notification Log entry...
                instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "ConversionFromCSVToObject",
                    "CSV filename: " + batchName + " failed with the following error, " + ex.Message, failedTransNum);
            }


            int i = 1;
            foreach (SalesOrder salesOrder in orders.salesOrders)
            {
                try
                {
                    if ((salesOrder.siteId == 60) || (salesOrder.siteId == 61) || (salesOrder.siteId == 66) || (salesOrder.siteId == 91))
                    {
                        Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Processing Tigers sales file.  Transaction Number: " + salesOrder.orderNumber);

                        //Call a method to process each transaction and map to the Sales Document
                        Order order = MapSalesOrdersToCanonical(salesOrder);

                        //Create envelope and add canonical message to the envelope
                        HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filterKeyValuePairs);
                        HipKeyValuePairCollection processCol = new HipKeyValuePairCollection(processKeyValuePairs);
                        outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, topic, filterCol, processCol, order.ConvertOrderToString(order), i, null, order.Header.OrderNumber.Substring(order.Header.OrderNumber.LastIndexOf('.') + 1));

                        int retryCount = 0;

                        //Place message on the message bus
                        PublishMessage(outgoingMessage.InnerXml, filterCol);

                        //Save a copy of message
                        if (tracingEnabled)
                        {
                            string site = order.Header.SiteId.ToString();
                            string textBody = "RAW\r\n";
                            foreach (SalesOrderLineItem item in salesOrder.salesOrderLineItem)
                            {
                                textBody += item.ConvertToCsv() + "\r\n";
                            }
                            textBody += "\r\nXML\r\n" + order.ConvertOrderToString(order);

                            SaveMessageToFile(textBody, serviceId + "." + salesOrder.orderNumber + "." + site, false);
                        }

                        //Log Activity
                        instrumentation.LogActivity(outgoingMessage, queueUrl, retryCount);
                        Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End processing: " + salesOrder.orderNumber);
                    }
                    i++;
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + " An exception occured while processing data in BC.Integration.AppService.BCSalesOrderSvc.Process.ProcessFile. Exception message: " + ex.Message);
                    instrumentation.LogMessagingException("An exception occured while processing a message in the " +
                        "BC.Integration.AppService.BCSalesOrderSvc.Process.ProcessFile method. The current process is - " + processName +
                        " document number being processed is " + salesOrder.orderNumber + ".", outgoingMessage, ex);
                }
            }


        }

        /// <summary>
        /// Map sales order data to Canonical message from this dll 'BC.Integration.Canonical.SalesChannelOrder'
        /// </summary>
        /// <param name="salesOrder"></param>
        /// <returns></returns>
        private Order MapSalesOrdersToCanonical(SalesOrder salesOrder)
        {
            try
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Start mapping the Tigers sales order transaction");
                Order order = new Order();
                order.Header = new OrderHeader();

                //Populate the message header
                order.Process = processName;
                //order.Header.OrderNumber = salesOrder.orderNumber.Substring(0, salesOrder.orderNumber.IndexOf('.')); //JP why are we removing the subscript .0 .1, et?
                order.Header.OrderNumber = salesOrder.orderNumber; //JP why are we removing the subscript .0 .1, et?

                order.Header.SiteId = salesOrder.siteId;
                order.Header.CurrencyId = salesOrder.currencyId;

                // Only EU B2B needs the TaxRegistrationNumber
                if (salesOrder.siteId == 60)
                {
                    if (String.IsNullOrEmpty(salesOrder.taxRegistrationNum))
                    {
                        instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "TaxRegistrationNum not found",
                         "An error occured while trying to map the Tigers batch message to the SalesChannelOrder message in MapSalesOrdersToCanonical(). The TaxRegistrationNumber is required for EU B2B orders.", salesOrder.orderNumber);

                        throw new Exception("An error occured while trying to map the Tigers batch message to the SalesChannelOrder message in MapSalesOrdersToCanonical(). The TaxRegistrationNumber is required for EU B2B orders.");
                    }
                    order.Header.TaxRegistrationNumber = salesOrder.taxRegistrationNum;
                }
                else
                {
                    order.Header.TaxRegistrationNumber = "";
                }

                order.Header.CarrierCode = salesOrder.carrierCode;
                order.Header.PaymentType = salesOrder.paymentType;
                order.Header.CustomerId = salesOrder.customerId;
                order.Header.CustomerPONum = salesOrder.customerPONum;

                order.Header.CustEmail = salesOrder.customerEmail; //added

                order.Header.OrderDate = salesOrder.orderDate;
                order.Header.QuoteExpirationDate = DateTime.Now;
                order.Header.CancelDate = salesOrder.cancelDate;
                order.Header.ReqShipDate = salesOrder.reqShipDate;
                order.Header.PaymentMethod = salesOrder.paymentMethod;
                order.Header.Comment = salesOrder.sopHeaderComment;
                order.Header.IncoTerms = salesOrder.incoTerms == "" ? "DDP" : salesOrder.incoTerms; //Sid review this

                

                order.LineItems = new List<OrderLineItem>();

                //Loop through the line items in the transaction and create the associated items in the canonical msg
                //NOTE: Sales transactions are split by line item
                for (int i = 0; i < salesOrder.salesOrderLineItem.Count; i++)
                {
                    OrderLineItem item = new OrderLineItem();
                    item.LineSeqNum = salesOrder.salesOrderLineItem[i].LineSeqNum;
                    item.ItemNumber = salesOrder.salesOrderLineItem[i].ItemNum;
                    item.UnitOfMeasure = salesOrder.salesOrderLineItem[i].UnitOfMeasure;
                    item.UnitQuantity = salesOrder.salesOrderLineItem[i].UnitQuantity;
                    item.UnitPrice = salesOrder.salesOrderLineItem[i].UnitPrice;
                    item.Comment = salesOrder.salesOrderLineItem[i].ItemLineComment;
                    item.ShipDate = salesOrder.salesOrderLineItem[i].ItemShipDate;
                    item.Category = salesOrder.salesOrderLineItem[i].ItemCategory;
                    item.ProductName = salesOrder.salesOrderLineItem[i].ItemProductName;
                    item.Size = salesOrder.salesOrderLineItem[i].ItemSize;
                    item.Colour = salesOrder.salesOrderLineItem[i].ItemColour;
                    item.Fabric = salesOrder.salesOrderLineItem[i].ItemFabric;
                    //item.ord = salesOrder.salesorderlineItem.OrderNumber;
                    item.SiteId = salesOrder.salesOrderLineItem[i].SiteId;

                    order.LineItems.Add(item);
                }

                order.Addresses = new List<OrderAddress>();
                OrderAddress address = new OrderAddress();


                //address.AddressId = salesOrder.shippingAddressId;
                address.AddressId = "";    // ConsigneeCode needs to be blank Jira: HSC-505
                address.Add1 = salesOrder.shippingAddress1;
                address.Add2 = salesOrder.shippingAddress2;
                address.Add3 = salesOrder.shippingAddress3;
                address.AddCity = salesOrder.shippingcity;
                address.AddPostalCode = salesOrder.shippingpostalCode;
                address.Country = salesOrder.shippingcountry;
                address.Phone = salesOrder.shippingphone;
                address.Email = salesOrder.customerEmail;
                address.AddressType = shipToAddressType;
                address.CustomerName = salesOrder.shippingName;
                order.Addresses.Add(address);

                address = new OrderAddress();

                address.AddressId = salesOrder.billingaddressId;
                address.Add1 = salesOrder.billingcustomerAddress1;
                address.Add2 = salesOrder.billingcustomerAddress2;
                address.Add3 = salesOrder.billingcustomerAddress3;
                address.AddCity = salesOrder.billingcity;
                address.AddPostalCode = salesOrder.billingpostalCode;
                address.Country = salesOrder.billingcountry;
                address.Phone = salesOrder.billingphone;
                address.Email = salesOrder.customerEmail;
                address.AddressType = billToAddressType;
                address.CustomerName = salesOrder.billingcustomerName;

                order.Addresses.Add(address);

                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End mapping the Tigers sales order transaction");
                return order;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + " An error occured while trying to map the Tigers batch message to the SalesChannelOrder message in MapSalesOrdersToCanonical()");
                throw new Exception("An error occured while trying to map the Tigers sales order batch message to the SalesChannelOrder message" +
                    "in the BC.Integration.AppService.BCSalesOrderSvc.Process.MapSalesOrdersToCanonical method. The Tigers sales order transaction " +
                    "number is: " + salesOrder.orderNumber, ex);
            }
        }

        /// <summary>
        /// Process .csv files to create and update associated object model
        /// </summary>
        /// <param name="colLines"></param>
        /// <returns></returns>
        public SalesOrders AddOrders(string[] colLines)
        {
            SalesOrder order = new SalesOrder();
            SalesOrderLineItem item = new SalesOrderLineItem();
            SalesOrderAddress address = new SalesOrderAddress();
            string[] colfields;
            decimal d;
            double dbl;
            int n;
            int a;
            string last = colLines.Last();

            foreach (string csvLine in colLines)
            {
                // extract the fields
                Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

                
                colfields = CSVParser.Split(csvLine).Select(s => s.Trim('"')).ToArray();

                //colfields = csvLine.Split(',').Select(s => s.Trim('"')).ToArray();

                if (colfields.Count() > 80)
                {
                    throw new Exception("The AddLine method of the BC.Integration.AppService.Tigers.Orders class was given a data line with too many" +
                        "elements.  The dataset should have 20 elements.  Please ensure the data does not contain any extra commas or missing trailing commas.");
                }

                if (order.orderNumber != colfields[16] && order.orderNumber != null)  // next order
                {
                    orderList.salesOrders.Add(order);
                    order = new SalesOrder();
                }

                #region AddOrder

                order.orderNumber = colfields[16];
                order.customerId = colfields[20];

                order.shippingAddressId = colfields[30];
                order.shippingName = colfields[21];
                order.shippingAddress1 = colfields[22];
                order.shippingAddress2 = colfields[23];
                order.shippingAddress3 = colfields[24];
                order.shippingcity = colfields[25];
                order.shippingpostalCode = colfields[27];
                order.shippingcountry = colfields[28];
                order.shippingphone = colfields[29];

                order.billingaddressId = colfields[30];
                order.billingcustomerName = colfields[31];
                order.billingcustomerAddress1 = colfields[32];
                order.billingcustomerAddress2 = colfields[33];
                order.billingcustomerAddress3 = colfields[34];
                order.billingcity = colfields[35];
                order.billingpostalCode = colfields[37];
                order.billingcountry = colfields[38];
                order.billingphone = colfields[39];

                order.customerPONum = colfields[40];
                order.orderType = colfields[41];

                try
                {
                    //salesOrder.orderDate = FromExcelSerialDate(colfields[43]);
                    if(!(colfields[43] == "0/0/0000"))
                    {
                        order.orderDate = DateTime.Parse(colfields[43]);

                    }
                    else
                    {
                        if (DateTime.Parse(colfields[46]) < DateTime.Today)
                        {
                            order.orderDate = DateTime.Parse(colfields[46]);

                        }
                        else
                        {
                            order.orderDate = DateTime.Today;

                        }

                    }

                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to create Delivery Date field for transaction number: " + order.orderNumber + ".  The value could not be converted (" + colfields[5] + ").", ex);
                }

                try
                {
                    DateTime dt;
                    if (DateTime.TryParseExact(colfields[44], "MM/dd/yyyy", null, DateTimeStyles.None, out dt) == true)
                    {
                        order.quoteExpirationDate = dt;
                    }
                    else
                    {
                        order.quoteExpirationDate = null;
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to create Delivery Date field for transaction number: " + order.orderNumber + ".  The value could not be converted (" + colfields[5] + ").", ex);
                }

                try
                {
                    order.cancelDate = DateTime.Parse(colfields[45]);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to create Delivery Date field for transaction number: " + order.orderNumber + ".  The value could not be converted (" + colfields[5] + ").", ex);
                }

                try
                {
                    order.reqShipDate = DateTime.Parse(colfields[48]);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to create Delivery Date field for transaction number: " + order.orderNumber + ".  The value could not be converted (" + colfields[5] + ").", ex);
                }

                order.paymentMethod = colfields[50];
                order.carrierCode = colfields[54];
                order.sopHeaderComment = colfields[65];
                if (Utilities.ProcessIntString(colfields[67], out n))
                {
                    order.siteId = n;
                }
                else
                {
                    throw new Exception("Failed to set the Actual Price field for transaction number: " + order.orderNumber + ".  The value was not recognised as a valid value (" + colfields[9] + ").");
                }
                order.customerEmail = colfields[74];
                order.currencyId = colfields[76];
                order.taxRegistrationNum = colfields[77];
                order.incoTerms = colfields[78];

                #endregion

                #region AddItem

                item = new SalesOrderLineItem();

                item.LineSeqNum = colfields[0];
                item.ItemNum = colfields[1];
                item.UnitOfMeasure = colfields[2];

                if (double.TryParse(colfields[3], out dbl))
                {
                    item.UnitQuantity = Convert.ToInt32(dbl);
                }
                else
                {
                    throw new Exception("Failed to set the Returned Goods in a Good Condition field for transaction number: " + item.OrderNumber + ".  The value was not recognised as a valid value (" + colfields[19] + ").");
                }

                if (Utilities.ProcessDecimalString(colfields[4], out d))
                {
                    item.UnitPrice = d;
                }
                else
                {
                    throw new Exception("Failed to set the Actual Price field for transaction number: " + item.OrderNumber + ".  The value was not recognised as a valid value (" + colfields[9] + ").");
                }

                item.ItemLineComment = colfields[5];
                item.ItemCategory = colfields[6];
                item.ItemProductName = colfields[7];
                item.ItemSize = colfields[8];
                item.ItemColour = colfields[9];
                item.ItemFabric = colfields[10];
                item.OrderNumber = colfields[14];

                if (Utilities.ProcessIntString(colfields[15], out n))
                {
                    item.SiteId = n;
                }
                else
                {
                    throw new Exception("Failed to set the Actual Price field for transaction number: " + item.OrderNumber + ".  The value was not recognised as a valid value (" + colfields[9] + ").");
                }

                if (order.salesOrderLineItem == null)
                {
                    order.salesOrderLineItem = new List<SalesOrderLineItem>();
                }
                if (orderList.salesOrders == null)
                {
                    orderList.salesOrders = new List<SalesOrder>();
                }

                #endregion

                order.salesOrderLineItem.Add(item);

                //if it's the last order
                if (csvLine.Equals(last))
                {
                    orderList.salesOrders.Add(order);
                }
            }
            return orderList;
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
            string strUrls = "";
            List<string> urls;

            //Get SQS Queue URL's
            urls = config.GetDestinationQueueUrls(processName, serviceId, servicePostOperationId, msgType, filterCol.ConvertToKeyValuePairs());

            foreach (string url in urls)
            {
                publishService.Push(url, msgString, out retryCount);
                strUrls += url.Substring(url.LastIndexOf('/') + 1) + ",";
            }
            //Set queueUrl for the instrumentation
            queueUrl = strUrls.TrimEnd(',');
        }

        ///// <summary>
        ///// Checks a collection of failed transaction numbers against the current transaction number.  If there is a match the method
        ///// returns false.
        ///// </summary>
        ///// <param name="transactionNum">Current Transaction Number</param>
        ///// <param name="failedTransNums">Collection of failed transaction numbers</param>
        ///// <returns>Boolean</returns>
        //private bool HasFailedLineItems(string orderNumner, List<string> failedTransNums)
        //{
        //    foreach (string num in failedTransNums)
        //    {
        //        if (orderNumner == num)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

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