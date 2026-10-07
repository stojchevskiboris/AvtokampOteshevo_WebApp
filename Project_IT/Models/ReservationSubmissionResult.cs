using System.Collections.Generic;

namespace Project_IT.Models
{
    public class ReservationSubmissionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public Dictionary<string, IEnumerable<string>> ValidationErrors { get; set; }
    }
}
