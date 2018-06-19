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
    public class ExamAnswerManager : IExamAnswerManager
    {
        DbLeonContext _context;
        public ExamAnswerManager(DbLeonContext dbcontext)
        {
            this._context = dbcontext;
        }
        public ResponseModel<ExamAnswerModel> Save(List<QuestionsList> modelList, int? ExamDetailId)
        {
            ResponseModel<ExamAnswerModel> result = new ResponseModel<ExamAnswerModel> { Data = new ExamAnswerModel() };
            try
            {
                // foreach (var model in modelList.Where(x => !String.IsNullOrWhiteSpace(x.Name)).ToList())
                foreach (var model in modelList)
                {
                    ExamQuestionAnswer db = new ExamQuestionAnswer();
                    db.ExamDetailId = ExamDetailId;
                    db.Answer = model.Name;
                    db.CreatedOn = DateTime.Now;
                    db.QuestionId = model.Id;
                    _context.ExamQuestionAnswer.Add(db);
                }
                modelList = null;
                _context.SaveChanges();
                result = new ResponseModel<ExamAnswerModel> { status = true, message = "Success" };
            }
            catch (Exception ex)
            {
                result.status = false;
                result.message = ex.Message;
            }
            return result;
        }

        public ResponseModel<ExamAnswerModel> Update(List<ExamAnswerModel> modelList, long TestId)
        {
            throw new NotImplementedException();
        }
    }
}
