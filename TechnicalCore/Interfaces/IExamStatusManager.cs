using System;
using System.Collections.Generic;
using System.Text;
using TechnicalCore.Models;

namespace TechnicalCore.Interfaces
{
   public interface IExamStatusManager
    {
        ResponseModel<ExamStatusModel> Save(ExamStatusModel model);
        //ResponseModel<ExamStatusModel> View(int id);
        ResponseModel<ExamStatusModel> Delete(int id);
        ResponseModel<List<ExamStatusModel>> ExamStatusList();
        ResponseModel<ExamStatusModel> Edit(int id);
        ResponseModel<ExamStatusModel> Update(ExamStatusModel model);
    }
}
