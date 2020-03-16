using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Net;
using System.IO;
using System.Xml;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using BC.Integration.Interfaces;
using Microsoft.Practices.Unity;
using BC.Integration.Utility;
using Microsoft.Practices.Unity.Configuration;

namespace BC.Integration.APICalls
{
    public class API_Calls// : IDisposable
    {
        #region Properties

        //  bool disposed = false;

        //constants
        private const string DESTINATION = "BlueCherry";
        private const string NAMESPACE = "BC.Integration.AppService.BC_API_Calls";

        private List<KeyValuePair<string, string>> configuration = null;
        MessageManager msgMgr = new MessageManager();
        private bool tracingEnabled = false;
    
        private Configuration localConfig;

        //Dependency Injection Components
        IConfiguration config;
        IInstrumentation instrumentation;
        UnityContainer container;


        //Key service Properties that should never change from design time
        private string processName;
        private string messageType;
        private string serviceId;
        private Decimal serviceVersion;
        private string serviceOperationId;
        private string servicePostOperationId;
        private string centralConfigConnString;

        //Local processing properties
        public List<KeyValuePair<string, string>> Configuration { get => configuration; set => configuration = value; }
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";

        //Authentication
        private string authKey;
        private string authValue;

        //UPC 
        private string UPC_endpoint;
        private string UPC_param_style;
        private string UPC_param_color;
        private string UPC_param_size;

        //Customer 
        private string Customer_endpoint;
        private string Customer_param_location;

        //Order
        private string ORDER_Iendpoint;
        private string RETURN_Iendpoint;
        private string ORDER_endpoint;
        private string ORDER_param_po_num;
        private string ORDER_DetailsOEndpoint;


        //OrderShipment 945
        private string ORDERSHIPMENT_Iendpoint;

        //allocation
        private bool US_East_Enabled;
        private bool US_West_Enabled;
        private bool CA_East_Enabled;
        private bool CA_West_Enabled;

        //Shipping 
        private string SHIPPING_endpoint;
        private string SHIPPING_param_name;


        //carrier code
        private static string ShipmentCarrierToCarrierCodeMapping;
        private static Dictionary<string, string> shipmentCarrierToCarrierCode;

        //Inventory
        private static string CUTSOLDBYLOC_endpoint;
        private static string CUTSOLDBYLOC_param_loc;



        public API_Calls()
        {
            try
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = "C:\\BC Herschel Integration Platform\\Utility\\BC_API_Calls\\BC_API_Calls\\web.config" };
                localConfig = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                CreateDiComponents();

                string m = localConfig.AppSettings.Settings["ProcessName"].Value;
                processName = localConfig.AppSettings.Settings["ProcessName"].Value; //Set to the value of the incoming message. 
                messageType = localConfig.AppSettings.Settings["ServiceId"].Value; //Set to the value of the incoming message.
                                                                                   //Key service Properties that should never change from design time
                serviceId = localConfig.AppSettings.Settings["ServiceId"].Value;
                serviceVersion = Convert.ToDecimal(localConfig.AppSettings.Settings["ServiceVersion"].Value);
                serviceOperationId = localConfig.AppSettings.Settings["ServiceOperationId"].Value;
                servicePostOperationId = localConfig.AppSettings.Settings["ServicePostOperationId"].Value;
                centralConfigConnString = localConfig.AppSettings.Settings["CentralConfigConnString"].Value;

                //Local processing properties
                tracingPrefix = localConfig.AppSettings.Settings["TracingPrefix"].Value + ": ";
                tracingExceptionPrefix = localConfig.AppSettings.Settings["TracingPrefix"].Value + ".EXCEPTION : ";

                //Auth
                authKey = localConfig.AppSettings.Settings["authKey"].Value;
                authValue = localConfig.AppSettings.Settings["authValue"].Value;


                //UPC 

                UPC_endpoint = localConfig.AppSettings.Settings["UPC_endpoint"].Value;
                UPC_param_style = localConfig.AppSettings.Settings["UPC_param_style"].Value;
                UPC_param_color = localConfig.AppSettings.Settings["UPC_param_color"].Value;
                UPC_param_size = localConfig.AppSettings.Settings["UPC_param_size"].Value;

                //Customer 
                Customer_endpoint = localConfig.AppSettings.Settings["Customer_endpoint"].Value;
                Customer_param_location = localConfig.AppSettings.Settings["Customer_param_location"].Value;

