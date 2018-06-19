using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalCore.Models
{
   public class ExamAnswerModel
    {
        public long AnswerId { get; set; }
        [Required]
        [Display(Name = "Answer")]
        public string Answer { get; set; }
        [Required]
        [Display(Name = "Question")]
        public long QuestionId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public long ExamDetailId { get; set; }
    }
    public class AnswerList
    {
        public long ExamDetailId { get; set; }
        public long QuestionId { get; set; }
        public string Question { get; set; }
        public long AnswerId { get; set; }
        public string Answer { get; set; }
    }
}
