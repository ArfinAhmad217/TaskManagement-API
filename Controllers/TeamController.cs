using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TaskManagement.API.DTOs.Team;
using TaskManagement.API.Services;

namespace TaskManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TeamController : ControllerBase
    {
        private readonly TeamService _teamService;

        public TeamController(TeamService teamService)
        {
            _teamService = teamService;
        }


        // POST: api/Team
        // Admin only
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTeam(
            CreateTeamRequest request)
        {
            try
            {
                var result =
                    await _teamService.CreateTeam(request);

                return CreatedAtAction(
                    nameof(GetTeamById),
                    new { id = result.Id },
                    result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // GET: api/Team
        // Admin / Manager / User
        [HttpGet]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> GetAllTeams()
        {
            var result = await _teamService.GetAllTeams();

            return Ok(result);
        }


        // GET: api/Team/1
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,User")]
        public async Task<IActionResult> GetTeamById(int id)
        {
            try
            {
                var result =
                    await _teamService.GetTeamById(id);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new
                {
                    message = ex.Message
                });
            }
        }


        // POST: api/Team/1/members
        // Admin / Manager
        [HttpPost("{teamId}/members")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> AddMember(
            int teamId,
            AddTeamMemberRequest request)
        {
            try
            {
                var currentUserId =
                    int.Parse(
                        User.FindFirstValue(
                            ClaimTypes.NameIdentifier)!);

                var currentUserRole =
                    User.FindFirstValue(
                        ClaimTypes.Role);

                var result =
                    await _teamService.AddMember(
                        teamId,
                        request.UserId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }


        // DELETE: api/Team/1/members/2
        // Admin / Manager
        [HttpDelete("{teamId}/members/{userId}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> RemoveMember(
            int teamId,
            int userId)
        {
            try
            {
                var result =
                    await _teamService.RemoveMember(
                        teamId,
                        userId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
    }
}