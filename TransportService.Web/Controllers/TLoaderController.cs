using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using TransportService.Web.Models;
using System.Data.SqlClient;
using TransportService.Web.BusinessLayer;
using TransportService.Web.Models.Activity;
using System.Data;
using TransportService.Web.Models.Masters;

namespace TransportService.Web.Controllers
{
    public class TLoaderController : Controller
    {
        TripBusinessLayer _tripBusinessLayer;
        public TLoaderController()
        {
            _tripBusinessLayer = new TripBusinessLayer();
        }

        #region "New Code"

        public ActionResult Index(int? page)
        {

            Transpoter _transpoter = new Transpoter();
            _transpoter.TransDetails = _tripBusinessLayer.GetTripDetails(page, "", "");
            _transpoter.SubTDetails = _tripBusinessLayer.GetSubTripDetails();
            _transpoter.RouteList = _tripBusinessLayer.GetTripRouteDetails();

            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["SearchApplied"] = 0;

            //return View();
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("Index", _transpoter) : View("Index", _transpoter);
        }
        public ActionResult SearchTrips(int? page, string Source, string Destination)
        {
            Transpoter _transpoter = new Transpoter();
            _transpoter.TransDetails = _tripBusinessLayer.GetTripDetails(page, Source, Destination);
            _transpoter.SubTDetails = _tripBusinessLayer.GetSubTripDetails();
            _transpoter.RouteList = _tripBusinessLayer.GetTripRouteDetails();
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["SearchApplied"] = 1;
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("_TripList", _transpoter) : View("_TripList", _transpoter);
        }


        

        public ActionResult New_LoaderIndex(int? page)
        {

            Loader _loader = new Loader();
            _loader.Loaders = _tripBusinessLayer.GetLoaderList(page, "", "");
            _loader.LoadDetails = _tripBusinessLayer.GetLoadeDetails();
            _loader.MaterialList = _tripBusinessLayer.GetMaterialList();

            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["SearchApplied"] = 0;

            // return View();
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("New_LoaderIndex", _loader) : View("New_LoaderIndex", _loader);
        }
        public ActionResult AddTrip()
        {
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["VehicalTypeList"] = _tripBusinessLayer.GetDropDownData("VehicalTypeList", 0);
            return View();
        }


