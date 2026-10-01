using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_IT.Models.ViewModels
{
    public class SendEmailRequestModel
    {
        public string From { get; set; }
        public string[] To { get; set; }
        public string Subject { get; set; }
        public string Html { get; set; }
    }
}