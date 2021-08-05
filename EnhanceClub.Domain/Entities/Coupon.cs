using System;

namespace EnhanceClub.Domain.Entities
{
    public class Coupon

    {
        public int CouponId { get; set; }
        public string CouponCode { get; set; }
        public decimal CouponDiscountAmount { get; set; }
        public decimal CouponDiscountRate { get; set; }
        public int CouponMaxUse { get; set; }
        public decimal CouponMinPurchase { get; set; }
        public decimal CouponMaxDiscount { get; set; }
        public bool CouponActive { get; set; }
        public bool CouponFreeShip { get; set; }
        public DateTime? CouponExpiryDate { get; set; }
        public bool CouponNewCustomerOnly { get; set; }

        // these two fields are set if coupon is not valid
        public int CouponValid { get; set; }
        public string CouponMessage { get; set; }

        public bool CouponPromotionalFreeOrder { get; set; }
        // if an over ride exists 
        public void CouponOverride()
        {
            
        }
    }
}
