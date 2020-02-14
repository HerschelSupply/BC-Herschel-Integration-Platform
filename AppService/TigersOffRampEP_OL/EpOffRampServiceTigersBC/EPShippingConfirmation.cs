using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;

namespace BC.Integration.AppService.EpOffRampServiceTigersBC
{
    public partial class EPShippingConfirmation
    {
        [JsonProperty("orderNumber")]
        public string OrderNumber { get; set; }

        [JsonProperty("shipmentNotificationLines")]
        public List<LineItems> ShipmentNotificationLines { get; set; }

        private DateTime FromUnixTime(double unixTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return epoch.AddMilliseconds(unixTime);
        }

        private long ToUnixTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalMilliseconds);
        }
    }

    /// <summary>
    /// Line Items for elastic path JSON 
    /// </summary>
    public partial class LineItems
    {
        [JsonProperty("orderNumber")]
        public string OrderNbr { get; set; }

        [JsonProperty("nriOrderNumber")]
        public string NriOrderNumber { get; set; }

        [JsonProperty("itemNumber")]
        public string ItemNumber { get; set; }

        [JsonProperty("quantityShipped")]
        public int QuantityShipped { get; set; }

        [JsonProperty("shippingDate")]
        public long ShippingDate { get; set; }

        [JsonProperty("bol")]
        public string Bol { get; set; }

        [JsonProperty("shipmentType")]
        public string ShipmentType { get; set; }

        [JsonProperty("carrierCode")]
        public string CarrierCode { get; set; }

        [JsonProperty("freight")]
        public decimal Freight { get; set; }

        [JsonProperty("totalWeight")]
        public string TotalWeight { get; set; }

        [JsonProperty("totalQuantityShipped")]
        public int TotalQuantityShipped { get; set; }

        [JsonProperty("totalCartons")]
        public int TotalCartons { get; set; }

        [JsonProperty("lineNumber")]
        public string LineNumber { get; set; }

        [JsonProperty("customer")]
        public string Customer { get; set; }

        [JsonProperty("orderDate")]
        public long OrderDate { get; set; }

        [JsonProperty("quantityOrdered")]
        public decimal QuantityOrdered { get; set; }

        [JsonProperty("fulfillType")]
        public string FulfillType { get; set; }

        [JsonProperty("site")]
        public string Site { get; set; }


    }

    public partial class EPShippingConfirmation
    {
        public static EPShippingConfirmation FromJson(string json) => JsonConvert.DeserializeObject<EPShippingConfirmation>(json, Converter.Settings);
    }

    public static class SerializeEpShippingConfirmation
    {
        public static string ToJson(this EPShippingConfirmation self) => JsonConvert.SerializeObject(self, Converter.Settings);
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