namespace API.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Identity;
    using API.Models;
    using API.DTOs;
    using Microsoft.EntityFrameworkCore;
    using API.Auth;

    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly JwtTokenService _jwtTokenService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UsersController(JwtTokenService jwtTokenService, UserManager<User> userManager, SignInManager<User> signInManager)
        {
            _jwtTokenService = jwtTokenService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // POST: api/users/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var user = new User
            {
                UserName = dto.Email,
                Email = dto.Email,
                FirstName = dto.FirstName,
                LastName = dto.LastName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok(new { user.Id, user.Email });
        }

        // Not needed ATM
        //[HttpGet]
        //[ProducesResponseType(typeof(List<User>), StatusCodes.Status200OK)]
        //public async Task<IActionResult> GetAll()
        //{
        //    var users = await _userManager.Users.ToListAsync();
        //    return Ok(users);
        //}

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]          // Successful login
        [ProducesResponseType(StatusCodes.Status400BadRequest)]   // Model validation failed
        [ProducesResponseType(StatusCodes.Status401Unauthorized)] // Invalid credentials
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Unauthorized(new { message = "Invalid email or password" });

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: false);

            if (!result.Succeeded)
                return Unauthorized(new { message = "Invalid email or password" });

            var token = _jwtTokenService.GenerateToken(user);

            return Ok(new { 
                message = "Login successful", 
                token,
                userId = user.Id, 
                email = user.Email });
        }
    }

}
