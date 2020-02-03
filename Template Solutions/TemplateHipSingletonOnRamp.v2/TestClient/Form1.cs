using System;
using System.Windows.Forms;


namespace SqsQueueTester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnActivate_Click(object sender, EventArgs e)
        {
            SalesWebService.WebSvcClient client = new SalesWebService.WebSvcClient();
            client.InitializeProcess(Guid.NewGuid().ToString());
        }

        private void btmPayment_Click(object sender, EventArgs e)
        {
            string paymentMethod = "Check";
            string paymentMethodType = "";

            if(paymentMethod.ToLower() == "cash" || paymentMethod.ToLower() == "debit card")
            {
                paymentMethodType = "Cash";
            }
            else if (paymentMethod.ToLower() == "check")
            {
                paymentMethodType = "Check";
            }
            else
            {
                paymentMethodType = "Credit";
            }
        }
    }
}
