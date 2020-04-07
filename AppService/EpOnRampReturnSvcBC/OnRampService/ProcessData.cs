using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using System.Xml;
using System.IO;
using BC.Integration.Utility;
using BC.Integration.Interfaces;
using BC.Integration.Canonical.CanonicalReturn;
using BC.Integration.APICalls;

namespace BC.Integration.AppService.EpReturnOnRampServiceBC
{
    public class Process
    {

        string connectionString = "";


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
        /// Process Data is the main method in the 'BC.Integration.AppService.EpReturnOnRampServiceBC class.  It implements the following steps;
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
                   // instrumentation.LogActivation(serviceId, new Guid(activationGuid.Substring(0, 36)), false);


                    //string data;
                    connectionString = Utilities.GetConfigurationValue(configuration, "ConnectionString");

                    //Get data from DB or Local File Pickup
                    if (!localFileSource)
                    {
                        //Get data from DB
                        try
                        {


                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Exception occurred while trying to retrieve and process BC data from EP_Integrations database. ('BC.Integration.AppService.EpReturnOnRampServiceBC.Process.ProcessData method)", ex);
                        }
                    }
                    else
                    {
                        try
                        {

                            //Get local file data
                            batchName = "LocalSalesOrderFile";

                            ProcessFileSourceData(pickupFileFolderPath, activationGuid);


                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Exception occurred while trying to retrieve and process Returns Local Sales files. (BC.Integration.AppService.EpReturnOnRampServiceBC.Process.ProcessData method)" + ex.ToString(), ex);
                        }

                    }


                    //Log Activation End (This step will be skipped if an exception occurs)
                  //  instrumentation.LogActivationEnd(new Guid(activationGuid.Substring(0, 36)));
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End processing data.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the 'BC.Integration.AppService.EpReturnOnRampServiceBC.Process.ProcessData method. Exception message: " + ex.Message);
                    instrumentation.LogGeneralException("An exception was raised when calling the 'BC.Integration.AppService.EpReturnOnRampServiceBC.Process.ProcessData method.", ex);
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
                Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the 'BC.Integration.AppService.EpReturnOnRampServiceBC.Process.ProcessData method trying to resolve the Unity DI components. Exception message: " + ex.Message);
                throw new Exception("An exception occured in the 'BC.Integration.AppService.EpReturnOnRampServiceBC.Process.ProcessData method trying to resolve the Unity DI components.", ex);
            }
        }




