using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BC.Integration.Interfaces;
using BC.Integration;
using System.Configuration;
using System.Data.Odbc;
using System.Diagnostics;



namespace BC.Integration.Utility
{
    /// <summary>
    /// The configuration component will ultimately look at a central configuration store and override local configuration.  If no
    /// central configuration is found then the value in the configuration file will be used.
    /// </summary>
    class CentralConfiguration : IConfiguration
    {
        #region Local Variables

        private string connString = "";
        private List<KeyValuePair<string, string>> localConfigurationCollection = null;
        private List<KeyValuePair<string, string>> centralConfigurationCollection = null;
        private Exception retryExceptions = null;
        private int retryCount = 5;
        private int retryInterval = 15;

        #endregion

        #region Configuration Collection Generation
        //Ultimately the configuration should be centrally located in a DB store.  The connection string will be used to access the central store.
        //Initially we will just be connecting to the config file 
        public List<KeyValuePair<string, string>> LocalConfigurationCollection { get => localConfigurationCollection; }

        public List<KeyValuePair<string, string>> CentralConfigurationCollection { get => centralConfigurationCollection; }


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
                KeyValuePair<string, string> pr = new KeyValuePair<string, string>(key, val);
                config.Add(pr);
                if (key == "CentralConfigConnString")
                    connString = val;
            }
            localConfigurationCollection = config;
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

