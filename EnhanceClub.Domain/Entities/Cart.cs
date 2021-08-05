using System;
using System.Collections.Generic;
using System.Linq;
using EnhanceClub.Domain.Abstract;

namespace EnhanceClub.Domain.Entities
{
    public class Cart
    {
        private readonly List<CartItem> _cartItemCollection = new List<CartItem>();

        public IEnumerable<CartItem> CartItems
        {
            get { return _cartItemCollection; }
        }

        // Coupon ID
        public int CouponId { get; set; }

        // coupon code
        public string CouponCode { get; set; }

        // coupon generic message valid or invalid
        public string CouponValidMessage { get; set; }

        // coupon message based on different validation criteria
        public string CouponValidMessageActual { get; set; }

        // value of coupon applied
        public decimal CouponAmount { get; set; }

        // coupon offers free ship
        public bool CouponFreeShip { get; set; }


        //coupon offers promotional free order
        public bool CouponPromotionalFreeOrder { get; set; }

        // credit applied
        public decimal CreditApplied { get; set; }

        public bool CreditAppliedFlag { get; set; }

        // Shipping price
        public decimal ShippingPrice { get; set; }

        // Shipping option Id 
        public int ShippingOptionId { get; set; }

        // Payment option Id 
        public int PaymentOptionId { get; set; }

        // Add Item to Cart
        public void AddCartItem(ProductCart productCart, int quantity, int cartItemOrder)
        {
            CartItem cartItem = _cartItemCollection
                .FirstOrDefault(p => p.ProductCart.ProductSizeId == productCart.ProductSizeId);

            //CartItem cartItem = _cartItemCollection
            //  .Where(p => p.Product.ProductId == product.ProductId)
            //  .FirstOrDefault();

            if (cartItem == null)
            {
                _cartItemCollection.Add(new CartItem
                {
                    ProductCart = productCart,
                    Quantity = quantity,
                    CartItemOrder = cartItemOrder
                });
            }
            else
            {
                cartItem.Quantity += quantity;
            }
        }

        // Remove Item from Cart
        public void RemoveCartItem(ProductCart productCart)
        {
            _cartItemCollection.RemoveAll(l => l.ProductCart.ProductSizeId == productCart.ProductSizeId);
            if (_cartItemCollection.Count == 0)
                Clear();
        }

        // Compute Cart Total
        public decimal ComputeCartTotalValue()
        {
            return _cartItemCollection.Sum(e => e.ProductCart.ProductSizeStoreFrontPrice * e.Quantity);
        }

        // Empty Cart

        public void Clear()
        {
            _cartItemCollection.Clear();
            CouponId = 0;
            CouponCode = null;
            CouponAmount = 0.0m;
            CouponId = 0;
            CouponFreeShip = false;
            CouponValidMessage = null;
            CouponValidMessageActual = null;
            ShippingPrice = 0;
            ShippingOptionId = 0;
            PaymentOptionId = 0;
            CreditApplied = 0;
            CreditAppliedFlag = false;
            IsAutoRefillOrder = false;
        }

        // check if cart has prescription products
        public bool CartHasRxProducts()
        {
            var cartRxCount = _cartItemCollection.Count(p => p.ProductCart.ProductTypeFk == 2);

            if (cartRxCount == 0)
            {
                return false;

            }
            return true;
        }

        // Cart Net Total
        public decimal CartNetTotal
        {
            //get { return ComputeCartTotalValue() - CouponAmount - CreditApplied + ShippingPrice + CartProvincialTaxAmount + CartGlobalTaxAmount + CartHSTTaxAmount; }

            get
            {
                var creditApplied = CreditApplied > (ComputeCartTotalValue() - CouponAmount) ? (ComputeCartTotalValue() - CouponAmount) : CreditApplied;
                return ComputeCartTotalValue() - CouponAmount - creditApplied + ShippingPrice + CartProvincialTaxAmount + CartGlobalTaxAmount + CartHSTTaxAmount;
            }
        }

        // Cart Total before Credit, used to checkout step 3 to display maximum credit
        public decimal CartNetTotalBeforeCredit
        {
            get { return ComputeCartTotalValue() - CouponAmount + ShippingPrice; }
        }

        // Comment: Tax - Get Cart Otc Products Amount to calculate province and global tax
        public decimal CartOtcProductsAmount
        {
            get
            {
                var otcProductAmount = _cartItemCollection.Where(x => x.ProductCart.ProductTypeFk == 1).Sum(x => x.ProductCart.ProductSizeStoreFrontPrice * x.Quantity);
                return otcProductAmount;
            }
        }

