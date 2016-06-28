using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web.Mvc;
using MegaZord.Library.Helpers;

namespace MegaZord.Library.MVC.Filters
{

    //Inject a ViewBag object to Views for getting information about an authenticated user
    public class MZConfigViewBag : ActionFilterAttribute
    {

        public bool Has(object obj, string propertyName)
        {
            var dynamic = obj as DynamicObject;
            return dynamic != null && dynamic.GetDynamicMemberNames().Contains(propertyName);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {


            if (!Has(filterContext.Controller.ViewBag, "Title"))
            {
                filterContext.Controller.ViewData.Add("Title", MZHelperConfiguration.MZAppName);
            }
 

            base.OnActionExecuted(filterContext);
        }
    }

}
