using System;
using System.Collections.Generic;

namespace TechnicalCore.Context
{
    public partial class ExamStatuses
    {
        public ExamStatuses()
        {
            ExamDetails = new HashSet<ExamDetails>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public ICollection<ExamDetails> ExamDetails { get; set; }
    }
}
