using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Masters
{
    public class ClientType
    {
        public int ClientTypeID { get; set; }
        public string  ClientTypeName { get; set; }
        public int IsActive { get; set; }

    }
}