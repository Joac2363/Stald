using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class StableController : ControllerBase
    {
        private readonly DbContext _context;

        public StableController(DbContext context)
        {
            _context = context;
        }

        // Not needed ATM
        //[HttpGet]
        //[ProducesResponseType(typeof(IEnumerable<Stable>), 200)]
        //public async Task<ActionResult<IEnumerable<Stable>>> GetStables()
        //{
        //    var stables = await _context.Stables.ToListAsync();
        //    return Ok(stables);
        //}

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Stable), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<Stable>> GetStable(int id)
        {
            var stable = await _context.Stables.FindAsync(id);
            if (stable == null)
                return NotFound();

            return Ok(stable);
        }
        
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateStable([FromBody] CreateStableDto dto)
        {

            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var stable = new Stable
            {
                Name = dto.Name,
                Address = dto.Address,
                OwnerId = userId
            };

            await _context.Stables.AddAsync(stable);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateStable(int id, [FromBody] CreateStableDto dto)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var stable = await _context.Stables.FindAsync(id);
            if (stable == null)
                return NotFound();

            if (stable.OwnerId != userId)
                return Unauthorized();

            stable.Name = dto.Name;
            stable.Address = dto.Address;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStable(int id)
        {
            string? userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            var stable = await _context.Stables.FindAsync(id);
            if (stable == null)
                return NotFound();

            if (stable.OwnerId != userId)
                return Forbid();

            _context.Stables.Remove(stable);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        //TODO:
        //Add custom Update DTO
    }
}
