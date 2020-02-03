using System;

namespace BC.Integration.Utility.NotificationSvc
{
    /// <summary>
    /// Notification Log class defining properties and access modifiers
    /// </summary>
    public class NotificationLog
    {
        private int notificationLogId;
        private int? notificationId;
        private string processName;
        private string serviceId;
        private string messageId;
        private string issueCategory;
        private string issueMessage;
        private string documentId;
        private int isProcessed;
        private DateTime? processedTimestamp;

        public int NotificationLogId
        {
            get { return notificationLogId; }
            set { notificationLogId = value; }
        }

        public int? NotificationId
        {
            get { return notificationId; }
            set { notificationId = value; }
        }

        public string ProcessName
        {
            get { return processName; }
            set { processName = value; }
        }

        public string ServiceId
        {
            get { return serviceId; }
            set { serviceId = value; }
        }

        public string MessageId
        {
            get { return messageId; }
            set { messageId = value; }
        }

        public string IssueCategory
        {
            get { return issueCategory; }
            set { issueCategory = value; }
        }

        public string IssueMessage
        {
            get { return issueMessage; }
            set { issueMessage = value; }
        }

        public string DocumentId
        {
            get { return documentId; }
            set { documentId = value; }
        }

        public int IsProcessed
        {
            get { return isProcessed; }
            set { isProcessed = value; }
        }

        public DateTime? ProcessedTimestamp
        {
            get { return processedTimestamp; }
            set { processedTimestamp = value; }
        }
    }
}