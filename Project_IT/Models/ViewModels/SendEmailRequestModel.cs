using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_IT.Models.ViewModels
{
    public class SendEmailRequestModel
    {
        [JsonProperty("from")]
        public string From { get; set; }

        [JsonProperty("to")]
        public string[] To { get; set; }

        [JsonProperty("subject")]
        public string Subject { get; set; }

        [JsonProperty("html")]
        public string Html { get; set; }
    }
}
