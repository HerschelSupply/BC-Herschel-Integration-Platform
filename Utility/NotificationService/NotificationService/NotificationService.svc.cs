using System;
using System.Diagnostics;


namespace BC.Integration.Utility.NotificationSvc
{
    public class NotificationService : INotificationService
    {
        public bool InitializeNotification(string processName, string serviceId, string issueCategory)
        {
            Trace.WriteLine("BC.Integration.Utility.NotificationSvc service initializing. ");
            try
            {
                ProcessNotifications processNotifications = new ProcessNotifications();
                return processNotifications.Execute(processName, serviceId, issueCategory);
            }
            catch (Exception ex)
            {
                Trace.WriteLine("BC.Integration.Utility.NotificationSvc service exception occurred. Exception message: " + ex.Message);
            }
            return false;
        }
    }
}
