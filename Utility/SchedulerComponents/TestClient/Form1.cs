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

namespace TestClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSingletonOnRamp_Click(object sender, EventArgs e)
        {
            Tasks tasks = new Tasks();

            tasks.OnRampSingletonInitializer(Guid.NewGuid().ToString(), "http://hip-development-loadbalancer-1-1047608554.us-west-2.elb.amazonaws.com/AppServices/BaozunAppService/SalesDocOnRampSvc.v1/ProcessBaozunBatchFiles.svc");


        }
    }
}