        //Comment: Tax- start: properties to calculate provincial and global tax
        public decimal ProvincialTaxPercentage { get; set; }

        public decimal GlobalTaxPercentage { get; set; }

        public decimal HSTPercentage { get; set; }

        public decimal CartProvincialTaxAmount
        {
            get
            {
                return CartOtcProductsAmount * ProvincialTaxPercentage / 100;
            }
        }

        public decimal CartGlobalTaxAmount
        {
            get
            {
                return CartOtcProductsAmount * GlobalTaxPercentage / 100;
            }
        }

        public decimal CartHSTTaxAmount
        {
            get
            {
                return CartOtcProductsAmount * HSTPercentage / 100;
            }
        }

        //Comment: Tax- end: properties to calculate provincial and global tax

        // recalculate coupon applied to cart
        public void CalculateCoupon(LoggedCustomer loggedCustomer, AffiliateInfo affiliateInfo, IOrderProcessor repoOrder, Cart cart)
        {
            Coupon couponApplied = null;

            couponApplied = repoOrder.ValidateCoupon(CouponCode,
                affiliateInfo.AffiliateStoreFrontFk,
                DateTime.Now);
            if (couponApplied != null)
            {



                // check for expired coupon
                if (couponApplied.CouponExpiryDate != null && couponApplied.CouponExpiryDate < DateTime.Now)
                {
                    CouponValidMessage = "Coupon Expired";
                    CouponValidMessageActual = "Coupon Expired";
                    CouponAmount = 0;
                    return;
                }


                if (couponApplied.CouponNewCustomerOnly)
                {
                    // check if customer placed order
                    bool customerHasOrders = repoOrder.IfCustomerHasOrders(loggedCustomer.CustomerId);
                    if (customerHasOrders)
                    {
                        CouponValidMessage = "Coupon Valid Only for New Customers";
                        CouponValidMessageActual = "Coupon Valid Only for New Customers";
                        CouponAmount = 0;
                        return;
                    }
                }

                // check for minimum purchase criteria
                if (ComputeCartTotalValue() <= couponApplied.CouponMinPurchase)
                {
                    CouponValidMessage = "Invalid Coupon";
                    CouponValidMessageActual = "Minimum purchase of " + couponApplied.CouponMinPurchase.ToString("c") + " not reached";
                    CouponAmount = 0;
                }
                else
                {
                    // check for coupon max use
                    int couponUsedCount =
                        repoOrder.GetCouponUsageCountByCustomer(loggedCustomer.CustomerId, couponApplied.CouponId, affiliateInfo.AffiliateStoreFrontFk);
                    if (couponUsedCount >= couponApplied.CouponMaxUse)
                    {
                        CouponValidMessage = "Invalid Coupon";
                        CouponValidMessageActual = "Coupon max usage reached";
                        CouponAmount = 0;
                    }
                    else
                    {
                        // check if coupon is allowed to customer 
                        bool isCustomerAllowedCoupon = repoOrder.IsCustomerAllowedCoupon(loggedCustomer.CustomerId,
                            couponApplied.CouponId,
                            affiliateInfo.AffiliateStoreFrontFk);
                        if (!isCustomerAllowedCoupon)
                        {
                            CouponValidMessage = "Invalid Coupon";
                            CouponValidMessageActual = "Coupon not applicable to Customer";
                            CouponAmount = 0;
                        }
                        // check for promotional free offer coupon
                        else if (couponApplied.CouponPromotionalFreeOrder)
                        {
                            CouponPromotionalFreeOrder = couponApplied.CouponPromotionalFreeOrder;
                            CouponAmount = ComputeCartTotalValue();
                            CouponId = couponApplied.CouponId;
                            CouponValidMessage = "Coupon Applied Successfully";
                            CouponValidMessageActual = "Coupon Applied Successfully";
                        }
                        else
                        {
                            switch (CouponCode.ToUpper())
                            {

                                case "30DAYPROMO":
                                    // set coupon amount to lowest rx product 
                                    var lowestRx =
                                        CartItems.Where(p => p.ProductCart.ProductTypeFk == 2)
                                            .OrderBy(m => m.ProductCart.ProductSizeStoreFrontPrice)
                                            .FirstOrDefault();
                                    if (lowestRx != null)
                                    {

                                        CouponAmount = lowestRx.ProductCart.ProductSizeStoreFrontPrice;
                                        if (CouponAmount > couponApplied.CouponMaxDiscount)
                                        {
                                            CouponAmount = Math.Round(couponApplied.CouponMaxDiscount, 2);

                                        }

                                        CouponValidMessage = "Coupon Applied Successfully";
                                        CouponValidMessageActual = "Coupon Applied Successfully";
                                        CouponId = couponApplied.CouponId;
                                    }
                                    break;

                                default:
                                    decimal couponAmt = Math.Max(couponApplied.CouponDiscountAmount,
                                        ComputeCartTotalValue() * couponApplied.CouponDiscountRate);

                                    if (couponApplied.CouponMaxDiscount != 0 && couponAmt > couponApplied.CouponMaxDiscount)
                                    {
                                        CouponAmount = Math.Round(couponApplied.CouponMaxDiscount, 2);
                                    }
                                    else
                                    {
                                        CouponAmount = Math.Round(couponAmt, 2);
                                    }
                                    // Reset shipping price and shipping option id for free ship coupon
                                    if (couponApplied.CouponFreeShip)
                                    {
                                        ShippingPrice = 0;
                                        ShippingOptionId = affiliateInfo.FreeShipShippingOption;
                                    }

                                    CouponFreeShip = couponApplied.CouponFreeShip;
                                    CouponValidMessage = "Coupon Applied Successfully";
                                    CouponValidMessageActual = "Coupon Applied Successfully";
                                    CouponId = couponApplied.CouponId;
                                    break;
                            }
                        }
                        
                        if (CartNetTotal < 0)
                        {
                            CouponValidMessage = "Discount Amount can not exceed Cart Total Amount";
                            CouponValidMessageActual = "Discount Amount can not exceed Cart Total Amount";
                            CouponId = 0;
                            CouponAmount = 0;
                        }
                    }
                }
            }
            else
            {
                // coupon added in previous step(s) could not be validated, make coupon amounts null
                CouponValidMessage = "Invalid Coupon";
                CouponValidMessageActual = "Invalid Coupon";
                CouponAmount = 0;

            }
        }

