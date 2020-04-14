using System;
using System.Collections.Generic;
using System.Configuration;


namespace BC.Integration.AppService.EpReturnOnRampServiceBC
{
    public static class Mapper
    {
        private static string StoreNameToSiteMapping = ConfigurationManager.AppSettings["StoreNameToSiteMapping"];
        private static string ShipmentCarrierToCarrierCodeMapping = ConfigurationManager.AppSettings["ShipmentCarrierToCarrierCodeMapping"];

        private static Dictionary<string, string> storeNameToSiteId;
        private static Dictionary<string, string> shipmentCarrierToCarrierCode;



        /// <summary>
        /// This code is used to convert an EP store name to a Site ID.  The mapping lives in the services .config file.
        /// </summary>
        /// <param name="storeName">Shop ID from the message.</param>
        /// <returns></returns>
       /* public static string MapStoreNameToSiteId(string storeName)
        {
            try
            {
                if (storeNameToSiteId == null)
                {
                    //populate soterNameToSiteId
                    storeNameToSiteId = new Dictionary<string, string>();
                    string[] sites = StoreNameToSiteMapping.Split(',');
                    foreach (string site in sites)
                    {
                        string[] vals = site.Split('|');
                        storeNameToSiteId.Add(vals[0], vals[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("While trying to convert store: " + storeName + " to a Herschel site ID an error occurred.  Please" +
                    " verify the EpReturnOnRampSvcBC  has a configuration for this EP store name.", ex);
            }

            return storeNameToSiteId[storeName];
        }*/


        /// <summary>
        /// This code is used to convert an EP shipment carrier to a carrier code.  The mapping lives in the services .config file.
        /// </summary>
        /// <param name="shipmentCarrier">ShipmentCarrier from the message.</param>
        /// <returns></returns>
        /*public static string MapShipmentCarrierToCarrierCode(string shipmentCarrier)
        {
            try
            {
                if (shipmentCarrierToCarrierCode == null)
                {
                    //populate soterNameToSiteId
                    shipmentCarrierToCarrierCode = new Dictionary<string, string>();
                    string[] codes = ShipmentCarrierToCarrierCodeMapping.Split(',');
                    foreach (string code in codes)
                    {
                        string[] vals = code.Split('|');
                        shipmentCarrierToCarrierCode.Add(vals[0], vals[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("While trying to convert shipment carrier: " + shipmentCarrier + " to a Herschel carrier code an error occurred.  Please" +
                    " verify the EpReturnOnRampSvcBC  has a configuration for this EP store name.", ex);
            }

            string value;
            if (shipmentCarrierToCarrierCode.TryGetValue(shipmentCarrier, out value))
            {
                // Key was in dictionary; "value" contains corresponding value
            }
            else
            {
                value = shipmentCarrier;
            }

            return value;
        }*/

    }
}