namespace EnhanceClub.Domain.Abstract
{
    public interface IEmailSender
    {
        void SendEmail(string toEmail, string mailSubject, string mailBody, string mailFrom, int orderInvoiceFk);
        void GetInTouchSendEmail(string toEmail, string mailSubject, string mailBody, string mailFrom, int orderInvoiceFk);

    }
}
