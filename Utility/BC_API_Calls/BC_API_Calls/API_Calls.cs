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

namespace BC.Integration.APICalls
{
    public class API_Calls
    {
        #region Properties
        // instrumentation and logException *notification  
        private static IInstrumentation instrumentation;

        //BC API Data
        private static string authKey = ConfigurationManager.AppSettings["authKey"];
        private static string authValue = ConfigurationManager.AppSettings["authValue"];

        //UPC 
        private static string UPC_endpoint = ConfigurationManager.AppSettings["UPC_endpoint"];
        private static string UPC_param_style = ConfigurationManager.AppSettings["UPC_param_style"];
        private static string UPC_param_color = ConfigurationManager.AppSettings["UPC_param_color"];
        private static string UPC_param_size = ConfigurationManager.AppSettings["UPC_param_size"];

        //Order
        private static string ORDER_Iendpoint = ConfigurationManager.AppSettings["ORDER_Iendpoint"].ToString();
        private static string ORDER_endpoint = ConfigurationManager.AppSettings["ORDER_endpoint"].ToString();
        private static string ORDER_param_po_num = ConfigurationManager.AppSettings["ORDER_param_po_num"].ToString();

        //OrderShipment 945
        private static string ORDERSHIPMENT_Iendpoint = ConfigurationManager.AppSettings["ORDERSHIPMENT_Iendpoint"].ToString();

        #endregion

        #region Outbound Endpoints

        /// <summary>
        /// UPC outbound endpoint
        /// </summary>
        public static string GetUPC(string style, string color, string size)
        {
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
                        //return "";
                        throw new Exception("The UPC  was not found. Style: "+style+ " Color: " + color + " Size: " + size + ". BlueCherry BC.Integration.Utility.BC_API_Calls.GetUPC");
                    }

                    JArray data = (JArray)jObj.SelectToken("data");
                    string upc = data[0].SelectToken("upc").ToString();

                    return upc;
                    //return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to get the UPC value from BlueCherry");
                instrumentation.LogGeneralException("An exception occured trying to get the UPC value from BlueCherry BC.Integration.Utility.BC_API_Calls.GetUPC. ", ex);
                throw new Exception("An exception occured trying to get the UPC value from BlueCherry BC.Integration.Utility.BC_API_Calls.GetUPC. ", ex);
            }
        } 

        /// <summary>
        /// Order outbound endpoint. Duplicates validation (Open orders & Pick tickets).
        /// </summary>
        public static bool OrderExists(string po_num)
        {
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
                                return false;
                            }
                            else
                            {
                                errorDesc += item.SelectToken("ErrorMessage").ToString() + Environment.NewLine;
                            }
                        }
                        throw new Exception(errorDesc);
                    }

                    JArray data = (JArray)jObj.SelectToken("data");

                    if (data.HasValues)
                    {
                        return true;
                    }
                    return false;
                }
            }
            catch (WebException ex)
            {
                Trace.WriteLine("BC_API_Calls: Exception occured trying to validate if an order exists in BlueCherry");
                instrumentation.LogGeneralException("An exception occured trying to validate if an order exists in BlueCherry BC.Integration.Utility.BC_API_Calls.OrderExists. ", ex);
                throw new Exception("An exception occured trying to validate if an order exists in BlueCherry BC.Integration.Utility.BC_API_Calls.OrderExists. ", ex);

            }
        }

        #endregion

        #region Inbound Endpoints

        /// <summary>
        /// Order Inbound Endpoint
        /// </summary>
        public static string PostOrder(string value)
        {
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
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

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
                        throw new Exception(errorDesc + " PO Number: " + po_num);
                    }

                    return responseString;

                }
                catch (WebException ex)
                {
                    Trace.WriteLine("BC_API_Calls: Exception occured trying to post an order into BlueCherry");
                    instrumentation.LogGeneralException("An exception occured trying to post an order into BlueCherry BC.Integration.Utility.BC_API_Calls.Post.", ex);
                    throw new Exception("An exception occured trying to post an order into BlueCherry BC.Integration.Utility.BC_API_Calls.Post.", ex);

                }
            }
        }

        /// <summary>
        /// Shipment Confirmation 945 Inbound Endpoint - NOT TESTED YET!!
        /// </summary>

        public static string PostShipmentConfirmation(string value)
        {
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
                    var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();

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
                        throw new Exception(errorDesc + " Pick Number: "+ pick_num);
                    }

                    return responseString;

                }
                catch (WebException ex)
                {
                    Trace.WriteLine("BC_API_Calls: Exception occured trying to post a shipment confirmation into BlueCherry");
                instrumentation.LogGeneralException("An exception occured trying to post an order into BlueCherry BC.Integration.Utility.BC_API_Calls.PostShipmentConfirmation.", ex);
                throw new Exception("An exception occured trying to post an order into BlueCherry BC.Integration.Utility.BC_API_Calls.PostShipmentConfirmation.", ex);

                }
           
        }
        
        #endregion
    }
}