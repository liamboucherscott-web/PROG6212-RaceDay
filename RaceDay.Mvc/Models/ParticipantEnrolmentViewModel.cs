using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class ParticipantEnrolmentViewModel
    {
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;

        [Required]
        public int CategoryId { get; set; }
    }
}