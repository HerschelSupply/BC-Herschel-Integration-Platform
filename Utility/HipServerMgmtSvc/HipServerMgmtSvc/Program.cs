using System;
using System.ServiceProcess;


namespace Corp.Instrumentation.ServerMgmt
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Manager()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
