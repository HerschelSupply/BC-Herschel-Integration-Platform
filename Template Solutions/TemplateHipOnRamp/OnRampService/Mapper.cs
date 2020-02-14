using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Diagnostics;

namespace BC.Integration.AppService.TemplateHipOnRamp
{
    public static class Mapper
    {
        private static string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private static string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
        public static string Convert (string message)
        {
            Trace.WriteLine(tracingPrefix + "Mapping 'Convert' method started.");
            //______________________________________________________________________________________________
            //Add code to convert message to the canonical format
            //______________________________________________________________________________________________

            return message;
        }
    }
}