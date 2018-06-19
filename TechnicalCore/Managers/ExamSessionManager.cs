using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
//using System.Net.Configuration;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TechnicalCore.Context;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;

namespace TechnicalCore.Managers
{
    public class ExamSessionManager : IExamSessionManager
    {
        DbLeonContext _context;
        List<Administration> lstsession;
        IHttpContextAccessor currentContext;
        public ExamSessionManager(DbLeonContext dbcontext, IHttpContextAccessor _currentContext)
        {
            this._context = dbcontext;
            currentContext = _currentContext;
        }

        /// <summary>
        /// check existing user with email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool CheckExistingUser(CreateSessionModel model)
        {
            var _exists = _context.ExamSessions.Where(x => x.EmailAddress == model.Email).FirstOrDefault();
            if (_exists == null)
                return false;
            else
                return true;
            }
        /// <summary>
        /// save and update test session
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel<CreateSessionModel> Save(CreateSessionModel model)
        {
            ResponseModel<CreateSessionModel> result = new ResponseModel<CreateSessionModel> { Data = new CreateSessionModel() };
            try
            {
                Guid obj = Guid.NewGuid();
                var _exists = _context.ExamSessions.Where(x => x.EmailAddress == model.Email).FirstOrDefault();
                if (_exists == null)
                {
                    ExamSessions db = new ExamSessions();
                    db.Firstname = model.FirstName;
                    db.Lastname = model.LastName;
                    db.Notes = model.Notes;
                    db.EmailAddress = model.Email;
                    db.Createdate = DateTime.Now;
                    db.SourceId = Convert.ToInt32(model.Source);
                    db.TestId = Convert.ToInt64(model.TestId);
                    _context.ExamSessions.Add(db);
                    _context.SaveChanges();
                    SendAccessMail(model, obj,null, db.Id);
                }
                else
                {
                    _exists.Modifydate = DateTime.Now;
                    _exists.Firstname = model.FirstName;
                    _exists.Lastname = model.LastName;
                    _exists.Notes = model.Notes;
                    _exists.SourceId = Convert.ToInt32(model.Source);
                    _exists.SessionCounts = _exists.SessionCounts == null ? 1 : _exists.SessionCounts + 1;
                    _exists.TestId = Convert.ToInt64(model.TestId);
                    _context.ExamSessions.Attach(_exists);
                    _context.Entry(_exists).State = EntityState.Modified;
                    _context.SaveChanges();
                    SendAccessMail(model, obj, null, _exists.Id);
                }

                result.status = true;
                result.message = obj.ToString();
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
            }

            return result;
        }
       

