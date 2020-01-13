using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Masters
{
    public class CompanyTypeMaster
    {
        public int CompanyTypeID { get; set; }
        public string CompanyType { get; set; }
        public int IsActive { get; set; }
        
        
    }
}