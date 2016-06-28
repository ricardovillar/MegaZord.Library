using MegaZord.Library.Helpers;
using OpenPop.Mime;
using OpenPop.Pop3;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MegaZord.Library.Mail
{
    public class PopClient
    {
        string _hostname;
        int _port;
        bool _useSsl;
        string _username;
        string _password;

        public PopClient(string hostname, int port, bool useSsl, string username, string password)
        {
            _hostname = hostname;
            _port = port;
            _useSsl = useSsl;
            _username = username;
            _password = password;
        }

        public PopClient()
            : this(MZHelperConfiguration.MZemail.MZReceive.MZServer,
            MZHelperConfiguration.MZemail.MZReceive.MZPort,
            MZHelperConfiguration.MZemail.MZReceive.MZEnableSsl,
            MZHelperConfiguration.MZemail.MZReceive.MZUserName,
            MZHelperConfiguration.MZemail.MZReceive.MZPassword)
        {

        }


        public List<PopMessage> FetchMessages(int count)
        {
            List<PopMessage> result = new List<PopMessage>();

            for (int i = 1; i <= count; i++)
            {
                // The client disconnects from the server when being disposed
                using (Pop3Client client = new Pop3Client())
                {
                    try
                    {
                        // Connect to the server
                        client.Connect(_hostname, _port, _useSsl);

                        // Authenticate ourselves towards the server
                        client.Authenticate(_username, _password, AuthenticationMethod.UsernameAndPassword);

                        result.Add(new PopMessage(client.GetMessage(i)));

                    }
                    catch
                    {

                    }
                    finally
                    {
                        client.Dispose();
                    }
                }
            }

            return result;
        }

        private List<PopMessage> FetchMessages(Pop3Client client, int messageCount)
        {
            // We want to download all messages
            List<PopMessage> allMessages = new List<PopMessage>(messageCount);

            // Messages are numbered in the interval: [1, messageCount]
            // Ergo: message numbers are 1-based.
            // Most servers give the latest message the highest number
            for (int i = messageCount; i > 0; i--)
            {
                allMessages.Add(new PopMessage(client.GetMessage(i)));
            }

            // Now return the fetched messages
            return allMessages;

        }

        public List<PopMessage> FetchAllMessages()
        {
            List<PopMessage> result = new List<PopMessage>();

            // The client disconnects from the server when being disposed
            using (Pop3Client client = new Pop3Client())
            {
                // Connect to the server
                client.Connect(_hostname, _port, _useSsl);

                // Authenticate ourselves towards the server
                client.Authenticate(_username, _password, AuthenticationMethod.UsernameAndPassword);

                // Get the number of messages in the inbox
                int messageCount = client.GetMessageCount();

                // Now return the fetched messages
                result = FetchMessages(client, messageCount);

            }

            return result;
        }

        public bool DeleteMail(string messageId)
        {
            var result = false;

            // The client disconnects from the server when being disposed
            using (Pop3Client client = new Pop3Client())
            {
                // Connect to the server
                client.Connect(_hostname, _port, _useSsl);

                // Authenticate ourselves towards the server
                client.Authenticate(_username, _password, AuthenticationMethod.UsernameAndPassword);

                // Get the number of messages on the POP3 server
                int messageCount = client.GetMessageCount();

                // Run trough each of these messages and download the headers
                for (int messageItem = 1; messageItem <= messageCount; messageItem++)
                {
                    // If the Message ID of the current message is the same as the parameter given, delete that message
                    if (client.GetMessageHeaders(messageItem).MessageId == messageId)
                    {
                        // Delete
                        client.DeleteMessage(messageItem);
                        result = true;
                        break;
                    }
                }

                client.Disconnect();
            }

            return result;
        }

    }
}
