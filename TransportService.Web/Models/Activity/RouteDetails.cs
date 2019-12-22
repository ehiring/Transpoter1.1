using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace TransportService.Web.Models.Activity
{
    public class RouteDetails
    {
        
       // public int RouteID { get; set; }
        [Key]
        public int TripID { get; set; }
        //public int CityID { get; set; }
        public decimal FreeHeight { get; set; }
        public decimal FreeWidth { get; set; }
        public decimal FreeLength { get; set; }
        public decimal FreeWeight { get; set; }
    }
}