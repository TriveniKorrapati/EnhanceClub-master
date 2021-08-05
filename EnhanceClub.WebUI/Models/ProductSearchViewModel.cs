using System.Collections.Generic;
using System.Linq;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    // used for passing product search data

    public class ProductSearchViewModel
    {
        public IEnumerable<ProductSearch> ProductList { get; set; }

      
        // scaled down version of ProductsSearch for all products regardless of pagination , needed for quick jump drop down
        public  List<ProductDropDown> ListForDropDown { get; set; }

        public string SearchTermPassed { get; set; }

        public string ContactNumber { get; set; }

        public decimal StoreFrontExchangeRate { get; set; }
        
        // Used List<> for adding top option as we can not add to IEnumerable 
        //public List<ProductSearch> ProductsAsList
        //{
        //    get
        //    {
        //        var myList = FullProductList.ToList();
        //        myList.Insert(0, new ProductSearch { ProductName = "Quick Jump to...", ProductStoreFrontSafeUrlName = SearchTermPassed, LetterSearched = SearchTermPassed.Trim(), ProductClass = SearchUrlLink });
        //        return myList;
        //    }

        //}

        public List<ProductDropDown> ProductsAsListDropDown
        {
           get
            {
                var myList = ListForDropDown.ToList();
                myList.Insert(0, new ProductDropDown { ProductName = "Quick Jump to...", ProductStoreFrontSafeUrlName = SearchTermPassed, LetterSearched = SearchTermPassed.Trim(), ProductClass = SearchUrlLink });
                return myList;
            }

        }

        // used for pagination
        public PagingInfo PagingInfo { get; set; }
    
        // used to format error page when product is not found
        public string SearchType { get; set; }

        // used to send to prescription or otc from page links on top of search page
        public string SearchUrlLink { get; set; }

        public IEnumerable<ProductAlphabet> ProductAlphabet { get; set; }


    }
}