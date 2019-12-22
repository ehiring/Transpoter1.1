using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Masters
{
    public class State
    {
        public int StateID { get; set; }
        public string StateName{ get; set; }
        public int IsActive{ get; set; }
        public int AddedBy{ get; set; }
        public DateTime AddedDate { get; set; }

        
    }
}