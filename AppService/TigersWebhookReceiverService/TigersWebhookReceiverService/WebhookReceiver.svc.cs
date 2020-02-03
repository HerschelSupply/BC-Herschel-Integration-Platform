using System;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using System.IO;
using BC.Integration.AppService.TigersWebhookReceiverService.Models;
using BC.Integration.AppService.TigersWebhookReceiverService.Managers;
using System.Configuration;
using System.Security.Claims;
using System.ServiceModel.Web;
using System.Diagnostics.Tracing;

namespace BC.Integration.AppService.TigersWebhookReceiverService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OnRamp" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select OnRamp.svc or OnRamp.svc.cs at the Solution Explorer and start debugging.
    public class WebhookReceiver : IWebhookReceiver
    {
        
        public void InitializeProcess()
        {
            //Start process
            Trace.WriteLine("TigersWebhookReceiver.All.OnR : BC.Integration.AppService.TigersWebhookReceiverService service initializing. ");
            Guid obj = Guid.NewGuid();
            Process processor = new Process();
            processor.ProcessData(obj.ToString());
        }
    }
}


