using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Masters
{
    public class City
    {

        public int CityID { get; set; }
        public string CityName { get; set; }
        public int StateID { get; set; }
        public int IsActive { get; set; }
        public int AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
         
        }
    }