            if (connString == "")
                throw new Exception("BC..Integration.Utility.CentralConfiguration.GetConfiguration could not get the central" +
                    "configuration data as the CentralConfigConnString appsetting is not set in the configuration file.");

            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            ConfigurationData[] configData = new ConfigurationData[0];

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ConfigurationGet ( '" + serviceId +
                                                        "', '" + processName + "')";
                bool isSuccess = Retry.Execute(() => ExecuteConfigQuery(cmd, out configData), new TimeSpan(0, 0, retryInterval), retryCount, true);
                if (!isSuccess)
                    throw new Exception("Failed to retrieve configuration data from the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
            try
            {
                //Process configData object.
                //Convert data to KeyValuePairs ordering keys with the Process type configuration being before Service type configurations.
                //This will effectively allow the Process configurations to overide Service configurations as a key match will be found 
                //and returned, for the process data, before the service data is searched.
                //Returned merged collection.
                if (configData.Count() > 0)
                    return ProcessCentralConfigData(configData);
            }
            catch (Exception ex)
            {
                throw new Exception("BC..Integration.Utility.CentralConfiguration.GetConfiguration, the call to ProcessCentralConfigData" +
                    "returned and exception.", ex);
            }
            return centralConfigurationCollection;
        }

        private List<KeyValuePair<string, string>> ProcessCentralConfigData(ConfigurationData[] configData)
        {
            bool hasProcess = false;
            KeyValuePair<string, string> instrumentationLevel = new KeyValuePair<string, string>();
            KeyValuePair<string, string> tracing = new KeyValuePair<string, string>();
            List<KeyValuePair<string, string>> processCollection = new List<KeyValuePair<string, string>>();
            List<KeyValuePair<string, string>> serviceCollection = new List<KeyValuePair<string, string>>();

            //Get keyvaluepairs...
            foreach (ConfigurationData data in configData)
            {
                if (data.Type.ToLower() == "process")
                {
                    processCollection.Add(new KeyValuePair<string, string>(data.Key, data.Value));
                    if (!hasProcess)
                    {
                        instrumentationLevel = new KeyValuePair<string, string>("InstrumentationLevel", data.InstrumentationLevel);
                        tracing = new KeyValuePair<string, string>("TracingEnabled", data.TracingEnabled);
                    }
                    hasProcess = true;
                }
                else if (data.Type.ToLower() == "service")
                {
                    serviceCollection.Add(new KeyValuePair<string, string>(data.Key, data.Value));
                    if (!hasProcess)
                    {
                        instrumentationLevel = new KeyValuePair<string, string>("InstrumentationLevel", data.InstrumentationLevel);
                        instrumentationLevel = new KeyValuePair<string, string>("TracingEnabled", data.TracingEnabled);
                    }
                }
                else
                    throw new Exception("The BC..Integration.Utility.CentralConfiguration.ProcessCentralConfigData method tried to process the" +
                        "config data but it contained a type other that Process or Service.  The type was: " + data.Type + ".");
            }
            //Set instumentation level and tracing...
            processCollection.Add(instrumentationLevel);
            processCollection.Add(tracing);
            centralConfigurationCollection = MergeCollections(processCollection, serviceCollection);
            return centralConfigurationCollection;
        }

        private List<KeyValuePair<string, string>> MergeCollections(List<KeyValuePair<string, string>> primaryCollection, List<KeyValuePair<string, string>> secondaryCollection)
        {
            //An improvement to this code would be to eliminate the duplicate keys leaving only the key the primary configuration.  This
            //approach relies on using the utility 'GetConfigurationValue' which will return the first key in the collection.  The union
            //command will remove duplicates where the key and value are the same.
            if (primaryCollection == null)
                return secondaryCollection;
            List<KeyValuePair<string, string>> unionedList = new List<KeyValuePair<string, string>>();
            unionedList.AddRange(primaryCollection.Union(secondaryCollection));

            return unionedList;
        }


        public List<KeyValuePair<string, string>> GetConfiguration(string serviceId, string processName)
        {
            //This method needs to be written to update the configuration collection that was recieved as a parameter, from the 
            //service specific values in the configuration DB and return the collection.
            if (localConfigurationCollection == null)
                PopulateConfigurationCollectionFromAppConfig();
            centralConfigurationCollection = GetCentralConfiguration(serviceId, processName);

            try
            {
                //Process localConfigurationCollection and CentralConfigurationCollection objects.
                //duplicates keys with the central config overriding local.
                //Returned merged collection.

                return MergeCollections(centralConfigurationCollection, localConfigurationCollection);
            }
            catch (Exception ex)
            {
                throw new Exception("An exception was raised in the BC..Integration.Utility.CentralConfiguration.GetConfiguration method, " +
                    "with the parameters serviceId: " + serviceId + ", and processName: " + processName + ". The exception was raised while " +
                    "calling the MergeCollections method to merge the central and local configurations", ex);
            }
        }

        private bool ExecuteConfigQuery(OdbcCommand cmd, out ConfigurationData[] configData)
        {
            ConfigurationData[] data = new ConfigurationData[0];
            OdbcDataReader dataReader = null;
            try
            {
                cmd.Connection.Open();
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    int i = 0;
                    data = new ConfigurationData[dataReader.RecordsAffected];
                    while (dataReader.Read())
                    {
                        ConfigurationData row = new ConfigurationData();
                        row.Type = dataReader["Type"].ToString();
                        row.InstrumentationLevel = dataReader["InstrumentationLevel"].ToString();
                        row.TracingEnabled = dataReader["TracingEnabled"].ToString();
                        row.Key = dataReader["Key"].ToString();
                        row.Value = dataReader["Value"].ToString();
                        data[i] = row;
                        i++;
                    }
                }
                dataReader.Close();
                configData = data;
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Central Configuration: Exception occurred trying to connect to the DB BC..Integration.Utility.CentralConfiguration.ExecuteConfigQuery. Exception: " + ex.Message);
                if (retryExceptions == null)
                    retryExceptions = new Exception("Error trying to retrieve configuration data from the DB", ex);
                else
                    retryExceptions = new Exception(ex.Message, retryExceptions);
                configData = null;
                return false;
            }
            finally
            {
                dataReader.Close();
                cmd.Connection.Close();
            }
        }

        #endregion

        #region Code used to determine max processing rate

