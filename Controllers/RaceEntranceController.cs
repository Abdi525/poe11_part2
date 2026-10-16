using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.Entity;
using poe11.Models;
using poe11.RaceDayDatabase;

namespace poe11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RaceEntranceController : ControllerBase
    {
        private readonly RaceDayDatabaseContext _context; 
        public RaceEntranceController(RaceDayDatabaseContext context)
        { 
            _context = context; 
        }

        [HttpPost]
        public IActionResult Enroll([FromBody] EnrollmentDto dto)
        {
            var userId = HttpContext.Session.GetInt32("UserId"); 
            var role = HttpContext.Session.GetString("UserRole"); 

            if (userId == null) 
                return Unauthorized(new { message = "Authentication required." }); 

            if (role != "Participant")
                return StatusCode(403, new { message = "Only participants can enroll in events." });

            var entrance = new RaceEntrance
            {
                UserId = userId.Value,
                EventTypeCode = dto.EventTypeCode,
                ChipNumber = "CHIP-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper(),
                PaymentStatus = "Pending",
                MedicalNotes = dto.MedicalNotes,
                EmergencyContactNum = dto.EmergencyContactNum
            }; 
            
            _context.RaceEntrances.Add(entrance);
            _context.SaveChanges(); 
            return StatusCode(201, entrance);
        }

        [HttpGet("event/{eventId}")]
        public IActionResult GetEventEnrollments(int eventId)
        {
            var role = HttpContext.Session.GetString("UserRole"); 
            
            if (role != "Organiser") 
                return StatusCode(403, new { message = "Access denied. Organiser role required." }); 
            
            var enrollments = _context.RaceEntrances
                .Include(r => r.User).Include(r => r.EventType)
                .Where(r => r.EventType!.EventId == eventId)
                .ToList(); 

            return Ok(enrollments);
        }
    }
    public class EnrollmentDto
    {
        public int EventTypeCode { get; set; }
        public string? MedicalNotes { get; set; }
        public string EmergencyContactNum { get; set; } = string.Empty;

    }
}