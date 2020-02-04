using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using BC.Integration.APICalls;
using System.Xml.Linq;

namespace BC.Integration.AppService.ShippingOffRampBC
{

    public class ProcessDoc
    {
        #region Properties
        private List<KeyValuePair<string, string>> configuration = null;
        MessageManager msgMgr = new MessageManager();
        private bool tracingEnabled = false;

        //Dependency Injection Components
        IConfiguration config;
        IInstrumentation instrumentation;
        IPublishService publishService;
        UnityContainer container;

        private string processName = ConfigurationManager.AppSettings["ProcessName"]; //Set to the value of the incoming message. 
        private string messageType = ConfigurationManager.AppSettings["ServiceId"]; //Set to the value of the incoming message.
        HipKeyValuePair[] filters = null; //Set to the value of the incoming message.
        //Key service Properties that should never change from design time
        private string serviceId = ConfigurationManager.AppSettings["ServiceId"];
        private Decimal serviceVersion = Convert.ToDecimal(ConfigurationManager.AppSettings["ServiceVersion"]);
        private string serviceOperationId = ConfigurationManager.AppSettings["ServiceOperationId"];
        private string servicePostOperationId = ConfigurationManager.AppSettings["ServicePostOperationId"];
        private string centralConfigConnString = ConfigurationManager.AppSettings["CentralConfigConnString"];
        private string messageFormat = "xml";
        private decimal messageVersion = 1;
        private string outgoingMessageType = "";
        private string destinationSystemConnectivity = "";
        //Local processing properties
        public List<KeyValuePair<string, string>> Configuration { get => configuration; set => configuration = value; }
        private string tracingPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ": ";
        private string tracingExceptionPrefix = ConfigurationManager.AppSettings["TracingPrefix"] + ".EXCEPTION : ";
        

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
            string destinationSystemConnectivity = Utilities.GetConfigurationValue(configuration, "DestinationSystemConnectivity");
            string outgoingMessageType = Utilities.GetConfigurationValue(configuration, "OutgoingMessageType");
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
        public bool Execute(string receiveMsg)
        {
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Starting ProcessSalesDoc workflow activity Execute method...");
            
            string msgID = "Message Unread";
            //string errorTrigger = "a";

            try
            {
                //Create Unity container for DI and create DI components
                CreateDiComponents();
                try
                {
                    //Log receipt of message
                    XmlDocument msgXml = new XmlDocument();
                    msgXml.LoadXml(receiveMsg);
                    msgXml = msgMgr.CreateReceiveMessage(msgXml, serviceId, serviceVersion, serviceOperationId);
                    processName = msgMgr.ReceivedEnvelope.Interchange.ProcessName;
                    msgID = msgMgr.ReceivedEnvelope.Msg.Id.ToString();
                    messageVersion = msgMgr.ReceivedEnvelope.Msg.Version;
                    //Reset configuration now that the process name is known.
                    OverrideConfigProperties(config);
                    //Log Message
                    instrumentation.LogActivity(msgXml);

                    //Map message to BC structure
                    string msgBody = msgMgr.ReceivedEnvelope.Body;
                    string outgoingMessage;
                    try
                    {
                        //Map to the canonical structure
                        //Alternative is to use a XSLT to map 
                        //message to the destination message type.
                        //XSLT is the prefered option as it adds 
                        //additional flexibility.

                        outgoingMessage = CreateBCMsg(msgBody);

                        //post the message to BC
                        API_Calls.PostShipmentConfirmation(outgoingMessage);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(tracingExceptionPrefix + "Occurred when calling Mapper.Convert() to create the outgoing message. Exception message: " + ex.Message);
                        throw new Exception("Error occurred when calling Mapper.Convert() to create the outgoing message.", ex);
                    }

                   

                    //Instrument OffRamp activity
                    msgXml = msgMgr.CreateOffRampMessage(servicePostOperationId, outgoingMessageType, messageVersion, messageFormat, outgoingMessage, null, null, ""); 
                    instrumentation.LogActivity(msgXml, destinationSystemConnectivity, 0);
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Output message process complete.");
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(tracingExceptionPrefix + "Occurred: " + ex.Message);
                    instrumentation.LogGeneralException("Error occurred in the BC.Integration.AppService.ShippingOffRampBC.ProcessDoc.Execute method. " +
                        "The processing of the received message caused the component to fail and processing to stop. Message ID: " + msgID, ex);
                     return false;
                }
                finally
                {
                    instrumentation.FlushActivity();
                    Debug.WriteLineIf(tracingEnabled, tracingPrefix + "Finally block called and Execute method complete.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine(tracingExceptionPrefix + "Occurred: " + ex.Message);
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                //This would be a configuration error so retrun an exception to calling service.
                throw new Exception("An exception occurred in the BC.Integration.AppService.ShippingOffRampBC.ProcessDoc.Execute method trying to resolve the Unity DI components.  Please check the service configuration.", ex);
            }

            return true;
        }

        public string CreateBCMsg(string msgBody)
        {
            try
            {
                XmlWriterSettings wSettings = new XmlWriterSettings();
                wSettings.OmitXmlDeclaration = true;

                Trace.WriteLineIf(tracingEnabled, "Start creating the BC message.");
                string output;
                var settings = new XsltSettings();
                settings.EnableScript = true;
                using (StringReader sReader = new StringReader(msgBody))
                using (XmlReader xReader = XmlReader.Create(sReader))
                using (StringWriter sWriter = new StringWriter())
                using (XmlWriter xWriter = XmlWriter.Create(sWriter, wSettings))
                {
                    XslCompiledTransform xslt = new XslCompiledTransform();
                    xslt.Load(GetXsltMap(), settings, null);
                    xslt.Transform(xReader, xWriter);
                    output = sWriter.ToString();
                }
                Trace.WriteLineIf(tracingEnabled, "End creating the BC message.");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured implementing the XSLT mapping in the BC.Integration.AppService.ShippingOffRampBC.", ex);
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
                Trace.WriteLine(tracingPrefix + "An exception occured trying to resolve the XSLT map path in the BC.Integration.AppService.ShippingOffRampBC.ProcessDoc.GetXsltMap. Exception message: " + ex.Message);
                throw new Exception("An exception occured trying to resolve the XSLT map path in the BC.Integration.AppService.ShippingOffRampBC.ProcessDoc.GetXsltMap.", ex);
            }
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
            PopulateLocalVaribale();
            instrumentation = container.Resolve<IInstrumentation>();
            instrumentation.Configuration = configuration;
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Unity container config complete...");
        }



    }

}
