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
    public class ExamQuestionManager : IExamQuestionManager
    {
        DbLeonContext _context;
        public ExamQuestionManager(DbLeonContext dbcontext)
        {
            this._context = dbcontext;
        } 
        public ResponseModel<List<ExamQuestionModel>> ListExamQuestionsByExam(int TestId)
        {
            ResponseModel<List<ExamQuestionModel>> result = new ResponseModel<List<ExamQuestionModel>> { Data = new List<ExamQuestionModel>() };
            try
            {
                var lst = _context.ExamQuestions.Where(e => e.ExamId == TestId && e.IsActive==true).ToList();
                foreach (var item in lst)
                {
                    ExamQuestionModel model = new ExamQuestionModel();
                    model.TestId = item.ExamId;
                    model.QuestionId = item.QuestionId;
                    model.Question = item.Question;
                    model.CreatedOn = item.CreatedOn;
                    model.IsActive = true;
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

        public ResponseModel<ExamQuestionModel> Save(List<QuestionsList> modelList, long TestId)
        {
            ResponseModel<ExamQuestionModel> result = new ResponseModel<ExamQuestionModel> { Data = new ExamQuestionModel() };
            try
            {
                foreach (var model in modelList)
                {
                    ExamQuestions db = new ExamQuestions();
                    db.ExamId = TestId;
                    db.Question = model.Name;
                    db.CreatedOn = DateTime.Now;
                    db.IsActive = true;
                    _context.ExamQuestions.Add(db);
                }
                modelList = null;
                _context.SaveChanges();
                result = new ResponseModel<ExamQuestionModel> { status = true, message = "Success" };
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = ex.Message;
            }
            return result;
        }       

        public ResponseModel<ExamQuestionModel> Update(List<QuestionsList> modelList, long TestId)
        {            
            ResponseModel<ExamQuestionModel> result = new ResponseModel<ExamQuestionModel> { Data = new ExamQuestionModel() };
            _context.ExamQuestions.Where(x => x.ExamId == TestId).ToList()
                .ForEach(a =>
                {
                    a.IsActive = false;
                }
                );
            _context.SaveChanges();
            try
            {
                foreach (var model in modelList)
                {
                    if (model.Id > 0)
                    {
                        if (!string.IsNullOrWhiteSpace(model.Name))
                        {
                            var item = _context.ExamQuestions.Where(e => e.ExamId == TestId && e.QuestionId == model.Id).Select(e => e).FirstOrDefault();
                            if (item != null)
                            {
                                item.Question = model.Name;
                                item.IsActive = true;
                                result = new ResponseModel<ExamQuestionModel> { status = true, message = "Success" };
                            }
                            else
                            {
                                result = new ResponseModel<ExamQuestionModel> { status = false, message = "Test Not Found" };
                            }
                        }
                    }
                    else
                    {
                        ExamQuestions db = new ExamQuestions();
                        db.ExamId = TestId;
                        db.Question = model.Name;
                        db.CreatedOn = DateTime.Now;
                        db.IsActive = true;
                        _context.ExamQuestions.Add(db);
                    }
                   
                }
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                result = new ResponseModel<ExamQuestionModel> { status = false, message = ex.Message.ToString() };
            }
            return result;
        }

    }
}
