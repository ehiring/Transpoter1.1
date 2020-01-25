using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransportService.Web.Models.Masters;
using TransportService.Web.BusinessLayer;
namespace TransportService.Web.Controllers
{
    public class ShowDatabaseImageController : Controller
    {
        public ActionResult Index()
        {
            List<TruckDocuments> images = GetImages();
            return View(images);
        }

        [HttpPost]
        public ActionResult Index(int imageId)
        {
            List<TruckDocuments> images = GetImages();
            TruckDocuments image = images.Find(p => p.DocumentID == imageId);
            if (image != null)
            {
                image.IsSelected = true;
                ViewBag.Base64String = "data:image/png;base64," + Convert.ToBase64String(image.Data, 0, image.Data.Length);
            }
            return View(images);
        }

        private List<TruckDocuments> GetImages()
        {
            
            List<TruckDocuments> images = new List<TruckDocuments>();
            using ( JobDbContext jobDbContext = new JobDbContext())
            {
                images = jobDbContext.DBTruckDocuments.SqlQuery("exec USP_SelectAllTruckDocuments").ToList<TruckDocuments>();

                return images;
            }
        }
    }
}