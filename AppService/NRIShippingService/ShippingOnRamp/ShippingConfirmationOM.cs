// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using BC.Integration.AppService.NriShippingOnRampService;
//
//    var data = ShippingConfirmation.FromJson(jsonString);

namespace BC.Integration.AppService.Nri.ShippingConfirmation
{
    using System;
    using System.Net;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    public partial class ShippingConfirmation
    {
        [JsonProperty("PickNumber")]
        public string PickNumber { get; set; }

        [JsonProperty("OrderID")]
        public string OrderId { get; set; }

        [JsonProperty("ShipmentNumber")]
        public string ShipmentNumber { get; set; }

        [JsonProperty("ShipmentDocumentDate")]
        public string ShipmentDocumentDate { get; set; }

        [JsonProperty("CompletionDate")]
        public string CompletionDate { get; set; }

        [JsonProperty("PaymentTerm")]
        public string PaymentTerm { get; set; }

        [JsonProperty("FreightTerm")]
        public string FreightTerm { get; set; }

        [JsonProperty("CarrierName")]
        public string CarrierName { get; set; }

        [JsonProperty("ShipmentPin")]
        public string ShipmentPin { get; set; }

        [JsonProperty("CodShipment")]
        public string CodShipment { get; set; }

        [JsonProperty("ShipmentDate")]
        public DateTime ShipmentDate { get; set; }

        [JsonProperty("ClientFreightCharge")]
        public string ClientFreightCharge { get; set; }

        [JsonProperty("ClientInsuranceCharge")]
        public string ClientInsuranceCharge { get; set; }

        [JsonProperty("ClientTotalFreightCharge")]
        public string ClientTotalFreightCharge { get; set; }

        [JsonProperty("CustomerFreightCharge")]
        public decimal CustomerFreightCharge { get; set; }

        [JsonProperty("ShipmentValue")]
        public string ShipmentValue { get; set; }

        [JsonProperty("CodValue")]
        public string CodValue { get; set; }

        [JsonProperty("ShipToCustomerCode")]
        public string ShipToCustomerCode { get; set; }

        [JsonProperty("ShipToName")]
        public string ShipToName { get; set; }

        [JsonProperty("ShipToAddressLine1")]
        public string ShipToAddressLine1 { get; set; }

        [JsonProperty("ShipToAddressLine2")]
        public string ShipToAddressLine2 { get; set; }

        [JsonProperty("ShipToCity")]
        public string ShipToCity { get; set; }

        [JsonProperty("ShipToProvinceCode")]
        public string ShipToProvinceCode { get; set; }

        [JsonProperty("ShipToPostalCode")]
        public string ShipToPostalCode { get; set; }

        [JsonProperty("ShipToCountryCode")]
        public string ShipToCountryCode { get; set; }

        [JsonProperty("ShipToPhoneNumber")]
        public string ShipToPhoneNumber { get; set; }

        [JsonProperty("BillToCustomerCode")]
        public string BillToCustomerCode { get; set; }

        [JsonProperty("BillToName")]
        public string BillToName { get; set; }

        [JsonProperty("BillToAddressLine1")]
        public string BillToAddressLine1 { get; set; }

        [JsonProperty("BillToAddressLine2")]
        public string BillToAddressLine2 { get; set; }

        [JsonProperty("BillToCity")]
        public string BillToCity { get; set; }

        [JsonProperty("BillToProvinceCode")]
        public string BillToProvinceCode { get; set; }

        [JsonProperty("BillToPostalCode")]
        public string BillToPostalCode { get; set; }

        [JsonProperty("BillToCountryCode")]
        public string BillToCountryCode { get; set; }

        [JsonProperty("ExpressService")]
        public string ExpressService { get; set; }

        [JsonProperty("CarrierSCAC")]
        public string CarrierScac { get; set; }

        [JsonProperty("RetailerType")]
        public string RetailerType { get; set; }

        [JsonProperty("ClientCarrierCode")]
        public string ClientCarrierCode { get; set; }

        [JsonProperty("BolNumber")]
        public string BolNumber { get; set; }

        [JsonProperty("OrderNumber")]
        public int OrderNumber { get; set; }

        [JsonProperty("OrderDate")]
        public DateTime OrderDate { get; set; }

        [JsonProperty("ClientReferenceNumber1")]
        public string ClientReferenceNumber1 { get; set; }

        [JsonProperty("ClientReferenceNumber2")]
        public string ClientReferenceNumber2 { get; set; }

        [JsonProperty("CustomerDepartment")]
        public string CustomerDepartment { get; set; }

        [JsonProperty("ClientDocumentType")]
        public string ClientDocumentType { get; set; }

        [JsonProperty("PurchaseOrderNumber")]
        public string PurchaseOrderNumber { get; set; }

        [JsonProperty("PurchaseOrderDate")]
        public string PurchaseOrderDate { get; set; }

        [JsonProperty("OrderValue")]
        public string OrderValue { get; set; }

        [JsonProperty("NetAmount")]
        public string NetAmount { get; set; }

        [JsonProperty("TaxAmount")]
        public string TaxAmount { get; set; }

        [JsonProperty("InvoiceAmount")]
        public string InvoiceAmount { get; set; }

        [JsonProperty("LeadOrderOnShipment")]
        public string LeadOrderOnShipment { get; set; }

        [JsonProperty("Imported")]
        public string Imported { get; set; }

        [JsonProperty("TotalQuantityOrdered")]
        public int TotalQuantityOrdered { get; set; }

        [JsonProperty("TotalQuantityShipped")]
        public int TotalQuantityShipped { get; set; }

        [JsonProperty("WarehouseName")]
        public string WarehouseName { get; set; }

        [JsonProperty("ClientWarehouseCode")]
        public string ClientWarehouseCode { get; set; }

        [JsonProperty("CarrierServiceCode")]
        public string CarrierServiceCode { get; set; }

        [JsonProperty("CarrierServiceName")]
        public string CarrierServiceName { get; set; }

        [JsonProperty("MarkFor")]
        public string MarkFor { get; set; }

        [JsonProperty("ShippingInstructions")]
        public string ShippingInstructions { get; set; }

        [JsonProperty("HandlingInstructions")]
        public string HandlingInstructions { get; set; }

        [JsonProperty("PackedDate")]
        public string PackedDate { get; set; }

        [JsonProperty("AtsNumber")]
        public string AtsNumber { get; set; }

        [JsonProperty("DataSource")]
        public string DataSource { get; set; }

        [JsonProperty("ShipmentCartons")]
        public List<ShipmentCarton> ShipmentCartons { get; set; }

        [JsonProperty("OrderConfirmationLines")]
        public List<OrderConfirmationLine> OrderConfirmationLines { get; set; }

        [JsonProperty("Data")]
        public Data Data { get; set; }

        [JsonProperty("CancellationReason")]
        public string CancellationReason { get; set; }

        /// <summary>
        /// Returns a sun of the shipment Carton weights
        /// </summary>
        /// <returns>Total shipment weight</returns>
        public decimal TotalWeight()
        {
            decimal totalWeight = 0;
            foreach(ShipmentCarton carton in ShipmentCartons)
            {
                totalWeight += carton.Weight;
            }
            return totalWeight;
        }
    }

