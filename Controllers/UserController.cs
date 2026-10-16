using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using poe11.Models;
using poe11.RaceDayDatabase;

namespace poe11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly RaceDayDatabaseContext _context; 
        public UserController(RaceDayDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet("profile")]
        public IActionResult GetProfile()
        {
            var userId = HttpContext.Session.GetInt32("UserId"); 

            if (userId == null) 
                return Unauthorized(new {message = "User not authenticated."}); 

            var user = _context.Users.Find(userId);
            
            if (user == null) 
                return NotFound(); 

            return Ok(new
            {
              user.UserId,
              user.FirstName,
              user.LastName,
              user.Email,
              user.PhoneNumber,
              user.Address,
              user.UserRole
            });
        }
    }
}
