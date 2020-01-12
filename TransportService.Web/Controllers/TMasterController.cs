using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportService.Web.BusinessLayer;
using TransportService.Web.Models.Masters;

namespace TransportService.Web.Controllers
{
    public class TMasterController : Controller
    {
        // GET: TMaster
        public ActionResult Index()
        {
            IEnumerable<VehicleType> result;
            using (JobDbContext jobDbContext = new JobDbContext())
            {
                result = jobDbContext.VehicleTypes.SqlQuery(@"exec USP_SelectAllVehicleTypes").ToList();

            }

            return PartialView("Index", result);
        }
        public ActionResult Create()
        {
            VehicleType vehicleType = new VehicleType();

            return View(vehicleType);
        }

    }
}