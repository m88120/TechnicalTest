using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using Technical.Filters;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;
using Microsoft.AspNetCore.Mvc;
using TechnicalCore.Utilities;
using TechnicalWeb.Filters;

namespace TechnicalWeb.Controllers.ApiControllers
{
    [Produces("application/json")]
    [Route("api/AccountApi")]
    public class AccountApiController : Controller
    {
        IAccountManager _accountManager = null;
        JwtAuthentication JwtAuthentication;
        public AccountApiController(IAccountManager accountManager, JwtAuthentication _JwtAuthentication)
        {
            _accountManager = accountManager;
            JwtAuthentication = _JwtAuthentication;
        }

        //[Route("Login/{url}")]
        //[HttpGet]
        //public IHttpActionResult FacebookLogin(string url)
        //{
        //    string facebookurl = AppUtilities.GetFacebookUrl(url);
        //    return Ok(facebookurl);
        //}
        //[Route("Facebook")]
        //[HttpPost]
        //public IHttpActionResult Facebook(FacebookCode code)
        //{
        //    if (!string.IsNullOrWhiteSpace(code.Code))
        //    {
        //        ResponseModel<FacebookResponseModel> response = new ResponseModel<FacebookResponseModel>();
        //        response = _accountManager.FacebookLoginApi (code.Code.ToString());
        //        if (response.status)
        //        {
        //            // AppUtilities.SetCookiesData(response.Data);
        //            return Ok(response.Data);
        //        }
        //        else
        //        {
        //            return BadRequest(response.message);
        //        }
        //    }
        //    // TempData["Error"] = "Error occurred. Please try again.";
        //    return BadRequest("Error occurred. Please try again.");
        //}

        //[Route("SignUp/{url}")]
        //[HttpGet]
        //public IHttpActionResult SignUp(string url)
        //{
        //    string facebookurl = AppUtilities.GetFacebookUrl(url);
        //    return Ok(facebookurl);
        //}
        //[Route("FacebookSignUp")]
        //[HttpPost]
        //public IHttpActionResult FacebookSignUp(FacebookCode code)
        //{
        //    if (!string.IsNullOrWhiteSpace(code.Code))
        //    {
        //        ResponseModel<FacebookResponseModel> response = new ResponseModel<FacebookResponseModel>();
        //        response = _accountManager.FacebookSignUpApi(code.Code.ToString());
        //        if (response.status)
        //        {
        //            // AppUtilities.SetCookiesData(response.Data);
        //            return Ok(response.Data);
        //        }
        //        else
        //        {
        //            return BadRequest(response.message);
        //        }
        //    }
        //    // TempData["Error"] = "Error occurred. Please try again.";
        //    return BadRequest("Error occurred. Please try again.");
        //}


        [Route("Login")]
        [HttpGet]
        public IActionResult Office365SignIn(string callbackUrl)
        {
            string officeUrl = ADAuthUtils.GetLoginUrl(callbackUrl);
            return Ok(officeUrl);
        }

        [Route("Office365")]
        [HttpPost]
        public IActionResult Office365([FromBody]Dictionary<string,string> code)
        {
            if (!string.IsNullOrWhiteSpace(code["code"]))
            {

                var accessTokenRes = ADAuthUtils.GetAccessToken(code["code"], code["callback"]);

                if (!accessTokenRes.status)
                {
                    //return BadRequest("Code is not valid.");
                    return BadRequest(accessTokenRes.message);
                }
                var claimData = ADAuthUtils.GetClaimData(accessTokenRes.Data.Id_Token);

                ResponseModel<Office365Model> response = new ResponseModel<Office365Model>();
                response = _accountManager.OfficeLoginApi(claimData);
                if (response.status)
                {
                    var tokenData = JwtAuthentication.GenerateToken(response.Data.Email);
                    response.Data.Token = tokenData.Data.Token;
                    response.Data.RefreshToken = tokenData.Data.RefreshToken;
                    response.Data.ExpiresIn = tokenData.Data.ExpiresIn;
                   //response.Data.Token = JwtAuthentication.BuildToken(response.Data.Email,response.Data.GivenName);
                    return Ok(response.Data);
                }
                else
                {
                    return BadRequest(response.message);
                }
            }
            return BadRequest("Error occurred. Please try again.");
        }

        [Route("SignUp/{url}")]
        [HttpGet]
        public IActionResult SignUp(string url)
        {
            string officeUrl = ADAuthUtils.GetLoginUrl(url);
            return Ok(officeUrl);
        }

        [Route("OfficeSignUp")]
        [HttpPost]
        public IActionResult Office365SignUp(string code,string callbackurl)
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                var accessTokenRes = ADAuthUtils.GetAccessToken(code, callbackurl);
                if (!accessTokenRes.status)
                {
                    return BadRequest("Code is not valid.");
                }
                var claimData = ADAuthUtils.GetClaimData(accessTokenRes.Data.Id_Token);
                ResponseModel<Office365Model> response = new ResponseModel<Office365Model>();
                response = _accountManager.Office365SignUpApi(claimData);
                if (response.status)
                {
                    return Ok(response.Data);
                }
                else
                {
                    return BadRequest(response.message);
                }
            }
            return BadRequest("Error occurred. Please try again.");
        }
        [HttpGet]
        public string JWTToken(string email)
        {
            //return JwtAuthenticationAttribute.GenerateToken(email);
            return "";
        }
        [HttpPost("RefreshToken")]
        public IActionResult RefreshToken([FromBody]JWModel parameters)
        {
            if (parameters == null)
            {
                return Json(new ResponseModel<Office365Model>
                {
                    status = false,
                    message = "null of parameters",
                    Data = null
                });
            }            
           // if (parameters.GrantType == "refresh_token")
           // {
                var result = JwtAuthentication.DoRefreshToken(parameters);
                if (result.status)
                {
                    return Ok(new { Token = result.Data.Token, RefreshToken = result.Data.RefreshToken, ExpiresIn = result.Data.ExpiresIn, Email = result.Data.Email });
                }
                else
                {
                    return Json(result);
                }
            //}
            //else
            //{
            //    return Json(new ResponseModel<Office365Model>
            //    {
            //        status = false,
            //        message = "bad request",
            //        Data = null
            //    });
            //}
        }
      
    }
}
