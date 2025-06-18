using EchoB.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoB.Domain.Entities
{
    public class Notification : BaseEntity
    {
        public Guid UserId { get; set; }
        public NotificationType Type { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Data { get; set; }
        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public static Notification Create(Guid userId, NotificationType type, string title, string content, string? data = null)
        {
            return new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = type,
                Title = title,
                Content = content,
                Data = data,
                Status = NotificationStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };
        }
    }

}