        /// <summary>
        /// Get the detail of indiviual session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<Administration> Edit(int id)
        {
            ResponseModel<Administration> result = new ResponseModel<Administration> { Data = new Administration() };
            try
            {
               // var item = _context.ExamDetails.Where(e => e.Id == id).Select(e => e).FirstOrDefault();
                var item = _context.ExamDetails.Include("ExamSesson").Include("ExamQuestionAnswer").Where(e => e.Id == id).Select(e => e).FirstOrDefault();
                _context.ExamStatuses.ToList();
                _context.Sources.ToList();
                _context.ExamQuestions.ToList();
                if (item != null)
                {
                    result.Data.Id = item.Id;
                    result.Data.FirstName = item.ExamSesson.Firstname;
                    result.Data.LastName = item.ExamSesson.Lastname;
                    result.Data.ExamStatus = Convert.ToString(item.ExamStatusId);
                    result.Data.Source = Convert.ToString(item.ExamSesson.SourceId);
                    result.Data.Notes = item.ExamSesson.Notes;
                    result.Data.TestId = item.ExamSesson.TestId;                    
                    result.Data.listQuestionAnswer = item.ExamQuestionAnswer.ToList();
                    result.status = true;
                }
                else
                {
                    result.status = false;
                    result.message = "Test Not Found";
                }
            }
            catch (Exception ex)
            {
                result.message = ex.Message;

            }

            //db.Createdate = DateTime.Now;
            return result;
        }
        public ResponseModel<string> Update(Administration administration)
        {
            ResponseModel<string> result = null;
            try
            {
                List<ExamQuestionAnswer> lstupdate = new List<ExamQuestionAnswer>();
               // var item = _context.ExamDetails.Where(e => e.Id == administration.Id).Select(e => e).FirstOrDefault();
                var item = _context.ExamDetails.Include("ExamSesson").Include("ExamQuestionAnswer").Where(e => e.Id == administration.Id).Select(e => e).FirstOrDefault();
                _context.ExamStatuses.ToList();
                _context.Sources.ToList();
                _context.ExamQuestions.ToList();
                if (item != null)
                {
                    item.LastUpdatedBy = administration.LastUpdatedBy;
                    item.LastUpdatedOn = administration.LastUpdatedOn;
                    item.ExamStatusId = Convert.ToInt32(administration.ExamStatus);
                    item.ExamSesson.TestId = Convert.ToInt64(administration.TestId);

                    item.ExamSesson.Firstname = administration.FirstName;
                    item.ExamSesson.Lastname = administration.LastName;
                    item.ExamSesson.Notes = administration.Notes;
                    item.ExamSesson.SourceId = Convert.ToInt32(administration.Source);
                    if (administration.listQuestionAnswer != null)
                    {
                        foreach (var items in administration.listQuestionAnswer)
                        {
                            var test = item.ExamQuestionAnswer.Where(x => x.AnswerId == items.AnswerId).FirstOrDefault();
                            test.Answer = items.Answer;
                            lstupdate.Add(test);
                        }
                        item.ExamQuestionAnswer = lstupdate.ToList();
                    }
                    _context.SaveChanges();
                    result = new ResponseModel<string> { status = true, message = "Success" };
                }
                else
                {
                    result = new ResponseModel<string> { status = false, message = "Test Not Found" };
                }
            }
            catch (Exception ex)
            {
                result = new ResponseModel<string> { status = false, message = ex.Message.ToString() };
            }
            return result;
        }

        /// <summary>
        /// token semd on email and save the token in exam detail
        /// </summary>
        /// <param name="model"></param>
        /// <param name="guid"></param>
        /// <param name="baseUrl"></param>
        /// <param name="Id"></param>
        public void SendAccessMail(CreateSessionModel model, Guid guid, string baseUrl, int Id)
        {
            ExamDetails db = new ExamDetails();
            db.UniqueId = guid.ToString();
            db.Createdate = DateTime.Now;
            db.Createtime = DateTime.Now.TimeOfDay;
            db.Ipaddress = GetIPAddress();
            db.ExamSesson = _context.ExamSessions.Where(e => e.Id == Id).Select(e => e).FirstOrDefault();
            db.ExamStatusId = _context.ExamStatuses.Where(e => e.Name.ToLower() == "pending").FirstOrDefault().Id;
            _context.ExamDetails.Add(db);
            _context.SaveChanges();
            //using (MailMessage mm = new MailMessage())
            //{
            //    mm.From = new MailAddress("manjinder.impinge@gmail.com"); //--- Email address of the sender
            //    mm.To.Add(new MailAddress(model.Email)); //---- Email address of the recipient.
            //    mm.Subject = "Test Link"; //---- Subject of email.

            //    string message = $"Dear {model.FirstName},<br/>Greetings,<br/><br />You have been granted access to start the test.<br/><br />";

            //    string link = $"{baseUrl}/CreateSession/token?id={HttpUtility.UrlEncode(guid.ToString())}";
            //    message += $"<a href=\"{link}\">Click here</a><br /><br />";
            //    message += "Thanks<br/><br />The Technical Team";

            //    mm.Body = "<div>" + message + "</div>"; //---- Content of email.
            //    mm.IsBodyHtml = true;
            //    using (SmtpClient smtp = new SmtpClient())
            //    {
            //        var credential = new NetworkCredential
            //        {
            //            UserName = "manjinder.impinge@gmail.com",  // replace with valid value
            //            Password = ""  // replace with valid value
            //        };
            //        smtp.Credentials = credential;
            //        smtp.Host = "smtp.gmail.com";
            //        smtp.Port = 587;
            //        smtp.EnableSsl = true;
            //        smtp.Send(mm);
            //    }
            //}

        }

