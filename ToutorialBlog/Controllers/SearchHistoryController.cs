using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ToutorialBlog.Models;

namespace ToutorialBlog.Controllers
{
    [NoDirectAccess]
    [AuthorizeAction1FilterAttribute]
    public class SearchHistoryController : Controller
    {
        ToutorialBlogEntities DB = new ToutorialBlogEntities();
        // GET: SearchHistory
        public ActionResult Index()
        {
            HttpCookie cookieObj = Request.Cookies["Data"];
            int UserId = Int32.Parse(cookieObj["UserId"]);
            List<HistoryArticle> Articles = new List<HistoryArticle>();

            Articles = (from h in DB.tblHistories.Where(x=>x.UserId==UserId) 
                                          join a in DB.tblArticles on h.ArticleId equals a.ArticleId
                                          select new HistoryArticle
                                          {
                                              History = h,
                                              Article = a
                                          }).OrderByDescending(o=>o.History.Date).ToList();


            //var FID = DB.tblHistories.Where(x => x.UserId == UserId).Select(s => s.ArticleId).ToList();
            //Articles = DB.tblArticles.Where(x => FID.Contains(x.ArticleId)).OrderBy(o=>o.da).ToList();
            return View(Articles);
        }
    }
}