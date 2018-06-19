using System;
using System.Collections.Generic;

namespace TechnicalCore.Context
{
    public partial class Sources
    {
        public Sources()
        {
            ExamSessions = new HashSet<ExamSessions>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<ExamSessions> ExamSessions { get; set; }
    }
}
