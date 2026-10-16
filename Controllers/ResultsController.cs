using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using poe11.Models;    
using poe11.RaceDayDatabase;
using System.Reflection;
using System.Data.Entity;

namespace poe11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultsController : ControllerBase
    {
        private readonly RaceDayDatabaseContext _context;
        public ResultsController(RaceDayDatabaseContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult CaptureResult([FromBody] ResultDto dto)
        {
            var role = HttpContext.Session.GetString("UserRole"); 
            if (role != "Organiser") 
                return StatusCode(403, new { message = "Access denied. Organiser role required." });
            
            var result = new Models.Results
            {
                EntranceId = dto.EntranceId,
                FinishStatus = dto.FinishStatus,
                OverallRank = dto.OverallRank,
                CategoryRank = dto.CategoryRank,
                AgeGroupRank = dto.AgeGroupRank
            };

            _context.Results.Add(result);
            _context.SaveChanges(); 
            return StatusCode(201, result);

        }

        [HttpGet("my-results")]
        public IActionResult GetMyResults()
        {
            var userId = HttpContext.Session.GetInt32("UserId"); 

            if (userId == null) 
                return Unauthorized(new { message = "Authentication required."});

            var results = _context.Results
                .Include(r => r.RaceEntrance)
                .Include(e => e!.EventTypes)
                .Where(r => r.RaceEntrance!.UserId == userId)
                .ToList(); 
            return Ok(results);
        }
    }

    public class ResultDto
    {
        public int EntranceId { get; set; }
        public string FinishStatus { get; set; } = "Finished"; 
        public int? OverallRank { get; set; }
        public int? CategoryRank { get; set; }
        public int? AgeGroupRank { get; set; }
    }
}
