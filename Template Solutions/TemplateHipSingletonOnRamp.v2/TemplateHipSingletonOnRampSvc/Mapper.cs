using System;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;
using System.Configuration;

namespace BC.Integration.AppService.TemplateHipSingletonOnRamp
{
    public static class Mapper
    {
        private static string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private static string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
        private static Dictionary<string, string> shopIdToSiteId;
        /// <summary>
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static string Transform ()
        {
            Trace.WriteLine(tracingPrefix + "Starting mapping.");
            string result = "";
            return result;
        }

 
    }
}