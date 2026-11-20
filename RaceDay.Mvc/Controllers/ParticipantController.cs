using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class ParticipantController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public ParticipantController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

       
        public async Task<IActionResult> BrowseEvents()
        {
            var client = _clientFactory.CreateClient("RaceDayAPI");

            
            var response = await client.GetAsync("/api/Events");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var events = JsonSerializer.Deserialize<List<EventViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(events);
            }

            return View(new List<EventViewModel>());
        }
    }
}