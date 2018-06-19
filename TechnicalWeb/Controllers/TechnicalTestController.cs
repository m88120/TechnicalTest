using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;

namespace Technical.Controllers
{
    public class TechnicalTestController : Controller
    {
        private ITechinicalTest testManager;
        private IExamAnswerManager examAnswerManager;
        public static int examId;
        private IHostingEnvironment _env;
        public TechnicalTestController(ITechinicalTest _testManager, IExamAnswerManager _examAnswerManager, IHostingEnvironment env)
        {
            this.testManager = _testManager;
            this.examAnswerManager = _examAnswerManager;
            _env = env;
        }
        // GET: TechnicalTest
        /// <summary>
        /// Welcome Method
        /// </summary>
        /// <param name="name"></param>
        /// <param name="examDetailId"></param>
        /// <returns></returns>
        public ActionResult Index(string name, int examDetailId)
        {
            ViewBag.Name = name;
            examId = examDetailId;
            var isExamStatus = testManager.GetTestStatus(examId);
            if (isExamStatus)
            {
                ViewBag.ErrorMessage = "Test completed";
                return View("Error");
            }
            return View();
        }

        /// <summary>
        /// Start the test method
        /// </summary>
        /// <param name="examdetailId"></param>
        /// <returns></returns>
        public ActionResult TestStart()
        {
            var result = testManager.GetTestDetails(examId);
            if (result.status)
            {
                ExamDetailModel model = new ExamDetailModel();
                model.Id = examId;
                ViewBag.examdetailId = examId;
                model.Questions = result.Data.Questions;
                model.ExamTitle = result.Data.TestName;
                model.IsAttachment = result.Data.IsAttachment;
                //if (string.IsNullOrEmpty(OfficeAccessSession.AccessCode))
                //{
                //    string url = OfficeAccessSession.GetLoginUrl("onedrive.readwrite");
                //    Session["examdetailId"] = examdetailId;
                //    return new RedirectResult(url);
                //}
                return View(model);
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                return View("Error");
            }
        }
        public ActionResult SubmitResponse(int examdetailId)
        {
            ExamDetailModel model = new ExamDetailModel();
            model.Id = examdetailId;
            return View(model);
        }
        /// <summary>
        /// submitted test resoponse
        /// </summary>
        /// <param name="examdetailId"></param>
        /// <returns></returns>
        [HttpPost]
       // [ValidateInput(false)]
        public ActionResult TestStart(ExamDetailModel model)
        {
            var webRoot = _env.WebRootPath;
            var path = System.IO.Path.Combine(webRoot, "TestFiles");
           // var path = Server.MapPath("~/TestFiles/");
            var result = testManager.SubmittedTest(model, path);
            if(model.Questions!=null)
            examAnswerManager.Save(model.Questions, model.Id);
            if (result.status)
            {
                return View("ThankYou");
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                return View(model);
            }

        }

    }

}