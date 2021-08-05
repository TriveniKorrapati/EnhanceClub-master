using System;
using System.Collections.Generic;
using EnhanceClub.Domain.Entities;
using System.Linq;
using System.Web;

namespace EnhanceClub.WebUI.Models
{
    public class QuestionGroupSideBarModel
    {
        public List<QuestionnaireGroup> QuestionnaireGroupList { get; set; }
        public string ActiveMenu { get; set; }
        public int ItemCount { get; set; }
    }
}