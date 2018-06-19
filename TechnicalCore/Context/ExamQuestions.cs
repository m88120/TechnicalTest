using System;
using System.Collections.Generic;

namespace TechnicalCore.Context
{
    public partial class ExamQuestions
    {
        public ExamQuestions()
        {
            ExamQuestionAnswer = new HashSet<ExamQuestionAnswer>();
        }

        public long QuestionId { get; set; }
        public long? ExamId { get; set; }
        public string Question { get; set; }
        public DateTime? CreatedOn { get; set; }
        public bool? IsActive { get; set; }

        public Exams Exam { get; set; }
        public ICollection<ExamQuestionAnswer> ExamQuestionAnswer { get; set; }
    }
}
