using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;
using TechnicalCore.Utilities;

namespace TechnicalWeb.Controllers.ApiControllers
{
    [Produces("application/json")]
    [Route("api/ManageExamApi")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExamApiController : Controller
    {
        private IExamManager examManager;
        private IExamQuestionManager examQuestionManager;
        public ExamApiController(IExamManager _examManager, IExamQuestionManager _examQuestionManager)
        {
            this.examManager = _examManager;
            this.examQuestionManager = _examQuestionManager;
        }
        // GET api/<controller>
        [HttpGet]
        [Route("ExamList")]
        public IActionResult Get()
        {
           var result= examManager.ListExams();
           
            if (result.status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.message);
            }
        }

        // GET api/<controller>/5
        [HttpGet]
        [Route("EditExam/{id}")]
        public IActionResult Edit(int id)
        {
            var result = examManager.Edit(id);
            if (result.status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.message);
            }
        }

        // POST api/<controller>
        [HttpPost]
        [Route("Save")]
        public IActionResult Save([FromBody]ExamModel model)
        {
            model.LastUpdatedBy = ADAuthUtils.GetLoggedUserEmail(HttpContext);
            model.LastUpdatedOn = DateTime.Now;
            var result = examManager.Save(model);
            if (result.status)
            {
                if (result.Data.TestId > 0)
                {
                    examQuestionManager.Save(model.Questions, result.Data.TestId);
                }
                return Ok(new { TestId =result.Data.TestId, TestTitle =result.Data.TestTitle});
            }
            else
            {
                return BadRequest(result.message);
            }
        }
        [HttpPut]
        [Route("Update")]
        // PUT api/<controller>/5
        public IActionResult Update([FromBody]ExamModel model)
        {            
            model.LastUpdatedBy= ADAuthUtils.GetLoggedUserEmail(HttpContext);
            model.LastUpdatedOn = DateTime.Now;
            var result = examManager.Update(model);
            if (result.status)
           {
                if (model.TestId > 0)
                {
                    examQuestionManager.Update(model.Questions, model.TestId);
                }
                return Ok(result.message);
            }
            else
            {
                return BadRequest(result.message);
            }
        }

        [HttpDelete]
        [Route("Delete/{Id}")]
        // DELETE api/<controller>/5
        public IActionResult Delete(int id)
        {

            var LastUpdatedBy = ADAuthUtils.GetLoggedUserEmail(HttpContext);
            var result = examManager.Delete(id, LastUpdatedBy);
            if (result.status)
            {
                return Ok(result.message);
            }
            else
            {
                return BadRequest(result.message);
            }
        }
    }
}