        [HttpPost]
        public ActionResult SaveTrip(Transpoter T, List<CityArray> CityRootId, List<CargoDetails> CargoDetails)
        {


            DataTable dtCityRoot = new DataTable();

            dtCityRoot.Columns.Add("CityId", typeof(int));
            dtCityRoot.Columns.Add("SequenceNo", typeof(int));


            // Adding Contact Person In DT
            if (CityRootId != null)
            {
                if (CityRootId.Count > 0)
                {
                    foreach (var item in CityRootId)
                    {
                        DataRow dr_CityRoot = dtCityRoot.NewRow();
                        dr_CityRoot["CityId"] = item.CityId;
                        dr_CityRoot["SequenceNo"] = item.SequenceNo;
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

            JobDbContext _jobDbContext = new JobDbContext();
            var result = _jobDbContext.Database.ExecuteSqlCommand(@"exec USP_SaveTrip
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
          tvpParamLoadDetails);


            return Json("Trip Posted Sucessfull");


        }



        public ActionResult AddSubTrip(int? Source, int TripId = 0)
        {
            ViewData["TripWiseCityList"] = _tripBusinessLayer.GetDropDownData("TripWiseCityList", 0, TripId);
            ViewBag.tripid = TripId;
            ViewBag.Source = Source;
            return View(); 
        }

        public JsonResult GetCityAgainstTheSource(int TripID = 0, int SourceID = 0)
        {
            var list = _tripBusinessLayer.GetDropDownData("TripAndSequenceWiseCityList", SourceID, TripID);
            return Json(list);

        }
        public ActionResult GetAvailableSpace(int? TripId, int? Source, int? Destination)
        {
            JobDbContext _db = new JobDbContext();
            AvalableSpace data = new AvalableSpace();
            var result1 = _db.DBAvalableSpace.SqlQuery(@"exec USP_GetAvailableSpace @TripId,@SourceId,@Destination",
                new SqlParameter("@TripId", TripId),
                new SqlParameter("@SourceId", Source),
                new SqlParameter("@Destination", Destination)).ToList<AvalableSpace>();
            data = result1.FirstOrDefault();
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("_AvailabeLoadSpace", data)
                    : View("_AvailabeLoadSpace", data);


        }
        [HttpPost]
        public ActionResult SaveSubTrip(int? sourceId, int? destinationId, int? TripId, List<CargoDetails> CargoDetails)
        {

            //Adding Load Details In DT

            DataTable dtLoad = new DataTable();

            dtLoad.Columns.Add("CargoTypeID", typeof(int));
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
                        dr_Load["CargoTypeID"] = item.CargoTypeID;
                        dr_Load["Height"] = item.Height;
                        dr_Load["Width"] = item.Width;
                        dr_Load["Length"] = item.Length;
                        dr_Load["Weight"] = item.Weight;
                        dr_Load["Qty"] = item.Qty;
                        dtLoad.Rows.Add(dr_Load);
                    }
                }
            }



            SqlParameter tvpParamCargoDetails = new SqlParameter();
            tvpParamCargoDetails.ParameterName = "@UDTable_CargoDetails";
            tvpParamCargoDetails.SqlDbType = System.Data.SqlDbType.Structured;
            tvpParamCargoDetails.Value = dtLoad;
            tvpParamCargoDetails.TypeName = "UDTable_CargoDetails";



            JobDbContext _db = new JobDbContext();
            var result = _db.Database.ExecuteSqlCommand(@"exec USP_SaveSubtrip
                  @SourceID 
                 ,@DestinationID 
                 ,@TripID 
                 ,@UDTable_CargoDetails",
            new SqlParameter("@SourceID", sourceId == null ? (object)DBNull.Value : sourceId),
            new SqlParameter("@DestinationID", destinationId == null ? (object)DBNull.Value : destinationId),
            new SqlParameter("@TripID", TripId == null ? (object)DBNull.Value : TripId),
            tvpParamCargoDetails
            );


            //DBStatus.ParameterName = "@DBStatus";
            //DBStatus.SqlDbType = SqlDbType.VarChar;
            //DBStatus.Direction = ParameterDirection.Output;


            return Json("Your SubTrip Booked Sucessfully");

        }
        public ActionResult AddTruck()
        {
            ViewData["TruckCapacityList"] = _tripBusinessLayer.GetDropDownData("TruckCapacityList");
            ViewData["VehicalTypeList"] = _tripBusinessLayer.GetDropDownData("VehicalTypeList");
            return View();
        }
        [HttpPost]
        public ActionResult AddTruck1(Vehicle _vehicle)
        {
            JobDbContext jobDbContext = new JobDbContext();
            var result = jobDbContext.Database.ExecuteSqlCommand(@"exec USP_AddVehicle @VehicleTypeID ,
            @CapacityID ,
            @VehicleNo ,
            @Height ,
            @Width ,
            @Length ,
            @Description ,
            @InsuredBy  ,
            @InsuranceStartDate ,
            @InsuranceExpDate ,
            @OwnerID ,
            @GPSStatus ,
            @Phone ,
            @ContactName ",
            new SqlParameter("@VehicleTypeID", _vehicle.VehicleTypeID),
             new SqlParameter("@CapacityID", _vehicle.CapacityID),
              new SqlParameter("@VehicleNo", _vehicle.VehicleNo),
               new SqlParameter("@Height", _vehicle.Height == null ? (object)DBNull.Value : _vehicle.Height),
                new SqlParameter("@Width", _vehicle.Width == null ? (object)DBNull.Value : _vehicle.Width),
                 new SqlParameter("@Length", _vehicle.Length == null ? (object)DBNull.Value : _vehicle.Length),
                  new SqlParameter("@Description", _vehicle.Description == null ? (object)DBNull.Value : _vehicle.Description),
                   new SqlParameter("@InsuredBy", _vehicle.InsuredBy == null ? (object)DBNull.Value : _vehicle.InsuredBy),
                    new SqlParameter("@InsuranceStartDate", _vehicle.InsuranceStartDate == null ? (object)DBNull.Value : _vehicle.InsuranceStartDate),
                     new SqlParameter("@InsuranceExpDate", _vehicle.InsuranceExpDate == null ? (object)DBNull.Value : _vehicle.InsuranceExpDate),
                      new SqlParameter("@OwnerID", _vehicle.OwnerID == null ? (object)DBNull.Value : _vehicle.OwnerID),
                       new SqlParameter("@GPSStatus", _vehicle.GPSStatus == null ? (object)DBNull.Value : _vehicle.GPSStatus),
                        new SqlParameter("@Phone", _vehicle.Phone == null ? (object)DBNull.Value : _vehicle.Phone),
                        new SqlParameter("@ContactName", _vehicle.ContactName == null ? (object)DBNull.Value : _vehicle.ContactName));
            return Json("Truck Added Sucessfully");
        }


