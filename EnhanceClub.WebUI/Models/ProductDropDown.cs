using System.Web;

namespace EnhanceClub.WebUI.Models
{
    // used for quick jump dropdowns 
    // this class is scaled down version of ProductSearch.cs because I don't want to pass full ProductSearch Model to Build dropdown which only needs few fields

    public class ProductDropDown
    {
        public string ProductName { get; set; }
        public string ProductStoreFrontSafeUrlName { get; set; }
        public string LetterSearched { get; set; }
        public string ProductClass { get; set; }

        public string UrlAction
        {
            get
            {
                string urlAction = "http://" + HttpContext.Current.Request.Url.Authority;
                
                if (ProductName != null && ProductName.Contains("Quick Jump"))
                {
                    // in this case send back to search page for search term typed
                    urlAction += "/search/" + ProductStoreFrontSafeUrlName;
                }
                else
                {

                    urlAction += "/products/" + ProductStoreFrontSafeUrlName;
                }


                return urlAction;
            }
        }


        // property to return product detail url for Alphbet Search
        public string UrlAlphabet
        {
            get
            {
                string urlAction = "http://" + HttpContext.Current.Request.Url.Authority;

                if (ProductName.Contains("Quick Jump"))
                {
                    // in this case send back to prescription page for serachLetter typed
                    urlAction += "/" + ProductClass + "/" + LetterSearched.Trim();
                }
                else
                {

                    urlAction += "/products/" + ProductStoreFrontSafeUrlName;
                }


                return urlAction;
            }
        }
    }
}