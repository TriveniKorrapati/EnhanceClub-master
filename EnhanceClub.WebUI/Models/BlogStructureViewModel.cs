using System;

namespace EnhanceClub.WebUI.Models
{
    public class BlogStructureViewModel
    {
        public string PageUrl { get; set; }
        public string MainImageUrl { get; set; }
        public string MainImagePath { get; set; }
        public DateTime? BlogDateCreated { get; set; }
        public DateTime? BlogDateModified { get; set; }
    }
}