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
using TransportService.Web.Models.Transaction;
using System.Web.Helpers;
using System.IO;
using System.Net.Mail;
using System.Net;
using TransportService.Web.Models.Other;

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

            if (Session[UserColumns.UserID] != null)
            {
                ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
                ViewData["VehicleList"] = _tripBusinessLayer.GetDropDownData("VehicleList", Convert.ToInt32(Session[ClientColumns.ClientID])); //pass @val= ownerID i.e. Login ClientID
                ViewData["MaterialList"] = _tripBusinessLayer.GetDropDownData("MaterialList", 0);
                ViewData["UOMList"] = _tripBusinessLayer.GetDropDownData("UOMList", 0);
                return View();
            }
            else
            {
                return RedirectToAction("LoginUser");
            }

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
                if (CityRootId.Count > 1)//null is also considered as single item
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
            dtTripDetail.Columns.Add("MaterialValue", typeof(decimal));


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
                        dr_LoadDetail["MaterialValue"] = item.MaterialValue;
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
                 ,@LoadID
                 ,@RouteCityIDs
                 ,@UDTable_TripDetails",
          new SqlParameter("@SourceID", T.SourceID == null ? (object)DBNull.Value : T.SourceID),
          new SqlParameter("@DestinationID", T.DestinationID == null ? (object)DBNull.Value : T.DestinationID),
          new SqlParameter("@VehicleID", T.VehicleID == null ? (object)DBNull.Value : T.VehicleID),
          new SqlParameter("@TripStartDate", T.TripStartDate == null ? (object)DBNull.Value : T.TripStartDate),
          new SqlParameter("@TripEndDate", T.TripEndDate == null ? (object)DBNull.Value : T.TripEndDate),
          new SqlParameter("@Status", T.TripStatus),
          new SqlParameter("@AddedBy", Session[UserColumns.UserID]),
          new SqlParameter("@LoadID", T.LoadID==null ?(object) DBNull.Value : T.LoadID),
          tvpParamCityRoot,
          tvpParamTripDetails);
            return Json("Trip Posted Sucessfull");


        }
        public ActionResult EditTrip(int TripID)
        {
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["VehicleList"] = _tripBusinessLayer.GetDropDownData("VehicleList",Convert.ToInt32( Session[ClientColumns.ClientID])); //pass @val= ownerID i.e. Login ClientID
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
            dtTripDetail.Columns.Add("MaterialValue", typeof(decimal));


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
                        dr_LoadDetail["MaterialValue"] = item.MaterialValue;
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
              new SqlParameter("@AddedBy", Session[UserColumns.UserID]),
              tvpParamCityRoot,
              tvpParamTripDetails);
            }
            return Json("Trip Updated Sucessfull");
            
        }
  
        public ActionResult AddSubTrip(int? Source, int TripId = 0)
        {
            if (Session[UserColumns.UserID] != null)
            {
                ViewData["TripWiseCityList"] = _tripBusinessLayer.GetDropDownData("TripWiseCityList", 0, TripId);
                ViewData["MaterialList"] = _tripBusinessLayer.GetDropDownData("MaterialList", 0);
                ViewData["UOMList"] = _tripBusinessLayer.GetDropDownData("UOMList", 0);
                ViewBag.tripid = TripId;
                ViewBag.Source = Source;
                return View();
            }
            else
            {
                return RedirectToAction("LoginUser");
            }
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
            dtTripDetail.Columns.Add("MaterialValue", typeof(decimal));


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
                        dr_LoadDetail["MaterialValue"] = item.MaterialValue;
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
        public ActionResult LoaderIndex(int? page)
        {
            Loader _loader = new Loader();
            _loader.Loaders = _tripBusinessLayer.GetLoaderList(page, "", "");
            _loader.LoadDetails = _tripBusinessLayer.GetLoadeDetails();
            _loader.MaterialList = _tripBusinessLayer.GetMaterialList();

            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["SearchApplied"] = 0;

            // return View();
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("LoaderIndex", _loader) : View("LoaderIndex", _loader);
        }
        public ActionResult SearchLoads(int? page, string Source, string Destination)
        {
            Loader _loader = new Loader();
            _loader.Loaders = _tripBusinessLayer.GetLoaderList(page, Source, Destination);
            _loader.LoadDetails = _tripBusinessLayer.GetLoadeDetails();
            _loader.MaterialList = _tripBusinessLayer.GetMaterialList();

            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["SearchApplied"] = 0;

            //return View();
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("_LoadList", _loader) : View("_LoadList", _loader);
        }
        public ActionResult AddLoad()
        {
            if (Session[UserColumns.UserID] != null)
            {
                ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
                //MaterialList
                ViewData["MaterialList"] = _tripBusinessLayer.GetDropDownData("MaterialList", 0);
                ViewData["UOMList"] = _tripBusinessLayer.GetDropDownData("UOMList", 0);
                ViewData["VehicalTypeList"] = _tripBusinessLayer.GetDropDownData("VehicalTypeList");
                return View();

            }
            else
            {
                return RedirectToAction("LoginUser");
            }

        }

        [HttpPost]
        public ActionResult AddLoad(Loader _loader, List<LoadDetail> _loadDetails, Quoatation quoatation)
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
                    dtLoadDetail.Columns.Add("MaterialValue", typeof(decimal));
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
                                dr_LoadDetail["MaterialValue"] = item.MaterialValue;
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
                     
                        @AddedBy ,
                        @Receiver,
                        @VehicleTypeID,
                        @Quotation,
                        @UDTable_LoadDetails",
                  new SqlParameter("@SourceID", _loader.SourceID),
                  new SqlParameter("@DestinationID", _loader.DestinationID),
                  new SqlParameter("@PickUpDate", _loader.PickUpDate),
                  new SqlParameter("@LoadType", _loader.LoadType),
                  new SqlParameter("@SubService", _loader.SubService),
                  new SqlParameter("@ContactNo", _loader.ContactNo),
                  new SqlParameter("@Email", _loader.Email),
                  new SqlParameter("@Address", _loader.Address),
                  new SqlParameter("@AddedBy", Session[UserColumns.UserID]),
                  new SqlParameter("@Receiver", _loader.Receiver),
                  new SqlParameter("@VehicleTypeID", _loader.VehicleTypeID),
                  new SqlParameter("@Quotation", quoatation.PrimaryQuotaionValue),
                  tvpParamLoadDetails);
                    return Json("Load Added Sucessfully");
                
            
        }

      
        public ActionResult EditLoad(int LoadID)
        {
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData["MaterialList"] = _tripBusinessLayer.GetDropDownData("MaterialList", 0);
            ViewData["UOMList"] = _tripBusinessLayer.GetDropDownData("UOMList", 0);
            ViewData["VehicalTypeList"] = _tripBusinessLayer.GetDropDownData("VehicalTypeList");

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
            dtLoadDetail.Columns.Add("MaterialValue", typeof(decimal));

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
                        dr_LoadDetail["MaterialValue"] = item.MaterialValue;
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
                        @VehicleTypeID,
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
          new SqlParameter("@Status", _loader.Status),
          new SqlParameter("@AddedBy", Session[UserColumns.UserID]),/*.....UserID 1 is Loader*/
          new SqlParameter("@Receiver", _loader.Receiver),
          new SqlParameter("@VehicleTypeID", _loader.VehicleTypeID == null ? (object)DBNull.Value : _loader.VehicleTypeID),
          tvpParamLoadDetails);
            return Json("Load Updated Sucessfully");

        }

        public ActionResult BookLoad(int LoadID,string LoadType,int VehicleTypeID)
        {
            if (Session[UserColumns.UserID] != null)
            {
                ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
                if (LoadType == constLoadType.FullTruckLoad)
                {
                        ViewData["VehicleList"] = _tripBusinessLayer.GetVehicleWhereVehicleType(Convert.ToInt32(Session[ClientColumns.ClientID]), VehicleTypeID); //pass @val= ownerID i.e. Login ClientID
                }
                else
                {
                    ViewData["VehicleList"] = _tripBusinessLayer.GetDropDownData("VehicleList", Convert.ToInt32(Session[ClientColumns.ClientID])); //pass @val= ownerID i.e. Login ClientID
                }
                LoaderEdit loaderEdit = new LoaderEdit();
                loaderEdit = _tripBusinessLayer.GetLoaderByID(LoadID);
                loaderEdit.LoadDetails = _tripBusinessLayer.GetLoadDetailsByID(LoadID);
                return View(loaderEdit);
                
            }
            else
            {
                return RedirectToAction("LoginUser");
            }

        }

        public int GetFullTruckQuotation(int VehicleTypeID, decimal Distance )
        {
            decimal TotalPrice = 0;
            using (JobDbContext jobDbContext = new JobDbContext())
            {
                    TotalPrice = jobDbContext.Database.SqlQuery<decimal>("USP_GetFullTruckQuotation @VehicleTypeID ,@Distance", new SqlParameter("@VehicleTypeID", VehicleTypeID), new SqlParameter("@Distance", Distance)).SingleOrDefault<decimal>();
            }
            return (int)TotalPrice;
        }
        public decimal GetDistanceForCities(int FromCity, int ToCity)
        {
            using (JobDbContext jobDbContext = new JobDbContext())
            {
                return jobDbContext.Database.SqlQuery<decimal>("USP_GetDistanceForCities @FromCity ,@ToCity", new SqlParameter("@FromCity", FromCity), new SqlParameter("@ToCity", ToCity)).SingleOrDefault<decimal>();

            }

        }


        #endregion

        #region "Masters"

        public ActionResult AddTruck(int isTripCall )
        {
            if (Convert.ToInt32(Session[RoleColumns.RoleID]) == enumRole.Transporter.GetHashCode())
            {
                ViewData["TruckCapacityList"] = _tripBusinessLayer.GetDropDownData("TruckCapacityList");
                ViewData[DDLListNames.VehicalTypeList] = _tripBusinessLayer.GetDropDownData(DDLListNames.VehicalTypeList);
                ViewData[DDLListNames.ProofTypeList] = _tripBusinessLayer.GetDropDownData(DDLListNames.ProofTypeList);
                ViewBag.IsTripCall = isTripCall;
                return View();
            }
            else
            {
                return RedirectToAction("LoginUser");
            }
            
        }
        [HttpPost]
        public ActionResult AddTruck1(Vehicle _vehicle, HttpPostedFileBase postedFile)
        {

            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }
            using (JobDbContext jobDbContext = new JobDbContext())
            {
                var result = jobDbContext.Database.ExecuteSqlCommand(@"exec USP_AddVehicle @VehicleTypeID ,
                                                                                        @VehicleNo ,
                                                                                        @Description ,
                                                                                        @InsuredBy  ,
                                                                                        @InsuranceStartDate ,
                                                                                        @InsuranceExpDate ,
                                                                                        @OwnerID ,
                                                                                        @GPSStatus ,
                                                                                        @Phone ,
                                                                                        @ContactName,
                                                                                       @ProofTypeID,
                                                                                       @DocumentName,
                                                                                       @ContentType,
                                                                                       @Data ",
                                                                                       new SqlParameter("@VehicleTypeID", _vehicle.VehicleTypeID),
                                                                                       new SqlParameter("@VehicleNo", _vehicle.VehicleNo),
                                                                                       new SqlParameter("@Description", _vehicle.Description == null ? (object)DBNull.Value : _vehicle.Description),
                                                                                       new SqlParameter("@InsuredBy", _vehicle.InsuredBy == null ? (object)DBNull.Value : _vehicle.InsuredBy),
                                                                                       new SqlParameter("@InsuranceStartDate", _vehicle.InsuranceStartDate == null ? (object)DBNull.Value : _vehicle.InsuranceStartDate),
                                                                                       new SqlParameter("@InsuranceExpDate", _vehicle.InsuranceExpDate == null ? (object)DBNull.Value : _vehicle.InsuranceExpDate),
                                                                                       new SqlParameter("@OwnerID", Session[ClientColumns.ClientID]),
                                                                                       new SqlParameter("@GPSStatus", _vehicle.GPSStatus == null ? (object)DBNull.Value : _vehicle.GPSStatus),
                                                                                       new SqlParameter("@Phone", _vehicle.Phone == null ? (object)DBNull.Value : _vehicle.Phone),
                                                                                       new SqlParameter("@ContactName", _vehicle.ContactName ?? (object)DBNull.Value),
                                                                                        new SqlParameter("@ProofTypeID", _vehicle.ProofTypeID ?? (object)DBNull.Value),
                                                                                        new SqlParameter("@DocumentName", Path.GetFileName(postedFile.FileName)),
                                                                                        new SqlParameter("@ContentType", postedFile.ContentType),
                                                                                        new SqlParameter("@Data", bytes));

                return Json("Truck Added Sucessfully");
            }
        }

        public ActionResult GetVehicleTypeByID(int VehicleTypeID)
        {
            var vehicleType = new JobDbContext().Database.SqlQuery<VehicleType>("exec USP_GetSizeAndCapacityByVehicleTypeID @VehicleTypeID", new SqlParameter("@VehicleTypeID", VehicleTypeID)).SingleOrDefault<VehicleType>();
            return Request.IsAjaxRequest() ? (ActionResult)PartialView("_VehiclTypeDimension", vehicleType) : View("_VehiclTypeDimension", vehicleType);
        }

        #endregion

        #region "LoginRegister"

        public ActionResult TRegisterUser(int IsCompany)
        {
            ViewBag.IsCompany = IsCompany;
            return View();
        }

        [HttpPost]
        public ActionResult TRegisterUser(User _user)
        {

            #region "Generate Activation Code"
            _user.ActivationCode = Guid.NewGuid();
            #endregion

            #region "Generate OTP"
            using (JobDbContext jobDbContext= new JobDbContext())
            {

                var OTP = jobDbContext.Database.SqlQuery<int>("USP_GenerateOTP").SingleOrDefault<int>();
                _user.OTP = OTP;
            }

           
            #endregion

            #region  "Password Hashing "
            _user.PasswordHash = Crypto.Hash(_user.Password);
            //_clientRegister.ConfirmPassword = Crypto.Hash(_clientRegister.ConfirmPassword); //
            #endregion


            using (JobDbContext _jobDbContext = new JobDbContext())
            {
                var result = _jobDbContext.Database.ExecuteSqlCommand(@"exec USP_RegisterClient
                                                                        @ClientTypeID  ,
                                                                        @Email ,
                                                                        @Password ,
                                                                        @PasswordHash ,
                                                                        @Mobile  ,
                                                                        @ActivationCode ,
                                                                        @OTP ",
                                                                    new SqlParameter("@ClientTypeID", _user.ClientTypeID),
                                                                    new SqlParameter("@Email", _user.Email == null ? (object)DBNull.Value : _user.Email),
                                                                    new SqlParameter("@Password", _user.Password),
                                                                    new SqlParameter("@Mobile", _user.Mobile),
                                                                    new SqlParameter("@PasswordHash", _user.PasswordHash),
                                                                    new SqlParameter("@ActivationCode", _user.ActivationCode),
                                                                    new SqlParameter("@OTP", _user.OTP));
                                                                    
                if (result > 0)
                {
                    return Json("Registration Sucessfull");
                }
                else
                {
                    return Json("Registration Failed");
                }
            }
                      
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


        public ActionResult LoginUser()
        {
            return View();

        }
        [HttpPost]
        public ActionResult LoginUser(LoginUser L,string ReturnUrl)
        {
            using (JobDbContext jobDbContext = new JobDbContext())
            {
                var result = jobDbContext.Database.SqlQuery<LoginDetail>(@"exec usp_login @UserName,@Password",
                new SqlParameter("@UserName", L.UserName == null ? (object)DBNull.Value : L.UserName),
                new SqlParameter("@Password", L.Password == null ? (object)DBNull.Value : L.Password)).SingleOrDefault<LoginDetail>();
                
                if (result == null)
                {
                    ViewBag.Error = "Please Enter Valid User Name Password";
                }
                else
                {
                    
                    Session[UserColumns.Mobile] = result.Mobile;
                    Session[UserColumns.Email] = result.Email;
                    Session[UserColumns.Password] = result.Password;
                    Session[UserColumns.UserID] = result.UserID;
                    Session[UserColumns.ClientID] = result.ClientID;
                    Session[UserColumns.ClientTypeID] = result.ClientTypeID;
                    Session[RoleColumns.RoleID] = result.RoleID;
                    Session[UserColumns.FirstName] = result.FirstName;

                    if (result.RoleID == enumRole.Transporter.GetHashCode())
                    {
                        return RedirectToAction("LoaderIndex");
                        
                    }
                    else
                    {
                        return RedirectToAction("Index");

                    }

                    
                }
            }
            

            return View();

        }

        public ActionResult LogOut()
        {
            Session[UserColumns.Mobile] = null;
            Session[UserColumns.Email] = null;
            Session[UserColumns.Password] = null;
            Session[UserColumns.UserID] = null;
            Session[UserColumns.ClientID] = null;
            Session[UserColumns.ClientTypeID] = null;
            Session[RoleColumns.RoleID] = null;
            Session[UserColumns.FirstName] = null;
            return RedirectToAction("Index");
        }
        public ActionResult ContactPerson()
        {
            return View();
        }
        public ActionResult LoadPersonalDetail()
        {
            ViewData["CityList"] = _tripBusinessLayer.GetDropDownData("CityList", 0);
            ViewData[DDLListNames.CompanyTypeList] = _tripBusinessLayer.GetDropDownData(DDLListNames.CompanyTypeList, 0);

            

             

            return View();
        }

        public ActionResult PersonalDetail()
        {

            ViewData[DDLListNames.CityList] = _tripBusinessLayer.GetDropDownData(DDLListNames.CityList, 0);
            ViewData[DDLListNames.CompanyTypeList] = _tripBusinessLayer.GetDropDownData(DDLListNames.CompanyTypeList, 0);
            if (Session[UserColumns.FirstName] != null || Convert.ToString(Session[UserColumns.FirstName]) != "")
            {



                using (JobDbContext jobDbContext = new JobDbContext())
                {
                    PersonDetail personDetail = new PersonDetail();


                    personDetail.user = jobDbContext.Database.SqlQuery<User>("exec USP_SelectAllFromUserWhereID @UserID",
                                                                                        new SqlParameter("@UserID", Session[UserColumns.UserID])).FirstOrDefault<User>();
                    
                    personDetail.company = jobDbContext.Database.SqlQuery<Company>("exec USP_SelectAllFromCompanyWhereClientID @ClientID",
                                                                                            new SqlParameter("@ClientID", Session[ClientColumns.ClientID])).FirstOrDefault<Company>();
                    
                    //personDetail.userDocuments = jobDbContext.Database.SqlQuery<UserDocuments>("exec USP_SelectAllFromUserDocumentsWhereUserID @UserID",
                    //                                                                        new SqlParameter("@UserID", Session[ClientColumns.ClientID])).FirstOrDefault<UserDocuments>();

                    return View("LoadPersonalDetail",personDetail);
                }



                
            }
            else
            {
                
                return View();
            }


            
        }



        [HttpPost]
        public ActionResult PersonalDetail(User user,Client client,Company company,HttpPostedFileBase postedFile)
        {
            byte[] bytes;
            using (BinaryReader br = new BinaryReader(postedFile.InputStream))
            {
                bytes = br.ReadBytes(postedFile.ContentLength);
            }

            using (JobDbContext jobDbContext = new JobDbContext())
            {
                var result = jobDbContext.Database.ExecuteSqlCommand(ExecuteUserProcedure.USP_UpdateClientAndUser,
                                                                        new SqlParameter("@" + UserColumns.ClientID, client.ClientID),
                                                                        new SqlParameter("@" + UserColumns.UserID, user.UserID),
                                                                        new SqlParameter("@" + UserColumns.FirstName, user.FirstName),
                                                                        new SqlParameter("@" + UserColumns.LastName, user.LastName == null ? (object)DBNull.Value : user.LastName),
                                                                        new SqlParameter("@" + UserColumns.Password, user.Password),
                                                                        new SqlParameter("@" + UserColumns.Email, user.Email == null ? (object)DBNull.Value : user.Email),
                                                                        new SqlParameter("@" + UserColumns.Address1, user.Address1 == null ?(object) DBNull.Value : user.Address1),
                                                                        new SqlParameter("@" + UserColumns.Address2, user.Address2 == null ?(object) DBNull.Value : user.Address2),
                                                                        new SqlParameter("@" + UserColumns.AlternateContactPerson, user.AlternateContactPerson == null ? (object)DBNull.Value : user.AlternateContactPerson),
                                                                        new SqlParameter("@" + UserColumns.AlternateMobileNo, user.AlternateMobileNo == null ? (object)DBNull.Value : user.AlternateMobileNo),
                                                                        new SqlParameter("@" + ClientColumns.GSTNumber, client.GSTNumber == null ? (object)DBNull.Value : client.GSTNumber),
                                                                        new SqlParameter("@" + ClientColumns.DocumentPath, client.DocumentPath == null ? (object)DBNull.Value : client.DocumentPath),
                                                                        new SqlParameter("@" + UserColumns.AdharCardNo, user.AdharCardNo == null ? (object)DBNull.Value : user.AdharCardNo),
                                                                        new SqlParameter("@" + UserColumns.PanCardNo, user.PanCardNo == null ? (object)DBNull.Value : user.AdharCardNo),
                                                                        new SqlParameter("@" + UserColumns.PinCode, user.PinCode == null ? (object)DBNull.Value : user.PinCode),
                                                                        new SqlParameter("@" + UserColumns.STD, user.STD == null ? (object)DBNull.Value : user.STD),
                                                                        new SqlParameter("@" + UserColumns.LandlineNo, user.LandlineNo == null ? (object)DBNull.Value : user.LandlineNo),
                                                                        new SqlParameter("@" + UserColumns.VehicleID, user.VehicleID == null ? (object)DBNull.Value : user.VehicleID),
                                                                        new SqlParameter("@" + CompanyColumns.CompanyName, company.CompanyName == null ? (object)DBNull.Value : company.CompanyName),
                                                                        new SqlParameter("@" + CompanyColumns.CompanyTypeID, company.CompanyTypeID == null ? (object)DBNull.Value : company.CompanyTypeID),
                                                                        new SqlParameter("@" + CompanyColumns.ServiceTaxNo, company.ServiceTaxNo == null ? (object)DBNull.Value : company.ServiceTaxNo),
                                                                        new SqlParameter("@" + CompanyColumns.CompanyPanNo, company.CompanyPanNo == null ? (object)DBNull.Value : company.CompanyPanNo),
                                                                        new SqlParameter("@" + CompanyColumns.CompanyWebsite, company.CompanyWebsite == null ? (object)DBNull.Value : company.CompanyWebsite),
                                                                        new SqlParameter("@DocumentName", Path.GetFileName(postedFile.FileName)),
                                                                        new SqlParameter("@ContentType", postedFile.ContentType),
                                                                        new SqlParameter("@Data", bytes));

                if (result >0)
                {
                    return Json("Details saved successfully");
                }
                else
                {
                    return Json("Error in DBInsertion");
                }

            }

            
        }


        [HttpGet]
        public ActionResult ForgotPassword()
        {           
            return View();
        }
        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";
            //bool status = false;

            using (JobDbContext jobDbContext = new JobDbContext())
            {
                //var account = dc.Users.Where(a => a.EmailID == EmailID).FirstOrDefault();
                var account = jobDbContext.Database.SqlQuery<int>(@"exec USP_SelectUserIDWhereEmail @Email", new SqlParameter("@Email", EmailID)).SingleOrDefault<int>();
                if (account != 0)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(EmailID, resetCode, "ResetPassword");
                    var result = jobDbContext.Database.ExecuteSqlCommand(@"USP_UpdateResetCodeWhereEmail @UserID,@ResetPasswordCode", new SqlParameter("@UserID", account), new SqlParameter("@ResetPasswordCode", resetCode));
                    
                    message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page

            //if(string.IsNullOrWhiteSpace(id))
            //{
            //    return HttpNotFound();
            //}

            using (JobDbContext jobDbContext = new JobDbContext())
            {
                //var user = dc.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                var user = jobDbContext.Database.SqlQuery<User>(@"exec USP_SelectUserWhereResetPasswordCode @ResetPasswordCode", new SqlParameter("@ResetPasswordCode", id)).FirstOrDefault<User>();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (JobDbContext jobDbContext = new JobDbContext())
                {
                    //var user = dc.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    //if (user != null)
                    //{
                    //user.Password = Crypto.Hash(model.NewPassword);
                    //user.ResetPasswordCode = "";
                    //dc.Configuration.ValidateOnSaveEnabled = false;
                    //dc.SaveChanges();
                    //message = "New password updated successfully";
                    //}
                    string PasswordHash = Crypto.Hash(model.NewPassword);
                    var result = jobDbContext.Database.ExecuteSqlCommand(@"exec USP_ChangePassword 
                                                                                @ResetPasswordCode,
                                                                                @Password,
                                                                                @PasswordHash",
                                                                                new SqlParameter("@ResetPasswordCode", model.ResetCode),
                                                                                new SqlParameter("@Password", model.NewPassword),
                                                                                new SqlParameter("@PasswordHash", PasswordHash)
                                                                            );



                    return Json("Password change succesfully");

                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }






        public int GetUserIDWhereUserAndPassword(string UserName, string Password)
        {                                        
            JobDbContext jobDbContext = new JobDbContext();
            var result = jobDbContext.Database.SqlQuery<int>(@"exec USP_SelectUserIDDWhereUserAndPassword @UserName, @Password", new SqlParameter("@UserName", UserName), new SqlParameter("@Password", Password)).SingleOrDefault<int>();
            return result;
        }


        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/TLoader/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("bhadaaindia@gmail.com", "Bhadaa.com Team");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "harishmn"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    "successfully created. Please click on the below link to verify your account" +
                    "<br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //UseDefaultCredentials = false,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }

        #endregion

        #region "Other"
        public ActionResult RankTheTrip(int TripID, int TransporteUserID)
        {
            
            ViewData["TripID"] = TripID;
            ViewData["TransporteUserID"] = TransporteUserID;
            using (JobDbContext jobDbContext = new JobDbContext())
                {
                List<RankingCriteria> rankingCriterias = jobDbContext.DBRankingCriterias.SqlQuery("USP_SelectAllFromRankingCriteria").ToList<RankingCriteria>();
                return View(rankingCriterias);
                }
            

        }

        public ActionResult Rating()
        {
            using (JobDbContext jobDbContext = new JobDbContext())
            {
                List<RankingCriteria> rankingCriterias = jobDbContext.DBRankingCriterias.SqlQuery("USP_SelectAllFromRankingCriteria").ToList<RankingCriteria>();
                return View(rankingCriterias);
            }
           
        }






        [HttpPost]
        public ActionResult RankTheTrip(int TripID,int TransporteUserID,List<TripRankingDetail> tripRankingDetails)
        {
            DataTable dtTripRankingDetails = new DataTable();
            dtTripRankingDetails.Columns.Add("SerialNo", typeof(int));
            dtTripRankingDetails.Columns.Add("CriteriaQuestionID", typeof(int));
            dtTripRankingDetails.Columns.Add("Answer", typeof(string));
            dtTripRankingDetails.Columns.Add("AnswerValue", typeof(decimal));


            if (tripRankingDetails != null)
            {
                if (tripRankingDetails.Count > 0)
                {
                    foreach (var item in tripRankingDetails)
                    {
                        DataRow dr_RankingDetails = dtTripRankingDetails.NewRow();
                        dr_RankingDetails["SerialNo"] = item.SerialNo;
                        dr_RankingDetails["CriteriaQuestionID"] = item.CriteriaQuestionID;
                        dr_RankingDetails["Answer"] = item.Answer;
                        dr_RankingDetails["AnswerValue"] = item.AnswerValue;
                        dtTripRankingDetails.Rows.Add(dr_RankingDetails);
                    }
                }
            }
            SqlParameter tvpParamRankingDetails = new SqlParameter();
            tvpParamRankingDetails.ParameterName = "@UDTable_TripRankingDetail";
            tvpParamRankingDetails.SqlDbType = System.Data.SqlDbType.Structured;
            tvpParamRankingDetails.Value = dtTripRankingDetails;
            tvpParamRankingDetails.TypeName = "UDTable_TripRankingDetail";


            using (JobDbContext jobDbContext = new JobDbContext())
            {
                var result = jobDbContext.Database.ExecuteSqlCommand(@"exec USP_InsertRankingWhereTripID
                                                                            @TripID ,
                                                                            @SubTripID ,
                                                                            @LoaderID ,
                                                                            @LoaderUserID  ,
                                                                            @TransporterUserID  ,
                                                                            @UDTable_TripRankingDetail ",
                                                                            new SqlParameter("@TripID",TripID),
                                                                            new SqlParameter("@SubTripID",DBNull.Value),
                                                                            new SqlParameter("@LoaderID",Session[ClientColumns.ClientID]),
                                                                            new SqlParameter("@LoaderUserID", Session[UserColumns.UserID]),
                                                                            new SqlParameter("@TransporterUserID", TransporteUserID),
                                                                            tvpParamRankingDetails);

                return Json("Your Rating is done Succesfully");
            }

        }


        public ActionResult VerifyAdhaar(string AadharNo)
        {
            bool isValidnumber = aadharcard.validateVerhoeff(AadharNo);
            if (isValidnumber)
            {
                return Json("Aadhaar Validation Success");
            }
            else
            {

                return Json("Invalid Aadhaar Number");
            }

        }
        #endregion


        #region "HttpErrors"
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;

            return View();
        }
        #endregion
    }
}