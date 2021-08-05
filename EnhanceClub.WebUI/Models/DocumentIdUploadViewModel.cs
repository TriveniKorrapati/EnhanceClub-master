using EnhanceClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.Models
{
    public class DocumentIdUploadViewModel
    {
        public List<CustomerIdDocument> CustomerDocumentList { get; set; }
        public LoggedCustomer LoggedCustomer { get; set; }
        public int StoreFrontFk { get; set; }
        public bool IdImageFound { get; set; }
        public string IdImageName { get; set; }
        public string IdBackImageName { get; set; }
        public int OrderId { get; set; }

        public bool CustomerDocumentIdValid { get; set; }

        public bool HavePrescription { get; set; }

        public bool IsMobileDevice { get; set; }
    }
}