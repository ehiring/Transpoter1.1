using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using TransportService.Web.BusinessLayer;
using TransportService.Web.Models.Masters;
namespace TransportService.Web.Controllers
{
    public class RegisterController : Controller
    {
        // GET: Register
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Test()
        {
            return View();

        }
        [HttpPost]
        public ActionResult Test([Bind(Exclude = "IsEmailVerified,ActivationCode,PasswordHash")]ClientRegister _clientRegister)
        {
            bool Status = false;
            string message = "";
            //
            // Model Validation 
            if (ModelState.IsValid)
            {
               


                #region Generate Activation Code 
                _clientRegister.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                _clientRegister.PasswordHash = Crypto.Hash(_clientRegister.Password);
                //_clientRegister.ConfirmPassword = Crypto.Hash(_clientRegister.ConfirmPassword); //
                #endregion


                #region Save to Database
                using (JobDbContext jobDbContext = new JobDbContext())
                {
                    //dc.Users.Add(user);
                    //dc.SaveChanges();


                    var result = jobDbContext.Database.ExecuteSqlCommand(@"exec USP_RegisterClient
                                                                    @ClientTypeID ,
                                                                    @Email ,
                                                                    @Password ,
                                                                    @PasswordHash,
                                                                    @Mobile ",
                                                                    new SqlParameter("@ClientTypeID", _clientRegister.ClientTypeID),
                                                                    new SqlParameter("@Email", _clientRegister.Email == null ? (object)DBNull.Value : _clientRegister.Email),
                                                                    new SqlParameter("@Password", _clientRegister.Password),
                                                                    new SqlParameter("@PasswordHash", _clientRegister.PasswordHash),
                                                                    new SqlParameter("@Mobile", _clientRegister.Mobile));

                    //Send Email to User
                    //SendVerificationLinkEmail(_clientRegister.Email, _clientRegister.ActivationCode.ToString());
                    message = "Registration successfully done. Account activation link " +
                        " has been sent to your email id:" + _clientRegister.Email;
                    Status = true;
                }
                #endregion


            }
            else
            {
                message = "Invalid Request";
            }
            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(_clientRegister);

        }

        public void SendVerificationLinkEmail(string email, string activationCode)
        {
           
                var verifyUrl = "/User/VerifyAccount/" + activationCode;
                var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

                var fromEmail = new MailAddress("dotnetawesome@gmail.com", "Dotnet Awesome");
                var toEmail = new MailAddress(email);
                var fromEmailPassword = "********"; // Replace with actual password
                string subject = "Your account is successfully created!";

                string body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
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

        public string Hash(string value)
        {
            return Convert.ToBase64String(
                System.Security.Cryptography.SHA256.Create()
                .ComputeHash(Encoding.UTF8.GetBytes(value))
                );
        }

        public int IsEmailExist(string Email)
        {
            using (JobDbContext jobDbContext = new JobDbContext())
            {
                return jobDbContext.Database.SqlQuery<int>(@"exec USP_SelectUserIDWhereEmail @Email", new SqlParameter("@Email", Email)).SingleOrDefault<int>();

            }
        }

        public int IsMobileExist(string MobileNo)
        {
            using (JobDbContext jobDbContext = new JobDbContext())
            {
                return jobDbContext.Database.SqlQuery<int>(@"exec USP_SelectUserIDWhereMobileNo @Mobile", new SqlParameter("@Mobile", MobileNo)).SingleOrDefault<int>();

            }
        }



        /// <summary>
        /// On click of  verification link send on user mail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            //bool Status = false;
            //using (MyDatabaseEntities dc = new MyDatabaseEntities())
            //{
            //    dc.Configuration.ValidateOnSaveEnabled = false; // This line I have added here to avoid 
            //                                                    // Confirm password does not match issue on save changes
            //    var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();
            //    if (v != null)
            //    {
            //        v.IsEmailVerified = true;
            //        dc.SaveChanges();
            //        Status = true;
            //    }
            //    else
            //    {
            //        ViewBag.Message = "Invalid Request";
            //    }
            //}
            //ViewBag.Status = Status;
            return View();
        }

        [HttpGet]
        public ActionResult TestLogin()
        {
            return View();
        }
    }
}