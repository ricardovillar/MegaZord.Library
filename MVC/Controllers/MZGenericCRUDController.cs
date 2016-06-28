using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using DevExpress.Web.Mvc;
using MegaZord.Library.Common;
using MegaZord.Library.DTO;
using MegaZord.Library.Interfaces;
using System.Web.Security;
using MegaZord.Library.Helpers;
using MegaZord.Library.MVC.Results;
using System.Text.RegularExpressions;
using System.Net;
using System.Collections.Specialized;
using System.Text;
using System.Collections.Generic;

namespace MegaZord.Library.Controllers
{

    [HandleError]
    public abstract class MZBaseController : Controller
    {
        private readonly IMZLoggerFactory _loggerFactory;
        protected MZBaseController()
            : this(MZHelperInjection.MZGetLogFactory())
        {
        }


        protected MZBaseController(IMZLoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnException(ExceptionContext filterContext)
        {

            _loggerFactory.CurrentErrorLog.Log(MZConsts.MZInfraConsts.OcorreuUmErro, filterContext.Exception);
            base.OnException(filterContext);
        }


        protected override void Initialize(System.Web.Routing.RequestContext requestContext)
        {
            InternalSetCulture(MZConsts.MZInfraConsts.CulturePTBR);

            base.Initialize(requestContext);
        }

        protected string GetHtml(ActionResult result, ControllerContext controllerContext)
        {
            return MegaZord.Library.Helpers.MZHelperMVC.GetHtmlReponse(result, controllerContext);
        }
        protected override void ExecuteCore()
        {
            InternalSetCulture(MZConsts.MZInfraConsts.CulturePTBR);

            base.ExecuteCore();
        }
        private void InternalSetCulture(string culture = null)
        {
            if ((RouteData != null) && (HttpContext != null))
            {
                if (RouteData.Values["lang"] != null &&
                    !string.IsNullOrWhiteSpace(RouteData.Values["lang"].ToString()))
                {
                    // set the culture from the route data (url)
                    var lang = RouteData.Values["lang"].ToString();
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(lang);
                }
                else
                {
                    // load the culture info from the cookie
                    var cookie = HttpContext.Request.Cookies[MZConsts.MZInfraConsts.CookieName];
                    var langHeader = (cookie != null) ? cookie.Value : HttpContext.Request.UserLanguages[0];
                    langHeader = string.IsNullOrEmpty(culture) ? langHeader : culture;
                    // set the lang value into route data
                    RouteData.Values["lang"] = langHeader;
                    Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(langHeader);
                }

                // save the location into cookie
                HttpCookie _cookie = new HttpCookie(MZConsts.MZInfraConsts.CookieName, Thread.CurrentThread.CurrentUICulture.Name);
                _cookie.Expires = DateTime.Now.AddYears(1);
                HttpContext.Response.SetCookie(_cookie);
            }
            else if (!string.IsNullOrEmpty(culture))
            {
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(culture);
            }
        }

        /// <summary>
        /// Registra um alert de erro no sistema
        /// </summary>
        /// <param name="msg">mensagem a ser registrada</param>
        protected void RegisterError(string msg)
        {
            RegisterJS(string.Format("erro('{0}')", msg));
        }
        /// <summary>
        /// registra um JS para ser executado
        /// </summary>
        /// <param name="js">js a ser executado</param>
        private void RegisterJS(string js)
        {
            TempData[MegaZord.Library.Common.MZConsts.MZInfraConsts.ScriptRegister] = string.Format(@"<script> $(document).ready(function () {{ {0}  }});</script>", js);
        }

        protected TReposity GetRepository<TReposity>() where TReposity : IMZRepository
        {

            return MZHelperInjection.MZGetRepository<TReposity>();
        }
        protected virtual void RegisterUserActive(DateTime date, UserDataDTO user)
        {

        }
        protected UserDataDTO GetUserDataInContext()
        {
            UserDataDTO result = null;

            if (FormsAuthentication.CookiesSupported == true)
            {
                if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
                {
                    try
                    {
                        //let us take out the username now                
                        var ticket = FormsAuthentication.Decrypt(Request.Cookies[FormsAuthentication.FormsCookieName].Value);
                        if (ticket != null)
                        {
                            result = MZHelperSerialize.Deserialize<UserDataDTO>(ticket.UserData);
                            RegisterUserActive(DateTime.Now, result);
                        }
                    }
                    catch
                    {
                        FormsAuthentication.SignOut();
                    }
                }
            }

            return result;
        }

        protected void SetViewDataError(Exception ex, string erroKey)
        {

            SetViewDataError(new MZServerError(ex), erroKey);

        }
        protected void SetViewDataError(MZServerError errorServer, string erroKey)
        {
            ViewData[erroKey] = errorServer;


        }

        public ActionResult SetCulture(string culture)
        {

            InternalSetCulture(culture);




            // Split the url to url + query string
            var fullUrl = Request.UrlReferrer.ToString();
            var questionMarkIndex = fullUrl.IndexOf('?');
            string queryString = null;
            string url = fullUrl;
            if (questionMarkIndex != -1) // There is a QueryString
            {
                url = fullUrl.Substring(0, questionMarkIndex);
                queryString = fullUrl.Substring(questionMarkIndex + 1);
            }

            // Arranges
            using (var sw = new StringWriter())
            {
                var request = new HttpRequest(null, url, queryString);
                var response = new HttpResponse(sw);
                var httpContext = new HttpContext(request, response);

                var routeData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

                // Extract the data    
                var values = routeData.Values;
                var controllerName = values["controller"];
                var actionName = values["action"];
                var areaName = values["area"];

                return RedirectToAction(actionName.ToString(), controllerName.ToString());
            }

        }

        public JsonResult GetCEP(string CEP)
        {
            var urlDetalhe = "http://www.buscacep.correios.com.br/servicos/dnec/detalheCEPAction.do";
            var urlCEP = "http://www.buscacep.correios.com.br/servicos/dnec/consultaLogradouroAction.do";
            var startToken = "javascript:detalharCep";
            var endToken = "</tr>";
            var clenToken = "<td";
            MZJsonResponse retorno;
            try
            {
                using (var wb = new WebClient())
                {
                    var data = new NameValueCollection();
                    data["CEP"] = CEP;
                    data["Metodo"] = "listaLogradouro";
                    data["TipoConsulta"] = "cep";
                    data["StartRow"] = "1";
                    data["EndRow"] = "10";

                    var dataDetalhe = new NameValueCollection();

                    dataDetalhe["Metodo"] = "detalhe";
                    dataDetalhe["Posicao"] = "1";
                    dataDetalhe["TipoCep"] = "1";
                    dataDetalhe["CEP"] = CEP;

                    var response2 = wb.UploadValues(urlDetalhe, "POST", dataDetalhe);
                    string result = Encoding.GetEncoding("iso-8859-1").GetString(response2);
                    var response = wb.UploadValues(urlCEP, "POST", data);
                    result = Encoding.GetEncoding("iso-8859-1").GetString(response);
                    //remove tudo que tem antes do start
                    var startIndex = result.IndexOf(startToken);
                    result = result.Substring(startIndex);

                    //remove tudo o que tem depos do end
                    var endindex = result.IndexOf(endToken);
                    result = result.Substring(0, endindex);

                    //remove o lixo que sobrou
                    var cleanindex = result.IndexOf(clenToken);
                    result = result.Substring(cleanindex);

                    //ajusta itens em branco
                    result = result.Replace("/>", "></td>");


                    var sPattern = "<td.*>(.*?)<\\/td>";


                    var matchs = System.Text.RegularExpressions.Regex.Matches(result, sPattern, RegexOptions.Multiline);

                    var query = from m in matchs.OfType<Match>().ToList()
                                where m.Groups.Count > 1
                                select
                                  m.Groups[1].Captures[0].Value;

                    var itens = query.ToList();

                    var obj = new { Rua = itens[0], Bairro = itens[1], Cidade = itens[2], Estado = itens[3], CEP = itens[4] };

                    retorno = new MZJsonResponse(true, string.Empty, obj);

                }
            }
            catch (Exception e)
            {

                retorno = new MZJsonResponse(false, e);
            }
            return Json(retorno, JsonRequestBehavior.AllowGet);
        }

        protected void Log(Exception exception)
        {
            _loggerFactory.CurrentErrorLog.Log(MZConsts.MZInfraConsts.OcorreuUmErro, exception);
        }

    }

