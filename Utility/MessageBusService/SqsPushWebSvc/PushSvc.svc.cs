using System;
using System.Collections.Generic;
using System.Configuration;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using BC.Integration.Utility;

namespace BC.Integration.Utility.SqsPushWebSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PushSvc" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PushSvc.svc or PushSvc.svc.cs at the Solution Explorer and start debugging.
    public class PushSvc : IPushSvc
    {
        public void InitializeProcess(string guid, string queueName, decimal maxRate, bool isBatch, int maxBatchSize)
        {
            try
            { 
                //Start process
                Trace.WriteLine("We are now in the InitializeProcess method of the Push Service.  GUID: " + guid);
                //SqsPushService svc = new SqsPushService();
                //svc.ProcessController(guid, queueName, maxRate, isBatch, maxBatchSize);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                //startInfo.FileName = @"C:\HIP Herschel Integration Platform\Utility\MessageBusService\SqsPushWebSvc.v1\bin\BC.Integration.Utility.SqsPushService.exe";
                startInfo.FileName = ConfigurationManager.AppSettings["ExePath"];
                startInfo.Arguments = guid + " " + queueName + " " + maxRate + " " + isBatch + " " + maxBatchSize;
                //Process.Start("BC.Integration.Utility.SqsPushService.exe " + guid + " " + queueName + " " + maxRate + " " + isBatch + " " + maxBatchSize);
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured in the BC.Integration.Utility.SqsPushWebSvc.PushSvc.InitializeProcess method.  See inner exception for details.", ex);
            }
        }

    }
}
