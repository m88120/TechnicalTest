using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Web;
//using System.Web.Security;
using TechnicalCore.Models;

namespace TechnicalCore.Utilities
{
    public class AppUtilities
    {
        public static AppSetting AppSettings;
        /// <summary>
        /// Set cookies for loged-in user
        /// </summary>
        /// <param name="model">Data to save in cookies</param>
        public static void SetCookiesData(Office365Model model)
        {
            //FormsAuthentication.SetAuthCookie(model.Email, false);
            var userData = JsonConvert.SerializeObject(model);

            //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
            //    model.Email,
            //    DateTime.Now,
            //    DateTime.Now.AddMinutes(30),
            //    true,
            //    userData,
            //    FormsAuthentication.FormsCookiePath);

            //// Encrypt the ticket.
            //string encTicket = FormsAuthentication.Encrypt(ticket);

            //// Create the cookie.
            //HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket))
        }

        public static void SetOffice365CookiesData(string response)
        {
            //JObject jo = JObject.Parse(response);
            //var userId = jo.SelectToken("id").Value<string>();
            //FormsAuthentication.SetAuthCookie(userId, false);


            //FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1,
            //    userId,
            //    DateTime.Now,
            //    DateTime.Now.AddMinutes(30),
            //    true,
            //    response,
            //    FormsAuthentication.FormsCookiePath);

            //// Encrypt the ticket.
            //string encTicket = FormsAuthentication.Encrypt(ticket);

            //// Create the cookie.
            //HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
        }

        /// <summary>
        /// Decrypt logged-in user's info from cookies
        /// </summary>
        /// <returns></returns>
        public static Office365Model DecryptCookie()
        {
            //var httpCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
            //Office365Model response = new Office365Model();

            //if (httpCookie == null)
            //{
            //    return response;
            //}

            ////Here throws error!
            //var decryptedCookie = FormsAuthentication.Decrypt(httpCookie.Value);

            //if (decryptedCookie == null)
            //{
            //    return response;
            //}

            //response = JsonConvert.DeserializeObject<Office365Model>(decryptedCookie.UserData);

            //return response;
            return new Office365Model();
        }

        /// <summary>
        /// Calling 3rd party web apis. 
        /// </summary>
        /// <param name="destinationUrl"></param>
        /// <param name="methodName"></param>
        /// <param name="requestJSON"></param>
        /// <returns></returns>
        public static ResponseModel<T> MakeWebRequest<T>(string destinationUrl, string methodName, string contentType = "", string requestJSON = "")
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(destinationUrl);
                request.Method = methodName;
                if (methodName == "POST")
                {
                    byte[] bytes = System.Text.Encoding.ASCII.GetBytes(requestJSON);
                    request.ContentType = contentType;
                    request.ContentLength = bytes.Length;
                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                    }
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                        {
                            return new ResponseModel<T> { status = true, Data = JsonConvert.DeserializeObject<T>(reader.ReadToEnd()) };
                        }
                    }
                    else
                    {
                        return new ResponseModel<T> { status = false, message = response.StatusCode.ToString() };
                    }
                }
            }
            catch (WebException webEx)
            {
                if (webEx.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = (HttpWebResponse)webEx.Response;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        return new ResponseModel<T> { status = false, message = reader.ReadToEnd() };
                    }
                }
                else
                {
                    return new ResponseModel<T> { status = false, message = webEx.Message };
                }
            }
        }

        /// <summary>
        /// Generates facebook url to login or sign-up using facebook
        /// </summary>
        /// <returns></returns>
        public static string GetFacebookUrl()
        {
            string url = string.Format(AppSettings.Facebook_url, AppSettings.Facebook_AppId, AppSettings.Facebook_RedirectUrl, AppSettings.Facebook_scope);
            return url;
        }
        public static string GetFacebookUrl(string callbackurl)
        {
            string url = string.Format(AppSettings.Facebook_url, AppSettings.Facebook_AppId, callbackurl, AppSettings.Facebook_scope);
            return url;
        }
        //public static string GetOfficeUrl(string callbackurl)
        //{
        //    string url = $"{ConfigurationManager.AppSettings["Office_url"]}oauth20_authorize.srf?client_id={ConfigurationManager.AppSettings["ClientId"]}&scope={ConfigurationManager.AppSettings["Office_scope"]} offline_access&response_type=code&redirect_uri={callbackurl}";
        //    return url;
        //}
        /// <summary>
        /// Generates url with query string parameters.
        /// </summary>
        /// <param name="methodName">method name of apersonal api.</param>
        /// <param name="queryStringParams">parameters to append in querystring of url.</param>
        /// <returns>string value</returns>
        public static string CombineUrl(string url, IDictionary<string, string> queryStringParams)
        {
            if (!url.EndsWith("?"))
            {
                url = url + "?";
            }
            List<string> parametersList = new List<string>();
            foreach (var parameter in queryStringParams)
            {
                parametersList.Add(parameter.Key + "=" + parameter.Value);
            }
            url = url + string.Join("&", parametersList);

            return url;
        }

    }
}
