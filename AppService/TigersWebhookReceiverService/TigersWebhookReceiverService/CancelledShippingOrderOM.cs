// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Corp.Integration.AppService.NriShippingOnRampService;
//
//    var data = CancelledShippingOrder.FromJson(jsonString);

namespace Corp.Integration.AppService.TigersWebhookReceiverService
{
    using System;
    using System.Net;
    using System.Collections.Generic;

    using Newtonsoft.Json;
    using System.Runtime.Serialization;

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

  
    [DataContract]
    public partial class CancelledShippingOrder
    {
        [DataMember]
        public object tradingPartnerId { get; set; }
        [DataMember]
        public string senderId { get; set; }
        [DataMember]
        public string locationNumber { get; set; }
        [DataMember]
        public List<Cancellation> cancellations { get; set; }
    }

    public partial class CancelledShippingOrder
    {
        public static CancelledShippingOrder FromJson(string json) => JsonConvert.DeserializeObject<CancelledShippingOrder>(json, Converter.Settings);
    }

    public static class SerializeCancelledShippingOrder
    {
        public static string ToJson(this CancelledShippingOrder self) => JsonConvert.SerializeObject(self, Converter.Settings);
        public static string OrderToJson(this CancelledShippingOrder self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    public class ConverterCancelledShippingOrder
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }
}
