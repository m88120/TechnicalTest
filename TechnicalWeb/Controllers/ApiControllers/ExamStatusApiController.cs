using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;

namespace TechnicalWeb.Controllers.ApiControllers
{
    [Produces("application/json")]
    [Route("api/ExamStatusApi")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExamStatusApiController : Controller
    {
        private IExamStatusManager examStatusManager;
        public ExamStatusApiController(IExamStatusManager _examStatusManager)
        {
            this.examStatusManager = _examStatusManager;
        }
        // GET api/<controller>
        [HttpGet]
        [Route("StatusList")]
        public IActionResult Get()
        {
            var result = examStatusManager.ExamStatusList();
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
        [Route("EditStatus/{id}")]
        public IActionResult Edit(int id)
        {
            var result = examStatusManager.Edit(id);
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
        public IActionResult Save([FromBody]ExamStatusModel model)
        {
            var result = examStatusManager.Save(model);
            if (result.status)
            {               
                return Ok(new { TestId = result.Data.Id, TestTitle = result.Data.Name });
            }
            else
            {
                return BadRequest(result.message);
            }
        }
        [HttpPut]
        [Route("Update")]
        // PUT api/<controller>/5
        public IActionResult Update([FromBody]ExamStatusModel model)
        {
            var result = examStatusManager.Update(model);
            if (result.status)
            {               
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
            var result = examStatusManager.Delete(id);
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