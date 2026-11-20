using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Net.Http.Json;
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

        public async Task<IActionResult> Enrol(int eventId)
        {
            var client = _clientFactory.CreateClient("RaceDayAPI");

            
            var response = await client.GetAsync($"/api/Events/{eventId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var eventDetails = JsonSerializer.Deserialize<EventViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                var model = new ParticipantEnrolmentViewModel
                {
                    EventId = eventDetails.EventId,
                    EventName = eventDetails.Name
                };
                return View(model);
            }

            return RedirectToAction("BrowseEvents");
        }

        
        [HttpPost]
        public async Task<IActionResult> Enrol(ParticipantEnrolmentViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _clientFactory.CreateClient("RaceDayAPI");
            var userId = HttpContext.Session.GetString("UserId");

            
            var enrolmentData = new
            {
                userId = int.Parse(userId!),
                eventId = model.EventId,
                categoryId = model.CategoryId
            };

          
            var response = await client.PostAsJsonAsync("/api/Enrolments", enrolmentData);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("MyEvents"); 
            }

            ViewBag.Error = "Failed to enrol. Please check the category ID.";
            return View(model);
        }
    }
}