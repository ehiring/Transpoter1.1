using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Masters
{
    public class RoleMaster
    {
        public int RoleID { get; set; }
        public string Role { get; set; }
        public int IsActive { get; set; }
    }
    public static class RoleColumns
    {
        public const string RoleID = "RoleID";
        public const string Role = "Role";
        public const string IsActive = "IsActive";
    }

}