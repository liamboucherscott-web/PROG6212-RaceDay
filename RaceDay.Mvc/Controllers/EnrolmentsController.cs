using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public EnrolmentsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

       
        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("RaceDayAPI");

           
            var response = await client.GetAsync("/api/Enrolments");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

              
                var enrolments = JsonSerializer.Deserialize<List<EnrolmentViewModel>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return View(enrolments ?? new List<EnrolmentViewModel>());
            }

            return View(new List<EnrolmentViewModel>());
        }
    }
}
