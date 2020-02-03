using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace BC.Integration.Utility.NotificationSvc
{
    public class Email
    {
        private string emailBody;

        public string EmailBody
        {
            get { return emailBody; }
            set { emailBody = value; }
        }

        private void SendHtmlFormattedEmail(string recepient, string ccRecepient, string subject, string body)
        {
            try
            {
                using (MailMessage mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["UserName"]);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    string[] toEmails = recepient.Split(';');
                    foreach (string email in toEmails)
                    { mailMessage.To.Add(new MailAddress(email.Trim())); }

                    string[] CcEmails = ccRecepient.Split(';');
                    foreach (string email in CcEmails)
                    { mailMessage.CC.Add(email.Trim()); }

                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = ConfigurationManager.AppSettings["Host"];
                    smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                    System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                    NetworkCred.UserName = ConfigurationManager.AppSettings["UserName"];
                    NetworkCred.Password = ConfigurationManager.AppSettings["Password"];
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = NetworkCred;
                    smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                    smtp.Send(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in SendHtmlFormattedEmail() method while sending email via smtp host.", ex);
            }
        }

        private string PopulateEmailBody(List<string> issueMessages, string bodyTemplate)
        {
            try
            {
                emailBody = string.Empty;

                using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(bodyTemplate)))
                {
                    emailBody = reader.ReadToEnd();
                }
                emailBody = emailBody.Replace("{CustomerName}", "NRI");
                string a = "";
                foreach (var issueMessage in issueMessages)
                {
                    a += "<p>&#9658;  <strong >" + issueMessage.Replace("<", "&#60;").Replace(">", "&#62;").Replace("\r\n", "<br />") + "</ strong ></p>";
                }
                emailBody = emailBody.Replace("{IssueMessage}", a);
                                
                return emailBody;
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in PopulateEmailBody() method while creating email body.", ex);
            }
        }

        private string PopulateEmailBody(string alternateBodyTemplate)
        {
            try
            {
                emailBody = string.Empty;
                using (StreamReader reader = new StreamReader(System.Web.HttpContext.Current.Server.MapPath(alternateBodyTemplate)))
                {
                    emailBody = reader.ReadToEnd();
                }

                return emailBody;
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in PopulateEmailBody() method while creating alternate email body.", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receipient"></param>
        /// <param name="ccReceipient"></param>
        /// <param name="emailSubject"></param>
        /// <param name="issueMessage"></param>
        /// <param name="bodyTemplate"></param>
        /// <param name="alternateBodyTemplate"></param>
        public string SendEmail(string receipient, string ccReceipient, string emailSubject, List<string> issueMessage, string bodyTemplate)
        {
            try
            {
                string body = this.PopulateEmailBody(issueMessage, bodyTemplate);

                this.SendHtmlFormattedEmail(receipient, ccReceipient, emailSubject, body);
                return body;
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in SendEmail() method while trying to send email notifications.", ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="receipient"></param>
        /// <param name="ccReceipient"></param>
        /// <param name="emailSubject"></param>
        /// <param name="issueMessage"></param>
        /// <param name="bodyTemplate"></param>
        /// <param name="alternateBodyTemplate"></param>
        public void SendEmail(string receipient, string ccReceipient, string emailSubject, string alternateBodyTemplate)
        {
            try
            {
                string body = this.PopulateEmailBody(alternateBodyTemplate);
                this.SendHtmlFormattedEmail(receipient, ccReceipient, "NRI - Herschel | Order Processing Success", body);
            }
            catch (Exception ex)
            {
                throw new Exception("An exception occured in SendEmail() method while trying to send email notifications.", ex);
            }
        }
    }


}