using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using MegaZord.Library.DTO;
using System.Net.Mime;
using System;
using System.Text.RegularExpressions;
using MegaZord.Library.Mail;
using OpenPop.Mime;

namespace MegaZord.Library.Helpers
{
    public static class MZHelperEmail
    {

        private static SmtpClient CreateSmtpSpamClient()
        {

            var client = new SmtpClient(MZHelperConfiguration.MZemail.MZSpamSend.MZServer,
                                        MZHelperConfiguration.MZemail.MZSpamSend.MZPort)
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials =
                    new System.Net.NetworkCredential(MZHelperConfiguration.MZemail.MZSpamSend.MZUserName,
                                                     MZHelperConfiguration.MZemail.MZSpamSend.MZPassword),
                EnableSsl = MZHelperConfiguration.MZemail.MZSpamSend.MZEnableSsl
            };


            return client;
        }
        private static SmtpClient CreateSmtpNormalClient()
        {

            var client = new SmtpClient(MZHelperConfiguration.MZemail.MZNormalSend.MZServer,
                                        MZHelperConfiguration.MZemail.MZNormalSend.MZPort)
            {
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials =
                        new System.Net.NetworkCredential(MZHelperConfiguration.MZemail.MZNormalSend.MZUserName,
                                                         MZHelperConfiguration.MZemail.MZNormalSend.MZPassword),
                EnableSsl = MZHelperConfiguration.MZemail.MZNormalSend.MZEnableSsl
            };


            return client;
        }

        private static PopClient CreatePopClient()
        {
            var client = new PopClient(MZHelperConfiguration.MZemail.MZReceive.MZServer, MZHelperConfiguration.MZemail.MZReceive.MZPort,
                                        MZHelperConfiguration.MZemail.MZReceive.MZEnableSsl, MZHelperConfiguration.MZemail.MZReceive.MZUserName,
                                        MZHelperConfiguration.MZemail.MZReceive.MZPassword);


            return client;
        }


        #region Stmp Methods

        private static void AddAttachmentsCollection(AttachmentCollection collection,
                                                     IEnumerable<Attachment> attachments)
        {
            foreach (var attachment in attachments)
            {
                collection.Add(attachment);
            }
        }

        private static void AddMailAddressCollection(MailAddressCollection collection, IEnumerable<string> emails)
        {

            foreach (var email in emails)
            {
                var splitedEmails = email.Split(';');
                foreach (var m in splitedEmails)
                {
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    Match match = regex.Match(m);
                    if (match.Success)
                    {
                        collection.Add(m);
                    }
                }
            }

        }

        private static MailMessage CreateMailMessage(EmailDTO email, bool sendBccDefault)
        {

            var body = email.MensagemMontada;
            var message = new MailMessage
            {
                Subject = email.Assunto,
                IsBodyHtml = true,
                Body = body,
                Priority = MailPriority.High,
                From = new MailAddress(email.EmailAutor, MZHelperConfiguration.MZemail.MZDefaultDisplayName)
            };




            AddMailAddressCollection(message.To, email.Dest);
            AddMailAddressCollection(message.CC, email.Cc);
            AddMailAddressCollection(message.Bcc, email.Bcc);
            AddAttachmentsCollection(message.Attachments, email.Anexos);

            if (sendBccDefault)
                AddMailAddressCollection(message.Bcc, MZHelperConfiguration.MZemail.MZDefaultReceiver.Split(';').ToList());
            return message;
        }

        public static void Send(EmailDTO email, bool sendBccDefault = false, bool sendUsingSpam = false)
        {
            InternalSend(email, sendBccDefault, sendUsingSpam);
        }

        public static void SendAsync(EmailDTO email, bool sendBccDefault = false, bool sendUsingSpam = false)
        {

            var ts = new ThreadStart(() => InternalSend(email, sendBccDefault, sendUsingSpam));
            var thread = new Thread(ts) { IsBackground = true };

            thread.Start();

        }


        private static void InternalSend(EmailDTO email, bool sendBccDefault, bool sendUsingSpam)
        {
            using (var smtp = sendUsingSpam ? CreateSmtpSpamClient() : CreateSmtpNormalClient())
            {
                using (var mailMessage = CreateMailMessage(email, sendBccDefault))
                {
                    try
                    {
                        smtp.Send(mailMessage);
                    }
                    catch (Exception e)
                    {
                        if (e.Message == string.Empty)
                            throw new Exception("Sem texto definido");

                    }
                }
            }

        }

        #endregion


        #region Pop3 Methods

        public static List<PopMessage> FetchMessages(int messagesCount = -1)
        {
            List<PopMessage> result = new List<PopMessage>();

            var pop3Client = CreatePopClient();

            if (messagesCount > 0)
            {
                result = pop3Client.FetchMessages(messagesCount);
            }
            else
            {
                result = pop3Client.FetchAllMessages();
            }

            return result;
        }

        public static bool DeleteInboxMessage(string messageId)
        {
            var pop3Client = CreatePopClient();

            return pop3Client.DeleteMail(messageId);
        }

        #endregion

    }
}
