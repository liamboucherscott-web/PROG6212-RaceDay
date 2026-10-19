using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Models;
using RaceDay.Api.Middleware;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RaceDayDbContext _context;
    public UsersController(RaceDayDbContext context) => _context = context;

    public record ProfileRequest(string FirstName, string LastName, string? PhoneNumber);

    [HttpGet("{id}")]
    [SessionRole("Any")]
    public async Task<IActionResult> GetProfile(int id)
    {
        var loggedInUserId = int.Parse(HttpContext.Session.GetString("UserId")!);

        if (id != loggedInUserId) return Forbid();

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        return Ok(new { user.UserId, user.FirstName, user.LastName, user.Email, user.PhoneNumber, user.ProfileImageUrl });
    }

    [HttpPut("{id}")]
    [SessionRole("Any")]
    public async Task<IActionResult> UpdateProfile(int id, ProfileRequest request)
    {
        var loggedInUserId = int.Parse(HttpContext.Session.GetString("UserId")!);

        if (id != loggedInUserId) return Forbid();

        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        await _context.SaveChangesAsync();
        return Ok(new { user.UserId, user.FirstName, user.LastName, user.PhoneNumber });
    }
}