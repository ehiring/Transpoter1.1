using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportService.Web.Models;
using PagedList;
using TransportService.Web.BusinessLayer;
using System.Data.SqlClient;
using System.Data;
using TransportService.Web.Models.Activity;
namespace TransportService.Web.Controllers
{
    public class HomeController : Controller
    {
        TripBusinessLayer _tripBusinessLayer;
        #region "Construtr"
        public HomeController()
        {
            _tripBusinessLayer = new TripBusinessLayer();
        }
        #endregion


        #region "Old Code"


        public ActionResult Old_Index(int? page)
        {
            Transpoter data = new Transpoter();
            StaticPagedList<Transpoter> itemsAsIPagedList;
            itemsAsIPagedList = GridLoadList(page, "", "");

            JobDbContext _db = new JobDbContext();
            IEnumerable<RouteDetails> result1 = _db.DBRouteDetails.SqlQuery(@"exec GetTripRouteDetails").ToList<RouteDetails>();

            data.TransDetails = itemsAsIPagedList;
            data.RouteList = result1;

            ViewData["CityList"] = Old_binddropdown("CityList", 0);
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("Index", data)
                    : View("Index", data);
        }
        public StaticPagedList<Transpoter> GridLoadList(int? page, string Source = "", string Destination = "")
        {

            JobDbContext _db = new JobDbContext();
            var pageIndex = (page ?? 1);
            const int pageSize = 4;
            int totalCount = 4;
            Transpoter Llist = new Transpoter();

            IEnumerable<Transpoter> result = _db.DBTransporter.SqlQuery(@"exec USP_GetTripDetails
                   @pPageIndex, @pPageSize,@Source,@Destination",
               new SqlParameter("@pPageIndex", pageIndex),
               new SqlParameter("@pPageSize", pageSize),
               new SqlParameter("@Source", Source == null ? (object)DBNull.Value : Source),
               new SqlParameter("@Destination", Destination == null ? (object)DBNull.Value : Destination)
               ).ToList<Transpoter>();

            totalCount = 0;
            if (result.Count() > 0)
            {
                totalCount = Convert.ToInt32(result.FirstOrDefault().TotalRows);
            }
            var itemsAsIPagedList = new StaticPagedList<Transpoter>(result, pageIndex, pageSize, totalCount);
            return itemsAsIPagedList;

        }
        public List<SelectListItem> Old_binddropdown(string action, int val = 0, int TripId = 0)
        {
            JobDbContext _db = new JobDbContext();

            var res = _db.Database.SqlQuery<SelectListItem>("exec USP_BindDropDown @action , @val, @TripId",
                    new SqlParameter("@action", action),
                    new SqlParameter("@val", val),
                    new SqlParameter("@TripId", TripId))
                   .ToList()
                   .AsEnumerable()
                   .Select(r => new SelectListItem
                   {
                       Text = r.Text.ToString(),
                       Value = r.Value.ToString(),
                       Selected = r.Value.Equals(Convert.ToString(val))
                   }).ToList();

            return res;
        }
        public ActionResult Old_AddTrip()
        {
            ViewData["CityList"] = Old_binddropdown("CityList", 0);
            ViewData["VehicalTypeList"] = Old_binddropdown("VehicalTypeList", 0);
            return View();
        }
        #endregion


        #region "New Code"

        public ActionResult Test()
        {

            return View();
        }
        public ActionResult Index(int? page)
        {
            Transpoter _transpoter = new Transpoter();


            _transpoter.TransDetails = _tripBusinessLayer.GetTripDetails(page, "", "");
            _transpoter.SubTDetails = _tripBusinessLayer.GetSubTripDetails();
            _transpoter.RouteList = _tripBusinessLayer.GetTripRouteDetails();
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);

            // return View();
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("Index", _transpoter) : View("Index", _transpoter);
        }

       
        public ActionResult AddTrip()
        {
            ViewData["CityList"] =  _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["VehicalTypeList"] = _tripBusinessLayer.GetDropDownData("VehicalTypeList", 0);
            return View();
        }

