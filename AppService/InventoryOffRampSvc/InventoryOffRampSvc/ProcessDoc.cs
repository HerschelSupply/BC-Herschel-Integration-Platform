﻿using System;
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
using Apache.NMS;
using Apache.NMS.Util;
using Newtonsoft.Json.Linq;


namespace BC.Integration.AppService.InventoryOffRampSvc
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

        private string processName = ""; //Set to the value of the incoming message. 
        private string messageType = ""; //Set to the value of the incoming message.
        HipKeyValuePair[] filters = null; //Set to the value of the incoming message.
        //Key service Properties that should never change from design time
        private string serviceId = ConfigurationManager.AppSettings["ServiceId"];
        private Decimal serviceVersion = Convert.ToDecimal(ConfigurationManager.AppSettings["ServiceVersion"]);
        private string serviceOperationId = ConfigurationManager.AppSettings["ServiceOperationId"];
        private string servicePostOperationId = ConfigurationManager.AppSettings["ServicePostOperationId"];
        private string centralConfigConnString = ConfigurationManager.AppSettings["CentralConfigConnString"];
        private string messageFormat = "xml";
        private decimal messageVersion = 1;
        private string outgoingMessageType = ConfigurationManager.AppSettings["OutgoingMessageType"];
        private string destinationUrl;
        private string queueName;

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
            //ProcessName not know at this point so will need to be reloaded once message is received.
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
            destinationUrl = Utilities.GetConfigurationValue(configuration, "DestinationUrl");
            queueName = Utilities.GetConfigurationValue(configuration, "QueueName");

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
        }
        #endregion


        /// <summary> 
        /// If your activity returns a value, derive from CodeActivity<TResult>
        /// and return the value from the Execute method.
        /// </summary>
        /// <param name="receiveMsg"></param>
        /// <returns></returns>
        public bool Execute(string receiveMsg)
        {
            Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Starting ProcessSalesDoc workflow activity Execute method...");
            
            string msgID = "Message Unread";

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

                    //Reset configuration now that the process name is known.
                    OverrideConfigProperties(config);

                    //Log Message
                    instrumentation.LogActivity(msgXml);

                    
                    string msgBody = msgMgr.ReceivedEnvelope.Body;
                    string outgoingMessage="";

                    outgoingMessage = HttpUtility.HtmlDecode(msgBody);


                    try
                    {
                        // create the Ep Message
                        Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Creating EP message...");

                        outgoingMessage =  ConvertToEPMessage(outgoingMessage);

                        Trace.WriteLineIf(tracingEnabled, tracingPrefix + "End EP message creation...");

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(tracingPrefix + "Error occurred when calling Mapper.ConvertShipmentConfirm() to create the outgoing message.", ex);
                    }

                    //******************************Implement Destination Code*********************************************

                    string message = outgoingMessage;
                    WarehouseSettings wsSettings = Mapper.GetWarehouseCodes(msgBody);

                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Send message to ActiveMQ ( " + queueName + " )...");
                    Uri connecturi = new Uri(destinationUrl);
                    IConnectionFactory factory = new Apache.NMS.ActiveMQ.ConnectionFactory(connecturi);
                    IConnection connection = factory.CreateConnection();
                    ISession session = connection.CreateSession();
                    IDestination destination = SessionUtil.GetDestination(session, queueName);
                    try
                    {
                        using (IMessageProducer producer = session.CreateProducer(destination))
                        {
                            // Start the connection so that messages will be processed.
                            connection.Start();
                            producer.DeliveryMode = MsgDeliveryMode.Persistent;

                            // Send a message
                            ITextMessage request = session.CreateTextMessage(message);
                            request.Properties.SetString("warehouseUid", wsSettings.warehouseUid);
                            request.Properties.SetString("warehouseCode", wsSettings.warehouseCode);
                            producer.Send(request);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw new Exception(tracingPrefix + "Error occurred in the BC.Integration.AppService.InventoryOffRampSvc.ProcessData.Execute method, when sending the outgoing message to the ActiveMQ.  Exception message: " + ex.Message, ex);
                    }
                    finally
                    {
                        session.Close();
                        session.Dispose();
                        destination.Dispose();
                        connection.Close();
                        connection.Dispose();
                    }
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Finished sending message to ActiveMQ ( " + queueName + " )...");


                    //**************************END Destination Code*********************************************

                    //Instrument OffRamp activity
                    msgXml = msgMgr.CreateOffRampMessage(servicePostOperationId, outgoingMessageType, messageVersion, messageFormat, outgoingMessage, null, null, ""); 
                    instrumentation.LogActivity(msgXml, destinationUrl, 0);
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "Output message process complete.");

                                     
                    
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "EXCEPTION Occurred: " + ex.Message);
                    instrumentation.LogGeneralException(tracingPrefix + "Error occurred in the BC.Integration.AppService.InventoryOffRampSvc.ProcessData.Execute (EP) method. " +
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
                Trace.WriteLineIf(tracingEnabled,tracingExceptionPrefix + " Occurred: " + ex.Message);
                //Since the implementation of DI raised the exception we can not log the exception using the instrumentation component.
                //This would be a configuration error so retrun an exception to calling service.
                throw new Exception(tracingPrefix + "An exception occurred in the BC.Integration.AppService.TemplateService.ProcessDoc.Execute method trying to resolve the Unity DI components.  Please check the service configuration.", ex);
            }

            return true;
        }


        /// <summary>
        /// Converts Canonical shipping confirmation into a EP shipping confirmation message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>EP shipping confirmation</returns>
        public  string ConvertToEPMessage(string message)
        {
            string epMessage = "itemnmbr,qtyonhand\r\n";
                       
                JArray data = JArray.Parse(message);

              foreach (var item in data)
              {
                try
                {
                    if (item.Value<string>("location") == null || item.Value<string>("product_id") == null || item.Value<string>("qoh") == null)
                    {
                        throw new Exception("Location, product_id and qoh are required fields. Current values: Location=" + item.Value<string>("location") + ", ProductId=" + item.Value<string>("product_id") + ", QOH=" + item.Value<string>("qoh"));
                       
                    }
                    else
                    {
                        epMessage += item.Value<string>("product_id") + "," + item.Value<string>("qoh") + "\r\n";
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(tracingEnabled, tracingPrefix + "EXCEPTION Occurred: " + ex.Message);
                    instrumentation.LogGeneralException(tracingPrefix + "Error occurred in the BC.Integration.AppService.InventoryOffRampSvc.ConvertToEPMessage method. "
                     , ex);
                }
              }

           

            return epMessage;


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
