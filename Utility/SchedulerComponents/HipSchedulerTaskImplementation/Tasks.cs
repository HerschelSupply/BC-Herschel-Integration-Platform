using System;
using System.Diagnostics;
using System.Configuration;
using System.ServiceModel;
using System.Data.Odbc;

namespace Corp.Integration.Utility
{
    /// <summary>
    /// These methods are used to simplify the scheduler implementation by placing the complexity in the C# code controled by the company.
    /// </summary>
    public class Tasks
    {
        #region Initializer methods
        /// <summary>
        /// This method is used by the scheduler to activate the push service.
        /// </summary>
        /// <param name="activationGuid">GUID provided by the scheduler to allow tracking of activations</param>
        /// <param name="queueName">Name of queue the push service will process.</param>
        /// <param name="maxRate">Max rate the push service should process the queue</param>
        /// <param name="isBatch">Flag to determine if the push service should batch the messages from the queue.</param>
        /// <param name="maxBatchSize">If the push service should batch the messages what is the max sixe of a batch</param>
        /// <returns>Returns 'Complete' or throws and exception</returns>
        public string PushInitializer(string activationGuid, string queueName, decimal maxRate, bool isBatch, int maxBatchSize)
        {
            Trace.WriteLine("HIP Scheduler Component: Start the HIP Push Initializer");
            string url = ConfigurationManager.AppSettings["PushServiceUrl"];

            Trace.WriteLine("HIP Scheduler Component: Call Service from client. URL: '" + url + "', Max Rate '" + maxRate + "', Queue Name: '" + queueName +
                                "', Is Batch: '" + isBatch + ", Max Batch Size: '" + maxBatchSize + "'.");

            try
            {
                PushService.PushSvcClient client = new PushService.PushSvcClient();
                client.Endpoint.Address = new EndpointAddress(url);
                client.InitializeProcess(activationGuid, queueName, maxRate, isBatch, maxBatchSize);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Exception occured whilst running the HIP Push Initializer.");
                throw new Exception("An error occurred trying to call the Push service at '" + url + "'. Please review the inner exception for details of the error.", ex);
            }

            Trace.WriteLine("HIP Scheduler Component: Push Servcie call complete.");
            return "Completed";
        }

        /// <summary>
        /// This method is used by the scheduler to activate the push service.
        /// </summary>
        /// <param name="activationGuid">GUID provided by the scheduler to allow tracking of activations</param>
        /// <param name="queueName">Name of queue the push service will process.</param>
        /// <param name="maxRate">Max rate the push service should process the queue</param>
        /// <param name="isBatch">Flag to determine if the push service should batch the messages from the queue.</param>
        /// <param name="maxBatchSize">If the push service should batch the messages what is the max sixe of a batch</param>
        /// <returns>Returns 'Complete' or throws and exception</returns>
        public string PushSingletonInitializer(string activationGuid, string queueName, decimal maxRate, bool isBatch, int maxBatchSize)
        {
            Trace.WriteLine("HIP Scheduler Component: Start the HIP Push Singleton Initializer");
            string url = ConfigurationManager.AppSettings["PushServiceUrl"];

            //Get the IP of the top IIS server from the Server Status table. The IP will be used to determine the final URL
            //Get base URL form config and substitute IP address then add on the service name.
            string serverIp = GetIisServerIp();
            string stringToReplace;
            if (url.Substring(0, 5).ToLower() == "https")
            {
                stringToReplace = url.Substring(8, url.IndexOf('/', 8) - 8);
            }
            else //http...
            {
                stringToReplace = url.Substring(7, url.IndexOf('/', 7) - 7);
            }
            string updatedUrl = url.Replace(stringToReplace, serverIp);

            Trace.WriteLine("HIP Scheduler Component: Call Service from client. URL: '" + url + "', Max Rate '" + maxRate + "', Queue Name: '" + queueName +
                                "', Is Batch: '" + isBatch + ", Max Batch Size: '" + maxBatchSize + "'.");
            //Adjust maxRate.  The push servise spreads the max rate out accross all available servers, but in a singleton
            //only one push server will be activated (not one on each server)  The max rate needs to be increased by the number 
            //of servers, so that when the push service calculates the rate it will be correct for a single server.
            int svrCount = GetHipIisServerCount();
            maxRate = maxRate * svrCount;
            Trace.WriteLine("HIP Scheduler Component: Singleton adjusted max rate: '" + maxRate);

            try
            {
                PushService.PushSvcClient client = new PushService.PushSvcClient();
                client.Endpoint.Address = new EndpointAddress(url);
                client.InitializeProcess(activationGuid, queueName, maxRate, isBatch, maxBatchSize);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Exception occured whilst running the HIP Push Singleton Initializer.");
                throw new Exception("An error occurred trying to call the Push service at '" + url + "'. Please review the inner exception for details of the error.", ex);
            }

            Trace.WriteLine("HIP Scheduler Component: Singleton Push Servcie call complete.");
            return "Completed";
        }

