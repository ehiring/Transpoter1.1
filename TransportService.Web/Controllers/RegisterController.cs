﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
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
        public ActionResult Test([Bind(Exclude = "IsEmailVerified,ActivationCode,PasswordHash")]User _user)
        {
            bool Status = false;
            string message = "";
            //
            // Model Validation 
            if (ModelState.IsValid)
            {
                
                #region "Generate Activation Code"
                _user.ActivationCode = Guid.NewGuid();
                #endregion

                #region "Generate OTP"
                _user.OTP = new JobDbContext().Database.SqlQuery<int>("USP_GenerateOTP").SingleOrDefault<int>();
                #endregion

                #region  "Password Hashing "
                _user.PasswordHash = Crypto.Hash(_user.Password);
                //_clientRegister.ConfirmPassword = Crypto.Hash(_clientRegister.ConfirmPassword); //
                #endregion


                #region "Save to Database"
                using (JobDbContext jobDbContext = new JobDbContext())
                {
                    //dc.Users.Add(user);
                    //dc.SaveChanges();


                    var result = jobDbContext.Database.ExecuteSqlCommand(@"exec USP_RegisterClient
                                                                    @ClientTypeID ,
                                                                    @Email ,
                                                                    @Password ,
                                                                    @PasswordHash,
                                                                    @Mobile ,
                                                                    @ActivationCode,
                                                                    @OTP",
                                                                    new SqlParameter("@ClientTypeID", _user.ClientTypeID),
                                                                    new SqlParameter("@Email", _user.Email == null ? (object)DBNull.Value : _user.Email),
                                                                    new SqlParameter("@Password", _user.Password),
                                                                    new SqlParameter("@PasswordHash", _user.PasswordHash),
                                                                    new SqlParameter("@Mobile", _user.Mobile),
                                                                    new SqlParameter("@ActivationCode", _user.ActivationCode ),
                                                                    new SqlParameter("@OTP", _user.ActivationCode ));
                    

                    SendOTPOnMobile(_user.Mobile,_user.OTP);

                    //Send Email to User
                    if (_user.Email != null)
                    {
                        SendVerificationLinkEmail(_user.Email, _user.ActivationCode.ToString());
                    }
                    
                    message = "Registration successfully done. Account activation link " +
                        " has been sent to your email id:" + _user.Email;
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
            return View(_user);
        }
        public void SendOTPOnMobile(string MobileNo,int OTP)
        {


        }

        public void SendVerificationLinkEmail(string email, string activationCode)
        {

            var verifyUrl = "/Register/VerifyAccountByMail/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("info@bhadaa.com", "Bhada.com");
            var toEmail = new MailAddress(email);
            var fromEmailPassword = "harishmn"; // Replace with actual password
            string subject = "Your account is successfully created!";

            string body = "<br/><br/>We are excited to tell you that your Truck Load account is" +
                " successfully created. Please click on the below link to verify your account" +
                " <br/><br/><a href='" + link + "'>" + link + "</a> ";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                //UseDefaultCredentials = false,
                UseDefaultCredentials = true,
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
        public ActionResult VerifyAccountByMail(string id)
        {
            bool Status = false;
            using (JobDbContext _jobDbContext = new JobDbContext())
            {
                var v = _jobDbContext.Database.SqlQuery<User>("exec USP_SelectFromUserWhereActivationCode @ActivationCode ", new SqlParameter("@ActivationCode", id)).SingleOrDefault<User>();
                if (v != null)
                {
                   v.IsUserNameVerified  = true;
                   _jobDbContext.Database.ExecuteSqlCommand("exec USP_UpdateUserIsVerifiedByMail @ActivationCode", new SqlParameter("@ActivationCode", id));
                   Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }
            }
            ViewBag.Status = Status;
            return View();
        }

        [HttpGet]
        public ActionResult TestLogin()
        {
            return View();
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult TestLogin(LoginUser _loginUser, string ReturnUrl = "")
        {
            string message = "";
            using (JobDbContext jobDbContext = new JobDbContext())
            {
                var v = jobDbContext.Database.SqlQuery<User>("exec USP_SelectPasswordHashWhereUserName @Username", new SqlParameter("@Username", _loginUser.UserName)).FirstOrDefault<User>();
                if (v != null)
                {

                    /*  Is User Entered the OTP and verified
                     
                     */

                    if (!v.IsUserNameVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }
                    if (string.Compare(Crypto.Hash(_loginUser.Password), v.PasswordHash) == 0)
                    {
                        int timeout = _loginUser.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(_loginUser.UserName, _loginUser.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);


                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("LoginSuccess", "Register");
                        }
                        

                    }
                    else
                    {
                        message = "Invalid credential provided";
                    }
                }
                else
                {
                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            return View();
        }


        [Authorize]
        public ActionResult LoginSuccess()
        {
            return View();
        }
    }
}
