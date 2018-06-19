using System;
using System.Collections.Generic;
using System.Linq;
using TechnicalCore.Context;
using TechnicalCore.Interfaces;
using TechnicalCore.Models;
using TechnicalCore.Utilities;

namespace TechnicalCore.Managers
{
    public class AccountManager : IAccountManager
    {
        DbLeonContext _context;
        public AccountManager(DbLeonContext dbcontext)
        {
            _context = dbcontext;
        }
        public ResponseModel<string> Office365Login(IDictionary<string, string> data)
        {
            string email = string.Empty;
            if (data.ContainsKey("email"))
            {
                email = data["email"];
            }
            else if (data.ContainsKey("upn"))
            {
                email = data["upn"];
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResponseModel<string> { message = "Invalid user! Your office365 account is not registered with us." };
            }
            var userProfile = _context.UserProfile.FirstOrDefault(x => x.Email == email);
            if (userProfile == null)
            {
                return new ResponseModel<string> { message = "Invalid user! Your office365 account is not registered with us." };
            }
            AppUtilities.SetCookiesData(new Office365Model
            {
                GivenName = data.ContainsKey("given_name") == true ? data["given_name"] : data["name"],
                UserPrincipalName = email,
                Surname = data.ContainsKey("family_name") == true ? data["family_name"] : string.Empty
            });
            return new ResponseModel<string> { status = true, message = "Success", Data = "" };
        }
        public ResponseModel<string> Office365SignUp(IDictionary<string, string> data)
        {
            string email = string.Empty;
            if (data.ContainsKey("email"))
            {
                email = data["email"];
            }
            else if (data.ContainsKey("upn"))
            {
                email = data["upn"];
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResponseModel<string> { message = "Invalid email! Cannot register account at the moment." };
            }

            var userProfile = _context.UserProfile.FirstOrDefault(x => x.Email == email);
            if (userProfile == null)
            {
                Random rnd = new Random();
                Int64 i64 = rnd.Next(10000000, 99999999);
                i64 = (i64 * 100000000) + rnd.Next(0, 999999999);
                var result = Math.Abs(i64).ToString();
                UserProfile profile = new UserProfile
                {
                    Id = Convert.ToInt64(result),
                    Email = email,
                    FirstName = data.ContainsKey("given_name") == true ? data["given_name"] : data["name"],
                    LastName = data.ContainsKey("family_name") == true ? data["family_name"] : string.Empty,
                    Gender = "male"
                };
                _context.UserProfile.Add(profile);
                _context.SaveChanges();
                AppUtilities.SetCookiesData(new Office365Model
                {
                    GivenName = data.ContainsKey("given_name") == true ? data["given_name"] : data["name"],
                    UserPrincipalName = data["email"],
                    Surname = data.ContainsKey("family_name") == true ? data["family_name"] : string.Empty
                });
                return new ResponseModel<string> { status = true, message = "Success", Data = "" };
            }
            else
            {
                return new ResponseModel<string> { message = "User already registered." };
            }

           
        }
        public ResponseModel<Office365Model> OfficeLoginApi(IDictionary<string, string> data)
        {
            string email = string.Empty;
            if (data.ContainsKey("email"))
            {
                email = data["email"];
            }
            else if (data.ContainsKey("upn"))
            {
                email = data["upn"];
            }
            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResponseModel<Office365Model> { message = "Invalid user! Your office365 account is not registered with us." };
            }
            var userProfile = _context.UserProfile.FirstOrDefault(x => x.Email == email);
            if (userProfile == null)
            {
                return new ResponseModel<Office365Model> { message = "Invalid user! Your office365 account is not registered with us." };
            }

            var officedetailObj = new Office365Model
            {
                GivenName = data.ContainsKey("given_name") == true ? data["given_name"] : data["name"],
                UserPrincipalName = email,
                Surname = data.ContainsKey("family_name") == true ? data["family_name"] : string.Empty
            };
            return new ResponseModel<Office365Model> { status = true, message = "Success", Data = officedetailObj };
        }
        public ResponseModel<Office365Model> Office365SignUpApi(IDictionary<string, string> data)
        {
            string email = string.Empty;
            if (data.ContainsKey("email"))
            {
                email = data["email"];
            }
            else if (data.ContainsKey("upn"))
            {
                email = data["upn"];
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                return new ResponseModel<Office365Model> { message = "Invalid user! Your office365 account is not registered with us." };
            }

            var userProfile = _context.UserProfile.FirstOrDefault(x => x.Email == email);
            if (userProfile == null)
            {
                Random rnd = new Random();
                Int64 i64 = rnd.Next(10000000, 99999999);
                i64 = (i64 * 100000000) + rnd.Next(0, 999999999);
                var result = Math.Abs(i64).ToString();
                UserProfile profile = new UserProfile
                {
                    Id = Convert.ToInt64(result),
                    Email = email,
                    FirstName = data.ContainsKey("given_name") == true ? data["given_name"] : data["name"],
                    LastName = data.ContainsKey("family_name") == true ? data["family_name"] : string.Empty,
                    Gender = "male"
                };
                _context.UserProfile.Add(profile);
                _context.SaveChanges();
            }
            else
            {
                return new ResponseModel<Office365Model> { message = "Your office365 account is already registered with us." };
            }
            var office365Obj = new Office365Model
            {
                GivenName = data.ContainsKey("given_name") == true ? data["given_name"] : data["name"],
                UserPrincipalName = data["email"],
                Surname = data.ContainsKey("family_name") == true ? data["family_name"] : string.Empty
            };
            return new ResponseModel<Office365Model> { status = true, message = "Success", Data = office365Obj };
        }
    }
}
