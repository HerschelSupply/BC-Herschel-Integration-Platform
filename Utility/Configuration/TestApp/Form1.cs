using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.IO.MemoryMappedFiles;
using System.Threading;
using System.IO;

namespace TestApp
{
    public partial class Form1 : Form
    {
        private Mutex mutex;
        public Form1()
        {
            InitializeComponent();
        }

        private void btnEndRequest_Click(object sender, EventArgs e)
        {
            //QueueTracker.FinishedProcessingQueue(txtQueueName.Text);
            //txtQueueNameHistory.Text += "End: " + txtQueueName.Text + ", Ended\r\n";
            mutex.Close();
            mutex.Dispose();

        }

        private void btnStartRequest_Click(object sender, EventArgs e)
        {
            MemoryMappedFile mmf = MemoryMappedFile.CreateOrOpen("Queues", 10000);
           
            bool mutexCreated;
            //Mutex mutex = null;
            bool result = Mutex.TryOpenExisting(txtQueueName.Text, out mutex);
            if (!result)
            {
                mutex = new Mutex(true, txtQueueName.Text, out mutexCreated);
                mutex.ReleaseMutex();
                txtStartResponse.Text = "Process Queue.";
                txtQueueNameHistory.Text += "Start: " + txtQueueName.Text + ", Started\r\n";
            }
            else
            {
                txtStartResponse.Text = "Already exists.";
                txtQueueNameHistory.Text += "Start: " + txtQueueName.Text + ", Blocked\r\n";
            }
        }




        private bool MmfContainsQueue(string csv, string queueName)
        {
            string[] arr = csv.Split(',');
            foreach (string queue in arr)
            {
                if (queue == queueName)
                    return true;
            }
            return false;
        }


    }
}
