using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalCore.Models
{
   public class ExamQuestionModel
    {
        public long QuestionId { get; set; }
        [Required]
        [Display(Name = "Question")]
        public string Question { get; set; }
        [Required]
        [Display(Name = "Test")]
        public long? TestId { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsActive { get; set; }
    }
    public class QuestionsList
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
