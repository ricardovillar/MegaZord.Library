using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MegaZord.Library.MVC.Filters
{
    public class MZCultureFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var culture = "pt-BR";
            if (HttpContext.Current.Session != null)
                culture = (string)HttpContext.Current.Session["culture"];
            if (culture == null)
            {
                culture = "pt-BR";
                HttpContext.Current.Session["culture"] = culture;
            }
            

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        }
    }
}
