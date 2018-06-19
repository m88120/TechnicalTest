using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalCore.Models;

namespace TechnicalCore.Interfaces
{
   public interface IExamManager
    {
        ResponseModel<ExamModel> Save(ExamModel model);
        ResponseModel<ExamModel> View(int id);
        ResponseModel<ExamModel> Delete(int id,string LastUpdatedBy);
        ResponseModel<List<ExamModel>> ListExams();
        ResponseModel<ExamModel> Edit(int id);
        ResponseModel<ExamModel> Update(ExamModel administration);
    }
}
