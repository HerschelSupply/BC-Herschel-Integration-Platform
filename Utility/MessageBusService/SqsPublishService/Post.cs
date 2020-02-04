using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Amazon.SQS;
using Amazon.SQS.Model;
using BC.Integration.Interfaces;
using System.Diagnostics;

namespace BC.Integration.Utility
{
    public class SqsPublishService : IPublishService
    {
        private List<KeyValuePair<string, string>> configurationCollection = null;
        private Exception retryExceptions = null;
        private int retryCount = 0;
        private int serviceRetryInterval = 0;
        private int serviceRetryCount = 0;
        private bool tracingEnabled = false;

        public List<KeyValuePair<string, string>> Configuration { get => configurationCollection; set => configurationCollection = value; }

        private void Setup()
        {
            if (configurationCollection == null)
                throw new Exception("The SqsPublishService needs the Configuration property populated before calling the services methods.");
            try
            {
                serviceRetryCount = Convert.ToInt16(Utilities.GetConfigurationValue(configurationCollection, "ServiceRetryCount"));
                serviceRetryInterval = Convert.ToInt16(Utilities.GetConfigurationValue(configurationCollection, "ServiceRetryInterval"));
                string val = Utilities.GetConfigurationValue(configurationCollection, "TracingEnabled");
                if (val != "")
                {
                    if (val.ToLower() == "true" || val.ToLower() == "false")
                        tracingEnabled = Convert.ToBoolean(val);
                    if (val == "1" || val == "0")
                        tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occurred in the SqsPublishService Setup method while trying to set the retry configuration values.", ex);
            }
        }

        public void Push(string queueUrl, string msg, out int retries)
        {
            Setup();
            //var listQueuesRequest = new ListQueuesRequest();

            var sendMessageRequest = new SendMessageRequest
            {
                QueueUrl = queueUrl, //URL from initial queue creation
                MessageBody = msg
            };

            bool isSuccess = Retry.Execute(() => SendMsgToQueue(sendMessageRequest), new TimeSpan(0, 0, serviceRetryInterval), serviceRetryCount, true);
            if (!isSuccess)
                throw new Exception("Failed to publish message to SQS queue after " + serviceRetryCount + " retries with an interval of " + serviceRetryInterval + " seconds.  The queue name is; " + queueUrl, retryExceptions);
            retries = retryCount;
        }

        /// <summary>
        /// This method is used to wrap the SQS send method to support the retry logic.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private bool SendMsgToQueue(SendMessageRequest request)
        {
            var sqs = new AmazonSQSClient();
            try
            {
                sqs.SendMessage(request);
                return true;
            }
            catch (Exception ex)
            {
                retryCount++;
                Trace.WriteLine("Exception occurred trying to send a message to the SQS message queue (Retry: " + retryCount + "). The queue URL is: " + request.QueueUrl);
                if (retryExceptions == null)
                    retryExceptions = new Exception("Error trying to send a message to the SQS message queue (Retry: " + retryCount + "). The queue URL is: " + request.QueueUrl, ex);
                else
                    retryExceptions = new Exception(ex.Message, retryExceptions);
                return false;
            }
            finally
            {
                sqs.Dispose();
            }
        }
    }
}
