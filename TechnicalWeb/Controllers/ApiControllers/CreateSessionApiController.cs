using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using Technical.Filters;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;
using TechnicalCore.Utilities;

namespace TechnicalWeb.Controllers.ApiControllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    //[JwtAuthentication]
    [Produces("application/json")]
    [Route("api/CreateSessionApi")]
    public class CreateSessionApiController : Controller
    {
        private IExamSessionManager examsession;
        IHttpContextAccessor currentContext;
        private IHostingEnvironment env;
        public CreateSessionApiController(IExamSessionManager _examsession, IHttpContextAccessor _currentContext, IHostingEnvironment _env)
        {
            this.examsession = _examsession;
            this.currentContext = _currentContext;
            this.env = _env;
        }
        [Route("TestList")]
        [HttpGet]
        public IActionResult GetTestList()
        {
            if (User.Identity.IsAuthenticated) {

            }
            var result = examsession.ListCreateSession();
            if (result.status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.message);
            }
        }
        [Route("View/{id}")]
        [HttpGet]
        public IActionResult GetTestDetails(int id)
        {
            var result = examsession.View(id);
            if (result.status)
            {

                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.message);
            }
        }
        [Route("Delete/{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var LastUpdatedBy = ADAuthUtils.GetLoggedUserEmail(HttpContext);
            var result = examsession.Delete(id,LastUpdatedBy);
            if (result.status)
            {

                return Json(new { isSuccess = true });
            }
            else
            {
                return Json(new { isSuccess = false });
            }
        }
        [Route("DeleteMultiple")]
        [HttpPost]
        public IActionResult DeleteMultipleRecords([FromBody]List<int> Ids)
        {
            var LastUpdatedBy = ADAuthUtils.GetLoggedUserEmail(HttpContext);
            var result = examsession.DeleteMultiple(Ids.ToArray(),LastUpdatedBy);
            if (result.status)
            {

                return Json(new { isSuccess = true });
            }
            else
            {                
                return Json(new { isSuccess = false });
            }
        }
        [Route("UpdateStatusForMultiple")]
        [HttpPost]
        public IActionResult UpdateStatusForMultipleRecords([FromBody]Technical.Models.UpdateMultiRecords records)
        {
            List<int> Ids = records.Ids;
            int statusId = records.StatusId;
            var LastUpdatedBy = ADAuthUtils.GetLoggedUserEmail(HttpContext);
            var result = examsession.UpdateExamStatusForMultiple(Ids.ToArray(), statusId,LastUpdatedBy);
            if (result)
            {
                return Json(new { isSuccess = true });
            }
            else
            {
                return Json(new { isSuccess = false });
            }
        }
        [Route("Create")]
        [HttpPost]
        public IActionResult Create([FromBody]CreateSessionModel model)
        {
            if (ModelState.IsValid)
            {
                var result = examsession.Save(model);
                if (result.status)
                {
                    string url= "ValidateToken/" + result.message;
                    //string url = HttpContext.Current.Request.Url.Host + "/api/CreateSessionApi/ValidateToken/" + result.message;
                    return Ok(url);
                }
                else
                {
                    return BadRequest(result.message);
                }
            }
            else
            {
                return BadRequest(ModelState);
            }
        }
        [Route("Edit/{id}")]
        [HttpGet]
        public IActionResult EditTestDetails(int id)
        {
            var result = examsession.Edit(id);
            if (result.status)
            {
                System.Collections.Generic.List<QuestionAnswerListModel> lst = new System.Collections.Generic.List<QuestionAnswerListModel>();
                foreach (var item in result.Data.listQuestionAnswer)
                {
                    QuestionAnswerListModel obj = new QuestionAnswerListModel();
                    obj.AnswerId = item.AnswerId;
                    obj.Answer = item.Answer;
                    obj.QuestionId = item.QuestionId;
                    obj.Question = item.Question.Question;
                    // obj.Question = item.ExamQuestion.Question;
                    obj.TestId = item.ExamDetailId;
                    lst.Add(obj);
                }
                return Ok(new { ID = result.Data.Id, FirstName = result.Data.FirstName, LastName = result.Data.LastName, Notes = result.Data.Notes, Source = result.Data.Source, ExamStatus = result.Data.ExamStatus,TestId=result.Data.TestId, QuestionsAnswerList= lst });
            }
            else
            {
                return BadRequest(result.message);
            }
        }
        [Route("Update")]
        [HttpPut]
        public IActionResult UpdateTestDetails([FromBody] Administration model)
        {
            var result = examsession.Update(model);
            if (result.status)
            {
                return Ok("Done");
            }
            else
            {
                return BadRequest(result.message);
            }
        }
        [Route("ValidateToken/{id}")]
        [HttpGet]
        public IActionResult token(string id)
        {
            var resultModel = examsession.ValidateToken(id);
            if (resultModel.status)
            {
                var data = new { ExamDetailId= resultModel.Data.ExamDetailId, FirstName= resultModel.Data.FirstName , LastName= resultModel.Data.LastName };
                return Ok(data);
            }
            else
            {              
                return BadRequest(resultModel.message);
            }
        }
      
        [AllowAnonymous()]
        [HttpGet]
        public HttpResponseMessage FileDownload(string fileName)
        {
            var Folderpath = Path.Combine(env.WebRootPath, "TestFiles");
            var path = Path.Combine(Folderpath, Path.GetFileName(fileName.ToString()));
           // var path = Path.Combine(HttpContext.Current.Server.MapPath("~/TestFiles/"), Path.GetFileName(fileName.ToString()));
            if (path != null)
                return FileAsAttachment(path, fileName);
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }
        public static HttpResponseMessage FileAsAttachment(string path, string filename)
        {
            
            if (System.IO.File.Exists(path))
            {
                HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                var stream = new FileStream(path, FileMode.Open);
                result.Content = new StreamContent(stream);
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment");
                result.Content.Headers.ContentDisposition.FileName = filename;
                return result;
            }
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

    }
}
