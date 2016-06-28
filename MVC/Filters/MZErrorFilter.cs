using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MegaZord.Library.MVC.Filters
{


    public class MZErrorFilter : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            
            // if the request is AJAX return JSON else view.
            if (filterContext.HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                filterContext.Result = new JsonResult
                {
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    Data = new
                    {
                        error = true,
                        message = filterContext.Exception.Message
                    }
                };
            }
            else
            {
                var controllerName = (string)filterContext.RouteData.Values["controller"];
                var actionName = (string)filterContext.RouteData.Values["action"];
                var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary(model),
                    TempData = filterContext.Controller.TempData
                };
            }


            filterContext.ExceptionHandled = true;
            filterContext.HttpContext.Response.Clear();
             


            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
     
}
