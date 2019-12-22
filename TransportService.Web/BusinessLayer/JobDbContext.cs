using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using TransportService.Web.Models;
using TransportService.Web.Models.Activity;

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
    }
}