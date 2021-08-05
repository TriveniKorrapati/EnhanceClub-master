using System.Collections.Generic;
using System.Web.Mvc;
using EnhanceClub.Domain.Entities;

namespace EnhanceClub.WebUI.Models
{
    // this class is used to pass customer  profile data to update Patient Profile
    public class CustomerProfileViewModel
    {
        public PatientProfile PatientProfile { get; set; }
        public IEnumerable<PatientMedication> MedicationList { get; set; }
        

        public List<SelectListItem> GenderList
        {
            get
            {
                List<SelectListItem> genderList = new List<SelectListItem>
                {
                     new SelectListItem() {Text = "Select Gender", Value = "", Selected = false},
                    new SelectListItem() {Text = "Male", Value = "m", Selected = false},
                    new SelectListItem() {Text = "Female", Value = "f", Selected = false},
                    new SelectListItem() {Text = "Prefer Not To Select", Value = "u", Selected = false},
                    new SelectListItem() {Text = "Other", Value = "o", Selected = false}
                };

                return genderList;
            }
        }
        public bool havePrescription { get; set; }

        public List<ConsultationHours> ConsultationHours { get; set; }
        public List<string> ConsultationHoursFk { get; set; }

        public List<SocialHistory> SocialHistoryList { get; set; }
    }

    
    
}