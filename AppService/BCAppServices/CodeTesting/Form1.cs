using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BC.Integration.AppService.BC;
using System.Xml;
using System.Web;
using BC.Integration;
using System.Activities;
using BC.Integration.Utility;
using System.ServiceModel;

namespace CodeTesting
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            ProcessBCMsgFiles(@"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\SalesDoc Post 3cb96880-a389-412c-8731-cd8b01f257f6.xml");
            //ProcessBCMsgFiles(@"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\Sales Receive 8f1b6aa3-8465-414b-9350-8e07b342fa35.xml");
            //ProcessBCMsgFiles(@"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\Sales Receive be42fb8a-1678-488c-9183-eae21a538c2d.xml");
            //ProcessBCMsgFiles(@"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\Sales Receive c67526a2-6472-4155-9be5-a6efaa8e8301.xml");
            //ProcessBCMsgFiles(@"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\SalesDoc Post f2e37d70-ebf1-4886-8d63-72e5d2364b21.xml");
            //ProcessBCMsgFiles(@"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\Sales Receive 24bb3902-ba3d-44b8-ae54-229d88c722eb.xml");
        }

        private void ProcessBCMsgFiles(string path)
        {

            XmlDocument msgXml = new XmlDocument();
            msgXml.Load(path);

            string body = msgXml.SelectSingleNode(@"/MsgEnvelope/Body").InnerText;
            ProcessSalesDoc salesDoc = new ProcessSalesDoc();
            string msg = salesDoc.CreateBCMsg(body);

            XmlDocument BCXml = new XmlDocument();
            BCXml.LoadXml(msg);
            

            InstrumentationDB inst = new InstrumentationDB();
            inst.WriteMsgToFile(@"C:\Temp\FTP Test", "GpBC", BCXml, "BC", "OffRamp");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //string path = @"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\SalesDoc Post 3cb96880-a389-412c-8731-cd8b01f257f6.xml";
            ////string path = @"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\Sales Receive 8f1b6aa3-8465-414b-9350-8e07b342fa35.xml";
            ////string path = @"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\Sales Receive be42fb8a-1678-488c-9183-eae21a538c2d.xml";
            ////string path = @"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\Sales Receive c67526a2-6472-4155-9be5-a6efaa8e8301.xml";
            ////string path = @"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\SalesDoc Post f2e37d70-ebf1-4886-8d63-72e5d2364b21.xml";
            string path = @"C:\HIP Herschel Integration Platform\AppService\GPAppServices\CodeTesting\test.xml";

            XmlDocument msgXml = new XmlDocument();
            msgXml.Load(path);

            //string msg = "<Invoice xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://BC.Integration.Schema.ECommerce.RetailChannelInvoice\"><Process xmlns=\"\">PerfTest</Process><Header xmlns=\"\"><InvoiceNumber>E0561231</InvoiceNumber><InvoiceDate>2017-09-13</InvoiceDate><InvoiceType>Return</InvoiceType><Currency>CYN</Currency><SiteId>51</SiteId><ShipmentDate>0001-01-01T00:00:00</ShipmentDate><FreightCost>0</FreightCost><FreightTax>0</FreightTax><FreightTotal>0</FreightTotal><TotalLineItemAmountBeforeTax>0</TotalLineItemAmountBeforeTax><TotalTax>0</TotalTax><TotalAmount>80</TotalAmount></Header><LineItems xmlns=\"\"><LineItem><ItemNumber>1234567888</ItemNumber><UnitPrice>80</UnitPrice><ActualPrice>80</ActualPrice><Quantity>1</Quantity><Discount>0</Discount><ExtendedPrice>0</ExtendedPrice><LineItemTaxes>0</LineItemTaxes><LineItemTotalAmount>80</LineItemTotalAmount><ReturnedInPerfectCondition>1</ReturnedInPerfectCondition><ReturnedDamaged>0</ReturnedDamaged><ReturnedInUnknownCondition>0</ReturnedInUnknownCondition></LineItem></LineItems></Invoice>";
            //string msg = "<?xml version=\"1.0\" encoding=\"utf - 16\"?><MsgEnvelope xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"><Interchange><Id>c0d4fbcd-8077-490e-803a-f64f9a38f8e1</Id><EntryPoint>PerfTestService</EntryPoint><Timestamp>2018-01-05T11:07:42.3472022-08:00</Timestamp><ProcessName>PerfTest</ProcessName><BatchId>00000000-0000-0000-0000-000000000000</BatchId></Interchange><Msg><Id>c5d03939-f7b3-4242-9e06-46892d1054e4</Id><Type>BC.Integration.RetailChannel.Invoice</Type><ver>1</ver><Format>XML</Format><ParentMsgId>a744bf7f-1797-4f95-a8bb-3cd4d71dac1f</ParentMsgId><Topic /><FilterKeyValuePairs /><ProcessEnd>false</ProcessEnd><MessageSplitIndex>19</MessageSplitIndex><DocId>TBHE4709000020</DocId></Msg><Svc><SvcId>SqsPushService</SvcId><SvcInstId>5cc63ce8-0697-4531-900f-7c462552e5a1</SvcInstId><ver>1</ver><SvcOpId>MessageSubscriptionPull</SvcOpId><SvcOpInstId>f5095ec6-dbc9-4625-8730-fc6ff8205c38</SvcOpInstId><SvcPostId>MessageSubscriptionPush</SvcPostId><SvcPostInstId>723d30b0-966f-4fff-9a32-2ae8c049c2f3</SvcPostInstId></Svc><Body>&lt;Invoice xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns=\"http://BC.Integration.Schema.ECommerce.RetailChannelInvoice\"&gt;&lt;Process xmlns=\"\"&gt;PerfTest&lt;/Process&gt;&lt;Header xmlns=\"\"&gt;&lt;InvoiceNumber&gt;TBHE4709000020&lt;/InvoiceNumber&gt;&lt;InvoiceDate&gt;2017-09-12&lt;/InvoiceDate&gt;&lt;InvoiceType&gt;Sales&lt;/InvoiceType&gt;&lt;Currency&gt;CYN&lt;/Currency&gt;&lt;SiteId&gt;51&lt;/SiteId&gt;&lt;ShipmentDate&gt;0001-01-01T00:00:00&lt;/ShipmentDate&gt;&lt;FreightCost&gt;10&lt;/FreightCost&gt;&lt;FreightTax&gt;0&lt;/FreightTax&gt;&lt;FreightTotal&gt;0&lt;/FreightTotal&gt;&lt;TotalLineItemAmountBeforeTax&gt;0&lt;/TotalLineItemAmountBeforeTax&gt;&lt;TotalTax&gt;0&lt;/TotalTax&gt;&lt;TotalAmount&gt;430&lt;/TotalAmount&gt;&lt;/Header&gt;&lt;LineItems xmlns=\"\"&gt;&lt;LineItem&gt;&lt;ItemNumber&gt;1356789123&lt;/ItemNumber&gt;&lt;UnitPrice&gt;100&lt;/UnitPrice&gt;&lt;ActualPrice&gt;90&lt;/ActualPrice&gt;&lt;Quantity&gt;2&lt;/Quantity&gt;&lt;Discount&gt;10&lt;/Discount&gt;&lt;ExtendedPrice&gt;0&lt;/ExtendedPrice&gt;&lt;LineItemTaxes&gt;0&lt;/LineItemTaxes&gt;&lt;LineItemTotalAmount&gt;180&lt;/LineItemTotalAmount&gt;&lt;ReturnedInPerfectCondition&gt;0&lt;/ReturnedInPerfectCondition&gt;&lt;ReturnedDamaged&gt;0&lt;/ReturnedDamaged&gt;&lt;ReturnedInUnknownCondition&gt;0&lt;/ReturnedInUnknownCondition&gt;&lt;/LineItem&gt;&lt;LineItem&gt;&lt;ItemNumber&gt;1234567888&lt;/ItemNumber&gt;&lt;UnitPrice&gt;80&lt;/UnitPrice&gt;&lt;ActualPrice&gt;80&lt;/ActualPrice&gt;&lt;Quantity&gt;3&lt;/Quantity&gt;&lt;Discount&gt;0&lt;/Discount&gt;&lt;ExtendedPrice&gt;0&lt;/ExtendedPrice&gt;&lt;LineItemTaxes&gt;0&lt;/LineItemTaxes&gt;&lt;LineItemTotalAmount&gt;240&lt;/LineItemTotalAmount&gt;&lt;ReturnedInPerfectCondition&gt;0&lt;/ReturnedInPerfectCondition&gt;&lt;ReturnedDamaged&gt;0&lt;/ReturnedDamaged&gt;&lt;ReturnedInUnknownCondition&gt;0&lt;/ReturnedInUnknownCondition&gt;&lt;/LineItem&gt;&lt;/LineItems&gt;&lt;/Invoice&gt;</Body></MsgEnvelope>";
            string encodedMsg = System.Web.HttpUtility.HtmlEncode(msgXml.OuterXml);

            //SalesDocWcfSvc.SalesDocWcfServiceClient client = new SalesDocWcfSvc.SalesDocWcfServiceClient();
            WcfDestinationService.WcfMessagingServiceClient client = new WcfDestinationService.WcfMessagingServiceClient();
            //EndpointAddress address = new EndpointAddress(@"http://hip-development-loadbalancer-1-1047608554.us-west-2.elb.amazonaws.com/AppServices/GpAppService/SalesDocWcfOffRamp.v1/WcfMessagingService.svc");
            EndpointAddress address = new EndpointAddress(@"http://localhost:36072/WcfMessagingService.svc");
            client.Endpoint.Address = address;
                
            //client.InitializeProcess(msgXml.OuterXml);
            bool response = client.InitializeProcess(encodedMsg);



        }
    }
}
