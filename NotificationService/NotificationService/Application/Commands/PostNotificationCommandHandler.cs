using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MimeKit;
using Newtonsoft.Json;
using NotificationService.Application.Queries;
using NotificationService.Model;
using FirebaseAdmin.Messaging;
using System.IO;

namespace NotificationService.Application.Commands
{
    public class PostNotificationCommandHandler : IRequestHandler<PostNotificationCommand, CommandReturnData>
    {
        private NotificationContext _context;

        public PostNotificationCommandHandler(NotificationContext context)
        {
            _context = context;
        }

        public async Task<CommandReturnData> Handle(PostNotificationCommand request, CancellationToken cancellationToken)
        {
            //Adding Notification to Db
            var notificationList = _context.Notifications.ToList();
            var userTokens = new List<string>
            {
                "token1", "token2", "token3", "token4"
            };

            var notificationData = new Model.Notification()
            {
                Message = request.Data.Attributes.Message,
                Title = request.Data.Attributes.Title
            };

            if (!notificationList.Any(x => x.Title == request.Data.Attributes.Title))
            {
                _context.Notifications.Add(notificationData);
            }
            await _context.SaveChangesAsync();

            //Adding Notification_log to Db
            var theNotification = _context.Notifications.First(x => x.Title == request.Data.Attributes.Title);
            foreach (var x in request.Data.Attributes.Targets)
            {
                _context.Notification_logs.Add(new NotificationLogs
                {
                    Notification_id = theNotification.Id,
                    Type = request.Data.Attributes.Type,
                    From = request.Data.Attributes.From,
                    Target = x.Id,
                    Email_destination = x.Email_destination
                });

                await _context.SaveChangesAsync();
                if (x.Email_destination != null)
                {
                    await SendMail("admin@admin.com", x.Email_destination, request.Data.Attributes.Title, request.Data.Attributes.Message);
                }
                try
                {
                    await SendNotification(userTokens, request.Data.Attributes.Title, request.Data.Attributes.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }

            await _context.SaveChangesAsync();

            return new CommandReturnData()
            {
                Message = "Successfully Added",
                Success = true
            };
        }

        //Method for sending notification email
        public async Task SendMail(string emailfrom, string emailto, string subject, string body)
        {
            var client = new SmtpClient("smtp.mailtrap.io", 2525)
            {
                Credentials = new NetworkCredential("4101aedaf3b46c", "e05b5c377ba6d8"),
                EnableSsl = true
            };
            await client.SendMailAsync(emailfrom, emailto, subject, body);
            Console.WriteLine("Email has been sent");
        }

        //Method for sending notification
        public async Task SendNotification(List<string> tokens, string title, string body)
        {
            var registrationTokens = new List<string>();
            foreach(var x in tokens)
            {
                registrationTokens.Add(x);
            }

            var message = new MulticastMessage()
            {
                Tokens = registrationTokens,
                Data = new Dictionary<string, string>()
                {
                    { "Title", title },
                    { "Message", body },
                },
            };

            var response = await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
            Console.WriteLine($"{response.SuccessCount} messages were sent successfully");
        }

        //Method for posting HTTP
        public string AddOrder(string url, string stringOrder, string bigApiUserID, string BigApiKey)
        {

            try
            {
                string responseStr = "";
                HttpStatusCode statusCode;
                WebRequest request = WebRequest.Create(url);
                request.Credentials = new NetworkCredential(bigApiUserID, BigApiKey);
                request.Method = "POST";
                byte[] bytes;
                bytes = System.Text.Encoding.ASCII.GetBytes(stringOrder);

                request.Headers.Add("SecurityToken", BigApiKey);
                request.ContentType = "application/json";

                request.ContentLength = bytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bytes, 0, bytes.Length);
                requestStream.Close();
                HttpWebResponse response;
                response = (HttpWebResponse)request.GetResponse();
                statusCode = response.StatusCode;
                if (Convert.ToString(response.StatusCode) == "OK")
                {
                    Stream responseStream = response.GetResponseStream();
                    responseStr = new StreamReader(responseStream).ReadToEnd();
                    return responseStr;
                }
                return responseStr;
            }
            catch (WebException e)
            {

                string responseStr = "";
                using (WebResponse response = e.Response)
                {
                    HttpWebResponse httpResponse = (HttpWebResponse)response;
                    responseStr = httpResponse.StatusCode.ToString();
                    using (Stream data = response.GetResponseStream())
                    {
                        responseStr = new StreamReader(data).ReadToEnd();
                    }
                }
                return responseStr;
            }
        }
    }
}
