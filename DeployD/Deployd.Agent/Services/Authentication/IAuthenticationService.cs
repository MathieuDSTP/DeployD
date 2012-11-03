using System;
using Deployd.Agent.Authentication;

namespace Deployd.Agent.Services.Authentication
{
    public interface IAuthenticationService
    {
        bool CredentialsAuthenticate(string username, string password, bool preHashed=false);
        Guid GenerateAuthenticationToken(string username);
        DeploydUserIdentity GetUserByAuthenticationToken(Guid guid);
    }
}