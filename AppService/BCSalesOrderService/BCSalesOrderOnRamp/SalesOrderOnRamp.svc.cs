using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using System.Configuration;

namespace Corp.Integration.AppService.GPSalesOrderSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SalesOrderOnRamp" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SalesOrderOnRamp.svc or SalesOrderOnRamp.svc.cs at the Solution Explorer and start debugging.
    public class SalesOrderOnRamp : ISalesOrderOnRamp
    {
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
        public void InitializeProcess(string guid)
        {
            //Sid - Delete this later
            guid = Guid.NewGuid().ToString();

            //Start process
            Trace.WriteLine(tracingPrefix + "Service initializing. ");
            Process processor = new Process();
            processor.ProcessData(guid);
        }
    }
}
