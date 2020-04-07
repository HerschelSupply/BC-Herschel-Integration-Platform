using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using BC.Integration.Interfaces;
using BC.Integration.Utility;
using System.Configuration;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Diagnostics;
using System.Web;
using System.IO;
using BC.Integration.APICalls;

namespace BC.Integration.AppService.BC
{

    public sealed class ProcessSalesDoc : CodeActivity
    {
        // Define an activity input argument of type string
        public InArgument<string> ReceiveMsg { get; set; }

        #region Properties
        private string processName = ConfigurationManager.AppSettings["ProcessName"]; //Set to the value of the incoming message. 
        private string messageType = ConfigurationManager.AppSettings["ServiceId"]; //Set to the value of the incoming message.
        HipKeyValuePair[] filters = null; //Set to the value of the incoming message.
        //Key service Properties that should never change from design time
        private string serviceId = ConfigurationManager.AppSettings["ServiceId"];
        private Decimal serviceVersion = Convert.ToDecimal(ConfigurationManager.AppSettings["ServiceVersion"]);
        private string serviceOperationId = ConfigurationManager.AppSettings["ServiceOperationId"];
        private string servicePostOperationId = ConfigurationManager.AppSettings["ServicePostOperationId"];
        private string centralConfigConnString = ConfigurationManager.AppSettings["CentralConfigConnString"];
        private bool tracingEnabled = false;
        private IInstrumentation instrumentation = null;

        private string messageFormat = "xml";
        private decimal messageVersion = 1;
        private string outgoingMessageType = "";
        private string destinationSystemConnectivity = "";

        //Local processing properties
        MessageManager msgMgr = new MessageManager();
        private List<KeyValuePair<string, string>> configuration = null;

        public List<KeyValuePair<string, string>> Configuration { get => configuration; set => configuration = value; }
        #endregion

        #region Central Configuration methods
        /// <summary>
        /// Retrives the service and global central config values from the store.  The service properties are applied to the 
        /// service first, so global properties will override all properties.
        /// </summary>
        /// <param name="config">The configuration class to implement the retrieval of the central configuration</param>
        private void OverrideConfigProperties(IConfiguration config)
        {
            //ProcessName not know at this point so will need to be reloaded once message is recieved.
            configuration = config.GetConfiguration(serviceId, processName);
            PopulateLocalVaribale();
            instrumentation.Configuration = configuration;
        }

        /// <summary>
        /// Applies the central configurations to the loacal properties.
        /// </summary>
        /// <param name="configCol">The key value pair collection containing the configuration values.</param>
        private void PopulateLocalVaribale()
        {
            string val = Utilities.GetConfigurationValue(configuration, "TracingEnabled");
            if (val != "")
            {
                if (val.ToLower() == "true" || val.ToLower() == "false")
                    tracingEnabled = Convert.ToBoolean(val);
                if (val == "1" || val == "0")
                    tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
            }

        }
        #endregion

