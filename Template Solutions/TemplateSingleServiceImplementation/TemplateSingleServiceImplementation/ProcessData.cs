using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using BC.Integration.Interfaces;
using BC.Integration.Utility;
using System.Xml;
using System.IO;

namespace BC.Integration.AppService.ProjectName.TemplateProject
{
    public class Process
    {

        #region Properties

        private List<KeyValuePair<string, string>> configuration = null;

        //Dependency Injection Components
        IConfiguration config;
        IInstrumentation instrumentation;
        IFtpIntegrator integrator;
        UnityContainer container;

        //Key service Properties that should never change from design time
        private string processName = ConfigurationManager.AppSettings["ProcessName"];
        private string serviceId = ConfigurationManager.AppSettings["ServiceId"];
        private string serviceOperationId = ConfigurationManager.AppSettings["ServiceOperationId"];
        private string servicePostOperationId = ConfigurationManager.AppSettings["ServicePostOperationId"];
        private string centralConfigConnString = ConfigurationManager.AppSettings["CentralConfigConnString"];
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";

        //These rest of the properties could be overridden by the central config store at the service ot process level
        private string archiveFolder = "";
        
        //Msg Envelope properties
        private string messageType = "";
        private string outgoingMessageType = "";
        private string filterKeyValuePairs = "";
        private string processKeyValuePairs = "";
        private string topic = "";
       
        private string ftpHost = "";
        private MessageManager msgMgr = new MessageManager();
        private string batchName = "";
        private string documentNumber;
        private XmlDocument outgoingMessage;
        private string messageFormat = "csv";
        private decimal serviceVersion = 1;
        private decimal messageVersion = 1;
        private bool tracingEnabled = false;
        private bool localFileSource = false;
        private string exceptionMessageFolderPath = "";
        private string tracingMessageFolderPath = "";
        private string pickupFileFolderPath = "";
        private string siteIdsToAggregate = "";

        #endregion

        #region Configuration methods
        /// <summary>
        /// Retrives the service and global central config values from the store.  The service properties are applied to the 
        /// service first, so global properties will override all properties.
        /// </summary>
        /// <param name="config">The configuration class to implement the retrieval of the central configuration</param>
        private void OverrideConfigProperties(IConfiguration config)
        {
            //This is an On-Ramp so we know process name from the beginning so no reset needed.
            configuration = config.GetConfiguration(serviceId, processName);
            PopulateLocalVaribale();
        }

        /// <summary>
        /// This method populates local variables with the data from the configuration key/value pair collection.  It is
        /// called by the OverrideConfigProperties method to populated the variables with local and central values.
        /// </summary>
        private void PopulateLocalVaribale()
        {
            messageFormat = Utilities.GetConfigurationValue(configuration, "MessageFormat");
            serviceVersion = Convert.ToDecimal(Utilities.GetConfigurationValue(configuration, "ServiceVersion"));
            messageVersion = Convert.ToDecimal(Utilities.GetConfigurationValue(configuration, "MessageVersion"));
            archiveFolder = Utilities.GetConfigurationValue(configuration, "ArchiveFolder");
            exceptionMessageFolderPath = Utilities.GetConfigurationValue(configuration, "ExceptionMessageFolderPath");
            exceptionMessageFolderPath = exceptionMessageFolderPath.Replace("#ProcessName#", processName);
            tracingMessageFolderPath = Utilities.GetConfigurationValue(configuration, "TracingMessageFolderPath");
            tracingMessageFolderPath = tracingMessageFolderPath.Replace("#ProcessName#", processName);
            pickupFileFolderPath = Utilities.GetConfigurationValue(configuration, "PickupFileFolderPath");
            messageType = Utilities.GetConfigurationValue(configuration, "MessageType");
            outgoingMessageType = Utilities.GetConfigurationValue(configuration, "OutgoingMessageType");
            filterKeyValuePairs = Utilities.GetConfigurationValue(configuration, "FilterKeyValuePairs");
            processKeyValuePairs = Utilities.GetConfigurationValue(configuration, "ProcessKeyValuePairs");
            topic = Utilities.GetConfigurationValue(configuration, "Topic");
            siteIdsToAggregate = Utilities.GetConfigurationValue(configuration, "SiteIdsToAggregate");
            ftpHost = Utilities.GetConfigurationValue(configuration, "FtpHost");

            string val = Utilities.GetConfigurationValue(configuration, "TracingEnabled");
            if (val != "")
            {
                if (val.ToLower() == "true" || val.ToLower() == "false")
                {
                    tracingEnabled = Convert.ToBoolean(val);
                }
                if (val == "1" || val == "0")
                {
                    tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
                }
            }

            string val2 = Utilities.GetConfigurationValue(configuration, "LocalFileSource");
            if (val2 != "")
            {
                if (val2.ToLower() == "true" || val2.ToLower() == "false")
                {
                    localFileSource = Convert.ToBoolean(val2);
                }
                if (val2 == "1" || val2 == "0")
                {
                    localFileSource = Convert.ToBoolean(Convert.ToInt16(val2));
                }
            }
        }

        #endregion

