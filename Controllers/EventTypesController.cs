using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using poe11.Models;
using poe11.RaceDayDatabase;
namespace poe11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventTypesController : ControllerBase
    {
        private readonly RaceDayDatabaseContext _context; 
        
        public EventTypesController(RaceDayDatabaseContext context)
        { 
            _context = context; 
        }

        [HttpGet]
        public IActionResult GetCategories(int eventId)
        {
            var categories = _context.EventTypes.Where(c => c.EventId == eventId).ToList(); 
            return Ok(categories);
        }

        [HttpPost]
        public IActionResult AddCategory(int eventId, [FromBody] EventTypeDto dto)
        {
            var role = HttpContext.Session.GetString("UserRole"); 

            if (role != "Organiser") 
                return StatusCode(403, new { message = "Access denied. Organiser role required." }); 

            var ev = _context.Events.Find(eventId); 

            if (ev == null) 
                return NotFound(new {message = "Event not found."}); 

            var category = new EventTypes
            {
                EventId = eventId,
                EventTypeName = dto.EventTypeName,
                DistanceKM = dto.DistanceKM,
                EntranceFee = dto.EntranceFee,
                MaxParticipants = dto.MaxParticipants
            };

            _context.EventTypes.Add(category); 
            _context.SaveChanges(); return
            StatusCode(201, category);
        }
    }
    public class EventTypeDto
    {
        public string EventTypeName { get; set; } = string.Empty; 
        public decimal DistanceKM { get; set; }
        public decimal EntranceFee { get; set; }
        public int MaxParticipants { get; set; }
    }
}