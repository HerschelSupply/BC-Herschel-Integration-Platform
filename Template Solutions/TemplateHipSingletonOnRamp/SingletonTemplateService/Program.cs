using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Configuration;
using BC.Integration.Interfaces;
using BC.Integration.Utility;
using System.Threading;
using System.Xml;
using System.ServiceModel;
using System.Diagnostics;
using System.Web;
using System.IO;

namespace BC.Integration.AppService.SingletonTemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
            string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";

            if (args.Length != 1)
                throw new Exception("The SingletonTemplate app service takes an activation GUID string as the only argument.");

            string guid = args[0];
            Trace.WriteLine(tracingPrefix + "SingletonTemplate Service executable initiated.  Starting Process Controler.");

            try
            {
                SingletonTemplateService svc = new SingletonTemplateService();
                svc.ProcessController(guid);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + " Occurred whilst starting the Process Controler.");
                throw new Exception("An error occurred trying to initialize the SingletonTemplate service Process Controler. Please review the inner exception for details of the error.", ex);
            }
        }
    }


    public class SingletonTemplateService
    {
        #region Properties
        private static Mutex mutex = null; //Used to block multiple istances working on the same queue

        private List<KeyValuePair<string, string>> configuration = null;

        //Dependency Injection Components
        IConfiguration config;
        IInstrumentation instrumentation;
        IPublishService publishService;
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
        private string msgType = "";
        private string filterKeyValuePairs = "";
        private string processKeyValuePairs = "";
        private string topic = "";

        //Messaging Queue Properties
        private string queueUrl = "";

        private string batchData = "";
        private MessageManager msgMgr = new MessageManager();
        private string batchName = "";
        private string documentNumber;
        private XmlDocument outgoingMessage;
        private string messageFormat = "xml";
        private int serviceVersion = 1;
        private int messageVersion = 1;
        private bool tracingEnabled = false;
        private bool localFileSource = false;
        private string exceptionMessageFolderPath = "";
        private string tracingMessageFolderPath = "";
        private string pickupFileFolderPath = "";


        #endregion

        #region Central Configuration methods
        /// <summary>
        /// Retrives the service and global central config values from the store.  The service properties are applied to the 
        /// service first, so global properties will override all properties.
        /// </summary>
        /// <param name="config">The configuration class to implement the retrieval of the central configuration</param>
        private void OverrideConfigProperties(IConfiguration config)
        {
            configuration = config.GetConfiguration(serviceId, processName);
            PopulateLocalVaribale();
            instrumentation.Configuration = configuration;
        }

        /// <summary>
        /// Applies the central configurations to the local variables.
        /// </summary>
        private void PopulateLocalVaribale()
        {
            archiveFolder = Utilities.GetConfigurationValue(configuration, "ArchiveFolder");
            exceptionMessageFolderPath = Utilities.GetConfigurationValue(configuration, "ExceptionMessageFolderPath");
            tracingMessageFolderPath = Utilities.GetConfigurationValue(configuration, "TracingMessageFolderPath");
            pickupFileFolderPath = Utilities.GetConfigurationValue(configuration, "PickupFileFolderPath");
            msgType = Utilities.GetConfigurationValue(configuration, "MsgType");
            filterKeyValuePairs = Utilities.GetConfigurationValue(configuration, "FilterKeyValuePairs");
            processKeyValuePairs = Utilities.GetConfigurationValue(configuration, "ProcessKeyValuePairs");
            topic = Utilities.GetConfigurationValue(configuration, "Topic");
            queueUrl = Utilities.GetConfigurationValue(configuration, "QueueUrl");

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

        public void ProcessController(string guid)
        {
            try
            {
                CreateDiComponents();
                try
                {
                    //Ensure that this application cannot be started twice to process the same queue.
                    string appName = serviceId;
                    bool createNew;
                    mutex = new Mutex(true, appName, out createNew);
                    if (!createNew)
                    {
                        instrumentation.LogActivation(serviceId, new Guid(guid), true);
                        Trace.WriteLine(tracingPrefix + "SingletonTemplateService already running. This instance will terminate.");
                        return; //terminates ProcessController method and ends service activation.
                    }
                    Trace.WriteLine(tracingPrefix + "SingletonTemplateService starting. ");

                    //Log Activation
                    instrumentation.LogActivation(serviceId, new Guid(guid), false);

                    ProcessData(guid);

                    //Log Activation End (This step will be skipped if an exception occurs)
                    instrumentation.LogActivationEnd(new Guid(guid));
                }
                catch (Exception ex)
                {
                    instrumentation.LogGeneralException("Error occured in the BC.Integration.AppService.SingletonTemplate.ProcessController method " +
                        "processing. Please review inner exceptions for details: ", ex);
                }
                finally
                {
                    mutex.ReleaseMutex();
                    mutex.Close();
                    mutex.Dispose();
                }

            }
            catch (Exception ex)
            {
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                throw new Exception("An exception occured in the BC.Integration.AppService.SingletonTemplate ProcessController method trying to resolve the Unity DI components.", ex);
            }
        }



        /// <summary>
        /// Process
        /// </summary>
        private void ProcessData(string activationGuid)
        {
            try
            {
                //Log Activation Start
                instrumentation.LogActivation(serviceId, new Guid(activationGuid.Substring(0, 36)), false);

                int msgCount = 0;
                string data;
                //_____________________________________________________________________________________________
                //Add code to get data from source system.  This will usally be a collection of messages.

                //Get data from Source System or Local File Pickup
                if (!localFileSource)
                {
                    //Get data from source system
                    //batchName = Filename;
                }
                else
                {
                    //Get local file data
                    data = GetFileSourceData(pickupFileFolderPath);
                    batchName = "LocalFile";
                }


                //Remember to... Set 'batchName' for instrumentation.
                //______________________________________________________________________________________________

                //Process message collection if it contains one or more messages
                if (msgCount > 0)
                {
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Data retrieved.");
                    //Create initial msg envelope to represent the start of the service process and call Instrumenation
                    XmlDocument msg = msgMgr.CreateOnRampMessage(processName, serviceId, serviceVersion, serviceOperationId, msgType, messageVersion, messageFormat, batchName);
                    //instrumentation.WriteMsgToFile(archiveFolder, batchFilename, msg, "SalesDoc", "OnRamp");
                    instrumentation.LogActivity(msg);
                    //Log Activation Association with Interchange
                    instrumentation.AssociateActivationWithInterchages(new Guid(activationGuid.Substring(0, 36)), msgMgr.EntryPointEnvelope.Interchange.InterchangeId);

                    //______________________________________________________________________________________________
                    //Add code to split the data to create a collection of messages to post onto the message bus.
                    List<string> messages = new List<string>();


                    //______________________________________________________________________________________________

                    int i = 1;
                    foreach (string message in messages)
                    {
                        try
                        {
                            documentNumber = ""; //Set document number to support tracking and troubleshooting.
                            Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Processing message collection.  Document Number: " + documentNumber);

                            //Map to the canonical structure
                            string canonicalMsg = Mapper.Convert(message);

                            //Create envelope and add canonical message to the envelope
                            HipKeyValuePairCollection filterCol = new HipKeyValuePairCollection(filterKeyValuePairs);
                            HipKeyValuePairCollection processCol = new HipKeyValuePairCollection(processKeyValuePairs);
                            outgoingMessage = msgMgr.CreatePostMessage(servicePostOperationId, msgType, messageVersion, messageFormat, topic, filterCol, processCol, canonicalMsg, i, null, documentNumber);

                            //Place message on the message bus
                            int retryCount = 0;
                            publishService.Configuration = configuration;
                            publishService.Push(queueUrl, outgoingMessage.InnerXml, out retryCount);

                            //Remove or mark data as processed
                            if (!localFileSource)
                            {
                                //clear source data
                            }

                            //Log Activity
                            instrumentation.LogActivity(outgoingMessage, queueUrl, retryCount);
                            Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End processing: " + documentNumber);
                            i++;
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteLine(tracingExceptionPrefix + "An exception occurred while processing data in BC.Integration.AppService.TemplateHipOnRamp.Process.ProcessData. Exception message: " + ex.Message);
                            instrumentation.LogMessagingException("An exception occured while processing a message in the " +
                                "BC.Integration.AppService.TemplateHipOnRamp.Process.ProcessData method. The current process is - " + processName +
                                " document number being processed is " + documentNumber + ".", outgoingMessage, ex);
                        }
                    }

                } //end while loop

                //Log Activation End (This step will be skipped if an exception occurs)
                instrumentation.LogActivationEnd(new Guid(activationGuid.Substring(0, 36)));
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End processing data.");
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + "An exception was raised when calling the BC.Integration.AppService.TemplateHipOnRamp.Process.ProcessData method. Exception message: " + ex.Message);
                instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.AppService.TemplateHipOnRamp.Process.ProcessData method.", ex);
            }
            finally
            {
                instrumentation.FlushActivity();
                container.Dispose();
            }
        }

        /// <summary>
        /// This method allows the pickup of files from a local folder.
        /// </summary>
        /// <param name="pickupMessageFolderPath">Folder path to pickup the files</param>
        /// <returns></returns>
        private string GetFileSourceData(string pickupMessageFolderPath)
        {
            //This is setup for JSON data...
            string data = "[";

            string path = pickupMessageFolderPath.Replace("#ProcessName#", processName);

            var fileNames = Directory.EnumerateFiles(path);
            foreach (string file in fileNames)
            {
                Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Get file messages.  Filename: " + file);
                string fileText = File.ReadAllText(file);
                data += fileText.Substring(fileText.IndexOf("{"), fileText.LastIndexOf("}") - fileText.IndexOf("{") + 1) + ",";
                //Remove file after processing
                File.Delete(file);
            }
            data = data.TrimEnd(',') + "]";
            if (data == "[]")
                data = "";

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
            config = container.Resolve<IConfiguration>();
            configuration = config.PopulateConfigurationCollectionFromAppConfig();
            OverrideConfigProperties(config);
            instrumentation = container.Resolve<IInstrumentation>();
            instrumentation.Configuration = configuration;
            publishService = container.Resolve<IPublishService>();
        }


    }
}