    public abstract class MZGenericCRUDController<TEntity, TIReposity, TCreateUpdateCommand, TDeleteCommand> : MZBaseController
        where TEntity : class, IMZEntity
        where TIReposity : IMZRepository<TEntity>
        where TCreateUpdateCommand : class , IMZCommand
        where TDeleteCommand : class , IMZCommand
    {
        private readonly IMZCommandBus _commandBus;
        private readonly TIReposity _repository;

        protected MZGenericCRUDController(IMZCommandBus commandBus, TIReposity repository)
            : base(MZHelperInjection.MZGetLogFactory())
        {
            _commandBus = commandBus;
            _repository = repository;
            ViewBag.Title = this.TitleUnit;
            ViewBag.TitleList = string.Format("Lista de: {0}", ViewBag.Title);
        }


        #region Visoes
        public ActionResult Index([ModelBinder(typeof(DevExpressEditorsBinder))] int? page)
        {
            if (!page.HasValue || page.Value == 0)
                page = 1;

            var onePageOfEntity = _repository.GetAll();
            return View(onePageOfEntity);
        }

        public ActionResult List()
        {
            return PartialView(_repository.GetAll());
        }

        public ActionResult Details([ModelBinder(typeof(DevExpressEditorsBinder))] int id)
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Edit([ModelBinder(typeof(DevExpressEditorsBinder))]int id)
        {
            var entity = _repository.GetById(id);
            var viewModel = GetMappedViewModel(entity);
            return View(viewModel);
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Save([ModelBinder(typeof(DevExpressEditorsBinder))] TEntity form)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    var command = GetCreateCommand(form);
                    var errors = _commandBus.Validate(command);

                    ModelState.AddModelErrors(errors);

                    if (ModelState.IsValid)
                    {

                        var result = _commandBus.Submit(command);
                        if (result.Success)
                        {
                            if (UsingUniqueViewInCRUD)
                                return RedirectToAction(ViewNameToIndex);
                            else
                                return View(form.ID == 0 ? ViewNameToCreate : ViewNameToEdit, form);
                        }
                        else
                        {
                            SetViewDataError(result.Error, ViewDataErrorKey);
                        }

                    }
                }
                if (!ModelState.IsValid)
                {

                    var str = new System.Text.StringBuilder();
                    str.Append("<ul>");
                    foreach (var e in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        str.Append("<li>" + e.ErrorMessage + "</li>");
                    }
                    str.Append("</ul>");
                    throw new Exception(str.ToString());
                }
            }
            catch (Exception e)
            {
                SetViewDataError(e, ViewDataErrorKey);

            }


