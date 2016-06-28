using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MegaZord.Library.MVC
{

    public class MZRazorViewEngine : RazorViewEngine
    {
        public MZRazorViewEngine(IList<string> custonPaths)
            : base()
        {
            //{0} o que foi digitado o {1} o path corrent
            var itensLocation = new List<string>
                {
                    "~/Views/CRUD/{1}/{0}.cshtml",
                    "~/Views/Shared/LogInLogOut/{0}.cshtml",
                    "~/Views/{1}/{0}.cshtml",
                    "~/Views/{0}.cshtml",
                };
            if (custonPaths.Any())
            {
                itensLocation = itensLocation.Concat(custonPaths).ToList();
            }
            base.ViewLocationFormats = base.ViewLocationFormats.Concat(itensLocation).Distinct().ToArray();

            base.PartialViewLocationFormats = base.PartialViewLocationFormats.Concat(itensLocation).Distinct().ToArray();

        }
    }

}
