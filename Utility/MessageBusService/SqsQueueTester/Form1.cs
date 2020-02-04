using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Corp.Integration.Utility;

namespace SqsQueueTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnPush_Click(object sender, EventArgs e)
        {
            try
            {
                int retryCount = 0;
                SqsPublishService svc = new SqsPublishService();
                svc.Push("https://sqs.us-west-2.amazonaws.com/879101183817/HIP-SMQ-Test", "Test message", out retryCount);
            }
            catch (Exception ex)
            {
                txtResult.Text = ex.Message;
            }
        }

        private void btnHeartbeat_Click(object sender, EventArgs e)
        {
            bool result = false;
            HeartBeat.HeartBeatClient hbClient = new HeartBeat.HeartBeatClient();
            try
            {
                result = hbClient.Pulse();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }
            if (result)
            {
                MessageBox.Show("All is well...");
            }
            else
            {
                MessageBox.Show("Ooops...");
            }
        }
    }
}
