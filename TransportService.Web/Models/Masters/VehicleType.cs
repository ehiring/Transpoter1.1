using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Masters
{
    public class VehicleType
    {
        public int VehicleTypeID { get; set; }
        public string VehicleTypeName { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Capacity { get; set; }
        public int IsActive { get; set; }
        public int AddedBy { get; set; }
    }
}