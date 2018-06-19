using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Technical.Models
{
    public class CreateSessionModel
    {
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
        [Required]
        [Display(Name = "Notes")]
        public string Notes { get; set; }

        public string emailCode { get; set; }

    }
    public class TestModel
    {
        public string Id { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }

    }
    public class ExamDetail
    {
        public int Id { get; set; }
        public Nullable<int> ExamSessonId { get; set; }
        public string UniqueId { get; set; }
        public Nullable<System.DateTime> Createdate { get; set; }

        public Nullable<System.TimeSpan> Createtime { get; set; }

        public Nullable<System.DateTime> Startdate { get; set; }

        public Nullable<System.TimeSpan> Starttime { get; set; }

        public Nullable<System.DateTime> Enddate { get; set; }

        public Nullable<System.TimeSpan> Endtime { get; set; }
       
        public Nullable<bool> Status { get; set; }
        public string Ipaddress { get; set; }
        public string FileName { get; set; }

        public IFormFile File { get; set; }

    }
    public class UpdateMultiRecords
    {
       public List<int> Ids { get; set; }
       public int StatusId { get; set; }
    }
}