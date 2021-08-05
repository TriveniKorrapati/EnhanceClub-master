using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Concrete;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Filters;
using EnhanceClub.WebUI.Infrastructure.Utility;
using EnhanceClub.WebUI.Models;

namespace EnhanceClub.WebUI.Controllers
{

    public class CartController : Controller
    {
        private readonly IProductRepository _repositoryProduct;
        private readonly IOrderProcessor _repositoryOrder;

        public CartController(IProductRepository repo, IOrderProcessor repoOrder)
        {
            _repositoryProduct = repo;
            _repositoryOrder = repoOrder;
        }

        public ViewResult Index(Cart cart, AffiliateInfo affiliateInfo, string returnUrl)
        {

            @ViewBag.Title = "Shopping Cart | " + SiteConfigurationsWc.StorefrontUrl;
            if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

            // since we only have product size Id in cart, add other fields required in every cartProduct object in cart

            for (var x = 0; x < cart.CartItems.Count(); x++)
            {
                // get related info for product size in cart and update cart object
                var frontendVisible = cart.CartItems.ElementAt(x).ProductCart.FrontendVisibleProductSizeFk;
                ProductCart productInCart =
                    _repositoryProduct.GetProductSizeInfoForCart(cart.CartItems.ElementAt(x).ProductCart.ProductSizeId, affiliateInfo.AffiliateStoreFrontFk, frontendVisible);
                if (productInCart != null)
                {
                    cart.CartItems.ElementAt(x).ProductCart = productInCart;
                }
            }
            return View(new CartIndexViewModel
            {
                Cart = cart,
                ReturnUrl = returnUrl
            });
        }


