using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using Technical.Filters;
using TechnicalCore.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace TechnicalWeb.Controllers.ApiControllers
{
    //[JwtAuthentication]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/CommonApi")]
    public class CommonApiController : Controller
    {
        private IListManager listManager;
        public CommonApiController(IListManager _listManager)
        {
            this.listManager = _listManager;
        }
       
        [HttpGet]
        [Route("SourceList")]
        public IActionResult GetSourceList()
        {
            var result = listManager.GetSources();
            if (result.status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.message);
            }
        }

        [HttpGet]
        [Route("ExamStatusList")]
        public IActionResult GetExamStatusList()
        {
            var result = listManager.GetExamStatuses();
            if (result.status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.message);
            }
        }
        [HttpGet]
        [Route("ExamsList")]
        public IActionResult GetExamList()
        {
            var result = listManager.GetExamList();
            if (result.status)
            {
                return Ok(result.Data);
            }
            else
            {
                return BadRequest(result.message);
            }
        }

    }
}