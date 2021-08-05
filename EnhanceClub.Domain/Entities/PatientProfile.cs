using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

// Created by Rajiv S : 26 Mar 2020
namespace EnhanceClub.Domain.Entities
{
    public class PatientProfile
    {
        public int           PatientProfileId { get; set; }
        public string        PatientProfileAllergy { get; set; }
	    public string        PatientProfileFirstName { get; set; }
		public string		 PatientProfileLastName { get; set; }
	    public string		 PatientProfileSex   { get; set; }
		public string		 PatientProfileMedicalHistory { get; set; }
		public int   		 PatientProfileWeightLb { get; set; }

        private DateTime _dateMin = DateTime.MinValue;
        [DataType(DataType.DateTime)]
        public DateTime PatientProfileBirthDate
        {
            get
            {
                return (_dateMin == DateTime.MinValue) ? DateTime.Now : _dateMin;
            }
            set { _dateMin = value; }
        }
		public DateTime	     PatientProfileOwnerBirthDate { get; set; }
		public bool		     PatientProfilePet { get; set; }
		public string		 PatientProfilePhysicianFirstName { get; set; }
		public string		 PatientProfilePhysicianLastName { get; set; }
		public string		 PatientProfilePhysicianPhone { get; set; }
		public string        PatientProfilePhysicianFax { get; set; }
		public bool		     PatientProfileActive { get; set; }
        public bool          PatientProfileChildproofCap { get; set; }

        public string PatientProfileMedication { get; set; }
        public string PatientProfilePhoneNumber { get; set; }

        public string PatientName {
            get { return PatientProfileFirstName + " " + PatientProfileLastName; }
        }
        
        public bool? PatientProfileConsultationConsent { get; set; }
        public string CustomerProvinceCode { get; set; }
        public string PatientPersonalHealthNumber { get; set; }

        public string PatientProfilePastSurgeries { get; set; }

        public string PatientProfileFamilyHistoryIllness { get; set; }

        public string PatientProfileSocialHistory { get; set; }

        public string PatientProfileHerbalSupplements { get; set; }

        public string PatientProfileGenderOther { get; set; }
    }
}