        /// <summary>
        /// Get list of  created session 
        /// </summary>
        /// <returns></returns>
        public ResponseModel<List<Administration>> ListCreateSession()
        {
            ResponseModel<List<Administration>> result = new ResponseModel<List<Administration>> { Data = new List<Administration>() };
            try
            {

                var lst = _context.ExamDetails.Include("ExamSesson").Include("ExamStatus").Where(k=>k.IsDeleted==false || k.IsDeleted==null).Select(e => e).ToList().OrderByDescending(k=>k.Createdate).ThenByDescending(k => k.Createtime);
                
                lstsession = new List<Administration>();
                _context.Sources.ToList();
                _context.Exams.ToList();
                foreach (var item in lst)
                {

                    Administration model = new Administration();
                    model.Id = item.Id;
                    model.Name = item.ExamSesson.Firstname + " " + item.ExamSesson.Lastname;
                    model.DateCreated = item.Createdate.Value.ToString("MM/dd/yyyy");
                    model.ExamStatus = Convert.ToString(item.ExamStatusId);// item.ExamStatus != null ? item.ExamStatus.Name : string.Empty;
                    model.ExamStatusResult = item.ExamStatus != null ? item.ExamStatus.Name : string.Empty;
                    model.Source = item.ExamSesson.Source != null ? item.ExamSesson.Source.Name : string.Empty;
                    model.SourceId = item.ExamSesson.Source != null ? item.ExamSesson.Source.Id : 0;
                    TimeSpan ? Endtime = item.Endtime;
                    TimeSpan? Starttime = item.Starttime;

                    if (Endtime != null && Starttime != null)
                    {
                        model.TestEnd = item.Enddate.Value.Add(item.Endtime.Value).ToString();
                        //model.TestEndDateTime = Convert.ToDateTime(item.Enddate.Value.Add(item.Endtime.Value).ToString());
                        TimeSpan timeSpan = item.Enddate.Value.Add(item.Endtime.Value).Subtract(item.Startdate.Value.Add(item.Starttime.Value));
                        model.TestTime = timeSpan;
                        model.Timetaken = (int)timeSpan.TotalHours + " hour " + timeSpan.ToString("mm") + " min";
                    }
                    if (Starttime != null)
                    {
                        model.TestStart = item.Startdate.Value.Add(item.Starttime.Value).ToString();
                    }
                    model.Notes = item.ExamSesson.Notes;
                    model.Linktozip = item.FileName;
                    if (item.ExamSesson.Test != null)
                    {
                        //if(item.ExamSesson.Test.IsActive!=false)
                        result.Data.Add(model);
                    }
                }

                result.status = true;
                result.message = "success";

            }
            catch (Exception ex)
            {
                result.message = ex.Message;
            }
            return result;

        }
        /// <summary>
        /// Get the detail of indiviual session
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel<Administration> View(int id)
        {
            ResponseModel<Administration> result = new ResponseModel<Administration> { Data = new Administration() };
            try
            {
                var item = _context.ExamDetails.Include("ExamSesson").Include("ExamQuestionAnswer").Where(e => e.Id == id).Select(e => e).FirstOrDefault();
                _context.ExamStatuses.ToList();
                _context.Sources.ToList();
                _context.ExamQuestions.ToList();
                if (item != null)
                {
                    result.Data.Id = item.Id;

                    result.Data.Name = item.ExamSesson.Firstname + " " + item.ExamSesson.Lastname;
                    result.Data.DateCreated = item.Createdate.Value.ToString("MM/dd/yyyy");
                    result.Data.ExamStatus = item.ExamStatus != null ? item.ExamStatus.Name : string.Empty;
                    result.Data.Source = item.ExamSesson.Source != null ? item.ExamSesson.Source.Name : string.Empty;
                    TimeSpan? Endtime = item.Endtime;
                    TimeSpan? Starttime = item.Starttime;
                    if (Endtime != null && Starttime != null)
                    {
                        result.Data.TestEnd = item.Enddate.Value.Add(item.Endtime.Value).ToString();
                        TimeSpan timeSpan = item.Enddate.Value.Add(item.Endtime.Value).Subtract(item.Startdate.Value.Add(item.Starttime.Value));
                        result.Data.Timetaken = (int)timeSpan.TotalHours + " hour " + timeSpan.ToString("mm") + " min";
                    }

                    if (Starttime != null)
                    {
                        result.Data.TestStart = item.Startdate.Value.Add(item.Starttime.Value).ToString();
                    }
                    result.Data.Notes = item.ExamSesson.Notes;
                    result.Data.Linktozip = item.FileName;
                    result.Data.TestId =item.ExamSesson.TestId;
                    result.Data.TestTitleName = item.ExamSesson.Test==null?"------": item.ExamSesson.Test.TestTitle;
                    result.status = true;
                    result.Data.UniqueTestId = item.UniqueId;
                    //result.Data.listQuestionAnswer = item.ExamQuestionAnswer.ToList();
                    result.Data.QuestionAndAnswerList = item.ExamQuestionAnswer.Select(x => new ClsExamQuestionAnswer { AnswerID = x.AnswerId, Answer = x.Answer, Question = x.Question.Question, QuestionID = x.QuestionId, ExamDetailId = x.ExamDetailId }).ToList();


                }
                else
                {
                    result.status = false;
                    result.message = "Test Not Found";
                }
            }
            catch (Exception ex)
            {
                result.message = ex.Message;

            }

            //db.Createdate = DateTime.Now;
            return result;
        }

