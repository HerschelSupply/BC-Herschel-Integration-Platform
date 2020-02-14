using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;
using BC.Integration.Utility;
using System.Configuration;

namespace BC.Integration.AppService.SingletonTemplate
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "WebSvc" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select PushSvc.svc or PushSvc.svc.cs at the Solution Explorer and start debugging.
    public class WebSvc : IWebSvc
    {
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        public void InitializeProcess(string guid)
        {
            try
            { 
                //Start process
                Trace.WriteLine("SingletonTemplate: We are now in the InitializeProcess method of the SingletonTemplate Service.  GUID: " + guid);
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
                startInfo.FileName = ConfigurationManager.AppSettings["ExecutableFilePath"];
                startInfo.Arguments = guid;
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured in the BC.Integration.AppService.SingletonTemplate.WebSvc.InitializeProcess method.  See inner exception for details.", ex);
            }
        }

    }
}
