using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;


// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BC.Integration;
//
//    var data = CanonicalShippingConfirmation.FromJson(jsonString);

namespace BC.Integration.Canonical.Tigers
{

    public partial class CanonicalShippingConfirmation
    {
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

        [JsonProperty("site")]
        public string Site { get; set; }

        [JsonProperty("lineItems")]
        public List<LineItem> LineItems { get; set; }
    }

    public partial class LineItem
    {
        [JsonProperty("lineNbr")]
        public string LineNbr { get; set; }

        [JsonProperty("itemNbr")]
        public string ItemNbr { get; set; }

        [JsonProperty("qtyOrdered")]
        public int QtyOrdered { get; set; }

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

