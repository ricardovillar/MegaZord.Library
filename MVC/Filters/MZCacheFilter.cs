using MegaZord.Library.Helpers;
using System;
using System.Web;
using System.Web.Mvc;

namespace MegaZord.Library.MVC.Filters
{
    public class MZCacheFilter : ActionFilterAttribute
    { 

       
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if ((MZHelperConfiguration.MZCacheTime <= 0) || !MZHelperConfiguration.MZEnableMinify) return;

            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            TimeSpan cacheDuration = TimeSpan.FromSeconds(MZHelperConfiguration.MZCacheTime);

            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
        }
    }
}