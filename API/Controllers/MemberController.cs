using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly DbContext _context;
        public MemberController(DbContext context)
        {
            _context = context;
        }

        [HttpGet("stable/{id}")] // Stable id
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IEnumerable<Member>), 200)]
        public async Task<ActionResult<IEnumerable<Member>>> GetAll(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool stableExists = await _context.Stables.AnyAsync(s => s.Id == id);
            if (!stableExists)
                return BadRequest();

            bool userIsMember = await _context.Members.AnyAsync(m => m.UserId == userId && m.StableId == id);
            if (!userIsMember)
                return Unauthorized();

            return await _context.Members
                .Include(m => m.User)
                .Where(m => m.StableId == id)
                .ToListAsync();
        }

        [HttpPost("stable/{stableId}/user/{id}")] // user id
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateMember(int stableId, string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool stableExists = await _context.Stables.AnyAsync(s => s.Id == stableId);
            if (!stableExists)
                return BadRequest();

            bool userIsOwner = (await _context.Stables.FirstOrDefaultAsync(s => s.Id == stableId))!.OwnerId == userId;
            if (!userIsOwner)
                return Unauthorized();

            var member = new Member
            {
                UserId = id,
                StableId = stableId
            };

            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
            return StatusCode(201);
        }

        [HttpDelete("stable/{stableId}/user/{id}")] // user id
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteMember(int stableId, string id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool stableExists = await _context.Stables.AnyAsync(s => s.Id == stableId);
            if (!stableExists)
                return BadRequest();

            bool userIsOwner = (await _context.Stables.FirstOrDefaultAsync(s => s.Id == stableId))!.OwnerId == userId;
            bool isTheDeletedUser = userId == id;
            if (!userIsOwner && !isTheDeletedUser)
                return Unauthorized();

            var member = await _context.Members.FindAsync(id);
            if (member == null)
                return NotFound();


            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
