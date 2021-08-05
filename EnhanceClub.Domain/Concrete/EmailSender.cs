using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Helpers;

namespace EnhanceClub.Domain.Concrete
{
    // Used to Send Emails
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;
        private readonly IAdminRepository _repositoryAdmin;

        // constructs gets email settings from Ninject
        public EmailSender(EmailSettings settings,
                IAdminRepository repositoryAdmin)
        {
            _emailSettings = settings;
            _repositoryAdmin = repositoryAdmin;
        }
        // send email
        // if mail from is specified 
        public void SendEmail(string toEmail, string mailSubject, string mailBody, string mailFrom = "", int orderInvoiceFk = 0)
        {
            using (var smtpClient = new SmtpClient())
            {

                if (String.IsNullOrEmpty(mailFrom))
                {
                    mailFrom = _emailSettings.MailFromAddress;
                }
                smtpClient.EnableSsl = _emailSettings.UseSsl;
                smtpClient.Host = _emailSettings.ServerIp;
                smtpClient.Port = _emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;

                smtpClient.Credentials = new NetworkCredential(_emailSettings.UserName, _emailSettings.Password);

                if (_emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = _emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                var mailFromName = new MailAddress(mailFrom, _emailSettings.EmailFromDisplayName).ToString();

                MailMessage mailMessage = new MailMessage( mailFromName, toEmail, mailSubject, mailBody.ToString());
                mailMessage.IsBodyHtml = true;
                if (_emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                try
                {
                    smtpClient.Send(mailMessage);
                }
                // if sending and receiving server are same, it will right way tell if non existent email is used and throw this error
                // in this case we ignore it as it will happen in our test accounts only
                catch (SmtpFailedRecipientException ex)
                {
                    // add email exception log in case of failure
                    var logId = _repositoryAdmin.AddLogEmailException(toEmail, mailSubject, mailFrom, ex.Message, _emailSettings.ServerIp, _emailSettings.ServerPort.ToString(), SiteConfigurations.StoreFrontId, 0, orderInvoiceFk, DateTime.Now);

                }
                catch (Exception ex)
                {
                    // add email exception log in case of failure
                    var logId = _repositoryAdmin.AddLogEmailException(toEmail, mailSubject, mailFrom, ex.Message, _emailSettings.ServerIp, _emailSettings.ServerPort.ToString(), SiteConfigurations.StoreFrontId, 0, orderInvoiceFk, DateTime.Now);

                }

            }
        }

        public void GetInTouchSendEmail(string toEmail, string mailSubject, string mailBody, string mailFrom = "", int orderInvoiceFk = 0)
        {
            using (var smtpClient = new SmtpClient())
            {

                if (String.IsNullOrEmpty(mailFrom))
                {
                    mailFrom = _emailSettings.GetInTouchMailFromAddress;
                }
                smtpClient.EnableSsl = _emailSettings.UseSsl;
                smtpClient.Host = _emailSettings.ServerIp;
                smtpClient.Port = _emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;

                smtpClient.Credentials = new NetworkCredential(_emailSettings.GetInTouchUserName, _emailSettings.GetInTouchPassword);

                if (_emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = _emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }

                var mailFromName = new MailAddress(mailFrom, _emailSettings.EmailFromDisplayName).ToString();

                MailMessage mailMessage = new MailMessage(mailFromName, toEmail, mailSubject, mailBody.ToString());
                mailMessage.IsBodyHtml = true;
                if (_emailSettings.WriteAsFile)
                {
                    mailMessage.BodyEncoding = Encoding.ASCII;
                }

                try
                {
                    smtpClient.Send(mailMessage);
                }
                // if sending and receiving server are same, it will right way tell if non existent email is used and throw this error
                // in this case we ignore it as it will happen in our test accounts only
                catch (SmtpFailedRecipientException ex)
                {
                    // add email exception log in case of failure
                    var logId = _repositoryAdmin.AddLogEmailException(toEmail, mailSubject, mailFrom, ex.Message, _emailSettings.ServerIp, _emailSettings.ServerPort.ToString(), SiteConfigurations.StoreFrontId, 0, orderInvoiceFk, DateTime.Now);

                }
                catch (Exception ex)
                {
                    // add email exception log in case of failure
                    var logId = _repositoryAdmin.AddLogEmailException(toEmail, mailSubject, mailFrom, ex.Message, _emailSettings.ServerIp, _emailSettings.ServerPort.ToString(), SiteConfigurations.StoreFrontId, 0, orderInvoiceFk, DateTime.Now);

                }

            }
        }

    }
}
