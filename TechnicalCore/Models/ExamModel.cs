using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalCore.Models
{
   public class ExamModel
    {
        public long TestId { get; set; }
        [Required]
        [Display(Name = "Test Title")]
        public string TestTitle { get; set; }       
        public DateTime? CreatedOn { get; set; }
        public bool? IsActive { get; set; }        
        public bool IsAttachmentRequired { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public List<QuestionsList> Questions { get; set; }
    }
    public class ExamList
    {
        public long TestId { get; set; }
        public string TestName { get; set; }
    }
}
