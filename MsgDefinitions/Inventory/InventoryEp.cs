using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace BC.Integration.Canonical.Inventory.Ep
{
    public class Inventory
    {
        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("division")]
        public string Division { get; set; }

        [JsonProperty("upc")]
        public string Upc { get; set; }

        [JsonProperty("product_id")]
        public string Product_id { get; set; }

        [JsonProperty("open")]
        public int Open { get; set; }

        [JsonProperty("picked")]
        public int Picked { get; set; }

        [JsonProperty("invoiced")]
        public int Invoiced { get; set; }

        [JsonProperty("unshipped")]
        public int Unshipped { get; set; }

        [JsonProperty("qoh")]
        public int Qoh { get; set; }

        [JsonProperty("available_qoh")]
        public int Available_qoh { get; set; }

        [JsonProperty("ots_inventory")]
        public int Ots_inventory { get; set; }


    }

    public partial class CanonicalInventory
    {
        public static CanonicalInventory FromJson(string json) => JsonConvert.DeserializeObject<CanonicalInventory>(json, Converter.Settings);
    }
    public static class SerializeCanonicalInventory
    {
        public static string ToJson(this Inventory self) => JsonConvert.SerializeObject(self, Converter.Settings);
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