        // Add to Cart Method
        public JsonResult AddToCart(LoggedCustomer loggedCustomer, Cart cart, int productsizeId, int productFk, string returnUrl, int cartItemQuantity = 1)
        {

            ProductCart productCart = new ProductCart { ProductSizeId = productsizeId };

            var cartItemFound = cart.CartItems.FirstOrDefault(p => p.ProductCart.ProductId == productFk);
            if (cartItemFound == null)
            {
                var cartItemOrder = cart.CartItems.Count() + 1;
                cart.AddCartItem(productCart, cartItemQuantity, cartItemOrder);

                return Json(new { cartItemsCount = cart.CartItems.Count(), cartItemAdded = true }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var productName = cartItemFound.ProductCart.ProductStoreFrontName;
                TempData["productName"] = productName;
                return Json(new { cartItemsCount = cart.CartItems.Count(), cartItemAdded = false, productName = productName }, JsonRequestBehavior.AllowGet);
            }

        }


        // Add to Cart Method for multiple products
        public RedirectToRouteResult AddToCartMultiple(Cart cart, IEnumerable<CartProduct> productAdded, string returnUrl)
        {
            foreach (var cartProduct in productAdded)
            {
                if (cartProduct.ProductSizeAdd)
                {
                    ProductCart productCart = new ProductCart { ProductSizeId = cartProduct.CartProductSizeFk };
                    var cartItemOrder = cart.CartItems.Count() + 1;

                    cart.AddCartItem(productCart, cartProduct.ProductSizeAddQty, cartItemOrder);
                }
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        // Remove from Cart Method
        public JsonResult RemoveFromCart(Cart cart, int productId, string returnUrl)
        {
            var carItemFound = cart.CartItems.FirstOrDefault(p => p.ProductCart.ProductSizeId == productId);
            if (carItemFound != null)
            {
                ProductCart productCart = carItemFound.ProductCart;

                if (productCart != null)
                {
                    cart.RemoveCartItem(productCart);
                }
                return Json(new { cartItemsCount = cart.CartItems.Count() }, JsonRequestBehavior.AllowGet);
            }
            return Json("error", JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", new { returnUrl });
        }


        // Update Cart Quantity
        public RedirectToRouteResult UpdateCart(Cart cart, IEnumerable<ProductsOrdered> prodList, string returnUrl)
        {
            foreach (var prod in prodList)
            {
                var thisCartItem =
                    cart.CartItems.FirstOrDefault(x => x.ProductCart.ProductSizeId == prod.CartProductSizeFk);
                if (thisCartItem != null)
                {
                    thisCartItem.Quantity = prod.ProductSizeAddQty;
                }
                else
                {
                    // add to unexpected log 
                    var actionLog = new GlobalFunctions();
                    actionLog.AddLogUnexpected("cart item not found when trying to update cart quantity",
                        "CartController.cs",
                        "",
                        0,
                        0,
                        prod.CartProductSizeFk,
                        0
                        );
                }
            }

            return RedirectToAction("Index", new { returnUrl });
        }

        // apply coupon to cart
        public ActionResult ApplyCoupon(string couponCode, Cart cart, LoggedCustomer loggedCustomer,
            AffiliateInfo affiliateInfo, string returnUrl)
        {
            if (loggedCustomer.CustomerId == 0)
            {
                return RedirectToAction("Login", "Customer");
            }
            // check if couponCode is not null
            if (couponCode != null)
            {
                cart.CouponCode = couponCode;

                cart.CalculateCoupon(loggedCustomer, affiliateInfo, _repositoryOrder, cart);
            }
            return Redirect(returnUrl ?? "/order/checkout/checkoutstep/2");
        }

        // cart summary
        public PartialViewResult Summary(Cart cart)
        {
            return PartialView(cart);
        }

        public JsonResult UpdateCartItem(Cart cart, int productSizeFk, int productSizeQuantity)
        {

            var thisCartItem =
                cart.CartItems.FirstOrDefault(x => x.ProductCart.ProductSizeId == productSizeFk);
            if (thisCartItem != null)
            {
                thisCartItem.Quantity = productSizeQuantity;
                return Json("success", JsonRequestBehavior.AllowGet);
            }
            else
            {
                // add to unexpected log 
                var actionLog = new GlobalFunctions();
                actionLog.AddLogUnexpected("cart item not found when trying to update cart quantity",
                    "CartController.cs",
                    "",
                    0,
                    0,
                    productSizeFk,
                    0
                    );
                return Json("error", JsonRequestBehavior.AllowGet);
            }


        }

        public PartialViewResult CartPopup(Cart cart, AffiliateInfo affiliateInfo)
        {
            for (var x = 0; x < cart.CartItems.Count(); x++)
            {
                // FrontendVisibleProductSizeFk is used to fetch the discount volume products
                var frontendVisible = cart.CartItems.ElementAt(x).ProductCart.ProductSizeId;
                if (cart.CartItems.ElementAt(x).ProductCart.FrontendVisibleProductSizeFk != 0)
                    frontendVisible = cart.CartItems.ElementAt(x).ProductCart.FrontendVisibleProductSizeFk;

                ProductCart productInCart =
                    _repositoryProduct.GetProductSizeInfoForCart(cart.CartItems.ElementAt(x).ProductCart.ProductSizeId, affiliateInfo.AffiliateStoreFrontFk, frontendVisible);
                if (productInCart != null)
                {
                    cart.CartItems.ElementAt(x).ProductCart = productInCart;
                }

                // get volume discounted products
                var discountedProducts = productInCart.VolumeDiscountProductSize;
                if (discountedProducts != null && discountedProducts.Count() > 0 && cart.CartItems.ElementAt(x).ProductCart.ProductSizeVisibleFrontEnd)
                {

                    productInCart.ProductSizeNonDiscountedPrice = discountedProducts.Where(prod => prod.RelatedProductSizeFk == cart.CartItems.ElementAt(x).ProductCart.ProductSizeId).Select(prod => prod.ProductSizeNonDiscountedPrice).FirstOrDefault();
                }
            }

            // get related products
            FeaturedProduct relatedBuyProduct = null;
            if (cart.CartItems.Any())
            {
                var productId = cart.CartItems.Select(x => x.ProductCart.ProductId).FirstOrDefault();

                var relatedProducts = _repositoryProduct.GetRelatedProducts(productId, affiliateInfo.AffiliateStoreFrontFk, string.Empty).ToList();
                if (relatedProducts.Any())
                {
                    if (!cart.CartItems.Any(x => x.ProductCart.ProductId == relatedProducts.FirstOrDefault().FeaturedProductId))
                    {
                        relatedBuyProduct = relatedProducts.FirstOrDefault();
                    }
                }
            }



            return PartialView("_cartPopup",
                new CartIndexViewModel
                {
                    Cart = cart,
                    RelatedProducts = relatedBuyProduct
                });
        }

        public ActionResult ReplaceCartItem(Cart cart, AffiliateInfo affiliateInfo, int currentProductSizeFk, int updatedProductSizeFk, int frontendVisibleProductSizeFk)
        {
            var cartItemFound = cart.CartItems.FirstOrDefault(p => p.ProductCart.ProductSizeId == currentProductSizeFk);
            var cartItemOrder = 0;
            if (cartItemFound != null)
            {
                cartItemOrder = cartItemFound.CartItemOrder;
                ProductCart productCart = cartItemFound.ProductCart;

                if (productCart != null)
                {
                    cart.RemoveCartItem(productCart);
                }

            }

            ProductCart productCartAdd = new ProductCart { ProductSizeId = updatedProductSizeFk, FrontendVisibleProductSizeFk = frontendVisibleProductSizeFk };
            cart.AddCartItem(productCartAdd, 1, cartItemOrder);

            for (var x = 0; x < cart.CartItems.Count(); x++)
            {
                // get cart info along with volume discount products

                ProductCart productInCart =
                    _repositoryProduct.GetProductSizeInfoForCart(cart.CartItems.ElementAt(x).ProductCart.ProductSizeId, affiliateInfo.AffiliateStoreFrontFk,
                                        cart.CartItems.ElementAt(x).ProductCart.FrontendVisibleProductSizeFk);
                if (productInCart != null && cart.CartItems.ElementAt(x).ProductCart.FrontendVisibleProductSizeFk == frontendVisibleProductSizeFk)
                {

                    // get volume discounted products
                    var discountedProducts = productInCart.VolumeDiscountProductSize;

                    // if cart item has volume discount products, the set the frontend visible flags
                    if (discountedProducts != null && discountedProducts.Count() > 0 && cart.CartItems.ElementAt(x).ProductCart.ProductSizeVisibleFrontEnd)
                    {
                        productInCart.VolumeDiscountProductSize = discountedProducts;
                        productInCart.FrontendVisibleProductSizeFk = frontendVisibleProductSizeFk;
                        productInCart.ProductSizeVisibleFrontEnd = true;
                        productInCart.ProductSizeNonDiscountedPrice = discountedProducts.Where(prod => prod.RelatedProductSizeFk == updatedProductSizeFk).Select(prod => prod.ProductSizeNonDiscountedPrice).FirstOrDefault();
                    }
                    cart.CartItems.ElementAt(x).ProductCart = productInCart;
                }
            }

            return PartialView("_cartPopup",
                new CartIndexViewModel
                {
                    Cart = cart,


                });
        }

    }
}

