
//using System.Web.Http;
//using System.Web.Http.Filters;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TechnicalCore.Models;

namespace TechnicalWeb.Filters
{
    public class JwtAuthentication 
    {
        private IConfiguration _config;
        public JwtAuthentication(IConfiguration config)
        {
            _config = config;
        }
        public string BuildToken(string email,string givenName)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.GivenName,givenName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
              _config["Jwt:Issuer"],
              claims,
              expires: DateTime.Now.AddMinutes(20),
              signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public ResponseModel<Office365Model> GenerateToken(string email)
        {
            var refresh_token = Guid.NewGuid().ToString().Replace("-", "");

            var rToken = new RToken
            {
                Email = email,
                RefreshToken = refresh_token,
                Id = Guid.NewGuid().ToString(),
                IsStop = 0
            };

            //store the refresh_token   
            if (RToken.AddToken(rToken))
            {
                return new ResponseModel<Office365Model>
                {
                    status = true,
                    message = "OK",
                    Data = GetJwt(email, refresh_token)
                };
            }
            else
            {
                return new ResponseModel<Office365Model>
                {
                    status = false,
                    message = "can not add token to database",
                    Data = null
                };
            }
        }
        public ResponseModel<Office365Model> DoRefreshToken(JWModel parameters)
        {
            var token = RToken.GetToken(parameters.RefreshToken, parameters.Email);

            if (token == null)
            {
                return new ResponseModel<Office365Model>
                {
                    status = false,
                    message = "can not refresh token",
                    Data = null
                };
            }

            if (token.IsStop == 1)
            {
                return new ResponseModel<Office365Model>
                {
                    status = false,
                    message = "refresh token has expired",
                    Data = null
                };
            }

            var refresh_token = Guid.NewGuid().ToString().Replace("-", "");

            token.IsStop = 1;
            //expire the old refresh_token and add a new refresh_token  
            var updateFlag = RToken.ExpireToken(token);

            var addFlag = RToken.AddToken(new RToken
            {
                Email = parameters.Email,
                RefreshToken = refresh_token,
                Id = Guid.NewGuid().ToString(),
                IsStop = 0
            });

            if (updateFlag && addFlag)
            {
                return new ResponseModel<Office365Model>
                {
                    status = true,
                    message = "OK",
                    Data = GetJwt(parameters.Email, refresh_token)
                };
            }
            else
            {
                return new ResponseModel<Office365Model>
                {
                    status = false,
                    message = "can not expire token or a new token",
                    Data = null
                };
            }
        }
        private Office365Model GetJwt(string email, string refresh_token)
        {
            var now = DateTime.UtcNow;

            var claims = new Claim[]
            {
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(JwtRegisteredClaimNames.Iat, now.ToUniversalTime().ToString(), ClaimValueTypes.Integer64)
            };

            var keyByteArray = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
            var signingKey = new SymmetricSecurityKey(keyByteArray);

            var jwt = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Issuer"],
                claims: claims,
                notBefore: now,
                expires: now.Add(TimeSpan.FromMinutes(20)),
                signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            var response = new Office365Model
            {
                UserPrincipalName=email,
                Token = encodedJwt,
                ExpiresIn = (int)TimeSpan.FromMinutes(20).TotalMinutes,
                RefreshToken = refresh_token,
            };

            return response;
        }
    }
}