        [HttpPost]
        public ActionResult SaveTrip(Transpoter T, List<CityArray> CityRootId, List<CargoDetails> CargoDetails)
        {
            DataTable dtCityRoot = new DataTable();
            dtCityRoot.Columns.Add("CityId", typeof(int));

            // Adding Contact Person In DT
            if (CityRootId != null)
            {
                if (CityRootId.Count > 0)
                {
                    foreach (var item in CityRootId)
                    {
                        DataRow dr_CityRoot = dtCityRoot.NewRow();
                        dr_CityRoot["CityId"] = item.CityId;
                        dtCityRoot.Rows.Add(dr_CityRoot);
                    }
                }
            }

            SqlParameter tvpParamCityRoot = new SqlParameter();
            tvpParamCityRoot.ParameterName = "@RouteCityIDs";
            tvpParamCityRoot.SqlDbType = System.Data.SqlDbType.Structured;
            tvpParamCityRoot.Value = dtCityRoot;
            tvpParamCityRoot.TypeName = "UDTable_RootCityIDs";

            //Adding Load Details In DT

            DataTable dtLoad = new DataTable();

            dtLoad.Columns.Add("CargoTypeId", typeof(int));
            dtLoad.Columns.Add("Height", typeof(decimal));
            dtLoad.Columns.Add("Width", typeof(decimal));
            dtLoad.Columns.Add("Length", typeof(decimal));
            dtLoad.Columns.Add("Weight", typeof(decimal));
            dtLoad.Columns.Add("Qty", typeof(int));

            if (CargoDetails != null)
            {
                if (CargoDetails.Count > 0)
                {
                    foreach (var item in CargoDetails)
                    {
                        DataRow dr_Load = dtLoad.NewRow();
                        dr_Load["CargoTypeId"] = item.CargoTypeID;
                        dr_Load["Height"] = item.Height;
                        dr_Load["Width"] = item.Width;
                        dr_Load["Length"] = item.Length;
                        dr_Load["Weight"] = item.Weight;
                        dr_Load["Qty"] = item.Qty;
                        dtLoad.Rows.Add(dr_Load);
                    }
                }
            }

            SqlParameter tvpParamLoadDetails = new SqlParameter();
            tvpParamLoadDetails.ParameterName = "@CargoDetails";
            tvpParamLoadDetails.SqlDbType = System.Data.SqlDbType.Structured;
            tvpParamLoadDetails.Value = dtLoad;
            tvpParamLoadDetails.TypeName = "UDTable_CargoDetails";


            JobDbContext _db = new JobDbContext();
            var result = _db.Database.ExecuteSqlCommand(@"exec USP_SaveTrip
                  @SourceID 
                 ,@DestinationID 
                 ,@VehicleTypeID 
                 ,@TripStartDate
                 ,@TripEndDate 
                 ,@Status 
                 ,@AddedBy
                 ,@RouteCityIDs
                 ,@CargoDetails",
            new SqlParameter("@SourceID", T.SourceID == null ? (object)DBNull.Value : T.SourceID),
            new SqlParameter("@DestinationID", T.DestinationID == null ? (object)DBNull.Value : T.DestinationID),
            new SqlParameter("@VehicleTypeID", T.VehicleTypeID == null ? (object)DBNull.Value : T.VehicleTypeID),
            new SqlParameter("@TripStartDate", T.StartDate == null ? (object)DBNull.Value : T.StartDate),
            new SqlParameter("@TripEndDate", T.EndDate == null ? (object)DBNull.Value : T.EndDate),
            new SqlParameter("@Status", T.TripStatus),
            new SqlParameter("@AddedBy", 1),
            tvpParamCityRoot,
            tvpParamLoadDetails
            );

            return Json("Trip Posted Sucessfull");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }
        #endregion
    }
}