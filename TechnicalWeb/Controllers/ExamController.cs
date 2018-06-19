using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using TechnicalCore.Interfaces;
using TechnicalCore.Models;

namespace Technical.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ExamController : Controller
    {
        private IExamManager _exam;
        private IExamQuestionManager _examQuestion;
        public ExamController(IExamManager exam, IExamQuestionManager examQuestion)
        {
            this._exam = exam;
            this._examQuestion = examQuestion;
        }
        // GET: Exam
        public ActionResult Index()
        {
            var result = _exam.ListExams();
            if (result.status)
            {
                return View(result.Data);
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                return View("Error");
            }            
        }
        public ActionResult Create()
        {
            return PartialView("_CreateEditTest", new TechnicalCore.Models.ExamModel { });
        }
        [HttpPost]
       // [ValidateInput(false)]
        public ActionResult Create(ExamModel examModel)
        {
            examModel.LastUpdatedBy = User.Identity.Name;
            examModel.LastUpdatedOn = DateTime.Now;
            ResponseModel<ExamModel> result = null;
            if (examModel.TestId == 0)
            {
                result = _exam.Save(examModel);
                if (result.Data.TestId > 0)
                {
                    _examQuestion.Save(examModel.Questions, result.Data.TestId);
                }
            }
            else
            {
                result = _exam.Update(examModel);
                _examQuestion.Update(examModel.Questions, examModel.TestId);
            }
            if (result.status)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                return View("Error");
            }            
        }
        [HttpGet]
        public ActionResult Edit(int id)
        {
            var result = _exam.Edit(id);
            return PartialView("_CreateEditTest", result.Data);
        }
        public ActionResult Delete(int id)
        {           
            var result = _exam.Delete(id,User.Identity.Name);
            if (result.status)
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                return View("Error");
            }
        }
    }
}