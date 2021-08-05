using System;
using System.Collections.Generic;
using System.Data;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.Domain.Abstract
{
    public interface IOrderProcessor
    {
        
        // process order will create the order with no payment 
        int ProcessOrderNoPayment(Cart cart, AffiliateInfo affiliateInfo, CustomerMinimal customerInfo, ShippingDetails shipDetails, StoreFrontInfo storeFrontInfo, ICustomerRepository repositoryCustomer, 
            IProductRepository repositoryProdduct, IEmailSender emailSender, PatientProfile patientInfo, int transactionTypeFk);
       

        // Validate coupon applied to order
        Coupon ValidateCoupon(string couponCode, int storeFrontId, DateTime thisDate );

        // check if coupon max used by customer
        int GetCouponUsageCountByCustomer(int customerId, int couponId, int storeFrontId);

        // check if coupon is allowed to customer
        bool IsCustomerAllowedCoupon(int customerId, int couponId, int storeFrontId);

        // get order detail used by order confirmation page
        OrderDetailMinimal GetOrderDetailMinimal(int orderId, int storefrontFk);

        // get cart details for orderInvoice
        List<CartDetail> GetOrderCart(int orderId, int storefrontId);

        // check if customer has placed any order
        bool IfCustomerHasOrders(int customerId);

        //Comment: Tax- get provincial tax details
        TaxDetails GetProvinceTaxDetails(int provinceStateId);

        // update order invoice to clear payment pending
        int UpdateOrderInvoicePaymentPending(string orderInvoiceToUpdate, bool paymentPending, DateTime dateUpdated);
        //Get Questionnaire based on Category
        List<Questionnaire> GetQuestionnaireByCategoryId(string categoryId, string sortBy);
        //Get Questionnaire based on Category
        List<QuestionnaireOption> GetQuestionOptionByQuestionnaireId(string QuestionnaireId, string sortBy);
          // add Question Response
        int AddQuestionnaireResponse(int customerId,
                                       int orderInvoiceFk,
                                       int QuestionnaireFk,
                                       int QuestionnaireCategoryFk,
            string answerText,
            string moreInfo,
                                       int QuestionnaireOptionFk, DateTime DateCreated
                                      );

        //Get Questionnaire Group based on Category
        List<QuestionnaireGroup> GetQuestionnaireGroupByCatId(string QuestionnaireCatId, string sortBy);
        // check if user has order of same amount in past 30 minutes
        bool IfUserAlreadyAnsweredQuestionnaire(int catId, int customerId, DateTime currentTimeStamp, int orderTimeSpan,int orderId);
        // add Question Response
        int AddCustomerQuestionnaireResponse(int customerId,
                                               int orderInvoiceFk,
                                               int categoryFk,
                                               int approve,
                                               DateTime dateCreated,
                                               DateTime dateModified,
                                                  bool? hardStop
                                                );

        List<int> GetProductQuestionnaireCategoryByOrderId(int orderInvoicefk);
        // add to order transaction
        int AddOrderTransaction(int customerId, int orderInvoiceId,
                                int orderTempId, int paymentOptionFk,
                                string cardCvv2Code, string cardName, string cardNum,
                                string cardType, string clientIp, string orderTransactionMsg,
                                string transactionNum, decimal orderTransactionAmount, int cardCvv2Type,
                                int cardExpireMonth, int cardExpireYear, int orderTransactionError,
                                DateTime dateCreated, int storeFrontFk, int paymentTransactionTypeFk
                                       );

        int AddCustomerSubscription(int customerFk, int orderInvoiceFk, int productFk, int productSizeFk,  decimal cartItemQuantity, bool autoFillActive, DateTime dateCreated);

        List<CustomerSubscription> GetCustomerSubscription(int customerFk, string productSizeFkList);

        RefillOrder GetOrderRefillDetails(int customerFk, string productSizeStrength, int productSizeId);
        // update Cart Refill
        int UpdateOrderCartRefillStatus( int productSizeId,
           int orderInvoiceId,
           bool refillStatus);
        List<QuestionOptionSelected> GetAnsweredQuestionnaireForCustomer(int QuestionnaireCatId, int customerFk, DateTime currentTimeStamp, int orderTimeSpan, int orderId);

        int LinkOrderTransactionToOrder(int orderTransactionId, int orderInvoiceId, string statusMessage, 
                            int transactionError, DateTime lastModified, string transactionnum, string transactionAuthCode, int transactionType);

        int AddLogOrderTransaction(int orderTransactionFk, int orderInvoiceFk, int paymentOptionFk,
            int paymentTransactionTypeFk, DateTime dateCreated);

        int AddCustomerSessionTracker(int customerFk, int orderInvoiceFk, string actionName, string message, int trackinStatus, DateTime dateCreated);

        int UpdateCustomerSessionTracker(int trackingId, string trackingMessage, int trackingStatus, DateTime dateUpdated);

        int UpdateOrderInvoiceBPStatus(int orderInvoiceId, bool BPNotKnown);

        List<BPQuestionnaires> GetBPQuestionnaires();

        int UpdateCustomerQuestionnaireCategoryResponse(int questionnaireResponseId, int customerFk, int questionnaireProblemFk, string problemAssignUserAdmin, 
             DateTime problemAssignDate, string assignDoctorComment);

        int UpdateBPQuestionnaireResponse(int customerFk, int orderInvoiceFk, int questionFk, int responseOptionFk);
         
        int AddLogPreScreenOrderBPUpdate(int customerFk, int orderInvoiceFk, int categoryResponseFk, string comment, string ipAddress, DateTime dateCreated);

        int AddEmailRecordForProcessing(int storeFrontFk, int sourceApplicationFk, string toEmail, string emailSubject, string emailBody, int orderInvoiceFk, 
            int customerFk, int nitroUser_Fk, int emailTypeFk, int interval, int maxEmailSentCount);

        int UpdateNitroEmailQueueStatus(int customerFk, int orderInvoiceFk);

        int AddCustomerQuestionnaireIssues(int customerFk, int orderInvoiceFk, int questionnaireFk, int deviceType, string exception, bool completeStatus, DateTime dateCreated);

        int UpdateCustomerQuestionnaireResponse(int customerFk, int orderInvoiceFk, int questionnaireFk, int deviceType, bool completeStatus, string operationType, DateTime dateCreated);

        RefillOrder GetCustomerActiveRefillOrders(int customerId, int productSizeFk, DateTime currentDate);

        int UpdateOrderInvoiceRefillStatus(bool refillStatus, int orderInvoiceFk);

        int AddLogRefillOrders(int orderInvoiceFk, int shipInvoiceFk, int refillsAllowed, int refillsAvailable,
                                        string refillMessage, string refillAction, 
                                        DateTime? autoRefillStartDate,  DateTime dateCreated, bool isAutoRefill);
        int UpdateLogRefillOrder(int logRefillOrderId, bool refillComplete, DateTime refillCompleteDate, int orderInvoiceFk);

        List<Questionnaire> GetRefillQuestionnaire();

        int AddRefillQuestionnaireResponse(int customerFk, int orderInvoiceFk, List<QuestionOptionSelected> answerList, DateTime dateCreated);
    }
}
