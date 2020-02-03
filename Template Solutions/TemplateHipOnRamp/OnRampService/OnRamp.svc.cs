using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using System.Configuration;

namespace BC.Integration.AppService.TemplateHipOnRamp
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OnRamp" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select OnRamp.svc or OnRamp.svc.cs at the Solution Explorer and start debugging.
    public class OnRamp : IOnRamp
    {
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
        public void InitializeProcess(string guid)
        {
            //Start process
            Trace.WriteLine(tracingPrefix + "Service initializing. ");
            Process processor = new Process();
            processor.ProcessData(guid);
        }
    }
}
