using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;
using log4net;
using URL_Parser.Properties;
using WebGrease.Activities;

namespace URL_Parser.Configuration
{
    public class ErrorNotifier
    {
        public static void EmailError(string error)
        {
            if (string.IsNullOrWhiteSpace(error)) return;
            try
            {
                var mail = new MailMessage {From = new MailAddress(Settings.Default.NoReplyEmail)};
                mail.To.Add(Settings.Default.SupportEmail);
                mail.Subject = "[URL Parser] Unhandled exception in application.";
                mail.IsBodyHtml = true;
                mail.Body = error;
                var smtpServer = new SmtpClient("smtp.gmail.com")
                {
                    Port = Settings.Default.SMTPPort,
                    UseDefaultCredentials = false,
                    Credentials =
                        new System.Net.NetworkCredential(Settings.Default.NoReplyEmail, Settings.Default.NoReplyPassword),
                    EnableSsl = true
                };
                smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                var logger = LogManager.GetLogger(typeof(MvcApplication));
                logger.Fatal("Unable to sent unhandled exception notification email.", ex);
            }
        }
    }
}