using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using MegaZord.Library.Common;
using System.Web;
using System.Web.Security;
using MegaZord.Library.DTO;
using System;
using System.Data.Entity.Validation;
using System.Text;

namespace MegaZord.Library.Helpers
{
    public static class MZHelperValidators
    {
        public static string GetMensagemRequerido(string field)
        {
            return string.Format(CultureInfo.CurrentCulture, "O Campo {0}: é obrigatório.", field);
        }

        public static string GetMensagemCampoPrecisaSerUmNumero(string field)
        {
            return string.Format(CultureInfo.CurrentCulture, "O Campo {0}: precisa ser um número válido.", field);
        }

        public static string GetMensagemCampoPrecisaSerUmaData(string field)
        {
            return string.Format(CultureInfo.CurrentCulture, "O Campo {0}: precisa ser uma data válida.", field);
        }

        public static string GetMensagemCampoPrecisaSerUmEmail(string field)
        {
            return string.Format(CultureInfo.CurrentCulture, "O Campo  {0} : Precisa ser um e-mail válido.", field);
        }

        public static string GetMensagemTamanhoCampo(string field, int minlength, int maxlength)
        {

            return string.Format(CultureInfo.CurrentCulture, "O Campo {0}: deve possuir um tamanho mínimo de {1} caracteres e no máximo de {2} caracteres.", field, minlength, maxlength);
        }

        public static string GetMensagemIntervaloValor(string field, object min, object max)
        {
            return string.Format(CultureInfo.CurrentCulture, "O Campo {0}: precisa estar contido  obrigatório entre {1} e {2}."
                            , new[] { field, min, max });

        }

        public static string GetMensagemMaiorOuIgual(string field, object min)
        {
            return string.Format(CultureInfo.CurrentCulture, "O Campo {0}: precisa ser maior ou igual a {1}."
                        , new[] { field, min });

        }

        public static string GetMensagemMenorOuIgual(string field, object min)
        {
            return string.Format(CultureInfo.CurrentCulture, "O Campo {0}: precisa ser menor ou igual a {1}."
                        , new[] { field, min });

        }

    }
}
