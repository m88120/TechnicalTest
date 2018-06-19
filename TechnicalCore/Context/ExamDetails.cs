using System;
using System.Collections.Generic;

namespace TechnicalCore.Context
{
    public partial class ExamDetails
    {
        public ExamDetails()
        {
            ExamQuestionAnswer = new HashSet<ExamQuestionAnswer>();
        }

        public int Id { get; set; }
        public int? ExamSessonId { get; set; }
        public string UniqueId { get; set; }
        public DateTime? Createdate { get; set; }
        public TimeSpan? Createtime { get; set; }
        public DateTime? Startdate { get; set; }
        public TimeSpan? Starttime { get; set; }
        public DateTime? Enddate { get; set; }
        public TimeSpan? Endtime { get; set; }       
        public bool? Status { get; set; }
        public string Ipaddress { get; set; }
        public string FileName { get; set; }
        public int? ExamStatusId { get; set; }
        public bool? IsDeleted { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }

        public ExamSessions ExamSesson { get; set; }
        public ExamStatuses ExamStatus { get; set; }
        public ICollection<ExamQuestionAnswer> ExamQuestionAnswer { get; set; }
    }
}