        public ActionResult AddLoad()
        {
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            //MaterialList
            ViewData["MaterialList"] = _tripBusinessLayer.GetDropDownData("MaterialList", 0);
            return View();
        }

        [HttpPost]
        public ActionResult AddLoad(Loader _loader, List<LoadDetail> _loadDetails)
        {

            //Adding Load Details In DT

            DataTable dtLoadDetail = new DataTable();

            dtLoadDetail.Columns.Add("MaterialID", typeof(int));
            dtLoadDetail.Columns.Add("UnitOfMeasure", typeof(string));
            dtLoadDetail.Columns.Add("Height", typeof(decimal));
            dtLoadDetail.Columns.Add("Width", typeof(decimal));
            dtLoadDetail.Columns.Add("Length", typeof(decimal));
            dtLoadDetail.Columns.Add("Weight", typeof(decimal));
            dtLoadDetail.Columns.Add("Qty", typeof(int));

            if (_loadDetails != null)
            {
                if (_loadDetails.Count > 0)
                {
                    foreach (var item in _loadDetails)
                    {
                        DataRow dr_LoadDetail = dtLoadDetail.NewRow();
                        dr_LoadDetail["MaterialID"] = item.MaterialID;
                        dr_LoadDetail["UnitOfMeasure"] = item.UnitOfMeasure;
                        dr_LoadDetail["Height"] = item.Height;
                        dr_LoadDetail["Width"] = item.Width;
                        dr_LoadDetail["Length"] = item.Length;
                        dr_LoadDetail["Weight"] = item.Weight;
                        dr_LoadDetail["Qty"] = item.Qty;
                        dtLoadDetail.Rows.Add(dr_LoadDetail);
                    }
                }
            }

            SqlParameter tvpParamLoadDetails = new SqlParameter();
            tvpParamLoadDetails.ParameterName = "@UDTable_LoadDetails";
            tvpParamLoadDetails.SqlDbType = System.Data.SqlDbType.Structured;
            tvpParamLoadDetails.Value = dtLoadDetail;
            tvpParamLoadDetails.TypeName = "UDTable_LoadDetails";
            //@PickUpDate,
            JobDbContext _jobDbContext = new JobDbContext();
            var result = _jobDbContext.Database.ExecuteSqlCommand(@"exec USP_SaveLoad
                        @SourceID ,
                        @DestinationID, 
                        @PickUpDate,
                        @LoadType ,
                        @SubService ,
                        @ContactNo ,
                        @Email ,
                        @Address ,
                        @Status ,
                        @AddedBy ,
                        @Receiver,
                        @UDTable_LoadDetails",
          new SqlParameter("@SourceID", _loader.SourceID),
          new SqlParameter("@DestinationID", _loader.DestinationID),
          new SqlParameter("@PickUpDate", _loader.PickUpDate),
          new SqlParameter("@LoadType", _loader.LoadType),
          new SqlParameter("@SubService", _loader.SubService),
          new SqlParameter("@ContactNo", _loader.ContactNo),
          new SqlParameter("@Email", _loader.Email),
          new SqlParameter("@Address", _loader.Address),
           new SqlParameter("@Status", 1),
          new SqlParameter("@AddedBy", 1),/*.....UserID 1 is Loader*/
          new SqlParameter("@Receiver", _loader.Receiver),
          tvpParamLoadDetails);
            return Json("Load Added Sucessfully");

        }

