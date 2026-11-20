using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.Models
{
    public class ResultViewModel
    {
        public int ResultId { get; set; }

        [Required]
        public int EnrolmentId { get; set; }

        [Required]
        public string ParticipantName { get; set; } = string.Empty;

        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public string CategoryName { get; set; } = string.Empty;

        [Required]
        public TimeSpan FinishTime { get; set; }

        [Required]
        public int FinishingPosition { get; set; }
    }
}