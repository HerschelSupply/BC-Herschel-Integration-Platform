using System;
using System.Diagnostics;
using System.Configuration;

namespace BC.Integration.InventoryOnRamp
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "OnRamp" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select OnRamp.svc or OnRamp.svc.cs at the Solution Explorer and start debugging.
    public class OnRamp : IOnRamp
    {
        //CR7: Tracing prefix
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        public void InitializeProcess(string guid)
        {
            //Start process
            guid = Guid.NewGuid().ToString();

            Trace.WriteLine(tracingPrefix + " BC.Integration.InventoryOnRamp service initializing. ");
            Process processor = new Process();
            processor.ProcessData(guid);
        }
    }
}
