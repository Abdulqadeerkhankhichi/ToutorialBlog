using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using ToutorialBlog.Models;

namespace ToutorialBlog
{
    
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAction1FilterAttribute : ActionFilterAttribute
    {
       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ToutorialBlogEntities DB = new ToutorialBlogEntities();
            
            HttpSessionStateBase session = filterContext.HttpContext.Session;
            Controller controller = filterContext.Controller as Controller;
            
            if (controller != null)
            {
                if (session["Access"] == null)
                {
                    var url = filterContext.HttpContext.Request.Url;

                    filterContext.Result = new RedirectToRouteResult(new
                                         RouteValueDictionary(new { controller = "Accounts", action = "Login", url  }));
                }
            }

            base.OnActionExecuting(filterContext);
        }
    }


    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class NoDirectAccessAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.UrlReferrer == null ||
     filterContext.HttpContext.Request.Url.Host != filterContext.HttpContext.Request.UrlReferrer.Host)
            {
                filterContext.Result = new RedirectToRouteResult(new
                                          RouteValueDictionary(new { controller = "Accounts", action = "Login" }));
            }
        }
    }
}
