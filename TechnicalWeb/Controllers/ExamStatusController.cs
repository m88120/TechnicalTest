using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;

namespace TechnicalWeb.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class ExamStatusController : Controller
    {
        IExamStatusManager examStatus;
        public ExamStatusController(IExamStatusManager _examStatus)
        {
            this.examStatus = _examStatus;
        }
        public IActionResult Index()
        {
            var result = examStatus.ExamStatusList();
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
            return PartialView("_CreateEditStatus", new TechnicalCore.Models.ExamStatusModel { });
        }
        [HttpPost]
        // [ValidateInput(false)]
        public ActionResult Create(ExamStatusModel examModel)
        {
            ResponseModel<ExamStatusModel> result = null;
            if (examModel.Id == 0)
            {
                result = examStatus.Save(examModel);              
            }
            else
            {
                result = examStatus.Update(examModel);
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
            var result = examStatus.Edit(id);
            return PartialView("_CreateEditStatus", result.Data);
        }
        public IActionResult Update(ExamStatusModel model)
        {
            return Json("");
        }
        public ActionResult Delete(int id)
        {
            var result = examStatus.Delete(id);
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