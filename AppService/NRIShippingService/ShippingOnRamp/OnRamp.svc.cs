using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;

namespace BC.Integration.AppService.NriShippingOnRampService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OnRamp" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select OnRamp.svc or OnRamp.svc.cs at the Solution Explorer and start debugging.
    public class OnRamp : IOnRamp
    {
        public void InitializeProcess(string guid)
        {
            //Start process
            Trace.WriteLine("NriShippingOnRampServiceBC: BC.Integration.AppService.NriShippingOnRampServiceBC service initializing. ");
            Process processor = new Process();
            processor.ProcessData(guid);
        }
    }
}
