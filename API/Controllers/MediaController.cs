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
    public class MediaController : ControllerBase
    {
        private readonly DbContext _context;
        public MediaController(DbContext context)
        {
            _context = context;
        }

        [HttpGet("stable/post/{postId}/media")] // Post id
        [ProducesResponseType(401)]
        [ProducesResponseType(400)]
        [ProducesResponseType(typeof(IEnumerable<Media>), 200)]
        public async Task<ActionResult<IEnumerable<Media>>> Get(int postId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                return Unauthorized();

            bool postExists = await _context.Posts.AnyAsync(p => p.Id == postId);
            if (!postExists)
                return BadRequest();

            // Maybe add check for if user is member in the stable where this post is posted

            //var posts = await _context.Posts
            //    .Include(p => p.MediaItems)
            //    .Include(p => p.User)
            //    .OrderBy(p => p.CreatedDate)
            //    .Skip(page)
            //    .Take(postsPerPage)
            //    .ToListAsync();

            var media = await _context.Media
                .Where(m => m.PostId == postId)
                .ToArrayAsync();

            return Ok(media);

            // TODO:
            // Add endpoints:
            // Create
            // Update
            // Delete
        }
    }
}
