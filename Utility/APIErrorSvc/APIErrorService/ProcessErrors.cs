using BC.Integration.Interfaces;
using BC.Integration.Utility;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using Microsoft.Dynamics.GP.eConnect;
using System.Web;
using System.Net;
using Newtonsoft.Json.Linq;
using BC.Integration.APICalls;

namespace BC.Integration.APIError
{
    public class ProcessErrors
    {

        private List<KeyValuePair<string, string>> configurationList = null;
        IConfiguration config;
        IInstrumentation instrumentation;
        UnityContainer container;

        //constants
        private const string DESTINATION = "BlueCherry";
        private const string NAMESPACE = "BC.Integration.APIError";

        //Local processing properties
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";

        //Authentication
        private string authKey;
        private string authValue;


        //service properties
        //Key service Properties that should never change from design time
        private string processName = ConfigurationManager.AppSettings["ProcessName"];
        private string serviceId = ConfigurationManager.AppSettings["ServiceId"];
        private string serviceOperationId = ConfigurationManager.AppSettings["ServiceOperationId"];
        private string servicePostOperationId = ConfigurationManager.AppSettings["ServicePostOperationId"];
        private string centralConfigConnString = ConfigurationManager.AppSettings["CentralConfigConnString"];
        private Decimal serviceVersion = Convert.ToDecimal(ConfigurationManager.AppSettings["ServiceVersion"]);
        //These rest of the properties could be overridden by the central config store at the service ot process level
        private string archiveFolder = "";
        private string exceptionMessageFolderPath = "";
        private string tracingMessageFolderPath = "";
        private string pickupConfirmationFolderPath = "";
        private string pickupCancellationFolderPath = "";
        //Msg Envelope properties
        private string msgType = "";
        private string messageType = ConfigurationManager.AppSettings["ServiceId"]; //Set to the value of the incoming message.
        private string filter = "";
        private string process = "";
        private string prefix = "";

        //Messaging Queue Properties
        private string queueUrl = "";

        private MessageManager msgMgr = new MessageManager();
        private string documentNumber;
        string exceptionMessage = "";
        private bool tracingEnabled = false;
        private string messageFormat = "eConnect";
        private int messageVersion = 1;

        private string econnectConnectionString = ConfigurationManager.AppSettings["eConnectConnectionString"];

        private void CreateDIComponents()
        {
            container = new UnityContainer();
            container.LoadConfiguration();
            config = container.Resolve<IConfiguration>();

            configurationList = config.PopulateConfigurationCollectionFromAppConfig(); //Populates the configuration collection with config data in the AppConfig file
            OverrideConfigProperties(config);
            instrumentation = container.Resolve<IInstrumentation>();
            instrumentation.Configuration = configurationList;
        }


        private void PopulateLocalVariabale()
        {
            archiveFolder = Utilities.GetConfigurationValue(configurationList, "ArchiveFolder");
            exceptionMessageFolderPath = Utilities.GetConfigurationValue(configurationList, "ExceptionMessageFolderPath");
            tracingMessageFolderPath = Utilities.GetConfigurationValue(configurationList, "TracingMessageFolderPath");
            pickupConfirmationFolderPath = Utilities.GetConfigurationValue(configurationList, "PickupConfirmationFolderPath");
            pickupCancellationFolderPath = Utilities.GetConfigurationValue(configurationList, "PickupCancellationFolderPath");
            msgType = Utilities.GetConfigurationValue(configurationList, "MsgType");
            filter = Utilities.GetConfigurationValue(configurationList, "Filter");
            process = Utilities.GetConfigurationValue(configurationList, "ProcessShipmentDoc");
            prefix = process + ": ";
            queueUrl = Utilities.GetConfigurationValue(configurationList, "QueueUrl");
            string val = Utilities.GetConfigurationValue(configurationList, "TracingEnabled");
            if (val != "")
            {
                if (val.ToLower() == "true" || val.ToLower() == "false")
                    tracingEnabled = Convert.ToBoolean(val);
                if (val == "1" || val == "0")
                    tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
            }

        }

        private void OverrideConfigProperties(IConfiguration config)
        {
            //This is an On-Ramp so we know process name from the beginning so no reset needed.
          //  configurationList = config.GetConfiguration(serviceId, processName);
          //  PopulateLocalVariabale();
        }
        

     
        private DateTime getDateTime(string dateString)
        {
            return DateTime.Parse(dateString.ToString());

        }
        public bool Execute()
        {
            Trace.WriteLineIf(tracingEnabled, prefix + " Starting ProcessSalesDoc workflow activity Execute method...");

            string msgID = "Message Unread";

            try
            {
                CreateDIComponents();

                Trace.WriteLineIf(tracingEnabled, prefix + " Unity container config complete...");

                try
                {

                    //Log reciept of message
                  
                    Trace.WriteLineIf(tracingEnabled, prefix + " Before Shipment initialization TEST version 1");

                    JArray errors = new JArray();
                    List<Error> errorList = new List<Error>();
                    API_Calls helper = new API_Calls();
                    errors = helper.GetAPIErrors();
                    if (errors.HasValues)
                    {
                      
                        foreach (JToken element in errors)
                        {
                            SalesOrderError error = new SalesOrderError();
                            error.DocumentId = element["ord_num"].ToString();
                            error.PickNumber = element["pick_num"].ToString();
                            error.Customer = element["customer"].ToString(); ;
                            error.Season = element["season"].ToString(); ;
                            error.ErrorMsg = element["errs_msg_h"].ToString();
                            error.Location = element["location"].ToString();
                            ErrorAdapter.InsertError(error);
                        }
                    }                    

                    OverrideConfigProperties(config);                                   
                 
                }
                catch (Exception ex)
                {
                    instrumentation.LogNotification(processName, serviceId, msgMgr.ReceivedEnvelope.Msg.Id, msgType, ex.Message, documentNumber);

                    Trace.WriteLineIf(tracingEnabled, prefix + " EXCEPTION Occurred: " + ex.Message);
                    instrumentation.LogGeneralException("Error occurred in the Corp.Integration.AppService.APIError.Execute method. " +
                       "The processing of the received message caused the component to fail and processing to stop. Message ID: " + msgID, ex);
                    return false;
                }
                finally
                {
                    instrumentation.FlushActivity();
                    Trace.WriteLineIf(tracingEnabled, prefix + " Finally block called and Execute method complete.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(prefix + " EXCEPTION Occurred: 33333" + ex.Message);
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                //This would be a configuration error so retrun an exception to calling service.
                throw new Exception("An exception occurred in the Corp.Integration.AppService.APIError Execute method trying to resolve the Unity DI components.  Please check the service configuration.", ex);
            }

            return true;
        }


    }
}
