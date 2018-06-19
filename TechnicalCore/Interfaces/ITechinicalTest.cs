using System.Collections.Generic;
using TechnicalCore.Models;

namespace TechnicalCore.Interfaces
{
    public interface ITechinicalTest
    {
        ResponseModel<TestModel> GetTestDetails(int examdetailId);

        ResponseModel<int> SubmittedTest(ExamDetailModel model,string path);
        bool GetTestStatus(int examDetailId);

    }
}
