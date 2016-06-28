using MegaZord.Library.MVC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MegaZord.Library.Helpers
{
    public static class MZHelperMVC
    {
        public static string Absolute(string relativeOrAbsolute)
        {
            var path = VirtualPathUtility.ToAbsolute(relativeOrAbsolute);

            return new Uri(HttpContext.Current.Request.Url, path).AbsoluteUri;

            
        }

        public static string GetHtmlReponse(ActionResult result, ControllerContext controllerContext)
        {
            using (var it = new MZMVCResponseCapture(controllerContext.RequestContext.HttpContext.Response))
            {
                result.ExecuteResult(controllerContext);
                return it.ToString();
            }
        }
    }
}
