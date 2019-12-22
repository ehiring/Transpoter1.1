using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Activity
{
    public class Trip
    {
        public int TripID { get; set; }
        public int SourceID { get; set; }
        public int DestinationID { get; set; }
        public int VehicleID { get; set; }
        public int VehicleTypeID { get; set; }
        public DateTime TripStartDate { get; set; }
        public DateTime TripEndDate { get; set; }
        public int IsActive { get; set; }
        public int Status { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
    }
}