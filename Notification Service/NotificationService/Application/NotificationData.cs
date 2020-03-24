using System;
using System.Collections.Generic;
using NotificationService.Model;

namespace NotificationService.Application
{
    public class NotificationDTO
    {
        public int Total { get; set; }
        public int Read_count { get; set; }
        public NotificationData Notifications { get; set; }
        public List<NotificationLogData> Notification_logs { get; set; }
    }
        
    public class NotificationData
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Message { get; set; }
    }

    public class NotificationLogData
    {
        public int Notification_id { get; set; }
        public int From { get; set; }
        public int Target { get; set; }
        public Nullable<DateTime> Read_at { get; set; }
    }
}
