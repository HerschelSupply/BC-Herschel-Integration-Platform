using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using System.Web;

namespace BC.Integration.AppService.BC
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WcfMessagingService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select WcfMessagingService.svc or WcfMessagingService.svc.cs at the Solution Explorer and start debugging.
    public class WcfMessagingService : IWcfMessagingService
    {
        public bool InitializeProcess(string encodedMessage)
        {
            Trace.WriteLine("Starting Initialization process of SalesDocWcfService.");
            ProcessSalesDoc processSalesDoc = new ProcessSalesDoc();
            string message = HttpUtility.HtmlDecode(encodedMessage);
            return processSalesDoc.Execute(message);
        }
    }
}
