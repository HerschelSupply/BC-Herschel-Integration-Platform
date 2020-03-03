using System;
using System.Collections.Generic;
using Microsoft.Practices.Unity;
using System.Configuration;
using Amazon.SQS;
using BC.Integration.Interfaces;
using BC.Integration;
using Amazon.SQS;
using Amazon.SQS.Model;
using System.Threading;
using System.Xml;
using System.ServiceModel;
using System.Diagnostics;
using System.Web;
using Microsoft.Practices.Unity.Configuration;
using Amazon.SQS.Model;

namespace BC.Integration.Utility
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 4)
                throw new Exception("Please pass atleast 4 arguments 'guid queueName maxRate isBatch maxBatchSize'. Use '0' for Max Rate if no throttling is needed. If isBatch = false there is no need to pass the Max Batch Size.");

            string guid = args[0];
            string queueName = args[1];
            decimal maxRate = Convert.ToDecimal(args[2]);
            bool isBatch = Convert.ToBoolean(args[3]);
            int maxBatchSize = 0;
            if (isBatch)
            {
                if (args.Length != 4)
                    throw new Exception("When isBatch is 'true', please pass all 4 arguments 'queueName maxRate isBatch maxBatchSize'. Use '0' for Max Rate if no throttling is needed. If isBatch = false there is no need to pass the Max Batch Size.");
                maxBatchSize = Convert.ToInt16(args[4]);
            }
            //string guid = Guid.NewGuid().ToString();

            Trace.WriteLine("Push Service: Call Service from client. Max Rate '" + maxRate + "', Queue Name: '" + queueName +
                                "', Is Batch: '" + isBatch + ", Max Batch Size: '" + maxBatchSize + "'.");

            try
            {
                SqsPushService svc = new SqsPushService();
                svc.ProcessController(guid, queueName, maxRate, isBatch, maxBatchSize);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Push Service: Exception occured whilst running the HIP Push Initializer.");
                throw new Exception("An error occured trying to call the Push service. Please review the inner exception for details of the error.", ex);
            }

        }
    }

    /// <summary>
    /// Push Service throttling options on a queue bases:
    /// 1) Interval between messages.
    /// 2) Process the queue between specific times of the day
    /// 3) Batch all the messages currently in the queue upto a max number.
    /// or any combination of the options
    /// 
    /// Initially this component will not be multi threaded but this could be done as a phase 2 when the balance
    /// between overall server load is low but the processing of messages in the queue is a bottle neck.  A thread 
    /// should be created for each queue.
    /// </summary>

    public class SqsPushService
    {
        #region Properties
        private static Mutex mutex = null; //Used to block multiple istances working on the same queue

        private IConfiguration config;
        private IInstrumentation instrumentation;
        private string processName = ""; //Set to the value of the incoming message. 
        private string messageType = ""; //Set to the value of the incoming message.
        HipKeyValuePairCollection filters = null; //Set to the value of the incoming message.
        //Key service Properties that should never change from design time
        private string serviceId = ConfigurationManager.AppSettings["ServiceId"];
        private Decimal serviceVersion = Convert.ToDecimal(ConfigurationManager.AppSettings["ServiceVersion"]);
        private string serviceOperationId = ConfigurationManager.AppSettings["ServiceOperationId"];
        private string servicePostOperationId = ConfigurationManager.AppSettings["ServicePostOperationId"];
        private string centralConfigConnString = ConfigurationManager.AppSettings["CentralConfigConnString"];

        private List<KeyValuePair<string, string>> configuration = null;
        private List<string> queues = new List<string>();
        private MessageManager msgMgr = new MessageManager();
        private Exception retryExceptions = null;
        private int retryCount = 0;
        private int serviceRetryCount = 0;
        private int serviceRetryInterval = 0;
        private bool tracingEnabled = false;

        private bool isDestActivationSuccessful = true; //Flag to determine if send instrumentation point should be logged.

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
            configuration = config.GetConfiguration(serviceId, processName);
            PopulateLocalVaribale();
            instrumentation.Configuration = configuration;
        }

        /// <summary>
        /// Applies the central configurations to the local variables.
        /// </summary>
        private void PopulateLocalVaribale()
        {
            serviceRetryCount = Convert.ToInt16(Utilities.GetConfigurationValue(configuration, "ServiceRetryCount"));
            serviceRetryInterval = Convert.ToInt16(Utilities.GetConfigurationValue(configuration, "ServiceRetryInterval"));
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

        public void ProcessController(string guid, string queueName, decimal maxRate, bool isBatch, int maxBatchSize)
        {
            try
            {
                //Create Unity container for DI and create DI components
                UnityContainer container = new UnityContainer();
                container.LoadConfiguration();
                config = container.Resolve<IConfiguration>();
                configuration = config.PopulateConfigurationCollectionFromAppConfig();
                PopulateLocalVaribale();
                instrumentation = container.Resolve<IInstrumentation>();
                instrumentation.Configuration = configuration;

                try
                {
                    //Ensure that this application cannot be started twice to process the same queue.
                    string appName = queueName.Substring(queueName.LastIndexOf('/') + 1);
                    bool createNew;
                    mutex = new Mutex(true, appName, out createNew);
                    if (!createNew)
                    {
                        instrumentation.LogActivation(serviceId + " (" + queueName.Substring(queueName.LastIndexOf('/') + 1) + ")", new Guid(guid), true);
                        Trace.WriteLine("Push Service: SqsPushService already running. This instance will terminate.");
                        return; //terminates ProcessController method and ends service activation.
                    }
                    Trace.WriteLine("Push Service: SqsPushService starting. ");
                    
                    //Log Activation
                    instrumentation.LogActivation(serviceId + " (" + queueName.Substring(queueName.LastIndexOf('/') + 1) + ")", new Guid(guid), false);

                    ProcessQueue(guid, queueName, maxRate, isBatch, maxBatchSize);

                    //Log Activation End (This step will be skipped if an exception occurs)
                    instrumentation.LogActivationEnd(new Guid(guid));
                }
                catch (Exception ex)
                {
                    instrumentation.LogGeneralException("Error occured in the BC.Integration.Utility.SqsPushService.ProcessController method " +
                        "processing messages from the SQS queue that caused the whole component to fail and all message processing to stop.", ex);
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
                throw new Exception("An exception occured in the BC.Integration.Utility.SqsPushService ProcessController method trying to resolve the Unity DI components.", ex);
            }
        }


        /// <summary>
        /// This method is responsible for processing the SQS queue specified in the parameter list.
        /// </summary>
        private void ProcessQueue(string guid, string queueName, decimal maxRate, bool isBatch, int maxBatchSize)
        {
            if (isBatch)
                throw new NotImplementedException("The batching of messages from the message queue has not been implemented yet.");

            bool isQueueEmpty = false;
            string msgQueueID = ""; //Used for exception msg.
            while(!isQueueEmpty)
            {
                try
                {
                    Trace.WriteLineIf(tracingEnabled, "Push Service: Get message from SQS queue.  Queue:" + queueName);
                    QueuedMessage msg = GetMessage(queueName);
                    if (msg.Body == "")
                    {
                        isQueueEmpty = true;
                        break;
                    }
                    msgQueueID = msg.Id;  //SQS Message ID Not HIP Message ID
                    //Get message
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(msg.Body);
                    doc = msgMgr.CreateReceiveMessage(doc, serviceId, serviceVersion, serviceOperationId);
                    messageType = msgMgr.ReceivedEnvelope.Msg.Type;
                    filters = msgMgr.ReceivedEnvelope.Msg.FilterKeyValuePairs;
                    processName = msgMgr.ReceivedEnvelope.Interchange.ProcessName;
                    //Reset configuration now that the process name is known.
                    //******
                    OverrideConfigProperties(config);
                    //******
                    //Log Message
                    //instrumentation.WriteMsgToFile(@"C:\Temp\FTP Test", "SqsOutput", doc, "XML", "Pull"); //Temp code
                    instrumentation.LogActivity(doc);

                    //Log Activation Association with Interchange
                    if (tracingEnabled) //Only track this if tracing is enabled.  This is due to the performance impact of making the ODBC connections
                        instrumentation.AssociateActivationWithInterchages(new Guid(guid), msgMgr.ReceivedEnvelope.Interchange.InterchangeId);

                    //Ver 2
                    //Update the code to include SQS queue name
                    string q = queueName.Substring(queueName.LastIndexOf('/') + 1);
                    //Route message to destination service
                    string destinationUrl = config.GetDestinationUrl(q, processName, serviceId, servicePostOperationId, messageType, filters.ConvertToKeyValuePairs());
                    //string destinationUrl = config.GetDestinationUrl(processName, serviceId, servicePostOperationId, messageType, filters.ConvertToKeyValuePairs());

                    //Update Envelope **Since this is a passthrough service use nulls to keep the values in the recieved message.**
                    doc = msgMgr.CreatePostMessage(servicePostOperationId, null, 0, null, null, null, null, null, null, null, "");

                    //Write code to post message to next service...
                    //Call IIS service
                    Trace.WriteLineIf(tracingEnabled, "Push Service: Pass message to destination service.  URL: " + destinationUrl);
                    bool success = false;
                    retryCount = 0; //reset count between post operations
                    retryExceptions = new Exception();
                    success = Retry.Execute(() => PostMsgToService(destinationUrl, doc), new TimeSpan(0, 0, serviceRetryInterval), serviceRetryCount, true);


                    if (!success)
                    {
                        //Log the failure to send but do not block other messages form being processed.
                        instrumentation.LogMessagingException("An exception was raised calling the BC.Integration.Utility.SqsPushService.ProcessQueues method." +
                            "Failed Pass message to destination service.  The message Document ID was " + msgMgr.ReceivedEnvelope.Msg.DocumentId + ".  URL: " + destinationUrl + " after " + serviceRetryCount + " retries with an interval of " + serviceRetryInterval + " seconds.", doc, retryExceptions);
                        Trace.WriteLineIf(tracingEnabled, "Push Service: Failed to pass message to destination service.  URL: " + destinationUrl + ". Exception message: " + retryExceptions.Message);
                    }

                    if (success)
                    {
                        Trace.WriteLineIf(tracingEnabled, "Push Service: Remove message from SQS queue.");
                        RemoveMessageFromQueue(msg.MessageRecieptHandle, queueName);//Delete message off queue
                    }

                    if(isDestActivationSuccessful)
                    {// Instrumentation should log activity if activation was successful.
                        instrumentation.LogActivity(doc, destinationUrl, retryCount);
                    }

                    isQueueEmpty = msg.QueueEmpty;
                    //To determine delay we need to get the maxRate and divide by the number of servers
                    //Convert to seconds per message
                    if (maxRate != 0) //Do not throttle if maxRate = 0
                    {
                        decimal secPerMsg = 1 / maxRate;
                        //Correct for server count
                        decimal delayInSec = secPerMsg * config.AvailableServerCount();
                        decimal delayInMilliseconds = delayInSec * 1000;
                        Trace.WriteLineIf(tracingEnabled, "Push Service: THROTTLING -Push service processing delay in seconds: " + delayInSec);
                        Thread.Sleep(Convert.ToInt32(delayInMilliseconds));
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLineIf(tracingEnabled, "Push Service: Logging exception that caused the system to fail.");
                    instrumentation.LogGeneralException("Error occured in the BC.Integration.Utility.SqsPushService.ProcessQueues method " +
                                        "processing a messages from the SQS queue (" + queueName + ") that caused the message to fail. " +
                                        "The SQS message ID on the queue is; " + msgQueueID + ".", ex);
                    //This is to stop the code from going into an infinite loop if the queue does not exist or is unavailable.
                    isQueueEmpty = true; 
                }
                finally
                {
                    instrumentation.FlushActivity();
                }
            }
        }

        /// <summary>
        /// Retrieves messages from the SQS message queue
        /// </summary>
        /// <param name="queueUrl">URL for the queue</param>
        /// <returns>Message retrieved from the message queue</returns>
        private QueuedMessage GetMessage(string queueUrl)
        {
            //var sqs = new AmazonSQSClient();
            QueuedMessage queuedMsg = new QueuedMessage();

            ReceiveMessageRequest receiveMessageRequest = new ReceiveMessageRequest { QueueUrl = queueUrl };
            ReceiveMessageResponse receiveMessageResponse = null;
            bool isSuccess = Retry.Execute(() => ReceiveMsgFromQueue(receiveMessageRequest, out receiveMessageResponse), new TimeSpan(0, 0, serviceRetryInterval), serviceRetryCount, true);
            if (!isSuccess)
                throw new Exception("Failed to retrieve messages for the SQS Message Queue after " + serviceRetryCount + " retries with an interval of " + serviceRetryInterval + " seconds.", retryExceptions);

            if (receiveMessageResponse.Messages != null)
            {
                foreach (var message in receiveMessageResponse.Messages)
                {
                    if (!string.IsNullOrEmpty(message.MessageId))
                        queuedMsg.Id = message.MessageId;

                    if (!string.IsNullOrEmpty(message.ReceiptHandle))
                        queuedMsg.MessageRecieptHandle = message.ReceiptHandle;

                    if (!string.IsNullOrEmpty(message.MD5OfBody))
                        queuedMsg.Md5OfBody = message.MD5OfBody;

                    if (!string.IsNullOrEmpty(message.Body))
                        queuedMsg.Body = message.Body;
                }
            }
            else
                queuedMsg.QueueEmpty = true;
            return queuedMsg;
        }

        /// <summary>
        /// Removes a message for the message queue
        /// </summary>
        /// <param name="messageRecieptHandle">Identifier that was downloaded with the message</param>
        /// <param name="queueUrl">URL for the queue</param>
        private void RemoveMessageFromQueue(string messageRecieptHandle, string queueUrl)
        {
            var sqs = new AmazonSQSClient();
            //Deleting a message
            var deleteRequest = new DeleteMessageRequest { QueueUrl = queueUrl, ReceiptHandle = messageRecieptHandle };
            sqs.DeleteMessage(deleteRequest);
        }

                    
        /// <summary>
        /// Wrapper method for the service call to post a message to a service.  The wrapped supports the retry logic.
        /// </summary>
        /// <param name="destinationUrl">Destination service URL</param>
        /// <param name="msg">Message to be sent to the service</param>
        /// <returns></returns>
        private bool PostMsgToService(string destinationUrl, XmlDocument msg)
        {
            HipWcfDestinationService.WcfMessagingServiceClient client = new HipWcfDestinationService.WcfMessagingServiceClient();
            try
            {
                isDestActivationSuccessful = false; //Assume the service activation will fail.
                client.Endpoint.Address = new EndpointAddress(destinationUrl);
                string encodedMsg = HttpUtility.HtmlEncode(msg.OuterXml);
                bool result = (bool)client.InitializeProcess(encodedMsg);
                isDestActivationSuccessful = true; //If an exception was not returned then the service was successfully activated.
                if (!result)
                {
                    if (retryExceptions == null)
                    {
                        retryExceptions = new Exception("The destination service, the Push Svc called, returned FALSE.  This suggests the service was unable to process the message and should have logged an exception. The destination service is: " + destinationUrl);
                    }
                    else
                    {
                        retryExceptions = new Exception("The destination service, the Push Svc called, returned FALSE.  This suggests the service was unable to process the message and should have logged an exception. The destination service is: " + destinationUrl, retryExceptions);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                retryCount++;
                Trace.WriteLineIf(tracingEnabled, "Push Service: Exception occured trying to post the message to the destination service (Retry: " + retryCount + "). The destination service is: " + destinationUrl);
                if (retryExceptions == null)
                {
                    retryExceptions = new Exception("Error tring to post the message to the destination service (Retry: " + retryCount + "). The destination service is: " + destinationUrl, ex);
                }
                else
                {
                    retryExceptions = new Exception(ex.Message, retryExceptions);
                }
                return false;
            }
            finally
            {
                client.Close();
            }
        }

        /// <summary>
        /// Wrapper method to retrieve messages from the SQS queue. The wrapped supports the retry logic.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="response"></param>
        /// <returns></returns>
        private bool ReceiveMsgFromQueue(ReceiveMessageRequest request, out ReceiveMessageResponse response)
        {
            var sqs = new AmazonSQSClient();
            try
            {
                response = sqs.ReceiveMessage(request);
                return true;
            }
            catch (Exception ex)
            {
                retryCount++;
                Trace.WriteLineIf(tracingEnabled, "Push Service: Exception occured trying to receive messages from the SQS message queue (Retry: " + retryCount + "). The queue URL is: " + request.QueueUrl);
                if (retryExceptions == null)
                    retryExceptions = new Exception("Error tring to receive messages from the SQS message queue (Retry: " + retryCount + "). The queue URL is: " + request.QueueUrl, ex);
                else
                    retryExceptions = new Exception(ex.Message, retryExceptions);
                response = null;
                return false;
            }
            finally
            {
                sqs.Dispose();
            }
        }

    ///// <summary>
    ///// Retrieves a list of available queues from the SQS server.
    ///// </summary>
    //private void ListQueues()
    //{
    //    var sqs = new AmazonSQSClient();
    //    var listQueuesRequest = new ListQueuesRequest();
    //    var listQueuesResponse = sqs.ListQueues(listQueuesRequest);

    //    if (listQueuesResponse.QueueUrls != null)
    //    {
    //        foreach (String queueUrl in listQueuesResponse.QueueUrls)
    //        {
    //            if(!queueUrl.Contains("DL_"))
    //                queues.Add(queueUrl);
    //        }
    //    }
    //    Trace.WriteLineIf(tracingEnabled, "Get list of active SQS queues.  Count:" + queues.Count);
    //}
    }

    /// <summary>
    /// Class to represent a message retrieved from a queue
    /// </summary>
    public class QueuedMessage
    {
        private string body = "";
        private string id = "";
        private string messageRecieptHandle = "";
        private string md5OfBody = "";
        private bool queueEmpty = false;

        public string Body { get => body; set => body = value; }
        public string Id { get => id; set => id = value; }
        public string MessageRecieptHandle { get => messageRecieptHandle; set => messageRecieptHandle = value; }
        public string Md5OfBody { get => md5OfBody; set => md5OfBody = value; }
        public bool QueueEmpty { get => queueEmpty; set => queueEmpty = value; }
    }

}
