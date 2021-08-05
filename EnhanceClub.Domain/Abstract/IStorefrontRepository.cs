using System;
using System.Collections.Generic;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.Domain.Abstract
{
    // takes care of all affiliate and storefront related queries
    public interface IStorefrontRepository
    {
        StoreFrontInfo GetStoreFrontInfo(int storeFrontId);

        List<ShippingOption> GetShippingOptionsAffiliate( int affiliateId,int shippingOptionId);

        List<ShippingOption> GetShippingOptionsDefault(int shippingOptionId);

        List<BlogTable> GetBlogList(int blogId, int blogLanguageFk, int storeFrontFk, string sortBy);


        BlogTable GetBlog(string blogUrl, int blogLanguageFk, int storeFrontFk);

        int AddSubscribedUser(int storefrontFk, string name, string email, string ipAddress, DateTime dateCreated);

        List<int> GetAllBlogNumbers(int blogLanguageFk, int storeFrontFk);
        List<PaymentOption> GetPaymentOptionsStoreFront(int storeFrontId, int paymentOptionFk, bool active);
    }
}
