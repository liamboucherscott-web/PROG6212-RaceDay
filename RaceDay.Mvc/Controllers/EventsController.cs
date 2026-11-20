using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace RaceDay.Mvc.Controllers
{
    public class EventsController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public EventsController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
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

        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EventViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _clientFactory.CreateClient("RaceDayAPI");

            // 1. Handle Image Upload to Azure (via your Part 2 API)
            if (model.BannerImage != null && model.BannerImage.Length > 0)
            {
                using var content = new MultipartFormDataContent();
                var fileContent = new StreamContent(model.BannerImage.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue(model.BannerImage.ContentType);
                content.Add(fileContent, "file", model.BannerImage.FileName);

                var uploadResponse = await client.PostAsync("/api/FileUpload/upload", content);

                if (uploadResponse.IsSuccessStatusCode)
                {
                    var uploadResult = JsonSerializer.Deserialize<JsonElement>(await uploadResponse.Content.ReadAsStringAsync());
                    model.BannerImageUrl = uploadResult.GetProperty("url").GetString();
                }
            }

            // 2. Prepare the Event data to send to the API
            var userId = HttpContext.Session.GetString("UserId");
            var eventData = new
            {
                name = model.Name,
                description = model.Description,
                eventDate = model.EventDate,
                location = model.Location,
                distance = model.Distance,
                eventTypeId = model.EventTypeId,
                organiserId = int.Parse(userId!), 
                bannerImageUrl = model.BannerImageUrl
            };

            // 3. Send to API
            var json = JsonSerializer.Serialize(eventData);
            var httpContent = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/Events", httpContent); // Adjust if your API route is different

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Failed to create event.";
            return View(model);
        }
    }
}
