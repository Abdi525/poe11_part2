using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Data.Entity;
using poe11.Models;
using poe11.RaceDayDatabase;

namespace poe11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayDatabaseContext _context; 
        public EventsController(RaceDayDatabaseContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetEvents()
        {
            var events = _context.Events
                .Include(e => e.Organizer)
                .ToList(); return Ok(events);
        }

        [HttpPost]
        public IActionResult CreateEvent([FromBody] EventDto dto)
        {
            var role = HttpContext.Session.GetString("UserRole"); 
            var userId = HttpContext.Session.GetInt32("UserId"); 
            
            if (userId == null) 
                return Unauthorized(new { message = "Authentication required."}); 

            if (role != "Organiser") 
                return StatusCode(403, new { message = "Access denied. Organiser role required." }); 

            var newEvent = new Event
            {
                OrganizerId = userId.Value,
                EventName = dto.EventName,
                EventLocation = dto.EventLocation,
                EventDate = dto.EventDate,
                EventDescription = dto.EventDescription
            }; 

            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return StatusCode(201, newEvent);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateEvent(int id, [FromBody] EventDto dto)
        {
            var role = HttpContext.Session.GetString("UserRole");

            if (role != "Organiser") 
                return StatusCode(403, new { message = "Access denied. Organiser role required." });

            var ev = _context.Events.Find(id); 

            if (ev == null) 
                return NotFound(new { message = "Event not found." });

            ev.EventName = dto.EventName;
            ev.EventLocation = dto.EventLocation;
            ev.EventDate = dto.EventDate;
            ev.EventDescription = dto.EventDescription;

            _context.SaveChanges();
            return Ok(ev);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(int id)
        {
            var role = HttpContext.Session.GetString("UserRole"); 

            if (role != "Organiser") 
                return StatusCode(403, new { message = "Access denied. Organiser role required." });

            var ev = _context.Events.Find(id); 

            if (ev == null) 
                return NotFound(new {message = "Event not found."}); 
            
            _context.Events.Remove(ev); 
            _context.SaveChanges(); return NoContent();
        }
    }

    public class EventDto
    {
        public string EventName { get; set; } = string.Empty; 
        public string EventLocation { get; set; } = string.Empty; 
        public DateTime EventDate { get; set; }
        public string? EventDescription { get; set; }
    }
}