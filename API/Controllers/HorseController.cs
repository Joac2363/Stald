using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers;
//[Authorize]
[ApiController]
[Route("api")]
public class HorseController : ControllerBase
{
    private readonly DbContext _context;
    public HorseController(DbContext context)
    {
        _context = context;
    }

    [HttpGet("stable/{stableId}/horses")]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(IEnumerable<GetHorseDto>), 200)]
    public async Task<ActionResult<IEnumerable<GetHorseDto>>> Get(int stableId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        bool stableExists = await _context.Stables.AnyAsync(s => s.Id == stableId);
        if (!stableExists)
            return BadRequest();

        bool userIsMember = await _context.Members.AnyAsync(m => m.UserId == userId && m.StableId == stableId);
        if (!userIsMember)
            return Unauthorized();


        var horses = await _context.Horses
            .Include(h => h.Owner)
            .Include(h => h.GreenArea)
            .Include(h => h.Box)
            .Where(h => h.StableId == stableId)
            .ToListAsync();

        var dtos = horses.Select(h => new GetHorseDto
        {
            Id = h.Id,
            Name = h.Name,
            StableId = h.StableId,
            IsOwnedByStable = h.IsOwnedByStable,
            Owner = h.Owner == null ? null : new GetUserDto
            {
                Id = h.Owner.Id,
                FirstName = h.Owner.FirstName,
                LastName = h.Owner.LastName
            },
            BoxId = h.BoxId,
            BoxNumber = h.Box?.Number,
            GreenAreaId = h.GreenAreaId,
            GreenAreaName = h.GreenArea?.Name
        });

        return Ok(dtos);

    }

    [HttpPost("horse")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateHorseDto dto)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        bool userIsOwnerOfStable = await _context.Stables.AnyAsync(s => s.Id == dto.StableId && s.OwnerId == userId);

        if (dto.StableId != null)
        {
            if (dto.IsOwnedByStable)
            {
                if (!userIsOwnerOfStable)
                    return Unauthorized();
            }
            else
            {
                bool userIsMember = await _context.Members.AnyAsync(m => m.UserId == userId && m.StableId == dto.StableId);
                if (!userIsMember)
                    return Unauthorized();
            }

        }
        else if (dto.IsOwnedByStable)
            return BadRequest("Must supply StableId, when horse marked as IsOwnedByStable");

        var horse = new Horse
        {
            Name = dto.Name,
            StableId = dto.StableId,
            IsOwnedByStable = dto.IsOwnedByStable,
            OwnerId = dto.IsOwnedByStable ? null : Guid.Parse(userId),
            BoxId = userIsOwnerOfStable ? dto.BoxId : null,
            GreenAreaId = userIsOwnerOfStable ? dto.GreenAreaId : null
        };

        await _context.Horses.AddAsync(horse);
        await _context.SaveChangesAsync();

        return StatusCode(201);
    }

    [HttpPut("horse")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update([FromBody] UpdateHorseDto dto)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        Stable? stable = null;
        if (dto.StableId != null)
        {
            stable = await _context.Stables.FindAsync(dto.StableId);
            if (stable == null)
                return NotFound($"A stable with Id = '{dto.StableId}' was not found.");
        }

        bool userIsOwnerOfStable = stable?.OwnerId == userId;
        bool userIsMember = stable != null && await _context.Members.AnyAsync(m => m.UserId == userId && m.StableId == stable.Id);

        var horse = await _context.Horses.FindAsync(dto.Id);
        if (horse == null)
            return NotFound($"A horse with Id = '{dto.Id}' was not found.");

        if (dto.Name != horse.Name && horse.OwnerId.ToString() != userId)
            return Unauthorized("Only owner of horse can change horse name");

        if (dto.StableId != horse.StableId && !userIsMember)
            return Unauthorized("Cannot change StabelId to stable where user isn't member");
        if (dto.IsOwnedByStable != horse.IsOwnedByStable)
        {
            if (!userIsOwnerOfStable)
                return Unauthorized("Only stable owner can change stable ownership of horse");
            if (dto.IsOwnedByStable && dto.OwnerId != null)
                return BadRequest("Horse cannot both be IsOwnedByStable and have OwnerId");
            if (!dto.IsOwnedByStable && dto.OwnerId == null)
                return BadRequest("Horse not IsOwnedByStable must have OwnerId");
        }
        if (dto.OwnerId != horse.OwnerId && horse.OwnerId.ToString() != userId)
            return Unauthorized("Only horse owner can change horse ownership");
        if (dto.BoxId != horse.BoxId && !userIsOwnerOfStable)
            return Unauthorized("Only stable owner can change horse box.");
        if (dto.GreenAreaId != horse.GreenAreaId)
            return Unauthorized("Only stable owner can change horse greenarea.");

        horse.Name = dto.Name;
        horse.StableId = dto.StableId;
        horse.IsOwnedByStable = dto.IsOwnedByStable;
        horse.OwnerId = dto.OwnerId;
        horse.BoxId = dto.BoxId;
        horse.GreenAreaId = dto.GreenAreaId;

        await _context.SaveChangesAsync();
        return NoContent();

    }

    [HttpDelete("horse/{horseId}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(Guid horseId)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var horse = await _context.Horses
            .Include(a => a.Stable)
            .FirstAsync(h => h.Id == horseId);

        if (horse == null)
            return NotFound();

        if (horse.IsOwnedByStable && horse.Stable.OwnerId != userId)
            return Unauthorized("Ony stable owner can delete stable owned horses");

        if (horse.OwnerId?.ToString() != userId && !horse.IsOwnedByStable)
            return Unauthorized("Only horse owner can delete horse");

        _context.Remove(horse);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