        // If your activity returns a value, derive from CodeActivity<TResult>
        // and return the value from the Execute method.
        protected override void Execute(CodeActivityContext context)
        {
            Debug.WriteLineIf(tracingEnabled, "Starting ProcessSalesDoc workflow activity Execute method...");
            // Obtain the runtime value of the Text input argument
            string receiveMsg = context.GetValue(this.ReceiveMsg);
            string msgID = "Message Unread";
            //string errorTrigger = "a";

            try
            {
                //Create Unity container for DI and create DI components
                UnityContainer container = new UnityContainer();
                container.LoadConfiguration();
                IConfiguration config = container.Resolve<IConfiguration>();
                configuration = config.PopulateConfigurationCollectionFromAppConfig();
                PopulateLocalVaribale();
                instrumentation = container.Resolve<IInstrumentation>();
                instrumentation.Configuration = configuration;
                Trace.WriteLineIf(tracingEnabled, "Unity container config complete...");

                XmlDocument msgXml = new XmlDocument();
                try
                {
                    //Log receipt of message
                   
                    msgXml.LoadXml(receiveMsg);
                    msgXml = msgMgr.CreateReceiveMessage(msgXml, serviceId, 1, serviceOperationId);
                    processName = msgMgr.ReceivedEnvelope.Interchange.ProcessName;
                    msgID = msgMgr.ReceivedEnvelope.Msg.Id.ToString();
                    //Reset configuration now that the process name is known.
                    //******
                    OverrideConfigProperties(config);
                    //******
                    //Log Message
                    instrumentation.LogActivity(msgXml);

                    //Map message to BC structure
                    string msgBody = msgMgr.ReceivedEnvelope.Body;
                    string BCMsg;
                    try
                    {
                        BCMsg = CreateBCMsg(msgBody);

                        API_Calls APICalls = new API_Calls();
                        APICalls.PostOrder(BCMsg);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error occurred when calling CreateBCMsg() to create the BC message.", ex);
                    }
                   

                    //Instrument OffRamp activity
                    msgXml = msgMgr.CreateOffRampMessage(servicePostOperationId, outgoingMessageType, messageVersion, messageFormat, BCMsg, null, null, "");
                    instrumentation.LogActivity(msgXml, destinationSystemConnectivity, 0);
                    Trace.WriteLineIf(tracingEnabled, "Output mapped BC message to file");

                }
                catch (Exception ex)
                {
                    string docId = "";
                    string exMessage = ex.Message;
                    Trace.WriteLineIf(tracingEnabled, "EXCEPTION Occurred: " + ex.Message);

                    if (msgXml.SelectSingleNode("/MsgEnvelope/Msg/DocId").InnerText != "")
                        docId = msgXml.SelectSingleNode("/MsgEnvelope/Msg/DocId").InnerText;

                    instrumentation.LogMessagingException("Error occurred in the BC.Integration.AppService.BC.ProcessSalesDoc.Execute method. " +
                        "The processing of the received message caused the component to fail and processing to stop. DocId:" + docId + " Message ID: " + msgID, msgXml, ex);

                    if (!string.IsNullOrEmpty(ex.InnerException.Message))
                    {
                        exMessage = exMessage + " InnerExceptionMessage: " + ex.InnerException.Message;
                    }

                    instrumentation.LogNotification(processName, serviceId, msgMgr.EntryPointEnvelope.Msg.Id, "PostIntoBC",
                    "DocumentId: " + docId + "  failed with the following error, " + exMessage, docId);


                    throw new Exception("An exception occurred in the BC.Integration.AppService.BC.ProcessSalesDoc Execute method. The processing of the received message caused the component to fail and processing to stop. Message ID: " + msgID, ex);
                }
                finally
                {
                    instrumentation.FlushActivity();
                    Debug.WriteLineIf(tracingEnabled, "Finally block called and Execute method complete.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("EXCEPTION Occurred: " + ex.Message);
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                throw new Exception("An exception occurred in the BC.Integration.AppService.BC.ProcessSalesDoc Execute method trying to resolve the Unity DI components.", ex);
            }
        }

        public string CreateBCMsg(string msgBody)
        {
            try
            {
                Trace.WriteLineIf(tracingEnabled, "Start creating BC message.");
                string output;
                var settings = new XsltSettings();
                settings.EnableScript = true;
                using (StringReader sReader = new StringReader(msgBody))
                using (XmlReader xReader = XmlReader.Create(sReader))
                using (StringWriter sWriter = new StringWriter())
                using (XmlWriter xWriter = XmlWriter.Create(sWriter))
                {
                    XslCompiledTransform xslt = new XslCompiledTransform();
                    xslt.Load(GetXsltMap(), settings, null);
                    xslt.Transform(xReader, xWriter);
                    output = sWriter.ToString();
                }
                Trace.WriteLineIf(tracingEnabled, "End creating BC message.");
                return output;
            } catch (Exception ex)
            {
                throw new Exception("An exception occured implementing the XSLT mapping in the BC.Integration.AppService.BC.ProcessSalesDoc.CreateBCMsg.", ex);
            }
        }

        private string GetXsltMap()
        {
            try
            {
                //Determine the configuration key for the current message
                string mapKey = "Map." + processName;
                //Is there a message subtype?
                HipKeyValuePairCollection processConfig = msgMgr.ReceivedEnvelope.Msg.ProcessKeyValuePairs;
                if (processConfig != null)
                    mapKey += processConfig.GetMappingSubtypes();
                Trace.WriteLineIf(tracingEnabled, "Mapping key: " + mapKey);
                //Get XSLT path from configuration using the determined key
                string val = Utilities.GetConfigurationValue(configuration, mapKey);
                Trace.WriteLineIf(tracingEnabled, "Mapping value: " + val);
                return val;
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured trying to resolve the XSLT map path in the BC.Integration.AppService.BC.ProcessSalesDoc.GetXsltMap.", ex);
            }
        }

      
    }

}
