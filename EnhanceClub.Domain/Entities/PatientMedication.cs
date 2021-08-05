 using System;

namespace EnhanceClub.Domain.Entities
{
    public class PatientMedication
    {
        public int PatientMedicationId { get; set; }
        public int PatientProfileId { get; set; }
        public string PatientMedicationDrugName { get; set; }
        public string PatientMedicationEffectiveness { get; set; }
        public string PatientMedicationFrequency { get; set; }
        public string PatientMedicationHowLong { get; set; }
        public string PatientMedicationIllness { get; set; }
        public string PatientMedicationStrength { get; set; }
        public DateTime PatientMedicationDateCreated { get; set; }
        public DateTime PatientMedicationLastModified { get; set; }

    }
}
