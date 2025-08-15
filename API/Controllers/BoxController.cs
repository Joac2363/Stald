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
    [Route("api/[controller]")]
    public class BoxController : ControllerBase
    {
        private readonly DbContext _context;
        public BoxController(DbContext context)
        {
            _context = context;
        }

        [HttpPost("stable/{id}/area")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Create([FromBody] CreateBoxDto dto, int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool userIsOwnerOfStable = await _context.Stables.AnyAsync(s => s.Id == id && s.OwnerId == userId);
            if (!userIsOwnerOfStable)
                return Unauthorized();

            var Box = new Box
            {
                Number = dto.Number,
                AreaId = dto.AreaId
            };

            await _context.Boxes.AddAsync(Box);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        [HttpPut("stable/{stableId}/area")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Update([FromBody] UpdateBoxDto box, int stableId)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var stable = await _context.Stables.FindAsync(stableId);
            if (stable == null) 
                return NotFound();

            if (stable.OwnerId != userId)
                return Unauthorized();

            var oldBox = await _context.Boxes.FindAsync(box.Id);
            if (oldBox == null) 
                return NotFound();

            bool areaExists = true;
            if (box.AreaId != null)
                areaExists = await _context.Areas.AnyAsync(a => a.Id == box.AreaId);

            if (!areaExists)
                return NotFound();


            oldBox.Number = box.Number ?? oldBox.Number;
            oldBox.AreaId = box.AreaId ?? oldBox.AreaId;

            _context.Boxes.Update(oldBox);
            await _context.SaveChangesAsync();
            
            return NoContent();

        }

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var box = await _context.Boxes
                .Include(b => b.Area)
                .Include(b => b.Area.Stable)
                .FirstAsync(b => b.Id == id);
            if (box == null)
                return NotFound();

            if (box.Area.Stable.OwnerId != userId)
                return Unauthorized();

            _context.Boxes.Remove(box);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
