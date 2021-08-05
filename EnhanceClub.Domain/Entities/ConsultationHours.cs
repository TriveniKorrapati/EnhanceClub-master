using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class ConsultationHours
    {
        public int ConsultationHoursId { get; set; }
        public string ConsultationHour { get; set; }
        public bool IsSelected { get; set; }
    }
}