            return PartialView(ViewNameToList, _repository.GetAll());



        }

        [HttpPost]
        public ActionResult Delete(long id)
        {

            var command = GetDeleteCommand(id);
            var result = _commandBus.Submit(command);
            if (!result.Success)
            {
                SetViewDataError(result.Error, ViewDataErrorKey);
            }
            var entitys = _repository.GetAll();


            return PartialView(ViewNameAfterDelete, entitys);


        }
        #endregion

        #region Propriedades

        protected TIReposity Reposity
        {
            get
            {
                return this.GetRepository<TIReposity>();
            }
        }
        protected abstract string TitleUnit { get; }

        protected virtual bool UsingUniqueViewInCRUD
        {
            get { return true; }
        }

        protected virtual string ViewDataErrorKey
        {
            get { return MZConsts.MZControllerNamesConsts.ViewDataErrorKey; }
        }

        protected virtual string ViewNameAfterDelete
        {
            get { return MZConsts.MZControllerNamesConsts.ViewNameAfterDelete; }
        }

        protected virtual string ViewNameToCreate
        {
            get { return MZConsts.MZControllerNamesConsts.ViewNameToCreate; }
        }

        protected virtual string ViewNameToEdit
        {
            get { return MZConsts.MZControllerNamesConsts.ViewNameToEdit; }
        }

        protected virtual string ViewNameToIndex
        {
            get { return MZConsts.MZControllerNamesConsts.ViewNameToIndex; }
        }
        protected virtual string ViewNameToList
        {
            get { return MZConsts.MZControllerNamesConsts.ViewNameToList; }
        }
        #endregion

        #region Commands
        protected virtual TEntity GetMappedViewModel(TEntity entity)
        {
            //return Mapper.Map<TEntity, TEntity>(entity);
            return entity;
        }

        protected virtual TDeleteCommand GetDeleteCommand(long id)
        {
            var deleteCmd = Activator.CreateInstance<TDeleteCommand>();
            deleteCmd.ID = id;
            return deleteCmd;
        }

        protected virtual TCreateUpdateCommand GetCreateCommand(TEntity form)
        {
            return Mapper.Map<TEntity, TCreateUpdateCommand>(form);
        }
        #endregion





    }
}

