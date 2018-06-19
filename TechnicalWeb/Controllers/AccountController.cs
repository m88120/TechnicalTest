using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TechnicalCore;
using TechnicalCore.Interfaces;
using TechnicalCore.Utilities;

namespace TechnicalWeb.Controllers
{
    public class AccountController : Controller
    {
        IHttpContextAccessor currentContext;
        IAccountManager _accountManager = null;
        //private string ClientId = ConfigurationManager.AppSettings["ClientId"];
        //private string Secret = ConfigurationManager.AppSettings["ClientSecret"];
        //private string CallbackUri = ConfigurationManager.AppSettings["RedirectUrl"];
        public AccountController(IAccountManager accountManager, IOptions<AppSetting> appSettings, IHttpContextAccessor HttpContextProp)
        {
            _accountManager = accountManager;
            ADAuthUtils.AppSettings = appSettings.Value;
            currentContext = HttpContextProp;
        }

        public IActionResult Login()
        {
            if (currentContext.HttpContext.User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "CreateSession");
            }
            else
            {
                string url = Convert.ToString(currentContext.HttpContext.Request.Query["ReturnUrl"]);
                if (!string.IsNullOrWhiteSpace(url))
                {
                    if (url.ToLower().Contains("/createsession/view/"))
                    {
                        currentContext.HttpContext.Session.SetString("LoginType", "View");
                        currentContext.HttpContext.Session.SetString("url", url);
                        // Session["LoginType"] = "View";
                        // Session["url"] = url;
                    }

                }
                return View();
            }
            
        }
        public ActionResult Office365SignIn()
        {
            currentContext.HttpContext.Session.SetString("LoginType","Login");
            string loginUrl = ADAuthUtils.GetLoginUrl();
            return Redirect(loginUrl);
        }
        public ActionResult OnAuthComplete(string code)
        {
          //  HttpContext.Session.SetString(
            if (string.IsNullOrWhiteSpace(currentContext.HttpContext.Request.Query["code"]))
            {
                return RedirectToLogin("Error occurred!");
            }
            else
            {
                var callbackUrl = ADAuthUtils.AppSettings.RedirectUri;
                var accessTokenRes = ADAuthUtils.GetAccessToken(currentContext.HttpContext.Request.Query["code"], callbackUrl);
                if (!accessTokenRes.status)
                {
                    return RedirectToLogin(accessTokenRes.message);
                }
                var claimData = ADAuthUtils.GetClaimData(accessTokenRes.Data.Id_Token);
                if (Convert.ToString(currentContext.HttpContext.Session.GetString("LoginType")) == "Login")
                {
                    var result = _accountManager.Office365Login(claimData);
                    if (result.status)
                    {
                        // currentContext.HttpContext.Session.SetString("LoginType", null);

                        var claims = new List<Claim> {
                            new Claim(ClaimTypes.Name,claimData["email"]),
                            new Claim("FullName",claimData.ContainsKey("given_name")? claimData["given_name"]:claimData["name"])
                        };
                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties { };
                        currentContext.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                        currentContext.HttpContext.Session.Remove("LoginType");
                        return RedirectToAction("Index", "CreateSession");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = result.message;
                        return View("Error");
                    }
                }
                else if (Convert.ToString(currentContext.HttpContext.Session.GetString("LoginType")) == "SignUp")
                {

                    var result = _accountManager.Office365SignUp(claimData);
                    if (result.status)
                    {
                        // currentContext.HttpContext.Session.SetString("LoginType", null);
                        currentContext.HttpContext.Session.Remove("LoginType");
                        return RedirectToAction("Index", "CreateSession");
                    }
                    else
                    {
                        ViewBag.ErrorMessage = result.message;
                        return View("Error");
                    }
                }
                else if (Convert.ToString(currentContext.HttpContext.Session.GetString("LoginType")) == "View")
                {
                    var result = _accountManager.Office365Login(claimData);
                    if (result.status)
                    {
                        //currentContext.HttpContext.Session.SetString("LoginType", null);
                        currentContext.HttpContext.Session.Remove("LoginType");
                        var url = Convert.ToString(currentContext.HttpContext.Session.GetString("url"));
                        currentContext.HttpContext.Session.Remove("url");
                        //currentContext.HttpContext.Session.SetString("", null);
                        return Redirect(url);
                    }
                    else
                    {
                        return View("Error");
                    }
                }
                else
                {
                    return View("Error");
                }

            }
        }
        public ActionResult Office365SignUp()
        {
            currentContext.HttpContext.Session.SetString("LoginType", "SignUp");
            string loginUrl = ADAuthUtils.GetLoginUrl();
            return Redirect(loginUrl);
        }
        public ActionResult RedirectToLogin(string error)
        {
            TempData["AADLoginErr"] = error;
            return RedirectToAction("Login");
        }
        [AllowAnonymous]
        public ActionResult LogOff()
        {
            currentContext.HttpContext.SignOutAsync();
            //FormsAuthentication.SignOut();
            
            currentContext.HttpContext.Session.Clear();
            currentContext.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);            
            // Session.Abandon();
            return RedirectToAction("Login", "Account");
        }
    }
}