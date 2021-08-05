using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class QuestionnaireCategoryResponse
    {
        public int CustomerQuestionnaireCategoryResponseId { get; set; }
        public int CustomerQuestionnaireCategoryResponseApprove { get; set; }
        public DateTime CustomerQuestionnaireCategoryResponseDateCreated { get; set; }

        public int CustomerPrescriptionId { get; set; }

        public int CustomerPrescriptionIsValidated { get; set; }

        public int CustomerPrescriptionApproved { get; set; }

        public bool CustomerPrescriptionRefillAuthorization { get; set; }

        public int CustomerPrescriptionRefillCount { get; set; }
    }
}