        /// <summary>
        /// This method is used by the scheduler to activate a scaled on-ramp service.
        /// </summary>
        /// <param name="activationGuid">GUID provided by the scheduler to allow tracking of activations</param>
        /// <param name="url">URL of the on-ramp service to activate</param>
        /// <returns>Returns 'Complete' or throws and exception</returns>
        public string OnRampInitializer(string activationGuid, string url)
        {
            Trace.WriteLine("HIP Scheduler Component: Start the HIP On-Ramp Initializer");

            Trace.WriteLine("HIP Scheduler Component: OnRampInitializer method parameters. URL: '" + url + "', Activation GUID: '" + activationGuid + "'.");

            try
            {
                OnRampService.OnRampSvcClient client = new OnRampService.OnRampSvcClient();
                client.Endpoint.Address = new EndpointAddress(url);
                client.InitializeProcess(activationGuid);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Exception occured whilst running the HIP On-Ramp Initializer.");
                throw new Exception("An error occurred trying to call the On-Ramp service at '" + url + "'. Please review the inner exception for details of the error.", ex);
            }

            Trace.WriteLine("HIP Scheduler Component: On-Ramp Servcie call complete.");
            return "Completed";
        }

        /// <summary>
        /// This method is used by the scheduler to activate a singleton on-ramp service.
        /// </summary>
        /// <param name="activationGuid">GUID provided by the scheduler to allow tracking of activations</param>
        /// <param name="url">URL of the on-ramp service to activate.  The server name will be substituted.</param>
        /// <returns>Returns 'Complete' or throws and exception</returns>
        public string OnRampSingletonInitializer(string activationGuid, string url)
        {
            Trace.WriteLine("HIP Scheduler Component: Start the HIP On-Ramp Singleton Initializer");

            Trace.WriteLine("HIP Scheduler Component: OnRampSingletonInitializer method parameters. URL: '" + url + "', Activation GUID: '" + activationGuid + "'.");

            //Get the IP of the top IIS server from the Server Status table. The IP will be used to determine the final URL
            //Get base URL form config and substitute IP address then add on the service name.
            string serverIp = GetIisServerIp();
            string stringToReplace;
            if (url.Substring(0, 5).ToLower() == "https")
            {
                stringToReplace = url.Substring(8, url.IndexOf('/', 8) - 8);
            }
            else //http...
            {
                stringToReplace = url.Substring(7, url.IndexOf('/', 7) - 7);
            }
            string updatedUrl = url.Replace(stringToReplace, serverIp);

            Trace.WriteLine("HIP Scheduler Component: Updated URL for the OnRamp Singleton Initializer: " + updatedUrl);

            try
            {
                OnRampService.OnRampSvcClient client = new OnRampService.OnRampSvcClient();
                client.Endpoint.Address = new EndpointAddress(updatedUrl);
                client.InitializeProcess(activationGuid);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Exception occured whilst running the HIP On-Ramp Singleton Initializer.");
                throw new Exception("An error occurred trying to call the On-Ramp singleton service at '" + url + "'. Please review the inner exception for details of the error.", ex);
            }

            Trace.WriteLine("HIP Scheduler Component: On-Ramp Singleton Servcie call complete.");
            return "Completed";
        }

