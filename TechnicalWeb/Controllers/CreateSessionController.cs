using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using System.Linq;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using TechnicalCore.Utilities;

namespace Technical.Controllers
{
    [Authorize(AuthenticationSchemes= CookieAuthenticationDefaults.AuthenticationScheme)]
    public class CreateSessionController : Controller
    {
        private IHostingEnvironment env;
        IHttpContextAccessor currentContext;
        private IExamSessionManager examsession;
        private List<SelectListItem> statusList;
        private List<SelectListItem> sourceList;
        private List<SelectListItem> testList;
        private List<SelectListItem> activeExamList;
        // GET: CreateSession
        public CreateSessionController(IExamSessionManager _examsession, IListManager _list, IHttpContextAccessor HttpContextProp, IHostingEnvironment _env)
        {
            this.examsession = _examsession;
            this.statusList = _list.GetExamStatuses().Data.Select(s => new SelectListItem { Text = s.Name, Value = Convert.ToString(s.Id) }).ToList();
            this.sourceList = _list.GetSources().Data.Select(s => new SelectListItem { Text = s.Name, Value = Convert.ToString(s.Id) }).ToList();
            this.testList = _list.GetExams().Data.Select(e => new SelectListItem { Text = e.TestTitle, Value = e.TestId.ToString() }).ToList();
            this.activeExamList= _list.GetExams().Data.Where(k=>k.IsActive==true).Select(e => new SelectListItem { Text = e.TestTitle, Value = e.TestId.ToString() }).ToList();
            currentContext = HttpContextProp;
            env = _env;
        }        

        public ActionResult Index(AdministrationList model = null)
        {          
            var result = examsession.ListCreateSession();
            if (result.status)
            {
                AdministrationList lst = new AdministrationList();
                lst.lstAdministration = result.Data.ToList();

                lst.AdminSourceId = model.AdminSourceId;

                lst.showAll = model.CheckDefault == 0 ? true : model.showAll;
                lst.showHired = model.CheckDefault == 0 ? true : model.showHired;
                lst.showRequestOnSite = model.CheckDefault == 0 ? true : model.showRequestOnSite;
                lst.ListOrder = model.ListOrder;
                lst.ListSort = model.ListSort;
                var lstResult = lst.lstAdministration;
                ViewBag.ExamStatuses = statusList;
                lst.CheckDefault = 1;
                if (lst.AdminSourceId > 0)
                {
                    lstResult = lstResult.Where(k => k.SourceId == model.AdminSourceId).ToList();
                    List<SelectListItem> _list = new List<SelectListItem>();
                    foreach (var item in sourceList)
                    {
                        SelectListItem _itm = new SelectListItem();
                        _itm.Text = item.Text;
                        _itm.Value = item.Value;
                        if (_itm.Value == model.AdminSourceId.ToString())
                        {
                            _itm.Selected = true;
                        }
                        else
                        {
                            _itm.Selected = false;
                        }
                        _list.Add(_itm);
                    }
                    ViewBag.Sources = _list;
                }
                else
                    ViewBag.Sources = sourceList;

                // ViewData["ShowAll"] = showAll;

                // ViewData["ShowHired"] = showHired;
                // Session["SourceId"] = sourceId;
                List<string> statusListRecords = new List<string>();
                
                if (!lst.showAll)
                {                    
                    statusListRecords.Add(ADAuthUtils.AppSettings.Fail);
                }
                if (!lst.showHired)
                {
                    statusListRecords.Add(ADAuthUtils.AppSettings.Hired);
                }
                if (!lst.showRequestOnSite)
                {
                    statusListRecords.Add(ADAuthUtils.AppSettings.RequestOnsite);
                }
                if (lst.showAll && lst.showHired && lst.showRequestOnSite)
                {                   
                    statusListRecords = statusList.Select(x => x.Value).ToList();
                    statusListRecords.Remove(ADAuthUtils.AppSettings.Fail);
                    statusListRecords.Remove(ADAuthUtils.AppSettings.Hired);
                    statusListRecords.Remove(ADAuthUtils.AppSettings.RequestOnsite);
                }

                lst.lstAdministration = lstResult.Where(x => statusListRecords.Contains(x.ExamStatus)).ToList();
                
                if (!string.IsNullOrWhiteSpace(lst.ListOrder))
                {
                    if (lst.ListOrder.ToLower() == "asc".ToLower())
                    {
                        lst.lstAdministration = lst.lstAdministration.OrderBy(x => x.GetType().GetProperty(lst.ListSort).GetValue(x, null)).ToList();
                    }
                    else
                    {
                        lst.lstAdministration = lst.lstAdministration.OrderByDescending(x => x.GetType().GetProperty(lst.ListSort).GetValue(x, null)).ToList();
                    }
                }
                return View(lst);
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                return View("Error");
            }
        }

