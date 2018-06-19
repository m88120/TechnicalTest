using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalCore.Context;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;

namespace TechnicalCore.Managers
{
    public class ExamManager : IExamManager
    {
        DbLeonContext _context;
        
        public ExamManager(DbLeonContext dbcontext)
        {
            this._context = dbcontext;
        }
        public ResponseModel<ExamModel> Save(ExamModel model)
        {
            ResponseModel<ExamModel> result = new ResponseModel<ExamModel> { Data = new ExamModel() };
            try
            {
                Exams db = new Exams();
                db.TestTitle = model.TestTitle;
                db.CreatedOn = DateTime.Now;
                db.IsAttachmentRequired = model.IsAttachmentRequired;
                db.IsActive = true;
                db.LastUpdatedBy = model.LastUpdatedBy;
                db.LastUpdatedOn = model.LastUpdatedOn;
                _context.Exams.Add(db);
                _context.SaveChanges();
                result = new ResponseModel<ExamModel> { status = true, message = "Success",Data= new ExamModel() { TestId=db.TestId,TestTitle=db.TestTitle} };
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = ex.Message;
                result.Data = new ExamModel() { TestId = 0, TestTitle = null };
            }
            return result;
        }
        public ResponseModel<ExamModel> Update(ExamModel exam)
        {
            ResponseModel<ExamModel> result = new ResponseModel<ExamModel> { Data = new ExamModel() };
            try
            {
                var item = _context.Exams.Where(e => e.TestId == exam.TestId).Select(e => e).FirstOrDefault();
                if (item != null)
                {
                    item.TestTitle = exam.TestTitle;
                    item.IsAttachmentRequired = exam.IsAttachmentRequired;
                    item.IsActive = true;
                    item.LastUpdatedBy = exam.LastUpdatedBy;
                    item.LastUpdatedOn = DateTime.Now;
                    _context.SaveChanges();
                    result = new ResponseModel<ExamModel> { status = true, message = "Success" };
                }
                else
                {
                    result = new ResponseModel<ExamModel> { status = false, message = "Test Not Found" };
                }
            }
            catch (Exception ex)
            {
                result = new ResponseModel<ExamModel> { status = false, message = ex.Message.ToString() };
            }
            return result;
        }
        public ResponseModel<ExamModel> Delete(int id,string LastUpdatedBy)
        {
            ResponseModel<ExamModel> result = new ResponseModel<ExamModel> { Data = new ExamModel() };
            try
            {
                var item = _context.Exams.Where(e => e.TestId == id).Include("ExamQuestions").Include("ExamSessions").Select(e => e).FirstOrDefault();
                if (item != null)
                {
                    // _context.Exams.Remove(item);
                    item.LastUpdatedBy = LastUpdatedBy;
                    item.LastUpdatedOn = DateTime.Now;
                    item.IsActive = false;
                    _context.SaveChanges();
                    result.status = true;
                    result.message = "Success";
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

        public ResponseModel<ExamModel> Edit(int id)
        {
            ResponseModel<ExamModel> result = new ResponseModel<ExamModel> { Data = new ExamModel() };
            try
            {
                var item = _context.Exams.Include("ExamQuestions").Where(e => e.TestId == id).Select(e => e).FirstOrDefault();
                if (item != null)
                {
                    result.Data.TestId = item.TestId;
                    result.Data.TestTitle = item.TestTitle;
                    result.Data.CreatedOn = item.CreatedOn;
                    result.Data.IsActive = item.IsActive;
                    result.Data.IsAttachmentRequired = Convert.ToBoolean(item.IsAttachmentRequired);
                    result.Data.Questions = item.ExamQuestions.Where(x=>x.IsActive==true).Select(e =>new QuestionsList {Id=e.QuestionId, Name=e.Question }).ToList();
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
            return result;
        }

        public ResponseModel<List<ExamModel>> ListExams()
        {
            ResponseModel<List<ExamModel>> result = new ResponseModel<List<ExamModel>> { Data = new List<ExamModel>() };
            try
            {
                var lst = _context.Exams.Include("ExamQuestions").Where(k=>k.IsActive==true || k.IsActive==null).Select(e => e).ToList();
                foreach (var item in lst)
                {
                    ExamModel model = new ExamModel();
                    model.TestId = item.TestId;
                    model.TestTitle = item.TestTitle;
                    model.CreatedOn = item.CreatedOn;
                    model.IsActive = item.IsActive;
                    model.IsAttachmentRequired = Convert.ToBoolean(item.IsAttachmentRequired);
                    model.Questions = item.ExamQuestions.Where(k=>k.IsActive==true).Select(a=>new QuestionsList { Id=a.QuestionId,Name=a.Question}).ToList();
                    result.Data.Add(model);
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
        
        public ResponseModel<ExamModel> View(int id)
        {
            ResponseModel<ExamModel> result = new ResponseModel<ExamModel> { Data = new ExamModel() };
            try
            {
                var item = _context.Exams.Where(e => e.TestId == id).Select(e => e).FirstOrDefault();
                if (item != null)
                {
                    result.Data.TestId = item.TestId;
                    result.Data.TestTitle = item.TestTitle;
                    result.Data.CreatedOn = item.CreatedOn;
                    result.Data.IsActive = item.IsActive;
                    result.Data.IsAttachmentRequired = Convert.ToBoolean(item.IsAttachmentRequired);
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
            return result;
        }
    }
}
