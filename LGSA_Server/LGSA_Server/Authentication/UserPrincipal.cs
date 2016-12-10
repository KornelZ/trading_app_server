using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace LGSA_Server.Authentication
{
    public class UserPrincipal : IPrincipal
    {
        public int Id { get; set; }
        public string Password { get; set; }
        public UserPrincipal(int id, string password)
        {
            Id = id;
            Password = password;
        }
        public IIdentity Identity { get; set; }

        public bool IsInRole(string role)
        {
            return false;
        }
    }
}