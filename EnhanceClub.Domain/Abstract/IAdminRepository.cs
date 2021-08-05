using System;

namespace EnhanceClub.Domain.Abstract
{
    public interface IAdminRepository
    {
        // to Add to Action Log
        int AddActionLog(string logController, string logAction, string logIp, DateTime logDateTime, bool logSessionTimeout);

        // to add to log if something unexpected happens
        int AddLogUnexpected(string message, string module, string relevantInfo, int customerId, int storefrontId, int productsizeId, int productId);

        bool IsCanadianIp(long ipnum);

        // records exception when sending email
        int AddLogEmailException(string toEmail,
                        string mailSubject,
                        string mailFrom,
                        string exMessage,
                        string serverIp,
                        string serverPort,
                        int storeFrontId,
                        int customerFk,
                        int orderInvoiceFk,
                        DateTime dateCreated);

        int AddLogSendEmailReminder(int pharmacyFk, int storefrontFk, int customerFk, int orderInvoiceFk,
                                      string customerEmail, string customerPhone, DateTime dateCreated, int emailType,
                                      string emailSubject, string emailContent, int applicationFk);
    }
}
