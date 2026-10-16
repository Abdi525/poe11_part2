using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using poe11.Models;
using poe11.RaceDayDatabase;
using System.Linq;

namespace poe11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthControllers : ControllerBase
    {
        private readonly RaceDayDatabaseContext _context;
        public AuthControllers(RaceDayDatabaseContext context)
        { 
            _context = context;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] UserRegisterDto dto)
        {
            if (_context.Users.Any(u => u.Email == dto.Email)) 
                return Conflict(new {message = "Email already exists."}); 
            
            var user = new Users
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = dto.Password,
                PhoneNumber = dto.PhoneNumber,
                Address = dto.Address,
                UserRole = dto.UserRole

            };

            _context.Users.Add(user);
            _context.Users.Add(user); _context.SaveChanges(); return
            StatusCode(201, new {message = "User registered successfully", userId = user.UserId});

        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto dto)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == dto.Email); 

            if (user == null) 
                return Unauthorized(new {message = "Invalid email or password." }); 
            
            HttpContext.Session.SetInt32("UserId", user.UserId); 
            HttpContext.Session.SetString("UserRole", user.UserRole);
            HttpContext.Session.SetString("UserEmail", user.Email); 

            return Ok(new
            {
                message = "Login successful",
                userId = user.UserId,
                role = user.UserRole
            });

        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok(new { message = "Logged out successfully" });
        }
    }
    public class UserRegisterDto
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty; 
        public string LastName { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty; 
        public string Password { get; set; } = string.Empty; 
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
        public string UserRole { get; set; } = "Participant";
    }

    public class UserLoginDto
    {
        public string Email{ get; set; } = string.Empty; 
        public string Password { get; set; } = string.Empty;
    }
}