        // create list of products in cart 
        public IEnumerable<CartItem> GetProductList()
        {
            List<CartItem> newCartItems = new List<CartItem>();

            newCartItems.AddRange(CartItems.Select(cItem => new CartItem
            {
                Quantity = cItem.Quantity,
                ProductCart = new ProductCart
                {
                    ProductSizeId = cItem.ProductCart.ProductSizeId,
                    ProductSafeUrlName = cItem.ProductCart.ProductSafeUrlName,
                    ProductSizeStrength = cItem.ProductCart.ProductSizeStrength,
                    ProductSizeQuantity = cItem.ProductCart.ProductSizeQuantity,
                    ProductSizeStoreFrontPrice = cItem.ProductCart.ProductSizeStoreFrontPrice
                }
            }));

            // non linq version
            //foreach (var cItem in CartItems)
            //{
            //    CartItem lCartItem = new CartItem
            //    {
            //        Quantity = cItem.Quantity,
            //        ProductCart = new ProductCart
            //        {
            //            ProductSizeId = cItem.ProductCart.ProductSizeId,
            //            ProductSafeUrlName = cItem.ProductCart.ProductSafeUrlName,
            //            ProductSizeStrength = cItem.ProductCart.ProductSizeStrength,
            //            ProductSizeQuantity = cItem.ProductCart.ProductSizeQuantity,
            //            ProductSizeStoreFrontPrice = cItem.ProductCart.ProductSizeStoreFrontPrice
            //        }
            //    };

            //    newCartItems.Add(lCartItem);
            //}

            return newCartItems;
        }

        public void UpdateAutoReFill(int productSizeFk)
        {
            _cartItemCollection.Where(x => x.ProductCart.ProductSizeId == productSizeFk).FirstOrDefault().ProductCart.Subscription = true;
        }

        public bool IsRefill
        {
            get
            {
                bool isRefill = false;
                isRefill = _cartItemCollection.Where(x => x.CartRefill).Any();
                return isRefill;
            }
        }

        public int OriginalRefillOrderInvoiceFk { get; set; }

        public bool IsAutoRefillOrder { get; set; }

        public void UpdateCartItemRefill(int productSizeFk, bool isRefill, int shippingInvoiceFk, 
                    int allowedRefill, int availableRefill, int logRefillId = 0)
        {
            _cartItemCollection.Where(x => x.ProductCart.ProductSizeId == productSizeFk).Select(c =>
            {
                c.IsRefillCartItem = isRefill;
                c.CartItemShipInvoiceFk = shippingInvoiceFk;
                c.NumberOfRefillsAllowed = allowedRefill;
                c.NumberOfRefillsAvailable = availableRefill;
                return c;
            }).ToList();
        }
    }
}
