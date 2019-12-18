using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Diagnostics;

namespace Corp.Integration.AppService.EpOnRampServiceBC
{
    public static class Mapper
    {
        private static string StoreNameToSiteMapping = ConfigurationManager.AppSettings["StoreNameToSiteMapping"];

        private static Dictionary<string, string> storeNameToSiteId;
       


        /// <summary>
        /// This code is used to convert an EP store name to a Site ID.  The mapping lives in the services .config file.
        /// </summary>
        /// <param name="storeName">Shop ID from the message.</param>
        /// <returns></returns>
        public static string MapStoreNameToSiteId(string storeName)
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
                    " verify the EpOnRampSvcBC  has a configuration for this EP store name.", ex);
            }

            return storeNameToSiteId[storeName];
        }
    }
}