        /// <summary>
        /// View Individual session details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult View(int id)
        {           
            var result = examsession.View(id);
            if (result.status)
            {
                ViewBag.ExamStatuses = statusList;
                return View(result.Data);
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                return View("Error");
            }
        }
        [HttpGet]
        public ActionResult ViewRecord(int id)
        {
            var result = examsession.View(id);
            if (result.status)
            {                
                ViewBag.ExamStatuses = statusList;
                return PartialView("_ViewSession", result.Data);
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                return View("Error");
            }
        }
        /// <summary>
        /// View Individual session details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Edit(int id)
        {

            var result = examsession.Edit(id);
            if (result.status)
            {
                ViewBag.ExamStatuses = statusList;
                ViewBag.Sources = sourceList;
                ViewBag.Exams = testList;
                return PartialView("_EditSession", result.Data);
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                return View("Error");
            }
        }
        [HttpPost]
        // [ValidateInput(false)]
        public ActionResult Edit(Administration model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.ExamStatuses = statusList;
                ViewBag.Sources = sourceList;
                ViewBag.Exams = testList;
                return View(model);
            }
            model.LastUpdatedBy = User.Identity.Name;
            model.LastUpdatedOn = DateTime.Now;
            var result = examsession.Update(model);
            if (result.status)
            {
                ViewBag.token = result.message;
                // return RedirectToAction("View", new { id = model.Id });
                //return RedirectToAction("Index");
                return Json(new { status="success"});
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                //return View("Error");
                return Json(new { status = "error" });
            }
        }

        /// <summary>
        /// Delete Test
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult Delete(int id)
        {
            var result = examsession.Delete(id, User.Identity.Name);
            if (result.status)
            {

                return Json(new { isSuccess = true });
            }
            else
            {
                //ViewBag.ErrorMessage = result.message;
                //return View("Error");
                return Json(new { isSuccess = false });
            }
        }
        /// <summary>
        /// Delete Test
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DeleteMultipleRecords(List<int> Ids)
        {            
            var result = examsession.DeleteMultiple(Ids.ToArray(), User.Identity.Name);
            if (result.status)
            {

                return Json(new { isSuccess = true });
            }
            else
            {
                //ViewBag.ErrorMessage = result.message;
                //return View("Error");
                return Json(new { isSuccess = false });
            }
        }
        [HttpPost]
        public ActionResult UpdateStatusForMultipleRecords(List<int> Ids, int statusId)
        {
            var result = examsession.UpdateExamStatusForMultiple(Ids.ToArray(), statusId, User.Identity.Name);
            if (result)
            {
                return Json(new { isSuccess = true });
            }
            else
            {
                return Json(new { isSuccess = false });
            }
            // return Json(new { isSuccess = true });
        }
        /// <summary>
        /// Create Session View
        /// </summary>
        /// <returns></returns>
        public ActionResult Create()
        {
            ViewBag.Sources = sourceList;
            ViewBag.Exams = activeExamList;
            return View(new CreateSessionModel());
        }
        /// <summary>
        /// Create session and send toke in  email to user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(CreateSessionModel model)
        {
            var result = examsession.Save(model);
            if (result.status)
            {
                ViewBag.token = result.message;
                return View(result.Data);
            }
            else
            {
                ViewBag.ErrorMessage = result.message;
                return View("Error");
            }

        }
        [HttpPost]
        public ActionResult CheckUser(CreateSessionModel model)
        {
            var result = examsession.CheckExistingUser(model);
            return Json(new { data = result });
        }
        [AllowAnonymous]
        //[HttpGet]
        //[Route("CreateSession/token/{id}")]
        /// <summary>
        /// Verify the token 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult token(string id)
        {
            CreateSessionModel model = new CreateSessionModel();
            var resultModel = examsession.ValidateToken(id);
            if (resultModel.status)
            {
                return RedirectToAction("Index", "TechnicalTest", new { name = resultModel.Data.FirstName + " " + resultModel.Data.LastName, examDetailId = resultModel.Data.ExamDetailId });
            }
            else
            {
                ViewBag.ErrorMessage = resultModel.message;
                return View("Error");
            }
        }
        public ActionResult Filedownload(string FileName)
        {
            var webRoot = env.WebRootPath;
            var file = System.IO.Path.Combine(webRoot, "test.txt");
            //var path = Path.Combine(Server.MapPath("~/TestFiles/"), Path.GetFileName(FileName.ToString()));
            //if (System.IO.File.Exists(Server.MapPath(path)))
            //{
            //    Response.ContentType = "application/octet-stream";
            //    Response.AddHeader("Content-Disposition", ("attachment;filename=\""
            //                    + (FileName + "\"")));
            //    Response.TransmitFile(Server.MapPath(path));
            //    Response.End();
            //}
            //else
            //{
            //    Response.StatusCode = 404;
            //    // Response.[End]()
            //}
            return View();
        }

        [HttpPost]
        public JsonResult ExamStatusUpdate(int examId, string status)
        {
            var result = examsession.UpdateExamStatus(examId, status,User.Identity.Name);
            if (result)
            {
                return Json(new { isSuccess = true });
            }
            else
            {
                return Json(new { isSuccess = false });
            }
        }

    }
}