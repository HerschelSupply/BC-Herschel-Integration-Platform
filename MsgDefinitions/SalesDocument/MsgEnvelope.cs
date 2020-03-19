using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

namespace Corp.Integration
{
    public class MessageManager
    {
        //Used to store a copy of the entry point env so it can be used to populate child envelopes and provide consistent data
        private MsgEnvelope entryPointEnvelope = new MsgEnvelope();


        /// <summary>
        /// Use this method when for non batch message processes
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="serviceId"></param>
        /// <param name="svcVersion"></param>
        /// <param name="serviceOperationId"></param>
        /// <param name="msgType"></param>
        /// <param name="msgVersion"></param>
        /// <param name="format"></param>
        /// <param name="topic"></param>
        /// <param name="filter"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public XmlDocument CreateOnRampMessage(string processName, string serviceId, decimal svcVersion, string serviceOperationId,
                                          string msgType, decimal msgVersion, string format, string body)
        {
            entryPointEnvelope = new MsgEnvelope();
            entryPointEnvelope.Interchange.NewInterchange();
            entryPointEnvelope.Interchange.ProcessName = processName;
            entryPointEnvelope.Interchange.EntryPoint = serviceId;

            entryPointEnvelope.Service.ServiceID = serviceId;
            entryPointEnvelope.Service.NewServiceInstanceId();
            entryPointEnvelope.Service.Version = svcVersion;
            entryPointEnvelope.Service.ServiceOperationId = serviceOperationId;
            entryPointEnvelope.Service.NewServiceOperationInstanceId();

            entryPointEnvelope.Msg.NewMessageId();
            entryPointEnvelope.Msg.Type = msgType;
            entryPointEnvelope.Msg.Version = msgVersion;
            entryPointEnvelope.Msg.Format = format;

            XmlDocument msg = AddBodyToEnvelope(entryPointEnvelope, body, format);
            return msg;
        }

