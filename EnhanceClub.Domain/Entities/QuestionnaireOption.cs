using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
    public class QuestionnaireOption
    {
        public int QuestionOptionId { get; set; }
        public string QuestionOptionText { get; set; }
        public int QuestionnaireFk { get; set; }
        public int QuestionOptionAskOrder { get; set; }
        public bool QuestionOptionAskMoreInfo { get; set; }
        public bool QuestionOptionNone { get; set; }
        public DateTime dateCreated { get; set; }
        public bool HardStop { get; set; }

        public string QuestionOptionPlaceHolder {get; set;}

    }
}
