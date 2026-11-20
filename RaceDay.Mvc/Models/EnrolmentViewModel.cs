namespace RaceDay.Mvc.Models
{
    public class EnrolmentViewModel
    {
        public int EnrolmentId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string ParticipantName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; 
        public DateTime EnrolmentDate { get; set; }
    }
}