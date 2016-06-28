using System;
using System.Web;
using System.Web.Mvc;
using System.Security.Permissions;
using System.Text;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc.Html;

namespace MegaZord.Library.MVC
{
    [AspNetHostingPermission(SecurityAction.LinkDemand, Level = AspNetHostingPermissionLevel.Minimal)]
    public static class HtmlHelperExtensions
    {


        public static string ActionImage<T>(this HtmlHelper html, Expression<Action<T>> action, string imageRelativeUrl, string alt, object imageAttributes)
             where T : Controller
        {
            var url = new UrlHelper(html.ViewContext.RequestContext);

            var imgBuilder = new TagBuilder("img");
            imgBuilder.MergeAttribute("src", url.Content(imageRelativeUrl));
            imgBuilder.MergeAttribute("alt", alt);
            string imgHtml = imgBuilder.ToString(TagRenderMode.SelfClosing);


            var expression = action.Body as MethodCallExpression;
            string actionMethodName = string.Empty;
            if (expression != null)
            {
                actionMethodName = expression.Method.Name;
            }
            string href = new UrlHelper(html.ViewContext.RequestContext, html.RouteCollection).Action(actionMethodName, typeof(T).Name.Remove(typeof(T).Name.IndexOf("Controller"))).ToString();

            var retorno = string.Format("<a href=\"{0}\">{1}</a>", href, imgHtml);

            return retorno;
        }
    }
}
