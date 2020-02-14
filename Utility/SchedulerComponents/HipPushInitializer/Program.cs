using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ServiceModel;
using System.Configuration;


namespace Corp.Integration.Utility.HipPushInitializer
{
    class Program
    {
        /// <summary>
        /// Expected Parameter are Queue Name as a string, Maximum Rate to process the queue as a decimal, a flag to determine if the queue needs to be
        /// processed as a batch (true or false) and the maximum batch size, as an int, for when the batch flag is true.  The push service URl is retrieved 
        /// from the config file.
        /// </summary>
        /// <param name="args">queueName maxRate (Messages/sec) isBatch maxBatchSize</param>
        static void Main(string[] args)
        {
            Trace.WriteLine("Start the HIP Push Initializer");
            string url = ConfigurationManager.AppSettings["PushServiceUrl"];
            if (args.Count() < 3)
                throw new Exception("Please pass atleast 3 arguments 'queueName maxRate isBatch maxBatchSize'. Use '0' for Max Rate if no throttling is needed. If isBatch = false there is no need to pass the Max Batch Size.");

            string queueName = args[0];
            decimal maxRate = Convert.ToDecimal(args[1]);
            bool isBatch = Convert.ToBoolean(args[2]);
            int maxBatchSize = 0;
            if (isBatch)
            {
                if (args.Count() != 4)
                    throw new Exception("When isBatch is 'true', please pass all 4 arguments 'queueName maxRate isBatch maxBatchSize'. Use '0' for Max Rate if no throttling is needed. If isBatch = false there is no need to pass the Max Batch Size.");
                maxBatchSize = Convert.ToInt16(args[3]);
            }
            string guid = Guid.NewGuid().ToString();

            Trace.WriteLine("Call Service from client. URL: '" + url + "', Max Rate '" + maxRate + "', Queue Name: '" + queueName + 
                                "', Is Batch: '" + isBatch + ", Max Batch Size: '" + maxBatchSize + "'.");

            EndpointAddress add = new EndpointAddress(url);

            try
            { 
                Push.PushSvcClient client = new Push.PushSvcClient();
                client.Endpoint.Address = new EndpointAddress(url);
                client.InitializeProcess(guid, queueName, maxRate, isBatch, maxBatchSize);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception occured whilst running the HIP Push Initializer.");
                throw new Exception("An error occured trying to call the Push service at '" + url + "'. Please review the inner exception for details of the error.", ex);
            }

            Trace.WriteLine("Push Servcie call complete.");
        }
    }
}
