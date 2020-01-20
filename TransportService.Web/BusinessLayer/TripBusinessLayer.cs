using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using TransportService.Web.Models;
using System.Data.SqlClient;
using System.Web.Mvc;
using TransportService.Web.Models.Activity;
using System.Data;
using TransportService.Web.Models.Masters;
namespace TransportService.Web.BusinessLayer
{
    public class TripBusinessLayer
    {
        #region "fields"
        JobDbContext _jobDbContext;
        #endregion

        #region "Constructors"
        public TripBusinessLayer()
        {
            _jobDbContext = new JobDbContext();
        }

        #endregion

        #region "Trip Methods"

        public StaticPagedList<Transpoter> GetTripDetails(int? page, string Source = "", string Destination = "")
        {
            
            var pageIndex = (page ?? 1);
            const int pageSize = 8;
            int totalcount = 8;
            Transpoter Llist = new Transpoter();
            IEnumerable<Transpoter> result = _jobDbContext.DBTransporter.SqlQuery(@"exec USP_GetTripDetails @pPageIndex, @pPageSize,@Source,@Destination",
                                                                        new SqlParameter("@pPageIndex", pageIndex),
                                                                        new SqlParameter("@pPageSize", pageSize),
                                                                        new SqlParameter("@Source", Source == null ? (object)DBNull.Value : Source),
                                                                        new SqlParameter("@Destination", Destination == null ? (object)DBNull.Value : Destination)).ToList<Transpoter>();
            totalcount = 0;
            if (result.Count() > 0)
            {
                totalcount = Convert.ToInt32(result.FirstOrDefault().TotalRows);

            }

            var itemAsIPagedList = new StaticPagedList<Transpoter>(result, pageIndex, pageSize, totalcount);
            return itemAsIPagedList;
        }

        public IEnumerable<subtripDetails> GetSubTripDetails()
        {
            return _jobDbContext.DBsubtripDetails.SqlQuery(@"exec GetSubTripDetails").ToList<subtripDetails>();
        }

        public IEnumerable<RouteDetails> GetTripRouteDetails()
        {
            return _jobDbContext.DBRouteDetails.SqlQuery(@"exec GetTripRouteDetails").ToList<RouteDetails>();
        }

        public TranspoterEdit GetTripByID(int id)
        {
            return _jobDbContext.DBTranspoterEdit.SqlQuery(@"exec USP_GetTripByID @TripID", new SqlParameter("@TripID", id)).FirstOrDefault<TranspoterEdit>();
        }

        public IEnumerable<TripDetail> GetTripDetailsByID(int id)
        {
            return _jobDbContext.DBTripDetail.SqlQuery(@"exec USP_GetTripDetailsByID @TripID", new SqlParameter("@TripID", id)).ToList<TripDetail>();
        }
        public IEnumerable<CityArray> GetRouteWhereTripID (int TripId)
        {

            return _jobDbContext.DBCityArray.SqlQuery("exec [USP_SelectRouteWhereTripID] @TripId",
                   new SqlParameter("@TripId", TripId)).ToList<CityArray>();

                   
        }
        #endregion

        #region "Loader Methods"
        public StaticPagedList<Loader> GetLoaderList(int? page, string Source = "", string Destination = "")
        {


            var pageIndex = (page ?? 1);
            const int pageSize = 4;
            int totalcount = 4;
            
            IEnumerable<Loader> result = _jobDbContext.DBLoader.SqlQuery(@"exec USP_GetLoaderList @pPageIndex, @pPageSize,@Source,@Destination",
                                                                        new SqlParameter("@pPageIndex", pageIndex),
                                                                        new SqlParameter("@pPageSize", pageSize),
                                                                        new SqlParameter("@Source", Source == null ? (object)DBNull.Value : Source),
                                                                        new SqlParameter("@Destination", Destination == null ? (object)DBNull.Value : Destination)).ToList<Loader>();
            totalcount = 0;
            if (result.Count() > 0)
            {
                totalcount = Convert.ToInt32(result.FirstOrDefault().TotalRows);

            }

            var itemAsIPagedList = new StaticPagedList<Loader>(result, pageIndex, pageSize, totalcount);
            return itemAsIPagedList;
        }
        public IEnumerable<LoadDetail> GetLoadeDetails()
        {
            return _jobDbContext.DBLoadDetails.SqlQuery(@"exec GetLoadDetails").ToList<LoadDetail>();
        }
        public IEnumerable<MaterialList> GetMaterialList()
        {
            return _jobDbContext.DBMaterialList.SqlQuery(@"exec USP_GetMaterialListByLoadID").ToList<MaterialList>();
        }
        public LoaderEdit GetLoaderByID(int id)
        {
            return _jobDbContext.DBLoaderEdit.SqlQuery(@"exec USP_GetLoaderByID @LoaderID", new SqlParameter("@LoaderID", id)).FirstOrDefault<LoaderEdit>();
        }
        public IEnumerable<LoadDetail> GetLoadDetailsByID(int id)
        {
            return _jobDbContext.DBLoadDetails.SqlQuery(@"exec USP_GetLoadDetailsByID @LoaderID", new SqlParameter("@LoaderID", id)).ToList<LoadDetail>();
        }
        #endregion

        #region "Common methods"
        public List<SelectListItem> GetDropDownData(string action, int val = 0, int TripId = 0)
        {
            return _jobDbContext.Database.SqlQuery<SelectListItem>("exec USP_BindDropDown @action , @val, @TripId",
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
        }

        public AvalableSpace GetVehicleSizeCapacityWhereID(int id)
        {
            return _jobDbContext.DBAvalableSpace.SqlQuery(@"exec USP_GetSizeAndCapacityFromVehicleWhereID @VehicleID", new SqlParameter("@VehicleID", id)).FirstOrDefault<AvalableSpace>();
        }
        public AvalableSpace GetAvailableSizeCapacityOfVehicle(int TripId,int SourceID,int DestinationID)
        {
            return _jobDbContext.DBAvalableSpace.SqlQuery(@"exec USP_GetAvailableSpace @TripId,@SourceId,@Destination",
                new SqlParameter("@TripId", TripId),
                new SqlParameter("@SourceId", SourceID),
                new SqlParameter("@Destination", DestinationID)).FirstOrDefault<AvalableSpace>();
        }
        #endregion


    }
}