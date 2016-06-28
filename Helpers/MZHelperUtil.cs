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
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace MegaZord.Library.Helpers
{
    public static class MZHelperUtil
    {
        public static CultureInfo PTBRCulture
        {
            get
            {

                return CultureInfo.CreateSpecificCulture(MZConsts.MZInfraConsts.CulturePTBR);
            }
        }

        public static void AddClass(ViewDataDictionary viewData, string newClass)
        {
            var hasClass = viewData.Keys.Any(x => x.ToLower().Equals("class"));

            if (hasClass)
            {
                viewData["class"] += string.Format(" {0}", newClass);
            }
            else
            {
                viewData.Add("class", newClass);
            }
        }
        /// <summary>
        /// Autentica o usuário na app e gera um cookie de autenticação
        /// </summary>
        /// <param name="login"></param>
        /// <param name="lembrarSenha"></param>
        /// <param name="userData"></param>
        /// <returns></returns>
        public static HttpCookie GetCookiToLogin(string login, bool lembrarSenha, UserDataDTO userData)
        {
            FormsAuthentication.SetAuthCookie(login, lembrarSenha);
            var cookie = FormsAuthentication.GetAuthCookie(login, lembrarSenha);
            var ticket = FormsAuthentication.Decrypt(cookie.Value);



            if (ticket != null)
            {

                // Store UserData inside the Forms Ticket with all the attributes
                // in sync with the web.config
                var newticket = new FormsAuthenticationTicket(ticket.Version,
                                                              ticket.Name,
                                                              ticket.IssueDate,
                                                              ticket.Expiration,
                                                              lembrarSenha,
                                                              MZHelperSerialize.Serialize(userData),
                                                              ticket.CookiePath);

                // Encrypt the ticket and store it in the cookie
                cookie.Value = FormsAuthentication.Encrypt(newticket);

            }
            return cookie;
        }

        public static string GetErrorMessage(Exception ex)
        {

            string retorno = string.Empty;
            if (ex is DbEntityValidationException)
            {
                var strLogErro = new StringBuilder();
                var e = (DbEntityValidationException)ex;
                foreach (var eve in e.EntityValidationErrors)
                {
                    var separador = strLogErro.Length == 0 ? string.Empty : System.Environment.NewLine;
                    strLogErro.AppendFormat("{2}Entidade do tipo \"{0}\" com estado \"{1}\" Possui os seguintes erros:", eve.Entry.Entity.GetType().Name, eve.Entry.State, separador);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        strLogErro.AppendFormat(System.Environment.NewLine + "- Propriedade: \"{0}\", Erro: \"{1}\"", ve.PropertyName, ve.ErrorMessage);
                    }
                }
                retorno = strLogErro.ToString();
            }
            else if (ex.InnerException != null)
            {
                var isInnerEx = ex.InnerException;
                while (isInnerEx != null && isInnerEx.InnerException != null)
                {
                    isInnerEx = isInnerEx.InnerException;
                }
                if (isInnerEx != null)
                    retorno = isInnerEx.Message;
            }
            else
            {
                retorno = ex.Message;
            }
            return retorno;
        }

        public static string GeneratePassword()
        {
            IEnumerable<char> characterSet =
       "ABCDEFGHIJKLMNOPQRSTUVWXYZ" +
       "abcdefghijklmnopqrstuvwxyz" +
       "0123456789" +
       MZHelperConfiguration.MZPublicCryptoKey;
            var length = MZHelperConfiguration.MZLengthPassowrdsAutomatic;

            if (length < 0)
                throw new ArgumentException("length must not be negative", "length");
            if (length > int.MaxValue / 8) // 250 million chars ought to be enough for anybody
                throw new ArgumentException("length is too big", "length");
            if (characterSet == null)
                throw new ArgumentNullException("characterSet");
            var characterArray = characterSet.Distinct().ToArray();
            if (characterArray.Length == 0)
                throw new ArgumentException("characterSet must not be empty", "characterSet");

            var bytes = new byte[length * 8];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            var result = new char[length];
            for (int i = 0; i < length; i++)
            {
                ulong value = BitConverter.ToUInt64(bytes, i * 8);
                result[i] = characterArray[value % (uint)characterArray.Length];
            }
            return new string(result);
        }

        public static string StringToFullTextContainsCriteria(string term)
        {
            string baseComa = "*\" AND \"*";
            var cleanedString = RemoveSpecialChars(term);
            cleanedString = Regex.Replace(cleanedString, @"\s+", " ");
            cleanedString = Regex.Replace(cleanedString, @"\s+", baseComa);
            cleanedString = string.Format("{0}{1}{2}", "\"*", cleanedString, "*\"");
            return cleanedString;

        }
        public static string RemoveSpecialChars(string term, string replacechar = " ")
        {
            var cleanedString = string.Empty;
            if (!string.IsNullOrEmpty(term))
                cleanedString = term.Trim();

            var specialChars = "[’',;:`´~!@#$%^&*()_|+\\-=?\\/<>{}\\[\\]\\\"¨-ªº°§.]";


            cleanedString = Regex.Replace(cleanedString, specialChars, replacechar);
            return cleanedString.ToLower().Trim();
        }
        public static string StringToLikeCriteria(string term)
        {
            string baseComa = "%";
            var cleanedString = RemoveSpecialChars(term);
            cleanedString = Regex.Replace(cleanedString, @"\s+", " ");
            cleanedString = Regex.Replace(cleanedString, @"\s+", baseComa);
            cleanedString = string.Format("{0}{1}{2}", "%", cleanedString, "%");
            return cleanedString;

        }

        public static string CombineURL(string baseurl, string relativepath)
        {
            return new Uri(new Uri(baseurl), relativepath).AbsoluteUri;
        }
    }


}
