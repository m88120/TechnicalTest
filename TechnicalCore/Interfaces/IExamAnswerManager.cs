using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalCore.Models;

namespace TechnicalCore.Interfaces
{
    public interface IExamAnswerManager
    {
        ResponseModel<ExamAnswerModel> Save(List<QuestionsList> modelList, int? ExamDetailId);
        ResponseModel<ExamAnswerModel> Update(List<ExamAnswerModel> modelList, long TestId);
    }
}
