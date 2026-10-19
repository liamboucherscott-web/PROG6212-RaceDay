using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Middleware;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api")]
public class CategoriesResultsController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    public CategoriesResultsController(RaceDayDbContext context) => _context = context;

    public record CategoryRequest(string Name, string? Description, int? MaxParticipants, decimal EntryFee);
    public record ResultRequest(TimeSpan FinishTime, int FinishingPosition);

    [HttpGet("events/{eventId}/categories")]
    [SessionRole("Any")]
    public async Task<IActionResult> GetCategories(int eventId)
    {
        return Ok(await _context.Categories.Where(c => c.EventId == eventId).ToListAsync());
    }

    [HttpPost("events/{eventId}/categories")]
    [SessionRole("Organiser")]
    public async Task<IActionResult> CreateCategory(int eventId, CategoryRequest request)
    {
        var organiserId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var ev = await _context.Events.FindAsync(eventId);

        if (ev == null) return NotFound();
        if (ev.OrganiserId != organiserId) return Forbid();

        var category = new Category
        {
            EventId = eventId,
            Name = request.Name,
            Description = request.Description,
            MaxParticipants = request.MaxParticipants,
            EntryFee = request.EntryFee
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return Created("", category);
    }

    [HttpPost("enrolments/{enrolmentId}/result")]
    [SessionRole("Organiser")]
    public async Task<IActionResult> CaptureResult(int enrolmentId, ResultRequest request)
    {
        var organiserId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var enrolment = await _context.Enrolments.Include(e => e.Event).FirstOrDefaultAsync(e => e.EnrolmentId == enrolmentId);

        if (enrolment == null) return NotFound();
        if (enrolment.Event.OrganiserId != organiserId) return Forbid();
        if (await _context.Results.AnyAsync(r => r.EnrolmentId == enrolmentId)) return Conflict(new { message = "Result already captured." });

        var result = new Result
        {
            EnrolmentId = enrolmentId,
            FinishTime = request.FinishTime,
            FinishingPosition = request.FinishingPosition
        };

        _context.Results.Add(result);
        await _context.SaveChangesAsync();
        return Created("", result);
    }

    [HttpGet("results/my")]
    [SessionRole("Participant")]
    public async Task<IActionResult> GetMyResults()
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var results = await _context.Results
            .Include(r => r.Enrolment).ThenInclude(e => e.Event)
            .Include(r => r.Enrolment).ThenInclude(e => e.Category)
            .Where(r => r.Enrolment.UserId == userId)
            .Select(r => new {
                r.ResultId,
                EventName = r.Enrolment.Event.Name,
                EventDate = r.Enrolment.Event.EventDate,
                Category = r.Enrolment.Category.Name,
                r.FinishTime,
                r.FinishingPosition
            }).ToListAsync();

        return Ok(results);
    }
}