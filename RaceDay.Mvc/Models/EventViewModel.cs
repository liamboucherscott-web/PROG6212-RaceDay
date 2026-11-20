using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RaceDay.Mvc.Models
{
    public class EventViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required, DataType(DataType.Date)]
        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public decimal Distance { get; set; }

        [Required]
        public int EventTypeId { get; set; }

        // This holds the file the user selects from their computer
        public IFormFile? BannerImage { get; set; }

        // This will hold the URL returned from Azure after upload
        public string? BannerImageUrl { get; set; }
    }
}