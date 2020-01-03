using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using BC.Integration.Utility;
using BC.Integration.Utility;

namespace InstrumentationTestApp
{
    public partial class Form1 : Form
    {
        InstrumentationDB inst = new InstrumentationDB();

        private List<KeyValuePair<string, string>> configuration = null;
        private string instrumentationConn = "Dsn=HipMgmtAuroraDB;data source={'DRIVER={MySQL ODBC 5.3 ANSI Driver}};Trusted_Connection = true'}";
        private string InstrumentationLevel = "normal";
        private string retryCount = "1";
        private string retryInterval = "5";
        private string tracingEnabled = "true";


        public Form1()
        {
            InitializeComponent();

            configuration = new List<KeyValuePair<string, string>>();
            configuration.Add( new KeyValuePair<string, string>("InstrumentationConn", instrumentationConn));
            configuration.Add( new KeyValuePair<string, string>("InstrumentationLevel", InstrumentationLevel));
            configuration.Add( new KeyValuePair<string, string>("DatabaseRetryCount", retryCount));
            configuration.Add( new KeyValuePair<string, string>("DatabaseRetryInterval", retryInterval));
            configuration.Add( new KeyValuePair<string, string>("TracingEnabled", tracingEnabled));
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(txtXmlPath.Text);
            inst.LogActivity(xml, "normal",0);
        }

        private void btnExceptionMsg_Click(object sender, EventArgs e)
        {
            XmlDocument xml = new XmlDocument();
            xml.Load(txtXmlPath.Text);
            
            Exception innerInner = new Exception("Inner inner exception....");
            Exception inner = new Exception("Inner Exception....", innerInner);
            Exception ex = new Exception("First Exception....", inner);

            //inst.LogException(null, ex, "normal");
            //inst.LogException(xml, ex, "normal");

            inst.FlushActivity();

        }

        private void btnFlush_Click(object sender, EventArgs e)
        {

            inst.FlushActivity();

        }


        private void btnSendActivityToAurora_Click(object sender, EventArgs e)
        {
            //Load sample data
            XmlDocument xmlDoc = new XmlDocument();
            //xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc OnRamp 80c43ec3-33b4-4588-aaf6-b80e4c6640d8.xml");
            //xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc Post 983c442f-a83e-437a-ba34-3de79490bfdb.xml");
            //xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc OnRamp 35b2df4b-e73a-46c6-afc4-384bfb2d8595.xml");
            //xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc Post 8f4f9af3-1ef2-4fcf-84ca-6bcad9e175e8.xml");
            //xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc OnRamp 58f6d920-ebbd-4c27-b917-4ac1d89f806a.xml");
            xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc Post 5a96838a-17d4-4174-8f27-b535c93b38a3.xml");


            AuroraConnector connector = new AuroraConnector(configuration);
            connector.LogActivity(xmlDoc, @"http://SomeService/SomeWhere/SomeHow", 0);
        }

        private void btnExceptionDbCall_Click(object sender, EventArgs e)
        {
            XmlDocument msgXml = new XmlDocument();
            ////xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc OnRamp 80c43ec3-33b4-4588-aaf6-b80e4c6640d8.xml");
            ////xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc Post 983c442f-a83e-437a-ba34-3de79490bfdb.xml");
            ////xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc OnRamp 35b2df4b-e73a-46c6-afc4-384bfb2d8595.xml");
            ////xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc Post 8f4f9af3-1ef2-4fcf-84ca-6bcad9e175e8.xml");
            ////xmlDoc.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc OnRamp 58f6d920-ebbd-4c27-b917-4ac1d89f806a.xml");
            //msgXml.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\SalesDoc Post 5a96838a-17d4-4174-8f27-b535c93b38a3.xml");

            //AuroraConnector connector = new AuroraConnector(configuration);
            ////connector.LogExceptions(Guid.NewGuid(), Guid.NewGuid(), "Message Body", "Exception Msg", "Stack Trace");
            //connector.LogExceptions(new Guid(msgXml.SelectSingleNode("/MsgEnvelope/Interchange/Id").InnerText),
            //                    new Guid(msgXml.SelectSingleNode("/MsgEnvelope/Msg/Id").InnerText),
            //                    msgXml.OuterXml, "Exception Msg", "Stack Trace");


            msgXml.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\XML Pull 5dc3ede8-353d-45fc-abf3-9375a7454d2d.xml");
            inst.Configuration = configuration;
            inst.LogActivity(msgXml, "normal", 0);

        }

        private void btnLogMessagingException_Click(object sender, EventArgs e)
        {
            XmlDocument msgXml = new XmlDocument();
            msgXml.Load(@"C:\HIP Herschel Integration Platform\Utility\Instrumentation\InstrumentationTestApp\ExceptionTestMessage.xml");

            Exception innerInner = new Exception("Inner inner exception....");
            Exception inner = new Exception("Inner Exception....", innerInner);
            Exception ex = new Exception("First Exception....", inner);

            inst.Configuration = configuration;
            inst.LogMessagingException("Exception Message", null, ex);

        }
    }
}
