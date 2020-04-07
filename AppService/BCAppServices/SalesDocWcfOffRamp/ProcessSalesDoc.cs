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
using System.Xml.Linq;
using BC.Integration.APICalls;

namespace BC.Integration.AppService.BC
{

    public class ProcessSalesDoc
    {
        #region Properties
        //Key service Properties that should never change from design time
        private string invoiceReturnBaozun = "InvoiceReturn"; //Set to the value of the incoming message. 
        private string invoiceReturnPOS = "LightspeedPosReturn"; //Set to the value of the incoming message. 
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
        private int messageVersion = 1;
        private string destinationSystemConnectivity = "";
        private string outgoingMessageType = "BC.Intergration.BC.SOPTransaction";
        string BCConnectionStringCAN = ConfigurationManager.AppSettings["BCConnectionStringCAN"];
        string BCConnectionStringUSA = ConfigurationManager.AppSettings["BCConnectionStringUSA"];
        string BCConnectionStringHSS = ConfigurationManager.AppSettings["BCConnectionStringHSS"];
        private string documentNumber;

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
        public bool Execute(string receiveMsg)
        {
            Trace.WriteLineIf(tracingEnabled, "BCSalesDocOffRamp: Starting ProcessSalesDoc workflow activity Execute method...");

            string msgID = "Message Unread";

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
                Trace.WriteLineIf(tracingEnabled, "BCSalesDocOffRamp: Unity container config complete...");

                XmlDocument msgXml = new XmlDocument();
                try
                {
                    //Log reciept of message
                   
                    msgXml.LoadXml(receiveMsg);
                    msgXml = msgMgr.CreateReceiveMessage(msgXml, serviceId, serviceVersion, serviceOperationId);
                    processName = msgMgr.ReceivedEnvelope.Interchange.ProcessName;
                    documentNumber = msgMgr.ReceivedEnvelope.Msg.DocumentId;
                    msgID = msgMgr.ReceivedEnvelope.Msg.Id.ToString();

                    //Reset configuration now that the process name is known.
                    OverrideConfigProperties(config);

                    //Log Message
                    instrumentation.LogActivity(msgXml);

                    //Map message to BC structure
                    string msgBody = msgMgr.ReceivedEnvelope.Body;
                    string BCMsg;
                    try
                    {
                        BCMsg = CreateBCMsg(msgBody);

                        API_Calls APICalls = new API_Calls();
                         APICalls.PostOrder(BCMsg, "Baozun order");  
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine( "BCAPPServices: Error occurred when calling Mapper.Convert() to create the outgoing message. Exception message: " + ex.Message);
                        throw new Exception("Error occurred when calling CreateBCMsg() to create the BC message.", ex);
                    }
                    //Instrument OffRamp activity
                    msgXml = msgMgr.CreateOffRampMessage(servicePostOperationId, outgoingMessageType, messageVersion, messageFormat, BCMsg, null, null, "");
                    instrumentation.LogActivity(msgXml, destinationSystemConnectivity, 0);
                    Trace.WriteLineIf(tracingEnabled, "BCAPPServices: Output message process complete.");
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

                        return false;
                    }
                finally
                {
                    instrumentation.FlushActivity();
                    Trace.WriteLineIf(tracingEnabled, "BCSalesDocOffRamp: Finally block called and Execute method complete.");
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BCSalesDocOffRamp: EXCEPTION Occurred: " + ex.Message);
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                //This would be a configuration error so retrun an exception to calling service.
                throw new Exception("An exception occurred in the BC.Integration.AppService.BC.ProcessSalesDoc Execute method trying to resolve the Unity DI components.  Please check the service configuration.", ex);
            }

            return true;
        }

        public string CreateBCMsg(string msgBody)
        {
            try
            {
                Trace.WriteLineIf(tracingEnabled, "BCSalesDocOffRamp: Start creating BC message.");
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
                Trace.WriteLineIf(tracingEnabled, "BCSalesDocOffRamp: End creating BC message.");
                return output;
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured implementing the XSLT mapping in the BC.Integration.AppService.BC.ProcessSalesDoc.CreateBCMsg.", ex);
            }
        }

        private string GetXsltMap()
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(msgMgr.ReceivedEnvelope.Body);
                XmlNodeList nodes = doc.DocumentElement.SelectNodes("//InvoiceType");
                XmlNodeList site = doc.DocumentElement.SelectNodes("//SiteId");
                string mapKey = "";

                string siteId = nodes[0].InnerText;

                //Determine the configuration key for the current message
                if (nodes.Count > 0 )
                {
                    if (processName == "LightspeedPosSalesCAN" || processName == "LightspeedPosSalesUSA")
                    {
                        if (nodes[0].InnerText == "Return")
                        {
                            mapKey = "Map." + invoiceReturnPOS + site[0].InnerText;
                        }
                        else
                        {
                            mapKey = "Map." + processName + site[0].InnerText;

                        }
                    }
                    else if (processName == "BaozunSales")
                    {

                        if (nodes[0].InnerText == "Return")
                        {
                            mapKey = "Map." + invoiceReturnBaozun;
                        }
                        else
                        {
                            mapKey = "Map." + processName;

                        }
                    }
                }

                HipKeyValuePairCollection processConfig = msgMgr.ReceivedEnvelope.Msg.ProcessKeyValuePairs;
                if (processConfig != null)
                {
                    mapKey += processConfig.GetMappingSubtypes();
                }
                Trace.WriteLineIf(tracingEnabled, "BCSalesDocOffRamp: Mapping key: " + mapKey);
                //Get XSLT path from configuration using the determined key
                string val = Utilities.GetConfigurationValue(configuration, mapKey);
                Trace.WriteLineIf(tracingEnabled, "BCSalesDocOffRamp: Mapping value: " + val);
                return val;
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured trying to resolve the XSLT map path in the BC.Integration.AppService.BC.ProcessSalesDoc.GetXsltMap.", ex);
            }
        }
    }

}
