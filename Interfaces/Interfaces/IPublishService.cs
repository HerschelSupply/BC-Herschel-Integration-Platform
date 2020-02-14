using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BC.Integration.Interfaces
{
    public interface IPublishService
    {
        /// <summary>
        /// Provides the componenet with a full set of configuration values from local and global sources.
        /// </summary>
        List<KeyValuePair<string, string>> Configuration { get; set; }

        /// <summary>
        /// Service that pushes a message onto a messaging queue
        /// </summary>
        /// <param name="queueUrl">Url to connect to the queue.</param>
        /// <param name="msg">Message to be pushed on the queue.</param>
        void Push(string queueUrl, string msg, out int retryCount);
    }
}
