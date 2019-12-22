using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.ComponentModel.DataAnnotations;

namespace TransportService.Web.Models.Activity
{
    public class Loader
    {
        [Key]
        public int LoadID { get; set; }
        public int SourceID { get; set; }
        public int DestinationID { get; set; }
        public string Receiver { get; set; }
        public DateTime? PickUpDate { get; set; }
        public string LoadType { get; set; }
        public string SubService { get; set; }
        public int Status { get; set; }
        public int AddedBy { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public IEnumerable<LoadDetail> _loadDetails { get; set; }
        public StaticPagedList<Loader> _loaders { get; set; }
    }
}