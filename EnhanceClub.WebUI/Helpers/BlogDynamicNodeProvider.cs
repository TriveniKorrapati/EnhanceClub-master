using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Concrete;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Infrastructure.Utility;
using MvcSiteMapProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace EnhanceClub.WebUI.Helpers
{
    public class BlogDynamicNodeProvider : IDynamicNodeProvider
    {
       
        public IEnumerable<DynamicNode> GetDynamicNodeCollection(ISiteMapNode nodes)
        {
            IStorefrontRepository _storefrontRepository = new StorefrontRepositorySql();
            var returnValue = new List<DynamicNode>();

            // get list of blogs setup in backend
            List<BlogTable> blogList = _storefrontRepository.GetBlogList(0, 1, Convert.ToInt32(SiteConfigurationsWc.StorefrontId),
                " blog_featured desc, blog_datecreated desc ");

            foreach(var blog in blogList)
            {
                var node = new DynamicNode
                {
                    Title = Regex.Replace(blog.BlogHeadline, "<.*?>", string.Empty),
                    Url = "/blog/" + blog.BlogLink,
                    Controller = "Blog",
                    Action = "BlogSelector"
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