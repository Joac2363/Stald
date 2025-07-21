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
    public class AreaController : ControllerBase
    {
        private readonly DbContext _context;
        public AreaController(DbContext context)
        {
            _context = context;
        }

        [HttpGet("stable/{id}/area")]
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IEnumerable<Area>), 200)]
        public async Task<ActionResult<IEnumerable<Area>>> Get(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool userIsMember = await _context.Members.AnyAsync(m => m.UserId == userId && m.StableId == id);
            if (!userIsMember)
                return Unauthorized();

            bool stableExists = await _context.Stables.AnyAsync(s => s.Id == id);
            if (!stableExists)
                return BadRequest();

            IEnumerable<Area> areas = await _context.Areas
                .Where(a => a.StableId == id)
                .ToArrayAsync();
            
            return Ok(areas);
        }
    }
}
