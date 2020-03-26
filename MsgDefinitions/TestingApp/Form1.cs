using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Corp.Integration;
using System.IO;
using System.Xml;

namespace TestingApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnFilterToSting_Click(object sender, EventArgs e)
        {
            XmlDocument msgXml = new XmlDocument();
            msgXml.Load(@"C:\HIP Herschel Integration Platform\MsgDefinitions\TestingApp\SalesDoc Post b2f0d4ca-3bb1-496c-9669-9713f06840eb.xml");
            MessageManager mgr = new MessageManager();
            MsgEnvelope env = mgr.ConvertHipEnvelopeToOm(msgXml);

            string data = env.Msg.FilterKeyValuePairs.ToString();

        }
    }
}
