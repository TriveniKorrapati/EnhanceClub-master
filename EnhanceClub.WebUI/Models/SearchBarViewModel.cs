namespace EnhanceClub.WebUI.Models
{
    // used by search bar
    public class SearchBarViewModel
    {
        public string SearchType { get; set; } // determines product type to search
        public string SearchBarLocation { get; set; } // differentiates search bar in master page with search bar in page body
    }
}