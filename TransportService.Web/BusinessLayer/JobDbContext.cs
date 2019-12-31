using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TransportService.Web.Models;
using TransportService.Web.Models.Activity;
using TransportService.Web.Models.Masters;

namespace TransportService.Web.BusinessLayer
{
    public class JobDbContext:DbContext
    {
        static JobDbContext()
        {
            Database.SetInitializer<JobDbContext>(null);
        }
        public JobDbContext():base("Name=JobDbContext")
        {

        }
        public DbSet<Transpoter> DBTransporter { get; set; }
        public DbSet<RouteDetails> DBRouteDetails { get; set; }
        public DbSet<AvalableSpace> DBAvalableSpace { get; set; }
        public DbSet<subtripDetails>  DBsubtripDetails { get; set; }
        public DbSet<Loader> DBLoader { get; set; }
        public DbSet<LoadDetail> DBLoadDetails { get; set; }
        public DbSet<MaterialList> DBMaterialList { get; set; }
        public DbSet<LoaderEdit> DBLoaderEdit { get; set; }
        public DbSet<VehicleCapacity> DBvehicleCapacities { get; set; }
    }
}