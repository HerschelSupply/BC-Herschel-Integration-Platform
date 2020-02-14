using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;

namespace OnRampEndPointDefinition
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OnRampSvc" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select OnRampSvc.svc or OnRampSvc.svc.cs at the Solution Explorer and start debugging.
    public class OnRampSvc : IOnRampSvc
    {
        public void InitializeProcess(string guid)
        {
            //Start process
            Trace.WriteLine("We are now in the InitializeProcess method of the generic On Ramp Service.  GUID: " + guid);
        }
    }
}
