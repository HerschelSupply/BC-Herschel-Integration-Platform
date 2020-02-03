using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.IO;

using System.Web;
using System.Diagnostics;


namespace BC.Integration
{
    #region Message Manager
    public class MessageManager
    {
        //Used to store a copy of the entry point and receive message envelopes so they can be used to populate 
        //child envelopes and provide consistent data.
        private MsgEnvelope entryPointEnvelope = new MsgEnvelope();
        private MsgEnvelope receivedEnvelope = new MsgEnvelope();
        private bool onRampService = false;

        public MsgEnvelope EntryPointEnvelope { get => entryPointEnvelope; }
        public MsgEnvelope ReceivedEnvelope { get => receivedEnvelope; }


        /// <summary>
        /// Use this method when for non batch message processes.  This method includes the ability to log the message body.  In a batch
        /// process the amount of data could exceed variable size limitations.
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
                                          string msgType, decimal msgVersion, string format, string documentId, string body)
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
            entryPointEnvelope.Msg.DocumentId = documentId;

            XmlDocument msg = AddBodyToEnvelope(entryPointEnvelope, body, format);
            onRampService = true;
            return msg;
        }

        /// <summary>
        /// Use this method for services that are initiated to process a batch message.  This method call does not include the message body as the size could 
        /// cause processing problems.
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
                                          string msgType, decimal msgVersion, string format, string documentId)
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
            entryPointEnvelope.Msg.DocumentId = documentId;

            XmlDocument msg = ConvertHipEnvelopeToXml(entryPointEnvelope);
            onRampService = true;
            return msg;
        }

        /// <summary>
        /// This method is used to update the envelope before the message is posted to the next service.  In some cases there is no need
        /// to update the values and the current service is not aware of the values.  In this case if null for strings or 0 for doubles
        /// is passed instead of a value the original value will not be overwritten.
        /// </summary>
        /// <param name="servicePostOperationId"></param>
        /// <param name="msgType">If null the value from the onRamp or receive message is used.</param>
        /// <param name="msgVersion">If null the value from the onRamp or receive message is used.</param>
        /// <param name="format">If null the value from the onRamp or receive message is used.</param>
        /// <param name="topic">If null the value from the onRamp or receive message is used.</param>
        /// <param name="filterKeyValuePairs">If null the value from the onRamp or receive message is used.</param>
        /// <param name="processKeyValuePairs">If null the value from the onRamp or receive message is used.</param>
        /// <param name="body">If null the value from the onRamp or receive message is used.</param>
        /// <param name="messageSplitIndex">Incremental ID used to track the splitting of a batch message</param>
        /// <param name="batchId">ID used to track messages that are being collected together into a single batch message.</param>
        /// <param name="documentId">Document ID of the payload message.  If an empty string is passed the original value will be maintained.</param>
        /// <returns></returns>
        public XmlDocument CreatePostMessage(string servicePostOperationId, string msgType, decimal msgVersion, string format, 
                                        string topic, HipKeyValuePairCollection filterKeyValuePairs, HipKeyValuePairCollection processKeyValuePairs, 
                                        string body, Nullable<int> messageSplitIndex, Nullable<Guid> batchId, string documentId)
        {
            MsgEnvelope originalMsg = new MsgEnvelope();
            if (onRampService)
                originalMsg = entryPointEnvelope;
            else
                originalMsg = receivedEnvelope;

            MsgEnvelope newMsg = new MsgEnvelope();
            //Use values from the entry point envelope
            newMsg.Interchange.InterchangeId = originalMsg.Interchange.InterchangeId;
            newMsg.Interchange.ProcessName = originalMsg.Interchange.ProcessName;
            newMsg.Interchange.EntryPoint = originalMsg.Interchange.EntryPoint;
            newMsg.Interchange.Timestamp = originalMsg.Interchange.Timestamp;
            if(batchId != null)
                newMsg.Interchange.BatchId = (Guid)batchId;

            newMsg.Service.ServiceID = originalMsg.Service.ServiceID;
            newMsg.Service.ServiceInstanceId = originalMsg.Service.ServiceInstanceId;
            newMsg.Service.Version = originalMsg.Service.Version;
            newMsg.Service.ServiceOperationId = originalMsg.Service.ServiceOperationId;
            newMsg.Service.ServiceOperationInstanceId = originalMsg.Service.ServiceOperationInstanceId;
            
            //Add new values for the Post operation and updated message data.
            newMsg.Service.NewServicePostOperationInstanceId();
            newMsg.Service.ServicePostOperationtId = servicePostOperationId;

            newMsg.Msg.DocumentId = originalMsg.Msg.DocumentId;
            newMsg.Msg.ParentMsgId = originalMsg.Msg.Id;
            newMsg.Msg.NewMessageId();
            if (msgType != null)
                newMsg.Msg.Type = msgType;
            else
                newMsg.Msg.Type = originalMsg.Msg.Type;
            if (msgVersion != 0)
                newMsg.Msg.Version = msgVersion;
            else
                newMsg.Msg.Version = originalMsg.Msg.Version;
            if (format != null)
                newMsg.Msg.Format = format;
            else
                newMsg.Msg.Format = originalMsg.Msg.Format;
            if (topic != null)
                newMsg.Msg.Topic = topic;
            else
                newMsg.Msg.Topic = originalMsg.Msg.Topic;
            if (filterKeyValuePairs != null)
                newMsg.Msg.FilterKeyValuePairs = filterKeyValuePairs;
            else
                newMsg.Msg.FilterKeyValuePairs = originalMsg.Msg.FilterKeyValuePairs;
            if (processKeyValuePairs != null)
                newMsg.Msg.ProcessKeyValuePairs = processKeyValuePairs;
            else
                newMsg.Msg.ProcessKeyValuePairs = originalMsg.Msg.ProcessKeyValuePairs;
            if (messageSplitIndex != null)
                newMsg.Msg.MessageSplitIndex = (int)messageSplitIndex;
            else
                newMsg.Msg.MessageSplitIndex = originalMsg.Msg.MessageSplitIndex;
            if (documentId != "")
                newMsg.Msg.DocumentId = documentId;
            else
                newMsg.Msg.DocumentId = originalMsg.Msg.DocumentId;

            //Attach message body
            if (body != null)
                return AddBodyToEnvelope(newMsg, body, format);
            else
                newMsg.Body = originalMsg.Body;
            return ConvertHipEnvelopeToXml(newMsg);
        }

        /// <summary>
        /// This method is designed for updating the envelope properties of an inflight message after it has been recieved by a service.
        /// Once the recieved envelope has been updated the calling service can access the envelope properties, as an object model, using the 
        /// MessageManager.ReceivedMessage property.
        /// </summary>
        /// <param name="receivedMsg"> XML version of the message the service received.</param>
        /// <param name="serviceId">ID of the current service</param>
        /// <param name="svcVersion">version of the current service</param>
        /// <param name="serviceOperationId">Service operation ID of the current service</param>
        /// <returns>Updated XML representation of received msg</returns>
        public XmlDocument CreateReceiveMessage(XmlDocument receivedMsg, string serviceId, decimal svcVersion, string serviceOperationId)
        {
            Debug.WriteLine("MessageManager.CreateReceiveMessage with serviceId: " + serviceId + ", serviceOperationId: " + serviceOperationId);
            //de-serialize the received message and update the properties before re-serializing it and returning the message.  ReceivedEnvelope property should be populated.
            //Duplicate the envelope from the received message
            receivedEnvelope = ConvertHipEnvelopeToOm(receivedMsg);
            //Update the key fields of the received message envelope
            receivedEnvelope.Msg.ParentMsgId = receivedEnvelope.Msg.Id;
            receivedEnvelope.Msg.NewMessageId();
            receivedEnvelope.Service.ServiceID = serviceId;
            receivedEnvelope.Service.NewServiceInstanceId();
            receivedEnvelope.Service.Version = svcVersion;
            receivedEnvelope.Service.ServiceOperationId = serviceOperationId;
            receivedEnvelope.Service.NewServiceOperationInstanceId();  //Line added July 26th to fix issue HSC-240
            receivedEnvelope.Service.ServicePostOperationtId = "";
            receivedEnvelope.Service.ServicePostOperationInstanceId = null;

            return ConvertHipEnvelopeToXml(receivedEnvelope);
        }

        /// <summary>
        /// This method is used to update the envelope to track the posing of the message to the off ramp of the dervice to a end system.
        /// This will usally signify the end of the Process.
        /// </summary>
        /// <param name="servicePostOperationId"></param>
        /// <param name="msgType"></param>
        /// <param name="msgVersion"></param>
        /// <param name="format"></param>
        /// <param name="body"></param>
        /// <param name="messageSplitIndex">Incremental ID used to track the splitting of a batch message</param>
        /// <param name="batchId">ID used to track messages that are being collected together into a single batch message.</param>
        /// <param name="documentId">Document ID of the payload message.  If an empty string is passed the original value will be maintained.</param>
        /// <returns></returns>
        public XmlDocument CreateOffRampMessage(string servicePostOperationId, string msgType, decimal msgVersion, string format, string body, 
                                                Nullable<int> messageSplitIndex, Nullable<Guid> batchId, string documentId)
        {
            Debug.WriteLine("MessageManager.CreateReceiveMessage with serviceOperationPostId " + servicePostOperationId + ", onRampService flag: " + onRampService.ToString());
            MsgEnvelope originalMsg = new MsgEnvelope();
            if (onRampService)
                originalMsg = entryPointEnvelope;
            else
                originalMsg = receivedEnvelope;

            MsgEnvelope newMsg = new MsgEnvelope();
            //Use values from the entry point envelope
            newMsg.Interchange.InterchangeId = originalMsg.Interchange.InterchangeId;
            newMsg.Interchange.ProcessName = originalMsg.Interchange.ProcessName;
            newMsg.Interchange.EntryPoint = originalMsg.Interchange.EntryPoint;
            newMsg.Interchange.Timestamp = originalMsg.Interchange.Timestamp;
            if (batchId != null)
                newMsg.Interchange.BatchId = (Guid)batchId;

            newMsg.Service.ServiceID = originalMsg.Service.ServiceID;
            newMsg.Service.ServiceInstanceId = originalMsg.Service.ServiceInstanceId;
            newMsg.Service.Version = originalMsg.Service.Version;
            newMsg.Service.ServiceOperationId = originalMsg.Service.ServiceOperationId;
            newMsg.Service.ServiceOperationInstanceId = originalMsg.Service.ServiceOperationInstanceId;

            //Add new values for the Post operation and updated message data.
            newMsg.Service.NewServicePostOperationInstanceId();
            newMsg.Service.ServicePostOperationtId = servicePostOperationId;

            newMsg.Msg.ParentMsgId = originalMsg.Msg.Id;
            newMsg.Msg.NewMessageId();
            if (msgType != null)
                newMsg.Msg.Type = msgType;
            if (msgVersion != 0)
                newMsg.Msg.Version = msgVersion;
            if (format != null)
                newMsg.Msg.Format = format;
            newMsg.Msg.Topic = null;
            newMsg.Msg.FilterKeyValuePairs = null;
            newMsg.Msg.ProcessKeyValuePairs = null;
            newMsg.Msg.ProcessEnd = true;
            if (messageSplitIndex != null)
                newMsg.Msg.MessageSplitIndex = (int)messageSplitIndex;
            else
                newMsg.Msg.MessageSplitIndex = originalMsg.Msg.MessageSplitIndex;
            if (documentId != "")
                newMsg.Msg.DocumentId = documentId;
            else
                newMsg.Msg.DocumentId = originalMsg.Msg.DocumentId;

            //Attach message body
            if (body != null)
                return AddBodyToEnvelope(newMsg, body, format);
            else
                newMsg.Body = null;
            return ConvertHipEnvelopeToXml(newMsg);
        }

        /// <summary>
        /// Adds the message body to the envelope and encodes the body to avoid it impacting the serialization of the message.
        /// </summary>
        /// <param name="env"></param>
        /// <param name="body"></param>
        /// <param name="bodyFormat"></param>
        /// <returns></returns>
        private XmlDocument AddBodyToEnvelope(MsgEnvelope env, string body, string bodyFormat)
        {
            XmlDocument msg = ConvertHipEnvelopeToXml(env);
            if (bodyFormat.ToLower() == "xml")
            {
                //If Format is XML remove '<?xml version=\"1.0\" encoding=\"utf-16\"?>' at the begining of xml string
                body = body.Substring(body.IndexOf("><") + 1);
                msg.DocumentElement["Body"].InnerXml = HttpUtility.HtmlEncode(body);
            }
            else
            {
                msg.DocumentElement["Body"].InnerText = HttpUtility.HtmlEncode(body);
            }
            return msg;
        }

        /// <summary>
        /// This method serializes an envelope object model to an xml string.
        /// </summary>
        /// <param name="env">The HIP envelope object to be serialized</param>
        /// <returns>XML string representation of the Message Envelope object</returns>
        public XmlDocument ConvertHipEnvelopeToXml(MsgEnvelope env)
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

        /// <summary>
        /// This method converts a XmlDocument representation of a Hip message envelope
        /// into an object model.
        /// </summary>
        /// <param name="msg">XmlDocument to convert</param>
        /// <returns>Envelope object model</returns>
        public MsgEnvelope ConvertHipEnvelopeToOm (XmlDocument msg)
        {
            using (TextReader sr = new StringReader(msg.InnerXml))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(MsgEnvelope));
                MsgEnvelope msgEnv = (MsgEnvelope)serializer.Deserialize(sr);
                return msgEnv;
            }
        }

    }
    #endregion

    #region Message Envelope Classes

    public class MsgEnvelope
    {
        private Interchange interchange = new Interchange();
        private Message msg = new Message();
        private Service service = new Service();
        private string body = "";

        [XmlElement]
        public Interchange Interchange { get => interchange; set => interchange = value; }
        [XmlElement("Msg")]
        public Message Msg { get => msg; set => msg = value; }
        [XmlElement("Svc")]
        public Service Service { get => service; set => service = value; }
        public string Body
        { get => body; set => body = value; }


        /// <summary>
        /// Use to encode the body text in the object model
        /// </summary>
        public void EncodeBody()
        {
            body = HttpUtility.HtmlEncode(body);
        }

        /// <summary>
        /// Use to decode the body text in the object model
        /// </summary>
        public void DecodeBody()
        {
            body = HttpUtility.HtmlDecode(body);
        }
    }

    public class Interchange
    {
        private Guid interchangeId = new Guid();
        private string entryPoint = "";
        private DateTime timestamp = new DateTime();
        private string processName = "";
        private Guid batchId = new Guid();

        [XmlElement("Id")]
        public Guid InterchangeId { get => interchangeId; set => interchangeId = value; }
        [XmlElement]
        public string EntryPoint { get => entryPoint; set => entryPoint = value; }
        [XmlElement]
        public DateTime Timestamp { get => timestamp; set => timestamp = value; }
        [XmlElement]
        public string ProcessName { get => processName; set => processName = value; }
        [XmlElement]
        public Guid BatchId { get => batchId; set => batchId = value; }
        //private int childInterchangeId = 0;
        //public int ChildInterchangeId { get => childInterchangeId; set => childInterchangeId = value; }

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
        public string ServicePostOperationtId { get => servicePostOperationId; set => servicePostOperationId = value; }
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
        private HipKeyValuePairCollection filterKeyValuePairs = null;
        private HipKeyValuePairCollection processKeyValuePairs = null;
        private bool processEnd = false;
        private int messageSplitIndex = 0;
        private string documentId = "";

        #endregion
        /// <summary>
        /// This method is read only.  To create a new msg ID use the NewMessageId method.
        /// </summary>
        [XmlElement("Id")]
        public Guid Id { get => id; set => id = value; }
        [XmlElement]
        public string Type { get => type; set => type = value; }
        [XmlElement("ver")]
        public decimal Version { get => version; set => version = value; }
        [XmlElement]
        public string Format { get => format; set => format = value; }
        [XmlElement]
        public Nullable<Guid> ParentMsgId { get => parentMsgId; set => parentMsgId = value; }
        [XmlElement]
        public string Topic { get => topic; set => topic = value; }
        [XmlElement]
        public HipKeyValuePairCollection FilterKeyValuePairs { get => filterKeyValuePairs; set => filterKeyValuePairs = value; }
        [XmlElement]
        public HipKeyValuePairCollection ProcessKeyValuePairs { get => processKeyValuePairs; set => processKeyValuePairs = value; }
        [XmlElement]
        public bool ProcessEnd { get => processEnd; set => processEnd = value; }
        [XmlElement]
        public int MessageSplitIndex { get => messageSplitIndex; set => messageSplitIndex = value; }
        [XmlElement("DocId")]
        public string DocumentId { get => documentId; set => documentId = value; }

        /// <summary>
        /// This method is used when a new message ID is required.
        /// </summary>
        public void NewMessageId()
        {
            id = Guid.NewGuid();
        }

    }

    public class HipKeyValuePair
    {
        private string key = "";
        private string val = "";

        public string Key { get => key; set => key = value; }
        public string Value { get => val; set => val = value; }

        public HipKeyValuePair(string key, string val)
        {
            this.key = key;
            this.val = val;
        }
        public HipKeyValuePair()
        {
        }
    }

    [Serializable]
    public class HipKeyValuePairCollection
    {
        private List<HipKeyValuePair> list = new List<HipKeyValuePair>();

        [XmlAttribute]
        public string values { get => this.ToString(); set => this.AddConfigString(value); }

        /// <summary>
        /// This method is designed to enable the adding of configuration strings to the collection.
        /// The string needs to be formatted as 'Key1|Value1,Key2|Value2,Key3|Value3'.
        /// </summary>
        /// <param name="configString"></param>
        public HipKeyValuePairCollection(string configString)
        {
            if (configString == "" || configString == null)
            {
                //Continue without calling add code.
            }
            else
            {
                AddConfigString(configString);
            }
        }

        public HipKeyValuePairCollection()
        {
        }

        public void Add(HipKeyValuePair hipKeyValuePair)
        {
            list.Add(hipKeyValuePair);
        }

        /// <summary>
        /// This method is designed to enable the adding of configuration strings to the collection.
        /// The string needs to be formatted as 'Key1|Value1,Key2|Value2,Key3|Value3'.
        /// </summary>
        /// <param name="configString"></param>
        public void AddConfigString(string configString)
        {
            if (configString == "" || configString == null)
            {
                //Do nothing...
            }
            else
            { 
                string[] items = configString.Split(',');
                foreach (string str in items)
                {
                    if (configString.Contains("|"))
                    {
                        HipKeyValuePair pr = new HipKeyValuePair();
                        pr.Key = str.Substring(0, str.IndexOf('|'));
                        pr.Value = str.Substring(str.IndexOf('|') + 1);
                        list.Add(pr);
                    }
                }
            }
        }

        public List<KeyValuePair<string, string>> ConvertToKeyValuePairs()
        {
            List<KeyValuePair<string, string>> col = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < list.Count(); i++)
            {
                KeyValuePair<string, string> pr = new KeyValuePair<string, string>(list[i].Key, list[i].Value);
                col.Add(pr);
            }
            return col;
        }

        public int Count()
        {
            return list.Count;
        }

        public string GetMappingSubtypes()
        {
            foreach(HipKeyValuePair pr in list)
            {
                if (pr.Key.ToLower() == "subtype")
                    return "." + pr.Value;
            }
            return "";
        }

        public override string ToString()
        {
            string data = "";
            if (list != null)
            {
                foreach (HipKeyValuePair pr in list)
                {
                    data += pr.Key + "|" + pr.Value + ",";
                }
            }
            return data.TrimEnd(',');
        }
    }
    #endregion

}