        /// <summary>
        /// This method returns the number of IIS services servers available to process messages.
        /// </summary>
        /// <returns>Count of available servers</returns>
        public int AvailableServerCount()
        {
            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            int serverCount = 1;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ServerStatusGetServerCount";
                bool isSuccess = Retry.Execute(() => ExecuteServerCountQuery(cmd, out serverCount), new TimeSpan(0, 0, retryInterval), retryCount, true);
                if (!isSuccess)
                    throw new Exception("Failed to retrieve server count data (ServerStatusGetServerCount proc) from the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryExceptions);
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
            //There has to atleast one server so if 0 return 1.
            if (serverCount == 0)
            {
                Trace.WriteLine("Central Configuration: HIP server count method returned 0 when there has to be atleaset one active server.");
                return 1;
            }

            return serverCount;
        }

        private bool ExecuteServerCountQuery(OdbcCommand cmd, out int serverCount)
        {
            ConfigurationData[] data = new ConfigurationData[0];
            OdbcDataReader dataReader = null;
            serverCount = 1;
            try
            {
                cmd.Connection.Open();
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    serverCount = Convert.ToInt32(dataReader["ServerCount"].ToString());
                }
                dataReader.Close();
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Central Configuration: Exception occurred trying to connect to the DB BC..Integration.Utility.CentralConfiguration.ExecuteServerCountQuery");
                if (retryExceptions == null)
                    retryExceptions = new Exception("Error trying to retrieve server count data from the DB", ex);
                else
                    retryExceptions = new Exception(ex.Message, retryExceptions);
                return false;
            }
            finally
            {
                dataReader.Close();
                cmd.Connection.Close();
            }
        }

        #endregion

        #region Code used to dynamically route messages

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
        public string GetDestinationUrl(string queue, string processName, string serviceId, string servicePostOperationId, string messageType, List<KeyValuePair<string, string>> filters)
        {
            Trace.WriteLine("Central Configuration: Start getting destination URL process!!");
            List<string> dests = new List<string>();
            string filterString = "";
            if (filters.Count == 0)
            {
                string dest = LocalCache.GetDestinationUrl(connString, retryInterval, retryCount, processName, queue, serviceId, servicePostOperationId, messageType, "", "");
                if (dest != "")
                {
                    dests.Add(dest);
                }
            }
            else
            {
                foreach (KeyValuePair<string, string> item in filters)
                {
                    string dest = LocalCache.GetDestinationUrl(connString, retryInterval, retryCount, processName, queue, serviceId, servicePostOperationId, messageType, item.Key, item.Value);
                    if (dest != "")
                    {
                        dests.Add(dest);
                    }
                    filterString += item.Key + "|" + item.Value + ",";
                }
            }
            if (dests.Count == 1)
                return dests[0];
            else if (dests.Count > 1)
                throw new Exception("The routing database tables contained multiple destination records that matched the parameters provided.  Process Name: " +
                        processName + ", Service ID: " + serviceId + ", Service Post Operation ID: " + servicePostOperationId +
                        ", Message Type: " + messageType + ", Routing Filters: " + filterString.TrimEnd(','));
            else
                throw new Exception("The routing database tables did not match any records with the parameters provided.  Process Name: " +
                        processName + ", Service ID: " + serviceId + ", Service Post Operation ID: " + servicePostOperationId +
                        ", Message Type: " + messageType + ", Routing Filters: " + filterString.TrimEnd(','));

        }

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
        public List<string> GetDestinationQueueUrls(string processName, string serviceId, string servicePostOperationId, string messageType, List<KeyValuePair<string, string>> filters)
        {
            Trace.WriteLine("Central Configuration: Start get destination URL collection process");
            List<string> dests = new List<string>();
            string filterString = "";
            if (filters.Count == 0)
            {
                dests = LocalCache.GetQueueUrls(connString, retryInterval, retryCount, processName, serviceId, servicePostOperationId, messageType, "", "");
            }
            else
            {
                foreach (KeyValuePair<string, string> item in filters)
                {
                    List<string> ds = LocalCache.GetQueueUrls(connString, retryInterval, retryCount, processName, serviceId, servicePostOperationId, messageType, item.Key, item.Value);
                    if (ds.Count != 0)
                    {
                        foreach(string d in ds)
                        {
                            dests.Add(d);
                        }
                    }
                    filterString += item.Key + "|" + item.Value + ","; //used for exception logging
                }
            }
            if (dests.Count > 0)
                return dests;
            else
                throw new Exception("The routing database tables did not match any records with the parameters provided.  Process Name: " +
                        processName + ", Service ID: " + serviceId + ", Service Post Operation ID: " + servicePostOperationId +
                        ", Message Type: " + messageType + ", Routing Filters: " + filterString.TrimEnd(','));
        }

