using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using TechnicalCore.Context;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;
using TechnicalCore.Utilities;

namespace TechnicalCore.Managers
{
    public class TechinicalTestManager : ITechinicalTest
    {
        DbLeonContext _context;
        public TechinicalTestManager(DbLeonContext context, IOptions<AppSetting> appSettings)
        {
            this._context = context;
            ADAuthUtils.AppSettings = appSettings.Value;
        }
        /// <summary>
        /// Get the testdetails and save the startdate, time and status in exam details 
        /// </summary>
        /// <param name="examdetailId"></param>
        /// <returns></returns>
        public ResponseModel<TestModel> GetTestDetails(int examdetailId)
        {
            ResponseModel<TestModel> result = new ResponseModel<TestModel>();
            try
            {
                var examDetailModels = _context.ExamDetails.Include("ExamSesson").Where(e => e.Id == examdetailId && e.UniqueId != null).Select(e => e).FirstOrDefault();
                if (examDetailModels != null)
                {

                    if (examDetailModels.Startdate == null && examDetailModels.Starttime == null && examDetailModels.Status == null)
                    {
                        examDetailModels.Startdate = DateTime.Now;
                        examDetailModels.Starttime = DateTime.Now.TimeOfDay;
                        examDetailModels.Status = false;
                        _context.SaveChanges();
                    }
                    if (examDetailModels.ExamSesson != null && examDetailModels.ExamSesson.TestId != null)
                    {
                        _context.Exams.ToList();
                        _context.ExamQuestions.ToList();
                        result.Data = new TestModel
                        {
                            Id = Convert.ToString(examDetailModels.ExamSesson.TestId),
                            TestName = examDetailModels?.ExamSesson?.Test?.TestTitle,
                            IsAttachment = Convert.ToBoolean(examDetailModels?.ExamSesson?.Test?.IsAttachmentRequired),
                            Questions = examDetailModels.ExamSesson.Test.ExamQuestions.Where(x => x.IsActive == true).Select(e => new QuestionsList { Id = e.QuestionId, Name = e.Question }).ToList()
                        };
                    }
                    else
                    {
                        result.Data = new TestModel();
                    }
                    result.status = true;
                    result.message = examDetailModels.UniqueId;
                }
                else
                {
                    result.status = false;
                    result.message = "Test does not exists";
                }
            }
            catch (Exception ex)
            {

                result.message = ex.Message;
            }

            return result;
        }
        /// <summary>
        /// save the exam response in exam details like enddate, endime , status, solution file etc..
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel<int> SubmittedTest(ExamDetailModel model, string Serverpath)
        {
            ResponseModel<int> result = null;
            try
            {
                string FileName = string.Empty;
                if (model.IsAttachment)
                {
                    if (model.File != null)
                    {
                        var examDetailModel = _context.ExamDetails.Include("ExamSesson").Where(e => e.Id == model.Id && e.Status == false).Select(e => e).FirstOrDefault();
                        if (examDetailModel != null)
                        {
                            if (model.File != null)
                            {
                                FileName = examDetailModel.ExamSesson.Firstname + "-" + examDetailModel.ExamSesson.Lastname + "_" + Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
                                var path = Path.Combine(Serverpath, Path.GetFileName(FileName.ToString()));
                                using (var fileStream = new FileStream(path, FileMode.Create))
                                {
                                    model.File.CopyTo(fileStream);
                                }
                            }
                            examDetailModel.Enddate = DateTime.Now;
                            examDetailModel.Endtime = DateTime.Now.TimeOfDay;
                            examDetailModel.FileName = FileName;
                            examDetailModel.Status = true;
                            _context.SaveChanges();
                            var TestEnd = examDetailModel.Enddate.Value.Date.Date.ToString();
                            TimeSpan timeSpan = examDetailModel.Enddate.Value.Date.Date.Add(examDetailModel.Endtime.Value).Subtract(examDetailModel.Startdate.Value.Add(examDetailModel.Starttime.Value));

                            // var Timetaken = (int)timeSpan.TotalHours + " hour " + timeSpan.ToString("mm") + " min";
                            var resultTime = timeSpan.Days > 0 ? Convert.ToInt32(timeSpan.Hours + (24 * timeSpan.Days)) + " hour " + timeSpan.ToString("mm") + " min" : Convert.ToInt32(timeSpan.Hours) + " hour " + timeSpan.ToString("mm") + " min";
                            SendEmail(examDetailModel.ExamSesson.EmailAddress, examDetailModel.ExamSesson.Firstname + " " + examDetailModel.ExamSesson.Lastname, examDetailModel.Id, model, resultTime);
                            result = new ResponseModel<int> { status = true, message = "Success", Data = 1 };
                        }
                        else
                        {
                            result = new ResponseModel<int> { status = false, message = "Test does not exists" };
                        }
                    }

                    else if (model != null && !string.IsNullOrWhiteSpace(model.Filebase64))
                    {
                        var examDetailModel = _context.ExamDetails.Include("ExamSesson").Where(e => e.Id == model.Id && e.Status == false).Select(e => e).FirstOrDefault();

                        if (examDetailModel != null)
                        {
                            if (!string.IsNullOrWhiteSpace(model.Filebase64))
                            {
                                //  FileName = examDetailModel.ExamSesson.Firstname + "-" + examDetailModel.ExamSesson.Lastname + "_" + Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
                                //  var path = Path.Combine(Serverpath, Path.GetFileName(FileName.ToString()));
                                //using (var fileStream = new FileStream(path, FileMode.Create))
                                //{
                                //    model.File.CopyTo(fileStream);
                                //}


                                //sFileName = model.FileName;
                                FileName = examDetailModel.ExamSesson.Firstname + "-" + examDetailModel.ExamSesson.Lastname + "_" + Guid.NewGuid().ToString() + "." + Convert.ToString(model.FileName.Split('.')[1]);
                                var path = Path.Combine(Serverpath, FileName.ToString());
                                //Convert Base64 Encoded string to Byte Array.
                                byte[] imageBytes = Convert.FromBase64String(model.Filebase64);
                                File.WriteAllBytes(path, imageBytes);
                            }
                            examDetailModel.Enddate = DateTime.Now;
                            examDetailModel.Endtime = DateTime.Now.TimeOfDay;
                            examDetailModel.FileName = FileName;
                            examDetailModel.Status = true;
                            _context.SaveChanges();

                            var TestEnd = examDetailModel.Enddate.Value.Date.Date.ToString();
                            TimeSpan timeSpan = examDetailModel.Enddate.Value.Date.Date.Add(examDetailModel.Endtime.Value).Subtract(examDetailModel.Startdate.Value.Add(examDetailModel.Starttime.Value));

                            // var Timetaken = (int)timeSpan.TotalHours + " hour " + timeSpan.ToString("mm") + " min";
                            var resultTime = timeSpan.Days > 0 ? Convert.ToInt32(timeSpan.Hours + (24 * timeSpan.Days)) + " hour " + timeSpan.ToString("mm") + " min" : Convert.ToInt32(timeSpan.Hours) + " hour " + timeSpan.ToString("mm") + " min";


                            // var TestEnd = examDetailModel.Enddate.Value.Add(examDetailModel.Endtime.Value).ToString();
                            // TimeSpan timeSpan = examDetailModel.Enddate.Value.Add(examDetailModel.Endtime.Value).Subtract(examDetailModel.Startdate.Value.Add(examDetailModel.Starttime.Value));
                            // var Timetaken = (int)timeSpan.TotalHours + " hour " + timeSpan.ToString("mm") + " min";
                            //  var resultTime = timeSpan.Days > 0 ? Convert.ToInt32(timeSpan.Hours + (24 * timeSpan.Days)) + " hour " + timeSpan.ToString("mm") + " min" : Convert.ToInt32(timeSpan.Hours) + " hour " + timeSpan.ToString("mm") + " min";

                            SendEmail(examDetailModel.ExamSesson.EmailAddress, examDetailModel.ExamSesson.Firstname + " " + examDetailModel.ExamSesson.Lastname, examDetailModel.Id, model, resultTime);
                            result = new ResponseModel<int> { status = true, message = "Success", Data = 1 };
                        }
                        else
                        {
                            result = new ResponseModel<int> { status = false, message = "Test does not exists" };
                        }
                    }
                    else
                    {
                        result = new ResponseModel<int> { status = false, message = "please upload the file " };
                    }
                }
                else
                {
                    var examDetailModel = _context.ExamDetails.Include("ExamSesson").Where(e => e.Id == model.Id && e.Status == false).Select(e => e).FirstOrDefault();
                    if (examDetailModel != null)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Filebase64))
                        {
                            FileName = examDetailModel.ExamSesson.Firstname + "-" + examDetailModel.ExamSesson.Lastname + "_" + Guid.NewGuid().ToString() + "." + Convert.ToString(model.FileName.Split('.')[1]);
                            var path = Path.Combine(Serverpath, FileName.ToString());
                            //Convert Base64 Encoded string to Byte Array.
                            byte[] imageBytes = Convert.FromBase64String(model.Filebase64);
                            File.WriteAllBytes(path, imageBytes);
                        }
                        if (model.File != null)
                        {
                            FileName = examDetailModel.ExamSesson.Firstname + "-" + examDetailModel.ExamSesson.Lastname + "_" + Guid.NewGuid().ToString() + Path.GetExtension(model.File.FileName);
                            var path = Path.Combine(Serverpath, Path.GetFileName(FileName.ToString()));
                            using (var fileStream = new FileStream(path, FileMode.Create))
                            {
                                model.File.CopyTo(fileStream);
                            }
                        }
                        examDetailModel.Enddate = DateTime.Now;
                        examDetailModel.Endtime = DateTime.Now.TimeOfDay;
                        examDetailModel.FileName = FileName;
                        examDetailModel.Status = true;
                        _context.SaveChanges();
                        var TestEnd = examDetailModel.Enddate.Value.Date.Date.ToString();
                        TimeSpan timeSpan = examDetailModel.Enddate.Value.Date.Date.Add(examDetailModel.Endtime.Value).Subtract(examDetailModel.Startdate.Value.Add(examDetailModel.Starttime.Value));

                        // var Timetaken = (int)timeSpan.TotalHours + " hour " + timeSpan.ToString("mm") + " min";
                        var resultTime = timeSpan.Days > 0 ? Convert.ToInt32(timeSpan.Hours + (24 * timeSpan.Days)) + " hour " + timeSpan.ToString("mm") + " min" : Convert.ToInt32(timeSpan.Hours) + " hour " + timeSpan.ToString("mm") + " min";
                        SendEmail(examDetailModel.ExamSesson.EmailAddress, examDetailModel.ExamSesson.Firstname + " " + examDetailModel.ExamSesson.Lastname, examDetailModel.Id, model, resultTime);
                        result = new ResponseModel<int> { status = true, message = "Success", Data = 1 };
                    }
                    else
                    {
                        result = new ResponseModel<int> { status = false, message = "Test does not exists" };
                    }
                }
            }
            catch (Exception ex)
            {
                result.message = ex.Message;
            }

            return result;
        }

        private void SendEmail(string email, string name, int id, ExamDetailModel model, string Timetaken)
        {
            try
            {
                StringBuilder strAnswer = new StringBuilder();
                foreach (var item in model.Questions)
                {
                    var question = _context.ExamQuestions.Where(x => x.QuestionId == item.Id).FirstOrDefault().Question;
                    strAnswer.Append("<p><b>Question: " + question + "</b></p>");
                    strAnswer.Append("<p><b>Answer: </b></p>");
                    strAnswer.Append("<p>" + item.Name + "</p>");
                    strAnswer.Append("</br>");
                }
                var client = new SendGridClient(ADAuthUtils.AppSettings.SendGrid_AppId);

                var recipients = new List<EmailAddress>
                {
                    new EmailAddress(ADAuthUtils.AppSettings.WashingtonSendToEmail, "Washington Leon-Jordan"),
                    new EmailAddress(ADAuthUtils.AppSettings.JeffSendToEmail, "Jeff Vanzant"),
                };

                foreach (var recipient in recipients)
                {
                    var msg = new SendGridMessage()
                    {
                        From = new EmailAddress("washington.leon@innroad.com", "Test Update"),
                        Subject = "Test Completed",
                        HtmlContent = $"<strong>Hello {recipient.Name},</strong><br/> <p>Test has been completed by " + name + ". <p>" +
                        "<p>Email: " + email + "!</p>" +
                        "<p>Time Taken:" + Timetaken + "</p> " +
                        strAnswer.ToString() +
                        "<a href='https://techview.azurewebsites.net/CreateSession/View/" + id + "'>Click here to view the test</a><p>Thank you.</p>"
                        //HtmlContent = $"<strong>Hello {recipient.Name},</strong><br/> <p>Test has been completed by " + name + " email is " + email + "!</p>
                        //<a href='http://createsession.azurewebsites.net/CreateSession/View/" + id + "'>Click here to view the test</a><p>Thank you.</p>"

                    };
                    msg.AddTo(recipient);
                    var response = client.SendEmailAsync(msg);
                }

            }
            catch (Exception ex)
            {
                //
                //return false;
            }
        }

        public bool GetTestStatus(int examDetailId)
        {
            var examDetailModel = _context.ExamDetails.Where(e => e.Id == examDetailId).Select(e => e).FirstOrDefault();
            return Convert.ToBoolean(examDetailModel.Status);
        }
    }
}
