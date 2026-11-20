using Microsoft.AspNetCore.Mvc;
using RaceDay.Mvc.Models;
using System.Net.Http.Json;

namespace RaceDay.Mvc.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public CategoriesController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var client = _clientFactory.CreateClient("RaceDayAPI");

           
            var response = await client.PostAsJsonAsync("/api/Categories", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index", "Events");
            }

            ViewBag.Error = "Failed to create category. Please check the Event ID.";
            return View(model);
        }
    }
}