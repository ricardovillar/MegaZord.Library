using System;
using System.Web.Mvc;
using MegaZord.Library.Common;
using MegaZord.Library.MVC.Filters;

namespace MegaZord.Library.MVC
{
    public class MZFilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new MZConfigViewBag());
            filters.Add(new MZCultureFilter());
            filters.Add(new MZCacheFilter());
            filters.Add(new MZCompressResponse());
            
        }
    }
}