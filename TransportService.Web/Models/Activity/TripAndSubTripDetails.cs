using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Activity
{
    public class CargoDetails
    {

        public int? TripDetailID { get; set; }
        public int? TripID { get; set; }
        public int? SubTripID { get; set; }
        public int CargoTypeID { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
        public decimal Qty{ get; set; }
    }
}