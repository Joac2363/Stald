using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
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

        //[HttpPost]
        //[ProducesResponseType(typeof(Stable), 201)]
        //[ProducesResponseType(400)]
        //public async Task<ActionResult<Stable>> CreateStable([FromBody] CreateStableDto dto)
        //{
        //    var stable = new Stable
        //    {
        //        Name = dto.Name,
        //        Address = dto.Address
        //    };

        //    _context.Stables.Add(stable);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetStable), new { id = stable.Id }, stable);
        //}

        //[HttpPut("{id}")]
        //[ProducesResponseType(204)]
        //[ProducesResponseType(404)]
        //public async Task<IActionResult> UpdateStable(int id, [FromBody] CreateStableDto dto)
        //{
        //    var stable = await _context.Stables.FindAsync(id);
        //    if (stable == null)
        //        return NotFound();

        //    stable.Name = dto.Name;
        //    stable.Address = dto.Address;

        //    await _context.SaveChangesAsync();
        //    return NoContent();
        //}

        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteStable(int id)
        {
            var stable = await _context.Stables.FindAsync(id);
            if (stable == null)
                return NotFound();

            _context.Stables.Remove(stable);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
