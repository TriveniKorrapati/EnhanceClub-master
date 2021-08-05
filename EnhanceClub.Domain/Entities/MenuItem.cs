using System.Net;

namespace EnhanceClub.Domain.Entities
{
    public class MenuItem
    {
        public string LinkController { get; set; }
        public string LinkAction { get; set; }
        public string LinkText { get; set; }

        // ResetParam is used to reset searchLetter pro product menus , when this property is set searchLetter will be cleared so that top menu does not add it
        public bool ResetParam { get; set; } 

        public string LinkTextEncoded
        {
            get { return WebUtility.HtmlEncode(LinkText); }
        }

        public string ProductSearchTerm { get; set; }
        public string ProductType { get; set; }
        public bool HasChildMenu { get;set;}
        public bool HasChildTab { get; set; }
    }
}