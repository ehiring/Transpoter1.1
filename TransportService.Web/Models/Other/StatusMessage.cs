using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Other
{
    public class StatusMessage
    {
        public string Message { get; set; }
        public string RedirectViewName { get; set; }
        public string RedirectViewLabel { get; set; }
    }
}