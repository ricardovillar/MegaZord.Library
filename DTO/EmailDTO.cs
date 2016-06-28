using System;
using System.Collections.Generic;
using System.Net.Mail;
using MegaZord.Library.Common;
using MegaZord.Library.Helpers;
using MegaZord.Library.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;

namespace MegaZord.Library.DTO
{
    public class EmailDTO
    {
        public EmailDTO()
        {
            this.Dest = new List<string>();
            this.Cc = new List<string>();
            this.Bcc = new List<string>();
            this.Anexos = new List<Attachment>();
        }

        public EmailDTO(string preLoadedMail) : this()
        {
            this.PreLoadedMail = preLoadedMail;
        }

        public string Assunto { get; set; }
        public IList<string> Dest { get; private set; }
        public IList<string> Cc { get; private set; }
        public IList<string> Bcc { get; private set; }
        public string EmailAutor { get; set; }
        public string ConteudoMensagem { get; set; }
        public string ConteudoCabecalho { get; set; }
        public string ConteudoRodape { get; set; }
        public IList<Attachment> Anexos { get; private set; }

        private string PreLoadedMail { get; set; }


        private string Repace(string nomeTag, string valorTag, string texto)
        {

            string patter = string.Format("\\[\\<{0}\\>\\]", nomeTag);

            if (string.IsNullOrEmpty(valorTag))
                valorTag = "";

            return new Regex(patter, RegexOptions.IgnoreCase).Replace(texto, valorTag);
        }

        public string MensagemMontada
        {
            get
            {
                var text = "";

                if (String.IsNullOrEmpty(this.PreLoadedMail))
                {
                    var parameter = MZHelperInjection.MZGetRepository<IMZParametrosRepository>().GetMany(x => x.NomeParametro.ToUpper().Equals(MegaZord.Library.Common.MZConsts.MZInfraConsts.HtlBaseParameterName)).FirstOrDefault();
                    text = parameter.ValorParametro;

                    text = Repace("HEADER", this.ConteudoCabecalho, text);
                    text = Repace("CONTEUDO", this.ConteudoMensagem, text);
                    text = Repace("FOOTER", this.ConteudoRodape, text);
                }
                else
                {
                    text = this.PreLoadedMail;
                }

                try
                {
                    
                    text = CssInliner.Inliner.InlineCssIntoHtml(text);
                }
                catch
                {//abafa qualquer erro no inline CSS


                }

                return text;
            }
        }
    }
}
