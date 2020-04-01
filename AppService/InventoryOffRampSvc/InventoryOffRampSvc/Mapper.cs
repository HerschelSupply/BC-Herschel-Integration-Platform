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
        

        public static WarehouseSettings GetWarehouseCodes(string message)
        {
            string BCSiteId = "";
            var data = JArray.Parse(message);
            BCSiteId = data.FirstOrDefault().Value<string>("location");


            string[] vals = warehouseSettings.Split(',');
            foreach (string val in vals)
            {
                string[] codes = val.Split('|');
                if (BCSiteId.Trim() == codes[0])
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


