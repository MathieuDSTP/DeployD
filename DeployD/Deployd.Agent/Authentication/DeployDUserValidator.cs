using Deployd.Agent.Services.Authentication;
using Nancy.Authentication.Basic;
using Nancy.Security;

namespace Deployd.Agent.Authentication
{
    public class DeployDUserValidator  : IUserValidator
    {
        private readonly IAuthenticationService _authenticationService;

        public DeployDUserValidator(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public IUserIdentity Validate(string username, string password)
        {
            if (_authenticationService.CredentialsAuthenticate(username, password))
            {
                return new DeploydUserIdentity(){UserName=username};
            }
            return null;
        }
    }
}