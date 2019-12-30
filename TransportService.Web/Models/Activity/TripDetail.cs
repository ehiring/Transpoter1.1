using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
namespace TransportService.Web.Models.Activity
{
    public class TripDetail
    {
       
            [Key]
            public int? TripDetailID { get; set; }
            public int? TripID { get; set; }
            public int MaterialID { get; set; }
            public string MaterialName { get; set; }
            public string UnitOfMeasure { get; set; }
            public decimal Length { get; set; }
            public decimal Width { get; set; }
            public decimal Height { get; set; }
            public decimal Weight { get; set; }
            public int Qty { get; set; }
            public int IsDeleted { get; set; }
        public decimal ConversionFactor { get; set; }

    }
}