using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
   public class Questionnaire
    {
       public int QuestionnaireId { get; set; }
       public string QuestionnaireText { get; set; }
       public int QuestionnaireCategoryFk { get; set; }
       public int QuestionnaireAskOrder { get; set; }
       public DateTime dateCreated { get; set; }
       public string QuestionnaireCategory { get; set; }
       public int QuestionnaireGroupId { get; set; }
       public string QuestionnaireGroupText { get; set; }
       public bool QuestionnaireMulAns { get; set; }
       public bool QuestionnaireAskMoreInfo { get; set; }

    }
}