        private void ProcessXML(string data, string activationGuid)
        {
            String docId = "";
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " 'BC.Integration.AppService.EpReturnOnRampServiceBC.Process.ProcessFile.  Start Processing File.");
            //Create initial msg envelope to represent the start of the service process and call Instrumenation
            XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, msgType, messageVersion, messageFormat, batchName);
            instrumentation.LogActivity(msg);
            //Log Activation Association with Interchange
          //  instrumentation.AssociateActivationWithInterchages(new Guid(activationGuid.Substring(0, 36)), msgMgr.EntryPointEnvelope.Interchange.InterchangeId);

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " 'BC.Integration.AppService.EpReturnOnRampServiceBC.ProcessData.  Convert XML message to an object.");
            try
            {
                using (TextReader sr = new StringReader(data))
                {

                    Return canonicalReturn = new Return();

                    /*  XmlSerializer serializer = new XmlSerializer(typeof(JMS.ReturnJMS));
                      MemoryStream memStream = new MemoryStream(Encoding.UTF8.GetBytes(data));
                      returnJMS = (JMS.ReturnJMS)serializer.Deserialize(memStream);
                      */
                    var serializer_ = new System.Xml.Serialization.XmlSerializer(typeof(JMS.ReturnJMS));
                    JMS.ReturnJMS returnJMS = (JMS.ReturnJMS)serializer_.Deserialize(sr);
                    canonicalReturn = MapReturnJMSToCanonical(returnJMS);

                    string body = canonicalReturn.ConvertReturnToString();

                    docId = canonicalReturn.Header.OrderNumber;
                    //Create envelope and add canonical message to the envelope
                    HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filterKeyValuePairs);
                    HipKeyValuePairCollection processCol = new HipKeyValuePairCollection(processKeyValuePairs);
                    outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, topic, filterCol, processCol, canonicalReturn.ConvertReturnToString(), 1, null, returnJMS.Header.OrderNumber.Substring(docId.LastIndexOf('.') + 1));

                    int retryCount = 0;

                    //Place message on the message bus
                    PublishMessage(outgoingMessage.InnerXml, filterCol);


                    instrumentation.LogActivity(outgoingMessage, queueUrl, retryCount);
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End processing: " + docId);



                }
            }
            catch (Exception ex)
            {
                string exMessage = ex.Message;
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppServiceEpReturn.OnRampServiceBC.Process.ProcessFile.  The creation of the canonical message failed. The message wasn't sent to the queue. DocumentId: " + docId + " Exception message: " + ex.Message);

                instrumentation.LogMessagingException("An exception occured while processing a message in the " +
               "BC.Integration.AppService.EpReturnOnRampServiceBC.Process.ProcessXML method.  DocumentId: " + docId, outgoingMessage, ex);

                //Notification Log entry...
                if (!string.IsNullOrEmpty(ex.InnerException.Message))
                {
                    exMessage = exMessage + " InnerExceptionMessage: " + ex.InnerException.Message;
                }

                instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "ConversionFromXML",
                "DocumentId: " + docId + " failed with the following error, " + exMessage, docId);


                SaveMessageToFile(ex.Message + "\r\n" + data, serviceId + "." + docId + ".Mapping", true, "Return");

            }

        }

        /// <summary>
        /// Maps new sales order data from JMS to Canonical message from this dll 'BC.Integration.Canonical.SalesChannelOrder'
        /// Order = CanonicalSalesChannelOrder,  NewOrder.Order = JMS Object
        /// </summary>
        /// <param name="salesOrder"></param>
        /// <returns></returns>
        private Return MapReturnJMSToCanonical(JMS.ReturnJMS returnMessage)
        {
            API_Calls APIcalls = new API_Calls();
            Return canonicalReturn = new Return();

            try 
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Start mapping the BC sales order transaction");


                //Populate the message header
                canonicalReturn.Process = processName;
                canonicalReturn.Header = new ReturnHeader();
                canonicalReturn.Header.OrderNumber = returnMessage.Header.OrderNumber;
                canonicalReturn.Header.SiteId = Convert.ToInt32(APIcalls.AllocateBasedOnState(returnMessage.Shipments.Shipment.ShippingAddress.Region, returnMessage.Shipments.Shipment.ShippingAddress.Country));
                canonicalReturn.Header.CurrencyId = returnMessage.Header.Currency;
                canonicalReturn.Header.TaxRegistrationNumber = "";
                canonicalReturn.Header.CarrierCode = APIcalls.GetShipper(APIcalls.ConvertShipmenMethodForEast(canonicalReturn.Header.SiteId.ToString(), returnMessage.Shipments.Shipment.ShipmentCarrier));
                canonicalReturn.Header.PaymentType = "";
                try
                {

                    //API_Calls helper = new API_Calls();
                    canonicalReturn.Header.CustomerId = APIcalls.GetCustomerFromSite(canonicalReturn.Header.SiteId.ToString());

               
                }
                catch (BlueCherryException ex)
                {

                    throw ex;
                }
                catch(Exception ex)
                {
                    throw ex;
                }

                canonicalReturn.Header.CustomerPONum = returnMessage.Customer.CustomerId;
                canonicalReturn.Header.CustEmail = returnMessage.Customer.Email;
                canonicalReturn.Header.isStaffOrder = false;
                canonicalReturn.Header.itHasDiscount = false;
                canonicalReturn.Header.itHasGiftCard = false;
                canonicalReturn.Header.IsForExchangeOrder = false;
                canonicalReturn.Header.OrderDate = Convert.ToDateTime(returnMessage.Header.CreatedDate);
                canonicalReturn.Header.QuoteExpirationDate = DateTime.Now;
                canonicalReturn.Header.CancelDate = Convert.ToDateTime("1900-01-01");
                canonicalReturn.Header.ReqShipDate = Convert.ToDateTime("1900-01-01");
                canonicalReturn.Header.PaymentMethod = "";
                canonicalReturn.Header.Comment = "";
                canonicalReturn.Header.IncoTerms = "";
                canonicalReturn.Header.PriceCode = "OL";
                canonicalReturn.Header.Freight = 0;
                canonicalReturn.Header.TaxAmount = Convert.ToDecimal(returnMessage.Header.TotalTaxes);
                canonicalReturn.Header.Discount = Convert.ToDecimal(returnMessage.Header.TotalDiscountAmount);
             //   canonicalReturn.Header.TaxAmount = 0; //initialize tax a mount to zero because the header might not be corret
                //tax exempt 
                foreach (var payment in returnMessage.Payments.Payment)
                {
                    if (payment.Reason == "Staff Allowance")
                    {
                        canonicalReturn.Header.isStaffOrder = true;
                        break;
                    }
                }

                // canonicalReturn.LineItems = new List<ReturnLine();

                //Loop through the line items in the transaction and create the associated items in the canonical msg
                //NOTE: Sales transactions are split by line item
                canonicalReturn.ReturnLineItems = new List<ReturnLine>();

                foreach (JMS.Return returnLine in returnMessage.Returns.Return)
                {

                    ReturnLine lineItem = new ReturnLine();
                    lineItem.ReturnId = returnLine.ReturnId;
                    lineItem.CreatedDate = Convert.ToDateTime(returnLine.CreatedDate);
                    lineItem.CreatedBy = returnLine.CreatedBy;
                    lineItem.ReceivedBy = returnLine.ReceivedBy;
                    lineItem.RmaCode = returnLine.RmaCode;
                    lineItem.ReturnStatus = returnLine.Status;
                    lineItem.ReturnType = returnLine.ReturnType;
                    if(lineItem.ReturnType == "EXCHANGE")
                    {
                        canonicalReturn.Header.IsForExchangeOrder = true;
                    }
                    lineItem.IsPhysicalReturn = returnLine.PhysicalReturn;
                    lineItem.ExchangeOrderId = returnLine.ExchangeOrderId;
                    canonicalReturn.Header.ExchangeOrderId = lineItem.ExchangeOrderId;
                    lineItem.ShippingCost = returnLine.ShippingCost;
                    lineItem.ShippingDiscount = returnLine.ShippingDiscount;
                    lineItem.LessRestockAmount = returnLine.LessRestockAmount;
                    lineItem.SKU = new List<ReturnSKU>();

                    foreach(JMS.ReturnSku item in returnLine.ReturnSkus.ReturnSku)
                    {
                        ReturnSKU sku = new ReturnSKU();
                        sku.ReturnSKUId = item.ReturnSkuId;
                        sku.ReturnItemCode = item.ReturnItemCode;
                        sku.LineItemId = item.LineItemId;
                        sku.Quantity = item.Quantity;
                        sku.ReceivedQuantity = item.ReceivedQuantity;
                        sku.ReturnAmount = item.ReturnAmount;
                        sku.UnitPrice = item.UnitPrice;
                        sku.Tax = item.Tax; 
                        canonicalReturn.Header.TaxAmount += item.Tax; //take taxes from the returned item instead
                        sku.ItemSubtotalPrice = item.ItemSubtotalPrice;
                        sku.AmountBeforeTax = item.AmountBeforeTax;
                        sku.AmountInludingTax = item.AmountIncludingTax;
                        sku.ReturnReason = item.ReturnReason;
                        sku.ReceivedState = item.ReceivedState;


                        try
                        {
                            //unable to get invoice number
                            // API_Calls helper = new API_Calls();
                            // lineItem.OrderInvoiceNumber = helper.GetInvoiceNumberFromOrder(canonicalReturn.Header.OrderNumber);
                            lineItem.OrderInvoiceNumber = "100175";


                        }
                        catch (BlueCherryException ex)
                        {

                            throw ex;
                        }
                        lineItem.AddSku(sku);
                    }
                   
                    canonicalReturn.ReturnLineItems.Add(lineItem);
                    canonicalReturn.Addresses = new List<ReturnAddress>();

                    ReturnAddress shippingAddress = new ReturnAddress();
                    ReturnAddress billingAddress = new ReturnAddress();

                    shippingAddress.AddressId = returnLine.ReturnAddress.AddressId;
                    shippingAddress.Add1 = returnLine.ReturnAddress.Street1;
                    shippingAddress.Add2 = returnLine.ReturnAddress.Street2;
                    shippingAddress.Add3 = "";
                    shippingAddress.AddCity = returnLine.ReturnAddress.City;
                    shippingAddress.State = returnLine.ReturnAddress.Region;
                    shippingAddress.AddPostalCode = returnLine.ReturnAddress.ZipPostalCode;
                    shippingAddress.Country = returnLine.ReturnAddress.Country;
                    shippingAddress.Phone = returnLine.ReturnAddress.PhoneNumber;
                    shippingAddress.Email = returnMessage.Customer.Email;
                    shippingAddress.AddressType =  shipToAddressType;
                    shippingAddress.CustomerName = returnLine.ReturnAddress.FirstName + " " + returnLine.ReturnAddress.LastName;

                    billingAddress = shippingAddress.DeepCopy();
                    billingAddress.AddressType =  billToAddressType;



                    canonicalReturn.Addresses.Add(shippingAddress);
                    canonicalReturn.Addresses.Add(billingAddress);

             
                }


            }               
  
            catch (Exception ex)
            {

                Trace.WriteLine(tracingExceptionPrefix + " An error occured while trying to map the EP batch message to the SalesChannelOrder message in MapSalesOrdersToCanonical()");
                throw new Exception("An error occured while trying to map the EP sales order batch message to the SalesChannelOrder message" +
                    "in the BC.Integration.EpReturnOnRampServiceBC.Process.MapReturnJMSToCanonical method. The EP sales order transaction " +
                    "number is: " + returnMessage.Header.OrderNumber, ex);
            }

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End mapping the EP sales order transaction");
            return canonicalReturn;
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




          /// <summary>
          /// Helper method to standardize behaviour when saving files and update the process name tags.
          /// </summary>
          /// <param name="message">Message (data) to be saved to files</param>
          /// <param name="fileName">Provide the start of the file name. Format for tracing: 'svcID.Doc#.SiteID', for exceptions: ‘svcID.Doc#.SiteID.ValidationType’
          /// Or ‘SvcID.ValidationType’ depending what variables are available.  Time and '.txt' will be added later. </param>
          /// <param name="exception">Flag to identify which folder path to use (Exception or Tracing)</param>
          private void SaveMessageToFile(string message, string fileName, bool exception, string type)
          {          

              string path;
              if (exception)
              {
                  path = exceptionMessageFolderPath.Replace("#ProcessName#", processName);

                  //fileName Time and txt will be added by instrumentation.
                  instrumentation.WriteMsgToFile(path, null, message, fileName, null);
              }

          }

          /// <summary>
          /// This method allows the pickup of files from a local folder.
          /// </summary>
          /// <param name="pickupMessageFolderPath">Folder path to pickup the files</param>
          /// <returns></returns>
          private void ProcessFileSourceData(string pickupMessageFolderPath, string activationGuid)
          {
              //This is setup for JSON data...
              string fileText = "";

              string path = pickupMessageFolderPath;
              string tracing_path = tracingMessageFolderPath.Replace("#ProcessName#", processName);

              var fileNames = Directory.EnumerateFiles(path);
              foreach (string file in fileNames)
              {
                  Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Get file messages.  Filename: " + file);
                  fileText = File.ReadAllText(file);

                  //Save the file in Tracing Enabled folder
                  if (tracingEnabled)
                  {
                      instrumentation.WriteMsgToFile(tracing_path, null, fileText, Path.GetFileName(file), null);
                  }
                  //Remove file after processing
                  File.Delete(file);

                  if (!String.IsNullOrEmpty(fileText))
                  {
                      ProcessXML(fileText, activationGuid);
                  }
              }          


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
            PopulateLocalVaribale();
            instrumentation = container.Resolve<IInstrumentation>();
            instrumentation.Configuration = configuration;
            publishService = container.Resolve<IPublishService>();
            publishService.Configuration = configuration;
        }

    }
}