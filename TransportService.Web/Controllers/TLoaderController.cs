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
using TransportService.Web.Models;

namespace TransportService.Web.Controllers
{
    public class TLoaderController : Controller
    {

        TripBusinessLayer _tripBusinessLayer;
        public TLoaderController()
        {
            _tripBusinessLayer = new TripBusinessLayer();
        }

        #region "Trip Methods"

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
        public ActionResult AddTrip()
        {
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["VehicleList"] = _tripBusinessLayer.GetDropDownData("VehicleList", 1); //pass @val= ownerID i.e. Login ClientID
            ViewData["MaterialList"] = _tripBusinessLayer.GetDropDownData("MaterialList", 0);
            ViewData["UOMList"] = _tripBusinessLayer.GetDropDownData("UOMList", 0);
            return View();
        }

        [HttpPost]
        public ActionResult SaveTrip(Transpoter T, List<CityArray> CityRootId, List<TripDetail> _tripDetails)
        {
            DataTable dtCityRoot = new DataTable();

            dtCityRoot.Columns.Add("CityID", typeof(int));
            dtCityRoot.Columns.Add("SequenceNo", typeof(int));

            // Adding Contact Person In DT
            if (CityRootId != null)
            {
                if (CityRootId.Count > 0)
                {
                    foreach (var item in CityRootId)
                    {
                        DataRow dr_CityRoot = dtCityRoot.NewRow();
                        dr_CityRoot["CityID"] = item.CityID;
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

            DataTable dtTripDetail = new DataTable();
            dtTripDetail.Columns.Add("TripDetailID", typeof(int));
            dtTripDetail.Columns.Add("MaterialID", typeof(int));
            dtTripDetail.Columns.Add("UnitOfMeasure", typeof(string));
            dtTripDetail.Columns.Add("Height", typeof(decimal));
            dtTripDetail.Columns.Add("Width", typeof(decimal));
            dtTripDetail.Columns.Add("Length", typeof(decimal));
            dtTripDetail.Columns.Add("Weight", typeof(decimal));
            dtTripDetail.Columns.Add("Qty", typeof(int));
            dtTripDetail.Columns.Add("ConversionFactor", typeof(decimal));


            if (_tripDetails != null)
            {
                if (_tripDetails.Count > 0)
                {
                    foreach (var item in _tripDetails)
                    {
                        DataRow dr_LoadDetail = dtTripDetail.NewRow();
                        dr_LoadDetail["TripDetailID"] = item.TripDetailID;
                        dr_LoadDetail["MaterialID"] = item.MaterialID;
                        dr_LoadDetail["ConversionFactor"] = item.ConversionFactor;
                        dr_LoadDetail["UnitOfMeasure"] = item.UnitOfMeasure;
                        dr_LoadDetail["Height"] = item.Height;
                        dr_LoadDetail["Width"] = item.Width;
                        dr_LoadDetail["Length"] = item.Length;
                        dr_LoadDetail["Weight"] = item.Weight;
                        dr_LoadDetail["Qty"] = item.Qty;
                        dtTripDetail.Rows.Add(dr_LoadDetail);
                    }
                }
            }

            SqlParameter tvpParamTripDetails = new SqlParameter();
            tvpParamTripDetails.ParameterName = "@UDTable_TripDetails";
            tvpParamTripDetails.SqlDbType = System.Data.SqlDbType.Structured;
            tvpParamTripDetails.Value = dtTripDetail;
            tvpParamTripDetails.TypeName = "UDTable_TripDetails";

            JobDbContext _jobDbContext = new JobDbContext();
            var result = _jobDbContext.Database.ExecuteSqlCommand(@"exec USP_SaveTrip
                  @SourceID 
                 ,@DestinationID 
                 ,@VehicleID 
                 ,@TripStartDate
                 ,@TripEndDate 
                 ,@Status 
                 ,@AddedBy
                 ,@RouteCityIDs
                 ,@UDTable_TripDetails",
          new SqlParameter("@SourceID", T.SourceID == null ? (object)DBNull.Value : T.SourceID),
          new SqlParameter("@DestinationID", T.DestinationID == null ? (object)DBNull.Value : T.DestinationID),
          new SqlParameter("@VehicleID", T.VehicleID == null ? (object)DBNull.Value : T.VehicleID),
          new SqlParameter("@TripStartDate", T.TripStartDate == null ? (object)DBNull.Value : T.TripStartDate),
          new SqlParameter("@TripEndDate", T.TripEndDate == null ? (object)DBNull.Value : T.TripEndDate),
          new SqlParameter("@Status", T.TripStatus),
          new SqlParameter("@AddedBy", 1),
          tvpParamCityRoot,
          tvpParamTripDetails);
            return Json("Trip Posted Sucessfull");


        }
        public ActionResult EditTrip(int TripID)
        {
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["VehicleList"] = _tripBusinessLayer.GetDropDownData("VehicleList", 1); //pass @val= ownerID i.e. Login ClientID
            ViewData["MaterialList"] = _tripBusinessLayer.GetDropDownData("MaterialList", 0);
            ViewData["UOMList"] = _tripBusinessLayer.GetDropDownData("UOMList", 0);


            TranspoterEdit transpoterEdit = _tripBusinessLayer.GetTripByID(TripID);
            transpoterEdit.TripDetails = _tripBusinessLayer.GetTripDetailsByID(TripID);
            transpoterEdit.CityArray = _tripBusinessLayer.GetRouteWhereTripID(TripID);

            return View(transpoterEdit);
        }

        [HttpPost]
        public ActionResult EditTrip(Transpoter transpoter, List<CityArray> CityRootId, List<TripDetail> _tripDetails)
        {
            DataTable dtCityRoot = new DataTable();

            dtCityRoot.Columns.Add("CityID", typeof(int));
            dtCityRoot.Columns.Add("SequenceNo", typeof(int));

            // Adding Contact Person In DT
            if (CityRootId != null)
            {
                if (CityRootId.Count > 0)
                {
                    foreach (var item in CityRootId)
                    {
                        DataRow dr_CityRoot = dtCityRoot.NewRow();
                        dr_CityRoot["CityID"] = item.CityID;
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


            DataTable dtTripDetail = new DataTable();
            dtTripDetail.Columns.Add("TripDetailID", typeof(int));
            dtTripDetail.Columns.Add("MaterialID", typeof(int));
            dtTripDetail.Columns.Add("UnitOfMeasure", typeof(string));
            dtTripDetail.Columns.Add("Height", typeof(decimal));
            dtTripDetail.Columns.Add("Width", typeof(decimal));
            dtTripDetail.Columns.Add("Length", typeof(decimal));
            dtTripDetail.Columns.Add("Weight", typeof(decimal));
            dtTripDetail.Columns.Add("Qty", typeof(int));
            dtTripDetail.Columns.Add("ConversionFactor", typeof(decimal));


            if (_tripDetails != null)
            {
                if (_tripDetails.Count > 0)
                {
                    foreach (var item in _tripDetails)
                    {
                        DataRow dr_LoadDetail = dtTripDetail.NewRow();
                        dr_LoadDetail["TripDetailID"] = item.TripDetailID;
                        dr_LoadDetail["MaterialID"] = item.MaterialID;
                        dr_LoadDetail["ConversionFactor"] = item.ConversionFactor;
                        dr_LoadDetail["UnitOfMeasure"] = item.UnitOfMeasure;
                        dr_LoadDetail["Height"] = item.Height;
                        dr_LoadDetail["Width"] = item.Width;
                        dr_LoadDetail["Length"] = item.Length;
                        dr_LoadDetail["Weight"] = item.Weight;
                        dr_LoadDetail["Qty"] = item.Qty;
                        dtTripDetail.Rows.Add(dr_LoadDetail);
                    }
                }
            }

            SqlParameter tvpParamTripDetails = new SqlParameter();
            tvpParamTripDetails.ParameterName = "@UDTable_TripDetails";
            tvpParamTripDetails.SqlDbType = System.Data.SqlDbType.Structured;
            tvpParamTripDetails.Value = dtTripDetail;
            tvpParamTripDetails.TypeName = "UDTable_TripDetails";

            using (JobDbContext _jobDbContext = new JobDbContext())
            {
                var result = _jobDbContext.Database.ExecuteSqlCommand(@"exec USP_UpdateTripWhereID
                    @TripID,
                    @SourceID, 
                    @DestinationID ,
                    @VehicleID, 
                    @TripStartDate,
                    @TripEndDate ,
                    @Status ,
                    @AddedBy,
                    @RouteCityIDs,
                    @UDTable_TripDetails",
              new SqlParameter("@TripID", transpoter.TripID),
              new SqlParameter("@SourceID", transpoter.SourceID == null ? (object)DBNull.Value : transpoter.SourceID),
              new SqlParameter("@DestinationID", transpoter.DestinationID == null ? (object)DBNull.Value : transpoter.DestinationID),
              new SqlParameter("@VehicleID", transpoter.VehicleID == null ? (object)DBNull.Value : transpoter.VehicleID),
              new SqlParameter("@TripStartDate", transpoter.TripStartDate == null ? (object)DBNull.Value : transpoter.TripStartDate),
              new SqlParameter("@TripEndDate", transpoter.TripEndDate == null ? (object)DBNull.Value : transpoter.TripEndDate),
              new SqlParameter("@Status", transpoter.TripStatus),
              new SqlParameter("@AddedBy", 1),
              tvpParamCityRoot,
              tvpParamTripDetails);
            }
            return Json("Trip Updated Sucessfull");
            
        }
        public ActionResult AddSubTrip(int? Source, int TripId = 0)
        {
            ViewData["TripWiseCityList"] = _tripBusinessLayer.GetDropDownData("TripWiseCityList", 0, TripId);
            ViewData["MaterialList"] = _tripBusinessLayer.GetDropDownData("MaterialList", 0);
            ViewData["UOMList"] = _tripBusinessLayer.GetDropDownData("UOMList", 0);
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
        public ActionResult GetVehicleSizeCapacityWhereID(int VehicleID)
        {

            var data = _tripBusinessLayer.GetVehicleSizeCapacityWhereID(VehicleID);
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("_VehiclSizeAndCapacity", data)
                    : View("_VehiclSizeAndCapacity", data);


        }
        public ActionResult GetAvailableSizeCapacityOfVehicle(int TripId, int SourceID, int DestinationID)
        {

            var data = _tripBusinessLayer.GetAvailableSizeCapacityOfVehicle(TripId, SourceID, DestinationID);
            return Request.IsAjaxRequest()
                    ? (ActionResult)PartialView("_VehiclSizeAndCapacity", data)
                    : View("_VehiclSizeAndCapacity", data);


        }
       
        
        [HttpPost]
        public ActionResult SaveSubTrip(int? sourceId, int? destinationId, int? TripId, List<TripDetail> _tripDetails)
        {

            DataTable dtTripDetail = new DataTable();
            dtTripDetail.Columns.Add("TripDetailID", typeof(int));
            dtTripDetail.Columns.Add("MaterialID", typeof(int));
            dtTripDetail.Columns.Add("UnitOfMeasure", typeof(string));
            dtTripDetail.Columns.Add("Height", typeof(decimal));
            dtTripDetail.Columns.Add("Width", typeof(decimal));
            dtTripDetail.Columns.Add("Length", typeof(decimal));
            dtTripDetail.Columns.Add("Weight", typeof(decimal));
            dtTripDetail.Columns.Add("Qty", typeof(int));
            dtTripDetail.Columns.Add("ConversionFactor", typeof(decimal));


            if (_tripDetails != null)
            {
                if (_tripDetails.Count > 0)
                {
                    foreach (var item in _tripDetails)
                    {
                        DataRow dr_LoadDetail = dtTripDetail.NewRow();
                        dr_LoadDetail["TripDetailID"] = item.TripDetailID;
                        dr_LoadDetail["MaterialID"] = item.MaterialID;
                        dr_LoadDetail["ConversionFactor"] = item.ConversionFactor;
                        dr_LoadDetail["UnitOfMeasure"] = item.UnitOfMeasure;
                        dr_LoadDetail["Height"] = item.Height;
                        dr_LoadDetail["Width"] = item.Width;
                        dr_LoadDetail["Length"] = item.Length;
                        dr_LoadDetail["Weight"] = item.Weight;
                        dr_LoadDetail["Qty"] = item.Qty;
                        dtTripDetail.Rows.Add(dr_LoadDetail);
                    }
                }
            }

            SqlParameter tvpParamTripDetails = new SqlParameter();
            tvpParamTripDetails.ParameterName = "@UDTable_TripDetails";
            tvpParamTripDetails.SqlDbType = System.Data.SqlDbType.Structured;
            tvpParamTripDetails.Value = dtTripDetail;
            tvpParamTripDetails.TypeName = "UDTable_TripDetails";

            using (JobDbContext _db = new JobDbContext())
            {
                    var result = _db.Database.ExecuteSqlCommand(@"exec USP_SaveSubtrip
                         @SourceID 
                        ,@DestinationID 
                        ,@TripID 
                        ,@UDTable_TripDetails",
                        new SqlParameter("@SourceID", sourceId == null ? (object)DBNull.Value : sourceId),
                        new SqlParameter("@DestinationID", destinationId == null ? (object)DBNull.Value : destinationId),
                        new SqlParameter("@TripID", TripId == null ? (object)DBNull.Value : TripId),
                    tvpParamTripDetails
                   );
            }
            return Json("Your SubTrip Booked Sucessfully");

        }

        #endregion

        #region "Load Methods"
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


        public ActionResult SearchLoads(int? page, string Source, string Destination)
        {
            Loader _loader = new Loader();
            _loader.Loaders = _tripBusinessLayer.GetLoaderList(page, Source, Destination);
            _loader.LoadDetails = _tripBusinessLayer.GetLoadeDetails();
            _loader.MaterialList = _tripBusinessLayer.GetMaterialList();

            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["SearchApplied"] = 0;

            // return View();
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("_LoadList", _loader) : View("_LoadList", _loader);
        }
        public ActionResult AddLoad()
        {
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            //MaterialList
            ViewData["MaterialList"] = _tripBusinessLayer.GetDropDownData("MaterialList", 0);
            ViewData["UOMList"] = _tripBusinessLayer.GetDropDownData("UOMList", 0);

            return View();
        }

        [HttpPost]
        public ActionResult AddLoad(Loader _loader, List<LoadDetail> _loadDetails)
        {

                //Adding Load Details In DT
                

                    DataTable dtLoadDetail = new DataTable();
                    dtLoadDetail.Columns.Add("LoadDetailID", typeof(int));
                    dtLoadDetail.Columns.Add("MaterialID", typeof(int));
                    dtLoadDetail.Columns.Add("UnitOfMeasure", typeof(string));
                    dtLoadDetail.Columns.Add("Height", typeof(decimal));
                    dtLoadDetail.Columns.Add("Width", typeof(decimal));
                    dtLoadDetail.Columns.Add("Length", typeof(decimal));
                    dtLoadDetail.Columns.Add("Weight", typeof(decimal));
                    dtLoadDetail.Columns.Add("Qty", typeof(int));
                    dtLoadDetail.Columns.Add("ConversionFactor", typeof(decimal));
            if (_loadDetails != null)
                    {
                        if (_loadDetails.Count > 0)
                        {
                            foreach (var item in _loadDetails)
                            {
                                DataRow dr_LoadDetail = dtLoadDetail.NewRow();
                                dr_LoadDetail["LoadDetailID"] = item.LoadDetailID;
                                dr_LoadDetail["MaterialID"] = item.MaterialID;
                                dr_LoadDetail["UnitOfMeasure"] = item.UnitOfMeasure;
                                dr_LoadDetail["Height"] = item.Height;
                                dr_LoadDetail["Width"] = item.Width;
                                dr_LoadDetail["Length"] = item.Length;
                                dr_LoadDetail["Weight"] = item.Weight;
                                dr_LoadDetail["Qty"] = item.Qty;
                                dr_LoadDetail["ConversionFactor"] = item.ConversionFactor;
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
            ViewData["UOMList"] = _tripBusinessLayer.GetDropDownData("UOMList", 0);

            LoaderEdit loaderEdit = new LoaderEdit();
            loaderEdit = _tripBusinessLayer.GetLoaderByID(LoadID);
            loaderEdit.LoadDetails = _tripBusinessLayer.GetLoadDetailsByID(LoadID);
            ViewData["LoadDetailCount"] = loaderEdit.LoadDetails.Count<LoadDetail>();
            return View(loaderEdit);

        }

        [HttpPost]
        public ActionResult EditLoad(Loader _loader, List<LoadDetail> _loadDetails=null)
        {

            //Adding Load Details In DT

            DataTable dtLoadDetail = new DataTable();

            dtLoadDetail.Columns.Add("LoadDetailID", typeof(int));
            dtLoadDetail.Columns.Add("MaterialID", typeof(int));
            dtLoadDetail.Columns.Add("UnitOfMeasure", typeof(string));
            dtLoadDetail.Columns.Add("Height", typeof(decimal));
            dtLoadDetail.Columns.Add("Width", typeof(decimal));
            dtLoadDetail.Columns.Add("Length", typeof(decimal));
            dtLoadDetail.Columns.Add("Weight", typeof(decimal));
            dtLoadDetail.Columns.Add("Qty", typeof(int));
            dtLoadDetail.Columns.Add("ConversionFactor", typeof(decimal));

            if (_loadDetails != null)
            {
                if (_loadDetails.Count > 0)
                {
                    foreach (var item in _loadDetails)
                    {
                        DataRow dr_LoadDetail = dtLoadDetail.NewRow();
                        dr_LoadDetail["LoadDetailID"] = item.LoadDetailID;
                        dr_LoadDetail["MaterialID"] = item.MaterialID;
                        dr_LoadDetail["UnitOfMeasure"] = item.UnitOfMeasure;
                        dr_LoadDetail["Height"] = item.Height;
                        dr_LoadDetail["Width"] = item.Width;
                        dr_LoadDetail["Length"] = item.Length;
                        dr_LoadDetail["Weight"] = item.Weight;
                        dr_LoadDetail["Qty"] = item.Qty;
                        dr_LoadDetail["ConversionFactor"] = item.ConversionFactor;
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
        #endregion



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


        public ActionResult Registration()
        {
            return PartialView("_Registration");
        }

        public ActionResult RegistrationNew()
        {
            return View();
        }


        public ActionResult Test()
        {
            return View();

        }

        [HttpPost]
        public ActionResult Test(User _user)
        {
            /*....ClientTypeId Logic here*/
            JobDbContext _jobDbContext = new JobDbContext();

            var result = _jobDbContext.Database.ExecuteSqlCommand(@"exec USP_RegisterClient
                                                                    @ClientTypeID ,
                                                                    @Email ,
                                                                    @Password ,
                                                                    @Mobile ",
                                                                    new SqlParameter("@ClientTypeID", _user.ClientTypeID),
                                                                    new SqlParameter("@Email", _user.Email == null ?(object) DBNull.Value: _user.Email),
                                                                    new SqlParameter("@Password", _user.Password),
                                                                    new SqlParameter("@Mobile", _user.Mobile));
          return Json("Registration Sucessfull");
          
         }

        public int GetUserIDWhereMobileNo(string MobileNo)
        {
            JobDbContext jobDbContext = new JobDbContext();
            var result = jobDbContext.Database.SqlQuery<int>(@"exec USP_SelectUserIDWhereMobileNo @Mobile", new SqlParameter("@Mobile", MobileNo)).SingleOrDefault<int>();
            return result;
        }
        public int GetUserIDWhereEmail(string Email)
        {
            JobDbContext jobDbContext = new JobDbContext();
            var result = jobDbContext.Database.SqlQuery<int>(@"exec USP_SelectUserIDWhereEmail @Email", new SqlParameter("@Email", Email)).SingleOrDefault<int>();
            return result;
        }


        public ActionResult TestLogin()
        {
            return View();

        }
        public int GetUserIDWhereUserAndPassword(string UserName, string Password)
        {                                        
            JobDbContext jobDbContext = new JobDbContext();
            var result = jobDbContext.Database.SqlQuery<int>(@"exec USP_SelectUserIDDWhereUserAndPassword @UserName, @Password", new SqlParameter("@UserName", UserName), new SqlParameter("@Password", Password)).SingleOrDefault<int>();
            return result;
        }

        public decimal GetDistanceForCities(int FromCity, int ToCity)
        {
            using (JobDbContext jobDbContext = new JobDbContext())
            {
                return jobDbContext.Database.SqlQuery<decimal>("USP_GetDistanceForCities @FromCity ,@ToCity", new SqlParameter("@FromCity", FromCity), new SqlParameter("@ToCity", ToCity)).SingleOrDefault<decimal>();

            }

        }
        
        public decimal GetFullTruckQuotation(decimal Weight, decimal Distance,string strLoadType)
        {
            decimal TotalPrice = 0;
            if (strLoadType == constLoadType.FullTruckLoad)
            {
                using (JobDbContext jobDbContext = new JobDbContext())
                {
                    TotalPrice = jobDbContext.Database.SqlQuery<decimal>("USP_GetFullTruckQuotation @Weight ,@Distance", new SqlParameter("@Weight", Weight), new SqlParameter("@Distance", Distance)).SingleOrDefault<decimal>();
                }
            }
            return TotalPrice;
        }
    }
}