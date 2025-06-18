using EchoB.Domain.Entities;
using EchoB.Domain.Enums;
using EchoB.Domain.Interfaces;
using EchoB.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EchoB.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly EchoBDbContext _context;

        public NotificationRepository(EchoBDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Notification notification)
        {
            _context.Notifications.Update(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<Notification?> GetByIdAsync(string id)
        {
            return await _context.Notifications.FindAsync(id);
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(string userId, int count = 20)
        {
            return await _context.Notifications
                .Where(n => n.UserId == Guid.Parse(userId))
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }
        public async Task<List<Notification>> GetPendingNotificationsAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == Guid.Parse(userId) && n.Status == NotificationStatus.Pending)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }

    }
}
