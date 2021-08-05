using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class BPQuestionnaires
    {
        public int QuestionnaireOptionId { get; set; }
        public string QuestionnaireOptionText { get; set; }
        public int QuestionnaireFk { get; set; }
        public int QuestionnaireOptionOrder { get; set; }
        public string QuestionnaireOption { get; set; }
    }
}
