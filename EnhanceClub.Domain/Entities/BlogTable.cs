using System;

namespace EnhanceClub.Domain.Entities
{
    public class BlogTable
    {
        public int BlogId { get; set; }

        public string BlogHeadline { get; set; }

        public string BlogTnImage { get; set; }

        public string BlogPreview { get; set; }

        public string BlogLink { get; set; }

        public bool BlogActive { get; set; }

        public bool? BlogFeatured { get; set; }

        public DateTime? BlogDateCreated { get; set; }

        public DateTime? BlogLastModified { get; set; }

        public int? BlogStorefrontFk { get; set; }

        public string BlogStorefrontName { get; set; }

        public string BlogMetaTitle { get; set; }
        public string BlogMetaDescription { get; set; }
        public string BlogMainImage { get; set; }
    }
}
