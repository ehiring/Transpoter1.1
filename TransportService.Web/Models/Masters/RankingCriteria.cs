using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TransportService.Web.Models.Masters
{
    public class RankingCriteria
    {
        [Key]
        public int CriteriaQuestionID { get; set; }
        public string CriteriaQuestion { get; set; }
        public string Answer1 { get; set; }
        public decimal Answer1Value { get; set; }
        public string Answer2 { get; set; }
        public decimal Answer2Value { get; set; }
        public string Answer3 { get; set; }
        public decimal Answer3Value { get; set; }
        public string Answer4 { get; set; }
        public decimal Answer4Value  { get; set; }
        public decimal TotalValue { get; set; }

    }
}