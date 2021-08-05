using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
   public class QuestionnaireGroup
    {
        public int QuestionnaireGroupId { get; set; }
        public string QuestionnaireGroupText { get; set; }
        public int QuestionnaireCatId { get; set; }
        public int GroupCount { get; set; }
    }
}
