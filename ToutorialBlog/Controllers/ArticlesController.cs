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
    public class ArticlesController : Controller
    {
        ToutorialBlogEntities DB = new ToutorialBlogEntities();
        // GET: Articles
        public ActionResult Index(string Createmessage, string Deletemessage, string updatemessage, string Error,int Tab=-1)
        {

            try
            {
                ViewBag.Createmessage = Createmessage;
                ViewBag.Deletemessage = Deletemessage;
                ViewBag.updatemessage = updatemessage;
                ViewBag.Error = Error;
                ViewBag.Tab = Tab;
                ViewBag.Access = Session["Access"];
                List<tblAccessLevel> AccessLevel = (List<tblAccessLevel>)ViewBag.Access;
                foreach (var item in AccessLevel)
                {
                    if (item.MenuId == 5)
                    {
                        ViewBag.CreateAccess = item.CreateAccess;
                        ViewBag.EditAccess = item.EditAccess;
                        ViewBag.DeleteAccess = item.DeleteAccess;
                        ViewBag.ApproveAccess = item.ApproveAccess;
                    }
                }


                HttpCookie cookieObj = Request.Cookies["Data"];
                int UserId = Int32.Parse(cookieObj["UserId"]);
                tblUser Data = DB.tblUsers.Where(x => x.UserId == UserId).FirstOrDefault();
                List<tblArticle> Articles = new List<tblArticle>();
                var FID = DB.tblFavoriteLists.Where(x => x.UserId == UserId).Select(s => s.ArticleId).ToList();
                ViewBag.FArticles = DB.tblArticles.Where(x => FID.Contains(x.ArticleId)).ToList();
                if (Data.RoleId == 1)
                {
                    Articles = DB.tblArticles.Where(x => x.isActive == true).ToList();
                }
                else
                {
                    Articles = DB.tblArticles.Where(x => x.isActive == true && x.Author == UserId).ToList();
                }
                return View(Articles);
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
        }

        public ActionResult Search(string Search, string Tags, bool AnyOne = false,int id=0)
        {
            try
            {
                HttpCookie cookieObj = Request.Cookies["Data"];
                int UserId = Int32.Parse(cookieObj["UserId"]);
                ViewBag.Access = Session["Access"];
                List<tblAccessLevel> AccessLevel = (List<tblAccessLevel>)ViewBag.Access;
                foreach (var item in AccessLevel)
                {
                    if (item.MenuId == 6)
                    {
                        ViewBag.CreateAccess = item.CreateAccess;
                        ViewBag.EditAccess = item.EditAccess;
                        ViewBag.DeleteAccess = item.DeleteAccess;
                        ViewBag.ApproveAccess = item.ApproveAccess;
                    }
                }

                if(Tags==null)
                {
                    Tags = "";
                    
                }
                ViewBag.ParaTag = Tags;
                List<string> mylist = new List<string>(new string[] { "" });
                if(Search==null)
                {
                    Search = "";
                }
                string[] NTags = new string[] { };
                if (Tags!=null)
                {
                    NTags = Tags.Split(',');
                    ViewBag.TagsState = NTags.ToList();
                }
                else
                {
                    ViewBag.TagsState = mylist;
                }
                ViewBag.SearchState = Search;
                
                ViewBag.AnyOneState = AnyOne;
                List<SearchArticle_Result2> Data = new List<SearchArticle_Result2>();
                string Query = " where 1 = 1 and Status<3 ";


                if (Search != "")
                {
                    Query += " and  (Title  LIKE '%" + Search + "%' or dbo.fn_parsehtml(Body) LIKE '%" + Search + "%')";
                }



                string Tag = "";

                var TTags = ViewBag.TagsState;

                if (Tags!=null&& Tags != "")
                {

                    Tag += " and T.Tag in(";
                    for (int i = 0; i < TTags.Count; i++)
                    {
                        Tag += "'" + TTags[i] + "'";
                        if (i + 1 == TTags.Count)
                        {

                        }
                        else
                        {
                            Tag += ',';
                        }
                    }
                    Tag += ") ";

                    //Tag += " and T.Tag in( "+Tags+")";

                }
                else if (Tags != null && Tags != "")
                {
                    Tag += " and A.Tags like";
                    for (int i = 0; i < NTags.Length; i++)
                    {
                        Tag += "'%" + NTags[i] + "%'";
                        if (i + 1 == NTags.Length)
                        {

                        }
                        else
                        {
                            Tag += " and A.Tags like ";
                        }
                    }

                }

                Query += Tag;

                Data = DB.SearchArticle(Query, false, " and F.UserId=" + UserId.ToString()).ToList();
                ViewBag.Tags = DB.tblTags.Select(s => s.Tag).Distinct().OrderBy(c => c).ToList();
                return View(Data);

            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
            
        }

        [HttpPost]
        public ActionResult Search(string Search,List<string> Tags,bool AnyOne=false)
        {
            try
            {
                


                HttpCookie cookieObj = Request.Cookies["Data"];
                int UserId = Int32.Parse(cookieObj["UserId"]);
                ViewBag.Access = Session["Access"];


                tblHistory History = new tblHistory();
                History.Search = Search;
                History.Toggle = AnyOne;
                string HTag = "";
                if (Tags!=null)
                {
                    for (int i = 0; i < Tags.Count(); i++)
                    {
                        if (i + 1 == Tags.Count())
                        {
                            HTag += Tags[i];
                        }
                        else
                        {
                            HTag += Tags[i] + ",";
                        }
                    }
                }
                
                History.Tags = HTag;
                History.UserId = UserId;
                History.Date = DateTime.Now;
                DB.tblHistories.Add(History);
                DB.SaveChanges();

                List<tblAccessLevel> AccessLevel = (List<tblAccessLevel>)ViewBag.Access;
                foreach (var item in AccessLevel)
                {
                    if (item.MenuId == 6)
                    {
                        ViewBag.CreateAccess = item.CreateAccess;
                        ViewBag.EditAccess = item.EditAccess;
                        ViewBag.DeleteAccess = item.DeleteAccess;
                        ViewBag.ApproveAccess = item.ApproveAccess;
                    }
                }
                List<SearchArticle_Result2> Data = new List<SearchArticle_Result2>();

                ViewBag.SearchState = Search;
                if (Tags != null)
                {
                    ViewBag.TagsState = Tags;
                }
                else
                {
                    List<string> mylist = new List<string>(new string[] { "" });
                    ViewBag.TagsState = mylist;
                }

                ViewBag.AnyOneState = AnyOne;


                string ParaTag = "";

                if(Tags!=null)
                {
                    for (int i = 0; i < Tags.Count(); i++)
                    {
                        if(i+1==Tags.Count())
                        {
                            ParaTag += Tags[i];
                        }
                        else
                        {
                            ParaTag += Tags[i]+",";
                        }
                    }
                }

                ViewBag.ParaTag = ParaTag;
                string Query = " where 1 = 1 and Status<3 ";
                if (Search != "")
                {
                    Query += " and  (Title  LIKE '%" + Search + "%' or dbo.fn_parsehtml(Body)  LIKE '%" + Search + "%')";
                }



                string Tag = "";

                if (Tags != null && AnyOne == true)
                {
                    Tag += " and T.Tag in(";
                    for (int i = 0; i < Tags.Count(); i++)
                    {
                        Tag += "'" + Tags[i] + "'";
                        if (i + 1 == Tags.Count())
                        {

                        }
                        else
                        {
                            Tag += ',';
                        }
                    }
                    Tag += ") ";
                }
                else if (Tags != null)
                {
                    Tag += " and A.Tags like";
                    for (int i = 0; i < Tags.Count(); i++)
                    {
                        Tag += "'%" + Tags[i] + "%'";
                        if (i + 1 == Tags.Count())
                        {

                        }
                        else
                        {
                            Tag += " and A.Tags like ";
                        }
                    }

                }

                Query += Tag;

                Data = DB.SearchArticle(Query, false, " and F.UserId=" + UserId.ToString()).ToList();
                ViewBag.Tags = DB.tblTags.Select(s => s.Tag).Distinct().OrderBy(c => c).ToList();
                return View(Data);
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return View();
            }
            
        }
        public ActionResult Add(int? id=0)
        {
            try
            {
                tblArticle Data = new tblArticle();
                if (id != 0)
                {
                    Data = DB.tblArticles.Where(x => x.ArticleId == id).FirstOrDefault();
                }
                else
                {
                    Data.ArticleId = 0;
                    Data.Title = "";
                    Data.Body = "";
                    Data.Tags = "";
                }
                return View(Data);

            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return RedirectToAction("Index",new { Error= ex.Message });
            }
            
        }

        [HttpPost]
        public ActionResult Add(tblArticle Article)
        {
           
            tblArticle Data = new tblArticle();
            try
            {
                HttpCookie cookieObj = Request.Cookies["Data"];
                int UserId = Int32.Parse(cookieObj["UserId"]);
                if (Article.ArticleId == 0)
                {
                    //if (DB.tbl.Select(r => r).Where(x => x.Email == User.Email && x.isActive == true).FirstOrDefault() == null)
                    //{
                        Data = Article;
                        Data.Status = 0;
                        Data.CreatedDate = DateTime.Now;
                        Data.CreatedBy = UserId;
                        Data.EditDate = DateTime.Now;
                        Data.EditBy = UserId;
                        Data.isActive = true;
                        DB.tblArticles.Add(Data);
                        DB.SaveChanges();
                    int ID = Data.ArticleId;
                    string[] Tags = Article.Tags.Split(',');
                    var TagList = DB.tblTags.Select(s=>s.Tag).ToList();
                    TagList = TagList.ConvertAll(d => d.ToLower());
                    foreach (var item in Tags)
                    {

                        //if(!TagList.Contains(item.ToLower()))
                        //{
                            tblTag Data1 = new tblTag();
                            Data1.Tag = item;
                            Data1.ArticleId = ID;
                            DB.tblTags.Add(Data1);
                            DB.SaveChanges();
                        //}
                        
                    }


                    ViewBag.Createmessage = "Article has been add successfully.";
                    //}
                    //else
                    //{
                    //    ViewBag.Error = "User Already Exsist!!!";
                    //}
                    
                    return Json(1);
                }
                else
                {
                    Data = DB.tblArticles.Select(r => r).Where(x => x.ArticleId == Article.ArticleId).FirstOrDefault();
                    //if (Data == null || Data.UserId == User.UserId)
                    //{
                        Data.Title = Article.Title;
                        Data.Body = Article.Body;
                        Data.Tags = Article.Tags;
                        Data.Status = 0;
                        Data.EditDate = DateTime.Now;
                        Data.EditBy = UserId;
                        DB.Entry(Data);
                        DB.SaveChanges();
                    int ID = Data.ArticleId;

                    List<tblTag> Data2 = new List<tblTag>();
                    Data2 = DB.tblTags.Select(r => r).Where(x => x.ArticleId == ID).ToList();
                    foreach (var item in Data2)
                    {
                        DB.tblTags.Remove(item);
                    }
                    string[] Tags = Article.Tags.Split(',');
                        var TagList = DB.tblTags.Select(s => s.Tag).ToList();
                        TagList = TagList.ConvertAll(d => d.ToLower());
                        foreach (var item in Tags)
                        {

                            if (!TagList.Contains(item.ToLower()))
                            {
                                tblTag Data1 = new tblTag();
                                Data1.Tag = item;
                                Data1.ArticleId = ID;
                                DB.tblTags.Add(Data1);
                                DB.SaveChanges();
                            }

                        }

                        ViewBag.updatemessage = "Article has been Update successfully.";
                    //}
                    //else
                    //{
                    //    ViewBag.Error = "User Already Exsist!!!";
                    //}
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return Json(-1);
            }
            return Json(2);
        }

        [HttpPost]
        public ActionResult Approved(int Id,int Value)
        {
            
            tblArticle Data = new tblArticle();
            string Message = "";
            try
            {
                HttpCookie cookieObj = Request.Cookies["Data"];
                int UserId = Int32.Parse(cookieObj["UserId"]);

                Data = DB.tblArticles.Where(x => x.ArticleId == Id).FirstOrDefault();
                if (Data.Status==0)
                {
                    ViewBag.Tab = 0;
                }
                else if(Data.Status == 1)
                {
                    ViewBag.Tab = 1;
                }
                else if(Data.Status == 2)
                {
                    ViewBag.Tab = 2;
                }
                
                Data.Status = Value;
                
                if(Value==1)
                {
                    Data.ApprovedDate = DateTime.Now;
                    Data.ApprovedBy = UserId;
                    Message = "Article has been Approved successfully.";
                }
                else
                {
                    Data.UnApprovedDate = DateTime.Now;
                    Data.UnApprovedBy = UserId;
                    Message = "Article has been UnApproved successfully.";
                }
                DB.Entry(Data);
                DB.SaveChanges();

            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return RedirectToAction("Index", new { Error = ex.Message});
            }
            return RedirectToAction("Index",new { updatemessage= Message, Tab = ViewBag.Tab });
        }

        [HttpPost]
        public ActionResult Delete(int Id,int Tab)
        {
           
            tblArticle Data = new tblArticle();
            try
            {
                List<tblTag> Data2 = new List<tblTag>();
                List<tblFavoriteList> Data3 = new List<tblFavoriteList>();
                List<tblHistory> Data4 = new List<tblHistory>();
                Data2 = DB.tblTags.Select(r => r).Where(x => x.ArticleId== Id).ToList();
                foreach (var item in Data2)
                {
                    DB.tblTags.Remove(item);
                    DB.SaveChanges();
                }
                Data3 = DB.tblFavoriteLists.Select(r => r).Where(x => x.ArticleId== Id).ToList();
                foreach (var item in Data3)
                {
                    DB.tblFavoriteLists.Remove(item);
                    DB.SaveChanges();
                }
                Data4 = DB.tblHistories.Select(r => r).Where(x => x.ArticleId== Id).ToList();
                foreach (var item in Data4)
                {
                    DB.tblHistories.Remove(item);
                    DB.SaveChanges();
                }
                Data = DB.tblArticles.Select(r => r).Where(x => x.ArticleId == Id).FirstOrDefault();
                DB.Entry(Data).State = EntityState.Deleted;
                DB.SaveChanges();
                if (Tab==0)
                {
                    if (Data.Status == 0)
                    {
                        ViewBag.Tab = 0;
                    }
                    else if (Data.Status == 1)
                    {
                        ViewBag.Tab = 1;
                    }
                    else if (Data.Status == 2)
                    {
                        ViewBag.Tab = 2;
                    }
                }
                else if(Tab==1)
                {
                    ViewBag.Tab = 3;
                }
                else
                {
                    ViewBag.Tab = 4;
                }
                
                
                return RedirectToAction("Index", new { Deletemessage = "Article has been delete successfully.", Tab = ViewBag.Tab });
            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return RedirectToAction("Index", new { Error = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult SetFavorite(tblFavoriteList FavoriteList, int Check)
        {
            tblFavoriteList Data = new tblFavoriteList();
            if(Check==1)
            {
                Data = FavoriteList;
                DB.tblFavoriteLists.Add(Data);
                DB.SaveChanges();
            }
            else
            {
                Data = DB.tblFavoriteLists.Where(x => x.ArticleId == FavoriteList.ArticleId).FirstOrDefault();
                DB.tblFavoriteLists.Remove(Data);
                DB.SaveChanges();
            }

            return Json(1);
        }



        public ActionResult ViewArticles(int? Id=0)
        {
            try
            {
                ViewBag.Access = Session["Access"];
                List<tblAccessLevel> AccessLevel = (List<tblAccessLevel>)ViewBag.Access;
                foreach (var item in AccessLevel)
                {
                    if (item.MenuId == 5)
                    {
                        ViewBag.CreateAccess = item.CreateAccess;
                        ViewBag.EditAccess = item.EditAccess;
                        ViewBag.DeleteAccess = item.DeleteAccess;
                        ViewBag.ApproveAccess = item.ApproveAccess;
                    }
                }
                tblArticle Data = DB.tblArticles.Where(x => x.ArticleId == Id).FirstOrDefault();
                return View(Data);

            }
            catch (Exception ex)
            {

                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return RedirectToAction("Index", new { Error = ex.Message });
            }
            
        }


        public ActionResult SearchViewArticles(string Search,string Tags,bool AnyOne, int? Id = 0)
        {
            try
            {
                HttpCookie cookieObj = Request.Cookies["Data"];
                int UserId = Int32.Parse(cookieObj["UserId"]);
                tblHistory History = new tblHistory();


                ViewBag.Search = Search;
                ViewBag.Tags = Tags;
                ViewBag.AnyOne = AnyOne;

                History.ArticleId= Id;
                History.UserId = UserId;
                History.Date = DateTime.Now;
                DB.tblHistories.Add(History);
                DB.SaveChanges();
                ViewBag.Access = Session["Access"];
                List<tblAccessLevel> AccessLevel = (List<tblAccessLevel>)ViewBag.Access;
                foreach (var item in AccessLevel)
                {
                    if (item.MenuId == 5)
                    {
                        ViewBag.CreateAccess = item.CreateAccess;
                        ViewBag.EditAccess = item.EditAccess;
                        ViewBag.DeleteAccess = item.DeleteAccess;
                        ViewBag.ApproveAccess = item.ApproveAccess;
                    }
                }
                tblArticle Data = DB.tblArticles.Where(x => x.ArticleId == Id).FirstOrDefault();
                return View(Data);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                Console.WriteLine("Error" + ex.Message);
                return RedirectToAction("Index", new { Error = ex.Message });
            }
            
        }
    }
}