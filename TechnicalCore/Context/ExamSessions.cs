using System;
using System.Collections.Generic;

namespace TechnicalCore.Context
{
    public partial class ExamSessions
    {
        public ExamSessions()
        {
            ExamDetails = new HashSet<ExamDetails>();
        }

        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string EmailAddress { get; set; }
        public string Notes { get; set; }
        public int? SessionCounts { get; set; }
        public DateTime? Createdate { get; set; }
        public DateTime? Modifydate { get; set; }
        public int? SourceId { get; set; }
        public long? TestId { get; set; }

        public Sources Source { get; set; }
        public Exams Test { get; set; }
        public ICollection<ExamDetails> ExamDetails { get; set; }
    }
}
