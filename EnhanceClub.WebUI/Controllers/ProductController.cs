using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using EnhanceClub.Domain.Abstract;
using EnhanceClub.Domain.Entities;
using EnhanceClub.WebUI.Filters;
using EnhanceClub.WebUI.Infrastructure.Utility;
using EnhanceClub.WebUI.Models;

namespace EnhanceClub.WebUI.Controllers
{
    public class ProductController : Controller
    {

        // product repository
        private readonly IProductRepository _repository;
        private readonly IStorefrontRepository _storefrontRepository;
        public int PageSize = 25;

        public ProductController(IProductRepository productRepository, IStorefrontRepository storefrontRepository)
        {
            _repository = productRepository;
            _storefrontRepository = storefrontRepository;
        }

        // otc product search
      
        [OutputCache(CacheProfile = "cp-server")]
        public ActionResult Otc(AffiliateInfo affiliateInfo, string searchLetter, int page = 1)
        {
            // get count of products under each letter so that we can disable alphabets that don't have any product
            IEnumerable<ProductAlphabet> productAlphabet = _repository.GetProductCountByAlphabet(affiliateInfo.AffiliateStoreFrontFk, 1);

            // if search letter was not selected 
            if (searchLetter == null)
            {

                @ViewBag.Title = "Buy Online Non-Prescription Medications  | " + SiteConfigurationsWc.StorefrontUrl;
                @ViewBag.Description =
                   SiteConfigurationsWc.StorefrontName + " has high quality over-the-counter (OTC) medications at discounted prices. Search through our extensive list of over-the-counter medications.";
                if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

                return View(new SearchLetterViewModel
                {
                    AffiliateInfo = affiliateInfo,
                    ProductAlphabet = productAlphabet,
                    AlphaLink = "otc"
                });
            }
            else
            {
                // letter filter was specified
                @ViewBag.MetaRobot = "noindex,follow";

                @ViewBag.SeachLetter = searchLetter;

                var pagePrefix = page > 1 ? "Page " + page + " for " : "";

                @ViewBag.Title = pagePrefix + searchLetter.ToUpper() + " - Over-the-Counter Products | " + SiteConfigurationsWc.StorefrontUrl;
                @ViewBag.Description = pagePrefix +
                   "Letter " + searchLetter.ToUpper() + " | " + SiteConfigurationsWc.StorefrontName + " carries a vast selection of discount prescription medications that include both generic and brand name prescription medications.";

                if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

                if (Request.Url != null && Request.Url.AbsoluteUri.Contains("/page1"))
                {
                    @ViewBag.canonicalRef = Utility.UriWithoutPagination(Request.Url.AbsoluteUri, "/page" + page);
                    @ViewBag.Title = pagePrefix + searchLetter.ToUpper() + " Page 1 - Over-the-Counter Products | " + SiteConfigurationsWc.StorefrontUrl;
                    @ViewBag.Description = pagePrefix +
                                               "Letter " + searchLetter.ToUpper() + " Page 1 | " + SiteConfigurationsWc.StorefrontName + " carries a vast selection of discount prescription medications that include both generic and brand name prescription medications.";

                }
                var modifySearchRxOtc = "";

                // search all products
                var productType = 1;
                IEnumerable<ProductSearch> productList = _repository.GetProductBasedOnLetterFilter(searchLetter,
                                                                                                 affiliateInfo.AffiliateStoreFrontFk,
                                                                                                 modifySearchRxOtc.ToString(),
                                                                                                 productType,
                                                                                                 true,
                                                                                                 true,
                                                                                                 true,
                                                                                                 true);
                // Check how many products are returned by Search
                if (productList != null)
                {

                    // remove products that dont have any active product size: 3 Nov 2017
                    var productsWithActiveSize = productList.Where(x => x.ProductSizeList != null && x.ProductSizeList.Count > 0);

                    var productListAsList = productsWithActiveSize as IList<ProductSearch> ?? productsWithActiveSize.ToList();

                    // Start: removed logic to go straight to product detail as per Amir : 8 Nov 2017 
                    // if only one product is returned, go to product detail page directly
                    //if (productListAsList.Count() == 1)
                    //{
                    //    var productSafeUrl = productListAsList.FirstOrDefault().ProductStoreFrontSafeUrlName;

                    //    return RedirectToAction("ProductDetail", new { searchTerm = productSafeUrl });
                    //}
                    //else
                    //{

                    // if more than one products are found, display products for customer to choose

                    // Because drop down in the view needs all products regardless of pagination, 
                    // It is best to send scaled down version of ProductSearch with all products 
                    // that can be used to build quick Jump drop down for all products.

                    List<ProductDropDown> listForQuickJump = productListAsList.Select
                                                                          (row => new ProductDropDown
                                                                          {
                                                                              ProductName = row.ProductName,
                                                                              ProductStoreFrontSafeUrlName = row.ProductStoreFrontSafeUrlName

                                                                          }).ToList();


                    PagingInfo pageInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = PageSize,
                        TotalItems = productListAsList.Count()
                    };

                    if (Request.Url != null)
                    {
                        string sPage = "page" + page;
                        string pPage = "page" + (page - 1);
                        string nPage = "page" + (page + 1);

                        if (page == 1 && pageInfo.TotalPages > 1)
                        {
                            if (Request.Url.AbsoluteUri.EndsWith("1"))
                            {
                                @ViewBag.canonicalNext = Request.Url.AbsoluteUri.Replace("1", "2");
                            }
                            else
                            {
                                @ViewBag.canonicalNext = Request.Url.AbsoluteUri + "/page2";
                            }

                        }

                        if (page > 1 && pageInfo.TotalPages > page)
                        {
                            @ViewBag.canonicalPrev = Regex.Replace(Request.Url.AbsoluteUri, sPage, pPage, RegexOptions.IgnoreCase);
                            @ViewBag.canonicalNext = Regex.Replace(Request.Url.AbsoluteUri, sPage, nPage, RegexOptions.IgnoreCase);
                        }

                        if (page > 1 && pageInfo.TotalPages == page)
                        {
                            @ViewBag.canonicalPrev = Regex.Replace(Request.Url.AbsoluteUri, sPage, pPage, RegexOptions.IgnoreCase);

                        }

                    }

                    return View("SearchByAlphabet", new ProductSearchViewModel
                    {
                        ProductList = productListAsList.OrderBy(p => p.ProductName).Skip((page - 1) * PageSize).Take(PageSize),
                        ListForDropDown = listForQuickJump,
                        SearchTermPassed = searchLetter,
                        ContactNumber = affiliateInfo.StorefrontContact,
                        PagingInfo = pageInfo,
                        SearchUrlLink = "otc",
                        ProductAlphabet = productAlphabet
                    });
                    // }
                }
                else
                {

                    // display product not found page
                    return View("ProductNotFound", new ProductSearchViewModel
                    {
                        ProductList = productList,
                        SearchTermPassed = searchLetter,
                        ContactNumber = affiliateInfo.StorefrontContact,
                        SearchType = "letter",
                        SearchUrlLink = "otc",
                        ProductAlphabet = productAlphabet
                    });
                }

            }
        }

