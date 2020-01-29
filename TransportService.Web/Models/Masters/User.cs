using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TransportService.Web.Models.Masters
{
    public class User 
    {
        public int UserID { get; set; }
        public int? UserTypeID { get; set; }
        public int ClientID { get; set; }
        public int ClientTypeID { get; set; }
        public int? CityID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
        
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public int? VehicleID { get; set; }

        public string AdharCardNo { get; set; }
        public string PanCardNo { get; set; }
        public string PinCode { get; set; }
        public string AlternateContactPerson { get; set; }
        public string AlternateMobileNo { get; set; }
        public string STD { get; set; }
        public string LandlineNo { get; set; }
        public int IsActive { get; set; }

        /*.....*/
        //public string IPAddress { get; set; }
        public bool IsUserNameVerified { get; set; }
        public Guid? ActivationCode { get; set; }
        public int? OTP { get; set; }

        


    }
    public class LoginUser
    {
        [Key]
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }

    public class LoginDetail
    {
        public int UserID { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public int ClientID { get; set; }
        public int ClientTypeID { get; set; }
        public int RoleID { get; set; }

    }

    public class UserDocuments
    {
        public int DocumentID { get; set; }
        public int DocumentTypeID { get; set; }
        public string DocumentName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
        public int UserID { get; set; }
        

    }
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "New password required", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New password and confirm password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }
    }
    public class PersonDetail
    {
        public User user { get; set; }
        public Company company { get; set; }
        public UserDocuments userDocuments { get; set; }
    }




    public static class UserColumns
    {
        public const string UserID = "UserID";
        public const string UserTypeID = "UserTypeID";
        public const string ClientID = "ClientID";
        public const string ClientTypeID = "ClientTypeID";
        public const string FirstName = "FirstName";
        public const string LastName = "LastName";
        public const string Password = "Password";
        public const string PasswordHash = "PasswordHash";
        public const string Email = "Email";
        public const string Mobile = "Mobile";
        public const string Address1 = "Address1";
        public const string Address2 = "Address2";
        public const string VehicleID = "VehicleID";
        public const string IsActive = "IsActive";
        public const string AdharCardNo = "AdharCardNo";
        public const string PanCardNo = "PanCardNo";
        public const string PinCode = "PinCode";
        public const string AlternateContactPerson = "AlternateContactPerson";
        public const string AlternateMobileNo = "AlternateMobileNo";
        public const string STD = "STD";
        public const string LandlineNo = "LandlineNo";


    }
    public static class ExecuteUserProcedure
    {

        public const string USP_UpdateClientAndUser = @"exec USP_UpdateClientAndUser 
                                                        @ClientID,
                                                        @UserID,
                                                        @FirstName,
                                                        @LastName,
                                                        @Password,
                                                        @Email,
                                                        @Address1,
                                                        @Address2,
                                                        @AlternateContactPerson  ,
                                                        @AlternateMobileNo ,
                                                        @GSTNumber  ,
                                                        @DocumentPath ,
                                                        @AdharCardNo ,
                                                        @PanCardNo ,
                                                        @PinCode ,
                                                        @STD ,
                                                        @LandlineNo ,
                                                        @VehicleID ,
                                                        @CompanyName ,
                                                        @CompanyTypeID  ,
                                                        @ServiceTaxNo ,
                                                        @CompanyPanNo ,		
                                                        @CompanyWebsite,
                                                        @DocumentName ,
                                                        @ContentType ,
                                                        @Data ";

    }
}