        public ActionResult EditLoad(int LoadID)
        {
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["MaterialList"] = _tripBusinessLayer.GetDropDownData("MaterialList", 0);
            IEnumerable<LoaderEdit> LE = _tripBusinessLayer.GetLoaderByID(LoadID);
            LoaderEdit data = new LoaderEdit();
           
            
            data = LE.FirstOrDefault();
            data.LoadDetails = _tripBusinessLayer.GetLoadDetailsByID(LoadID);
            ViewData["LoadDetailCount"] = data.LoadDetails.Count<LoadDetail>();
            return View(data);

        }

        [HttpPost]
        public ActionResult EditLoad(Loader _loader, List<LoadDetail> _loadDetails=null)
        {

            //Adding Load Details In DT

            DataTable dtLoadDetail = new DataTable();

            dtLoadDetail.Columns.Add("MaterialID", typeof(int));
            dtLoadDetail.Columns.Add("UnitOfMeasure", typeof(string));
            dtLoadDetail.Columns.Add("Height", typeof(decimal));
            dtLoadDetail.Columns.Add("Width", typeof(decimal));
            dtLoadDetail.Columns.Add("Length", typeof(decimal));
            dtLoadDetail.Columns.Add("Weight", typeof(decimal));
            dtLoadDetail.Columns.Add("Qty", typeof(int));

            if (_loadDetails != null)
            {
                if (_loadDetails.Count > 0)
                {
                    foreach (var item in _loadDetails)
                    {
                        DataRow dr_LoadDetail = dtLoadDetail.NewRow();
                        dr_LoadDetail["MaterialID"] = item.MaterialID;
                        dr_LoadDetail["UnitOfMeasure"] = item.UnitOfMeasure;
                        dr_LoadDetail["Height"] = item.Height;
                        dr_LoadDetail["Width"] = item.Width;
                        dr_LoadDetail["Length"] = item.Length;
                        dr_LoadDetail["Weight"] = item.Weight;
                        dr_LoadDetail["Qty"] = item.Qty;
                        dtLoadDetail.Rows.Add(dr_LoadDetail);
                    }
                }
            }

            SqlParameter tvpParamLoadDetails = new SqlParameter();
            tvpParamLoadDetails.ParameterName = "@UDTable_LoadDetails";
            tvpParamLoadDetails.SqlDbType = System.Data.SqlDbType.Structured;
            tvpParamLoadDetails.Value = dtLoadDetail;
            tvpParamLoadDetails.TypeName = "UDTable_LoadDetails";
            //@PickUpDate,
            JobDbContext _jobDbContext = new JobDbContext();
            var result = _jobDbContext.Database.ExecuteSqlCommand(@"exec USP_UpdateLoadWhereID
                        @LoadID,
                        @SourceID ,
                        @DestinationID, 
                        @PickUpDate,
                        @LoadType ,
                        @SubService ,
                        @ContactNo ,
                        @Email ,
                        @Address ,
                        @Status ,
                        @AddedBy ,
                        @Receiver,
                        @UDTable_LoadDetails",
          new SqlParameter("@LoadID", _loader.LoadID),
          new SqlParameter("@SourceID", _loader.SourceID),
          new SqlParameter("@DestinationID", _loader.DestinationID),
          new SqlParameter("@PickUpDate", _loader.PickUpDate),
          new SqlParameter("@LoadType", _loader.LoadType),
          new SqlParameter("@SubService", _loader.SubService),
          new SqlParameter("@ContactNo", _loader.ContactNo),
          new SqlParameter("@Email", _loader.Email),
          new SqlParameter("@Address", _loader.Address),
           new SqlParameter("@Status", _loader.Status == null ? 0:_loader.Status),
          new SqlParameter("@AddedBy", 1),/*.....UserID 1 is Loader*/
          new SqlParameter("@Receiver", _loader.Receiver),
          tvpParamLoadDetails);
            return Json("Load Updated Sucessfully");

        }


        public ActionResult test()
        {            
            return View();
        }
        #endregion

    }
}