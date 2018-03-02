using System;
using System.Collections.Generic;
using System.Text;

namespace Auth.Domain.Authentication
{
    public class DomainScopes
    {
        public const string Roles = "roles";
        public const string MvcClientUser = "mvc_client_user";
        public static string ApiKeys { get; set; }
    }
}
