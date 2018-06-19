using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TechnicalCore.Context;

namespace TechnicalCore.Models
{
    public class CreateSessionModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Email")]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public int ExamDetailId { get; set; }
        [Required]
        [Display(Name = "Source")]
        public string Source { get; set; }
        [Required]
        [Display(Name = "Test")]
        public long? TestId { get; set; }

    }
    public class Administration
    {
        [Display(Name = "First Name:")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name = "Last Name:")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Name:")]
        public string Name { get; set; }
        public int Id { get; set; }
        [Display(Name = "Date Created:")]
        public string DateCreated { get; set; }
        [Display(Name = "DateTime Test Start:")]
        public string TestStart { get; set; }
        [Display(Name = "DateTime Test End:")]
        public string TestEnd { get; set; }
        [Display(Name = "Time Taken:")]
        public string Timetaken { get; set; }

        public string Linktozip { get; set; }

        [Display(Name = "Status:")]
        [Required]
        public string ExamStatus { get; set; }
        public string ExamStatusResult { get; set; }
        [Required]
        [Display(Name = "Source:")]
        public string Source { get; set; }
        [Display(Name = "Notes:")]
        public string Notes { get; set; }

        [Display(Name = "Test Time")]
        public TimeSpan TestTime { get; set; }

        [Required]
        [Display(Name = "Test")]
        public long? TestId { get; set; }
        [Display(Name = "Test Title")]
        public string TestTitleName { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
        public string LastUpdatedBy { get; set; }

        public List<ExamQuestionAnswer> listQuestionAnswer { get; set; }
        public int SourceId { get; set; }
        public string TestLink { get; set; }
        public string UniqueTestId { get; set; }
        public List<ClsExamQuestionAnswer> QuestionAndAnswerList { get; set; }
        //  public List<QuestionAnswerListModel>
    }
    public class AdministrationList
    {
        public int AdminSourceId { get; set; }

        public bool showAll { get; set; }

        public bool showHired { get; set; }
        public bool showRequestOnSite { get; set; }
        public int CheckDefault { get; set; }

        public string ListSort { get; set; }
        public string ListOrder { get; set; }

        public List<Administration> lstAdministration { get; set; }

    }
    public class TestModel
    {
        public string Id { get; set; }
        public string TestName { get; set; }
        public List<QuestionsList> Questions { get; set; }
        public bool IsAttachment { get; set; }

    }
    public class ExamDetailModel
    {
        public int Id { get; set; }
        public int? ExamSessonId { get; set; }
        public string UniqueId { get; set; }
        public DateTime? Createdate { get; set; }

        public TimeSpan? Createtime { get; set; }

        public DateTime? Startdate { get; set; }

        public TimeSpan? Starttime { get; set; }

        public DateTime? Enddate { get; set; }

        public TimeSpan? Endtime { get; set; }


        public Nullable<bool> Status { get; set; }
        public string Ipaddress { get; set; }
        [Required]
        public string FileName { get; set; }
        public string Filebase64 { get; set; }
        [Required]
        public IFormFile File { get; set; }
        //public string File { get; set; }

        public string ExamTitle { get; set; }
        public List<QuestionsList> Questions { get; set; }
        public bool IsAttachment { get; set; }

    }
    public class ClsExamQuestionAnswer
    {
        public long? QuestionID { get; set; }
        public string Question { get; set; }
        public long? AnswerID { get; set; }
        public string Answer { get; set; }
        public long? ExamDetailId { get; set; }
    }
}
