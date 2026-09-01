using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Dashboard;

namespace TaskManagement.API.Services
{
    public class DashboardService
    {
        private readonly ApplicationDbContext _context;

        public DashboardService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<DashboardSummaryResponse> GetSummary(
            int currentUserId,
            string currentUserRole)
        {
            var query = _context.Tasks.AsQueryable();

            // Same role-based visibility jo TaskService.GetTasks() mein hai
            if (currentUserRole == "User")
                query = query.Where(x => x.AssignedToUserId == currentUserId);
            else if (currentUserRole == "Manager")
                query = query.Where(x => x.Team!.ManagerId == currentUserId);
            // Admin => sab tasks

            var tasks = await query.ToListAsync();
            var now = DateTime.UtcNow;

            var summary = new DashboardSummaryResponse
            {
                TotalTasks = tasks.Count,
                ToDoCount = tasks.Count(x => x.Status == "ToDo"),
                InProgressCount = tasks.Count(x => x.Status == "InProgress"),
                DoneCount = tasks.Count(x => x.Status == "Done"),

                HighPriorityCount = tasks.Count(x => x.Priority == "High"),
                MediumPriorityCount = tasks.Count(x => x.Priority == "Medium"),
                LowPriorityCount = tasks.Count(x => x.Priority == "Low"),

                OverdueCount = tasks.Count(x =>
                    x.Deadline.HasValue &&
                    x.Deadline.Value < now &&
                    x.Status != "Done"),

                UpcomingDeadlines = tasks
                    .Where(x => x.Deadline.HasValue && x.Status != "Done")
                    .OrderBy(x => x.Deadline)
                    .Take(5)
                    .Select(x => new UpcomingTaskItem
                    {
                        Id = x.Id,
                        Title = x.Title,
                        Status = x.Status,
                        Priority = x.Priority,
                        Deadline = x.Deadline
                    })
                    .ToList()
            };

            return summary;
        }
    }
}