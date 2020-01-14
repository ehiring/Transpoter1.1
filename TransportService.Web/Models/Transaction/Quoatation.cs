using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TransportService.Web.Models.Transaction
{
    public class Quoatation
    {
        [Key]
        public int QuotationID { get; set; }
        public decimal PrimaryQuotaionValue { get; set; }
        public int LoadID { get; set; }
        
    }
}