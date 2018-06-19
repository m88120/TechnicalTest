using System;
using System.Collections.Generic;
using System.Text;

namespace TechnicalCore
{
   public class AppSetting
    {
      public string OneDriveClientId { get; set; }
        public string OneDriveSecret { get; set; }
        public string OneDriveCallbackUri { get; set; }
        //"OneDriveCallbackUri"
        public string SendGrid_AppId { get; set; }
        public string OneDrive { get; set; }
        public string WashingtonSendToEmail { get; set; }
        public string JeffSendToEmail { get; set; }
        public string AzureADInstance { get; set; }
        public string DirectoryId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
        public string GrantType { get; set; }
        public string ResponseType { get; set; }
        public string ResponseMode { get; set; }
        public string LogoutUri { get; set; }
        public string Resource { get; set; }
        public string Facebook_url { get; set; }
        public string Facebook_AppId { get; set; }
        public string Facebook_RedirectUrl { get; set; }
        public string Facebook_scope { get; set; }
        public string RequestOnsite { get; set; }
        public string Hired { get; set; }
        public string Fail { get; set; }
    }
}
