using System;
namespace EnhanceClub.Domain.Entities
{
    // This class is used for passing cart details  
    public class CartDetail
    {
        public int CartShippingInvoiceFk { get; set; }
        public int CartItemQuantity { get; set; }
        public decimal CartItemPrice { get; set; }

        public decimal CartLineTotal
        {
            get { return CartItemQuantity*CartItemPrice; }
        }
        public string ProductName { get; set; }
        public string ProductSizeHeader { get; set; }
        public string ProductSizeStrength { get; set; }
        public decimal ProductSizeQuantity { get; set; }

        // these properties are used by order confirm page
        public int ProductSizeId { get; set; }
        public string ProductSafeUrlName { get; set; }
        public decimal ProductSizeStoreFrontPrice { get; set; }
        public int ProductTypeFk { get; set; }

        public int ProductId { get; set; }
        public int ProductQuestionnaireCatId { get; set; }
        public string ProductUnitNamePk { get; set; }
        public bool ProductSizeGeneric { get; set; }

        public bool IsQuestionnaireAnswered { get; set; }

        public QuestionnaireCategoryResponse QuestionnaireCategoryResponse { get; set; }
        public int CustomerQuestionnaireCategoryResponseId { get; set; }        
        public bool CustomerQuestionnaireCategoryResponseApprove {get; set;}
        public DateTime? CustomerQuestionnaireCategoryResponseApproveDate { get; set; }

        public bool IsRefillCartItem { get; set; }

        public int OriginalRefillOrderShippIngInvoiceFk { get; set; }

        public int NumberOfRefillsAllowed { get; set; }

        public int NumberOfRefillsAvailable { get; set; }

        public int RefillOrdersPlaced { get; set; }

        public int OrderInvoiceFk { get; set; }

        public bool CartItemShipped { get; set; }
    }
}
