using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.Integration.Interfaces
{
    public interface IConfiguration
    {
        //Ultimately the configuration should be centrally located in a DB store.  The connection string will be used to access the central store.
        //Initially we will just be connecting to the config file 
        List<KeyValuePair<string, string>> LocalConfigurationCollection { get; }
        List<KeyValuePair<string, string>> CentralConfigurationCollection { get; }

        /// <summary>
        /// This method populates a key value pair collection with the calling components configuration files AppConfig settings.
        /// This collection will be passed to components for configuration.  This approach allows new configurations to be added
        /// to support injected code without the need to modify the parent service.
        /// </summary>
        /// <returns>A key value collection of the calling service's AppSettings collection</returns>
        List<KeyValuePair<string, string>> PopulateConfigurationCollectionFromAppConfig();

        /// <summary>
        /// Since initialy we will be just connecting to the config file the parameters will not be used.  When a central DB is used
        /// the parameters will be used to filter the results.
        /// The service will return a union of the search results using both parameter and the service ID and where the Process Name = null.
        /// </summary>
        /// <param name="serviceId">Service Id of the calling component</param>
        /// <param name="processName">Process Name</param>
        /// <returns>Returns an array collection of key value pairs containing the configurations</returns>
        List<KeyValuePair<string, string>> GetCentralConfiguration(string serviceId, string processName);

        List<KeyValuePair<string, string>> GetConfiguration(string serviceId, string processName);

        /// <summary>
        /// This method returns the number of IIS services servers available to process messages.
        /// </summary>
        /// <returns>Count of available servers</returns>
        int AvailableServerCount();


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
        string GetDestinationUrl(string queue, string processName, string serviceId, string servicePostOperationId, string messageType, List<KeyValuePair<string, string>> filters);


        /// <summary>
        /// Dynamic routing method.  The method can be used by a service to determine where to route a message based on the parameters
        /// supplied.  If the parameter does not need to be considered in determining the destination pass null.
        /// This method differs from the other as it can return multiple URL's.  It was written to support message splitting 
        /// by publishing to multiple SQS queues.
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="serviceId"></param>
        /// <param name="servicePostOperationId"></param>
        /// <param name="messageType"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<string> GetDestinationQueueUrls(string processName, string serviceId, string servicePostOperationId, string messageType, List<KeyValuePair<string, string>> filters);

    }
}
