using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace DVLD_EmailService
{
    public class EmailService : IEmailService
    {
        public bool SendEmail(
            string toEmail,
            string subject,
            string body)
        {
            try
            {
                string fromEmail = ConfigurationManager.AppSettings["EmailFrom"];
                string appPassword = ConfigurationManager.AppSettings["EmailAppPassword"];
                string displayName = ConfigurationManager.AppSettings["EmailDisplayName"];

                if (string.IsNullOrWhiteSpace(fromEmail) ||
                    string.IsNullOrWhiteSpace(appPassword))
                {
                    return false;
                }

                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(fromEmail, displayName);
                    mail.To.Add(toEmail);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = false;

                    using (SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587))
                    {
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential(
                            fromEmail,
                            appPassword);

                        smtp.Send(mail);
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}