using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using API.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api")]
    public class MemberController : ControllerBase
    {
        private readonly DbContext _context;
        public MemberController(DbContext context)
        {
            _context = context;
        }

        [HttpGet("stable/{stableId}/members")] // Stable id
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IEnumerable<Member>), 200)]
        public async Task<ActionResult<IEnumerable<Member>>> GetAll(int stableId)
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

            return await _context.Members
                .Include(m => m.User)
                .Where(m => m.StableId == stableId)
                .ToListAsync();
        }

        [HttpPost("stable/{stableId}/member/user/{userId}")] // user id
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreateMember(int stableId, string userId)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
                return Unauthorized();

            bool stableExists = await _context.Stables.AnyAsync(s => s.Id == stableId);
            if (!stableExists)
                return BadRequest();

            bool userIsOwner = (await _context.Stables.FirstOrDefaultAsync(s => s.Id == stableId))!.OwnerId == id;
            if (!userIsOwner)
                return Unauthorized();

            var member = new Member
            {
                UserId = userId,
                StableId = stableId
            };

            await _context.Members.AddAsync(member);
            await _context.SaveChangesAsync();
            return StatusCode(201);
        }

        [HttpDelete("stable/{stableId}/member/user/{userId}")] // user id
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteMember(int stableId, string userId)
        {
            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (id == null)
                return Unauthorized();

            bool stableExists = await _context.Stables.AnyAsync(s => s.Id == stableId);
            if (!stableExists)
                return BadRequest();

            bool userIsOwner = (await _context.Stables.FirstOrDefaultAsync(s => s.Id == stableId))!.OwnerId == id;
            bool isTheDeletedUser = id == userId;
            if (!userIsOwner && !isTheDeletedUser)
                return Unauthorized();

            var member = await _context.Members.FindAsync(userId);
            if (member == null)
                return NotFound();


            _context.Members.Remove(member);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
