using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalCore.Models
{
   public class QuestionAnswerListModel
    {
        public long? QuestionId { get; set; }
        public string Question { get; set; }
        public long AnswerId { get; set; }
        public string Answer { get; set; }
        public int? TestId { get; set; }
    }
}
