using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace BC.Integration.Interfaces
{
    public interface IInstrumentation
    {
        /// <summary>
        /// Provides the componenet with a full set of configuration values from local and global sources.
        /// </summary>
        List<KeyValuePair<string, string>> Configuration { get; set; }

        /// <summary>
        /// For instrumentation level should be 'Normal' or 'Verbose'.  Verbose will also include the message body.
        /// </summary>
        /// <param name="msg">HIP msg emvelope. The envelope properties will be used to document activity</param>
        /// <param name="level">'Normal' or 'Verbose'</param>
        void LogActivity(XmlDocument msg);

        /// <summary>
        /// Failure to log should not return an exception message as instumentation is a non critical path so should never 
        /// cause a message to fail.
        /// </summary>
        /// <param name="msg">Serialized Msg Envelope</param>
        /// <param name="level">Normal or Verbose</param>
        /// <param name="destination">The destination the post operation send the message too.</param>
        /// <param name="retries">Number of retries the Post Operation had to preform.</param>
        void LogActivity(XmlDocument msg, string destination, int retries);

        /// <summary>
        /// Saves the collection of records to the instumentation data store.
        /// </summary>
        void FlushActivity();

        /// <summary>
        /// Exceptions are written immediately to the data store.
        /// Instrumentation level should be 'Normal' or 'Verbose'.  Verbose will also include the message body.
        /// </summary>
        /// <param name="methodName">Method that trapped the exception</param>
        /// <param name="parameters">Parameters used to call the method</param>
        /// <param name="msg">HIP msg emvelope. The envelope properties will be used to document activity</param>
        /// <param name="ex">Exception that was raised</param>
        /// <param name="level">'Normal' or 'Verbose'</param>
//        void LogException(string methodName, string parameters, XmlDocument msg, Exception ex, string level);

        void LogMessagingException(string message, XmlDocument msg, Exception ex);

        void LogGeneralException(string message, Exception ex);

        /// <summary>
        /// Persists an XML message to the file storage.  The method builds a directory structure including the date 
        /// and batch filename. The saved file uses the messages GUID ID as part of the file name.
        /// </summary>
        /// <param name="path">Root path to plave file</param>
        /// <param name="batchFilename">Used to create a subdirectory</param>
        /// <param name="msg">Message to be saved to file</param>
        /// <param name="documentType">Used to create part of the file name</param>
        /// <param name="processStep">Used to create part of the file name</param>
        void WriteMsgToFile(string path, string batchFilename, XmlDocument msg, string documentType, string processStep);

        /// <summary>
        /// Persists an message string to the file storage.  The method builds a directory structure including the date and batch 
        /// filename. This method was added to same Json serialized messages.
        /// </summary>
        /// <param name="path">Path to the route file storage location</param>
        /// <param name="batchFilename">If the messages need to be grouped in a batch include the batch filename, otherwise pass null</param>
        /// <param name="msg">message to be persisted</param>
        /// <param name="documentType">The document type being transmitted.  This parameter used to build the message name.</param>
        /// <param name="processStep">Process step the message was created (onRamp or Post).  This parameter used to build the message name.</param>
        void WriteMsgToFile(string path, string batchFilename, string msg, string documentType, string processStep);

        void LogActivation(string serviceName, Guid activationGuid, bool isBlocked);

        void LogActivationEnd(Guid activationGuid);

        void AssociateActivationWithInterchages(Guid activationGuid, Guid interchageId);

        void LogNotification(string processName, string serviceID, Guid messageId, string issueCategory, string issueMessage, string documentId);
    }
}
