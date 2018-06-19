using System.Collections.Generic;
using TechnicalCore.Context;
using TechnicalCore.Models;

namespace TechnicalCore.Interfaces
{
    public interface IListManager
    {
        ResponseModel<List<ExamStatusModel>> GetExamStatuses();
        ResponseModel<List<SourceModel>> GetSources();
        ResponseModel<List<Exams>> GetExams();
        ResponseModel<List<ExamList>> GetExamList();
    }
}
