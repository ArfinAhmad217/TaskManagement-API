using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;
using TaskManagement.API.DTOs.Team;
using TaskManagement.API.Models;

namespace TaskManagement.API.Services
{
    public class TeamService
    {
        private readonly ApplicationDbContext _context;

        public TeamService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Create Team
        public async Task<TeamResponse> CreateTeam(
            CreateTeamRequest request)
        {
            // If ManagerId is provided, verify manager
            if (request.ManagerId.HasValue)
            {
                var manager = await _context.Users
                    .FirstOrDefaultAsync(x =>
                        x.Id == request.ManagerId.Value &&
                        x.Role == "Manager");

                if (manager == null)
                {
                    throw new Exception(
                        "Selected user is not a valid Manager.");
                }
            }

            var team = new Team
            {
                Name = request.Name.Trim(),
                ManagerId = request.ManagerId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Teams.Add(team);

            await _context.SaveChangesAsync();

            return await GetTeamById(team.Id);
        }


        // Get All Teams
        public async Task<List<TeamResponse>> GetAllTeams()
        {
            var teams = await _context.Teams
                .Include(x => x.Manager)
                .Include(x => x.Members)
                    .ThenInclude(x => x.User)
                .OrderBy(x => x.Name)
                .ToListAsync();

            return teams.Select(MapToResponse).ToList();
        }


        // Get Team By Id
        public async Task<TeamResponse> GetTeamById(int id)
        {
            var team = await _context.Teams
                .Include(x => x.Manager)
                .Include(x => x.Members)
                    .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (team == null)
            {
                throw new Exception("Team not found.");
            }

            return MapToResponse(team);
        }


        // Add Member
        public async Task<TeamResponse> AddMember(
            int teamId,
            int userId)
        {
            var team = await _context.Teams
                .Include(x => x.Members)
                .FirstOrDefaultAsync(x => x.Id == teamId);

            if (team == null)
            {
                throw new Exception("Team not found.");
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == userId);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Manager cannot be added as normal member
            if (user.Role == "Admin")
            {
                throw new Exception(
                    "Admin cannot be added as a team member.");
            }

            // Check duplicate membership
            var alreadyMember = team.Members
                .Any(x => x.UserId == userId);

            if (alreadyMember)
            {
                throw new Exception(
                    "User is already a member of this team.");
            }

            var teamMember = new TeamMember
            {
                TeamId = teamId,
                UserId = userId,
                JoinedAt = DateTime.UtcNow
            };

            _context.TeamMembers.Add(teamMember);

            await _context.SaveChangesAsync();

            return await GetTeamById(teamId);
        }


        // Remove Member
        public async Task<TeamResponse> RemoveMember(
            int teamId,
            int userId)
        {
            var teamMember = await _context.TeamMembers
                .FirstOrDefaultAsync(x =>
                    x.TeamId == teamId &&
                    x.UserId == userId);

            if (teamMember == null)
            {
                throw new Exception(
                    "User is not a member of this team.");
            }

            _context.TeamMembers.Remove(teamMember);

            await _context.SaveChangesAsync();

            return await GetTeamById(teamId);
        }


        private TeamResponse MapToResponse(Team team)
        {
            return new TeamResponse
            {
                Id = team.Id,
                Name = team.Name,
                ManagerId = team.ManagerId,
                ManagerName = team.Manager?.FullName,
                CreatedAt = team.CreatedAt,

                Members = team.Members
                    .Select(member => new TeamMemberResponse
                    {
                        UserId = member.UserId,
                        FullName = member.User.FullName,
                        Email = member.User.Email,
                        Role = member.User.Role,
                        JoinedAt = member.JoinedAt
                    })
                    .ToList()
            };
        }
    }
}