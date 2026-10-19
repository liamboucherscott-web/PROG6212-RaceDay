using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDay.Api.Data;
using RaceDay.Api.Helpers;
using RaceDay.Api.Models;
using RaceDay.Api.Middleware;

namespace RaceDay.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RaceDayDbContext _context;

    public AuthController(RaceDayDbContext context) => _context = context;

    public record RegisterRequest(string FirstName, string LastName, string Email, string Password, string? PhoneNumber, int RoleId);
    public record LoginRequest(string Email, string Password);

    /// <summary>
    /// Register a user.
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            return Conflict(new { message = "Email already in use." });

        var user = new User
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = PasswordHasher.HashPassword(request.Password),
            PhoneNumber = request.PhoneNumber,
            RoleId = request.RoleId
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Register), new { id = user.UserId }, new { user.UserId, user.Email, user.RoleId });
    }

    /// <summary>
    /// Login and start session.
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            return Unauthorized(new { message = "Invalid email or password." });

        HttpContext.Session.SetString("UserId", user.UserId.ToString());
        HttpContext.Session.SetString("Role", user.Role.RoleName);

        return Ok(new { user.UserId, user.FirstName, user.LastName, Role = user.Role.RoleName });
    }

    /// <summary>
    /// Logout.
    /// </summary>
    [HttpPost("logout")]
    [SessionRole("Any")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return Ok(new { message = "Logged out successfully." });
    }

    /// <summary>
    /// Get current user.
    /// </summary>
    [HttpGet("me")]
    [SessionRole("Any")]
    public async Task<IActionResult> GetMe()
    {
        var userId = int.Parse(HttpContext.Session.GetString("UserId")!);
        var user = await _context.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.UserId == userId);
        return Ok(new { user!.UserId, user.FirstName, user.LastName, user.Email, Role = user.Role.RoleName });
    }
}