using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nancy.Authentication.Forms;

namespace Deployd.Agent.Services.Authentication
{
    public interface IAuthenticationService
    {
        bool CredentialsAuthenticate(string username, string password);
        Guid GenerateAuthenticationToken();
    }

    public class AuthenticationService : IAuthenticationService
    {
        public bool CredentialsAuthenticate(string username, string password)
        {
            // todo: hash password

            // todo: check against credentials store

            return true;
        }

        public Guid GenerateAuthenticationToken()
        {
            return Guid.NewGuid();
        }
    }
}
