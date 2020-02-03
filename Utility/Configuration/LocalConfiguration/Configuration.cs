using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Corp.Integration.Interfaces;
using Corp.Integration;
using System.Configuration;


namespace Corp.Integration.Utility
{
    /// <summary>
    /// The configuration component will ultimately look at a central configuration store and override local configuration.  If no
    /// central configuration is found then the value in the configuration file will be used.
    /// </summary>
    class Configuration : IConfiguration
    {
        private string connString = "";
        private List<KeyValuePair<string, string>> configurationCollection = null;
        private List<KeyValuePair<string, string>> centralConfigurationCollection = null;

        //Ultimately the configuration should be centrally located in a DB store.  The connection string will be used to access the central store.
        //Initially we will just be connecting to the config file 
        public List<KeyValuePair<string, string>> LocalConfigurationCollection { get => configurationCollection; set => configurationCollection = value; }
        public List<KeyValuePair<string, string>> CentralConfigurationCollection { get => centralConfigurationCollection; set => centralConfigurationCollection = value; }


        /// <summary>
        /// This method populates a key value pair collection with the calling components configuration files AppConfig settings.
        /// This collection will be passed to components for configuration.  This approach allows new configurations to be added
        /// to support injected code without the need to modify the parent service.
        /// </summary>
        /// <returns>A key value collection of the calling service's AppSettings collection</returns>
        public List<KeyValuePair<string, string>> PopulateConfigurationCollectionFromAppConfig()
        {
            List<KeyValuePair<string, string>> config = new List<KeyValuePair<string, string>>();

            for (int i = 0; i < ConfigurationManager.AppSettings.Count; i++)
            {
                string key = ConfigurationManager.AppSettings.Keys[i];
                string val = ConfigurationManager.AppSettings[key];
                KeyValuePair<string, string> pr = new KeyValuePair<string, string>( key, val );
                config.Add(pr);
            }
            LocalConfigurationCollection = config;
            return config;
        }

        /// <summary>
        /// When a central DB is implemented the Service ID and Process Name parameters will be used to filter the central
        /// configuration data.
        /// The service will return a union of the search results using both parameter and the service ID and where the Process Name = null.
        /// NOTE: Any configuration data found in the central configuration store will override the local web.config values.
        /// </summary>
        /// <param name="serviceId">Service Id of the calling component</param>
        /// <param name="processName">Process Name</param>
        /// <returns>Returns an array collection of key value pairs containing the 
        /// configurations.</returns>
        public List<KeyValuePair<string, string>> GetCentralConfiguration(string serviceId, string processName)
        {
            //This method needs to be written to update the configuration collection that was recieved as a parameter, from the 
            //service specific values in the configuration DB and return the collection.

            return LocalConfigurationCollection;
        }

        /// <summary>
        /// Since initialy we will be just connecting to the config file the parameters will not be used.  When a central DB is used
        /// the parameters will be used to filter the results.
        /// </summary>
        /// <param name="processName">Process Name</param>
        /// <returns>Returns an array collection of key value pairs containing the configurations</returns>
        public List<KeyValuePair<string, string>> GetConfiguration(string serviceId, string processName)
        {
            //This method needs to be written to update the configuration collection that was recieved as a parameter, from the 
            //global values in the configuration DB and return the collection.

            return LocalConfigurationCollection;
        }

        /// <summary>
        /// Dynamic routing method.  The method can be used by a service to determine where to route a message based on the parameters
        /// supplied.  If the parameter does not need to be considered in determining the destination pass null.
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="serviceId"></param>
        /// <param name="servicePostOperationId"></param>
        /// <param name="messageType"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        public string GetDestinationUrl(string processName, string serviceId, string servicePostOperationId, string messageType, List<KeyValuePair<string, string>> filters)
        {
            //This code will need to go to a DB to resolve the URL destination but for now the path will be hard coded
            //in the method.
            string environ = ConfigurationManager.AppSettings["Environment"];
            if (processName == "BaozunSales")
            {
                if (environ.ToLower() == "dev")
                {
                    return "http://localhost:51715/SalesDocOffRamp.xamlx";
                }
                else if (environ.ToLower() == "test")
                {
                    return @"http://hip-development-loadbalancer-1-1047608554.us-west-2.elb.amazonaws.com/AppServices/GpAppService/SalesDocOffRamp/SalesDocOffRamp.xamlx";
                }
            }

            return "";
        }

    }
}
