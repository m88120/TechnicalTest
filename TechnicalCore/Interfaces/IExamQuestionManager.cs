using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalCore.Models;

namespace TechnicalCore.Interfaces
{
   public interface IExamQuestionManager
    {
        ResponseModel<ExamQuestionModel> Save(List<QuestionsList> modelList, long TestId);      
        ResponseModel<List<ExamQuestionModel>> ListExamQuestionsByExam(int TestId);
        ResponseModel<ExamQuestionModel> Update(List<QuestionsList> modelList, long TestId);
    }
}
