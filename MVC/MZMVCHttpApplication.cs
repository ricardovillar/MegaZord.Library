using MegaZord.Library.DataAnnotation;
using MegaZord.Library.DTO;
using MegaZord.Library.Helpers;
using Ninject;
using Ninject.Web.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace MegaZord.Library.MVC
{
    public abstract class MZMVCHttpApplication : NinjectHttpApplication
    {
        protected override void OnApplicationStarted()
        {
            AppDomain.CurrentDomain.FirstChanceException += (sender, e) =>
            {
               // MZHelperInjection.MZGetLogFactory().CurrentDebugLog.Log(string.Empty, e.Exception);
            };



            MZFilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            MZRouteConfig.RegisterRoutes(RouteTable.Routes, this.GetProjectRoutes(), this.GetProjectViewSearchPath());

            MZResourcesBundleConfig.RegisterBundles(BundleTable.Bundles, GetJsResourcesFiles(), GetCssResourcesFiles());

            MZDataAnnotations.Register();

            SetAutoMapper();

            CreateDBContext();

            base.OnApplicationStarted();
        }

        protected void Application_Error()
        {

            if (Context.IsCustomErrorEnabled)
                ShowCustomErrorPage(Server.GetLastError());

        }

        private void ShowCustomErrorPage(Exception exception)
        {
            var httpException = exception as HttpException ?? new HttpException(500, "Internal Server Error", exception);

            Response.Clear();
            var routeData = new RouteData();
            routeData.Values.Add("controller", "Error");
            routeData.Values.Add("fromAppErrorEvent", true);

            switch (httpException.GetHttpCode())
            {
                case 403:
                    routeData.Values.Add("action", "HttpError403");
                    break;

                case 404:
                    routeData.Values.Add("action", "HttpError404");
                    break;

                case 500:
                    routeData.Values.Add("action", "HttpError500");
                    break;

                default:
                    routeData.Values.Add("action", "GeneralError");
                    routeData.Values.Add("httpStatusCode", httpException.GetHttpCode());
                    break;
            }

            Server.ClearError();

            IController controller = GetErrorController();
            if (controller != null)
                controller.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
        }

        private void CreateDBContext()
        {
            BackgroundWorker bgWorker = new BackgroundWorker();
            bgWorker.DoWork += (sender, e) =>
            {
                RealizeFirsGetEFContext();
            };
            bgWorker.RunWorkerAsync();
        }

        protected abstract IController GetErrorController();
        protected abstract IList<string> GetJsResourcesFiles();

        protected abstract IList<string> GetProjectViewSearchPath();

        protected abstract IList<Route> GetProjectRoutes();
        protected abstract IList<string> GetCssResourcesFiles();

        protected abstract void RealizeFirsGetEFContext();
        protected abstract void AddMapsInKernel(IKernel kernel);
        protected abstract void SetAutoMapper();
        protected override IKernel CreateKernel()
        {
            var kernel = new StandardKernel();
            AddMapsInKernel(kernel);
            DependencyResolver.SetResolver(new MZDependencyResolver(kernel));
            return kernel;
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            //if (HttpContext.Current.Session == null)
            //    FormsAuthentication.RedirectToLoginPage();

        }

        protected void FormsAuthentication_OnAuthenticate(Object sender, FormsAuthenticationEventArgs e)
        {
            if (FormsAuthentication.CookiesSupported)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                
                        var ticket = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value);
                        if (ticket != null)
                        {
                            string username = ticket.Name;
                            //string roles = ticket.UserData;

                            var userData = MZHelperSerialize.Deserialize<UserDataDTO>(ticket.UserData);

                            var roles = userData.Roles;

                            //Let us set the Pricipal with our user specific details
                            e.User = new System.Security.Principal.GenericPrincipal(
                                new System.Security.Principal.GenericIdentity(username, "Forms"), roles.Split(';'));
                        }
                    }
                    catch
                    {
                        FormsAuthentication.SignOut();
                        //somehting went wrong
                    }
                }
            }
        }
    }
}
