using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Middleware;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventsController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    public EventsController(RaceDayDbContext context) => _context = context;

    public record EventRequest(string Name, string Description, DateTime EventDate, string Location, decimal Distance, int EventTypeId);

    /// <summary>
    /// Get all events.
    /// </summary>
    [HttpGet]
    [SessionRole("Any")]
    public async Task<IActionResult> GetAll([FromQuery] int? eventTypeId, [FromQuery] DateTime? date)
    {
        var query = _context.Events.Include(e => e.EventType).Include(e => e.Organiser).AsQueryable();

        if (eventTypeId.HasValue) query = query.Where(e => e.EventTypeId == eventTypeId);
        if (date.HasValue) query = query.Where(e => e.EventDate.Date == date.Value.Date);

        return Ok(await query.ToListAsync());
    }

    /// <summary>
    /// Get a single event by ID.
    /// </summary>
    [HttpGet("{id}")]
    [SessionRole("Any")]
    public async Task<IActionResult> Get(int id)
    {
        var ev = await _context.Events.Include(e => e.EventType).Include(e => e.Categories).FirstOrDefaultAsync(e => e.EventId == id);
        return ev == null ? NotFound() : Ok(ev);
    }

    /// <summary>
    /// Create a new event.
    /// </summary>
    [HttpPost]
    [SessionRole("Organiser")]
    public async Task<IActionResult> Create(EventRequest request)
    {
        var organiserId = int.Parse(HttpContext.Session.GetString("UserId")!);

        var ev = new Event
        {
            Name = request.Name,
            Description = request.Description,
            EventDate = request.EventDate,
            Location = request.Location,
            Distance = request.Distance,
            EventTypeId = request.EventTypeId,
            OrganiserId = organiserId
        };

        _context.Events.Add(ev);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = ev.EventId }, ev);
    }

    /// <summary>
    /// Update an event.
    /// </summary>
    [HttpPut("{id}")]
    [SessionRole("Organiser")]
    public async Task<IActionResult> Update(int id, EventRequest request)
    {
        var organiserId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var ev = await _context.Events.FindAsync(id);

        if (ev == null) return NotFound();
        if (ev.OrganiserId != organiserId) return Forbid();

        ev.Name = request.Name;
        ev.Description = request.Description;
        ev.EventDate = request.EventDate;
        ev.Location = request.Location;
        ev.Distance = request.Distance;
        ev.EventTypeId = request.EventTypeId;

        await _context.SaveChangesAsync();
        return Ok(ev);
    }

    /// <summary>
    /// Delete an event.
    /// </summary>
    [HttpDelete("{id}")]
    [SessionRole("Organiser")]
    public async Task<IActionResult> Delete(int id)
    {
        var organiserId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var ev = await _context.Events.FindAsync(id);

        if (ev == null) return NotFound();
        if (ev.OrganiserId != organiserId) return Forbid();

        _context.Events.Remove(ev);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}