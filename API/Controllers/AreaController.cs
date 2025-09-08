using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class AreaController : ControllerBase
    {
        private readonly DbContext _context;
        public AreaController(DbContext context)
        {
            _context = context;
        }

        [HttpGet("stable/{stableId}/areas")]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IEnumerable<GetAreaDto>), 200)]
        public async Task<ActionResult<IEnumerable<GetAreaDto>>> Get(int stableId)
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

            
            var areas = await _context.Areas
                .Include(a => a.Boxes)
                .Where(a => a.StableId == stableId)
                .ToArrayAsync();

            IEnumerable<GetAreaDto> dtos = areas
                .Select(a => new GetAreaDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    StableId = a.StableId,
                    IsGreenArea = a.IsGreenArea,
                    Boxes = a.Boxes,
                });

            return Ok(areas);
        }


        [HttpPost("stable/{stableId}/area")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateAreaDto dto, int stableId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool userIsOwnerOfStable = await _context.Stables.AnyAsync(s => s.Id == stableId && s.OwnerId == userId);
            if (!userIsOwnerOfStable)
                return Unauthorized();
            
            var area = new Area
            {
                Name = dto.Name,
                IsGreenArea = dto.IsGreenArea,
                StableId = stableId
            };

            await _context.Areas.AddAsync(area);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        [HttpPut("stable/{stableId}/area")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] UpdateAreaDto area, int stableId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var stable = await _context.Stables.FindAsync(stableId);
            if (stable == null) 
                return NotFound();

            if (stable.OwnerId != userId)
                return Unauthorized();

            var oldArea = await _context.Areas.FindAsync(area.Id);
            if (oldArea == null) 
                return NotFound();

            bool hasBoxes = await _context.Boxes.AnyAsync(b => b.AreaId == area.Id);
            if (hasBoxes && area.IsGreenArea)
                return BadRequest("Cannot make area into GreenArea, when boxes are associated");

            oldArea.Name = area.Name;
            oldArea.IsGreenArea = area.IsGreenArea;

            await _context.SaveChangesAsync();
            
            return NoContent();

        }

        [HttpDelete("stable/area/{areaId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int areaId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var area = await _context.Areas.Include(a => a.Stable).FirstAsync(a => a.Id == areaId);
            if (area == null)
                return NotFound();

            if (area.Stable.OwnerId != userId)
                return Unauthorized();

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //TODO:
        // Make employees able to Create update and delete areas
    }
}
