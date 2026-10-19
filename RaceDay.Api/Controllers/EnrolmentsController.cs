using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Middleware;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnrolmentsController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    public EnrolmentsController(RaceDayDbContext context) => _context = context;

    public record EnrolmentRequest(int EventId, int CategoryId);

    [HttpPost]
    [SessionRole("Participant")]
    public async Task<IActionResult> Enrol(EnrolmentRequest request)
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);

        if (await _context.Enrolments.AnyAsync(e => e.UserId == userId && e.EventId == request.EventId && e.CategoryId == request.CategoryId))
            return Conflict(new { message = "Already enrolled in this category for this event." });

        var enrolment = new Enrolment { UserId = userId, EventId = request.EventId, CategoryId = request.CategoryId };
        _context.Enrolments.Add(enrolment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = enrolment.EnrolmentId }, enrolment);
    }

    [HttpGet("my")]
    [SessionRole("Participant")]
    public async Task<IActionResult> GetMyEnrolments()
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var enrolments = await _context.Enrolments
            .Include(e => e.Event).Include(e => e.Category).Include(e => e.Result)
            .Where(e => e.UserId == userId).ToListAsync();
        return Ok(enrolments);
    }

    [HttpGet("{id}")]
    [SessionRole("Any")]
    public async Task<IActionResult> Get(int id)
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var role = HttpContext.Session.GetString("Role")!;

        var enrolment = await _context.Enrolments.Include(e => e.Event).Include(e => e.Category).FirstOrDefaultAsync(e => e.EnrolmentId == id);
        if (enrolment == null) return NotFound();

        if (role == "Participant" && enrolment.UserId != userId) return Forbid();
        if (role == "Organiser" && enrolment.Event.OrganiserId != userId) return Forbid();

        return Ok(enrolment);
    }

    [HttpDelete("{id}")]
    [SessionRole("Participant")]
    public async Task<IActionResult> Cancel(int id)
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var enrolment = await _context.Enrolments.FindAsync(id);
        if (enrolment == null) return NotFound();
        if (enrolment.UserId != userId) return Forbid();

        _context.Enrolments.Remove(enrolment);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpGet("/api/events/{eventId}/enrolments")]
    [SessionRole("Organiser")]
    public async Task<IActionResult> GetEventEnrolments(int eventId)
    {
        var organiserId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var ev = await _context.Events.FindAsync(eventId);
        if (ev == null) return NotFound();
        if (ev.OrganiserId != organiserId) return Forbid();

        var enrolments = await _context.Enrolments.Include(e => e.User).Include(e => e.Category).Where(e => e.EventId == eventId).ToListAsync();
        return Ok(enrolments);
    }
}