using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TransportService.Web.Models.Masters
{
    public class User
    {
        public int UserID { get; set; }
        public int UserTypeID { get; set; }
        public int ClientID { get; set; }
        public string Userame { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address { get; set; }

        public int VehicleID { get; set; }
        public int IsActive { get; set; }

        /*.....*/
        public string IPAddress { get; set; }
        public int IsUserNameVerified { get; set; }
        public string ActivationCode { get; set; }



    }
    public class LoginUser
    {
        public string EmailID { get; set; }
        public string Mobile { get; set; }
        public string Password { get; set; }
        public int RememberMe { get; set; }
    }
}