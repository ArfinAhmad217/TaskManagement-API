namespace TaskManagement.API.DTOs.Team
{
    public class TeamResponse
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public int? ManagerId { get; set; }

        public string? ManagerName { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<TeamMemberResponse> Members { get; set; }
            = new List<TeamMemberResponse>();
    }

    public class TeamMemberResponse
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime JoinedAt { get; set; }
    }
}