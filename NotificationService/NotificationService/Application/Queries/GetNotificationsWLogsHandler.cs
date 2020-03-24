using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NotificationService.Model;

namespace NotificationService.Application.Queries
{
    public class GetNotificationsWLogsHandler : IRequestHandler<GetNotificationsWLogsQuery, GetNotificationsWLogsDTO>
    {
        private NotificationContext _context;

        public GetNotificationsWLogsHandler(NotificationContext context)
        {
            _context = context;
        }

        public async Task<GetNotificationsWLogsDTO> Handle(GetNotificationsWLogsQuery request, CancellationToken cancellationToken)
        {
            var notificationData = await _context.Notifications.ToListAsync();
            var notificationLogData = await _context.Notification_logs.ToListAsync();
            var notifList = new List<NotificationDTO>();

            foreach (var x in notificationData)
            {
                var total = 0;
                var read_count = 0;
                var logList = new List<NotificationLogData>();
                var logs = notificationLogData.Where(y => y.Notification_id == x.Id);

                foreach(var y in logs)
                {
                    total++;
                    if(y.Read_at != null)
                    {
                        read_count++;
                    }
                    logList.Add(new NotificationLogData
                    {
                       Notification_id = y.Notification_id,
                       From = y.From,
                       Read_at = y.Read_at,
                       Target = y.Target
                    });
                }

                notifList.Add(new NotificationDTO()
                {
                    Total = total,
                    Read_count = read_count,
                    Notifications = new NotificationData()
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Message = x.Message
                    },
                    Notification_logs = logList
                });
            }

            return new GetNotificationsWLogsDTO()
            {
                Message = "Successfully retrieving data",
                Success = true,
                Data = notifList
            };
        }
    }
}
