using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Concrete;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Infrastructure.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using MvcSiteMapProvider;

namespace EnhanceClub.WebUI.Helpers
{
    public class ProductDynamicNodeProvider : IDynamicNodeProvider
    {
       

        public IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode nodes)
        {
            IProductRepository _productRepository = new ProductRepositorySql();
            var returnValue = new List<DynamicNode>();
            List<FeaturedProduct> products = _productRepository.GetAllProducts(Convert.ToInt32(SiteConfigurationsWc.StorefrontId)).ToList();
            foreach(var product in products)
            {
                var node = new DynamicNode
                {
                    Title = product.FeaturedProductName,
                    Url = "/products/" + product.ProductUrl,
                    Controller = "Product",
                    Action = "ProductDetails"
                };
                returnValue.Add(node);

            }
            
            return returnValue;
        }

        public bool AppliesTo(string providerName)
        {
            if (string.IsNullOrEmpty(providerName))
                return false;
            return GetType() == Type.GetType(providerName, false);
        }
    }
}