                //Order
                ORDER_Iendpoint = localConfig.AppSettings.Settings["ORDER_Iendpoint"].Value.ToString();
                RETURN_Iendpoint = localConfig.AppSettings.Settings["RETURN_Iendpoint"].Value.ToString();
                ORDER_endpoint = localConfig.AppSettings.Settings["ORDER_endpoint"].Value.ToString();
                ORDER_param_po_num = localConfig.AppSettings.Settings["ORDER_param_po_num"].Value.ToString();
                ORDER_DetailsOEndpoint = localConfig.AppSettings.Settings["ORDER_DetailsOEndpoint"].Value.ToString();


                //OrderShipment 945
                ORDERSHIPMENT_Iendpoint = localConfig.AppSettings.Settings["ORDERSHIPMENT_Iendpoint"].Value.ToString();

                //allocation
                US_East_Enabled = Convert.ToBoolean(Convert.ToInt16(localConfig.AppSettings.Settings["US_East_Enabled"].Value));
                US_West_Enabled = Convert.ToBoolean(Convert.ToInt16(localConfig.AppSettings.Settings["US_West_Enabled"].Value));
                CA_East_Enabled = Convert.ToBoolean(Convert.ToInt16(localConfig.AppSettings.Settings["CA_East_Enabled"].Value));
                CA_West_Enabled = Convert.ToBoolean(Convert.ToInt16(localConfig.AppSettings.Settings["CA_West_Enabled"].Value));

                //Shipping
                SHIPPING_endpoint = localConfig.AppSettings.Settings["SHIPPING_endpoint"].Value;
                SHIPPING_param_name = localConfig.AppSettings.Settings["SHIPPING_param_name"].Value;

                //carrier code
                ShipmentCarrierToCarrierCodeMapping = localConfig.AppSettings.Settings["ShipmentCarrierToCarrierCodeMapping"].Value;

