using System.Collections.Generic;
using System.Web.Mvc;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Infrastructure.Utility;
using EnhanceClub.WebUI.Models;
using System.Linq;
namespace EnhanceClub.WebUI.Controllers
{
    public class BlogController : Controller
    {
        private readonly IStorefrontRepository _storefrontRepository;

        public BlogController(IStorefrontRepository storefrontRepository)
        {
            _storefrontRepository = storefrontRepository;
        }

        // GET: Blog
        public ActionResult Index(AffiliateInfo affiliateInfo,int page=1)
        {
            @ViewBag.Title = "Latest news for Canadian Pharmacy Health | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description = "Check out the Enhance Club blog! Keep up to date on "
                + SiteConfigurationsWc.StorefrontName + " and everything related to men's health that you'll want to know.";

            if (Request.Url != null)
            {
                @ViewBag.canonicalRef = Request.Url.AbsoluteUri;
                if (Request.Url.AbsoluteUri.EndsWith("1"))
                {
                    @ViewBag.canonicalNext = Request.Url.AbsoluteUri.Replace("1", "2");
                }
                else
                {
                    @ViewBag.canonicalNext =  Request.Url.AbsoluteUri + "/2";
                }

            }
            // get list of blogs setup in backend
            List<BlogTable> blogList = _storefrontRepository.GetBlogList(0, 1,affiliateInfo.AffiliateStoreFrontFk, " blog_featured desc, blog_datecreated desc ");

            
            return View("Blog", new BlogListViewModel { BlogList = blogList });

           
        }

        // handles all blog pages
        public ActionResult BlogSelector(string blogUrl, AffiliateInfo affiliateInfo)
        {
            // get blog data by url
            BlogTable thisBlog = _storefrontRepository.GetBlog(blogUrl.Trim(),1,affiliateInfo.AffiliateStoreFrontFk);

            // when try to access inactive blog returns empty record 
            if (thisBlog.BlogId == 0)
            {
                return RedirectToAction("Index");
            }

            else 
            {

                @ViewBag.Title = @thisBlog.BlogMetaTitle;

                @ViewBag.Description = @thisBlog.BlogMetaDescription;

                if (Request.Url != null)
                {
                    @ViewBag.MainImageUrl = "https://" + Request.Url.Host + "/content/images/blog/" + @thisBlog.BlogMainImage;
                    @ViewBag.MainImagepath = "~/content/images/blog/" + @thisBlog.BlogTnImage;
                }
                @ViewBag.BlogDateCreated = @thisBlog.BlogDateCreated;
                @ViewBag.BlogLastModified = @thisBlog.BlogLastModified;

                //to Load the next blog 
                List<int> blogNumbers = _storefrontRepository.GetAllBlogNumbers(1, affiliateInfo.AffiliateStoreFrontFk);
                int blogsAfterCurrentBlog = blogNumbers.FirstOrDefault(x => x > thisBlog.BlogId);
                if (blogsAfterCurrentBlog == 0) {
                   blogsAfterCurrentBlog = blogNumbers.First();
                }
                var nextBlogs = _storefrontRepository.GetBlogList(blogsAfterCurrentBlog, 1, affiliateInfo.AffiliateStoreFrontFk, " blog_id");
                @ViewBag.NextBlogUrl = nextBlogs.First().BlogLink;
                @ViewBag.NextImageUrl = "/content/images/blog/" + nextBlogs.First().BlogMainImage;
                @ViewBag.NextBlogTitle = nextBlogs.First().BlogMetaTitle;
            }

            return View(blogUrl, thisBlog);
        }


    }
}