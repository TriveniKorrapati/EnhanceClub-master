using EnhanceClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.Models
{
    public class PrescriptionUploadViewModel
    {
        public List<CustomerIdDocument> CustomerDocumentList { get; set; }
        public int OrderId { get; set; }
        public bool PrescriptionImageFound { get; set; }
        public string PrescriptionImageName { get; set; }
        public string IdImageName { get; set; }
        public string IdBackImageName { get; set; }
        public int CustomerId { get; set; }
        public string QuestionnaireCatId { get; set; }
        public bool backIdImageFound { get; set; }
    }
}