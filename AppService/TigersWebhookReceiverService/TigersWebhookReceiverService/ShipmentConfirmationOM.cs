using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace BC.Integration.AppService.TigersWebhookReceiverService
{

    [DataContract]
    public partial class DespatchItems
    {
        [DataMember]
        public string stockCode { get; set; }
        [DataMember]
        public string altStockCode { get; set; }
        [DataMember]
        public int qtyDespatched { get; set; }
        [DataMember]
        public int qtyRequested { get; set; }
        [DataMember]
        public string serialBatchNo { get; set; }
        [DataMember]
        public string orderLineNumber { get; set; }
    }


    //[DataContract]
    //public partial class DespatchItems
    //{
    //    [DataMember]
    //    public string itemNumber { get; set; }
    //    [DataMember]
    //    public double altItemNumber { get; set; }
    //    [DataMember]
    //    public double qtyShipped { get; set; }
    //    [DataMember]
    //    public double qtyordered { get; set; }
    //    [DataMember]
    //    public double serialBatchNumber { get; set; }
    //    [DataMember]
    //    public List<Item> items { get; set; }
    //}

    [DataContract]
    public partial class Address
    {
        [DataMember]
        public int addressType { get; set; }
        [DataMember]
        public string locationNumber { get; set; }
        [DataMember]
        public string contactName { get; set; }
        [DataMember]
        public string firstName { get; set; }
        [DataMember]
        public string lastName { get; set; }
        [DataMember]
        public string companyName { get; set; }
        [DataMember]
        public string addressLine1 { get; set; }
        [DataMember]
        public string addressLine2 { get; set; }
        [DataMember]
        public string addressLine3 { get; set; }
        [DataMember]
        public string city { get; set; }
        [DataMember]
        public string state { get; set; }
        [DataMember]
        public string postcode { get; set; }
        [DataMember]
        public string countryCode { get; set; }
        [DataMember]
        public string phoneNumber { get; set; }
        [DataMember]
        public string faxNumber { get; set; }
        [DataMember]
        public string emailAddress { get; set; }
        [DataMember]
        public string addInfo1 { get; set; }
        [DataMember]
        public string addInfo2 { get; set; }
        [DataMember]
        public List<object> customValues { get; set; }
        [DataMember]
        public string locationNumberQualifier { get; set; }
        public List<object> ediReferences { get; set; }
    }

    [DataContract]
    public partial class ShipmentConfirmation
    {
        [DataMember]
        public string interfaceId { get; set; }
        [DataMember]
        public bool syncValidate { get; set; }
        [DataMember]
        public string salesOrderRef { get; set; }
        [DataMember]
        public string internalRef { get; set; }
        [DataMember]
        public string despatchRef { get; set; }
        [DataMember]
        public string tradingPartnerId { get; set; }
        [DataMember]
        public string senderId { get; set; }
        [DataMember]
        public string consigneeCode { get; set; }
        [DataMember]
        public string consigneeRef { get; set; }
        [DataMember]
        public string locationNumber { get; set; }
        [DataMember]
        public string status { get; set; }
        [DataMember]
        public string receivedDate { get; set; }
        [DataMember]
        public string receivedTime { get; set; }
        [DataMember]
        public string receivedItems { get; set; }
        [DataMember]
        public string packedDate { get; set; }
        [DataMember]
        public string despatchDate { get; set; }
        [DataMember]
        public string packedCartons { get; set; }
        [DataMember]
        public string deliveryCompany { get; set; }
        [DataMember]
        public string deliveryCompanyName { get; set; }
        [DataMember]
        public string deliveryService { get; set; }
        [DataMember]
        public string shipmentMethod { get; set; }
        [DataMember]
        public string trackingRef { get; set; }
        [DataMember]
        public decimal deliveryCharge { get; set; }
        [DataMember]
        public string scacCode { get; set; }
        [DataMember]
        public decimal shipmentWeight { get; set; }
        [DataMember]
        public double shipmentCube { get; set; }
        [DataMember]
        public double orderWeight { get; set; }
        [DataMember]
        public double orderCube { get; set; }
        [DataMember]
        public string shortReasonCode { get; set; }
        [DataMember]
        public string bookingDetails { get; set; }
        [DataMember]
        public string despatchCartons { get; set; }
        [DataMember]
        public List<DespatchItems> despatchItems { get; set; }
        //[DataMember]
         public List<object> customValues { get; set; }
        //[DataMember]
         public List<object> ediReferences { get; set; }
        [DataMember]
        public List<Address> addresses { get; set; }
        [DataMember]
        public List<Cancellation> cancellations { get; set; }
    }


    public partial class ShipmentConfirmation
    {
        public static ShipmentConfirmation FromJson(string json) => JsonConvert.DeserializeObject<ShipmentConfirmation>(json, Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<ShipmentConfirmation> self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string OrderToJson(this ShipmentConfirmation self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }

    [DataContract]
    public partial class Cancellation
    {
        [DataMember]
        public string documentType { get; set; }
        [DataMember]
        public string documentRef { get; set; }
        [DataMember]
        public string secondaryRef { get; set; }
        [DataMember]
        public string dateCancelled { get; set; }
        [DataMember]
        public string timeCancelled { get; set; }
        [DataMember]
        public string cancelReason { get; set; }
        [DataMember]
        public string cancelNotes { get; set; }
    }


    //[DataContract]
    //public partial class CancelledShippingOrder
    //{
    //    [DataMember]
    //    public object tradingPartnerId { get; set; }
    //    [DataMember]
    //    public string senderId { get; set; }
    //    [DataMember]
    //    public string locationNumber { get; set; }
    //    [DataMember]
    //    public List<Cancellation> cancellations { get; set; }
    //}

    //public partial class CancelledShippingOrder
    //{
    //    public static CancelledShippingOrder FromJson(string json) => JsonConvert.DeserializeObject<CancelledShippingOrder>(json, Converter.Settings);
    //}

    //public static class SerializeCancelledShippingOrder
    //{
    //    public static string ToJson(this CancelledShippingOrder self) => JsonConvert.SerializeObject(self, Converter.Settings);
    //    public static string OrderToJson(this CancelledShippingOrder self) => JsonConvert.SerializeObject(self, Converter.Settings);
    //}

    //public class ConverterCancelledShippingOrder
    //{
    //    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    //    {
    //        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
    //        DateParseHandling = DateParseHandling.None,
    //    };
    //}
}
