using MegaZord.Library.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaZord.Library.MVC.Results
{
    public class MZJsonResponse
    {
        public bool Sucesso { get; private set; }
        public string Mensagem { get; private set; }

        public object Results { get; private set; }
        public MZJsonResponse()
            : this(true)
        {

        }
        public MZJsonResponse(bool sucesso)
            : this(sucesso, string.Empty)
        {

        }
        public MZJsonResponse(Exception ex)
            : this(false, MZHelperUtil.GetErrorMessage(ex))
        {

        }

        public MZJsonResponse(bool sucesso, Exception ex)
            : this(sucesso, MZHelperUtil.GetErrorMessage(ex))
        {

        }
        public MZJsonResponse(bool sucesso, string mensagem)
            : this(sucesso, mensagem, null)
        {
        }

        public MZJsonResponse(bool sucesso, string mensagem, object results)
        {
            Sucesso = sucesso;
            Mensagem = mensagem;
            Results = results;
        }
    }
}
