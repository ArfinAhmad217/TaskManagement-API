using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Notification;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class NotificationService
    {
        private readonly ApplicationDbContext _context;

        public NotificationService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Internal helper — TaskService isko call karega
        public async Task CreateNotification(int userId, int? taskItemId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                TaskItemId = taskItemId,
                Message = message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        public async Task<List<NotificationResponse>> GetUserNotifications(int userId)
        {
            var notifications = await _context.Notifications
                .Include(x => x.TaskItem)
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return notifications.Select(x => new NotificationResponse
            {
                Id = x.Id,
                TaskItemId = x.TaskItemId,
                TaskTitle = x.TaskItem?.Title,
                Message = x.Message,
                IsRead = x.IsRead,
                CreatedAt = x.CreatedAt
            }).ToList();
        }

        public async Task<NotificationResponse> MarkAsRead(int id, int userId)
        {
            var notification = await _context.Notifications
                .Include(x => x.TaskItem)
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (notification == null)
                throw new Exception("Notification not found.");

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return new NotificationResponse
            {
                Id = notification.Id,
                TaskItemId = notification.TaskItemId,
                TaskTitle = notification.TaskItem?.Title,
                Message = notification.Message,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt
            };
        }
    }
}