        /// <summary>
        /// This method is used by the scheduler to activate a notification service to process a specific integration service based on Process Name, Sevice ID and Issue Category.
        /// </summary>
        /// <param name="processName">Name of the Integration Process to use to search the notification log entries.</param>
        /// <param name="serviceId">Service ID to use to search the notification log entries.</param>
        /// <param name="issueCategory">Issue Category to use to search the notification log entries.</param>
        /// <returns>Returns 'Complete' or throws and exception</returns>
        public string NotificationInitializer(string processName, string serviceId, string issueCategory)
        {
            Trace.WriteLine("HIP Scheduler Component: Start the HIP Notification Initializer");
            string url = ConfigurationManager.AppSettings["NotificationServiceUrl"];

            Trace.WriteLine("HIP Scheduler Component: Call Service from client. URL: '" + url + "', Process Name '" + processName + "', Service ID: '" + serviceId +
                                "', Issue Category: '" + issueCategory + "'.");

            EndpointAddress add = new EndpointAddress(url);

            try
            {
                NotificationService.NotificationServiceClient client = new NotificationService.NotificationServiceClient();
                Trace.WriteLine("HIP Scheduler Component: Client created");
                client.Endpoint.Address = new EndpointAddress(url);
                Trace.WriteLine("HIP Scheduler Component: Endpoint updated");
                bool result = client.InitializeNotification(processName, serviceId, issueCategory);

                if(result)
                {
                    Trace.WriteLine("HIP Scheduler Component: HIP Notification Web Service returned true.");
                }
                else
                {
                    Trace.WriteLine("HIP Scheduler Component: HIP Notification Web Service returned false.");
                    throw new Exception("HIP Notification Web Service returned false.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Exception occured whilst running the HIP Notification Initializer. Exception message: " + ex.Message);
                throw new Exception("An error occurred trying to call the Notification service at " + url + ". Please review the inner exception for details of the error.", ex);
            }

            Trace.WriteLine("HIP Scheduler Component: Notification Servcie call complete.");
            return "Completed";

        }

        #endregion

        #region Activation Monitoring Methods

        /// <summary>
        /// Used by the scheduler to check to see if the service was successfully activated.
        /// </summary>
        /// <param name="activationGuid">Activation GUID to serch for in the Activation Log Table</param>
        /// <returns>Activation Status</returns>
        public string CheckForActivation(string activationGuid)
        {
            Trace.WriteLine("HIP Scheduler Component: CheckForActivation method parameter value. Guid: '" + activationGuid + "'.");

            string status = CallActivationLog(activationGuid);

            if (status == "Activation Failed")
                return "Exception:: " + status;
                //throw new Exception("No activation record could be found in the ActivationLog table for the activation GUID -" + activationGuid + ". This suggests that the service was not activated successfully.");
            if (status == "Multiple Activations")
                return "Exception:: " + status;
                //throw new Exception("Multiple activation records were found in the ActivationLog table for the activation GUID -" + activationGuid);

            return status;
        }
        
        /// <summary>
        /// Used by the scheduler to check to see if a long running service completed successfully.
        /// </summary>
        /// <param name="activationGuid">Activation GUID to serch for in the Activation Log Table</param>
        /// <returns>Activation Status</returns>
        public string CheckForLongRunningActivation(string activationGuid)
        {
            Trace.WriteLine("HIP Scheduler Component: CheckForLongRunningActivation method parameter value. Guid: '" + activationGuid + "'.");

            string status = CallActivationLog(activationGuid);

            if (status == "Multiple Activations")
                return "Exception:: " + status;
            //throw new Exception("Multiple activation records were found in the ActivationLog table for the activation GUID -" + activationGuid);

            return status;
        }

        #endregion

        #region DB calling methods
        /// <summary>
        /// Used to communicate with the DB when determining activation status.
        /// </summary>
        /// <param name="activationGuid">Activation GUID to serch for in the Activation Log Table</param>
        /// <returns></returns>
        private string CallActivationLog(string activationGuid)
        {
            string connString;
            try
            {
                connString = ConfigurationManager.AppSettings["DbConnectionString"];
                Trace.WriteLine("HIP Scheduler Component: CheckForActivation method connection string value: '" + connString + "'.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Corp.Integration.Utility.Tasks.CheckForActivation could not connect to the activation history as the" +
                    " DbConnectionString appsetting is not set in the configuration file.");
                throw new Exception("Corp.Integration.Utility.Tasks.CheckForActivation could not connect to the activation history as the" +
                    " DbConnectionString appsetting is not set in the configuration file.", ex);
            }
            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            OdbcDataReader dataReader = null;
            string status = "";

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ActivationStatus ( '" + activationGuid + "')";
                cmd.Connection.Open();
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                    status = dataReader["ActivationStatus"].ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Exception occurred trying to connect to the DB Corp.Integration.Utility.Tasks.CheckForActivation. Exception message: " + ex.Message);
                throw new Exception("Exception occurred trying to connect to the DB Corp.Integration.Utility.Tasks.CheckForActivation. Exception message: ", ex);
            }
            finally
            {
                dataReader.Close();
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
            return status;
        }

        /// <summary>
        /// This method retrieves the top IIS server IP address ordered by ServiceStatusId
        /// </summary>
        /// <returns>IP address</returns>
        private string GetIisServerIp()
        {
            string connString;
            try
            {
                connString = ConfigurationManager.AppSettings["DbConnectionString"];
                Trace.WriteLine("HIP Scheduler Component: GetIisServerIp method connection string value: '" + connString + "'.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Corp.Integration.Utility.Tasks.GetIisServerIp could not connect to Server Status table as the" +
                    " DbConnectionString appsetting is not set in the configuration file.");
                throw new Exception("Corp.Integration.Utility.Tasks.GetIisServerIp could not connect to the Server Status table as the" +
                    " DbConnectionString appsetting is not set in the configuration file.", ex);
            }
            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            OdbcDataReader dataReader = null;
            string ip = "";

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ServerStatusGetTopOne";
                cmd.Connection.Open();
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                    ip = dataReader["ServerIp"].ToString();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Exception occurred trying to connect to the DB Corp.Integration.Utility.Tasks.GetIisServerIp. Exception message: " + ex.Message);
                throw new Exception("Exception occurred trying to connect to the DB Corp.Integration.Utility.Tasks.GetIisServerIp. Exception message: ", ex);
            }
            finally
            {
                dataReader.Close();
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
            return ip;
        }

        /// <summary>
        /// Used by the Singleton Push Intialization method to adjust rate based on the number of servers that are available.
        /// </summary>
        /// <returns>Server Count</returns>
        private int GetHipIisServerCount()
        {
            string connString;
            try
            {
                connString = ConfigurationManager.AppSettings["DbConnectionString"];
                Trace.WriteLine("HIP Scheduler Component: GetHipIisServerCount method connection string value: '" + connString + "'.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Corp.Integration.Utility.Tasks.GetHipIisServerCount could not connect to Server Status table as the" +
                    " DbConnectionString appsetting is not set in the configuration file.");
                throw new Exception("Corp.Integration.Utility.Tasks.GetHipIisServerCount could not connect to the Server Status table as the" +
                    " DbConnectionString appsetting is not set in the configuration file.", ex);
            }
            OdbcConnection conn = new OdbcConnection(connString);
            OdbcCommand cmd = new OdbcCommand();
            OdbcDataReader dataReader = null;
            int svrCount = 0;

            try
            {
                cmd.Connection = conn;
                cmd.CommandText = "Call ServerStatusGetServerCount";
                cmd.Connection.Open();
                dataReader = cmd.ExecuteReader();

                if (dataReader.HasRows)
                    svrCount = Convert.ToInt16(dataReader["ServerCount"].ToString());
            }
            catch (Exception ex)
            {
                Trace.WriteLine("HIP Scheduler Component: Exception occurred trying to connect to the DB Corp.Integration.Utility.Tasks.GetHipIisServerCount. Exception message: " + ex.Message);
                throw new Exception("Exception occurred trying to connect to the DB Corp.Integration.Utility.Tasks.GetHipIisServerCount. Exception message: ", ex);
            }
            finally
            {
                dataReader.Close();
                cmd.Connection.Close();
                conn.Dispose();
                cmd.Dispose();
            }
            return svrCount;
        }

        #endregion
    }
}


