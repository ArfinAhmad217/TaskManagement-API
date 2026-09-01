namespace TaskManagement.API.DTOs.Dashboard
{
    public class DashboardSummaryResponse
    {
        public int TotalTasks { get; set; }
        public int ToDoCount { get; set; }
        public int InProgressCount { get; set; }
        public int DoneCount { get; set; }

        public int HighPriorityCount { get; set; }
        public int MediumPriorityCount { get; set; }
        public int LowPriorityCount { get; set; }

        public int OverdueCount { get; set; }

        public List<UpcomingTaskItem> UpcomingDeadlines { get; set; } = new();
    }

    public class UpcomingTaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime? Deadline { get; set; }
    }
}