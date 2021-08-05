using System.Collections.Generic;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    public class SearchLetterViewModel
    {
        public AffiliateInfo AffiliateInfo { get; set; }
        public IEnumerable<ProductAlphabet> ProductAlphabet { get; set; }
        public string AlphaLink { get; set; }
    }
}