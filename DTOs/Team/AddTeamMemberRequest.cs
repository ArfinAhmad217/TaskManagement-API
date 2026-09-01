using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Team
{
    public class AddTeamMemberRequest
    {
        [Required]
        public int UserId { get; set; }
    }
}