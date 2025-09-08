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
    public class BoxController : ControllerBase
    {
        private readonly DbContext _context;
        public BoxController(DbContext context)
        {
            _context = context;
        }

        [HttpPost("stable/{stableId}/area/{areaId}/box")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] List<CreateBoxDto> dtos, int stableId, int areaId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool userIsOwnerOfStable = await _context.Stables
                .AnyAsync(s => s.Id == stableId && s.OwnerId == userId);
            if (!userIsOwnerOfStable)
                return Unauthorized();

            var isGreenArea = await _context.Areas
                .Where(a => a.StableId == stableId && a.Id == areaId)
                .Select(a => (bool?)a.IsGreenArea)
                .FirstOrDefaultAsync();


            if (isGreenArea == null)
                return NotFound($"An area with Id = '{areaId}' was not found in stabel with Id = '{stableId}'");

            if (isGreenArea == true)
                return BadRequest("Cannot add boxes to Area marked as IsGreenArea");

            IEnumerable<Box> boxes = dtos
                .Select(dto => new Box
                {
                    AreaId = areaId,
                    Number = dto.Number
                });

            await _context.Boxes.AddRangeAsync(boxes);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        [HttpPut("stable/{stableId}/area/box")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] UpdateBoxDto box, int stableId, int areaId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var oldBox = await _context.Boxes
                .Include(b => b.Area)
                .Include(b => b.Area.Stable)
                .FirstOrDefaultAsync(b => b.Id == box.Id);

            if (oldBox == null)
                return NotFound($"A box with Id = '{box.Id}' was not found");

            if (oldBox.Area.Stable.OwnerId != userId)
                return Unauthorized();

            oldBox.Number = box.Number;

            await _context.SaveChangesAsync();
            
            return NoContent();

        }

        [HttpDelete("stable/area/box/{boxId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int boxId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var box = await _context.Boxes
                .Include(b => b.Area)
                .Include(b => b.Area.Stable)
                .FirstAsync(b => b.Id == boxId);

            if (box == null)
                return NotFound($"A box with Id = '{boxId}' was not found");

            if (box.Area.Stable.OwnerId != userId)
                return Unauthorized();

            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
