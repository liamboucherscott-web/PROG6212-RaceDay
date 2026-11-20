using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class CategoryViewModel
    {
        public int CategoryId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty; 

        public string? Description { get; set; }

        public int? MaxParticipants { get; set; }

        [Required]
        public decimal EntryFee { get; set; }
    }
}
