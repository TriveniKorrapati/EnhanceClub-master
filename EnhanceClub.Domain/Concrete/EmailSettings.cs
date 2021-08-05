using System;
using System.Configuration;

namespace EnhanceClub.Domain.Concrete
{
    // this class is used to configure email settings
    public class EmailSettings
    {
       
        public string MailFromAddress = ConfigurationManager.AppSettings["SignupEmailFrom"].ToString();
        public bool UseSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["UseSsl"].ToString());
        public string UserName = ConfigurationManager.AppSettings["MailServerUserName"].ToString();
        public string Password = ConfigurationManager.AppSettings["MailServerPassword"].ToString();
        public string ServerIp = ConfigurationManager.AppSettings["MailServerIP"].ToString();
        public int ServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["MailServerPort"].ToString());
        public bool WriteAsFile = Convert.ToBoolean(ConfigurationManager.AppSettings["WriteEmailAsFile"].ToString());
        public string FileLocation = ConfigurationManager.AppSettings["FileLocation"].ToString();
        public string DebugMessageRecipient = ConfigurationManager.AppSettings["DebugMessageRecipient"].ToString();
        public string EmailFromDisplayName = ConfigurationManager.AppSettings["EmailFromDisplayName"].ToString();

        // mail server setting for error and other notification emails
        public string NotifyUserName = ConfigurationManager.AppSettings["NotifyMailServerUserName"].ToString();
        public string NotifyPassword = ConfigurationManager.AppSettings["NotifyMailServerPassword"].ToString();
        public string NotifyServerIp = ConfigurationManager.AppSettings["NotifyMailServerIP"].ToString();
        public int NotifyServerPort = Convert.ToInt32(ConfigurationManager.AppSettings["NotifyMailServerPort"].ToString());
        public bool NotifyUseSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["NotifyUseSsl"].ToString());
        public bool NotifyWriteAsFile = Convert.ToBoolean(ConfigurationManager.AppSettings["NotifyWriteEmailAsFile"].ToString());
      
        //settings for mail from get in touch
        public string GetInTouchUserName = ConfigurationManager.AppSettings["GetInTouchMailServerUserName"].ToString();
        public string GetInTouchPassword = ConfigurationManager.AppSettings["GetInTouchMailServerPassword"].ToString();
        public string GetInTouchMailFromAddress = ConfigurationManager.AppSettings["GetInTouchEmailFrom"].ToString();
    }
}
