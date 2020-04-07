using System;
using System.Xml.Serialization;
using System.IO;
using System.Xml;

namespace Corp.Integration.Canonical
{

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class NewRelicData
    {

        private object[] itemsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Exception", typeof(NewRelicDataException), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("Instrumentation", typeof(NewRelicDataInstrumentation), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        [System.Xml.Serialization.XmlElementAttribute("Push", typeof(NewRelicDataPush), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public object[] Items
        {
            get
            {
                return this.itemsField;
            }
            set
            {
                this.itemsField = value;
            }
        }

        /// <summary>
        /// This method serializes an object model to an xml string.
        /// </summary>
        /// <param name="doc">The New Relic data object to be serialized</param>
        /// <returns>XML string representation of the New Relic Data</returns>
        public string ConvertToString(NewRelicData doc)
        {
            StringWriter sw = new StringWriter();
            XmlTextWriter tw = null;
            try
            {
                XmlSerializer serializer = new XmlSerializer(doc.GetType());
                tw = new XmlTextWriter(sw);
                serializer.Serialize(tw, doc);
            }
            catch (Exception ex)
            {
                throw new Exception("Error occured serializing the Corp.Integration.Canonical.InventoryTransferRequest Document object model.  The error occured in the ConvertToString method.", ex);
            }
            finally
            {
                sw.Close();
                if (tw != null)
                {
                    tw.Close();
                }
            }
            return sw.ToString();
        }

        /// <summary>
        /// This method is used to deserialize a XML representation of New Relic data to the object model.
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        public NewRelicData ConvertToObjectModel(string doc)
        {
            using (TextReader sr = new StringReader(doc))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(NewRelicData));
                NewRelicData msgEnv = (NewRelicData)serializer.Deserialize(sr);
                return msgEnv;
            }

        }

    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NewRelicDataException
    {

        private string messageField;

        private string messageIdField;

        private string documentIdField;

        private string processNameField;

        private string machineNameField;

        private DateTime timestampField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string Message
        {
            get
            {
                return this.messageField;
            }
            set
            {
                this.messageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MessageId
        {
            get
            {
                return this.messageIdField;
            }
            set
            {
                this.messageIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DocumentId
        {
            get
            {
                return this.documentIdField;
            }
            set
            {
                this.documentIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ProcessName
        {
            get
            {
                return this.processNameField;
            }
            set
            {
                this.processNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MachineName
        {
            get
            {
                return this.machineNameField;
            }
            set
            {
                this.machineNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime Timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NewRelicDataInstrumentation
    {

        private NewRelicDataInstrumentationRecord[] recordField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Record", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public NewRelicDataInstrumentationRecord[] Record
        {
            get
            {
                return this.recordField;
            }
            set
            {
                this.recordField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NewRelicDataInstrumentationRecord
    {

        private string messageIdField;

        private string documentIdField;

        private string processNameField;

        private string machineNameField;

        private DateTime timestampField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MessageId
        {
            get
            {
                return this.messageIdField;
            }
            set
            {
                this.messageIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string DocumentId
        {
            get
            {
                return this.documentIdField;
            }
            set
            {
                this.documentIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ProcessName
        {
            get
            {
                return this.processNameField;
            }
            set
            {
                this.processNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MachineName
        {
            get
            {
                return this.machineNameField;
            }
            set
            {
                this.machineNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime Timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class NewRelicDataPush
    {

        private string sqsQueueNameField;

        private int processedMessageCountField;

        private string processNameField;

        private string machineNameField;

        private DateTime activationTimestampField;

        private DateTime activationEndTimestampField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string SqsQueueName
        {
            get
            {
                return this.sqsQueueNameField;
            }
            set
            {
                this.sqsQueueNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public int ProcessedMessageCount
        {
            get
            {
                return this.processedMessageCountField;
            }
            set
            {
                this.processedMessageCountField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ProcessName
        {
            get
            {
                return this.processNameField;
            }
            set
            {
                this.processNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string MachineName
        {
            get
            {
                return this.machineNameField;
            }
            set
            {
                this.machineNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime ActivationTimestamp
        {
            get
            {
                return this.activationTimestampField;
            }
            set
            {
                this.activationTimestampField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public DateTime ActivationEndTimestamp
        {
            get
            {
                return this.activationEndTimestampField;
            }
            set
            {
                this.activationEndTimestampField = value;
            }
        }
    }


}
