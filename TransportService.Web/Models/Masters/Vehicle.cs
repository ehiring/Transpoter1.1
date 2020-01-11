using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.Design;

namespace TransportService.Web.Models.Masters
{
    public class Vehicle
    {
        public int VehicleID { get; set; }
        public int VehicleTypeID { get; set; }
        public decimal? Capacity { get; set; }
        public string VehicleNo { get; set; }
        public decimal? Height { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public string Description { get; set; }
        public string InsuredBy { get; set; }
        public string InsuranceStartDate { get; set; }
        public string InsuranceExpDate { get; set; }
        public int? OwnerID { get; set; }
        public int? GPSStatus { get; set; }
        public string Phone { get; set; }
        public string ContactName { get; set; }
        public string DocumentPath { get; set; }
        public int? IsActive { get; set; }
    }
    
}