using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Controllers
{
    public class FeaturedProductController : Controller
    {
        private readonly IFeaturedProductRepository _repository;
        private readonly IProductRepository _productRepository;

        public FeaturedProductController(IFeaturedProductRepository repo,IProductRepository productRepository)
        {
            _repository = repo;
            this._productRepository = productRepository;
        }

        public PartialViewResult PopularProductList(AffiliateInfo affiliateInfo, int hideBuy = 0)
        {

            @ViewBag.HideBuy = hideBuy;

            IEnumerable<FeaturedProduct> popularProducts = null;

             popularProducts = _repository.PopularProducts;

             return PartialView(popularProducts); 
            
        }

        public PartialViewResult TopProductList(AffiliateInfo affiliateInfo, string category = null, string productType = "Rx", int productId = 0, string sortBy = "", int limitDisplay = 0, int hideBuy = 0)
        {
            
            @ViewBag.HideBuy = hideBuy;

            IEnumerable<FeaturedProduct> topProducts = null;
            
            topProducts = _repository.TopProducts;
            
            if (limitDisplay > 0)
            {
                return PartialView(topProducts.Take(limitDisplay)); // limit product display to n products
            }
            else
            {
                return PartialView(topProducts); // limit to 40
            }
        }


        // this view returns in button format for product pages
        public PartialViewResult FeaturedProductList(AffiliateInfo affiliateInfo, string category = null, string productType = "Rx", int productId = 0, string sortBy = "", int limitDisplay = 0, int hideBuy = 0)
        {
            // related products come from db while other featured products come from static repository
            
            @ViewBag.HideBuy = hideBuy;

            IEnumerable<FeaturedProduct> featuredProducts= null;
            if (productType == "Related")
            {
                // filter out products that don't have any active product size for storefront
                 featuredProducts = _productRepository.GetRelatedProducts(productId,affiliateInfo.AffiliateStoreFrontFk,sortBy).Where(x=>x.ProductSizeCount > 0);
            }
            else
            {
                 featuredProducts = category == null ? _repository.FeaturedProducts.Where(e => e.ProductType == productType) : _repository.FeaturedProducts.Where(e => e.Category == category && e.ProductType == productType);                
            }

            if (limitDisplay > 0)
            {
                return PartialView(featuredProducts.Take(limitDisplay)); // limit product display to n products
            }
            else
            {
                return PartialView(featuredProducts); // limit to six products
            }
        }
        // this view returns plain <li>

        public PartialViewResult FeaturedProductListNoFormat(string category = null, string productType = "Rx", int limitDisplay = 0, int hideBuy = 0)
        {
            @ViewBag.HideBuy = hideBuy;
            IEnumerable<FeaturedProduct> featuredProducts = category == null ? _repository.FeaturedProducts.Where(e => e.ProductType == productType) : _repository.FeaturedProducts.Where(e => e.Category == category && e.ProductType == productType);

            if (limitDisplay > 0)
            {
                return PartialView(featuredProducts.Take(limitDisplay)); // limit product display to n products
            }
            else
            {
                return PartialView(featuredProducts);
            }
            }
    }
}