        public ResponseModel<Administration> Delete(int id,string lastUpdatedBy)
        {
            ResponseModel<Administration> result = new ResponseModel<Administration> { Data = new Administration() };
            try
            {
                var item = _context.ExamDetails.Include("ExamQuestionAnswer").Include("ExamSesson").Where(e => e.Id == id).Select(e => e).FirstOrDefault();
                if (item != null)
                {
                    item.LastUpdatedBy = lastUpdatedBy;
                    item.LastUpdatedOn = DateTime.Now;
                    //_context.ExamDetails.Remove(item);
                    item.IsDeleted = true;
                    _context.SaveChanges();
                    result.status = true;
                }
                else
                {
                    result.status = false;
                    result.message = "Test Not Found";
                }
            }
            catch (Exception ex)
            {
                result.message = ex.Message;

            }

            //db.Createdate = DateTime.Now;
            return result;
        }
        public ResponseModel<Administration> DeleteMultiple(int[] ids, string lastUpdatedBy)
        {
            ResponseModel<Administration> result = new ResponseModel<Administration> { Data = new Administration() };
            bool isStatus = true;
            foreach (var id in ids)
            {
                try
                {
                    var item = _context.ExamDetails.Include("ExamQuestionAnswer").Include("ExamSesson").Where(e => e.Id == id).Select(e => e).FirstOrDefault();
                    if (item != null)
                    {
                        // _context.ExamDetails.Remove(item); 
                        item.LastUpdatedBy = lastUpdatedBy;
                        item.LastUpdatedOn = DateTime.Now;
                        item.IsDeleted = true;
                        result.status = true;
                    }                    
                }
                catch (Exception ex)
                {
                    result.message = ex.Message;
                    result.status = false;
                    isStatus = false;
                    break;
                }
            }
            if(isStatus)
            _context.SaveChanges();

            //db.Createdate = DateTime.Now;
            return result;
        }
        /// <summary>
        /// Token verify from exam details 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public ResponseModel<CreateSessionModel> ValidateToken(string token)
        {
            ResponseModel<CreateSessionModel> result = new ResponseModel<CreateSessionModel> { Data = new CreateSessionModel() };
            try
            {
                //var model = _context.ExamDetails.Include("ExamSesson").Where(x => (x.UniqueId == token && x.Status == null) || (x.UniqueId == token && x.Status == false)).FirstOrDefault();
                var model = _context.ExamDetails.Include("ExamSesson").Where(x => (x.UniqueId == token)).FirstOrDefault();
                if (model != null)
                {
                    if (model.IsDeleted == true)
                    {
                        result.status = false;
                        result.message = "Test deleted";
                    }
                    else
                    {
                        if (model.Status == null || model.Status == false)
                        {
                            result.Data.FirstName = model.ExamSesson.Firstname;
                            result.Data.LastName = model.ExamSesson.Lastname;
                            //result.Data.Email = model.ExamSession.EmailAddress;
                            //result.Data.Notes = model.ExamSession.Notes;
                            result.Data.ExamDetailId = model.Id;
                            result.status = true;
                        }
                        else
                        {
                            result.status = false;
                            result.message = "Test completed";
                        }
                    }
                }
                else
                {
                    result.status = false;
                    result.message = "Test Not Found";

                }
            }
            catch (Exception ex)
            {

                result.message = ex.Message;
            }

            return result;

        }
        /// <summary>
        /// get the ipaddress
        /// </summary>
        /// <returns></returns>
        private string GetIPAddress()
        {
            // Gets the current context
            // System.Web.HttpContext context = System.Web.HttpContext.Current;

            // Checks the HTTP_X_FORWARDED_FOR Header (which can be multiple IPs)
            //string ipAddress = currentContext.HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            ////If that is not empty
            //if (!string.IsNullOrEmpty(ipAddress))
            //{
            //    // Grab the first address
            //    string[] addresses = ipAddress.Split(',');
            //    if (addresses.Length != 0)
            //    {
            //        return addresses[0];
            //    }
            //}
            var remoteAddress = currentContext.HttpContext.Connection.RemoteIpAddress.ToString();
            // Otherwise use the REMOTE_ADDR Header
            //return context.Request.ServerVariables["REMOTE_ADDR"];
            return remoteAddress;
        }

        public bool UpdateExamStatus(int examId, string status,string lastUpdatedBy)
        {
            try
            {
                var item = _context.ExamDetails.Where(e => e.Id == examId).Select(e => e).FirstOrDefault();
                if (item != null)
                {
                    item.LastUpdatedBy = lastUpdatedBy;
                    item.LastUpdatedOn = DateTime.Now;
                    item.ExamStatusId = Convert.ToInt32(status);
                    _context.Entry(item).State = EntityState.Modified;
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
        public bool UpdateExamStatusForMultiple(int[] ids, int status, string lastUpdatedBy)
        {
            bool isStatus = true;
            foreach (var examId in ids)
            {
                try
                {
                    var item = _context.ExamDetails.Where(e => e.Id == examId).Select(e => e).FirstOrDefault();
                    if (item != null)
                    {
                        item.LastUpdatedBy = lastUpdatedBy;
                        item.LastUpdatedOn = DateTime.Now;
                        item.ExamStatusId = Convert.ToInt32(status);
                        _context.Entry(item).State = EntityState.Modified;                       
                    }
                }
                catch (Exception ex)
                {
                    isStatus= false;
                    break;
                }
            }
            if(isStatus)
            _context.SaveChanges();

            return isStatus;
        }
    }
}
