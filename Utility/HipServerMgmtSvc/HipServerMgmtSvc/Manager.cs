using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.ServiceProcess;
using System.Timers;
using System.Configuration;
using Corp.Integration.Utility;
using System.Net;
using System.Net.Http;

namespace Corp.Instrumentation.ServerMgmt
{
    partial class Manager : ServiceBase
    {
        private AuroraConnector conn = new AuroraConnector();
        private System.Timers.Timer timer;
        private bool tracingEnabled = Convert.ToBoolean(ConfigurationManager.AppSettings["TracingEnabled"]);
        private int delay = Convert.ToInt16(ConfigurationManager.AppSettings["RunDelayInSeconds"]);
        private int skipLimit = Convert.ToInt16(ConfigurationManager.AppSettings["SkipLimit"]);
        private string heartbeatAddress = ConfigurationManager.AppSettings["HeartbeatAddress"];
        private Dictionary<string, int> serverHeartbeatTracking = new Dictionary<string, int>();
        private string serverIp = Utilities.GetServerIpAddress();
        private bool isCurrentServerMarkedActive = true;

        public Manager()
        {
            Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Starting Manager.");
            InitializeComponent();
            int wait = delay * 1000; //30 seconds
            timer = new Timer(wait);
            timer.Elapsed += timer_Elapsed;

            // We don't want the timer to start ticking again till we tell it to.
            timer.AutoReset = false;
        }

        protected override void OnStart(string[] args)
        {
            Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: OnStart event fired.");
            // TODO: Add code here to start your service.
            //Register the server
            try
            {
                conn.RecordServerActivation();
            }
            catch (Exception ex)
            {
                string exMsg = ex.Message;
                while (ex.InnerException != null)
                {
                    exMsg += ex.InnerException.Message;
                    ex = ex.InnerException;
                }
                Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Exception occured trying to record server activation.  Exception: " + exMsg);
            }
            //Start looping through Check Server Availability.
            timer.Start();
        }

        protected void CheckServerAvailability()
        {
            try
            {
                Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Starting CheckServerAvailability method.");
                string webAddress = "";
                List<string> unaccessibleServers = new List<string>();
                //Get IP of all registered servers
                Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Get Active Servers.");
                List<string> serverIps = conn.GetActiveServers(serverIp, out isCurrentServerMarkedActive);
                //Check for health of registered servers
                Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Active Server Count: " + serverIps.Count + ".");
                foreach (string ip in serverIps)
                {
                    Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Checking the health of '" + ip + "'.");
                    //Call Health web service.  This logic needs to be implemented in the future
                    //http://10.20.12.5/Utility/MessageBusService/SqsPushWebSvc.v1/Heartbeat.aspx
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            webAddress = @"http://" + ip + heartbeatAddress;
                            Uri uri = new Uri(webAddress);
                            byte[] arr = client.DownloadData(uri);
                            serverHeartbeatTracking[ip] = 0;
                        }

                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Heartbeat check for : " + webAddress + ".  Exception message returned by call: " + ex.Message);
                        unaccessibleServers.Add(ip);
                    }
                }
                //Update availability of servers
                foreach (string ip in unaccessibleServers)
                {
                    Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Updating tracking dictionary for server '" + ip + "'.");
                    //Update the DB record
                    if(serverHeartbeatTracking[ip] == skipLimit)
                    {
                        Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Skip limit reached, deactivating server record for '" + ip + "'.");
                        conn.RecordServerDeactivation(ip);
                        serverHeartbeatTracking[ip] = 0;
                    }
                    else
                    {
                        Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Heartbeat tracking count increased for '" + ip + "'.");
                        serverHeartbeatTracking[ip] = serverHeartbeatTracking[ip] + 1;
                    }
                }
                //If current server is not found in active server list do a self check.
                if(!isCurrentServerMarkedActive)
                {
                    try
                    {
                        using (WebClient client = new WebClient())
                        {
                            webAddress = @"http://localhost" + heartbeatAddress;
                            Uri uri = new Uri(webAddress);
                            byte[] arr = client.DownloadData(uri);
                            //If the call succeeded should we reactivate?  Logging error for the time being...
                            conn.LogGeneralException("HSMS Error (2). The HIP Svr Mgmt Svc is marked as deactivated but the local host is responding to requests.", new Exception("No Inner Exceptions"));
                        }
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Local host heartbeat check for : " + webAddress + ".  Exception message returned by call: " + ex.Message);
                        conn.LogGeneralException("HSMS Error (1). The HIP Svr Mgmt Svc is marked as deactivated and the local host is returning exceptions.", ex);
                    }
                }

                Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Server health check complete.");
                timer.Start();
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: An exception occured while checking server availability.  Exception msg: '" + ex.Message + "'.");
            }
        }

        protected override void OnStop()
        {
            try
            {

                // TODO: Add code here to perform any tear-down necessary to stop your service.
                timer.Stop();

                Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: Deactivating current server during shut down. Server: '" + Utilities.GetServerIpAddress() + "'.");
                //Mark server as unavailable
                conn.RecordServerDeactivation(Utilities.GetServerIpAddress());
            }
            catch(Exception ex)
            {
                string exMsg = ex.Message;
                while (ex.InnerException != null)
                {
                    exMsg += ex.InnerException.Message;
                    ex = ex.InnerException;
                }
                Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: An exception occured while shutting down the Corp.Instrumentation.ServerMgmt.Manager service.  Exception msg: '" + exMsg + "'.");
            }
        }

        void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                CheckServerAvailability();
            }
            catch (Exception ex)
            {
                //this.Stop(); //Stop the service
                Trace.WriteLineIf(tracingEnabled, "HIP Mgmt Svc: timer_Elapsed method raised an exception.  Exception message: '" + ex.Message + "'.");
                throw new Exception("Error raised while trying to check server availability.", ex);
            }
        }
    }
}
