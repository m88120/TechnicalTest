using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TechnicalCore.Models;

namespace TechnicalCore.Utilities
{
    public class ADAuthUtils
    {
        public static AppSetting AppSettings;
        public static string GetLoginUrl()
        {
            IDictionary<string, string> urlParams = new Dictionary<string, string>();

            urlParams["client_id"] = AppSettings.ClientId;
            urlParams["response_type"] = AppSettings.ResponseType;
            urlParams["redirect_uri"] = AppSettings.RedirectUri;
            urlParams["response_mode"] = AppSettings.ResponseMode;
            urlParams["state"] = Guid.NewGuid().ToString();
            string url = string.Format("{0}/{1}/oauth2/authorize", AppSettings.AzureADInstance,
               AppSettings.DirectoryId);
            return AppUtilities.CombineUrl(url, urlParams);
        }

        public static string GetLoginUrl(string callbackUri)
        {
            IDictionary<string, string> urlParams = new Dictionary<string, string>();
            urlParams["client_id"] = AppSettings.ClientId;
            urlParams["response_type"] = AppSettings.ResponseType;
            urlParams["redirect_uri"] = callbackUri;
            urlParams["response_mode"] = AppSettings.ResponseMode;
            urlParams["state"] = Guid.NewGuid().ToString();

            string url = string.Format("{0}/{1}/oauth2/authorize", AppSettings.AzureADInstance,
                 AppSettings.DirectoryId);
            return AppUtilities.CombineUrl(url, urlParams);

        }

        public static ResponseModel<AccessTokenResponseModel> GetAccessToken(string code,string RedirectUri)
        {
            string url = string.Format("{0}/{1}/oauth2/token", AppSettings.AzureADInstance,
              AppSettings.DirectoryId);
            StringBuilder postData = new StringBuilder();
            AppendUrlEncoded(postData, "grant_type", AppSettings.GrantType);
            AppendUrlEncoded(postData, "client_id", AppSettings.ClientId);
            AppendUrlEncoded(postData, "code", code);
            //AppendUrlEncoded(postData, "redirect_uri", AppSettings.RedirectUri);
            AppendUrlEncoded(postData, "redirect_uri", RedirectUri);
            AppendUrlEncoded(postData, "client_secret", AppSettings.ClientSecret);
            AppendUrlEncoded(postData, "resource", AppSettings.Resource);
            var result = AppUtilities.MakeWebRequest<AccessTokenResponseModel>(url, "POST", "application/x-www-form-urlencoded", postData.ToString());
            return result;

        }

        public static IDictionary<string, string> GetClaimData(string idToken)
        {
            var jwtToken = new JwtSecurityToken(idToken);
            IDictionary<string, string> claimsData = new Dictionary<string, string>();
            foreach (var claim in jwtToken.Claims)
            {
                claimsData.Add(claim.Type, claim.Value);
            }
            return claimsData;
        }
        private static string GetAccessToken(HttpContext httpContext)
        {
            var accessToken = (httpContext.Request.Headers as FrameRequestHeaders).HeaderAuthorization;
            var token = accessToken.First().Remove(0, "Bearer ".Length);
            return token;
        }
        public static string GetLoggedUserEmail(HttpContext httpContext)
        {
            var token = GetAccessToken(httpContext);
            var claimData = GetClaimData(token);
            if (claimData.ContainsKey("email"))
            {
                return Convert.ToString(claimData["email"]);
            }
            else
                return "";
        }
        public static string GetLogoutUrl()
        {
            IDictionary<string, string> urlParams = new Dictionary<string, string>();
            urlParams["post_logout_redirect_uri"] = AppSettings.LogoutUri;

            string url = string.Format("{0}/Common/oauth2/logout", AppSettings.AzureADInstance);

            return AppUtilities.CombineUrl(url, urlParams);
        }

        private static void AppendUrlEncoded(StringBuilder sb, string name, string value)
        {
            if (sb.Length != 0)
                sb.Append("&");
            sb.Append(HttpUtility.UrlEncode(name));
            sb.Append("=");
            sb.Append(HttpUtility.UrlEncode(value));
        }
    }
}
