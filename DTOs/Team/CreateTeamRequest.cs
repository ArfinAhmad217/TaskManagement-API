using System.ComponentModel.DataAnnotations;

namespace TaskManagement.API.DTOs.Team
{
    public class CreateTeamRequest
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public int? ManagerId { get; set; }
    }
}