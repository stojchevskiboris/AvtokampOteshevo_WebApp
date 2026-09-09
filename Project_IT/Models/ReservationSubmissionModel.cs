using System;

namespace Project_IT.Models
{
    public class ReservationSubmissionModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Guests { get; set; }
        public string AccommodationType { get; set; }
        public string CheckInDate { get; set; }
        public string CheckOutDate { get; set; }
        public string Info { get; set; }

        // Additional data / EmailJS built-in & browser context fields
        public string UserOs { get; set; }
        public string UserIp { get; set; }
        public string UserPlatform { get; set; }
        public string UserAgent { get; set; }
        public string UserBrowser { get; set; }
        public string UserVersion { get; set; }
        public string UserCountry { get; set; }
        public string UserReferrer { get; set; }
    }
}
