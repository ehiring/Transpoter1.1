using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Activity
{
    public class TripRankingSummary
    {
        public int TripRankingID { get; set; }
        public int TripID { get; set; }
        public int SubTripID { get; set; }
        public int TransporterID { get; set; }
        public int LoaderID { get; set; }
        public decimal TotalValue { get; set; }
        public decimal OutOfFive { get; set; }
        public int LoaderUserID { get; set; }
        public int TransporterUserID { get; set; }
        public bool LikeToUseBhada { get; set; }
        public string AnyOtherQuestion { get; set; }
    }
}