using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
	

	

namespace TransportService.Web.Models.Masters
{
    public class Client
    {
        [Key]
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
    public static class ClientColumns
    {

        public const string ClientID = "ClientID";
        public const string ClientTypeID = "ClientTypeID";
        public const string Name = "Name";
        public const string Email = "Email";
        public const string Mobile = "Mobile";
        public const string Address1 = "Address1";
        public const string Address2 = "Address2";
        public const string ContactPerson = "ContactPerson";
        public const string ContactPersonMobile = "ContactPersonMobile";
        public const string GSTNumber = "GSTNumber";
        public const string DocumentPath = "DocumentPath";
        public const string IsActive = "IsActive";
    }

}