    public partial class OrderConfirmationLine
    {
        [JsonProperty("LineNumber")]
        public string LineNumber { get; set; }

        [JsonProperty("ItemNumber")]
        public string ItemNumber { get; set; }

        [JsonProperty("ClientItemNumber")]
        public string ClientItemNumber { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("UomCode")]
        public string UomCode { get; set; }

        [JsonProperty("UomDescription")]
        public string UomDescription { get; set; }

        [JsonProperty("WarehouseName")]
        public string WarehouseName { get; set; }

        [JsonProperty("QuantityOrdered")]
        public int QuantityOrdered { get; set; }

        [JsonProperty("QuantityShipped")]
        public int QuantityShipped { get; set; }

        [JsonProperty("CustomerItemNumber")]
        public string CustomerItemNumber { get; set; }

        [JsonProperty("ClientLineNumber")]
        public string ClientLineNumber { get; set; }

        [JsonProperty("GrossPrice")]
        public string GrossPrice { get; set; }

        [JsonProperty("DiscountAmount")]
        public string DiscountAmount { get; set; }

        [JsonProperty("DependencyCode")]
        public string DependencyCode { get; set; }

        [JsonProperty("NetPrice")]
        public string NetPrice { get; set; }

        [JsonProperty("ExtendedPriceQuantityOrdered")]
        public string ExtendedPriceQuantityOrdered { get; set; }

        [JsonProperty("ExtendedPriceQuantityShipped")]
        public string ExtendedPriceQuantityShipped { get; set; }

        [JsonProperty("WholesaleValue")]
        public string WholesaleValue { get; set; }

        [JsonProperty("ExtendedValue")]
        public string ExtendedValue { get; set; }

        [JsonProperty("ServiceCharge")]
        public string ServiceCharge { get; set; }

        [JsonProperty("Gtin")]
        public string Gtin { get; set; }

        [JsonProperty("CancellationReason")]
        public string CancellationReason { get; set; }
    }

    public partial class ShipmentCarton
    {
        [JsonProperty("CartonNumber")]
        public string CartonNumber { get; set; }

        [JsonProperty("LicensePlate")]
        public string LicensePlate { get; set; }

        [JsonProperty("PinNumber")]
        public string PinNumber { get; set; }

        [JsonProperty("Sscc")]
        public string Sscc { get; set; }

        [JsonProperty("CrossDock")]
        public string CrossDock { get; set; }

        [JsonProperty("Length")]
        public string Length { get; set; }

        [JsonProperty("Width")]
        public string Width { get; set; }

        [JsonProperty("Height")]
        public string Height { get; set; }

        [JsonProperty("Weight")]
        public decimal Weight { get; set; }

        [JsonProperty("TrackingUrl")]
        public string TrackingUrl { get; set; }

        [JsonProperty("ShipmentCartonItems")]
        public List<ShipmentCartonItem> ShipmentCartonItems { get; set; }
    }

    public partial class ShipmentCartonItem
    {
        [JsonProperty("ItemNumber")]
        public string ItemNumber { get; set; }

        [JsonProperty("ClientItemNumber")]
        public string ClientItemNumber { get; set; }

        [JsonProperty("Description")]
        public string Description { get; set; }

        [JsonProperty("UomDescription")]
        public string UomDescription { get; set; }

        [JsonProperty("Gtin")]
        public string Gtin { get; set; }

        [JsonProperty("Quantity")]
        public string Quantity { get; set; }
    }

    public partial class Data
    {
        [JsonProperty("FulfillType")]
        public string FulfillType { get; set; }

        [JsonProperty("WarehouseCode")]
        public string ClientWarehouseCode { get; set; }

    }


    public partial class ShippingConfirmation
    {
        public static List<ShippingConfirmation> FromJson(string json) => JsonConvert.DeserializeObject<List<ShippingConfirmation>>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<ShippingConfirmation> self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string OrderToJson(this ShippingConfirmation self) => JsonConvert.SerializeObject(self, Converter.Settings);
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
