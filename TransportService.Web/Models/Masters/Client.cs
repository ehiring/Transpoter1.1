﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Masters
{
    public class Client
    {
        public int ClientID { get; set; }
        public int ClientTypeID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonMobile { get; set; }
        public string GSTNumber { get; set; }
        public string DocumentPath { get; set; }
        public int IsActive { get; set; }
    }
}