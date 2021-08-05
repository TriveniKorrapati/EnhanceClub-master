using EnhanceClub.Domain.Entities;
using System.Collections.Generic;

namespace EnhanceClub.WebUI.Models
{
    public class HomePageViewModel
    {
        public Cart Cart { get; set; }
        public List<BlogTable> BlogList { get; set; }
        public List<TopFeaturedProduct> FeaturedProducts { get; set; }

        public bool ShowPrescriptionNotification { get; set; }

        public bool UploadDocumentStatus { get; set; }
    }
}