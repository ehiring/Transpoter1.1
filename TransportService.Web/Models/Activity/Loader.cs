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

        [Required]
        [Display(Name ="Source")]
        public int SourceID { get; set; }

        [Required]
        [Display(Name = "Destination")]
        public int? DestinationID { get; set; }

        public string Source { get; set; }
        public string Destination { get; set; }

            [Required]
            [Display(Name = "Receiver")]
            public string Receiver { get; set; }

        [DataType(DataType.Date)]
        [Required]
        [Display(Name = "Pick Up Date")]
        public DateTime? PickUpDate { get; set; }

        public string LoadType { get; set; }
        public string SubService { get; set; }
        public int? Status { get; set; }
        public string LoadStatus { get; set; }
        public int AddedBy { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int? TotalRows { get; set; }
        public decimal? PrimaryQuotaionValue { get; set; }
        public IEnumerable<LoadDetail> LoadDetails { get; set; }
        public IEnumerable<MaterialList> MaterialList { get; set; }
        public StaticPagedList<Loader> Loaders { get; set; }
    }

    public class LoaderEdit
    {
        [Key]
        public int LoadID { get; set; }
        public int SourceID { get; set; }
        public int? DestinationID { get; set; }
        public string Receiver { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? PickUpDate { get; set; }
        public string LoadType { get; set; }
        public string SubService { get; set; }
        public int? Status { get; set; }
        public int? AddedBy { get; set; }
        public string ContactNo { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public IEnumerable<LoadDetail> LoadDetails { get; set; }
    }
    public class MaterialList
    {
        [Key]
        public int LoadID { get; set; }
        public int LoadDetailID { get; set; }
        public string MaterialName { get; set; }
        public decimal? TotalWeight { get; set; }
    }
}