using System;
using System.Data.Entity.Validation;
using System.Reflection;
using System.Text;
using System.Web;

namespace MegaZord.Library.Common
{
    [Serializable]
    public sealed class MZServerError
    {

        public MZServerError(Exception ex)
        {
            this.Message = ex.Message;
            this._msgErrorFull = new StringBuilder();

            InternalGetCompleteRecursiveMessage(ex, 1);


        }

        public string Message { get; private set; }
        public string FullMessageError
        {
            get
            {
                return this._msgErrorFull.ToString().Trim();
            }
        }

        private readonly StringBuilder _msgErrorFull;
        private void InternalGetCompleteRecursiveMessage(Exception ex, long nivel)
        {
            if (ex.InnerException != null)
            {
                var newNivel = nivel + 1;
                InternalGetCompleteRecursiveMessage(ex.InnerException, newNivel);

            }



            var currentMessage = GetExceptionMessage(ex);
            var currentTypeEx = GetExType(ex);
            var currentStack = GetStackMessage(ex);
            var currentEFValidationFail = GetEntityValidationErrorsText(ex);
            var currentLoadExceptionErros = GetLoadReflectionErrorsText(ex);
            var currentContext = HttpContext.Current;


            _msgErrorFull.AppendLine(string.Format("Nível :{0}", nivel));
            _msgErrorFull.AppendLine(string.Format("Tipo da exception: {0}", currentTypeEx));
            if (currentContext != null && string.IsNullOrEmpty(currentLoadExceptionErros))
            {

                var url = "Indefinido";
                try
                {
                    url = currentContext.Request.Url.ToString();
                }
                catch { }
                _msgErrorFull.AppendLine(string.Format("URL de contexto: {0}", url));
            }

            _msgErrorFull.AppendLine();

            _msgErrorFull.AppendLine("Mensagem:");
            _msgErrorFull.AppendLine(currentMessage);
            _msgErrorFull.AppendLine();

            if (!string.IsNullOrEmpty(currentEFValidationFail))
            {
                _msgErrorFull.AppendLine("Erro de validação do EF apresentando:");
                _msgErrorFull.AppendLine(currentEFValidationFail);
                _msgErrorFull.AppendLine();
            }

            if (!string.IsNullOrEmpty(currentLoadExceptionErros))
            {
                _msgErrorFull.AppendLine("Erro de load via reflection:");
                _msgErrorFull.AppendLine(currentLoadExceptionErros);
                _msgErrorFull.AppendLine();
            }

            _msgErrorFull.AppendLine("Stack:");
            _msgErrorFull.AppendLine(currentStack);
            _msgErrorFull.AppendLine();
            _msgErrorFull.AppendLine();

        }

        private string GetExType(Exception ex)
        {
            return ex.GetType().FullName.Trim();
        }
        private string GetStackMessage(Exception ex)
        {
            return ex.StackTrace;
            //var startIndex = ex.StackTrace != null ? ex.StackTrace.IndexOf('\n') : 0;
            //if (startIndex < 0)
            //    startIndex = 0;
            //var stack = string.Format("{0}", ex.StackTrace != null ? ex.StackTrace.Substring(startIndex).Trim() : string.Empty);
            //return stack;
        }
        private string GetExceptionMessage(Exception ex)
        {
            return ex.Message.Trim();
        }

        private string GetLoadReflectionErrorsText(Exception ex)
        {
            var LoadReflectionError = (ex as ReflectionTypeLoadException);

            var sb = new StringBuilder();
            if (LoadReflectionError != null)
            {
                foreach (var failure in LoadReflectionError.LoaderExceptions)
                {
                    if (sb.ToString().IndexOf(failure.Message) < 0)
                        sb.AppendLine(failure.Message);

                }
            }
            return sb.ToString().Trim();
        }

        private string GetEntityValidationErrorsText(Exception ex)
        {
            var EFExceptionDbValidation = (ex as DbEntityValidationException);

            var sb = new StringBuilder();
            if (EFExceptionDbValidation != null)
            {
                foreach (var failure in EFExceptionDbValidation.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} falha na validação\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }
            }
            return sb.ToString().Trim();

        }

    }
}
