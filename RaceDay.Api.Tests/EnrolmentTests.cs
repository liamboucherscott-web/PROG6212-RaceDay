using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace RaceDay.Api.Tests;

public class EnrolmentTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public EnrolmentTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Enrol_ShouldFail_WhenNotLoggedIn()
    {
        var request = new { EventId = 1, CategoryId = 1 };
        var response = await _client.PostAsJsonAsync("/api/enrolments", request);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMyEnrolments_ShouldFail_WhenNotLoggedIn()
    {
        var response = await _client.GetAsync("/api/enrolments/my");
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
