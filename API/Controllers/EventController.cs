using API.DTOs;
using API.Models;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;
using System.Security.Claims;

namespace API.Controllers;

//[Authorize]
[ApiController]
[Route("api")]
public class EventController : ControllerBase
{
    private readonly DbContext _context;
    public EventController(DbContext context)
    {
        _context = context;
    }

    [HttpGet("stable/{stableId}/events")]
    [ProducesResponseType(401)]
    [ProducesResponseType(400)]
    [ProducesResponseType(typeof(IEnumerable<EventDto>), 200)]
    public async Task<ActionResult<List<EventDto>>> GetEvents(int stableId, [FromQuery] DateTime from, [FromQuery] DateTime to)
    {
        //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //if (userId == null)
        //    return Unauthorized();

        //bool userIsMember = await _context.Members.AnyAsync(m => m.UserId == userId && m.StableId == stableId);
        //if (!userIsMember)
        //    return Unauthorized();

        //bool stableExists = await _context.Stables.AnyAsync(s => s.Id == stableId);
        //if (!stableExists)
        //    return BadRequest();

        var overrides = await _context.Events
            .Where(e => e.StableId == stableId)
            .Where(e => e.EndDate < to)
            .Where(e => e.isOverride)
            .ToArrayAsync();


        // Put all overrides into list
        var returnEvents = new List<EventDto>();
        foreach (var e in overrides)
        {
            returnEvents.Add(
                new EventDto
                {
                    Id = e.Id,
                    SeriesId = e.SeriesId,
                    OriginalStartDate = e.OriginalStartDate,
                    StableId = e.StableId,
                    isOverride = e.isOverride,
                    RecurrenceRule = e.RecurrenceRule,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Title = e.Title,
                    Description = e.Description
                }
            );
        }

        var recurringEvents = await _context.Events
            .Where(e => e.StableId == stableId)
            .Where(e => e.EndDate < to)
            .Where(e => e.isRecurring)
            .ToArrayAsync();

        // Calculate recurrence and add to list
        foreach (var e in recurringEvents)
        {
            // Recurring event — calculate occurrences using iCal.Net
            var calendarEvent = new CalendarEvent
            {
                DtStart = new CalDateTime(e.StartDate),
                DtEnd = new CalDateTime(e.EndDate),
                RecurrenceRules = [new RecurrencePattern(e.RecurrenceRule)]
            };


            // Get occurrences within the requested range
            var recurrences = calendarEvent.GetOccurrences(new CalDateTime(from)).TakeWhileBefore(new CalDateTime(to));

            foreach (var r in recurrences)
            {
                bool isOverridden = overrides.Any(o => o.OriginalStartDate == r.Period.StartTime.AsUtc);
                if (isOverridden)
                    continue;

                returnEvents.Add(
                    new EventDto
                    {
                        Id = null, // null since they have no unique id
                        SeriesId = e.Id, // e.id since they have to refer back to the original event
                        OriginalStartDate = r.Period.StartTime.AsUtc, // Used for overrides
                        StableId = e.StableId,
                        isOverride = false,
                        RecurrenceRule = e.RecurrenceRule,
                        StartDate = r.Period.StartTime.AsUtc,
                        EndDate = r.Period.EndTime?.AsUtc ?? r.Period.StartTime.AsUtc.Add(e.EndDate - e.StartDate),
                        Title = e.Title,
                        Description = e.Description
                    }
                );
            }
        }

        var events = await _context.Events
            .Where(e => e.StableId == stableId)
            .Where(e => e.StartDate > from)
            .Where(e => e.EndDate < to)
            .Where(e => !e.isOverride)
            .Where(e => !e.isRecurring)
            .ToArrayAsync();

        // Put remaining events into list
        foreach (var e in events)
        {
            returnEvents.Add(
                new EventDto
                {
                    Id = e.Id,
                    SeriesId = e.SeriesId,
                    OriginalStartDate = e.OriginalStartDate,
                    StableId = e.StableId,
                    isOverride = e.isOverride,
                    RecurrenceRule = e.RecurrenceRule,
                    StartDate = e.StartDate,
                    EndDate = e.EndDate,
                    Title = e.Title,
                    Description = e.Description
                }
            );
        }

        return returnEvents;
    }

    [HttpPost("stable/{stableId}/event")]
    [ProducesResponseType(201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] EventDto dto, int stableId)
    {
        //string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //if (userId == null)
        //    return Unauthorized();

        //bool userIsOwnerOfStable = await _context.Stables.AnyAsync(s => s.Id == stableId && s.OwnerId == userId);
        //if (!userIsOwnerOfStable)
        //    return Unauthorized();

        var e = new Event
        {
            StableId = stableId,
            isOverride = false,
            isRecurring = dto.RecurrenceRule != null,
            RecurrenceRule = dto.RecurrenceRule,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Title = dto.Title,
            Description = dto.Description
        };

        await _context.AddAsync(e);
        await _context.SaveChangesAsync();

        return Created();
    }


    [HttpPut("stable/{stableId}/event")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Update([FromBody] EventDto dto, int stableId, [FromQuery] bool editAll = false)
    {
        string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
            return Unauthorized();

        var stable = await _context.Stables.FindAsync(stableId);
        if (stable == null)
            return NotFound();

        if (stable.OwnerId != userId)
            return Unauthorized();


        var oldEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == dto.Id);
        var baseEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == dto.SeriesId);

        if (!editAll)
        {
            // User wants to edit ONE event. If old event doesnt exist: Create one. Also, if its a recurring event, hanlde that aswell
            if (oldEvent == null && dto.OriginalStartDate == null)
            {
                if (dto.Id != null)
                    return NotFound($"An event with Id = '{dto.Id}' was not found.");
                return BadRequest("When using editAll = 'false' (default) you must supply either Id or OriginalStartDate.");
            }

            if (oldEvent != null)
            {
                oldEvent.StartDate = dto.StartDate;
                oldEvent.EndDate = dto.EndDate;
                oldEvent.Title = dto.Title;
                oldEvent.Description = dto.Description;
                if (dto.SeriesId == null)
                    oldEvent.RecurrenceRule = dto.RecurrenceRule;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            if (baseEvent == null)
                return NotFound($"An event series with Id = '{dto.SeriesId}' was not found.");
            if (dto.SeriesId == null)
                return BadRequest("When editAll is false (default) and OriginalStartDate is not null, SeriesId is required");

            var e = new Event
            {
                SeriesId = dto.SeriesId,
                OriginalStartDate = dto.OriginalStartDate,
                StableId = stableId,
                isOverride = true,
                isRecurring = false,
                RecurrenceRule = baseEvent.RecurrenceRule,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Title = dto.Title,
                Description = dto.Description
            };

            await _context.Events.AddAsync(e);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        if (dto.SeriesId == null)
            return BadRequest("When editAll = true SeriesId must be supplied");
        if (baseEvent == null)
            return NotFound($"An event series with Id = '{dto.SeriesId}' was not found.");

        baseEvent.RecurrenceRule = dto.RecurrenceRule;
        baseEvent.StartDate = dto.StartDate;
        baseEvent.EndDate = dto.EndDate;
        baseEvent.Title = dto.Title;
        baseEvent.Description = dto.Description;
        baseEvent.isRecurring = baseEvent.RecurrenceRule != null;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}


