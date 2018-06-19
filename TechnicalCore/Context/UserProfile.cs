using System;
using System.Collections.Generic;

namespace TechnicalCore.Context
{
    public partial class UserProfile
    {
        public long Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
    }
}
