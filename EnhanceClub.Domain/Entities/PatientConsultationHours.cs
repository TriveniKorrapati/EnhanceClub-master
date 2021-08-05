using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class PatientConsultationHours
    {
        
        public int PatientConsultationDetailsId { get; set; }
        public int PatientConsultationHoursFk { get; set; }
        public int PatientConsultationHoursCustomerFk { get; set; }
        public bool PatientConsultationHoursActive { get; set; }
        public DateTime PatientConsultationPreferenceHourStartDate { get; set; }
        public DateTime PatientConsultationPreferenceHourEndDate { get; set; }
    }
}
