using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechnicalCore.Interfaces;

namespace Technical.Controllers
{
    [Authorize]
    public class ExamQuestionController : Controller
    {
        private IExamQuestionManager _examQuestion;
        private IExamManager _exam;
        public ExamQuestionController(IExamQuestionManager examQuestion, IExamManager exam)
        {
            this._examQuestion = examQuestion;
            this._exam = exam;
        }
        // GET: ExamQuestion
        public ActionResult Index(int examId)
        {
            ViewBag.TestName = _exam.View(examId).Data.TestTitle;
            var result = _examQuestion.ListExamQuestionsByExam(examId);
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
    }
}