        #endregion
    }

    /// <summary>
    /// This static class implements a local cache based on generating a hash from the parameters.  The class retrieves all destination URL's
    /// for a specific Process Name an places them in the cache.  If message form another process are also processed the destination URL's will
    /// also be added to the cache.  This will simplify the MySQL required to implement the logic and place the complexity in the C# code.
    /// </summary>
    static class LocalCache
    {
        private static Dictionary<Int64, string> cache = new Dictionary<Int64, string>();
        private static Exception retryCacheExceptions = null;

        /// <summary>
        /// This method is the only method that is needed.  If the destination cannot be resolved, the code will call the DB to populate the 
        /// data into the cache.
        /// Ver 3. Add 'queue' to the parameter list.
        /// 
        /// </summary>
        /// <param name="dbConn">DB connection string</param>
        /// <param name="processName">Process Name</param>
        /// <param name="serviceId">Name of the service that is requesting the information</param>
        /// <param name="servicePostOperationId">Name of the service post operation that is requesting the information.  If there is no post operation either a null or empty string can be passed.</param>
        /// <param name="messageType">Message type that is being posted.  If no message is being posted a null or empty string can be used.</param>
        /// <param name="filterKey">The envelope routing filters.  If there are no filters a null can be passed.</param>
        /// <param name="filterValue">The envelope routing filters.  If there are no filters a null can be passed.</param>
        /// <returns></returns>
        public static string GetDestinationUrl(string dbConn, int retryInterval, int retryCount, string processName, string queue, string serviceId, string servicePostOperationId, string messageType, string filterKey, string filterValue)
        {
            Int64 hash = CreateHash(processName, queue, serviceId, servicePostOperationId, messageType, filterKey, filterValue);
            Trace.WriteLine("Central Configuration: Routing Hash (" + hash.ToString() + ") Generated from " + processName + ", " + queue + ", " + serviceId + ", " + servicePostOperationId + ", " + messageType);
            string url = "";
            try
            {
                url = cache[hash];
            }
            catch (KeyNotFoundException)
            {
                //Go get all the destination URL's available for the process.  Create hashes for each one and populate the cache.
                Trace.WriteLine("Central Configuration: Hash key not found so populate routing cache from DB.");
                try
                {
                    PopulateCache(dbConn, retryInterval, retryCount, processName, false);
                    url = cache[hash];
                }
                catch (KeyNotFoundException)
                {
                    Trace.WriteLine("Central Configuration: The routing database tables did not contain a record that matched the parameters provided.  Process Name: " +
                        processName + ", Service ID: " + serviceId + ", Service Post Operation ID: " + servicePostOperationId +
                        ", Message Type: " + messageType + ", Routing Filter: " + filterKey + "|" + filterValue);
                    return "";
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Central Configuration: Populating routing cache failed. Exception msg: " + ex.Message);
                    throw new Exception("An unexpected error occurred trying to get routing data from the database.  The error occurred in the LocalCache.GetDestinationUrl method.  The exception was -" + ex.Message, ex);
                }

            }
            Trace.WriteLine("Central Configuration: URL found: " + url);
            return url;
        }

        /// <summary>
        /// This method is the only method that is needed.  If the destination cannot be resolved, the code will call the DB to populate the 
        /// data into the cache.
        /// This code uses an index to allow multiple dictionary enteries to exist for the same set of keys.  This allows
        /// multiple queue urls to be returned.
        /// Ver 3. Queue added to CreateHash method.  Since this method is used to get the path to the queues, the value will always be null.
        /// </summary>
        /// <param name="dbConn">DB connection string</param>
        /// <param name="processName">Process Name</param>
        /// <param name="serviceId">Name of the service that is requesting the information</param>
        /// <param name="servicePostOperationId">Name of the service post operation that is requesting the information.  If there is no post operation either a null or empty string can be passed.</param>
        /// <param name="messageType">Message type that is being posted.  If no message is being posted a null or empty string can be used.</param>
        /// <param name="filterKey">The envelope routing filters.  If there are no filters a null can be passed.</param>
        /// <param name="filterValue">The envelope routing filters.  If there are no filters a null can be passed.</param>
        /// <returns></returns>
        public static List<string> GetQueueUrls(string dbConn, int retryInterval, int retryCount, string processName, string serviceId, string servicePostOperationId, string messageType, string filterKey, string filterValue)
        {
            bool isEnd = false;
            List<string> queues = new List<string>();
            int i = 0;
            while (!isEnd)
            {
                //Int64 hash = CreateHash(processName + i, serviceId, servicePostOperationId, messageType, filterKey, filterValue);
                Int64 hash = CreateHash(processName + i, null, serviceId, servicePostOperationId, messageType, filterKey, filterValue);
                string url = "";
                try
                {
                    url = cache[hash];
                    queues.Add(url);
                }
                catch (KeyNotFoundException)
                {
                    if(i > 0)
                    {//If this is the second time this has happened, then exit the while loop.
                        return queues;
                    }
                    //Go get all the destination URL's available for the process.  Create hashes for each one and populate the cache.
                    Trace.WriteLine("Central Configuration: Hash key not found so populate routing cache from DB.");
                    try
                    {
                        PopulateCache(dbConn, retryInterval, retryCount, processName, true);
                        url = cache[hash];
                        queues.Add(url);
                    }
                    catch (KeyNotFoundException)
                    {
                        Trace.WriteLine("Central Configuration: The routing database tables did not contain a record that matched the parameters provided.  Process Name: " +
                            processName + ", Service ID: " + serviceId + ", Service Post Operation ID: " + servicePostOperationId +
                            ", Message Type: " + messageType + ", Routing Filter: " + filterKey + "|" + filterValue);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine("Central Configuration: Populating routing cache failed. Exception msg: " + ex.Message);
                        throw new Exception("An unexpected error occurred trying to get routing data from the database.  The error occurred in the LocalCache.GetDestinationUrl method.  The exception was -" + ex.Message, ex);
                    }

                }
                i++;
            }
            Trace.WriteLine("Central Configuration: GetQueueUrls() complete.  Returning: " + queues.Count + " queue URL's.");
            return queues;
        }

        /// <summary>
        ///Generates a hash based on parameters.
        ///Ver 3. Queue added to parameters.
        /// </summary>
        /// <param name="processName">Process Name</param>
        /// <param name="serviceId">Name of the service that is requesting the information</param>
        /// <param name="servicePostOperationId">Name of the service post operation that is requesting the information.  If there is no post operation either a null or empty string can be passed.</param>
        /// <param name="messageType">Message type that is being posted.  If no message is being posted a null or empty string can be used.</param>
        /// <param name="filters">The envelope routing filters.  If there are no filters a null can be passed.</param>
        /// <returns></returns>
        //private static Int64 CreateHash(string processName, string serviceId, string servicePostOperationId, string messageType, string filterKey, string filterValue)
        private static Int64 CreateHash(string processName, string queue, string serviceId, string servicePostOperationId, string messageType, string filterKey, string filterValue)
        {
            string value = "";

            if (processName != null)
                value += processName;
            else
                throw new Exception("When calling the LocalCache.GetDestinationUrl the process name is required, to retrieve HIP routing information.");

            if (queue != null)
                value += queue;

            if (serviceId != null)
                value += serviceId;

            if (servicePostOperationId != null)
                value += servicePostOperationId;

            if (messageType != null)
                value += messageType;

            if (filterKey != null)
                value += filterKey;

            if (filterValue != null)
                value += filterValue;
            //Trace.WriteLine("Hash Key: " + value.GetHashCode());
            return value.GetHashCode();
        }

        /// <summary>
        /// Used to populate the cache from the routing database tables.
        /// </summary>
        /// <param name="dbConn">Database connection string</param>
        /// <param name="processName">Process Name</param>
        private static void PopulateCache(string dbConn, int retryInterval, int retryCount, string processName, bool isQueues)
        {
            //# Connect to the DB and get the data based on the process name.
            //# The code needs to add to the existing cache as it maybe possible for 2 or more processes to pass through the same
            //  service during a single activation. 
            //# Code need to avoid adding the same Hash key twice.

            if (dbConn == "")
                throw new Exception("BC..Integration.Utility.CentralConfiguration.GetConfiguration could not get the central" +
                    "configuration data as the CentralConfigConnString appsetting is not set in the configuration file.");

            OdbcConnection conn = new OdbcConnection(dbConn);
            OdbcCommand cmd = new OdbcCommand();
            List<string[]> data = new List<string[]>();

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call RoutingGet2 ( '" + processName + "', 0)";  //Flag limits responses to service destinations
                if (isQueues)
                {
                    cmd.CommandText = "Call RoutingGet2 ( '" + processName + "', 1)"; //Flag limits responses to queues
                }
                bool isSuccess = Retry.Execute(() => ExecuteConfigQuery(cmd, out data), new TimeSpan(0, 0, retryInterval), retryCount, true);
                if (!isSuccess)
                    throw new Exception("Failed to retrieve configuration data from the DB after " + retryCount + " retries with an interval of " + retryInterval + " seconds.", retryCacheExceptions);
            }
            finally
            {
                conn.Dispose();
                cmd.Dispose();
            }
            try
            {
                //Process configData object.
                //Convert data to KeyValuePairs removing the duplicates keys with the Process type configuration overriding Service type configuration.
                //Returned merged collection.
                //Ver 3. CreateHash now needs a queue parameter for the push service. Value will be null everywhere else.
                if (data.Count() > 0)
                {
                    if(isQueues)//This code adds an index to the key values support multiple queues for the same key set.
                    {
                        int i = 0;
                        foreach (string[] record in data)
                        {
                            //cache.Add(CreateHash(record[1] + i, record[2], record[3], record[4], record[5], record[6]), record[0]);
                            cache.Add(CreateHash(record[1] + i, null, record[2], record[3], record[4], record[5], record[6]), record[0]);
                            i++;
                       }
                    }
                    else //This is used for service routing where there is a single destination.
                    {
                        foreach (string[] record in data)
                        {
                            //cache.Add(CreateHash(record[1], record[2], record[3], record[4], record[5], record[6]), record[0]);
                            cache.Add(CreateHash(record[1], record[7], record[2], record[3], record[4], record[5], record[6]), record[0]);
                            Trace.WriteLine("Central Configuration: Dictionary hashes generated from " + record[1] + ", " + record[7] + ", " + record[2] + ", " + record[3] + ", " + record[4] + ", " + record[5] + ", " + record[7]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("BC..Integration.Utility.CentralConfiguration.GetConfiguration, The call to CreateHash and " +
                    "add the returned hash to the cache raised an exception.", ex);
            }

        }

        private static bool ExecuteConfigQuery(OdbcCommand cmd, out List<string[]> data)
        {
            List<string[]> collection = new List<string[]>();
            OdbcDataReader dataReader = null;
            try
            {
                cmd.Connection.Open();
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                {
                    int i = 0;
                    while (dataReader.Read())
                    {
                        string[] record = new string[8];
                        record[0] = dataReader["TargetUrl"].ToString();
                        record[1] = dataReader["ProcessName"].ToString();
                        record[2] = dataReader["ServiceId"].ToString();
                        record[3] = dataReader["ServicePostOperationId"].ToString();
                        record[4] = dataReader["MessageType"].ToString();
                        record[5] = dataReader["Key"].ToString();
                        record[6] = dataReader["Value"].ToString();
                        record[7] = dataReader["QueueName"].ToString();
                        collection.Add(record);
                        i++;
                    }
                }
                dataReader.Close();
                data = collection;
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Central Configuration: Exception occurred trying to connect to the DB BC..Integration.Utility.LocalCache.ExecuteConfigQuery. Exception: " + ex.Message);
                if (retryCacheExceptions == null)
                    retryCacheExceptions = new Exception("Error trying to retrieve configuration data from the DB", ex);
                else
                    retryCacheExceptions = new Exception(ex.Message, retryCacheExceptions);
                data = null;
                return false;
            }
            finally
            {
                dataReader.Close();
                cmd.Connection.Close();
            }
        }

    }

    class ConfigurationData
    {
        private string type = "";
        private string instrumentationLevel = "";
        private string tracingEnabled = "";
        private string key = "";
        private string value = "";

        public string Type { get => type; set => type = value; }
        public string InstrumentationLevel { get => instrumentationLevel; set => instrumentationLevel = value; }
        public string TracingEnabled { get => tracingEnabled; set => tracingEnabled = value; }
        public string Key { get => key; set => key = value; }
        public string Value { get => value; set => this.value = value; }
    }

}
