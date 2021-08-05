using System.Collections.Generic;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.Domain.Abstract
{
    public interface IFeaturedProductRepository
    {
        IEnumerable<FeaturedProduct> FeaturedProducts { get; }

        // TopProducts used to list top 40 products on home page
        IEnumerable<FeaturedProduct> TopProducts { get; }

        IEnumerable<FeaturedProduct> PopularProducts { get; set; }
    }
    
}
