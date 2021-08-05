using System;
using System.Net;
using System.Net.Mail;
using System.Web;
using EnhanceClub.Domain.Abstract;

namespace EnhanceClub.Domain.Concrete
{
    // This class is used for all logging activity
    public class GlobalFunctions
    {
        
        public string LogController { get; set; }
        public string LogAction { get; set; }
        public string LogIp { get; set; }
        public DateTime LogDateTime { get; set; }
        public bool LogSessionTimeout { get; set; }

        private readonly IAdminRepository _repositoryAdmin = new AdminRepositorySql();

        // Add to Action Log table
        public int AddActionLog()
        {
            int actionLogId = 0;
            
            actionLogId = _repositoryAdmin.AddActionLog(LogController,LogAction,LogIp,LogDateTime,LogSessionTimeout);

            return actionLogId;
        }

        // Add to Action Log table
        public int AddLogUnexpected(string message, string module, string relevantInfo, int customerId, int storefrontId, int productsizeId, int productId)
        {
            int actionLogId = 0;

            actionLogId = _repositoryAdmin.AddLogUnexpected(message, module, relevantInfo,customerId, storefrontId, productsizeId, productId);

            return actionLogId;
        }
 
        // send email for debugging  
        public void SendDebugEmail(string msgBody, string subject)
        {
            EmailSettings mailSetting =  new EmailSettings();

            string mailBody = msgBody;
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = mailSetting.NotifyUseSsl;
                smtpClient.Host = mailSetting.NotifyServerIp;
                smtpClient.Port = mailSetting.NotifyServerPort;
                smtpClient.UseDefaultCredentials = false;

                smtpClient.Credentials = new NetworkCredential(mailSetting.NotifyUserName, mailSetting.NotifyPassword);

                if (HttpContext.Current.Request.UserHostAddress != null)
                {
                    subject = subject + " ( " + HttpContext.Current.Request.UserHostAddress + " )";
                }

                MailMessage mailMessage = new MailMessage("alerts@notify.EnhanceClub.com",
                                                           mailSetting.DebugMessageRecipient,
                                                           subject, 
                                                           mailBody.ToString()
                                                          );

                mailMessage.IsBodyHtml = true;

                //smtpClient.Send(mailMessage);

            }
        }

       // check if ip is canadian ip
        public bool IsCanadianIp(long ipNum)
        {
           return _repositoryAdmin.IsCanadianIp(ipNum);
        }
        
    }
}
