using BC.Integration.APIError;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Web;

namespace BC.Integration.APIError

{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OnRamp" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select OnRamp.svc or OnRamp.svc.cs at the Solution Explorer and start debugging.
    public class WcfMessagingService : IWcfMessagingService
    {

        public bool InitializeProcess(string encodedMessage)
        {
            Trace.WriteLine("Corp.Integration.AppService.APIError service initializing.");
            try
            {
                ProcessErrors processErrors = new ProcessErrors();
                string message = HttpUtility.HtmlDecode(encodedMessage);
                return processErrors.Execute();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Corp.Integration.AppService.APIError service exception occurred. Exception message: " + ex.Message);
            }
            return false;
        }
    }
}

