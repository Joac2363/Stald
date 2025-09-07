using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices.Marshalling;
using System.Security.Claims;

namespace API.Controllers;
[Authorize]
[ApiController]
[Route("api")]

public class TeamController : ControllerBase
{
    private readonly DbContext _context;
    public TeamController(DbContext context)
    {
        _context = context;
    }

    [HttpGet("stable/{stableId}/teams")]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(IEnumerable<GetTeamDto>), 200)]
    public async Task<ActionResult<List<GetTeamDto>>> GetTeams(int stableId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        bool userIsMember = await _context.Members.AnyAsync(m => m.UserId == userId && m.StableId == stableId);
        if (!userIsMember)
            return Unauthorized();

        bool stableExists = await _context.Stables.AnyAsync(s => s.Id == stableId);
        if (!stableExists)
            return BadRequest();

        var teams = await _context.Teams
            .Include(t => t.TeamUsers)
            .Where(t => t.StableId == stableId)
            .ToListAsync();
        var teamDtos = new List<GetTeamDto>();
        foreach (var team in teams)
        {
            var userDtos = new List<GetUserDto>();
            foreach (var user in team.TeamUsers)
                userDtos.Add(new GetUserDto{Id = user.Id, FirstName = user.FirstName,LastName = user.LastName});

            var teamDto = new GetTeamDto
            {
                Id = team.Id,
                StableId = stableId,
                Name = team.Name,
                TeamUsers = userDtos
            };
            teamDtos.Add(teamDto);
        }
        return teamDtos;
    }

    [HttpPost("stable/{stableId}/team")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateTeamDto dto, int stableId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        bool stableExists = _context.Stables.Any(s => s.Id == stableId);
        if (!stableExists)
            return NotFound($"A stable with Id = '{stableId}' was not found.");

        bool userIsOwnerOfStable = await _context.Stables.AnyAsync(s => s.Id == stableId && s.OwnerId == userId);
        if (!userIsOwnerOfStable)
            return Unauthorized();

        List<string> userIds = new();
        foreach (var id in dto.TeamUserIds)
        {
            bool userIsMember = _context.Members
                .Where(m => m.StableId == stableId)
                .Any(m => m.UserId == id.ToString());
            if (!userIsMember)
                return NotFound($"A member with Id = '{id.ToString()}' was not found in stable with Id = '{stableId}'");
            userIds.Add(id.ToString());
        }

        var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

        if (!users.Any())
            return NotFound("No users with the given Id's were found");

        var team = new Team
        {
            StableId = stableId,
            Name = dto.Name,
            TeamUsers = users
        };

        await _context.Teams.AddAsync(team);
        await _context.SaveChangesAsync();
        return StatusCode(201);
    }

    [HttpPut("stable/{stableId}/team")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update([FromBody] UpdateTeamDto dto, int stableId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var stable = await _context.Stables.FirstOrDefaultAsync(s => s.Id == stableId);
        if (stable == null)
            return NotFound($"A stable with Id = '{stableId}' was not found.");

        if (stable.OwnerId != userId)
            return Unauthorized();

        var team = await _context.Teams.FirstOrDefaultAsync(t => t.Id == dto.Id);
        if (team == null)
            return NotFound($"A team with Id = '{dto.Id}' was not found.");

        List<string> userIds = new();
        foreach (var id in dto.TeamUserIds)
        {
            bool userIsMember = _context.Members
                .Where(m => m.StableId == stableId)
                .Any(m => m.UserId == id.ToString());
            if (!userIsMember)
                return NotFound($"A member with Id = '{id.ToString()}' was not found in stable with Id = '{stableId}'");

            userIds.Add(id.ToString());
        }

        var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToListAsync();

        if (!users.Any())
            return NotFound("No users with the given Id's were found");

        team.Name = dto.Name;
        team.TeamUsers = users;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("stable/team/{teamId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int teamId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var team = await _context.Teams.FirstOrDefaultAsync(e => e.Id == teamId);
        if (team == null)
            return NotFound($"A team with Id = '{teamId}' was not found.");

        var isOwner = _context.Events.Any(e => e.Stable.OwnerId == userId);
        if (!isOwner)
            return Unauthorized();

        _context.Teams.Remove(team);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}

