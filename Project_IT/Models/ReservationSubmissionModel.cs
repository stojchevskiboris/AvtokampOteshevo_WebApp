using System;

namespace Project_IT.Models
{
    public class ReservationSubmissionModel
    {
        public string Ime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Lica { get; set; }
        public string Smestuvanje { get; set; }
        public string DataNaPristignuvanje { get; set; }
        public string DataNaZaminuvanje { get; set; }
        public string Poraka { get; set; }

        // Additional data / EmailJS built-in & browser context fields
        public string UserOs { get; set; }
        public string UserIp { get; set; }
        public string UserPlatform { get; set; }
        public string UserBrowser { get; set; }
        public string UserVersion { get; set; }
        public string UserCountry { get; set; }
        public string UserReferrer { get; set; }
    }
}
