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
        public string Open { get; set; }

        [JsonProperty("picked")]
        public string Picked { get; set; }

        [JsonProperty("invoiced")]
        public string Invoiced { get; set; }

        [JsonProperty("unshipped")]
        public string Unshipped { get; set; }

        [JsonProperty("qoh")]
        public string Qoh { get; set; }

        [JsonProperty("available_qoh")]
        public string Available_qoh { get; set; }

        [JsonProperty("ots_inventory")]
        public string Ots_inventory { get; set; }


    }
}