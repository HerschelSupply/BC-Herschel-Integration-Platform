using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Diagnostics;

namespace NotificationSvcEndPointDefinition
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "NotificationService" in both code and config file together.
    public class NotificationService : INotificationService
    {
        public bool InitializeNotification(string processName, string serviceId, string issueCategory)
        {
            Trace.WriteLine("Notification Service called successfully...");
            return true;
        }
    }
}
