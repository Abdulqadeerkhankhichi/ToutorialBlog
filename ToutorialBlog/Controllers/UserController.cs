using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToutorialBlog.Models;

namespace ToutorialBlog.Controllers
{
    [NoDirectAccess]
    [AuthorizeAction1FilterAttribute]
    public class UserController : Controller
    {
        ToutorialBlogEntities DB = new ToutorialBlogEntities();
        // GET: User
        public ActionResult Users(string Createmessage, string Deletemessage, string updatemessage, string Error)
        {
            try
            {
                ViewBag.Createmessage = Createmessage;
                ViewBag.Deletemessage = Deletemessage;
                ViewBag.updatemessage = updatemessage;
                ViewBag.Error = Error;
                List<tblUser> Users = DB.tblUsers.Where(x => x.isActive == true).ToList();
                ViewBag.Roles = DB.tblRoles.Where(x => x.isActive == true).ToList();
                return View(Users);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
           
        }

        [HttpPost]
        public ActionResult Users(tblUser User)
        {
            tblUser Data = new tblUser();
            List<tblUser> Users = new List<tblUser>();
            try
            {
                HttpCookie cookieObj = Request.Cookies["Data"];
                int UserId = Int32.Parse(cookieObj["UserId"]);
                if (User.UserId == 0)
                {
                    if (DB.tblUsers.Select(r => r).Where(x => x.Email == User.Email && x.isActive== true).FirstOrDefault() == null)
                    {
                        Data = User;
                        byte[] EncDataBtye = new byte[User.Password.Length];
                        EncDataBtye = System.Text.Encoding.UTF8.GetBytes(User.Password);
                        Data.Password = Convert.ToBase64String(EncDataBtye);
                        Data.CreatedDate = DateTime.Now;
                        Data.CreatedBy = UserId;
                        Data.EditDate = DateTime.Now;
                        Data.EditBy = UserId;
                        Data.isActive = true;
                        DB.tblUsers.Add(Data);
                        DB.SaveChanges();
                        ViewBag.Createmessage = "User has been add successfully." ;
                    }
                    else
                    {
                        ViewBag.Error = "User Already Exsist!!!";
                    }
                    Users = DB.tblUsers.Where(x => x.isActive == true).ToList();
                    ViewBag.Roles = DB.tblRoles.Where(x => x.isActive == true).ToList();
                    return View(Users);
                }
                else
                {
                    Data = DB.tblUsers.Select(r => r).Where(x => x.Email == User.Email).FirstOrDefault();
                    if (Data == null || Data.UserId == User.UserId)
                    {
                        Data.Name = User.Name;
                        Data.Email = User.Email;
                        Data.RoleId = User.RoleId;
                       
                        if (User.Password != null)
                        {
                            byte[] EncDataBtye = new byte[User.Password.Length];
                            EncDataBtye = System.Text.Encoding.UTF8.GetBytes(User.Password);
                            Data.Password = Convert.ToBase64String(EncDataBtye);
                        }
                        Data.EditDate = DateTime.Now;
                        Data.EditBy = UserId;
                        DB.Entry(Data);
                        DB.SaveChanges();
                        ViewBag.updatemessage = "User has been Update successfully." ;
                    }
                    else
                    {
                        ViewBag.Error = "User Already Exsist!!!";
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
            Users = DB.tblUsers.Where(x => x.isActive == true).ToList();
            ViewBag.Roles = DB.tblRoles.Where(x => x.isActive == true).ToList();
            return View(Users);
        }

        [HttpPost]
        public ActionResult Delete(int Id)
        {
            tblUser Data = new tblUser();
            try
            {
                if(DB.tblArticles.Where(x=>x.Author==Id||x.CreatedBy==Id||x.EditBy==Id).FirstOrDefault()==null&& DB.tblRoles.Where(x =>x.CreatedBy == Id || x.EditBy == Id).FirstOrDefault() == null)
                {
                    Data = DB.tblUsers.Select(r => r).Where(x => x.UserId == Id).FirstOrDefault();
                    DB.Entry(Data).State = EntityState.Deleted;
                    DB.SaveChanges();
                    return RedirectToAction("Users", new { Deletemessage = "User has been delete successfully." });
                }
                else
                {
                    return RedirectToAction("Users", new { Error = "User has been used in articles." });
                }
               
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return RedirectToAction("Users", new { Error = ex.Message });
            }
        }

        public ActionResult Roles(string Createmessage, string Deletemessage, string updatemessage, string Error, int? id = 0)
        {
            try
            {
                ViewBag.Message = "Your application description page.";
                ViewBag.Createmessage = Createmessage;
                ViewBag.Deletemessage = Deletemessage;
                ViewBag.updatemessage = updatemessage;
                ViewBag.Error = Error;
                var Roles = DB.tblRoles.ToList();
                ViewBag.AdminCount = DB.tblUsers.Where(x => x.RoleId == 1).Count();
                ViewBag.UserCount = DB.tblUsers.Where(x => x.RoleId == 2).Count();
                ViewBag.SelectedMenuAccess = (from h in DB.tblMenus
                                              join t in DB.tblAccessLevels.Where(x => x.RoleId == id) on h.MenuId equals t.MenuId into gj
                                              from acc in gj.DefaultIfEmpty()
                                              select new MenuAccess
                                              {
                                                  Menu = h,
                                                  //accesslevelid = acc.accesslevelid,
                                                  EditAccess = acc.EditAccess,
                                                  DeleteAccess = acc.DeleteAccess,
                                                  CreateAccess = acc.CreateAccess,
                                                  ApproveAccess = acc.ApproveAccess,
                                                  isActive = acc.isActive,
                                                  RoleId = id,
                                                  MenuId = h.MenuId,

                                              }).ToList();
                return View(Roles);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
            
        }


        public ActionResult CreateRole(List<tblRole> HeadData, List<MenuAccess> TailData)
        {
            tblRole Data = new tblRole();
            try
            {
                HttpCookie cookieObj = Request.Cookies["Data"];
                int UserId = Int32.Parse(cookieObj["UserId"]);
                if (HeadData.FirstOrDefault().RoleId == 0)
                {
                    var Role = HeadData.FirstOrDefault().Role;
                    if (DB.tblRoles.Where(x => x.Role == Role).FirstOrDefault() == null)
                    {
                        Data.Role = Role;
                        Data.isActive = true;
                        Data.CreatedDate = DateTime.Now;
                        Data.CreatedBy = UserId;
                        Data.EditDate = DateTime.Now;
                        Data.EditBy = UserId;
                        DB.tblRoles.Add(Data);
                        DB.SaveChanges();
                        var ID = Data.RoleId;

                        foreach (var item in TailData)
                        {
                            tblAccessLevel Data1 = new tblAccessLevel();
                            Data1.RoleId = ID;
                            Data1.MenuId = item.MenuId;
                            Data1.isActive = item.isActive;
                            Data1.CreateAccess = item.CreateAccess;
                            Data1.EditAccess = item.EditAccess;
                            Data1.DeleteAccess = item.DeleteAccess;
                            Data1.ApproveAccess = item.ApproveAccess;
                            DB.tblAccessLevels.Add(Data1);
                            DB.SaveChanges();
                        }
                        
                    }
                    else
                    {


                    }

                }
                else
                {
                    var Role = HeadData.FirstOrDefault().Role;
                    var RoleId = HeadData.FirstOrDefault().RoleId;
                    var Check = DB.tblRoles.Where(x => x.Role == Role).FirstOrDefault();
                    if (Check == null || Check.RoleId == RoleId)
                    {
                        Data = DB.tblRoles.Where(x => x.RoleId == RoleId).FirstOrDefault();
                        Data.Role = Role;
                        Data.isActive = true;
                        Data.EditDate = DateTime.Now;
                        DB.Entry(Data);
                        DB.SaveChanges();
                        var ID = Data.RoleId;
                        List<tblAccessLevel> Data2 = new List<tblAccessLevel>();
                        Data2 = DB.tblAccessLevels.Select(r => r).Where(x => x.RoleId == ID).ToList();
                        foreach (var item in Data2)
                        {
                            DB.tblAccessLevels.Remove(item);
                        }

                        foreach (var item in TailData)
                        {
                            tblAccessLevel Data1 = new tblAccessLevel();
                            Data1.RoleId = ID;
                            Data1.MenuId = item.MenuId;
                            Data1.isActive = item.isActive;
                            Data1.CreateAccess = item.CreateAccess;
                            Data1.EditAccess = item.EditAccess;
                            Data1.DeleteAccess = item.DeleteAccess;
                            Data1.ApproveAccess = item.ApproveAccess;
                            DB.tblAccessLevels.Add(Data1);
                            DB.SaveChanges();
                        }
                        
                    }
                    else
                    {


                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return Json(-1);
            }

            return Json(1);
        }


        [HttpPost]
        public ActionResult GetAccessLevel(int RoleId = 0)
        {
            List<MenuAccess> MenuAccess = new List<MenuAccess>();
            List<tblMenu> Menu = new List<tblMenu>();
            DB.Configuration.ProxyCreationEnabled = false;
            try
            {
                Menu = DB.tblMenus.ToList();
                MenuAccess = (from h in DB.tblMenus
                              join t in DB.tblAccessLevels.Where(x => x.RoleId == RoleId) on h.MenuId equals t.MenuId into gj
                              from acc in gj.DefaultIfEmpty()
                              select new MenuAccess
                              {
                                  Menu = h,
                                  //accesslevelid = acc.accesslevelid,
                                  EditAccess = acc.EditAccess,
                                  DeleteAccess = acc.DeleteAccess,
                                  CreateAccess = acc.CreateAccess,
                                  ApproveAccess = acc.ApproveAccess,
                                  isActive = acc.isActive,
                                  RoleId = RoleId,
                                  MenuId = h.MenuId,

                              }).ToList();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return Json(-1);
            }

            return Json(MenuAccess, JsonRequestBehavior.AllowGet);
        }
    }
}