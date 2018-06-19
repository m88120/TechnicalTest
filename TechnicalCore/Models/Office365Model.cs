using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TechnicalCore.Models
{
    public class JWModel
    {
        public string Email { get; set; }
        public string RefreshToken { get; set; }
    }
    public class Office365Model
    {
        public string First_Name { get { return GivenName; } }
        public string Last_Name { get { return Surname; } }
        public string Email { get { return UserPrincipalName; } }
      
        public string Id { get; set; }
       
        public string UserPrincipalName { get; set; }
        public string GivenName { get; set; }
        public string Surname { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresIn { get; set; }
        // public string JwtToken { get; set; }
    }
    public class AccessTokenResponseModel
    {
        public string Access_Token { get; set; }
        public string Token_Type { get; set; }
        public string Expires_In { get; set; }
        public string Expires_On { get; set; }
        public string Resource { get; set; }
        public string Refresh_Token { get; set; }
        public string Scope { get; set; }
        public string Id_Token { get; set; }

        //Error handeling
        public string Error { get; set; }
        public string Error_Description { get; set; }
    }
    public class RToken
    {    
        public string Email { get; set; }
        public string RefreshToken { get; set; }
        public string Id { get; set; }
        public int IsStop { get; set; }
        public static List<RToken> RefreshTokens { get; set; }
        static RToken()
        {
            if (RefreshTokens == null)
            {
                RefreshTokens = new List<RToken>();
            }
        }

        public static bool AddToken(RToken rToken)
        {
            RefreshTokens.Add(rToken);
            return true;
        }

        public static RToken GetToken(string rToken, string email)
        {
            return RefreshTokens.FirstOrDefault(x => x.RefreshToken == rToken && x.Email == email);
        }

        public static bool ExpireToken(RToken token)
        {
            var tokenInfo = RefreshTokens.FirstOrDefault(x => x.Id == token.Id);
            tokenInfo.IsStop = token.IsStop;
            return true;
        }
    }
}
