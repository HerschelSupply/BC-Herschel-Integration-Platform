using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;

namespace PushSvcEndPointDefinition
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "PushSvc" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PushSvc.svc or PushSvc.svc.cs at the Solution Explorer and start debugging.
    public class PushSvc : IPushSvc
    {
        public void InitializeProcess(string guid, string queueName, decimal maxRate, bool isBatch, int maxBatchSize)
        {
            //Start process
            Trace.WriteLine("We are now in the InitializeProcess method of the Push Service.  GUID: " + guid);
        }
    }
}
