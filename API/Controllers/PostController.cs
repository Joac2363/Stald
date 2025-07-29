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
    public class PostController : ControllerBase
    {
        private readonly DbContext _context;
        public PostController(DbContext context)
        {
            _context = context;
        }

        [HttpGet("stable/{id}")] // Stable id
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IEnumerable<Post>), 200)]
        public async Task<ActionResult<IEnumerable<Post>>> GetAll(int id, [FromQuery] int page, [FromQuery] int postsPerPage)
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

            var posts = await _context.Posts
                .Include(p => p.MediaItems)
                .Include(p => p.User)
                .OrderBy(p => p.CreatedDate)
                .Skip(page)
                .Take(postsPerPage)
                .ToListAsync();

            return Ok(posts);
        }

        [HttpPost("stable/{id}")] // Stable id
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> CreatePost(int id, [FromBody] CreatePostDto dto)
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

            var post = new Post
            {
                StableId = dto.StableId,
                Title = dto.Title,
                Text = dto.Text,
                CreatedDate = DateTime.Now,
                UserId = userId
            };

            await _context.AddAsync(post);
            await _context.SaveChangesAsync();
            return StatusCode(201);

        }

        [HttpDelete("stable/{id}/post/{pid}")] // Stable id
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DeletePost(int id, int pid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool stableExists = await _context.Stables.AnyAsync(s => s.Id == id);
            if (!stableExists)
                return NotFound();

            bool userIsMember = await _context.Members.AnyAsync(m => m.UserId == userId && m.StableId == id);
            if (!userIsMember)
                return Unauthorized();

            bool postExists = await _context.Posts.AnyAsync(p => p.Id == pid);
            if (!postExists) 
                return NotFound();

            Post post = await _context.Posts.FirstAsync(p => p.Id == id);
            if (post.UserId != userId)
                return Unauthorized();

            
            _context.Remove(post);
            await _context.SaveChangesAsync();
            return NoContent();

            // TODO:
            // Add checks for if user is employee (So both members and employees can post)
        }
    }
}
