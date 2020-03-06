using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;

namespace BC.Integration.AppService.InventoryOffRampSvc
{
    public static class Mapper
    {

        private static string warehouseSettings = ConfigurationManager.AppSettings["WarehouseSettings"];
        /// <summary>
        /// Converts Canonical shipping confiramtion into a EP shipping confirmation message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>EP shipping confirmation</returns>
        public static string ConvertToEPMessage(string message)
        {
            string epMessage = "itemnmbr,qtyonhand\r\n";

            JArray data = JArray.Parse(message);

            foreach (var item in data)
            {
                epMessage += item.Value<string>("product_id") + "," + item.Value<string>("qoh") + "\r\n";
            }


            return epMessage;
        }

        public static WarehouseSettings GetWarehouseCodes(string message)
        {
            string BCSiteId = "";
            var data = JArray.Parse(message);
            BCSiteId = data.FirstOrDefault().Value<string>("location");


            string[] vals = warehouseSettings.Split(',');
            foreach (string val in vals)
            {
                string[] codes = val.Split('|');
                if (BCSiteId == codes[0])
                {
                    WarehouseSettings whSettings = new WarehouseSettings();
                    
                    whSettings.warehouseCode = codes[1];
                    whSettings.warehouseUid = codes[2];

                    return whSettings;

                }
            }

            throw new Exception("Error occurred in the BC.Integration.AppService.InventoryOffRampSvc.Mapper.GetWarehouseCodes method, when trying to get the WarehouseUid and WarehouseCode. BCSiteId: "+ BCSiteId);

        }


        
       
    }

    public class WarehouseSettings
    {
        public string warehouseUid { get; set; }
        public string warehouseCode { get; set; }
    }

}


