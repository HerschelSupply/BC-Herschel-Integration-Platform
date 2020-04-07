using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.IO;
using System.Xml;
using System.Xml.Serialization;



// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BC.Integration;
//
//    var data = CanonicalShippingConfirmation.FromJson(jsonString);

namespace BC.Integration.Canonical.NRI
{

    public partial class CanonicalShippingConfirmation
    {
        [JsonProperty("pickNum")]
        public string PickNum { get; set; }

        [JsonProperty("orderNbr")]
        public string OrderNbr { get; set; }

        [JsonProperty("nriOrderNbr")]
        public string NriOrderNbr { get; set; }

        [JsonProperty("orderStatus")]
        public string OrderStatus { get; set; }

        [JsonProperty("customer")]
        public string Customer { get; set; }

        [JsonProperty("orderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("shippingDate")]
        public DateTime ShippingDate { get; set; }

        [JsonProperty("bol")]
        public string Bol { get; set; }

        [JsonProperty("shipmentType")]
        public string ShipmentType { get; set; }

        [JsonProperty("carrierCode")]
        public string CarrierCode { get; set; }

        [JsonProperty("totalQtyShipped")]
        public int TotalQtyShipped { get; set; }

        [JsonProperty("totalCartons")]
        public int TotalCartons { get; set; }

        [JsonProperty("freight")]
        public decimal Freight { get; set; }

        [JsonProperty("totalWeight")]
        public decimal TotalWeight { get; set; }

        [JsonProperty("fulfillType")]
        public string FulfillType { get; set; }

        [JsonProperty("cancellationReason")]
        public string CancellationReason { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("lineItems")]
        public List<LineItem> LineItems { get; set; }

        [JsonProperty("cartons")]
        public List<Carton> Cartons { get; set; }

        /// <summary>
        /// This method serializes an object model to an xml string.
        /// </summary>
        /// <param name="doc">The invoice document object to be serialized</param>
        /// <returns>XML string representation of the invoice document</returns>
        /*public string ConvertShippingConfirmationToString(CanonicalShippingConfirmation doc)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, "http://BC.Integration.Schema.ShippingConfirmation.NRI");


                XmlSerializer serializer = new XmlSerializer(doc.GetType(), "http://BC.Integration.Schema.ShippingConfirmation.NRI");
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, doc, ns);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured serializing the Corp.Integration.Invoice Document object model.  The error occured in the ConvertInvoiceToString method.", ex);
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }*/
    }

    public partial class LineItem
    {
        [JsonProperty("lineNbr")]
        public string LineNbr { get; set; }

        [JsonProperty("itemNbr")]
        public string ItemNbr { get; set; }
        
        [JsonProperty("itemPrice")]
        public decimal ItemPrice { get; set; }

        [JsonProperty("qtyOrdered")]
        public int QtyOrdered { get; set; }

        [JsonProperty("qtyShipped")]
        public int QtyShipped { get; set; }

        [JsonProperty("cancellationReason")]
        public string CancellationReason { get; set; }


    }

    public partial class Carton
    {
        [JsonProperty("cartonNbr")]
        public string CartonNbr { get; set; }

        [JsonProperty("cartonType")]
        public string CartonType { get; set; }

        [JsonProperty("cartonWeight")]
        public decimal CartonWeight { get; set; }

        [JsonProperty("trackingNumber")]
        public string TrackingNumber { get; set; }

        [JsonProperty("cartonItems")]
        public List<CartonItem> CartonItems { get; set; }


    }

    public partial class CartonItem
    {
        [JsonProperty("cartonNbr")]
        public string CartonNbr { get; set; }

        [JsonProperty("itemNumber")]
        public string ItemNumber { get; set; }

        [JsonProperty("itemUom")]
        public string ItemUom { get; set; }

        [JsonProperty("qtyShipped")]
        public int QtyShipped { get; set; }


    }

    

    public partial class CanonicalShippingConfirmation
    {
        public static CanonicalShippingConfirmation FromJson(string json) => JsonConvert.DeserializeObject<CanonicalShippingConfirmation>(json, Converter.Settings);
    }

    public static class SerializeCanonicalShippingConfirmation
    {
        public static string ToJson(this CanonicalShippingConfirmation self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}

