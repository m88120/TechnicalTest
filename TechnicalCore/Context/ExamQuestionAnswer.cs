using System;
using System.Collections.Generic;

namespace TechnicalCore.Context
{
    public partial class ExamQuestionAnswer
    {
        public long AnswerId { get; set; }
        public long? QuestionId { get; set; }
        public string Answer { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ExamDetailId { get; set; }

        public ExamDetails ExamDetail { get; set; }
        public ExamQuestions Question { get; set; }
    }
}