                //Inventory
                CUTSOLDBYLOC_endpoint = localConfig.AppSettings.Settings["CUTSOLDBYLOC_endpoint"].Value;
                CUTSOLDBYLOC_param_loc = localConfig.AppSettings.Settings["CUTSOLDBYLOC_param_loc"].Value;
            }
            catch (Exception ex)
            {
               instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "BC API",
                   "An exception was raised when instantiating BC_APICalls class.  " + ex.Message, "");
            }



        }

    private void CreateDiComponents()
        {
            //Create Unity container for DI and create DI components
            container = new UnityContainer();
            container.LoadConfiguration();
            config = container.Resolve<IConfiguration>();
            configuration = config.PopulateConfigurationCollectionFromAppConfig();
            PopulateLocalVaribale();

            instrumentation = container.Resolve<IInstrumentation>();
            OverrideConfigProperties(config);
            instrumentation.Configuration = configuration;
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Unity container config complete...");
        }

        //BC API Data


        #region Central Configuration methods
        /// <summary>
        /// Retrives the service and global central config values from the store.  The service properties are applied to the 
        /// service first, so global properties will override all properties.
        /// </summary>
        /// <param name="config">The configuration class to implement the retrieval of the central configuration</param>
        private void OverrideConfigProperties(IConfiguration config)
        {
            //ProcessName not know at this point so will need to be reloaded once message is recieved.
            configuration = config.GetConfiguration(serviceId, processName);
            PopulateLocalVaribale();
            instrumentation.Configuration = configuration;
        }

        /// <summary>
        /// Applies the central configurations to the loacal properties.
        /// </summary>
        /// <param name="configCol">The key value pair collection containing the configuration values.</param>
        private void PopulateLocalVaribale()
        {
            string destinationSystemConnectivity = Utilities.GetConfigurationValue(configuration, "DestinationSystemConnectivity");
            string outgoingMessageType = Utilities.GetConfigurationValue(configuration, "OutgoingMessageType");
            string val = Utilities.GetConfigurationValue(configuration, "TracingEnabled");
            if (val != "")
            {
                if (val.ToLower() == "true" || val.ToLower() == "false")
                    tracingEnabled = Convert.ToBoolean(val);
                if (val == "1" || val == "0")
                    tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
            }

        }
        #endregion


        #endregion

        #region IDisposable implementation for future
        /*
                public void Dispose()
                {
                    Dispose(true);
                    GC.SuppressFinalize(this);
                }
                protected virtual void Dispose(bool disposing)
                {
                    if (disposed)
                        return;

                    if (disposing)
                    {
                        // Free any other managed objects here.
                        //
                    }

                    // Free any unmanaged objects here.
                    //
                    disposed = true;
                }

                ~API_Calls()
                {
                    Dispose(false);
                }
                */

        #endregion
     
        #region Outbound Endpoints

        /// <summary>
        /// UPC outbound endpoint
        /// </summary>
        public string GetUPC(string style, string color, string size)
        {
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".GetUPC start retrieving UPC.");

            string upc = "";
            try
            {
                string uri = UPC_endpoint + "?" + UPC_param_style + "=" + style + "&" + UPC_param_color + "=" + color + "&" + UPC_param_size + "=" + size;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add(authKey, authValue);
                /*Servers sometimes compress their responses to save on bandwidth, when this happens, you need to decompress the response before attempting to read it.
                 Fortunately, the .NET framework can do this automatically, however, we have to turn the setting on. */
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {

                    var jObj = JObject.Parse(reader.ReadToEnd());
                    // validates if there are any errors in the "Error Array". HTTP Response of 200-OK
                    JArray errors = (JArray)jObj.SelectToken("Errors");

                    if (errors.HasValues)
                    {
                        throw new BlueCherryException("The UPC  was not found. Style: " + style + " Color: " + color + " Size: " + size + ". BlueCherry BC.Integration.Utility.BC_API_Calls.GetUPC");
                    }

                    JArray data = (JArray)jObj.SelectToken("data");
                    upc = data[0].SelectToken("upc").ToString();

                    //return reader.ReadToEnd();
                }

            }
            catch (BlueCherryException ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to get the UPC value from BlueCherry");
              
                throw new Exception("An exception occured trying to get the UPC value from BlueCherry BC.Integration.Utility.BC_API_Calls.GetUPC. ", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured trying to get the UPC value from BlueCherry BC.Integration.Utility.BC_API_Calls.GetUPC. ", ex);
            }
            finally
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".GetUPC completed retrieving UPC.");

                Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and GetUPC method complete.");
            }

            return upc;
        }

        /// <summary>
        /// Order outbound endpoint. Duplicates validation (Open orders & Pick tickets).
        /// </summary>
        public bool OrderExists(string po_num)
        {

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".OrderExists started checking if order exists.");

            bool exists = false;
            try
            {
                string uri = ORDER_endpoint + "?" + ORDER_param_po_num + "=" + po_num;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add(authKey, authValue);
                /*Servers sometimes compress their responses to save on bandwidth, when this happens, you need to decompress the response before attempting to read it.
                 Fortunately, the .NET framework can do this automatically, however, we have to turn the setting on. */
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {

                    var jObj = JObject.Parse(reader.ReadToEnd());
                    // validates if there are any errors in the "Error Array".
                    JArray errors = (JArray)jObj.SelectToken("Errors");
                    string errorDesc = "";

                    if (errors.HasValues)
                    {
                        foreach (var item in errors)
                        {
                            if (item.SelectToken("ErrorMessage").ToString() == "No Data Found")
                            {
                                exists = false;
                            }
                            else
                            {
                                errorDesc += item.SelectToken("ErrorMessage").ToString() + Environment.NewLine;
                                throw new BlueCherryException(errorDesc);
                            }
                        }
                        
                    }

                    else if (jObj.SelectToken("data") != null)
                    {
                        //JArray data = (JArray)jObj.SelectToken("data");

                        //if (data.HasValues)
                        //{
                            exists = true;
                        //}
                        
                    }
                }
            }
            catch (BlueCherryException ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to validate if an order exists in BlueCherry");
                //  instrumentation.LogGeneralException("An exception occured trying to validate if an order exists in BlueCherry BC.Integration.Utility.BC_API_Calls.OrderExists. ", ex);
                throw new Exception("An exception occured trying to validate if an order exists in BlueCherry BC.Integration.Utility.BC_API_Calls.OrderExists. ", ex);

            }
            finally
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".OrderExists completed checking if order exists.");

                instrumentation.FlushActivity();
                Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and OrderExists method complete.");
            }


            return exists;
        }

        /// <summary>
        /// Shipping outbound endpoint
        /// </summary>
        public string GetShipper(string ship_name)
        {
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".GetUPC start retrieving UPC.");

            string shipper = "";
            try
            {
                string uri = SHIPPING_endpoint + "?" + SHIPPING_param_name + "=" + ship_name ;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add(authKey, authValue);
                /*Servers sometimes compress their responses to save on bandwidth, when this happens, you need to decompress the response before attempting to read it.
                 Fortunately, the .NET framework can do this automatically, however, we have to turn the setting on. */
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {

                    var jObj = JObject.Parse(reader.ReadToEnd());
                    // validates if there are any errors in the "Error Array". HTTP Response of 200-OK
                    JArray errors = (JArray)jObj.SelectToken("Errors");

                    if (errors.HasValues)
                    {
                        string errorDesc="";
                        //return "";
                        foreach (JToken error in errors)
                        {
                            errorDesc += error.Value<string>("ErrorMessage") + " ";
                        }
                        throw new BlueCherryException("ErrorMessage:" + errorDesc + " Ship_name: " + ship_name + ". BlueCherry BC.Integration.Utility.BC_API_Calls.GetShipper");
                    }

                    JArray data = (JArray)jObj.SelectToken("data");
                    shipper = data[0].SelectToken("shipper").ToString();


                    //return reader.ReadToEnd();
                }

            }
            catch (BlueCherryException ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to get the Shipper value from BlueCherry");
                //instrumentation.LogGeneralException("An exception occured trying to get the Shipper value from BlueCherry BC.Integration.Utility.BC_API_Calls.GetShipper. ", ex);
                throw new Exception("An exception occured trying to get the UPC value from BlueCherry BC.Integration.Utility.BC_API_Calls.GetShipper. ", ex);
            }
            finally
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".GetShipper completed retrieving Shipper code.");

               // instrumentation.FlushActivity();
                Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and GetShipper method complete.");
            }

            return shipper;
        }

        /// <summary>
        /// Cut Sold by location outbound endpoint
        /// </summary>
        public string GetCutSoldByLocation(string siteId)
        {
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".GetCutSoldByLocation start retrieving UPC.");

            string data = null;
            try
            {
                string uri = CUTSOLDBYLOC_endpoint+"?"+ CUTSOLDBYLOC_param_loc+"="+siteId;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add(authKey, authValue);
                /*Servers sometimes compress their responses to save on bandwidth, when this happens, you need to decompress the response before attempting to read it.
                 Fortunately, the .NET framework can do this automatically, however, we have to turn the setting on. */
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {

                    var jObj = JObject.Parse(reader.ReadToEnd());
                    // validates if there are any errors in the "Error Array". HTTP Response of 200-OK
                    JArray errors = (JArray)jObj.SelectToken("Errors");

                    if (errors.HasValues)
                    {
                        string errorDesc = "";
                        foreach (JToken error in errors)
                        {
                            errorDesc += error.Value<string>("ErrorMessage") + " ";
                        }
                        throw new BlueCherryException("ErrorMessage: " + errorDesc + " BlueCherry BC.Integration.Utility.BC_API_Calls.GetCutSoldByLocation");
                    }

                    data = jObj.SelectToken("data").ToString();


                }

            }
            catch (BlueCherryException ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to get the inventory by location value from BlueCherry.");
                //instrumentation.LogGeneralException("An exception occured trying to get the inventory by location from BlueCherry BC.Integration.Utility.BC_API_Calls.GetCutSoldByLocation. ", ex);
                throw new Exception("An exception occured trying to get the inventory by location from BlueCherry BC.Integration.Utility.BC_API_Calls.GetCutSoldByLocation. ", ex);
            }
            finally
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".GetCutSoldByLocation completed retrieving Shipper code.");

                //instrumentation.FlushActivity();
                Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and GetCutSoldByLocation method complete.");
            }

            return data;
        }

        public string GetCustomerFromSite(string site)
        {

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".GetCustomerFromSite start retrieving customer from site.");

            string customer = "";

            try
            {
                string uri = Customer_endpoint + "?" + Customer_param_location + "=" + site;
                // string uri = "https://bcmultiws.azure-api.net/HCEL/HCTR/api/Customer?location=" + site;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add("Ocp-Apim-Subscription-Key", "151def3e976c4798897af920c656390c");
                /*Servers sometimes compress their responses to save on bandwidth, when this happens, you need to decompress the response before attempting to read it.
                 Fortunately, the .NET framework can do this automatically, however, we have to turn the setting on. */
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {

                    var jObj = JObject.Parse(reader.ReadToEnd());
                    // validates if there are any errors in the "Error Array". HTTP Response of 200-OK
                    JArray errors = (JArray)jObj.SelectToken("Errors");

                    if (errors.HasValues)
                    {
                        string errorDesc = "";
                        foreach (JToken error in errors)
                        {
                            errorDesc += error.Value<string>("ErrorMessage") + " ";
                        }
                        throw new BlueCherryException("ErrorMessage:"+ errorDesc+" Location: " + site + " . BlueCherry BC.Integration.Utility.BC_API_Calls.GetCustomerFromSite");
                    }

                    JArray data = (JArray)jObj.SelectToken("data");
                    customer = data[0].SelectToken("customer").ToString();


                    //return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to get the UPC value from BlueCherry");
                //instrumentation.LogGeneralException("An exception occured trying to get the UPC value from BlueCherry BC.Integration.Utility.BC_API_Calls.GetCustomerFromSite. ", ex);
                throw new Exception("An exception occured trying to get the Customer value from BlueCherry BC.Integration.Utility.BC_API_Calls.GetCustomerFromSite. ", ex);
            }
            finally
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".GetCustomerFromSite completed retrieving customer from site.");

                //instrumentation.FlushActivity();
                Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and GetCustomerFromSite method complete.");
            }


            return customer;
        }

        public string GetInvoiceNumberFromOrder(string poNumber)
        {
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".GetInvoiceNumberFromOrder start retrieving invoice number from PO.");
            string invoiceNumber;

            try
            {
                string uri = ORDER_DetailsOEndpoint + "?" + ORDER_param_po_num + "=" + poNumber;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
                request.Headers.Add("Ocp-Apim-Subscription-Key", "151def3e976c4798897af920c656390c");
                /*Servers sometimes compress their responses to save on bandwidth, when this happens, you need to decompress the response before attempting to read it.
                 Fortunately, the .NET framework can do this automatically, however, we have to turn the setting on. */
                request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {

                    var jObj = JObject.Parse(reader.ReadToEnd());
                    // validates if there are any errors in the "Error Array". HTTP Response of 200-OK
                    JArray errors = (JArray)jObj.SelectToken("Errors");

                    if (errors.HasValues)
                    {
                        string errorDesc = "";
                        foreach (JToken error in errors)
                        {
                            errorDesc += error.Value<string>("ErrorMessage") + " ";
                        }
                        throw new BlueCherryException("ErrorMessage:"+ errorDesc +" Order Number: " + poNumber + " . BlueCherry BC.Integration.Utility.BC_API_Calls.GetInvoiceNumberFromOrder");
                    }

                    JArray data = (JArray)jObj.SelectToken("data");
                    invoiceNumber = data[0].SelectToken("inv_num").ToString();

                    
                    //return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to get the invoice number from BlueCherry");
               // instrumentation.LogGeneralException("An exception occured trying to get the Invoice number from BlueCherry BC.Integration.Utility.BC_API_Calls.GetInvoiceNumberFromOrder. ", ex);
                throw new Exception("An exception occured trying to get the invoice number from BlueCherry BC.Integration.Utility.BC_API_Calls.GetInvoiceNumberFromOrder. ", ex);
            }
            finally
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".GetInvoiceNumberFromOrder cmpleted retrieving invoice number from PO.");
                //instrumentation.FlushActivity();
                Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and GetInvoiceNumberFromOrder method complete.");
            }

            return invoiceNumber;
        }

        #endregion


        #region Business Logic
        private string GetSecondaryLocation(string primary)
        {
            string secondary = primary;
            switch (primary)
            {
                case "11":
                    secondary = "12";
                    break;
                case "12":
                    secondary = "11";
                    break;
                case "21":
                    secondary = "22";
                    break;
                case "22":
                    secondary = "21";
                    break;
                default:
                    secondary = primary;
                    break;
            }
            return secondary;
        }

 
        /// <summary>
        /// Shipping outbound endpoint
        /// </summary>
        /// 
        public string ConvertShipmenMethodForEast(string site, string shipmentMethod)
        {
              string newShipMethod =  shipmentMethod;
              if(site == "12" || site == "22")
              {
                    try
                    {
                        if (shipmentCarrierToCarrierCode == null)
                        {
                            shipmentCarrierToCarrierCode = new Dictionary<string, string>();
                            string[] codes = ShipmentCarrierToCarrierCodeMapping.Split(',');
                            foreach (string code in codes)
                            {
                                string[] vals = code.Split('|');
                                shipmentCarrierToCarrierCode.Add(vals[0], vals[1]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("While trying to convert shipment carrier: " + shipmentMethod + " to a Herschel carrier code an error occurred.  Please" +
                            " verify the EpOnRampSvcBC  has a configuration for this EP store name.", ex);
                    }

                    string value;
                    if (shipmentCarrierToCarrierCode.TryGetValue(shipmentMethod, out value))
                    {
                       newShipMethod = value;
                    }
              }
            else
              {
                  newShipMethod = shipmentMethod;
              }
              
            return newShipMethod;
        }
        private Dictionary<string,string> InitializeWarehousePairs()
        {
            Dictionary<string, string> warehousePairs = new Dictionary<string, string>();
            warehousePairs.Add("AB", "11");
            warehousePairs.Add("BC", "11");
            warehousePairs.Add("MB", "11");
            warehousePairs.Add("SK", "11");
            warehousePairs.Add("YT", "11");
            warehousePairs.Add("NU", "11");
            warehousePairs.Add("NT", "11");
            warehousePairs.Add("PE", "12");
            warehousePairs.Add("NS", "12");
            warehousePairs.Add("NB", "12");
            warehousePairs.Add("NL", "12");
            warehousePairs.Add("ON", "12");
            warehousePairs.Add("QC", "12");
            warehousePairs.Add("AR", "21");
            warehousePairs.Add("AZ", "21");
            warehousePairs.Add("CA", "21");
            warehousePairs.Add("CO", "21");
            warehousePairs.Add("AK", "21");
            warehousePairs.Add("KS", "21");
            warehousePairs.Add("GU", "21");
            warehousePairs.Add("HI", "21");
            warehousePairs.Add("IA", "21");
            warehousePairs.Add("ID", "21");
            warehousePairs.Add("LA", "21");
            warehousePairs.Add("MN", "21");
            warehousePairs.Add("MO", "21");
            warehousePairs.Add("MS", "21");
            warehousePairs.Add("MT", "21");
            warehousePairs.Add("ND", "21");
            warehousePairs.Add("NE", "21");
            warehousePairs.Add("NM", "21");
            warehousePairs.Add("NV", "21");
            warehousePairs.Add("OK", "21");
            warehousePairs.Add("OR", "21");
            warehousePairs.Add("PR", "21");
            warehousePairs.Add("SD", "21");
            warehousePairs.Add("TX", "21");
            warehousePairs.Add("WA", "21");
            warehousePairs.Add("WY", "21");
            warehousePairs.Add("WI", "22");
            warehousePairs.Add("WV", "22");
            warehousePairs.Add("VA", "22");
            warehousePairs.Add("VT", "22");
            warehousePairs.Add("TN", "22");
            warehousePairs.Add("RI", "22");
            warehousePairs.Add("SC", "22");
            warehousePairs.Add("PA", "22");
            warehousePairs.Add("NY", "22");
            warehousePairs.Add("OH", "22");
            warehousePairs.Add("NH", "22");
            warehousePairs.Add("NJ", "22");
            warehousePairs.Add("NC", "22");
            warehousePairs.Add("MA", "22");
            warehousePairs.Add("MD", "22");
            warehousePairs.Add("ME", "22");
            warehousePairs.Add("MI", "22");
            warehousePairs.Add("IL", "22");
            warehousePairs.Add("IN", "22");
            warehousePairs.Add("KY", "22");
            warehousePairs.Add("AL", "22");
            warehousePairs.Add("CT", "22");
            warehousePairs.Add("DC", "22");
            warehousePairs.Add("DE", "22");
            warehousePairs.Add("FL", "22");
            warehousePairs.Add("GA", "22");

            return warehousePairs;
        }

        ///
        /// 
        /// 
        public string AllocateBasedOnState(string state, string country)
        {

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".AllocateBasedOnState start allocating site.");

            string allocateTo = state;
            Dictionary<string, string> warehousePairs;

            try
            {
                warehousePairs = InitializeWarehousePairs();
                if (country == "US")
                {
                    if (US_East_Enabled && US_West_Enabled)
                    {
                        allocateTo = warehousePairs[state];
                    }
                    else if (US_East_Enabled && !US_West_Enabled)
                    {
                        allocateTo = "22";
                    }
                    else
                    {
                        allocateTo = "21";
                    }
                }
                else
                {
                    if (CA_East_Enabled && CA_West_Enabled)
                    {
                        allocateTo = warehousePairs[state];
                    }
                    else if (CA_East_Enabled && !CA_West_Enabled)
                    {
                        allocateTo = "12";
                    }
                    else
                    {
                        allocateTo = "11";
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to post allocate site based on state and country");
                throw new Exception("An exception occured trying to retrieve correct site allocation. BC.Integration.Utility.BC_API_Calls.AllocateBasedOnState. The combination of the state:" + state + " and country:" + country + "was not found.", ex);

            }
            finally
            {
                Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and AllocateBasedOnState method complete.");
            }


            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".AllocateBasedOnState completed allocating site.");

            return allocateTo;
        }

        #endregion  


        #region Inbound Endpoints
        public string PostOrder(string value)
        {
            XmlDocument originalDoc = new XmlDocument();
            originalDoc.LoadXml(value);
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".PostOrder start posting order.");

            var responseString ="";
            /* Turns orderDetail to an Array, even when there's only one line item.*/
            value = value.Replace("<orderDetail>", "<orderDetail xmlns:json=\"http://james.newtonking.com/projects/json\" json:Array=\"true\">");

            Uri url = new Uri(ORDER_Iendpoint);
            XmlDocument doc = new XmlDocument();
       

            doc.LoadXml(value);
            string po_num = doc.DocumentElement.SelectSingleNode("//po_num").InnerText;


            if (OrderExists(po_num))
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to post an order into BlueCherry");
                throw new Exception("An exception occured trying to post an order into BlueCherry BC.Integration.Utility.BC_API_Calls. The PO_Number:" + po_num + " already exists in BC.");
            }
            else
            {

                string json = JsonConvert.SerializeXmlNode(doc);

                json = json.Replace("http://_x002E_Schemas.DestinationSchema.BC_InboundOrder", "");
                json = json.Replace("\"@xmlns:ns0\":\"\",", "");
                json = json.Replace("{\"ns0:Root\":", "[");
                json = json.Replace("}}", "}]");


                var request = HttpWebRequest.Create(url);
                request.Headers.Add(authKey, authValue);
                var byteData = Encoding.ASCII.GetBytes(json);
                request.ContentType = "application/json";
                request.Method = "POST";

                try
                {
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(byteData, 0, byteData.Length);
                    }
                    var response = (HttpWebResponse)request.GetResponse();
                    responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                    var jObj = JObject.Parse(responseString);
                    JArray messages = (JArray)jObj.SelectToken("Message");
                    // validates if there are any errors 
                    JArray errors = (JArray)jObj.SelectToken("Errors");
                    string errorDesc = "";

                    if (errors.HasValues)
                    {
                        foreach (var item in errors)
                        {
                            errorDesc += item.SelectToken("ErrorMessage").ToString() + Environment.NewLine;
                        }
                        throw new BlueCherryException(errorDesc + " PO Number: " + po_num);
                    }
                    else if (messages.HasValues)
                    {
                        string a = messages[0].SelectToken("message").ToString();
                            if (messages[0].SelectToken("message").ToString().Contains("could not be processed due to errors"))
                            {
                                throw new BlueCherryException(" PO Number: " + po_num + "could not be processed due to errors");
                            }
                        
                    }
                    
                }
                catch (BlueCherryException ex)
                {
                   
                    Trace.WriteLine("BC_API_Calls: Exception occured trying to post an order into BlueCherry");
                    throw new Exception("An exception occured trying to post an order into BlueCherry BC.Integration.Utility.BC_API_Calls.Post.", ex);

                }
                finally
                {
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".PostOrder completed posting order.");
                    Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and PostOrder method complete.");
                }
            }

            return responseString;
        }

        /// <summary>
        /// Shipment Confirmation 945 Inbound Endpoint - NOT TESTED YET!!
        /// </summary>

        public string PostShipmentConfirmation(string value)
        {

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".PostShipmentConfirmation start posting shipment confirmation.");

            XmlDocument originalDoc = new XmlDocument();
            originalDoc.LoadXml(value);

            String responseString = "";
            //Turns orderDetail to an Array, even when there's only one line item.
            value = value.Replace("<pickDetail>", "<pickDetail xmlns:json=\"http://james.newtonking.com/projects/json\" json:Array=\"true\">");
            value = value.Replace("<cartonHeader>", "<cartonHeader xmlns:json=\"http://james.newtonking.com/projects/json\" json:Array=\"true\">");
            value = value.Replace("<cartonDetail>", "<cartonDetail xmlns:json=\"http://james.newtonking.com/projects/json\" json:Array=\"true\">");

            Uri url = new Uri(ORDERSHIPMENT_Iendpoint);
            XmlDocument doc = new XmlDocument();


            doc.LoadXml(value);
            string pick_num = doc.DocumentElement.SelectSingleNode("//pick_num").InnerText;
           
            string json = JsonConvert.SerializeXmlNode(doc);

            json = json.Replace("http://Schemas.DestinationSchema.BC_ShipmentConfirmation", "");
            json = json.Replace("\"@xmlns:ns0\":\"\",", "");
            json = json.Replace("{\"ns0:Root\":", "[");
            json = json.Replace("}}", "}]");
            
            var request = HttpWebRequest.Create(url);
            request.Headers.Add(authKey, authValue);
            var byteData = Encoding.ASCII.GetBytes(json);
            request.ContentType = "application/json";
            request.Method = "POST";

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                var jObj = JObject.Parse(responseString);

                // validates if there are any errors 
                JArray errors = (JArray)jObj.SelectToken("Errors");
                string errorDesc = "";
                
                if (errors.HasValues)
                {
                    foreach (var item in errors)
                    {
                        errorDesc += item.SelectToken("ErrorMessage").ToString() + Environment.NewLine;
                    }
                    throw new BlueCherryException(errorDesc + " Pick Number: " + pick_num);
                }
            }
            catch (BlueCherryException ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to post a shipment confirmation into BlueCherry");
               throw new Exception("An exception occured trying to post an order into BlueCherry BC.Integration.Utility.BC_API_Calls.PostShipmentConfirmation.", ex);

            }
            finally
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".PostShipmentConfirmation completed posting shipment confirmation.");
                Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and PostShipmentConfirmation method complete.");
            }
            
            return responseString;

        }
        public string PostReturn(string value)
        {
            XmlDocument originalDoc = new XmlDocument();
            originalDoc.LoadXml(value);

            Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE +".PostResturn start posting return.");

            String responseString = "";
            /* Turns orderDetail to an Array, even when there's only one line item.*/
            value = value.Replace("<ReturnsDetail>", "");
            value = value.Replace("</ReturnsDetail>", "");
            value = value.Replace("<returnsDetail>", "<returnsDetail xmlns:json=\"http://james.newtonking.com/projects/json\" json:Array=\"true\">");
            value = value.Replace("<returnsHeaderAddress>", "<returnsHeaderAddress xmlns:json=\"http://james.newtonking.com/projects/json\" json:Array=\"true\">");


            Uri url = new Uri(RETURN_Iendpoint);
            XmlDocument doc = new XmlDocument();
            
            doc.LoadXml(value);

            String json = JsonConvert.SerializeXmlNode(doc);

            json = json.Replace("http://Schemas.DestinationSchema.BC_Return", "");
            json = json.Replace("\"@xmlns:ns0\":\"\",", "");
            json = json.Replace("{\"ns0:Root\":", "[");
            json = json.Replace("}}", "}]");

            var request = HttpWebRequest.Create(RETURN_Iendpoint);
            request.Headers.Add("Ocp-Apim-Subscription-Key", "151def3e976c4798897af920c656390c");
            var byteData = Encoding.ASCII.GetBytes(json);
            request.ContentType = "application/json";
            request.Method = "POST";

            try
            {
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                }
                var response = (HttpWebResponse)request.GetResponse();
                responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

                var jObj = JObject.Parse(responseString);

                // validates if there are any errors 
                JArray errors = (JArray)jObj.SelectToken("Errors");
                string errorDesc = "";
                
                if (errors.HasValues)
                {
                    foreach (var item in errors)
                    {
                        errorDesc += item.SelectToken("ErrorMessage").ToString() + Environment.NewLine;
                    }
                    throw new BlueCherryException(errorDesc);
                }
                
            }
            catch (BlueCherryException ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to post an order into BlueCherry");
                throw new Exception("An exception occured trying to post an order into BlueCherry BC.Integration.Utility.BC_API_Calls.PostReturn.", ex);

            }
            finally
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + NAMESPACE + ".PostResturn completed posting return.");
                Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and PostReturn method complete.");
            }
            
            return responseString;
           
        }
        #endregion
        /// <summary>
        /// Customer ID outbound endpoint
        /// </summary>



    }
}