        // prescription product search 
       
        [OutputCache(CacheProfile = "cp-server")]
        public ActionResult Prescription(AffiliateInfo affiliateInfo, string searchLetter, int page = 1)
        {
            // get count of products under each letter so that we can disable alphbets that don't have any product
            IEnumerable<ProductAlphabet> productAlphabet = _repository.GetProductCountByAlphabet(affiliateInfo.AffiliateStoreFrontFk, 2);

            // if search letter was not selected 
            if (searchLetter == null)
            {

                @ViewBag.Title = "Buy Discount Prescription Medications  | " + SiteConfigurationsWc.StorefrontUrl;
                @ViewBag.Description =
                   SiteConfigurationsWc.StorefrontName + " carries a vast selection of discount prescription medications that include both generic and brand name prescription medications.";
                if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

                return View(new SearchLetterViewModel
                {
                    AffiliateInfo = affiliateInfo,
                    ProductAlphabet = productAlphabet,
                    AlphaLink = "prescription"
                });

            }
            else
            {
                // letter filter was specified
                @ViewBag.MetaRobot = "noindex,follow";

                @ViewBag.SeachLetter = searchLetter;

                var pagePrefix = page > 1 ? "Page " + page + " for " : "";
                @ViewBag.Title = pagePrefix + searchLetter.ToUpper() + " - Prescription Products  | " + SiteConfigurationsWc.StorefrontUrl;
                @ViewBag.Description = pagePrefix +
                   "Letter " + searchLetter.ToUpper() + " |  " + SiteConfigurationsWc.StorefrontName + " carries a vast selection of discount prescription medications that include both generic and brand name prescription medications.";

                if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

                if (Request.Url != null && Request.Url.AbsoluteUri.Contains("/page1"))
                {

                    @ViewBag.canonicalRef = Utility.UriWithoutPagination(Request.Url.AbsoluteUri, "/page" + page);
                    @ViewBag.Title = pagePrefix + searchLetter.ToUpper() + " Page 1 - Prescription Products  | " + SiteConfigurationsWc.StorefrontUrl;
                    @ViewBag.Description = pagePrefix +
                                               "Letter " + searchLetter.ToUpper() + " Page 1|  " + SiteConfigurationsWc.StorefrontName + " carries a vast selection of discount prescription medications that include both generic and brand name prescription medications.";

                }
                var modifySearchRxOtc = "";


                // search all products
                var productType = 2;
                IEnumerable<ProductSearch> productList = _repository.GetProductBasedOnLetterFilter(searchLetter,
                                                                                                 affiliateInfo.AffiliateStoreFrontFk,
                                                                                                 modifySearchRxOtc.ToString(),
                                                                                                 productType,
                                                                                                 true,
                                                                                                 true,
                                                                                                 true,
                                                                                                 true);
                // Check how many products are returned by Search
                if (productList != null)
                {
                    // remove products that dont have any active product size: 3 Nov 2017
                    var productsWithActiveSize = productList.Where(x => x.ProductSizeList != null && x.ProductSizeList.Count > 0);

                    var productListAsList = productsWithActiveSize as IList<ProductSearch> ?? productsWithActiveSize.ToList();

                    // if only one product is returned, go to product detail page directly

                    // Start: removed logic to go straight to product detail as per Amir : 8 Nov 2017 

                    //if (productListAsList.Count() == 1)
                    //{
                    //    var productSafeUrl = productListAsList.FirstOrDefault().ProductStoreFrontSafeUrlName;

                    //    return RedirectToAction("ProductDetail", new { searchTerm = productSafeUrl });
                    //}
                    //else
                    //{
                    // if more than one products are found, display products for customer to choose

                    // Because drop down in the view needs all products regardless of pagination, 
                    // It is best to send scaled down version of ProductSearch with all products 
                    // that can be used to build quick Jump drop down for all products.

                    List<ProductDropDown> listForQuickJump = productListAsList.Select
                                                                          (row => new ProductDropDown
                                                                          {
                                                                              ProductName = row.ProductName,
                                                                              ProductStoreFrontSafeUrlName = row.ProductStoreFrontSafeUrlName

                                                                          }).ToList();


                    PagingInfo pageInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = PageSize,
                        TotalItems = productListAsList.Count()
                    };

                    if (Request.Url != null)
                    {
                        string sPage = "page" + page;
                        string pPage = "page" + (page - 1);
                        string nPage = "page" + (page + 1);

                        if (page == 1 && pageInfo.TotalPages > 1)
                        {
                            if (Request.Url.AbsoluteUri.EndsWith("1"))
                            {
                                @ViewBag.canonicalNext = Request.Url.AbsoluteUri.Replace("1", "2");
                            }
                            else
                            {
                                @ViewBag.canonicalNext = Request.Url.AbsoluteUri + "/page2";
                            }

                        }

                        if (page > 1 && pageInfo.TotalPages > page)
                        {
                            @ViewBag.canonicalPrev = Regex.Replace(Request.Url.AbsoluteUri, sPage, pPage, RegexOptions.IgnoreCase);
                            @ViewBag.canonicalNext = Regex.Replace(Request.Url.AbsoluteUri, sPage, nPage, RegexOptions.IgnoreCase);
                        }

                        if (page > 1 && pageInfo.TotalPages == page)
                        {
                            @ViewBag.canonicalPrev = Regex.Replace(Request.Url.AbsoluteUri, sPage, pPage, RegexOptions.IgnoreCase);

                        }

                    }

                    return View("SearchByAlphabet", new ProductSearchViewModel
                    {
                        ProductList = productListAsList.OrderBy(p => p.ProductName).Skip((page - 1) * PageSize).Take(PageSize),
                        ListForDropDown = listForQuickJump,
                        SearchTermPassed = searchLetter,
                        ContactNumber = affiliateInfo.StorefrontContact,
                        PagingInfo = pageInfo,
                        SearchUrlLink = "prescription",
                        ProductAlphabet = productAlphabet
                    });
                    // }
                }
                else
                {
                    // display product not found page
                    return View("ProductNotFound", new ProductSearchViewModel
                    {
                        ProductList = productList,
                        SearchTermPassed = searchLetter,
                        ContactNumber = affiliateInfo.StorefrontContact,
                        SearchType = "letter",
                        SearchUrlLink = "prescription",
                        ProductAlphabet = productAlphabet
                    });
                }

            }
        }

        // pet product search
        
        [OutputCache(CacheProfile = "cp-server")]
        public ActionResult Pet(AffiliateInfo affiliateInfo, string searchLetter, int page = 1)
        {
            // get count of products under each letter so that we can disable alphbets that don't have any product
            IEnumerable<ProductAlphabet> productAlphabet = _repository.GetPetProductCountByAlphabet(affiliateInfo.AffiliateStoreFrontFk);

            // if search letter was not selected 
            if (searchLetter == null)
            {

                @ViewBag.Title = "Buy Discount Pet Meds  | " + SiteConfigurationsWc.StorefrontUrl;
                @ViewBag.Description =
                    SiteConfigurationsWc.StorefrontUrl + " provides its customers with only the best quality pet meds that include both prescription and over-the-counter medications at low prices.";
                if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

                return View(new SearchLetterViewModel
                {
                    AffiliateInfo = affiliateInfo,
                    ProductAlphabet = productAlphabet,
                    AlphaLink = "pet"
                });
            }
            else
            {
                // letter filter was specified

                @ViewBag.MetaRobot = "noindex,follow";

                @ViewBag.SeachLetter = searchLetter;

                var pagePrefix = page > 1 ? "Page " + page + " for " : "";
                @ViewBag.Title = pagePrefix + searchLetter.ToUpper() + " - Pet Medications  | " + SiteConfigurationsWc.StorefrontUrl;
                @ViewBag.Description = pagePrefix +
                   "Letter " + searchLetter.ToUpper() + " | " + SiteConfigurationsWc.StorefrontName + " provides its customers with only the best quality pet meds that include both prescription and over-the-counter medications at low prices.";

                if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

                if (Request.Url != null && Request.Url.AbsoluteUri.Contains("/page1"))
                {
                    @ViewBag.canonicalRef = Utility.UriWithoutPagination(Request.Url.AbsoluteUri, "/page" + page);
                    @ViewBag.Title = pagePrefix + searchLetter.ToUpper() + " Page 1 - Pet Medications  | " + SiteConfigurationsWc.StorefrontUrl;
                    @ViewBag.Description = pagePrefix +
                                               "Letter " + searchLetter.ToUpper() + " Page 1 | " + SiteConfigurationsWc.StorefrontName + " provides its customers with only the best quality pet meds that include both prescription and over-the-counter medications at low prices.";

                }
                var modifySearchRxOtc = "";

                // search all products
                var productType = 0;
                IEnumerable<ProductSearch> productList = _repository.GetPetProductBasedOnLetterFilter(searchLetter,
                                                                                                 affiliateInfo.AffiliateStoreFrontFk,
                                                                                                 modifySearchRxOtc.ToString(),
                                                                                                 productType,
                                                                                                 true,
                                                                                                 true,
                                                                                                 true,
                                                                                                 true);
                // Check how many products are returned by Search
                if (productList != null)
                {

                    // remove products that dont have any active product size: 3 Nov 2017
                    var productsWithActiveSize = productList.Where(x => x.ProductSizeList != null && x.ProductSizeList.Count > 0);

                    var productListAsList = productsWithActiveSize as IList<ProductSearch> ?? productsWithActiveSize.ToList();

                    // Start: removed logic to go straight to product detail as per Amir : 8 Nov 2017 

                    // if only one product is returned, go to product detail page directly
                    //if (productListAsList.Count() == 1)
                    //{
                    //    var productSafeUrl = productListAsList.FirstOrDefault().ProductStoreFrontSafeUrlName;

                    //    return RedirectToAction("ProductDetail", new {searchTerm = productSafeUrl});
                    //}
                    //else
                    //{
                    // if more than one products are found, display products for customer to choose

                    // Because drop down in the view needs all products regardless of pagination, 
                    // It is best to send scaled down version of ProductSearch with all products 
                    // that can be used to build quick Jump drop down for all products.

                    List<ProductDropDown> listForQuickJump = productListAsList.Select
                                                                          (row => new ProductDropDown
                                                                          {
                                                                              ProductName = row.ProductName,
                                                                              ProductStoreFrontSafeUrlName = row.ProductStoreFrontSafeUrlName

                                                                          }).ToList();


                    PagingInfo pageInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = PageSize,
                        TotalItems = productListAsList.Count()
                    };

                    if (Request.Url != null)
                    {
                        string sPage = "page" + page;
                        string pPage = "page" + (page - 1);
                        string nPage = "page" + (page + 1);

                        if (page == 1 && pageInfo.TotalPages > 1)
                        {
                            if (Request.Url.AbsoluteUri.EndsWith("1"))
                            {
                                @ViewBag.canonicalNext = Request.Url.AbsoluteUri.Replace("1", "2");
                            }
                            else
                            {
                                @ViewBag.canonicalNext = Request.Url.AbsoluteUri + "/page2";
                            }

                        }

                        if (page > 1 && pageInfo.TotalPages > page)
                        {
                            @ViewBag.canonicalPrev = Regex.Replace(Request.Url.AbsoluteUri, sPage, pPage, RegexOptions.IgnoreCase);
                            @ViewBag.canonicalNext = Regex.Replace(Request.Url.AbsoluteUri, sPage, nPage, RegexOptions.IgnoreCase);
                        }

                        if (page > 1 && pageInfo.TotalPages == page)
                        {
                            @ViewBag.canonicalPrev = Regex.Replace(Request.Url.AbsoluteUri, sPage, pPage, RegexOptions.IgnoreCase);

                        }

                    }
                    // get product
                    return View("SearchByAlphabet", new ProductSearchViewModel
                    {
                        ProductList = productListAsList.OrderBy(p => p.ProductName).Skip((page - 1) * PageSize).Take(PageSize),
                        ListForDropDown = listForQuickJump,
                        SearchTermPassed = searchLetter,
                        ContactNumber = affiliateInfo.StorefrontContact,
                        PagingInfo = pageInfo,
                        SearchUrlLink = "pet",
                        ProductAlphabet = productAlphabet

                    });
                    // }
                }
                else
                {
                    // display product not found page
                    return View("ProductNotFound", new ProductSearchViewModel
                    {
                        ProductList = productList,
                        SearchTermPassed = searchLetter,
                        ContactNumber = affiliateInfo.StorefrontContact,
                        SearchType = "letter",
                        SearchUrlLink = "pet",
                        ProductAlphabet = productAlphabet
                    });
                }

            }
        }

        // search form submits to this action
        [OutputCache(CacheProfile = "cp-server")]
        public ActionResult SearchProduct(string productNameFilter, int productType)
        {
            TempData["pType"] = productType;
            return RedirectToAction("search", new { searchTerm = productNameFilter });

            //return Redirect(Url.Action("search", new { searchTerm = productNameFilter }));

            //return Redirect(Url.Action("search","Product", new { searchTerm = productNameFilter }));
        }

        // product search based on name
        
        [OutputCache(CacheProfile = "cp-server")]
        public ActionResult Search(string searchTerm, AffiliateInfo affiliateInfo, int page = 1)
        {

            int productType = 0;

            if (TempData["pType"] != null)
            {
                productType = Convert.ToInt32(TempData["pType"].ToString());
            }


            @ViewBag.Title = "Keyword Search Results  | " + SiteConfigurationsWc.StorefrontUrl;

            @ViewBag.Description =
            "Search for prescriptions, over-the-counter medications, or pet medications at " + SiteConfigurationsWc.StorefrontUrl + " using primary keywords.";

            if (Request.Url != null) { @ViewBag.canonicalRef = Request.Url.AbsoluteUri; }

            // strip special characters from search term


            // add search term to log table

            _repository.AddLogSearchTerm(searchTerm, affiliateInfo.AffiliateId, DateTime.Now);

            // get data from search term table for the search string typed by user

            IEnumerable<string> searchTermIdList = _repository.GetProductSearchTermList(searchTerm, affiliateInfo.AffiliateStoreFrontFk, true);

            // Start -- Test 

            //IEnumerable<string> searchTermIdList = _repository.GetProductSearchTermList(searchTerm, 14, true);

            // End   -- Test 

            // get Storefront Currency  data -- CURRENTLY THIS DATA NOT BEING USED
            StoreFrontInfo storeFrontInfo = _storefrontRepository.GetStoreFrontInfo(affiliateInfo.AffiliateStoreFrontFk);

            IEnumerable<ProductSearch> productList = null;

            // if search term is defined get products data based on search term
            if (searchTermIdList != null)
            {
                productList = _repository.GetProductBasedOnSearchTerm(searchTermIdList,
                   affiliateInfo.AffiliateStoreFrontFk,
                   true,
                   true,
                   true,
                   true);
            }
            else
            // if search term is not defined do regular search
            {

                // this variable should be implemented in session if we want to show OTC Products in Rx Search 
                // coldfusion version uses cookie for this purpose

                var modifySearchRxOtc = 'R';

                // search all products

                productList = _repository.GetProductBasedOnNameFilter(searchTerm,
                   affiliateInfo.AffiliateStoreFrontFk,
                   modifySearchRxOtc.ToString(),
                   productType,
                   true,
                   true,
                   true,
                   true);

            }

            // Check how many products are returned by Search
            if (productList != null)
            {

                // remove products that don't have any active product size: 3 Nov 2017
                var productsWithActiveSize = productList.Where(x => x.ProductSizeList != null && x.ProductSizeList.Count > 0);

                var productListAsList = productsWithActiveSize as IList<ProductSearch> ?? productsWithActiveSize.ToList();

                // Start: removed logic to go straight to product detail as per Amir : 8 Nov 2017 
                // if only one product is returned, go to product detail page directly
                //if (productListAsList.Count() == 1)
                //{

                //    var productSafeUrl = productListAsList.FirstOrDefault().ProductStoreFrontSafeUrlName;

                //    return RedirectToAction("ProductDetail", new { searchTerm = productSafeUrl });
                //}
                //else
                //{
                // if more than one products are found, display products for customer to choose

                // Because drop down in the view needs all products regardless of pagination, 
                // It is best to send scaled down version of ProductSearch with all products 
                // that can be used to build quick Jump drop down for all products.

                List<ProductDropDown> listForQuickJump = productListAsList.Select
                                                                      (row => new ProductDropDown
                                                                      {
                                                                          ProductName = row.ProductName,
                                                                          ProductStoreFrontSafeUrlName = row.ProductStoreFrontSafeUrlName

                                                                      }).ToList();
                PagingInfo pageInfo = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = PageSize,
                    TotalItems = productListAsList.Count()
                };

                if (Request.Url != null)
                {
                    string sPage = "page" + page;
                    string pPage = "page" + (page - 1);
                    string nPage = "page" + (page + 1);

                    if (page == 1 && pageInfo.TotalPages > 1)
                    {
                        if (Request.Url.AbsoluteUri.EndsWith("1"))
                        {
                            @ViewBag.canonicalNext = Request.Url.AbsoluteUri.Replace("1", "2");
                        }
                        else
                        {
                            @ViewBag.canonicalNext = Request.Url.AbsoluteUri + "/page2";
                        }

                    }

                    if (page > 1 && pageInfo.TotalPages > page)
                    {
                        @ViewBag.canonicalPrev = Regex.Replace(Request.Url.AbsoluteUri, sPage, pPage, RegexOptions.IgnoreCase);
                        @ViewBag.canonicalNext = Regex.Replace(Request.Url.AbsoluteUri, sPage, nPage, RegexOptions.IgnoreCase);
                    }

                    if (page > 1 && pageInfo.TotalPages == page)
                    {
                        @ViewBag.canonicalPrev = Regex.Replace(Request.Url.AbsoluteUri, sPage, pPage, RegexOptions.IgnoreCase);

                    }

                    @ViewBag.Robots = @"<meta name=""robots"" content=""noindex,follow"">";
                }

                return View(new ProductSearchViewModel
                {
                    ProductList = productListAsList.OrderBy(p => p.ProductName).Skip((page - 1) * PageSize).Take(PageSize),
                    ListForDropDown = listForQuickJump,
                    SearchTermPassed = searchTerm,
                    ContactNumber = affiliateInfo.StorefrontContact,
                    StoreFrontExchangeRate = storeFrontInfo.StoreFrontCurrencyExchangeRate,
                    PagingInfo = pageInfo
                });
                //   }
            }
            else
            {
                // display product not found page
                return View("ProductNotFound", new ProductSearchViewModel
                {
                    ProductList = null,
                    SearchTermPassed = searchTerm,
                    ContactNumber = affiliateInfo.StorefrontContact,
                    SearchType = "name"
                });
            }

        }

        // product details action when product name is selected
        // [OutputCache(CacheProfile = "cp-server")]
        public ActionResult ProductDetail(string searchTerm, AffiliateInfo affiliateInfo, Cart cart, LoggedCustomer loggedCustomer)
        {
            ViewBag.IsAutoRefill = TempData["IsAutoRefill"];
            var modifySearchRxOtc = "";

            // search only Rx products
            var productType = 0;


            ProductSearch productSearch = _repository.GetProductBasedOnUrlName(searchTerm,
                                                                         null,
                                                                         affiliateInfo.AffiliateStoreFrontFk,
                                                                         modifySearchRxOtc.ToString(),
                                                                         productType,
                                                                         true,
                                                                         true,
                                                                         true,
                                                                         true);

            productSearch.RelatedProducts = productSearch.RelatedProducts.Where(x => x.FeaturedProductId != productSearch.ProductId);

            string productDisplayClass = "";

            if (productSearch != null)
            {
                switch (productSearch.ProductId)
                {
                    case 13412:
                        //productDisplayClass = "viagra";
                        productDisplayClass = "red";
                        break;

                    case 13413:
                        //productDisplayClass = "cialis";
                        productDisplayClass = "yellow";
                        break;

                    case 13414:
                        //productDisplayClass = "finasteride";
                        productDisplayClass = "green";
                        break;
                    case 13424:
                        //productDisplayClass = "levitra";
                        productDisplayClass = "orange";
                        break;

                    case 13425:
                        productDisplayClass = "caverject";
                        break;

                    case 13429:
                        productDisplayClass = "staxyn";
                        break;
                    case 13430:
                        productDisplayClass = "valtrex";
                        break;
                    case 13431:
                        productDisplayClass = "acyclovir";
                        break;
                }

                var productGenericImage = SiteConfigurationsWc.ProductImagePath + SiteConfigurationsWc.ProductGenericImagePrefix  + productSearch.ProductId + ".png";
                var productBrandImage = SiteConfigurationsWc.ProductImagePath + SiteConfigurationsWc.ProductBrandImagePrefix + productSearch.ProductId + ".png";
                var productBgImage = "/content/images/products/product-item-" + productSearch.ProductId + ".png";
                var productBgVideo = "/content/images/video/" + productSearch.ProductId + ".mp4";


                @ViewBag.ProductUrlName = searchTerm;

                if (productSearch.ProductTypeFk == 2)
                {

                    @ViewBag.ProductType = "prescription";
                }
                else
                {

                    @ViewBag.ProductType = "otc";
                }

                var recommendedDosage = productSearch.ProductSizeList.Select(x => x.ProductSizeStoreFrontRecommendedDosage).FirstOrDefault();

                @ViewBag.ProductName = productSearch.ProductName;

                if (string.IsNullOrEmpty(productSearch.ProductStoreFrontMetaTitle))
                {
                    ViewBag.Title = "Buy " + Utility.FirstCharToUpper(searchTerm.Trim()) + " Online  | " + SiteConfigurationsWc.StorefrontUrl;

                }
                else
                {
                    ViewBag.Title = productSearch.ProductStoreFrontMetaTitle;
                }

                if (string.IsNullOrEmpty(productSearch.ProductStoreFrontMetaDescription))
                {
                    ViewBag.MetaDescription = "Buy " + searchTerm.Trim() + " Online from " + SiteConfigurationsWc.StorefrontName + ", an Online Canadian Pharmacy that provides the best quality products at a discounted rate for " + searchTerm.Trim() + ".";
                }
                else
                {
                    ViewBag.Description = productSearch.ProductStoreFrontMetaDescription;
                }

                if (string.IsNullOrEmpty(productSearch.ProductStoreFrontMetaKeyWords))
                {
                    // ViewBag.MetaKeywords = "";   
                }
                else
                {
                    //--Start: Commented for Amir on 31 Aug 2017
                    // ViewBag.MetaKeywords = productSearch.ProductStoreFrontMetaKeyWords;
                    //--End: Commented for Amir on 31 Aug 2017
                }

                // check for the product data collect flag and return template view to collect user information
                if (productSearch.ProductDataCollect == true)
                {
                    return View("ProductDetailDataCollect", new ProductDetailViewModel
                    {
                        ProductSearch = productSearch,
                        CustomerEmail = loggedCustomer.CustomerEmail
                    });
                }

                if (@SiteConfigurationsWc.ProductPageUpdateJune2021 == 1)
                {
                    return View("productDetailV2", new ProductDetailViewModel
                    {
                        ProductSearch = productSearch,
                        ProductDisplayClass = productDisplayClass,

                        ProductGenericImage = productGenericImage,
                        ProductBrandImage = productBrandImage,
                        ProductBgImage = productBgImage,
                        ProductBgVideo = productBgVideo,
                        RecommendedDosage = recommendedDosage
                    });
                }
                else
                {
                    return View("productDetail", new ProductDetailViewModel
                    {
                        ProductSearch = productSearch,
                        ProductDisplayClass = productDisplayClass,

                        ProductGenericImage = productGenericImage,
                        ProductBrandImage = productBrandImage,
                        ProductBgImage = productBgImage,
                        ProductBgVideo = productBgVideo,
                        RecommendedDosage = recommendedDosage
                    });
                }
              

            }

            else
            {
                @ViewBag.Title = "Product Not Found  | " + SiteConfigurationsWc.StorefrontUrl;

                @ViewBag.Description =
                "Buy " + searchTerm.Trim() + " Online from " + SiteConfigurationsWc.StorefrontName + ", an Online Canadian Pharmacy that provides the best quality products at a discounted rate for " + searchTerm.Trim() + ".";

                // set http status 404
                Response.StatusCode = 404;

                return View("ProductNotFound");
            }

        }

        public ActionResult GetMatchingProductJson(string searchTerm, AffiliateInfo affiliateInfo)
        {
            // get affiliates for storefront
            List<ProductSearchBox> productList = _repository.GetMatchingProducts(searchTerm, affiliateInfo.AffiliateStoreFrontFk, true, true);

            var result = productList.Take(10);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        // Save product enquiry details for the unavailable brand/generic for a product
        public JsonResult ProductEnquiry(int productId, string email, int brandGeneric, int dataCollect, AffiliateInfo affiliateInfo, LoggedCustomer loggedCustomer)
        {
            try
            {
                var eMailValidator = new System.Net.Mail.MailAddress(email);

                var enquiryId = _repository.AddProductEnquiry(productId, email, brandGeneric, loggedCustomer.CustomerId, affiliateInfo.AffiliateStoreFrontFk, dataCollect, DateTime.Now);

                if (enquiryId > 0)
                    return Json(new { enquiryId = enquiryId, message = "success" });
                else
                    return Json(null);
            }
            catch (FormatException)
            {
                return Json(new { enquiryId = 0, message = "invalid email" });
            }
            catch (Exception)
            {
                return Json(new { enquiryId = 0, message = "error" }); ;
            }
        }


        public ActionResult Products(AffiliateInfo affiliateInfo)
        {
            @ViewBag.Title = "Men Health Products - ED Products | " + SiteConfigurationsWc.StorefrontName;
            @ViewBag.Description = SiteConfigurationsWc.StorefrontName +
                " offers high-quality ED products for men. From Sildenafil to Tadalafil, " + SiteConfigurationsWc.StorefrontName +
                "  has got you covered! Order ED meds discretely online.";
         
            List<FeaturedProduct> products = _repository.GetAllProducts(affiliateInfo.AffiliateStoreFrontFk).ToList();
            return View("Products", products);
        }
    }
}