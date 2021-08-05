using EnhanceClub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.Models
{
    public class RequirePrescriptionViewModel
    {
        public List<Questionnaire> QuestionnaireList { get; set; }
        public List<QuestionnaireOption> QuestionnaireOptionList { get; set; }
        public List<QuestionnaireGroup> QuestionnaireGroupList { get; set; }
        public string QuestionnaireCatId { get; set; }
        public int OrderId { get; set; }

        public int CustomerSessionTrackerFk { get; set; }

    }
}