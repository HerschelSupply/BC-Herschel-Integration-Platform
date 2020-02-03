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
using System.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using BC.Integration.Canonical.SaleChannelOrder;
using System.Data.SqlClient;
using System.Data;
using System.Net;
using Newtonsoft.Json.Linq;
using BC_API_Calls;
using BC.Integration.Canonical.SaleChannelOrder.New;

namespace BC.Integration.AppService.EpOnRampServiceBC
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
        /// Process Data is the main method in the BC.Integration.AppService.EpOnRampServiceBC class.  It implements the following steps;
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
                    connectionString = Utilities.GetConfigurationValue(configuration, "ConnectionString");

                    //Get data from DB or Local File Pickup
                    if (!localFileSource)
                    {
                        //Get data from DB
                        try
                        {

                            using (SqlConnection connection = new SqlConnection(connectionString))
                            {

                                SqlCommand command = new SqlCommand();
                                command.Connection = connection;
                                command.CommandType = CommandType.StoredProcedure;
                                command.CommandText = "sp_ep_ImportedOrders_BC";
                                connection.Open();
                                SqlDataReader reader = command.ExecuteReader();

                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        ProcessXML(reader["eConnectXML"].ToString(), activationGuid);
                                    }
                                }

                                reader.Close();
                                connection.Close();
                            }
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Exception occurred while trying to retrieve and process BC data from EP_Integratios database. (BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessData method)", ex);
                        }
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
                                ProcessXML(data, activationGuid);
                            }



                        }
                        catch (Exception ex)
                        {
                            throw new Exception("An Exception occurred while trying to retrieve and process Tigers Local Sales files. (BC.Integration.AppService.GPSalesOrderSvc.Process.ProcessData method)" + ex.ToString(), ex);
                        }

                    }
                                        

                    //Log Activation End (This step will be skipped if an exception occurs)
                    instrumentation.LogActivationEnd(new Guid(activationGuid.Substring(0, 36)));
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End processing data.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessData method. Exception message: " + ex.Message);
                    instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessData method.", ex);
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
                Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessData method trying to resolve the Unity DI components. Exception message: " + ex.Message);
                throw new Exception("An exception occured in the BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessData method trying to resolve the Unity DI components.", ex);
            }
        }



        private void ProcessEConnectXML(string data, string activationGuid)
        {
            String docId = "";
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessFile.  Start Processing File.");
            //Create initial msg envelope to represent the start of the service process and call Instrumenation
            XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, msgType, messageVersion, messageFormat, batchName);
            instrumentation.LogActivity(msg);
            //Log Activation Association with Interchange
            instrumentation.AssociateActivationWithInterchages(new Guid(activationGuid.Substring(0, 36)), msgMgr.EntryPointEnvelope.Interchange.InterchangeId);
            
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.EpOnRampServiceBC.ProcessData.  Convert XML message to an object.");
            try
            {
                 using (TextReader sr = new StringReader(data))
                {
                    var serializer_ = new System.Xml.Serialization.XmlSerializer(typeof(EConnect));
                    EConnect response = (EConnect)serializer_.Deserialize(sr);
                    Order order = MapEconnectXMLToCanonical(response);
                    docId = order.Header.OrderNumber;
                    //Create envelope and add canonical message to the envelope
                    HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filterKeyValuePairs);
                    HipKeyValuePairCollection processCol = new HipKeyValuePairCollection(processKeyValuePairs);
                    outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, topic, filterCol, processCol, order.ConvertOrderToString(order), 1, null, order.Header.OrderNumber.Substring(order.Header.OrderNumber.LastIndexOf('.') + 1));

                    int retryCount = 0;

                    //Place message on the message bus
                    PublishMessage(outgoingMessage.InnerXml, filterCol);


                    //Mark the transaction as processed in the DB
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {

                        SqlCommand command = new SqlCommand();
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "sp_process_ep_ImportedOrders_BC";
                        connection.Open();

                        SqlParameter param1 = new SqlParameter("SOPNUMBER", SqlDbType.VarChar, 50);
                        param1.Value = docId;
                        command.Parameters.Add(param1);

                        int count = command.ExecuteNonQuery();
                        command.Parameters.Clear();
                        connection.Close();

                    }


                    //Log Activity
                     instrumentation.LogActivity(outgoingMessage, queueUrl, retryCount);
                     Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End processing: " + docId);

                }

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessFile.  The creation of the canonical message failed. The message wasn't sent to the queue. DocumentId: " + docId + " Exception message: " + ex.Message);
                instrumentation.LogGeneralException("An exception occured while processing a message in the " +
                "BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessXML method.  DocumentId: " + docId + " The current XML message processed is - " + data
                 , ex);

                //Notification Log entry...
                if (ex.InnerException.Source == "BC_API_Calls")
                {
                    instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "ConversionFromXML",
                    "CSV filename: " + batchName + " failed with the following error: " + ex.InnerException.Message, docId);
                }
                else
                {
                   
                    instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "ConversionFromXML",
                    "CSV filename: " + batchName + " failed with the following error, " + ex.Message, docId);
                }              
                
            }
            
        }

        private void ProcessXML(string data, string activationGuid)
        {
            String docId = "";
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessFile.  Start Processing File.");
            //Create initial msg envelope to represent the start of the service process and call Instrumenation
            XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, msgType, messageVersion, messageFormat, batchName);
            instrumentation.LogActivity(msg);
            //Log Activation Association with Interchange
            instrumentation.AssociateActivationWithInterchages(new Guid(activationGuid.Substring(0, 36)), msgMgr.EntryPointEnvelope.Interchange.InterchangeId);

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.EpOnRampServiceBC.ProcessData.  Convert XML message to an object.");
            try
            {
                using (TextReader sr = new StringReader(data))
                {
                    XmlDocument doc = new XmlDocument();
                    Order order = new Order();

                    doc.LoadXml(data);

                    string status = doc.InnerText.Substring(0, 11);
                    if (status.Contains("IN_PROGRESS"))
                    {
                        var serializer_ = new System.Xml.Serialization.XmlSerializer(typeof(NewOrder.Order));
                        NewOrder.Order response = (NewOrder.Order)serializer_.Deserialize(sr);
                        order = MapSalesOrdersToCanonical(response);
                    }
                    else if (status.Contains("COMPLETED"))
                    {

                    }
                    else
                    {  //eConnect MSG from DB

                        var serializer_ = new System.Xml.Serialization.XmlSerializer(typeof(EConnect));
                        EConnect response = (EConnect)serializer_.Deserialize(sr);
                        order = MapEconnectXMLToCanonical(response);
                    }
                    

                    //var serializer_ = new System.Xml.Serialization.XmlSerializer(typeof(EConnect));
                    //EConnect response = (EConnect)serializer_.Deserialize(sr);
                    //Order order = MapSalesOrdersToCanonical(response);
                    docId = order.Header.OrderNumber;
                    //Create envelope and add canonical message to the envelope
                    HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filterKeyValuePairs);
                    HipKeyValuePairCollection processCol = new HipKeyValuePairCollection(processKeyValuePairs);
                    outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, topic, filterCol, processCol, order.ConvertOrderToString(order), 1, null, order.Header.OrderNumber.Substring(order.Header.OrderNumber.LastIndexOf('.') + 1));

                    int retryCount = 0;

                    //Place message on the message bus
                    PublishMessage(outgoingMessage.InnerXml, filterCol);


                    //Mark the transaction as processed in the DB
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {

                        SqlCommand command = new SqlCommand();
                        command.Connection = connection;
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "sp_process_ep_ImportedOrders_BC";
                        connection.Open();

                        SqlParameter param1 = new SqlParameter("SOPNUMBER", SqlDbType.VarChar, 50);
                        param1.Value = docId;
                        command.Parameters.Add(param1);

                        int count = command.ExecuteNonQuery();
                        command.Parameters.Clear();
                        connection.Close();

                    }


                    //Log Activity
                    instrumentation.LogActivity(outgoingMessage, queueUrl, retryCount);
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End processing: " + docId);

                }

            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessFile.  The creation of the canonical message failed. The message wasn't sent to the queue. DocumentId: " + docId + " Exception message: " + ex.Message);
                instrumentation.LogGeneralException("An exception occured while processing a message in the " +
                "BC.Integration.AppService.EpOnRampServiceBC.Process.ProcessXML method.  DocumentId: " + docId + " The current XML message processed is - " + data
                 , ex);

                //Notification Log entry...
                if (ex.InnerException.Source == "BC_API_Calls")
                {
                    instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "ConversionFromXML",
                    "CSV filename: " + batchName + " failed with the following error: " + ex.InnerException.Message, docId);
                }
                else
                {

                    instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "ConversionFromXML",
                    "CSV filename: " + batchName + " failed with the following error, " + ex.Message, docId);
                }

            }

        }
        /// <summary>
        /// Map sales order data from eConnect XML to Canonical message from this dll 'BC.Integration.Canonical.SalesChannelOrder'
        /// </summary>
        /// <param name="salesOrder"></param>
        /// <returns></returns>
        private Order MapEconnectXMLToCanonical(EConnect salesOrder)
        {
            try
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Start mapping the BC sales order transaction");
                Order order = new Order();
                order.Header = new OrderHeader();



                //Populate the message header
                order.Process = processName;
                order.Header.OrderNumber = salesOrder.SOPTransactionType.TaSopHdrIvcInsert.SOPNUMBE;
                order.Header.SiteId = Convert.ToInt32(salesOrder.SOPTransactionType.TaSopHdrIvcInsert.LOCNCODE);
                order.Header.CurrencyId = salesOrder.SOPTransactionType.TaSopHdrIvcInsert.CURNCYID;
                order.Header.TaxRegistrationNumber = "";
                order.Header.CarrierCode = "";
                order.Header.PaymentType = "";
                order.Header.CustomerId = salesOrder.SOPTransactionType.TaSopHdrIvcInsert.CUSTNMBR;
                order.Header.CustomerPONum = salesOrder.SOPTransactionType.TaSopHdrIvcInsert.CUSTNMBR;
                order.Header.CustEmail = "";


                order.Header.OrderDate = Convert.ToDateTime(salesOrder.SOPTransactionType.TaSopHdrIvcInsert.DOCDATE);
                order.Header.QuoteExpirationDate = DateTime.Now;
                order.Header.CancelDate = Convert.ToDateTime("1900-01-01");
                order.Header.ReqShipDate = Convert.ToDateTime(salesOrder.SOPTransactionType.TaSopHdrIvcInsert.ReqShipDate);
                order.Header.PaymentMethod = "";
                order.Header.Comment = "";
                order.Header.IncoTerms = "";
                order.Header.PriceCode = salesOrder.SOPTransactionType.TaSopHdrIvcInsert.PRCLEVEL;
                order.Header.Freight = Convert.ToDecimal(salesOrder.SOPTransactionType.TaSopHdrIvcInsert.FREIGHT);
                order.Header.TaxAmount = Convert.ToDecimal(salesOrder.SOPTransactionType.TaSopHdrIvcInsert.TAXAMNT);



                order.LineItems = new List<OrderLineItem>();

                //Loop through the line items in the transaction and create the associated items in the canonical msg
                //NOTE: Sales transactions are split by line item

                decimal OrderDiscount = 0;
                foreach (TaSopLineIvcInsert so_item in salesOrder.SOPTransactionType.TaSopLineIvcInsert)
                {

                    if (so_item.ITEMNMBR == "HSC-DISCOUNT")
                    {
                        if (so_item.ITEMDESC.Contains("ORDER"))
                        {
                            OrderDiscount += Math.Abs(Convert.ToDecimal(so_item.UNITPRCE));
                        }
                    }
                    else
                    {

                        OrderLineItem item = new OrderLineItem();

                        item.LineSeqNum = "";
                        item.ItemNumber = so_item.ITEMNMBR;
                        item.UnitOfMeasure = "EA";
                        item.UnitQuantity = Convert.ToInt32(Convert.ToDecimal(so_item.Quantity));
                        item.UnitPrice = Convert.ToDecimal(so_item.UNITPRCE);
                        item.Comment = so_item.COMMENT_1;
                        item.ShipDate = Convert.ToDateTime("1900-01-01");
                        item.Category = "";
                        item.ProductName = so_item.ITEMDESC;


                        String[] sku = so_item.ITEMNMBR.Split('-');
                        item.Fabric = sku[0];
                        item.Colour = sku[1];
                        item.Size = sku[2];

                        item.SiteId = Convert.ToInt32(salesOrder.SOPTransactionType.TaSopHdrIvcInsert.LOCNCODE);
                        item.DiscountCode = "";
                        item.TaxExempt = "N";
                        item.UPC = API_Calls.GetUPC(item.Fabric, item.Colour, item.Size);

                        order.LineItems.Add(item);
                    }
                }



                order.Header.Discount = OrderDiscount;

                order.Addresses = new List<OrderAddress>();
                OrderAddress address = new OrderAddress();
                /* Shipping Address */

                address.AddressId = salesOrder.SOPTransactionType.TaCreateCustomerAddress.ADRSCODE;
                address.Add1 = salesOrder.SOPTransactionType.TaCreateCustomerAddress.ADDRESS1;
                address.Add2 = salesOrder.SOPTransactionType.TaCreateCustomerAddress.ADDRESS2;
                address.Add3 = "";
                address.AddCity = salesOrder.SOPTransactionType.TaCreateCustomerAddress.CITY;
                address.State = salesOrder.SOPTransactionType.TaCreateCustomerAddress.STATE;
                address.AddPostalCode = salesOrder.SOPTransactionType.TaCreateCustomerAddress.ZIPCODE;
                address.Country = salesOrder.SOPTransactionType.TaCreateCustomerAddress.COUNTRY;
                address.Phone = salesOrder.SOPTransactionType.TaCreateCustomerAddress.PHNUMBR1;
                address.Email = "";
                address.AddressType = shipToAddressType;
                address.CustomerName = salesOrder.SOPTransactionType.TaCreateCustomerAddress.ShipToName;
                order.Addresses.Add(address);

                address = new OrderAddress();
                /* Billing Address */

                address.AddressId = salesOrder.SOPTransactionType.TaCreateCustomerAddress.ADRSCODE;
                address.Add1 = salesOrder.SOPTransactionType.TaCreateCustomerAddress.ADDRESS1;
                address.Add2 = salesOrder.SOPTransactionType.TaCreateCustomerAddress.ADDRESS2;
                address.Add3 = "";
                address.AddCity = salesOrder.SOPTransactionType.TaCreateCustomerAddress.CITY;
                address.State = salesOrder.SOPTransactionType.TaCreateCustomerAddress.STATE;
                address.AddPostalCode = salesOrder.SOPTransactionType.TaCreateCustomerAddress.ZIPCODE;
                address.Country = salesOrder.SOPTransactionType.TaCreateCustomerAddress.COUNTRY;
                address.Phone = salesOrder.SOPTransactionType.TaCreateCustomerAddress.PHNUMBR1;
                address.Email = salesOrder.SOPTransactionType.TaCreateInternetAddresses.INET1;
                address.AddressType = billToAddressType;
                address.CustomerName = salesOrder.SOPTransactionType.TaCreateCustomerAddress.ShipToName;

                order.Addresses.Add(address);

                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End mapping the EP sales order transaction");
                return order;
            }
            catch (Exception ex)
            {
                /*if (ex.Source == "BC_API_Calls")
                {
                    Exception api_exc = new Exception();
                    api_exc.Source = "BC_API_Calls";
                    throw new Exception(ex.Message.ToString(), api_exc);
                }*/

                Trace.WriteLine(tracingExceptionPrefix + " An error occured while trying to map the EP batch message to the SalesChannelOrder message in MapSalesOrdersToCanonical()");
                throw new Exception("An error occured while trying to map the EP sales order batch message to the SalesChannelOrder message" +
                    "in the BC.Integration.AppService.EpOnRampServiceBC.Process.MapSalesOrdersToCanonical method. The EP sales order transaction " +
                    "number is: " + salesOrder.SOPTransactionType.TaSopHdrIvcInsert.SOPNUMBE, ex);
            }
        }

        /// <summary>
        /// Maps new sales order data from JMS to Canonical message from this dll 'BC.Integration.Canonical.SalesChannelOrder'
        /// Order = CanonicalSalesChannelOrder,  NewOrder.Order = JMS Object
        /// </summary>
        /// <param name="salesOrder"></param>
        /// <returns></returns>
        private Order MapSalesOrdersToCanonical(NewOrder.Order salesOrder)
        {
            try
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " Start mapping the BC sales order transaction");
                Order order = new Order();
                order.Header = new OrderHeader();



                //Populate the message header
                order.Process = processName;
                order.Header.OrderNumber = salesOrder.Header.OrderNumber;
                order.Header.SiteId = Convert.ToInt32(Mapper.MapStoreNameToSiteId(salesOrder.Header.StoreCode));
                order.Header.CurrencyId = salesOrder.Header.Currency;
                order.Header.TaxRegistrationNumber = "";
                order.Header.CarrierCode = "";
                order.Header.PaymentType = "";
                order.Header.CustomerId = salesOrder.Customer.CustomerId;
                order.Header.CustomerPONum = salesOrder.Customer.CustomerId;
                order.Header.CustEmail = salesOrder.Customer.Email;
                order.Header.isStaffOrder = false; /* To do*/


                order.Header.OrderDate = Convert.ToDateTime(salesOrder.Header.CreatedDate);
                order.Header.QuoteExpirationDate = DateTime.Now;
                order.Header.CancelDate = Convert.ToDateTime("1900-01-01");
                order.Header.ReqShipDate = Convert.ToDateTime("1900-01-01");
                order.Header.PaymentMethod = "";
                order.Header.Comment = "";
                order.Header.IncoTerms = "";
                order.Header.PriceCode = "A";
                order.Header.Freight = 0;
                order.Header.TaxAmount = Convert.ToDecimal(salesOrder.Header.TotalTaxes);
                order.Header.Discount = Convert.ToDecimal(salesOrder.Header.TotalDiscountAmount);


                order.LineItems = new List<OrderLineItem>();

                //Loop through the line items in the transaction and create the associated items in the canonical msg
                //NOTE: Sales transactions are split by line item

               
                foreach (NewOrder.LineItem so_item in salesOrder.Shipments.Shipment.LineItems.LineItem)
                {

                        OrderLineItem item = new OrderLineItem();

                        item.LineSeqNum = "";
                        item.ItemNumber = so_item.ItemCode;
                        item.isGiftCard = so_item.ItemCode.Contains("HSC-GC") ? true : false;
                        item.UnitOfMeasure = "EA";
                        item.UnitQuantity = Convert.ToInt32(Convert.ToDecimal(so_item.Quantity));
                        item.UnitPrice = Convert.ToDecimal(so_item.UnitPrice);
                        item.Comment = "";
                        item.ShipDate = Convert.ToDateTime("1900-01-01");
                        item.Category = "";
                        item.ProductName = item.isGiftCard ? "GIFTCERTIFICATE" : so_item.DisplayName;

                        /*Verify if there are any discount applied to this item*/
                        foreach(NewOrder.AppliedPromotions.Promotion  )


                        String[] sku = so_item.ItemCode.Split('-');
                        item.Fabric = sku[0];
                        item.Colour = sku[1];
                        item.Size = sku[2];

                        item.SiteId = 20;
                        item.DiscountCode = "";
                        item.TaxExempt = "N";
                        item.UPC = API_Calls.GetUPC(item.Fabric, item.Colour, item.Size);

                        order.LineItems.Add(item);
                    
                }



               

                order.Addresses = new List<OrderAddress>();
                foreach (NewOrder.Address so_item in salesOrder.Addresses.Address)
                {
                    OrderAddress address = new OrderAddress();

                    address.AddressId = so_item.AddressId;
                    address.Add1 = so_item.Street1;
                    address.Add2 = so_item.Street2;
                    address.Add3 = "";
                    address.AddCity = so_item.City;
                    address.State = so_item.Region;
                    address.AddPostalCode =so_item.ZipPostalCode;
                    address.Country = so_item.Country;
                    address.Phone = so_item.PhoneNumber;
                    address.Email = "";
                    address.AddressType = so_item.AddressType=="BILLING" ? billToAddressType : shipToAddressType;
                    address.CustomerName = so_item.FirstName + " " + so_item.LastName;
                    order.Addresses.Add(address);

                }

                Trace.WriteLineIf(tracingEnabled, tracingPrefix + " End mapping the EP sales order transaction");
                return order;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + " An error occured while trying to map the EP batch message to the SalesChannelOrder message in MapSalesOrdersToCanonical()");
                throw new Exception("An error occured while trying to map the EP sales order batch message to the SalesChannelOrder message" +
                    "in the BC.Integration.AppService.EpOnRampServiceBC.Process.MapSalesOrdersToCanonical method. The EP sales order transaction " +
                    "number is: " + salesOrder.Header.OrderNumber, ex);
            }
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
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Get file messages.  Filename: " + file);
                fileText = File.ReadAllText(file);
                //data += fileText.Substring(fileText.IndexOf("{"), fileText.LastIndexOf("}") - fileText.IndexOf("{") + 1) + ",";
                //Remove file after processing

                //File.Delete(file);
            }
            /*data = data.TrimEnd(',') + "]";
            if (data == "[]")
                data = "";*/

            return fileText;
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