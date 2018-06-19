using System.Collections.Generic;
using TechnicalCore.Models;

namespace TechnicalCore.Interfaces
{
    public interface IAccountManager
    {
        ResponseModel<string> Office365Login(IDictionary<string, string> data);
        ResponseModel<string> Office365SignUp(IDictionary<string, string> data);
        ResponseModel<Office365Model> OfficeLoginApi(IDictionary<string, string> data);
        ResponseModel<Office365Model> Office365SignUpApi(IDictionary<string, string> data);
    }
}
