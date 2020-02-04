using System;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Configuration;

namespace BC.Integration.Utility.SqsPushWebSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "HeartBeat" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select HeartBeat.svc or HeartBeat.svc.cs at the Solution Explorer and start debugging.
    public class HeartBeat : IHeartBeat
    {
        /// <summary>
        /// This method is used by the Server Management component to verify the health of a server.
        /// </summary>
        /// <returns>true</returns>
        public bool Pulse()
        {
            //May want to develop some additional tests to validate server health before returning a response.
            string response = ConfigurationManager.AppSettings["HeartbeatResponse"];

            if (response.ToLower() == "exception")
            {
                throw new Exception("Config file instructed the pulse the return an exception.");
            }

            return true;
        }
    }
}
