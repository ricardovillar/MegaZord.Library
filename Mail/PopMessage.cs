using OpenPop.Mime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MegaZord.Library.Mail
{
    public class PopMessage
    {
        public PopMessage(Message message)
        {
            MessageId = message.Headers.MessageId;
            RawMessage = message.RawMessage;
            Subject = message.Headers.Subject;
            Sender = message.Headers.From.Address;
        }

        public string MessageId { get; set; }
        public byte[] RawMessage { get; set; }
        public string Subject { get; set; }
        public string Sender { get; set; }

        public bool IsInexistingMail()
        {
            var result = false;
            result = IsDeliveryStatusNotification() || IsUndeliveredMailUserUserUnknown() || IsUndeliverable() || IsMailDeliveryFailed();
            return result;
        }


        public string GetRecipient()
        {
            return getRecipientFromUserUnknown();
        }


        private bool IsComplaint()
        {
            return Sender.ToLower().Equals("complaints@us-west-2.email-abuse.amazonses.com");
        }


        private bool IsDeliveryStatusNotification()
        {
            return Subject.ToLower().IndexOf("delivery status notification") >= 0;
        }

        private bool IsMailDeliveryFailed()
        {
            return Subject.ToLower().IndexOf("mail delivery failed") >= 0;
        }
        
        private bool IsUndeliveredMailUserUserUnknown()
        {
            var result = false;

            var mailMessage = System.Text.Encoding.UTF8.GetString(RawMessage);

            result = mailMessage.ToLower().IndexOf("user unknown") >= 0 ||
                     mailMessage.ToLower().IndexOf("550 não existe tal pessoa nesse endereço (in reply to rcpt to command)") >= 0 ||
                     mailMessage.ToLower().IndexOf("550 n??o existe tal pessoa nesse endere??o (in reply to rcpt to command)") >= 0 ||
                     mailMessage.ToLower().IndexOf("no mailbox here by that name") >= 0 ||
                     mailMessage.ToLower().IndexOf("resolver.adr.recipnotfound") >= 0 ||
                     mailMessage.ToLower().IndexOf("the email account that you tried to reach does not exist") >= 0 ||
                     mailMessage.ToLower().IndexOf("that domain isn't in my list of allowed rcpthosts") >= 0 ||
                     mailMessage.ToLower().IndexOf("your message wasn't delivered due to a permission or security issue") >= 0 ||
                     mailMessage.ToLower().IndexOf("no such user") >= 0 ||
                     mailMessage.ToLower().IndexOf("maildir delivery failed") >= 0 ||
                     mailMessage.ToLower().IndexOf("the account or domain may not exist") >= 0;

            return result;
        }

        private bool IsUndeliverable()
        {
            var result = false;

            var mailMessage = System.Text.Encoding.UTF8.GetString(RawMessage);

            result = mailMessage.ToLower().IndexOf("the email address you specified couldn't be found or is invalid") >= 0 ||
                     mailMessage.ToLower().IndexOf("o endereço de email que você inseriu não pôde ser encontrado") >= 0;

            return result;
        }

        public bool IsOverQuota()
        {
            var result = false;

            var mailMessage = System.Text.Encoding.UTF8.GetString(RawMessage);

            result = (mailMessage.ToLower().IndexOf("a caixa postal do usuario") >= 0 && mailMessage.ToLower().IndexOf("ultrapassou o limite maximo de espaco.") >= 0) ||
                     (mailMessage.ToLower().IndexOf("esta conta excedeu a quota") >= 0 && mailMessage.ToLower().IndexOf("maxima permitida.") >= 0) ||
                     mailMessage.ToLower().IndexOf("quota exceeded") >= 0 ||
                     mailMessage.ToLower().IndexOf("quotaexceeded") >= 0 ||
                     mailMessage.ToLower().IndexOf("mailbox is full") >= 0 ||
                     mailMessage.ToLower().IndexOf("over quota") >= 0;

            return result;

        }

        private string getRecipientFromUserUnknown()
        {
            var result = "";            
            var findInitStr = "";            
            var findEndStr = "";

            var mailMessage = System.Text.Encoding.UTF8.GetString(RawMessage);

            if (Subject.ToLower().IndexOf("undelivered mail: user unknown") >= 0)
            {
                findInitStr = "----- The following addresses had permanent delivery errors -----\r\n";
                findEndStr = "\r\n";
            }
            else
            {
                findInitStr = "From: \"WeProc\" <noreply@weproc.com>";

                if (mailMessage.IndexOf(findInitStr) < 0)
                {
                    findInitStr = "From: WeProc <noreply@weproc.com>";
                }

                findEndStr = "X-Priority: 1";
            }

            var indexInit = mailMessage.IndexOf(findInitStr) + findInitStr.Length;
            var indexEnd = mailMessage.IndexOf(findEndStr, indexInit);

            var recpientLine = mailMessage.Substring(indexInit, indexEnd - indexInit);


            Regex rx = new Regex(MegaZord.Library.Common.MZConsts.MZInfraConsts.MatchEmailPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);

            MatchCollection matches = rx.Matches(recpientLine);

            if (matches.Count > 0)
            {
                result = matches[0].Value;
            }

            return result;
        }

    }
}
