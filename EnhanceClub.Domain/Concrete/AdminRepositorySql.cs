using System;
using EnhanceClub.Domain.Abstract;

namespace EnhanceClub.Domain.Concrete
{
    // All Administrative database task handled by this repository
    public class AdminRepositorySql : IAdminRepository
    {
        private readonly AdminDbLayer _adminDbl = new AdminDbLayer();
       
        // Add to Action Log
        public int AddActionLog(string logController, string logAction, string logIp, DateTime logDateTime, bool logSessionTimeout)
        {
            
            int newActionLogId = _adminDbl.AddActionLog(logController,logAction,logIp,logDateTime,logSessionTimeout);
            return newActionLogId;
        
        }
        
        public int AddLogUnexpected(string message, string module, string relevantInfo, int customerId, int storefrontId, int productsizeId, int productId)
        {
            int newActionLogId = _adminDbl.AddUnexpectedLog(message, module, relevantInfo, customerId, storefrontId, productsizeId, productId);
            return newActionLogId;
        }

        // check if ip is canadian IP
        public bool IsCanadianIp(long ipNum)
        {
            return _adminDbl.IsCanadianIp(ipNum);
        }

        // records exception when sending email
        public int AddLogEmailException(string toEmail,
            string mailSubject,
            string mailFrom,
            string exMessage,
            string serverIp,
            string serverPort,
            int storeFrontId,
            int customerFk,
            int orderInvoiceFk,
            DateTime dateCreated)
        {
            int newLogId = _adminDbl.AddLogEmailException(toEmail, mailSubject, mailFrom, exMessage, serverIp, serverPort, storeFrontId, customerFk, orderInvoiceFk, dateCreated);
            return newLogId;
        }

        public int AddLogSendEmailReminder(int userAdminFk, int storefrontFk, int customerFk, int orderInvoiceFk,
                                    string customerEmail, string customerPhone, DateTime dateCreated,
                                    int emailType, string emailSubject, string emailContent, int applicationFk)
        {
            var updateStatus = _adminDbl.AddLogSendEmailReminder(userAdminFk, storefrontFk, customerFk, orderInvoiceFk,
                            customerEmail, customerPhone, dateCreated, emailType, emailSubject, emailContent, applicationFk);
            return updateStatus;
        }
    }

}
