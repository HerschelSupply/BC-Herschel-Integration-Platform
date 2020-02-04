// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BC.Integration.AppService.NriShippingOnRampService;
//
//    var data = CancelledShippingOrder.FromJson(jsonString);

namespace BC.Integration.AppService.Nri.CancelledShippingOrder
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;

    public partial class CancelledShippingOrder
    {
        [JsonProperty("OrderID")]
        public string OrderId { get; set; }

        [JsonProperty("DocumentNumber")]
        public string DocumentNumber { get; set; }

        [JsonProperty("DocumentDate")]
        public string DocumentDate { get; set; }

        [JsonProperty("ClientReferenceNumber1")]
        public string ClientReferenceNumber1 { get; set; }

        [JsonProperty("ClientReferenceNumber2")]
        public string ClientReferenceNumber2 { get; set; }

        [JsonProperty("Status")]
        public string Status { get; set; }

        [JsonProperty("CompletionDate")]
        public string CompletionDate { get; set; }

        [JsonProperty("ClientDocumentType")]
        public string ClientDocumentType { get; set; }

        [JsonProperty("PurchaseOrderNumber")]
        public string PurchaseOrderNumber { get; set; }

        [JsonProperty("FirstShipDate")]
        public string FirstShipDate { get; set; }

        [JsonProperty("LastShipDate")]
        public string LastShipDate { get; set; }

        [JsonProperty("MarkFor")]
        public string MarkFor { get; set; }

        [JsonProperty("PackedDate")]
        public string PackedDate { get; set; }

        [JsonProperty("ShipToCustomerCode")]
        public string ShipToCustomerCode { get; set; }

        [JsonProperty("ShipToName")]
        public string ShipToName { get; set; }

        [JsonProperty("ShipToAddressLine1")]
        public string ShipToAddressLine1 { get; set; }

        [JsonProperty("ShipToAddressLine2")]
        public string ShipToAddressLine2 { get; set; }

        [JsonProperty("ShipToAddressLine3")]
        public string ShipToAddressLine3 { get; set; }

        [JsonProperty("ShipToCity")]
        public string ShipToCity { get; set; }

        [JsonProperty("ShipToProvinceCode")]
        public string ShipToProvinceCode { get; set; }

        [JsonProperty("ShipToCountryCode")]
        public string ShipToCountryCode { get; set; }

        [JsonProperty("ShipToPostalCode")]
        public string ShipToPostalCode { get; set; }

        [JsonProperty("BillToCustomerCode")]
        public string BillToCustomerCode { get; set; }

        [JsonProperty("BillToName")]
        public string BillToName { get; set; }

        [JsonProperty("BillToAddressLine1")]
        public string BillToAddressLine1 { get; set; }

        [JsonProperty("BillToAddressLine2")]
        public string BillToAddressLine2 { get; set; }

        [JsonProperty("BillToAddressLine3")]
        public string BillToAddressLine3 { get; set; }

        [JsonProperty("BillToCity")]
        public string BillToCity { get; set; }

        [JsonProperty("BillToProvinceCode")]
        public string BillToProvinceCode { get; set; }

        [JsonProperty("BillToCountryCode")]
        public string BillToCountryCode { get; set; }

        [JsonProperty("BillToPostalCode")]
        public string BillToPostalCode { get; set; }

        [JsonProperty("OrderLines")]
        public List<OrderLine> OrderLines { get; set; }

        [JsonProperty("Data")]
        public Data Data { get; set; }

        [JsonProperty("CancellationReason")]
        public string CancellationReason { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("WarehouseCode")]
        public string WarehouseCode { get; set; }

        [JsonProperty("FulfillType")]
        public string FulfillType { get; set; }
    }

    public partial class OrderLine
    {
        [JsonProperty("LineNumber")]
        public string LineNumber { get; set; }

        [JsonProperty("ClientLineNumber")]
        public string ClientLineNumber { get; set; }

        [JsonProperty("ItemNumber")]
        public string ItemNumber { get; set; }

        [JsonProperty("ClientItemNumber")]
        public string ClientItemNumber { get; set; }

        [JsonProperty("ItemDescription")]
        public string ItemDescription { get; set; }

        [JsonProperty("UomCode")]
        public string UomCode { get; set; }

        [JsonProperty("UomDescription")]
        public string UomDescription { get; set; }

        [JsonProperty("WarehouseName")]
        public string WarehouseName { get; set; }

        [JsonProperty("GrossPrice")]
        public string GrossPrice { get; set; }

        [JsonProperty("DiscountAmount")]
        public string DiscountAmount { get; set; }

        [JsonProperty("NetPrice")]
        public string NetPrice { get; set; }

        [JsonProperty("DependencyCode")]
        public string DependencyCode { get; set; }

        [JsonProperty("Gtin")]
        public string Gtin { get; set; }

        [JsonProperty("CustomerGtin")]
        public string CustomerGtin { get; set; }

        [JsonProperty("QuantityOrdered")]
        public int QuantityOrdered { get; set; }

        [JsonProperty("QuantityAllocated")]
        public string QuantityAllocated { get; set; }

        [JsonProperty("QuantityPicked")]
        public string QuantityPicked { get; set; }

        [JsonProperty("QuantityShipped")]
        public string QuantityShipped { get; set; }

        [JsonProperty("CancellationReason")]
        public string CancellationReason { get; set; }
    }

    public partial class CancelledShippingOrder
    {
        public static List<CancelledShippingOrder> FromJson(string json) => JsonConvert.DeserializeObject<List<CancelledShippingOrder>>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<CancelledShippingOrder> self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string OrderToJson(this CancelledShippingOrder self) => JsonConvert.SerializeObject(self, Converter.Settings);
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
