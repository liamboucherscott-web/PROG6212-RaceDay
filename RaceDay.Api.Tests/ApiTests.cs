using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RaceDay.Api.Tests;

public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_ShouldFail_IfEmailExists()
    {
        var request = new
        {
            FirstName = "Test",
            LastName = "User",
            Email = "thabo.organiser@raceday.co.za",
            Password = "password123",
            PhoneNumber = "0821234567",
            RoleId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_ShouldFail_WithWrongPassword()
    {
        var request = new { Email = "test@test.com", Password = "wrongpassword" };

        var response = await _client.PostAsJsonAsync("/api/auth/login", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task CreateEvent_ShouldFail_WhenNotLoggedIn()
    {
        var request = new
        {
            Name = "Test Event",
            Description = "Test",
            EventDate = "2026-01-01",
            Location = "Test",
            Distance = 10.0,
            EventTypeId = 1
        };

        var response = await _client.PostAsJsonAsync("/api/events", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetEvents_ShouldFail_WhenNotLoggedIn()
    {
        var response = await _client.GetAsync("/api/events");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}