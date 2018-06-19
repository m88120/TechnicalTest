using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechnicalCore.Models;

namespace TechnicalCore.Interfaces
{
   public interface IExamSessionManager
    {
        ResponseModel<CreateSessionModel> Save(CreateSessionModel model);
        ResponseModel<Administration> View(int id);
        ResponseModel<Administration> Delete(int id, string lastUpdatedBy);
        ResponseModel<Administration> DeleteMultiple(int[] ids, string lastUpdatedBy);
        ResponseModel<List<Administration>> ListCreateSession();
        ResponseModel<CreateSessionModel> ValidateToken(string token);
        bool UpdateExamStatus(int examId, string status, string lastUpdatedBy);
        ResponseModel<Administration> Edit(int id);
        ResponseModel<string> Update(Administration administration);
        bool CheckExistingUser(CreateSessionModel model);
        bool UpdateExamStatusForMultiple(int[] ids, int status, string lastUpdatedBy);
    }
}
