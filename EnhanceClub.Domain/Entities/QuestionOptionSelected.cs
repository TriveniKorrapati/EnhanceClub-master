using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EnhanceClub.Domain.Entities
{
   public class QuestionOptionSelected
    {
       public int QuestionnaireId { get; set; }
       public int QuestionnaireCategoryId { get; set; }
       public int QuestionnaireOptionId { get; set; }
       [Required]
       public bool isSelected { get; set; }
       public string AnswerText { get; set; }
       public string ExplanationText { get; set; }
        public string ProductName { get; set; }
        public bool HardStop { get; set; }
        public int AskMoreInfoMulOption { get; set; }
    }
}
