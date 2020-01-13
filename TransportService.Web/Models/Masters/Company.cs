using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Masters
{
    public class Company
    {
        public int CompanyID { get; set; }
        public string CompanyName { get; set; }
        public int CompanyTypeID { get; set; }
        public int UserID { get; set; }
        public string ServiceTaxNo { get; set; }
        public string CompanyPanNo { get; set; }
        public string CompanyWebsite { get; set; }
        public string IsActive { get; set; }
    }
    public static class CompanyColumns
    {
        public const string CompanyID = "CompanyID";
        public const string CompanyName = "CompanyName";
        public const string CompanyTypeID = "CompanyTypeID";
        public const string UserID = "UserID";
        public const string ServiceTaxNo = "ServiceTaxNo";
        public const string CompanyPanNo = "CompanyPanNo";
        public const string CompanyWebsite = "CompanyWebsite";
        public const string IsActive = "IsActive";
    }
}