        /// <summary>
        /// Use this method for services that are initiated to process a batch message.
        /// </summary>
        /// <param name="processName"></param>
        /// <param name="serviceId"></param>
        /// <param name="svcVersion"></param>
        /// <param name="serviceOperationId"></param>
        /// <param name="msgType"></param>
        /// <param name="msgVersion"></param>
        /// <param name="format"></param>
        /// <param name="topic"></param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public XmlDocument CreateOnRampMessage(string processName, string serviceId, decimal svcVersion, string serviceOperationId,
                                          string msgType, decimal msgVersion, string format)
        {
            entryPointEnvelope = new MsgEnvelope();
            entryPointEnvelope.Interchange.NewInterchange();
            entryPointEnvelope.Interchange.ProcessName = processName;
            entryPointEnvelope.Interchange.EntryPoint = serviceId;

            entryPointEnvelope.Service.ServiceID = serviceId;
            entryPointEnvelope.Service.NewServiceInstanceId();
            entryPointEnvelope.Service.Version = svcVersion;
            entryPointEnvelope.Service.ServiceOperationId = serviceOperationId;
            entryPointEnvelope.Service.NewServiceOperationInstanceId();

            entryPointEnvelope.Msg.NewMessageId();
            entryPointEnvelope.Msg.Type = msgType;
            entryPointEnvelope.Msg.Version = msgVersion;
            entryPointEnvelope.Msg.Format = format;

            XmlDocument msg = ConvertMessageToXml(entryPointEnvelope);
            return msg;
        }

        public XmlDocument CreatePostMessage(string servicePostOperationId, string msgType, decimal msgVersion, string format, string topic, string filter, string body)
        {
            MsgEnvelope newMsg = new MsgEnvelope();
            //Use values from the entry point envelope
            newMsg.Interchange.InterchangeId = entryPointEnvelope.Interchange.InterchangeId;
            newMsg.Interchange.ProcessName = entryPointEnvelope.Interchange.ProcessName;
            newMsg.Interchange.EntryPoint = entryPointEnvelope.Interchange.EntryPoint;
            newMsg.Interchange.Timestamp = entryPointEnvelope.Interchange.Timestamp;

            newMsg.Service.ServiceID = entryPointEnvelope.Service.ServiceID;
            newMsg.Service.ServiceInstanceId = entryPointEnvelope.Service.ServiceInstanceId;
            newMsg.Service.Version = entryPointEnvelope.Service.Version;
            newMsg.Service.ServiceOperationId = entryPointEnvelope.Service.ServiceOperationId;
            newMsg.Service.ServiceOperationInstanceId = entryPointEnvelope.Service.ServiceOperationInstanceId;
            
            //Add new values for the Post operation and updated message data.
            newMsg.Service.NewServicePostOperationInstanceId();
            newMsg.Service.ServicePosOperationtId = servicePostOperationId;

            newMsg.Msg.ParentMsgId = entryPointEnvelope.Msg.Id;
            newMsg.Msg.NewMessageId();
            newMsg.Msg.Type = msgType;
            newMsg.Msg.Version = msgVersion;
            newMsg.Msg.Format = format;
            newMsg.Msg.Topic = topic;
            newMsg.Msg.TopicFilter = filter;
            //Attach message body
            XmlDocument msg = AddBodyToEnvelope(newMsg, body, format);
            return msg;
        }

        public XmlDocument CreateReceiveMessage(string processName, string serviceId, decimal svcVersion, string serviceOperationId,
                                          string msgType, decimal msgVersion, string format, string body)
        {
            return CreateOnRampMessage(processName, serviceId, svcVersion, serviceOperationId, msgType, msgVersion, format, body);
        }

        public XmlDocument CreateOffRampMessage(string servicePostOperationId, string msgType, decimal msgVersion, string format, string topic, string filter, string body)
        {
            return CreatePostMessage(servicePostOperationId, msgType, msgVersion, format, topic, filter, body);
        }


        private XmlDocument AddBodyToEnvelope(MsgEnvelope env, string body, string bodyFormat)
        {
            XmlDocument msg = ConvertMessageToXml(env);
            if (bodyFormat.ToLower() == "xml")
            {
                //If Format is XML remove '<?xml version=\"1.0\" encoding=\"utf-16\"?>' at the begining of xml string
                body = body.Substring(body.IndexOf("><") + 1);
                msg.DocumentElement["Body"].InnerXml = body;
            }
            else
            {
                msg.DocumentElement["Body"].InnerText = body;
            }
            return msg;
        }

        /// <summary>
        /// This method serializes an object model to an xml string.
        /// </summary>
        /// <param name="env">The sales document object to be serialized</param>
        /// <returns>XML string representation of the Message Envelope object</returns>
        private XmlDocument ConvertMessageToXml(MsgEnvelope env)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(env.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, env);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured serializing the Message Envelope object model.  The error occured in the ConvertMessageToXml method.", ex);
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(sw.ToString());
            return doc;
        }
    }

    public class MsgEnvelope
    {
        private Interchange interchange = new Interchange();
        private Message msg = new Message();
        private Service service = new Service();
        private Correlation correlation = null;
        private string body = "";

        public Interchange Interchange { get => interchange; set => interchange = value; }
        [XmlElement("Msg")]
        public Message Msg { get => msg; set => msg = value; }
        [XmlElement("Svc")]
        public Service Service { get => service; set => service = value; }
        public Correlation Correlation { get => correlation; set => correlation = value; }
        public string Body { get => body; set => body = value; }
    }

    public class Interchange
    {
        private Guid interchangeId = new Guid();
        private string entryPoint = "";
        private DateTime timestamp = new DateTime();
        private string processName = "";

        [XmlElement("Id")]
        public Guid InterchangeId { get => interchangeId; set => interchangeId = value; }
        public string EntryPoint { get => entryPoint; set => entryPoint = value; }
        public DateTime Timestamp { get => timestamp; set => timestamp = value; }
        public string ProcessName { get => processName; set => processName = value; }

        /// <summary>
        /// Updates the interchange Id with a new GUID and sets the timestamp to the current date and time.
        /// </summary>
        public void NewInterchange()
        {
            interchangeId = Guid.NewGuid();
            timestamp = DateTime.Now;
        }
    }

    public class Service
    {
        #region Properties
        private string serviceID = "";
        private Guid serviceInstanceId = new Guid();
        private decimal version = 0;
        private string serviceOperationId = "";
        private Guid serviceOperationInstanceId = new Guid();
        private string servicePostOperationId = "";
        private Nullable<Guid> servicePostOperationInstanceId = null;
        #endregion
        [XmlElement("SvcId")]
        public string ServiceID { get => serviceID; set => serviceID = value; }
        [XmlElement("SvcInstId")]
        public Guid ServiceInstanceId { get => serviceInstanceId; set => serviceInstanceId = value; }
        [XmlElement("ver")]
        public decimal Version { get => version; set => version = value; }
        [XmlElement("SvcOpId")]
        public string ServiceOperationId { get => serviceOperationId; set => serviceOperationId = value; }
        [XmlElement("SvcOpInstId")]
        public Guid ServiceOperationInstanceId { get => serviceOperationInstanceId; set => serviceOperationInstanceId = value; }
        [XmlElement("SvcPostId")]
        public string ServicePosOperationtId { get => servicePostOperationId; set => servicePostOperationId = value; }
        [XmlElement("SvcPostInstId")]
        public Nullable<Guid> ServicePostOperationInstanceId { get => servicePostOperationInstanceId; set => servicePostOperationInstanceId = value; }
 
        /// <summary>
        /// Updates the serviceInstanceId with a new GUID
        /// </summary>
        public void NewServiceInstanceId()
        {
            serviceInstanceId = Guid.NewGuid();
        }

        /// <summary>
        /// Updates the ServiceOperationInstanceId with a new GUID
        /// </summary>
        public void NewServiceOperationInstanceId()
        {
            ServiceOperationInstanceId = Guid.NewGuid();
        }

        /// <summary>
        /// Updates the servicePostInstanceId with a new GUID
        /// </summary>
        public void NewServicePostOperationInstanceId()
        {
            servicePostOperationInstanceId = Guid.NewGuid();
        }
    }

    public class Message
    {
        #region Properties
        private Guid id = new Guid();
        private string type = "";
        private decimal version = 0;
        private string format = "";
        private Nullable<Guid> parentMsgId = null;
        private string topic = "";
        private string topicFilter = "";
        #endregion
        /// <summary>
        /// This method is read only.  To create a new msg ID use the NewMessageId method.
        /// </summary>
        [XmlElement("Id")]
        public Guid Id { get => id; set => id = value; }
        public string Type { get => type; set => type = value; }
        [XmlElement("ver")]
        public decimal Version { get => version; set => version = value; }
        public string Format { get => format; set => format = value; }
        public Nullable<Guid> ParentMsgId { get => parentMsgId; set => parentMsgId = value; }
        public string Topic { get => topic; set => topic = value; }
        public string TopicFilter { get => topicFilter; set => topicFilter = value; }

        /// <summary>
        /// This method is used when a new message ID is required.  It automatically updates the parentMsgId.
        /// </summary>
        public void NewMessageId()
        {
            id = Guid.NewGuid();
        }

    }

    public class Correlation
    {
        private List<string> list = new List<string>();

        public string CorrelationTokens
        {
            set => list.Add(value);
        }

        public string GetCorrelationToken(int index)
        {
            return list[index].ToString();
        }
    }

}
