using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Activity
{
    public class SubTrip
    {
        public int SubTripID { get; set; }
        public int TripID { get; set; }
        public int SourceID { get; set; }
        public int DestinationID { get; set; }
    }
}