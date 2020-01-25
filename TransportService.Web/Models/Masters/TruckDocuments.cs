using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TransportService.Web.Models.Masters
{
    public class TruckDocuments
    {
        [Key]
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public int DocumentTypeID { get; set; }
        public int VehicleID { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }

        public bool IsSelected { get; set; }

    }
}