        /// <summary>
        /// Process Data is the main method in the BC.Integration.AppService.TemplateHipOnRamp class.  It implements the following steps;
        /// -Resolves the Unity DI classes.
        /// -Overrides local property setting with the central configuration using the IConfiguration component.
        /// -Connect to the source system and checks to see if there are available files
        /// -Download data
        /// -Log activity
        /// -Process the data and spliting it into seperate messages
        /// -Log activity for each message
        /// </summary>
        public void ProcessData(string activationGuid)
        {
            try
            {
                CreateDiComponents();
                try
                {
                    //Log Activation Start
                    instrumentation.LogActivation(serviceId, new Guid(activationGuid.Substring(0, 36)), false);

                    batchName = "TemplateProject";
                    List<string> data = new List<string>();
                    //_____________________________________________________________________________________________
                    //Add code to get data from source system.  

                    //Get historical and current dates from tracker DB then consolidate into a single list of dates.
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Get source message data.");

                    //Process message if it contains one or more messages
                    if (data.Count > 0)
                    {
                        integrator.CreateSession(configuration);

                        Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Dates retrieved start processing each date.  Count: " + data.Count);
                        //Create initial msg envelope to represent the start of the service process and call Instrumenation
                        XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, messageType, messageVersion, messageFormat, batchName);
                        instrumentation.LogActivity(msg);
                        //Log Activation Association with Interchange
                        instrumentation.AssociateActivationWithInterchages(new Guid(activationGuid.Substring(0, 36)), msgMgr.EntryPointEnvelope.Interchange.InterchangeId);


                        //______________________________________________________________________________________________

                        int i = 1;
                        foreach (string msgIn in data)
                        {
                            documentNumber = ""; //Set document number to support tracking and troubleshooting.
                            Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Processing message collection.  Document Number: " + documentNumber);

                            //ADD implementation logic to get message data
                            try
                            {
                                if (msgIn != "")
                                {
                                    //----------------------------------------------------------------
                                    //Map message
                                    string msgOut = msgIn;

                                    //----------------------------------------------------------------

                                    //Create envelope and add canonical message to the envelope
                                    outgoingMessage = msgMgr.CreateOffRampMessage(servicePostOperationId, outgoingMessageType, messageVersion, messageFormat, msgOut, i, null, documentNumber);


                                    try
                                    {
                                        //-----------------------------------------------------------------
                                        //Push msgOut to destination system

                                        //-----------------------------------------------------------------
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("An Exception occurred while trying to .... (BC.Integration.AppService.ProjectName.TemplateProject.ProcessData method)", ex);
                                    }

                                    //--------------------------------------------------------------------
                                    //Mark date as processed
                                    //ADD implementation logic to avoid getting the same record again
                                    //--------------------------------------------------------------------
                                }

                                //Log Activity
                                instrumentation.LogActivity(outgoingMessage, ftpHost, 0);
                                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End processing: " + documentNumber);
                                i++;
                            }
                            catch (Exception ex)
                            {
                                string issueMsg = "An exception occurred while ...... Please review the exception log to determine the issue";
                                instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "Category", issueMsg, documentNumber);
                                Trace.WriteLine(tracingExceptionPrefix + "An exception occurred while processing data in BC.Integration.ProjectName.TemplateProject.SalesAggregation.Process.ProcessData. Exception message: " + ex.Message);
                                instrumentation.LogMessagingException("An exception occurred while processing a message in the " +
                                    "BC.Integration.AppService.ProjectName.TemplateProject.Process.ProcessData method. The current process is - " + processName +
                                    " document number being processed is " + documentNumber + ".", outgoingMessage, ex);
                            }
                        }//end date while loop
                    } //end if count 

                    //Log Activation End (This step will be skipped if an exception occurs)
                    instrumentation.LogActivationEnd(new Guid(activationGuid.Substring(0, 36)));
                    Trace.WriteLineIf(tracingEnabled, "TemplateProject: End processing.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the BC.Integration.AppService.ProjectName.TemplateProject.Process.ProcessData method. Exception message: " + ex.Message);
                    instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.AppService.ProjectName.TemplateProject.Process.ProcessData method.", ex);
                }
                finally
                {
                    instrumentation.FlushActivity();
                    container.Dispose();
                    integrator.CloseSession();
                }
            }
            catch (Exception ex)
            {
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the BC.Integration.AppService.ProjectName.TemplateProject.Process.ProcessData method trying to resolve the Unity DI components. Exception message: " + ex.Message);
                throw new Exception("An exception occurred in the BC.Integration.AppService.ProjectName.TemplateProject.Process.ProcessData method trying to resolve the Unity DI components.", ex);
            }
        }

        /// <summary>
        /// This method allows the pickup of files from a local folder.
        /// This method may not be applicable to a single service implementation as it 
        /// be going to a HIP persistence point to a destination persistence point.  In 
        /// the case of ShopperTrak we needed to aggregate data over a whole day.
        /// </summary>
        /// <param name="pickupMessageFolderPath">Folder path to pickup the files</param>
        /// <returns></returns>
        private string GetFileSourceData(string pickupMessageFolderPath)
        {
            string data = "";
            string path = pickupMessageFolderPath.Replace("#ProcessName#", processName);

            var fileNames = Directory.EnumerateFiles(path);
            foreach (string file in fileNames)
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Get file messages.  Filename: " + file);
                string fileText = File.ReadAllText(file);
                data += fileText;
                //Remove file after processing
                File.Delete(file);
            }

            return data;
        }

        /// <summary>
        /// Instantiates the objects that are used via dependency injection
        /// </summary>
        private void CreateDiComponents()
        {
            //Create Unity container for DI and create DI components
            container = new UnityContainer();
            container.LoadConfiguration();
            integrator = container.Resolve<IFtpIntegrator>();
            config = container.Resolve<IConfiguration>();
            configuration = config.PopulateConfigurationCollectionFromAppConfig();
            OverrideConfigProperties(config);
            instrumentation = container.Resolve<IInstrumentation>();
            instrumentation.Configuration = configuration;
        }

    }
}