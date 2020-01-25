using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TransportService.Web.Models.Activity
{
    public class TripRankingDetail
    {

        [Key]
        public int TripRankingID { get; set; }
        public int SerialNo { get; set; }
        public int CriteriaQuestionID { get; set; }
        public string Answer { get; set; }
        public decimal AnswerValue { get; set; }
    }
}