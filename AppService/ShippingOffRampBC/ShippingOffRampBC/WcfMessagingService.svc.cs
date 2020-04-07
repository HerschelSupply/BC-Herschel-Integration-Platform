using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Diagnostics;
using System.Web;
using System.Configuration;

namespace BC.Integration.AppService.ShippingOffRampBC
{
    //****DO NOT**** change the class or method name as it will cause a contact mismatch when the service is called from the Push Service.
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WcfMessagingService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WcfMessagingService.svc or WcfMessagingService.svc.cs at the Solution Explorer and start debugging.
    public class WcfMessagingService : IWcfMessagingService
    {
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        public bool InitializeProcess(string encodedMessage)
        {
            Trace.WriteLine(tracingPrefix + "Starting Initialization process of SalesDocWcfService.");
            ProcessDoc processSalesDoc = new ProcessDoc();
            string message = HttpUtility.HtmlDecode(encodedMessage);
            return processSalesDoc.Execute(message);
        }
    }
}
