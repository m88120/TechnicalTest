using System;
using System.Collections.Generic;

namespace TechnicalCore.Context
{
    public partial class Exams
    {
        public Exams()
        {
            ExamQuestions = new HashSet<ExamQuestions>();
            ExamSessions = new HashSet<ExamSessions>();
        }

        public long TestId { get; set; }
        public string TestTitle { get; set; }       
        public DateTime? CreatedOn { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsAttachmentRequired { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }
        public ICollection<ExamQuestions> ExamQuestions { get; set; }
        public ICollection<ExamSessions> ExamSessions { get; set; }
    }
}
