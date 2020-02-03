using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ServiceModel;
using System.Threading;

namespace Corp.Integration.Utility.HipOnRampInitializer
{
    class Program
    {
        /// <summary>
        /// The On-Ramp Initializer needs to know which On-Ramp service to initialize. To do this the service needs the URL as a parameter.
        /// </summary>
        /// <param name="args">Target Service URL</param>
        static void Main(string[] args)
        {
            Trace.WriteLine("Call Service from client. Guid: '" + args[1] + "'.");

            Trace.WriteLine("Start the HIP OnRamp Initializer");
            if (args.Count() != 2)
                throw new Exception("The HipOnRampInitializer requires the target On-Ramp service URL and a guid to track activations.");
            string url = args[0];
            string guid = args[1];

            Trace.WriteLine("Call Service from client. URL: '" + url + "'.");

            EndpointAddress add = new EndpointAddress(url);
            try
            {
                OnRamp.OnRampSvcClient client = new OnRamp.OnRampSvcClient();
                client.Endpoint.Address = new EndpointAddress(url);
                client.InitializeProcess(guid);
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Exception occured whilst running the HIP OnRamp Initializer.");
                throw new Exception("An error occured trying to call the on-Ramp service at '" + url + "'. Please review the inner exception for details of the error.", ex);
            }

            Trace.WriteLine("On-Ramp Servcie call complete.");
        }

    }

    public class ExecuteService
    {
        public string DoWork(string url, string guid)
        {
            Trace.WriteLine("Do Work Start: " + DateTime.Now.ToLocalTime());
            Trace.WriteLine("Call Service from client. Guid: '" + guid + "', URL: '" + url + "'.");
            return "Testing";

        }

        public string CheckForActivation(string guid)
        {
            Trace.WriteLine("Call Service from client. Guid: '" + guid + "'.");
            Trace.WriteLine("Check for activation End: " + DateTime.Now.ToLocalTime());

            //return "Success";
            return "Still active";
            //            return "It Blew Up: Exception message....";
            //            throw new Exception("It blew up...!");

        }

        public string CheckForLongRunningActivation(string guid)
        {
            Trace.WriteLine("Call Service from client. Guid: '" + guid + "'.");
            Trace.WriteLine("Check for activation End: " + DateTime.Now.ToLocalTime());

            return "Still active";
            //return "Complete";
//            throw new Exception("It blew up...!");
            
        }

    }
}
