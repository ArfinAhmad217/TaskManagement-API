using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Task;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class TaskService
    {
        private readonly ApplicationDbContext _context;
        private static readonly string[] AllowedStatuses = { "ToDo", "InProgress", "Done" };
        private static readonly string[] AllowedPriorities = { "Low", "Medium", "High" };
        private readonly NotificationService _notificationService;

        public TaskService(ApplicationDbContext context, NotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        public async Task<TaskResponse> CreateTask(CreateTaskRequest request, int createdByUserId)
        {
            if (!AllowedPriorities.Contains(request.Priority))
                throw new Exception("Invalid priority. Allowed: Low, Medium, High.");

            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == request.TeamId);
            if (team == null)
                throw new Exception("Team not found.");

            if (request.AssignedToUserId.HasValue)
            {
                var isMember = await _context.TeamMembers
                    .AnyAsync(x => x.TeamId == request.TeamId && x.UserId == request.AssignedToUserId.Value);

                if (!isMember)
                    throw new Exception("Assigned user is not a member of this team.");
            }

            var task = new TaskItem
            {
                Title = request.Title.Trim(),
                Description = request.Description,
                Status = "ToDo",
                Priority = request.Priority,
                Deadline = request.Deadline,
                TeamId = request.TeamId,
                AssignedToUserId = request.AssignedToUserId,
                CreatedByUserId = createdByUserId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();

            if (task.AssignedToUserId.HasValue)
            {
                await _notificationService.CreateNotification(
                    task.AssignedToUserId.Value,
                    task.Id,
                    $"You have been assigned a new task: \"{task.Title}\"");
            }

            return await GetTaskById(task.Id);
        }

        public async Task<TaskResponse> UpdateTask(int id, UpdateTaskRequest request)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (task == null) throw new Exception("Task not found.");

            if (!string.IsNullOrWhiteSpace(request.Title))
                task.Title = request.Title.Trim();

            if (request.Description != null)
                task.Description = request.Description;

            if (!string.IsNullOrWhiteSpace(request.Priority))
            {
                if (!AllowedPriorities.Contains(request.Priority))
                    throw new Exception("Invalid priority.");
                task.Priority = request.Priority;
            }

            if (request.Deadline.HasValue)
                task.Deadline = request.Deadline;

            if (request.AssignedToUserId.HasValue)
            {
                var isMember = await _context.TeamMembers
                    .AnyAsync(x => x.TeamId == task.TeamId && x.UserId == request.AssignedToUserId.Value);

                if (!isMember)
                    throw new Exception("Assigned user is not a member of this team.");

                task.AssignedToUserId = request.AssignedToUserId;
            }

            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return await GetTaskById(task.Id);
        }

        public async Task<TaskResponse> UpdateStatus(int id, string status)
        {
            if (!AllowedStatuses.Contains(status))
                throw new Exception("Invalid status. Allowed: ToDo, InProgress, Done.");

            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (task == null) throw new Exception("Task not found.");

            task.Status = status;
            task.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            if (task.AssignedToUserId.HasValue)
            {
                await _notificationService.CreateNotification(
                    task.AssignedToUserId.Value,
                    task.Id,
                    $"Task \"{task.Title}\" status updated to {status}");
            }

            return await GetTaskById(task.Id);
        }

        public async Task DeleteTask(int id)
        {
            var task = await _context.Tasks.FirstOrDefaultAsync(x => x.Id == id);
            if (task == null) throw new Exception("Task not found.");

            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }

        public async Task<TaskResponse> GetTaskById(int id)
        {
            var task = await _context.Tasks
                .Include(x => x.Team)
                .Include(x => x.AssignedToUser)
                .Include(x => x.CreatedByUser)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (task == null) throw new Exception("Task not found.");

            return MapToResponse(task);
        }

        // Role-aware list + filters
        public async Task<List<TaskResponse>> GetTasks(
            int currentUserId,
            string currentUserRole,
            string? status,
            string? priority,
            DateTime? deadlineFrom,
            DateTime? deadlineTo)
        {
            var query = _context.Tasks
                .Include(x => x.Team)
                .Include(x => x.AssignedToUser)
                .Include(x => x.CreatedByUser)
                .AsQueryable();

            if (currentUserRole == "User")
                query = query.Where(x => x.AssignedToUserId == currentUserId);
            else if (currentUserRole == "Manager")
                query = query.Where(x => x.Team!.ManagerId == currentUserId);
            // Admin => sab dikhega

            if (!string.IsNullOrEmpty(status))
                query = query.Where(x => x.Status == status);

            if (!string.IsNullOrEmpty(priority))
                query = query.Where(x => x.Priority == priority);

            if (deadlineFrom.HasValue)
                query = query.Where(x => x.Deadline >= deadlineFrom);

            if (deadlineTo.HasValue)
                query = query.Where(x => x.Deadline <= deadlineTo);

            var tasks = await query.OrderBy(x => x.Deadline).ToListAsync();
            return tasks.Select(MapToResponse).ToList();
        }

        private TaskResponse MapToResponse(TaskItem task)
        {
            return new TaskResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                Status = task.Status,
                Priority = task.Priority,
                Deadline = task.Deadline,
                TeamId = task.TeamId,
                TeamName = task.Team?.Name,
                AssignedToUserId = task.AssignedToUserId,
                AssignedToUserName = task.AssignedToUser?.FullName,
                CreatedByUserId = task.CreatedByUserId,
                CreatedByUserName = task.CreatedByUser?.FullName,
                CreatedAt = task.CreatedAt,
                UpdatedAt = task.UpdatedAt
            };
        }
    }
}