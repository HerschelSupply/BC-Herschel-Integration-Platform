using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using BC.Integration.Utility.NotificationSvc;
using BC.Integration.Interfaces;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using BC.Integration.Utility;

namespace BC.Integration.Utility.NotificationSvc
{
    public class ProcessNotifications
    {
        #region Properties

        private List<KeyValuePair<string, string>> configuration = null;
        private bool tracingEnabled = false;

        //Dependency Injection Components
        IConfiguration config;
        IInstrumentation instrumentation;
        UnityContainer container;

        #endregion

        public bool Execute(string processName, string serviceId, string issueCategory)
        {
            Trace.WriteLineIf(tracingEnabled,"BC.Integration.Utility.NotificationSvc.ProcessNotifications.Execute() method initializing. ");

            try
            {
                CreateDiComponents();

                NotificationSvcDbConnection dbConn = new NotificationSvcDbConnection();

                List<NotificationLog> notificationsLogsUnProcessed;
                List<NotificationLog> notificationsLogsProcessed;

                //Get notification configurations
                List<string> notificationConfiguration = dbConn.GetNotificationConfiguration(processName, serviceId, issueCategory);

                if (notificationConfiguration != null && notificationConfiguration.Any())
                {
                    //Get unprocessed NotificationLogs
                    notificationsLogsUnProcessed = dbConn.GetNotificationLogs(0, processName, serviceId, issueCategory);

                    //Get processed NotificationLogs
                    notificationsLogsProcessed = dbConn.GetNotificationLogs(1, processName, serviceId, issueCategory, Convert.ToInt32(notificationConfiguration[6]));

                    if (notificationsLogsProcessed != null && notificationsLogsProcessed.Any())
                    {
                        //exclude already processed notification logs if they are present in unprocessed notification logs
                        var alreadyProcessedNotifications = from unProcessed in notificationsLogsUnProcessed
                                                            from processed in notificationsLogsProcessed
                                                            where ((unProcessed.IssueMessage == processed.IssueMessage) && (unProcessed.DocumentId == processed.DocumentId))
                                                            select unProcessed;

                        var notificationsLogs = notificationsLogsUnProcessed.Except(alreadyProcessedNotifications);

                        //remove duplicate unprocessed and processed notification logs
                        var notificationLog = notificationsLogs.GroupBy(x => new { x.IssueMessage, x.DocumentId }).Select(y => y.FirstOrDefault());

                        var listIssueMessage = notificationLog.Select(o => o.IssueMessage);

                        var listNotificationLogIds = notificationsLogsUnProcessed.Select(o => o.NotificationLogId);

                        if (notificationLog != null && notificationLog.Any())
                        {
                            Email email = new Email();
                            string emailbody = email.SendEmail(notificationConfiguration[0], notificationConfiguration[1], notificationConfiguration[2], listIssueMessage.ToList(), notificationConfiguration[3]);

                            int notificationId = dbConn.InsertNotification(Convert.ToInt32(notificationConfiguration[7]), notificationConfiguration[2], emailbody, notificationConfiguration[0], notificationConfiguration[1]);

                            //Set IsProcessed flag to true
                            dbConn.UpdateNotificationLog(1, notificationId, listNotificationLogIds.ToList());
                        }
                        else if (notificationConfiguration[4] == "1")
                        {
                            Email email = new Email();
                            email.SendEmail(notificationConfiguration[0], notificationConfiguration[1], notificationConfiguration[2], notificationConfiguration[5]);
                        }
                    }
                    else if (notificationsLogsUnProcessed != null && notificationsLogsUnProcessed.Any())
                    {
                        //remove duplicate unprocessed notification logs
                        var notificationLogs = notificationsLogsUnProcessed.GroupBy(x => new { x.IssueMessage, x.DocumentId }).Select(y => y.FirstOrDefault());

                        var listIssueMessage = notificationLogs.Select(o => o.IssueMessage);

                        var listNotificationLogIds = notificationsLogsUnProcessed.Select(o => o.NotificationLogId);

                        if (notificationLogs != null && notificationLogs.Any())
                        {
                            Email email = new Email();
                            string emailbody = email.SendEmail(notificationConfiguration[0], notificationConfiguration[1], notificationConfiguration[2], listIssueMessage.ToList(), notificationConfiguration[3]);

                            int notificationId = dbConn.InsertNotification(Convert.ToInt32(notificationConfiguration[7]), notificationConfiguration[2], emailbody, notificationConfiguration[0], notificationConfiguration[1]);

                            //Set IsProcessed flag to true
                            dbConn.UpdateNotificationLog(1, notificationId, listNotificationLogIds.ToList());
                        }
                        else if (notificationConfiguration[4] == "1")
                        {
                            Email email = new Email();
                            email.SendEmail(notificationConfiguration[0], notificationConfiguration[1], notificationConfiguration[2], notificationConfiguration[5]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLineIf(tracingEnabled, "BC.Integration.Utility.NotificationSvc service exception occurred. Exception message: " + ex.Message);
                instrumentation.LogGeneralException("An exception was raised when calling the BC.Integration.AppService.NotificationService.ProcessNotification method.", ex);
            }

            Trace.WriteLineIf(tracingEnabled, "BC.Integration.Utility.NotificationSvc.ProcessNotifications.Execute() method finished. ");
            return true;
        }


        /// <summary>
        /// Instantiates the objects that are used via dependency injection
        /// </summary>
        private void CreateDiComponents()
        {
            //Create Unity container for DI and create DI components
            container = new UnityContainer();
            container.LoadConfiguration();
            config = container.Resolve<IConfiguration>();
            configuration = config.PopulateConfigurationCollectionFromAppConfig();
            PopulateLocalVaribale();
            instrumentation = container.Resolve<IInstrumentation>();
            instrumentation.Configuration = configuration;
            Trace.WriteLineIf(tracingEnabled, "NotificationService: Unity container config complete...");
        }

        /// <summary>
        /// Applies the central configurations to the loacal properties.
        /// </summary>
        /// <param name="configCol">The key value pair collection containing the configuration values.</param>
        private void PopulateLocalVaribale()
        {
            string val = Utilities.GetConfigurationValue(configuration, "TracingEnabled");
            if (val != "")
            {
                if (val.ToLower() == "true" || val.ToLower() == "false")
                {
                    tracingEnabled = Convert.ToBoolean(val);
                }

                if (val == "1" || val == "0")
                {
                    tracingEnabled = Convert.ToBoolean(Convert.ToInt16(val));
                }
            }
        }
    }
}