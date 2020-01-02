using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using PagedList;
using TransportService.Web.Models.Activity;
using TransportService.Web.Models.Masters;
namespace TransportService.Web.Models
{

    #region "Old Classes"

   
    public class Trans
    {
    }

    public class Old_Transpoter
    {
        [Key]
        public int TripID { get; set; }
        public int? SourceID { get; set; }
        public int? DestinationID { get; set; }
        public int? VehicalId { get; set; }
        public string VehicalType { get; set; }
        public string stratdate { get; set; }
        public string enddate { get; set; }
        public string Source { get; set; }
        public string Destination { get; set; }
        public string Route_Details { get; set; }
        public string TripStatus { get; set; }
        public int? TotalRows { get; set; }
        public IEnumerable<RouteDetails> RouteList { get; set; }
        public StaticPagedList<Transpoter> TransDetails { get; set; }
        public IEnumerable<subtripDetails> SubTDetails { get; set; }



    }

   
    public class  Old_RouteDetails
    {
        [Key]
        public  int TripID { get; set; }
        public decimal? FreeHeight { get; set; }
        public decimal? FreeWidth { get; set; }
        public decimal? FreeLength { get; set; }
        public decimal? FreeWeight { get; set; }
    }
    public class Old_subtripDetails
    {
        [Key]
        public int SubTripID { get; set; }
        public int TripID { get; set; }
        public string SourceName { get; set; }
        public string DestinationName { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
    }

    #endregion


    public class Transpoter
    {
        public Transpoter()
        {
            
        }
        [Key]
        public int TripID { get; set; }
        public int? SourceID { get; set; }
        public int? DestinationID { get; set; }
        public int? VehicleID { get; set; }
        public string VehicleType { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? TripStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? TripEndDate { get; set; }

        public string Source { get; set; }
        public string Destination { get; set; }
        public string Route_Details { get; set; }
        public string TripStatus { get; set; }
        public int? TotalRows { get; set; }
        public IEnumerable<TripDetail> TripDetails { get; set; }
        public IEnumerable<RouteDetails> RouteList { get; set; }
        public StaticPagedList<Transpoter> TransDetails { get; set; }
        public IEnumerable<subtripDetails> SubTDetails { get; set; }
        



    }
    public class TranspoterEdit
    {
        public TranspoterEdit()
        {

        }
        [Key]
        public int TripID { get; set; }
        public int? SourceID { get; set; }
        public int? DestinationID { get; set; }
        public int? VehicleID { get; set; }
        

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? TripStartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? TripEndDate { get; set; }
        
        
        public int Status { get; set; }
        
        public IEnumerable<TripDetail> TripDetails { get; set; }
        public IEnumerable<RouteDetails> RouteList { get; set; }
        
    }


    public class CityArray
    {
        [Key]
        public int CityId { get; set; }
        public int SequenceNo { get; set; }


    }
    public class AvalableSpace
    {

        [Key]
        public decimal? FreeHeight { get; set; }
        public decimal? FreeWidth { get; set; }
        public decimal? FreeLength { get; set; }
        public decimal? FreeWeight { get; set; }

    }
    public class subtripDetails
    {
        [Key]
        public int SubTripID { get; set; }
        public int TripID { get; set; }
        public string SourceName { get; set; }
        public string DestinationName { get; set; }
        public decimal Height { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
    }

}