using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;

namespace TechnicalWeb.Controllers.ApiControllers
{
    //[JwtAuthentication]
    [Produces("application/json")]
    [Route("api/TechnicalTestApi")]
    public class TechnicalTestApiController : Controller
    {
        private int Id = 0;
        private ITechinicalTest testManager;
        private IExamAnswerManager examAnswerManager;
        IHttpContextAccessor currentContext;
        private IHostingEnvironment env;
        public TechnicalTestApiController(ITechinicalTest _testManager, IExamAnswerManager _examAnswerManager, IHttpContextAccessor _currentContext, IHostingEnvironment _env)
        {
            this.testManager = _testManager;
            this.examAnswerManager = _examAnswerManager;
            this.currentContext = _currentContext;
            this.env = _env;
        }
        [Route("Test")]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [Route("StartTest/{examdetailId}")]
        [HttpGet]
        public IActionResult TestStart(int examdetailId)
        {
            Id = examdetailId;
            var result = testManager.GetTestDetails(examdetailId);
            if (result.status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.message);
            }
        }

        [Route("SubmitTest")]
        [HttpPut]
        public IActionResult SubmitResponse([FromBody]ExamDetailModel model)
        {
            //ModelState["File"].Errors.Clear();
            if (ModelState["File"].Errors.Count > 0)
            {
                ModelState["File"].Errors.Clear();
                ModelState.ClearValidationState("File");
                ModelState.MarkFieldValid("File");
            }
            if (ModelState.IsValid)
            {
                var path = Path.Combine(env.WebRootPath, "TestFiles");
               // var path = currentContext.HttpContext.Current.Server.MapPath("~/TestFiles/");
                var result = testManager.SubmittedTest(model, path);              
                if (result.status)
                {
                    if (model.Questions != null)
                        examAnswerManager.Save(model.Questions, model.Id);
                    return Ok("ThankYou");
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




    }
}
