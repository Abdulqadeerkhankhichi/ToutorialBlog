using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ToutorialBlog.Models;

namespace ToutorialBlog.Controllers
{
    public class AccountsController : Controller
    {
        ToutorialBlogEntities DB = new ToutorialBlogEntities();
        // GET: Accounts
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string url)
        {
            try
            {
                ViewBag.url = url;
                FormsAuthentication.SignOut();
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
           
        }
        [HttpPost]
        public ActionResult Login(string Email, string Password,string url)
        {
            string pass = null;
            try
            {
                if (Password != null)
                {
                    byte[] EncDataBtye = new byte[Password.Length];
                    EncDataBtye = System.Text.Encoding.UTF8.GetBytes(Password);
                    pass = Convert.ToBase64String(EncDataBtye);
                }

                if (DB.tblUsers.Select(r => r).Where(x => x.Email == Email && x.Password == pass && x.isActive != false).FirstOrDefault() != null)
                {
                    var User = DB.tblUsers.Select(r => r).Where(x => x.Email == Email).FirstOrDefault();

                    HttpCookie cookie = new HttpCookie("Data");

                    cookie["Email"] = DB.tblUsers.Where(x => x.Email== Email).Select(s => s.Email).FirstOrDefault();
                    cookie["UserId"] = DB.tblUsers.Where(x => x.Email== Email).Select(s => s.UserId).FirstOrDefault().ToString();
                    cookie["RoleId"] = DB.tblUsers.Where(x => x.Email== Email).Select(s => s.RoleId).FirstOrDefault().ToString();
                    cookie["UserName"] = DB.tblUsers.Where(x => x.Email== Email).Select(s => s.Name).FirstOrDefault().ToString();
                    // This cookie will remain  for one month.
                    cookie.Expires = DateTime.Now.AddMonths(1);

                    // Add it to the current web response.
                    Response.Cookies.Add(cookie);
                    Session["Access"] = DB.tblAccessLevels.Select(r => r).Where(x => x.RoleId == User.RoleId && x.isActive == true).ToList();
                    FormsAuthentication.SetAuthCookie(Email, false);

                    if(url!=null)
                    {
                        string[] URL = url.Split('/');
                        string[] Para = URL[4].Split('?');
                        if (Para.Count()>1)
                        {
                            string P = Para[1];
                            string[] FP = P.Split('=');
                            string A = FP[0];
                            string Combine= URL[3]+"/"+Para[0]+"?"+A+"="+FP[1];
                            return RedirectToAction(Para[0], URL[3],new {id= FP[1] });
                        }
                        return RedirectToAction(Para[0], URL[3]);
                    }

                    if(User.RoleId==2)
                    {
                        return RedirectToAction("Index", "Articles");
                    }

                    return RedirectToAction("Users", "User");
                }
                else
                {
                    var UserCheck = DB.tblUsers.Select(r => r).Where(x => x.Email == Email && x.Password == pass).FirstOrDefault();
                    if (UserCheck != null && UserCheck.isActive == false)
                    {
                        ViewBag.Error = "User is in-active";
                    }
                    else
                    {
                        ViewBag.Error = "Invalid Email or Password";
                    }

                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
        }

        public ActionResult Register(string Createmessage, string Deletemessage, string updatemessage, string Error)
        {
            try
            {
                ViewBag.Createmessage = Createmessage;
                ViewBag.Deletemessage = Deletemessage;
                ViewBag.updatemessage = updatemessage;
                ViewBag.Error = Error;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
           
        }
        
        [HttpPost]
        public ActionResult Register(tblUser User)
        {
            tblUser Data = new tblUser();
            try
            {
                if (DB.tblUsers.Select(r => r).Where(x => x.Email == User.Email).FirstOrDefault() == null)
                {

                    Data = User;
                    byte[] EncDataBtye = new byte[User.Password.Length];
                    EncDataBtye = System.Text.Encoding.UTF8.GetBytes(User.Password);
                    Data.Password = Convert.ToBase64String(EncDataBtye);
                    //Data.RoleId = 2;
                    Data.CreatedDate = DateTime.Now;
                    Data.EditDate = DateTime.Now;
                    Data.isActive = true;
                    DB.tblUsers.Add(Data);
                    DB.SaveChanges();

                }
                else
                {
                    return RedirectToAction("Register", new { Error = "User Already Exsist!!!" });
                    
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }

            return RedirectToAction("Register", new { Createmessage = "User has been Added successfully." });
        }



        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string Email)
        {
            
            try
            {
                if (DB.tblUsers.Where(x => x.Email == Email).FirstOrDefault() != null)
                {
                    string SenderEmail = System.Configuration.ConfigurationManager.AppSettings["SenderEmail"].ToString();
                    string SenderPassword = System.Configuration.ConfigurationManager.AppSettings["SenderPassword"].ToString();
                    SmtpClient Client = new SmtpClient("smtp.gmail.com", 587);
                    Client.EnableSsl = true;
                    Client.Timeout = 100000;
                    Client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    Client.UseDefaultCredentials = false;
                    Client.Credentials = new System.Net.NetworkCredential(SenderEmail, SenderPassword);

                    string link = Request.Url.ToString();
                    link = link.Replace("ForgetPassword", "ChangeForgetPassword");

                    byte[] b = System.Text.ASCIIEncoding.ASCII.GetBytes(Email);
                    string encrypted = Convert.ToBase64String(b);


                    string body1 = "";
                    body1 += "Welcome to Knowledge Base!";
                    body1 += "<br />To Change your password, please click on the button below: ";
                    body1 += "<br /> <button style='padding: 10px 28px 11px 28px;color: #fff;background:#009ef7;'><a style='color:white !important' href = '" + link + "?Email=" + encrypted + "'>Change Account Password</a></button>";
                    body1 += "<br /><br />Yours,<br />The Knowledge Base Team";

                    string body = "";
                    body += "<body  style='background-color:white !important'>";
                    body += " <div>";
                    //body += "<h3>Hello " + sa.ReceiveName + ",</h3>";
                    body += " <table style='background-color: #f2f3f8; max-width:670px;' width='100%' border='0'  cellpadding='0' cellspacing='0'>";
                    body += " <tbody> <tr style='background-color:rgba(40, 58, 90, 0.9);'><td style='padding: 0 35px; background-color:#009ef7;'><a><h1 style ='color:white' > Knowledge Base </h1>   </a></td> </tr>";
                    body += "<tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'></td></tr><tr style='color:#455056; font-size:15px;line-height:35px;text-align: center;'><td style='padding:6px;text-align: center;'>" + body1 + "</td></tr>";
                    body += "  </tbody></table>";
                    body += "</body>";


                    MailMessage mailMessage = new MailMessage(SenderEmail, Email, "Forget Password Email", body);
                    mailMessage.IsBodyHtml = true;
                    mailMessage.BodyEncoding = System.Text.UTF8Encoding.UTF8;

                    Client.Send(mailMessage);

                    //mailMessage = new MailMessage(SenderEmail, Email, "Thank You Email", "Thank You for Contacting Us!!!");
                    //mailMessage.IsBodyHtml = true;
                    //mailMessage.BodyEncoding = System.Text.UTF8Encoding.UTF8;

                    //Client.Send(mailMessage);

                    ViewBag.Createmessage = "Email has been successfully sent";
                    return View();
                }
                else
                {
                    ViewBag.Error = "User not register";
                    return View();
                }

            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
        }

        public ActionResult ChangeForgetPassword(string Email)
        {
            try
            {
                byte[] b = Convert.FromBase64String(Email);
                string decrypted = System.Text.ASCIIEncoding.ASCII.GetString(b);

                ViewBag.Email = decrypted;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
            return View();
        }

        [HttpPost]
        public ActionResult ChangeForgetPassword(string NewPassword, string ConfirmPassword, string Email)
        {

            string pass = null;
            try
            {


                byte[] EncDataBtye = null;
                tblUser Data = new tblUser();
                Data = DB.tblUsers.Select(r => r).Where(x => x.Email == Email).FirstOrDefault();
                if (Data != null)
                {

                    if (NewPassword == ConfirmPassword)
                    {
                        EncDataBtye = new byte[NewPassword.Length];
                        EncDataBtye = System.Text.Encoding.UTF8.GetBytes(NewPassword);
                        pass = Convert.ToBase64String(EncDataBtye);
                    }
                    else
                    {
                        ViewBag.Error = "New Password and Confirm Password not Matched!!!";
                        return View();
                    }

                    Data.Password = pass;
                    Data.EditDate = DateTime.Now;
                    DB.Entry(Data);
                    DB.SaveChanges();
                    return RedirectToAction("Login", "Accounts");
                }
                else
                {
                    ViewBag.Error = "Your request is not valid!!!";
                    return View();
                }


            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
        }

        public ActionResult SetupNewPassword()
        {
            return View();
        }
        public ActionResult ChangePassword()
        {
            try
            {
                HttpCookie cookieObj = Request.Cookies["Data"];
                ViewBag.Email = cookieObj["Email"];
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
            
        }

        [HttpPost]
        public ActionResult ChangePassword(string OldPassword, string NewPassword, string ConfirmPassword, string Email)
        {
            HttpCookie cookieObj = Request.Cookies["Data"];
            ViewBag.Email = cookieObj["Email"];
            string pass = null;
            try
            {


                byte[] EncDataBtye = new byte[OldPassword.Length];
                EncDataBtye = System.Text.Encoding.UTF8.GetBytes(OldPassword);
                pass = Convert.ToBase64String(EncDataBtye);
                tblUser Data = new tblUser();
                Data = DB.tblUsers.Select(r => r).Where(x => x.Email == Email && x.Password == pass).FirstOrDefault();
                if (Data != null)
                {

                    if (NewPassword == ConfirmPassword)
                    {
                        EncDataBtye = new byte[NewPassword.Length];
                        EncDataBtye = System.Text.Encoding.UTF8.GetBytes(NewPassword);
                        pass = Convert.ToBase64String(EncDataBtye);
                    }
                    else
                    {
                        ViewBag.Error = "New Password and Confirm Password not Matched!!!";
                        return View();
                    }

                    Data.Password = pass;
                    Data.EditDate = DateTime.Now;
                    DB.Entry(Data);
                    DB.SaveChanges();
                    ViewBag.Createmessage = "Password has been change successfully!!!";
                    return View();
                }
                else
                {
                    ViewBag.Error = "Old password is not Correct!!!";
                    return View();
                }


            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
        }


    }
}