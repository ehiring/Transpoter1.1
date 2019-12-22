using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Masters
{
    public class UserType
    {
        public int UserTypeID { get; set; }
        public string UserTypeName { get; set; }
        public int IsActive { get; set; }

    }
}