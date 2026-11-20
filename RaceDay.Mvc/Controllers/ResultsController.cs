using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Net.Http.Json;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class ResultsController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public ResultsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        
        public async Task<IActionResult> Index(int eventId)
        {
            var client = _clientFactory.CreateClient("RaceDayAPI");

            
            var response = await client.GetAsync($"/api/Enrolments?eventId={eventId}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var enrolments = JsonSerializer.Deserialize<List<EnrolmentViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(enrolments);
            }

            return View(new List<EnrolmentViewModel>());
        }

        
        public IActionResult Create(int enrolmentId)
        {
            
            var model = new ResultViewModel { EnrolmentId = enrolmentId };
            return View(model);
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(ResultViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _clientFactory.CreateClient("RaceDayAPI");

            
            var response = await client.PostAsJsonAsync("/api/Results", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Enrolments");
            }

            ViewBag.Error = "Failed to capture result.";
            return View(model);
        }
    }
}
