using System.Collections.Generic;

namespace EnhanceClub.Domain.Entities
{
    // this represents one menu Tab that can have single or multiple menu items
    public class MenuTab
    {
        public IEnumerable<MenuItem> TabItems { get; set; }
        public string TabHeader { get; set; } // used if it has sub items
        public string TabUrl { get; set; }
        public string TabType { get; set; }
        
    }
}
