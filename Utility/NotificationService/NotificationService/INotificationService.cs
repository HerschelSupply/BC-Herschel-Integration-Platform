using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace BC.Integration.Utility.NotificationSvc
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "INotificationService" in both code and config file together.
    [ServiceContract]
    public interface INotificationService
    {
        [OperationContract]
        bool InitializeNotification(string processName, string serviceId, string issueCategory);
    }
}
