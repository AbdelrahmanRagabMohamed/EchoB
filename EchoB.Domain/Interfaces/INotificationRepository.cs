using EchoB.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EchoB.Domain.Interfaces
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task UpdateAsync(Notification notification);
        Task<Notification?> GetByIdAsync(string id);
        Task<List<Notification>> GetUserNotificationsAsync(string userId, int count = 20);
        Task<List<Notification>> GetPendingNotificationsAsync(string userId);

    }
}
